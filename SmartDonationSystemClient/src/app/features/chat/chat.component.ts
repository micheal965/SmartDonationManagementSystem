import {
  Component,
  ElementRef,
  ViewChild,
  AfterViewChecked,
  OnInit,
  inject,
  AfterViewInit,
  AfterContentInit,
  effect,
  computed,
} from '@angular/core';
import { ChatService } from '../../core/services/chat.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MessagePayload } from '../../shared/models/message-payload-model';
import { InfiniteScrollDirective } from '../../shared/directives/infinite-scroll.directive';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-chat',
  imports: [FormsModule, CommonModule, InfiniteScrollDirective, MatIcon],
  standalone: true,
  templateUrl: './chat.component.html',
})
export class ChatComponent implements AfterViewChecked {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
  chatService = inject(ChatService);
  newMessage: string = '';

  private typingInterval: ReturnType<typeof setInterval> | null = null;
  private typingTimeout: ReturnType<typeof setTimeout> | null = null;

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
    this.scrollToBottom();
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
  }

  closeChat() {
    this.chatService.closeConversation();
  }
  private scrollToBottom() {
    try {
      this.messagesContainer.nativeElement.scrollTop =
        this.messagesContainer.nativeElement.scrollHeight;
    } catch {}
  }
}
