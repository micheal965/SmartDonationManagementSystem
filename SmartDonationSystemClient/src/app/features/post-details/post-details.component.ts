import { PriorityClassPipe } from './../../shared/pipes/priority-class.pipe';
import {
  AfterViewInit,
  Component,
  ElementRef,
  inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TimeAgoPipe } from '../../shared/pipes/time-ago.pipe';
import { CommonModule } from '@angular/common';
import { UserService } from '../../core/services/user.service';
import { PriorityLabelPipe } from '../../shared/pipes/priority-label.pipe';
import { Post } from '../feed/models/post.model';
import { FormsModule } from '@angular/forms';
import { FeedService } from '../feed/services/feed.service';
import { Comment } from '../feed/models/post-comments.model';
import { CreateCommentDto } from '../feed/models/create-comment.model';
import { CommentItemComponent } from '../comment-item/comment-item.component';
import Tribute from 'tributejs';
import { AnalyticsService } from '../../core/services/analytics.service';
import { ChatService } from '../../core/services/chat.service';
import { AuthService } from '../auth/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-post-details',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatTooltipModule,
    TimeAgoPipe,
    PriorityLabelPipe,
    PriorityClassPipe,
    FormsModule,
    CommentItemComponent,
    RouterLink,
  ],
  templateUrl: './post-details.component.html',
  styleUrl: './post-details.component.scss',
})
export class PostDetailsComponent implements OnInit, AfterViewInit {
  @ViewChild('commentTextarea') textarea!: ElementRef<HTMLTextAreaElement>;
  post!: Post;
  comments!: Comment[];
  users = [];
  newComment: CreateCommentDto = {
    Content: '',
    PostId: 0,
    ParentCommentId: undefined,
  };
  showAllComments = false;
  mentionedUserIds: string[] = [];

  userService = inject(UserService);
  authService = inject(AuthService);
  chatService = inject(ChatService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private titleService = inject(Title);
  private feedService = inject(FeedService);
  private analyticsService = inject(AnalyticsService);
  private toastr = inject(ToastrService);

  get displayedComments() {
    return this.showAllComments ? this.comments : this.comments.slice(0, 4);
  }

  ngOnInit() {
    this.post = this.route.snapshot.data['post'];
    if (this.post) {
      this.titleService.setTitle(this.post.title);
      this.analyticsService.trackPostEntrance(this.post.id);
    }
    this.comments = this.route.snapshot.data['comments'];
    this.userService.loadProfile();
  }

  ngAfterViewInit() {
    const tribute = new Tribute({
      lookup: 'value',
      fillAttr: 'value',
      menuItemTemplate: (item: any) => `
        <div class="flex items-center gap-2">
            <img src="${item.original.avatar ? item.original.avatar : './assets/avatar.png'}" 
       class="w-8 h-8 rounded-full object-cover"/>
          <span class="font-normal">${item.original.value}</span>
        </div>
      `,
      selectTemplate: (item: any) => {
        if (!item) return '';

        if (!this.mentionedUserIds.includes(item.original.key))
          this.mentionedUserIds.push(item.original.key);

        return `@${item.original.value}`;
      },
      values: (text: string, callback: any) => {
        if (!text) return;
        const query = text.replace('@', '');
        this.userService
          .searchUsers(query)
          .subscribe((users) => callback(users));
      },
      requireLeadingSpace: true,
    });

    tribute.attach(this.textarea.nativeElement);
  }

  onLike() {
    if (this.post.isReacting) return;
    this.post.isReacting = true;

    const previousState = {
      hasReacted: this.post.hasReacted,
      likesCount: this.post.likesCount,
    };
    this.post.hasReacted = !this.post.hasReacted;
    this.post.likesCount += this.post.hasReacted ? 1 : -1;

    this.feedService.reactToPost(this.post.id).subscribe({
      next: () => (this.post.isReacting = false),
      error: () => {
        this.post.hasReacted = previousState.hasReacted;
        this.post.likesCount = previousState.likesCount;
        this.post.isReacting = false;
      },
    });
  }

  onDonate() {
    this.toastr.info('Donation feature coming soon!', 'Stay Tuned');
  }

  sendComment() {
    const content = this.newComment?.Content.trim();
    if (!content) return;

    const dto: CreateCommentDto = {
      Content: content,
      PostId: this.post.id,
      MentionedUserIds: this.mentionedUserIds,
    };
    this.feedService.addComment(dto).subscribe({
      next: (comment) => {
        this.comments = [comment, ...this.comments];
        this.newComment.Content = '';

        this.mentionedUserIds = [];
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

  insertMention(user: any) {
    return '@' + user.name;
  }
  toggleShowAll() {
    this.showAllComments = !this.showAllComments;
  }

  autoResize(event: Event) {
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.height = textarea.scrollHeight + 'px';
  }
  getFileIcon(url: string): string {
    if (!url) return 'attach_file';

    const lower = url.toLowerCase();

    if (lower.endsWith('.pdf')) return 'picture_as_pdf';
    if (lower.match(/\.(jpg|jpeg|png|gif|webp)$/)) return 'image';
    if (lower.match(/\.(doc|docx)$/)) return 'description';
    if (lower.match(/\.(xls|xlsx)$/)) return 'table_chart';
    if (lower.match(/\.(zip|rar)$/)) return 'folder_zip';

    return 'link';
  }

  private findCommentById(comments: Comment[], id: number): Comment | null {
    for (const comment of comments) if (comment.id === id) return comment;
    return null;
  }
}
