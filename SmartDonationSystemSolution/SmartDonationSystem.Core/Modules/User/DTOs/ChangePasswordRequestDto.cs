using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.Modules.User.DTOs;

public class ChangePasswordRequestDto
{
    [Required]
    public required string OldPassword { get; set; }
    [Required]
    public required string NewPassword { get; set; }
}
