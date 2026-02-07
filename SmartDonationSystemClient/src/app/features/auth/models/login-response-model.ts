export interface LoginResponse {
  token?: string;
  user?: {
    id: number;
    fullName: string;
  };
}
