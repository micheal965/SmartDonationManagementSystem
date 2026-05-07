import {
  Component,
  ElementRef,
  ViewChild,
  AfterViewChecked,
  inject,
  computed,
  PLATFORM_ID,
  signal,
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { ChatService } from '../../core/services/chat.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MessagePayload } from '../../shared/models/message-payload-model';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-chat',
  imports: [FormsModule, CommonModule, MatIconModule],
  standalone: true,
  templateUrl: './chat.component.html',
})
export class ChatComponent implements AfterViewChecked {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
  chatService = inject(ChatService);
  private platformId = inject(PLATFORM_ID);
  newMessage: string = '';

  isLargeScreen = signal(true);

  private typingInterval: ReturnType<typeof setInterval> | null = null;
  private typingTimeout: ReturnType<typeof setTimeout> | null = null;

  private isUserAtBottom = true;
  private lastMessageCount = 0;
  private isLoadingMore = false;
  private previousScrollHeight = 0;
  private previousScrollTop = 0;

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

  isTyping = computed(() => {
    return (
      this.chatService.typingUserId() ===
      this.chatService.state().selectedConversation?.otherUserId
    );
  });

  get messages(): MessagePayload[] {
    return this.chatService.state()?.messages || [];
  }
  ngOnInit(): void {
    this.chatService.startConnection();
    this.chatService.loadMessages();
  }

  ngAfterViewChecked(): void {
    if (isPlatformBrowser(this.platformId)) {
      const element = this.messagesContainer.nativeElement as HTMLElement;

      if (
        this.isLoadingMore &&
        element.scrollHeight > this.previousScrollHeight
      ) {
        element.scrollTop =
          this.previousScrollTop +
          (element.scrollHeight - this.previousScrollHeight);
        this.isLoadingMore = false;
        this.previousScrollHeight = 0;
        this.previousScrollTop = 0;
      }

      if (this.isUserAtBottom && this.haveNewMessages()) {
        this.scrollToBottom(true);
      }
    }
  }

  onScroll() {
    if (isPlatformBrowser(this.platformId)) {
      const element = this.messagesContainer.nativeElement as HTMLElement;
      const atBottom =
        element.scrollHeight - element.scrollTop <= element.clientHeight + 100;
      this.isUserAtBottom = atBottom;

      if (element.scrollTop <= 150 && !this.isLoadingMore) {
        this.onLoadMore();
      }
    }
  }

  onInputChange(event: Event) {
    this.newMessage = (event.target as HTMLInputElement).value;

    const receiverId =
      this.chatService.state()?.selectedConversation?.otherUserId;
    if (!receiverId) return;

    // start typing once
    if (!this.typingInterval) {
      this.chatService.sendTyping();

      this.typingInterval = setInterval(() => {
        this.chatService.sendTyping();
      }, 2000); // keep alive signal
    }

    // reset stop timer
    if (this.typingTimeout) {
      clearTimeout(this.typingTimeout);
    }

    this.typingTimeout = setTimeout(() => {
      this.stopTyping();
    }, 1500);
  }

  private stopTyping() {
    if (this.typingInterval) {
      clearInterval(this.typingInterval);
      this.typingInterval = null;
    }
  }

  sendMessage() {
    if (!this.newMessage.trim()) return;
    this.chatService.sendMessage(this.newMessage);

    this.newMessage = '';
    this.isUserAtBottom = true;
    setTimeout(() => this.scrollToBottom(true), 50);
  }

  onLoadMore() {
    const state = this.chatService.state();
    if (!state || state.page >= state.totalPages) return;

    const element = this.messagesContainer.nativeElement as HTMLElement;
    this.previousScrollHeight = element.scrollHeight;
    this.previousScrollTop = element.scrollTop;
    this.isLoadingMore = true;

    this.chatService.loadMore();
  }

  closeChat() {
    this.chatService.closeConversation();
  }

  private haveNewMessages(): boolean {
    const currentCount = this.messages.length;
    const changed = currentCount !== this.lastMessageCount;
    this.lastMessageCount = currentCount;
    return changed;
  }

  private scrollToBottom(smooth = false) {
    try {
      const element = this.messagesContainer.nativeElement as HTMLElement;
      if (smooth && 'scrollTo' in element) {
        element.scrollTo({ top: element.scrollHeight, behavior: 'smooth' });
      } else {
        element.scrollTop = element.scrollHeight;
      }
    } catch {}
  }
}
