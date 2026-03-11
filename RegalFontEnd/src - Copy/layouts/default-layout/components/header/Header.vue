<template>
  <!--begin::Header-->
  <div v-if="headerDisplay" id="kt_app_header" class="app-header" data-kt-sticky="true"
    data-kt-sticky-activate="{default: true, lg: true}" data-kt-sticky-name="app-header-minimize"
    data-kt-sticky-offset="{default: '200px', lg: '0'}" data-kt-sticky-animation="false">
    <!--begin::Header container-->
    <div class="app-container d-flex align-items-stretch justify-content-between" :class="{
      'container-fluid': headerWidthFluid,
      'container-xxl': !headerWidthFluid,
    }">
      <div v-if="layout === 'light-header' || layout === 'dark-header'"
        class="d-flex align-items-center flex-grow-1 flex-lg-grow-0 me-lg-15">
        <router-link to="/">
          <img v-if="themeMode === 'light' && layout === 'light-header'" alt="Logo"
            :src="getAssetPath('media/logos/default.svg')"
            class="h-20px h-lg-30px app-sidebar-logo-default theme-light-show" />
          <img v-if="
            layout === 'dark-header' ||
            (themeMode === 'dark' && layout === 'light-header')
          " alt="Logo" :src="getAssetPath('media/logos/default-dark.svg')"
            class="h-20px h-lg-30px app-sidebar-logo-default" />
        </router-link>
      </div>
      <template v-else>
        <!--begin::sidebar mobile toggle-->
        <div class="d-flex align-items-center d-lg-none ms-n3 me-1 me-md-2">
          <div class="btn btn-icon btn-active-color-primary w-35px h-35px" id="kt_app_sidebar_mobile_toggle">
            <KTIcon icon-name="abstract-14" icon-class="fs-2 fs-md-1" />
          </div>
        </div>
        <!--end::sidebar mobile toggle-->
        <!--begin::Mobile logo-->
        <div class="d-flex align-items-center flex-grow-1 flex-lg-grow-0">
          <router-link to="/" class="d-lg-none">
            <!-- <img
              alt="Logo"
              :src="getAssetPath('media/logos/default-small.svg')"
              class="h-30px"
            /> -->
            <div>
              <el-icon size="30" class="logo-icon-wrapper">
                <HomeFilled />
              </el-icon>
            </div>

          </router-link>
        </div>
        <!--end::Mobile logo-->
      </template>
      <!--begin::Header wrapper-->
      <div class="d-flex align-items-stretch flex-lg-grow-1 justify-content-between" id="kt_app_header_wrapper">
        <TabsHeader v-if="!isMobile" />
        <KTHeaderNavbar />
      </div>
      <!--end::Header wrapper-->
    </div>
    <!--end::Header container-->
  </div>
  <!--end::Header-->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { defineComponent, ref, onMounted, onUnmounted } from "vue";
import KTHeaderMenu from "@/layouts/default-layout/components/header/menu/Menu.vue";
import KTHeaderNavbar from "@/layouts/default-layout/components/header/Navbar.vue";
import { HomeFilled } from "@element-plus/icons-vue";
import TabsHeader from "@/components/tab-header/TabsHeader.vue";

import {
  headerDisplay,
  headerWidthFluid,
  layout,
  themeMode,
  headerDesktopFixed,
  headerMobileFixed,

} from "@/layouts/default-layout/config/helper";

export default defineComponent({
  name: "layout-header",
  components: {
    KTHeaderMenu,
    KTHeaderNavbar,
    HomeFilled,
    TabsHeader,
  },
  setup() {
    const isMobile = ref(window.innerWidth <= 768);
    const handleResize = () => {
      isMobile.value = window.innerWidth <= 768;
    };
    onMounted(() => window.addEventListener("resize", handleResize));
    onUnmounted(() => window.removeEventListener("resize", handleResize));
    return {
      layout,
      headerWidthFluid,
      headerDisplay,
      themeMode,
      getAssetPath,
      headerDesktopFixed,
      headerMobileFixed,
      isMobile,
    };
  },
});
</script>
<style scoped lang="scss">
.logo-icon-wrapper {
  background-color: var(--kt-menu-active-bg);
  padding: 6px;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--kt-menu-text-color-active);
}

#kt_app_header_wrapper {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.flex-grow-1 {
  flex: 1 1 0%;
  min-width: 0;
}

.tabs-header-container {
  flex: 0 1 70vw; // Chỉ cho TabsHeader tối đa 70% chiều ngang
  max-width: 70vw;
  min-width: 0;
}

.flex-shrink-0 {
  flex-shrink: 0;
}
</style>