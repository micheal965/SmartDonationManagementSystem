export interface MessagePayload {
  id: number;
  conversationId: number;

  senderId: string;
  senderName: string;
  senderImage?: string | null;

  receiverId: string;
  receiverName: string;
  receiverImage?: string | null;
  
  content: string;

  createdAt: string | Date;

  isMine: boolean;
}
