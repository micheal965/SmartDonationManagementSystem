import { Component } from '@angular/core';
import { AuthHeaderComponent } from './auth-header/auth-header.component';
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from '../../shared/components/footer/footer.component';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [
    AuthHeaderComponent,
    FooterComponent,
    RouterOutlet,
    FooterComponent,
  ],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss',
})
export class AuthLayoutComponent {}
