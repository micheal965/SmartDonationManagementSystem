import { NgFor, NgIf } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-header',
  standalone: true,
  imports: [RouterLink, NgIf, NgFor],
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
      iconPath: 'M10 20v-6h4v6h5v-8h3L12 3 2 12h3v8z',
    },
    {
      label: 'Messaging',
      link: '/messaging',
      iconPath:
        'M18 4H6C3.8 4 2 5.8 2 8v8c0 2.2 1.8 4 4 4h1.2l3.5 3.5c.2.2.5.3.8.3.1 0 .3 0 .4-.1.3-.1.5-.4.5-.7V20h6c2.2 0 4-1.8 4-4V8c0-2.2-1.8-4-4-4z',
    },
    {
      label: 'Notifications',
      link: '/notifications',
      iconPath:
        'M12 22c1.1 0 2-.9 2-2h-4c0 1.1.89 2 2 2zm6-6v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2z',
    },
  ];
  toggleMe() {
    this.isMeClicked = !this.isMeClicked;
  }
}
