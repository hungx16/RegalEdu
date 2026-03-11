<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :width="computedDialogWidth" :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit"
        @delete="onDelete" @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20" class="form-group-block" justify="start">
                <!-- Thông tin cơ bản -->
                <el-col :span="24">
                    <h5 class="text-gray-600 mb-2">{{ t('employee.basicInfo') }}</h5>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item label-position="top" :label="t('employee.code')" prop="applicationUser.userCode"
                        required>
                        <el-input v-model="formData.applicationUser.userCode" :disabled="isView"
                            placeholder="VD: NV001" />
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('applicationUser.fullName')" label-position="top"
                        prop="applicationUser.fullName" required>
                        <el-input v-model="formData.applicationUser.fullName" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('applicationUser.gender')" label-position="top"
                        prop="applicationUser.gender" required>
                        <el-select v-model="formData.applicationUser.gender" :disabled="isView">
                            <el-option label="Nam" :value="true" />
                            <el-option label="Nữ" :value="false" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="12">
                    <el-form-item :label="t('employee.dateOfBirth')" label-position="top"
                        prop="applicationUser.dateOfBirth">
                        <el-date-picker v-model="formData.applicationUser.dateOfBirth" type="date" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="12">
                    <el-form-item :label="t('applicationUser.phoneNumber')" label-position="top" required
                        prop="applicationUser.phoneNumber">
                        <el-input v-model="formData.applicationUser.phoneNumber" :disabled="isView" />
                    </el-form-item>
                </el-col>


                <!-- Cơ cấu tổ chức -->
                <el-col :span="24">
                    <h5 class="text-gray-600 mt-4 mb-2">{{ t('employee.structureInfo') }}</h5>
                </el-col>
                <el-col :xs="24" :md="12">
                    <el-form-item :label="t('employee.region')" label-position="top" prop="selectedRegionId" required>
                        <!-- Region -->
                        <el-select v-model="formData.selectedRegionId" :disabled="isView" filterable clearable
                            :placeholder="t('employee.selectRegion')">
                            <el-option v-for="item in regionStore.regions" :key="item.id" :label="item.regionName"
                                :value="item.id" />
                        </el-select>

                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="12">
                    <el-form-item :label="t('models.Company')" label-position="top" prop="companyId" required>

                        <!-- Company -->
                        <el-select v-model="formData.companyId" :disabled="isView || !formData.selectedRegionId"
                            filterable clearable :placeholder="t('employee.selectCompany')">
                            <el-option v-for="item in filteredCompanies" :key="item?.id" :label="item?.companyName"
                                :value="item?.id" :disabled="item?.disabled" />
                        </el-select>

                    </el-form-item>
                </el-col>

                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('employee.block')" label-position="top" prop="divisionId" required>
                        <!-- Block (Division) -->
                        <el-select v-model="formData.divisionId" :disabled="isView" filterable clearable>
                            <el-option v-for="item in divisionStore.divisions" :key="item.id" :label="item.divisionName"
                                :value="item.id" :disabled="item?.status === StatusType.Inactive" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('employee.department')" label-position="top" prop="departmentId">
                        <el-select v-model="formData.departmentId" :disabled="isView || !formData.divisionId" filterable
                            clearable>
                            <el-option v-for="item in filteredDepartments" :key="item.id" :label="item.departmentName"
                                :value="item.id" :disabled="item?.disabled" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('employee.position')" label-position="top" prop="positionId" required>
                        <!-- Position -->
                        <el-select v-model="formData.positionId" :disabled="isView || !formData.departmentId" filterable
                            clearable>
                            <el-option v-for="item in filteredPositions" :key="item?.id" :label="item?.positionName"
                                :value="item?.id" :disabled="item?.disabled" />
                        </el-select>
                    </el-form-item>
                </el-col>


                <!-- Thông tin liên hệ -->
                <el-col :span="24">
                    <h5 class="text-gray-600 mt-4 mb-2">{{ t('employee.contactInfo') }}</h5>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('employee.email')" label-position="top" prop="email">
                        <el-input v-model="formData.personalEmail" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('employee.companyEmail')" label-position="top" prop="applicationUser.email"
                        required>
                        <el-input v-model="formData.applicationUser.email" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('employee.province')" label-position="top" prop="province">
                        <!-- Province -->
                        <el-select v-model="formData.applicationUser.provinceCode" :disabled="isView" filterable
                            clearable>
                            <el-option v-for="item in commonStore.provinces" :key="item.provinceCode || item"
                                :label="item.provinceName || item" :value="item.provinceCode || item" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('employee.address')" label-position="top" prop="address">
                        <el-input v-model="formData.applicationUser.address" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- Thông tin công việc -->
                <el-col :span="24">
                    <h5 class="text-gray-600 mt-4 mb-2">{{ t('employee.jobInfo') }}</h5>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('employee.startDate')" label-position="top" prop="employeeStartedDate"
                        required>

                        <el-date-picker v-model="formData.employeeStartedDate" type="date" :disabled="isView"
                            format="DD-MM-YYYY" value-format="YYYY-MM-DD" />
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('employee.endDate')" label-position="top" prop="employeeEndDate">
                        <el-date-picker v-model="formData.employeeEndDate" type="date" :disabled="isView"
                            format="DD-MM-YYYY" value-format="YYYY-MM-DD" />
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('employee.tax')" label-position="top" prop="employeeTax">
                        <el-input v-model="formData.employeeTax" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :xs="24" :md="8">
                    <el-form-item :label="t('employee.newStaffEndDate')" label-position="top" prop="employeeNewEndDate">
                        <el-date-picker v-model="formData.employeeNewEndDate" type="date" :disabled="isView"
                            format="DD-MM-YYYY" value-format="YYYY-MM-DD" />
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
                <!-- Ghi chú -->
                <el-col :span="24">
                    <el-form-item :label="t('employee.note')" label-position="top" prop="note">
                        <el-input v-model="formData.applicationUser.note" type="textarea" :disabled="isView"
                            :rows="3" />
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>



<script setup lang="ts">
import { computed, onMounted, ref, watch, nextTick } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { EmployeeModel } from '@/api/EmployeeApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useRegionStore } from '@/stores/regionStore'
import { useCompanyStore } from '@/stores/companyStore'
import { useDivisionStore } from '@/stores/divisionStore'
import { useDepartmentStore } from '@/stores/departmentStore'
import { usePositionStore } from '@/stores/positionStore'
import { useCommonStore } from '@/stores/commonStore'
import { StatusType } from '@/types'

const regionStore = useRegionStore()
const companyStore = useCompanyStore()
const divisionStore = useDivisionStore()
const departmentStore = useDepartmentStore()
const positionStore = usePositionStore()
const commonStore = useCommonStore()

const windowWidth = ref(window.innerWidth)

const computedDialogWidth = computed(() => {
    return windowWidth.value < 768 ? '100%' : '60%'
})
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    employeeData: Partial<EmployeeModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])

const { t } = useI18n()
const notificationStore = useNotificationStore()

const isView = computed(() => props.mode === 'view')

const modeTitle = computed(() => {
    if (isView.value) return t('employee.detailTitle')
    if (props.mode === 'edit') return t('employee.editTitle')
    if (props.mode === 'create') {
        formData.value.applicationUser.userCode = commonStore.code
    }
    return t('employee.addTitle')
})

const formRef = ref()

const filteredCompanies = computed(() => {
    if (!formData.value.selectedRegionId) return []
    return companyStore.companies
        .map(c => {
            const logs = Array.isArray(c.logRegionComs)
                ? c.logRegionComs.filter(lr => lr.regionId === formData.value.selectedRegionId)
                : []
            if (logs.length === 0) return null

            const hasActive = logs.some(lr => lr.status === 0)
            return {
                ...c,
                disabled: !hasActive
            }
        })
        .filter(Boolean)
})
const filteredDepartments = computed(() => {
    if (!formData.value.divisionId) return []
    return departmentStore.departments
        .filter(d => d.divisionId === formData.value.divisionId)
        .map(d => ({
            ...d,
            disabled: d.status === 1
        }))
})

const filteredPositions = computed(() => {
    if (!formData.value.departmentId) return []
    // Lọc tất cả các liên kết của phòng ban này (từ toàn bộ position)
    const allDepPositions = positionStore.positions.flatMap(p =>
        Array.isArray(p.departmentPositions)
            ? p.departmentPositions.filter(dp => dp.departmentId === formData.value.departmentId)
            : []
    )

    // Map depPosition sang đúng Position, gắn trạng thái enable/disable
    return allDepPositions.map(dp => {
        const pos = positionStore.positions.find(p => p.id === dp.positionId)
        if (!pos) return null
        return {
            ...pos,
            disabled: pos.status !== 0 // chỉ enable nếu liên kết active
        }
    }).filter(Boolean)
})



const formData = ref<EmployeeModel & { selectedRegionId?: string, divisionId?: string }>({
    id: '',
    applicationUser: {
        userCode: '',
        fullName: '',
        dateOfBirth: '',
        phoneNumber: '',
        email: '',
        gender: false,
        provinceCode: '',
        address: '',
        note: '',
    },
    companyId: '',
    company: null,
    positionId: '',
    position: null,
    departmentId: '',
    department: null,
    divisionId: '',
    employeeTax: '',
    employeeStartedDate: '',
    employeeEndDate: '',
    employeeNewEndDate: '',
    createdAt: '',
    createdBy: '',
    isDeleted: false,
    selectedRegionId: '',
    personalEmail: '',
    status: StatusType.Active
})

watch(
    () => props.employeeData,
    async (data) => {
        if (data) {
            let regionId = ''
            if (data.companyId && companyStore.companies.length) {
                const company = companyStore.companies.find(c => c.id === data.companyId)
                regionId = company?.logRegionComs?.[0]?.regionId || ''
            }
            // Gán trước các giá trị cơ bản
            formData.value = {
                id: data.id ?? '',
                applicationUser: {
                    id: data.applicationUserId ?? '',
                    userName: data.applicationUser?.userName ?? '',
                    userCode: data.applicationUser?.userCode ?? '',
                    fullName: data.applicationUser?.fullName ?? '',
                    dateOfBirth: data.applicationUser?.dateOfBirth ?? null,
                    phoneNumber: data.applicationUser?.phoneNumber ?? '',
                    email: data.applicationUser?.email ?? '',
                    gender: data.applicationUser?.gender ?? false,
                    provinceCode: data.applicationUser?.provinceCode ?? '',
                    address: data.applicationUser?.address ?? '',
                    note: data.applicationUser?.note ?? '',
                },
                applicationUserId: data.applicationUserId ?? '',
                companyId: data.companyId ?? '',
                company: data.company ?? null,
                positionId: '', // tạm thời để rỗng
                position: null,
                departmentId: '', // tạm thời để rỗng
                department: null,
                divisionId: data.department?.divisionId ?? '',
                employeeTax: data.employeeTax ?? '',
                employeeStartedDate: data.employeeStartedDate ?? null,
                employeeEndDate: data.employeeEndDate ?? null,
                employeeNewEndDate: data.employeeNewEndDate ?? null,
                createdAt: data.createdAt ?? '',
                createdBy: data.createdBy ?? '',
                isDeleted: data.isDeleted ?? false,
                selectedRegionId: regionId,
                personalEmail: data.personalEmail ?? '',
                status: data.status ?? StatusType.Active
            }
            // Đảm bảo departmentId và positionId được gán lại sau khi divisionId đã được set
            await nextTick()
            formData.value.departmentId = data.departmentId ?? ''
            formData.value.positionId = data.positionId ?? ''
        } else {
            formData.value = {
                applicationUser: {
                    userCode: '',
                    fullName: 'Sale_',
                    dateOfBirth: '2011-01-01',
                    phoneNumber: '0915672312',
                    email: 'sale@gmail.com',
                    gender: false,
                    provinceCode: '04',
                    address: '',
                    note: '',
                    userName: ''
                },
                companyId: '',
                company: null,
                positionId: '',
                position: null,
                departmentId: '',
                department: null,
                divisionId: '',
                employeeTax: '',
                employeeStartedDate: '2012-11-11',
                employeeEndDate: null,
                employeeNewEndDate: null,
                isDeleted: false,
                selectedRegionId: '',
                personalEmail: '',
                status: StatusType.Active
            }
        }
    },
    { immediate: true }
)

function updateWindowWidth() {
    windowWidth.value = window.innerWidth
}
onMounted(async () => {
    window.addEventListener('resize', updateWindowWidth)
    updateWindowWidth()
    if (!positionStore.positions.length) await positionStore.fetchAllPositions()
    if (!divisionStore.divisions.length) await divisionStore.fetchAllDivisions()
    if (!regionStore.regions.length) await regionStore.fetchAllRegions()
    if (!companyStore.companies.length) await companyStore.fetchAllCompanies()
    if (!departmentStore.departments.length) await departmentStore.fetchAllDepartments()
    if (!commonStore.provinces.length) await commonStore.fetchProvinces()
})
watch(
    () => formData.value.selectedRegionId,
    () => {
        const availableCompany = filteredCompanies.value.find(c => c && c.id === formData.value.companyId)
        if (!availableCompany || availableCompany.disabled) {
            formData.value.companyId = ''
        }
    }
)

// Khi đổi khối (division)
watch(
    () => formData.value.divisionId,
    () => {
        const availableDept = filteredDepartments.value.find(d => d.id === formData.value.departmentId)
        if (!availableDept || availableDept.disabled) {
            formData.value.departmentId = ''
        }
        formData.value.positionId = ''
    }
)

// Khi đổi phòng ban (department)
watch(
    () => formData.value.departmentId,
    () => {
        const availablePos = filteredPositions.value.find(p => p && p.id === formData.value.positionId)
        if (!availablePos || availablePos.disabled) {
            formData.value.positionId = ''
        }
    }
)
const rules = {
    userCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    fullName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    companyId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    selectedRegionId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    positionId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    departmentId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    divisionId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    'applicationUser.email': [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        {
            type: 'email',
            message: t('validation.invalidEmail'), // hoặc t('validation.emailFormat')
            trigger: ['blur', 'change'],
        }],
    personalEmail: [
        {
            type: 'email',
            message: t('validation.invalidEmail'), // hoặc t('validation.emailFormat')
            trigger: ['blur', 'change'],
        }],
    'applicationUser.phoneNumber': [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        {
            pattern: /^(0|\+84)[3-9][0-9]{8}$/,
            message: t('validation.invalidPhone'),
            trigger: ['blur', 'change']
        }
    ]

    ,
    employeeStartedDate: [
        { required: true, message: t('validation.required'), trigger: 'change' },
        {
            type: 'date',
            message: t('validation.invalidDate'),
            trigger: ['blur', 'change']
        }
    ],
    employeeTax: [
        // { required: true, message: t('validation.required'), trigger: 'blur' },
        {
            pattern: /^\d{10}(-\d{3})?$/,
            message: t('validation.invalidTax'),
            trigger: ['blur', 'change']
        }
    ]

}

const baseDialogRef = ref()

function closeModal() {
    emit('update:visible', false)
    emit('close')
}

function onSubmit() {
    const form = baseDialogRef.value?.formRef
    form.validate(async (valid: boolean) => {
        if (valid) {
            if (props.mode === 'create') {
                console.log('formData.value', formData.value)
                formData.value.applicationUser.userName = formData.value.applicationUser.email // Đảm bảo userName là rỗng khi tạo mới
            }
            else if (props.mode === 'edit') {
                formData.value.employeeEndDate = formData.value.employeeEndDate
            }
            emit('submit', formData.value)
        } else {
            notificationStore.showToast('error', {
                key: 'validation.formInvalid'
            })
        }
    })
}
function onDelete() {
    emit('delete', formData.value)
}
</script>
>