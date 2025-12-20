using System;
using System.ComponentModel.DataAnnotations.Schema;
using SmartDonationSystem.Core.Common;

namespace SmartDonationSystem.Core.Auth.Models;

public class UserLoginHistory : BaseEntity
{
    public required string IpAddress { get; set; }
    public DateTime LoginTime { get; set; }

    [ForeignKey("ApplicationUser")]
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}
