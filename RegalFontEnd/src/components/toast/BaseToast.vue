<template>
  <div class="toast-container">
    <transition-group name="toast-slide" tag="div">
      <div v-for="(toast, idx) in toasts.filter(t => t.show)" :key="toast.id" class="custom-alert toast-item"
        :class="toastClass(toast)" :style="{
          animationDelay: (idx * 0.15) + 's',
          '--toast-leave-delay': (idx * 0.15) + 's'
        }">
        <div class="toast-content">
          <div class="toast-body">
            <i :class="iconClass(toast)" class="toast-icon"></i>
            <div class="toast-text">
              <div class="fw-bold fs-5 mb-1">
                {{ t(`common.${toast.type}`) }}
              </div>
              <div class="fs-6 toast-message-content">
                <template v-if="Array.isArray(toast.message)">
                  <div v-for="(msg, i) in toast.message" :key="i">
                    {{ formatToastMessage(msg) }}
                  </div>
                </template>
                <template v-else>
                  {{ formatToastMessage(toast.message) }}
                </template>
              </div>
            </div>
          </div>
          <button class="toast-close" @click="notificationStore.hideToast(toast.id)">
            <i class="bi bi-x-lg"></i>

          </button>
        </div>
      </div>
    </transition-group>
  </div>
</template>

<script setup lang="ts">
import { useNotificationStore, type ToastContent } from '@/stores/notificationStore'
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const notificationStore = useNotificationStore()
const toasts = computed(() => notificationStore.toasts)

function formatToastMessage(message: ToastContent | null | undefined) {
  if (!message) {
    return ''
  }

  if (typeof message === 'string') {
    return message
  }

  return t(message.key, message.params ?? {})
}

function toastClass(toast: any) {
  switch (toast.type) {
    case 'success':
      return 'custom-alert-success'
    case 'error':
      return 'custom-alert-error'
    case 'warning':
      return 'custom-alert-warning'
    case 'info':
      return 'custom-alert-info'
    default:
      return 'custom-alert-success'
  }
}

function iconClass(toast: any) {
  switch (toast.type) {
    case 'success':
      return 'bi bi-check-circle-fill text-success'
    case 'error':
      return 'bi bi-x-circle-fill text-danger'
    case 'warning':
      return 'bi bi-exclamation-triangle-fill text-warning'
    case 'info':
      return 'bi bi-info-circle-fill text-info'
    default:
      return 'bi bi-bell-fill text-primary'
  }
}
</script>

<style scoped>
.toast-message-content {
  white-space: pre-line;
}

.toast-container {
  position: fixed;
  top: 24px;
  right: 24px;
  z-index: 9999;
  display: flex;
  flex-direction: column;
  gap: 16px;
  align-items: flex-end;
}

.toast-item {
  display: flex;
  flex-direction: column;
}

.toast-content {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  width: 100%;
}

.toast-body {
  display: flex;
  align-items: flex-start;
  flex: 1;
}

.toast-icon {
  font-size: 2rem;
  margin-right: 1.25rem;
  margin-top: 2px;
  flex-shrink: 0;
  line-height: 1;
}

.toast-text {
  flex: 1;
  min-width: 0;
  word-break: break-word;
}

.toast-close {
  background: none;
  border: none;
  color: #999;
  font-size: 1.5rem;
  padding: 0 0 0 10px;
  align-self: flex-start;
  cursor: pointer;
  display: flex;
  align-items: center;
  height: 32px;
  transition: color 0.18s;
  z-index: 10;
  pointer-events: auto;
}

.toast-close:hover,
.toast-close:focus {
  color: #f1416c;
}

.custom-alert {
  min-width: 340px;
  max-width: 420px;
  border: 1px solid #54d48a;
  background: #eafff3;
  color: #212529;
  border-radius: 12px;
  font-size: 1rem;
  box-shadow: 0 2px 18px rgba(52, 71, 103, 0.12);
  padding: 18px 28px;
  transition: box-shadow 0.18s;
  position: relative;
}

.custom-alert-success {
  border-color: #54d48a;
  background: #eafff3;
  color: #212529;
}

.custom-alert-error {
  border-color: #f1416c;
  background: #fff0f6;
  color: #f1416c;
}

.custom-alert-warning {
  border-color: #ffc700;
  background: #fffbe6;
  color: #664d03;
}

.custom-alert-info {
  border-color: #009ef7;
  background: #e3f6ff;
  color: #009ef7;
}

.fw-bold {
  font-weight: bold;
}

.fs-5 {
  font-size: 1.15rem;
}

.fs-6 {
  font-size: 0.98rem;
}

.mb-1 {
  margin-bottom: 0.3rem;
}

/* Animation toast slide */
.toast-slide-enter-active,
.toast-slide-leave-active {
  transition: opacity 0.42s cubic-bezier(.25, .8, .25, 1), transform 0.42s cubic-bezier(.25, .8, .25, 1);
}

.toast-slide-enter-from {
  opacity: 0;
  transform: translateX(90px) scale(0.98);
}

.toast-slide-enter-to {
  opacity: 1;
  transform: translateX(0) scale(1);
}

.toast-slide-leave-active {
  transition-delay: var(--toast-leave-delay, 0s);
}

.toast-slide-leave-from {
  opacity: 1;
  transform: translateX(0) scale(1);
}

.toast-slide-leave-to {
  opacity: 0;
  transform: translateX(90px) scale(0.98);
}

/* Responsive */
@media (max-width: 600px) {
  .toast-container {
    top: 8px;
    left: 0;
    right: 0;
    width: 100vw;
    padding: 0 4px;
    align-items: center;
  }

  .custom-alert {
    width: 96vw;
    max-width: 96vw;
    padding: 12px 16px;
    font-size: 0.95rem;
  }

  .toast-icon {
    font-size: 1.3rem;
    margin-right: 0.6rem;
  }

  .toast-close {
    font-size: 1.2rem;
    height: 26px;
    padding-left: 5px;
  }

  .fs-5 {
    font-size: 1.02rem;
  }

  .fs-6 {
    font-size: 0.9rem;
  }
}
</style>
