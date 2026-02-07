import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../../features/auth/services/auth.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.getAccessToken();

  let authReq = req;
  if (token) {
    authReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`),
    });
  }

  return next(authReq).pipe(
    catchError((err) => {
      // 401 => means Access token expired
      if (err instanceof HttpErrorResponse && err.status === 401) {
        // rotate refresh token
        return authService.refreshToken().pipe(
          switchMap(() => {
            const newToken = authService.getAccessToken();
            const newReq = req.clone({
              headers: req.headers.set('Authorization', `Bearer ${newToken}`),
            });
            return next(newReq);
          }),
        );
      }
      return throwError(() => err);
    }),
  );
};
