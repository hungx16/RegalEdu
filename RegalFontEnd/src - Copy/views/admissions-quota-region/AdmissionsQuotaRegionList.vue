<template>
    <div>
        <!-- Header (FilterComponent chỉ dùng để hiển thị tiêu đề/mô tả cho đúng design) -->
        <FilterComponent ref="filterComponentRef" headerTitle="admissionsQuotaRegion.headerTitle"
            headerDesc="admissionsQuotaRegion.headerDesc" :disabledDelete="true" class="mb-6" />

        <!-- Card: Danh sách chỉ tiêu theo tháng -->
        <el-card class="mb-6 shadow-sm" body-class="p-0">
            <div class="card-header px-4 py-4 d-flex align-items-center justify-between">
                <div>
                    <h3 class="m-0">{{ t('admissionsQuotaRegion.listTitle') }}</h3>
                    <div class="text-muted small">{{ t('admissionsQuotaRegion.listFunction') }}</div>
                </div>
            </div>

            <!-- Bảng -->
            <div class="px-4 pb-4">
                <BaseTable :columns="columns" :items="filteredQuotasAll" :loading="formLoading" :showPagination="true"
                    :page="page" :total="filteredQuotasAll.length" :pageSize="pageSize" :filter="filter"
                    @update:filter="onTableFilter" @update:rows="val => selectedRows = val" @view="viewRegion"
                    :showActionsColumn="true" :showEdit="false" :showDelete="false" :showView="true"
                    @update:page="val => page = val" @update:pageSize="onPageSizeChange">
                    <template #cell-quotaStage="{ item }">
                        <BaseBadge :label="t(statusKeyMap[item.quotaStage] ?? 'common.unknown')"
                            :color="statusColorMap[item.quotaStage] ?? 'gray'" bordered bold />
                    </template>
                    <!-- Tổng chỉ tiêu (link xanh) -->
                    <template #cell-totalRevenue="{ item }">
                        <el-link type="primary">{{ formatCurrency(item.totalRevenue) }}</el-link>
                    </template>

                    <!-- Thực hiện (nếu null hiển thị dấu '-') -->
                    <template #cell-actual="{ item }">
                        <span :class="item.actual ? 'text-primary fw-600' : 'text-muted'">{{ formatCurrency(0)
                        }}</span>
                    </template>

                    <!-- Sale phân bổ cho vùng: 2 dòng -->
                    <template #cell-currentSales="{ item }">
                        <div class="d-flex flex-column">
                            <span class="fw-semibold">
                                {{ Number(item.currentSales || 0) }} {{ t('admissionsQuota.salesUnit') }}
                            </span>
                            <span v-if="salesDiff(item) !== 0" class="fs-8"
                                :class="salesDiff(item) > 0 ? 'text-success' : 'text-danger'">
                                ({{ salesDiff(item) > 0 ? '+' : '−' }}{{ Math.abs(salesDiff(item)) }}
                                {{ t(salesDiff(item) > 0 ? 'admissionsQuota.newSales' : 'admissionsQuota.reducedSales')
                                }})
                            </span>
                        </div>
                    </template>

                    <!-- Tổng sale của vùng (đậm) -->
                    <template #cell-numberOfSalesAllocated="{ item }">
                        <div class="fw-600">{{ item.numberOfSalesAllocated }} {{ t('admissionsQuota.salesUnit') }}</div>
                    </template>

                    <!-- Doanh thu / sale -->
                    <template #cell-revenuePerSale="{ item }">
                        <div>{{ formatCurrency(item.revenuePerSale) }}</div>
                    </template>
                </BaseTable>
            </div>
        </el-card>
    </div>
    <AdmissionsQuotaRegionDialog :visible="showRegionDialog" mode="view" :region-id="selectedRegionId"
        @update:visible="v => showRegionDialog = v" />
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useI18n } from 'vue-i18n';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import { useAdmissionsQuotaRegionStore } from '@/stores/admissionsQuotaRegionStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { formatCurrency, formatDate } from '@/utils/format';
import { QuotaStatus } from '@/types';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import AdmissionsQuotaRegionDialog from './AdmissionsQuotaRegionDialog.vue';
// ===== i18n =====
const { t } = useI18n();

// ===== store & loading =====
const admissionsQuotaRegionStore = useAdmissionsQuotaRegionStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);

// ===== refs =====
const filterComponentRef = ref();
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRows = ref([]);
const showRegionDialog = ref(false)
const selectedRegionId = ref<string | null>(null)
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
const filterOptions = [
    { label: 'common.all', value: '' },
    { label: 'admissionsQuota.Allocated', value: QuotaStatus.Allocated },
    { label: 'admissionsQuota.InProgress', value: QuotaStatus.InProgress },
    { label: 'admissionsQuota.Completed', value: QuotaStatus.Completed },
];

// ===== columns =====
const columns: BaseTableColumn[] = [
    { key: 'monthYear', labelKey: 'admissionsQuota.monthYear', width: 180, filterType: 'text', sortable: true, align: 'center' },
    { key: 'totalRevenue', labelKey: 'admissionsQuotaRegion.totalTarget', width: 180, align: 'right' },
    { key: 'actual', labelKey: 'admissionsQuotaRegion.actual', width: 180, align: 'right' },
    { key: 'companyCount', labelKey: 'admissionsQuotaRegion.branchesCount', width: 130, align: 'center' },
    { key: 'currentSales', labelKey: 'admissionsQuotaRegion.salesAllocatedForRegion', width: 200, align: 'center' },
    { key: 'numberOfSalesAllocated', labelKey: 'admissionsQuotaRegion.totalSalesInRegion', width: 160, align: 'center' },
    { key: 'revenuePerSale', labelKey: 'admissionsQuotaRegion.revenuePerSale', width: 160, align: 'right' },
    { key: 'quotaStage', labelKey: 'common.status', width: 200, filterType: 'select', filterOptions, sortable: true },
    { key: 'createdBy', labelKey: 'common.createdBy', width: 140 },
    { key: 'receivedAt', labelKey: 'admissionsQuotaRegion.receivedAt', width: 140 },
    { key: 'updatedAt', labelKey: 'common.updatedAt', width: 140, formatter: (value: string) => formatDate(value, 'DD/MM/YYYY'), align: 'center' },
    { key: 'actions', labelKey: 'common.actions', width: 100, align: 'right' },
];
const quotasNormalized = computed(() =>
    (admissionsQuotaRegionStore.regions || []).map((x: any) => ({
        id: x.id,
        monthYear: (x?.admissionsQuota?.month && x?.admissionsQuota?.year ? `${String(x.admissionsQuota.month).padStart(2, '0')}/${x.admissionsQuota.year}` : ''),
        totalRevenue: x.totalRevenue ?? 0,
        companyCount: x.companyCount ?? 0,
        currentSales: x.currentSales ?? 0,
        numberOfSalesAllocated: x.numberOfSalesAllocated ?? 0,
        revenuePerSale: x.revenuePerSale ?? 0,
        createdBy: x.createdBy ?? '—',
        createdAt: x.createdAt ?? '',
        quotaStage: x?.admissionsQuota?.quotaStage ?? 0,
        admissionsQuotaCompanies: x.admissionsQuotaCompanies || [],
        admissionsQuotaEmployees: x.admissionsQuotaEmployees || [],
        admissionsQuotaAdjustments: x.admissionsQuotaAdjustments || [],
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
// ===== watch =====
onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams({ listParams: [], listBtn: [], });
    await admissionsQuotaRegionStore.fetchAllRegions();
});
// ===== handlers =====
function onPageSizeChange(newSize: number) { pageSize.value = newSize; page.value = 1; }

const salesDiff = (row: any) =>
    Number(row.numberOfSalesAllocated || 0) - Number(row.currentSales || 0);

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function viewRegion(item: any) {
    selectedRegionId.value = item.id       // chính là AdmissionsQuotaRegionId
    showRegionDialog.value = true
}

</script>

<style scoped>
.card-header {
    border-bottom: 1px solid rgba(0, 0, 0, .06);
}

.fw-600 {
    font-weight: 600;
}

.text-12 {
    font-size: 12px;
}

.leading-tight {
    line-height: 1.2;
}

.w-44 {
    width: 176px;
}
</style>
