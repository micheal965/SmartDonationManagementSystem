namespace SmartDonationSystem.Core.Auth.DTOs;

public class UserLoginsHistoryResponseDto
{
    public required string IpAddress { get; set; }
    public DateTime LoginTime { get; set; }
}
