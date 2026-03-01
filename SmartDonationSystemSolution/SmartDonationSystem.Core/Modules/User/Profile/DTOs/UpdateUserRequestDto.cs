using SmartDonationSystem.API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.Core.Modules.User.Profile.DTOs
{
    public class UpdateUserRequestDto
    {
        [Required]
        public string phoneNumber { get; set; }
        [Required]
        public string address { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [BirthDateValidation(ErrorMessage = "Birth date must be in the past")]
        public DateOnly? birthDate { get; set; }
    }
}
