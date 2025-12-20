
namespace SmartDonationSystem.Core.Auth.DTOs;

public class LoginOrRotateTokenResponseDto
{
    public string Username { get; set; }
    public string Token { get; set; }
    public List<string> Roles { get; set; }
}
