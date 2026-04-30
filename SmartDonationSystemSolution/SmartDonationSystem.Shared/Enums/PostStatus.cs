using System.Runtime.Serialization;

namespace SmartDonationSystem.Shared.Enums
{
    public enum PostStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "Approved")]
        Approved,
        [EnumMember(Value = "Rejected")]
        Rejected,
        [EnumMember(Value = "Freezed")]
        Freezed,
        [EnumMember(Value = "Completed")]
        Completed
    }
}
