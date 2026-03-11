<template>
    <BaseDialogForm :visible="visible" :title="modeTitle" :form-data="formData" :mode="dialogModeForForm"
        :submit-disabled="!canAct" :show-action-buttons="actionButtonsVisible" :width="computedDialogWidth"
        :loading="loading || internalLoading" :rules="rules" class="admissions-quota-dialog"
        @update:visible="$emit('update:visible', $event)" @submit="onSubmit" @close="onClose">
        <template #form>
            <!-- TOP FIELDS: Month/Year + Note -->
            <div class="row g-4 align-items-end">
                <div class="col-12 col-md-3">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('admissionsQuota.month') }}</label>
                    <el-form-item prop="month">
                        <el-select v-model="formData.month" :disabled="isView" filterable style="width:100%"
                            @change="handleAllocationChange">
                            <el-option v-for="m in 12" :key="m" :label="String(m).padStart(2, '0')" :value="m" />
                        </el-select>
                    </el-form-item>
                </div>
                <div class="col-12 col-md-3">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('admissionsQuota.year') }}</label>
                    <el-form-item prop="year">
                        <el-input v-model.number="formData.year" :disabled="isView" />
                    </el-form-item>
                </div>
                <div class="col-12 col-md-6">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.note') }}</label>
                    <el-form-item prop="note">
                        <el-input v-model="formData.note" :disabled="isView"
                            :placeholder="t('admissionsQuota.notePlaceholder')" />
                    </el-form-item>
                </div>
            </div>

            <!-- SUMMARY CARDS -->
            <div class="row g-4 mt-2 mb-6">
                <div class="col-12 col-xl-4">
                    <div class="summary-card p-6 rounded-4 shadow-sm bg-body h-100">
                        <div class="d-flex justify-content-between align-items-start mb-1">
                            <span class="fw-semibold fs-6">{{ t('admissionsQuota.totalAllocatedRevenue') }}</span>
                            <i class="bi bi-cash-coin fs-4 text-body-secondary"></i>
                        </div>
                        <div class="fs-2 fw-bold">{{ formatCurrency(totalRevenue) }}</div>
                        <div class="fs-8 text-body-secondary">{{ t('admissionsQuota.totalAllocatedRevenueHint') }}</div>
                    </div>
                </div>
                <div class="col-12 col-xl-4">
                    <div class="summary-card p-6 rounded-4 shadow-sm bg-body h-100">
                        <div class="d-flex justify-content-between align-items-start mb-1">
                            <span class="fw-semibold fs-6">{{ t('admissionsQuota.allocationInfo') }}</span>
                            <i class="bi bi-info-circle fs-4 text-body-secondary"></i>
                        </div>
                        <div class="d-flex gap-8 flex-wrap">
                            <div>
                                <div class="fs-3 fw-bold">{{ companyCount }}</div>
                                <div class="fs-8 text-body-secondary">{{ t('admissionsQuota.companyCount') }}</div>
                            </div>
                            <div>
                                <div class="fs-3 fw-bold">{{ totalSales }}</div>
                                <div class="fs-8 text-body-secondary">{{ t('admissionsQuota.salesTotal') }}</div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-12 col-xl-4">
                    <div class="summary-card p-6 rounded-4 shadow-sm bg-body h-100">
                        <div class="d-flex justify-content-between align-items-start mb-1">
                            <span class="fw-semibold fs-6">{{ t('admissionsQuota.actualRevenueSummary') }}</span>
                            <i class="bi bi-cash-stack fs-4 text-success"></i>
                        </div>
                        <div class="fs-2 fw-bold text-success">{{ formatCurrency(totalActualRevenue) }}</div>
                        <div class="fs-8 text-body-secondary">{{ t('admissionsQuota.actualRevenueHint') }}</div>
                    </div>
                </div>
            </div>
            <template v-if="showCompanyTable">
                <!-- COMPANY TABLE -->
                <div class="card mb-6">
                    <div class="card-header d-flex align-items-center justify-content-between">
                        <div class="card-title">
                            <h3 class="fs-5 fw-bold m-0">{{ t('admissionsQuota.tableCompany') }}</h3>
                        </div>
                        <div class="d-flex gap-2">
                            <el-button v-if="showRecalcBtn" @click="recalculate" :disabled="internalLoading"
                                :loading="internalLoading">
                                {{ t('admissionsQuota.recalculate') }}
                            </el-button>

                        </div>
                    </div>
                    <div class="card-body p-2">
                        <QuotaCompanyTable :key="`${formData.id ?? 'new'}-${formData.month}-${formData.year}`"
                            :quota-stage="formData.quotaStage" :quota-id="formData.id" :month="formData.month"
                            @request-close="closeAndReload" @adjusted="onCompanyAdjusted" :year="formData.year"
                            :show-edit="true" :show-support="false" :rows="companyRows" :disabled="false"
                            @update:rows="onCompanyRows" />

                    </div>
                </div>
            </template>


            <!-- REGION + EMPLOYEE TABLES: chỉ hiển thị sau khi tính toán -->
            <template v-if="showAllocations">
                <div class="card mb-6">
                    <div class="card-header d-flex align-items-center justify-content-between">
                        <div class="card-title">
                            <h3 class="fs-5 fw-bold m-0">{{ t('admissionsQuota.tableRegion') }}</h3>
                        </div>
                    </div>
                    <div class="card-body p-2">
                        <QuotaRegionTable :quota-id="formData.id" :month="formData.month" :year="formData.year"
                            :quota-stage="formData.quotaStage" @request-close="closeAndReload" :disabled="isView"
                            :rows="regionRows" :company-rows="companyRows" @update:rows="onRegionRows" />

                    </div>
                </div>

                <div class="card mb-2">
                    <div class="card-header d-flex align-items-center justify-content-between">
                        <div class="card-title">
                            <h3 class="fs-5 fw-bold m-0">{{ t('admissionsQuota.tableEmployee') }}</h3>
                        </div>
                    </div>
                    <div class="card-body p-2">
                        <QuotaEmployeeTable :quota-id="formData.id" :month="formData.month" :year="formData.year"
                            :rows="employeeRows" :disabled="isView" @update:rows="onEmployeeRows" />
                    </div>
                </div>
            </template>
        </template>
        <!-- Footer: Tạo phân bổ chỉ enable khi canAct -->
        <template #footer-extra="{ mode, loading }">
            <el-button class="btn btn-success" v-if="props.mode !== 'view' && hasCalculated"
                :disabled="loading || mode === 'view' || !hasCalculated" @click="handleApprove()">
                {{ t('admissionsQuota.createAllocation') }}
            </el-button>
        </template>

    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import QuotaCompanyTable from '@/components/admissions-quota-table/QuotaCompanyTable.vue';
import QuotaRegionTable from '@/components/admissions-quota-table/QuotaRegionTable.vue';
import QuotaEmployeeTable from '@/components/admissions-quota-table/QuotaEmployeeTable.vue';
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue';
import { formatCurrency } from '@/utils/format';
import { useCompanyStore } from '@/stores/companyStore';
import { useRegionStore } from '@/stores/regionStore';
import { useEmployeeStore } from '@/stores/employeeStore';
import { usePositionStore } from '@/stores/positionStore';
import { useAdmissionsQuotaStore } from '@/stores/admissionsQuotaStore';
import { addDays, countWorkingDaysBetween, countWorkingDaysInMonth, countWorkingDaysAccumulated, firstDayOfMonth, getEmployeeStatus, isWithinMonth, lastDayOfMonth, toDate, toDateLocal, isActiveInMonth } from '@/utils/dateUtils';
import { QuotaRole, QuotaStatus, PaymentStatus } from '@/types';
import type { AdmissionsQuotaModel } from '@/api/AdmissionsQuotaApi';
import { useNotificationStore } from '@/stores/notificationStore';
import { useReceiptStore } from '@/stores/receiptStore';
const companyStore = useCompanyStore();
const regionStore = useRegionStore();
const employeeStore = useEmployeeStore();
const positionStore = usePositionStore();
const admissionsQuotaStore = useAdmissionsQuotaStore();
const notificationStore = useNotificationStore();
const receiptStore = useReceiptStore();
interface Props {
    visible: boolean;
    mode: 'create' | 'edit' | 'view';
    loading?: boolean;
    quotaData?: any | null;
}


const windowWidth = ref(window.innerWidth);
const computedDialogWidth = computed(() => windowWidth.value < 768 ? '100%' : '80%');
const props = defineProps<Props>();
const emit = defineEmits(['update:visible', 'submit', 'close']);
const { t } = useI18n();

const internalLoading = ref(false);
const isView = computed(() => props.mode === 'view');
const modeTitle = computed(() => props.mode === 'edit' ? t('admissionsQuota.editTitle') : props.mode === 'view' ? t('admissionsQuota.viewTitle') : t('admissionsQuota.createTitle'));
const dialogModeForForm = computed(() => (props.mode === 'view' ? 'edit' : props.mode));
const hasCalculated = ref(false);
const showRecalcBtn = ref(false);
const actionButtonsVisible = computed(() => hasCalculated.value && !isView.value);
const canAct = computed(() => actionButtonsVisible.value);
const formData = ref<any>({ id: '', month: new Date().getMonth() + 2, year: new Date().getFullYear(), note: '' });
const rules = {
    month: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    year: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
};
const companiesSnapshot = ref<string>(''); // chữ ký trạng thái company ban đầu

const showAllocations = ref(false);
const checkAllocationExist = ref(true);
const showCompanyTable = computed(() => {
    if (props.mode === 'edit' || props.mode === 'view') return true;
    // ở create thì chỉ hiện khi chưa có phân bổ tháng/năm
    return checkAllocationExist.value === false;
});
const companyRows = ref<any[]>([]);
const regionRows = ref<any[]>([]);
const employeeRows = ref<any[]>([]);
watch(() => props.quotaData, (val) => {
    if (val) {
        formData.value = {
            id: val.id ?? '',
            month: val.month ?? (new Date().getMonth() + 1) % 12,
            year: val.year ?? new Date().getFullYear(),
            note: val.note ?? '',
            quotaStage: val.quotaStage ?? 0,
        };
        if (Array.isArray(val.admissionsQuotaRegions) && val.admissionsQuotaRegions.length) {
            hydrateFromQuota(val);
            showAllocations.value = true;
            if (props.mode === 'edit') {
                companiesSnapshot.value = signatureCompanies(companyRows.value);
            }
            hasCalculated.value = false;
            showRecalcBtn.value = hasCompanyData(companyRows.value);
        } else {
            showAllocations.value = false;
        }
    } else {
        formData.value = {
            year: new Date().getFullYear(),
            note: '',
            quotaStage: QuotaStatus.Draft,
        };
        showAllocations.value = false;    // t???o m??>i: ?`???i tA-nh toA?n
        hasCalculated.value = false;
        showRecalcBtn.value = false;
    }
}, { immediate: true });

onMounted(() => {
    if (!receiptStore.receipts.length) {
        receiptStore.fetchAllReceipts();
    }
});


const companyCount = computed(() => {
    const fromCompany = new Set(companyRows.value.map(r => r.companyId)).size;
    const fromRegion = regionRows.value.reduce((s, r) => s + Number(r.companyCount || 0), 0);
    return fromCompany || fromRegion || 0;
});

const totalSales = computed(() => {
    const sum = (arr: any[]) => arr.reduce((s, r) => s + Number(r.currentSales || 0), 0);
    const byCompany = sum(companyRows.value);
    const byRegion = sum(regionRows.value);
    return byCompany || byRegion || 0;
});

const totalRevenue = computed(() => {
    const byRegion = sum(regionRows.value);
    const byCompany = sum(companyRows.value);
    return byRegion || byCompany || 0; // ưu tiên region nếu có
});
const receiptMonth = computed(() => Number(formData.value.month));
const receiptYear = computed(() => Number(formData.value.year));
const receiptRevenueByCompany = computed(() => {
    const map = new Map<string, number>();
    const targetMonth = receiptMonth.value;
    const targetYear = receiptYear.value;
    if (!targetMonth || !targetYear) return map;

    for (const receipt of receiptStore.receipts) {
        if (![PaymentStatus.Paid, PaymentStatus.PartiallyPaid].includes(receipt.status ?? -1)) continue;
        const createdAt = receipt.createdAt ? new Date(receipt.createdAt) : null;
        if (!createdAt) continue;
        if (createdAt.getMonth() + 1 !== targetMonth || createdAt.getFullYear() !== targetYear) continue;
        const companyId = receipt.employee?.companyId;
        if (!companyId) continue;
        map.set(companyId, (map.get(companyId) ?? 0) + (receipt.totalAmount ?? 0));
    }

    return map;
});
const totalActualRevenue = computed(() =>
    Array.from(receiptRevenueByCompany.value.values()).reduce((sum, value) => sum + value, 0)
);
function sum(arr: any[]) {
    return (arr ?? []).reduce((s, r) => {
        const trRaw = r?.totalRevenue;
        const tr = trRaw === '' || trRaw === null || trRaw === undefined ? NaN : Number(trRaw);

        if (!Number.isNaN(tr)) {
            // Ưu tiên totalRevenue nếu là số hợp lệ (kể cả 0)
            return s + tr;
        }

        // Fallback: tính theo tích
        const n = Number(r?.numberOfSalesAllocated ?? r?.numberOfSales ?? 0);
        const rev = Number(r?.revenuePerSale ?? 0);
        return s + n * rev;
    }, 0);
}

async function recalculate() {
    internalLoading.value = true;
    try {
        // 1. Lấy thông tin vùng, chi nhánh, nhân viên (nếu chưa có)
        if (!regionStore.regions?.length) await regionStore.fetchAllRegions();
        if (!companyStore.companies?.length) await companyStore.fetchAllCompanies();
        if (!employeeStore.employees?.length) await employeeStore.fetchAllEmployees();
        if (!positionStore.positions?.length) await positionStore.fetchAllPositions();

        if (!companyStore.LogRegionComs?.length) { await companyStore.fetchAllCompanyRegions(); }
        const validMappings = companyStore.LogRegionComs.filter(x => x.status === 0 && companyRows.value.some(c => c.companyId === x.companyId));
        // 2. Gom nhóm chi nhánh theo vùng 
        const regionMap = new Map<string, { regionId: string, regionName: string, companyIds: string[] }>();
        validMappings.forEach(x => {
            const regionId = x.regionId ?? ''; const regionName = x.region?.regionName ?? '';
            if (!regionMap.has(regionId)) { regionMap.set(regionId, { regionId, regionName, companyIds: [] }); } regionMap.get(regionId)!.companyIds.push(x.companyId!);
        });
        // 3. Tính toán dữ liệu cho từng vùng 
        const existingRegionRowsById = new Map((regionRows.value || []).map((r: any) => [r.regionId, r]));
        const nextRegionRows = Array.from(regionMap.values()).map(region => {
            const relatedCompanies = companyRows.value.filter(c => region.companyIds.includes(c.companyId));
            const numberOfSalesAllocated = relatedCompanies.reduce((sum, c) => sum + (parseInt(c.numberOfSalesAllocated) || 0), 0);
            const totalRevenue = relatedCompanies.reduce((sum, c) => sum + (parseInt(c.numberOfSalesAllocated) || 0) * (parseInt(c.revenuePerSale) || 0), 0);
            const avgRevenuePerSale = numberOfSalesAllocated > 0 ? Math.round(totalRevenue / numberOfSalesAllocated) : 0;
            const currentSales = relatedCompanies.reduce((sum, c) => sum + (parseInt(c.currentSales) || 0), 0);
            const next = {
                regionId: region.regionId,
                regionName: region.regionName,
                companyCount: relatedCompanies.length,
                currentSales: currentSales,
                numberOfSalesAllocated: numberOfSalesAllocated,
                revenuePerSale: avgRevenuePerSale, totalRevenue: totalRevenue
            };
            const existing = existingRegionRowsById.get(region.regionId);
            if (existing) {
                Object.assign(existing, next);
                return existing;
            }
            return next;
        });
        regionRows.value = nextRegionRows;

        // 4. Tính toán bảng nhân viên (phân bổ theo thứ tự quản lý vùng, chi nhánh, sale lead, sales)
        const y = Number(formData.value.year);
        const m = Number(formData.value.month);

        const employeesActive = (employeeStore.employees || []).filter((e: any) => e.status === 0 && isActiveInMonth(y, m, e.employeeStartedDate, e.employeeEndDate));
        const regionById = new Map((regionStore.regions || []).map((r: any) => [r.id, r]));
        const companyById = new Map((companyStore.companies || []).map((c: any) => [c.id, c]));
        const companyRowById = new Map((companyRows.value || []).map((r: any) => [r.companyId, r]));
        const result: any[] = [];
        const employeeRowKey = (row: any) =>
            `${row.employeeId ?? ''}_${row.quotaRole ?? ''}_${row.regionId ?? ''}_${row.companyId ?? ''}`;
        const existingEmployeeRowsByKey = new Map(
            (employeeRows.value || []).map((row: any) => [employeeRowKey(row), row])
        );
        const mergeEmployeeRow = (next: any) => {
            const key = employeeRowKey(next);
            const existing = existingEmployeeRowsByKey.get(key);
            if (existing) {
                Object.assign(existing, next);
                return existing;
            }
            return next;
        };
        const pushEmployeeRow = (row: any) => {
            result.push(mergeEmployeeRow(row));
        };

        const pushed = new Set<string>(); // tránh trùng 1 NV xuất hiện 2 lần

        // DUYỆT THEO VÙNG GIỮ ĐÚNG THỨ TỰ
        for (const region of Array.from(regionMap.values())) {
            const regionEntity = regionById.get(region.regionId);

            // 1) ASM (quản lý vùng): quota = tổng chỉ tiêu vùng
            const asm = employeesActive.find((e: any) => e.id === regionEntity?.managerId);
            if (asm) {
                const regionAgg = regionRows.value.find((x: any) => x.regionId === region.regionId);
                const joinDate = toDate(asm.employeeStartedDate);
                const resignDate = toDate(asm.employeeEndDate);
                const { allocationStartAt, allocationEndAt } = getAllocationRange(y, m, joinDate, resignDate);
                pushEmployeeRow({
                    employeeId: asm.id,
                    employeeCode: asm.applicationUser?.userCode,
                    fullName: asm.applicationUser?.fullName,
                    positionName: asm.position?.positionName,
                    positionId: asm.position?.id ?? null,
                    regionId: region.regionId,
                    regionName: region.regionName,
                    quotaRole: QuotaRole.ASM,
                    workStage: getEmployeeStatus({
                        joinedAt: asm.employeeStartedDate ?? undefined,
                        endAt: asm.employeeEndDate ?? undefined,
                        probationEnd: asm.employeeNewEndDate ?? undefined
                    }),
                    companyId: null,
                    companyName: null,
                    revenuePerSale: Math.round(Number(regionAgg?.totalRevenue || 0)),
                    allocationStartAt,
                    allocationEndAt,
                    joinedAt: asm.employeeStartedDate ?? null,
                    endAt: asm.employeeEndDate ?? null,
                    probationEnd: asm.employeeNewEndDate ?? null,
                });
                pushed.add(asm.id ?? '');
            }

            // Danh sách chi nhánh thuộc vùng
            const companyIds = region.companyIds.slice().sort((a, b) => {
                const ca = companyById.get(a)?.companyName || ''; const cb = companyById.get(b)?.companyName || '';
                return ca.localeCompare(cb, 'vi');
            });

            // 2) BM → 3) LEAD → 4) SALE cho từng chi nhánh (chấp nhận trùng)
            for (const cid of companyIds) {
                const comp = companyById.get(cid); if (!comp) continue;
                const cRow = companyRowById.get(cid); if (!cRow) continue;

                const regionName = region.regionName;
                const companyTotalRevenue = Number(cRow.numberOfSalesAllocated || 0) * Number(cRow.revenuePerSale || 0);
                const baseRevenuePerSale = Number(cRow.revenuePerSale || 0);

                // 2) BM chi nhánh: quota = chỉ tiêu toàn chi nhánh (luôn đẩy, dù trùng)
                if (comp.managerId) {
                    const bm = employeesActive.find((e: any) => e.id === comp.managerId);
                    if (bm) {
                        const joinDate = toDate(bm.employeeStartedDate);
                        const resignDate = toDate(bm.employeeEndDate);
                        const { allocationStartAt, allocationEndAt } = getAllocationRange(y, m, joinDate, resignDate);
                        pushEmployeeRow({
                            employeeId: bm.id,
                            employeeCode: bm.applicationUser?.userCode,
                            fullName: bm.applicationUser?.fullName,
                            positionName: bm.position?.positionName,
                            positionId: bm.position?.id ?? null,
                            regionId: region.regionId,
                            regionName: regionName,
                            quotaRole: QuotaRole.BM,
                            workStage: getEmployeeStatus({
                                joinedAt: bm.employeeStartedDate ?? undefined,
                                endAt: bm.employeeEndDate ?? undefined,
                                probationEnd: bm.employeeNewEndDate ?? undefined
                            }),
                            companyId: comp.id,
                            companyName: comp.companyName,
                            revenuePerSale: Math.round(companyTotalRevenue),
                            allocationStartAt,
                            allocationEndAt,
                            joinedAt: bm.employeeStartedDate ?? null,
                            endAt: bm.employeeEndDate ?? null,
                            probationEnd: bm.employeeNewEndDate ?? null,
                        });
                        pushed.add(bm.id ?? '');
                    }
                }

                // 3) SALE LEAD trước
                const leads = employeesActive
                    .filter((e: any) => e.companyId === cid && e.position?.isSaleLead === true)
                    .sort((a: any, b: any) =>
                        (a.applicationUser?.fullName || a.fullName || '').localeCompare(
                            b.applicationUser?.fullName || b.fullName || '', 'vi'
                        )
                    );

                for (const emp of leads) {
                    const joinDate = toDate(emp.employeeStartedDate);
                    const probationEnd = toDate(emp.employeeNewEndDate);
                    const resignDate = toDate(emp.employeeEndDate);
                    const { allocationStartAt, allocationEndAt } = getAllocationRange(y, m, joinDate, resignDate);
                    const quotaRole = resolveEmployeeQuotaRole(emp, QuotaRole.SalesLead, y, m);
                    let quota = calcQuotaForSale(baseRevenuePerSale, y, m, joinDate, probationEnd, resignDate);

                    // Kiểm tra nếu chi nhánh có tháng thành lập là tháng phân bổ
                    const companyStartMonth = new Date(comp.establishmentDate).getMonth() + 1;
                    if (companyStartMonth === m) {
                        quota = Math.round(baseRevenuePerSale); // Quota = doanh thu chi nhánh
                    }

                    pushEmployeeRow({
                        employeeId: emp.id,
                        employeeCode: emp.applicationUser?.userCode,
                        fullName: emp.applicationUser?.fullName,
                        positionName: emp.position?.positionName,
                        positionId: emp.position?.id ?? null,
                        regionId: region.regionId,
                        regionName: regionName,
                        companyId: comp.id,
                        quotaRole,
                        workStage: getEmployeeStatus({
                            joinedAt: emp.employeeStartedDate ?? undefined,
                            endAt: emp.employeeEndDate ?? undefined,
                            probationEnd: emp.employeeNewEndDate ?? undefined
                        }),
                        companyName: comp.companyName,
                        revenuePerSale: quota,
                        allocationStartAt,
                        allocationEndAt,
                        joinedAt: emp.employeeStartedDate ?? null,
                        endAt: emp.employeeEndDate ?? null,
                        probationEnd: emp.employeeNewEndDate ?? null,
                    });
                    pushed.add(emp.id ?? '');
                }

                // 4) SALE thường
                const sales = employeesActive
                    .filter((e: any) => e.companyId === cid && e.position?.isSale === true && e.position?.isSaleLead !== true)
                    .sort((a: any, b: any) =>
                        (a.applicationUser?.fullName || a.fullName || '').localeCompare(
                            b.applicationUser?.fullName || b.fullName || '', 'vi'
                        )
                    );

                for (const emp of sales) {
                    const joinDate = toDate(emp.employeeStartedDate);
                    const probationEnd = toDate(emp.employeeNewEndDate);
                    const resignDate = toDate(emp.employeeEndDate);
                    const { allocationStartAt, allocationEndAt } = getAllocationRange(y, m, joinDate, resignDate);
                    const quotaRole = resolveEmployeeQuotaRole(emp, QuotaRole.Sale, y, m);
                    let quota = calcQuotaForSale(baseRevenuePerSale, y, m, joinDate, probationEnd, resignDate);

                    // Kiểm tra nếu chi nhánh có tháng thành lập là tháng phân bổ
                    const companyStartMonth = new Date(comp.establishmentDate).getMonth() + 1;
                    if (companyStartMonth === m) {
                        console.log(`Chi nhánh ${comp.companyName} thành lập vào tháng ${m}`);
                        quota = Math.round(baseRevenuePerSale); // Quota = doanh thu chi nhánh
                    }
                    //console.log(quota);

                    pushEmployeeRow({
                        employeeId: emp.id,
                        employeeCode: emp.applicationUser?.userCode,
                        fullName: emp.applicationUser?.fullName,
                        positionId: emp.position?.id,
                        positionName: emp.position?.positionName,
                        regionId: region.regionId,
                        regionName: regionName,
                        companyId: comp.id,
                        quotaRole,
                        workStage: getEmployeeStatus({
                            joinedAt: emp.employeeStartedDate ?? undefined,
                            endAt: emp.employeeEndDate ?? undefined,
                            probationEnd: emp.employeeNewEndDate ?? undefined
                        }),
                        companyName: comp.companyName,
                        revenuePerSale: quota,
                        allocationStartAt,
                        allocationEndAt,
                        joinedAt: emp.employeeStartedDate ?? null,
                        endAt: emp.employeeEndDate ?? null,
                        probationEnd: emp.employeeNewEndDate ?? null,
                    });
                    pushed.add(emp.id ?? '');
                }
            }
        }
        applyCompanyTotalsFromEmployees(result);
        employeeRows.value = result;
        console.log(employeeRows.value);

        showAllocations.value = true;
        hasCalculated.value = true;                // cho phép Lưu/Phân bổ
        companiesSnapshot.value = signatureCompanies(companyRows.value); // chốt snapshot mới
        showRecalcBtn.value = false;
    } catch (err) {
        console.error("Recalculate error", err);
        hasCalculated.value = false;

    } finally {
        internalLoading.value = false;
    }
}

// async function handleAllocationChange() {
//     showAllocations.value = false;
//     hasCalculated.value = false;
//     showRecalcBtn.value = false;
//     checkAllocationExist.value = true;
//     await admissionsQuotaStore.fetchAllAdmissionsQuotas();
//     checkAllocationExist.value = !!admissionsQuotaStore.quotas
//         .find(q => q.month === formData.value.month && q.year === formData.value.year);
//     if (checkAllocationExist.value) {
//         notificationStore.showToast('warning', { key: t('toast.allocationExists') });
//     }
// }
async function handleAllocationChange() {
    showAllocations.value = false;
    hasCalculated.value = false;
    showRecalcBtn.value = false;
    checkAllocationExist.value = true;

    await admissionsQuotaStore.fetchAllAdmissionsQuotas();
    checkAllocationExist.value = !!admissionsQuotaStore.quotas
        .find(q => q.month === formData.value.month && q.year === formData.value.year);

    if (checkAllocationExist.value) {
        notificationStore.showToast('warning', { key: t('toast.allocationExists') });
        return;
    }

    // No existing allocation for this month/year; let the Save button be active
}

// có dữ liệu hợp lệ trong company table?
function hasCompanyData(rows: any[]) {
    return Array.isArray(rows) && rows.some(r =>
        Number(r.numberOfSalesAllocated ?? r.numberOfSales ?? 0) > 0 &&
        Number(r.revenuePerSale || 0) > 0
    );
}
// gọi khi bảng company emit update:rows
function onCompanyRows(rows: any[], recalculateTotals = true) {
    companyRows.value = normalizeCompanyRows(rows, recalculateTotals);
    if (isView.value) return;

    hasCalculated.value = false;

    if (props.mode === 'edit') {
        if (!companiesSnapshot.value) {
            companiesSnapshot.value = signatureCompanies(rows);
            showRecalcBtn.value = false;
            return;
        }
        const dirty = signatureCompanies(rows) !== companiesSnapshot.value;
        if (dirty) {
            showRecalcBtn.value = hasCompanyData(rows);
            showAllocations.value = false;
            regionRows.value = [];
            employeeRows.value = [];
        } else {
            showRecalcBtn.value = false;
        }
    } else {
        showRecalcBtn.value = hasCompanyData(rows);
        showAllocations.value = false;
    }
}


function onRegionRows(rows: any[]) {
    regionRows.value = rows;
}
function onEmployeeRows(rows: any[]) {
    employeeRows.value = rows;
    if (isView.value) return;
    applyCompanyTotalsFromEmployees(rows);
}

function resolveTotalRevenue(raw: any, fallback: number) {
    const value = raw === '' || raw === null || raw === undefined ? NaN : Number(raw);
    return Number.isNaN(value) ? fallback : value;
}

function normalizeCompanyRows(rows: any[], recalculateTotals = true) {
    return (rows || []).map((row: any) => {
        const allocated = Number(row.numberOfSalesAllocated ?? row.numberOfSales ?? 0);
        const revenuePerSale = Number(row.revenuePerSale ?? 0);
        const computedTotal = allocated * revenuePerSale;
        const totalRevenue = recalculateTotals
            ? computedTotal
            : resolveTotalRevenue(row.totalRevenue, computedTotal);
        return {
            ...row,
            numberOfSalesAllocated: allocated,
            revenuePerSale,
            totalRevenue,
        };
    });
}

function onSubmit() {
    if (!hasCalculated.value) return; // vô hiệu hoá lưu khi chưa đủ điều kiện
    const data: AdmissionsQuotaModel = buildPayloadNested(formData.value.quotaStage ?? QuotaStatus.Draft, formData.value, regionRows.value, companyRows.value, employeeRows.value);
    emit('submit', { ...data });
}
function onClose() {
    formData.value = {
        year: new Date().getFullYear(),
        note: '',
    };
    checkAllocationExist.value = true;
    showAllocations.value = false;
    hasCalculated.value = false;
    showRecalcBtn.value = false;
    companyRows.value = [];
    regionRows.value = [];
    employeeRows.value = [];

    emit('close');
}

// Approve handler for the footer button
function handleApprove() {
    if (!hasCalculated.value) return;
    const y = Number(formData.value.year);
    const m = Number(formData.value.month);
    const currentMonth = new Date().getMonth() + 1;
    const quotaStage =
        currentMonth == m ? QuotaStatus.InProgress : QuotaStatus.Allocated;
    const data = buildPayloadNested(quotaStage, formData.value, regionRows.value, companyRows.value, employeeRows.value);
    emit('submit', { ...data });
}
function buildPayloadNested(
    quotaStage: number,
    form: any,
    regionRows: any[],
    companyRows: any[],
    employeeRows: any[]
) {
    // Nếu form.id có giá trị => đang update
    const isUpdate = !!form?.id;

    const actualByCompany = new Map<string, number>();
    for (const e of (employeeRows || [])) {
        const companyId = e?.companyId;
        if (!companyId) continue;
        if (
            e.quotaRole === QuotaRole.Sale ||
            e.quotaRole === QuotaRole.SalesLead ||
            e.quotaRole === QuotaRole.ProbationEmployee ||
            e.quotaRole === QuotaRole.LeavingEmployee
        ) {
            const current = actualByCompany.get(companyId) ?? 0;
            actualByCompany.set(companyId, current + Number(e.revenuePerSale || 0));
        }
    }

    // 1) Region map
    const regionMap = new Map<string, any>();
    for (const r of (regionRows || [])) {
        const n = Number(r.numberOfSalesAllocated ?? r.numberOfSales ?? 0);
        const rev = Number(r.revenuePerSale || 0);

        const regionDto: any = {
            // CHỈ include id khi update
            id: isUpdate ? (r.id || undefined) : undefined,
            regionId: r.regionId,
            companyCount: Number(r.companyCount || 0),
            currentSales: Number(r.currentSales || 0),
            numberOfSalesAllocated: n,
            revenuePerSale: rev,
            totalRevenue: resolveTotalRevenue(r.totalRevenue, n * rev),
            note: r.note ?? '',
            admissionsQuotaCompanies: [],
            admissionsQuotaEmployees: []
        };

        regionMap.set(r.regionId, regionDto);
    }

    // 2) Companies vào region
    for (const c of (companyRows || [])) {
        const rId = c.regionId;
        const parentRegion = regionMap.get(rId);
        if (!parentRegion) continue; // dữ liệu lệch thì bỏ qua (hoặc tạo fallback nếu muốn)

        const n = Number(c.numberOfSalesAllocated ?? c.numberOfSales ?? 0);
        const rev = Number(c.revenuePerSale || 0);
        const planned = n * rev;
        const actual = actualByCompany.get(c.companyId) ?? 0;
        const defaultTotal = Math.max(planned, actual);
        const hasAdjustment = Array.isArray(c.admissionsQuotaAdjustments) && c.admissionsQuotaAdjustments.length > 0;
        const totalRevenue = hasAdjustment
            ? resolveTotalRevenue(c.totalRevenue, defaultTotal)
            : defaultTotal;

        const companyDto: any = {
            // CHỈ include id khi update
            id: isUpdate ? (c.id || undefined) : undefined,
            // CHỈ include FK admissionsQuotaRegionId khi update và parentRegion đã có id
            admissionsQuotaRegionId: isUpdate ? (parentRegion.id || undefined) : undefined,
            companyId: c.companyId,
            currentSales: Number(c.currentSales || 0),
            numberOfSalesAllocated: n,
            numberOfPartTimeSales: Number(c.numberOfPartTimeSales || 0),
            regionId: rId,
            revenuePerSale: rev,
            totalRevenue,
            note: c.note ?? '',
            admissionsQuotaEmployees: []
        };

        parentRegion.admissionsQuotaCompanies.push(companyDto);
    }
    for (const reg of regionMap.values()) {
        const totalRevenue = (reg.admissionsQuotaCompanies || []).reduce(
            (s: number, comp: any) => s + Number(comp.totalRevenue || 0),
            0
        );
        reg.totalRevenue = totalRevenue;
        const salesAllocated = Number(reg.numberOfSalesAllocated || 0);
        reg.revenuePerSale = salesAllocated > 0 ? Math.round(totalRevenue / salesAllocated) : 0;
    }



    // Index công ty theo regionId_companyId để nhét nhân viên nhanh
    const companyIndex = new Map<string, any>();
    for (const reg of regionMap.values()) {
        for (const comp of reg.admissionsQuotaCompanies) {
            companyIndex.set(`${reg.regionId}_${comp.companyId}`, comp);
        }
    }

    // 3) Employees
    for (const e of (employeeRows || [])) {
        const key = `${e.regionId ?? ''}_${e.companyId ?? ''}`;
        const comp = companyIndex.get(key);
        const parentRegion = regionMap.get(e.regionId ?? '');

        const { allocationStartAt, allocationEndAt } = clampAllocationRange(
            Number(form.year),
            Number(form.month),
            e.allocationStartAt ?? e.joinedAt ?? e.joinDate,
            e.allocationEndAt ?? e.endAt ?? e.endDate
        );
        const eDto: any = {
            // CHỈ include id khi update
            id: isUpdate ? (e.id || undefined) : undefined,
            // CHỈ include các FK nested khi update
            admissionsQuotaCompanyId: isUpdate ? (comp?.id || undefined) : undefined,
            admissionsQuotaRegionId: isUpdate ? (parentRegion?.id || undefined) : undefined,

            employeeId: e.employeeId,
            companyId: e.companyId ?? null,
            regionId: e.regionId ?? null,
            positionId: e.positionId ?? null,
            revenuePerSale: Number(e.revenuePerSale || 0),
            quotaRole: e.quotaRole ?? null,
            workStage: e.workStage ?? null,
            joinedAt: e.joinedAt ?? null,
            endAt: e.endAt ?? null,
            allocationStartAt,
            allocationEndAt,
            probationEnd: e.probationEnd ?? null,
            note: e.note ?? ''
        };

        if (comp) comp.admissionsQuotaEmployees.push(eDto);        // BM/LEAD/SALE
        else if (parentRegion) parentRegion.admissionsQuotaEmployees.push(eDto); // ASM
    }

    // 4) Root payload
    const admissionsQuotaRegions = Array.from(regionMap.values()).map((reg: any) => {
        // luôn đồng bộ companyCount theo mảng con
        reg.companyCount = Array.isArray(reg.admissionsQuotaCompanies)
            ? reg.admissionsQuotaCompanies.length
            : 0;
        return reg;
    });
    const payload: any = {
        // Root id: CHỈ include khi update
        month: Number(form.month),
        year: Number(form.year),
        note: form.note ?? '',
        status: 1,
        quotaStage,
        // dùng length để chắc chắn có số đúng
        companyCount: admissionsQuotaRegions.reduce(
            (s: number, r: any) => s + (r.admissionsQuotaCompanies?.length ?? 0),
            0
        ), currentSales: admissionsQuotaRegions.reduce((s, r) => s + (r.currentSales || 0), 0),
        totalSalesAllocated: admissionsQuotaRegions.reduce((s, r) => s + (r.numberOfSalesAllocated || 0), 0),
        totalQuota: admissionsQuotaRegions.reduce((s, r) => s + (r.totalRevenue || 0), 0),
        admissionsQuotaRegions
    };

    if (isUpdate) {
        payload.id = form.id; // để BE map đúng bản ghi gốc
    }
    console.log(payload);

    return payload;
}

function getAllocationRange(
    y: number,
    m: number,
    joinDate?: Date | null,
    resignDate?: Date | null
) {
    const monthStart = firstDayOfMonth(y, m);
    const monthEnd = lastDayOfMonth(y, m);
    const startDate = joinDate && joinDate > monthStart ? joinDate : monthStart;
    const endDate = resignDate && resignDate < monthEnd ? resignDate : monthEnd;
    if (endDate < startDate) {
        return { allocationStartAt: null, allocationEndAt: null };
    }
    return {
        allocationStartAt: toDateOnlyString(startDate),
        allocationEndAt: toDateOnlyString(endDate),
    };
}

function clampAllocationRange(
    y: number,
    m: number,
    startInput?: any,
    endInput?: any
) {
    const monthStart = firstDayOfMonth(y, m);
    const monthEnd = lastDayOfMonth(y, m);
    const startDate = toDateLocal(startInput) ?? monthStart;
    const endDate = toDateLocal(endInput) ?? monthEnd;
    const allocationStartAt = startDate < monthStart ? monthStart : startDate > monthEnd ? monthEnd : startDate;
    const allocationEndAt = endDate < monthStart ? monthStart : endDate > monthEnd ? monthEnd : endDate;
    if (allocationEndAt < allocationStartAt) {
        return { allocationStartAt: null, allocationEndAt: null };
    }
    return {
        allocationStartAt: toDateOnlyString(allocationStartAt),
        allocationEndAt: toDateOnlyString(allocationEndAt),
    };
}

function toDateOnlyString(date: Date | null) {
    if (!date) return null;
    const y = date.getFullYear();
    const m = String(date.getMonth() + 1).padStart(2, '0');
    const d = String(date.getDate()).padStart(2, '0');
    return `${y}-${m}-${d}`;
}

function resolveEmployeeQuotaRole(emp: any, baseRole: QuotaRole, y: number, m: number) {
    const resignDate = toDate(emp.employeeEndDate);
    if (resignDate && isWithinMonth(resignDate, y, m)) {
        return QuotaRole.LeavingEmployee;
    }
    const joinDate = toDate(emp.employeeStartedDate);
    const probationEnd = toDate(emp.employeeNewEndDate);
    const monthStart = firstDayOfMonth(y, m);
    const monthEnd = lastDayOfMonth(y, m);
    const startedThisMonth = !!joinDate && isWithinMonth(joinDate, y, m);
    const probationInMonth =
        !!probationEnd && probationEnd >= monthStart && (!joinDate || joinDate <= monthEnd);
    if (startedThisMonth || probationInMonth) {
        return QuotaRole.ProbationEmployee;
    }
    return baseRole;
}


function calcQuotaForSale(
    baseRevenuePerSale: number,
    y: number,
    m: number,
    joinDate?: Date | null,        // Ngay bat dau
    _probationEnd?: Date | null,   // reserved
    resignDate?: Date | null       // Ngay nghi viec
) {
    const monthStart = firstDayOfMonth(y, m);
    const monthEnd = lastDayOfMonth(y, m);
    const stdDays = countWorkingDaysInMonth(y, m);
    const probationStdDays = Math.min(stdDays, 26);
    if (!stdDays) return 0;

    if (resignDate && isWithinMonth(resignDate, y, m)) {
        const startDate = joinDate && joinDate > monthStart ? joinDate : monthStart;
        const endDate = resignDate;
        if (!startDate || !endDate || endDate < startDate) return 0;
        const dWork = countWorkingDaysBetween(startDate, endDate);
        if (!dWork || dWork < 7) return 0;
        return Math.round((baseRevenuePerSale / stdDays) * dWork);
    }

    const rangeStart = joinDate && joinDate > monthStart ? joinDate : monthStart;
    const rangeEnd = resignDate && resignDate < monthEnd ? resignDate : monthEnd;
    if (!rangeStart || !rangeEnd || rangeEnd < rangeStart) return 0;

    const dWork = countWorkingDaysBetween(rangeStart, rangeEnd);
    if (dWork < 7) return 0;

    const dayBeforeRange = addDays(rangeStart, -1);
    const dProb = countWorkingDaysAccumulated(joinDate, dayBeforeRange);
    const factor = (joinDate && dProb < 26)
        ? (baseRevenuePerSale / probationStdDays)
        : (baseRevenuePerSale / stdDays);

    if (!joinDate) {
        return Math.round(factor * dWork);
    }

    const d60 = Math.max(0, Math.min(dWork, 26 - dProb));
    const d100 = dWork - d60;

    const quota = factor * (0.6 * d60 + 1.0 * d100);
    return Math.round(quota);
}

function applyCompanyTotalsFromEmployees(rows: any[]) {
    const actualByCompany = new Map<string, number>();
    for (const row of rows || []) {
        if (!row?.companyId) continue;
        if (
            row.quotaRole === QuotaRole.Sale ||
            row.quotaRole === QuotaRole.SalesLead ||
            row.quotaRole === QuotaRole.ProbationEmployee ||
            row.quotaRole === QuotaRole.LeavingEmployee
        ) {
            const current = actualByCompany.get(row.companyId) ?? 0;
            actualByCompany.set(row.companyId, current + Number(row.revenuePerSale || 0));
        }
    }

    const totalByCompany = new Map<string, number>();
    companyRows.value = (companyRows.value || []).map((c: any) => {
        const planned = Number(c.numberOfSalesAllocated ?? c.numberOfSales ?? 0) * Number(c.revenuePerSale ?? 0);
        const actual = actualByCompany.get(c.companyId) ?? 0;
        const defaultTotal = Math.max(planned || 0, actual || 0);
        const hasAdjustment = Array.isArray(c.admissionsQuotaAdjustments) && c.admissionsQuotaAdjustments.length > 0;
        const total = hasAdjustment ? resolveTotalRevenue(c.totalRevenue, defaultTotal) : defaultTotal;
        totalByCompany.set(c.companyId, total);
        return { ...c, totalRevenue: total };
    });

    recomputeRegionsFromCompanies();
    const totalByRegion = new Map((regionRows.value || []).map((r: any) => [r.regionId, Number(r.totalRevenue ?? 0)]));

    for (const row of rows || []) {
        if (row.quotaRole === QuotaRole.BM && row.companyId) {
            const total = totalByCompany.get(row.companyId);
            if (total !== undefined) row.revenuePerSale = Math.round(total);
        }
        if (row.quotaRole === QuotaRole.ASM && row.regionId) {
            const total = totalByRegion.get(row.regionId);
            if (total !== undefined) row.revenuePerSale = Math.round(total);
        }
    }
}

// function hydrateFromQuota(quota: any) {
//     const regRows: any[] = [];
//     const compRows: any[] = [];
//     const empRows: any[] = [];

//     const regions = quota?.admissionsQuotaRegions ?? [];
//     for (const reg of regions) {
//         // REGION
//         regRows.push({
//             id: reg.id ?? null,
//             regionId: reg.regionId,
//             regionName: reg.region?.regionName ?? reg.regionName ?? '',
//             companyCount: Number(reg.companyCount ?? (reg.admissionsQuotaCompanies?.length ?? 0)),
//             currentSales: Number(reg.currentSales ?? 0),
//             numberOfSalesAllocated: Number(reg.numberOfSalesAllocated ?? 0),
//             revenuePerSale: Number(reg.revenuePerSale ?? 0),
//             totalRevenue: Number(reg.totalRevenue ?? 0),
//             admissionsQuotaAdjustments: reg.admissionsQuotaAdjustments ?? [],
//             note: reg.note ?? ''
//         });

//         // COMPANY
//         //console.log(reg.admissionsQuotaCompanies);

//         for (const comp of reg.admissionsQuotaCompanies ?? []) {
//             compRows.push({
//                 id: comp.id ?? null,
//                 admissionsQuotaRegionId: reg.id ?? null,
//                 regionId: reg.regionId,
//                 regionName: reg.region?.regionName ?? '',
//                 companyId: comp.companyId,
//                 companyName: comp.company?.companyName ?? comp.companyName ?? '',
//                 currentSales: Number(comp.currentSales ?? 0),
//                 numberOfSalesAllocated: Number(comp.numberOfSalesAllocated ?? 0),
//                 revenuePerSale: Number(comp.revenuePerSale ?? 0),
//                 totalRevenue: Number(comp.totalRevenue ?? 0),
//                 admissionsQuotaAdjustments: comp.admissionsQuotaAdjustments ?? [],
//                 note: comp.note ?? ''
//             });
//         }
//         // console.log(empRows);
//         //console.log(reg.admissionsQuotaEmployees);

//         // EMP vùng (ASM)
//         for (const e of reg.admissionsQuotaEmployees ?? []) {
//             empRows.push(createEmp(e, reg));

//         }
//     }
//     if (regionRows !== undefined) {
//         regionRows.value = regRows;
//     }
//     if (companyRows !== undefined) {
//         companyRows.value = compRows;
//     }
//     if (employeeRows !== undefined) {
//         employeeRows.value = empRows;
//     }
//     showAllocations.value = true;        // hiện Region/Employee
//     checkAllocationExist.value = true;   // đã có phân bổ
// }
function hydrateFromQuota(quota: any) {
    const regRows: any[] = [];
    const compRows: any[] = [];
    const empRows: any[] = [];

    const regions = quota?.admissionsQuotaRegions ?? [];
    for (const reg of regions) {
        // REGION
        regRows.push({
            id: reg.id ?? null,
            regionId: reg.regionId,
            regionName: reg.region?.regionName ?? reg.regionName ?? '',
            companyCount: Number(reg.companyCount ?? (reg.admissionsQuotaCompanies?.length ?? 0)),
            currentSales: Number(reg.currentSales ?? 0),
            numberOfSalesAllocated: Number(reg.numberOfSalesAllocated ?? 0),
            revenuePerSale: Number(reg.revenuePerSale ?? 0),
            totalRevenue: Number(reg.totalRevenue ?? 0),
            admissionsQuotaAdjustments: reg.admissionsQuotaAdjustments ?? [],
            note: reg.note ?? ''
        });
        empRows.push(...(reg.admissionsQuotaEmployees ?? []).map((e: any) => createEmp(e, reg)));
        // COMPANY
        //console.log(reg.admissionsQuotaCompanies);

        for (const comp of reg.admissionsQuotaCompanies ?? []) {
            compRows.push({
                id: comp.id ?? null,
                admissionsQuotaRegionId: reg.id ?? null,
                regionId: reg.regionId,
                regionName: reg.region?.regionName ?? '',
                companyId: comp.companyId,
                companyName: comp.company?.companyName ?? comp.companyName ?? '',
                currentSales: Number(comp.currentSales ?? 0),
                numberOfSalesAllocated: Number(comp.numberOfSalesAllocated ?? 0),
                numberOfPartTimeSales: Number(comp.numberOfPartTimeSales || 0),
                revenuePerSale: Number(comp.revenuePerSale ?? 0),
                totalRevenue: Number(comp.totalRevenue ?? 0),
                admissionsQuotaAdjustments: comp.admissionsQuotaAdjustments ?? [],
                note: comp.note ?? ''
            });
            // EMP chi nhánh (BM/LEAD/SALE)
            empRows.push(...(comp.admissionsQuotaEmployees ?? []).map((e: any) => createEmp(e, reg)));
        }
        // console.log(empRows);
        //console.log(reg.admissionsQuotaEmployees);

        // EMP vùng (ASM)
        // for (const e of reg.admissionsQuotaEmployees ?? []) {
        //     empRows.push(createEmp(e, reg));

        // }
    }
    if (regionRows !== undefined) {
        regionRows.value = regRows;
    }
    if (companyRows !== undefined) {
        companyRows.value = normalizeCompanyRows(compRows, false);
    }
    if (employeeRows !== undefined) {
        employeeRows.value = empRows;
    }
    showAllocations.value = true;        // hiện Region/Employee
    checkAllocationExist.value = true;   // đã có phân bổ
}
// tạo chữ ký ổn định cho bảng company (chỉ lấy field quan trọng, sort theo companyId)
function signatureCompanies(rows: any[]) {
    const picked = (rows || []).map(r => ({
        companyId: r.companyId,
        regionId: r.regionId,
        currentSales: Number(r.currentSales || 0),
        numberOfSalesAllocated: Number(r.numberOfSalesAllocated ?? r.numberOfSales ?? 0),
        numberOfPartTimeSales: Number(r.numberOfPartTimeSales || 0),
        revenuePerSale: Number(r.revenuePerSale || 0),
        note: r.note ?? ''
    })).sort((a, b) => (a.companyId || '').localeCompare(b.companyId || ''));
    return JSON.stringify(picked);
}
function onCompanyAdjusted({ aqcId, totalAfter }) {
    // sync companyRows
    const i = companyRows.value.findIndex(r => r.id === aqcId || r.admissionsQuotaCompanyId === aqcId);
    if (i > -1) companyRows.value[i].totalRevenue = Number(totalAfter ?? 0);

    // gom lại Region từ companyRows (nếu đang hiển thị region table)
    recomputeRegionsFromCompanies();
    if (isView.value) return;

    // Mark as not calculated and show recalc button when company data changes
    hasCalculated.value = false;
    showRecalcBtn.value = hasCompanyData(companyRows.value);
}


function recomputeRegionsFromCompanies() {
    // map regionId -> list companies
    const map = new Map<string, any[]>();
    for (const c of companyRows.value) {
        if (!map.has(c.regionId)) map.set(c.regionId, []);
        map.get(c.regionId)!.push(c);
    }
    const existingByRegionId = new Map((regionRows.value || []).map((r: any) => [r.regionId, r]));
    // build regionRows mới: totalRevenue = SUM(company.totalRevenue)
    regionRows.value = Array.from(map.entries()).map(([regionId, comps]) => {
        const numberOfSalesAllocated = comps.reduce((s, x) => s + Number(x.numberOfSalesAllocated || 0), 0);
    const totalRevenue = comps.reduce((s, x) => {
        const planned = Number(x.numberOfSalesAllocated || 0) * Number(x.revenuePerSale || 0);
        return s + resolveTotalRevenue(x.totalRevenue, planned);
    }, 0);
        const currentSales = comps.reduce((s, x) => s + Number(x.currentSales || 0), 0);
        const revenuePerSale = numberOfSalesAllocated > 0 ? Math.round(totalRevenue / numberOfSalesAllocated) : 0;

        // giữ regionName từ company đầu
        const regionName = comps[0]?.regionName ?? '';
        const next = {
            regionId,
            regionName,
            companyCount: comps.length,
            currentSales,
            numberOfSalesAllocated,
            revenuePerSale,
            totalRevenue
        };
        const existing = existingByRegionId.get(regionId);
        if (existing) {
            Object.assign(existing, next);
            return existing;
        }
        return next;
    });
}
async function closeAndReload() {
    // reload danh sách ở store (để list ngoài cập nhật)
    await admissionsQuotaStore.fetchAllAdmissionsQuotas()

    // đóng dialog hiện tại
    emit('update:visible', false)

    // báo cho parent (list page) nếu cần hook thêm
    emit('close')
}
function createEmp(e: any, reg: any) {
    const { allocationStartAt, allocationEndAt } = clampAllocationRange(
        Number(formData.value.year),
        Number(formData.value.month),
        e.allocationStartAt ?? e.joinedAt ?? e.joinDate,
        e.allocationEndAt ?? e.endAt ?? e.endDate
    );
    return {
        id: e.id ?? null,
        admissionsQuotaCompanyId: e.admissionsQuotaCompanyId ?? null,
        admissionsQuotaRegionId: reg.id ?? null,
        employeeId: e.employeeId,
        employeeCode: e.employee?.applicationUser?.userCode ?? e.employeeCode ?? '',
        fullName: e.employee?.applicationUser?.fullName ?? e.fullName ?? '',
        positionId: e.positionId ?? e.position?.id ?? null,
        positionName: e.position?.positionName ?? e.positionName ?? '',
        quotaRole: e.quotaRole ?? QuotaRole.ASM,
        workStage: e.workStage ?? null,
        regionId: reg.regionId,
        regionName: reg.region?.regionName ?? '',
        companyId: e.companyId ?? null,
        companyName: e.company?.companyName ?? '',
        numberOfSalesAllocated: Number(e.numberOfSalesAllocated ?? 1),
        revenuePerSale: Number(e.revenuePerSale ?? 0),
        allocationStartAt,
        allocationEndAt,
        joinedAt: e.joinedAt ?? null,
        endAt: e.endAt ?? null,
        probationEnd: e.probationEnd ?? null,
        note: e.note ?? ''
    }
}
</script>

<style scoped>
.summary-card {
    min-height: 120px;
}
</style>
