import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from '@microsoft/signalr'
import { useNotificationStore, type ToastType } from '@/stores/notificationStore'

interface NotificationHubPayload {
  recipientId?: string | null
  title: string
  message?: string | null
  payload?: string | null
  type?: string | null
  channel?: number | null
}

const DEFAULT_HUB_ROUTE = '/hubs/notifications'

const TOAST_TYPE_MAP: Record<string, ToastType> = {
  success: 'success',
  error: 'error',
  danger: 'error',
  warning: 'warning',
  warn: 'warning',
  info: 'info',
}

class NotificationHubService {
  private connection: HubConnection | null = null
  private currentToken: string | null = null

  private resolveHubUrl(): string {
    // Highest priority: provide a full hub URL via env and skip all guessing.
    const absoluteHubUrl = import.meta.env.VITE_APP_NOTIFICATION_HUB_URL as string | undefined
    if (absoluteHubUrl && absoluteHubUrl.startsWith('http')) {
      return absoluteHubUrl.replace(/\/$/, '')
    }

    let baseUrl =
      (import.meta.env.VITE_APP_BASE_SERVER_URL ??
        import.meta.env.VITE_APP_API_URL ??
        window.location.origin
      ).replace(/\/$/, '')

    const route =
      (import.meta.env.VITE_APP_NOTIFICATION_HUB_PATH as string | undefined) ?? DEFAULT_HUB_ROUTE

    // If API url contains /api/* but hub lives at root (/hubs/notifications), strip /api/* for hub only.
    try {
      const parsed = new URL(baseUrl)
      if (parsed.pathname?.startsWith('/api') && route.startsWith('/hubs/')) {
        parsed.pathname = ''
        parsed.search = ''
        parsed.hash = ''
        baseUrl = parsed.toString().replace(/\/$/, '')
      }
    } catch {
      // keep baseUrl as-is
    }

    // Allow relative or absolute route
    if (route.startsWith('http')) {
      return route.replace(/\/$/, '')
    }

    const normalizedRoute = route.startsWith('/') ? route : `/${route}`
    return `${baseUrl}${normalizedRoute}`
  }

  private resolveToastType(type?: string | null): ToastType {
    if (!type) {
      return 'info'
    }
    const normalized = type.toLowerCase()
    return TOAST_TYPE_MAP[normalized] ?? 'info'
  }

  private emitToast(payload: NotificationHubPayload) {
    if (!payload || !payload.title) {
      return
    }

    const notificationStore = useNotificationStore()
    const line = payload.message ? `${payload.title}\n${payload.message}` : payload.title
    notificationStore.showToast(this.resolveToastType(payload.type ?? null), line)
    notificationStore.addRealtimeNotification({
      id: crypto.randomUUID(),
      title: payload.title,
      message: payload.message,
      type: payload.type ?? undefined,
      isRead: false,
      createdAt: new Date().toISOString(),
    })
  }

  private handlePayload(payload: any) {
    if (!payload) return
    const normalized: NotificationHubPayload = {
      title: payload.title ?? payload?.payload?.title,
      message: payload.message ?? payload?.payload?.message,
      type: payload.type ?? payload?.payload?.type,
      recipientId: payload.recipientId ?? payload?.payload?.recipientId,
      channel: payload.channel ?? payload?.payload?.channel,
      payload: payload.payload ?? payload?.payload?.payload,
    }
    this.emitToast(normalized)
  }

  public async connect(token: string) {
    if (!token?.trim()) {
      return
    }

    if (
      this.connection &&
      this.currentToken === token &&
      this.connection.state === HubConnectionState.Connected
    ) {
      return
    }

    await this.disconnect()

    const hubUrl = this.resolveHubUrl()
    this.connection = new HubConnectionBuilder()
      .withUrl(hubUrl, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    this.connection.on('ReceiveNotification', (payload: NotificationHubPayload) => {
      console.info('[NotificationHub] ReceiveNotification', payload)
      this.handlePayload(payload)
    })
    // Fallback: surface any other method name toasts (if backend uses a different method)
    const connAny = this.connection as any
    if (typeof connAny.onAny === 'function') {
      connAny.onAny((methodName: string, ...args: any[]) => {
        console.debug('[NotificationHub] onAny', methodName, args)
        if (methodName === 'ReceiveNotification') return
        if (args?.length) this.handlePayload(args[0])
      })
    }

    this.connection.onclose(() => {
      this.currentToken = null
    })

    try {
      await this.connection.start()
      console.info('[NotificationHub] connected', hubUrl)
      this.currentToken = token
    } catch (error) {
      console.error('[NotificationHub] failed to connect', error)
    }
  }

  public async disconnect() {
    if (!this.connection) {
      return
    }

    if (
      this.connection.state === HubConnectionState.Connected ||
      this.connection.state === HubConnectionState.Connecting
    ) {
      try {
        await this.connection.stop()
        console.info('[NotificationHub] disconnected')
      } catch (error) {
        console.warn('[NotificationHub] disconnect failed', error)
      }
    }

    this.connection.off('ReceiveNotification')
    this.connection = null
    this.currentToken = null
  }
}

export default new NotificationHubService()
