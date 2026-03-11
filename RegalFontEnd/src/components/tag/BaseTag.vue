<!-- src/components/BaseTag.vue -->
<template>
    <span class="badge base-tag d-inline-flex align-items-center" :class="[sizeClass, shapeClass, colorClass]"
        :title="title || text" :tabindex="dismissible ? 0 : -1" @keydown.enter.prevent="onDismiss"
        @keydown.space.prevent="onDismiss">
        <i v-if="icon" :class="['me-1', icon]" aria-hidden="true"></i>
        <slot name="icon" />
        <span class="text-truncate">{{ text }}</span>

        <button v-if="dismissible" class="btn btn-sm btn-icon ms-2 p-0 border-0 bg-transparent text-reset"
            :aria-label="t('common.remove') || 'Remove'" @click.stop="onDismiss" type="button">
            <i :class="dismissIcon || 'bi bi-x-lg'"></i>
        </button>
    </span>
</template>

<script lang="ts" setup>
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { type Variant, variantToClass } from '@/utils/tags'

type Tone = 'light' | 'solid'
type Size = 'sm' | 'md' | 'lg'
type Shape = 'rounded' | 'pill'

const props = defineProps<{
    text: string
    title?: string
    icon?: string
    variant?: Variant
    tone?: Tone
    size?: Size
    shape?: Shape
    dismissible?: boolean
    dismissIcon?: string
}>()

const emit = defineEmits<{ (e: 'dismiss'): void }>()
const { t } = useI18n()

const colorClass = computed(() => variantToClass(props.variant ?? 'primary', props.tone ?? 'solid'))

const sizeClass = computed(() => {
    switch (props.size ?? 'md') {
        case 'sm': return 'px-2 py-0 fs-8 base-tag--compact'
        case 'lg': return 'px-4 py-2 fs-7'
        default: return 'px-3 py-1 fs-8'
    }
})

const shapeClass = computed(() => (props.shape === 'pill' ? 'rounded-pill' : 'rounded-2'))
function onDismiss() { if (props.dismissible) emit('dismiss') }
</script>

<style scoped>
.base-tag {
    max-width: 100%;
}

.base-tag .btn-icon i::before {
    vertical-align: middle;
}

.base-tag--compact {
    --tag-h: 26px;
    height: var(--tag-h);
    line-height: calc(var(--tag-h) - 2px);
    display: inline-flex;
    align-items: center;
    padding-top: 0 !important;
    padding-bottom: 0 !important;
    font-size: 12px;
}
</style>
