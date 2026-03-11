<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
        :height="250" @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('degree.name') }}</label>
                    <el-form-item prop="degreeName">
                        <el-input v-model="formData.degreeName" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('degree.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="2" :disabled="isView" />
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
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { DegreeModel } from '@/api/DegreeApi'
import { StatusType } from '@/types';
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    degreeData: Partial<DegreeModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()

const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
    if (isView.value) return t('degree.detailTitle')
    if (isEdit.value) return t('degree.editTitle')
    if (isCreate.value) return t('degree.addTitle')
    return ''
})

const formRef = ref()
const formData = ref<DegreeModel>({
    id: '',
    degreeName: '',
    description: ''
})
watch(
    () => props.degreeData,
    (data) => {
        if (data) {
            formData.value = {
                ...data,
                degreeName: data.degreeName ?? '',
                description: data.description ?? '',
                status: data.status ?? StatusType.Active
            }
        } else {
            formData.value = { degreeName: '', description: '', status: StatusType.Active }
        }
    },
    { immediate: true }
)

const rules = {
    degreeName: [{ required: true, message: t('validation.required'), trigger: 'blur' }]
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
