import { inject, Injectable } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';
import { HttpClient } from '@angular/common/http';
import { ApiResult } from '../../shared/models/api-result-model';
import { apiBaseUrl } from '../utils/app.config';

@Injectable({
  providedIn: 'root',
})
export class AnalyticsService {
  private httpClient = inject(HttpClient);
  trackEntrance() {
    if (typeof window === 'undefined') return;

    // Only track once per session
    if (sessionStorage.getItem('entranceTracked')) return;

    this.httpClient.post(`${apiBaseUrl}/track-page`, {}).subscribe({
      next: () => sessionStorage.setItem('entranceTracked', 'true'),
    });
  }
}
