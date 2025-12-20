using System.ComponentModel.DataAnnotations;
using SmartDonationSystem.Shared.Enums;
using Microsoft.AspNetCore.Http;

namespace SmartDonationSystem.Core.DTOs;

public class RegisterRequestDto
{
    [Required]
    public required RegisterInputType InputType { get; set; }

    // manual inputs
    public string? IdentityNumber { get; set; }
    public string? UserName { get; set; }
    public DateTime? BirthDate { get; set; }

    // image input
    [Required]
    public required IFormFile IdentityCard { get; set; }

    //General Data
    [Required]
    public required string Password { get; set; }
    [Required]
    public required UserRole Role { get; set; }
    [Required]
    public required IFormFile Picture { get; set; }
    [Required]
    public required string PhoneNumber { get; set; }
    public string? Address { get; set; }
}
