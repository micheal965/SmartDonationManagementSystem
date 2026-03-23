import { Component } from '@angular/core';
import { AdminAsideComponent } from './admin-aside/admin-aside.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [AdminAsideComponent, RouterOutlet],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss',
})
export class AdminLayoutComponent {}
