<template>
  <div class="tab-bar sticky-tabs" :class="[{ dark: themeMode === 'dark' }, { mobile: isMobile }]">
    <div class="tab-bar-inner">
      <div v-for="tab in tabStore.openedTabs" :key="tab.route"
        :class="['tab-item', { active: tab.route === tabStore.activeTab }]" @click="activate(tab.route)"
        :tabindex="tab.route === tabStore.activeTab ? -1 : 0">
        {{ t('models.' + capitalizeFirst(tab.title)) }}
        <span v-if="tab.route !== '/dashboard'" class="close-btn" @click.stop="close(tab.route)">
          <ElIcon>
            <Close />
          </ElIcon>
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { useTabStore } from '@/stores/tabStore'
import { useThemeStore } from '@/stores/theme'
import { useI18n } from 'vue-i18n'
import { ElIcon } from 'element-plus'
import { Close } from '@element-plus/icons-vue'
import { capitalizeFirst } from '@/utils/format'
const themeStore = useThemeStore()
const themeMode = computed(() => themeStore.mode)

const router = useRouter()
const tabStore = useTabStore()
const { t } = useI18n()

import { ref } from 'vue'

const isMobile = ref(window.innerWidth <= 768)
window.addEventListener('resize', () => {
  isMobile.value = window.innerWidth <= 768
})

async function activate(route: string) {
  await nextTick() // đảm bảo tabStore cập nhật xong
  if (route !== router.currentRoute.value.fullPath) {
    await router.push(route)
  }
  tabStore.setActiveTab(route)
}
function close(route: string) {
  const wasActive = route === tabStore.activeTab
  tabStore.closeTab(route)
  if (wasActive) {
    // Tìm tab còn lại gần nhất bên trái, nếu không có thì lấy tab đầu
    const tabs = tabStore.openedTabs
    if (tabs.length > 0) {
      router.push(tabs[tabs.length - 1].route)
      tabStore.setActiveTab(tabs[tabs.length - 1].route)
    } else {
      router.push('/dashboard')
      tabStore.setActiveTab('/dashboard')
    }
  }
}

</script>

<style scoped>
.tab-header-row {
  display: flex;
  align-items: center;
  width: 80%;
}

.tab-bar {
  display: flex;
  overflow-x: auto;
  background: var(--kt-menu-bg, #fff);
}

/* Thêm màu nền cho mobile */
.tab-bar.mobile {
  position: fixed;
  left: 0;
  right: 0;
  top: 60px;
  /* hoặc top: 0 nếu không có header/topbar */
  width: 100vw;
  min-width: 100vw;
  max-width: 100vw;
  background: var(--kt-menu-bg, #ffe4b5) !important;
  margin-left: 0;
  padding-top: 10px;
  padding-bottom: 5px;
  height: 60px;
  z-index: 100;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
}

.tab-bar::-webkit-scrollbar {
  height: 6px;
}

.tab-bar::-webkit-scrollbar-thumb {
  background: #e0e0e0;
  border-radius: 3px;
}

.tab-bar-inner {
  display: flex;
  min-width: max-content;
  padding-left: 5px;
  padding-right: 5px;
}

.tab-bar.dark {
  background: var(--kt-menu-bg, #2a2d3a);
}

.tab-bar.dark .tab-item {
  background: #2a2d3a;
  border-color: #3e404f;
  color: #c9d1d9;
}

.tab-bar.dark .tab-item.active {
  background: #1e1e2d;
  border-color: #5294e2;
  color: #ffffff;
}


.tab-item {
  padding: 8px 12px;
  margin-right: 4px;
  cursor: pointer;
  background: var(--kt-body-bg, #ffffff);
  border: 1px solid #ddd;
  border-bottom: none;
  border-top-left-radius: 8px;
  border-top-right-radius: 8px;
  /* transition: border-color 0.2s, background 0.2s, color 0.2s; */
  color: #004085;
  position: relative;
  user-select: none;
  transition: all 0.2s;
}

.tab-item.active {
  background: var(--kt-body-bg, #ffffff);
  border-top: 2px solid #4b8cfb;
  border-bottom: none;
  font-weight: 600;
  color: #003e9c;
  position: relative;
  z-index: 2;
}

.tab-item:not(.active):hover {
  background: #d4e2fa;
  color: #2152d9;
}

.close-btn {
  font-size: 16px;
  margin-left: 6px;
  margin-top: 2px;
  color: #a7a7a7;
  border-radius: 50%;
  background: transparent;
  font-size: 0.875rem;
  width: 15px;
  height: 15px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: background 0.15s, color 0.2s;
  flex-shrink: 0;
  /* Không cho nút bị co lại */
}

.close-btn:hover {
  color: #fff;
  background: #f88b8b;
}

.close-btn svg {
  display: block;
}

.sticky-tabs {
  position: sticky;
  top: 60px;
  /* hoặc top: 0 nếu không có topbar */
  z-index: 1;
  padding-top: 10px;
  padding-bottom: 20px;
  height: 70px;
}

/* Mobile chiếm toàn bộ màn hình */
@media (max-width: 768px) {
  .tab-bar {
    flex: 1 1 100vw;
    max-width: 100vw;
    min-width: 100vw;
  }
}
</style>
