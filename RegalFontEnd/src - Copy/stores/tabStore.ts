import { defineStore } from 'pinia'
import { useRouter } from 'vue-router'
import { useNotificationStore } from './notificationStore'

export interface TabItem {
  name: string
  route: string
  title: string
}

const MAX_TABS = 10
const STORAGE_KEY = 'tab-store'

export const useTabStore = defineStore('tabStore', {
  state: () => {
    const saved = JSON.parse(localStorage.getItem(STORAGE_KEY) || 'null')
    return {
      openedTabs: saved?.openedTabs || [
        { name: 'dashboard', route: '/dashboard', title: 'Dashboard' }
      ],
      activeTab: saved?.activeTab || '/dashboard'
    }
  },

  actions: {
    async openTab(tab: TabItem): Promise<boolean> {
      const router = useRouter()

      // Nếu vượt quá số lượng tab và tab chưa tồn tại
      if (this.openedTabs.length >= MAX_TABS && !this.openedTabs.find(t => t.route === tab.route)) {
        const notificationStore = useNotificationStore()
        notificationStore.showToast('warning', { key: 'toast.maxTab', params: { model: MAX_TABS } })
        return false
      }

      // Nếu tab chưa có thì thêm
      if (!this.openedTabs.find(t => t.route === tab.route)) {
        this.openedTabs.push(tab)
      }

      // Cập nhật tab đang hoạt động
      this.activeTab = tab.route
      this.saveState()

      // ⚡ Điều hướng luôn đến route mới nếu khác route hiện tại
      if (router.currentRoute.value.fullPath !== tab.route) {
        await router.push(tab.route)
      }

      return true
    },

    closeTab(route: string) {
      const index = this.openedTabs.findIndex(t => t.route === route)
      if (index !== -1) {
        this.openedTabs.splice(index, 1)
        if (this.activeTab === route) {
          const next = this.openedTabs[index] || this.openedTabs[index - 1]
          this.activeTab = next ? next.route : '/dashboard'
        }
      }
      this.saveState()
    },

    setActiveTab(route: string) {
      this.activeTab = route
      this.saveState()
    },

    saveState() {
      localStorage.setItem(
        STORAGE_KEY,
        JSON.stringify({
          openedTabs: this.openedTabs,
          activeTab: this.activeTab
        })
      )
    }
  }
})
