<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <!-- Mã phòng ban -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('department.code') }}</label>
                    <el-form-item prop="departmentCode">
                        <el-input v-model="formData.departmentCode" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Khối tổ chức (Division) -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('department.division') }}</label>
                    <el-form-item prop="divisionId">
                        <el-select v-model="formData.divisionId" filterable clearable
                            :placeholder="t('department.division')" :disabled="isView">
                            <el-option v-for="d in divisionStore.divisions" :key="d.id" :label="d.divisionName"
                                :value="d.id" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <!-- Tên phòng ban -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('department.name') }}</label>
                    <el-form-item prop="departmentName">
                        <el-input v-model="formData.departmentName" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Tên phòng ban tiếng Anh-->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">English Department Name</label>
                    <el-form-item prop="enDepartmentName">
                        <el-input v-model="formData.enDepartmentName" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- Phòng ban cha -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('department.departmentParent') }}</label>
                    <el-form-item prop="departmentParentId">
                        <el-select v-model="formData.departmentParentId" filterable clearable
                            :placeholder="t('department.departmentParent')" :disabled="isView">
                            <el-option v-for="d in departmentParentOptions" :key="d.id" :label="d.departmentName"
                                :value="d.id" />
                        </el-select>
                    </el-form-item>

                </el-col>
                <!-- Trạng thái -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('department.status') }}</label>
                    <el-form-item prop="status">
                        <el-radio-group v-model="formData.status" :disabled="isView">
                            <el-radio :value="0">{{ t('department.active') }}</el-radio>
                            <el-radio :value="1">{{ t('department.inactive') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
                <!-- Mô tả -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('department.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="2" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Thông tin tạo/chỉnh sửa (View mode) -->
                <el-col :span="12" v-if="isView">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdBy') }}</label>
                    <el-input :value="formData.createdBy || ''" disabled />
                </el-col>
                <el-col :span="12" v-if="isView">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdAt') }}</label>
                    <el-input :value="formatDate(formData.createdAt || '', 'YYYY-MM-DD HH:mm:ss')" disabled />
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { DepartmentModel } from '@/api/DepartmentApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useDivisionStore } from '@/stores/divisionStore'
import { formatDate } from '@/utils/format'
import { useCommonStore } from '@/stores/commonStore'
import { useDepartmentStore } from '@/stores/departmentStore'
const departmentStore = useDepartmentStore()

const departmentParentOptions = computed(() => {
    return departmentStore.departments.filter(
        d => d.id !== formData.value.id // Loại bản ghi đang chỉnh
    )
})
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    departmentData: Partial<DepartmentModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const commonStore = useCommonStore()

const notificationStore = useNotificationStore()
const divisionStore = useDivisionStore()
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
    if (isView.value) return t('department.detailTitle')
    if (isEdit.value) return t('department.editTitle')
    if (isCreate.value) {
        formData.value.departmentCode = commonStore.code ?? ''
    }
    return t('department.addTitle')
})

const formRef = ref()
const loading = ref(false)

const formData = ref<DepartmentModel>({
    id: '',
    departmentCode: '',
    departmentName: '',
    divisionId: '',
    departmentParentId: '',
    status: 0,
    description: '',
    createdAt: '',
    createdBy: ''
})

watch(
    () => props.departmentData,
    (data) => {
        if (data) {
            formData.value = {
                id: data.id ?? '',
                departmentCode: data.departmentCode ?? '',
                departmentName: data.departmentName ?? '',
                divisionId: data.divisionId ?? '',
                departmentParentId: data.departmentParentId ?? null,
                status: data.status ?? 0,
                description: data.description ?? '',
                createdAt: data.createdAt ?? '',
                createdBy: data.createdBy ?? ''
            }
        } else {
            formData.value = {
                departmentCode: '',
                departmentName: '',
                divisionId: '',
                departmentParentId: null,
                status: 0,
                description: '',
            }
        }
    },
    { immediate: true }
)

// Luôn load division khi mở dialog (nếu cần)
watch(() => props.visible, val => {
    if (val && !divisionStore.divisions.length) {
        divisionStore.fetchAllDivisions()
    }
})

const rules = {
    departmentCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    departmentName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    divisionId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    status: [{ required: true, message: t('validation.required'), trigger: 'change' }]
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
