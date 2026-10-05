export interface User {
  id: string;
  email: string;
  fullName: string;
  phoneNumber: string;
  faydaVerified: boolean;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  userId: string;
  email: string;
  fullName: string;
  phoneNumber: string;
  faydaVerified: boolean;
  faydaOtpDevHint?: string | null;
  requiresFaydaVerification: boolean;
  message: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  phoneNumber: string;
  password: string;
  faydaFan: string;
}

export interface VerifyFaydaRequest {
  userId: string;
  otp: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}
