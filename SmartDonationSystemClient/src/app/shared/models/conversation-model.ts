export interface Conversation {
  id: number;

  lastMessage: string;
  lastMessageAt: string;
  lastMessageIsRead:boolean;
  
  otherUserId: string;
  otherUserName: string;
  otherUserImage?: string;
}
