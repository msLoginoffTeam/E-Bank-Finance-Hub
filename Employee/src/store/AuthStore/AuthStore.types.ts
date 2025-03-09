export interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  profile: EmployeeProfile;
  isLoggedIn: boolean;
  isLoading: boolean;
  error?: string;
}

export interface AuthCredentials {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
}

export interface CreateUser {
  email: string;
  password: string;
  fullName: string;
  userRole: UserRole;
}

export enum UserRole {
  Client = 'Client',
  Employee = 'Employee',
}

export interface EmployeeProfile {
  id: string;
  email: string;
  fullName: string;
  isBlocked: boolean;
}
