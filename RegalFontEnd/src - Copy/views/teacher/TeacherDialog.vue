<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :width="computedDialogWidth" :mode="currentMode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit"
        @delete="onDelete" @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <!-- Tabs header -->
            <TabbedComponent v-model="activeTab" :tabs="tabs" />

            <!-- TAB 1: Toàn bộ form dữ liệu -->
            <div v-if="activeTab === 'basic'">
                <el-row :gutter="20" class="form-group-block" justify="start">
                    <el-col :span="24">
                        <h5 class="text-gray-600 mb-2">{{ t('teacher.basicInfo') }}</h5>
                    </el-col>

                    <!-- Toàn bộ form của anh giữ nguyên bên dưới -->
                    <!-- Code -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('teacher.code')" prop="applicationUser.userCode"
                            required>
                            <el-input v-model="formData.applicationUser.userCode" :disabled="isView"
                                placeholder="VD: GV001" />
                        </el-form-item>
                    </el-col>

                    <!-- Họ tên -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('applicationUser.fullName')"
                            prop="applicationUser.fullName" required>
                            <el-input v-model="formData.applicationUser.fullName" :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <!-- Nickname -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('teacher.nickname')" prop="teacherNickname">
                            <el-input v-model="formData.teacherNickname" :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <!-- Giới tính -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('applicationUser.gender')"
                            prop="applicationUser.gender" required>
                            <el-select v-model="formData.applicationUser.gender" :disabled="isView">
                                <el-option label="Nam" :value="true" />
                                <el-option label="Nữ" :value="false" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <!-- Nationality -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('teacher.nationality')" prop="teacherNationality">
                            <el-input v-model="formData.applicationUser.nationality" :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <!-- CCCD -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('teacher.idCard')" prop="teacherIdCard">
                            <el-input v-model="formData.applicationUser.identityNumber" :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <!-- Ngày sinh -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('employee.dateOfBirth')"
                            prop="applicationUser.dateOfBirth" required>
                            <el-date-picker v-model="formData.applicationUser.dateOfBirth" type="date"
                                :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <!-- Số điện thoại -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('applicationUser.phoneNumber')"
                            prop="applicationUser.phoneNumber" required>
                            <el-input v-model="formData.applicationUser.phoneNumber" :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <!-- Email -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('applicationUser.email')"
                            prop="applicationUser.email" required>
                            <el-input v-model="formData.applicationUser.email" type="email" :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <!-- Tỉnh / Thành phố -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('employee.province')"
                            prop="applicationUser.provinceCode">
                            <el-select v-model="formData.applicationUser.provinceCode" :disabled="isView" clearable
                                filterable>
                                <el-option v-for="p in commonStore.provinces" :key="p.provinceCode"
                                    :label="p.provinceName" :value="p.provinceCode" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <el-col :span="24">
                        <h5 class="text-gray-600 mb-2">{{ t('teacher.jobInfo') }}</h5>
                    </el-col>

                    <!-- Trình độ chuyên môn -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('teacher.qualifications')"
                            prop="teacherQualifications">
                            <el-input v-model="formData.teacherQualifications" :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <!-- Bằng cấp -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('teacher.degree')" prop="degreeId">
                            <el-select v-model="formData.degreeId" :disabled="isView" clearable>
                                <el-option v-for="d in degreeStore.degrees" :key="d.id" :label="d.degreeName"
                                    :value="d.id" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <!-- Chuyên môn -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('teacher.specialization')"
                            prop="teacherSpecialization">
                            <el-input v-model="formData.teacherSpecialization" :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <!-- Loại hình -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('teacher.workType')" prop="workType" required>
                            <el-select v-model="formData.workType" :disabled="isView" clearable>
                                <el-option v-for="w in workTypeOptions" :key="w.value" :label="w.label"
                                    :value="w.value" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <!-- Ngày vào làm -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('teacher.joinDate')" prop="joinDate" required>
                            <el-date-picker v-model="formData.joinDate" type="date" :disabled="isView"
                                format="DD-MM-YYYY" value-format="YYYY-MM-DD" />
                        </el-form-item>
                    </el-col>

                    <!-- Mức độ ưu tiên -->
                    <el-col :xs="24" :md="8">
                        <el-form-item label-position="top" :label="t('teacher.preferLevel')" prop="preferLevel">
                            <el-select v-model="selectedLevels" multiple clearable filterable :disabled="isView">
                                <el-option v-for="option in levelOptions" :key="option.value" :label="option.label"
                                    :value="option.value" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <!-- Tuỳ chọn -->
                    <el-col :span="24">
                        <el-form-item>
                            <el-checkbox v-model="formData.teachingOutside" :disabled="isView">{{
                                t('teacher.teachingOutside') }}</el-checkbox>
                            <el-checkbox v-model="formData.teacherAssistant" :disabled="isView">{{
                                t('teacher.teacherAssistant') }}</el-checkbox>
                            <el-checkbox v-model="formData.isOnline" :disabled="isView">{{ t('teacher.isOnline')
                                }}</el-checkbox>
                        </el-form-item>
                    </el-col>

                    <!-- Chi nhánh -->
                    <el-col :span="24">
                        <h5 class="text-gray-600 mt-4 mb-2">{{ t('teacher.companyInfo') }}</h5>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item :label="t('teacher.mainCompany')" prop="companyId" required>
                            <el-select v-model="formData.companyId" :disabled="isView" filterable clearable>
                                <el-option v-for="company in companyList" :key="company.id" :label="company.companyName"
                                    :value="company.id" />
                            </el-select>
                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item :label="t('teacher.subCompanies')" prop="subCompanyIds">
                            <el-select v-model="selectedSubCompanies" multiple filterable clearable :disabled="isView">
                                <el-option v-for="company in companyList" :key="company.id" :label="company.companyName"
                                    :value="company.id" />
                            </el-select>
                        </el-form-item>
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

                    <!-- Ghi chú -->
                    <el-col :xs="24">
                        <el-form-item label-position="top" :label="t('teacher.notes')" prop="notes">
                            <el-input v-model="formData.applicationUser.note" type="textarea" :rows="3"
                                :disabled="isView" />
                        </el-form-item>
                    </el-col>
                </el-row>
            </div>
            <!-- TAB 2 & 3: trống (dành cho mở rộng sau) -->
            <div v-else-if="activeTab === 'classInfo'">
                <TeacherClassList :teacher-id="formData.id || ''" />
            </div>
            <!-- TAB 2 & 3: trống (dành cho mở rộng sau) -->
            <div v-else-if="activeTab === 'teachingSchedule'">
                <TeacherSchedule :teacher-id="formData.id || ''" />
            </div>
            <div v-else-if="activeTab === 'attendance'">
                <TeacherWorkBoard :teacher-id="formData.id || ''" />
            </div>

            <div v-else-if="activeTab === 'evaluation'">
                <TeacherEvaluation :teacher-id="formData.id || ''" />
            </div>

        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import TabbedComponent from '@/components/tabbed/TabbedComponent.vue'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import { useCommonStore } from '@/stores/commonStore'
import { useCompanyStore } from '@/stores/companyStore'
import { useDegreeStore } from '@/stores/degreeStore'
import { StatusType, WorkType } from '@/types'
import type { TeacherModel } from '@/api/TeacherApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { getLevelTypeOptions, getWorkTypeOptions } from '@/utils/makeList'
import TeacherSchedule from './TeacherSchedule.vue'
import TeacherClassList from './TeacherClassList.vue'
import TeacherWorkBoard from './TeacherWorkBoard.vue'
import TeacherEvaluation from './TeacherEvaluation.vue'
const { t } = useI18n()
const commonStore = useCommonStore()
const companyStore = useCompanyStore()
const degreeStore = useDegreeStore()
const notificationStore = useNotificationStore()
const props = defineProps<{
    visible: boolean
    mode?: 'create' | 'edit' | 'view'
    loading: boolean
    teacherData: Partial<TeacherModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const activeTab = ref('basic')
const tabs = [{ name: 'basic', label: t('teacher.basicInfo') }, { name: 'classInfo', label: t('teacher.classInfo') }, { name: 'teachingSchedule', label: t('teacher.teachingSchedule') }
    , { name: 'attendance', label: t('teacher.attendance') }, { name: 'salary', label: t('teacher.salary') }, { name: 'evaluation', label: t('teacher.evaluation') }
]

// Thêm computed để kiểm soát mode của BaseDialogForm
const currentMode = computed(() => {
    if (activeTab.value !== 'basic') {
        return 'view' // force view mode khi không ở tab basic
    }
    return props.mode || 'view' // fallback to view mode
})

// Sửa lại isView để dùng currentMode
const isView = computed(() => currentMode.value === 'view')

const windowWidth = ref(window.innerWidth)
const computedDialogWidth = computed(() => (windowWidth.value < 768 ? '100%' : '90%'))



const baseDialogRef = ref()
const formRef = ref()
const SPLIT_TAG = import.meta.env.VITE_SPLIT_TAG || '#$#'
const companyList = ref<any[]>([])
const selectedSubCompanies = ref<string[]>([])
const selectedLevels = ref<number[]>([])
const workTypeOptions = getWorkTypeOptions(t)
const levelOptions = getLevelTypeOptions(t)

const createDefaultApplicationUser = () => ({
    userName: 'eee',
    userCode: '',
    fullName: 'eee',
    dateOfBirth: '',
    phoneNumber: '0915672323',
    email: 'ntvinh194@gmail.comemp',
    gender: false,
    provinceCode: '',
    address: '',
    note: '',
    nationality: '',
    identityNumber: '',
})

const createDefaultFormData = (): TeacherModel => ({
    teacherNickname: '',
    teacherQualifications: '',
    teacherSpecialization: '',
    workType: WorkType.FullTime,
    joinDate: null,
    preferLevel: '',
    teachingOutside: false,
    teacherAssistant: false,
    isOnline: false,
    applicationUser: createDefaultApplicationUser(),
    degreeId: '',
    companyId: '',
    subCompanyIds: '',
    status: StatusType.Active,
})

const formData = ref<TeacherModel>(createDefaultFormData())

watch(
    () => props.teacherData,
    (data) => {
        if (data) {
            formData.value = {
                id: data.id ?? '',
                teacherNickname: data.teacherNickname ?? '',
                teacherQualifications: data.teacherQualifications ?? '',
                teacherSpecialization: data.teacherSpecialization ?? '',
                workType: data.workType ?? WorkType.FullTime,
                joinDate: data.joinDate ?? null,
                preferLevel: data.preferLevel ?? '',
                teachingOutside: data.teachingOutside ?? false,
                teacherAssistant: data.teacherAssistant ?? false,
                isOnline: data.isOnline ?? false,
                applicationUserId: data.applicationUserId ?? '',
                applicationUser: {
                    ...createDefaultApplicationUser(),
                    id: data.applicationUser?.id ?? '',
                    userName: data.applicationUser?.userName ?? '',
                    userCode: data.applicationUser?.userCode ?? '',
                    fullName: data.applicationUser?.fullName ?? '',
                    dateOfBirth: data.applicationUser?.dateOfBirth ?? '',
                    phoneNumber: data.applicationUser?.phoneNumber ?? '',
                    email: data.applicationUser?.email ?? '',
                    gender: data.applicationUser?.gender ?? false,
                    provinceCode: data.applicationUser?.provinceCode ?? '',
                    address: data.applicationUser?.address ?? '',
                    note: data.applicationUser?.note ?? '',
                    nationality: data.applicationUser?.nationality ?? '',
                    identityNumber: data.applicationUser?.identityNumber ?? '',
                },
                degreeId: data.degreeId ?? '',
                companyId: data.companyId ?? '',
                subCompanyIds: data.subCompanyIds ?? '',
                status: data.status ?? StatusType.Active,
                createdAt: data.createdAt ?? '',
                createdBy: data.createdBy ?? '',
                updatedAt: data.updatedAt ?? '',
                updatedBy: data.updatedBy ?? '',
                isDeleted: data.isDeleted ?? false,
            }
            selectedSubCompanies.value = data.subCompanyIds ? data.subCompanyIds.split(SPLIT_TAG) : []
            selectedLevels.value = data.preferLevel ? String(data.preferLevel).split(SPLIT_TAG).map(Number) : []
        } else {
            formData.value = createDefaultFormData()
            selectedSubCompanies.value = []
            selectedLevels.value = []
        }
    },
    { immediate: true }
)

watch(
    () => props.visible,
    (visible) => {
        if (visible) {
            activeTab.value = 'basic'
        }
    }
)

watch(selectedSubCompanies, val => (formData.value.subCompanyIds = val.join(SPLIT_TAG)))
watch(selectedLevels, val => (formData.value.preferLevel = val.join(SPLIT_TAG)))

onMounted(async () => {
    if (!companyStore.companies.length) await companyStore.fetchAllCompanies()
    if (!degreeStore.degrees.length) await degreeStore.fetchAllDegrees()
    if (!commonStore.provinces.length) await commonStore.fetchProvinces()
    companyList.value = companyStore.companies
})

const modeTitle = computed(() => {
    if (isView.value) return t('teacher.detailTitle')
    if (props.mode === 'edit') return t('teacher.editTitle')
    if (props.mode === 'create') formData.value.applicationUser.userCode = commonStore.code
    return t('teacher.addTitle')
})

const rules = {
    'applicationUser.userCode': [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    'applicationUser.fullName': [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    'applicationUser.dateOfBirth': [{ required: true, message: t('validation.required'), trigger: 'change' }],
    'applicationUser.email': [{ type: 'email', message: t('validation.invalidEmail'), trigger: 'blur' }],
    'applicationUser.phoneNumber': [{ required: true, pattern: /^(0|\+84)[3-9][0-9]{8}$/, message: t('validation.invalidPhone'), trigger: 'blur' }],
    companyId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    joinDate: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    workType: [{ required: true, message: t('validation.required'), trigger: 'change' }],
}

function closeModal() {
    emit('update:visible', false)
    emit('close')
}

function onSubmit(e?: Event) {
    const form = baseDialogRef.value?.formRef
    form.validate((valid: boolean) => {
        if (valid) {
            if (props.mode === 'create') {
                delete formData.value.id
                formData.value.applicationUser.userName = formData.value.applicationUser.email
            }
            emit('submit', formData.value)
        } else {
            notificationStore.showToast('error', { key: 'validation.formInvalid' })
        }
    })
}

function onDelete() {
    emit('delete', formData.value)
}
</script>
