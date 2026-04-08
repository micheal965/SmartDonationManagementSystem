using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Notifications.DTOs;
using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Services.Modules.SignalR.Hubs;
using SmartDonationSystem.Shared.Helpers;
using SmartDonationSystem.Shared.Pagination;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Modules.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }
        public async Task CreateAsync(CreateNotificationRequest request)
        {
            var notification = new Notification
            {
                ReceiverId = request.ReceiverId,
                ActorId = request.ActorId,

                Title = request.Title,
                Message = request.Message,

                Type = request.Type,

                EntityId = request.EntityId,

                RedirectUrl = NotificationHelper.GetRedirectUrl(request.Type, request.EntityId),

                ActorName = request.ActorName,
                ActorImage = request.ActorImage,
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();

            await _hub.Clients.User(notification.ReceiverId)
                .SendAsync("ReceiveNotification", new NotificationPayload
                {

                    Id = notification.Id,
                    Title = notification.Title,
                    Message = notification.Message,
                    EntityId = notification.EntityId,
                    RedirectUrl = notification.RedirectUrl,
                    ActorName = notification.ActorName,
                    ActorImage = notification.ActorImage,
                    CreatedAt = notification.CreatedAt,
                    IsRead = notification.IsRead
                });
        }

        public async Task<Result<object>> GetUserNotificationsAsync(string userId, int page)
        {
            const int pageSize = 25;

            var query = _context.Notifications.Where(x => x.ReceiverId == userId);

            // 1. total count
            var totalCount = await query.Where(n => n.IsAllMarkedAsRead == false).CountAsync();

            // 2. unread count
            var unreadCount = await query.CountAsync(x => !x.IsAllMarkedAsRead);

            var notifications = await query
                .OrderBy(x => x.IsRead ? 1 : 0)
                .ThenByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new NotificationPayload
                {
                    Id = x.Id,
                    Title = x.Title,
                    Message = x.Message,
                    EntityId = x.EntityId,
                    RedirectUrl = x.RedirectUrl,
                    ActorName = x.ActorName,
                    ActorImage = x.ActorImage,
                    CreatedAt = x.CreatedAt,
                    IsRead = x.IsRead
                })
                .ToListAsync();

            var result = new PaginatedList<NotificationPayload>(notifications, page, pageSize, totalCount);
            return Result<object>.Ok(new
            {
                Result = result,
                UnreadCount = unreadCount
            });
        }

        public async Task MarkAsReadAsync(string userId, int notificationId)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.ReceiverId == userId);

            if (notification == null) return;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
        public async Task MarkAllAsReadAsync(string userId)
        {
            await _context.Notifications.Where(x => x.ReceiverId == userId)
                   .ExecuteUpdateAsync(x => x.SetProperty(n => n.IsAllMarkedAsRead, true));
        }
    }
}
