import { ApiClient } from '@/api/ApiClient'
import type { Result } from '@/types/Result'

export interface NotificationModel {
  id: string
  recipientId?: string | null
  title: string
  message?: string | null
  payload?: string | null
  type?: string | null
  channel?: number | null
  status?: number | null
  isRead?: boolean
  sentAt?: string | null
  deliveredAt?: string | null
  readAt?: string | null
  createdAt?: string
}

export interface NotificationPagedResult {
  items: NotificationModel[]
  total: number
}

export class NotificationApi extends ApiClient {
  controller = 'Notification'

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  async getNotifications(page = 1, pageSize = 10): Promise<Result<NotificationPagedResult>> {
    return await this.get<Result<NotificationPagedResult>>(`/${this.controller}/GetNotifications`, { params: { page, pageSize } })
  }

  async markAllRead(): Promise<Result<any>> {
    return await this.patch<Result<any>>(`/${this.controller}/read-all`)
  }
}
