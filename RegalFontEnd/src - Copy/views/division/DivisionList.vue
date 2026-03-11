<template>
    <div>
        <!-- FilterComponent (đặt ngoài row/card) -->
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="division.headerTitle" headerDesc="division.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <!-- ROW cho 2 CARD -->
        <div class="row g-4 mb-8">
            <div class="col-12 col-md-6">
                <div class="summary-card bg-body  text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('division.totalBlocks') }}</span>
                        <i class="bi bi-buildings fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ divisionStore.total }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-6">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('division.totalDepartments') }}</span>
                        <i class="bi bi-building fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ divisionStore.totalDepartments }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('division.belongingBlocks') }}</div>
                </div>
            </div>
        </div>

        <!-- BẢNG DỮ LIỆU -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('division.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('division.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredDivisionsAll"
                    :loading="divisionStore.loading" :showPagination="true" :page="page"
                    :total="filteredDivisionsAll.length" :pageSize="pageSize" :filter="filter"
                    @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val" @edit="editModelEvent"
                    @view="viewModelEvent" :showActionsColumn="true" :showEdit="true" :showDelete="true"
                    :showView="true" @delete="handleDelete" @update:page="val => page = val"
                    @update:pageSize="onPageSizeChange">
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-divisionLevel="{ item }">
                        <BaseBadge :label="item.divisionLevel"
                            :color="item.divisionLevel === 1 ? 'green' : item.divisionLevel === 2 ? 'blue' : 'purple'"
                            bordered bold displayType="level" />
                    </template>
                    <template #cell-divisionCode="{ item }">
                        <BaseBadge :label="item.divisionCode" color="blue" soft :bold="true" bordered />
                    </template>
                    <template #cell-department="{ item }">
                        <BaseBadge :label="item.department" color="deepPurple" displayType="department" />
                    </template>
                </BaseTable>
            </div>
        </div>

        <DivisionDialog v-model:visible="showFormModal" :mode="dialogMode"
            :division-data="divisionStore.selectedDivision" :loading="formLoading" @submit="handleSave"
            @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
/** ===== IMPORT ===== */
import { onMounted, ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useDivisionStore } from '@/stores/divisionStore';
import { useCommonStore } from '@/stores/commonStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import type { DivisionModel } from '@/api/DivisionApi';
import DivisionDialog from '@/views/division/DivisionDialog.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { formatDate } from '@/utils/format';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';

/** ===== KHAI BÁO BIẾN & REF ===== */
const { t } = useI18n();
const divisionStore = useDivisionStore();
const commonStore = useCommonStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)

const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(30);
const filter = ref({});
const selectedRowsData = ref<Array<DivisionModel>>([]);

/** ===== KHAI BÁO CẤU HÌNH ===== */
const listHeaderParams = {
    listParams: [],
    listBtn: [
        { event: 'add', label: 'division.add', type: 'add' },
        { event: 'delete', label: 'division.delete', type: 'delete' },
    ],
};

const columns: BaseTableColumn[] = [
    { key: 'divisionName', labelKey: 'division.name', filterType: 'text', sortable: true, sticky: true, width: 160, isBold: true },
    { key: 'divisionCode', labelKey: 'division.code', filterType: 'text', sortable: true, sticky: true, width: 160, isBold: true, align: 'center' },
    { key: 'description', labelKey: 'division.description', filterType: 'text', sortable: false },
    { key: 'divisionLevel', labelKey: 'division.level', filterType: 'text', sortable: true, width: 120, align: 'center' },
    { key: 'department', labelKey: 'models.Department', sortable: true, width: 180, align: 'center' },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 250 },
    {
        key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 250,
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    { key: 'actions', labelKey: 'common.actions', width: 220 },
];


/** ===== COMPUTED ===== */
// Chặn nút xoá nếu không chọn dòng nào
const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

// Tính số phòng ban cho từng division
const divisionsWithCount = computed(() =>
    divisionStore.divisions.map(item => ({
        ...item,
        department: item.departments?.length ?? 0,
    }))
);

// Lọc data theo filter
const filteredDivisionsAll = computed(() => {
    let arr = divisionsWithCount.value;
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

/** ===== FUNCTION HANDLER ===== */
function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.division') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1; // reset page khi filter
}

function handleDelete(divisions: DivisionModel | DivisionModel[]) {
    const list = Array.isArray(divisions) ? divisions : [divisions];
    const ids = list
        .filter(item => typeof item.id === 'string' && item.id)
        .map(item => item.id as string);

    notificationStore.showConfirm(
        {
            key: 'toast.delete',
            params: { model: t('models.division') },
        },
        async () => {
            startLoading()

            try {
                await divisionStore.deleteDivisions(ids);
                await divisionStore.fetchAllDivisions();
                stopLoading() // Hoặc chỉ cần đóng dialog, watcher sẽ reset loading về false
                showFormModal.value = false;

            } catch (err: any) {
                console.error('Error deleting:', err);
                stopLoading()
            }
        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    await commonStore.generateCode('DI', 'Division', 'DivisionCode', 4);
    divisionStore.selectDivision(null);
    showFormModal.value = true;
}

function editModelEvent(division: any) {
    dialogMode.value = 'edit';
    divisionStore.selectDivision({ ...division });
    showFormModal.value = true;
}

function viewModelEvent(division: any) {
    dialogMode.value = 'view';
    divisionStore.selectDivision({ ...division });
    showFormModal.value = true;
}

async function handleSave(division: any) {
    startLoading()
    try {
        await divisionStore.saveDivision(division);
        if (division.id) {
            notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.division') } });
        } else {
            notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.division') } });
        }
        await divisionStore.fetchAllDivisions();
        stopLoading() // Hoặc chỉ cần đóng dialog, watcher sẽ reset loading về false
        showFormModal.value = false;

    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
        stopLoading()
    }
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}

/** ===== LIFECYCLE ===== */
onMounted(() => {
    filterComponentRef.value?.initListHeaderParams(listHeaderParams);
    divisionStore.fetchAllDivisions();
});
</script>
