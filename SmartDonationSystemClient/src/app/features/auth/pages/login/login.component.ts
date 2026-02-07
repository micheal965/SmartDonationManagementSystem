import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/login-request.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  loginForm = this.fb.group({
    identityNumber: ['', [Validators.required, Validators.pattern(/^\d{14}$/)]],
    password: ['', Validators.required],
  });

  get identityNumber() {
    return this.loginForm.get('identityNumber');
  }
  get password() {
    return this.loginForm.get('password');
  }

  onSubmit() {
    if (this.loginForm.invalid) return;
    console.log(this.loginForm.value);

    this.authService.login(this.loginForm.value as LoginRequest).subscribe({
      next: (res) => {
        console.log('logged successfully ', res);
      },
      error: (err) => {
        console.error(err);
      },
    });
  }
}
