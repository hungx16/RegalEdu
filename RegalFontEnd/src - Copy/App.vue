<template>
  <BaseToast />
  <ConfirmDialog />
  <el-config-provider :locale="currentElLocale">
    <RouterView />
  </el-config-provider>
</template>

<script lang="ts">
import {
  computed,
  defineComponent,
  nextTick,
  onBeforeMount,
  onMounted,
  onBeforeUnmount,
  watch,
} from "vue";
import { RouterView } from "vue-router";
import { useConfigStore } from "@/stores/config";
import { useThemeStore } from "@/stores/theme";
import { useBodyStore } from "@/stores/body";
import { themeConfigValue } from "@/layouts/default-layout/config/helper";
import { initializeComponents } from "@/core/plugins/keenthemes";
import BaseToast from "@/components/toast/BaseToast.vue";
import ConfirmDialog from "@/components/toast/ConfirmDialog.vue";
import { useLocaleStore } from "./stores/localeStore";
import en from 'element-plus/dist/locale/en.mjs'
import vi from 'element-plus/dist/locale/vi.mjs'
import { useAuthStore } from "./stores/useAuthStore";
import { userPermissionStore } from "./stores/permissionStore";
import notificationHubService from "@/services/NotificationHubService";
export default defineComponent({
  name: "app",
  components: {
    RouterView,
    BaseToast,
    ConfirmDialog,
  },
  setup() {
    const configStore = useConfigStore();
    const themeStore = useThemeStore();
    const bodyStore = useBodyStore();
    const authStore = useAuthStore()
    const permissionStore = userPermissionStore()
    const localeStore = useLocaleStore()
    const currentElLocale = computed(() => {
      return localeStore.currentLocale === 'vi' ? vi : en
    })
    onBeforeMount(async () => {

      if (authStore.token && !permissionStore.hasLoaded) {
        try {
          await permissionStore.loadResource()
        } catch (err) {
          console.error('Lỗi load quyền', err)
        }
      }
      /**
       * Overrides the layout config using saved data from localStorage
       * remove this to use static config (@/layouts/default-layout/config/DefaultLayoutConfig.ts)
       */
      configStore.overrideLayoutConfig();

      /**
       *  Sets a mode from configuration
       */
      themeStore.setThemeMode(themeConfigValue.value);
    });

    watch(
      () => authStore.token,
      (token) => {
        if (token) {
          notificationHubService.connect(token).catch((error) => {
            console.error('[NotificationHub] connect error', error)
          })
        } else {
          notificationHubService.disconnect()
        }
      },
      { immediate: true }
    )

    onBeforeUnmount(() => {
      notificationHubService.disconnect()
    })

    onMounted(() => {
      nextTick(() => {
        initializeComponents();

        bodyStore.removeBodyClassName("page-loading");
      });
    });

    return {
      currentElLocale
    };
  },
});
</script>

<style lang="scss">
@import "bootstrap-icons/font/bootstrap-icons.css";
@import "apexcharts/dist/apexcharts.css";
@import "quill/dist/quill.snow.css";
@import "animate.css";
@import "sweetalert2/dist/sweetalert2.css";
@import "nouislider/dist/nouislider.css";
@import "@fortawesome/fontawesome-free/css/all.min.css";
@import "socicon/css/socicon.css";
@import "line-awesome/dist/line-awesome/css/line-awesome.css";
@import "dropzone/dist/dropzone.css";
@import "@vueform/multiselect/themes/default.css";
@import "prism-themes/themes/prism-shades-of-purple.css";
@import "element-plus/dist/index.css";
@import "@/assets/sass/overrides/_sidebar-theme.scss";

// Main demo style scss
@import "assets/keenicons/duotone/style.css";
@import "assets/keenicons/outline/style.css";
@import "assets/keenicons/solid/style.css";
@import "assets/sass/element-ui.dark";
@import "assets/sass/plugins";
@import "assets/sass/style";

#app {
  display: contents;
}
</style>
