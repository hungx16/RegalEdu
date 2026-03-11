<template>
    <div>
        <!-- FilterComponent (đặt ngoài row/card) -->
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="region.headerTitle" headerDesc="region.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <!-- ROW cho 2 CARD -->
        <div class="row g-4 mb-8">
            <div class="col-6 col-md-3">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('region.totalRegions') }}</span>
                        <i class="bi bi-geo-alt fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ regionStore.totalActiveRegions }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('region.totalCompanies') }}</span>
                        <i class="bi bi-building fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ regionStore.totalCompanies }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.onRegion', {
                        number: regionStore.total
                    }) }}</div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('common.TotalStudents') }}</span>
                        <i class="bi bi-people fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ regionStore.totalCompanies }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.allRegions') }}</div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('common.Average') }}</span>
                        <i class="bi bi-calculator fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ regionStore.totalCompanies }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('models.Student') }} / {{ t('models.region') }}</div>
                </div>
            </div>
        </div>

        <!-- BẢNG DỮ LIỆU -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('region.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('region.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredRegionsAll"
                    :loading="regionStore.loading" :showPagination="true" :page="page"
                    :total="filteredRegionsAll.length" :pageSize="pageSize" :filter="filter"
                    @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val" @edit="editModelEvent"
                    @view="viewModelEvent" :showActionsColumn="true" :showEdit="true" :showDelete="true"
                    :showView="true" @delete="handleDelete" @update:page="val => page = val"
                    @update:pageSize="onPageSizeChange">
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-manager="{ item }">
                        <span>{{ item.manager?.applicationUser?.fullName || '-' }}</span>
                    </template>
                    <template #cell-companies="{ item }">
                        <BaseBadge :label="item.companies" color="deepPurple" displayType="department" />
                    </template>
                </BaseTable>
            </div>
        </div>

        <RegionDialog v-model:visible="showFormModal" :mode="dialogMode" :region-data="regionStore.selectedRegion"
            :loading="formLoading" @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRegionStore } from '@/stores/regionStore';
import { useCommonStore } from '@/stores/commonStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import RegionDialog from './RegionDialog.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { formatDate } from '@/utils/format';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)

const { t } = useI18n();
const regionStore = useRegionStore();
const commonStore = useCommonStore();
const notificationStore = useNotificationStore();

const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(30);
const filter = ref({});
const selectedRowsData = ref([]);

const listHeaderParams = {
    listParams: [],
    listBtn: [
        { event: 'add', label: 'region.add', type: 'add' },
        { event: 'delete', label: 'region.delete', type: 'delete' },
    ],
};

const columns: BaseTableColumn[] = [
    { key: 'regionCode', labelKey: 'region.code', filterType: 'text', sortable: true, sticky: true, width: 130, isBold: true, align: 'center' },
    { key: 'regionName', labelKey: 'region.name', filterType: 'text', sortable: true, sticky: true, width: 160, isBold: true },
    { key: 'description', labelKey: 'region.description', filterType: 'text', sortable: false },
    { key: 'companies', labelKey: 'region.companyNumber', width: 150, sortable: true, align: 'center' },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 180 },
    {
        key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 180,
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    { key: 'actions', labelKey: 'common.actions', width: 220 },
];

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

const regionsWithCompanyCount = computed(() =>
    regionStore.regions.map(item => ({
        ...item,
        companies: item.companies?.length ?? 0,
    }))
);

const filteredRegionsAll = computed(() => {
    let arr = regionsWithCompanyCount.value;
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
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.region') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function handleDelete(regions) {
    const list = Array.isArray(regions) ? regions : [regions];
    const ids = list
        .filter(item => typeof item.id === 'string' && item.id)
        .map(item => item.id as string);

    notificationStore.showConfirm(
        {
            key: 'toast.delete',
            params: { model: t('models.region') },
        },
        async () => {
            try {
                startLoading();
                await regionStore.deleteRegions(ids);
                await regionStore.fetchAllRegions();
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
    await commonStore.generateCode('RE', 'Region', 'RegionCode', 4);
    regionStore.selectRegion(null);
    showFormModal.value = true;
}

function editModelEvent(region) {
    dialogMode.value = 'edit';
    regionStore.selectRegion({ ...region });
    showFormModal.value = true;
}

function viewModelEvent(region) {
    dialogMode.value = 'view';
    regionStore.selectRegion({ ...region });
    showFormModal.value = true;
}

async function handleSave(region) {
    try {
        startLoading();
        await regionStore.saveRegion(region);
        showFormModal.value = false;
        if (region.id) {
            notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.region') } });
        } else {
            notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.region') } });
        }
        await regionStore.fetchAllRegions();
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
    } finally {
        stopLoading();
        showFormModal.value = false;
    }
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}

onMounted(() => {
    filterComponentRef.value?.initListHeaderParams(listHeaderParams);
    regionStore.fetchAllRegions();
});
</script>
