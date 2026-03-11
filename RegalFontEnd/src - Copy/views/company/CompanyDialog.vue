<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="submitting || props.loading" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <!-- Bật đa ngôn ngữ -->
                <el-col :span="12">
                    <el-form-item>
                        <el-checkbox v-model="formData.isMultilingual" :disabled="isView">
                            {{ t('common.allowMultilingual') }}
                        </el-checkbox>
                    </el-form-item>
                </el-col>
                <!-- Công khai -->
                <el-col :span="12">
                    <el-form-item>
                        <el-checkbox v-model="formData.isPublish" :disabled="isView">
                            {{ t('common.isPublish') }}
                        </el-checkbox>
                    </el-form-item>
                </el-col>
                <!-- CompanyCode -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.code') }}</label>
                    <el-form-item prop="companyCode">
                        <el-input v-model="formData.companyCode" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Manager -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('region.manager') }}</label>
                    <el-form-item prop="managerId" v-if="!isView">
                        <el-select v-model="formData.managerId" filterable clearable>
                            <el-option v-for="opt in employeeOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                    <BaseBadge v-else :label="formData.manager?.applicationUser?.fullName || '-'" :rawLabel="true" />
                </el-col>
                <!-- CompanyName -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.name') }}</label>
                    <el-form-item prop="companyName">
                        <el-input v-model="formData.companyName" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- English CompanyName -->
                <el-col :span="12" v-if="formData.isMultilingual">
                    <label class="required fs-6 fw-semibold mb-2 d-block">English Name</label>
                    <el-form-item prop="enCompanyName">
                        <el-input v-model="formData.enCompanyName" :disabled="isView" :rows="5" />
                    </el-form-item>
                </el-col>
                <!-- Description -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="5" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- English Description -->
                <el-col :span="24" v-if="formData.isMultilingual">
                    <label class="fs-6 fw-semibold mb-2 d-block">English Description</label>
                    <el-form-item prop="enDescription">
                        <el-input v-model="formData.enDescription" :disabled="isView" :rows="5" type="textarea" />
                    </el-form-item>
                </el-col>
                <!-- Address -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.address') }}</label>
                    <el-form-item prop="companyAddress">
                        <el-input v-model="formData.companyAddress" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- English Address -->
                <el-col :span="24" v-if="formData.isMultilingual">
                    <label class="fs-6 fw-semibold mb-2 d-block">English Address</label>
                    <el-form-item prop="enCompanyAddress">
                        <el-input v-model="formData.enCompanyAddress" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- Phone -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.phone') }}</label>
                    <el-form-item prop="companyPhone">
                        <el-input v-model="formData.companyPhone" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- Email -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.email') }}</label>
                    <el-form-item prop="companyEmail">
                        <el-input v-model="formData.companyEmail" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- Establishment Date -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.establishmentDate') }}</label>
                    <el-form-item prop="establishmentDate">
                        <el-date-picker v-model="formData.establishmentDate" type="date" format="YYYY-MM-DD"
                            value-format="YYYY-MM-DD" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- Region -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.region') }}</label>
                    <el-form-item prop="regionId" v-if="!isView">
                        <el-select v-model="formData.regionId" filterable clearable>
                            <el-option v-for="opt in regionOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                    <BaseBadge v-else :label="formData.region?.regionName || '-'" :rawLabel="true" />
                </el-col>
                <!-- Province -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.provinceCode') }}</label>
                    <el-form-item prop="provinceCode" v-if="!isView">
                        <el-select v-model="formData.provinceCode" filterable clearable>
                            <el-option v-for="opt in provinceOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                    <BaseBadge v-else :label="provinceName || '-'" :rawLabel="true" />
                </el-col>

                <!-- Ward -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.wardCode') }}</label>
                    <el-form-item prop="wardCode" v-if="!isView">
                        <el-select v-model="formData.wardCode" filterable clearable :disabled="!formData.provinceCode">
                            <el-option v-for="opt in wardOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                    <BaseBadge v-else :label="wardName || '-'" :rawLabel="true" />
                </el-col>

                <!-- Toạ độ -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">Toạ độ (Lat, Lng)</label>
                    <el-form-item>
                        <div class="latlng-row">
                            <el-input-number v-model="formData.latitude" :precision="6" :step="0.0001" :min="-90"
                                :max="90" placeholder="Lat" :disabled="isView" />
                            <span class="sep">,</span>
                            <el-input-number v-model="formData.longitude" :precision="6" :step="0.0001" :min="-180"
                                :max="180" placeholder="Lng" :disabled="isView" />
                            <el-button class="ml-2" @click="dlgMap = true" :disabled="isView" round>Chọn trên bản
                                đồ</el-button>
                        </div>
                    </el-form-item>
                </el-col>

                <!-- Dialog Map -->
                <el-dialog v-model="dlgMap" title="Chọn vị trí trên bản đồ" width="880px" destroy-on-close
                    @open="onMapOpen">
                    <!-- <MapPicker v-model:modelValueLat="formData.latitude" v-model:modelValueLng="formData.longitude"
                        :center-lat="centerLat" :center-lng="centerLng" :zoom="12" /> -->
                    <MapPicker v-model:lat="formData.latitude" v-model:lng="formData.longitude" height="400px" />

                    <template #footer>
                        <el-button @click="dlgMap = false">Đóng</el-button>
                    </template>
                </el-dialog>
                <!-- working time -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.workingTime') }}</label>
                    <el-form-item prop="workingTime">
                        <el-input v-model="formData.workingTime" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- English working time -->
                <el-col :span="12" v-if="formData.isMultilingual">
                    <label class="required fs-6 fw-semibold mb-2 d-block">English Working Time</label>
                    <el-form-item prop="enWorkingTime">
                        <el-input v-model="formData.enWorkingTime" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Convenience -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.convenience') }}</label>
                    <el-form-item prop="convenience">
                        <TagList v-model="formData.convenience" :maxVisible="10" :maxTags="8" :dismissible="true"
                            :distinct-colors="true" :autoColor="true" />
                    </el-form-item>
                </el-col>
                <!-- English Convenience -->
                <el-col :span="24" v-if="formData.isMultilingual">
                    <label class="fs-6 fw-semibold mb-2 d-block">English Convenience</label>

                    <el-form-item prop="enConvenience">
                        <TagList v-model="formData.enConvenience" :maxVisible="10" :maxTags="8" :dismissible="true"
                            :distinct-colors="true" :autoColor="true" />
                    </el-form-item>
                </el-col>
                <!-- NumberOfStudents -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.numberOfStudents') }}</label>
                    <el-form-item prop="numberOfStudents">
                        <el-input-number v-model="formData.numberOfStudents" :min="0" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- VotingRate -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.votingRate') }}</label>
                    <el-form-item prop="votingRate">
                        <el-input-number v-model="formData.votingRate" :min="0" :max="5" :step="0.1"
                            :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- LearningRoadMapTags -->
                <el-col :span="24" v-if="!isView">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('company.learningRoadMapTags')
                    }}</label>
                    <el-form-item prop="learningRoadMapIds">
                        <el-select v-model="formData.learningRoadMapIds" multiple clearable filterable
                            :collapse-tags="false" :max-collapse-tags="2" placeholder="Chọn phòng ban liên kết"
                            style="width: 100%" popper-class="department-select-dropdown">
                            <!-- Header: Checkbox Chọn tất cả -->
                            <template #header>
                                <el-checkbox v-model="checkAll" :indeterminate="indeterminate" @change="handleCheckAll">
                                    {{ t('common.selectAll') }}
                                </el-checkbox>
                            </template>
                            <!-- Option: danh sách phòng ban -->
                            <el-option v-for="learningItem in learningRoadMapStore.learningRoadMaps"
                                :key="learningItem.id" :label="learningItem.learningRoadMapName"
                                :value="learningItem.id" />
                        </el-select>
                    </el-form-item>


                </el-col>

                <!-- Status -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
                    <el-form-item prop="status">
                        <el-radio-group v-model="formData.status" :disabled="isView">
                            <el-radio :value="0">{{ t('common.active') }}</el-radio>
                            <el-radio :value="1">{{ t('common.inactive') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
                <!-- Trụ sở chính -->
                <el-col :span="12">
                    <el-form-item>
                        <el-checkbox v-model="formData.isHeadQuarters" :disabled="isView">
                            {{ t('company.isHeadQuarters') }}
                        </el-checkbox>
                    </el-form-item>
                </el-col>
                <!-- Images -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('company.images') }}</label>
                    <FileManager ref="fileMgrRef" v-model="(formData as any).companyImages"
                        accept="image/png,image/jpeg,image/jpg,image/webp,image/gif,image/bmp,image/tiff,image/svg"
                        v-model:removedIds="removedImageIds" :fields="imageFields" unique-boolean-key="isCover"
                        :item-title="t('company.image')" :multiple="true" :disabled="isView" />
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>



<script setup lang="ts">
import { computed, nextTick, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { CompanyModel } from '@/api/CompanyApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useCommonStore } from '@/stores/commonStore'
import { useRegionStore } from '@/stores/regionStore'
import { useEmployeeStore } from '@/stores/employeeStore'
import { buildSelectOptions } from '@/utils/publicFunction'
import { StatusType } from '@/types'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import FileManager from '@/components/file-manager/FileManager.vue'
import type { FieldSchema } from '@/api/FileApi'
import TagList from '@/components/tag/TagList.vue'
import { useLearningRoadMapStore } from '@/stores/learningRoadMapStore'
import MapPicker from '@/components/map/MapPicker.vue' // <-- import

const dlgMap = ref(false)
const learningRoadMapStore = useLearningRoadMapStore()
const fileMgrRef = ref<any>(null)
const indeterminate = ref(false)
const checkAll = ref(false)
const onMapOpen = async () => {
    await nextTick()
    // không cần gọi gì thêm vì MapPicker tự invalidateSize rồi,
    // nhưng nếu anh wrap thêm layer khác thì có thể phát sự kiện xuống để gọi lại.
}
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    companyData: Partial<CompanyModel> | null
}>()
const wardOptions = ref<{ label: string; value: string }[]>([])
const wardName = computed(() => {
    const found = wardOptions.value.find(w => w.value === formData.value.wardCode)
    return found?.label || formData.value.wardCode || '-'
})
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
        e => e.applicationUser?.fullName + (e.applicationUser?.status === 1 ? ` (${t('common.inactive')})` : ''),
        e => e.applicationUser?.status === 1
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
// Xử lý chọn tất cả
const handleCheckAll = (val: boolean) => {
    indeterminate.value = false
    if (val) {
        formData.value.learningRoadMapIds = learningRoadMapStore.learningRoadMaps
            .map(dep => dep.id)
            .filter((id): id is string => typeof id === 'string' && id !== null && id !== undefined)
    } else {
        formData.value.learningRoadMapIds = []
    }
}
const formData = ref<Partial<CompanyModel> & { learningRoadMapIds?: string[], learningRoadMaps?: any[]; enConvenience: string | string[]; convenience: string | string[] }>({
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
    wardCode: null,
    companyEmail: null,
    numberOfStudents: null,
    convenience: '',
    enConvenience: '',
    votingRate: null,
    isPublish: false,
    latitude: null,        // thêm
    longitude: null,       // thêm
    isMultilingual: false,
    enCompanyAddress: '',
    learningRoadMapIds: [],
    learningRoadMaps: [],
    companyLearningRoadMaps: [],
    enCompanyName: '',
    enDescription: '',
    workingTime: '',
    enWorkingTime: '',
})

function normalizeImages(arr: any[] | undefined | null) {
    return (arr || []).map((x: any, i: number) => {
        const base: any = {
            path: x.path ?? '',
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
            const activeLog = (data.logRegionComs || []).find(l => l.companyId === data.id && l.status === 0)

            // Lấy danh sách phòng ban đã liên kết từ departmentIds hoặc departmentPositions
            const ids = data.companyLearningRoadMaps
                ? data.companyLearningRoadMaps.map((dp: any) => dp.learningRoadMapId)
                : (data as any).learningRoadMapIds || [];

            // Lấy object phòng ban từ store cho view mode
            const learningRoadMaps = ids.map((depId: string) =>
                learningRoadMapStore.learningRoadMaps.find(dep => dep.id === depId)
            ).filter(Boolean);
            formData.value = {
                id: data.id ?? '',
                companyCode: data.companyCode ?? '',
                companyName: data.companyName ?? '',
                companyAddress: data.companyAddress ?? '',
                companyPhone: data.companyPhone ?? '',
                establishmentDate: data.establishmentDate ?? '',
                provinceCode: data.provinceCode ?? '',
                wardCode: data.wardCode ?? null,
                companyEmail: data.companyEmail ?? null,
                numberOfStudents: data.numberOfStudents ?? null,
                convenience: (data as any).convenience ?? formData.value.convenience ?? '',
                enConvenience: (data as any).enConvenience ?? formData.value.enConvenience ?? '',
                votingRate: data.votingRate ?? null,
                managerId: data.managerId ?? '',
                learningRoadMaps: learningRoadMaps,
                learningRoadMapIds: ids,
                companyLearningRoadMaps: data.companyLearningRoadMaps ?? [],
                manager: data.manager ?? null,
                regionId: activeLog?.regionId || '',
                region: activeLog?.region || null,
                createdAt: data.createdAt ?? '',
                createdBy: data.createdBy ?? '',
                isPublish: data.isPublish ?? false,
                isDeleted: data.isDeleted ?? false,
                status: data.status ?? 0,
                logRegionComs: data.logRegionComs ?? [],
                companyImages: normalizeImages(data.companyImages),
                latitude: data.latitude ?? null,    // thêm
                longitude: data.longitude ?? null,  // thêm
                workingTime: data.workingTime ?? '',
                enWorkingTime: data.enWorkingTime ?? '',
                description: data.description ?? '',
                enDescription: data.enDescription ?? '',
                isMultilingual: data.isMultilingual ?? false,
                enCompanyAddress: data.enCompanyAddress ?? '',
                enCompanyName: data.enCompanyName ?? '',
                isHeadQuarters: data.isHeadQuarters ?? false,

            }
            removedImageIds.value = []
        } else {
            // reset khi không có data (trường hợp create)
            formData.value = {
                companyCode: '',
                companyName: '',
                companyAddress: '',
                companyPhone: '',
                establishmentDate: '',
                provinceCode: '',
                wardCode: null,
                companyEmail: null,
                numberOfStudents: null,
                convenience: '',
                votingRate: null,
                managerId: '',
                manager: null,
                regionId: '',
                region: null,
                isPublish: false,
                status: 0,
                logRegionComs: [],
                companyImages: [],
                latitude: null,        // thêm
                longitude: null,       // thêm
                enConvenience: '',
                isMultilingual: false,
                enCompanyAddress: '',
                enCompanyName: '',
                enDescription: '',
                isHeadQuarters: false,
            } as any
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
    companyEmail: [
        { required: true, type: 'email', message: t('validation.invalidEmail'), trigger: 'blur' }
    ],
    // managerId: [{ required: true, message: t('validation.required'), trigger: 'change' }]
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

            formData.value.companyLearningRoadMaps = (formData.value.learningRoadMapIds || []).map(learningId => ({
                learningRoadMapId: learningId,
                companyId: formData.value.id || undefined // thêm nếu cần cho BE, không có cũng được khi thêm mới
            }))
            const payload: any = {
                ...formData.value,
                latitude: formData.value.latitude != null ? Number(formData.value.latitude) : null,
                longitude: formData.value.longitude != null ? Number(formData.value.longitude) : null,
                managerId: toNullIfEmpty(formData.value.managerId),
                regionId: toNullIfEmpty((formData.value as any).regionId),
                establishmentDate: toNullIfEmpty(formData.value.establishmentDate),
                companyImages: attachments
            }
            // console.log('Submitting payload:', payload);

            emit('submit', payload)
        } finally {
            submitting.value = false
        }
    })
}
watch(
    () => formData.value.provinceCode,
    async (newVal) => {
        if (!newVal) {
            wardOptions.value = []
            formData.value.wardCode = null
            return
        }

        try {
            // ✅ gọi API khi user chọn Province
            await commonStore.fetchWards(newVal)

            wardOptions.value = commonStore.wards.map((w: any) => ({
                label: w.wardName,
                value: w.wardCode
            }))

            // nếu wardCode hiện tại không nằm trong options → reset
            if (!wardOptions.value.find(w => w.value === formData.value.wardCode)) {
                formData.value.wardCode = null
            }
        } catch (err) {
            console.error('Load wards error:', err)
            wardOptions.value = []
            formData.value.wardCode = null
        }
    }
)

onMounted(async () => {
    await learningRoadMapStore.fetchAllLearningRoadMaps()
})

function onDelete() {
    emit('delete', formData.value)
}
</script>
