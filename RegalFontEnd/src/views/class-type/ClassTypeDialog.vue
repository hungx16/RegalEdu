<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('classType.code') }}</label>
                    <el-form-item prop="classTypeCode">
                        <el-input v-model="formData.classTypeCode" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('classType.name') }}</label>
                    <el-form-item prop="classTypeName">
                        <el-input v-model="formData.classTypeName" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('classType.sessionsPerWeek') }}</label>
                    <el-form-item prop="sessionsPerWeek">
                        <el-input-number :min="1" v-model="formData.sessionsPerWeek" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('classType.hoursPerSession') }}</label>
                    <el-form-item prop="hoursPerSession">
                        <el-input-number :min="1" v-model="formData.hoursPerSession" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('classType.minStudents') }}</label>
                    <el-form-item prop="minStudents">
                        <el-input-number v-model="formData.minStudents" :min="0" :max="formData.maxStudents"
                            :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('classType.maxStudents') }}</label>
                    <el-form-item prop="maxStudents">
                        <el-input-number v-model="formData.maxStudents" :min="formData.minStudents"
                            :disabled="isView" />
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
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('ageGroup.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="2" :disabled="isView" />
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { ClassTypeModel } from '@/api/ClassTypeApi'
import { StatusType } from '@/types';
import { useCommonStore } from '@/stores/commonStore';
const commonStore = useCommonStore();

const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    classTypeData: Partial<ClassTypeModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
    if (isView.value) return t('classType.detailTitle')
    if (isEdit.value) return t('classType.editTitle')
    if (isCreate.value) {
        formData.value.classTypeCode = commonStore.code ?? ''
        return t('classType.addTitle')
    }
    return ''
})

const formRef = ref()
const formData = ref<ClassTypeModel>({
    id: '',
    classTypeCode: '',
    classTypeName: '',
    description: '',
    sessionsPerWeek: 0,
    hoursPerSession: 0,
    maxStudents: 0,
    minStudents: 0,
})
watch(
    () => props.classTypeData,
    (data) => {
        if (data) {
            formData.value = {
                id: data.id ?? '',
                classTypeCode: data.classTypeCode ?? '',
                classTypeName: data.classTypeName ?? '',
                description: data.description ?? '',
                sessionsPerWeek: data.sessionsPerWeek ?? 0,
                hoursPerSession: data.hoursPerSession ?? 0,
                maxStudents: data.maxStudents ?? 0,
                minStudents: data.minStudents ?? 0,
                createdAt: data.createdAt ?? '',
                createdBy: data.createdBy ?? '',
                status: data.status ?? 0,
            }
        } else {
            formData.value = {
                classTypeCode: '',
                classTypeName: '',
                description: '',
                sessionsPerWeek: 0,
                hoursPerSession: 0,
                maxStudents: 0,
                minStudents: 0,
                status: StatusType.Active,
            }
        }
    },
    { immediate: true }
)

const rules = {
    classTypeCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    classTypeName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    sessionsPerWeek: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    hoursPerSession: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    minStudents: [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        { type: 'number', message: t('validation.invalidNumber'), trigger: 'blur' }
    ],
    maxStudents: [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        { type: 'number', message: t('validation.invalidNumber'), trigger: 'blur' },
        { validator: validateFromTo, trigger: 'blur' }
    ],
}
// Hàm kiểm tra validate cho từ và đến
function validateFromTo(rule: any, value: any, callback: any) {
    if (value < (formData.value.minStudents ?? 0)) {
        callback(new Error(t('classType.toGreaterThanMinStudents')));
    } else {
        callback();
    }
}
function closeModal() {
    emit('update:visible', false)
    emit('close')
}
function onSubmit() {
    emit('submit', formData.value)
}
function onDelete() {
    emit('delete', formData.value)
}
</script>
