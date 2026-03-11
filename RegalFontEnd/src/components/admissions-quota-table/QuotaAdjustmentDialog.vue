<!-- src/components/admissions-quota/QuotaAdjustmentDialog.vue -->
<template>
    <BaseDialogForm :visible="visible" :title="computedTitle" :mode="mode" :form-data="formData" :rules="rules"
        :loading="props.loading" :submit-disabled="submitDisabled" class="quota-adjustment-dialog"
        @update:visible="$emit('update:visible', $event)" @submit="emitSubmit" @close="$emit('close')">
        <template #form="{ mode }">
            <div class="row g-4">
                <!-- Vùng/Chi nhánh -->
                <div class="col-12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">
                        {{ scope === AdjustmentScope.Company ? t('company.name') : t('region.name') }}
                    </label>
                    <el-input :model-value="targetName" :disabled="true" />
                </div>

                <!-- Doanh thu trước / sau -->
                <div class="col-12 col-md-6">
                    <label class="required fs-6 fw-semibold mb-2 d-block">
                        {{ t('adjustment.totalBefore') }}
                    </label>
                    <CurrencyInput :modelValue="formData.totalQuotaBefore" locale="vi-VN" currency="VND"
                        :disabled="true" align="right" />
                </div>
                <div class="col-12 col-md-6">
                    <label class="required fs-6 fw-semibold mb-2 d-block">
                        {{ t('adjustment.totalAfter') }}
                    </label>
                    <el-form-item prop="totalQuotaAfter">
                        <CurrencyInput v-model="formData.totalQuotaAfter" locale="vi-VN" currency="VND"
                            :disabled="isView || loading" align="right" @change="() => void 0" />
                    </el-form-item>
                </div>

                <!-- Chênh lệch -->
                <div class="col-12">
                    <div class="d-flex align-items-center gap-2">
                        <span class="fs-7 text-body-secondary">{{ t('adjustment.delta') }}:</span>
                        <el-tag :type="delta >= 0 ? 'success' : 'danger'">
                            {{ signedDelta }}
                        </el-tag>
                        <small class="text-body-secondary">{{ t('adjustment.deltaHint') }}</small>
                    </div>
                </div>

                <!-- Ghi chú -->
                <div class="col-12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.note') }}</label>
                    <el-input v-model="formData.reason" :disabled="isView || loading" type="textarea" :rows="3"
                        :placeholder="t('adjustment.reasonPlaceholder')" maxlength="500" show-word-limit />
                </div>
            </div>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import CurrencyInput from '@/components/currency-input/CurrencyInput.vue';
import { AdjustmentScope } from '@/types'; // enum Region=1, Company=2
import BaseDialogForm from '../base-dialog-form/BaseDialogForm.vue';


const { t } = useI18n();
/* Props */
interface Props {
    visible: boolean;
    loading?: boolean;
    mode?: 'create' | 'edit' | 'view';

    scope: AdjustmentScope;              // Region / Company
    targetId: string;                    // regionId OR companyId (informational)
    targetName: string;                  // region/company display name

    admissionsQuotaRegionId?: string | null;   // Id AQR (khi scope = Region)
    admissionsQuotaCompanyId?: string | null;  // Id AQC (khi scope = Company)

    totalQuotaBefore?: number | null;
    totalQuotaAfter?: number | null;
}
const props = withDefaults(defineProps<Props>(), {
    loading: false,
    mode: 'create',
    totalQuotaBefore: 0,
    totalQuotaAfter: null,
    admissionsQuotaRegionId: null,
    admissionsQuotaCompanyId: null
});

const emit = defineEmits<{
    (e: 'update:visible', v: boolean): void;
    (e: 'close'): void;
    (e: 'submit', payload: any): void; // AdmissionsQuotaAdjustmentModel
}>();

/* expose to template */
const visible = computed(() => props.visible);
const loading = computed(() => props.loading);
const mode = computed(() => props.mode!);
const scope = computed(() => props.scope);
const targetName = computed(() => props.targetName);

const isView = computed(() => mode.value === 'view');

/* Form model (local) */
const formData = ref({
    scope: props.scope,
    admissionsQuotaRegionId: props.admissionsQuotaRegionId ?? undefined,
    admissionsQuotaCompanyId: props.admissionsQuotaCompanyId ?? undefined,
    totalQuotaBefore: Number(props.totalQuotaBefore || 0),
    totalQuotaAfter:
        props.totalQuotaAfter != null ? Number(props.totalQuotaAfter) : Number(props.totalQuotaBefore || 0),
    reason: '' as string | null
});

watch(
    () => [
        props.scope,
        props.admissionsQuotaRegionId,
        props.admissionsQuotaCompanyId,
        props.totalQuotaBefore,
        props.totalQuotaAfter
    ],
    () => {
        formData.value.scope = props.scope;
        formData.value.admissionsQuotaRegionId = props.admissionsQuotaRegionId ?? undefined;
        formData.value.admissionsQuotaCompanyId = props.admissionsQuotaCompanyId ?? undefined;
        formData.value.totalQuotaBefore = Number(props.totalQuotaBefore || 0);
        formData.value.totalQuotaAfter =
            props.totalQuotaAfter != null ? Number(props.totalQuotaAfter) : Number(props.totalQuotaBefore || 0);
    },
    { immediate: true }
);

/* Validation rules */
const rules = {
    totalQuotaAfter: [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        {
            validator: (_: any, val: any, cb: any) => {
                const n = Number(val);
                if (Number.isNaN(n) || n < 0) return cb(new Error(t('validation.mustBeNonNegative')));
                cb();
            },
            trigger: 'blur'
        }
    ]
};

/* Header title */
const computedTitle = computed(() =>
    scope.value === AdjustmentScope.Company
        ? t('adjustment.titleCompany')
        : t('adjustment.titleRegion')
);

/* Delta helpers */
const delta = computed(
    () => Number(formData.value.totalQuotaAfter || 0) - Number(formData.value.totalQuotaBefore || 0)
);
const signedDelta = computed(() => {
    const v = Math.abs(delta.value);
    const s = v.toLocaleString('vi-VN');
    return (delta.value >= 0 ? '+' : '−') + s;
});

/* Enable save */
const submitDisabled = computed(() => {
    if (isView.value) return true;
    const n = Number(formData.value.totalQuotaAfter);
    return Number.isNaN(n) || n < 0;
});

/* Submit -> emit model for API */
async function emitSubmit() {
    const payload = {
        scope: formData.value.scope,
        admissionsQuotaRegionId: formData.value.admissionsQuotaRegionId,
        admissionsQuotaCompanyId: formData.value.admissionsQuotaCompanyId,
        totalQuotaBefore: Number(formData.value.totalQuotaBefore || 0),
        totalQuotaAfter: Number(formData.value.totalQuotaAfter || 0),
        reason: formData.value.reason ?? null
    };
    emit('submit', payload);
}
</script>

<style scoped>
.quota-adjustment-dialog :deep(.el-tag) {
    font-weight: 600;
}
</style>
