using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using SmartDonationSystem.Core.Modules.Payment.Abstractions;
using SmartDonationSystem.Core.Modules.Payment.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SmartDonationSystem.Services.Modules.Payment
{
    public class PaymobGateway : PaymentNotify, IPaymentGateway
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public string Name => "Paymob";

        public PaymobGateway(UserManager<ApplicationUser> userManager, ApplicationDbContext context, INotificationService notificationService, HttpClient http, IConfiguration config)
            : base(userManager, context, notificationService)
        {
            _http = http;
            _config = config;
            _context = context;
        }
        public async Task<Result<string>> CreateCheckoutAsync(Donation donation)
        {
            var authResponse = await _http.PostAsJsonAsync("https://accept.paymob.com/api/auth/tokens",
                                                            new { api_key = _config["Payments:Paymob:ApiKey"] });

            var authData = await authResponse.Content.ReadFromJsonAsync<JsonElement>();
            string token = authData.GetProperty("token").GetString();

            var donor = await _context.Users
                        .Where(u => u.Id == donation.DonorId)
                        .Select(u => new
                        {
                            u.FullName,
                            u.PhoneNumber,
                        }).FirstOrDefaultAsync();

            var names = (donor?.FullName ?? "Anonymous User").Split(' ', 2);

            var firstName = names[0];
            var lastName = names.Length > 1 ? names[1] : "Donor";

            var result = await CreateIntentionAsync(donation, firstName, lastName, donor.PhoneNumber);
            return Result<string>.Ok(result);
        }

        private async Task<string> CreateIntentionAsync(Donation donation, string firstName, string lastName, string phoneNumber)
        {
            var secretKey = _config["Payments:Paymob:SecretKey"];

            Console.WriteLine("Wallet ID: " + _config["Payments:Paymob:WalletIntegrationId"]);
            Console.WriteLine("Card ID: " + _config["Payments:Paymob:CardIntegrationId"]);
            var payload = new
            {
                amount = donation.Amount * 100, // Paymob uses minor units
                currency = "EGP",
                payment_methods = new[]
                {
                //int.Parse(_config["Payments:Paymob:WalletIntegrationId"]),
                int.Parse(_config["Payments:Paymob:CardIntegrationId"])
                },
                items = new[]
                {
                    new {
                        name = "-",
                        amount = donation.Amount * 100,
                        description="Donation to Smart Donation System",
                        quantity = 1
                        }
                },
                billing_data = new
                {
                    apartment = "NA",
                    first_name = firstName,
                    last_name = lastName,
                    street = "NA",
                    building = "NA",
                    phone_number = phoneNumber,
                    city = "Cairo",
                    country = "EG",
                    email = "donor@smartdonation.com", // Mandatory field for API
                    floor = "NA",
                    state = "Cairo",

                }
            };


            var request = new HttpRequestMessage(HttpMethod.Post,
                "https://accept.paymob.com/v1/intention/");

            request.Headers.Authorization =
                    new AuthenticationHeaderValue("Token", secretKey);

            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                    "application/json");

            var response = await _http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Paymob Intention API Error: {response.StatusCode} - {json}");
            }

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string? clientSecret = null;
                if (root.TryGetProperty("client_secret", out var csProp))
                    clientSecret = csProp.GetString();

                // Fallback for different casing if any
                if (string.IsNullOrEmpty(clientSecret) && root.TryGetProperty("ClientSecret", out var csProp2))
                    clientSecret = csProp2.GetString();

                if (string.IsNullOrEmpty(clientSecret))
                {
                    throw new Exception($"Paymob response missing 'client_secret'. Full Response: {json}");
                }

                long id = 0;
                if (root.TryGetProperty("id", out var idProp))
                {
                    if (idProp.ValueKind == JsonValueKind.Number)
                        id = idProp.GetInt64();
                    else if (idProp.ValueKind == JsonValueKind.String && long.TryParse(idProp.GetString(), out var parsedId))
                        id = parsedId;
                }

                var publicKey = _config["Payments:Paymob:PublicKey"];
                var url = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&client_secret={clientSecret}";

                donation.CheckoutUrl = url;
                donation.ExternalOrderId = id.ToString();
                await _context.SaveChangesAsync();

                return url;
            }
            catch (Exception ex) when (ex.Message.Contains("Paymob response missing") == false)
            {
                throw new Exception($"Failed to parse Paymob response: {ex.Message}. Raw JSON: {json}");
            }
        }


        public async Task HandleWebhookAsync(string payload, string signature)
        {
            var json = JsonDocument.Parse(payload);

            if (!json.RootElement.TryGetProperty("obj", out var obj))
                return;

            var receivedHmac = json.RootElement.GetProperty("hmac").GetString();

            if (!IsValidHmac(obj, receivedHmac))
                return;

            var success = GetBool(obj, "success");
            var isVoided = GetBool(obj, "is_voided");
            var isRefunded = GetBool(obj, "is_refunded");

            var orderId = GetInt(obj.GetProperty("order"), "id");
            var transactionId = GetInt(obj, "id");

            var donation = await _context.Donations
                .FirstOrDefaultAsync(d => d.ExternalOrderId == orderId.ToString());

            if (donation == null)
                return;

            if (donation.Status == DonationStatus.Paid.ToString())
                return;

            if (success && !isVoided && !isRefunded)
            {
                donation.Status = DonationStatus.Paid.ToString();
                donation.ExternalTransactionId = transactionId.ToString();

                await _context.SaveChangesAsync();

                await NotifyDonationPaidAsync(donation.Id);
            }
            else
            {
                donation.Status = DonationStatus.Failed.ToString();
                await _context.SaveChangesAsync();
            }
        }


        //Helpers
        private bool IsValidHmac(JsonElement obj, string receivedHmac)
        {
            if (string.IsNullOrEmpty(receivedHmac))
                return false;

            var secret = _config["Paymob:HmacSecret"];

            // ⚠️ Order MUST match Paymob docs EXACTLY
            var concatenated = string.Concat(
                GetInt(obj, "amount_cents"),
                GetString(obj, "created_at"),
                GetString(obj, "currency"),
                GetBool(obj, "error_occured"),
                GetBool(obj, "has_parent_transaction"),
                GetInt(obj, "id"),
                GetInt(obj, "integration_id"),
                GetBool(obj, "is_3d_secure"),
                GetBool(obj, "is_auth"),
                GetBool(obj, "is_capture"),
                GetBool(obj, "is_refunded"),
                GetBool(obj, "is_standalone_payment"),
                GetBool(obj, "is_voided"),
                GetInt(obj.GetProperty("order"), "id"),
                GetInt(obj, "owner"),
                GetBool(obj, "pending"),
                GetNestedString(obj, "source_data", "pan"),
                GetNestedString(obj, "source_data", "sub_type"),
                GetNestedString(obj, "source_data", "type"),
                GetBool(obj, "success")
            );

            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenated));

            var computed = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return computed == receivedHmac.ToLower();
        }

        private static string GetString(JsonElement obj, string property)
        {
            return obj.TryGetProperty(property, out var val) && val.ValueKind != JsonValueKind.Null
                ? val.ToString()
                : "";
        }

        private static string GetNestedString(JsonElement obj, string parent, string child)
        {
            if (!obj.TryGetProperty(parent, out var parentObj))
                return "";

            return parentObj.TryGetProperty(child, out var val) && val.ValueKind != JsonValueKind.Null
                ? val.ToString()
                : "";
        }

        private static int GetInt(JsonElement obj, string property)
        {
            return obj.TryGetProperty(property, out var val) && val.TryGetInt32(out var result)
                ? result
                : 0;
        }

        private static bool GetBool(JsonElement obj, string property)
        {
            return obj.TryGetProperty(property, out var val) && val.ValueKind == JsonValueKind.True;
        }
    }
}

public class PaymobIntentionResponse
{
    public int id { get; set; }
    public string ClientSecret { get; set; }
    public string status { get; set; }
}