import {
  Component,
  EventEmitter,
  inject,
  OnInit,
  Output,
  output,
  signal,
} from '@angular/core';
import { ChatService } from '../../core/services/chat.service';
import { CommonModule } from '@angular/common';
import { Conversation } from '../../shared/models/conversation-model';

@Component({
  selector: 'app-conversations',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './conversations.component.html',
  styleUrl: './conversations.component.scss',
})
export class ConversationsComponent {
  chatService = inject(ChatService);

  selectConversation(conversation: Conversation) {
    this.chatService.selectConversation(conversation);
  }
}
