using Microsoft.AspNetCore.Identity;

namespace SmartDonationSystem.Core.Common.Models;

public class ApplicationUser : IdentityUser
{
    public required string FullName { get; set; } = string.Empty;
    public required string IdentityNumber { get; set; }
    public string? PictureUrl { get; set; }
    public DateOnly BirthDate { get; set; }
    public string? Address { get; set; }
    public bool IsSoftDeleted { get; set; } = false;

    //for tracking IP Address for each login
    public List<UserLoginHistory>? UserLoginsHistory { get; set; } = new List<UserLoginHistory>();
    public List<RefreshToken>? RefreshTokens { get; set; } = new List<RefreshToken>();
    public List<Post>? Posts { get; set; } = new List<Post>();
    public List<Reaction>? Reactions { get; set; } = new List<Reaction>();
    public List<Comment>? Comments { get; set; } = new List<Comment>();
    public List<CommentTag>? CommentTags { get; set; } = new List<CommentTag>();
    public List<Notification>? Notifications { get; set; } = new List<Notification>();
    public List<UserCategory>? UserCategories { get; set; } = new List<UserCategory>();

}
