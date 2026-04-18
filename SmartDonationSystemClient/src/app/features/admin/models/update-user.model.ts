export interface UpdateUserDto {
  id: string;
  fullName?: string;
  identityNumber?: string;
  phoneNumber?: string;
  pictureUrl?: string | null;
  birthDate?: string;
  address?: string | null;
  role?: string;
}
