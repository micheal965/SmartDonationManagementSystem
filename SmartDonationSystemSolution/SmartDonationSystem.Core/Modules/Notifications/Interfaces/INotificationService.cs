using SmartDonationSystem.Core.Modules.Notifications.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Modules.Notifications.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(CreateNotificationRequest request);
        Task<Result<object>> GetUserNotificationsAsync(string userId, int page, int pageSize);
        Task MarkAsReadAsync(string userId, int notificationId);
        Task MarkAllAsReadAsync(string userId);
    }
}
