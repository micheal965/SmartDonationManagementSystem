using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Auth.Interfaces;
using SmartDonationSystem.Core.Auth.Models;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Identity;

public class UserServices : IUserServices
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserServices(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _applicationDbContext = applicationDbContext;
        _httpContextAccessor = httpContextAccessor;
    }
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
    public async Task<Result<IReadOnlyList<UserLoginHistory>>> GetLoginHistoryAsync(string userId)
        => Result<IReadOnlyList<UserLoginHistory>>.Ok(await _applicationDbContext.UserLoginsHistory
            .Where(lg => lg.ApplicationUserId == userId)
            .OrderByDescending(l => l.LoginTime).ToListAsync());
}
