<template>
    <div class="space-y-8">
        <!-- Header actions -->
        <div class="flex justify-end items-center gap-2 mb-3 file-manager-header"
            :class="{ 'is-hidden': props.hideHeader }">
            <el-upload ref="uploadRef" :auto-upload="false" :show-file-list="false" :multiple="multiple"
                :on-change="onPickFiles" :accept="accept">
                <el-button type="primary" :disabled="disabled">
                    {{ t('file.add') }}
                </el-button>
            </el-upload>
            <slot name="extra-actions" />
        </div>

        <!-- Empty -->
        <div v-if="items.length === 0" class="text-gray-500 text-sm">
            {{ t('file.noFile') }}
        </div>

        <!-- List -->
        <div v-else class="space-y-8 md:space-y-10">
            <el-card v-for="(it, idx) in items" :key="it.uid" shadow="hover" class="mb-3 item-card"
                :class="idx % 2 === 0 ? 'is-even' : 'is-odd'">
                <!-- Tiêu đề: bám góc trên-trái của body -->
                <div v-if="props.itemTitle" class="item-title mb-3" :class="idx % 2 === 0 ? 'title-even' : 'title-odd'">
                    {{ props.itemTitle }} {{ idx + 1 }}
                </div>
                <div class="flex items-start justify-between gap-5">
                    <!-- LEFT: thumbnail + fields -->
                    <div class="flex items-start gap-5 flex-1 min-w-0">
                        <!-- Thumbnail + hover preview -->

                        <div class="relative mt-9 thumb-column">
                            <template v-if="previewSrc(it)">
                                <el-popover placement="right" trigger="hover" :width="popoverWidth"
                                    popper-class="image-preview-popper" :teleported="true" :popper-options="popperOpts">
                                    <template #reference>
                                        <img :src="previewSrc(it)" class="upload-thumb"
                                            @error="() => onThumbError(it)" />
                                    </template>

                                    <div class="preview-wrap">
                                        <img :src="previewSrc(it)" class="upload-preview-large"
                                            @error="() => onThumbError(it)" />
                                    </div>
                                </el-popover>
                            </template>
                            <template v-else>
                                <div class="file-name text-gray-600">
                                    {{ fileLabel(it) }}
                                </div>
                            </template>
                        </div>

                        <!-- Fields -->
                        <div class="flex-1 grid grid-cols-12 gap-4 min-w-0">
                            <template v-for="field in fields" :key="field.key">
                                <div :class="`col-span-${field.span || 12}`">
                                    <label class="fs-8 fw-semibold mb-2 mt-2 d-block">{{ field.label }}</label>

                                    <el-input v-if="field.type === 'text'" v-model="it[field.key]"
                                        :placeholder="field.placeholder || ''" size="small"
                                        :disabled="disabled || field.disabled" />

                                    <el-input v-else-if="field.type === 'textarea'" type="textarea"
                                        v-model="it[field.key]" :placeholder="field.placeholder || ''" size="small"
                                        :rows="2" :disabled="disabled || field.disabled" />

                                    <el-input-number v-else-if="field.type === 'number'" v-model="it[field.key]"
                                        :min="0" size="small" :disabled="disabled || field.disabled" />

                                    <el-switch v-else-if="field.type === 'switch'" v-model="it[field.key]"
                                        :active-text="field.activeText" :inactive-text="field.inactiveText"
                                        :disabled="disabled || field.disabled" size="small"
                                        @change="onBooleanFieldChange(field.key, idx)" />

                                    <el-select v-else-if="field.type === 'select'" v-model="it[field.key]"
                                        :placeholder="field.placeholder || ''" size="small"
                                        :disabled="disabled || field.disabled" filterable clearable>
                                        <el-option v-for="opt in (field.options || [])" :key="String(opt.value)"
                                            :label="opt.label" :value="opt.value" :disabled="opt.disabled" />
                                    </el-select>

                                    <slot name="custom-field" :field="field" :item="it" :index="idx" />
                                </div>
                            </template>
                        </div>
                    </div>

                    <!-- RIGHT: actions (cột cố định bên phải) -->
                    <div class="d-flex justify-content-end mt-4">
                        <el-tooltip v-if="props.enableDownload && it.path" :content="t('file.download')"
                            placement="left">
                            <el-button class="fm-action-btn" :icon="Download" size="small" circle :disabled="disabled"
                                @click="download(idx)" />
                        </el-tooltip>

                        <el-tooltip :content="t('file.moveUp')" placement="left">
                            <el-button class="fm-action-btn" :icon="ArrowUp" size="small" circle
                                :disabled="disabled || idx === 0" @click="move(idx, -1)" />
                        </el-tooltip>

                        <el-tooltip :content="t('file.moveDown')" placement="left">
                            <el-button class="fm-action-btn" :icon="ArrowDown" size="small" circle
                                :disabled="disabled || idx === items.length - 1" @click="move(idx, +1)" />
                        </el-tooltip>

                        <el-popconfirm :title="t('file.confirmRemove')" :confirm-button-text="t('common.ok')"
                            :cancel-button-text="t('common.cancel')" @confirm="remove(idx)">
                            <template #reference>
                                <el-button type="danger" size="small" :icon="Delete" :disabled="disabled">
                                    {{ t('file.remove') }}
                                </el-button>
                            </template>
                        </el-popconfirm>
                    </div>
                </div>
            </el-card>
        </div>
    </div>
</template>

<script setup lang="ts">
import { toRefs, watch, ref, onMounted, onBeforeUnmount } from 'vue'
import { useI18n } from 'vue-i18n'
import type { UploadFile } from 'element-plus'
import { ArrowUp, ArrowDown, Delete, Download } from '@element-plus/icons-vue'
import type { Attachment, FieldSchema } from '@/api/FileApi'
import { useFileStore } from '@/stores/fileStore'

type UploadedFile = { relativePath: string; fileName: string; size?: number; contentType?: string }

const fileStore = useFileStore()
const popoverWidth = ref(360)

function calcPopoverWidth() {
    const w = window.innerWidth
    // Anh chỉnh breakpoint & size theo ý:
    if (w < 576) popoverWidth.value = 240      // phone
    else if (w < 992) popoverWidth.value = 320 // tablet
    else popoverWidth.value = 520              // desktop
}
onMounted(() => {
    calcPopoverWidth()
    window.addEventListener('resize', calcPopoverWidth)
})
onBeforeUnmount(() => {
    window.removeEventListener('resize', calcPopoverWidth)
})
const props = withDefaults(defineProps<{
    modelValue: Attachment[]
    fields: FieldSchema[]
    uniqueBooleanKey?: string
    multiple?: boolean
    accept?: string
    disabled?: boolean
    removedIds?: string[]
    itemTitle?: string
    enableDownload?: boolean
    hideHeader?: boolean
}>(), {
    modelValue: () => [],
    fields: () => [],
    uniqueBooleanKey: '',
    multiple: true,
    accept: '',
    disabled: false,
    removedIds: () => [],
    itemTitle: '',
    enableDownload: false,
    hideHeader: false
})

const baseUrl = (import.meta.env.VITE_APP_BASE_SERVER_URL as string || '')
    .replace(/\/+$/, '')
    .replace(/\/api$/i, '')

const emit = defineEmits<{
    (e: 'update:modelValue', v: Attachment[]): void
    (e: 'update:removedIds', v: string[]): void
    (e: 'change', v: Attachment[]): void
    (e: 'error', v: any): void
}>()

const { t } = useI18n()
const { modelValue } = toRefs(props)

const items = ref<Attachment[]>([])
const removed = ref<string[]>([])
const uploadRef = ref<any>(null)

watch(modelValue, (v) => {
    items.value = (v || []).map((x) => ({ ...x, uid: x.uid || safeUUID() }))
}, { immediate: true })

watch(() => props.removedIds, (v) => {
    removed.value = [...(v || [])]
}, { immediate: true })
const popperOpts = {
    strategy: 'fixed',
    modifiers: [
        { name: 'preventOverflow', options: { boundary: 'viewport', altAxis: true, padding: 8 } },
        { name: 'flip', options: { fallbackPlacements: ['left', 'bottom', 'top'], padding: 8 } },
        { name: 'computeStyles', options: { gpuAcceleration: false } }
    ]
}

function onThumbError(it: Attachment) {
    (it as any).thumbError = true
}

function sync() {
    emit('update:modelValue', items.value)
    emit('update:removedIds', removed.value)
    emit('change', items.value)
}

/** default value theo schema (+ defaultValue nếu có) */
function defaultFields() {
    return props.fields.reduce((acc, f) => {
        if (acc[f.key] === undefined) {
            if ((f as any).defaultValue !== undefined) acc[f.key] = (f as any).defaultValue
            else acc[f.key] = f.type === 'switch' ? false : (f.type === 'number' ? 0 : '')
        }
        return acc
    }, {} as Record<string, any>)
}

/** Chỉ giữ file local, KHÔNG upload ở đây */
function onPickFiles(file: UploadFile) {
    const raw = file.raw
    if (!raw) return

    items.value.push({
        uid: safeUUID(),
        file: raw,
        fileName: raw.name,
        path: '',
        ...defaultFields()
    })

    if (props.uniqueBooleanKey && !items.value.some(x => !!(x as any)[props.uniqueBooleanKey!])) {
        (items.value[items.value.length - 1] as any)[props.uniqueBooleanKey!] = true
    }
    sync()
}
function safeUUID() {
    if (typeof crypto !== 'undefined' && crypto.randomUUID) {
        return crypto.randomUUID()
    }
    // Fallback nếu không hỗ trợ randomUUID
    return 'xxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0
        const v = c === 'x' ? r : (r & 0x3 | 0x8)
        return v.toString(16)
    })
}

/** Preview */
function previewSrc(it: Attachment): string | undefined {
    if ((it as any).thumbError) return undefined
    const name = it.file?.name || it.fileName || ''
    const path = it.path || ''
    // Ưu tiên file local (mới chọn)
    if (it.file && isImage(name || it.file.type)) return URL.createObjectURL(it.file)
    // Nếu đường dẫn có thể preview
    if (path && (isImage(path) || isImage(name))) return resolveUrl(path)
    return undefined
}
function fileLabel(it: Attachment) {
    return it.file?.name || it.fileName || it.path || t('file.noPreview')
}
function resolveUrl(path: string): string {
    if (!path) return ''
    if (/^(https?:)?\/\//i.test(path) || /^data:/i.test(path)) return path
    const p = path.replace(/^\/+/, '')
    return `${baseUrl}/${p}`
}
function isImage(nameOrType: string): boolean {
    const s = (nameOrType || '').toLowerCase()
    // Nếu là MIME type
    if (s.startsWith('image/')) return true
    // Nếu là data URL
    if (s.startsWith('data:image/')) return true
    // Nếu là tên file
    return /\.(jpg|jpeg|png|webp|gif|bmp|tiff|svg)(\?.*)?$/.test(s)
}


/** đảm bảo chỉ 1 boolean true */
function onBooleanFieldChange(key: string, idx: number) {
    if (!props.uniqueBooleanKey || key !== props.uniqueBooleanKey) return
    items.value.forEach((x, i) => { (x as any)[key] = i === idx })
    sync()
}
function move(idx: number, delta: number) {
    const to = idx + delta
    if (to < 0 || to >= items.value.length) return
    const tmp = items.value[idx]
    items.value[idx] = items.value[to]
    items.value[to] = tmp
    if (items.value.every(x => Object.prototype.hasOwnProperty.call(x, 'sortOrder'))) {
        items.value.forEach((x, i) => ((x as any).sortOrder = i + 1))
    }
    sync()
}
function remove(idx: number) {
    const it = items.value[idx]
    if (it?.id) removed.value.push(String(it.id))
    items.value.splice(idx, 1)
    sync()
}

async function download(idx: number) {
    const it = items.value[idx]
    if (!it?.path) return
    try {
        await fileStore.downloadFile(it.path, it.fileName || undefined)
    } catch (err) {
        emit('error', err)
    }
}

function openFilePicker() {
    if (!uploadRef.value) return
    if (typeof uploadRef.value.handleClick === 'function') {
        uploadRef.value.handleClick()
        return
    }
    const input =
        uploadRef.value?.$el?.querySelector('input[type=file]') ||
        document.querySelector<HTMLInputElement>('input[type=file]')
    input?.click()
}

/* ================== UPLOAD & PACK (EXPOSE) ================== */
async function uploadPendingFiles(): Promise<UploadedFile[]> {
    const slots: { idx: number; file: File }[] = []
    items.value.forEach((it, i) => { if (it?.file && it.file instanceof File) slots.push({ idx: i, file: it.file }) })
    if (slots.length === 0) return []
    const uploaded = await fileStore.uploadTemp(slots.map(s => s.file)) as UploadedFile[]
    uploaded.forEach((u, k) => {
        const i = slots[k].idx
        items.value[i].path = u.relativePath
        // giữ tên gốc người dùng chọn, fallback về tên server trả nếu thiếu
        items.value[i].fileName = slots[k].file.name || u.fileName
        items.value[i].contentType = slots[k].file.type || u.contentType
        items.value[i].file = null
    })
    sync()
    return uploaded
}
function packImages(dropId = false) {
    let arr = (items.value || [])
        .filter(x => x && ((x as any).path || (x as any).id))
        .map((x: any, i: number) => {
            const base: any = {
                path: x.path ?? '',
                fileName: x.fileName ?? '',
                contentType: x.contentType ?? '',
                caption: x.caption ?? '',
                isCover: !!x.isCover,
                sortOrder: Number.isFinite(x.sortOrder) ? x.sortOrder : i + 1
            }
            if (!dropId && x.id) base.id = x.id
            return base
        })
        .sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0))
        .map((x, i) => ({ ...x, sortOrder: i + 1 }))
    if (props.uniqueBooleanKey) {
        const k = props.uniqueBooleanKey
        const coverIdx = arr.findIndex(x => !!x[k])
        if (coverIdx === -1 && arr.length) arr[0][k] = true
        else if (coverIdx !== -1) arr = arr.map((x, i) => ({ ...x, [k]: i === coverIdx }))
    }
    return arr
}
function packAttachments(dropId = false) {
    return packImages(dropId).map(x => ({ ...x, filePath: x.path }))
}
function packCustom(opts?: { dropId?: boolean; pathKey?: string; extraKeys?: string[] }) {
    const dropId = !!opts?.dropId
    const pathKey = opts?.pathKey || 'path'
    const extra = new Set<string>(['fileName', ...(opts?.extraKeys || [])])
    const schemaKeys = new Set(props.fields.map(f => f.key))

    let arr = (items.value || [])
        .filter(x => x && ((x as any).path || (x as any).id))
        .map((x: any, i: number) => {
            const out: any = {
                [pathKey]: x.path ?? '',
                sortOrder: Number.isFinite(x.sortOrder) ? x.sortOrder : i + 1
            }
            if (!dropId && x.id) out.id = x.id
            schemaKeys.forEach(k => { out[k] = x[k] })
            extra.forEach(k => { out[k] = x[k] })
            if (props.uniqueBooleanKey) out[props.uniqueBooleanKey] = !!x[props.uniqueBooleanKey]
            return out
        })
        .sort((a, b) => (a.sortOrder ?? 0) - (b.sortOrder ?? 0))
        .map((x, i) => ({ ...x, sortOrder: i + 1 }))

    if (props.uniqueBooleanKey) {
        const k = props.uniqueBooleanKey
        const coverIdx = arr.findIndex(x => !!x[k])
        if (coverIdx === -1 && arr.length) arr[0][k] = true
        else if (coverIdx !== -1) arr = arr.map((x, i) => ({ ...x, [k]: i === coverIdx }))
    }
    return arr
}

defineExpose({ uploadPendingFiles, packImages, packAttachments, packCustom, openFilePicker })
</script>

<style scoped>
/* Thumbnail nhỏ (giống component mẫu) */
.upload-thumb {
    width: 56px;
    height: 56px;
    object-fit: cover;
    border-radius: 0.5rem;
    /* rounded */
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.08);
    border: 1px solid var(--el-border-color, #ebeef5);
    background: #f8f9fa;
}

/* Ảnh lớn – không khóa cứng kích thước */
.upload-preview-large {
    display: block;
    width: 100%;
    /* fill theo width của popover */
    height: auto;
    /* giữ tỉ lệ */
    object-fit: contain;
    border-radius: 0.75rem;
}

/* Popover responsive; width đã set bằng :width="popoverWidth" */
:deep(.image-preview-popper) {
    padding: 10px 10px 8px;
    border-radius: 0.75rem;
    box-shadow: 0 10px 30px rgba(0, 0, 0, .12);
    background: #fff;
    max-width: min(100vw, 640px);
    max-height: min(70vh, 560px);
    overflow: hidden;
    /* tránh tràn */
    z-index: 3000;
}

:deep(.image-preview-popper .preview-wrap) {
    width: 100%;
    max-height: min(70vh, 560px);
}

:deep(.image-preview-popper .upload-preview-large) {
    width: 100% !important;
    /* đảm bảo override mọi rule khác */
    max-width: 100% !important;
    height: auto !important;
    max-height: min(70vh, 560px) !important;
    object-fit: contain;
}

/* ---------- Title pill ---------- */
.item-title {
    display: inline-flex;
    align-items: center;
    padding: .35rem .6rem;
    border-radius: .6rem;
    font-size: .8rem;
    /* ~text-sm */
    font-weight: 700;
    letter-spacing: .02em;
    line-height: 1;
    user-select: none;
}

/* Chẵn: tông Xanh dương */
.title-even {
    background: #E8F1FF;
    /* bg-blue-50-ish */
    color: #1D4ED8;
    /* text-blue-700 */
    border: 1px solid #BFDBFE;
    /* ring-blue-200 */
}

/* Lẻ: tông Xanh lá */
.title-odd {
    background: #E8FFF4;
    /* bg-emerald-50-ish */
    color: #047857;
    /* text-emerald-700 */
    border: 1px solid #BBF7D0;
    /* ring-emerald-200 */
}

/* ---------- Card background theo chẵn/lẻ ---------- */
/* Viền ngoài card */
.item-card.is-even {
    border: 1px solid #DBEAFE;
}

/* blue-100 */
.item-card.is-odd {
    border: 1px solid #D1FAE5;
}

/* emerald-100 */

/* Nền phần body của Element Plus Card */
.item-card.is-even :deep(.el-card__body) {
    background: #F8FBFF;
    /* rất nhạt tông xanh dương */
}

.item-card.is-odd :deep(.el-card__body) {
    background: #F7FFFB;
    /* rất nhạt tông xanh lá */
}


.thumb-column {
    display: flex;
    flex-direction: column;
    align-items: flex-start;
}

.file-name {
    font-size: 0.85rem;
    word-break: break-all;
}

/* Cho body của el-card có vị trí tham chiếu để đặt absolute */
.item-card :deep(.el-card__body) {
    position: relative;
    padding-top: 18px;
    /* chừa chỗ bên dưới tiêu đề */
}

/* Đặt title bám góc trên-trái, hơi “nhô” lên trên 1 chút */
.item-title {
    position: absolute;
    top: 8px;
    /* đẩy lên cao */
    left: 12px;
    /* sát trái */
    z-index: 1;
    display: inline-flex;
    align-items: center;
    padding: .35rem .6rem;
    border-radius: .6rem;
    font-size: .8rem;
    font-weight: 700;
    letter-spacing: .02em;
    line-height: 1;
}

/* Màu theo chẵn/lẻ vẫn giữ như trước */
.title-even {
    background: #E8F1FF;
    color: #1D4ED8;
    border: 1px solid #BFDBFE;
}

.title-odd {
    background: #E8FFF4;
    color: #047857;
    border: 1px solid #BBF7D0;
}

.fm-action-btn {
    background: #5a2a83 !important;
    color: #fff !important;
    border-color: #5a2a83 !important;
}

.fm-action-btn.is-disabled {
    background: #d1d5db !important;
    border-color: #d1d5db !important;
    color: #f3f4f6 !important;
}

.file-manager-header.is-hidden {
    display: none;
}
</style>
