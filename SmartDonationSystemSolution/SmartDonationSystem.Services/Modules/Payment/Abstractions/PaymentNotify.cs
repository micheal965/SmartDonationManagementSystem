using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.Core.Common.Models;
using SmartDonationSystem.Core.Modules.Notifications.DTOs;
using SmartDonationSystem.Core.Modules.Notifications.Interfaces;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;

namespace SmartDonationSystem.Core.Modules.Payment.Abstractions
{
    public abstract class PaymentNotify
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public PaymentNotify(UserManager<ApplicationUser> userManager, ApplicationDbContext context, INotificationService notificationService)
        {
            _userManager = userManager;
            _context = context;
            _notificationService = notificationService;
        }

        public virtual async Task NotifyDonationPaidAsync(int donationId)
        {
            var donation = await _context.Donations.FindAsync(donationId);

            if (donation == null)
                return;

            await NotifyAdmins(donation);
            await NotifyDonor(donation);

            if (donation.Type == DonationType.Post.ToString())
                await NotifyPostCreator(donation);
        }

        protected virtual async Task NotifyAdmins(Donation donation)
        {
            var admins = await _userManager.GetUsersInRoleAsync(AppRoles.Admin);
            var donor = await _context.Users
                        .Where(u => u.Id == donation.DonorId)
                        .Select(u => new { u.FullName, u.PictureUrl })
                        .FirstOrDefaultAsync();

            foreach (var admin in admins)
            {
                await _notificationService.CreateAsync(new CreateNotificationRequest
                {
                    ReceiverId = admin.Id,
                    Title = "Donation Ready for Payout",
                    Message = $"A new donation of {donation.Amount} EGP has been successfully processed.",
                    Type = NotificationType.AdminDonationReceived,
                    EntityId = (int)donation.PostId,
                    ActorName = donor?.FullName ?? "Anonymous",
                    ActorImage = donor?.PictureUrl,
                });
            }
        }

        protected virtual async Task NotifyPostCreator(Donation donation)
        {
            var post = await _context.Posts.FindAsync(donation.PostId);

            if (post == null)
                return;
            var donor = await _context.Users
                                    .Where(u => u.Id == donation.DonorId)
                                    .Select(u => new { u.FullName, u.PictureUrl })
                                    .FirstOrDefaultAsync();

            await _notificationService.CreateAsync(new CreateNotificationRequest
            {
                ReceiverId = post.ApplicationUserId,
                Title = "New Donation",
                Message = "Your post received a donation. The amount will be transferred to you within 7 days.",
                Type = NotificationType.DonationReceived,
                EntityId = (int)donation.PostId,
                ActorName = donor?.FullName ?? "Anonymous",
                ActorImage = donor?.PictureUrl,
            });
        }
        protected virtual Task NotifyDonor(Donation donation)
        {
            return _notificationService.CreateAsync(new CreateNotificationRequest
            {
                ReceiverId = donation.DonorId,
                Title = "Donation Successful",
                Message = "Your donation was completed successfully. Thank you for your support!",
                Type = NotificationType.DonationReceived,
                EntityId = (int)donation.PostId,
            });
        }
    }
}
