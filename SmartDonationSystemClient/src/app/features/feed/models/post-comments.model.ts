export interface Comment {
  id: number;
  content: string;
  createdAt: Date;
  userName: string;
  creatorPictureUrl: string;
  replies: Comment[];

  // UI state (frontend only)
  showReplies?: boolean;
  showReplyInput?: boolean;
}
