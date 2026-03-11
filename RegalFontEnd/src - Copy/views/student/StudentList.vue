<template>
    <div>
        <div class="d-flex justify-content-between align-items-center mb-5">
            <div class="d-flex flex-column">
                <h3 class="fw-bold fs-4 mb-1">{{ t('student.manageTitle') }}</h3>
                <span class="text-body-secondary fw-light fs-8">{{ t('student.manageDesc') }}</span>
            </div>
            <el-button type="primary" @click="addModelEvent">{{ t('student.addStudent') }}</el-button>
        </div>

        <!-- <div class="row g-4 mb-8">
            <div class="col-md-2">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('student.totalCustomers') }}</span>
                    <div class="fs-2 fw-bold mt-1">{{ studentStore.total }} </div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('student.newCustomers') }}</span>
                    <div class="fs-2 fw-bold mt-1">1</div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('student.leads') }}</span>
                    <div class="fs-2 fw-bold mt-1">1</div>
                </div>
            </div>
            <div class="col-md-2">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('student.converted') }}</span>
                    <div class="fs-2 fw-bold mt-1">1</div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="summary-card p-4 rounded-4 shadow-sm h-100 bg-light">
                    <span class="fw-semibold fs-5">{{ t('student.conversionRate') }}</span>
                    <div class="fs-2 fw-bold mt-1 text-danger">33.3%</div>
                </div>
            </div>
        </div> -->

        <!-- <div class="card shadow-sm p-4 mb-5">
            <h5 class="fw-bold mb-3">{{ t('student.listTitle') }}</h5>
            <div class="row g-3 align-items-end">
                <div class="col-md-4">
                    <el-input v-model="filter.searchTerm" :placeholder="t('student.searchPlaceholder')" />
                </div>
                <div class="col-md-3">
                    <el-select v-model="filter.status" :placeholder="t('student.allStatus')" class="w-100">
                    </el-select>
                </div>
                <div class="col-md-3">
                    <el-select v-model="filter.source" :placeholder="t('student.allSources')" class="w-100">
                    </el-select>
                </div>
                <div class="col-md-2 d-flex justify-content-end">
                    <el-button @click="clearFilters">{{ t('common.clearFilters') }}</el-button>
                </div>
            </div>
        </div> -->


        <div class="card w-100 p-4">
            <BaseTable :columns="columns" :items="studentList" :loading="studentStore.loading" :showIndex="true"
                :showActionsColumn="true" :showEdit="true" :showView="true" :showDelete="true"
                :page="studentStore.query.page" :total="studentStore.total" :pageSize="studentStore.query.pageSize"
                :filter="filter" @edit="editModelEvent" @view="viewModelEvent" @delete="deleteModelEvent"
                @update:page="studentStore.setPage" @update:pageSize="studentStore.setPageSize" :showPagination="true"
                @update:filter="onTableFilter">

                <!-- <template #cell-customerInfo="{ item }">
                    <div class="d-flex flex-column">
                        <span class="fw-semibold">{{ item.fullName }}</span>
                        <span class="text-body-secondary">{{ item.phone }}</span>
                        <span class="text-body-secondary">{{ item.email }}</span>
                    </div>
                </template>

<template #cell-source="{ item }">
                    <BaseBadge :label="getSourceLabel(item.leadSource)" :color="getSourceColor(item.leadSource)" />
                </template> -->

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

                <template #cell-lastContact="{ item }">
                    <span>{{ getLastContactDate(item.studentActivities) || t('student.noContact') }}</span>
                </template>

                <template #cell-actions="{ item }">
                    <el-button @click="viewModelEvent(item)" type="primary" link>{{ t('common.view') }}</el-button>
                    <el-button @click="editModelEvent(item)" type="primary" link>{{ t('common.edit') }}</el-button>
                    <el-button @click="deleteModelEvent" type="success" link>{{
                        t('student.convert') }}</el-button>
                </template>
                <template #cell-companyId="{ item }">
                    <BaseBadge :label="getRegistrationCompany(item.companyId)"
                        :color="item.type === 1 ? 'lightBlue' : 'lightPurple'" />
                </template>
            </BaseTable>
        </div>

        <CustomeDialogWrapper v-model:visible="showFormModal" :mode="dialogMode" :loading="formLoading"
            :student-data="normalizeStudent(studentStore.selectedStudent)" @submit="handleSave"
            @delete="deleteModelEvent" />

        <CustomeDetailDialog v-model:visible="showDetaiFormModal" :loading="formLoading"
            :student-data="normalizeStudent(studentStore.selectedStudent)"
            @refreshList="studentStore.fetchPagedStudents" @submit="handleSave" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useStudentStore } from '@/stores/studentStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
// import StudentDialog from './StudentDialog.vue'; // Sẽ tạo ở bước 3.2
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import CustomeDialog from './CustomeDialog.vue';
import CustomeDialogWrapper from './studentDialogWrapper.vue';
import { useEmployeeStore } from '@/stores/employeeStore';
import { useAgeGroupStore } from '@/stores/ageGroupStore';
import { useCompanyStore } from '@/stores/companyStore';
import CustomeDetailDialog from './CustomeDetailDialog.vue';
import type { StudentModel } from '@/api/StudentApi';
const employeeStore = useEmployeeStore()
const ageGroupStore = useAgeGroupStore()
const companyStore = useCompanyStore()
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
const { t } = useI18n();
const studentStore = useStudentStore();
const notificationStore = useNotificationStore();


const showFormModal = ref(false);
const showDetaiFormModal = ref(false);
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
type StudentFilter = { searchTerm?: string; status?: string | number | null; source?: string };
const filter = ref<StudentFilter>({ searchTerm: '', status: null, source: '' });
import { StudentStatus } from '@/types';
// --- Helpers (Giả định giá trị Enum/String) ---
const StudentStatusEnum = StudentStatus; // Ví dụ
const getRegistrationCompany = (companyId?: string) => {
    if (!companyId) return '';
    // companyStore shape may vary between projects; try common property names and fallback gracefully
    const companies = (companyStore as any).companies || (companyStore as any).list || [];
    const company = companies.find((c: any) => c.id === companyId || c.companyId === companyId);
    return company?.name || company?.companyName || '';
};
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
}
const getStatusColor = (status?: number | null) => {
    const normalized = normalizeStatus(status)
    if (normalized === StudentStatusEnum.Prospect) return 'warning'
    if (normalized === StudentStatusEnum.NewRegister) return 'success'
    if (normalized === StudentStatusEnum.Enrolled) return 'success'
    // if (normalized === StudentStatusEnum.NewRegister || normalized === StudentStatusEnum.Enrolled) return 'success'
    if (normalized === StudentStatusEnum.Paused) return 'gray'
    if (normalized === StudentStatusEnum.Dropped) return 'danger'
    if (normalized === StudentStatusEnum.Graduated) return 'purple'
    return 'default'
}
//lấy danh sách các student có studentStatus là Lead hoặc NewCustomer
const studentList = computed(() => {
    return studentStore.students.filter(student =>
        resolveStatus(student) === StudentStatusEnum.NewRegister ||
        resolveStatus(student) === StudentStatusEnum.Enrolled ||
        resolveStatus(student) === StudentStatusEnum.Paused ||
        resolveStatus(student) === StudentStatusEnum.Graduated ||
        resolveStatus(student) === StudentStatusEnum.Dropped
    );
});
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

// const isLead = (status: number) => status === StudentStatusEnum.Lead || status === StudentStatusEnum.NewCustomer;

const getLastContactDate = (activities: any) => {
    if (!activities || activities.length === 0) return null;

    // Logic tìm ngày tương tác gần nhất
    return activities.activityDate; // Mock
};

// Return employee display name by id (safe lookup - handles missing store or different shapes)
const getEmployee = (employeeId: string | number | null | undefined): string | null => {

    if (employeeId === null || employeeId === undefined) return null;

    return employeeStore.employees.find(emp => emp.id === employeeId)?.applicationUser?.fullName || null;
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
    { key: 'studentCode', labelKey: 'student.code', filterType: 'text', width: 100, isBold: true },
    { key: 'fullName', labelKey: 'student.name', width: 250, filterType: 'text', },
    { key: 'email', labelKey: 'student.email', width: 150, filterType: 'text', },
    { key: 'phone', labelKey: 'student.phone', width: 200, filterType: 'text', },
    { key: 'companyId', labelKey: 'student.company', filterType: 'text', },
    { key: 'status', labelKey: 'student.status', width: 200, filterType: 'text', },
    // {
    //     key: 'priority', labelKey: 'student.priority', width: 100, filterType: "select", filterOptions: [
    //         { label: t('common.high'), value: 2 },
    //         { label: t('common.medium'), value: 1 },
    //         { label: t('common.low'), value: 0 },
    //     ],
    // },
    // { key: 'employeeName', labelKey: 'student.advisor', filterType: "text", },
    // { key: 'lastContact', labelKey: 'student.lastContact', width: 120 },
    { key: 'actions', labelKey: 'common.actions', width: 150 },
];

// --- Handlers ---
function onTableFilter(val: Record<string, any>) {
    studentStore.setFilter(val);
    studentStore.fetchPagedStudents();
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
        //thiết lập trạng thái cho student bằng 6
        student.StudentStatus = StudentStatusEnum.NewRegister;
        await studentStore.saveStudent(student);
        notificationStore.showToast('success', { key: 'toast.saveSuccess', params: { model: t('models.student') } });
        await studentStore.fetchAllStudents();
    } catch (err: any) {
        console.error('Error saving:', err);
    } finally {
        stopLoading();
        showFormModal.value = false;
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
    studentStore.fetchAllStudents();
});

onMounted(() => {
    studentStore.fetchAllStudents();
    employeeStore.fetchAllEmployees();
    ageGroupStore.fetchAllAgeGroups();
    companyStore.fetchAllCompanies();

    // *MOCK DATA tạm thời*
    // studentStore.students = [
    //     { id: '1', studentCode: 'KH001', fullName: 'Nguyễn Thị Lan', phone: '0901234567', email: 'lan.nguyen@email.com', leadSource: 'Website', studentStatus: 0, priority: 2, employeeName: 'Nguyễn Văn A' },
    //     { id: '2', studentCode: 'KH002', fullName: 'Trần Văn Minh', phone: '0987654321', email: 'minh.tran@email.com', leadSource: 'Social', studentStatus: 1, priority: 1, employeeName: 'Trần Thị B' },
    //     { id: '3', studentCode: 'KH003', fullName: 'Lê Thị Mai', phone: '0909876543', email: 'mai.le@email.com', leadSource: 'Facebook', studentStatus: 2, priority: 2, employeeName: 'Nguyễn Văn A' },
    // ] as any;
    studentStore.total = studentStore.students.length;
    // console.log('Students loaded:', studentStore.students);

});
</script>
