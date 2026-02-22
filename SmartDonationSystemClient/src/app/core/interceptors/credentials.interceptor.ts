import { HttpInterceptorFn } from '@angular/common/http';
import { apiBaseUrl } from '../utils/app.config';

export const credentialsInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.includes(apiBaseUrl)) return next(req);

  const clonedReq = req.clone({ withCredentials: true });
  return next(clonedReq);
};
