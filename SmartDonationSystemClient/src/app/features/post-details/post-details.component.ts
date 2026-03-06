import { PriorityClassPipe } from './../../shared/pipes/priority-class.pipe';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { MatIconModule } from '@angular/material/icon';
import { TimeAgoPipe } from '../../shared/pipes/time-ago.pipe';
import { NgIf, NgClass, NgFor, CommonModule } from '@angular/common';
import { UserService } from '../../core/services/user.service';
import { PriorityLabelPipe } from '../../shared/pipes/priority-label.pipe';
import { Post } from '../feed/models/post.model';
import { FormsModule } from '@angular/forms';
import { FeedService } from '../feed/services/feed.service';
import { Comment } from '../feed/models/post-comments.model';
import { CreateCommentDto } from '../feed/models/create-comment.model';
import { CommentItemComponent } from '../comment-item/comment-item.component';

@Component({
  selector: 'app-post-details',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    TimeAgoPipe,
    PriorityLabelPipe,
    PriorityClassPipe,
    TimeAgoPipe,
    FormsModule,
    CommentItemComponent,
  ],
  templateUrl: './post-details.component.html',
  styleUrl: './post-details.component.scss',
})
export class PostDetailsComponent implements OnInit {
  post!: Post;
  comments!: Comment[];
  newComment: CreateCommentDto = {
    Content: '',
    PostId: 0,
    ParentCommentId: undefined,
  };
  showAllComments = false;
  userService = inject(UserService);
  private feedService = inject(FeedService);
  private route = inject(ActivatedRoute);
  private titleService = inject(Title);

  ngOnInit() {
    this.post = this.route.snapshot.data['post'];
    if (this.post) this.titleService.setTitle(this.post.title);
    this.comments = this.route.snapshot.data['comments'];
  }
  sendComment() {
    const content = this.newComment?.Content.trim();
    if (!content) return;

    const dto: CreateCommentDto = {
      Content: content,
      PostId: this.post.id,
    };
    this.feedService.addComment(dto).subscribe({
      next: (comment) => {
        this.comments = [comment, ...this.comments];
        this.newComment.Content = '';
      },
    });
  }
  handleReply(event: { parentId: number; content: string }) {
    const { parentId, content } = event;

    const dto: CreateCommentDto = {
      Content: content,
      ParentCommentId: parentId,
      PostId: this.post.id,
    };

    this.feedService.addComment(dto).subscribe({
      next: (reply) => {
        const parent = this.findCommentById(this.comments, parentId);
        if (!parent) return;

        parent.replies = [...(parent.replies || []), reply];
        parent.showReplies = true;
      },
    });
  }

  get displayedComments() {
    return this.showAllComments ? this.comments : this.comments.slice(0, 4);
  }

  toggleShowAll() {
    this.showAllComments = !this.showAllComments;
  }
  autoResize(event: Event) {
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.height = textarea.scrollHeight + 'px';
  }

  private findCommentById(comments: Comment[], id: number): Comment | null {
    for (const comment of comments) if (comment.id === id) return comment;
    return null;
  }
}
