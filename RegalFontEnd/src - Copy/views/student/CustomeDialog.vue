<!-- src/components/student/StudentDialog.vue -->
<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="loading" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.code') }}</label>
                    <el-form-item prop="studentCode">
                        <el-input v-model="formData.studentCode" :disabled="isView || isEdit"
                            :placeholder="t('student.codePlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.name') }}</label>
                    <el-form-item prop="fullName">
                        <el-input v-model="formData.fullName" :disabled="isView"
                            :placeholder="t('student.namePlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.phone') }}</label>
                    <el-form-item prop="phone">
                        <el-input v-model="formData.phone" :disabled="isView"
                            :placeholder="t('student.phonePlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.email') }}</label>
                    <el-form-item prop="email">
                        <el-input v-model="formData.email" :disabled="isView"
                            :placeholder="t('student.emailPlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.source') }}</label>
                    <el-form-item prop="leadSource">
                        <el-select v-model="formData.leadSource" :disabled="isView" class="w-100">
                            <el-option :label="t('student.source.website')" value="Website" />
                            <el-option :label="t('student.source.facebook')" value="Facebook" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.priority') }}</label>
                    <el-form-item prop="priority">
                        <el-select v-model="formData.priority" :disabled="isView" class="w-100">
                            <el-option :label="t('common.low')" :value="0" />
                            <el-option :label="t('common.medium')" :value="1" />
                            <el-option :label="t('common.high')" :value="2" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.expectedBudget') }}</label>
                    <el-form-item prop="expectedBudget">
                        <el-input-number v-model="formData.expectedBudget" :min="0" :disabled="isView" class="w-100" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.expectedStartDate') }}</label>
                    <el-form-item prop="expectedStartDate">
                        <el-date-picker v-model="formData.expectedStartDate" type="date" :disabled="isView"
                            format="DD/MM/YYYY" value-format="YYYY-MM-DD" class="w-100" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.age') }}</label>
                    <el-form-item prop="age">
                        <el-input-number v-model="formData.age" :min="0" :disabled="isView" class="w-100" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.gender') }}</label>
                    <el-form-item prop="gender">
                        <el-select v-model="formData.gender" :disabled="isView" class="w-100">
                            <el-option :label="t('common.male')" value="Nam" />
                            <el-option :label="t('common.female')" value="Nữ" />
                            <el-option :label="t('common.other')" value="Khác" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.advisor') }}</label>
                    <el-form-item prop="employeeId">
                        <el-select v-model="formData.employeeId" :disabled="isView" clearable filterable class="w-100">
                            <el-option v-for="employee in employeeStore.employees" :key="employee.id"
                                :label="employee.applicationUser?.fullName" :value="employee.id" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.address') }}</label>
                    <el-form-item prop="address">
                        <el-input v-model="formData.address" type="textarea" :rows="2" :disabled="isView"
                            :placeholder="t('student.addressPlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.note') }}</label>
                    <el-form-item prop="reason">
                        <el-input v-model="formData.reason" type="textarea" :rows="2" :disabled="isView"
                            :placeholder="t('student.notePlaceholder')" />
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
import type { StudentModel } from '@/api/StudentApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useEmployeeStore } from '@/stores/employeeStore'

const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    studentData: Partial<StudentModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const employeeStore = useEmployeeStore()
const notificationStore = useNotificationStore()
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
    if (isView.value) return t('student.detailTitle')
    if (isEdit.value) return t('student.editTitle')
    return t('student.addTitle')
})

const baseDialogRef = ref()
const loading = ref(false)

const defaultFormData: Partial<StudentModel & { priority: number, expectedBudget: number }> = {
    studentCode: '',
    fullName: '',
    phone: '',
    gender: 'Nam',
    age: 0,
    leadSource: 'Website',
    englishName: 'Elsa',
    employeeId: null,
    expectedBudget: 0, // Trường giả định từ hình ảnh
    priority: 1, // Trường giả định từ hình ảnh
}

const formData = ref<Partial<StudentModel & { priority: number, expectedBudget: number }>>({ ...defaultFormData })

watch(
    () => props.studentData,
    (data) => {
        if (data && data.id) {
            formData.value = {
                ...data,
                priority: 1, // Cần ánh xạ từ API
                expectedBudget: 15000000, // Cần ánh xạ từ API
            } as any;
        } else {
            formData.value = { ...defaultFormData }
        }
    },
    { immediate: true }
)

const rules = {
    studentCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    fullName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    phone: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    leadSource: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    employeeId: [{ required: false, message: t('validation.required'), trigger: 'change' }],
}

function closeModal() {
    emit('update:visible', false)
    emit('close')
}

function onSubmit() {
    const form = baseDialogRef.value?.formRef
    form.validate(async (valid: boolean) => {
        if (valid) {
            loading.value = true
            emit('submit', formData.value)
            loading.value = false
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