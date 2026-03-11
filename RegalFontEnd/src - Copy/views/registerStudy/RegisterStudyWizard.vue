<template>
    <div class="card shadow-sm p-5">
        <div class="d-flex justify-content-between align-items-center mb-5">
            <h3 class="fw-bold fs-4">
                {{ store.currentStep === 4 ? t('registerStudy.selectPromotion') : t('registerStudy.createTitle') }}
            </h3>

        </div>

        <div class="d-flex justify-content-center mb-10" v-if="store.currentStep !== 4">
            <div class="d-flex align-items-center me-5">
                <span
                    :class="['step-circle', { 'active': store.currentStep === 1, 'done': store.currentStep > 1 }]">1</span>
                <span class="ms-2 fw-semibold">{{ t('registerStudy.step1') }}</span>
            </div>
            <div class="d-flex align-items-center me-5">
                <span
                    :class="['step-circle', { 'active': store.currentStep === 2, 'done': store.currentStep > 2 }]">2</span>
                <span class="ms-2 fw-semibold">{{ t('registerStudy.step2') }}</span>
            </div>
            <div class="d-flex align-items-center">
                <span :class="['step-circle', { 'active': store.currentStep === 3 }]">3</span>
                <span class="ms-2 fw-semibold">{{ t('registerStudy.step3') }}</span>
            </div>
        </div>

        <div class="wizard-content">
            <Step1_CustomerInfo v-if="store.currentStep === 1" @next="store.goToStep(2)"
                @view-promotion="store.goToStep(4)" @exit="closeModal" />

            <Step2_Payment v-if="store.currentStep === 2" @prev="store.goToStep(1)" @submit="handleSubmission"
                @next="store.goToStep(3)" />

            <Step3_Confirmation v-if="store.currentStep === 3" @complete="handleComplete" @prev="store.goToStep(2)" />

            <Step4_PromotionView v-if="store.currentStep === 4" @confirm="handlePromotionConfirm"
                @cancel="store.goToStep(1)" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { defineComponent, h } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRegisterStudyStore } from '@/stores/registerStudyStore';
import { useNotificationStore } from '@/stores/notificationStore';
import Step1_CustomerInfo from './Step1_CustomerInfo.vue';
import Step2_Payment from './Step2_Payment.vue';
import Step3_Confirmation from './Step3_Confirmation.vue';
import Step4_PromotionView from './Step4_PromotionView.vue';


const { t } = useI18n();
const store = useRegisterStudyStore();
const notificationStore = useNotificationStore();

const emit = defineEmits(['close', 'refreshList', "submit"]);

const closeModal = () => {
    store.resetForm();
    emit('close');
};

const handleSubmission = async () => {
    // Lấy payload đã được tính toán từ store
    const payload = store.formData;

    try {
        // Gọi action createRegistration mới từ store
        await store.saveRegisterStudy(payload);

        // Sau khi store hoàn thành, nó sẽ tự chuyển currentStep = 3
        notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.registerStudy') } });
    } catch (error) {
        notificationStore.showToast('error', { key: 'toast.error', params: { message: 'Không thể tạo đăng ký học.' } });
    }
};

const handleComplete = async () => {
    const payload = store.formData;
    // if (payload.detailRegisterStudys?.paymentMethodType === PaymentMethodType.OneTime) {
    //     payload.tuitionFeesPaid = payload.totalAfterDiscount;
    //     payload.remainingTuitionFees = 0;
    // }
    console.log('Final payload on completion:', payload);
    //gửi dữ liệu về trang cha qua emit và đóng modal   
    emit('submit', payload);
    //closeModal();

    // try {
    //     // Gọi action createRegistration mới từ store
    //     await store.saveRegisterStudy(payload);
    //     console.log('Registration saved successfully on completion.');

    //     // Sau khi store hoàn thành, nó sẽ tự chuyển currentStep = 3
    //     notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.registerStudy') } });
    // } catch (error) {
    //     notificationStore.showToast('error', { key: 'toast.error', params: { message: 'Không thể tạo đăng ký học.' } });
    // }

    //emit('refreshList');
    //closeModal();
};
const handlePromotionConfirm = () => {
    // Xử lý khi xác nhận khuyến mãi (nếu cần)
    // Quay lại bước 1 để bắt đầu lại
    store.goToStep(1);
};
</script>

<style scoped>
.step-circle {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 30px;
    height: 30px;
    border-radius: 50%;
    background-color: #e4e6ef;
    color: #a1a5b7;
    font-weight: bold;
    transition: all 0.3s;
}

.step-circle.active {
    background-color: var(--el-color-primary);
    /* Màu chính */
    color: white;
}

.step-circle.done {
    background-color: var(--el-color-success);
    /* Màu xanh lá (Hoàn thành) */
    color: white;
}
</style>