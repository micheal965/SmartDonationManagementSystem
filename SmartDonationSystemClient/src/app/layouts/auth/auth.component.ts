import { Component } from '@angular/core';
import { AuthHeaderComponent } from './auth-header/auth-header.component';
import { RouterOutlet } from '@angular/router';
import { AuthFooterComponent } from './auth-footer/auth-footer.component';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [AuthHeaderComponent, RouterOutlet, AuthFooterComponent],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss',
})
export class AuthLayoutComponent {}
