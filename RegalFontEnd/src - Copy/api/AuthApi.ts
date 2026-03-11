// /src/api/AuthApiClient.ts

import { ApiClient } from '@/api/ApiClient'
import type { Result } from '@/types/Result'

// Định nghĩa các interface theo backend của bạn
export interface LoginRequest {
  userName: string
  password: string
  rememberMe?: boolean
}
export interface RegisterRequest {
  userName: string
  email: string
  password: string
}
export interface RefreshTokenRequest {
  accessToken: string
  refreshToken: string
}
export interface ForgotPasswordRequest {
  email: string
}
export interface ChangePasswordRequest {
  userName: string
  oldPassword: string
  newPassword: string
}
export interface VerifyTokenRequest {
  accessToken: string
}
export interface VerifyTokenResponse {
  isValid: boolean
  userName?: string
}
export interface IdentityResult {
  userName: string
  roles: string[]
  originalUserName: string
  accessToken: string
  refreshToken: string
  succeeded: boolean
  errorMessage: string
  userStatus: UserStatus,
  avatarUrl?: string
}
export interface User {
  userName: string;
  surname: string;
  email: string;
  password: string;
  accessToken: string;
  rememberMe: boolean;
}

// Định nghĩa UserStatus giống backend (ví dụ enum)
export enum UserStatus {
  LoginFailed = 0,
  LoginSucceeded = 1,
  LockedUser = 2,
  MustChangePassword = 3
}

// Hoặc nếu bạn dùng kiểu type:
// export type UserStatusType = 0 | 1 | 2 | 3

export class AuthApi extends ApiClient {
  controller: string = 'auth'
  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  // Đăng nhập
  public async login(data: LoginRequest): Promise<Result> {
    return await this.post<Result>(`/${this.controller}/login`, data)
  }

  // Đăng ký
  public async register(data: RegisterRequest): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/register`, data)
  }

  // Đăng xuất
  public async logout(): Promise<void> {
    return await this.get<void>(`/${this.controller}/logout`)
  }

  // Refresh token
  public async refreshToken(data: RefreshTokenRequest): Promise<IdentityResult> {
    return await this.post<IdentityResult>(`/${this.controller}/refresh-token`, data)
  }

  // Verify token
  public async verifyToken(data: VerifyTokenRequest): Promise<VerifyTokenResponse> {
    return await this.post<VerifyTokenResponse>(`/${this.controller}/verify-token`, data)
  }

  // Quên mật khẩu
  public async forgotPassword(data: ForgotPasswordRequest): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/ForgotPassword`, data)
  }

  // Reset password (GET)
  public async resetPassword(userId: string, token: string, culture = 'en-US'): Promise<Result<any>> {
    return await this.get<Result<any>>(
      `/${this.controller}/ResetPassword?userId=${userId}&token=${encodeURIComponent(token)}&culture=${culture}`
    )
  }

  // Yêu cầu đặt lại mật khẩu
  public async requestPasswordReset(data: ForgotPasswordRequest): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/RequestPasswordReset`, data)
  }

  // Đổi mật khẩu
  public async changePassword(data: ChangePasswordRequest): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/ChangePassword`, data)
  }

  // Lấy thông tin user hiện tại
  public async getUserInfo(): Promise<IdentityResult> {
    return await this.get<IdentityResult>(`/${this.controller}/info`)
  }
}

