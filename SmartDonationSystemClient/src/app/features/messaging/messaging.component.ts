import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { ChatService } from '../../core/services/chat.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ConversationsComponent } from '../conversations/conversations.component';
import { ChatComponent } from '../chat/chat.component';
import { Router, NavigationEnd } from '@angular/router';
import { filter, startWith, map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-messaging',
  standalone: true,
  imports: [CommonModule, FormsModule, ConversationsComponent, ChatComponent],
  templateUrl: './messaging.component.html',
  styleUrl: './messaging.component.scss',
})
export class MessagingComponent implements OnInit {
  chatService = inject(ChatService);
  private router = inject(Router);

  isConversationOpen = signal(false);

  isOnMessagingPage = toSignal(
    this.router.events.pipe(
      filter((event) => event instanceof NavigationEnd),
      startWith(null),
      map(() => this.router.url.includes('/messaging')),
    ),
  );

  showFloatingChat = computed(() => {
    return (
      !this.isOnMessagingPage() &&
      this.chatService.state().selectedConversation?.id
    );
  });

  ngOnInit(): void {
    this.chatService.startConnection();
    this.chatService.loadConversations();
  }

  toggleConversations() {
    this.isConversationOpen.set(!this.isConversationOpen());
  }
}
