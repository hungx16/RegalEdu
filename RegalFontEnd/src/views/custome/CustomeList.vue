<template>
    <div>
        <!-- <div class="d-flex justify-content-between align-items-center mb-5">
            <div class="d-flex flex-column">
                <h3 class="fw-bold fs-4 mb-1">{{ t('custome.manageTitle') }}</h3>
                <span class="text-body-secondary fw-light fs-8">{{ t('custome.manageDesc') }}</span>
            </div>
            <el-button class="me-2" type="primary" @click="addModelEvent">{{ t('custome.addCustomer') }}</el-button>
            <el-button type="primary" @click="addModelEvent">{{ t('custome.importCustomers') }}</el-button>
        </div> -->
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" headerTitle="custome.manageTitle"
            headerDesc="custome.manageDesc" class="mb-6" />
        <div class="row g-4 mb-8">
            <div class="col-md-2">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('custome.totalCustomers') }}</span>
                    <div class="fs-2 fw-bold mt-1">{{ studentStore.total }} </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('custome.newCustomers') }}</span>
                    <div class="fs-2 fw-bold mt-1">{{ getNewCustomersThisMonth() }}</div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('custome.leads') }}</span>
                    <div class="fs-2 fw-bold mt-1">{{ getPotentialCustomerCount() }}</div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('custome.converted') }}</span>
                    <div class="fs-2 fw-bold mt-1">{{ getConvertedCustomerCount() }}</div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('custome.conversionRate') }}</span>
                    <div class="fs-2 fw-bold mt-1 text-danger">{{ getConversionRate() }}</div>
                </div>
            </div>
        </div>

        <!-- <div class="card shadow-sm p-4 mb-5">
            <h5 class="fw-bold mb-3">{{ t('custome.listTitle') }}</h5>
            <div class="row g-3 align-items-end">
                <div class="col-md-4">
                    <el-input v-model="filter.searchTerm" :placeholder="t('custome.searchPlaceholder')" />
                </div>
                <div class="col-md-3">
                    <el-select v-model="filter.status" :placeholder="t('custome.allStatus')" class="w-100">
                    </el-select>
                </div>
                <div class="col-md-3">
                    <el-select v-model="filter.source" :placeholder="t('custome.allSources')" class="w-100">
                    </el-select>
                </div>
                <div class="col-md-2 d-flex justify-content-end">
                    <el-button @click="clearFilters">{{ t('common.clearFilters') }}</el-button>
                </div>
            </div>
        </div> -->


        <div class="card w-100 p-4">
            <BaseTable :columns="columns" :items="studentStore.students" :loading="studentStore.loading"
                :showIndex="true" :showActionsColumn="true" :showEdit="true" :showView="true" :showDelete="true"
                :page="studentStore.query.page" :total="studentStore.total" :pageSize="studentStore.query.pageSize"
                :filter="filter" @edit="editModelEvent" @view="viewModelEvent" @delete="deleteModelEvent"
                @update:page="studentStore.setPage" @update:pageSize="studentStore.setPageSize" :showPagination="true"
                @update:filter="onTableFilter">

                <template #cell-customerInfo="{ item }">
                    <div class="d-flex flex-column">
                        <span class="fw-semibold">{{ item.fullName }}</span>
                        <span class="text-body-secondary">{{ item.phone }}</span>
                        <span class="text-body-secondary">{{ item.email }}</span>
                    </div>
                </template>

                <template #cell-source="{ item }">
                    <BaseBadge :label="item.leadSource" :color="getSourceColor(item.leadSource)" />
                </template>

                <template #cell-status="{ item }">
                    <BaseBadge :label="getStatusLabel(resolveStatus(item))"
                        :color="getStatusColor(resolveStatus(item))" />
                </template>

                <template #cell-priority="{ item }">
                    <BaseBadge :label="getPriorityLabel(item.priority)" :color="getPriorityColor(item.priority)" />
                </template>

                <template #cell-employeeName="{ item }">
                    <span>{{ getEmployee(item.employeeId) || '-' }}</span>
                </template>
                <template #cell-companyName="{ item }">
                    <span>{{ getCompany(item.companyId) || '-' }}</span>
                </template>
                <template #cell-lastContact="{ item }">
                    <span>{{ getLastContactDate(item.studentActivity) || t('custome.noContact') }}</span>
                </template>
                <template #cell-createdAt="{ item }">
                    <span>{{ formatDate(item.createdAt) || t('custome.noDate') }}</span>
                </template>

                <template #cell-actions="{ item }">
                    <el-button @click="viewModelEvent(item)" type="primary" link>{{ t('common.view') }}</el-button>
                    <el-button @click="editModelEvent(item)" type="primary" link>{{ t('common.edit') }}</el-button>
                    <el-button v-if="isLead(item.studentStatus)" @click="convertLead(item)" type="success" link>{{
                        t('student.convert') }}</el-button>
                </template>

            </BaseTable>
        </div>

        <CustomeDialogWrapper v-model:visible="showFormModal" :mode="dialogMode" :loading="formLoading"
            :student-data="normalizeStudent(studentStore.selectedStudent)" @submit="handleSave"
            @delete="deleteModelEvent" />

        <CustomeDetailDialog v-model:visible="showDetaiFormModal" :loading="formLoading"
            :student-data="normalizeStudent(studentStore.selectedStudent)" @refreshList="studentStore.fetchAllCustoms"
            @submit="handleSave" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useStudentStore } from '@/stores/studentStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import CustomeDialog from './CustomeDialog.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import CustomeDialogWrapper from './CustomeDialogWrapper.vue';
import { useEmployeeStore } from '@/stores/employeeStore';
import CustomeDetailDialog from './CustomeDetailDialog.vue';
import type { StudentModel } from '@/api/StudentApi';
import { StudentStatus } from '@/types';
import { formatDate } from '@/utils/format';

import { useCompanyStore } from '@/stores/companyStore'
const companyStore = useCompanyStore()
const filterComponentRef = ref();
const employeeStore = useEmployeeStore()
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
const { t } = useI18n();
const studentStore = useStudentStore();
const notificationStore = useNotificationStore();

const showFormModal = ref(false);
const showDetaiFormModal = ref(false);
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
type StudentFilter = { searchTerm?: string; status?: string | number | null; source?: string };
const filter = ref<StudentFilter>({ searchTerm: '', status: null, source: '' });

// --- Helpers (Giả định giá trị Enum/String) ---
const StudentStatusEnum = StudentStatus; // Ví dụ

const normalizeStatus = (status?: number | string | null) => {
    if (status === null || status === undefined) return null
    const numeric = typeof status === 'string' ? Number(status) : status
    if (Number.isNaN(numeric)) return null
    if (numeric === 6) return StudentStatusEnum.NewRegister
    return numeric as StudentStatus
}
const resolveStatus = (student: any) => normalizeStatus(student?.studentStatus ?? student?.status)

const getStatusLabel = (status?: number | null) => {
    const normalized = normalizeStatus(status)
    if (normalized === StudentStatusEnum.Prospect) return t('status.prospect')
    if (normalized === StudentStatusEnum.Enrolled) return t('status.enrolled')
    if (normalized === StudentStatusEnum.Paused) return t('status.paused')
    if (normalized === StudentStatusEnum.Dropped) return t('status.dropped')
    if (normalized === StudentStatusEnum.Graduated) return t('status.graduated')
    if (normalized === StudentStatusEnum.NewRegister) return t('status.newRegister')
    return t('common.unknown')
};
function getStatusColor(status?: number | null) {
    const normalized = normalizeStatus(status)
    if (normalized === StudentStatusEnum.Prospect) return 'warning'
    if (normalized === StudentStatusEnum.Enrolled) return 'info'
    if (normalized === StudentStatusEnum.NewRegister) return 'success'
    if (normalized === StudentStatusEnum.Paused) return 'gray'
    if (normalized === StudentStatusEnum.Dropped) return 'danger'
    if (normalized === StudentStatusEnum.Graduated) return 'purple'
    return 'default'
}
//lấy thông tin về số lượng khách hàng có tiềm năng (studentStatus = 0)
const getPotentialCustomerCount = () => {
    return studentStore.students.filter(s => resolveStatus(s) === StudentStatusEnum.Prospect).length;
};
//lấy thông tin về số lượng khách hàng đã chuyển đổi (studentStatus != 0)
const getConvertedCustomerCount = () => {
    return studentStore.students.filter(s => resolveStatus(s) !== StudentStatusEnum.Prospect).length;
};
//lấy thông tin về tỷ lệ chuyển đổi khách hàng
const getConversionRate = () => {
    const total = studentStore.students.length;
    if (total === 0) return '0%';
    const converted = getConvertedCustomerCount();
    const rate = (converted / total) * 100;
    return rate.toFixed(1) + '%';
};
//lấy thông tin về khách hàng mới trong tháng
const getNewCustomersThisMonth = () => {
    const now = new Date();
    return studentStore.students.filter(s => {
        const createdAt = new Date(s.createdAt || '');
        return createdAt.getMonth() === now.getMonth() && createdAt.getFullYear() === now.getFullYear();
    }).length;
};


const getSourceLabel = (source: string) => {
    if (source === 'Website') return t('source.website');
    if (source === 'Facebook') return t('source.facebook');
    return t('common.other');
};
const getSourceColor = (source: string) => {
    if (source === 'Website') return 'primary';
    if (source === 'Facebook') return 'secondary';
    return 'info';
};

const getPriorityLabel = (priority: number) => {
    if (priority === 2) return t('common.high'); // Cao
    if (priority === 1) return t('common.medium'); // Trung bình
    return t('common.low'); // Thấp
};
const getPriorityColor = (priority: number) => {
    if (priority === 2) return 'danger';
    if (priority === 1) return 'warning';
    return 'info';
};

const isLead = (status: number) => normalizeStatus(status) === StudentStatusEnum.Prospect;

const getLastContactDate = (activities: any) => {
    if (!activities || activities.length === 0) return null;
    //chuyển đổi ngày gần nhất để hiển thị
    activities.sort((a: any, b: any) => new Date(b.activityDate).getTime() - new Date(a.activityDate).getTime());
    const latestActivity = activities[0];
    // Logic tìm ngày tương tác gần nhất
    return latestActivity ? formatDate(latestActivity.activityDate, "DD/MM/YYYY") : null;
};

// Return employee display name by id (safe lookup - handles missing store or different shapes)
const getEmployee = (employeeId: string | number | null | undefined): string | null => {

    if (employeeId === null || employeeId === undefined) return null;

    return employeeStore.employees.find(emp => emp.id === employeeId)?.applicationUser?.fullName || null;
};

// Return company display name by id (safe lookup - handles missing store or different shapes)
// Attempts to read companies from a company-specific store if available,
// otherwise falls back to any companies array present on the studentStore (loose shape).
const getCompany = (companyId: string | number | null | undefined): string | null => {
    if (companyId === null || companyId === undefined) return null;
    return companyStore.companies.find(comp => comp.id === companyId)?.companyName || null;
};

// Normalize student object to satisfy child prop typing (convert null numeric fields to undefined)
function normalizeStudent(s: any) {
    if (!s) return null;
    const copy = { ...s } as any;
    // Some properties in StudentModel are typed as number | undefined in the dialog prop,
    // but the store may keep them as number | null | undefined — convert null -> undefined.
    if (copy.priority === null) copy.priority = undefined;
    if (copy.expectedBudget === null) copy.expectedBudget = undefined;
    // add other nullable numeric fields here if needed:
    // if (copy.someNumericField === null) copy.someNumericField = undefined;
    return copy;
}

const columns: BaseTableColumn[] = [
    { key: 'studentCode', labelKey: 'custome.code', filterType: 'text', width: 100, isBold: true },
    { key: 'customerInfo', labelKey: 'custome.customerInfo', width: 250, filterType: "text", },
    {
        key: 'source', labelKey: 'custome.source', width: 150, filterType: "select", filterOptions: [
            { label: "Website", value: "Website" },
            { label: "Facebook", value: "Facebook" },
            { label: "Zalo", value: "Zalo" },
            { label: "Social", value: "Social" },
            { label: "Event", value: "Event" },
            { label: "Tiktok", value: "Tiktok" },
            { label: "Support", value: "Support" },
            { label: "Other", value: "Other" },
        ],
    },
    { key: 'companyName', labelKey: 'custome.companyName', width: 180, filterType: "text", },
    { key: 'address', labelKey: 'custome.address', width: 180, filterType: "text", },
    { key: 'createdAt', labelKey: 'custome.createdAt', width: 180, filterType: "text", },
    { key: 'status', labelKey: 'custome.status', width: 150, filterType: "text", },
    {
        key: 'priority', labelKey: 'custome.priority', width: 100, filterType: "select", filterOptions: [
            { label: "Cao", value: 2 },
            { label: "Trung bình", value: 1 },
            { label: "Thấp", value: 0 },
        ],
    },

    { key: 'employeeName', labelKey: 'custome.advisor', width: 180, filterType: "text", },
    { key: 'lastContact', labelKey: 'custome.lastContact', filterType: "text", },
    { key: 'createdBy', labelKey: 'custome.createdBy', width: 180, filterType: "text", },
    { key: 'actions', labelKey: 'common.actions', width: 150, filterType: "text", },
];

// --- Handlers ---
function onTableFilter(val: Record<string, any>) {
    studentStore.setFilter(val);
    studentStore.fetchAllCustoms();
}

function clearFilters() {
    filter.value = {};
    studentStore.setFilter({});
    studentStore.fetchPagedStudents();
}

function addModelEvent() {
    dialogMode.value = 'create';
    studentStore.selectStudent(null);
    showFormModal.value = true;
}

function importModelEvent() {
    // Placeholder import handler - open import dialog or start import flow here.
    // Implement actual import logic (e.g., open file picker, show import modal, call API) as needed.
    // This prevents the template binding error when @import="importModelEvent" is used.
    console.warn('importModelEvent not implemented yet');
}

function editModelEvent(student: StudentModel) {
    dialogMode.value = 'edit';
    studentStore.selectStudent({ ...student });
    showFormModal.value = true;
}

function viewModelEvent(student: StudentModel) {
    dialogMode.value = 'edit';
    studentStore.selectStudent({ ...student });
    //console.log('Viewing student:', student);
    showDetaiFormModal.value = true;
    //showFormModal.value = false;
    //
    //showFormModal.value = true; // Sử dụng lại biến này để mở StudentDetailDialog
}

async function handleSave(student: any) {
    // console.log('Saving student:', student);

    try {
        startLoading();

        await studentStore.saveStudent(student);
        notificationStore.showToast('success', { key: 'toast.saveSuccess', params: { model: t('models.student') } });
        await studentStore.fetchAllCustoms();
    } catch (err: any) {
        console.error('Error saving:', err);
    } finally {
        stopLoading();
        showFormModal.value = false;
        showDetaiFormModal.value = false;
    }
}

function deleteModelEvent(student: StudentModel) {
    // Logic xóa
}

function convertLead(student: StudentModel) {
    // Logic chuyển đổi Lead sang Student (chuyển trạng thái)
}

// --- Lifecycle ---
watch(() => [studentStore.query.page, studentStore.query.pageSize], () => {
    studentStore.fetchAllCustoms();
});

onMounted(() => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'custome.add', type: 'add' },
            { event: 'import', label: 'custome.import', type: 'import' },
        ],
    });
    studentStore.fetchAllCustoms();
    employeeStore.fetchAllEmployees();
    studentStore.total = studentStore.students.length;
    companyStore.fetchAllCompanies();
});
</script>
