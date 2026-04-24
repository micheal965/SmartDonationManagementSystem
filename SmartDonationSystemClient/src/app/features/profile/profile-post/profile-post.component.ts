import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { userPost } from '../models/user-posts.model';

@Component({
  selector: 'app-profile-post',
  standalone: true,
  imports: [MatIconModule, RouterLink],
  templateUrl: './profile-post.component.html',
  styleUrl: './profile-post.component.scss',
})
export class ProfilePostComponent {
  @Input() post!: userPost;
}
