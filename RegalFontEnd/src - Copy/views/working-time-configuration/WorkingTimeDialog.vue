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
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('workingTime.name') }}</label>
                    <el-form-item prop="name">
                        <el-input v-model="formData.name" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('workingTime.dayOfWeek') }}</label>
                    <el-form-item prop="dayOfWeek">
                        <el-select v-model="formData.dayOfWeek" :disabled="true">
                            <el-option v-for="(d, idx) in weekdays" :key="idx" :label="d" :value="idx" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('workingTime.startTime') }}</label>
                    <el-form-item prop="startTime">
                        <el-time-picker v-model="formData.startTime" :disabled="isView" format="HH:mm"
                            value-format="HH:mm:00" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('workingTime.endTime') }}</label>
                    <el-form-item prop="endTime">
                        <el-time-picker v-model="formData.endTime" :disabled="isView" format="HH:mm"
                            value-format="HH:mm:00" />
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('workingTime.isWorkingDay') }}</label>
                    <el-form-item prop="isWorkingDay">
                        <el-radio-group v-model="formData.isWorkingDay" :disabled="isView">
                            <el-radio :value="true">{{ t('common.yes') }}</el-radio>
                            <el-radio :value="false">{{ t('common.no') }}</el-radio>
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
import type { WorkingTimeModel } from '@/api/WorkingTimeApi'
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    workingTimeData: Partial<WorkingTimeModel> | null,
    configurationName?: string, // nếu muốn hiển thị tên cấu hình
    configurationId?: string, // nếu cần id cấu hình
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')
const weekdays = ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7']

const modeTitle = computed(() => {
    if (isView.value) return t('workingTime.detailTitle')
    if (isEdit.value) return t('workingTime.editTitle')
    if (isCreate.value) return t('workingTime.addTitle')
    return ''
})
const baseDialogRef = ref()

const formData = ref<WorkingTimeModel>({
    id: '',
    name: '',
    startTime: '',
    endTime: '',
    dayOfWeek: 1,
    isWorkingDay: true,
    workingTimeConfigurationId: undefined,
})
watch(
    () => props.workingTimeData,
    (data) => {
        if (data) {
            formData.value = {
                id: data.id ?? '',
                name: data.name ?? '',
                startTime: data.startTime ?? '',
                endTime: data.endTime ?? '',
                dayOfWeek: data.dayOfWeek ?? 1,
                isWorkingDay: data.isWorkingDay ?? true,
                workingTimeConfigurationId: data.workingTimeConfigurationId ?? undefined,
            }
        } else {
            formData.value = {
                name: '',
                startTime: '',
                endTime: '',
                dayOfWeek: 1,
                isWorkingDay: true,
                workingTimeConfigurationId: undefined,
            }
        }
    },
    { immediate: true }
)

const rules = {
    name: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    startTime: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    endTime: [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        {
            validator: validateTime,
            trigger: 'change'
        }
    ],
    dayOfWeek: [{ required: true, message: t('validation.required'), trigger: 'change' }],
};

function validateTime(_: any, value: string, callback: Function) {
    const start = formData.value.startTime;
    const end = value;

    // Chuyển "HH:mm:ss" thành số phút trong ngày để so sánh
    function toMinutes(str: string) {
        if (!str) return 0;
        const [h, m, s] = str.split(':').map(Number);
        return h * 60 + m + (s ? s / 60 : 0);
    }

    if (start && end && toMinutes(start) >= toMinutes(end)) {
        callback(new Error(t('validation.startTimeMustBeLessThanEndTime')));
    } else {
        callback();
    }
}


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
