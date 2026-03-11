<template>
  <!--begin::Navbar-->
  <div class="app-navbar flex-shrink-0">
    <!--begin::Search-->
    <!-- <div class="app-navbar-item align-items-stretch ms-1 ms-md-4">
      <KTSearch />
    </div> -->
    <!--end::Search-->

    <!--begin::Activities-->
    <!-- <div class="app-navbar-item ms-1 ms-md-4">
      <div class="btn btn-icon btn-custom btn-icon-muted btn-active-light btn-active-color-primary w-35px h-35px"
        id="kt_activities_toggle">
        <KTIcon icon-name="messages" icon-class="fs-2" />
      </div>
    </div> -->
    <!--end::Activities-->

    <!--begin::Notifications-->
    <div class="app-navbar-item ms-1 ms-md-4">
      <div
        class="btn btn-icon btn-custom btn-icon-muted btn-active-light btn-active-color-primary w-35px h-35px position-relative"
        data-kt-menu-trigger="{default: 'click', lg: 'hover'}" data-kt-menu-attach="parent"
        data-kt-menu-placement="bottom-end" id="kt_menu_item_notifications">
        <KTIcon icon-name="notification-status" icon-class="fs-2" />
        <span v-if="unreadCount > 0"
          class="badge badge-circle bg-danger position-absolute top-0 end-0 translate-middle fs-8">
          {{ unreadCount }}
        </span>
      </div>
      <div class="menu menu-sub menu-sub-dropdown menu-column w-325px" data-kt-menu="true">
        <div class="px-6 py-5 border-bottom d-flex align-items-center justify-content-between">
          <div>
            <div class="fw-bold fs-5">{{ t('common.notifications') }}</div>
            <div class="text-muted fs-8">{{ unreadCount }} {{ t('common.unread') }}</div>
          </div>
          <button class="btn btn-sm btn-light-primary" type="button" @click.stop="markAllRead"
            :disabled="unreadCount === 0">
            {{ t('common.markAllRead') }}
          </button>
        </div>
        <div class="scroll-y mh-300px">
          <div v-if="notifications.length === 0" class="px-6 py-5 text-muted fs-6">
            {{ t('common.noData') }}
          </div>
          <div v-else v-for="item in notifications" :key="item.id" class="px-6 py-4 border-bottom">
            <div class="d-flex align-items-start">
              <div class="symbol symbol-35px me-3">
                <span class="symbol-label bg-light">
                  <KTIcon icon-name="notification-status" icon-class="fs-3" />
                </span>
              </div>
              <div class="flex-grow-1">
                <div class="fw-semibold" :class="{ 'text-muted': item.isRead }">
                  {{ item.title }}
                </div>
                <div class="text-gray-600 fs-7 text-break" v-if="item.message">
                  {{ item.message }}
                </div>
                <div class="text-gray-500 fs-8 mt-1" v-if="item.createdAt">
                  {{ formatTime(item.createdAt) }}
                </div>
              </div>
              <span v-if="!item.isRead" class="badge badge-dot bg-success ms-2"></span>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!--end::Notifications-->

    <!--begin::Chat-->
    <!-- <div class="app-navbar-item ms-1 ms-md-4">
      <div
        class="btn btn-icon btn-custom btn-icon-muted btn-active-light btn-active-color-primary w-35px h-35px position-relative"
        id="kt_drawer_chat_toggle">
        <KTIcon icon-name="message-text-2" icon-class="fs-2" />
        <span
          class="bullet bullet-dot bg-success h-6px w-6px position-absolute translate-middle top-0 start-50 animation-blink"></span>
      </div>
    </div> -->
    <!--end::Chat-->

    <!--begin::Theme mode-->
    <div class="app-navbar-item ms-1 ms-md-3">
      <button type="button" @click="toggleThemeMode"
        class="btn btn-icon btn-custom btn-icon-muted btn-active-light btn-active-color-primary w-30px h-30px w-md-40px h-md-40px">
        <KTIcon v-if="themeMode === 'light'" icon-name="night-day" icon-class="fs-2" />
        <KTIcon v-else icon-name="moon" icon-class="fs-2" />
      </button>
      <KTThemeModeSwitcher />
    </div>
    <!--end::Theme mode-->

    <!--begin::Language Switch-->
    <div class="app-navbar-item ms-1 ms-md-3">
      <button type="button" @click="toggleLanguage"
        class="btn btn-icon btn-custom btn-icon-muted btn-active-light btn-active-color-primary w-35px h-35px w-md-40px h-md-40px"
        :title="locale === 'vi' ? 'Switch to English' : 'Chuyển sang Tiếng Việt'">
        <img v-if="locale === 'vi'" :src="getAssetPath('media/flags/vietnam.svg')" alt="VI"
          class="h-20px w-25px object-fit-cover rounded-1" />
        <img v-else :src="getAssetPath('media/flags/united-kingdom.svg')" alt="EN"
          class="h-20px w-25px object-fit-cover rounded-1" />
      </button>
    </div>
    <!--end::Language Switch-->

    <!--begin::User menu-->
    <div class="app-navbar-item ms-1 ms-md-4" id="kt_header_user_menu_toggle">
      <div class="cursor-pointer symbol symbol-35px" data-kt-menu-trigger="{default: 'click', lg: 'hover'}"
        data-kt-menu-attach="parent" data-kt-menu-placement="bottom-end">
        <img :alt="userName" :src="avatarUrl" class="symbol symbol-35px symbol-circle" />
      </div>
      <KTUserMenu />
    </div>
    <!--end::User menu-->

    <!--begin::Header menu toggle-->
    <div class="app-navbar-item d-lg-none ms-2 me-n2">
      <div class="btn btn-flex btn-icon btn-active-color-primary w-30px h-30px" id="kt_app_header_menu_toggle">
        <KTIcon icon-name="element-4" icon-class="fs-2" />
      </div>
    </div>
    <!--end::Header menu toggle-->
  </div>
  <!--end::Navbar-->
</template>

<script setup lang="ts">
import { computed, onMounted, onBeforeUnmount, ref } from 'vue'
import { useI18n } from 'vue-i18n'
// import KTSearch from '@/layouts/default-layout/components/search/Search.vue'
// import KTNotificationMenu from '@/layouts/default-layout/components/menus/NotificationsMenu.vue'
import KTUserMenu from '@/layouts/default-layout/components/menus/UserAccountMenu.vue'
import KTThemeModeSwitcher from '@/layouts/default-layout/components/theme-mode/ThemeModeSwitcher.vue'
import { ThemeModeComponent } from '@/assets/ts/layout'
import { useThemeStore } from '@/stores/theme'
import { getAssetPath } from '@/core/helpers/assets'
import { makeAvatarSrc, readUserFromLS, type UserLS } from '@/utils/accountUser'
import { useNotificationStore } from '@/stores/notificationStore'
import { NotificationApi } from '@/api/NotificationApi'

// đặt tên component (devtools)
defineOptions({ name: 'header-navbar' })

/** Theme */
const themeStore = useThemeStore()
const themeMode = computed(() => (themeStore.mode === 'system' ? ThemeModeComponent.getSystemMode() : themeStore.mode))
const toggleThemeMode = () => {
  const next = themeMode.value === 'light' ? 'dark' : 'light'
  themeStore.setThemeMode(next)
  ThemeModeComponent.setMode(next, '')
}

/** i18n */
const { locale, t } = useI18n()
const toggleLanguage = () => {
  locale.value = locale.value === 'vi' ? 'en' : 'vi'
  localStorage.setItem('lang', locale.value)
}

/** User from localStorage (reactive) */
const user = ref<UserLS | null>(readUserFromLS())
const userName = computed(() => user.value?.originalUserName || user.value?.userName || 'User')
const avatarUrl = computed(() => makeAvatarSrc(user.value?.avatarUrl))

/** Notifications */
const notificationStore = useNotificationStore()
const notifications = computed(() => notificationStore.notifications)
const unreadCount = computed(() => notificationStore.unreadCount)
const notificationApi = new NotificationApi()

function formatTime(value?: string) {
  if (!value) return ''
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString()
}

async function loadNotifications() {
  try {
    const res = await notificationApi.getNotifications(1, 10)
    const items = res.data?.items ?? []
    notificationStore.setNotifications(
      items.map(n => ({
        id: n.id,
        title: n.title,
        message: n.message,
        type: n.type ?? undefined,
        isRead: n.isRead ?? false,
        createdAt: n.sentAt ?? n.createdAt,
      }))
    )
  } catch (err) {
    console.error('[Notification] load failed', err)
  }
}

const markAllRead = async () => {
  try {
    if (unreadCount.value === 0) return
    await notificationApi.markAllRead()
    notificationStore.markAllRead()
  } catch (err) {
    console.error('[Notification] mark all read failed', err)
  }
}

// cập nhật khi localStorage thay đổi (cross-tab + cùng tab nhờ custom event)
function handleStorage(e: StorageEvent) {
  if (!e.key || e.key === 'userData') {
    user.value = readUserFromLS()
  }
}
function handleUserUpdated() {
  user.value = readUserFromLS()
}

onMounted(() => {
  window.addEventListener('storage', handleStorage)
  window.addEventListener('userData:updated', handleUserUpdated as EventListener)
  loadNotifications()
})
onBeforeUnmount(() => {
  window.removeEventListener('storage', handleStorage)
  window.removeEventListener('userData:updated', handleUserUpdated as EventListener)
})
</script>

<style scoped>
/* đảm bảo wrapper đúng kích thước Metronic */
.symbol.symbol-35px {
  width: 35px;
  height: 35px;
  min-width: 35px;
}
</style>
