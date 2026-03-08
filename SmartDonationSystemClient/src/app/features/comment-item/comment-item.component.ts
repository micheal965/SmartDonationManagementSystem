import { FormsModule } from '@angular/forms';
import { Comment } from './../feed/models/post-comments.model';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimeAgoPipe } from '../../shared/pipes/time-ago.pipe';
import { MatIconModule } from '@angular/material/icon';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { Router } from '@angular/router';
@Component({
  selector: 'app-comment-item',
  standalone: true,
  imports: [FormsModule, CommonModule, TimeAgoPipe, MatIconModule],
  templateUrl: './comment-item.component.html',
  styleUrl: './comment-item.component.scss',
})
export class CommentItemComponent {
  @Input() comment!: Comment;
  @Input() isReply: boolean = false;
  @Output() replyAdded = new EventEmitter<{
    parentId: number;
    content: string;
  }>();

  showReplyInput = false;
  replyContent = '';
  constructor(
    private sanitizer: DomSanitizer,
    private router: Router,
  ) {}
  formatContent(): SafeHtml {
    let formatted = this.comment.content;

    if (this.comment.mentions?.length) {
      this.comment.mentions.forEach((m) => {
        const regex = new RegExp(`@${m.userName}`, 'g');
        formatted = formatted.replace(
          regex,
          `<span class="mention text-blue-500 font-semibold hover:text-blue-700 hover:underline cursor-pointer" data-userid="${m.userId}">@${m.userName}</span>`,
        );
      });
    }

    return this.sanitizer.bypassSecurityTrustHtml(formatted);
  }

  onMentionClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const userId = target.getAttribute('data-userid');
    if (userId) {
      this.goToProfile(userId);
    }
  }

  toggleReplies() {
    this.comment.showReplies = !this.comment.showReplies;
  }

  submitReply() {
    if (!this.replyContent.trim()) return;
    this.replyAdded.emit({
      parentId: this.comment.id,
      content: this.replyContent,
    });
    this.replyContent = '';
    this.showReplyInput = false;
  }

  autoResize(event: Event) {
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.height = textarea.scrollHeight + 'px';
  }

  private goToProfile(userId: string) {
    console.log('userid' + userId);
    this.router.navigate(['/profile', userId]);
  }
}
