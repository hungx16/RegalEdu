<template>
    <div>
        <div class="row g-4 mb-8">
            <div class="col-md-3">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('receipt.totalPlan') }}</span>
                    <div class="fs-2 fw-bold mt-1">{{ totalDueByRegistration.toLocaleString("vi-VN") }} {{
                        t('common.currency') }}</div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('receipt.completed') }}</span>
                    <div class="fs-2 fw-bold mt-1">{{ completedReceipts }}</div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('receipt.collected') }}</span>
                    <div class="fs-2 fw-bold mt-1 text-success">{{ totalCollected.toLocaleString("vi-VN") }} {{
                        t('common.currency') }}</div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('receipt.remaining') }}</span>
                    <div class="fs-2 fw-bold mt-1 text-danger">{{ totalRemaining.toLocaleString("vi-VN") }} {{
                        t('common.currency') }}</div>
                </div>
            </div>
        </div>

        <div class="card shadow-sm p-4 mb-5">
            <div class="d-flex justify-content-between align-items-center mb-4">
                <h5 class="fw-bold mb-0">{{ t('receipt.listTitle') }}</h5>
                <el-button type="primary" @click="addModelEvent">{{ t('receipt.addReceipt') }}</el-button>
            </div>
            <!-- <div class="row g-3 align-items-end">
                <div class="col-md-3">
                    <el-input v-model="filter.searchTerm" :placeholder="t('receipt.searchPlaceholder')" />
                </div>
                <div class="col-md-2">
                    <el-select v-model="filter.type" :placeholder="t('receipt.allTypes')" class="w-100">
                        <el-option :label="t('common.all')" value="" />
                        <el-option :label="t('receipt.type.order')" value="Phiếu theo đơn" />
                        <el-option :label="t('receipt.type.supplement')" value="Phiếu bổ sung" />
                    </el-select>
                </div>
                <div class="col-md-2">
                    <el-select v-model="filter.status" :placeholder="t('receipt.allStatus')" class="w-100">
                        <el-option :label="t('common.all')" value="" />
                        <el-option :label="t('receipt.status.confirmed')" value="1" />
                        <el-option :label="t('receipt.status.pending')" value="2" />
                    </el-select>
                </div>
                <div class="col-md-3">
                    <el-select v-model="filter.paymentMethodType" :placeholder="t('receipt.allPaymentMethods')"
                        class="w-100">
                        <el-option :label="t('common.all')" value="" />
                        <el-option :label="t('receipt.methodType.once')" value="Thanh toán một lần" />
                        <el-option :label="t('receipt.methodType.multiple')" value="Thanh toán nhiều lần" />
                    </el-select>
                </div>
                <div class="col-md-2 d-flex justify-content-end">
                    <el-button @click="clearFilters">{{ t('common.clearFilters') }}</el-button>
                </div>
            </div> -->
        </div>


        <div class="card w-100 p-4">
            <BaseTable :columns="columns" :items="receiptStore.receipts" :loading="receiptStore.loading"
                :showIndex="true" :showActionsColumn="true" :showEdit="true" :showView="true" :showDelete="true"
                :page="receiptStore.query.page" :total="receiptStore.total" :pageSize="receiptStore.query.pageSize"
                :filter="filter" @edit="editModelEvent" @view="viewModelEvent" @delete="deleteModelEvent"
                @update:page="receiptStore.setPage" @update:pageSize="receiptStore.setPageSize" :showPagination="true"
                @update:filter="onTableFilter">
                <template #cell-receiptType="{ item }">
                    <BaseBadge :label="getReceiptTypeLabel(item.receiptType)" :color="getStatusColor(item.status)" />
                </template>

                <template #cell-status="{ item }">
                    <BaseBadge :label="getStatusLabel(item.status)" :color="getStatusColor(item.status)" />
                </template>
                <template #cell-registerStudyId="{ item }">
                    <BaseBadge :label="getRegisterStudyCodeLabel(item.registerStudyId)"
                        :color="getStatusColor(item.status)" />
                </template>
                <!-- <template #cell-studentName="{ item }">
                    <BaseBadge :label="getStudentNameLabel(item.studentId)" :color="getStatusColor(item.status)" />
                </template> -->
                <!-- <template #cell-totalAmount="{ item }">
                    <span class="fw-bold">{{ (item.totalAmount || 0).toLocaleString('vi-VN') }} đ</span>
                </template> -->
                <template #cell-paymentType="{ item }">
                    <BaseBadge :label="getPaymentTypeLabel(item.paymentType)" :color="getStatusColor(item.status)" />
                </template>
                <template #cell-paymentMethodType="{ item }">
                    <BaseBadge :label="getPaymentMethodTypeLabel(item.paymentMethodType)"
                        :color="getStatusColor(item.status)" />
                </template>
                <template #cell-paymentMethod="{ item }">
                    <BaseBadge :label="getPaymentMethodLabel(item.paymentMethod)"
                        :color="getStatusColor(item.status)" />
                </template>
                <template #cell-totalAmount="{ item }">
                    <BaseBadge :label="getTotalAmountLabel(item.registerStudyId)"
                        :color="getStatusColor(item.status)" />
                </template>
                <template #cell-totalDiscount="{ item }">
                    <BaseBadge :label="getTotalDiscountLabel(item.registerStudyId)"
                        :color="getStatusColor(item.status)" />
                </template>
                <template #cell-totalAfterDiscount="{ item }">
                    <BaseBadge :label="getTotalAfterDiscountLabel(item.registerStudyId)"
                        :color="getStatusColor(item.status)" />
                </template>
                <template #cell-totalPaid="{ item }">
                    <BaseBadge :label="getTotalPaidLabel(item.registerStudyId)" :color="getStatusColor(item.status)" />
                </template>
                <template #cell-totalRemaining="{ item }">
                    <BaseBadge :label="getTotalRemainingLabel(item.registerStudyId)"
                        :color="getStatusColor(item.status)" />
                </template>
                <!-- <template #cell-status="{ item }">
                    <BaseBadge :label="getStatusLabel(item.status)"
                        :color="getStatusColor(item.status)" />
                </template> -->
            </BaseTable>
        </div>

        <ReceiptDialog v-model:visible="showFormModal" :mode="dialogMode" :loading="formLoading"
            :receipt-data="receiptStore.selectedReceipt" @submit="handleSave" @delete="deleteModelEvent" />

    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useReceiptStore } from '@/stores/receiptStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import ReceiptDialog from './ReceiptDialog.vue'; // Sẽ tạo ở bước 3.2
import type { ReceiptsModel } from '@/api/ReceiptApi';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { ReceiptType, PaymentType, PaymentMeThod, PaymentMethodType, PaymentStatus } from '@/types';
// đăng ký
import { useRegisterStudyStore } from '@/stores/registerStudyStore';

const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
const { t } = useI18n();
const receiptStore = useReceiptStore();
const notificationStore = useNotificationStore();
//khai báo sử dụng store đăng ký học
const registerStudyStore = useRegisterStudyStore();
const showFormModal = ref(false);
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
type ReceiptFilter = {
    searchTerm?: string;
    type?: string;
    status?: string;
    paymentMethodType?: string;
    [key: string]: any;
};
const filter = ref<ReceiptFilter>({ searchTerm: '', type: '', status: '', paymentMethodType: '' });
//thống kê số lượng phiếu thu
const totalReceipts = computed(() => receiptStore.receipts.length);
//thống kế số lượng phiếu thu đã hoàn thành
const completedReceipts = computed(() => receiptStore.receipts.filter(r => r.status === PaymentStatus.Paid).length);
//thống kê tổng số tiền đã thu
const totalCollected = computed(() => receiptStore.receipts.reduce((sum, r) => sum + (r.totalAmount || 0), 0));
//thống kê tổng số tiền còn lại

//tính tổng tiên phải thu theo đăng ký học
const totalDueByRegistration = computed(() => {
    return registerStudyStore.registerStudies.reduce((sum, reg) => {
        return sum + (reg.totalAmount || 0);
    }, 0);
});

const totalRemaining = computed(() => totalDueByRegistration.value - totalCollected.value);
// --- Helpers (Giả định giá trị Enum/String) ---
const StatusEnum = { Confirmed: 1, Pending: 2 };

const getStatusLabel = (status: number) => {
    if (status === PaymentStatus.Paid) return t('receipt.paymentStatus.paid');
    if (status === PaymentStatus.PartiallyPaid) return t('receipt.paymentStatus.partiallyPaid');
    if (status === PaymentStatus.Unpaid) return t('receipt.paymentStatus.unpaid');
    return t('common.other');
};
const getStatusColor = (status: number) => {
    if (status === PaymentStatus.Paid) return 'success';
    if (status === PaymentStatus.PartiallyPaid) return 'warning';
    return 'danger';
};

const getReceiptTypeLabel = (type?: number | null) => {
    // Map known Vietnamese type strings to translation keys; fallback to raw type or a generic label.
    // if (type < 0) return t('common.other');
    if (type === ReceiptType.Order) return t('receipt.receiptType.order');
    if (type === ReceiptType.Additional) return t('receipt.receiptType.additional');
    if (type === ReceiptType.Deposit) return t('receipt.receiptType.deposit');
    return t('common.other');
};

const getRegisterStudyCodeLabel = (registerStudyId?: string | null) => {
    //lấy đăng ký theo RegisterStudyId từ store đăng ký học
    const registerStudy = registerStudyStore.registerStudies.find(rs => rs.id === registerStudyId);
    if (registerStudy && registerStudy.code) {
        return registerStudy.code;
    }
    // Return a readable label for the registration/study code used in the table.
    // If absent, return a localized fallback.
    if (!registerStudyId) return t('common.other');
    return String(registerStudyId);
};

const getPaymentTypeLabel = (paymentType?: number | string | null) => {
    // Provide labels for known payment term types; fallback to the raw value or a generic label.
    // Adjust mappings to match your backend enum/values if necessary.
    if (paymentType === undefined || paymentType === null || paymentType === '') return t('common.other');

    // numeric enums
    if (paymentType === PaymentType.Direct || paymentType === '1') return t('receipt.paymentType.direct');
    if (paymentType === PaymentType.Installment || paymentType === '2') return t('receipt.paymentType.installment');

    return String(paymentType);
};

const getPaymentMethodTypeLabel = (paymentMethodType?: number | string | null) => {
    // Provide labels for payment method types; fallback to the raw value or a generic label.
    // Adjust mappings to match your backend enum/values if necessary.
    if (paymentMethodType === undefined || paymentMethodType === null || paymentMethodType === '') return t('common.other');

    // numeric enums or string representations from UI/backend
    if (paymentMethodType === PaymentMethodType.OneTime || paymentMethodType === '1' || paymentMethodType === 'Thanh toán một lần') return t('receipt.PaymentMethodType.oneTime');
    if (paymentMethodType === PaymentMethodType.Multiple || paymentMethodType === '2' || paymentMethodType === 'Thanh toán nhiều lần') return t('receipt.PaymentMethodType.multiple');

    return String(paymentMethodType);
};

const getPaymentMethodLabel = (paymentMethod?: number | string | null) => {
    // Provide labels for concrete payment methods; fallback to the raw value or a generic label.
    if (paymentMethod === undefined || paymentMethod === null || paymentMethod === '') return t('common.other');

    // Try numeric enums, Vietnamese strings or common English identifiers.
    if (paymentMethod === PaymentMeThod.Cash || paymentMethod === '1' || paymentMethod === 'Tiền mặt' || paymentMethod === 'cash') return t('receipt.PaymentMethod.cash');
    if (paymentMethod === PaymentMeThod.VnPay || paymentMethod === '2' || paymentMethod === 'Chuyển khoản' || paymentMethod === 'bank') return t('receipt.PaymentMethod.vnPay');
    if (paymentMethod === PaymentMeThod.Transfer || paymentMethod === '3' || paymentMethod === 'Thẻ tín dụng' || paymentMethod === 'card') return t('receipt.PaymentMethod.transfer');

    return String(paymentMethod);
};

const getTotalAmountLabel = (registerStudyId?: string | null) => {
    // Return a formatted total amount for the given registerStudyId; fallback to a readable label if missing.
    if (!registerStudyId) return t('common.other');

    const registerStudy = registerStudyStore.registerStudies.find(rs => rs.id === registerStudyId);
    if (!registerStudy) return String(registerStudyId);

    // Expecting registerStudy.totalAmount to be a numeric field; adjust property name if your store uses a different key.
    const total = (registerStudy as any).totalAmount ?? 0;
    // Format number according to locale and append currency label
    return `${Number(total).toLocaleString('vi-VN')} ${t('common.currency')}`;
};

const getTotalDiscountLabel = (registerStudyId?: string | null) => {
    // Return a formatted total discount for the given registerStudyId; fallback to a readable label if missing.
    if (!registerStudyId) return t('common.other');

    const registerStudy = registerStudyStore.registerStudies.find(rs => rs.id === registerStudyId);
    if (!registerStudy) return String(registerStudyId);

    // Expecting registerStudy.totalDiscount to be a numeric field; adjust property name if your store uses a different key.
    const discount = (registerStudy as any).totalDiscount ?? 0;
    // Format number according to locale and append currency label
    return `${Number(discount).toLocaleString('vi-VN')} ${t('common.currency')}`;
};

const getTotalAfterDiscountLabel = (registerStudyId?: string | null) => {
    // Return a formatted total after discount for the given registerStudyId; fallback to a readable label if missing.
    if (!registerStudyId) return t('common.other');

    const registerStudy = registerStudyStore.registerStudies.find(rs => rs.id === registerStudyId);
    if (!registerStudy) return String(registerStudyId);

    // Expecting registerStudy.totalAmount and registerStudy.totalDiscount to be numeric fields; adjust names if different.
    const total = (registerStudy as any).totalAmount ?? 0;
    const discount = (registerStudy as any).totalDiscount ?? 0;
    const after = total - discount;

    // Format number according to locale and append currency label
    return `${Number(after).toLocaleString('vi-VN')} ${t('common.currency')}`;
};

const getTotalPaidLabel = (registerStudyId?: string | null) => {
    // Return a formatted total paid amount for the given registerStudyId; fallback to a readable label if missing.
    if (!registerStudyId) return t('common.other');

    const registerStudy = registerStudyStore.registerStudies.find(rs => rs.id === registerStudyId);
    if (!registerStudy) return String(registerStudyId);

    // Expecting registerStudy.totalPaid to be a numeric field; adjust property name if your store uses a different key.
    const paid = (registerStudy as any).tuitionFeesPaid ?? 0;
    // Format number according to locale and append currency label
    return `${Number(paid).toLocaleString('vi-VN')} ${t('common.currency')}`;
};

const getTotalRemainingLabel = (registerStudyId?: string | null) => {
    // Return a formatted remaining total for the given registerStudyId; fallback to a readable label if missing.
    if (!registerStudyId) return t('common.other');

    const registerStudy = registerStudyStore.registerStudies.find(rs => rs.id === registerStudyId);
    if (!registerStudy) return String(registerStudyId);

    // Compute remaining = (totalAmount - totalDiscount) - totalPaid
    const remaining = ((registerStudy as any).totalAmount ?? 0) - ((registerStudy as any).totalDiscount ?? 0) - ((registerStudy as any).tuitionFeesPaid ?? 0);

    // Format number according to locale and append currency label
    return `${Number(remaining).toLocaleString('vi-VN')} ${t('common.currency')}`;
};


const columns: BaseTableColumn[] = [
    { key: 'receiptType', labelKey: 'receipt.type', width: 150, filterType: 'text', sortable: true, },
    { key: 'createdAt', labelKey: 'receipt.date', width: 150, formatter: (val: string) => new Date(val).toLocaleDateString(), filterType: 'date', sortable: true, },
    { key: 'receiptCode', labelKey: 'receipt.code', width: 120, isBold: true, filterType: 'text', sortable: true },
    { key: 'registerStudyId', labelKey: 'receipt.registrationCode', width: 120, filterType: 'text', sortable: true }, // Cần ánh xạ Code
    { key: 'studentName', labelKey: 'receipt.student', width: 150, filterType: 'text', sortable: true },
    { key: 'companyName', labelKey: 'receipt.company', width: 120, filterType: 'text', sortable: true },
    { key: 'regionName', labelKey: 'receipt.region', width: 120, filterType: 'text', sortable: true },
    { key: 'paymentType', labelKey: 'receipt.paymentTermType', width: 100, filterType: 'text', sortable: true },
    { key: 'paymentMethodType', labelKey: 'receipt.paymentMethodType', width: 200, filterType: 'text', sortable: true },
    { key: 'paymentMethod', labelKey: 'receipt.paymentMethod', width: 200, filterType: 'text', sortable: true },
    { key: 'totalAmount', labelKey: 'receipt.totalAmount', width: 120, align: 'right', filterType: 'text', sortable: true },
    { key: 'totalDiscount', labelKey: 'receipt.totalDiscount', width: 120, align: 'right', filterType: 'text', sortable: true },
    { key: 'totalAfterDiscount', labelKey: 'receipt.totalAfterDiscount', width: 120, align: 'right', filterType: 'text', sortable: true },
    { key: 'totalPaid', labelKey: 'receipt.totalPaid', width: 120, align: 'right', filterType: 'text', sortable: true },
    { key: 'totalRemaining', labelKey: 'receipt.totalRemaining', width: 120, align: 'right', filterType: 'text', sortable: true },
    { key: 'status', labelKey: 'receipt.status', width: 100, filterType: 'text', sortable: true },
    { key: 'actions', labelKey: 'common.actions', width: 100 },
];

// --- Handlers ---
function onTableFilter(val: Record<string, any>) {
    receiptStore.setQueryFilters(val);
    receiptStore.fetchAllReceipts();
}

function clearFilters() {
    filter.value = {};
    receiptStore.setQueryFilters({});
    receiptStore.fetchPagedReceipts();
}

function addModelEvent() {
    dialogMode.value = 'create';
    receiptStore.selectReceipt(null);
    showFormModal.value = true;
}

function editModelEvent(receipt: ReceiptsModel) {
    dialogMode.value = 'edit';
    receiptStore.selectReceipt({ ...receipt });
    showFormModal.value = true;
}
function viewModelEvent(receipt: ReceiptsModel) {
    dialogMode.value = 'view';
    receiptStore.selectReceipt({ ...receipt });
    showFormModal.value = true;
}

async function handleSave(receipt: any) {
    try {
        startLoading();
        await receiptStore.saveReceipt(receipt);
        notificationStore.showToast('success', { key: 'toast.saveSuccess', params: { model: t('models.receipt') } });
        await receiptStore.fetchPagedReceipts();
    } catch (err: any) {
        console.error('Error saving:', err);
    } finally {
        stopLoading();
        showFormModal.value = false;
    }
}

function deleteModelEvent(receipt: ReceiptsModel) {
    // Logic xóa
}

// --- Lifecycle ---
watch(() => [receiptStore.query.page, receiptStore.query.pageSize], () => {
    receiptStore.fetchAllReceipts();
});

onMounted(() => {
    // Mock data (thay thế bằng store.fetchPagedReceipts() khi API sẵn sàng)
    receiptStore.fetchAllReceipts();
    registerStudyStore.fetchAllRegisterStudies();

    receiptStore.total = receiptStore.receipts.length;
});
</script>