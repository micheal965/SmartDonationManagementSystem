export interface UserCommentsDto {
  totalCommentsCount: number;
  comments: UserCommentDto[];
}

export interface UserCommentDto {
  postId: number;
  content: string;
  createdAt: Date;
}
