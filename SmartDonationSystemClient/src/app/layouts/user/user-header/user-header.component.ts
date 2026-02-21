import { UserService } from './../../../core/services/user.service';
import { NgFor, NgIf } from '@angular/common';
import { Component, effect, inject, OnInit, signal } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../features/auth/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-user-header',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, MatIconModule, RouterLinkActive],
  templateUrl: './user-header.component.html',
  styleUrl: './user-header.component.scss',
})
export class UserHeaderComponent implements OnInit {
  private authService = inject(AuthService);
  private toastr = inject(ToastrService);
  userService = inject(UserService);
  isMenuOpen = false; // For mobile toggle
  isMeClicked = false; // For the "Me" dropdown

  ngOnInit(): void {
    this.userService.loadProfile();
  }

  navItems = [
    {
      label: 'Home',
      link: '/feed',
      iconName: 'home',
    },
    {
      label: 'Messaging',
      link: '/messaging',
      iconName: 'message',
    },
    {
      label: 'Notifications',
      link: '/notifications',
      iconName: 'notifications',
    },
    {
      label: 'My Donations',
      link: '/donations',
      iconName: 'favorite',
    },
    {
      label: 'Categories',
      link: '/categories',
      iconName: 'grid_view',
    },
  ];
  toggleMe() {
    this.isMeClicked = !this.isMeClicked;
  }

  onLogout(): void {
    this.authService.logout().subscribe({
      next: () => this.toastr.success('Logged out successfully'),
    });
  }
}
