namespace SmartDonationSystem.Core.Modules.Auth.DTOs;

public class LoginOrRotateTokenResponseDto
{
    public string Token { get; set; }
    public string refreshToken { get; set; }
}
