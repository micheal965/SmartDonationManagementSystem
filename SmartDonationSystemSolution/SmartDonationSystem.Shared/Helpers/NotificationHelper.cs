using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Shared.Helpers
{
    public static class NotificationHelper
    {
        public static string GetRedirectUrl(NotificationType notificationType, int entityId)
        {
            return notificationType switch
            {
                NotificationType.Like => $"/posts/{entityId}",
                NotificationType.Comment => $"/posts/{entityId}",
                NotificationType.Tag => $"/posts/{entityId}",
                NotificationType.PostCreation => $"/admin/posts/{entityId}",
                NotificationType.PostApproval => $"/posts/{entityId}",
                NotificationType.PostRejection => "/feed",
                NotificationType.UserRegistered => $"/admin/users",
                NotificationType.Message => "/chat",
                _ => "/"
            };
        }
    }
}
