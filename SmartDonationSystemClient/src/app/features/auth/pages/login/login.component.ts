import { finalize } from 'rxjs';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/login-request.model';
import { NgIf } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';
import { Router, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { passwordStrengthValidator } from '../../../../shared/validators/password.validator';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, NgIf, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private toastr = inject(ToastrService);
  private router = inject(Router);
  showPassword: boolean = false;
  isLoading: boolean = false;

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
          this.toastr.success('Logged in successfully');
          //navigate
          this.router.navigate(['/home']);
        },
      });
  }
  togglePassword() {
    this.showPassword = !this.showPassword;
  }
}
