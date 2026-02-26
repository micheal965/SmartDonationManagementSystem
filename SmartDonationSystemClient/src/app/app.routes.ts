import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest.guard';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layouts/auth/auth.component').then(
        (m) => m.AuthLayoutComponent,
      ),
    canActivateChild: [guestGuard],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/home/pages/landing-page/landing-page.component').then(
            (m) => m.LandingPageComponent,
          ),
        title: 'Home',
      },
      {
        path: 'signin',
        loadComponent: () =>
          import('./features/auth/pages/login/login.component').then(
            (m) => m.LoginComponent,
          ),
        title: 'Sign In',
      },
      {
        path: 'signup',
        loadComponent: () =>
          import('./features/auth/pages/register/register.component').then(
            (m) => m.RegisterComponent,
          ),
        title: 'Sign Up',
      },
    ],
  },
  {
    path: '',
    loadComponent: () =>
      import('./layouts/user/user.component').then(
        (m) => m.UserLayoutComponent,
      ),
    canActivateChild: [authGuard],
    children: [
      {
        path: 'feed',
        loadComponent: () =>
          import('./features/feed/feed.component').then((m) => m.FeedComponent),
        title: 'Feed',
      },
      { path: '', redirectTo: 'feed', pathMatch: 'full' },
    ],
  },
  {
    path: 'admin',
    loadComponent: () =>
      import('./layouts/admin/admin.component').then(
        (m) => m.AdminLayoutComponent,
      ),
  },
  {
    path: '**',
    redirectTo: '',
  },
];
