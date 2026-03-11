import type { AuthApi, LoginRequest, RegisterRequest, ChangePasswordRequest, ForgotPasswordRequest, VerifyTokenRequest } from '@/api/AuthApi'
import type { IdentityResult } from '@/api/AuthApi'

export class AuthService {
  private authApi: AuthApi

  constructor(authApiInstance: AuthApi) {
    this.authApi = authApiInstance
  }

  async login(payload: LoginRequest): Promise<any> {
    const result = await this.authApi.login(payload)
    console.log('Login result:', result)
    if (!result.succeeded) {
      throw new Error(result.errors || 'Login failed')
    }
    if (!result.data) {
      throw new Error('No data returned from login')
    }
    return result.data
  }

  async register(payload: RegisterRequest): Promise<void> {
    const result = await this.authApi.register(payload)
    if (!result.succeeded) {
      throw new Error(result.message || 'Register failed')
    }
  }

  async logout(): Promise<void> {
    await this.authApi.logout()
  }

  //   async refreshToken(payload: RefreshTokenRequest): Promise<IdentityResult> {
  //     const result = await this.authApi.refreshToken(payload)
  //     if (!result.succeeded) {
  //       throw new Error((result as any).message || 'Refresh token failed')
  //     }
  //     if (!('data' in result) || !result.data) {
  //       throw new Error('No data returned from refresh token')
  //     }
  //     return result.data
  //   }

  async verifyToken(payload: VerifyTokenRequest) {
    return await this.authApi.verifyToken(payload)
  }

  async forgotPassword(payload: ForgotPasswordRequest): Promise<void> {
    const result = await this.authApi.forgotPassword(payload)
    if (!result.succeeded) {
      throw new Error(result.message || 'Forgot password failed')
    }
  }

  async resetPassword(userId: string, token: string, culture = 'en-US') {
    const result = await this.authApi.resetPassword(userId, token, culture)
    if (!result.succeeded) {
      throw new Error(result.message || 'Reset password failed')
    }
    return result.data
  }

  async requestPasswordReset(payload: ForgotPasswordRequest): Promise<void> {
    const result = await this.authApi.requestPasswordReset(payload)
    if (!result.succeeded) {
      throw new Error(result.message || 'Request password reset failed')
    }
  }

  async changePassword(payload: ChangePasswordRequest): Promise<void> {
    const result = await this.authApi.changePassword(payload)
    if (!result.succeeded) {
      throw new Error(result.message || 'Change password failed')
    }
  }

  async getUserInfo(): Promise<IdentityResult> {
    return await this.authApi.getUserInfo()
  }
}