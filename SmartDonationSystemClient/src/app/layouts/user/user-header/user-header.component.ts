import { NgFor, NgIf } from '@angular/common';
import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-user-header',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor, MatIconModule, RouterLinkActive],
  templateUrl: './user-header.component.html',
  styleUrl: './user-header.component.scss',
})
export class UserHeaderComponent {
  isMenuOpen = false; // For mobile toggle
  isMeClicked = false; // For the "Me" dropdown

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
}
