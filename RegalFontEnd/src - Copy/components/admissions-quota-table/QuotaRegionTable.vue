<template>
    <BaseTable :columns="columns" :items="localRows" :loading="loading" :showPagination="false" :height="300"
        @edit="openEdit" :showActionsColumn="showActionsColumn" :showEdit="showEdit" :showCheckboxColumn="false"
        :showIndex="true"
        :disable-row-dbl-click="true" @update:rows="() => { }">
        <template #cell-currentSales="{ item }">
            <div class="d-flex flex-column">
                <span class="fw-semibold">
                    {{ Number(item.currentSales || 0) }} {{ t('admissionsQuota.salesUnit') }}
                </span>
                <span v-if="salesDiff(item) !== 0" class="fs-8"
                    :class="salesDiff(item) > 0 ? 'text-success' : 'text-danger'">
                    ({{ salesDiff(item) > 0 ? '+' : '-' }}{{ Math.abs(salesDiff(item)) }}
                    {{ t(salesDiff(item) > 0 ? 'admissionsQuota.newSales' : 'admissionsQuota.reducedSales') }})
                </span>
            </div>
        </template>

        <template #cell-revenuePerSale="{ item }">
            <span>{{ formatCurrency(item.revenuePerSale) }}</span>
        </template>
        <template #cell-totalRevenue="{ item }">
            <span class="fw-semibold">
                {{ formatCurrency(
                    item.totalRevenue ?? ((item.numberOfSalesAllocated || 0) * (item.revenuePerSale || 0))
                ) }}
            </span>
            <!-- Hiện icon nếu có lịch sử điều chỉnh -->
            <el-tooltip v-if="(item.admissionsQuotaAdjustments?.length ?? 0) > 0"
                :content="t('admissionsQuota.historyAdjustments')" placement="bottom" :hide-after="0">
                <div class="div-history">
                    <el-button link @click="openAdjHistory(item)" size="small">
                        <el-icon class="icon-refresh">
                            <RefreshRight />
                        </el-icon>
                    </el-button>
                </div>
            </el-tooltip>
        </template>

        <template #cell-actualRevenue="{ item }">
            <span class="fw-semibold">
                {{ formatCurrency(getRegionActualRevenue(item)) }}
            </span>
        </template>

    </BaseTable>
    <!-- Dialog lịch sử điều chỉnh -->
    <QuotaAdjustmentHistoryDialog :visible="adjDlg.visible" :loading="formLoading"
        :title="t('admissionsQuota.adjustmentsForRegion')" :target-name="adjDlg.targetName" :adjustments="adjDlg.items"
        @update:visible="v => (adjDlg.visible = v)" />
    <!-- Company -->
    <!-- Dialog điều chỉnh (đặt 1 lần dưới bảng) -->
    <QuotaAdjustmentDialog :visible="dlg.visible" :loading="formLoading" mode="create" :scope="AdjustmentScope.Region"
        :target-id="dlg.targetId" :target-name="dlg.targetName" :admissions-quota-region-id="dlg.aqcId"
        :total-quota-before="dlg.totalBefore" @update:visible="v => (dlg.visible = v)" @submit="onSaveAdjustment" />


</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import { formatCurrency } from '@/utils/format';
import QuotaAdjustmentDialog from '@/components/admissions-quota-table/QuotaAdjustmentDialog.vue';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { useNotificationStore } from '@/stores/notificationStore';
import QuotaAdjustmentHistoryDialog from '@/components/admissions-quota-table/QuotaAdjustmentHistoryDialog.vue'
import { RefreshRight } from '@element-plus/icons-vue'
import { useAdmissionsQuotaStore } from '@/stores/admissionsQuotaStore';
import { AdjustmentScope, PaymentStatus, QuotaStatus } from '@/types';
import { useReceiptStore } from '@/stores/receiptStore';

interface Props {
    quotaId?: string | number;
    month?: number;
    year?: number;
    disabled?: boolean;
    rows?: any[];
    quotaStage?: number;
    companyRows?: any[];
}

const adjDlg = ref<{ visible: boolean, targetName: string, items: any[] }>({
    visible: false,
    targetName: '',
    items: []
})
function openAdjHistory(item: any) {
    adjDlg.value.targetName = item.companyName
    adjDlg.value.items = Array.isArray(item.admissionsQuotaAdjustments) ? item.admissionsQuotaAdjustments : []
    adjDlg.value.visible = true
} const notificationStore = useNotificationStore();
const props = withDefaults(defineProps<Props>(), {
    disabled: false,
    rows: () => [],
    companyRows: () => []
});
const isInProgress = computed(() => props.quotaStage === QuotaStatus.InProgress)
const showEdit = computed(() => !props.disabled)
const showActionsColumn = computed(() => showEdit.value)
const admissionsQuotaStore = useAdmissionsQuotaStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
const dlg = ref({
    visible: false,
    loading: false,
    scope: AdjustmentScope.Region,
    targetId: '' as string,
    targetName: '' as string,
    aqcId: null as string | null,
    totalBefore: 0
});
const emit = defineEmits(['update:rows', 'request-close']);
const { t } = useI18n();

const loading = ref(false);
const localRows = ref<any[]>([]);
const receiptStore = useReceiptStore();
const receiptMonth = computed(() => Number(props.month));
const receiptYear = computed(() => Number(props.year));
const receiptRevenueByCompany = computed(() => {
    const map = new Map<string, number>();
    const targetMonth = receiptMonth.value;
    const targetYear = receiptYear.value;
    if (!targetMonth || !targetYear) return map;
    for (const receipt of receiptStore.receipts) {
        if (![PaymentStatus.Paid, PaymentStatus.PartiallyPaid].includes(receipt.status ?? -1)) continue;
        const created = receipt.createdAt ? new Date(receipt.createdAt) : null;
        if (!created) continue;
        if (created.getMonth() + 1 !== targetMonth || created.getFullYear() !== targetYear) continue;
        const companyId = receipt.employee?.companyId;
        if (!companyId) continue;
        map.set(companyId, (map.get(companyId) ?? 0) + (receipt.totalAmount ?? 0));
    }
    return map;
});
const regionActualRevenue = computed(() => {
    const map = new Map<string, number>();
    for (const company of props.companyRows || []) {
        const regionId = company.regionId ?? '';
        const companyId = company.companyId ?? company.id ?? '';
        if (!regionId || !companyId) continue;
        const actual = receiptRevenueByCompany.value.get(companyId) ?? 0;
        map.set(regionId, (map.get(regionId) ?? 0) + actual);
    }
    return map;
});

const columns: BaseTableColumn[] = [
    { key: 'regionName', labelKey: 'region.name', sticky: true },
    { key: 'companyCount', labelKey: 'admissionsQuota.companyCount', align: 'center', width: 160 },
    { key: 'currentSales', labelKey: 'admissionsQuota.currentSales', align: 'center', width: 200 },
    { key: 'numberOfSalesAllocated', labelKey: 'admissionsQuota.numberOfSalesAllocated', align: 'center', width: 160 },
    { key: 'revenuePerSale', labelKey: 'admissionsQuota.revenuePerSale', align: 'right', width: 240 },
    { key: 'totalRevenue', labelKey: 'admissionsQuota.totalRevenue', align: 'left', width: 160 },
    { key: 'actualRevenue', labelKey: 'admissionsQuota.actualRevenue', align: 'right', width: 180 },
    { key: 'actions', labelKey: 'common.actions', width: 140 },

];

watch(() => props.rows, (newVal) => {
    if (Array.isArray(newVal)) {
        localRows.value = newVal;
    }
}, { immediate: true });

onMounted(() => {
    if (!receiptStore.receipts.length) {
        receiptStore.fetchAllReceipts();
    }
});

function emitRows() {
    emit('update:rows', localRows.value);
}

function getRegionActualRevenue(row: any) {
    const regionId = row.regionId ?? '';
    return regionActualRevenue.value.get(regionId) ?? 0;
}
const salesDiff = (row: any) =>
    Number(row.numberOfSalesAllocated || 0) - Number(row.currentSales || 0);
async function onSaveAdjustment(payload: any) {
    startLoading()
    try {
        await admissionsQuotaStore.addAdmissionsQuotaAdjustment(payload);
        notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.division') } });
        await admissionsQuotaStore.fetchAllAdmissionsQuotas();
        emit('request-close');
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
        stopLoading()
    } finally {
        stopLoading() // Hoặc chỉ cần đóng dialog, watcher sẽ reset loading về false
        dlg.value.visible = false
    }
}
function openEdit(item: any) {
    // BaseTable emit 'edit' -> mở dialog điều chỉnh
    dlg.value.visible = true
    dlg.value.scope = AdjustmentScope.Region
    dlg.value.targetId = item.regionId
    dlg.value.targetName = item.regionName
    // AQC id chỉ có ở EDIT/VIEW
    dlg.value.aqcId = item.id ?? item.admissionsQuotaRegionId ?? null
    dlg.value.totalBefore = item.totalRevenue || 0
}
</script>
