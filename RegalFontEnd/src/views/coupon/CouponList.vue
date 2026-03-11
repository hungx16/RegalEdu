<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            @monthlyCompanyAllocation="monthlyCompanyAllocationFunction" headerTitle="company.headerTitle"
            headerDesc="company.headerDesc" :disabledDelete="getDisableDelete" class="mb-6" />

        <div class="row g-4 mb-8">
            <div class="col-12 col-md-4">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('company.totalCompanies') }}</span>
                        <i class="bi bi-building fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ companyStore.total }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-4">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('company.totalByProvince') }}</span>
                        <i class="bi bi-geo-alt fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ companiesByProvince }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('company.provinceStatistic') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-4">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('common.TotalStudents') }}</span>
                        <i class="bi bi-people fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ companiesByProvince }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.allCompanies') }}</div>
                </div>
            </div>
        </div>

        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('company.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('company.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredCompaniesAll"
                    :loading="companyStore.loading" :showPagination="true" :page="page"
                    :total="filteredCompaniesAll.length" :pageSize="pageSize" :filter="filter"
                    @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val" @edit="editModelEvent"
                    @view="viewModelEvent" :showActionsColumn="true" :showEdit="true" :showDelete="true"
                    :showView="true" @delete="handleDelete" @update:page="val => page = val"
                    @update:pageSize="onPageSizeChange">
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-managerName="{ item }">
                        <BaseBadge :label="item.managerName || t('common.none')"
                            :colorByLabelMap="{ [t('common.none')]: 'red' }" :containerBackgroundColor="'#ffffff'" />
                    </template>
                    <template #cell-provinceName="{ item }">
                        <BaseBadge :rawLabel="true" :label="item.provinceName || t('common.none')" />
                    </template>
                </BaseTable>
            </div>
        </div>

        <CompanyDialog v-model:visible="showFormModal" :mode="dialogMode" :company-data="companyStore.selectedCompany"
            :loading="formLoading" @submit="handleSave" @delete="handleDelete" />
        <CompanyRegionAllocation v-model:visible="showAllocationDialog"
            :companyId="companyStore.selectedCompany?.id ?? null" />
    </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed, watch, onBeforeMount } from 'vue';
import { useI18n } from 'vue-i18n';
import { useCompanyStore } from '@/stores/companyStore';
import { useCommonStore } from '@/stores/commonStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import CompanyDialog from '@/views/company/CompanyDialog.vue';
import { formatDate } from '@/utils/format';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { useRegionStore } from '@/stores/regionStore';
import { useEmployeeStore } from '@/stores/employeeStore';
import CompanyRegionAllocation from '@/views/company/CompanyRegionAllocation.vue';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
const { t } = useI18n();
const companyStore = useCompanyStore();
const commonStore = useCommonStore();
const notificationStore = useNotificationStore();
const employeeStore = useEmployeeStore();
const regionStore = useRegionStore();
const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(30);
const filter = ref({});
const selectedRowsData = ref([]);
const showAllocationDialog = ref(false)
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)

const managerFilterOptions = ref<{ label: string; value: any }[]>([]);
const provinceFilterOptions = ref<{ label: string; value: any }[]>([]);
const regionFilterOptions = ref<{ label: string; value: any }[]>([]);

const columns = computed<BaseTableColumn[]>(() => [
    { key: 'companyCode', labelKey: 'company.code', filterType: 'text', sortable: true, sticky: true, width: 150, isBold: true, align: 'center' },
    { key: 'companyName', labelKey: 'company.name', filterType: 'text', sortable: true, sticky: true, width: 180, isBold: true },
    { key: 'companyAddress', labelKey: 'company.address', filterType: 'text', sortable: false },
    { key: 'companyPhone', labelKey: 'company.phone', sortable: false, width: 130, align: 'center', filterType: 'text' },
    { key: 'currentRegion', labelKey: 'company.currentRegion', width: 160, sortable: true, align: 'center', filterType: 'select', filterOptions: regionFilterOptions.value },

    { key: 'provinceName', labelKey: 'company.provinceCode', width: 160, sortable: true, align: 'center', filterType: 'select', filterOptions: provinceFilterOptions.value },
    { key: 'managerName', labelKey: 'company.manager', width: 150, sortable: false, align: 'center', filterType: 'select', filterOptions: managerFilterOptions.value },
    { key: 'establishmentDate', labelKey: 'company.establishmentDate', filterType: 'date', sortable: true, formatter: (value: string) => formatDate(value, 'DD/MM/YYYY'), width: 150, align: 'center' },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 200 },
    { key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, formatter: (value: string) => formatDate(value, 'DD/MM/YYYY'), width: 200 },
    { key: 'actions', labelKey: 'common.actions', width: 180 },
]);

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

const companiesByProvince = computed(() => {
    // Thống kê số lượng tỉnh/thành có công ty (provinceCode khác nhau)
    const codes = new Set(companyStore.companies.map(x => x.provinceCode).filter(x => !!x));
    return codes.size;
});
const companiesWithDisplay = computed(() =>
    companyStore.companies.map(item => ({
        ...item,
        provinceName: commonStore.provinces.find(p => p.provinceCode === item.provinceCode)?.provinceName ?? t('common.none'),
        managerName: item.manager?.applicationUser?.fullName ?? t('common.none'),
        currentRegion: regionStore.regions.find(r => r.id === item.logRegionComs?.find(t => t.companyId === item.id && t.status === 0)?.regionId)?.regionName ?? t('common.none'),
    }))
);


const filteredCompaniesAll = computed(() => {
    let arr = companiesWithDisplay.value;
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            if (key === 'establishmentDate' || key === 'createdAt') {
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

watch(
    () => commonStore.provinces,
    (provinces) => {
        provinceFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...provinces.map(province => ({
                label: province.provinceName,
                value: province.provinceName,
                isLocale: false,
            }))
            ,
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
    () => regionStore.regions,
    (regions) => {
        regionFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...regions.map(region => ({
                label: region.regionName,
                value: region.regionName,
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
    () => employeeStore.employees,
    (emps) => {
        managerFilterOptions.value = [
            { label: ('common.all'), value: '', },
            ...emps.map(e => ({
                label: e.applicationUser.fullName,
                value: e.applicationUser.fullName,
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

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.company') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function handleDelete(companies) {
    const list = Array.isArray(companies) ? companies : [companies];
    const ids = list
        .filter(item => typeof item.id === 'string' && item.id)
        .map(item => item.id as string);

    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.company') } },
        async () => {
            startLoading();
            try {
                await companyStore.deleteCompanies(ids);
                await companyStore.fetchAllCompanies();
                showFormModal.value = false;

            } catch (err) {
                console.error('Error deleting:', err);
            } finally {
                stopLoading();
            }
        }
    );
}
async function monthlyCompanyAllocationFunction() {
    await companyStore.fetchAllCompanyRegions()

    showAllocationDialog.value = true


}
async function addModelEvent() {
    dialogMode.value = 'create';
    await commonStore.generateCode('CO', 'Company', 'CompanyCode', 4);
    companyStore.selectCompany(null);
    showFormModal.value = true;
}

function editModelEvent(company) {
    dialogMode.value = 'edit';
    companyStore.selectCompany({ ...company });
    showFormModal.value = true;
}

function viewModelEvent(company) {
    dialogMode.value = 'view';
    companyStore.selectCompany({ ...company });
    showFormModal.value = true;
}

async function handleSave(company) {
    try {
        startLoading();
        await companyStore.saveCompany(company);
        showFormModal.value = false;
        if (company.id) {
            notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.company') } });
        } else {
            notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.company') } });
        }
        await companyStore.fetchAllCompanies();
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
    }
    finally {
        stopLoading();
    }
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}
onBeforeMount(async () => {
    await companyStore.fetchAllCompanies();


});
onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'company.add', type: 'add' },
            { event: 'delete', label: 'company.delete', type: 'delete' },
            { event: 'monthlyCompanyAllocation', label: 'company.monthlyCompanyAllocation', type: 'monthlyCompanyAllocation' },
        ],
    });
    await commonStore.fetchProvinces();
    await regionStore.fetchAllRegions();
    await employeeStore.fetchAllEmployees();
});
</script>
