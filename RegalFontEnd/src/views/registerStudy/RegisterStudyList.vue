<template>
    <div>
        <div class="d-flex justify-content-between align-items-center mb-5">
            <div class="d-flex flex-column">
                <h3 class="fw-bold fs-4 mb-1">{{ t('registerStudy.manageTitle') }}</h3>
                <span class="text-body-secondary fw-light fs-8">{{ t('registerStudy.manageDesc') }}</span>
            </div>
            <el-button type="primary" @click="goToWizard">{{ t('registerStudy.addRegistration') }}</el-button>
        </div>

        <div class="card w-100 p-4">
            <h5 class="fw-bold mb-3">{{ t('registerStudy.listTitle') }} ({{ filteredRegisterStudies.length }})</h5>

            <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredRegisterStudies" :showIndex="true"
                :loading="store.loading" :showPagination="true" :page="store.query.page"
                :total="filteredRegisterStudies.length" :pageSize="store.query.pageSize" :filter="filter"
                @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val" :showActionsColumn="true"
                :showEdit="true" :showView="true" @edit="editRegistration" @view="viewRegistration"
                @update:page="val => store.setPage(val)" @update:pageSize="val => store.setPageSize(val)">
                <template #cell-studentCode="{ item }">
                    <BaseBadge :label="getRegistrationStudentCode(item.studentId)"
                        :color="item.type === 1 ? 'lightBlue' : 'lightPurple'" />
                </template>
                <template #cell-companyName="{ item }">
                    <BaseBadge :label="getRegistrationCompany(item.companyId)"
                        :color="item.type === 1 ? 'lightBlue' : 'lightPurple'" />
                </template>
                <template #cell-regionName="{ item }">
                    <BaseBadge :label="getRegistrationRegion(item.regionId)" :color="'lightPurple'" />
                </template>
                <template #cell-type="{ item }">
                    <BaseBadge :label="getRegistrationType(item.type)"
                        :color="item.type === 1 ? 'lightBlue' : 'lightPurple'" />
                </template>
                <template #cell-status="{ item }">
                    <BaseBadge :label="getRegistrationStatus(item.status)" :color="getRegStatusColor(item.status)" />
                </template>
                <template #cell-paymentStatus="{ item }">
                    <BaseBadge :label="getPaymentStatus(item.paymentStatus)"
                        :color="getPaymentStatusColor(item.paymentStatus)" />
                </template>
                <template #cell-totalAmount="{ item }">
                    <span class="fw-bold text-danger">{{ (item.totalAmount || 0).toLocaleString('vi-VN') }} VND</span>
                </template>
                <template #cell-totalDiscount="{ item }">
                    <span class="fw-bold text-danger">{{ (item.totalDiscount || 0).toLocaleString('vi-VN') }} VND</span>
                </template>
                <template #cell-totalAfterDiscount="{ item }">
                    <span class="fw-bold text-danger">{{ (item.totalAfterDiscount || 0).toLocaleString('vi-VN') }}
                        VND</span>
                </template>

            </BaseTable>
        </div>
        <el-dialog v-model="showDetailModal" :title="t('registerStudy.viewTitle')" width="80%"
            :close-on-click-modal="false">
            <RegisterStudyDetailView v-if="showDetailModal" :visible="showDetailModal"
                @update:visible="val => showDetailModal = val" @close="showDetailModal = false" @add-payment="id => {
                    // Mở ReceiptDialog để thêm thanh toán cho đăng ký học hiện tại
                    showFormModal = true;
                    dialogMode = 'create';
                    receiptData.value = { registerStudyId: id };
                }" />

        </el-dialog>


        <el-dialog v-model="showWizardModal" :title="t('registerStudy.createTitle')" width="80%"
            :close-on-click-modal="false">
            <RegisterStudyWizard v-if="showWizardModal" @close="showWizardModal = false"
                @refreshList="handleRefreshList" @submit="handleSubmit" />
        </el-dialog>
        <ReceiptDialog v-model:visible="showFormModal" :mode="dialogMode" :loading="formLoading"
            :receiptData="receiptData" @submit="handleSave" @close="showFormModal = false"
            @update:visible="val => showFormModal = val" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRegisterStudyStore } from '@/stores/registerStudyStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import RegisterStudyWizard from './RegisterStudyWizard.vue';
import RegisterStudyDetailView from './RegisterStudyDetailView.vue';
// hãy khai báo ReceiptDialog
import ReceiptDialog from './ReceiptDialog.vue';
import type { RegisterStudyQuery, RegisterStudyListModel, RegisterStudyModel } from '@/api/RegisterStudyApi';
import { useEmployeeStore } from '@/stores/employeeStore'
import { useCompanyStore } from '@/stores/companyStore';
import { useRegionStore } from '@/stores/regionStore';
import { useStudentStore } from '@/stores/studentStore';
import { PaymentMethodType, PaymentStatus } from '@/types';
//import { s } from 'node_modules/@fullcalendar/core/internal-common';
const regionStore = useRegionStore();
const employeeStore = useEmployeeStore()
const { t } = useI18n();
const store = useRegisterStudyStore();
const notificationStore = useNotificationStore();
const showWizardModal = ref(false);
const companyStore = useCompanyStore();
const filter = ref({}); // Bộ lọc hiện tại của bảng
const selectedRowsData = ref<Array<RegisterStudyListModel>>([]);
const studentStore = useStudentStore();
//khai báo cho ReceiptDialog
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
const showFormModal = ref(false);
const dialogMode = ref<'view' | 'create' | 'edit'>('create');
const receiptData = ref<Partial<any>>({});
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
// --- Cột Bảng (Tích hợp filterOptions) ---
const columns: BaseTableColumn[] = [
    { key: 'code', labelKey: 'registerStudy.code', filterType: 'text', sortable: true, width: 130, isBold: true },
    { key: 'studentCode', labelKey: 'registerStudy.studentCode', filterType: 'text', sortable: true, width: 100 },
    { key: 'companyName', labelKey: 'registerStudy.company', filterType: 'text', sortable: true, width: 150 },
    { key: 'regionName', labelKey: 'registerStudy.region', filterType: 'text', sortable: true, width: 120 },
    { key: 'createdAt', labelKey: 'registerStudy.registrationDate', filterType: 'date', sortable: true, width: 120, formatter: (val: string) => new Date(val).toLocaleDateString() },
    {
        key: 'type', labelKey: 'registerStudy.registrationType', filterType: 'select', sortable: true, width: 100,
        filterOptions: [
            { label: t('common.all'), value: undefined },
            { label: t('registerStudy.type.new'), value: 1 },
            { label: t('registerStudy.type.renewal'), value: 2 },
        ]
    },
    {
        key: 'status', labelKey: 'registerStudy.registrationStatus', filterType: 'select', sortable: true, width: 120,
        filterOptions: [
            { label: t('common.all'), value: undefined },
            { label: t('registerStudy.status.confirmed'), value: 1 },
            { label: t('registerStudy.status.pending'), value: 2 },
        ]
    },
    {
        key: 'paymentStatus', labelKey: 'registerStudy.paymentStatus', filterType: 'select', sortable: true, width: 120,
        filterOptions: [
            { label: t('common.all'), value: undefined },
            { label: t('registerStudy.payment.paid'), value: 1 },
            { label: t('registerStudy.payment.partiallyPaid'), value: 2 },
            { label: t('registerStudy.payment.unpaid'), value: 3 },
        ]
    },
    { key: 'totalAmount', labelKey: 'registerStudy.totalBeforeDiscount', filterType: 'text', sortable: true, width: 150, align: 'right' },
    { key: 'totalDiscount', labelKey: 'registerStudy.totalDiscount', filterType: 'text', sortable: true, width: 150, align: 'right' },
    { key: 'totalAfterDiscount', labelKey: 'registerStudy.totalAfterDiscount', filterType: 'text', sortable: true, width: 150, align: 'right' },
    { key: 'actions', labelKey: 'common.actions', width: 100 },
];

// --- Computed Property cho Lọc (tương tự CouponTypeList.vue) ---
const filteredRegisterStudies = computed(() => {
    let arr = store.registerStudies;

    // Áp dụng bộ lọc từ BaseTable
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            const valStr = String(val).toLowerCase();

            if (key === 'createdAt') {
                // Lọc theo ngày (giả định val là chuỗi YYYY-MM-DD)
                arr = arr.filter(item => item.createdAt?.substring(0, 10) === val);
            } else if (['type', 'status', 'paymentStatus'].includes(key)) {
                // Lọc theo giá trị số/enum
                arr = arr.filter(item => String(item[key]) === valStr);
            } else {
                // Lọc theo chuỗi (code, name, companyName, etc.)
                arr = arr.filter(item =>
                    String(item[key] ?? '').toLowerCase().includes(valStr)
                );
            }
        }
    });
    return arr;
});

// --- Logic Getter cho Badge ---


const getRegistrationStudentCode = (studentId?: string) => {
    console.log('Looking up student code for ID:', studentId);

    if (!studentId) return '';
    // studentStore shape may vary between projects; try common property names and fallback gracefully

    const student = studentStore.students.find((s: any) => s.id === studentId || s.studentId === studentId);
    return student?.studentCode || student?.fullName || '';
};
const getRegistrationCompany = (companyId?: string) => {
    if (!companyId) return '';
    // companyStore shape may vary between projects; try common property names and fallback gracefully
    const companies = (companyStore as any).companies || (companyStore as any).list || [];
    const company = companies.find((c: any) => c.id === companyId || c.companyId === companyId);
    return company?.name || company?.companyName || '';
};
const getRegistrationRegion = (regionId?: string) => {
    if (!regionId) return '';
    // companyStore may expose regions under different property names; try common fallbacks
    // const regions = (companyStore as any).regions || (companyStore as any).listRegions || (companyStore as any).list || [];
    //khai báo regionStore để lấy vùng
    const regions = (regionStore as any).regions || (regionStore as any).list || [];
    const region = regions.find((r: any) => r.id === regionId || r.regionId === regionId);
    return region?.name || region?.regionName || 'Tìm không thấy';
};

const getRegistrationType = (type: number) => (type === 1) ? t('registerStudy.type.new') : t('registerStudy.type.renewal');
const getRegistrationStatus = (status: number) => (status === 1) ? t('registerStudy.status.confirmed') : t('registerStudy.status.pending');
const getRegStatusColor = (status: number) => (status === 1) ? 'success' : 'warning';
const getPaymentStatus = (status: number) => {
    if (status === PaymentStatus.Paid) return t('paymentStatus.paid');
    if (status === PaymentStatus.PartiallyPaid) return t('paymentStatus.partiallyPaid');
    return t('paymentStatus.unpaid');
};
const getPaymentStatusColor = (status: number) => {
    if (status === PaymentStatus.Paid) return 'success';
    if (status === PaymentStatus.PartiallyPaid) return 'info';
    return 'warning';
};

// --- Xử lý sự kiện ---

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    // Chuyển đổi bộ lọc table thành query API (nếu BaseTable chỉ lọc frontend)
    // Nếu muốn lọc backend: store.setQueryFilters(val); store.fetchPagedRegisterStudies();
}

const goToWizard = () => {
    store.resetForm();
    showWizardModal.value = true;
};

const showDetailModal = ref(false);

// ... (Hàm Handlers)

const editRegistration = (item: RegisterStudyModel) => {
    // Tải dữ liệu chi tiết của đăng ký này vào store.formData
    // store.loadRegistrationForEdit(item.id); 
    store.selectRegisterStudy(item);
    showWizardModal.value = true;
};
const viewRegistration = (item: RegisterStudyModel) => {
    // Tải dữ liệu chi tiết
    console.log("Viewing registration:", item);

    store.selectRegisterStudy(item);
    //store.fetchRegisterStudyDetails(item.id);
    showDetailModal.value = true;
};

const deleteRegistration = (item: RegisterStudyListModel) => {
    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.registerStudy') } },
        async () => {
            await store.deleteRegisterStudies([item.id as string]);
            await store.fetchPagedRegisterStudies();
            notificationStore.showToast('success', { key: 'toast.deleteSuccess', params: { model: t('models.registerStudy') } });
        }
    );
};

const handleRefreshList = () => {
    // Được gọi sau khi hoàn thành Wizard (Step 3)
    store.fetchPagedRegisterStudies();
};

// Xử lý submit từ RegisterStudyWizard: đóng modal và refresh danh sách
const handleSubmit = async (payload?: any) => {

    if (!payload) {
        payload = store.formData;
    }

    //câp nhật dữ liệu đăng ký học
    await store.saveRegisterStudy(payload);

    showWizardModal.value = false;
    // Làm mới danh sách từ backend

    // Hiển thị thông báo thành công (nếu cần)
    notificationStore.showToast('success', { key: 'toast.saveSuccess', params: { model: t('models.registerStudy') } });
    store.fetchAllRegisterStudies();
};

// Xử lý submit từ ReceiptDialog (được gọi bởi @submit="handleSave")
const handleSave = async (payload?: any) => {
    startLoading();
    try {
        // Nếu ReceiptDialog trả về payload để lưu, ưu tiên gọi saveReceipt nếu store có,
        // nếu không thì fallback sang saveRegisterStudy (dự phòng).
        if (payload) {
            if (typeof (store as any).saveReceipt === 'function') {
                await (store as any).saveReceipt(payload);
            } else {
                await store.saveRegisterStudy(payload);
            }
        }

        showFormModal.value = false;

        // Làm mới danh sách từ backend
        await store.fetchAllRegisterStudies();

        notificationStore.showToast('success', { key: 'toast.saveSuccess', params: { model: t('models.registerStudy') } });
    } catch (err) {
        console.error('handleSave error:', err);
        notificationStore.showToast('error', { key: 'toast.saveError' });
    } finally {
        stopLoading();
    }
};

// --- Lifecycle & Data Fetching ---
watch(() => [store.query.page, store.query.pageSize], () => {
    store.fetchPagedRegisterStudies(); // Kích hoạt nếu dùng phân trang/lọc backend
});

onMounted(() => {
    employeeStore.fetchAllEmployees();
    companyStore.fetchAllCompanies();
    regionStore.fetchAllRegions();
    //  store.fetchPagedRegisterStudies();
    studentStore.fetchAllStudents();
    store.fetchAllRegisterStudies();
    // *MOCK DATA tạm thời*
    // store.registerStudies = [
    //     { id: '1', code: 'REG2024-001', studentCode: 'STU001', companyName: 'Chi nhánh Hà Nội 1', regionName: 'Vùng 1 Hà Nội', createdAt: '2024-06-15', type: 1, status: 1, paymentStatus: 1, totalAmount: 4500000 },
    //     { id: '2', code: 'REG2024-002', studentCode: 'NEW002', companyName: 'Chi nhánh Hà Nội 2', regionName: 'Vùng 1 Hà Nội', createdAt: '2024-06-20', type: 1, status: 2, paymentStatus: 3, totalAmount: 2700000 },
    //     { id: '3', code: 'REG2024-003', studentCode: 'STU003', companyName: 'Chi nhánh TP.HCM 1', regionName: 'Vùng TP.HCM', createdAt: '2024-06-18', type: 2, status: 1, paymentStatus: 2, totalAmount: 5500000 },
    // ] as any;
    store.total = store.registerStudies.length;
});
</script>