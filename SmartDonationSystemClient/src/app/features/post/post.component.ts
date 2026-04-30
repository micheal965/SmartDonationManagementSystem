import { Component, inject, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { Post } from '../feed/models/post.model';
import { TimeAgoPipe } from '../../shared/pipes/time-ago.pipe';
import { NgClass, NgIf } from '@angular/common';
import { FeedService } from '../feed/services/feed.service';
import { Router, RouterLink } from '@angular/router';
import { PriorityClassPipe } from '../../shared/pipes/priority-class.pipe';
import { PriorityLabelPipe } from '../../shared/pipes/priority-label.pipe';
import { ChatService } from '../../core/services/chat.service';
import { ShortNumberPipe } from '../../shared/pipes/short-number.pipe';
import { AuthService } from '../auth/services/auth.service';
import { PaymentService } from '../../core/services/payment.service';
@Component({
  selector: 'app-post',
  standalone: true,
  imports: [
    MatIconModule,
    TimeAgoPipe,
    NgClass,
    NgIf,
    PriorityClassPipe,
    PriorityLabelPipe,
    RouterLink,
    ShortNumberPipe,
  ],
  templateUrl: './post.component.html',
  styleUrl: './post.component.scss',
})
export class PostComponent {
  private feedService = inject(FeedService);
  private router = inject(Router);
  private paymentService = inject(PaymentService);
  authService = inject(AuthService);
  chatService = inject(ChatService);
  post = input.required<Post>();

  goToDetails() {
    this.router.navigate(['/posts', this.post().id]);
  }

  onLike(post: Post) {
    if (post.isReacting) return;
    post.isReacting = true;

    const previousState = {
      hasReacted: post.hasReacted,
      likesCount: post.likesCount,
    };
    post.hasReacted = !post.hasReacted;
    post.likesCount += post.hasReacted ? 1 : -1;

    this.feedService.reactToPost(post.id).subscribe({
      next: () => (post.isReacting = false),
      error: () => {
        post.hasReacted = previousState.hasReacted;
        post.likesCount = previousState.likesCount;
        post.isReacting = false;
      },
    });
  }

  onDonate() {
    this.router.navigate(['/posts', this.post().id, 'donate']);
  }
}
