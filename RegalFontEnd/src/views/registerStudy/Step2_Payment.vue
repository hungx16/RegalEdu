<template>
    <el-form ref="formRef" :model="store.formData" :rules="rules" label-position="top">
        <h3 class="fw-bold mb-5">{{ t('registerStudy.step2') }}: {{ t('registerStudy.payment') }}</h3>

        <div class="card p-4 mb-5 bg-light-subtle">
            <h5 class="fw-bold mb-3">{{ t('registerStudy.paymentSummary') }}</h5>
            <div class="d-flex justify-content-between mb-2">
                <span>{{ t('registerStudy.totalBeforeDiscount') }}:</span>
                <span class="fw-semibold">{{ store.totalAmount.toLocaleString('vi-VN') }} VND</span>
            </div>
            <div class="d-flex justify-content-between mb-3">
                <span class="text-success">{{ t('registerStudy.totalDiscount') }}:</span>
                <span class="fw-semibold text-success">-{{ store.totalDiscount.toLocaleString('vi-VN') }} VND</span>
            </div>
            <el-divider class="my-2" />
            <div class="d-flex justify-content-between">
                <span class="fw-bold fs-5">{{ t('registerStudy.totalAfterDiscount') }}:</span>
                <span class="fw-bold fs-5 text-danger">{{ store.totalAfterDiscount.toLocaleString('vi-VN') }} VND</span>
            </div>
        </div>

        <div class="mb-5">
            <h5 class="fw-bold mb-3">{{ t('registerStudy.paymentMethodType') }}</h5>
            <el-radio-group v-model="store.formData.paymentMethodType">
                <el-radio :value="PaymentMethodType.OneTime">{{ t('registerStudy.payOnce') }}</el-radio>
                <el-radio :value="PaymentMethodType.Multiple">{{ t('registerStudy.payMultiple') }}</el-radio>
            </el-radio-group>

            <div v-if="store.formData.paymentMethodType === PaymentMethodType.Multiple">
                <label class="required fs-6 fw-semibold mb-2 d-block mt-3">{{ t('registerStudy.paymentAmount')
                }}</label>
                <el-form-item prop="tuitionFeesPaid">
                    <el-input-number v-model="store.formData.tuitionFeesPaid" :min="0" :max="store.totalAfterDiscount"
                        :controls="false" class="w-50" :placeholder="t('registerStudy.paymentAmountPlaceholder')" />
                </el-form-item>
            </div>
            <div v-else>
            </div>
        </div>

        <div class="mb-5">
            <h5 class="fw-bold mb-3">{{ t('registerStudy.paymentType') }}</h5>
            <el-radio-group v-model="store.formData.paymentType">
                <el-radio :value="PaymentType.Direct">{{ t('registerStudy.payDirect') }}</el-radio>
                <el-radio :value="PaymentType.Installment">{{ t('registerStudy.payInstallment') }}</el-radio>
            </el-radio-group>
        </div>

        <div class="mb-5">
            <h5 class="fw-bold mb-3">{{ t('registerStudy.paymentMethod') }}</h5>
            <el-radio-group v-model="store.formData.paymentMethod">
                <el-row :gutter="20">
                    <el-col :span="6">
                        <el-radio border :label="t('registerStudy.cash')" :value="PaymentMeThod.Cash">{{
                            t('registerStudy.cash') }}</el-radio>
                    </el-col>
                    <el-col :span="6">
                        <el-radio border :label="t('registerStudy.vnPay')" :value="PaymentMeThod.VnPay">{{
                            t('registerStudy.atmCard') }}</el-radio>
                    </el-col>
                    <!-- <el-col :span="6">
                        <el-radio border :label="t('registerStudy.creditCard')" value="PaymentMeThod.Transfer">{{
                            t('registerStudy.creditCard')
                            }}</el-radio>
                    </el-col> -->
                    <el-col :span="6">
                        <el-radio border :label="t('registerStudy.bankTransfer')" :value="PaymentMeThod.Transfer">{{
                            t('registerStudy.bankTransfer')
                        }}</el-radio>
                    </el-col>
                </el-row>
            </el-radio-group>
        </div>

        <div class="d-flex justify-content-between">
            <el-button @click="emit('prev')">{{ t('common.goBack') }}</el-button>
            <el-button type="primary" :loading="store.loading" @click="handleSubmit">{{ t('common.confirm')
            }}</el-button>
        </div>
    </el-form>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRegisterStudyStore } from '@/stores/registerStudyStore';
import { useNotificationStore } from '@/stores/notificationStore';
import { PaymentType, PaymentMethodType, PaymentMeThod, PaymentStatus } from '@/types';

const emit = defineEmits(['prev', 'next', 'submit']);
const { t } = useI18n();
const store = useRegisterStudyStore();
const notificationStore = useNotificationStore();
const formRef = ref();

const rules = {
    firstPaymentAmount: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
};

const handleSubmit = () => {
    formRef.value?.validate((valid: boolean) => {
        if (valid) {
            store.formData.paymentStatus = PaymentStatus.PartiallyPaid;
            // Cập nhật Store và gọi Submit API (hàm submit này sẽ được RegisterStudyWizard xử lý)
            if (store.formData.paymentMethodType === PaymentMethodType.OneTime) {
                store.formData.tuitionFeesPaid = store.totalAfterDiscount;
                store.formData.remainingTuitionFees = 0;
                store.formData.paymentStatus = PaymentStatus.Paid;
            }
            store.updateFormData(store.formData);

            emit('next');
        } else {
            notificationStore.showToast('error', { key: 'validation.formInvalid' });
        }
    });
};
</script>