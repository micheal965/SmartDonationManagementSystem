import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-auth-footer',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './auth-footer.component.html',
  styleUrl: './auth-footer.component.scss',
})
export class AuthFooterComponent {}
