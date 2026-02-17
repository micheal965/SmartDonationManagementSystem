using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.Modules.Auth.DTOs;

public class RegisterRequestDto
{
    // manual inputs
    [Required]
    [StringLength(14)]
    public required string IdentityNumber { get; set; }
    [Required]
    public required string FullName { get; set; }
    [Required]
    public DateOnly BirthDate { get; set; }

    //General Data
    [Required]
    public required string Password { get; set; }
    [Required]
    public required string Role { get; set; }
    [Required]
    public required string ProfilePictureUrl { get; set; }
    [Required]
    public required string PhoneNumber { get; set; }
    public string? Address { get; set; }
}
