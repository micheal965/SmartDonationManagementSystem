import { AuthService } from './../../../features/auth/services/auth.service';
import { NgClass, NgIf } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink, RouterLinkActive } from '@angular/router';
import { MatIcon } from '@angular/material/icon';
import { NotificationService } from '../../../core/services/notification.service';
import { ChatService } from '../../../core/services/chat.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-auth-header',
  standalone: true,
  imports: [RouterLink, NgIf, NgClass, MatIcon],
  templateUrl: './auth-header.component.html',
  styleUrl: './auth-header.component.scss',
})
export class AuthHeaderComponent implements OnInit {
  currentFragment: string | null = '';
  isMenuOpen = false;
  private activatedRoute = inject(ActivatedRoute);
  private chatService = inject(ChatService);
  private notificationService = inject(NotificationService);
  private toastr = inject(ToastrService);

  authService = inject(AuthService);
  ngOnInit(): void {
    this.activatedRoute.fragment.subscribe((frag) => {
      this.currentFragment = frag;
    });
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => {
        this.notificationService.stopConnection();
        this.notificationService.state.set(null);

        this.chatService.stopConnection();
        this.chatService.resetChatState();

        this.toastr.success('Logged out successfully');
      },
    });
  }
}
