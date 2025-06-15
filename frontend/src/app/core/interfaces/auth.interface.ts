export interface AuthResponseDto {
  accessToken: string;
}
export interface TokenRefreshResponseDto {
  accessToken: string;
}
export interface UserProfileDto {
  username: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  image: string;
  date: string;
}
export interface UserUpdateDto {
}