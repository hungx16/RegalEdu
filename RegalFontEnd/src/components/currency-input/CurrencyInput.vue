<template>
    <el-input ref="inputRef" :model-value="displayValue" :disabled="disabled" inputmode="numeric" @input="onInput"
        @blur="onBlur" :placeholder="placeholder"
        :class="['ci', bordered ? 'ci--bordered' : 'ci--borderless', radiusClass]" :style="rootStyle"
        :input-style="inputInlineStyle" />
</template>

<script setup lang="ts">
import { computed, nextTick, ref, watch } from 'vue'
import type { ElInput } from 'element-plus'

/**
 * CurrencyInput.vue
 * - Luôn format số tiền theo locale/currency ngay khi gõ
 * - Element Plus + Vue 3 + TypeScript
 */

interface Props {
    modelValue: number | null | undefined
    disabled?: boolean
    locale?: string
    currency?: string

    /** Căn lề nội dung input: 'left' | 'right' | 'center' */
    align?: 'left' | 'right' | 'center'
    /** Hiển thị viền hay không */
    bordered?: boolean
    /** Màu nền của ô input (vd: '#F5F8FA', 'transparent') */
    background?: string
    /** Màu chữ (vd: '#111827') */
    textColor?: string
    /** Màu viền khi bordered=true (vd: '#E5E7EB') */
    borderColor?: string
    /** Độ bo góc: 'none' | 'sm' | 'md' | 'lg' | 'xl' */
    rounded?: 'none' | 'sm' | 'md' | 'lg' | 'xl'
    /** Placeholder hiển thị khi chưa có giá trị */
    placeholder?: string
}

const props = withDefaults(defineProps<Props>(), {
    disabled: false,
    locale: 'vi-VN',
    currency: 'VND',
    align: 'right',
    bordered: true,
    background: '',
    textColor: '',
    borderColor: '',
    rounded: 'md',
    placeholder: ''
})

const emit = defineEmits<{
    (e: 'update:modelValue', v: number): void
    (e: 'change', v: number): void
}>()

/** Giá trị số thô (integer) */
const raw = ref<number>(Number(props.modelValue ?? 0))

/** Đồng bộ khi prop thay đổi từ ngoài */
watch(
    () => props.modelValue,
    v => {
        raw.value = Number(v ?? 0)
    }
)

/** Intl.NumberFormat theo locale/currency; 0 chữ số thập phân */
const nf = computed(
    () =>
        new Intl.NumberFormat(props.locale, {
            style: 'currency',
            currency: props.currency,
            maximumFractionDigits: 0
        })
)

/** Luôn format khi hiển thị */
const displayValue = computed(() => nf.value.format(raw.value))

/** Style truyền thẳng vào input nội bộ của Element Plus */
const inputInlineStyle = computed(() => {
    return {
        textAlign: props.align,
        color: props.textColor || undefined
    } as Record<string, string>
})

/** Style gắn lên root để bơm biến CSS cho wrapper */
const rootStyle = computed(() => {
    return {
        '--ci-bg': props.background || 'transparent',
        '--ci-border': props.borderColor || 'var(--el-border-color)'
    } as Record<string, string>
})

/** Bo góc theo preset */
const radiusClass = computed(() => {
    switch (props.rounded) {
        case 'none':
            return 'ci--r-none'
        case 'sm':
            return 'ci--r-sm'
        case 'md':
            return 'ci--r-md'
        case 'lg':
            return 'ci--r-lg'
        case 'xl':
            return 'ci--r-xl'
    }
})

/** Ref tới ElInput để thao tác caret */
const inputRef = ref<InstanceType<typeof ElInput> | null>(null)

/**
 * Xử lý nhập:
 * - Lấy chữ số từ chuỗi đang gõ
 * - Cập nhật raw + phát ra update:modelValue
 * - nextTick: Element Plus render lại (đã format), đưa caret về cuối cho trải nghiệm gõ liên tục
 */
function onInput(val: string) {
    const digits = val.replace(/\D/g, '')
    const n = Number(digits || 0)

    raw.value = isNaN(n) ? 0 : n
    emit('update:modelValue', raw.value)

    nextTick(() => {
        const el = inputRef.value?.input as HTMLInputElement | undefined
        if (el) {
            const end = el.value.length
            el.setSelectionRange(end, end)
        }
    })
}

/** Blur: phát sự kiện change (nếu cần lắng nghe) */
function onBlur() {
    emit('change', raw.value)
}
</script>

<style scoped>
/* NỀN, VIỀN, BO GÓC của wrapper ElInput */
.ci :deep(.el-input__wrapper) {
    background-color: var(--ci-bg);
    transition: background-color 0.15s ease, box-shadow 0.15s ease, border-color 0.15s ease,
        border-radius 0.15s ease;
}

/* Bordered: dùng box-shadow của Element Plus như border */
.ci.ci--bordered :deep(.el-input__wrapper) {
    box-shadow: inset 0 0 0 1px var(--ci-border);
}

/* Borderless: bỏ viền */
.ci.ci--borderless :deep(.el-input__wrapper) {
    box-shadow: none !important;
    border: none !important;
}

/* Preset radius */
.ci.ci--r-none :deep(.el-input__wrapper) {
    border-radius: 0;
}

.ci.ci--r-sm :deep(.el-input__wrapper) {
    border-radius: 0.375rem;
}

/* 6px */
.ci.ci--r-md :deep(.el-input__wrapper) {
    border-radius: 0.5rem;
}

/* 8px */
.ci.ci--r-lg :deep(.el-input__wrapper) {
    border-radius: 0.75rem;
}

/* 12px */
.ci.ci--r-xl :deep(.el-input__wrapper) {
    border-radius: 1rem;
}

/* 16px */
</style>
