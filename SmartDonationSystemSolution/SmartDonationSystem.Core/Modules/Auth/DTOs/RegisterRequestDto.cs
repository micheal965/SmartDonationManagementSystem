using SmartDonationSystem.API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.Modules.Auth.DTOs;

public class RegisterRequestDto
{
    [Required]
    [StringLength(14)]
    public required string IdentityNumber { get; set; }
    [Required]
    public required string FullName { get; set; }
    [Required]
    [DataType(DataType.Date)]
    [BirthDateValidation(ErrorMessage = "Birth date must be in the past")]
    public DateOnly BirthDate { get; set; }

    [Required]
    public required string Password { get; set; }
    [Required]
    public required string Role { get; set; }
    [Required]
    public required string ProfilePictureUrl { get; set; }
    [Required]
    public required string PhoneNumber { get; set; }
    public string? Address { get; set; }
    public List<int> InterestingCategoriesIds { get; set; } = new();
}
