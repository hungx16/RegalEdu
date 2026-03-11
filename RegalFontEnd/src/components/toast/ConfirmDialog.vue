<!-- filepath: src/components/ConfirmDialog.vue -->
<template>
    <transition name="toast-slide">
        <div v-if="confirm.show" class="confirm-toast custom-alert">
            <div class="d-flex align-items-center">
                <i class="bi bi-question-circle-fill fs-2x me-4 text-info"></i>
                <div class="flex-grow-1 text-start">
                    <div class="fw-bold fs-5 mb-1">
                        {{ t(confirm.message.key, confirm.message.params ?? {}) }}
                    </div>
                    <div class="confirm-actions mt-3">
                        <button class="btn btn-danger me-2" @click="notificationStore.confirmNo">{{ t('common.cancel')
                            }}</button>
                        <button class="btn btn-success" @click="notificationStore.confirmYes">{{ t('common.confirm')
                            }}</button>
                    </div>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">
import { useNotificationStore } from '@/stores/notificationStore'
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const notificationStore = useNotificationStore()
const confirm = computed(() => notificationStore.confirm)
const { t } = useI18n()
</script>

<style scoped>
.confirm-toast {
    position: fixed;
    top: 50%;
    left: 50%;
    z-index: 20001;
    transform: translate(-50%, -50%);
    min-width: 300px;
    max-width: 700px;
    border: 1px solid #93d2f7;
    background: #f4f8fa;
    color: #212529;
    border-radius: 10px;
    font-size: 1rem;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
    padding: 24px 32px;
    display: flex;
    align-items: center;
    animation: toast-slide-in 0.5s cubic-bezier(.25, .8, .25, 1) both;
    /* //box-shadow: 4px 4px rgba(238, 141, 141, 0.1); */
    box-shadow: 2px 5px 10px rgba(245, 141, 141, 0.5);

}

.confirm-actions {
    display: flex;
    justify-content: center;
    gap: 9px;
    margin-top: 8px;
}

.confirm-actions .btn {
    padding: 2px 9px !important;
    font-size: 0.95rem;
    min-width: 80px;
    min-height: 36px;
    border-radius: 6px;
}

@keyframes toast-slide-in {
    from {
        opacity: 0;
        transform: translate(-50%, -60%) scale(0.95);
    }

    to {
        opacity: 1;
        transform: translate(-50%, -50%) scale(1);
    }
}

.toast-slide-enter-active,
.toast-slide-leave-active {
    transition: all 0.5s cubic-bezier(.25, .8, .25, 1);
}

.toast-slide-enter-from {
    opacity: 0;
    transform: translate(-50%, -60%) scale(0.95);
}

.toast-slide-enter-to {
    opacity: 1;
    transform: translate(-50%, -50%) scale(1);
}

.toast-slide-leave-from {
    opacity: 1;
    transform: translate(-50%, -50%) scale(1);
}

.toast-slide-leave-to {
    opacity: 0;
    transform: translate(-50%, -60%) scale(0.95);
}
</style>