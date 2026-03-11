<template>
    <div>
        <!-- FilterComponent (đặt ngoài row/card) -->
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            @transfer="openTransferDialog"
            headerTitle="admissionsQuota.headerTitle" headerDesc="admissionsQuota.headerDesc"
            :disabledDelete="getDisableDelete" :disabledAssign="false" class="mb-6" />

        <!-- BẢNG DỮ LIỆU -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('admissionsQuota.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('admissionsQuota.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredQuotasAll"
                    :loading="admissionsQuotaStore.loading" :showPagination="true" :page="page"
                    :total="filteredQuotasAll.length" :pageSize="pageSize" :filter="filter"
                    @update:filter="onTableFilter" @update:rows="val => (selectedRowsData = val)" @edit="editModelEvent"
                    @view="viewModelEvent" :showActionsColumn="true" :showEdit="true" :showDelete="true"
                    :showView="true" @delete="handleDelete" @update:page="val => (page = val)"
                    @update:pageSize="onPageSizeChange">
                    <template #cell-quotaStage="{ item }">
                        <BaseBadge :label="t(statusKeyMap[item.quotaStage] ?? 'common.unknown')"
                            :color="statusColorMap[item.quotaStage] ?? 'gray'" bordered bold />
                    </template>

                    <template #cell-monthYear="{ item }">
                        <BaseBadge :label="item.monthYear" color="gray" bordered />
                    </template>
                    <template #cell-totalRevenue="{ item }">
                        <span class="fw-semibold">{{ formatCurrency(item.totalRevenue) }}</span>
                    </template>
                </BaseTable>
            </div>
        </div>
        <AdmissionsQuotaDialog v-model:visible="showFormModal" :mode="dialogMode"
            :quota-data="admissionsQuotaStore.selectedQuota" :loading="formLoading" @submit="handleSave"
            @close="handleClose" @delete="handleDelete" />
        <!-- ⬅️ Gắn dialog luân chuyển -->
        <QuotaRoleTransferDialog v-model:show="showTransfer" :quota-options="quotas"
            @applied="admissionsQuotaStore.fetchAllAdmissionsQuotas()" />
    </div>
</template>

<script setup lang="ts">
// ===== IMPORT =====
import { onMounted, ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import AdmissionsQuotaDialog from '@/views/admissions-quota/AdmissionsQuotaDialog.vue';
import { useAdmissionsQuotaStore } from '@/stores/admissionsQuotaStore';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { formatDate, formatCurrency } from '@/utils/format';
import { useAdmissionsQuotaRegionStore } from '@/stores/admissionsQuotaRegionStore';
import QuotaRoleTransferDialog from './QuotaRoleTransferDialog.vue' // ⬅️ mới
// ⬅️ trạng thái mở dialog luân chuyển
const showTransfer = ref(false)
// ===== STORE & I18N =====
const { t } = useI18n();
const admissionsQuotaStore = useAdmissionsQuotaStore();
const admissionsQuotaRegionStore = useAdmissionsQuotaRegionStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);
const quotas = computed(() => admissionsQuotaStore.quotas)
function openTransferDialog() { showTransfer.value = true }

// ===== STATE =====
const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(20);
const filter = ref<Record<string, any>>({});
const selectedRowsData = ref<any[]>([]);
// map key i18n theo filterOptions
const statusKeyMap: Record<number, string> = {
    0: 'admissionsQuota.Draft',
    1: 'admissionsQuota.Allocated',
    2: 'admissionsQuota.InProgress',
    3: 'admissionsQuota.Completed',
};

// map màu hiển thị
const statusColorMap: Record<number, string> = {
    0: 'gray',    // Draft
    1: 'blue',    // Allocated
    2: 'orange',  // In progress
    3: 'green',   // Completed
};

// ===== HEADER CONFIG =====
const listHeaderParams = {
    listParams: [],
    listBtn: [
        { event: 'add', label: 'admissionsQuota.add', type: 'add' },
        { event: 'transfer', label: 'admissionsQuota.transfer', type: 'assign' },
        { event: 'delete', label: 'admissionsQuota.delete', type: 'delete' },
    ],
};

// ===== COLUMNS =====
const columns: BaseTableColumn[] = [
    { key: 'monthYear', labelKey: 'admissionsQuota.monthYear', filterType: 'text', sortable: true, width: 140, sticky: true, isBold: true },
    { key: 'totalRevenue', labelKey: 'admissionsQuota.totalRevenue', filterType: 'text', sortable: true, width: 220, align: 'right' },
    { key: 'companyCount', labelKey: 'admissionsQuota.totalCompanies', filterType: 'text', sortable: true, width: 160, align: 'center' },
    { key: 'totalSales', labelKey: 'admissionsQuota.totalSales', filterType: 'text', sortable: true, width: 160, align: 'center' },
    {
        key: 'quotaStage', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'admissionsQuota.Draft', value: 0 },
            { label: 'admissionsQuota.Allocated', value: 1 },
            { label: 'admissionsQuota.InProgress', value: 2 },
            { label: 'admissionsQuota.Completed', value: 3 },

        ],
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 220 },
    { key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, formatter: (v: string) => formatDate(v, 'DD/MM/YYYY') },
    { key: 'actions', labelKey: 'common.actions' },
];

// ===== COMPUTED =====
const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

const quotasNormalized = computed(() =>
    (admissionsQuotaStore.quotas || []).map((x: any) => ({
        id: x.id,
        monthYear: x.monthYear ?? (x.month && x.year ? `${String(x.month).padStart(2, '0')}/${x.year}` : ''),
        totalRevenue: x.totalQuota ?? 0,
        companyCount: x.companyCount ?? 0,
        totalSales: x.currentSales ?? 0,
        createdBy: x.createdBy ?? '—',
        createdAt: x.createdAt ?? '',
        quotaStage: x.quotaStage ?? 0,
        month: x.month ?? 0,
        year: x.year ?? 0,
    }))
);

const filteredQuotasAll = computed(() => {
    let arr = quotasNormalized.value;
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val !== '' && val != null) {
            if (key === 'createdAt') {
                arr = arr.filter((item: any) => String(item.createdAt ?? '').startsWith(String(val)));
            } else if (key === 'status') {
                arr = arr.filter((item: any) => String(item.status) === String(val));
            } else {
                arr = arr.filter((item: any) => String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase()));
            }
        }
    });
    return arr;
});



// ===== HANDLERS =====
function onDeleteClicked() {
    if (!selectedRowsData.value.length) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.admissionsQuota') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function handleDelete(items: any | any[]) {
    const list = Array.isArray(items) ? items : [items];
    const ids = list.filter(it => it.id).map(it => it.id);
    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.admissionsQuota') } },
        async () => {
            try {
                startLoading();
                await admissionsQuotaStore.deleteQuotas(ids);
                await admissionsQuotaStore.fetchAllAdmissionsQuotas();
            } finally {
                stopLoading();
                showFormModal.value = false;
            }
        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    admissionsQuotaStore.selectQuota(null);
    showFormModal.value = true;
}

async function editModelEvent(row: any) {
    dialogMode.value = 'edit';
    await admissionsQuotaRegionStore.FetchAdmissionsQuotaRegionByAdmissionsQuotaId(row.id);
    const data = { ...row, admissionsQuotaRegions: admissionsQuotaRegionStore.regions };
    admissionsQuotaStore.selectQuota({ ...data });
    showFormModal.value = true;
}

async function viewModelEvent(row: any) {
    dialogMode.value = 'edit';
    await admissionsQuotaRegionStore.FetchAdmissionsQuotaRegionByAdmissionsQuotaId(row.id);
    const data = { ...row, admissionsQuotaRegions: admissionsQuotaRegionStore.regions };
    admissionsQuotaStore.selectQuota({ ...data });
    showFormModal.value = true;
}

async function handleSave(model: any) {
    try {
        startLoading();
        await admissionsQuotaStore.saveQuota(model);
        await admissionsQuotaStore.fetchAllAdmissionsQuotas();
        showFormModal.value = false;
    } catch (err) {
        console.error(err);
    } finally {
        stopLoading();
    }
}
function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}
async function handleClose() {
    showFormModal.value = false;
    await admissionsQuotaRegionStore.fetchAllRegions();
}
// ===== LIFECYCLE =====
onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams(listHeaderParams);
    await admissionsQuotaStore.fetchAllAdmissionsQuotas();
});
</script>

<style scoped>
.summary-card {
    min-height: 132px;
}
</style>
