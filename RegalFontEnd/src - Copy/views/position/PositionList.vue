<template>
    <div>
        <!-- FilterComponent -->
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="position.headerTitle" headerDesc="position.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <!-- ROW cho 2 CARD -->
        <!-- <div class="row g-4 mb-8">
            <div class="col-12 col-md-6">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('position.totalPositions') }}</span>
                        <i class="bi bi-people fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ positionStore.totalActive }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-6">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ ('position.totalParents') }}</span>
                        <i class="bi bi-diagram-3 fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ 0 }}</div>
                    <div class="fs-7 text-body-secondary">{{ ('position.parentHint') }}</div>
                </div>
            </div>
        </div> -->

        <!-- BẢNG DỮ LIỆU -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('position.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('position.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredPositionsAll" :showIndex="true"
                    :loading="positionStore.loading" :showPagination="true" :page="page"
                    :total="filteredPositionsAll.length" :pageSize="pageSize" :filter="filter"
                    @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val" @edit="editModelEvent"
                    @view="viewModelEvent" :showActionsColumn="true" :showEdit="true" :showDelete="true"
                    :showView="true" @delete="handleDelete" @update:page="val => page = val"
                    @update:pageSize="onPageSizeChange">
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-positionCode="{ item }">
                        <BaseBadge :rawLabel="true" :label="item.positionCode" />
                    </template>
                    <template #cell-departments="{ item }">
                        <div style="display: flex; flex-wrap: wrap; gap: 8px;">
                            <BaseBadge v-for="dep in (item.departmentPositions || [])" :key="dep.departmentId"
                                :rawLabel="true"
                                :label="departmentStore.departments.find(d => d.id === dep.departmentId)?.departmentName || '-'"
                                class="me-1 mb-1" />
                        </div>
                    </template>
                    <template #cell-positionName="{ item }">
                        <div>
                            <div class="fw-bold">{{ item.positionName }}</div>
                            <div class="text-muted fs-7 text-truncate-1line"
                                style="white-space: nowrap; text-overflow: ellipsis; overflow: hidden; max-width: 220px;"
                                :title="item.description">
                                {{ item.description }}
                            </div>
                        </div>
                    </template>
                </BaseTable>
            </div>
        </div>

        <PositionDialog v-model:visible="showFormModal" :mode="dialogMode"
            :position-data="positionStore.selectedPosition" :loading="formLoading" @submit="handleSave"
            @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed, onBeforeMount, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { usePositionStore } from '@/stores/positionStore';
import { useCommonStore } from '@/stores/commonStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import type { PositionModel } from '@/api/PositionApi';
import PositionDialog from '@/views/position/PositionDialog.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { formatDate } from '@/utils/format';
import { useDepartmentStore } from '@/stores/departmentStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)

const { t } = useI18n();
const positionStore = usePositionStore();
const commonStore = useCommonStore();
const notificationStore = useNotificationStore();
const showFormModal = ref(false);
const filterComponentRef = ref();
const departmentStore = useDepartmentStore();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const departmentFilterOptions = ref<{ label: string; value: any }[]>([]);
const positionFilterOptions = ref<{ label: string; value: any }[]>([]);

const page = ref(1);
const pageSize = ref(30);
const filter = ref({});
const selectedRowsData = ref<Array<PositionModel>>([]);

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);


const positionsWithDisplay = computed(() =>
    positionStore.positions.map(item => ({
        ...item,
        departments: (item.departmentPositions || []).map(dep => dep.departmentId),
    }))
);

const filteredPositionsAll = computed(() => {
    let arr = positionsWithDisplay.value;
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            if (key === 'departments') {
                arr = arr.filter(item =>
                    (item.departmentPositions || []).some(dep => {
                        return (
                            String(dep.departmentId) === String(val)
                        );
                    })
                );
            }
            else if (key === 'createdAt') {
                arr = arr.filter(item => {
                    const value = item[key];
                    if (value == null) return false; // loại bỏ undefined hoặc null
                    return formatDate(value, 'yyyy-MM-dd') === val;
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

const listHeaderParams = {
    listParams: [],
    listBtn: [
        { event: 'add', label: 'position.add', type: 'add' },
        { event: 'delete', label: 'position.delete', type: 'delete' },
    ],
};
const columns = computed<BaseTableColumn[]>(() => [
    { key: 'positionCode', labelKey: 'position.code', filterType: 'text', sortable: true, sticky: true, width: 160, isBold: true, align: 'center' },
    { key: 'positionName', labelKey: 'position.name', filterType: 'text', sortable: true, sticky: true, width: 250, isBold: true },
    { key: 'departments', labelKey: 'position.belongingDepartments', filterType: 'select', filterOptions: departmentFilterOptions.value },
    {
        key: 'status', labelKey: 'position.status', filterType: 'select', sortable: true, width: 200, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 200 },

    {
        key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 220,
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    { key: 'actions', labelKey: 'common.actions', width: 220 },
]);


watch(
    () => departmentStore.departments,
    (deps) => {
        departmentFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...deps.map(dep => ({
                label: dep.departmentName,
                value: dep.id ?? '',
                isLocale: false,
            }))
        ];
    },
    { immediate: true }
);
watch(
    () => positionStore.positions,
    (positions) => {
        positionFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...positions.map(pos => ({
                label: pos.positionName,
                value: pos.positionName ?? '',
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

onBeforeMount(async () => {
    await departmentStore.fetchAllDepartments();
});
onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams(listHeaderParams);
    await positionStore.fetchAllPositions();
});

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.position') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function handleDelete(positions: PositionModel | PositionModel[]) {
    const list = Array.isArray(positions) ? positions : [positions];
    const ids = list
        .filter(item => typeof item.id === 'string' && item.id)
        .map(item => item.id as string);

    notificationStore.showConfirm(
        {
            key: 'toast.delete',
            params: { model: t('models.position') },
        },
        async () => {
            try {
                startLoading();
                await positionStore.deletePositions(ids);
                await positionStore.fetchAllPositions();
            } catch (err: any) {
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
    await commonStore.generateCode('PO', 'Position', 'PositionCode', 4);
    positionStore.selectPosition(null);
    showFormModal.value = true;
}

function editModelEvent(position: any) {
    dialogMode.value = 'edit';
    positionStore.selectPosition({ ...position });
    showFormModal.value = true;
}

function viewModelEvent(position: any) {
    dialogMode.value = 'view';
    positionStore.selectPosition({ ...position });
    showFormModal.value = true;
}

async function handleSave(position: any) {
    try {
        startLoading();
        await positionStore.savePosition(position);
        showFormModal.value = false;
        if (position.id) {
            notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.position') } });
        } else {
            notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.position') } });
        }
        await positionStore.fetchAllPositions();
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
    } finally {
        stopLoading();
    }
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    // KHÔNG reset page về 1 ở đây
}
</script>
