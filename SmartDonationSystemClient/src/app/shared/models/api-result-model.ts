export interface ApiResult<T> {
  statusCode: number;
  success: boolean;
  message?: string;
  data?: T;
  errors?: any;
}
