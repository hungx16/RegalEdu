<template>
    <div class="file-upload-card bg-body p-4 rounded-4 shadow-sm w-100" :class="{ 'is-disabled': isDisabled }">
        <!-- input file ẩn -->
        <input ref="fileInputRef" type="file" class="d-none" :accept="computedAcceptAttr" :disabled="isDisabled"
            @change="handleFileSelected" />

        <!-- (A) Đang có file MỚI (chưa upload) -->
        <div v-if="selectedFile" class="d-flex align-items-center justify-content-between">
            <div class="d-flex align-items-center gap-3 text-truncate">
                <!-- Ảnh mới: thumbnail + popover -->
                <template v-if="isSelectedImage">
                    <el-popover placement="right" trigger="hover" :width="popoverWidth"
                        popper-class="image-preview-popper" :teleported="true" :popper-options="popperOpts">
                        <template #reference>
                            <img :src="selectedPreviewUrl" alt="preview" class="upload-thumb" @error="onThumbError" />
                        </template>
                        <div class="preview-wrap">
                            <img :src="selectedPreviewUrl" alt="preview-large" class="upload-preview-large"
                                @error="onThumbError" />
                        </div>
                    </el-popover>

                    <div class="text-truncate">
                        <div class="fw-semibold text-truncate" :title="selectedFile.name">{{ selectedFile.name }}</div>
                        <div class="text-muted fs-8">{{ (selectedFile.size / 1024 / 1024).toFixed(2) }} MB</div>
                    </div>
                </template>

                <!-- File mới KHÔNG phải ảnh -->
                <template v-else>
                    <i class="bi bi-file-earmark-check fs-3 text-success"></i>
                    <span class="fw-semibold text-truncate" :title="selectedFile.name">{{ selectedFile.name }}</span>
                </template>
            </div>

            <div class="d-flex align-items-center gap-1">
                <!-- Chỉ clear pending (KHÔNG upload ở đây) -->
                <el-button text type="danger" @click="clearPending" :disabled="isDisabled">
                    <i class="bi bi-x-lg"></i>
                </el-button>
            </div>
        </div>

        <!-- (B) Có file HIỆN CÓ trên server (và không có file mới) -->
        <div v-else-if="!markedForDelete && currentFileUrl && currentOriginalName"
            class="d-flex align-items-center justify-content-between">
            <div class="d-flex align-items-center gap-3 text-truncate">
                <!-- Ảnh hiện có: thumbnail + popover -->
                <template v-if="existingIsImage">
                    <el-popover placement="right" trigger="hover" :width="popoverWidth"
                        popper-class="image-preview-popper" :teleported="true" :popper-options="popperOpts">
                        <template #reference>
                            <img :src="existingImageSrc" alt="preview" class="upload-thumb" @error="onThumbError" />
                        </template>
                        <div class="preview-wrap">
                            <img :src="existingImageSrc" alt="preview-large" class="upload-preview-large"
                                @error="onThumbError" />
                        </div>
                    </el-popover>

                    <div class="text-truncate">
                        <a v-if="!isDisabled" href="javascript:void(0);"
                            class="text-primary fw-semibold text-truncate d-inline-block" :title="currentOriginalName"
                            @click="downloadExisting">
                            {{ currentOriginalName }}
                        </a>
                        <span v-else class="text-muted fw-semibold text-truncate" :title="currentOriginalName">
                            {{ currentOriginalName }}
                        </span>
                    </div>
                </template>

                <!-- File hiện có KHÔNG phải ảnh -->
                <template v-else>
                    <i class="bi bi-paperclip fs-3 text-primary"></i>
                    <a v-if="!isDisabled" href="javascript:void(0);" class="text-primary fw-semibold text-truncate"
                        :title="currentOriginalName" @click="downloadExisting">
                        {{ currentOriginalName }}
                    </a>
                    <span v-else class="text-muted fw-semibold text-truncate" :title="currentOriginalName">
                        {{ currentOriginalName }}
                    </span>
                </template>
            </div>

            <!-- Actions -->
            <div class="d-flex align-items-center gap-1">
                <el-button text type="primary" @click="triggerFileSelect" :disabled="isDisabled"
                    :title="t('common.replace')">
                    <i style="color: darkgray;" class="bi bi-arrow-repeat"></i>
                </el-button>
                <el-button text type="danger" @click="markDelete()" :disabled="isDisabled" :title="t('common.remove')">
                    <i style="color: darkgray;" class="bi bi-trash"></i>
                </el-button>
            </div>
        </div>

        <!-- (C) Chưa có gì hoặc đã mark delete -->
        <div v-else class="text-center" :class="{ 'drop-active': isDragging }"
            :style="{ cursor: isDisabled ? 'not-allowed' : 'pointer' }" @click="onClickDropzone"
            @dragover.prevent="onDragOver" @dragenter.prevent="onDragEnter" @dragleave.prevent="onDragLeave"
            @drop.prevent="onDrop">
            <i class="bi bi-cloud-arrow-up fs-2x text-primary mb-2"></i>
            <i18n-t keypath="fileUpload.dragOrClick" tag="div" class="el-upload__text fw-semibold">
                <template #emphasis>
                    <em class="text-primary">{{ t('fileUpload.clickToSelect') }}</em>
                </template>
            </i18n-t>
            <div v-if="markedForDelete" class="mt-2 text-muted fs-8">{{ t('common.markedForDelete') }}</div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onBeforeUnmount } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import { useFileStore } from '@/stores/fileStore'

type UploadedFile = { relativePath: string; fileName: string; size?: number; contentType?: string }
const fileStore = useFileStore()
const baseUrl = (import.meta.env.VITE_APP_BASE_SERVER_URL as string || '')
    .replace(/\/+$/, '')
    .replace(/\/api$/i, '')
/** ========= config loại file ========= */
type FileGroup = 'image' | 'document' | 'spreadsheet' | 'presentation' | 'pdf' | 'video' | 'audio'
const FILE_GROUPS: Record<FileGroup, { extensions: string[]; mimes: string[] }> = {
    image: { extensions: ['.png', '.jpg', '.jpeg', '.webp', '.gif', '.bmp', '.svg'], mimes: ['image/*'] },
    document: { extensions: ['.pdf', '.doc', '.docx', '.txt', '.rtf'], mimes: ['application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 'text/plain', 'application/rtf'] },
    spreadsheet: { extensions: ['.xls', '.xlsx', '.csv', '.ods'], mimes: ['application/vnd.ms-excel', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'text/csv', 'application/vnd.oasis.opendocument.spreadsheet'] },
    presentation: { extensions: ['.ppt', '.pptx', '.odp'], mimes: ['application/vnd.ms-powerpoint', 'application/vnd.openxmlformats-officedocument.presentationml.presentation', 'application/vnd.oasis.opendocument.presentation'] },
    pdf: { extensions: ['.pdf'], mimes: ['application/pdf'] },
    video: { extensions: ['.mp4', '.mov', '.avi', '.mkv', '.webm'], mimes: ['video/*'] },
    audio: { extensions: ['.mp3', '.wav', '.aac', '.flac', '.m4a', '.ogg'], mimes: ['audio/*'] }
}
const DEFAULT_EXTENSIONS = ['.pdf', '.doc', '.docx', '.xls', '.xlsx', '.ppt', '.pptx', '.txt', '.png', '.jpg', '.jpeg']
// Helper nhận diện đuôi ảnh / không phải ảnh
// THAY 2 DÒNG NÀY
// const IMG_EXT_RE = /\.(png|jpe?g|webp|gif|bmp)(\?.*)?$/i
// const NON_IMG_EXT_RE = /\.(pdf|docx?|pptx?|xlsx?|csv|txt|rtf|zip|rar|7z|tar|gz|mp4|mov|avi|mkv|webm|mp3|wav|aac|flac|m4a|ogg)(\?.*)?$/i

// BẰNG:
const IMG_EXT_RE = /\.(png|jpe?g|webp|gif|bmp|svg)(\?.*)?$/i
const NON_IMG_EXT_RE = /\.(pdf|docx?|pptx?|xlsx?|csv|txt|rtf|zip|rar|7z|tar|gz|mp4|mov|avi|mkv|webm|mp3|wav|aac|flac|m4a|ogg)(\?.*)?$/i
const SVG_EXT_RE = /\.svg(\?.*)?$/i

/** ========= props & emits ========= */
const props = defineProps<{
    fileUrl?: string | null
    originalFileName?: string | null
    disabled?: boolean
    allowedGroups?: FileGroup[]
    allowedExtensionsOverride?: string[]
    maxSizeMb?: number
    existingContentType?: string | null
    forceExistingAsImage?: boolean
    resolveUrl?: (path: string | null | undefined) => string | null
    enableFetchImage?: boolean
}>()

const emit = defineEmits<{
    (e: 'change'): void
    (e: 'uploaded', payload: UploadedFile): void
    (e: 'delete-existing-file', payload?: { path?: string | null }): void
    (e: 'error', err: any): void
    (e: 'file-change', file: File | null): void  // tương thích code cũ
}>()

const { t } = useI18n()

/** ========= state ========= */
const fileInputRef = ref<HTMLInputElement | null>(null)
const isDisabled = computed(() => !!props.disabled)

const currentFileUrl = ref<string | null>(props.fileUrl ?? null)
const currentOriginalName = ref<string | null>(props.originalFileName ?? null)
const markedForDelete = ref(false)

watch(() => props.fileUrl, v => { if (!selectedFile.value) currentFileUrl.value = v ?? null })
watch(() => props.originalFileName, v => { if (!selectedFile.value) currentOriginalName.value = v ?? null })

/** ========= chọn file (pending) ========= */
const selectedFile = ref<File | null>(null)
const selectedPreviewUrl = ref<string>('')

const computedMaxSizeMb = computed(() => props.maxSizeMb ?? 10)
const computedAllowedExtensions = computed<string[]>(() => {
    if (props.allowedExtensionsOverride?.length) return props.allowedExtensionsOverride.map(x => x.toLowerCase())
    if (props.allowedGroups?.length) {
        const set = new Set<string>()
        props.allowedGroups.forEach(g => FILE_GROUPS[g]?.extensions.forEach(ext => set.add(ext.toLowerCase())))
        return Array.from(set)
    }
    return DEFAULT_EXTENSIONS
})
const computedAcceptAttr = computed<string>(() => {
    const mimes: string[] = []
    if (props.allowedGroups?.length) props.allowedGroups.forEach(g => FILE_GROUPS[g]?.mimes.forEach(m => mimes.push(m)))
    return Array.from(new Set([...mimes, ...computedAllowedExtensions.value])).join(',')
})

function isFileAllowed(file: File): boolean {
    const name = file.name.toLowerCase()
    const type = (file.type || '').toLowerCase()

    if (props.allowedGroups?.length) {
        for (const g of props.allowedGroups) {
            for (const m of FILE_GROUPS[g].mimes) {
                if (m.endsWith('/*')) {
                    const p = m.replace('/*', '')
                    if (type.startsWith(p)) return true
                } else if (type === m) return true
            }
        }
    }
    if (computedAllowedExtensions.value.some(ext => name.endsWith(ext))) return true
    if (props.allowedGroups?.length) {
        for (const g of props.allowedGroups) if (FILE_GROUPS[g].mimes.includes(type)) return true
    }
    return false
}

const isSelectedImage = computed(() => {
    if (!selectedFile.value) return false
    const type = (selectedFile.value.type || '').toLowerCase()
    return type.startsWith('image/') ||
        ['.png', '.jpg', '.jpeg', '.webp', '.gif', '.bmp', '.tiff', '.svg'].some(ext => selectedFile.value!.name.toLowerCase().endsWith(ext))
})

watch(selectedFile, (f) => {
    if (selectedPreviewUrl.value) {
        URL.revokeObjectURL(selectedPreviewUrl.value)
        selectedPreviewUrl.value = ''
    }
    if (!f) return
    if (isSelectedImage.value) {
        try { selectedPreviewUrl.value = URL.createObjectURL(f) } catch { selectedPreviewUrl.value = '' }
    }
})

function validateBasic(file: File): boolean {
    const sizeMb = file.size / 1024 / 1024
    if (sizeMb > computedMaxSizeMb.value) {
        ElMessage.error(t('validation.maxFileSize', { size: computedMaxSizeMb.value }))
        fileInputRef.value && (fileInputRef.value.value = '')
        return false
    }
    if (!isFileAllowed(file)) {
        const friendly = computedAllowedExtensions.value.join(', ')
        ElMessage.error(t('validation.fileTypeNotAllowed', { types: friendly }))
        fileInputRef.value && (fileInputRef.value.value = '')
        return false
    }
    return true
}

function triggerFileSelect() {
    if (isDisabled.value) return
    fileInputRef.value?.click()
}

function handleFileSelected(e: Event) {
    if (isDisabled.value) return
    const input = e.target as HTMLInputElement
    const file = input.files && input.files[0]
    if (!file) return
    if (!validateBasic(file)) return

    markedForDelete.value = false
    selectedFile.value = file
    emit('file-change', file) // tương thích
    emit('change')
}

function clearPending() {
    selectedFile.value = null
    if (selectedPreviewUrl.value) URL.revokeObjectURL(selectedPreviewUrl.value)
    selectedPreviewUrl.value = ''
    fileInputRef.value && (fileInputRef.value.value = '')
    emit('file-change', null) // tương thích
    emit('change')
}

function markDelete() {
    markedForDelete.value = true
    clearPending()
    emit('delete-existing-file', { path: currentFileUrl.value ?? undefined })
}

/** ========= Drag & Drop ========= */
const isDragging = ref(false)
function onClickDropzone() { if (!isDisabled.value) triggerFileSelect() }
function onDragOver(_: DragEvent) { if (isDisabled.value) return }
function onDragEnter() { if (!isDisabled.value) isDragging.value = true }
function onDragLeave() { if (!isDisabled.value) isDragging.value = false }
function onDrop(e: DragEvent) {
    if (isDisabled.value) return
    isDragging.value = false
    const files = e.dataTransfer?.files
    if (files && files.length > 0) {
        const f = files[0]
        if (!validateBasic(f)) return
        markedForDelete.value = false
        selectedFile.value = f
        emit('file-change', f)
        emit('change')
    }
}

/** ========= Hiển thị ảnh hiện có (server) ========= */
const resolvedExistingUrl = computed<string | null>(() => {
    const raw = currentFileUrl.value || null
    if (!raw) return null
    if (typeof props.resolveUrl === 'function') return props.resolveUrl(raw)
    if (/^https?:\/\//i.test(raw)) return raw
    return `${baseUrl}/${raw.replace(/^\/+/, '')}`
})
const existingBlobUrl = ref<string>('')
const existingImageSrc = computed(() => existingBlobUrl.value || resolvedExistingUrl.value || '')
const existingIsImage = ref(false)

function onThumbError(e: Event) {
    const img = e.target as HTMLImageElement
    img.style.display = 'none'
}

async function probeExistingIsImage() {
    const url = resolvedExistingUrl.value

    // cleanup blob cũ
    if (existingBlobUrl.value) {
        URL.revokeObjectURL(existingBlobUrl.value)
        existingBlobUrl.value = ''
    }

    if (!url) {
        existingIsImage.value = false
        return
    }

    // Nếu đã có tín hiệu chắc chắn → quyết ngay, không probe
    if (props.forceExistingAsImage) { existingIsImage.value = true; return }
    if ((props.existingContentType || '').toLowerCase().startsWith('image/')) { existingIsImage.value = true; return }

    const nameOrUrl = (currentOriginalName.value || url).toLowerCase()
    if (SVG_EXT_RE.test(nameOrUrl)) {
        existingIsImage.value = true
        return
    }

    // Đuôi ảnh → coi là ảnh (khỏi probe)
    if (IMG_EXT_RE.test(nameOrUrl)) { existingIsImage.value = true; return }


    // Đuôi chắc chắn KHÔNG phải ảnh → đừng probe (tránh 404/console error)
    if (NON_IMG_EXT_RE.test(nameOrUrl)) {
        existingIsImage.value = false
        return
    }

    // Chỉ còn trường hợp KHÔNG RÕ đuôi → mới thử nạp bằng <img>
    try {
        await new Promise<void>((resolve) => {
            const img = new Image()
            img.onload = () => { existingIsImage.value = true; resolve() }
            img.onerror = () => resolve()
            img.src = url as string
        })
    } catch { /* ignore */ }

    // Nếu vẫn chưa xác định và cần xác thực có cookie → thử fetch blob (nếu store hỗ trợ)
    if (!existingIsImage.value && props.enableFetchImage && (fileStore as any)?.fetchImageBlob) {
        try {
            const blob: Blob = await (fileStore as any).fetchImageBlob(url as string)
            if (blob?.type?.startsWith('image/')) {
                existingBlobUrl.value = URL.createObjectURL(blob)
                existingIsImage.value = true
            }
        } catch { /* ignore */ }
    }
}

onMounted(probeExistingIsImage)
watch([resolvedExistingUrl, currentOriginalName], () => {
    if (!selectedFile.value) probeExistingIsImage()
})
/** ========= Download file hiện có ========= */
async function downloadExisting() {
    if (isDisabled.value) return
    const url = currentFileUrl.value
    if (!url) return
    try {
        console.log(url);

        await fileStore.downloadFile(url, currentOriginalName.value ?? undefined)
    } catch (err) {
        emit('error', err)
    }
}

/** ========= Popover responsive ========= */
const popoverWidth = ref(360)
const popperOpts = {
    strategy: 'fixed',
    modifiers: [
        { name: 'preventOverflow', options: { boundary: 'viewport', altAxis: true, padding: 8 } },
        { name: 'flip', options: { fallbackPlacements: ['left', 'bottom', 'top'], padding: 8 } },
        { name: 'computeStyles', options: { gpuAcceleration: false } }
    ]
}
function calcPopoverWidth() {
    const w = window.innerWidth
    if (w < 576) popoverWidth.value = 240
    else if (w < 992) popoverWidth.value = 320
    else popoverWidth.value = 520
}
onMounted(() => { calcPopoverWidth(); window.addEventListener('resize', calcPopoverWidth) })
onBeforeUnmount(() => { window.removeEventListener('resize', calcPopoverWidth) })

/** ========= Expose cho cha gọi khi submit ========= */
function getPendingFile(): File | null { return selectedFile.value }
function getOriginalName(): string | null {
    // ưu tiên file đang chọn; nếu đã upload xong thì là currentOriginalName (tên gốc đã lưu)
    return selectedFile.value?.name ?? currentOriginalName.value ?? null
}
function getUploadedOriginalName(): string | null {
    // tên gốc của lần upload gần nhất (không phải GUID)
    return lastUploadedOriginalName.value ?? currentOriginalName.value ?? null
}

function hasPendingChange(): boolean { return !!selectedFile.value || markedForDelete.value }
function getCurrent(): { fileUrl: string | null; originalFileName: string | null; markedForDelete: boolean } {
    return { fileUrl: currentFileUrl.value ?? null, originalFileName: currentOriginalName.value ?? null, markedForDelete: markedForDelete.value }
}
const lastUploadedOriginalName = ref<string | null>(null)

/** Upload file pending lên thư mục tạm (nếu có) & cập nhật UI, trả URL + fileName */
async function uploadPending(): Promise<UploadedFile | null> {
    if (!selectedFile.value) return null
    try {
        const originalName = selectedFile.value.name
        const [up] = await fileStore.uploadTemp([selectedFile.value]) as UploadedFile[]
        if (up) {
            currentFileUrl.value = up.relativePath
            currentOriginalName.value = originalName
            lastUploadedOriginalName.value = originalName
            markedForDelete.value = false
            clearPending()
            await probeExistingIsImage()
            emit('uploaded', up)
        }
        return up ?? null
    } catch (err) {
        emit('error', err)
        return null
    }
}


defineExpose({
    getPendingFile,
    getOriginalName,
    getUploadedOriginalName,          // yêu cầu thêm
    hasPendingChange,
    getCurrent,
    uploadPending,
    uploadPendingToTemp: uploadPending, // alias cho code cũ
    clearPending,
    markDelete
})
</script>

<style scoped>
.file-upload-card.is-disabled {
    opacity: 0.7;
}

.file-upload-card.is-disabled a {
    pointer-events: none;
}

/* Thumbnail nhỏ */
.upload-thumb {
    width: 56px;
    height: 56px;
    object-fit: cover;
    border-radius: 0.5rem;
    box-shadow: 0 2px 6px rgba(0, 0, 0, .08);
    border: 1px solid var(--el-border-color, #ebeef5);
    background: #f8f9fa;
}

/* Ảnh to trong popover (responsive) */
.upload-preview-large {
    display: block;
    width: 100% !important;
    max-width: 100% !important;
    height: auto !important;
    max-height: min(70vh, 560px) !important;
    object-fit: contain;
    border-radius: 0.75rem;
}

/* Popper style + chặn tràn */
:deep(.image-preview-popper) {
    padding: 10px 10px 8px;
    border-radius: 0.75rem;
    box-shadow: 0 10px 30px rgba(0, 0, 0, .12);
    background: #fff;
    max-width: min(100vw, 640px);
    max-height: min(70vh, 560px);
    overflow: hidden;
    z-index: 3000;
}

:deep(.image-preview-popper .preview-wrap) {
    width: 100%;
    max-height: min(70vh, 560px);
}

/* Dragging */
.drop-active {
    outline: 2px dashed var(--el-color-primary);
    outline-offset: 6px;
    background: rgba(64, 158, 255, .06);
    border-radius: 0.75rem;
}
</style>
