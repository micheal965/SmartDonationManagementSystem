using System.ComponentModel.DataAnnotations;

namespace SmartDonationSystem.API.Attributes
{
    public class BirthDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateOnly date)
            {
                if (date > DateOnly.FromDateTime(DateTime.Now))
                {
                    return new ValidationResult(ErrorMessage ?? "Birth date cannot be in the future");
                }
            }
            return ValidationResult.Success;
        }
    }
}
