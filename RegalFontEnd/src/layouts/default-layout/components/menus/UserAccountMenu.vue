<template>
  <!--begin::Menu-->
  <div
    class="menu menu-sub menu-sub-dropdown menu-column menu-rounded menu-gray-600 menu-state-bg-light-primary fw-semibold py-4 fs-6 w-275px"
    data-kt-menu="true">
    <!--begin::Menu item (User head)-->
    <div class="menu-item px-3">
      <div class="menu-content d-flex align-items-center px-3">
        <div class="symbol symbol-50px me-5">
          <img :alt="userName" :src="avatarUrl" class="symbol symbol-35px symbol-circle" />
        </div>
        <div class="d-flex flex-column min-w-0">
          <div class="fw-bold d-flex align-items-center fs-5">
            <span class="text-truncate">{{ userName }}</span>
            <!-- <span v-if="userTier" class="badge badge-light-success fw-bold fs-8 px-2 py-1 ms-2">
              {{ userTier }}
            </span> -->
          </div>
          <span class="fw-semibold text-muted fs-7 text-truncate">
            {{ userEmail }}
          </span>
        </div>
      </div>
    </div>
    <!--end::Menu item-->

    <div class="separator my-2"></div>

    <!-- Profile -->
    <div class="menu-item px-5">
      <a href="#" class="menu-link px-5" @click.prevent="openProfileDialog">
        {{ t('menu.profile') }}
      </a>
    </div>

    <!-- Change password -->
    <div class="menu-item px-5">
      <a href="#" class="menu-link px-5" @click.prevent="openChangePasswordDialog">
        {{ t('menu.change_password') }}
      </a>
    </div>


    <!-- Account Settings -->
    <div class="menu-item px-5 my-1">
      <a href="#" class="menu-link px-5" @click.prevent="openAccountSettingsDialog">
        {{ t('menu.account_settings') }}
      </a>
    </div>
    <div class="separator my-2"></div>
    <!-- Sign out -->
    <div class="menu-item px-5">
      <a href="#" class="menu-link px-5 text-danger" @click.prevent="signOut">
        {{ t('menu.sign_out') }}
      </a>
    </div>
  </div>
  <!--end::Menu-->

  <!-- DIALOGS -->
  <ProfileDialog v-model="showProfile" :user="user" />
  <AccountSettingsDialog v-model="showAccountSettings" :user="user" />
  <ChangePasswordDialog v-model="showChangePassword" />

</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import { useI18n } from 'vue-i18n'
import ProfileDialog from '../account/ProfileDialog.vue'
import ChangePasswordDialog from '../account/ChangePasswordDialog.vue'
import AccountSettingsDialog from '../account/AccountSettingsDialog.vue'
import { makeAvatarSrc, readUserFromLS, type UserLS } from '@/utils/accountUser'
const { t } = useI18n()


/** ---- state user từ localStorage ---- */
const user = ref<UserLS | null>(readUserFromLS())
const userName = computed(() => user.value?.originalUserName || user.value?.userName || 'User')
const userEmail = computed(() => user.value?.userName || '')
const avatarUrl = computed(() => makeAvatarSrc(user.value?.avatarUrl))

/** cập nhật khi localStorage thay đổi (cross-tab) */
function handleStorage(e: StorageEvent) {
  if (e.key === 'userData') user.value = readUserFromLS()
}
onMounted(() => window.addEventListener('storage', handleStorage))
onBeforeUnmount(() => window.removeEventListener('storage', handleStorage))



/** Dialogs */
const showProfile = ref(false)
const showChangePassword = ref(false)
const showAccountSettings = ref(false)

const openProfileDialog = () => (showProfile.value = true)
const openChangePasswordDialog = () => (showChangePassword.value = true)
const openAccountSettingsDialog = () => (showAccountSettings.value = true)

/** Logout: xoá token + userData (không điều hướng nếu anh không muốn) */
const signOut = async () => {
  localStorage.removeItem('accessToken')
  localStorage.removeItem('refreshToken')
  localStorage.removeItem('userData')
  user.value = null
  window.location.href = '/sign-in'
}
</script>


<style scoped>
.menu-link.active {
  font-weight: 600;
}

/* Đảm bảo khung symbol đúng kích cỡ và cố định layout */
.symbol.symbol-50px {
  width: 50px;
  height: 50px;
  min-width: 50px;
}
</style>