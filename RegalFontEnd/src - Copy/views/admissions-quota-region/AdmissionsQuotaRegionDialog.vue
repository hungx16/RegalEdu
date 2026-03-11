<template>
    <BaseDialogForm :visible="visible" :title="modeTitle" :mode="mode" :form-data="formData" :rules="rules"
        :loading="loading" :submit-disabled="true" :width="computedDialogWidth" class="admissions-quota-region-dialog"
        @update:visible="$emit('update:visible', $event)" @close="$emit('close')">
        <template #form>
            <!-- TOP: Month/Year + Note (chỉ xem) -->
            <div class="row g-4 align-items-end">
                <div class="col-12 col-md-3">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('admissionsQuota.month') }}</label>
                    <el-form-item prop="month">
                        <el-input :model-value="formData.month" disabled />
                    </el-form-item>
                </div>
                <div class="col-12 col-md-3">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('admissionsQuota.year') }}</label>
                    <el-form-item prop="year">
                        <el-input :model-value="formData.year" disabled />
                    </el-form-item>
                </div>
                <div class="col-12 col-md-6">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.note') }}</label>
                    <el-form-item prop="note">
                        <el-input :model-value="formData.note" :placeholder="t('admissionsQuota.notePlaceholder')"
                            disabled />
                    </el-form-item>
                </div>
            </div>

            <!-- SUMMARY -->
            <div class="row g-4 mt-2 mb-6">
                <div class="col-12 col-xl-6">
                    <div class="summary-card p-6 rounded-4 shadow-sm bg-body h-100">
                        <div class="d-flex justify-content-between align-items-start mb-1">
                            <span class="fw-semibold fs-6">{{ t('admissionsQuota.totalAllocatedRevenue') }}</span>
                            <i class="bi bi-cash-coin fs-4 text-body-secondary"></i>
                        </div>
                        <div class="fs-2 fw-bold">{{ formatCurrency(totalRevenue) }}</div>
                        <div class="fs-8 text-body-secondary">{{ t('admissionsQuota.totalAllocatedRevenueHint') }}</div>
                    </div>
                </div>
                <div class="col-12 col-xl-6">
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
            </div>

            <!-- COMPANY TABLE (view only) -->
            <div class="card mb-6">
                <div class="card-header d-flex align-items-center justify-content-between">
                    <div class="card-title">
                        <h3 class="fs-5 fw-bold m-0">{{ regionTitle }}</h3>
                        <div class="text-muted fs-8" v-if="regionName">
                            {{ t('region.name') }}: <strong>{{ regionName }}</strong>
                        </div>
                    </div>
                </div>
                <div class="card-body p-2">
                    <QuotaCompanyTable
                        :key="`${formData.admissionsQuotaId}-${currentRegionId}-${formData.month}-${formData.year}`"
                        :quota-id="formData.admissionsQuotaId" :month="formData.month" :year="formData.year"
                        @request-close="closeAndReload" :show-support="true" :show-edit="false" :rows="companyRows"
                        :disabled="true" :quota-stage="formData.quotaStage" @update:rows="() => { }" />

                </div>
            </div>

            <!-- EMPLOYEE TABLE (view only) -->
            <div class="card mb-2">
                <div class="card-header d-flex align-items-center justify-content-between">
                    <div class="card-title">
                        <h3 class="fs-5 fw-bold m-0">{{ t('admissionsQuota.tableEmployee') }}</h3>
                    </div>
                </div>
                <div class="card-body p-2">
                    <QuotaEmployeeTable :quota-id="formData.admissionsQuotaId" :month="formData.month"
                        :year="formData.year" :rows="employeeRows" :disabled="true" />
                </div>
            </div>
        </template>

        <!-- Footer rỗng: chỉ xem -->
        <template #footer-extra />
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import QuotaCompanyTable from '@/components/admissions-quota-table/QuotaCompanyTable.vue'
import QuotaEmployeeTable from '@/components/admissions-quota-table/QuotaEmployeeTable.vue'
import { formatCurrency } from '@/utils/format'
import { useAdmissionsQuotaRegionStore } from '@/stores/admissionsQuotaRegionStore' // <- store gọi API getRegionById
// ---- Props / Emits
interface Props {
    visible: boolean
    mode: 'create' | 'edit' | 'view'
    regionId?: string | null
}
const props = defineProps<Props>()
const emit = defineEmits(['update:visible', 'close'])
const currentRegionId = ref<string>('')

// ---- i18n
const { t } = useI18n()

// ---- state
const loading = ref(false)
const regionName = ref<string>('')
const companyRows = ref<any[]>([])
const employeeRows = ref<any[]>([])
const formData = ref<any>({
    admissionsQuotaId: '',
    month: 0,
    year: 0,
    note: '',
    quotaStage: 0
})

const rules = {}
const computedDialogWidth = computed(() => window.innerWidth < 768 ? '100%' : '80%')
const modeTitle = computed(() => t('admissionsQuota.viewTitle'))
const regionTitle = computed(() => t('admissionsQuota.tableCompany'))

// ---- store + API
const admissionsQuotaRegionStore = useAdmissionsQuotaRegionStore()

// ---- helpers
const sumByCompany = (arr: any[]) =>
    arr.reduce((s, r) => s + Number(r.totalRevenue ?? ((r.numberOfSalesAllocated ?? r.numberOfSales ?? 0) * (r.revenuePerSale || 0))), 0)

const companyCount = computed(() => new Set(companyRows.value.map(r => r.companyId)).size)
const totalSales = computed(() => companyRows.value.reduce((s, r) => s + Number(r.currentSales || 0), 0))
const totalRevenue = computed(() => sumByCompany(companyRows.value))

// ---- load
async function loadRegionDetail(id?: string | null) {
    if (!id) return
    loading.value = true
    try {
        await admissionsQuotaRegionStore.fetchAdmissionsQuotaRegionById(id)
        const region = admissionsQuotaRegionStore.selectedRegion
        console.log(region);
        currentRegionId.value = region?.id ?? ''

        // reset trước khi gán mới để tránh flicker
        companyRows.value = []
        employeeRows.value = []
        // header info (tháng/năm/note lấy từ admissionsQuota nếu có)
        formData.value = {
            admissionsQuotaId: region?.admissionsQuotaId ?? region?.admissionsQuota?.id ?? '',
            month: region?.admissionsQuota?.month ?? '',
            year: region?.admissionsQuota?.year ?? '',
            note: region?.admissionsQuota?.note ?? '',
            quotaStage: region?.admissionsQuota?.quotaStage ?? 0
        }
        regionName.value = region?.region?.regionName ?? ''

        // companies
        companyRows.value = (region?.admissionsQuotaCompanies ?? []).map((c: any) => ({
            id: c.id ?? null,
            admissionsQuotaCompanyId: c.id ?? null,
            companyId: c.companyId,
            companyName: c.company?.companyName ?? c.companyName ?? '',
            regionId: region?.regionId ?? null,
            regionName: region?.region?.regionName ?? '',
            currentSales: Number(c.currentSales ?? 0),
            numberOfSalesAllocated: Number(c.numberOfSalesAllocated ?? 0),
            revenuePerSale: Number(c.revenuePerSale ?? 0),
            totalRevenue: Number(c.totalRevenue ?? ((c.numberOfSalesAllocated ?? 0) * (c.revenuePerSale ?? 0))),
            admissionsQuotaAdjustments: c.admissionsQuotaAdjustments ?? [],
            admissionsQuotaId: region?.admissionsQuotaId,
            month: formData.value.month,
            year: formData.value.year
        }))

        // employees (gộp ASM cấp vùng + NV ở các company)
        const empRegion = (region?.admissionsQuotaEmployees ?? []).map((e: any) => normalizeEmp(e, region))
        const empCompanies = (region?.admissionsQuotaCompanies ?? []).reduce((acc: any[], c: any) => {
            const emps = (c.admissionsQuotaEmployees ?? []).map((e: any) => normalizeEmp(e, region, c))
            return acc.concat(emps)
        }, [] as any[])
        employeeRows.value = [...empRegion, ...empCompanies]
        //employeeRows.value = [...empRegion]

    } finally {
        loading.value = false
    }
}

function normalizeEmp(e: any, reg: any, comp?: any) {
    return {
        id: e.id ?? null,
        employeeId: e.employeeId,
        employeeCode: e.employee?.applicationUser?.userCode ?? e.employeeCode ?? '',
        fullName: e.employee?.applicationUser?.fullName ?? e.fullName ?? '',
        positionId: e.positionId,
        positionName: e.position?.positionName ?? e.positionName ?? '',
        regionId: reg?.regionId ?? e.regionId ?? null,
        regionName: reg?.region?.regionName ?? '',
        companyId: comp?.companyId ?? e.companyId ?? null,
        companyName: comp?.company?.companyName ?? e.company?.companyName ?? '',
        quotaRole: e.quotaRole ?? null,
        workStage: e.workStage ?? null,
        revenuePerSale: Number(e.revenuePerSale ?? 0),
        joinedAt: e.joinAt ?? e.joinedAt ?? null,
        endAt: e.endAt ?? null,
        probationEnd: e.probationEnd ?? null,
        note: e.note ?? ''
    }
}

// ---- watch open
watch(
    () => [props.visible, props.mode, props.regionId],
    async ([v, m, id]) => {
        if (v && m === 'view' && typeof id === 'string') {
            await loadRegionDetail(id)
        }
    },
    { immediate: false }
)

async function closeAndReload() {
    await admissionsQuotaRegionStore.fetchAllRegions();
    emit('update:visible', false)
    emit('close')
}

</script>

<style scoped>
.summary-card {
    min-height: 120px;
}
</style>
