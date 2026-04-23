import { Component, inject } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { RouterLinkActive, RouterLink } from '@angular/router';
import { AuthService } from '../../../features/auth/services/auth.service';
import { NgIf } from '@angular/common';
import { UserProfile } from '../../../shared/models/user-profile.model';
import { UserService } from '../../../core/services/user.service';
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
  private userService = inject(UserService);
  authService = inject(AuthService);

  user!: UserProfile | null;

  ngOnInit(): void {
    const userId = this.authService.userData.id;

    this.userService.getUser(userId).subscribe({
      next: (user) => (this.user = user),
    });
  }
}
