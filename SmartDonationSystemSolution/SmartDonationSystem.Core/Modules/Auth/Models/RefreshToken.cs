using SmartDonationSystem.Core.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDonationSystem.Core.Modules.Auth.Models;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; }
    public DateTime expiryDate { get; set; }
    public DateTime createdOn { get; set; } = DateTime.UtcNow;
    public DateTime? revokedOn { get; set; }
    public bool isExpired => expiryDate <= DateTime.UtcNow;
    public bool isActive => revokedOn == null && !isExpired;

    //mapping 1 user : many refreshtoken

    [ForeignKey("ApplicationUser")]
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}
