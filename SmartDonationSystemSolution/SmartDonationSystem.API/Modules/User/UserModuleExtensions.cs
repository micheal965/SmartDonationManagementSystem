using SmartDonationSystem.Core.Modules.PostAggregate.Comment.Interfaces;
using SmartDonationSystem.Core.Modules.PostAggregate.Post.Interfaces;
using SmartDonationSystem.Core.Modules.PostAggregate.Reaction.interfaces;
using SmartDonationSystem.Core.Modules.User.Profile.Interfaces;
using SmartDonationSystem.Services.Modules.PostAggregate.Comment;
using SmartDonationSystem.Services.Modules.PostAggregate.Post;
using SmartDonationSystem.Services.Modules.PostAggregate.Reaction;
using SmartDonationSystem.Services.Modules.User.Profile;

namespace SmartDonationSystem.API.Modules.User
{
    public static class UserModuleExtensions
    {
        public static IServiceCollection AddUserModule(this IServiceCollection services)
        {
            services.AddScoped<IUserProfileService, UserProfileServices>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IReactionService, ReactionService>();
            services.AddScoped<ICommentService, CommentService>();
            return services;
        }
    }
}
