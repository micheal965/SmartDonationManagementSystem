import {
  HttpClient,
  HttpErrorResponse,
  HttpParams,
} from '@angular/common/http';
import { Inject, inject, Injectable, PLATFORM_ID } from '@angular/core';
import {
  catchError,
  finalize,
  map,
  Observable,
  switchMap,
  tap,
  throwError,
} from 'rxjs';
import { jwtDecode } from 'jwt-decode';

import { ApiResult } from '../../../shared/models/api-result-model';
import { LoginRequest } from '../models/login-request.model';
import { apiBaseUrl } from '../../../core/utils/app.config';
import { JwtPayloadModel } from '../models/jwt-payload.model';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { userDataModel } from '../models/user-data.model';
import { error } from 'console';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private accessToken: string | null = null;
  private router = inject(Router);
  private isBrowser: boolean;
  userData!: userDataModel;

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) platformId: object,
  ) {
    this.isBrowser = isPlatformBrowser(platformId);

    if (this.isBrowser) {
      const token = localStorage.getItem('token');
      if (token) {
        this.accessToken = token;
        this.decodeToken();
      }
    }
  }

  getAccessToken() {
    return this.accessToken;
  }
  getRefreshToken() {
    return sessionStorage.getItem('refreshToken');
  }
  isAuthenticated(): boolean {
    return !!this.accessToken;
  }

  register(data: any): Observable<ApiResult<any>> {
    return this.http.post<ApiResult<any>>(`${apiBaseUrl}/Auth/register`, data);
  }
  login(data: LoginRequest): Observable<void> {
    return this.http
      .post<
        ApiResult<{ token: string; refreshToken: string }>
      >(`${apiBaseUrl}/Auth/login`, data)
      .pipe(
        tap((res) => this.setTokens(res.data.token, res.data.refreshToken)),
        map(() => void 0),
      );
  }

  logout() {
    return this.http.post(`${apiBaseUrl}/Auth/logout`, {}).pipe(
      tap(() => {
        this.accessToken = null;
        localStorage.removeItem('token');
        sessionStorage.removeItem('refreshToken');
        this.router.navigate(['/signin']);
      }),
    );
  }

  rotateRefreshToken() {
    return this.http
      .post<
        ApiResult<{ token: string; refreshToken: string }>
      >(`${apiBaseUrl}/Auth/rotate-refresh-token`, { refreshToken: this.getRefreshToken() }, { withCredentials: true })
      .pipe(
        tap((res) => this.setTokens(res.data.token, res.data.refreshToken)),
        catchError((error: HttpErrorResponse) => {
          if (error.status === 400)
            return this.logout().pipe(switchMap(() => throwError(() => error)));

          return throwError(() => error);
        }),
      );
  }
  private setTokens(token: string, refreshToken: string) {
    localStorage.setItem('token', token);
    sessionStorage.setItem('refreshToken', refreshToken);
    this.accessToken = token;
    this.decodeToken();
  }
  private decodeToken() {
    if (this.accessToken == null) return;

    const decoded = jwtDecode<JwtPayloadModel>(this.accessToken);
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
