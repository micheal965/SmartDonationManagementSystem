using Microsoft.AspNetCore.Identity;

namespace SmartDonationSystem.Core.Auth.Models;

public class ApplicationUser : IdentityUser
{
    public required string FullName { get; set; } = string.Empty;
    public required string IdentityNumber { get; set; }
    public string? PictureUrl { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Address { get; set; }
    //for tracking IP Address for each login
    public List<UserLoginHistory> UserLoginsHistory { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; }

}
