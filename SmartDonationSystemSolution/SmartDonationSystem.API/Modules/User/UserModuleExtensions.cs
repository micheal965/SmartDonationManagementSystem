using SmartDonationSystem.Core.Modules.User.PostAggregate.Post.Interfaces;
using SmartDonationSystem.Core.Modules.User.Profile.Interfaces;
using SmartDonationSystem.Services.Modules.User.PostAggregate.Post;
using SmartDonationSystem.Services.Modules.User.Profile;

namespace SmartDonationSystem.API.Modules.User
{
    public static class UserModuleExtensions
    {
        public static IServiceCollection AddUserModule(this IServiceCollection services)
        {
            services.AddScoped<IUserProfileService, UserProfileServices>();
            services.AddScoped<IPostService, PostService>();
            return services;
        }
    }
}
