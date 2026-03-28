export interface UserToReturnDto {
  id: string;
  fullName: string;
  identityNumber: string;
  pictureUrl?: string | null;
  birthDate: string;
  address?: string | null;
  isSoftDeleted: boolean;
  role: string;
}
