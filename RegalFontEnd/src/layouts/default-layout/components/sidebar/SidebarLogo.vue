<template>
  <!--begin::Sidebar Logo-->
  <div class="app-sidebar-logo" id="kt_app_sidebar_logo">
    <router-link to="/" class="flex items-center gap-3 no-underline">
      <div class="app-sidebar-logo px-3 d-flex align-items-center gap-3">
        <!-- Icon -->
        <div class="logo-icon-wrapper">
          <img :src="getAssetPath('media/logos/Logo.png')" alt="Logo" class="logo-img" />
        </div>

        <!-- Text -->
        <div class="logo-text-container" v-show="(!isMinimized && !isMobile) || isHovering">
          <div class="logo-title">{{ t('app.name') }}</div>
          <div class="logo-slogan">{{ t('app.slogan') }}</div>
        </div>
      </div>
    </router-link>

    <!-- Sidebar Toggle -->
    <div v-if="sidebarToggleDisplay" ref="toggleRef" id="kt_app_sidebar_toggle"
      class="app-sidebar-toggle btn btn-icon btn-shadow btn-sm btn-color-muted btn-active-color-primary h-30px w-30px position-absolute top-50 start-100 translate-middle rotate"
      data-kt-toggle="true" data-kt-toggle-state="active" data-kt-toggle-target="body"
      data-kt-toggle-name="app-sidebar-minimize">
      <KTIcon icon-name="black-left-line" icon-class="fs-3 rotate-180 ms-1" />
    </div>
  </div>
  <!--end::Sidebar Logo-->
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, nextTick } from "vue";
import { ToggleComponent } from "@/assets/ts/components";
import { useI18n } from "vue-i18n";
import { sidebarToggleDisplay } from "@/layouts/default-layout/config/helper";
import { useRoute } from "vue-router";
import { getAssetPath } from "@/core/helpers/assets";
const { t } = useI18n();

interface IProps {
  sidebarRef: HTMLElement | null;
}
const props = defineProps<IProps>();
const toggleRef = ref<HTMLDivElement | null>(null);

/** =========================
 *  State & constants
 *  ========================= */
const isMinimized = ref(true);
const isMobile = ref(window.innerWidth <= 991.98);
const isHovering = ref(false);
const isReDirecting = ref(false);

const BODY_MINIMIZE_CLASS = "app-sidebar-minimize";

/** Debug logging */
const DEBUG = true;
const log = (...args: any[]) => {
  if (!DEBUG) return;
  // eslint-disable-next-line no-console
  console.log("[SIDEBAR-LOGO]", ...args);
};

/** Route guard cản observer khi chuyển tab/route (trong một khoảng ngắn) */
let routeGuardActive = false;
let routeGuardTimer: number | null = null;

/** tránh double-flip khi chính tay mình/Metronic flip */
let suppressObserver = false;
/** mốc observer để cleanup */
let bodyObserver: MutationObserver | null = null;

/** =========================
 *  Helpers
 *  ========================= */
const checkMinimized = (src: string) => {
  // đảo trạng thái theo toggle/observer (không dùng ở route)
  isMinimized.value = !isMinimized.value;
  log(`checkMinimized(${src}) -> isMinimized:`, isMinimized.value);
};

const handleResize = () => {
  isMobile.value = window.innerWidth <= 991.98;
  log("resize -> isMobile:", isMobile.value);
};

const bindSidebarHover = () => {
  const el = document.getElementById("kt_app_sidebar");
  if (!el) return;
  const enter = () => (isHovering.value = true);
  const leave = () => (isHovering.value = false);
  el.addEventListener("mouseenter", enter);
  el.addEventListener("mouseleave", leave);
  (el as any).__enter = enter;
  (el as any).__leave = leave;
};
const unbindSidebarHover = () => {
  const el = document.getElementById("kt_app_sidebar");
  if (!el) return;
  const enter = (el as any).__enter;
  const leave = (el as any).__leave;
  if (enter) el.removeEventListener("mouseenter", enter);
  if (leave) el.removeEventListener("mouseleave", leave);
  delete (el as any).__enter;
  delete (el as any).__leave;
};

const route = useRoute();

/** =========================
 *  Lifecycle
 *  ========================= */
onMounted(() => {
  window.addEventListener("resize", handleResize, { passive: true });
  isHovering.value = false;

  // Đồng bộ mốc ban đầu từ body (KHÔNG flip)
  isMinimized.value = document.body.classList.contains(BODY_MINIMIZE_CLASS);
  log("mounted -> initial isMinimized from body:", isMinimized.value);

  bindSidebarHover();

  // Metronic toggle -> tự flip, chặn observer 1 frame để không flip lần 2
  setTimeout(() => {
    const toggleObj = ToggleComponent.getInstance(toggleRef.value!) as ToggleComponent | null;
    if (toggleObj) {
      toggleObj.on("kt.toggle.change", () => {
        suppressObserver = true;
        log("toggle-click detected");

        // chỉ flip ở đây (nguồn: toggle); không để observer flip lại
        checkMinimized("toggle");

        // hiệu ứng nhỏ cho sidebarRef (nếu có)
        props.sidebarRef?.classList.add("animating");
        setTimeout(() => props.sidebarRef?.classList.remove("animating"), 300);

        // nhả cờ chặn ở frame kế
        requestAnimationFrame(() => (suppressObserver = false));
      });
      log("toggle instance bound");
    } else {
      log("toggle instance NOT found");
    }
  }, 1);

  // Observer: chỉ flip khi class app-sidebar-minimize THỰC SỰ đổi và không bị guard
  bodyObserver = new MutationObserver((mutations) => {
    if (suppressObserver) {
      log("observer: suppressed");
      return;
    }
    if (routeGuardActive) {
      log("observer: ignored due to routeGuardActive");
      return;
    }

    for (const m of mutations) {
      const mr = m as MutationRecord & { oldValue?: string };
      const prevHas =
        !!mr.oldValue &&
        new RegExp(`(?:^|\\s)${BODY_MINIMIZE_CLASS}(?:\\s|$)`).test(mr.oldValue);
      const nowHas = document.body.classList.contains(BODY_MINIMIZE_CLASS);

      if (prevHas !== nowHas) {
        log("observer: body class changed", { prevHas, nowHas, currentIsMin: isMinimized.value });
        // flip theo body (nguồn: observer)
        checkMinimized("observer");
      }
    }
  });

  bodyObserver.observe(document.body, {
    attributes: true,
    attributeFilter: ["class"],
    attributeOldValue: true,
  });
  log("observer bound");
});

onUnmounted(() => {
  window.removeEventListener("resize", handleResize);
  unbindSidebarHover();
  if (bodyObserver) {
    bodyObserver.disconnect();
    bodyObserver = null;
  }
  if (routeGuardTimer) {
    window.clearTimeout(routeGuardTimer);
    routeGuardTimer = null;
  }
});

/** =========================
 *  Watchers
 *  ========================= */

// Log mọi lần thay đổi isMinimized
watch(isMinimized, (val, oldVal) => {
  log("isMinimized changed:", { oldVal, newVal: val });
});

// Khi đổi route/tab:
// - KHÔNG flip (không gọi checkMinimized)
// - Nếu đang ở trạng thái thu nhỏ (isMinimized = true), GIỮ NGUYÊN (lock).
// - Nếu đang ở trạng thái mở (isMinimized = false), chỉ đồng bộ *nhẹ* từ body (nếu khác) để không lệch.
watch(
  () => route.fullPath,
  async (newPath, oldPath) => {
    log("route change:", { from: oldPath, to: newPath });

    // bật route guard để observer bỏ qua những dao động class do Metronic khi đổi tab
    routeGuardActive = true;
    if (routeGuardTimer) {
      window.clearTimeout(routeGuardTimer);
      routeGuardTimer = null;
    }
    // Thời gian guard ngắn là đủ (Metronic toggle/redraw)
    routeGuardTimer = window.setTimeout(() => {
      routeGuardActive = false;
      log("routeGuard deactivated");
    }, 350);

    await nextTick();

    const bodyHas = document.body.classList.contains(BODY_MINIMIZE_CLASS);

    if (isMinimized.value) {
      // ĐANG THU NHỎ → KHÓA, KHÔNG đổi
      log("route-sync: LOCKED (isMinimized true). Keep current state. bodyHas:", bodyHas);
      // Không assign isMinimized ở đây
    } else {
      // ĐANG MỞ → đồng bộ nhẹ cho nhất quán
      if (isMinimized.value !== bodyHas) {
        isMinimized.value = bodyHas;
        log("route-sync: synced from body because sidebar is expanded", { isMinimized: isMinimized.value });
      } else {
        log("route-sync: no change needed (already in sync)");
      }
    }
  }
);
</script>

<style scoped lang="scss">
/* chỉ style cho logo */

.logo-icon-wrapper {
  background-color: var(--kt-menu-active-bg);
  padding: 6px;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--kt-menu-text-color-active);
}

.logo-text-container {
  display: flex;
  flex-direction: column;
  line-height: 1.2;
  transition: all 0.3s ease;
}

.logo-title {
  font-size: 1rem;
  font-weight: 700;
  color: var(--kt-menu-text-title);
}

.logo-slogan {
  font-size: 0.8rem;
  color: var(--kt-menu-text-color);
}

/* Dark mode */
:root[data-theme="dark"] .logo-title {
  color: #ffffff;
}

:root[data-theme="dark"] .logo-slogan {
  color: rgba(255, 255, 255, 0.6);
}

/* ẨN CHỮ KHI THU NHỎ (phạm vi hẹp chỉ trong logo) */
:global(body.app-sidebar-minimize) #kt_app_sidebar_logo :deep(.logo-text-container) {
  display: none !important;
}

/* THU NHỎ + HOVER SIDEBAR -> HIỆN LẠI CHỮ */
:global(body.app-sidebar-minimize.app-sidebar-hover) #kt_app_sidebar_logo :deep(.logo-text-container) {
  display: flex !important;
}

/* Giữ wrapper kích thước vuông (32x32) */
.logo-icon-wrapper {
  background-color: transparent;
  padding: 0;
  border-radius: 0.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  min-width: 32px;
  min-height: 32px;
}

/* Ảnh logo */
.logo-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  /* dùng 'contain' nếu muốn lọt gọn */
  border-radius: 0.5rem;
  display: block;
}
</style>
