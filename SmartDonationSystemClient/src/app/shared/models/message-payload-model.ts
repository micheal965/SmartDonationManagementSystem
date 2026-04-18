import { MessageParticipantPayload } from './message-participants-model';

export interface MessagePayload {
  id: number;
  conversationId: number;

  senderId: string;
  receiverId: string;
  content: string;

  createdAt: string | Date;

  isMine: boolean;
  isRead: boolean;
  participants: MessageParticipantPayload;
}
