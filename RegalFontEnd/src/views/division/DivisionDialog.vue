<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('division.code') }}</label>
                    <el-form-item prop="divisionCode">
                        <el-input v-model="formData.divisionCode" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('division.level') }}</label>
                    <el-form-item prop="divisionLevel" v-if="!isView">
                        <el-input-number v-model="formData.divisionLevel" :min="1" :disabled="isView" />
                    </el-form-item>
                    <BaseBadge :label="formData.divisionLevel" v-if="isView"
                        :color="formData.divisionLevel === 1 ? 'green' : formData.divisionLevel === 2 ? 'blue' : 'purple'"
                        displayType="level" />
                </el-col>
                <el-col :span="12" v-if="!isView">
                    <label class="fs-6 mb-2 d-block">{{ t('division.codePlaceholder') }}</label>
                </el-col>
                <el-col :span="12" v-if="!isView">
                    <label class="fs-6  mb-2 d-block">{{ t('division.levelPlaceholder') }}</label>
                </el-col>
                <el-col :span="isView ? 12 : 24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('division.name') }}</label>
                    <el-form-item prop="divisionName">
                        <el-input v-model="formData.divisionName" :disabled="isView"
                            :placeholder="t('division.namePlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col v-if="isView" :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('division.departmentNumber') }}</label>
                    <BaseBadge :label="formData.departments?.length || 0" color="deepPurple" displayType="department" />

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
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('division.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="2" :disabled="isView"
                            :placeholder="t('division.descriptionPlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col :span="12" v-if="isView">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdBy') }}</label>
                    <BaseBadge :label="formData.createdBy || ''" color="purple" />

                </el-col>
                <el-col :span="12" v-if="isView">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdAt') }}</label>
                    <el-form-item>
                        <el-input :value="formatDate(formData.createdAt || '', 'YYYY-MM-DD HH:mm:ss')"
                            :disabled="true" />
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
import type { DivisionModel } from '@/api/DivisionApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useCommonStore } from '@/stores/commonStore'
import { formatDate } from '@/utils/format'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    divisionData: Partial<DivisionModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const notificationStore = useNotificationStore()
const commonStore = useCommonStore()
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
    if (isView.value) return t('division.detailTitle')
    if (isEdit.value) return t('division.editTitle')
    if (isCreate.value) {
        formData.value.divisionCode = commonStore.code ?? ''
    }
    return t('division.addTitle')
})

const formRef = ref()

const formData = ref<DivisionModel>({
    id: '',
    divisionCode: '',
    divisionName: '',
    divisionLevel: 1,
    status: 0,
    description: '',
    createdAt: '',
    isDeleted: false,
    departments: [],
})

watch(
    () => props.divisionData,
    (data) => {
        if (data) {
            formData.value = {
                id: data.id ?? '',
                divisionCode: data.divisionCode ?? '',
                divisionName: data.divisionName ?? '',
                divisionLevel: data.divisionLevel ?? 1,
                status: data.status ?? 0,
                description: data.description ?? '',
                createdAt: data.createdAt ?? '',
                createdBy: data.createdBy ?? '',
                departments: data.departments ?? [],
            }
        } else {
            formData.value = {
                divisionCode: '',
                divisionName: '',
                divisionLevel: 1,
                status: 0,
                description: ''
            }
        }
    },
    { immediate: true }
)

const rules = {
    divisionCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    divisionName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    divisionLevel: [{ required: true, message: t('validation.required'), trigger: 'change' }],
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
