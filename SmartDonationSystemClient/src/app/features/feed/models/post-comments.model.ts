export interface Comment {
  id: number;
  content: string;
  createdAt: Date;
  userName: string;
  creatorPictureUrl: string;
  replies: Comment[];
  mentions?: Mention[];
  // UI state (frontend only)
  showReplies?: boolean;
  showReplyInput?: boolean;
}
export interface Mention {
  userId: string;
  userName: string;
}
