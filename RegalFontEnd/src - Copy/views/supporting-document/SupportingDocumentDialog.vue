<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="props.loading" @submit="onSubmit" @delete="onDelete" width="800px"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <!-- Đa ngôn ngữ + Publish -->
                <el-col :span="12">
                    <el-form-item>
                        <el-checkbox v-model="formData.isMultilingual" :disabled="isView">
                            {{ t('common.allowMultilingual') }}
                        </el-checkbox>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item>
                        <el-checkbox v-model="formData.isPublish" :disabled="isView">
                            {{ t('common.isPublish') }}
                        </el-checkbox>
                    </el-form-item>
                </el-col>

                <!-- Tên tài liệu -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.name') }}</label>
                    <el-form-item prop="documentName">
                        <el-input v-model="formData.documentName" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- English Name -->
                <el-col :span="12" v-if="formData.isMultilingual">
                    <label class="required fs-6 fw-semibold mb-2 d-block">English Name</label>
                    <el-form-item prop="enDocumentName">
                        <el-input v-model="formData.enDocumentName" :disabled="isView" />
                    </el-form-item>
                </el-col>



                <!-- Tác giả -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.authorName')
                    }}</label>
                    <el-form-item prop="authorName">
                        <el-input v-model="formData.authorName" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="12" v-if="formData.isMultilingual">
                    <label class="required fs-6 fw-semibold mb-2 d-block">English Author</label>
                    <el-form-item prop="enAuthorName">
                        <el-input v-model="formData.enAuthorName" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Mô tả -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.description')
                    }}</label>
                    <el-form-item prop="description">
                        <el-input v-model="formData.description" type="textarea" :rows="3" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- English Description -->
                <el-col :span="24" v-if="formData.isMultilingual">
                    <label class="required fs-6 fw-semibold mb-2 d-block">English Description</label>
                    <el-form-item prop="enDescription">
                        <el-input v-model="formData.enDescription" type="textarea" :rows="3" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- Loại tài liệu -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{
                        t('supportingDocument.documentType') }}</label>
                    <el-form-item prop="documentTypeId">
                        <el-select v-model="formData.documentTypeId" :disabled="isView">
                            <el-option v-for="item in commonStore.documentTypes" :key="item.documentTypeId"
                                :label="item.documentTypeName" :value="item.documentTypeId" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <!-- Năm phát hành -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.yearRelease')
                        }}</label>
                    <el-form-item prop="yearRelease">
                        <el-input-number v-model="formData.yearRelease" :min="1900" :max="2100" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Ngày bắt đầu / kết thúc -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.startDate') }}</label>
                    <el-form-item prop="startDate">
                        <el-date-picker v-model="formData.startDate" type="date" format="YYYY-MM-DD"
                            value-format="YYYY-MM-DD" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.endDate') }}</label>
                    <el-form-item prop="endDate">
                        <el-date-picker v-model="formData.endDate" type="date" format="YYYY-MM-DD"
                            value-format="YYYY-MM-DD" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{
                        t('supportingDocument.websiteKeys') }}</label>
                    <el-form-item prop="websiteKeys">
                        <TagList v-model="websiteKeysModel" :suggestions="websiteKeySuggestions" :maxTags="Infinity"
                            :delimiter="'#$#'" :collapseToPopover="true" :maxVisible="6" :dismissible="!isView"
                            :distinctColors="true" :autoColor="true" :hideAddWhenLimit="true"
                            :placeholder="t('common.enterTag') || 'Enter key'" :addButtonText="t('common.add') || 'Add'"
                            @added="onKeyAdded" @removed="onKeyRemoved" />
                    </el-form-item>
                </el-col>
                <el-col :span="12" v-if="formData.isMultilingual">
                    <label class="required fs-6 fw-semibold mb-2 d-block"> English WebsiteKeys</label>
                    <el-form-item prop="enWebsiteKeys">
                        <TagList v-model="enWebsiteKeysModel" :suggestions="enWebsiteKeySuggestions" :maxTags="Infinity"
                            :delimiter="'#$#'" :collapseToPopover="true" :maxVisible="6" :dismissible="!isView"
                            :distinctColors="true" :autoColor="true" :hideAddWhenLimit="true"
                            :placeholder="t('common.enterTag') || 'Enter key'" :addButtonText="t('common.add') || 'Add'"
                            @added="onEnKeyAdded" @removed="onEnKeyRemoved" />
                    </el-form-item>
                </el-col>
                <!-- Format / Topic -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.format') }}</label>
                    <el-form-item prop="format">
                        <el-select v-model="formData.format" :disabled="isView">
                            <el-option v-for="item in formatOptions" :key="item.value" :label="item.label"
                                :value="item.value" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <!-- Level -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.level') || 'Level' }}</label>
                    <el-form-item prop="level">
                        <el-select v-model="formData.level" :disabled="isView" clearable
                            :placeholder="t('common.select')">
                            <el-option v-for="item in levelOptions" :key="item.value" :label="item.label"
                                :value="item.value" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.topic') }}</label>
                    <el-form-item prop="topic">
                        <el-select v-model="formData.topic" :disabled="isView" clearable
                            :placeholder="t('common.select')">
                            <el-option v-for="item in topicOptions" :key="item.value" :label="item.label"
                                :value="item.value" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <!-- Link tài liệu -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.link') }}</label>
                    <el-form-item prop="link">
                        <el-input v-model="formData.link" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- File đính kèm -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{
                        t('supportingDocument.fileAttached') }}</label>
                    <FileUpload ref="fileRef" :file-url="formData.attachment?.path" :key="fileUploadKey"
                        :maxSizeMb="500" :original-file-name="formData.attachment?.fileName"
                        :allowed-groups="allowedGroupsByFormat" :disabled="isFileUploadDisabled"
                        @file-change="onFileChange" />

                </el-col>

                <!-- Ảnh đại diện -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.avatar') }}</label>
                    <FileUpload ref="imageRef" :file-url="formData.image?.path" :key="imageUploadKey"
                        :original-file-name="formData.image?.fileName" :allowed-groups="['image']" :disabled="isView"
                        @change="onImgUploadChange" />
                </el-col>



                <!-- Trạng thái -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
                    <el-form-item prop="status">
                        <el-radio-group v-model="formData.status" :disabled="isView">
                            <el-radio :value="0">{{ t('common.active') }}</el-radio>
                            <el-radio :value="1">{{ t('common.inactive') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import FileUpload from '@/components/file-upload/FileUpload.vue'
import TagList from '@/components/tag/TagList.vue'
import { useCommonStore } from '@/stores/commonStore'
import type { SupportingDocumentModel } from '@/api/SupportingDocumentApi'
import { StatusType, FormatType } from '@/types'
import { getFormatOptions, getTopicOptions, getLevelTypeOptions } from '@/utils/makeList'

const props = defineProps<{
    visible: boolean
    mode?: 'create' | 'edit' | 'view'
    loading: boolean
    supportingDocumentData: Partial<SupportingDocumentModel> | null
}>()

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const commonStore = useCommonStore()

const isView = computed(() => props.mode === 'view')
const baseDialogRef = ref()
const submitting = ref(false)
const imageUploadKey = ref(0)
const fileUploadKey = ref(0)
const modeTitle = computed(() => {
    if (isView.value) return t('supportingDocument.detailTitle')
    if (props.mode === 'edit') return t('supportingDocument.editTitle')
    if (props.mode === 'create') return t('supportingDocument.addTitle')
    return ''
})

/** TagList v-model an toàn */
const websiteKeysModel = computed<string>({
    get: () => formData.value.websiteKeys ?? '',
    set: (v: string) => { formData.value.websiteKeys = v }
})
const enWebsiteKeysModel = computed<string>({
    get: () => formData.value.enWebsiteKeys ?? '',
    set: (v: string) => { formData.value.enWebsiteKeys = v }
})
const websiteKeySuggestions = computed<string[]>(() => (commonStore.websiteKeys ?? []).map(item => item.key))
const enWebsiteKeySuggestions = computed<string[]>(() => (commonStore.enWebsiteKeys ?? []).map(item => item.key))
const formData = ref<Partial<SupportingDocumentModel>>({})

const topicOptions = getTopicOptions(t);
const levelOptions = getLevelTypeOptions(t);

const formatOptions = getFormatOptions(t);
const fileRef = ref()
const imageRef = ref()

watch(
    () => props.supportingDocumentData,
    (data) => {
        if (data) {
            formData.value = {
                ...data,
                websiteKeys: (() => {
                    const raw = (data as any).websiteKeys
                    if (Array.isArray(raw)) return raw.join('#$#')
                    if (typeof raw === 'string') return raw
                    return ''
                })()
            }
        } else {
            formData.value = {
                documentName: '',
                description: '',
                documentTypeId: undefined,
                authorName: '',
                isPublish: false,
                isMultilingual: false,
                websiteKeys: '',
                enWebsiteKeys: '',
                format: undefined,
                topic: undefined,
                status: StatusType.Active,
            }
        }
        imageUploadKey.value++
        fileUploadKey.value++
    },
    { immediate: true, deep: true }
)
// (1) Validator: startDate phải < endDate
const validateStartBeforeEnd = (_: any, __: any, cb: (e?: Error) => void) => {
    const s = formData.value.startDate
    const e = formData.value.endDate
    if (s && e && s >= e) {
        return cb(new Error(t('validation.startDateBeforeEnd') || 'Start date must be before end date'))
    }
    cb()
}
// --- VALIDATORS ---
// yêu cầu endDate > startDate
const validateEndAfterStart = (_: any, __: any, cb: (e?: Error) => void) => {
    const s = formData.value.startDate
    const e = formData.value.endDate
    if (s && e && e <= s) return cb(new Error(t('validation.endDateAfterStart') || 'End date must be after start date'))
    cb()
}

// required động cho trường tiếng Anh khi bật đa ngôn ngữ
const makeMultilingualRequired = (fieldGetter: () => string | undefined) => {
    return (_: any, __: any, cb: (e?: Error) => void) => {
        if (formData.value.isMultilingual && !((fieldGetter() || '').trim())) {
            return cb(new Error(t('validation.required')))
        }
        cb()
    }
}

// --- FILE UPLOAD CONTROL ---
// map format -> allowed group(s) của FileUpload
const allowedGroupsByFormat = computed(() => {
    switch (formData.value.format) {
        case FormatType.Pdf:
            return ['pdf']
        case FormatType.Word:
            return ['document']
        case FormatType.Audio:
            return ['audio']
        case FormatType.Video:
            return ['video']
        case FormatType.Image:
            return ['image']
        default:
            return [] // chưa chọn format → không cho upload
    }
}) as unknown as any[]
const isFileUploadDisabled = computed(() =>
    isView.value || formData.value.format === undefined || formData.value.format === null
)
watch(() => formData.value.format, () => {
    // clear file đã chọn khi đổi format
    //formData.value.attachment = undefined
    fileUploadKey.value++  // force re-render FileUpload
})

const rules = {
    documentName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    documentTypeId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    authorName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    description: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    level: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    format: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    topic: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    yearRelease: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    // bắt buộc websiteKeys (VI)
    websiteKeys: [{ required: true, message: t('validation.required'), trigger: 'change' }],

    // NGÀY: endDate phải > startDate
    endDate: [{ validator: validateEndAfterStart, trigger: 'change' }],
    startDate: [{ validator: validateStartBeforeEnd, trigger: 'change' }],

    // CÁC TRƯỜNG TIẾNG ANH: chỉ required khi isMultilingual = true
    enDocumentName: [{ validator: makeMultilingualRequired(() => formData.value.enDocumentName as any), trigger: 'blur' }],
    enAuthorName: [{ validator: makeMultilingualRequired(() => formData.value.enAuthorName as any), trigger: 'blur' }],
    enDescription: [{ validator: makeMultilingualRequired(() => formData.value.enDescription as any), trigger: 'blur' }],
    enWebsiteKeys: [{ validator: makeMultilingualRequired(() => formData.value.enWebsiteKeys as any), trigger: 'change' }],
}


function closeModal() {
    emit('update:visible', false)
    emit('close')
}

async function onSubmit() {
    const form = (baseDialogRef.value as any)?.formRef
    form.validate(async (valid: boolean) => {
        if (!valid) return
        submitting.value = true
        type UploadRef = {
            uploadPending?: () => Promise<{ relativePath: string; fileName: string } | null>
            uploadPendingToTemp?: () => Promise<{ relativePath: string; fileName: string } | null>
            getCurrent?: () => { fileUrl: string | null; originalFileName: string | null; markedForDelete: boolean }
            getOriginalName?: () => string | null
            getUploadedOriginalName?: () => string | null
        }
        const imgRef = imageRef.value as UploadRef | undefined
        const imgUp = await (imgRef?.uploadPending?.() ?? imgRef?.uploadPendingToTemp?.() ?? Promise.resolve(null))
        const imgState = imgRef?.getCurrent?.()
        const fileUploadRef = fileRef.value as UploadRef | undefined
        const fileUp = await (fileUploadRef?.uploadPending?.() ?? fileUploadRef?.uploadPendingToTemp?.() ?? Promise.resolve(null))
        const fileState = fileUploadRef?.getCurrent?.()

        const payload: any = { ...formData.value }

        if (props.mode === 'create') {
            if (imgUp) {
                payload.image = {
                    path: imgUp.relativePath,
                    fileName: imgRef?.getOriginalName?.() ?? imgUp.fileName,
                }
            } else {
                delete payload.image
            }
            if (fileUp) {
                payload.attachment = {
                    path: fileUp.relativePath,
                    fileName: fileUploadRef?.getOriginalName?.() ?? fileUp.fileName,
                }
            } else {
                delete payload.attachment
            }
        } else if (props.mode === 'edit') {
            if (imgUp) {
                // ảnh mới upload
                payload.image = {
                    path: imgUp.relativePath,
                    fileName: imgRef?.getOriginalName?.() ?? imgUp.fileName,
                }
            } else if (imgState?.markedForDelete) {
                // xóa ảnh
                payload.image.path = ''
            }
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

        payload.listWebsiteKeys = commonStore.websiteKeys
        payload.listEnWebsiteKeys = commonStore.enWebsiteKeys
        emit('submit', payload)
    })
}
function onKeyAdded(text: string) {
    const k = (text || '').trim()
    if (!k) return
    commonStore.addOrIncreaseWebsiteKey(k)
}
function onKeyRemoved(text: string) {
    const k = (text || '').trim()
    if (!k) return
    commonStore.decreaseOrRemoveWebsiteKey(k)
}
function onEnKeyAdded(text: string) {
    const k = (text || '').trim()
    if (!k) return
    commonStore.addOrIncreaseEnWebsiteKey(k)
}
function onEnKeyRemoved(text: string) {
    const k = (text || '').trim()
    if (!k) return
    commonStore.decreaseOrRemoveEnWebsiteKey(k)
}
function onDelete() {
    emit('delete', formData.value)
}
function onFileChange(file: any) {
    formData.value.attachment = file
}
function onImgUploadChange() {
    baseDialogRef.value?.formRef?.validateField('formData.image.path')
}

onMounted(async () => {
    if (!commonStore.documentTypes.length) await commonStore.fetchDocumentTypes()
})
</script>
