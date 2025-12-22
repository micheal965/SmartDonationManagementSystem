using System.Runtime.Serialization;

namespace SmartDonationSystem.Shared.Enums;

public enum UserRole
{
    [EnumMember(Value = "Donor")]
    Donor,
    [EnumMember(Value = "Requester")]
    Requester,
    [EnumMember(Value = "Admin")]
    Admin
}
