import { finalize } from 'rxjs';
import { Component, inject, OnInit, PLATFORM_ID } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/login-request.model';
import { CommonModule, isPlatformBrowser, NgIf } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { passwordStrengthValidator } from '../../../../shared/validators/password.validator';
import { ChatService } from '../../../../core/services/chat.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { MatIcon } from '@angular/material/icon';
import { TimeAgoPipe } from '../../../../shared/pipes/time-ago.pipe';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    NgIf,
    RouterLink,
    MatIcon,
    CommonModule,
    TimeAgoPipe,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  private chatService = inject(ChatService);
  private notificationService = inject(NotificationService);
  private platformId = inject(PLATFORM_ID);

  showPassword: boolean = false;
  isLoading: boolean = false;
  signInData: any = null;

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      this.authService.getSignInData().subscribe({
        next: (data) => {
          this.signInData = data;
        },
      });
    }
  }

  loginForm = this.fb.group({
    identityNumber: ['', [Validators.required, Validators.pattern(/^\d{14}$/)]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  get identityNumber() {
    return this.loginForm.get('identityNumber');
  }
  get password() {
    return this.loginForm.get('password');
  }

  onSubmit() {
    if (this.loginForm.invalid) return;

    this.isLoading = true;

    this.authService
      .login(this.loginForm.value as LoginRequest)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: () => {
          this.chatService.startConnection();
          this.notificationService.startConnection();

          this.toastr.success('Logged in successfully');
          this.router.navigate(['/feed']);
        },
      });
  }
  togglePassword() {
    this.showPassword = !this.showPassword;
  }
}
