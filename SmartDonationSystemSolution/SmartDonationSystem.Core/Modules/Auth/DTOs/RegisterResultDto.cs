namespace SmartDonationSystem.Core.Modules.Auth.DTOs;

public class RegisterResultDto
{
    public string IdentityNumber { get; set; }
    public string FullName { get; set; }
    public string PictureUrl { get; set; }
    public DateOnly? BirthDate { get; set; }
}
