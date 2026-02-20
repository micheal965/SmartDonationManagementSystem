using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDonationSystem.Core.Common.Models;

public class UserLoginHistory : BaseEntity
{
    public required string IpAddress { get; set; }
    public DateTime LoginTime { get; set; }

    [ForeignKey("ApplicationUser")]
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}
