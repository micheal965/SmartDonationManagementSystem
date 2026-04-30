export interface MyDonation {
  id: number;
  amount: number;
  status: string;
  type: string;
  paymentGateway: string;
  postId?: number;
  postTitle?: string;
  postPicture?: string;
  checkoutUrl?: string;
  createdAt: Date;
}
