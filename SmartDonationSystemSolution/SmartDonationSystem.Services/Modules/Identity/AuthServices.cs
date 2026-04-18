using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Auth.DTOs;
using SmartDonationSystem.Core.Modules.Auth.Interfaces;
using SmartDonationSystem.Core.Modules.Notifications.DTOs;
using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartDonationSystem.Services.Modules.Identity;

public class AuthServices : IAuthService
{
    private static readonly HashSet<string> BlacklistedTokens = new();
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly INotificationService _notificationService;

    public AuthServices(IConfiguration configuration,
                        IHttpContextAccessor httpContextAccessor,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<IdentityRole> roleManager,
                        SignInManager<ApplicationUser> signInManager,
                        ApplicationDbContext applicationDbContext,
                        INotificationService notificationService)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _applicationDbContext = applicationDbContext;
        _notificationService = notificationService;
    }
    public async Task<Result<LoginOrRotateTokenResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        ApplicationUser? user = await _userManager.Users
                            .Include(u => u.RefreshTokens)
                            .FirstOrDefaultAsync(u => u.IdentityNumber.Equals(loginRequestDto.IdentityNumber));
        if (user == null || user.IsSoftDeleted)
            return Result<LoginOrRotateTokenResponseDto>.BadRequest("Invalid login attempt!");

        SignInResult checkPasswordResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDto.Password, false);
        if (!checkPasswordResult.Succeeded)
            return Result<LoginOrRotateTokenResponseDto>.BadRequest("Invalid login attempt!");

        //Track IPAddress in UserLoginHistory table
        await SaveLoginAttemptAsync(loginRequestDto.IdentityNumber);

        //Check for RefreshToken
        var RefreshTokenObj = new RefreshToken();
        if (user.RefreshTokens.Any(t => t.isActive))
            RefreshTokenObj = user.RefreshTokens.FirstOrDefault(t => t.isActive);
        else
        {
            //if there is no active RefreshToken for that user so generate new one 
            RefreshTokenObj = GenerateRefreshTokenObject();
            RefreshTokenObj.ApplicationUserId = user.Id;
            await _applicationDbContext.RefreshTokens.AddAsync(RefreshTokenObj);
            await _applicationDbContext.SaveChangesAsync();
        }

        //set refresh token if not empty in the cookies 
        if (!string.IsNullOrEmpty(RefreshTokenObj?.Token))
            AppendRefreshTokenInCookies(RefreshTokenObj.Token, RefreshTokenObj.expiryDate);

        return Result<LoginOrRotateTokenResponseDto>.Ok(new LoginOrRotateTokenResponseDto()
        {
            Token = await CreateJwtWebTokenAsync(user),
        });
    }
    public async Task<Result<RegisterResultDto>> RegisterAsync(RegisterRequestDto requestDto)
    {
        bool existingUser = await _applicationDbContext.ApplicationUsers
                                        .AnyAsync(u => u.IdentityNumber == requestDto.IdentityNumber.Trim());
        if (existingUser) return Result<RegisterResultDto>.BadRequest("A user with this Identity Number already exists.");

        ApplicationUser applicationUser = new ApplicationUser()
        {
            IdentityNumber = requestDto.IdentityNumber,
            FullName = requestDto.FullName,
            UserName = requestDto.IdentityNumber,
            BirthDate = requestDto.BirthDate,
            PhoneNumber = requestDto.PhoneNumber,
            Address = requestDto.Address,
            PictureUrl = requestDto.ProfilePictureUrl,
        };

        //Check if the role exists
        if (!await _roleManager.RoleExistsAsync(requestDto.Role) ||
            requestDto.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase)) return Result<RegisterResultDto>.BadRequest("Invalid role");


        IdentityResult createResult = await _userManager.CreateAsync(applicationUser, requestDto.Password);
        if (!createResult.Succeeded)
            return Result<RegisterResultDto>.BadRequest("Registration failed", createResult.Errors.Select(e => e.Description));

        IdentityResult roleResult = await _userManager.AddToRoleAsync(applicationUser, requestDto.Role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(applicationUser);
            return Result<RegisterResultDto>.BadRequest("Registration failed", roleResult.Errors.Select(e => e.Description));
        }

        //Saving Interesting Categories for registring user for notifications
        if (requestDto.InterestingCategoriesIds != null && requestDto.InterestingCategoriesIds.Any() && requestDto.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            var categoryIds = requestDto.InterestingCategoriesIds
                .Distinct()
                .ToList();

            var validCategoryIds = await _applicationDbContext.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();

            if (validCategoryIds.Count != categoryIds.Count)
                return Result<RegisterResultDto>.BadRequest("Invalid category selection");

            var userCategories = validCategoryIds.Select(categoryId => new UserCategory
            {
                UserId = applicationUser.Id,
                CategoryId = categoryId
            });

            await _applicationDbContext.UserCategories.AddRangeAsync(userCategories);
            await _applicationDbContext.SaveChangesAsync();
        }

        //Notify Admins about new user registration
        await NotifyAdminsNewUserRegistered(applicationUser);

        return Result<RegisterResultDto>.Created(applicationUser.Adapt<RegisterResultDto>());
    }
    public async Task<Result<LoginOrRotateTokenResponseDto>> RotateRefreshTokenAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Result<LoginOrRotateTokenResponseDto>.BadRequest("Token is required");

        var user = await _userManager.Users.Include(u => u.RefreshTokens)
                                           .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
        if (user == null)
            return Result<LoginOrRotateTokenResponseDto>.BadRequest("Invalid Token");

        var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);
        if (!refreshToken.isActive)
            return Result<LoginOrRotateTokenResponseDto>.BadRequest("Invalid Token");


        return Result<LoginOrRotateTokenResponseDto>.Ok(new LoginOrRotateTokenResponseDto
        {
            Token = await CreateJwtWebTokenAsync(user),
        }, "Token Rotated successfully!");
    }

    //Logout Services
    public async Task AddTokenBlacklistAsync(string token)
    {
        await Task.Delay(100);  // Simulate async I/O operation to add token to blacklist
        BlacklistedTokens.Add(token);
        var refreshTokenFromCookies = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
        DeleteRefreshTokenFromCookies();

        //Revoke RefreshToken in the Database
        var refreshTokenObj = await _applicationDbContext.RefreshTokens
                                    .FirstOrDefaultAsync(rf => rf.Token == refreshTokenFromCookies);

        if (refreshTokenObj == null || refreshTokenObj.isExpired) return;

        refreshTokenObj.revokedOn = DateTime.UtcNow;
        _applicationDbContext.RefreshTokens.Update(refreshTokenObj);
        await _applicationDbContext.SaveChangesAsync();
    }
    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        await Task.Delay(100); // Simulate a delay
        return BlacklistedTokens.Contains(token);
    }

    //Track IPAddress
    public async Task SaveLoginAttemptAsync(string IdentityNumber)
    {
        var user = await _applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.IdentityNumber.Equals(IdentityNumber));
        if (user != null)
        {
            var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

            // Check if the app is behind a proxy (e.g., Nginx, Cloudflare)
            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                //X-Forwarded-For: 203.0.113.45, 70.41.3.18, 150.172.238.178
                ipAddress = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
            }

            ipAddress = ipAddress == "::1" ? "127.0.0.1" : ipAddress; // Convert ::1 to 127.0.0.1 if local
            await _applicationDbContext.UserLoginsHistory.AddAsync(new UserLoginHistory()
            {
                ApplicationUserId = user.Id,
                IpAddress = ipAddress ?? "",
                LoginTime = DateTime.UtcNow,
            });
            await _applicationDbContext.SaveChangesAsync();
        }
    }

    //Token Aggregate
    private async Task<string> CreateJwtWebTokenAsync(ApplicationUser user)
    {
        //Authentication Claims
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id),
            new Claim(ClaimTypes.Name,user.FullName),
            new Claim("NationalId",user.IdentityNumber),
        };

        //RoleClaims 
        var userRoles = await _userManager.GetRolesAsync(user);
        if (userRoles != null)
            foreach (var role in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryMinutes"])),
            signingCredentials: cred
            );

        //write token and return
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private RefreshToken GenerateRefreshTokenObject()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return new RefreshToken()
        {
            Token = Convert.ToBase64String(randomBytes),
            createdOn = DateTime.UtcNow,
            expiryDate = DateTime.UtcNow.AddDays(7),
        };
    }
    private void AppendRefreshTokenInCookies(string token, DateTime expires)
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", token, GetRefreshTokenCookieOptions(expires));
    }
    private void DeleteRefreshTokenFromCookies()
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshToken", GetRefreshTokenCookieOptions());
    }
    private CookieOptions GetRefreshTokenCookieOptions(DateTime? expires = null)
    {
        return new CookieOptions
        {
            Expires = expires,
            HttpOnly = true,
            Secure = true,
            IsEssential = true,
            SameSite = SameSiteMode.None,
            Path = "/"
        };
    }

    private async Task NotifyAdminsNewUserRegistered(ApplicationUser newUser)
    {
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        if (!admins.Any()) return;

        var notificationsRequests = admins.Select(admin => new CreateNotificationRequest
        {
            ReceiverId = admin.Id,
            ActorId = newUser.Id,
            Title = "New User Registered",
            Message = $"{newUser.FullName} has just joined the platform.",
            Type = NotificationType.UserRegistered,
            ActorName = newUser.FullName,
            ActorImage = newUser.PictureUrl,
        }).ToList();

        foreach (var notificationRequest in notificationsRequests)
            await _notificationService.CreateAsync(notificationRequest);
    }
}
