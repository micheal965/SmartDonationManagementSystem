import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest.guard';
import { authGuard } from './core/guards/auth.guard';
import { postDetailsResolver } from './features/post-details/post-details.resolver';
import { profileResolver } from './features/profile/profile.resolver';
import { postCommentsResolver } from './features/post-details/post-comments.resolver';
import { adminGuard } from './core/guards/admin.guard';
import { userDetailsResolver } from './features/admin/pages/users/user-details.resolver';
import { postDetailsResolver as adminPostDetailsResolver } from './features/admin/pages/posts/post-details.resolver';

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
      {
        path: 'notifications',
        loadComponent: () =>
          import('./features/notifications/notifications.component').then(
            (m) => m.NotificationsComponent,
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
          import('./features/admin/pages/dashboard/dashboard.component').then(
            (m) => m.DashboardComponent,
          ),
        title: 'Dashboard',
      },
      {
        path: 'notifications',
        loadComponent: () =>
          import('./features/admin/pages/notifications/notifications.component').then(
            (m) => m.NotificationsComponent,
          ),
        title: 'Notifications',
      },
      {
        path: 'users',
        loadComponent: () =>
          import('./features/admin/pages/users/users.component').then(
            (m) => m.UsersComponent,
          ),
        title: 'Users',
      },
      {
        path: 'users/:id',
        resolve: { user: userDetailsResolver },
        loadComponent: () =>
          import('./features/admin/pages/users/user-details/user-details.component').then(
            (m) => m.UserDetailsComponent,
          ),
      },
      {
        path: 'edit/:id',
        resolve: { user: userDetailsResolver },
        loadComponent: () =>
          import('./features/admin/pages/users/edit-user/edit-user.component').then(
            (m) => m.EditUserComponent,
          ),
      },
      {
        path: 'posts',
        loadComponent: () =>
          import('./features/admin/pages/posts/posts.component').then(
            (m) => m.PostsComponent,
          ),
        title: 'Posts',
      },
      {
        path: 'posts/:id',
        resolve: { post: adminPostDetailsResolver },
        loadComponent: () =>
          import('./features/admin/pages/posts/post-details/post-details.component').then(
            (m) => m.PostDetailsComponent,
          ),
        title: 'Post Details',
      },
      {
        path: 'categories',
        loadComponent: () =>
          import('./features/admin/pages/categories/categories.component').then(
            (m) => m.CategoriesComponent,
          ),
        title: 'Categories',
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
