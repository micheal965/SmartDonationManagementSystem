import { UserService } from './../../../core/services/user.service';
import { NgFor, NgIf, AsyncPipe, DatePipe, NgClass } from '@angular/common';
import {
  Component,
  effect,
  HostListener,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../features/auth/services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { NotificationService } from '../../../core/services/notification.service';
import { NotificationPayload } from '../../../shared/models/notification-payload-model';

@Component({
  selector: 'app-user-header',
  standalone: true,
  imports: [
    RouterLink,
    NgIf,
    NgFor,
    NgClass,
    DatePipe,
    MatIconModule,
    MatMenuModule,
    MatBadgeModule,
    RouterLinkActive,
  ],
  templateUrl: './user-header.component.html',
  styleUrl: './user-header.component.scss',
})
export class UserHeaderComponent implements OnInit {
  private authService = inject(AuthService);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  public notificationService = inject(NotificationService);
  userService = inject(UserService);
  isMenuOpen = false; // For mobile toggle
  isMeClicked = false; // For the "Me" dropdown
  isNotificationsListOpen = false; // For the notifications dropdown

  ngOnInit(): void {
    this.userService.loadProfile();
    this.notificationService.loadNotifications();
  }

  navItems = [
    {
      label: 'Home',
      link: '/feed',
      iconName: 'home',
    },
    {
      label: 'Messaging',
      link: '/messaging',
      iconName: 'message',
    },
    {
      label: 'Notifications',
      link: '/notifications',
      iconName: 'notifications',
    },
    {
      label: 'My Donations',
      link: '/donations',
      iconName: 'favorite',
    },
    {
      label: 'Categories',
      link: '/categories',
      iconName: 'grid_view',
    },
  ];
  toggleMe() {
    this.isMeClicked = !this.isMeClicked;
  }
  toggleNotifications() {
    if (
      !this.isNotificationsListOpen &&
      this.notificationService.unreadCount() > 0
    )
      this.notificationService.markAllAsRead();

    this.isNotificationsListOpen = !this.isNotificationsListOpen;
  }
  getIcon(type: string): string {
    switch (type) {
      case 'Like':
        return 'favorite';

      case 'Comment':
        return 'comment';

      default:
        return 'notifications';
    }
  }
  goToDetails(notification: NotificationPayload) {
    this.notificationService.markAsRead(notification.id);
    this.toggleNotifications();
    this.router.navigateByUrl(notification.redirectUrl);
  }
  onLogout(): void {
    this.authService.logout().subscribe({
      next: () => this.toastr.success('Logged out successfully'),
    });
  }
}
