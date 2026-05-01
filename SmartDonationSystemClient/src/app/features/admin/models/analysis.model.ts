export interface TrendDto {
  date: Date;
  value: number;
}

export interface CategoryDistributionDto {
  categoryName: string;
  totalAmount: number;
  donationCount: number;
}

export interface CategoryTrendDto {
  categoryName: string;
  trends: TrendDto[];
}

export interface StatusDistributionDto {
  status: string;
  count: number;
}

export interface AnalysisModel {
  totalPaidAndProcessedDonations: number;
  totalPaidDonations: number;
  totalProcessedToClientDonations: number;
  totalCompletedDonations: number;
  totalCompletedTargets: number;
  totalDonationAmount: number;
  totalDonors: number;
  donationTrend: TrendDto[];
  categoryTrends: CategoryTrendDto[];
  categoryBreakdown: CategoryDistributionDto[];
  statusBreakdown: StatusDistributionDto[];
}
