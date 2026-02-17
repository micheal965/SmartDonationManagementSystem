import { Component } from '@angular/core';
import { UserHeaderComponent } from './user-header/user-header.component';
import { RouterOutlet } from '@angular/router';
import { UserFooterComponent } from './user-footer/user-footer.component';
import { UserIdentityCardComponent } from './user-identity-card/user-identity-card.component';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [
    UserHeaderComponent,
    RouterOutlet,
    UserFooterComponent,
    UserIdentityCardComponent,
  ],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss',
})
export class UserLayoutComponent {}
