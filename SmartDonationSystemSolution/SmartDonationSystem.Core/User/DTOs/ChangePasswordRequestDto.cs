using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.User.DTOs;

public class ChangePasswordRequestDto
{
    [Required]
    public required string OldPassword { get; set; }
    [Required]
    public required string NewPassword { get; set; }
}
