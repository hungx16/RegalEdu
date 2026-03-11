<template>
    <!-- <el-dialog :title="t('registerStudy.detailViewTitle', { code: registerStudy?.code || '...' })" :visible="visible"
        width="80%" @update:visible="emit('update:visible', $event)" :close-on-click-modal="true"> -->

    <div class="register-detail-view" v-loading="loading">

        <div class="section-card mb-5 p-4 rounded shadow-sm">
            <h4 class="fw-bold mb-4">{{ t('registerStudy.customerInfo') }}</h4>
            <el-row :gutter="30">
                <el-col :span="12">
                    <div class="detail-item mb-3">
                        <span class="text-body-secondary d-block">{{ t('registerStudy.employeeManager') }}</span>
                        <span class="fw-semibold">{{ getEmployee(registerStudy?.employeeId) || 'Nguyễn Văn A' }}</span>
                    </div>
                    <div class="detail-item mb-3">
                        <span class="text-body-secondary d-block">{{ t('registerStudy.studentName') }}</span>
                        <span class="fw-semibold">{{ registerStudy?.studentFullName || 'Học viên A' }}</span>
                    </div>
                    <div class="detail-item mb-3">
                        <span class="text-body-secondary d-block">{{ t('registerStudy.parentEmail') }}</span>
                        <span class="fw-semibold">{{ registerStudy?.contactAddress || 'phuhuynh@gmail.com' }}</span>
                    </div>
                    <div class="detail-item mb-3">
                        <span class="text-body-secondary d-block">{{ t('registerStudy.region') }}</span>
                        <span class="fw-semibold">{{ getRegistrationRegion(registerStudy?.regionId) || 'Vùng 1'
                            }}</span>
                    </div>
                </el-col>

                <el-col :span="12">
                    <div class="detail-item mb-3">
                        <span class="text-body-secondary d-block">{{ t('registerStudy.studentPhone') }}</span>
                        <span class="fw-semibold">{{ registerStudy?.studentPhone || '0334404540' }}</span>
                    </div>
                    <div class="detail-item mb-3">
                        <span class="text-body-secondary d-block">{{ t('registerStudy.parentName') }}</span>
                        <span class="fw-semibold">{{ registerStudy?.contactFullName || 'Phụ huynh A' }}</span>
                    </div>
                    <div class="detail-item mb-3">
                        <span class="text-body-secondary d-block">{{ t('registerStudy.expectedCompleteDate')
                        }}</span>
                        <span class="fw-semibold">{{ registerStudy?.expectedCompleteDate || '23/06/2025' }}</span>
                    </div>
                    <div class="detail-item mb-3">
                        <span class="text-body-secondary d-block">{{ t('registerStudy.company') }}</span>
                        <span class="fw-semibold">{{ getRegistrationCompany(registerStudy?.companyId) || 'Chi nhánh A'
                        }}</span>
                    </div>
                </el-col>
            </el-row>
        </div>

        <div class="section-card mb-5 p-4 rounded shadow-sm">
            <h4 class="fw-bold mb-4">{{ t('registerStudy.courseInfo') }}</h4>
            <el-table :data="registerStudy?.detailRegisterStudys" border class="mb-4">
                <el-table-column :label="t('registerStudy.courseCode')" prop="courseCode" width="100">
                    <template #default="{ row }">
                        {{ courseCodeDisplay(row.courseId) }}
                    </template>
                </el-table-column>
                <el-table-column :label="t('registerStudy.classType')" prop="classTypeName" width="200">
                    <template #default="{ row }">
                        <span>{{ classTypeDisplay(row.classTypeId) }}</span>
                    </template>
                </el-table-column>
                <el-table-column :label="t('registerStudy.courseName')" prop="courseName" width="200">
                    <template #default="{ row }">
                        <span>{{ courseNameDisplay(row.courseId) }}</span>
                        <br />
                        <span class="text-body-secondary">{{ row.courseLevelName }}</span>
                    </template>
                </el-table-column>
                <!-- <el-table-column :label="t('registerStudy.program')" prop="program" width="100" /> -->
                <el-table-column :label="t('registerStudy.courseHours')" prop="courseHours" width="200">
                    <template #default="{ row }">
                        {{ courseHoursDisplay(row.courseId) || '0' }} {{ t('unit.hour') }}
                    </template>
                </el-table-column>
                <el-table-column :label="t('registerStudy.unit')" prop="unit" width="150">
                    <template #default="{ row }">
                        <!-- /  selectedCourse.tuitions?.[0]?.unit == 0 ? t('unit.hours') : selectedCourse.tuitions?.[0]?.unit == 1 ? t('unit.session') : selectedCourse.tuitions?.[0]?.unit == 2 ? t('unit.month') : t('unit.course'); -->
                        {{ row.unit == 0 ? t('unit.hour') : row.unit == 1 ? t('unit.session') : row.unit == 2 ?
                            t('unit.month') : t('unit.course') }}
                    </template>
                </el-table-column>
                <el-table-column :label="t('registerStudy.tuitionFee')" prop="tuitionFee" width="100" align="right">
                    <template #default="{ row }">
                        {{ row.tuitionFee.toLocaleString('vi-VN') || '0' }} {{ t('common.currency') }}
                    </template>
                </el-table-column>
                <!-- <el-table-column :label="t('registerStudy.discount')" prop="discountAmount" width="100" align="right" /> -->
                <el-table-column :label="t('registerStudy.totalAmount')" prop="totalAmount" width="200" align="right">
                    <template #default="{ row }">
                        {{ (row.tuitionFee - (row.discountAmount || 0)).toLocaleString('vi-VN') || '0' }} {{
                            t('common.currency') }}
                    </template>
                </el-table-column>
            </el-table>

            <div class="d-flex justify-content-end">
                <div class="text-end" style="width: 250px;">
                    <p class="text-body-secondary mb-1">{{ t('registerStudy.totalTuitionFee') }}: <span
                            class="fw-bold">{{
                                totalTuitionFee.toLocaleString('vi-VN') }} đ</span></p>
                    <p class="text-body-secondary mb-1">{{ t('registerStudy.totalDiscount') }}: <span class="fw-bold">{{
                        totalDiscount.toLocaleString('vi-VN') }} đ</span></p>
                    <p class="fw-bold text-success mb-1">{{ t('registerStudy.netReceivable') }}: <span
                            class="fw-bold">{{
                                netReceivable.toLocaleString('vi-VN') }} đ</span></p>
                    <p class="fw-bold text-success mb-1">{{ t('registerStudy.tuitionFeesPaid') }}: <span
                            class="fw-bold">{{
                                tuitionFeesPaid.toLocaleString('vi-VN') }} đ</span></p>
                    <p class="fw-bold text-success mb-1">{{ t('registerStudy.remainingTuitionFees') }}: <span
                            class="fw-bold">{{
                                remainingTuitionFees.toLocaleString('vi-VN') }} đ</span></p>
                </div>
            </div>
        </div>

        <div class="section-card mb-5 p-4 rounded shadow-sm">
            <div class="d-flex justify-content-between align-items-center mb-4">
                <h4 class="fw-bold mb-0">{{ t('registerStudy.paymentInfo') }}</h4>
                <!-- <el-button type="primary" @click="onAddPayment" plain>{{ t('registerStudy.addPayment') }}</el-button> -->
            </div>

            <el-table :data="registerStudy?.receipts" border>
                <el-table-column :label="t('registerStudy.receiptCode')" prop="receiptCode" width="100">
                    <template #default="{ row }">
                        <span>{{ row.receiptCode || 'RC001' }}</span>
                    </template>
                </el-table-column>
                <el-table-column :label="t('registerStudy.paymentType')" prop="paymentTermType" width="200">
                    <template #default="{ row }">
                        <span>{{ row.paymentType === 0 ? t('registerStudy.payDirect') :
                            t('registerStudy.payInstallment')
                            }}</span>
                    </template>
                </el-table-column>
                <el-table-column :label="t('registerStudy.paymentMethodType')" prop="paymentMethodType" width="200">
                    <template #default="{ row }">
                        <span>{{ row.paymentMethodType === 0 ? t('registerStudy.payOnce') :
                            t('registerStudy.payMultiple')
                            }}</span>
                    </template>
                </el-table-column>
                <el-table-column :label="t('registerStudy.paymentMethod')" prop="paymentMethod" width="200">
                    <template #default="{ row }">
                        <!-- nếu paymentMethod  là 0 thì hiển thị  cash là 1 thì hiển thị là vnPay là 2 thì hiển thị là banktranfer -->
                        <span>{{ row.paymentMethod === 0 ? t('registerStudy.cash') :
                            row.paymentMethod === 1 ? t('registerStudy.vnPay') :
                                row.paymentMethod === 2 ? t('registerStudy.bankTransfer') :
                                    t('registerStudy.methodUnknown') }}</span>
                    </template>
                </el-table-column>

                <el-table-column :label="t('registerStudy.paymentDate')" prop="createdAt" width="120">
                    <template #default="{ row }">
                        <!-- chuyển  createdAt về "dd/MM/yyyy" -->
                        <span>{{ formatDate(row.createdAt) }}</span>
                    </template>
                </el-table-column>
                <el-table-column :label="t('registerStudy.paymentStatus')" prop="paymentStatus" width="200">
                    <template #default="{ row }">
                        <span>{{ row.paymentStatus === PaymentStatus.Paid ? t('paymentStatus.paid') :
                            row.paymentStatus === PaymentStatus.PartiallyPaid ? t('paymentStatus.partiallyPaid') :
                                t('paymentStatus.unpaid') }}</span>
                    </template>
                </el-table-column>
                <el-table-column :label="t('registerStudy.paymentAmount')" prop="totalAmount" width="150" align="right">
                    <template #default="{ row }">
                        <span>{{ row.totalAmount?.toLocaleString('vi-VN') }} {{ t('common.currency') }}</span>
                    </template>
                </el-table-column>
            </el-table>

            <div class="d-flex justify-content-end mt-3">
                <h5 class="fw-bold">{{ t('registerStudy.totalPayment') }}: {{
                    getTotalPayment(registerStudy?.id) }} {{ t('common.currency') }}
                </h5>
            </div>
        </div>
    </div>
    <div class="wizard-content">
        <el-button @click="emit('update:visible', false)">{{ t('common.close') }}</el-button>
        <!-- <el-button type="primary">{{ t('common.edit') }}</el-button> -->
    </div>
    <!-- <template #footer>
        <el-button @click="emit('update:visible', false)">{{ t('common.close') }}</el-button>
        <el-button type="primary">{{ t('common.edit') }}</el-button>
    </template> -->
    <!-- </BaseDialogForm> -->
    <!-- </el-dialog> -->


</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, defineAsyncComponent } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRegisterStudyStore } from '@/stores/registerStudyStore';
import { useCompanyStore } from '@/stores/companyStore';
import { useRegionStore } from '@/stores/regionStore';
import { useEmployeeStore } from '@/stores/employeeStore';
import { useCourseStore } from '@/stores/courseStore';
//import loại lớp học
import { useClassTypeStore } from '@/stores/classTypeStore';
//khai báo sử dụng useClassTypeStore
const classTypeStore = useClassTypeStore();
//import các kiểu thanh toán
import { PaymentStatus } from '@/types';
const props = defineProps<{ visible: boolean }>();
const emit = defineEmits<{
    (e: 'update:visible', value: boolean): void;
    (e: 'add-payment', id: string): void;
}>();
const { t } = useI18n();
const store = useRegisterStudyStore();
const companyStore = useCompanyStore();
const regionStore = useRegionStore();
const employeeStore = useEmployeeStore();
const courseStore = useCourseStore();
const registerStudy = computed(() => store.selectedRegisterStudy);
const loading = computed(() => store.loading);

// --- Tính toán tổng cộng cho Khóa học ---
const getRegistrationCompany = (companyId?: string | null) => {
    if (!companyId) return '';
    // companyStore shape may vary between projects; try common property names and fallback gracefully
    const companies = (companyStore as any).companies || (companyStore as any).list || [];
    const company = companies.find((c: any) => c.id === companyId || c.companyId === companyId);
    return company?.name || company?.companyName || '';
};
const getRegistrationRegion = (regionId?: string | null) => {
    if (!regionId) return '';
    // regionStore may expose regions under different property names; try common fallbacks
    const regions = (regionStore as any).regions || (regionStore as any).list || [];
    const region = regions.find((r: any) => r.id === regionId || r.regionId === regionId);
    return region?.name || region?.regionName || 'Tìm không thấy';
};
const getEmployee = (employeeId: string | number | null | undefined): string | null => {

    if (employeeId === null || employeeId === undefined) return null;

    return employeeStore.employees.find(emp => emp.id === employeeId)?.applicationUser?.fullName || null;
};
onMounted(() => {
    // Tải danh sách công ty, vùng, nhân viên để hiển thị tên
    companyStore.fetchAllCompanies();
    regionStore.fetchAllRegions();
    employeeStore.fetchAllEmployees();
    courseStore.fetchAllCourses();
    classTypeStore.fetchAllClassTypes();
});

/**
 * Handler for the "Add Payment" button in the template.
 * Emits an 'add-payment' event with the current registerStudy id so parent can open a payment form,
 * or extend this function to open a local modal/dialog directly.
//  */
const onAddPayment = () => {

    const id = registerStudy.value?.id;
    if (!id) {
        // No selected register study, nothing to do
        console.warn('onAddPayment called but no registerStudy selected');
        return;
    }
    // Mở dialog phiếu thu
    emit('add-payment', id);
    emit('update:visible', false);
    //emit('add-payment', id);
};

/**
 * Provide a course code string for the template.
 * Attempts to find a course record in common store keys, otherwise falls back to the raw id.
 */
const courseCodeDisplay = (courseId?: string | number | null): string => {
    if (courseId === null || courseId === undefined) return '';
    //tìm trong courseStore trả lại code
    const course = courseStore.courses.find(c => c.id == courseId);
    return course?.courseCode || course?.courseName || String(courseId);
};

const courseNameDisplay = (courseId?: string | number | null): string => {
    if (courseId === null || courseId === undefined) return '';
    // tìm trong courseStore trả lại tên khóa học
    const course = courseStore.courses.find(c => c.id == courseId);
    return course?.courseName || course?.courseCode || String(courseId);
};

/**
 * Provide a human readable class type string for the template.
 * Tries common locations for class type lists (on courseStore) and falls back to the id.
 */
const classTypeDisplay = (classTypeId?: string | number | null): string => {
    if (classTypeId === null || classTypeId === undefined) return '';
    // some projects keep class types on the course store under different keys
    const classType = classTypeStore.classTypes.find(ct => ct.id == classTypeId);
    return classType?.classTypeName || String(classTypeId);
};

/**
 * Provide course hours for the template.
 * Attempts to find a course record and read common hours field names, otherwise returns 0.
 */
const courseHoursDisplay = (courseId?: string | number | null): number | string => {
    if (courseId === null || courseId === undefined) return 0;
    // tìm trong courseStore trả lại số giờ
    const course = courseStore.courses.find(c => c.id == courseId);
    // try common field names for hours across different project shapes
    const hours = course?.tuitions?.[0]?.durationHours ?? 0;
    return hours;
};

const formatDate = (value?: string | number | Date | null): string => {
    if (!value) return '';
    const date = typeof value === 'string' || typeof value === 'number' ? new Date(value) : value;
    if (!date || Number.isNaN(date.getTime())) return '';
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
};

const totalTuitionFee = computed(() => {
    return registerStudy.value?.detailRegisterStudys?.reduce((sum, item) => sum + (item.tuitionFee || 0), 0) || 0;
});

const totalDiscount = computed(() => {
    return registerStudy.value?.totalDiscount || 0;
});

const netReceivable = computed(() => {
    // Giả định tổng tiền sau giảm giá = Tổng học phí - Tổng giảm giá
    return totalTuitionFee.value - totalDiscount.value;
});

// --- Tính toán tổng tiền đã thanh toán ---
const tuitionFeesPaid = computed(() => {
    return registerStudy.value?.tuitionFeesPaid || 0;
});

const remainingTuitionFees = computed(() => {
    // Remaining = net receivable (after discounts) - amount already paid
    return Math.max(0, netReceivable.value - tuitionFeesPaid.value);
});

/**
 * Calculate total payment for the selected registerStudy by summing receipts.
 * Returns a localized string representation (vi-VN) to match other displayed amounts.
 */
const getTotalPayment = (id?: string | null): string => {
    // We use the reactive registerStudy.value.receipts so the template updates automatically.
    const receipts = registerStudy.value?.receipts ?? [];
    const total = receipts.reduce((sum: number, r: any) => sum + (Number(r?.totalAmount) || 0), 0);
    return total.toLocaleString('vi-VN');
};

// Load dữ liệu chi tiết khi component hiển thị
watch(
    () => props.visible, (newVal) => {
        // if (newVal && store.selectedRegisterStudy?.id) {
        //     store.fetchRegisterStudyDetails(store.selectedRegisterStudy.id);
        // }
    }, { immediate: true });
</script>

<style scoped>
.detail-item {
    line-height: 1.2;
}

.detail-item span {
    font-size: 0.9em;
}

.detail-item span.fw-semibold {
    font-size: 1em;
    color: var(--el-text-color-primary);
}
</style>