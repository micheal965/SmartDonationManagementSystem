import { HttpClient } from '@angular/common/http';
import { Inject, inject, Injectable, PLATFORM_ID } from '@angular/core';
import { catchError, map, Observable, tap, throwError } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

import { ApiResult } from '../../../shared/models/api-result-model';
import { LoginRequest } from '../models/login-request.model';
import { apiBaseUrl } from '../../../core/utils/app.config';
import { JwtPayloadModel } from '../models/jwt-payload.model';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private accessToken: string | null = null;
  private router = inject(Router);
  private isBrowser: boolean;
  userData: any = null;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) platformId: object,
  ) {
    this.isBrowser = isPlatformBrowser(platformId);

    if (this.isBrowser) {
      const token = localStorage.getItem('token');
      if (token) this.setSession(token);
    }
  }

  getAccessToken() {
    return this.accessToken;
  }
  isAuthenticated(): boolean {
    return !!this.accessToken;
  }

  register(data: any): Observable<ApiResult<any>> {
    return this.http.post<ApiResult<any>>(`${apiBaseUrl}/Auth/register`, data);
  }
  login(data: LoginRequest): Observable<void> {
    return this.http
      .post<ApiResult<{ token: string }>>(`${apiBaseUrl}/Auth/login`, data)
      .pipe(
        map((res) => res.data?.token),
        tap((token) => {
          if (!token) throw new Error('Token missing');
          localStorage.setItem('token', token);
          this.setSession(token);
        }),
        map(() => void 0),
      );
  }

  logout() {
    this.accessToken = null;
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
    return this.http.post(`${apiBaseUrl}/Auth/logout`, {});
  }

  refreshToken() {
    return this.http
      .post<
        ApiResult<{ token: string }>
      >(`${apiBaseUrl}/Auth/rotate-refresh-token`, {})
      .pipe(tap((res) => (this.accessToken = res.data?.token ?? null)));
  }

  private setSession(token: string) {
    this.accessToken = token;
    const decoded = jwtDecode<JwtPayloadModel>(token);
    this.userData = {
      id: decoded[
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
      ],
      name: decoded[
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
      ],
      nationalId: decoded.NationalId,
      role: decoded[
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
      ],
    };
  }
}
