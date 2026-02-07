import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';

import { ApiResult } from '../../../shared/models/api-result-model';
import { LoginResponse } from '../models/login-response-model';
import { LoginRequest } from '../models/login-request.model';
import { apiBaseUrl } from '../../../core/utils/app.config';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private accessToken: string | null = null;
  private loggedIn$ = new BehaviorSubject<boolean>(false);

  constructor(private http: HttpClient) {}

  getAccessToken() {
    return this.accessToken;
  }

  isLoggedIn$() {
    return this.loggedIn$.asObservable();
  }

  login(data: LoginRequest): Observable<ApiResult<LoginResponse>> {
    return this.http
      .post<ApiResult<LoginResponse>>(`${apiBaseUrl}/Auth/login`, data)
      .pipe(
        tap((res) => {
          this.accessToken = res.data?.token ?? null;
          this.loggedIn$.next(true);
        }),
      );
  }

  logout() {
    this.accessToken = null;
    this.loggedIn$.next(false);
    return this.http.post(`${apiBaseUrl}/Auth/logout`, {});
  }

  refreshToken() {
    return this.http
      .post<
        ApiResult<{ token: string }>
      >(`${apiBaseUrl}/Auth/rotate-refresh-token`, {})
      .pipe(tap((res) => (this.accessToken = res.data?.token ?? null)));
  }
}
