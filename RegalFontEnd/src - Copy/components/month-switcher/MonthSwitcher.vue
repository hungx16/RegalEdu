<template>
    <div class="month-switcher d-flex align-items-center gap-3 mb-3">
        <el-button size="default" @click="prevMonth" plain>
            <i class="bi bi-chevron-left me-1"></i>
            {{ t('monthSwitcher.prevMonth') }}
        </el-button>
        <div class="current-month fw-semibold fs-5 d-flex align-items-center">
            <i class="bi bi-calendar2 me-2"></i>
            <span>{{ t('monthSwitcher.month') }} {{ monthDisplay }}</span>
        </div>
        <el-button size="default" type="primary" @click="goToCurrent" :disabled="isCurrentMonth">
            {{ t('monthSwitcher.current') }}
        </el-button>
        <el-button size="default" @click="nextMonth" plain>
            {{ t('monthSwitcher.nextMonth') }}
            <i class="bi bi-chevron-right ms-1"></i>
        </el-button>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'

// Props và Emit cho v-model
const props = defineProps<{ modelValue: Date }>()
const emit = defineEmits(['update:modelValue'])

const { t } = useI18n()

// State local (lấy theo props)
const selectedDate = ref(new Date(props.modelValue))

// Sync với parent khi prop thay đổi
watch(() => props.modelValue, (val) => {
    if (val) selectedDate.value = new Date(val)
})

// Hiển thị tháng/năm (VD: 07 2025)
const monthDisplay = computed(() => {
    const y = selectedDate.value.getFullYear()
    const m = (selectedDate.value.getMonth() + 1).toString().padStart(2, '0')
    return `${m} ${y}`
})

// Check nút "Hiện tại"
const isCurrentMonth = computed(() => {
    const now = new Date()
    return (
        now.getFullYear() === selectedDate.value.getFullYear() &&
        now.getMonth() === selectedDate.value.getMonth()
    )
})

// Sự kiện điều hướng
function prevMonth() {
    const d = new Date(selectedDate.value)
    d.setMonth(d.getMonth() - 1)
    selectedDate.value = d
    emit('update:modelValue', new Date(d))
}
function nextMonth() {
    const d = new Date(selectedDate.value)
    d.setMonth(d.getMonth() + 1)
    selectedDate.value = d
    emit('update:modelValue', new Date(d))
}
function goToCurrent() {
    const now = new Date()
    selectedDate.value = now
    emit('update:modelValue', new Date(now))
}
</script>

<style scoped>
.month-switcher {
    gap: 12px;
}

.current-month {
    min-width: 170px;
    justify-content: center;
}

.month-switcher .el-button {
    border-radius: 10px !important;
}
</style>
