import { Component, inject } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { RouterLinkActive, RouterLink } from '@angular/router';
import { AuthService } from '../../../features/auth/services/auth.service';
import { NgIf } from '@angular/common';
@Component({
  selector: 'app-user-left-sidebar',
  standalone: true,
  imports: [
    MatListModule,
    MatIconModule,
    MatButtonModule,
    MatDividerModule,
    RouterLinkActive,
    RouterLink,
    NgIf,
  ],
  templateUrl: './user-left-sidebar.component.html',
  styleUrl: './user-left-sidebar.component.scss',
})
export class UserLeftSidebarComponent {
  authService = inject(AuthService);
}
