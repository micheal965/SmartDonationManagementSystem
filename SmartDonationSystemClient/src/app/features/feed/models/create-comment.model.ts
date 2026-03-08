export interface CreateCommentDto {
  Content: string;
  PostId: number;
  ParentCommentId?: number;
  MentionedUserIds?: string[];
}
