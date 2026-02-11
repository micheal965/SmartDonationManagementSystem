import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest.guard';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'home',
  },
  {
    path: '',
    loadComponent: () =>
      import('./layouts/user/user.component').then(
        (m) => m.UserLayoutComponent,
      ),
    children: [],
  },
  {
    path: '',
    loadComponent: () =>
      import('./layouts/auth/auth.component').then(
        (m) => m.AuthLayoutComponent,
      ),
    canActivateChild: [guestGuard],
    children: [
      { path: '', redirectTo: 'signin', pathMatch: 'full' },
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
    path: 'admin',
    loadComponent: () =>
      import('./layouts/admin/admin.component').then(
        (m) => m.AdminLayoutComponent,
      ),
  },
  {
    path: '**',
    redirectTo: 'home',
    title: 'Not Found Page',
  },
];
