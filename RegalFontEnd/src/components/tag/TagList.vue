<!-- src/components/TagList.vue -->
<template>
    <div class="d-flex flex-wrap gap-2 align-items-start">
        <!-- Tags hiển thị -->
        <BaseTag v-for="(t, i) in visibleItems" :key="`tag-${i}-${t}`" :text="t" :variant="itemVariant(t)" tone="light"
            size="sm" shape="pill" :dismissible="props.dismissible ?? true" :dismissIcon="dismissIcon"
            @dismiss="removeVisible(i)" />

        <!-- +N gom vào popover -->
        <el-popover v-if="hiddenCount > 0 && collapseToPopover" :placement="popoverPlacement" trigger="hover"
            :width="popoverWidth">
            <template #reference>
                <BaseTag :text="`+${hiddenCount}`" :variant="moreVariant" tone="solid" size="sm" shape="pill" />
            </template>
            <div class="d-flex flex-wrap gap-2">
                <BaseTag v-for="(t, j) in hiddenItems" :key="`hidden-${j}-${t}`" :text="t" :variant="itemVariant(t)"
                    tone="solid" size="sm" shape="pill" :dismissible="false" />
            </div>
        </el-popover>

        <!-- Nút + -->
        <el-popover v-if="showAddButton" ref="addPopRef" placement="bottom-start" trigger="click" :width="popoverWidth">
            <template #reference>
                <button class="btn-add-tag rounded-pill d-inline-flex align-items-center" type="button"
                    :disabled="!canAdd" @click.stop>
                    <i class="bi bi-plus-lg me-1"></i>{{ addButtonText }}
                </button>

            </template>

            <div class="p-2">
                <el-autocomplete v-model="input" class="w-100" :fetch-suggestions="querySearch"
                    :placeholder="placeholder" :disabled="!canAdd" @keyup.enter="confirmAdd" @select="onSelect"
                    clearable />
                <div class="d-flex justify-content-end gap-2 mt-2">
                    <el-button size="small" @click="closePopover">{{ t('common.cancel') || 'Cancel' }}</el-button>
                    <el-button size="small" type="primary" :disabled="!canAdd" @click="confirmAdd">
                        {{ t('common.add') || 'Add' }}
                    </el-button>
                </div>
                <div v-if="dupWarning" class="text-warning fs-8 mt-2">
                    <i class="bi bi-exclamation-triangle me-1"></i>{{ dupWarning }}
                </div>
                <div v-if="limitWarning" class="text-danger fs-8 mt-2">
                    <i class="bi bi-exclamation-octagon me-1"></i>{{ limitWarning }}
                </div>
            </div>
        </el-popover>
    </div>
</template>

<script lang="ts" setup>
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseTag from './BaseTag.vue'
import {
    splitTags, joinTags, isStringMode, tagToVariant,
    type Variant, TAG_PALETTE, stableHash
} from '@/utils/tags'

const props = defineProps<{
    /** Chuỗi 'abc#$#xyz' hoặc mảng ['abc','xyz'] */
    modelValue: string | string[]
    delimiter?: string
    suggestions?: string[]
    /** Giới hạn tổng số tag */
    maxTags?: number
    /** Cho phép xóa */
    dismissible?: boolean
    /** Gom tag dư vào popover +N */
    collapseToPopover?: boolean
    /** Số tag hiển thị trước khi gom */
    maxVisible?: number

    /* UI popover & nút + */
    dismissIcon?: string
    addButtonText?: string
    placeholder?: string
    popoverPlacement?: 'top' | 'bottom' | 'left' | 'right' | 'top-start' | 'top-end' | 'bottom-start' | 'bottom-end'
    popoverWidth?: number
    /** Màu của thẻ +N */
    moreVariant?: Variant

    /** Tự tô màu theo text (default: true) */
    autoColor?: boolean
    /** Ẩn nút + khi đạt giới hạn (default: true) */
    hideAddWhenLimit?: boolean,
    distinctColors?: boolean

}>()


const emit = defineEmits<{
    (e: 'update:modelValue', v: string | string[]): void
    (e: 'added', text: string): void
    (e: 'removed', text: string): void
    (e: 'blocked', reason: 'limit' | 'duplicate'): void
}>()
const dismissible = computed(() => props.dismissible ?? true)

const { t } = useI18n()
const delimiter = computed(() => props.delimiter ?? '#$#')
const maxTags = computed(() => props.maxTags ?? Infinity)
const maxVisible = computed(() => Math.max(0, props.maxVisible ?? 5))
const collapseToPopover = computed(() => props.collapseToPopover ?? true)
const dismissIcon = computed(() => props.dismissIcon ?? 'bi bi-x-lg')
const addButtonText = computed(() => props.addButtonText ?? (t('common.addTag') || 'Add tag'))
const placeholder = computed(() => props.placeholder ?? (t('common.enterTag') || 'Enter tag'))
const popoverPlacement = computed(() => props.popoverPlacement ?? 'bottom-start')
const popoverWidth = computed(() => props.popoverWidth ?? 280)
const moreVariant = computed<Variant>(() => props.moreVariant ?? 'secondary')
const autoColor = computed(() => props.autoColor !== false)
const hideAddWhenLimit = computed(() => props.hideAddWhenLimit !== false)

const stringMode = ref<boolean>(isStringMode(props.modelValue))
const items = ref<string[]>(splitTags(props.modelValue, delimiter.value))

watch(() => props.modelValue, (v) => {
    stringMode.value = isStringMode(v)
    items.value = splitTags(v, delimiter.value)
})

const visibleItems = computed(() => items.value.slice(0, maxVisible.value))
const hiddenItems = computed(() => items.value.slice(maxVisible.value))
const hiddenCount = computed(() => Math.max(0, items.value.length - maxVisible.value))
const canAdd = computed(() => items.value.length < maxTags.value)
const showAddButton = computed(() => (hideAddWhenLimit.value ? canAdd.value : true))

function push(arr: string[]) {
    emit('update:modelValue', stringMode.value ? joinTags(arr, delimiter.value) : arr)
}

function itemVariant(text: string): Variant {
    if (!autoColor.value) return 'secondary'
    if (distinctColors.value) {
        return colorMap.value[text] ?? 'secondary'
    }
    // fallback: màu theo hash (có thể trùng)
    return tagToVariant(text)
}


/* XÓA */
function removeVisible(visibleIndex: number) {
    const text = visibleItems.value[visibleIndex]
    const idx = items.value.findIndex(s => s === text)
    if (idx !== -1) {
        const next = [...items.value]
        next.splice(idx, 1)
        items.value = next
        push(next)
        emit('removed', text)
    }
}

/* THÊM */
const input = ref('')
const addPopRef = ref()
const dupWarning = ref<string>('')
const limitWarning = ref<string>('')

function confirmAdd() {
    const raw = (input.value || '').trim()
    if (!raw) return
    const exists = items.value.some(s => s.toLowerCase() === raw.toLowerCase())
    if (exists) {
        dupWarning.value = t('common.alreadyExists') || 'Tag already exists'
        emit('blocked', 'duplicate')
        return
    }
    if (!canAdd.value) {
        limitWarning.value = t('common.reachLimit') || 'Reached maximum tags'
        emit('blocked', 'limit')
        return
    }
    const next = [...items.value, raw]
    items.value = next
    push(next)
    emit('added', raw)
    input.value = ''
    dupWarning.value = ''
    limitWarning.value = ''
    closePopover()
}

function onSelect(item: { value: string }) {
    input.value = item.value
    confirmAdd()
}

function querySearch(q: string, cb: (x: { value: string }[]) => void) {
    const list = (props.suggestions ?? []).filter(s =>
        s.toLowerCase().includes((q || '').toLowerCase())
    )
    cb(list.map(s => ({ value: s })))
}

function closePopover() {
    (addPopRef.value as any)?.hide?.()
}
const distinctColors = computed(() => props.distinctColors !== false) // default: true
// === Color map: gán màu không trùng trong danh sách ===
const colorMap = ref<Record<string, Variant>>({})

function recomputeColorMap() {
    const used = new Set<Variant>()
    const newMap: Record<string, Variant> = {}

    // Sắp xếp nhẹ theo hash để ổn định việc phân phối màu giữa các render
    const ordered = [...items.value].sort((a, b) => stableHash(a) - stableHash(b))

    for (const tag of ordered) {
        // Ưu tiên vị trí trong palette dựa trên hash để phân tán
        const start = stableHash(tag) % TAG_PALETTE.length
        let variant: Variant | null = null

        // Quét vòng tròn trong palette để tìm màu chưa dùng
        for (let step = 0; step < TAG_PALETTE.length; step++) {
            const idx = (start + step) % TAG_PALETTE.length
            const v = TAG_PALETTE[idx]
            if (!used.has(v)) { variant = v; break }
        }

        // Nếu số tag > số màu -> cho phép lặp lại (ít va chạm nhất)
        if (!variant) {
            // chọn màu ít “đụng” nhất theo tần suất đã dùng trong newMap
            const freq: Record<Variant, number> = TAG_PALETTE.reduce((acc, v) => (acc[v] = 0, acc), {} as Record<Variant, number>)
            for (const t in newMap) freq[newMap[t] as Variant]++
            variant = TAG_PALETTE.slice().sort((a, b) => (freq[a] - freq[b]))[0]
        }

        newMap[tag] = variant!
        used.add(variant!)
    }

    colorMap.value = newMap
}

// Tính lại khi danh sách tag thay đổi
watch(items, () => recomputeColorMap(), { immediate: true })
</script>

<style scoped>
/* Gọn chiều cao cho input và nút + */
:deep(.el-input),
:deep(.el-input__wrapper) {
    min-height: 28px;
    height: 28px;
    padding-top: 0;
    padding-bottom: 0;
}

button.btn.btn-light-primary.btn-sm.rounded-pill {
    height: 28px;
    line-height: 26px;
    padding: 0 .6rem;
}

.btn.btn-light-primary.btn-add-tag:hover {
    color: #fff !important;
}

:root {
    --tag-h: 10px;
    /* cùng chiều cao với BaseTag--compact */
}

.btn-add-tag {
    line-height: 26px !important;
    padding: 0 0.6rem;
    font-size: 12px;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    background-color: rgba(156, 243, 7, 0.12) !important;
    border: 1px solid rgba(109, 136, 245, 0.24);
    color: #2f56f1;
    cursor: pointer;
    transition: all 0.15s ease-in-out;
}

.btn-add-tag i {
    font-size: 12px;
}

.btn-add-tag:hover:not(:disabled) {
    background-color: rgba(30, 60, 182, 0.85);
    color: #fff;
}

.btn-add-tag:disabled {
    opacity: 0.6;
    cursor: not-allowed;
}
</style>
