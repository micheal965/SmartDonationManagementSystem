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
} from '@angular/core';
import { ChatService } from '../../core/services/chat.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MessagePayload } from '../../shared/models/message-payload-model';
import { InfiniteScrollDirective } from '../../shared/directives/infinite-scroll.directive';

@Component({
  selector: 'app-chat',
  imports: [FormsModule, CommonModule, InfiniteScrollDirective],
  standalone: true,
  templateUrl: './chat.component.html',
})
export class ChatComponent implements AfterViewChecked {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
  chatService = inject(ChatService);
  newMessage: string = '';

  get messages(): MessagePayload[] {
    return this.chatService.state()?.messages || [];
  }
  ngOnInit(): void {
    this.chatService.loadMessages();
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
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
