<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="props.loading" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('workingTime.configurationName') }}</label>
                    <el-form-item>
                        <el-input :model-value="props.configurationName" disabled />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('holiday.name') }}</label>
                    <el-form-item prop="name">
                        <el-input v-model="formData.name" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('holiday.date') }}</label>
                    <el-form-item prop="date">
                        <el-date-picker v-model="formData.date" type="date" format="DD-MM-YYYY"
                            value-format="YYYY-MM-DD" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('holiday.type') }}</label>
                    <el-form-item prop="type">
                        <el-select v-model="formData.categoryId" :disabled="isView" placeholder="Select Type">
                            <el-option v-for="type in holidayTypes" :key="type.id" :label="type.categoryName"
                                :value="type.id" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('holiday.frequency') }}</label>
                    <el-form-item prop="frequency">
                        <el-radio-group v-model="formData.frequency" :disabled="isView">
                            <el-radio :label="0">{{ t('holiday.frequencyYearly') }}</el-radio>
                            <el-radio :label="1">{{ t('holiday.frequencyOnce') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('holiday.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="2" :disabled="isView" />
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { HolidayModel } from '@/api/HolidayApi'
import { useHolidayTypeStore } from '@/stores/useHolidayTypeStore';
const holidayTypeStore = useHolidayTypeStore();

const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    holidayData: Partial<HolidayModel> | null,
    configurationName?: string, // nếu muốn hiển thị tên cấu hình
    configurationId?: string, // nếu cần id cấu hình
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
    if (isView.value) return t('holiday.detail')
    if (isEdit.value) return t('holiday.edit')
    if (isCreate.value) return t('holiday.add')
    return ''
})
const holidayTypes = computed(() => holidayTypeStore.holidayTypes);

const baseDialogRef = ref()
const formData = ref<HolidayModel>({
    id: '',
    name: '',
    date: '',
    categoryId: '',
    description: '',
    frequency: 0,
})
watch(
    () => props.holidayData,
    (data) => {
        if (data) {
            formData.value = {
                id: data.id ?? '',
                name: data.name ?? '',
                date: data.date ?? '',
                categoryId: data.categoryId ?? '',
                description: data.description ?? '',
                frequency: data.frequency ?? 0,
                status: data.status ?? 0,
                workingTimeConfigurationId: data.workingTimeConfigurationId ?? undefined,
                createdAt: data.createdAt ?? '',
                createdBy: data.createdBy ?? '',
                isDeleted: data.isDeleted ?? false,
            }
        } else {
            formData.value = {
                name: '',
                date: '',
                categoryId: '',
                description: '',
                frequency: 0,
                status: 0,
                workingTimeConfigurationId: undefined,
            }
        }
    },
    { immediate: true }
)

const rules = {
    name: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    date: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    categoryId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
}
onMounted(async () => {
    await holidayTypeStore.fetchAllHolidayTypes();
});

function closeModal() {
    emit('update:visible', false)
    emit('close')
}
function onSubmit() {
    const form = baseDialogRef.value.formRef
    form.validate((valid: boolean) => {
        if (valid) {
            formData.value.workingTimeConfigurationId = props.configurationId;
            if (isCreate.value) {
                formData.value.id = undefined; // Đặt id rỗng khi tạo mới
            }
            emit('submit', formData.value)
        } else {
            console.error('Validation failed')
        }
    })
}
function onDelete() {
    emit('delete', formData.value)
}
</script>
