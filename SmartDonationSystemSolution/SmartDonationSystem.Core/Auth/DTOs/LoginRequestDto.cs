using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.Auth.DTOs;

public class LoginRequestDto
{
    [Required]
    public required string IdentityNumber { get; set; }
    [Required]
    public required string Password { get; set; }
}
