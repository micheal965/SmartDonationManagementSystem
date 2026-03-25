import { Component, inject } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';
import { DashboardService } from '../../services/dashboard.service';
import { DashboardModel } from '../../models/dashboard.model';
import { VisitsChartComponent } from './visits-chart/visits-chart.component';
import { ShortNumberPipe } from "../../../../shared/pipes/short-number.pipe";

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MatIcon, RouterLink, VisitsChartComponent, ShortNumberPipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  dashboard?: DashboardModel;
  analytics: any;
  private authService = inject(AuthService);
  private dashboardService = inject(DashboardService);

  get adminName(): string {
    return this.authService.userData.name.split(' ')[0];
  }
  ngOnInit() {
    this.dashboardService.getDashboard().subscribe((result) => {
      this.dashboard = result;
    });
  }
}
