import { Component, inject, OnInit, signal } from '@angular/core';
import { ChatService } from '../../core/services/chat.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConversationsComponent } from '../conversations/conversations.component';
import { ChatComponent } from '../chat/chat.component';

@Component({
  selector: 'app-messaging',
  standalone: true,
  imports: [CommonModule, FormsModule, ConversationsComponent, ChatComponent],
  templateUrl: './messaging.component.html',
  styleUrl: './messaging.component.scss',
})
export class MessagingComponent implements OnInit {
  chatService = inject(ChatService);
  isConversationOpen = signal(false);

  ngOnInit(): void {
    this.chatService.loadConversations();
  }

  toggleConversations() {
    this.isConversationOpen.set(!this.isConversationOpen());
  }
}
