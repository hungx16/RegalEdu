<template>
    <BaseTable :columns="columns" :items="filteredRows" :loading="loading" :showPagination="true" :page="page"
        :pageSize="pageSize" :total="filteredRows.length" :showCheckboxColumn="false" :showActionsColumn="false"
        :showIndex="true" :filter="tableFilter" @update:filter="onFilterChange"
        @update:page="val => (page = val)" @update:pageSize="onPageSizeChange">
        <!-- Sale hiện tại + chênh lệch -->
        <template #cell-currentSales="{ item }">
            <div class="d-flex flex-column">
                <span class="fw-semibold">
                    {{ Number(item.currentSales || 0) }} {{ t('admissionsQuota.salesUnit') }}
                </span>
                <span v-if="salesDiff(item) !== 0" class="fs-8"
                    :class="salesDiff(item) > 0 ? 'text-success' : 'text-danger'">
                    ({{ salesDiff(item) > 0 ? '+' : '−' }}{{ Math.abs(salesDiff(item)) }}
                    {{ t(salesDiff(item) > 0 ? 'admissionsQuota.newSales' : 'admissionsQuota.reducedSales') }})
                </span>
            </div>
        </template>

        <!-- Doanh thu/Sale -->
        <template #cell-revenuePerSale="{ item }">
            <span>{{ formatCurrency(item.revenuePerSale) }}</span>
        </template>

        <!-- Tổng doanh thu (tính động) -->
        <template #cell-totalRevenue="{ item }">
            <span>{{ formatCurrency(totalRevenueOf(item)) }}</span>
        </template>
        <!-- Ngày tham gia -->
        <template #cell-joinDate="{ item }">
            <p class="m-0"
                v-if="item.joinedAt !== null && item.joinedAt != undefined && isWithinMonth(item.joinedAt, props.year ?? 0, props.month ?? 0)">
                {{ t('admissionsQuota.probationStart') }}: {{ formatDate(item.joinedAt, 'DD-MM-YYYY') }}</p>
            <p class="m-0" style="color: red;"
                v-if="getEndDate(item) !== undefined && getEndDate(item) !== null && isWithinMonth(getEndDate(item), props.year ?? 0, props.month ?? 0)">
                {{ t('admissionsQuota.endDate') }}: {{ formatDate(getEndDate(item), 'DD-MM-YYYY') }}
            </p>
            <p class="m-0"
                v-if="item.probationEnd !== null && item.probationEnd !== undefined && props.month !== undefined && isWithinMonth(item.probationEnd, props.year ?? 0, props.month ?? 0)">
                {{ t('admissionsQuota.probationEnd') }}: {{ formatDate(item.probationEnd, 'DD-MM-YYYY') }}
            </p>
            <!-- Thêm dòng “Nhân viên làm bình thường” -->
            <p class="m-0 text-muted" v-if="!isWithinMonth(item.joinedAt, props.year ?? 0, props.month ?? 0)
                && !isWithinMonth(getEndDate(item), props.year ?? 0, props.month ?? 0)
                && !isWithinMonth(item.probationEnd, props.year ?? 0, props.month ?? 0)">
                {{ t('admissionsQuota.normalEmployee') }}
            </p>
        </template>
        <template #cell-seniority="{ item }">
            <span>{{ calculateSeniority(item.joinedAt) }}</span>
        </template>
        <template #cell-note="{ item }">
            <el-input v-model="item.note" :disabled="disabled" @change="emitRows" />
        </template>
    </BaseTable>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import { formatCurrency, formatDate } from '@/utils/format';
import { isWithinMonth, toDateLocal } from '@/utils/dateUtils';
import { QuotaRole } from '@/types';

interface Props {
    quotaId?: string | number;
    month?: number;
    year?: number;
    disabled?: boolean;
    rows?: any[];          // dữ liệu nhân viên parent truyền vào
    pageSizeDefault?: number; // tuỳ chọn: kích thước trang mặc định
}

const props = withDefaults(defineProps<Props>(), {
    disabled: false,
    rows: () => [],
    pageSizeDefault: 30,
});
const emit = defineEmits(['update:rows']);
const { t } = useI18n();

const loading = ref(false);
const localRows = ref<any[]>([]);
const tableFilter = ref<Record<string, any>>({});

// Pagination state
const page = ref(1);
const pageSize = ref(props.pageSizeDefault);

const regionOptions = computed(() => {
    const values = new Set<string>();
    for (const row of localRows.value) {
        const name = String(row?.regionName ?? '').trim();
        if (name) values.add(name);
    }
    return Array.from(values)
        .sort((a, b) => a.localeCompare(b, 'vi'))
        .map(value => ({ label: value, value }));
});
const companyOptions = computed(() => {
    const values = new Set<string>();
    for (const row of localRows.value) {
        const name = String(row?.companyName ?? '').trim();
        if (name) values.add(name);
    }
    return Array.from(values)
        .sort((a, b) => a.localeCompare(b, 'vi'))
        .map(value => ({ label: value, value }));
});

// Columns
const columns = computed<BaseTableColumn[]>(() => [
    { key: 'employeeCode', labelKey: 'applicationUser.userCode', width: 200, sticky: true, filterType: 'text' },
    { key: 'fullName', labelKey: 'employee.name', width: 200, sticky: true, filterType: 'text' },
    { key: 'positionName', labelKey: 'employee.position', align: 'left', width: 180, filterType: 'text' },
    { key: 'regionName', labelKey: 'region.name', align: 'center', width: 140, filterType: 'select', filterOptions: regionOptions.value },
    { key: 'companyName', labelKey: 'company.name', width: 200, filterType: 'select', filterOptions: companyOptions.value },
    { key: 'allocationStartAt', labelKey: 'common.startDate', width: 140, formatter: (_: any, row: any) => formatDate(row?.allocationStartAt || row?.startAt || row?.start, 'DD/MM/YYYY') },
    { key: 'allocationEndAt', labelKey: 'common.endDate', width: 140, formatter: (_: any, row: any) => formatDate(row?.allocationEndAt || row?.endAt || row?.end, 'DD/MM/YYYY') },
    { key: 'revenuePerSale', labelKey: 'admissionsQuota.revenuePerSale', align: 'right', width: 180 },
    { key: 'joinDate', labelKey: 'admissionsQuota.employeeStatus', align: 'right' },
    { key: 'joinedAt', labelKey: 'admissionsQuota.joinDate', align: 'right', formatter: (v: string) => formatDate(v, 'DD/MM/YYYY'), width: 140 },
    { key: 'seniority', labelKey: 'admissionsQuota.seniority', align: 'right' },
]);

// Nhận dữ liệu từ parent
watch(
    () => props.rows,
    (val) => {
        if (Array.isArray(val)) {
            localRows.value = val;
            page.value = 1; // reset trang khi dữ liệu đổi
        }
    },
    { immediate: true }
);

// Nếu sau này có filter, thay vì localRows dùng computed filteredRows
const filteredRows = computed(() => {
    const q = tableFilter.value || {};
    return localRows.value.filter(row => {
        const matchText = (field: string, value?: string) => {
            if (!value) return true;
            const source = String((row as any)[field] ?? '').toLowerCase();
            return source.includes(String(value).toLowerCase());
        };
        const matchSelect = (field: string, value?: string) => {
            if (!value) return true;
            return String((row as any)[field] ?? '') === String(value);
        };
        return (
            matchText('employeeCode', q.employeeCode) &&
            matchText('fullName', q.fullName) &&
            matchText('positionName', q.positionName) &&
            matchSelect('regionName', q.regionName) &&
            matchSelect('companyName', q.companyName)
        );
    });
});

// Emits
function emitRows() {
    emit('update:rows', localRows.value);
}

function onFilterChange(val: Record<string, any>) {
    tableFilter.value = { ...val };
}

// Helpers
const salesDiff = (row: any) =>
    Number(row.numberOfSales || 0) - Number(row.currentSales || 0);

const totalRevenueOf = (row: any) =>
    Number(row.numberOfSales || 0) * Number(row.revenuePerSale || 0);

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}
function getEndDate(item: any) {
    if (item?.endAt !== undefined && item?.endAt !== null && item?.endAt !== '') {
        return item.endAt;
    }
    if (item?.quotaRole === QuotaRole.LeavingEmployee) {
        return item?.allocationEndAt ?? null;
    }
    return null;
}
function calculateSeniority(joinedAt: any): string {
    const start = toDateLocal(joinedAt);
    const now = new Date();
    const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());

    if (!start) return t('common.none');
    if (today < start) {
        return t('admissionsQuota.seniorityFormat', { months: 0, days: 0 });
    }

    const yearDiff = today.getFullYear() - start.getFullYear();
    const monthDiff = today.getMonth() - start.getMonth() + yearDiff * 12;
    const lastDayOfCurrentMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0).getDate();
    const effectiveStartDay = Math.min(start.getDate(), lastDayOfCurrentMonth);

    let months = monthDiff;
    if (today.getDate() < effectiveStartDay) {
        months -= 1;
    }

    const addMonthsClamped = (base: Date, add: number) => {
        const y = base.getFullYear();
        const m = base.getMonth() + add;
        const d = base.getDate();
        const end = new Date(y, m + 1, 0);
        const day = Math.min(d, end.getDate());
        return new Date(end.getFullYear(), end.getMonth(), day);
    };
    const totalMonths = Math.max(0, months);
    const anchor = addMonthsClamped(start, totalMonths);
    const dayDiff = Math.max(0, Math.floor((today.getTime() - anchor.getTime()) / (1000 * 60 * 60 * 24)));
    const years = Math.floor(totalMonths / 12);
    const remainingMonths = totalMonths % 12;

    return t('admissionsQuota.seniorityFormat', { years, months: remainingMonths, days: dayDiff });
}

</script>
