import { Component } from '@angular/core';
import { FooterComponent } from '../../shared/components/footer/footer.component';
import { UserHeaderComponent } from './user-header/user-header.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [FooterComponent, UserHeaderComponent, RouterOutlet],
  templateUrl: './user.component.html',
  styleUrl: './user.component.scss',
})
export class UserLayoutComponent {}
