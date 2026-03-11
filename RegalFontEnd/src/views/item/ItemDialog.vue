<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
        :height="230" @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('item.code') }}</label>
                    <el-form-item prop="itemCode">
                        <el-input v-model="formData.itemCode" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('item.name') }}</label>
                    <el-form-item prop="itemName">
                        <el-input v-model="formData.itemName" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('item.price') }}</label>
                    <el-form-item prop="price">
                        <CurrencyInput v-model="formData.price" :disabled="isView" locale="vi-VN" currency="VND" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('item.quantity') }}</label>
                    <el-form-item prop="quantity">
                        <el-input-number v-model="formData.quantity" :disabled="isView" :min="0" />
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
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue';
import type { ItemModel } from '@/api/ItemApi';
import CurrencyInput from '@/components/currency-input/CurrencyInput.vue';
import { StatusType } from '@/types';
import { useCommonStore } from '@/stores/commonStore';

const props = defineProps<{
    visible: boolean;
    mode?: 'create' | 'edit' | 'view';
    loading: boolean;
    itemData: Partial<ItemModel> | null;
}>();

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close']);
const { t } = useI18n();
const commonStore = useCommonStore();

const isView = computed(() => props.mode === 'view');

const modeTitle = computed(() => {
    if (props.mode === 'create') {
        formData.value.itemCode = commonStore.code ?? '';
        return t('item.addTitle');
    }
    if (props.mode === 'edit') {
        return t('item.editTitle');
    }
    if (props.mode === 'view') {
        return t('item.detailTitle');
    }
    return '';
});

const formRef = ref();
const formData = ref<ItemModel>({
    id: '',
    itemCode: '',
    itemName: '',
    price: 0,
    quantity: 0,
});

watch(() => props.itemData, (data) => {
    if (data) {
        formData.value = {
            id: data.id ?? '',
            itemCode: data.itemCode ?? '',
            itemName: data.itemName ?? '',
            price: data.price ?? 0,
            quantity: data.quantity ?? 0,
            status: data.status ?? 0,
            createdAt: data.createdAt ?? '',
            createdBy: data.createdBy ?? '',
        } as ItemModel;
    } else formData.value = { itemCode: '', itemName: '', price: 0, quantity: 0, status: StatusType.Active } as ItemModel;
}, { immediate: true });

const rules = {
    itemCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    itemName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
};

function closeModal() { emit('update:visible', false); emit('close'); }
function onSubmit() { emit('submit', formData.value); }
function onDelete() { emit('delete', formData.value); }
</script>
