export interface DonationToReturn {
  id: number;
  amount: number;
  status: string;
  type: string;
  paymentGateway: string;
  postId?: number;
  postTitle?: string;
  donorId: string;
  donorName: string;
  donorPhoneNumber: string;
  createdAt: Date;
}
