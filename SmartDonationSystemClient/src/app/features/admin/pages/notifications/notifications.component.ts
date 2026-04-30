import { Component, inject, OnInit } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { NotificationService } from '../../../../core/services/notification.service';
import { NgFor, NgIf, NgClass, DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { NotificationPayload } from '../../../../shared/models/notification-payload-model';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [MatIcon, NgFor, NgIf, NgClass, DatePipe],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.scss',
})
export class NotificationsComponent implements OnInit {
  private router = inject(Router);
  notificationService = inject(NotificationService);
  page = 1;
  pageSize = 5;
  notificationStyle: any = {
    PostCreation: {
      border: 'border-sky-500',
      dot: 'bg-sky-500',
    },
    UserRegistered: {
      border: 'border-indigo-600',
      dot: 'bg-indigo-600',
    },
    Comment: {
      border: 'border-orange-500',
      dot: 'bg-orange-500',
    },
    AdminDonationReceived: {
      border: 'border-emerald-600',
      dot: 'bg-emerald-600',
    },
  };
  ngOnInit(): void {
    this.notificationService.loadNotifications(this.page, this.pageSize);
  }

  goToDetails(notification: NotificationPayload) {
    this.notificationService.markAsRead(notification.id);
    this.router.navigateByUrl(notification.redirectUrl);
  }

  markAllAsRead() {
    if (this.notificationService.unreadCount() > 0)
      this.notificationService.markAllAsRead();
  }
  LoadMoreNotifications() {
    this.page++;
    this.notificationService.loadNotifications(this.page, this.pageSize);
  }
}
