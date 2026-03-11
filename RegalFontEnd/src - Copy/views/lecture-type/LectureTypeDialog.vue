<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="props.loading" @submit="onSubmit" @update:visible="emit('update:visible', $event)"
        @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('lectureType.name') }}</label>
                    <el-form-item prop="lectureName">
                        <el-input v-model="formData.lectureName" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('lectureType.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="2" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
                    <el-form-item prop="status">
                        <el-radio-group v-model="formData.status" :disabled="isView">
                            <el-radio :value="0">{{ t('common.active') }}</el-radio>
                            <el-radio :value="1">{{ t('common.inactive') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('lectureType.fileAttached') }}</label>
                    <FileUpload ref="fileRef" :file-url="formData.attachment?.path" :key="fileUploadKey"
                        :original-file-name="formData.attachment?.fileName" :allowed-groups="['document']"
                        @file-change="onFileChange" @delete-existing-file="onDeleteExistingFile" />
                </el-col>

                <el-col v-if="attachmentLink" :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('lectureType.attachmentLink') }}</label>
                    <el-input :model-value="attachmentLink" readonly>
                        <template #append>
                            <el-button type="primary" @click="copyAttachmentLink">
                                <i class="bi bi-clipboard me-1"></i>{{ t('lectureType.copyLink') }}
                            </el-button>
                        </template>
                    </el-input>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { LectureTypeModel } from '@/api/LectureTypeApi'
import FileUpload from '@/components/file-upload/FileUpload.vue'
import { StatusType } from '@/types'

const props = defineProps<{
    visible: boolean
    mode?: 'create' | 'edit' | 'view'
    loading: boolean
    lectureTypeData: Partial<LectureTypeModel> | null
}>()

const emit = defineEmits(['update:visible', 'submit', 'close'])
const { t } = useI18n()

const baseDialogRef = ref<any>()
const fileRef = ref<InstanceType<typeof FileUpload> | null>(null)
const fileUploadKey = ref(0)
const attachmentMarkedForDelete = ref(false)

const isView = computed(() => props.mode === 'view')
const isCreate = computed(() => props.mode === 'create')
const isEdit = computed(() => props.mode === 'edit')

const modeTitle = computed(() => {
    if (isCreate.value) return t('lectureType.addTitle')
    if (isEdit.value) return t('lectureType.editTitle')
    if (isView.value) return t('lectureType.detailTitle')
    return ''
})

// Form state
const formData = ref<Partial<LectureTypeModel>>({})

const apiBaseUrl = (import.meta.env.VITE_APP_API_URL as string || '')
    .replace(/\/+$/, '')
const attachmentLink = computed(() => {
    if (attachmentMarkedForDelete.value) return ''
    const attachmentId = formData.value.attachment?.id
    if (!attachmentId || !apiBaseUrl) return ''
    return `${apiBaseUrl}/file/download-by-attachment?id=${encodeURIComponent(attachmentId)}`
})

watch(
    () => props.lectureTypeData,
    (data) => {
        if (data) {
            formData.value = { ...data }
        } else {
            formData.value = {
                lectureName: '',
                description: '',
                status: StatusType.Active,
                attachment: undefined
            }
        }
        // reset FileUpload
        attachmentMarkedForDelete.value = false
        fileUploadKey.value++
    },
    { immediate: true, deep: true }
)

// ===== Validators =====
const validateFile = async () => {
    const pending = fileRef.value?.getPendingFile?.()
    const current = fileRef.value?.getCurrent?.()
    const hasExisting = !!current?.fileUrl && !current?.markedForDelete
    // Create: bắt buộc có file (pending hoặc existing chưa bị đánh dấu xoá)
    if (isCreate.value && !pending && !hasExisting) {
        return Promise.reject(new Error(t('validation.requiredFile')))
    }
    // Edit: nếu đã đánh dấu xoá nhưng không chọn file mới → lỗi
    if (isEdit.value && current?.markedForDelete && !pending) {
        return Promise.reject(new Error(t('validation.requiredFile')))
    }
    return Promise.resolve()
}

const rules = {
    lectureName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    fileUrl: [{ validator: (_r: any, _v: any, cb: any) => validateFile().then(() => cb()).catch(cb), trigger: 'change' }]
}

// ===== Submit =====
function onSubmit() {
    const form = baseDialogRef.value?.formRef
    form.validate(async (valid: boolean) => {
        if (!valid) return

        type UploadRef = {
            uploadPending?: () => Promise<{ relativePath: string; fileName: string } | null>
            uploadPendingToTemp?: () => Promise<{ relativePath: string; fileName: string } | null>
            getCurrent?: () => { fileUrl: string | null; originalFileName: string | null; markedForDelete: boolean }
            getOriginalName?: () => string | null
            getUploadedOriginalName?: () => string | null
        }
        const fileUploadRef = fileRef.value as UploadRef | undefined
        const fileUp = await (fileUploadRef?.uploadPending?.() ?? fileUploadRef?.uploadPendingToTemp?.() ?? Promise.resolve(null))
        const fileState = fileUploadRef?.getCurrent?.()

        const payload: any = { ...formData.value }
        if (props.mode === 'create') {
            if (fileUp) {
                payload.attachment = {
                    path: fileUp.relativePath,
                    fileName: fileUploadRef?.getOriginalName?.() ?? fileUp.fileName,
                }
            } else {
                delete payload.attachment
            }
        } else if (props.mode === 'edit') {
            if (fileUp) {
                // file mới upload
                payload.attachment = {
                    path: fileUp.relativePath,
                    fileName: fileUploadRef?.getUploadedOriginalName?.() ?? fileUp.fileName,
                }
            } else if (fileState?.markedForDelete) {
                // xóa file
                payload.attachment.path = ''
            }
        }
        emit('submit', payload)
    })
}
function onFileChange(file: any) {
    formData.value.attachment = file
    attachmentMarkedForDelete.value = false
}
function onDeleteExistingFile() {
    attachmentMarkedForDelete.value = true
}
function closeModal() {
    emit('close')
}

async function copyAttachmentLink() {
    const link = attachmentLink.value
    if (!link) return
    try {
        if (navigator?.clipboard?.writeText) {
            await navigator.clipboard.writeText(link)
        } else {
            const textarea = document.createElement('textarea')
            textarea.value = link
            textarea.setAttribute('readonly', '')
            textarea.style.position = 'fixed'
            textarea.style.opacity = '0'
            document.body.appendChild(textarea)
            textarea.select()
            document.execCommand('copy')
            document.body.removeChild(textarea)
        }
        ElMessage.success(t('lectureType.linkCopied'))
    } catch {
        ElMessage.error(t('common.somethingWentWrong'))
    }
}
</script>
