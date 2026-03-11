<template>
    <div class="text-center mb-5">
        <i class="el-icon-success text-success" style="font-size: 60px;"></i>
        <h3 class="fw-bold mt-3">{{ t('registerStudy.confirmationSuccess') }}</h3>
        <p class="text-body-secondary">{{ t('registerStudy.reviewInfo') }}</p>
    </div>

    <el-row :gutter="30">
        <el-col :span="12">
            <div class="card p-4">
                <h5 class="fw-bold mb-3">{{ t('registerStudy.customerInfo') }}</h5>
                <p><strong>{{ t('registerStudy.studentName') }}:</strong> {{ store.formData.studentFullName }}</p>
                <p><strong>{{ t('registerStudy.studentPhone') }}:</strong> {{ store.formData.studentPhone }}</p>
            </div>
        </el-col>

        <el-col :span="12">
            <div class="card p-4">
                <h5 class="fw-bold mb-3">{{ t('registerStudy.paymentInfo') }}</h5>
                <p><strong>{{ t('registerStudy.totalAmount') }}:</strong> {{ store.totalAmount.toLocaleString('vi-VN')
                }} VND</p>
                <p><strong>{{ t('registerStudy.totalDiscount') }}:</strong> {{
                    store.totalDiscount.toLocaleString('vi-VN') }} VND</p>
                <p><strong>{{ t('registerStudy.totalAfterDiscount') }}:</strong> {{
                    store.totalAfterDiscount.toLocaleString('vi-VN') }} VND</p>
                <p><strong>{{ t('registerStudy.tuitionFeesPaid') }}:</strong> {{
                    Number(store.formData.tuitionFeesPaid ?? 0).toLocaleString('vi-VN') }} VND
                </p>
                <p><strong>{{ t('registerStudy.remainingTuitionFees') }}:</strong> {{
                    Number(((store.totalAfterDiscount ?? 0) - (store.formData.tuitionFeesPaid ??
                        store.totalAfterDiscount))).toLocaleString('vi-VN') }} VND</p>
                <el-divider class="my-2" />
                <p><strong>{{ t('registerStudy.paymentMethodType') }}:</strong> {{ store.formData.paymentMethodType ===
                    PaymentMethodType.OneTime ? t('registerStudy.payOnce') : t('registerStudy.payMultiple')
                    }}</p>
                <p><strong>{{ t('registerStudy.paymentMethod') }}:</strong> {{ store.formData.paymentMethod ===
                    PaymentMeThod.Cash ? t('registerStudy.cash') : store.formData.paymentMethod ===
                        PaymentMeThod.VnPay ? t('registerStudy.atmCard') : t('registerStudy.bankTransfer') }}</p>
            </div>
        </el-col>
    </el-row>

    <div class="card p-4 mt-5">
        <h5 class="fw-bold mb-3">{{ t('registerStudy.selectedCourse') }}</h5>
        <el-table :data="store.formData.detailRegisterStudys" border>
            <el-table-column :label="t('registerStudy.courseName')" prop="courseName" />
            <el-table-column :label="t('registerStudy.tuitionFee')" prop="tuitionFee" width="150" />
            <el-table-column :label="t('registerStudy.totalAmount')" prop="totalAmount" width="150" />

        </el-table>
    </div>

    <div class="d-flex justify-content-between mt-5">
        <el-button @click="emit('prev')">{{ t('common.goBack') }}</el-button>
        <el-button type="success" @click="emit('complete')">{{ t('registerStudy.completeRegistration') }}</el-button>
    </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n';
import { useRegisterStudyStore } from '@/stores/registerStudyStore';
import { PaymentMethodType, PaymentMeThod } from '@/types';

const emit = defineEmits(['prev', 'complete']);
const { t } = useI18n();
const store = useRegisterStudyStore();
</script>