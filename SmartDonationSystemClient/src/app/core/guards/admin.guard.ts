import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../features/auth/services/auth.service';

export const adminGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  //if not authenticated will redirect to signin page
  if (!authService.isAuthenticated()) return router.createUrlTree(['/signin']);

  if (authService.isAdmin()) return true;

  // if user not admin and authenticated will be redirect to feed
  return router.createUrlTree(['/feed']);
};
