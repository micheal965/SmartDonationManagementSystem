import { Component, inject } from '@angular/core';
import { UserHeaderComponent } from './user-header/user-header.component';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { UserLeftSidebarComponent } from './user-left-sidebar/user-left-sidebar.component';
import { UserRightSidebarComponent } from './user-right-sidebar/user-right-sidebar.component';
import { MessagingComponent } from '../../features/messaging/messaging.component';
import { CommonModule } from '@angular/common';
import { filter, map, startWith } from 'rxjs';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [
    CommonModule,
    UserHeaderComponent,
    RouterOutlet,
    UserLeftSidebarComponent,
    UserRightSidebarComponent,
    MessagingComponent,
  ],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss',
})
export class UserLayoutComponent {
  private router = inject(Router);

  isMessagingPage$ = this.router.events.pipe(
    filter((event) => event instanceof NavigationEnd),
    map(() => this.checkIfMessagingPage()),
    startWith(this.checkIfMessagingPage()),
  );

  private checkIfMessagingPage(): boolean {
    return this.router.url.includes('/messaging');
  }
}
