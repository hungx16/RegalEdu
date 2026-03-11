<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('event.category') }}</label>
                    <el-form-item prop="category">
                        <el-select v-model="formData.category" :disabled="isView" @change="selectChange">
                            <el-option v-for="opt in categoryOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('event.code') }}</label>
                    <el-form-item prop="eventCode">
                        <el-input v-model="formData.eventCode" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('event.name') }}</label>
                    <el-form-item prop="eventName">
                        <el-input v-model="formData.eventName" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('event.description') }}</label>
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
import { ref, computed, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import type { EventModel } from '@/api/EventApi';
import { useCommonStore } from '@/stores/commonStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue';
import { getEventCategoryOptions } from '@/utils/makeList';
import { EventCategory, StatusType } from '@/types';

const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    eventData: Partial<EventModel> | null
}>();

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close']);
const { t } = useI18n();
const notificationStore = useNotificationStore();
const commonStore = useCommonStore();
const formRef = ref();
const baseDialogRef = ref();
const oldEventCode = ref('');
const oldCategory = ref(0);
const isView = computed(() => props.mode === 'view');
const isEdit = computed(() => props.mode === 'edit');
const isCreate = computed(() => props.mode === 'create');

const formData = ref<Partial<EventModel>>({
    id: '',
    eventCode: '',
    eventName: '',
    description: '',
    category: 0,
    status: 0,
});

const modeTitle = computed(() => {
    if (isView.value) return t('event.detailTitle');
    if (isEdit.value) return t('event.editTitle');
    if (isCreate.value) {
        formData.value.eventCode = commonStore.code ?? '';
    }
    return t('event.addTitle');
});

const rules = {
    eventCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    eventName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    category: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    status: [{ required: true, message: t('validation.required'), trigger: 'change' }],
};

const categoryOptions = getEventCategoryOptions(t);

watch(
    () => props.eventData,
    (data) => {
        if (data) {
            formData.value = { ...data };
            oldEventCode.value = data.eventCode ?? '';
            oldCategory.value = data.category ?? 0;
        } else {
            formData.value = {
                eventCode: '',
                eventName: '',
                description: '',
                category: 0,
                status: StatusType.Active,
            };
        }
    },
    { immediate: true }
);
async function selectChange(value: any) {
    let code = 'SK'
    switch (value) {

        case EventCategory.Report:
            code = 'BC';
            break;
            // case EventCategory.News:
            //     code = 'TT';
            //     break;
            // case EventCategory.Link:
            //     code = 'LK';
            break;
    }

    if (isEdit.value && oldCategory.value === value) {
        formData.value.eventCode = oldEventCode.value;
        return;
    }
    await commonStore.generateCode(code, 'Event', 'EventCode', 4);
    formData.value.eventCode = commonStore.code ?? '';
}
function onSubmit() {
    const form = baseDialogRef.value?.formRef;
    form.validate(async (valid: boolean) => {
        if (valid) {
            emit('submit', formData.value);
        } else {
            notificationStore.showToast('error', { key: 'validation.formInvalid' });
        }
    });
}

function onDelete() {
    emit('delete', formData.value);
}

function closeModal() {
    emit('update:visible', false);
    emit('close');
}
</script>
