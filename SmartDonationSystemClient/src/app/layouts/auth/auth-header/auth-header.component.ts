import { AuthService } from './../../../features/auth/services/auth.service';
import { NgClass, NgIf } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink, RouterLinkActive } from '@angular/router';
import { MatIcon } from "@angular/material/icon";

@Component({
  selector: 'app-auth-header',
  standalone: true,
  imports: [RouterLink, NgIf, NgClass, MatIcon],
  templateUrl: './auth-header.component.html',
  styleUrl: './auth-header.component.scss',
})
export class AuthHeaderComponent implements OnInit {
  currentFragment: string | null = '';
  isMenuOpen = false;
  private activatedRoute = inject(ActivatedRoute);
  authService = inject(AuthService);
  ngOnInit(): void {
    this.activatedRoute.fragment.subscribe((frag) => {
      this.currentFragment = frag;
    });
  }
}
