import {
  Component,
  inject,
  OnInit,
  signal,
  HostListener,
  PLATFORM_ID,
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CommonModule } from '@angular/common';
import { ChatService } from '../../../../core/services/chat.service';
import { ConversationsComponent } from '../../../conversations/conversations.component';
import { ChatComponent } from '../../../chat/chat.component';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-message-page',
  standalone: true,
  imports: [CommonModule, ConversationsComponent, ChatComponent, MatIconModule],
  templateUrl: './message-page.component.html',
  styleUrl: './message-page.component.scss',
})
export class MessagePageComponent implements OnInit {
  chatService = inject(ChatService);
  private platformId = inject(PLATFORM_ID);
  isLargeScreen = signal(true);

  constructor() {
    if (isPlatformBrowser(this.platformId)) {
      this.isLargeScreen.set(window.innerWidth >= 1024);
    }
  }

  onResize() {
    if (isPlatformBrowser(this.platformId)) {
      this.isLargeScreen.set(window.innerWidth >= 1024);
    }
  }

  ngOnInit(): void {
    this.chatService.loadConversations();
  }

  backToConversations() {
    this.chatService.closeConversation();
  }
}
