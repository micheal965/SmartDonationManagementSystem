import { Component, inject, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { Post } from '../feed/models/post.model';
import { TimeAgoPipe } from '../../shared/pipes/time-ago.pipe';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { NgClass, NgIf } from '@angular/common';
import { FeedService } from '../feed/services/feed.service';
import { Router, RouterLink } from '@angular/router';
import { PriorityClassPipe } from '../../shared/pipes/priority-class.pipe';
import { PriorityLabelPipe } from '../../shared/pipes/priority-label.pipe';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [
    MatIconModule,
    TimeAgoPipe,
    NgClass,
    NgIf,
    PriorityClassPipe,
    PriorityLabelPipe,
    RouterLink
],
  templateUrl: './card.component.html',
  styleUrl: './card.component.scss',
})
export class CardComponent {
  private feedService = inject(FeedService);
  private sanitizer = inject(DomSanitizer);
  private router = inject(Router);
  post = input.required<Post>();

  getSafeUrl(url: string): SafeResourceUrl {
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }

  goToDetails() {
    console.log(this.post().id);
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
}
