export interface User {
  id: string;
  email: string;
  fullName: string;
  phoneNumber: string;
  faydaVerified: boolean;
  createdAt: string;
}

export interface RegisterResponse {
  userId: string;
  email: string;
  phoneNumber: string;
  message: string;
  devSimulatedOtp?: string | null;
  faydaOtpDevHint?: string | null;
}

export interface AuthResponse {
  token: string;
  expiresIn: number;
  user: User;
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
  otpCode: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

