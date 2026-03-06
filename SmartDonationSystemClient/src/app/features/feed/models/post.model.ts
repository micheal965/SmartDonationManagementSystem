export interface Post {
  id: number;
  title: string;
  content: string;
  createdAt: string;
  priorityLevel: number;
  userId: string;
  fullName: string;
  phoneNumber: string;
  pictureUrl: string;
  attachments: string[];
  likesCount: number;
  hasReacted: boolean;
  isReacting: boolean;
  categoryName: string;
}
