export interface Post {
  id: number;
  title: string;
  content: string;
  createdAt: string;
  priorityLevel: number;
  userId: string;
  fullName: string;
  phoneNumber: string;
  viewCount: number;
  postPicture: string;
  createdByRole: 'Requester' | 'Donor';
  pictureUrl: string;
  attachments: string[];
  likesCount: number;
  hasReacted: boolean;
  isReacting: boolean;
  categoryName: string;
  targetMoney?: number;
  collectedMoney?: number;
  recentDonations?: RecentDonation[];
}

export interface RecentDonation {
  donorName: string;
  donorPictureUrl: string;
  amount: number;
  createdAt: string;
}
