<template>
    <BaseTable :columns="columns" :items="rows" :loading="loading" :showPagination="false" :showCheckboxColumn="false"
        :showIndex="true" @edit="openEdit" :showActionsColumn="isInProgress" :showEdit="isInProgress && props.showEdit"
        :height="400" :disable-row-dbl-click="true" @update:rows="onSelect">
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

        <template #cell-numberOfSalesAllocated="{ item }">
            <el-input v-model="item.numberOfSalesAllocated" :disabled="disabled" inputmode="numeric"
                @change="() => emitRows()" />
        </template>
        <template #cell-numberOfPartTimeSales="{ item }">
            <el-input v-model="item.numberOfPartTimeSales" :disabled="disabled" inputmode="numeric"
                @change="() => emitRows()" />
        </template>
        <template #cell-revenuePerSale="{ item }">
            <CurrencyInput v-model="item.revenuePerSale" :disabled="disabled" locale="vi-VN" currency="VND"
                @change="() => emitRows()" />
        </template>
        <template #cell-totalRevenue="{ item }">
            <span class="fw-semibold">
                {{ formatCurrency(getCompanyTotalRevenue(item)) }}
            </span>
            <!-- Hiện icon nếu có lịch sử điều chỉnh -->
            <el-tooltip v-if="(item.admissionsQuotaAdjustments?.length ?? 0) > 0"
                :content="t('admissionsQuota.historyAdjustments')" placement="bottom" :hide-after="0">
                <div class="div-history">
                    <el-button link @click="openAdjHistory(item)" size="small" class="el-button">
                        <el-icon class="icon-refresh">
                            <RefreshRight />
                        </el-icon>
                    </el-button>
                </div>
            </el-tooltip>
        </template>
        <template #cell-actualRevenue="{ item }">
            <span class="fw-semibold">
                {{ formatCurrency(getCompanyActualRevenue(item)) }}
            </span>
        </template>
        <template #actions="{ item }" v-if="props.showSupport">
            <el-tooltip :content="t('admissionsQuota.assignSupportTitle')" placement="top">
                <el-button circle size="small" class="btn-no-background" @click="openSupport(item)"
                    :disabled="!isInProgress">
                    <el-icon class="icon-refresh">
                        <User />
                    </el-icon>
                </el-button>
            </el-tooltip>
        </template>
    </BaseTable>
    <!-- Dialog lịch sử điều chỉnh -->
    <QuotaAdjustmentHistoryDialog :visible="adjDlg.visible" :loading="formLoading" width="80%"
        :title="t('admissionsQuota.adjustmentsForCompany')" :target-name="adjDlg.targetName" :adjustments="adjDlg.items"
        @update:visible="v => (adjDlg.visible = v)" />
    <QuotaAdjustmentDialog :visible="dlg.visible" :loading="formLoading" mode="create" :scope="AdjustmentScope.Company"
        :target-id="dlg.targetId" :target-name="dlg.targetName" :admissions-quota-company-id="dlg.aqcId"
        :total-quota-before="dlg.totalBefore" @update:visible="v => (dlg.visible = v)" @submit="onSaveAdjustment" />


    <QuotaSupportStaffDialog :visible="supportDlg.visible" :loading="formLoading" :target-name="supportDlg.targetName"
        :company-id="supportDlg.companyId" :quota-id="props.quotaId" :region-id="supportDlg.regionId"
        :admissions-quota-company-id="supportDlg.admissionsQuotaCompanyId"
        :admissions-quota-region-id="supportDlg.admissionsQuotaRegionId"
        :admissions-quota-id="supportDlg.admissionsQuotaId" @update:visible="v => (supportDlg.visible = v)"
        @submit="onSaveSupport" />
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import { useCompanyStore } from '@/stores/companyStore';
import CurrencyInput from '../currency-input/CurrencyInput.vue';
import { formatCurrency } from '@/utils/format';
import { StatusType, QuotaStatus, AdjustmentScope, PaymentStatus } from '@/types';
import QuotaAdjustmentDialog from '@/components/admissions-quota-table/QuotaAdjustmentDialog.vue';
import { isActiveInMonth, lastDayOfMonth, toDate } from '@/utils/dateUtils';
import { useAdmissionsQuotaStore } from '@/stores/admissionsQuotaStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { useNotificationStore } from '@/stores/notificationStore';
import QuotaAdjustmentHistoryDialog from '@/components/admissions-quota-table/QuotaAdjustmentHistoryDialog.vue'
import { RefreshRight } from '@element-plus/icons-vue'
import { User } from '@element-plus/icons-vue'
import QuotaSupportStaffDialog from './QuotaSupportStaffDialog.vue';
import { useReceiptStore } from '@/stores/receiptStore';
const supportDlg = ref({
    visible: false,
    companyId: '' as string,
    targetName: '' as string,
    admissionsQuotaCompanyId: '' as string,
    admissionsQuotaRegionId: '' as string,
    admissionsQuotaId: '' as string,
    regionId: '' as string
})

function openSupport(item: any) {
    supportDlg.value.companyId = item.companyId || item.id || '';
    supportDlg.value.targetName = item.companyName || '';
    supportDlg.value.admissionsQuotaCompanyId = item.id || item.admissionsQuotaCompanyId || '';
    supportDlg.value.admissionsQuotaId = item.admissionsQuotaId || '';
    supportDlg.value.regionId = item.regionId || '';
    supportDlg.value.admissionsQuotaRegionId = item.id || '';
    supportDlg.value.visible = true;
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
}
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
interface Props { quotaId?: string | number; month?: number; year?: number; disabled?: boolean, rows?: any[]; quotaStage?: number, showEdit?: boolean, showSupport?: boolean }
const props = withDefaults(defineProps<Props>(), { disabled: false });
const emit = defineEmits(['update:rows', 'adjusted', 'request-close']);
const { t } = useI18n();
const isInProgress = computed(() => props.quotaStage === QuotaStatus.InProgress)

const companyStore = useCompanyStore();
const rows = ref<any[]>([]);
const loading = ref(false);
const admissionsQuotaStore = useAdmissionsQuotaStore();
const notificationStore = useNotificationStore();
const receiptStore = useReceiptStore();

const columns: BaseTableColumn[] = [
    { key: 'companyName', labelKey: 'company.name', width: 240, sticky: true },
    { key: 'regionName', labelKey: 'region.name', align: 'center' },
    { key: 'currentSales', labelKey: 'admissionsQuota.currentSales', align: 'center', width: 140 },
    { key: 'numberOfSalesAllocated', labelKey: 'admissionsQuota.numberOfSalesAllocated', align: 'center', width: 160 },
    { key: 'numberOfPartTimeSales', labelKey: 'admissionsQuota.numberOfPartTimeSales', align: 'center', width: 160 },
    { key: 'revenuePerSale', labelKey: 'admissionsQuota.revenuePerSale', align: 'right', width: 240 },
    { key: 'totalRevenue', labelKey: 'admissionsQuota.totalRevenue', align: 'left', width: 160 },
    { key: 'actualRevenue', labelKey: 'admissionsQuota.actualRevenue', align: 'right', width: 180 },
    { key: 'actions', labelKey: 'common.actions', align: 'center', width: 140 }
];
const receiptRevenueByCompany = computed(() => {
    const map = new Map<string, number>();
    const targetMonth = Number(props.month);
    const targetYear = Number(props.year);
    if (!targetMonth || !targetYear) return map;

    for (const receipt of receiptStore.receipts) {
        if (![PaymentStatus.Paid, PaymentStatus.PartiallyPaid].includes(receipt.status ?? -1)) continue;
        const created = receipt.createdAt ? new Date(receipt.createdAt) : null;
        if (!created) continue;
        if (created.getMonth() + 1 !== targetMonth || created.getFullYear() !== targetYear) continue;
        const key = receipt.employee?.companyId;
        if (!key) continue;
        map.set(key, (map.get(key) ?? 0) + (receipt.totalAmount ?? 0));
    }
    return map;
});
const dlg = ref({
    visible: false,
    loading: false,
    scope: AdjustmentScope.Company,
    targetId: '' as string,
    targetName: '' as string,
    aqcId: null as string | null,
    totalBefore: 0,
    companyId: '' as string
});
onMounted(() => {
    init();
    if (!receiptStore.receipts.length) {
        receiptStore.fetchAllReceipts();
    }
});
watch(() => [props.quotaId, props.month, props.year], init);

watch(
    () => props.rows,
    (val) => {
        if (!Array.isArray(val)) return;
        rows.value = (val || []).map((x: any) => ({
            companyId: x.companyId || x.id,
            id: x.id ?? null,
            admissionsQuotaCompanyId: x.id ?? null,
            companyName: x.companyName,
            regionName: x.regionName,
            regionId: x.regionId ?? null,
            currentSales: x.currentSales ?? 0,
            numberOfSalesAllocated: x.numberOfSalesAllocated ?? 0,
            numberOfPartTimeSales: x.numberOfPartTimeSales ?? 0,
            revenuePerSale: x.revenuePerSale ?? 0,
            totalRevenue: x.totalRevenue ?? (x.numberOfSalesAllocated ?? 0) * (x.revenuePerSale ?? 0),
            admissionsQuotaAdjustments: x.admissionsQuotaAdjustments ?? [],
            admissionsQuotaId: props.quotaId,
            month: props.month,
            year: props.year,
        }))
        //emitRows()
    },
    { deep: true, immediate: true } // immediate để lần đầu cũng ăn data từ parent
)
async function init() {
    loading.value = true;
    try {
        const y = Number(props.year);
        const m = Number(props.month);
        const monthEnd = lastDayOfMonth(y, m);

        // chọn mapping region active mới nhất (nếu có)
        const pickActiveRegionMap = (c: any) => {
            const maps = (c.logRegionComs || []).filter((t: any) => t.status === 0);
            if (!maps.length) return null;
            maps.sort(
                (a: any, b: any) =>
                    new Date(b.createdAt ?? 0).getTime() - new Date(a.createdAt ?? 0).getTime()
            );
            return maps[0];
        };

        if (!props.quotaId) {
            // CREATE: nạp danh sách chi nhánh theo điều kiện
            await companyStore.fetchAllCompanies();

            const companies = (companyStore.companies || []).filter((c: any) => {
                if ((c.status ?? 0) !== StatusType.Active) return false;
                const est = toDate(c.establishmentDate);
                // nếu có ngày thành lập thì phải <= cuối tháng phân bổ
                return !est || est <= monthEnd;
            });

            rows.value = companies.map((c: any) => {
                const map = pickActiveRegionMap(c);

                const currentSales =
                    (c.employees || []).filter((emp: any) =>
                        (emp.status ?? 0) === StatusType.Active &&
                        (emp?.position?.isSale === true || emp?.position?.isSaleLead === true) &&
                        isActiveInMonth(y, m, emp.employeeStartedDate, emp.employeeEndDate)
                    ).length;

                return {
                    id: undefined,
                    companyId: c.id,
                    companyName: c.companyName || c.name,
                    regionId: map?.region?.id ?? null,
                    regionName: map?.region?.regionName ?? '',
                    currentSales,
                    numberOfSalesAllocated: 3,     // user nhập
                    numberOfPartTimeSales: 0,      // user nhập
                    revenuePerSale: 100000000,      // user nhập
                    admissionsQuotaId: undefined,
                    month: props.month,
                    year: props.year,
                };
            });

            // sắp xếp: RegionName -> CompanyName (tiếng Việt)
            const collator = new Intl.Collator('vi', { sensitivity: 'base', numeric: true });
            rows.value.sort((a: any, b: any) => {
                const byRegion = collator.compare(a.regionName || '', b.regionName || '');
                if (byRegion !== 0) return byRegion;
                return collator.compare(a.companyName || '', b.companyName || '');
            });
        } else {
            // EDIT/VIEW: dùng rows từ props (giữ nguyên dữ liệu đã lưu)
            rows.value = (props.rows || []).map((x: any) => ({
                companyId: x.companyId || x.id,
                id: x.id ?? null,
                admissionsQuotaCompanyId: x.id ?? null,
                companyName: x.companyName,
                regionName: x.regionName,
                regionId: x.regionId ?? null,
                currentSales: x.currentSales ?? 0,
                numberOfSalesAllocated: x.numberOfSalesAllocated ?? 0,
                numberOfPartTimeSales: x.numberOfPartTimeSales ?? 0,
                revenuePerSale: x.revenuePerSale ?? 0,
                totalRevenue: x.totalRevenue ?? (x.numberOfSalesAllocated ?? 0) * (x.revenuePerSale ?? 0),
                admissionsQuotaAdjustments: x.admissionsQuotaAdjustments ?? [],
                admissionsQuotaId: props.quotaId,
                month: props.month,
                year: props.year,
            }));
        }

        emitRows(!props.quotaId);
    } finally {
        loading.value = false;
    }
}
function openEdit(item: any) {
    // BaseTable emit 'edit' -> mở dialog điều chỉnh

    dlg.value.visible = true
    dlg.value.scope = AdjustmentScope.Company
    dlg.value.targetId = item.companyId
    dlg.value.targetName = item.companyName
    dlg.value.companyId = item.companyId
    // AQC id chỉ có ở EDIT/VIEW
    dlg.value.aqcId = item.id ?? item.admissionsQuotaCompanyId ?? null
    dlg.value.totalBefore = getCompanyTotalRevenue(item)
}

async function onSaveAdjustment(payload: any) {
    startLoading()
    try {
        await admissionsQuotaStore.addAdmissionsQuotaAdjustment(payload);
        notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.division') } });
        await admissionsQuotaStore.fetchAllAdmissionsQuotas();
        // // 1) PATCH local row theo AQC id (company trong quota)
        // const idx = rows.value.findIndex(r =>
        //     r.id === payload.admissionsQuotaCompanyId ||
        //     r.admissionsQuotaCompanyId === payload.admissionsQuotaCompanyId
        // );
        // if (idx > -1) {
        //     rows.value[idx].totalRevenue = Number(payload.totalQuotaAfter ?? 0);
        // }
        // 2) phát ra để parent cập nhật (region, summary…)
        // notify parent about the adjustment so it can update totals / show recalc
        emit('update:rows', rows.value);
        emit('adjusted', { scope: AdjustmentScope.Company, aqcId: payload.admissionsQuotaCompanyId, totalAfter: payload.totalQuotaAfter });
        emit('request-close');
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
        stopLoading()
    } finally {
        stopLoading() // Hoặc chỉ cần đóng dialog, watcher sẽ reset loading về false
        dlg.value.visible = false
    }
}
async function onSaveSupport(payload: any) {
    try {
        startLoading()
        await admissionsQuotaStore.assignSupportStaff(payload)
        notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('common.employee') } })
        emit('request-close')
    } catch (e: any) {
        console.error('Error saving:', e?.response?.data?.errors || e);
    } finally {
        stopLoading()
        supportDlg.value.visible = false

    }
}
function emitRows(recalculateTotals = true) {
    if (recalculateTotals) {
        syncRowTotals();
    }
    emit('update:rows', rows.value, recalculateTotals);
}

function syncRowTotals() {
    rows.value = rows.value.map(row => ({
        ...row,
        totalRevenue: computeRowTotal(row),
    }));
}

function computeRowTotal(row: any) {
    const sales = Number(row.numberOfSalesAllocated ?? row.numberOfSales ?? 0);
    const revenue = Number(row.revenuePerSale ?? 0);
    return sales * revenue;
}
function getLatestAdjustmentTotal(row: any): number | null {
    const list = Array.isArray(row?.admissionsQuotaAdjustments) ? row.admissionsQuotaAdjustments : [];
    if (!list.length) return null;
    const latest = list
        .slice()
        .sort((a: any, b: any) =>
            new Date(b?.createdAt ?? 0).getTime() - new Date(a?.createdAt ?? 0).getTime()
        )[0];
    const value = latest?.totalQuotaAfter;
    if (value === null || value === undefined || value === '') return null;
    const n = Number(value);
    return Number.isNaN(n) ? null : n;
}
function getCompanyTotalRevenue(row: any) {
    const adjusted = getLatestAdjustmentTotal(row);
    if (adjusted !== null) return adjusted;
    if (row?.totalRevenue !== null && row?.totalRevenue !== undefined && row?.totalRevenue !== '') {
        const n = Number(row.totalRevenue);
        if (!Number.isNaN(n)) return n;
    }
    return computeRowTotal(row);
}

function getCompanyActualRevenue(row: any) {
    const key = row.companyId ?? '';
    return receiptRevenueByCompany.value.get(key) ?? 0;
}

function onSelect() { /* reserved */ }

const salesDiff = (row: any) =>
    Number(row.numberOfSalesAllocated || 0) - Number(row.currentSales || 0);

</script>
<style scoped>
.icon-refresh {
    color: var(--el-color-primary);
    font-size: 1.2em;
}
</style>
