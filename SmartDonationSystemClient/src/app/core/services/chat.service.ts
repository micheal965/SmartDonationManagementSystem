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
import { Observable } from 'rxjs';
import { UserService } from './user.service';
import { ChatState } from '../../shared/models/chat-state-model';
import { AudioService } from './audio.service';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private http = inject(HttpClient);
  private auth = inject(AuthService);
  private userService = inject(UserService);
  private audioService = inject(AudioService);

  private hubConnection!: signalR.HubConnection;

  private isStarting = false;
  private isStarted = false;
  private _isLoadingMore = signal(false);

  // expose as readonly
  isLoadingMore = this._isLoadingMore.asReadonly();

  state = signal<ChatState>({
    conversations: [],
    selectedConversation: null,
    messages: [],

    page: 1,
    pageSize: 5,
    totalPages: 1,
    totalItems: 0,
  });

  typingUsers = signal<Set<string>>(new Set());

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
      if (convId) this.joinConversation(convId);
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

      // 🔥 ALWAYS update conversation preview (both users)
      this.updateConversationPreview(enriched);

      this.state.update((s) => {
        if (!s) return s;

        // 🔊 sound ONLY for non-active chat
        if (!isActive && !isMine) {
          this.audioService.playNotificationSound();
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
  }
  resetChatState() {
    this.state.set({
      conversations: [],
      selectedConversation: null,
      messages: [],
      page: 1,
      pageSize: 5,
      totalPages: 1,
      totalItems: 0,
    });
  }
  getOrCreateConversation(receiverId: string): Observable<number> {
    return this.http.post<number>(
      `${apiBaseUrl}/Chat/conversations/get-or-create`,
      { receiverId },
    );
  }
  openConversation(userId: string) {
    this.state.update((s) => ({
      ...s!,
      selectedConversation: null,
      messages: [],
    }));
    this.userService.getUser(userId).subscribe((user) => {
      if (!user) return;
      this.state.update((s) => ({
        ...s!,
        selectedConversation: {
          ...s!.selectedConversation,
          otherUserId: userId,
          otherUserName: user.fullName,
          otherUserImage: user.pictureUrl,
        } as Conversation,
      }));
    });
    this.getOrCreateConversation(userId).subscribe((conversationId) => {
      this.joinConversation(conversationId);
      this.state.update((s) => ({
        ...s!,
        selectedConversation: {
          ...s!.selectedConversation,
          id: conversationId,
        } as Conversation,
      }));
    });
  }

  closeConversation() {
    this.state.update((s) => ({
      ...s!,
      selectedConversation: null,
    }));
  }
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

  selectConversation(conversation: Conversation) {
    this.state.update((state) => ({
      ...state!,
      selectedConversation: conversation,
      messages: [],
    }));

    this.joinConversation(conversation.id);
  }

  joinConversation(conversationId: number) {
    if (!this.isStarted) return;

    this.hubConnection.invoke('JoinConversation', conversationId);

    this.state.update((state) => ({
      ...state!,
      selectedConversation: {
        ...state!.selectedConversation,
        id: conversationId,
      } as Conversation,
    }));
  }

  leaveConversation(conversationId: number) {
    this.hubConnection.invoke('LeaveConversation', conversationId);
  }

  loadMessages(page: number = 1) {
    const state = this.state();
    const conversationId = state?.selectedConversation?.id;

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
    const state = this.state();
    const conversation = state?.selectedConversation;

    if (!conversation) return;

    const request: MessageRequest = {
      conversationId: conversation.id,
      receiverId: conversation.otherUserId,
      content: newMessage,
    };

    return this.hubConnection.invoke('SendMessage', request);
  }

  sendTyping() {
    const conversationId = this.state()?.selectedConversation?.id;
    if (!conversationId) return;

    this.hubConnection.invoke('Typing', { conversationId });
  }
  private updateConversationPreview(message: MessagePayload) {
    this.state.update((s) => {
      if (!s) return s;

      const currentUserId = this.auth.userData.id;

      const isMine = message.senderId === currentUserId;

      const otherUserId = isMine ? message.receiverId : message.senderId;

      const otherUserName = isMine ? message.receiverName : message.senderName;

      const otherUserImage = isMine
        ? message.receiverImage
        : message.senderImage;

      const now = new Date().toISOString();

      const updated: Conversation = {
        id: message.conversationId,
        lastMessage: message.content,
        lastMessageAt: now,

        otherUserId,
        otherUserName,
        otherUserImage: otherUserImage || './assets/avatar.png',
      };

      const index = s.conversations.findIndex(
        (c) => c.id === message.conversationId,
      );

      const conversations =
        index === -1
          ? [updated, ...s.conversations]
          : [
              updated,
              ...s.conversations.filter((c) => c.id !== message.conversationId),
            ];

      const selectedConversation =
        s.selectedConversation?.id === message.conversationId
          ? updated
          : s.selectedConversation;

      return {
        ...s,
        conversations,
        selectedConversation,
      };
    });
  }
}
