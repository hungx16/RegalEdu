<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="loading" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('gift.name') }}</label>
                    <el-form-item prop="name">
                        <el-input v-model="formData.name" :disabled="isView" :placeholder="t('gift.namePlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('gift.prices') }}</label>
                    <el-form-item prop="prices">
                        <el-input-number v-model="formData.prices" :min="0" :disabled="isView"
                            :placeholder="t('gift.pricesPlaceholder')" :precision="0" class="w-100" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
                    <el-form-item prop="status">
                        <el-select v-model="formData.status" :disabled="isView"
                            :placeholder="t('common.statusPlaceholder')">
                            <el-option :label="t('common.active')" :value="0" />
                            <el-option :label="t('common.inactive')" :value="1" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('gift.description') }}</label>
                    <el-form-item prop="description">
                        <el-input v-model="formData.description" type="textarea" :rows="3" :disabled="isView"
                            :placeholder="t('gift.descriptionPlaceholder')" />
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useGiftStore } from '@/stores/giftStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { GiftModel } from '@/api/GiftApi';

const props = defineProps<{
    visible: boolean;
    mode: 'create' | 'edit' | 'view';
}>();

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close']);

const { t } = useI18n();
const giftStore = useGiftStore();
const notificationStore = useNotificationStore();

const loading = ref(false);
const baseDialogRef = ref();

const isView = computed(() => props.mode === 'view');
const modeTitle = computed(() => {
    if (props.mode === 'create') return t('gift.add');
    if (props.mode === 'edit') return t('gift.edit');
    return t('gift.view');
});

const defaultFormData: Partial<GiftModel> = {
    name: '',
    prices: 0,
    description: '',
    status: 0, // Mặc định là active
};
const formData = ref({ ...defaultFormData });

watch(
    () => giftStore.selectedGift,
    (data) => {
        if (data && data.id) {
            formData.value = { ...data };
        } else {
            formData.value = { ...defaultFormData };
        }
    },
    { immediate: true }
);

const rules = {
    name: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    prices: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    status: [{ required: true, message: t('validation.required'), trigger: 'change' }],
};

function closeModal() {
    emit('update:visible', false);
    emit('close');
}

function onSubmit() {
    const form = baseDialogRef.value?.formRef;
    form.validate(async (valid: boolean) => {
        if (valid) {
            loading.value = true;
            emit('submit', formData.value);
            loading.value = false;
        } else {
            notificationStore.showToast('error', { key: 'validation.formInvalid' });
        }
    });
}

function onDelete() {
    if (formData.value.id) {
        emit('delete', formData.value.id);
    }
}
</script>