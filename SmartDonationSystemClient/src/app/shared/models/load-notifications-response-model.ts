import { NotificationPayload } from './notification-payload-model';
import { PaginatedResponse } from './paginated-response.model';

export interface LoadNotificationsResponse {
  result: PaginatedResponse<NotificationPayload>;
  unreadCount: number;
}
