<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('ageGroup.code') }}</label>
                    <el-form-item prop="categoryCode">
                        <el-input v-model="formData.categoryCode" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('ageGroup.name') }}</label>
                    <el-form-item prop="categoryName">
                        <el-input v-model="formData.categoryName" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('ageGroup.enName') }}</label>
                    <el-form-item prop="categoryName">
                        <el-input v-model="formData.enCategoryName" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Thêm trường From và To -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('ageGroup.from') }}</label>
                    <el-form-item prop="from">
                        <el-input-number v-model="formData.from" :min="0" :max="formData.to" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('ageGroup.to') }}</label>
                    <el-form-item prop="to">
                        <el-input-number v-model="formData.to" :min="formData.from" :disabled="isView" />
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
import type { AgeGroupModel } from '@/api/AgeGroupApi'
import { useCommonStore } from '@/stores/commonStore';
import { StatusType } from '@/types';
const commonStore = useCommonStore();
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    ageGroupData: Partial<AgeGroupModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()

const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
    if (isView.value) return t('ageGroup.detailTitle')
    if (isEdit.value) return t('ageGroup.editTitle')
    if (isCreate.value) {
        formData.value.categoryCode = commonStore.code ?? ''
        return t('ageGroup.addTitle')
    }
    return ''
})

const formRef = ref()
const formData = ref<AgeGroupModel>({
    id: '',
    categoryCode: '',
    categoryName: '',
    enCategoryName: '',
    description: '',
    from: 0,
    to: 0
})

watch(
    () => props.ageGroupData,
    (data) => {
        if (data) {
            formData.value = {
                id: data.id ?? '',
                categoryCode: data.categoryCode ?? '',
                categoryName: data.categoryName ?? '',
                description: data.description ?? '',
                from: data.from ?? 0,
                to: data.to ?? 0,
                createdAt: data.createdAt,
                createdBy: data.createdBy,
                status: data.status,
                enCategoryName: data.enCategoryName ?? ''
            }
        } else {
            formData.value = { categoryCode: '', categoryName: '', description: '', from: 0, to: 0, status: StatusType.Active }
        }
    },
    { immediate: true }
)

const rules = {
    categoryCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    categoryName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    from: [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        { type: 'number', message: t('validation.invalidNumber'), trigger: 'blur' }
    ],
    to: [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        { type: 'number', message: t('validation.invalidNumber'), trigger: 'blur' },
        { validator: validateFromTo, trigger: 'blur' }
    ],
}

// Hàm kiểm tra validate cho từ và đến
function validateFromTo(rule: any, value: any, callback: any) {
    if (value <= (formData.value.from ?? 0)) {
        callback(new Error(t('ageGroup.toGreaterThanFrom')));
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
