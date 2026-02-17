using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.Modules.Auth.DTOs;

public class LoginRequestDto
{
    [Required]
    public required string IdentityNumber { get; set; }
    [Required]
    public required string Password { get; set; }
}
