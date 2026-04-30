import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Inject, inject, Injectable, PLATFORM_ID } from '@angular/core';
import { catchError, map, Observable, switchMap, tap, throwError } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

import { ApiResult } from '../../../shared/models/api-result-model';
import { LoginRequest } from '../models/login-request.model';
import { apiBaseUrl } from '../../../core/utils/app.config';
import { JwtPayloadModel } from '../models/jwt-payload.model';
import { Router } from '@angular/router';
import { isPlatformBrowser } from '@angular/common';
import { userDataModel } from '../models/user-data.model';

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

  isAuthenticated(): boolean {
    return !!this.accessToken;
  }
  isAdmin(): boolean {
    return this.userData.role === 'Admin';
  }
  isRequester(): boolean {
    return this.userData.role === 'Requester';
  }
  isDonor(): boolean {
    return this.userData.role === 'Donor';
  }
  register(data: any): Observable<ApiResult<any>> {
    return this.http.post<ApiResult<any>>(`${apiBaseUrl}/Auth/register`, data);
  }
  login(data: LoginRequest): Observable<void> {
    return this.http
      .post<
        ApiResult<{ token: string }>
      >(`${apiBaseUrl}/Auth/login`, data, { withCredentials: true })
      .pipe(
        tap((res) => this.setTokens(res.data.token)),
        map(() => void 0),
      );
  }

  logout() {
    return this.http.post(`${apiBaseUrl}/Auth/logout`, {}).pipe(
      tap(() => {
        this.accessToken = null;
        localStorage.removeItem('token');
        this.router.navigate(['/signin']);
      }),
    );
  }

  rotateRefreshToken() {
    return this.http
      .post<
        ApiResult<{ token: string }>
      >(`${apiBaseUrl}/Auth/rotate-refresh-token`, {})
      .pipe(
        tap((res) => this.setTokens(res.data.token)),
        catchError((error: HttpErrorResponse) => {
          if (error.status === 400)
            return this.logout().pipe(switchMap(() => throwError(() => error)));

          return throwError(() => error);
        }),
      );
  }

  getSignInData(): Observable<any> {
    return this.http.get<ApiResult<any>>(`${apiBaseUrl}/Auth/sign-in-data`)
      .pipe(map(res => res.data));
  }
  private setTokens(token: string) {
    localStorage.setItem('token', token);
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
