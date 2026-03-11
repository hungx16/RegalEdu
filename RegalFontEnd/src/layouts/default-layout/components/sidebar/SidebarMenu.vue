<template>
  <!--begin::sidebar menu-->
  <div class="app-sidebar-menu overflow-hidden flex-column-fluid">
    <!--begin::Menu wrapper-->
    <div id="kt_app_sidebar_menu_wrapper" class="app-sidebar-wrapper hover-scroll-overlay-y my-5" data-kt-scroll="true"
      data-kt-scroll-activate="true" data-kt-scroll-height="auto"
      data-kt-scroll-dependencies="#kt_app_sidebar_logo, #kt_app_sidebar_footer"
      data-kt-scroll-wrappers="#kt_app_sidebar_menu" data-kt-scroll-offset="5px" data-kt-scroll-save-state="true">
      <!--begin::Menu-->
      <div id="#kt_app_sidebar_menu" class="menu menu-column menu-rounded menu-sub-indention px-3" data-kt-menu="true">
        <template v-for="(section, i) in MainMenuConfig" :key="i">

          <!-- ========= Group CÓ HEADING: dùng menu-accordion (icon KHÔNG mất khi thu nhỏ) ========= -->
          <template v-if="section.heading">
            <div class="menu-item menu-accordion" :class="{ show: isGroupExpanded(i) }">
              <span class="menu-link" role="button" tabindex="0" @click="toggleGroup(i)"
                @keydown.enter.prevent="toggleGroup(i)" @keydown.space.prevent="toggleGroup(i)">
                <span class="menu-icon me-2 ms-n2 d-flex align-items-center" style="line-height:0;">
                  <KTIcon v-if="section.keenthemesIcon" :icon-name="section.keenthemesIcon" icon-class="fs-2" />
                  <i v-else :class="[(section.bootstrapIcon || DEFAULT_BS_ICON), 'fs-4']"></i>
                </span>
                <span class="menu-title text-uppercase fw-bold">
                  {{ translate(section.heading) }}
                </span>
                <span class="menu-arrow"></span>
              </span>

              <!-- Children -->
              <div class="menu-sub menu-sub-accordion" :class="{ show: isGroupExpanded(i) }">
                <template v-for="(menuItem, j) in section.pages" :key="j">
                  <!-- Lá cấp 2 -->
                  <template v-if="menuItem.heading">
                    <div class="menu-item">
                      <router-link v-if="menuItem.route" class="menu-link" active-class="active" :to="menuItem.route">
                        <span class="menu-icon">
                          <KTIcon v-if="menuItem.keenthemesIcon" :icon-name="menuItem.keenthemesIcon"
                            icon-class="fs-2" />
                          <i v-else :class="[(menuItem.bootstrapIcon || DEFAULT_BS_ICON), 'fs-5']"></i>
                        </span>
                        <span class="menu-title">
                          {{ translate(menuItem.heading) }}
                        </span>
                      </router-link>
                    </div>
                  </template>

                  <!-- Accordion cấp 2 (sectionTitle -> sub) -->
                  <div v-if="menuItem.sectionTitle && menuItem.route"
                    :class="{ show: hasActiveChildren(menuItem.route) }" class="menu-item menu-accordion"
                    data-kt-menu-sub="accordion" data-kt-menu-trigger="click">
                    <span class="menu-link">
                      <span class="menu-icon">
                        <KTIcon v-if="menuItem.keenthemesIcon" :icon-name="menuItem.keenthemesIcon" icon-class="fs-2" />
                        <i v-else :class="[(menuItem.bootstrapIcon || DEFAULT_BS_ICON), 'fs-5']"></i>
                      </span>
                      <span class="menu-title">
                        {{ translate(menuItem.sectionTitle) }}
                      </span>
                      <span class="menu-arrow"></span>
                    </span>

                    <div :class="{ show: hasActiveChildren(menuItem.route) }" class="menu-sub menu-sub-accordion">
                      <template v-for="(item2, k) in menuItem.sub" :key="k">
                        <!-- Lá trong sub -->
                        <div v-if="item2.heading" class="menu-item">
                          <router-link v-if="item2.route" class="menu-link" active-class="active" :to="item2.route">
                            <span class="menu-icon">
                              <KTIcon v-if="item2.keenthemesIcon" :icon-name="item2.keenthemesIcon" icon-class="fs-2" />
                              <i v-else :class="[(item2.bootstrapIcon || DEFAULT_BS_ICON), 'fs-6']"></i>
                            </span>
                            <span class="menu-title">
                              {{ translate(item2.heading) }}
                            </span>
                          </router-link>
                        </div>

                        <!-- Accordion sâu hơn -->
                        <div v-if="item2.sectionTitle && item2.route" :class="{ show: hasActiveChildren(item2.route) }"
                          class="menu-item menu-accordion" data-kt-menu-sub="accordion" data-kt-menu-trigger="click">
                          <span class="menu-link">
                            <span class="menu-icon">
                              <KTIcon v-if="item2.keenthemesIcon" :icon-name="item2.keenthemesIcon" icon-class="fs-2" />
                              <i v-else :class="[(item2.bootstrapIcon || DEFAULT_BS_ICON), 'fs-6']"></i>
                            </span>
                            <span class="menu-title">
                              {{ translate(item2.sectionTitle) }}
                            </span>
                            <span class="menu-arrow"></span>
                          </span>

                          <div :class="{ show: hasActiveChildren(item2.route) }" class="menu-sub menu-sub-accordion">
                            <template v-for="(item3, k3) in item2.sub" :key="k3">
                              <!-- Lá cấp 3 -->
                              <div v-if="item3.heading" class="menu-item">
                                <router-link v-if="item3.route" class="menu-link" active-class="active"
                                  :to="item3.route">
                                  <span class="menu-icon">
                                    <KTIcon v-if="item3.keenthemesIcon" :icon-name="item3.keenthemesIcon"
                                      icon-class="fs-2" />
                                    <i v-else :class="[(item3.bootstrapIcon || DEFAULT_BS_ICON), 'fs-6']"></i>
                                  </span>
                                  <span class="menu-title">
                                    {{ translate(item3.heading) }}
                                  </span>
                                </router-link>
                              </div>
                            </template>
                          </div>
                        </div>
                      </template>
                    </div>
                  </div>
                </template>
              </div>
            </div>
          </template>

          <!-- ========= Group KHÔNG HEADING: (ví dụ Dashboard) hiển thị bình thường ========= -->
          <template v-else>
            <template v-for="(menuItem, j) in section.pages" :key="j">
              <template v-if="menuItem.heading">
                <div class="menu-item">
                  <router-link v-if="menuItem.route" class="menu-link" active-class="active" :to="menuItem.route">
                    <span class="menu-icon">
                      <KTIcon v-if="menuItem.keenthemesIcon" :icon-name="menuItem.keenthemesIcon" icon-class="fs-2" />
                      <i v-else :class="[(menuItem.bootstrapIcon || DEFAULT_BS_ICON), 'fs-5']"></i>
                    </span>
                    <span class="menu-title">
                      {{ translate(menuItem.heading) }}
                    </span>
                  </router-link>
                </div>
              </template>

              <!-- Accordion (nếu sectionTitle/sub tồn tại ở cấp không heading) -->
              <div v-if="menuItem.sectionTitle && menuItem.route" :class="{ show: hasActiveChildren(menuItem.route) }"
                class="menu-item menu-accordion" data-kt-menu-sub="accordion" data-kt-menu-trigger="click">
                <span class="menu-link">
                  <span class="menu-icon">
                    <KTIcon v-if="menuItem.keenthemesIcon" :icon-name="menuItem.keenthemesIcon" icon-class="fs-2" />
                    <i v-else :class="[(menuItem.bootstrapIcon || DEFAULT_BS_ICON), 'fs-5']"></i>
                  </span>
                  <span class="menu-title">
                    {{ translate(menuItem.sectionTitle) }}
                  </span>
                  <span class="menu-arrow"></span>
                </span>

                <div :class="{ show: hasActiveChildren(menuItem.route) }" class="menu-sub menu-sub-accordion">
                  <template v-for="(item2, k) in menuItem.sub" :key="k">
                    <div v-if="item2.heading" class="menu-item">
                      <router-link v-if="item2.route" class="menu-link" active-class="active" :to="item2.route">
                        <span class="menu-icon">
                          <KTIcon v-if="item2.keenthemesIcon" :icon-name="item2.keenthemesIcon" icon-class="fs-2" />
                          <i v-else :class="[(item2.bootstrapIcon || DEFAULT_BS_ICON), 'fs-6']"></i>
                        </span>
                        <span class="menu-title">
                          {{ translate(item2.heading) }}
                        </span>
                      </router-link>
                    </div>

                    <div v-if="item2.sectionTitle && item2.route" :class="{ show: hasActiveChildren(item2.route) }"
                      class="menu-item menu-accordion" data-kt-menu-sub="accordion" data-kt-menu-trigger="click">
                      <span class="menu-link">
                        <span class="menu-icon">
                          <KTIcon v-if="item2.keenthemesIcon" :icon-name="item2.keenthemesIcon" icon-class="fs-2" />
                          <i v-else :class="[(item2.bootstrapIcon || DEFAULT_BS_ICON), 'fs-6']"></i>
                        </span>
                        <span class="menu-title">
                          {{ translate(item2.sectionTitle) }}
                        </span>
                        <span class="menu-arrow"></span>
                      </span>

                      <div :class="{ show: hasActiveChildren(item2.route) }" class="menu-sub menu-sub-accordion">
                        <template v-for="(item3, k3) in item2.sub" :key="k3">
                          <div v-if="item3.heading" class="menu-item">
                            <router-link v-if="item3.route" class="menu-link" active-class="active" :to="item3.route">
                              <span class="menu-icon">
                                <KTIcon v-if="item3.keenthemesIcon" :icon-name="item3.keenthemesIcon"
                                  icon-class="fs-2" />
                                <i v-else :class="[(item3.bootstrapIcon || DEFAULT_BS_ICON), 'fs-6']"></i>
                              </span>
                              <span class="menu-title">
                                {{ translate(item3.heading) }}
                              </span>
                            </router-link>
                          </div>
                        </template>
                      </div>
                    </div>
                  </template>
                </div>
              </div>
            </template>
          </template>

        </template>
        <!--end:Menu item-->
      </div>
      <!--end::Menu-->
    </div>
    <!--end::Menu wrapper-->
  </div>
  <!--end::sidebar menu-->
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { computed, defineComponent, onBeforeMount, onMounted, ref, watch } from "vue";
import { useRoute } from "vue-router";
import MainMenuConfigRaw from "@/layouts/default-layout/config/MainMenuConfig";
import { sidebarMenuIcons } from "@/layouts/default-layout/config/helper";
import { useI18n } from "vue-i18n";
import { userPermissionStore } from "@/stores/permissionStore";

export default defineComponent({
  name: "sidebar-menu",
  components: {},
  setup() {
    const { t, te } = useI18n();
    const route = useRoute();
    const scrollElRef = ref<null | HTMLElement>(null);
    const isReady = ref(false);
    const permissionStore = userPermissionStore();

    // Fallback icon khi item không có icon
    const DEFAULT_BS_ICON = "bi-app-indicator";

    onBeforeMount(async () => {
      if (!permissionStore.getListMenuAccept?.length) {
        await permissionStore.loadResource();
      }
      isReady.value = true;
    });

    onMounted(() => {
      if (scrollElRef.value) scrollElRef.value.scrollTop = 0;
      expandGroupsByActiveRoute();
    });

    // ==== EXPAND/COLLAPSE GROUP MENU ====
    const expandedGroupIdx = ref<number[]>([0]); // mở mặc định group đầu

    function toggleGroup(idx: number) {
      if (expandedGroupIdx.value.includes(idx)) {
        expandedGroupIdx.value = expandedGroupIdx.value.filter((i) => i !== idx);
      } else {
        expandedGroupIdx.value.push(idx);
      }
    }
    function isGroupExpanded(idx: number) {
      return expandedGroupIdx.value.includes(idx);
    }

    const translate = (text: string) => (te(text) ? t(text) : text);

    const hasActiveChildren = (match: string) => {
      return route.path.indexOf(match) !== -1;
    };

    const filteredMenu = computed(() =>
      isReady.value
        ? filterMenuByPermission(MainMenuConfigRaw, permissionStore.getListMenuAccept || [])
        : []
    );

    // Auto expand group theo route active
    function expandGroupsByActiveRoute() {
      const groups = filteredMenu.value || [];
      const activePath = route.path;
      if (!groups.length) return;

      const openIdx: number[] = [];
      groups.forEach((section: any, idx: number) => {
        const hasActive =
          Array.isArray(section.pages) &&
          section.pages.some((mi: any) => {
            if (mi?.route && activePath.includes(`/${mi.route}`)) return true;
            if (Array.isArray(mi?.sub)) {
              if (mi.sub.some((leaf: any) => leaf?.route && activePath.includes(`/${leaf.route}`))) return true;
              return mi.sub.some((s2: any) =>
                Array.isArray(s2?.sub) &&
                s2.sub.some((leaf2: any) => leaf2?.route && activePath.includes(`/${leaf2.route}`))
              );
            }
            return false;
          });

        if (hasActive) openIdx.push(idx);
      });

      if (openIdx.length) expandedGroupIdx.value = openIdx;
    }

    watch(() => route.fullPath, expandGroupsByActiveRoute);

    return {
      // helpers
      hasActiveChildren,
      translate,
      getAssetPath,
      sidebarMenuIcons,
      DEFAULT_BS_ICON,
      // data
      MainMenuConfig: filteredMenu,
      expandedGroupIdx,
      toggleGroup,
      isGroupExpanded,
    };
  },
});

function filterMenuByPermission(menuConfig: any[], acceptList: string[]): any[] {
  function recursiveFilter(items: any[]): any[] {
    return items
      .map((item) => {
        if (item.sub) {
          const filteredSub = recursiveFilter(item.sub);
          if (filteredSub.length > 0) return { ...item, sub: filteredSub };
          return null;
        }
        if (item.route && item.route !== "dashboard" && item.route !== "test") {
          return acceptList.includes(item.route) ? item : null;
        }
        return item;
      })
      .filter(Boolean);
  }

  return menuConfig
    .map((section) => {
      if (!section.pages) return null;
      const filteredPages = recursiveFilter(section.pages);
      if (filteredPages.length > 0) return { ...section, pages: filteredPages };
      return null;
    })
    .filter(Boolean);
}
</script>

<style scoped>
/* Transition cho các nơi vẫn dùng expand nội bộ (nếu có) */
.expand-enter-active,
.expand-leave-active {
  transition: max-height 0.25s cubic-bezier(0.4, 0, 0.2, 1), opacity 0.2s;
}

.expand-enter-from,
.expand-leave-to {
  max-height: 0;
  opacity: 0;
}

.expand-enter-to,
.expand-leave-from {
  max-height: 500px;
  opacity: 1;
}

.cursor-pointer {
  cursor: pointer;
}
</style>
