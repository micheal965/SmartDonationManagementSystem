export interface DonationDetails {
  id: number;
  amount: number;
  status: string;
  type: string;
  paymentGateway: string;
  postId?: number;
  postTitle?: string;
  postPicture?: string;
  categoryName?: string;
  donorId: string;
  donorName: string;
  donorPhoneNumber: string;
  requesterName?: string;
  requesterPhoneNumber?: string;
  createdAt: Date;
}
