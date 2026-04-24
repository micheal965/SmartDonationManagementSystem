import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { UserCommentDto } from '../models/user-comments.model';
import { TimeAgoPipe } from '../../../shared/pipes/time-ago.pipe';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-profile-comment',
  standalone: true,
  imports: [MatIconModule, RouterLink, TimeAgoPipe, DatePipe],
  templateUrl: './profile-comment.component.html',
  styleUrl: './profile-comment.component.scss',
})
export class ProfileCommentComponent {
  @Input() comment!: UserCommentDto;
}
