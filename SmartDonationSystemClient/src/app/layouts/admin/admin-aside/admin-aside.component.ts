import { Component, inject, OnInit } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { UserProfile } from '../../../shared/models/user-profile.model';
import { AuthService } from '../../../features/auth/services/auth.service';
import { UserService } from '../../../core/services/user.service';
import { firstValueFrom } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin-aside',
  standalone: true,
  imports: [MatIconModule, RouterLink, RouterLinkActive],
  templateUrl: './admin-aside.component.html',
  styleUrl: './admin-aside.component.scss',
})
export class AdminAsideComponent implements OnInit {
  private authService = inject(AuthService);
  private userService = inject(UserService);
  private toastr = inject(ToastrService);
  user!: UserProfile | null;

  ngOnInit(): void {
    const userId = this.authService.userData.id;

    this.userService.getUser(userId).subscribe({
      next: (user) => (this.user = user),
    });
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.toastr.success('Logged out successfully');
      },
    });
  }
}
