using System.Runtime.Serialization;

namespace SmartDonationSystem.Shared.Enums
{
    public enum PostStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "Approved")]
        Approved,
        [EnumMember(Value = "Freezed")]
        Freezed
    }
}
