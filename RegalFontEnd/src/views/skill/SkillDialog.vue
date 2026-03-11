<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :height="260" :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit"
        @delete="onDelete" @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('skill.code') }}</label>
                    <el-form-item prop="categoryCode">
                        <el-input v-model="formData.categoryCode" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('skill.name') }}</label>
                    <el-form-item prop="categoryName">
                        <el-input v-model="formData.categoryName" :disabled="isView" />
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
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('skill.description') }}</label>
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
import type { SkillModel } from '@/api/SkillApi'
import { useCommonStore } from '@/stores/commonStore';
import { StatusType } from '@/types';
import { useNotificationStore } from '@/stores/notificationStore';
const commonStore = useCommonStore();
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    skillData: Partial<SkillModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()

const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')
const baseDialogRef = ref()
const notificationStore = useNotificationStore()

const modeTitle = computed(() => {
    if (isView.value) return t('skill.detailTitle')
    if (isEdit.value) return t('skill.editTitle')
    if (isCreate.value) {
        formData.value.categoryCode = commonStore.code ?? ''
        return t('skill.addTitle')
    }
    return ''
})

const formRef = ref()
const formData = ref<SkillModel>({
    id: '',
    categoryCode: '',
    categoryName: '',
    description: '',
})

watch(
    () => props.skillData,
    (data) => {
        if (data) {
            formData.value = {
                id: data.id ?? '',
                categoryCode: data.categoryCode ?? '',
                categoryName: data.categoryName ?? '',
                description: data.description ?? '',
                createdAt: data.createdAt,
                createdBy: data.createdBy,
                status: data.status
            }
        } else {
            formData.value = { categoryCode: '', categoryName: '', description: '', status: StatusType.Active }
        }
    },
    { immediate: true }
)

const rules = {
    categoryCode: [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        // Giới hạn tối đa 10 ký tự
        { type: 'string', max: 10, message: t('validation.maxLength', { max: 10 }), trigger: 'change' }
    ], categoryName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
}


function closeModal() {
    emit('update:visible', false)
    emit('close')
}

function onSubmit() {
    const form = baseDialogRef.value?.formRef
    form.validate(async (valid: boolean) => {
        if (valid) {
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
