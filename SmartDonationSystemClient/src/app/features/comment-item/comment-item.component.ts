import { FormsModule } from '@angular/forms';
import { Comment } from './../feed/models/post-comments.model';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimeAgoPipe } from '../../shared/pipes/time-ago.pipe';
import { MatIconModule } from '@angular/material/icon';
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
}
