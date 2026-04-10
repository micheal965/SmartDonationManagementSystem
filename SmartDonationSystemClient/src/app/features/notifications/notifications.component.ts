import { Component, inject, OnInit } from '@angular/core';
import { NotificationService } from '../../core/services/notification.service';
import { NotificationPayload } from '../../shared/models/notification-payload-model';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { InfiniteScrollDirective } from '../../shared/directives/infinite-scroll.directive';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, InfiniteScrollDirective],
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.scss',
})
export class NotificationsComponent implements OnInit {
  private router = inject(Router);
  notificationService = inject(NotificationService);
  page = 1;
  pageSize = 5;

  ngOnInit(): void {
    this.notificationService.loadNotifications(this.page, this.pageSize);
  }
  goToDetails(notification: NotificationPayload) {
    this.notificationService.markAsRead(notification.id);
    this.router.navigateByUrl(notification.redirectUrl);
  }
  loadMoreNotifications() {
    this.page++;
    this.notificationService.loadNotifications(this.page, this.pageSize);
  }
  markAllAsRead() {
    if (this.notificationService.unreadCount() > 0)
      this.notificationService.markAllAsRead();
  }
}
