export interface DashboardModel {
  totalUsers: number;
  totalCategories: number;
  totalPublishedPosts: number;
  analytics: AnalyticsDto[];
  totalUniqueUsers: number;
}

export interface AnalyticsDto {
  date: string;
  count: number;
}
