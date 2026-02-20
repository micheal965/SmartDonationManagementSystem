using Mapster;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Admin.PostManagement.DTOs;

namespace SmartDonationSystem.Core.Modules.Admin.PostManagement.MapsterConfigurations
{
    public static class PostConfigs
    {
        public static void PostMappings()
        {
            TypeAdapterConfig<Post, PostToReturnDto>.NewConfig()
                .Map(dest => dest.PostAttachments, src => src.PostAttachments.Select(x => x.AttachmentUrl).ToList());
        }
    }
}
