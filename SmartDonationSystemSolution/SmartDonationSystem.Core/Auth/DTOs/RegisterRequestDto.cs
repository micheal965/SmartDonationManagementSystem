using System.ComponentModel.DataAnnotations;
using SmartDonationSystem.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace SmartDonationSystem.Core.DTOs;

public class RegisterRequestDto
{
    // manual inputs
    [Required]
    [StringLength(14)]
    public required string IdentityNumber { get; set; }
    [Required]
    public required string FullName { get; set; }
    [Required]
    public required DateTime BirthDate { get; set; }

    // image input
    public required IFormFile IdentityCard { get; set; }

    //General Data
    [Required]
    public required string Password { get; set; }
    [Required]
    public required string Role { get; set; }
    [Required]
    public required IFormFile ProfilePicture { get; set; }
    [Required]
    public required string PhoneNumber { get; set; }
    public string? Address { get; set; }
}
