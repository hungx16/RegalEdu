<!-- src/components/student/StudentDialog.vue -->
<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="submitting || props.loading" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <!-- Mã học viên -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.code') }}</label>
                    <el-form-item prop="studentCode">
                        <el-input v-model="formData.studentCode" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- Tên học viên -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.name') }}</label>
                    <el-form-item prop="fullName">
                        <el-input v-model="formData.fullName" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- Giới tính -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.gender') }}</label>
                    <el-form-item prop="gender">
                        <el-radio-group v-model="formData.gender" :disabled="isView">
                            <el-radio :value="'Male'">{{ t('common.male') }}</el-radio>
                            <el-radio :value="'Female'">{{ t('common.female') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>

                <!-- Ngày sinh -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.birthDate') }}</label>
                    <el-form-item prop="birthDate">
                        <el-date-picker v-model="formData.birthDate" type="date" :disabled="isView"
                            :placeholder="t('student.birthDatePlaceholder')" format="YYYY-MM-DD"
                            value-format="YYYY-MM-DD" clearable />
                    </el-form-item>
                </el-col>

                <!-- Email -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.email') }}</label>
                    <el-form-item prop="email">
                        <el-input v-model="formData.email" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- SĐT -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.phone') }}</label>
                    <el-form-item prop="phoneNumber">
                        <el-input v-model="formData.phone" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- CMND/CCCD -->
                <!-- <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.identityNumber') }}</label>
                    <el-form-item prop="identityNumber">
                        <el-input v-model="formData." :disabled="isView" />
                    </el-form-item>
                </el-col> -->

                <!-- Nhóm tuổi -->
                <!-- <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.ageGroup') }}</label>
                    <el-form-item prop="ageGroupId" v-if="!isView">
                        <el-select v-model="formData." filterable clearable
                            :placeholder="t('student.ageGroupPlaceholder')">
                            <el-option v-for="opt in ageGroupOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                    <BaseBadge v-else
                        :label="ageGroupOptions.find(opt => opt.value === formData.ageGroupId)?.label || '-'"
                        :rawLabel="true" />
                </el-col> -->

                <!-- Nhân viên tư vấn -->
                <!-- <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.consultant') }}</label>
                    <el-form-item prop="consultantId" v-if="!isView">
                        <el-select v-model="formData.consultantId" filterable clearable
                            :placeholder="t('student.consultantPlaceholder')">
                            <el-option v-for="opt in consultantOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                    <BaseBadge v-else
                        :label="consultantOptions.find(opt => opt.value === formData.consultantId)?.label || '-'"
                        :rawLabel="true" />
                </el-col> -->

                <!-- Chi nhánh -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.company') }}</label>
                    <el-form-item prop="companyId" v-if="!isView">
                        <el-select v-model="formData.companyId" filterable clearable
                            :placeholder="t('student.companyPlaceholder')">
                            <el-option v-for="opt in companyOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                    <BaseBadge v-else
                        :label="companyOptions.find(opt => opt.value === formData.companyId)?.label || '-'"
                        :rawLabel="true" />
                </el-col>

                <!-- Địa chỉ -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.address') }}</label>
                    <el-form-item prop="address">
                        <el-input v-model="formData.address" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- Lý do học -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.reason') }}</label>
                    <el-form-item prop="reason">
                        <el-input v-model="formData.reason" type="textarea" :disabled="isView" />
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { useNotificationStore } from '@/stores/notificationStore'
import { useAgeGroupStore } from '@/stores/ageGroupStore'
import { useEmployeeStore } from '@/stores/employeeStore'
import { useCompanyStore } from '@/stores/companyStore'
import type { StudentModel } from '@/api/StudentApi'

const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    studentData: Partial<StudentModel> | null
}>()

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()

const notificationStore = useNotificationStore()
const ageGroupStore = useAgeGroupStore()
const employeeStore = useEmployeeStore()
const companyStore = useCompanyStore()

const isView = computed(() => props.mode === 'view')
const baseDialogRef = ref()
const submitting = ref(false)

const formData = ref<StudentModel>({
    studentCode: '',
    fullName: '',
    studentStatus: 0
})

const ageGroupOptions = computed(() =>
    ageGroupStore.ageGroups.map(g => ({ label: g.categoryName, value: g.id }))
)
const consultantOptions = computed(() =>
    employeeStore.employees.map(e => ({ label: e.applicationUser.fullName, value: e.id }))
)
const companyOptions = computed(() =>
    companyStore.companies.map(b => ({ label: b.companyName, value: b.id }))
)

const modeTitle = computed(() => {
    if (isView.value) return t('student.detailTitle')
    if (props.mode === 'edit') return t('student.editTitle')
    return t('student.addTitle')
})

watch(() => props.studentData, (data) => {
    if (data) formData.value = { ...formData.value, ...data }
}, { immediate: true })

watch(() => props.visible, async (val) => {
    if (val) {
        await ageGroupStore.fetchAllAgeGroups()
        await employeeStore.fetchAllEmployees()
        await companyStore.fetchAllCompanies()
    }
}, { immediate: true })

const rules = {
    studentCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    fullName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    // phoneNumber: [
    //     { required: true, message: t('validation.required'), trigger: 'blur' },
    //     {
    //         validator: (_r: any, v: string, cb: any) => {
    //             const vnPhoneRegex = /^0\d{9}$/
    //             if (!v) cb(); else if (!vnPhoneRegex.test(v)) cb(new Error(t('validation.phoneInvalid'))); else cb()
    //         }, trigger: 'change'
    //     }
    // ],
    ageGroupId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    companyId: [{ required: true, message: t('validation.required'), trigger: 'change' }]
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
            emit('submit', { ...formData.value })
        } finally {
            submitting.value = false
        }
    })
}

function onDelete() {
    emit('delete', formData.value)
}
</script>
