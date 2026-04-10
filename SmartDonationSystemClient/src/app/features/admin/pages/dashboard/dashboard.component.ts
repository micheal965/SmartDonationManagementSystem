import { Component, inject } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';
import { DashboardService } from '../../services/dashboard.service';
import { DashboardModel } from '../../models/dashboard.model';
import { VisitsChartComponent } from './visits-chart/visits-chart.component';
import { ShortNumberPipe } from '../../../../shared/pipes/short-number.pipe';
import { NotificationService } from '../../../../core/services/notification.service';
import { DatePipe, NgClass, NgFor } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    MatIcon,
    RouterLink,
    VisitsChartComponent,
    ShortNumberPipe,
    NgFor,
    DatePipe,
    NgClass,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  dashboard?: DashboardModel;
  analytics: any;
  private authService = inject(AuthService);
  private dashboardService = inject(DashboardService);
  notificationService = inject(NotificationService);
  notificationUI: any = {
    UserRegistered: {
      icon: 'person_add',
      bg: 'bg-indigo-50',
      iconColor: 'text-indigo-600',
    },
    PostApproval: {
      icon: 'article',
      bg: 'bg-emerald-50',
      iconColor: 'text-emerald-500',
    },
    Like: {
      icon: 'favorite',
      bg: 'bg-pink-50',
      iconColor: 'text-pink-500',
    },
    Comment: {
      icon: 'chat_bubble',
      bg: 'bg-slate-50',
      iconColor: 'text-slate-500',
    },
  };
  get adminName(): string {
    return this.authService.userData.name.split(' ')[0];
  }
  ngOnInit() {
    this.dashboardService.getDashboard().subscribe((result) => {
      this.dashboard = result;
    });
    this.notificationService.loadNotifications(1, 5);
  }
}
