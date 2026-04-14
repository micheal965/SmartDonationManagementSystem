export interface Conversation {
  id: number;

  lastMessage: string;
  lastMessageAt: string;

  otherUserId: string;
  otherUserName: string;
  otherUserImage?: string;
}
