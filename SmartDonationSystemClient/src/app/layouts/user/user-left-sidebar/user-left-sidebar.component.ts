import { Component } from '@angular/core';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { RouterLinkActive } from "@angular/router";
@Component({
  selector: 'app-user-left-sidebar',
  standalone: true,
  imports: [MatListModule, MatIconModule, MatButtonModule, MatDividerModule, RouterLinkActive],
  templateUrl: './user-left-sidebar.component.html',
  styleUrl: './user-left-sidebar.component.scss',
})
export class UserLeftSidebarComponent {}
