import { ApiResult } from './../../shared/models/api-result-model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';
import { apiBaseUrl } from '../utils/app.config';
import { UserProfile } from '../../shared/models/user-profile.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  profile = signal<UserProfile | null>(null);

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  loadProfile(): void {
    if (this.profile()) return;

    const params = new HttpParams().set('UserId', this.authService.userData.id);

    this.http
      .get<
        ApiResult<UserProfile>
      >(`${apiBaseUrl}/userProfile/get-user-data`, { params })
      .subscribe({
        next: (res) => this.profile.set(res.data),
        error: () => this.profile.set(null),
      });
  }
}
