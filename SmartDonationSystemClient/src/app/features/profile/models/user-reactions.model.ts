export interface UserReactionsDto {
  totalLikesCount: number;
  reactions: userReaction[];
}

export interface userReaction {
  postId: number;
  postTitle: string;
  createdAt: Date;
}
