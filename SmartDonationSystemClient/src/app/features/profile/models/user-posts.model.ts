export interface UserPostsDto {
  totalPostsCount: number;
  posts: userPost[];
}

export interface userPost {
  id: number;
  title: string;
  content: string;
  likesCount: number;
  postPicture: string;
}
