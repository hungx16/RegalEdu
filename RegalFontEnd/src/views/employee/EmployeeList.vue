<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="employee.headerTitle" headerDesc="employee.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <div class="row g-4 mb-8">
            <div class="col-12 col-md-6">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('employee.totalEmployees') }}</span>
                        <i class="bi bi-person-badge fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ employeeStore.total }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-6">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('models.Employee') }}</span>
                        <i class="bi bi-stars fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{employeeStore.employees.filter(employee => employee.status ===
                        StatusType.Active).length}}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>
        </div>

        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('employee.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('employee.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredEmployeesAll"
                    :loading="formLoading" :showPagination="true" :page="page" :total="filteredEmployeesAll.length"
                    :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
                    @update:rows="val => selectedRowsData = val" @edit="editModelEvent" @view="viewModelEvent"
                    :showActionsColumn="true" :showEdit="true" :showDelete="true" :showView="true"
                    @delete="handleDelete" @update:page="val => page = val" @update:pageSize="onPageSizeChange">
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-company="{ item }">
                        <span>{{ item.companyName || '-' }}</span>
                    </template>
                    <template #cell-department="{ item }">
                        <span>{{ item.departmentName || '-' }}</span>
                    </template>
                    <template #cell-position="{ item }">
                        <span>{{ item.positionName || '-' }}</span>
                    </template>
                    <template #cell-employeeCode="{ item }">
                        <span>{{ item.applicationUser?.userCode || '-' }}</span>
                    </template>
                    <template #cell-fullName="{ item }">
                        <span>{{ item.applicationUser?.fullName || '-' }}</span>
                    </template>
                </BaseTable>
            </div>
        </div>

        <EmployeeDialog v-model:visible="showFormModal" :mode="dialogMode"
            :employee-data="employeeStore.selectedEmployee" :loading="formLoading" @submit="handleSave"
            @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useEmployeeStore } from '@/stores/employeeStore';
import { useCommonStore } from '@/stores/commonStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import EmployeeDialog from './EmployeeDialog.vue';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout'
import { useRegionStore } from '@/stores/regionStore'
import { useCompanyStore } from '@/stores/companyStore'
import { useDivisionStore } from '@/stores/divisionStore'
import { useDepartmentStore } from '@/stores/departmentStore'
import { usePositionStore } from '@/stores/positionStore'
import { formatDate } from '@/utils/format';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { StatusType } from '@/types';

const { t } = useI18n();
const employeeStore = useEmployeeStore();
const commonStore = useCommonStore();
const notificationStore = useNotificationStore();
const regionStore = useRegionStore();
const companyStore = useCompanyStore();
const divisionStore = useDivisionStore();
const departmentStore = useDepartmentStore();
const positionStore = usePositionStore();

const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(30);
const filter = ref({});
const selectedRowsData = ref([]);
const companyFilterOptions = ref<{ label: string; value: any }[]>([]);
const departmentFilterOptions = ref<{ label: string; value: any }[]>([]);
const positionFilterOptions = ref<{ label: string; value: any }[]>([]);

const columns = computed<BaseTableColumn[]>(() => [
    { key: 'employeeCode', labelKey: 'employee.code', filterType: 'text', sortable: true, sticky: true, width: 150, isBold: true },
    { key: 'fullName', labelKey: 'employee.name', filterType: 'text', sortable: true, sticky: true, isBold: true },
    {
        key: 'companyName', labelKey: 'employee.company', width: 160, sortable: false, align: 'center', filterType: 'select',
        filterOptions: companyFilterOptions.value
    },
    {
        key: 'departmentName', labelKey: 'employee.department', width: 160, sortable: false, align: 'center', filterType: 'select',
        filterOptions: departmentFilterOptions.value
    },
    {
        key: 'positionName', labelKey: 'employee.position', width: 160, sortable: false, align: 'center', filterType: 'select',
        filterOptions: positionFilterOptions.value
    },
    { key: 'employeeTax', labelKey: 'employee.tax', filterType: 'text', sortable: false, width: 140, align: 'center' },
    { key: 'employeeStartedDate', labelKey: 'employee.startDate', filterType: 'date', sortable: true, width: 180, align: 'center', formatter: (value: string) => formatDate(value, 'DD/MM/YYYY') },
    { key: 'employeeEndDate', labelKey: 'employee.endDate', filterType: 'date', sortable: true, width: 180, align: 'center', formatter: (value: string) => formatDate(value, 'DD/MM/YYYY') },
    {
        key: 'status', labelKey: 'position.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 160 },
    { key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 160, formatter: (value: string) => formatDate(value, 'DD/MM/YYYY') },
    { key: 'actions', labelKey: 'common.actions', width: 160 },
]);
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(50000)

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);
watch(
    () => companyStore.companies,
    (companies) => {
        companyFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...companies.map(company => ({
                label: company.companyName,
                value: company.companyName,
                isLocale: false,
            })),
            {
                label: t('common.none'),
                value: t('common.none'),
                isLocale: false,
            }
        ];
    },
    { immediate: true }
);
watch(
    () => departmentStore.departments,
    (departments) => {
        departmentFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...departments.map(department => ({
                label: department.departmentName,
                value: department.departmentName,
                isLocale: false,
            })),
            {
                label: t('common.none'),
                value: t('common.none'),
                isLocale: false,
            }
        ];
    },
    { immediate: true }
);
watch(
    () => positionStore.positions,
    (positions) => {
        positionFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...positions.map(position => ({
                label: position.positionName,
                value: position.positionName,
                isLocale: false,
            })),
            {
                label: t('common.none'),
                value: t('common.none'),
                isLocale: false,
            }
        ];
    },
    { immediate: true }
);

// Tính số phòng ban cho từng division
const employeesWithCount = computed(() =>
    employeeStore.employees.map(item => ({
        ...item,
        fullName: item.applicationUser?.fullName || t('common.none'),
        companyName: item.company?.companyName || t('common.none'),
        departmentName: item.department?.departmentName || t('common.none'),
        positionName: item.position?.positionName || t('common.none'),
    }))
);
// Lọc data theo filter
const filteredEmployeesAll = computed(() => {
    let arr = employeesWithCount.value;
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            if (key === 'createdAt') {
                arr = arr.filter(item => {
                    if (!item[key]) return false;
                    const dateOnly = String(item[key]).substring(0, 10);
                    return dateOnly === val;
                });
            } else {
                arr = arr.filter(item =>
                    String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase())
                );
            }
        }
    });
    return arr;
});
function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.employee') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function handleDelete(employees) {
    const list = Array.isArray(employees) ? employees : [employees];
    const ids = list
        .filter(item => typeof item.id === 'string' && item.id)
        .map(item => item.id as string);

    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.employee') } },
        async () => {
            try {
                startLoading()
                await employeeStore.deleteEmployees(ids);
                await employeeStore.fetchAllEmployees();
            } catch (err) {
                console.error('Error deleting:', err);
            } finally {
                stopLoading();
                showFormModal.value = false;
            }

        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    await commonStore.generateCode('NV', 'AspNetUsers', 'UserCode', 4);
    employeeStore.selectEmployee(null);
    showFormModal.value = true;
}

async function editModelEvent(employee) {
    dialogMode.value = 'edit';
    employeeStore.selectEmployee({ ...employee });
    showFormModal.value = true;
}

async function viewModelEvent(employee) {

    dialogMode.value = 'view';
    employeeStore.selectEmployee({ ...employee });
    showFormModal.value = true;
}

async function handleSave(employee) {
    startLoading()
    try {
        await employeeStore.saveEmployee(employee);
        showFormModal.value = false;

        if (employee.id) {
            notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.employee') } });
        } else {
            notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.employee') } });
        }
        await employeeStore.fetchAllEmployees();
        stopLoading() // Hoặc chỉ cần đóng dialog, watcher sẽ reset loading về false
        showFormModal.value = false;
    } catch (err: any) {
        stopLoading()
        console.error('Error saving:', err?.response?.data?.errors || err);
    }
}
function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}

onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'employee.add', type: 'add' },
            { event: 'delete', label: 'employee.delete', type: 'delete' },
        ],
    });
    await Promise.all([
        positionStore.positions.length ? null : positionStore.fetchAllPositions(),
        departmentStore.departments.length ? null : departmentStore.fetchAllDepartments(),
        divisionStore.divisions.length ? null : divisionStore.fetchAllDivisions(),
        companyStore.companies.length ? null : companyStore.fetchAllCompanies(),
        regionStore.regions.length ? null : regionStore.fetchAllRegions(),
    ])
    await employeeStore.fetchAllEmployees();

});
</script>
