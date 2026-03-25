import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';
import { finalize } from 'rxjs';
import { apiBaseUrl } from '../utils/app.config';

let activeRequests = 0;
export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  if (
    (req.url.includes(apiBaseUrl) && req.url.includes('signature')) ||
    (req.url.includes(apiBaseUrl) && req.url.includes('react')) ||
    (req.url.includes(apiBaseUrl) && req.url.includes('get-posts')) ||
    (req.url.includes(apiBaseUrl) && req.url.includes('search-user')) ||
    (req.url.includes(apiBaseUrl) && req.url.includes('track-page')) ||
    !req.url.includes(apiBaseUrl)
  )
    return next(req);

  const ngxSpinnerService = inject(NgxSpinnerService);
  activeRequests++;
  ngxSpinnerService.show();

  return next(req).pipe(
    finalize(() => {
      activeRequests--;
      if (activeRequests === 0) ngxSpinnerService.hide();
    }),
  );
};
