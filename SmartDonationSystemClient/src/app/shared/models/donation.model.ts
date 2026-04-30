export interface DonationDto {
  PostId: number;
  Amount: number;
  Gateway: 'Stripe' | 'PayPal';
}

