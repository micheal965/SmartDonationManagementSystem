using Mapster;

namespace SmartDonationSystem.Core.Modules.Auth.MapsterConfigurations;
public static class RegisterConfigs
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig.GlobalSettings
            .NewConfig<DateOnly, DateTime>()
            .MapWith(d => d.ToDateTime(TimeOnly.MinValue));

        TypeAdapterConfig.GlobalSettings
            .NewConfig<DateOnly?, DateTime?>()
            .MapWith(d => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : null);
    }
}
