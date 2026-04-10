export interface NotificationPayload {
  id: number;
  title: string;
  message: string;
  entityId?: number;
  redirectUrl: string;
  actorName?: string;
  type: string;
  actorImage?: string;
  createdAt: string;
  isRead: boolean;
}
