namespace SmartDonationSystem.Shared.Enums;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Donor = "Donor";
    public const string Requester = "Requester";

    public static readonly string[] All =
    {
        Admin,
        Donor,
        Requester
    };
}
