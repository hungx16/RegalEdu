<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="submitting || props.loading" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.code') }}</label>
                    <el-form-item prop="companyCode">
                        <el-input v-model="formData.companyCode" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.name') }}</label>
                    <el-form-item prop="companyName">
                        <el-input v-model="formData.companyName" :disabled="isView"
                            :placeholder="t('company.namePlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.address') }}</label>
                    <el-form-item prop="companyAddress">
                        <el-input v-model="formData.companyAddress" :disabled="isView"
                            :placeholder="t('company.addressPlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.phone') }}</label>
                    <el-form-item prop="companyPhone">
                        <el-input v-model="formData.companyPhone" :disabled="isView"
                            :placeholder="t('company.phonePlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.establishmentDate') }}</label>
                    <el-form-item prop="establishmentDate">
                        <el-date-picker v-model="formData.establishmentDate" type="date" :disabled="isView"
                            :placeholder="t('company.establishmentDatePlaceholder')" :clearable="true"
                            format="YYYY-MM-DD" value-format="YYYY-MM-DD" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.provinceCode') }}</label>
                    <el-form-item prop="provinceCode" v-if="!isView">
                        <el-select v-model="formData.provinceCode" filterable clearable
                            :placeholder="t('company.provincePlaceholder')">
                            <el-option v-for="opt in provinceOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                    <BaseBadge v-else :label="provinceName || '-'" :rawLabel="true" />
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.region') }}</label>
                    <el-form-item prop="regionId" v-if="!isView">
                        <el-select v-model="formData.regionId" filterable :placeholder="t('company.regionPlaceholder')"
                            clearable>
                            <el-option v-for="opt in regionOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" :disabled="opt.disabled" />
                        </el-select>
                    </el-form-item>
                    <BaseBadge v-else :label="formData.region?.regionName || '-'" :rawLabel="true" />
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('region.manager') }}</label>
                    <el-form-item prop="managerId" required v-if="!isView">
                        <el-select v-model="formData.managerId" filterable
                            :placeholder="t('company.managerPlaceholder')">
                            <el-option v-for="opt in employeeOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" :disabled="opt.disabled" />
                        </el-select>
                    </el-form-item>
                    <BaseBadge v-else :label="formData.manager?.applicationUser?.fullName || '-'" :color="'purple'"
                        :rawLabel="true" />
                </el-col>

                <el-col :span="12" v-if="isView">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdBy') }}</label>
                    <BaseBadge :label="formData.createdBy || '-'" :rawLabel="true" :soft="true" :pill="true" />
                </el-col>

                <el-col :span="12" v-if="isView">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdAt') }}</label>
                    <BaseBadge :label="formatDate(formData.createdAt || '', 'YYYY-MM-DD HH:mm:ss')" :rawLabel="true" />
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
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.images') }}</label>
                    <FileManager ref="fileMgrRef" v-model="(formData as any).companyImages"
                        v-model:removedIds="removedImageIds" :fields="imageFields" unique-boolean-key="isCover"
                        :item-title="t('company.image')" accept="image/*" :multiple="true" :disabled="isView" />
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { CompanyModel } from '@/api/CompanyApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { formatDate } from '@/utils/format'
import { useCommonStore } from '@/stores/commonStore'
import { useRegionStore } from '@/stores/regionStore'
import { useEmployeeStore } from '@/stores/employeeStore'
import { buildSelectOptions } from '@/utils/publicFunction'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { StatusType } from '@/types'
import FileManager from '@/components/file-manager/FileManager.vue'
import { useFileStore } from '@/stores/fileStore'
import type { FieldSchema } from '@/api/FileApi'

const fileStore = useFileStore()
const fileMgrRef = ref<any>(null)

const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    companyData: Partial<CompanyModel> | null
}>()

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const removedImageIds = ref<string[]>([])

const { t } = useI18n()
const notificationStore = useNotificationStore()
const commonStore = useCommonStore()
const employeeStore = useEmployeeStore()
const regionStore = useRegionStore()
const isView = computed(() => props.mode === 'view')

const modeTitle = computed(() => {
    if (isView.value) return t('company.detailTitle')
    if (props.mode === 'edit') return t('company.editTitle')
    if (props.mode === 'create') formData.value.companyCode = commonStore.code ?? ''
    return t('company.addTitle')
})

const employeeOptionsObj = computed(() =>
    buildSelectOptions(
        employeeStore.employees,
        formData.value.managerId,
        e => e.applicationUser.fullName + (e.applicationUser.status === 1 ? ` (${t('common.inactive')})` : ''),
        e => e.applicationUser.status === 1
    )
)
const employeeOptions = computed(() => employeeOptionsObj.value.options)

const regionOptionsObj = computed(() =>
    buildSelectOptions(
        regionStore.regions,
        formData.value.regionId,
        r => r.regionName + (r.status === 1 ? ` (${t('common.inactive')})` : ''),
        r => r.status === 1
    )
)
const regionOptions = computed(() => regionOptionsObj.value.options)

const provinceOptions = computed(() =>
    commonStore.provinces.filter(p => !!p.provinceCode).map(p => ({ label: p.provinceName, value: p.provinceCode }))
)

const provinceName = computed(() => {
    if (!formData.value.provinceCode) return '-'
    const found = commonStore.provinces.find(p => p.provinceCode === formData.value.provinceCode)
    return found?.provinceName || formData.value.provinceCode || '-'
})

const imageFields: FieldSchema[] = [
    { key: 'caption', label: t('image.caption'), type: 'text', span: 12 },
    { key: 'isCover', label: t('image.cover'), type: 'switch', span: 4, activeText: t('image.coverYes'), inactiveText: t('image.coverNo') },
    { key: 'sortOrder', label: t('image.order'), type: 'number', span: 4 }
]

const baseDialogRef = ref()
const submitting = ref(false)

const formData = ref<CompanyModel>({
    id: '',
    companyCode: '',
    companyName: '',
    companyAddress: '',
    companyPhone: '',
    establishmentDate: '',
    provinceCode: '',
    managerId: '',
    manager: null,
    createdAt: '',
    createdBy: '',
    isDeleted: false,
    status: 0,
    logRegionComs: [],
    companyImages: [],
    isPublish: false
})

function normalizeImages(arr: any[] | undefined | null) {
    return (arr || []).map((x: any, i: number) => {
        const base: any = {
            imageUrl: x.imageUrl ?? '',
            caption: x.caption ?? '',
            isCover: !!x.isCover,
            sortOrder: x.sortOrder ?? i + 1,
            file: null
        }
        const id = x.id ?? x.imageId
        if (id) base.id = id
        return base
    })
}

watch(
    () => props.companyData,
    (data) => {
        if (data) {
            const activeLog = (data.logRegionComs || []).find(l => l.status === 0 && l.companyId === data.id)
            formData.value = {
                id: data.id ?? '',
                companyCode: data.companyCode ?? '',
                companyName: data.companyName ?? '',
                companyAddress: data.companyAddress ?? '',
                companyPhone: data.companyPhone ?? '',
                establishmentDate: data.establishmentDate ?? '',
                provinceCode: data.provinceCode ?? '',
                managerId: data.managerId ?? '',
                manager: data.manager ?? null,
                regionId: activeLog?.regionId || '',
                region: activeLog?.region || null,
                createdAt: data.createdAt ?? '',
                createdBy: data.createdBy ?? '',
                isDeleted: data.isDeleted ?? false,
                status: data.status ?? 0,
                logRegionComs: data.logRegionComs ?? [],
                companyImages: normalizeImages(data.companyImages),
                isPublish: data.isPublish ?? false
            }
            removedImageIds.value = []
            formData.value = {
                companyCode: '',
                companyName: '',
                companyAddress: '',
                companyPhone: '',
                establishmentDate: '',
                provinceCode: '',
                managerId: '',
                manager: null,
                regionId: '',
                region: null,
                status: 0,
                logRegionComs: [],
                companyImages: [],
                isPublish: false
            } as any
            removedImageIds.value = []
            removedImageIds.value = []
        }
    },
    { immediate: true }
)

watch(
    () => props.visible,
    async (val) => {
        if (val) {
            removedImageIds.value = []
            await commonStore.fetchProvinces()
            await employeeStore.fetchAllEmployees()
            await regionStore.fetchAllRegions()
        }
    },
    { immediate: true }
)

const rules = {
    companyCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    companyName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    companyPhone: [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        {
            validator: (_r: any, v: string, cb: any) => {
                const vnPhoneRegex = /^0\d{9}$/
                if (!v) cb(); else if (!vnPhoneRegex.test(v)) cb(new Error(t('validation.phoneInvalid'))); else cb()
            }, trigger: 'blur'
        }
    ],
    provinceCode: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    regionId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    managerId: [{ required: true, message: t('validation.required'), trigger: 'change' }]
}

// Helpers
const toNullIfEmpty = (v: any) => (typeof v === 'string' && v.trim() === '' ? null : v)
const isFile = (f: any): f is File => typeof File !== 'undefined' && f instanceof File

/** Upload tất cả file mới (có .file) → set imageUrl = temp/xxx */
async function uploadPendingFiles(items: any[]) {
    const slots: { idx: number; file: File }[] = []
    items.forEach((it, i) => { if (it?.file && isFile(it.file)) slots.push({ idx: i, file: it.file }) })
    if (slots.length === 0) return

    const uploaded = await fileStore.uploadTemp(slots.map(s => s.file))
    uploaded.forEach((u, k) => {
        const i = slots[k].idx
        items[i].imageUrl = u.relativePath
        items[i].fileName = u.fileName
        items[i].file = null
    })
}

function closeModal() {
    emit('update:visible', false)
    emit('close')
}

async function onSubmit() {
    const form = (baseDialogRef.value as any)?.formRef
    form.validate(async (valid: boolean) => {
        if (!valid) {
            notificationStore.showToast('error', { key: 'validation.formInvalid' })
            return
        }
        submitting.value = true
        try {
            // 1) Upload toàn bộ file mới ngay trong FileManager
            await fileMgrRef.value?.uploadPendingFiles()

            // 2) Log vùng
            if (props.mode === 'create') {
                formData.value.logRegionComs = [{
                    regionId: formData.value.regionId || '',
                    status: 0,
                    endDate: null,
                    description: 'Phân bổ lần đầu',
                    startedDate: new Date().toISOString()
                }]
            } else if (props.mode === 'edit') {
                if (!Array.isArray(formData.value.logRegionComs)) formData.value.logRegionComs = []
                const existingLog = (formData.value.logRegionComs || []).find(l => l.companyId === formData.value.id && l.status === 0)
                if (existingLog) { existingLog.endDate = new Date().toISOString(); existingLog.status = StatusType.Inactive }
                formData.value.logRegionComs.push({
                    companyId: formData.value.id || null,
                    regionId: formData.value.regionId || '',
                    status: 0,
                    endDate: null,
                    description: existingLog?.region?.regionName ? `Chuyển từ vùng ${existingLog.region.regionName} sang` : 'Phân bổ vùng mới',
                    startedDate: new Date().toISOString()
                })
                    ; (formData.value as any).deletedImageIds = removedImageIds.value
            }

            // 3) Lấy danh sách attachments cuối (create => bỏ hẳn id)
            const attachments = fileMgrRef.value?.packImages(props.mode === 'create') ?? []

            // 4) Payload gọn sạch
            const toNullIfEmpty = (v: any) => (typeof v === 'string' && v.trim() === '' ? null : v)
            const payload: any = {
                ...formData.value,
                managerId: toNullIfEmpty(formData.value.managerId),
                regionId: toNullIfEmpty((formData.value as any).regionId),
                establishmentDate: toNullIfEmpty(formData.value.establishmentDate),
                companyImages: attachments
            }

            emit('submit', payload)
        } finally {
            submitting.value = false
        }
    })
}

function onDelete() {
    emit('delete', formData.value)
}
</script>
