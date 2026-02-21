import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';
import { ApiResult } from '../../shared/models/api-result-model';

export const toastrInterceptor: HttpInterceptorFn = (req, next) => {
  const toastr = inject(ToastrService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const body = error.error as ApiResult<any>;

      if (body) {
        if (!body.success && body.errors) {
          if (Array.isArray(body.errors)) {
            body.errors.forEach((err: string) => toastr.error(err));
          } else if (typeof body.errors === 'object') {
            Object.values(body.errors).forEach((fieldErrors: any) => {
              if (Array.isArray(fieldErrors)) {
                fieldErrors.forEach((err: string) => toastr.error(err));
              }
            });
          }
        } else if (body.message) {
          toastr.error(body.message);
        } else {
          toastr.error('Something went wrong');
        }
      }

      return throwError(() => error);
    }),
  );
};
