<template>
  <div id="kt_app_content" class="app-content flex-column-fluid ">
    <!--begin::Content container-->
    <div id="kt_app_content_container" class="app-container pt-0" :class="{
      'container-fluid': contentWidthFluid,
      'container-xxl': !contentWidthFluid,
    }">
      <div class="sticky-topbar-tab" v-if="isMobile">
        <TabsHeader />
      </div>
      <router-view v-slot="{ Component, route }">
        <keep-alive>
          <component :is="Component" :key="route.fullPath" />
        </keep-alive>
      </router-view>
    </div>
    <!--end::Content container-->
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted, onUnmounted } from "vue";
import { contentWidthFluid } from "@/layouts/default-layout/config/helper";
import { RouterView } from "vue-router";
import TabsHeader from "@/components/tab-header/TabsHeader.vue";

export default defineComponent({
  name: "default-layout-content",
  components: {
    RouterView,
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
      contentWidthFluid,
      isMobile,
    };
  },
});
</script>
<style scoped>
.sticky-topbar-tab {
  position: sticky;
  top: 60px;
  z-index: 10;
  background: var(--bs-app-blank-bg-color, #fff);

  height: 60px;
  /* line phân tách */
}
</style>
