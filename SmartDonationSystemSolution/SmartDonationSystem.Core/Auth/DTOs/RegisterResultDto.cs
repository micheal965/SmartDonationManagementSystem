namespace SmartDonationSystem.Core.DTOs;

public class RegisterResultDto
{
    public string IdentityNumber { get; set; }
    public string FullName { get; set; }
    public string PictureUrl { get; set; }
    public DateTime? BirthDate { get; set; }
}
