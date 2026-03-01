import { Component, Input } from '@angular/core';
import { userReaction } from '../models/user-reactions.model';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-profile-like',
  standalone: true,
  imports: [MatIconModule, RouterLink],
  templateUrl: './profile-like.component.html',
  styleUrl: './profile-like.component.scss',
})
export class ProfileLikeComponent {
  @Input() like!: userReaction;
}
