export interface UserProfile {
  id: string;
  fullName: string;
  pictureUrl: string;
  birthDate?: string;
  address: string;
  phoneNumber: string;
  role: string;
  isSoftDeleted: boolean;
}
