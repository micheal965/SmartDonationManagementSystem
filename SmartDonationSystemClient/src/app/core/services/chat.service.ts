import { Injectable, inject, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../features/auth/services/auth.service';
import { apiBaseUrl, BaseUrl } from '../utils/app.config';
import { ApiResult } from '../../shared/models/api-result-model';
import { PaginatedResponse } from '../../shared/models/paginated-response.model';
import { MessagePayload } from '../../shared/models/message-payload-model';
import { Conversation } from '../../shared/models/conversation-model';
import { MessageRequest } from '../../shared/models/message-request-model';
import { map, Observable } from 'rxjs';
import { UserService } from './user.service';
import { ChatState } from '../../shared/models/chat-state-model';
import { AudioService } from './audio.service';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);
  private audioService = inject(AudioService);

  private hubConnection!: signalR.HubConnection;

  private isStarting = false;
  private isStarted = false;

  private _isLoadingMore = signal(false);
  isLoadingMore = this._isLoadingMore.asReadonly();

  state = signal<ChatState>({
    conversations: [],
    selectedConversation: null,
    messages: [],

    page: 1,
    pageSize: 10,
    totalPages: 1,
    totalItems: 0,
  });

  typingUserId = signal<string | null>(null);
  private typingTimeout: ReturnType<typeof setTimeout> | null = null;
  private typingSoundLock = false;

  startConnection() {
    const token = this.auth.getAccessToken();
    if (!token || this.isStarting || this.isStarted) return;

    this.isStarting = true;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${BaseUrl}/hubs/chat`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    this.registerListeners();

    this.hubConnection
      .start()
      .then(() => {
        this.isStarted = true;
        this.isStarting = false;
      })
      .catch(() => (this.isStarting = false));

    this.hubConnection.onreconnected(() => {
      const convId = this.state()?.selectedConversation?.id;
      if (convId) this.loadMessages(convId);
    });
  }

  stopConnection() {
    this.hubConnection?.stop();
    this.isStarted = false;
  }

  private registerListeners() {
    this.hubConnection.on('ReceiveMessage', (message: MessagePayload) => {
      const state = this.state();
      if (!state) return;
      const isMine = message.senderId === this.auth.userData.id;

      const enriched: MessagePayload = {
        ...message,
        isMine,
      };

      const isActive =
        state.selectedConversation?.id === message.conversationId;
      if (isActive && !isMine) {
        this.hubConnection.invoke('MarkAsRead', message.conversationId);
      }

      this.updateConversationPreview(enriched, isMine);

      this.state.update((s) => {
        if (!s) return s;

        if (!isActive) {
          this.audioService.playSound('message');
          return s;
        }

        // 💬 update messages only if chat is open
        if (!isActive) return s;

        return {
          ...s,
          messages: [...s.messages, enriched],
        };
      });
    });

    this.hubConnection.on('UserTyping', (data) => {
      const senderId = data.senderId;

      this.typingUserId.set(senderId);

      if (this.typingTimeout) clearTimeout(this.typingTimeout);
      // play sound ONLY if not locked
      if (!this.typingSoundLock && this.state()?.selectedConversation != null) {
        this.audioService.playSound('typing');

        this.typingSoundLock = true;

        setTimeout(() => {
          this.typingSoundLock = false;
        }, 5000);
      }
      this.typingTimeout = setTimeout(() => {
        if (this.typingUserId() === senderId) this.typingUserId.set(null);
      }, 1500);
    });

    this.hubConnection.on('MessagesRead', (data) => {
      const { userId, conversationId } = data;
      this.state.update((s) => {
        const updatedConversations = s.conversations.map((c) =>
          c.id === conversationId ? { ...c, lastMessageIsRead: true } : c,
        );

        const updatedSelectedConversation =
          s.selectedConversation?.id === conversationId
            ? { ...s.selectedConversation, lastMessageIsRead: true }
            : s.selectedConversation;

        const updatedMessages = s.messages.map((m) =>
          m.senderId !== userId ? { ...m, isRead: true } : m,
        );

        return {
          ...s,
          conversations: updatedConversations as Conversation[],
          selectedConversation: updatedSelectedConversation as Conversation,
          messages: updatedMessages,
        };
      });
    });
  }

  //Conversations
  loadConversations() {
    this.http
      .get<ApiResult<Conversation[]>>(`${apiBaseUrl}/Chat/conversations`)
      .subscribe((res) => {
        this.state.update((state) => ({
          ...state!,
          conversations: res.data,
        }));
      });
  }
  getOrCreateConversation(receiverId: string): Observable<Conversation> {
    return this.http
      .post<
        ApiResult<Conversation>
      >(`${apiBaseUrl}/Chat/conversations/get-or-create`, { receiverId })
      .pipe(map((res) => res.data));
  }
  openConversation(userId: string) {
    this.state.update((s) => ({
      ...s!,
      selectedConversation: null,
      messages: [],
    }));

    this.getOrCreateConversation(userId).subscribe((conversation) => {
      this.state.update((s) => ({
        ...s!,
        conversations: this.state().conversations.map((c) =>
          c.id === conversation.id
            ? { ...conversation, lastMessageIsRead: true }
            : c,
        ),
        selectedConversation: conversation,
      }));
      this.hubConnection.invoke('MarkAsRead', conversation.id);
    });
  }
  selectConversation(conversation: Conversation) {
    if (this.state()?.selectedConversation?.id === conversation.id) return;
    this.hubConnection.invoke('MarkAsRead', conversation.id);

    this.state.update((state) => ({
      ...state!,
      conversations: this.state().conversations.map((c) =>
        c.id === conversation.id
          ? { ...conversation, lastMessageIsRead: true }
          : c,
      ),
      selectedConversation: conversation,
      messages: [],
    }));
  }

  closeConversation() {
    this.state.update((s) => ({
      ...s!,
      selectedConversation: null,
    }));
  }

  //Messages
  loadMessages(page: number = 1) {
    const conversationId = this.state()?.selectedConversation?.id;
    if (!conversationId) return;

    this.http
      .get<
        ApiResult<PaginatedResponse<MessagePayload>>
      >(`${apiBaseUrl}/Chat/conversations/${conversationId}/messages?page=${page}&pageSize=5`)
      .subscribe((res) => {
        const data = res.data;

        this.state.update((s) => {
          if (!s) return s;

          return {
            ...s,
            messages: page === 1 ? data.items : [...data.items, ...s.messages],

            page: data.pageNumber,
            totalPages: data.totalPages,
            totalItems: data.totalCount,
          };
        });
      });
  }
  loadMore() {
    if (this._isLoadingMore()) return;

    const state = this.state();
    const nextPage = state.page + 1;
    this.loadMessages(nextPage);
  }
  sendMessage(newMessage: string) {
    const conversation = this.state()?.selectedConversation;
    if (!conversation) return;
    const request: MessageRequest = {
      conversationId: conversation.id,
      receiverId: conversation.otherUserId,
      content: newMessage,
    };
    this.typingUserId.set(null);
    return this.hubConnection.invoke('SendMessage', request);
  }
  //Typing Indicator
  sendTyping() {
    const receiverId = this.state()?.selectedConversation?.otherUserId;
    if (!receiverId) return;

    this.hubConnection.invoke('Typing', { receiverId });
  }

  //Helpers
  resetChatState() {
    this.state.set({
      conversations: [],
      selectedConversation: null,
      messages: [],

      page: 1,
      pageSize: 10,
      totalPages: 1,
      totalItems: 0,
    });
  }
  private updateConversationPreview(message: MessagePayload, isMine: boolean) {
    this.state.update((s) => {
      if (!s) return s;

      const updatedConversations = s.conversations.map((c) => {
        if (c.id !== message.conversationId) return c;

        return {
          ...c,
          lastMessage: message.content,
          lastMessageAt: message.createdAt,
          lastMessageIsRead: isMine ? true : message.isMine,
        };
      });

      const exists = s.conversations.some(
        (c) => c.id === message.conversationId,
      );

      const conversations = exists
        ? updatedConversations.sort(
            (a, b) =>
              new Date(b.lastMessageAt ?? 0).getTime() -
              new Date(a.lastMessageAt ?? 0).getTime(),
          )
        : [
            {
              id: message.conversationId,
              lastMessage: message.content,
              lastMessageAt: message.createdAt,
            } as any,
            ...s.conversations,
          ];

      return {
        ...s,
        conversations,
      };
    });
  }
}
