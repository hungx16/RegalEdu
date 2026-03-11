import { defineStore } from 'pinia'
import { AuthService } from '@/services/AuthService'
import { AuthApi, type LoginRequest, type RegisterRequest, type ChangePasswordRequest, type ForgotPasswordRequest, type RefreshTokenRequest, type VerifyTokenRequest } from '@/api/AuthApi'
import type { IdentityResult } from '@/api/AuthApi'
import { userPermissionStore } from './permissionStore'

const authService = new AuthService(new AuthApi())

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null as IdentityResult | null,
    token: localStorage.getItem('accessToken') || '',
    refreshToken: localStorage.getItem('refreshToken') || '',
    error: null as string | null,
  }),
  actions: {
    async login(payload: LoginRequest) {
      this.error = null
      try {
        const data = await authService.login(payload)
        this.user = data
        this.token = data.accessToken
        this.refreshToken = data.refreshToken
        localStorage.setItem('accessToken', this.token)
        localStorage.setItem('refreshToken', this.refreshToken)
        localStorage.setItem('userData', JSON.stringify(data))
      } catch (err: any) {
        this.error = err.message || 'Login failed'
      }
    },
    async logout() {
      try {
        await authService.logout()
      } catch { }
      this.user = null
      this.token = ''
      this.refreshToken = ''
      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
      const permissionStore = userPermissionStore();
      permissionStore.reset();
    },
    async register(payload: RegisterRequest) {
      this.error = null
      try {
        await authService.register(payload)
      } catch (err: any) {
        this.error = err.message || 'Register failed'
      }
    },
    async changePassword(payload: ChangePasswordRequest) {
      this.error = null
      try {
        await authService.changePassword(payload)
      } catch (err: any) {
        this.error = err.message || 'Change password failed'
      }
    },
    async forgotPassword(payload: ForgotPasswordRequest) {
      this.error = null
      try {
        await authService.forgotPassword(payload)
      } catch (err: any) {
        this.error = err.message || 'Forgot password failed'
      }
    },
    // async refreshTokenAction(payload: RefreshTokenRequest) {
    //   this.loading = true
    //   this.error = null
    //   try {
    //     const result = await authService.refreshToken(payload)
    //     this.token = result.accessToken
    //     this.refreshToken = result.refreshToken
    //     localStorage.setItem('accessToken', this.token)
    //     localStorage.setItem('refreshToken', this.refreshToken)
    //   } catch (err: any) {
    //     this.error = err.message || 'Refresh token failed'
    //   } finally {
    //     this.loading = false
    //   }
    // },
    async getUserInfo() {
      this.error = null
      try {
        const user = await authService.getUserInfo()
        this.user = user
      } catch (err: any) {
        this.error = err.message || 'Get user info failed'
      }
    },

  }
})
