import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest.guard';
import { authGuard } from './core/guards/auth.guard';
import { postDetailsResolver } from './features/post-details/post-details.resolver';
import { profileResolver } from './features/profile/profile.resolver';

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
        title: 'Feed | Help Hand',
        loadComponent: () =>
          import('./features/feed/feed.component').then((m) => m.FeedComponent),
      },
      {
        path: 'posts/:id',
        resolve: { post: postDetailsResolver },
        loadComponent: () =>
          import('./features/post-details/post-details.component').then(
            (m) => m.PostDetailsComponent,
          ),
      },
      {
        path: 'profile/:id',
        resolve: { user: profileResolver },
        loadComponent: () =>
          import('./features/profile/profile.component').then(
            (m) => m.ProfileComponent,
          ),
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
