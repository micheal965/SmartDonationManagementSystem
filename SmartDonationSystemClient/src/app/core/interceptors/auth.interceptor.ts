import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';
import {
  BehaviorSubject,
  catchError,
  filter,
  switchMap,
  take,
  throwError,
} from 'rxjs';
import { apiBaseUrl } from '../utils/app.config';

let isRefreshing = false;
let refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.includes(apiBaseUrl) || req.url.includes('rotate-refresh-token'))
    return next(req);

  const authService = inject(AuthService);
  const token = authService.getAccessToken();

  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401) return throwError(() => error);

      if (!isRefreshing) {
        isRefreshing = true;
        refreshTokenSubject.next(null);

        return authService.rotateRefreshToken().pipe(
          switchMap(() => {
            isRefreshing = false;
            const newToken = authService.getAccessToken();
            refreshTokenSubject.next(newToken);

            return next(
              req.clone({
                setHeaders: { Authorization: `Bearer ${newToken}` },
              }),
            );
          }),
          catchError((err) => {
            isRefreshing = false;
            authService.logout();
            return throwError(() => err);
          }),
        );
      }

      return refreshTokenSubject.pipe(
        filter((token) => token != null),
        take(1),
        switchMap((token) =>
          next(
            req.clone({
              setHeaders: { Authorization: `Bearer ${token}` },
            }),
          ),
        ),
      );
    }),
  );
};
