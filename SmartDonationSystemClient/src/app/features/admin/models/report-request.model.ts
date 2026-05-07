export enum ReportType {
  DonationsReport,
  PostsReport,
  UsersReport,
}

export interface ReportFilter {
  field: string;
  operator: string;
  value: string;
}

export interface ReportRequest {
  reportType: ReportType;
  filters: ReportFilter[];
  dateFrom?: string | null;
  dateTo?: string | null;
  page: number;
  pageSize: number;
}
