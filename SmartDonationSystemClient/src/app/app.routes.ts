import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest.guard';
import { authGuard } from './core/guards/auth.guard';
import { postDetailsResolver } from './features/post-details/post-details.resolver';
import { profileResolver } from './features/profile/profile.resolver';
import { postCommentsResolver } from './features/post-details/post-comments.resolver';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layouts/auth/auth.component').then(
        (m) => m.AuthLayoutComponent,
      ),
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
        canActivate: [guestGuard],
        loadComponent: () =>
          import('./features/auth/pages/login/login.component').then(
            (m) => m.LoginComponent,
          ),
        title: 'Sign In',
      },
      {
        path: 'signup',
        canActivate: [guestGuard],
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
        path: 'categories',
        title: 'Categories | Help Hand',
        loadComponent: () =>
          import('./features/categories/categories.component').then(
            (m) => m.CategoriesComponent,
          ),
      },
      {
        path: 'posts/:id',
        resolve: { post: postDetailsResolver, comments: postCommentsResolver },
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
    canActivateChild: [adminGuard],
    loadComponent: () =>
      import('./layouts/admin/admin.component').then(
        (m) => m.AdminLayoutComponent,
      ),
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/admin/dashboard/dashboard.component').then(
            (m) => m.DashboardComponent,
          ),
        title: 'Dashboard',
      },
      {
        path: 'notifications',
        loadComponent: () =>
          import('./features/admin/notifications/notifications.component').then(
            (m) => m.NotificationsComponent,
          ),
        title: 'Notifications',
      },
      {
        path: 'users',
        loadComponent: () =>
          import('./features/admin/users/users.component').then(
            (m) => m.UsersComponent,
          ),
        title: 'Users',
      },
      {
        path: 'posts',
        loadComponent: () =>
          import('./features/admin/posts/posts.component').then(
            (m) => m.PostsComponent,
          ),
        title: 'Posts',
      },
      {
        path: 'categories',
        loadComponent: () =>
          import('./features/admin/categories/categories.component').then(
            (m) => m.CategoriesComponent,
          ),
        title: 'Categories',
      },
      {
        path: 'analytics',
        loadComponent: () =>
          import('./features/admin/analytics/analytics.component').then(
            (m) => m.AnalyticsComponent,
          ),
        title: 'Analytics',
      },
      {
        path: '**',
        redirectTo: 'dashboard',
      },
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];
