import { defineStore } from 'pinia'

export type ToastType = 'success' | 'error' | 'warning' | 'info'
export type ToastContent = { key: string, params?: Record<string, any> } | string
export type ToastMessage = ToastContent | ToastContent[]

export interface NotificationItem {
  id: string
  title: string
  message?: string | null
  type?: string | null
  isRead?: boolean
  createdAt?: string
}

interface Toast {
  id: number
  show: boolean
  message: ToastMessage
  type: ToastType
  timer?: ReturnType<typeof setTimeout> | null
}

interface ConfirmDialog {
  show: boolean
  message: { key: string, params?: Record<string, any> }
  onConfirm?: () => void
  onCancel?: () => void
}

export const useNotificationStore = defineStore('notification', {
  state: () => ({
    toasts: [] as Toast[],
    toastId: 0,
    notifications: [] as NotificationItem[],
    unreadCount: 0,
    confirm: {
      show: false,
      message: { key: '', params: {} },
      onConfirm: undefined,
      onCancel: undefined,
    } as ConfirmDialog,
  }),
  actions: {
    showToast(type: ToastType, message: ToastMessage) {
      const id = ++this.toastId
      const toast: Toast = { id, show: true, type, message, timer: null }
      toast.timer = setTimeout(() => {
        this.hideToast(id)
      }, 6000)
      this.toasts.push(toast) // Thêm vào cuối mảng (dưới cùng)
    },
    hideToast(id: number) {
      console.log('[Toast] Hide', id)
      const toast = this.toasts.find(t => t.id === id)
      if (toast) {
        toast.show = false
        if (toast.timer) {
          clearTimeout(toast.timer)
          toast.timer = null
        }
      }
    },
    removeToast(id: number) {
      this.toasts = this.toasts.filter(t => t.id !== id)
    },
    setNotifications(items: NotificationItem[]) {
      this.notifications = items
      this.unreadCount = items.filter(n => !n.isRead).length
    },
    addRealtimeNotification(item: NotificationItem) {
      this.notifications = [item, ...this.notifications].slice(0, 20)
      if (!item.isRead) this.unreadCount += 1
    },
    markAllRead() {
      this.notifications = this.notifications.map(n => ({ ...n, isRead: true }))
      this.unreadCount = 0
    },
    showConfirm(
      message: { key: string, params?: Record<string, any> },
      onConfirm?: () => void,
      onCancel?: () => void
    ) {
      this.confirm = {
        show: true,
        message,
        onConfirm,
        onCancel,
      }
    },
    hideConfirm() {
      this.confirm.show = false
    },
    confirmYes() {
      if (this.confirm.onConfirm) this.confirm.onConfirm()
      this.hideConfirm()
    },
    confirmNo() {
      if (this.confirm.onCancel) this.confirm.onCancel()
      this.hideConfirm()
    },
  }
})
