import { map, pipe } from 'rxjs';
import { computed, inject, Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { apiBaseUrl, BaseUrl } from '../utils/app.config';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../features/auth/services/auth.service';
import { ApiResult } from '../../shared/models/api-result-model';
import { PaginatedResponse } from '../../shared/models/paginated-response.model';
import { NotificationPayload } from '../../shared/models/notification-payload-model';

import { AudioService } from './audio.service';
import { LoadNotificationsResponse } from '../../shared/models/load-notifications-response-model';
@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  private audioService = inject(AudioService);
  private httpClient = inject(HttpClient);
  private authService = inject(AuthService);
  private hubConnection!: signalR.HubConnection;
  private isStarting = false;
  private isStarted = false;
  private listenersRegistered = false;

  state = signal<{
    items: NotificationPayload[];
    page: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
  } | null>(null);

  unreadCount = signal<number>(0);
  startConnection() {
    const token = this.authService.getAccessToken();
    if (this.isStarted || this.isStarting || !token) return;
    this.isStarting = true;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${BaseUrl}/hubs/notifications`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();
    if (!this.listenersRegistered) {
      this.registerListeners();
      this.listenersRegistered = true;
    }

    this.hubConnection
      .start()
      .then(() => {
        this.isStarted = true;
        this.isStarting = false;
      })
      .catch(() => {
        this.isStarting = false;
      });
  }

  private registerListeners() {
    this.hubConnection.on(
      'ReceiveNotification',
      (notification: NotificationPayload) => {
        this.audioService.playNotificationSound();
        this.unreadCount.set(this.unreadCount() + 1);
        this.state.update((state) => {
          if (!state) return state;
          return {
            ...state,
            items: [notification, ...state.items],
            totalItems: state.totalItems + 1,
          };
        });
      },
    );
  }

  stopConnection() {
    this.hubConnection?.stop();
  }
  loadNotifications(page: number = 1, pageSize: number = 10) {
    this.httpClient
      .get<ApiResult<LoadNotificationsResponse>>(
        `${apiBaseUrl}/notification/get-user-notifications?page=${page}&pageSize=${pageSize}`,
      )
      .pipe(map((res) => res.data))
      .subscribe((res) => {
        this.state.set({
          items: [...(this.state()?.items || []), ...res.result.items],
          page: res.result.pageNumber,
          pageSize: res.result.pageSize,
          totalItems: res.result.totalCount,
          totalPages: res.result.totalPages,
        });
        this.unreadCount.set(res.unreadCount);
      });
  }
  markAsRead(id: number) {
    this.httpClient
      .put(`${apiBaseUrl}/notification/read?id=${id}`, {})
      .subscribe(() => {
        this.state.update((state) => {
          if (!state) return state;

          return {
            ...state,
            items: state.items.map((n) =>
              n.id === id ? { ...n, isRead: true } : n,
            ),
          };
        });
      });
  }
  markAllAsRead() {
    this.httpClient
      .put(`${apiBaseUrl}/notification/mark-all-read`, {})
      .subscribe(() => {
        this.unreadCount.set(0);
      });
  }
}
