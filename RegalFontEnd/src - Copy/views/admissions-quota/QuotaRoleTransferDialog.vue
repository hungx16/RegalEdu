<template>
    <el-dialog v-model="showDialog" :title="t('admissionsQuota.transferDialog.title')" width="90%"
        :append-to-body="true">
        <div class="quota-transfer">
            <el-form label-position="top">
                <el-row :gutter="12">
                    <el-col :span="12">
                        <el-form-item :label="t('admissionsQuota.transferDialog.quotaLabel')" required>
                            <el-select v-model="selectedQuotaId" filterable clearable
                                :placeholder="t('admissionsQuota.transferDialog.selectQuota')" :loading="quotaLoading">
                                <el-option v-for="q in (quotaOptions || []).filter(q => q.id)" :key="q.id as string"
                                    :value="q.id" :label="`${q.year} / ${q.month}`">
                                    {{ q.year }} / {{ q.month }}
                                </el-option>
                            </el-select>
                        </el-form-item>
                    </el-col>
                    <el-col :span="12">
                        <el-form-item :label="t('admissionsQuota.transferDialog.employeeLabel')" required>
                            <el-select v-model="employeeId" filterable clearable :filter-method="filterEmployeeOption"
                                :placeholder="t('admissionsQuota.transferDialog.selectEmployee')"
                                :disabled="!employeesReady" :loading="!employeesReady">
                                <el-option v-for="opt in employeeOptions" :key="opt.id" :label="opt.label"
                                    :value="opt.id" />
                            </el-select>
                        </el-form-item>
                    </el-col>
                </el-row>
            </el-form>

            <el-alert v-if="errors.length" type="error" show-icon :closable="false" class="mb-2">
                <div v-for="(e, i) in errors" :key="i">{{ e }}</div>
            </el-alert>

            <div class="segment-toolbar">
                <el-button type="primary" plain @click="addSegment()" :disabled="!canAddSegment">
                    {{ t('admissionsQuota.transferDialog.addRow') }}
                </el-button>
                <div class="spacer"></div>
                <el-button type="info" plain @click="previewValidate">
                    {{ t('admissionsQuota.transferDialog.validate') }}
                </el-button>
                <el-button type="success" :loading="saving" :disabled="saving || quotaLoading || !quotaDetail"
                    @click="apply">
                    {{ t('admissionsQuota.transferDialog.apply') }}
                </el-button>
            </div>

            <el-table v-if="segments.length" :data="segments" border row-key="key" class="segment-table">
                <el-table-column type="index" :label="t('admissionsQuota.transferDialog.indexLabel')" width="50" />
                <el-table-column :label="t('admissionsQuota.transferDialog.roleLabel')" width="120">
                    <template #default="{ row }">
                        <el-select v-model="row.role" @change="onRoleChange(row)" style="width: 100%">
                            <el-option v-for="opt in roleOptions" :key="opt.value" :label="t(opt.label)"
                                :value="opt.value" />
                        </el-select>
                    </template>
                </el-table-column>
                <el-table-column :label="t('admissionsQuota.transferDialog.positionLabel')" width="180">
                    <template #default="{ row }">
                        <el-select v-model="row.positionId" filterable clearable :disabled="!positionOptions.length"
                            :placeholder="t('admissionsQuota.transferDialog.selectPosition')">
                            <el-option v-for="opt in positionOptions" :key="opt.id" :label="opt.label"
                                :value="opt.id" />
                        </el-select>
                    </template>
                </el-table-column>
                <el-table-column :label="t('admissionsQuota.transferDialog.companyLabel')" min-width="200">
                    <template #default="{ row }">
                        <el-select v-model="row.companyId" clearable filterable
                            :placeholder="t('admissionsQuota.transferDialog.selectCompany')"
                            :disabled="row.role === QuotaRole.ASM || !companiesReady" :loading="companyStore.loading"
                            @change="onCompanyChange(row)">
                            <el-option v-for="opt in companyOptions" :key="opt.id" :label="opt.label" :value="opt.id" />
                        </el-select>
                    </template>
                </el-table-column>
                <el-table-column :label="t('admissionsQuota.transferDialog.regionLabel')" min-width="200">
                    <template #default="{ row }">
                        <el-select v-model="row.regionId" clearable filterable
                            :placeholder="t('admissionsQuota.transferDialog.selectRegion')"
                            :disabled="row.role !== QuotaRole.ASM || !regionsReady" :loading="regionStore.loading">
                            <el-option v-for="opt in regionOptions" :key="opt.id" :label="opt.label" :value="opt.id" />
                        </el-select>
                    </template>
                </el-table-column>
                <el-table-column :label="t('admissionsQuota.transferDialog.startLabel')" width="140">
                    <template #default="{ row }">
                        <el-date-picker v-model="row.start" type="date" value-format="YYYY-MM-DD" format="YYYY-MM-DD"
                            style="width: 100%" />
                    </template>
                </el-table-column>
                <el-table-column :label="t('admissionsQuota.transferDialog.endLabel')" width="140">
                    <template #default="{ row }">
                        <el-date-picker v-model="row.end" type="date" value-format="YYYY-MM-DD" format="YYYY-MM-DD"
                            style="width: 100%" />
                    </template>
                </el-table-column>
                <el-table-column :label="t('admissionsQuota.transferDialog.baseQuotaLabel')" width="160" align="right">
                    <template #default="{ row }">
                        {{ formatCurrency(previewByKey.get(row.key)?.baseQuota ?? 0) }}
                    </template>
                </el-table-column>
                <el-table-column :label="t('admissionsQuota.transferDialog.workDaysLabel')" width="120" align="center">
                    <template #default="{ row }">
                        {{ previewByKey.get(row.key)?.workDays ?? 0 }}
                    </template>
                </el-table-column>
                <el-table-column :label="t('admissionsQuota.transferDialog.estimatedQuotaLabel')" width="160"
                    align="right">
                    <template #default="{ row }">
                        {{ formatCurrency(previewByKey.get(row.key)?.estQuota ?? 0) }}
                    </template>
                </el-table-column>
                <el-table-column :label="t('admissionsQuota.transferDialog.actionsLabel')" width="90">
                    <template #default="{ row }">
                        <el-button type="danger" text @click="removeSegment(row.key)">
                            {{ t('admissionsQuota.transferDialog.remove') }}
                        </el-button>
                    </template>
                </el-table-column>
            </el-table>

            <el-empty v-else :description="t('admissionsQuota.transferDialog.empty')" />

            <el-card v-if="segments.length" class="total-card">
                <div class="total-row">
                    <div>
                        <div class="total-label">{{ t('admissionsQuota.transferDialog.totalEstimated') }}</div>
                        <div class="total-value">{{ formatCurrency(previewTotal) }}</div>
                    </div>
                    <div class="total-meta">
                        <div>{{ t('admissionsQuota.transferDialog.monthWorkDays', { days: monthWorkingDays }) }}</div>
                        <div>{{ t('admissionsQuota.transferDialog.quotaData', { status: quotaDataStatus }) }}</div>
                    </div>
                </div>
                <div class="total-row total-row-secondary">
                    <div>
                        <div class="total-label">{{ t('admissionsQuota.transferDialog.totalAllocationAfter') }}</div>
                        <div class="total-value">{{ formatCurrency(allocationTotalAfter) }}</div>
                    </div>
                    <div class="total-meta" v-if="allocationTotalBefore > 0">
                        <div>{{ t('admissionsQuota.transferDialog.totalAllocationBefore') }}: {{
                            formatCurrency(allocationTotalBefore) }}</div>
                    </div>
                </div>
            </el-card>

            <el-card v-if="impactedCompanyRows.length || impactedRegionRows.length || impactedEmployeeRows.length"
                class="impact-card">
                <template #header>
                    <div class="card-title">{{ t('admissionsQuota.transferDialog.impactsTitle') }}</div>
                </template>
                <el-row :gutter="12">
                    <el-col :span="12" v-if="impactedCompanyRows.length">
                        <div class="card-title mb-2">{{ t('admissionsQuota.transferDialog.impactedCompanies') }}</div>
                        <el-table :data="impactedCompanyRows" size="small" border>
                            <el-table-column prop="companyName"
                                :label="t('admissionsQuota.transferDialog.companyColumn')" min-width="160" />
                            <el-table-column prop="regionName" :label="t('admissionsQuota.transferDialog.regionColumn')"
                                min-width="140" />
                            <el-table-column :label="t('admissionsQuota.transferDialog.before')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.before) }}
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('admissionsQuota.transferDialog.after')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.after) }}
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('admissionsQuota.transferDialog.delta')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.delta) }}
                                </template>
                            </el-table-column>
                        </el-table>
                    </el-col>
                    <el-col :span="12" v-if="impactedRegionRows.length">
                        <div class="card-title mb-2">{{ t('admissionsQuota.transferDialog.impactedRegions') }}</div>
                        <el-table :data="impactedRegionRows" size="small" border>
                            <el-table-column prop="regionName" :label="t('admissionsQuota.transferDialog.regionColumn')"
                                min-width="160" />
                            <el-table-column :label="t('admissionsQuota.transferDialog.before')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.before) }}
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('admissionsQuota.transferDialog.after')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.after) }}
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('admissionsQuota.transferDialog.delta')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.delta) }}
                                </template>
                            </el-table-column>
                        </el-table>
                    </el-col>
                    <el-col :span="24" v-if="impactedEmployeeRows.length">
                        <div class="card-title mt-4 mb-2">{{ t('admissionsQuota.transferDialog.impactedEmployees') }}
                        </div>
                        <el-table :data="impactedEmployeeRows" size="small" border>
                            <el-table-column prop="name" :label="t('admissionsQuota.transferDialog.employeeColumn')"
                                min-width="180" />
                            <el-table-column prop="role" :label="t('admissionsQuota.transferDialog.roleColumn')"
                                min-width="120" />
                            <el-table-column prop="companyName"
                                :label="t('admissionsQuota.transferDialog.companyColumn')" min-width="150" />
                            <el-table-column prop="regionName" :label="t('admissionsQuota.transferDialog.regionColumn')"
                                min-width="150" />
                            <el-table-column :label="t('admissionsQuota.transferDialog.before')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.before) }}
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('admissionsQuota.transferDialog.after')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.after) }}
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('admissionsQuota.transferDialog.delta')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.delta) }}
                                </template>
                            </el-table-column>
                        </el-table>
                    </el-col>
                </el-row>
            </el-card>

            <el-row v-if="companySummaryRows.length || regionSummaryRows.length" :gutter="12" class="summary-grid">
                <el-col :span="12" v-if="companySummaryRows.length">
                    <el-card>
                        <template #header>
                            <div class="card-title">{{ t('admissionsQuota.transferDialog.companyTotals') }}</div>
                        </template>
                        <el-table :data="companySummaryRows" size="small" border>
                            <el-table-column :label="t('admissionsQuota.transferDialog.companyColumn')"
                                prop="companyName" min-width="180" />
                            <el-table-column :label="t('admissionsQuota.transferDialog.regionColumn')" prop="regionName"
                                min-width="150" />
                            <el-table-column :label="t('admissionsQuota.transferDialog.plannedColumn')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.planned) }}
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('admissionsQuota.transferDialog.actualColumn')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.actual) }}
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('admissionsQuota.transferDialog.totalColumn')" align="right"
                                min-width="120">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.total) }}
                                </template>
                            </el-table-column>
                        </el-table>
                    </el-card>
                </el-col>
                <el-col :span="12" v-if="regionSummaryRows.length">
                    <el-card>
                        <template #header>
                            <div class="card-title">{{ t('admissionsQuota.transferDialog.regionTotals') }}</div>
                        </template>
                        <el-table :data="regionSummaryRows" size="small" border>
                            <el-table-column :label="t('admissionsQuota.transferDialog.regionColumn')" prop="regionName"
                                min-width="200" />
                            <el-table-column :label="t('admissionsQuota.transferDialog.totalColumn')" align="right"
                                min-width="140">
                                <template #default="{ row }">
                                    {{ formatCurrency(row.total) }}
                                </template>
                            </el-table-column>
                        </el-table>
                    </el-card>
                </el-col>
            </el-row>
        </div>
    </el-dialog>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import dayjs from 'dayjs'
import { AdmissionsQuotaApi, type TransferSegment, type AdmissionsQuotaModel } from '@/api/AdmissionsQuotaApi'
import { AdmissionsQuotaRegionApi } from '@/api/AdmissionsQuotaRegionApi'
import { useCompanyStore } from '@/stores/companyStore'
import { useEmployeeStore } from '@/stores/employeeStore'
import { useRegionStore } from '@/stores/regionStore'
import { useAdmissionsQuotaStore } from '@/stores/admissionsQuotaStore'
import { usePositionStore } from '@/stores/positionStore'
import { formatCurrency } from '@/utils/format'
import { countWorkingDaysBetween, countWorkingDaysInMonth, countWorkingDaysAccumulated, firstDayOfMonth, lastDayOfMonth, toDateLocal } from '@/utils/dateUtils'
import { QuotaRole } from '@/types'

const props = defineProps<{
    show: boolean
    quotaOptions: AdmissionsQuotaModel[] | null
}>()
const emit = defineEmits<{
    (e: 'update:show', v: boolean): void
    (e: 'applied'): void
}>()

const showDialog = computed({
    get: () => props.show,
    set: (val: boolean) => emit('update:show', val),
})
const { t } = useI18n()

const api = new AdmissionsQuotaApi()
const regionApi = new AdmissionsQuotaRegionApi()
const saving = ref(false)
const quotaLoading = ref(false)
const quotaDetail = ref<AdmissionsQuotaModel | null>(null)

const companyStore = useCompanyStore()
const regionStore = useRegionStore()
const employeeStore = useEmployeeStore()
const admissionsQuotaStore = useAdmissionsQuotaStore()
const positionStore = usePositionStore()

const selectedQuotaId = ref<string | undefined>(undefined)
const employeeId = ref<string>('')

type Seg = {
    key: string
    role: QuotaRole
    companyId?: string
    regionId?: string
    positionId?: string | null
    start: string
    end: string
    sourceId?: string | null
}
const segments = ref<Seg[]>([])
const baselineSegmentsSignature = ref('')
const baselineCompanyTotals = ref<Map<string, number>>(new Map())
const baselineRegionTotals = ref<Map<string, number>>(new Map())
const baselineEmployeeMap = ref<Map<string, any>>(new Map())

const errors = ref<string[]>([])

function resetDialogState() {
    errors.value = []
    selectedQuotaId.value = undefined
    employeeId.value = ''
    segments.value = []
    baselineSegmentsSignature.value = segmentsSignature([])
    baselineCompanyTotals.value = new Map()
    baselineRegionTotals.value = new Map()
    baselineEmployeeMap.value = new Map()
    quotaDetail.value = null
}

const quotaMap = computed<Record<string, AdmissionsQuotaModel>>(() => {
    const map: Record<string, AdmissionsQuotaModel> = {}
        ; (props.quotaOptions || []).forEach((q: AdmissionsQuotaModel) => {
            if (q.id) map[q.id] = q
        })
    return map
})
const quotaSelected = computed(() => selectedQuotaId.value ? quotaMap.value[selectedQuotaId.value] : undefined)
const quotaContext = computed(() => quotaDetail.value ?? quotaSelected.value)
const year = computed(() => quotaContext.value?.year ?? new Date().getFullYear())
const month = computed(() => quotaContext.value?.month ?? (new Date().getMonth() + 1))
const monthStartStr = computed(() =>
    dayjs(new Date(year.value, month.value - 1, 1)).format('YYYY-MM-DD')
)
const monthEndStr = computed(() =>
    dayjs(new Date(year.value, month.value, 0)).format('YYYY-MM-DD')
)

const employeesReady = computed(() => (employeeStore.employees || []).length > 0)
const companiesReady = computed(() => (companyStore.companies || []).length > 0)
const regionsReady = computed(() => (regionStore.regions || []).length > 0)
const canAddSegment = computed(() => !!selectedQuotaId.value && !!employeeId.value)

const employeeFilter = ref('')
const employeeOptionsBase = computed(() => {
    const list = (employeeStore.employees || []).filter(e => e.id)
    return list
        .map(e => ({
            id: e.id!,
            code: e.applicationUser?.userCode ?? '',
            name: e.applicationUser?.fullName || e.id!,
            label: `${e.applicationUser?.userCode ? `${e.applicationUser.userCode} - ` : ''}${e.applicationUser?.fullName || e.id!}${e.position?.positionName ? ` - ${e.position.positionName}` : ''}`,
            positionId: e.positionId ?? e.position?.id ?? null,
            positionName: e.position?.positionName ?? '',
            search: `${(e.applicationUser?.userCode ?? '')} ${(e.applicationUser?.fullName ?? '')}`.toLowerCase()
        }))
        .sort((a, b) => a.label.localeCompare(b.label, 'vi'))
})
const employeeOptions = computed(() => {
    const q = (employeeFilter.value || '').toLowerCase()
    if (!q) return employeeOptionsBase.value
    return employeeOptionsBase.value.filter(opt =>
        opt.label.toLowerCase().includes(q) ||
        opt.search.includes(q)
    )
})
const positionOptions = computed(() =>
    (positionStore.positions || []).map(p => ({
        id: p.id,
        label: p.positionName ?? p.id
    }))
)

const companyOptions = computed(() => {
    return (companyStore.companies || [])
        .filter(c => c.id)
        .map(c => ({
            id: c.id!,
            label: c.companyName ?? c.id!,
        }))
        .sort((a, b) => a.label.localeCompare(b.label, 'vi'))
})

const regionOptions = computed(() => {
    return (regionStore.regions || [])
        .filter(r => r.id)
        .map(r => ({
            id: r.id!,
            label: r.regionName ?? r.id!,
        }))
        .sort((a, b) => a.label.localeCompare(b.label, 'vi'))
})

const roleOptions: Array<{ value: QuotaRole; label: string; requiresCompany: boolean; requiresRegion: boolean }> = [
    { value: QuotaRole.Sale, label: 'admissionsQuota.transferDialog.roleSales', requiresCompany: true, requiresRegion: false },
    { value: QuotaRole.SalesLead, label: 'admissionsQuota.transferDialog.roleSalesLead', requiresCompany: true, requiresRegion: false },
    { value: QuotaRole.ProbationEmployee, label: 'admissionsQuota.transferDialog.roleProbation', requiresCompany: true, requiresRegion: false },
    { value: QuotaRole.LeavingEmployee, label: 'admissionsQuota.transferDialog.roleLeaving', requiresCompany: true, requiresRegion: false },
    { value: QuotaRole.Support, label: 'admissionsQuota.transferDialog.roleSupport', requiresCompany: true, requiresRegion: false },
    { value: QuotaRole.BM, label: 'admissionsQuota.transferDialog.roleBM', requiresCompany: true, requiresRegion: false },
    { value: QuotaRole.ASM, label: 'admissionsQuota.transferDialog.roleASM', requiresCompany: false, requiresRegion: true },
]

async function ensureReferenceData() {
    const tasks: Promise<void>[] = []
    if (!employeeStore.employees?.length) tasks.push(employeeStore.fetchAllEmployees())
    if (!companyStore.companies?.length) tasks.push(companyStore.fetchAllCompanies())
    if (!regionStore.regions?.length) tasks.push(regionStore.fetchAllRegions())
    if (!companyStore.LogRegionComs?.length) tasks.push(companyStore.fetchAllCompanyRegions())
    if (!positionStore.positions?.length) tasks.push(positionStore.fetchAllPositions())
    if (tasks.length) await Promise.all(tasks)
}

async function loadQuotaDetail(id?: string) {
    quotaDetail.value = null
    if (!id) return
    quotaLoading.value = true
    try {
        const res = await api.getAdmissionsQuotaById(id)
        let detail = res?.succeeded && res.data ? res.data : null
        const regionRes = await regionApi.getAdmissionsQuotaRegionByAdmissionsQuotaId(id)
        if (regionRes?.succeeded && detail) {
            detail = {
                ...detail,
                admissionsQuotaRegions: regionRes.data,
            }
        }
        if (detail) {
            quotaDetail.value = detail
            computeBaselines(detail)
        }
    } finally {
        quotaLoading.value = false
    }
}

function computeBaselines(detail: AdmissionsQuotaModel) {
    const companyMap = new Map<string, number>()
    const companyToRegion = new Map<string, string>()
    const regionMap = new Map<string, number>()
    const employeeMap = new Map<string, any>()

    // companies inside regions
    const regions = detail.admissionsQuotaRegions || []
    for (const reg of regions) {
        const regionId = reg.regionId ?? reg.region?.id ?? ''
        for (const comp of reg.admissionsQuotaCompanies || []) {
            const companyId = comp.companyId ?? comp.company?.id
            if (!companyId) continue
            const perSale = Number((comp as any).revenuePerSale ?? (comp as any).revenueQuotaPerSale ?? 0)
            const sales = Number((comp as any).numberOfSalesAllocated ?? (comp as any).numberOfSales ?? 0)
            const planned = sales * perSale
            const total = resolveTotalRevenue((comp as any).totalRevenue, planned)
            companyMap.set(companyId, Number(total.toFixed(2)))
            if (regionId) companyToRegion.set(companyId, regionId)
        }
    }
    for (const comp of detail.admissionsQuotaCompanies || []) {
        const companyId = comp.companyId ?? comp.company?.id
        if (!companyId || companyMap.has(companyId)) continue
        const perSale = Number((comp as any).revenuePerSale ?? (comp as any).revenueQuotaPerSale ?? 0)
        const sales = Number((comp as any).numberOfSalesAllocated ?? (comp as any).numberOfSales ?? 0)
        const planned = sales * perSale
        const total = resolveTotalRevenue((comp as any).totalRevenue, planned)
        companyMap.set(companyId, Number(total.toFixed(2)))
        const regionId = comp.admissionsQuotaRegion?.regionId ?? comp.admissionsQuotaRegion?.region?.id ?? ''
        if (regionId) companyToRegion.set(companyId, regionId)
    }

    // region totals: sum company totals first
    for (const [companyId, total] of companyMap.entries()) {
        const regionId = companyToRegion.get(companyId)
        if (!regionId) continue
        regionMap.set(regionId, (regionMap.get(regionId) ?? 0) + total)
    }
    // fallback from region total if no companies or to respect adjustments
    for (const reg of regions) {
        const regionId = reg.regionId ?? reg.region?.id ?? ''
        if (!regionId) continue
        const fromCompanies = regionMap.get(regionId) ?? 0
        const declared = Number(reg.totalRevenue ?? 0)
        const resolved = fromCompanies ? resolveTotalRevenue(declared, fromCompanies) : declared
        regionMap.set(regionId, Number(resolved.toFixed(2)))
    }

    // employees
    const { employeeRows } = buildRowsFromQuota(detail)
    const keyOf = (row: any) =>
        row?.id
            ? `id:${row.id}`
            : [
                row?.employeeId ?? '',
                row?.quotaRole ?? '',
                row?.companyId ?? '',
                row?.regionId ?? '',
                row?.allocationStartAt ?? '',
                row?.allocationEndAt ?? ''
            ].join('|')
    for (const row of employeeRows) {
        const key = keyOf(row)
        if (employeeMap.has(key) && !row?.id) continue
        employeeMap.set(key, row)
    }

    baselineCompanyTotals.value = companyMap
    baselineRegionTotals.value = regionMap
    baselineEmployeeMap.value = employeeMap
}

function collectQuotaEmployees(detail?: AdmissionsQuotaModel | null) {
    const list: any[] = []
    if (!detail) return list
    const byKey = new Map<string, any>()
    const pushUnique = (emp: any) => {
        const employeeId = emp?.employeeId ?? emp?.employee?.id ?? ''
        const companyId = emp?.companyId ?? emp?.company?.id ?? ''
        const regionId = emp?.regionId ?? emp?.region?.id ?? ''
        const role = emp?.quotaRole ?? ''
        const start = clampToMonth(
            emp?.allocationStartAt ?? emp?.joinedAt ?? emp?.joinDate ?? monthStartStr.value,
            monthStartStr.value,
            monthEndStr.value
        )
        const end = clampToMonth(
            emp?.allocationEndAt ?? emp?.endAt ?? emp?.endDate ?? monthEndStr.value,
            monthStartStr.value,
            monthEndStr.value
        )
        const key = [employeeId, role, companyId, regionId, start, end].join('|')
        const existing = byKey.get(key)
        if (existing?.id) {
            if (!emp?.id) return
        }
        byKey.set(key, emp)
    }
    if (Array.isArray(detail.admissionsQuotaEmployees)) {
        detail.admissionsQuotaEmployees.forEach(pushUnique)
    }
    for (const reg of detail.admissionsQuotaRegions || []) {
        if (Array.isArray((reg as any).admissionsQuotaEmployees)) {
            (reg as any).admissionsQuotaEmployees.forEach(pushUnique)
        }
        for (const comp of (reg as any).admissionsQuotaCompanies || []) {
            if (Array.isArray((comp as any).admissionsQuotaEmployees)) {
                (comp as any).admissionsQuotaEmployees.forEach(pushUnique)
            }
        }
    }
    list.push(...byKey.values())
    return list
}

function isSalesSegmentRole(role: QuotaRole) {
    return [
        QuotaRole.Sale,
        QuotaRole.SalesLead,
        QuotaRole.ProbationEmployee,
        QuotaRole.LeavingEmployee,
        QuotaRole.Support
    ].includes(role)
}
function mapQuotaRoleToSegmentRole(role: number | null | undefined): QuotaRole {
    const r = Number(role)
    if (Object.values(QuotaRole).includes(r as any)) return r as QuotaRole
    return QuotaRole.Sale
}
function mapSegmentRoleToQuotaRole(role: QuotaRole) {
    return role
}

function toYmd(input: any, fallback: string) {
    const d = toDateLocal(input)
    return dayjs(d ?? fallback).format('YYYY-MM-DD')
}

function clampToMonth(input: any, minStr: string, maxStr: string) {
    const d = toDateLocal(input)
    const min = toDateLocal(minStr)
    const max = toDateLocal(maxStr)
    if (!min || !max) return toYmd(d ?? minStr, minStr)
    if (!d) return toYmd(min, minStr)
    const value = d < min ? min : d > max ? max : d
    return dayjs(value).format('YYYY-MM-DD')
}
function segmentsSignature(list: Seg[]) {
    const normalized = (list || []).map(seg => ({
        role: seg.role,
        companyId: seg.companyId ?? '',
        regionId: seg.regionId ?? '',
        positionId: seg.positionId ?? '',
        start: seg.start,
        end: seg.end,
    }))
        .sort((a, b) =>
            [a.role, a.companyId, a.regionId, a.positionId, a.start, a.end]
                .join('|')
                .localeCompare([b.role, b.companyId, b.regionId, b.positionId, b.start, b.end].join('|'))
        )
    return JSON.stringify(normalized)
}

function loadEmployeeSegments() {
    if (!employeeId.value) {
        segments.value = []
        baselineSegmentsSignature.value = segmentsSignature([])
        return
    }
    const detail = quotaDetail.value ?? quotaSelected.value
    const allEmployees = collectQuotaEmployees(detail)
    const matched = allEmployees.filter(e => String(e?.employeeId ?? '') === String(employeeId.value))
    if (!matched.length) {
        segments.value = []
        baselineSegmentsSignature.value = segmentsSignature([])
        return
    }
    const next: Seg[] = []
    const seen = new Set<string>()
    for (const emp of matched) {
        const role = mapQuotaRoleToSegmentRole(emp?.quotaRole)
        let companyId = emp?.companyId ?? ''
        let regionId = emp?.regionId ?? ''
        const positionId = emp?.positionId ?? emp?.position?.id ?? null
        if (role === QuotaRole.ASM) {
            companyId = ''
        } else if (companyId && !regionId) {
            regionId = resolveRegionIdForCompany(companyId)
        }
        const start = clampToMonth(
            emp?.allocationStartAt ?? emp?.joinedAt ?? emp?.joinDate ?? monthStartStr.value,
            monthStartStr.value,
            monthEndStr.value
        )
        const end = clampToMonth(
            emp?.allocationEndAt ?? emp?.endAt ?? emp?.endDate ?? monthEndStr.value,
            monthStartStr.value,
            monthEndStr.value
        )
        const key = [role, companyId, regionId, positionId ?? '', start, end].join('|')
        if (seen.has(key)) continue
        seen.add(key)
        next.push({
            key: crypto.randomUUID(),
            role,
            companyId,
            regionId,
            positionId,
            start,
            end,
            sourceId: emp?.id ?? null,
        })
    }
    segments.value = next
    baselineSegmentsSignature.value = segmentsSignature(next)
}

onMounted(async () => {
    if (props.show) {
        resetDialogState()
        await ensureReferenceData()
        if (selectedQuotaId.value) {
            await loadQuotaDetail(selectedQuotaId.value)
            loadEmployeeSegments()
        }
    }
})
watch(() => props.show, async (val) => {
    if (val) {
        resetDialogState()
        await ensureReferenceData()
        if (selectedQuotaId.value) {
            await loadQuotaDetail(selectedQuotaId.value)
            loadEmployeeSegments()
        }
    }
})
watch(() => selectedQuotaId.value, async (val) => {
    errors.value = []
    await ensureReferenceData()
    await loadQuotaDetail(val)
    loadEmployeeSegments()
})
watch(() => employeeId.value, () => {
    errors.value = []
    loadEmployeeSegments()
})

function monthStartDate() { return firstDayOfMonth(year.value, month.value) }
function monthEndDate() { return lastDayOfMonth(year.value, month.value) }
function toYmdDate(date: Date) {
    return dayjs(date).format('YYYY-MM-DD')
}
function addDaysYmd(value: string, days: number) {
    const base = toDateLocal(value)
    if (!base) return value
    const next = new Date(base.getFullYear(), base.getMonth(), base.getDate() + days)
    return toYmdDate(next)
}

function isMappingActive(mapping: any, start: Date, end: Date) {
    const status = mapping?.status
    if (status !== undefined && status !== null && status !== 0) return false
    const started = toDateLocal(mapping?.startedDate)
    const ended = toDateLocal(mapping?.endDate)
    const startOk = !started || started <= end
    const endOk = !ended || ended >= start
    return startOk && endOk
}

const companyQuotaById = computed(() => {
    const map = new Map<string, { perSale: number; total: number; regionId?: string; numberOfSalesAllocated: number; hasAdjustment: boolean; currentSales: number }>()
    const detail = quotaDetail.value ?? quotaSelected.value
    const regions = detail?.admissionsQuotaRegions || []
    for (const reg of regions) {
        const regionId = reg.regionId ?? reg.region?.id ?? ''
        for (const comp of reg.admissionsQuotaCompanies || []) {
            const companyId = comp.companyId ?? comp.company?.id
            if (!companyId) continue
            const perSale = Number((comp as any).revenuePerSale ?? (comp as any).revenueQuotaPerSale ?? 0)
            const numberOfSalesAllocated = Number((comp as any).numberOfSalesAllocated ?? (comp as any).numberOfSales ?? 0)
            const total = Number((comp as any).totalRevenue ?? (comp as any).totalQuota ?? (perSale * numberOfSalesAllocated))
            const hasAdjustment = Array.isArray((comp as any).admissionsQuotaAdjustments) && (comp as any).admissionsQuotaAdjustments.length > 0
            const currentSales = Number((comp as any).currentSales ?? 0)
            map.set(companyId, { perSale, total, regionId, numberOfSalesAllocated, hasAdjustment, currentSales })
        }
    }
    for (const comp of detail?.admissionsQuotaCompanies || []) {
        const companyId = comp.companyId ?? comp.company?.id
        if (!companyId) continue
        if (map.has(companyId)) continue
        const perSale = Number((comp as any).revenuePerSale ?? (comp as any).revenueQuotaPerSale ?? 0)
        const numberOfSalesAllocated = Number((comp as any).numberOfSalesAllocated ?? (comp as any).numberOfSales ?? 0)
        const total = Number((comp as any).totalRevenue ?? (comp as any).totalQuota ?? (perSale * numberOfSalesAllocated))
        const hasAdjustment = Array.isArray((comp as any).admissionsQuotaAdjustments) && (comp as any).admissionsQuotaAdjustments.length > 0
        const currentSales = Number((comp as any).currentSales ?? 0)
        map.set(companyId, { perSale, total, regionId: comp.admissionsQuotaRegion?.regionId, numberOfSalesAllocated, hasAdjustment, currentSales })
    }
    return map
})

const regionQuotaById = computed(() => {
    const map = new Map<string, { total: number }>()
    const regions = (quotaDetail.value ?? quotaSelected.value)?.admissionsQuotaRegions || []
    for (const reg of regions) {
        const regionId = reg.regionId ?? reg.region?.id
        if (!regionId) continue
        const total = Number((reg as any).totalRevenue ?? (reg as any).totalQuota ?? 0)
        map.set(regionId, { total })
    }
    return map
})

function isSalesQuotaRole(role: any) {
    const value = typeof role === 'string' ? Number(role) : role
    return (
        value === QuotaRole.Sale ||
        value === QuotaRole.SalesLead ||
        value === QuotaRole.ProbationEmployee ||
        value === QuotaRole.LeavingEmployee
    )
}

function resolveRegionIdForCompany(companyId?: string) {
    if (!companyId) return ''
    const fromQuota = companyQuotaById.value.get(companyId)?.regionId
    if (fromQuota) return fromQuota
    const start = monthStartDate()
    const end = monthEndDate()
    const mapping = (companyStore.LogRegionComs || [])
        .filter((m: any) => m?.companyId === companyId && isMappingActive(m, start, end))
        .sort((a: any, b: any) => String(b?.startedDate ?? '').localeCompare(String(a?.startedDate ?? '')))[0]
    if (mapping?.regionId) return mapping.regionId
    const company = (companyStore.companies || []).find(c => c.id === companyId)
    return company?.regionId ?? ''
}

function findBaselineEmployeeRevenue(seg: Seg): number | undefined {
    const targetEmpId = employeeId.value
    if (!targetEmpId) return undefined
    for (const row of baselineEmployeeMap.value.values()) {
        if (String(row?.employeeId ?? '') !== String(targetEmpId)) continue
        if (Number(row?.quotaRole) !== Number(seg.role)) continue
        if (String(row?.companyId ?? '') !== String(seg.companyId ?? '')) continue
        if (String(row?.regionId ?? '') !== String(seg.regionId ?? '')) continue
        if (String(row?.allocationStartAt ?? row?.start ?? '') !== String(seg.start)) continue
        if (String(row?.allocationEndAt ?? row?.end ?? '') !== String(seg.end)) continue
        const value = row?.revenuePerSale ?? row?.revenueQuota
        const num = value === null || value === undefined || value === '' ? NaN : Number(value)
        if (!Number.isNaN(num)) return num
    }
    return undefined
}

function findBaselineEmployeeRow(seg: Seg): any | undefined {
    const targetEmpId = employeeId.value
    if (!targetEmpId) return undefined
    for (const row of baselineEmployeeMap.value.values()) {
        if (String(row?.employeeId ?? '') !== String(targetEmpId)) continue
        if (Number(row?.quotaRole) !== Number(seg.role)) continue
        if (String(row?.companyId ?? '') !== String(seg.companyId ?? '')) continue
        if (String(row?.regionId ?? '') !== String(seg.regionId ?? '')) continue
        if (String(row?.allocationStartAt ?? row?.start ?? '') !== String(seg.start)) continue
        if (String(row?.allocationEndAt ?? row?.end ?? '') !== String(seg.end)) continue
        return row
    }
    return undefined
}

function computeSegmentQuota(seg: Seg, workDays: number, monthDays: number) {
    const base = getBaseQuotaForSegment(seg)
    if (workDays < 7 || monthDays <= 0) return 0

    // Support không có chỉ tiêu cá nhân
    if (seg.role === QuotaRole.Support) return 0

    const isSalesLikeRole = seg.role === QuotaRole.Sale
        || seg.role === QuotaRole.SalesLead
        || seg.role === QuotaRole.ProbationEmployee
        || seg.role === QuotaRole.LeavingEmployee

    // BM/ASM s? d?ng c�ch chia theo ng�y c�ng don gi?n
    if (!isSalesLikeRole) {
        return (base / monthDays) * workDays
    }

    const emp = (employeeStore.employees || []).find(e => String(e.id) === String(employeeId.value))
    const joinDate = toDateLocal((emp as any)?.employeeStartedDate ?? (emp as any)?.joinedAt ?? null)
    const probationEnd = toDateLocal((emp as any)?.employeeNewEndDate ?? (emp as any)?.probationEnd ?? null)
    const monthStart = firstDayOfMonth(year.value, month.value)
    const monthEnd = lastDayOfMonth(year.value, month.value)
    const startedThisMonth = !!joinDate && joinDate >= monthStart && joinDate <= monthEnd
    const probationInMonth = !!probationEnd && probationEnd >= monthStart && (!joinDate || joinDate <= monthEnd)
    const isProbation = seg.role === QuotaRole.ProbationEmployee || startedThisMonth || probationInMonth

    if (isProbation) {
        const startDate = toDateLocal(seg.start)
        const beforeStart = startDate ? new Date(startDate.getFullYear(), startDate.getMonth(), startDate.getDate() - 1) : null
        // N?u role dang l� ProbationEmployee th� coi nhu D_prob = 0 trong th�ng n�y
        const dProb = seg.role === QuotaRole.ProbationEmployee
            ? 0
            : (joinDate && beforeStart ? countWorkingDaysAccumulated(joinDate, beforeStart) : 0)
        const d60 = Math.max(0, Math.min(workDays, 26 - dProb))
        const d100 = workDays - d60
        // Chu��n hoA� ho�8�c trA�n 26 ngA�y khi nhA)n s��� cA�n trong giai A�oan thA? viA?c (a������A�n theo vA� dA�u tham sA�)
        const stdDays = dProb < 26 ? Math.min(monthDays, 26) : monthDays
        return (stdDays > 0 ? base / stdDays : 0) * (0.6 * d60 + d100)
    }

    return (base / monthDays) * workDays
}

function getBaseQuotaForSegment(seg: Seg) {
    const baseline = findBaselineEmployeeRevenue(seg)
    if (baseline !== undefined) return Number(baseline)

    if (isSalesSegmentRole(seg.role)) {
        return Number(companyQuotaById.value.get(seg.companyId ?? '')?.perSale ?? 0)
    }
    if (seg.role === QuotaRole.BM) {
        const total = segmentsChanged.value
            ? companyTotalsById.value.get(seg.companyId ?? '')
            : companyTotalsCurrentById.value.get(seg.companyId ?? '')
        return Number(total ?? companyQuotaById.value.get(seg.companyId ?? '')?.total ?? 0)
    }
    if (seg.role === QuotaRole.ASM) {
        const total = segmentsChanged.value
            ? regionTotalsById.value.get(seg.regionId ?? '')
            : regionTotalsCurrentById.value.get(seg.regionId ?? '')
        return Number(total ?? regionQuotaById.value.get(seg.regionId ?? '')?.total ?? 0)
    }
    return 0
}

function workDaysBetween(start: string, end: string) {
    const s = toDateLocal(start)
    const e = toDateLocal(end)
    if (!s || !e) return 0
    return countWorkingDaysBetween(s, e)
}

function adjustWorkDaysForSegments(list: Seg[]) {
    if (!list.length) return []
    const ordered = [...list].sort((a, b) => (toDateLocal(a.start)?.getTime() ?? 0) - (toDateLocal(b.start)?.getTime() ?? 0))
    const raw = ordered.map(seg => workDaysBetween(seg.start, seg.end))
    const isContiguous = ordered.every((seg, idx) => {
        if (idx === 0) return true
        const expectedStart = addDaysYmd(ordered[idx - 1].end, 1)
        return seg.start === expectedStart
    })
    const coversMonth = ordered[0].start === monthStartStr.value && ordered[ordered.length - 1].end === monthEndStr.value
    if (isContiguous && coversMonth) {
        const totalRaw = raw.reduce((s, v) => s + v, 0)
        const totalMonth = monthWorkingDays.value
        const diff = totalRaw - totalMonth
        if (diff !== 0 && raw.length) {
            raw[raw.length - 1] = Math.max(0, raw[raw.length - 1] - diff)
        }
    }
    const map = new Map<string, number>()
    ordered.forEach((seg, idx) => map.set(seg.key, raw[idx]))
    return list.map(seg => map.get(seg.key) ?? workDaysBetween(seg.start, seg.end))
}

const monthWorkingDays = computed(() => countWorkingDaysInMonth(year.value, month.value))
const quotaDataStatus = computed(() =>
    quotaDetail.value ? t('admissionsQuota.transferDialog.quotaDataLoaded') : t('admissionsQuota.transferDialog.quotaDataMissing')
)
const allocationTotalBefore = computed(() =>
    Number((quotaDetail.value ?? quotaSelected.value)?.totalQuota ?? 0)
)

const segmentsChanged = computed(() =>
    baselineSegmentsSignature.value !== segmentsSignature(segments.value)
)

const baseEmployeesWithSegments = computed(() => {
    const detail = quotaDetail.value
    if (!detail) return []
    const { employeeRows } = buildRowsFromQuota(detail)
    if (!segmentsChanged.value) return employeeRows
    const withSegments = applySegmentsToEmployees(employeeRows)
    return adjustBmAllocations(withSegments)
})
const updatedEmployees = computed(() => {
    const withAdjustedBm = baseEmployeesWithSegments.value
    if (!segmentsChanged.value) return withAdjustedBm
    return updateManagerQuotas(withAdjustedBm)
})

const companyTouched = computed(() => {
    const set = new Set<string>()
    for (const seg of segments.value) {
        if (seg.companyId) set.add(seg.companyId)
    }
    for (const id of selectedEmployeeCompanyIds.value) set.add(id)
    return set
})

const regionTouched = computed(() => {
    const set = new Set<string>()
    for (const seg of segments.value) {
        if (seg.regionId) set.add(seg.regionId)
        if (seg.companyId) {
            const rid = resolveRegionIdForCompany(seg.companyId)
            if (rid) set.add(rid)
        }
    }
    for (const companyId of companyTouched.value) {
        const rid = resolveRegionIdForCompany(companyId)
        if (rid) set.add(rid)
    }
    return set
})

const selectedEmployeeCompanyIds = computed(() => {
    if (!employeeId.value) return []
    const detail = quotaDetail.value ?? quotaSelected.value
    const allEmployees = collectQuotaEmployees(detail)
    const ids = new Set<string>()
    for (const emp of allEmployees) {
        if (String(emp?.employeeId ?? '') !== String(employeeId.value)) continue
        if (emp?.companyId) ids.add(emp.companyId)
    }
    return Array.from(ids)
})

const existingSalesQuotaByCompany = computed(() => {
    const map = new Map<string, number>()
    for (const emp of baseEmployeesWithSegments.value) {
        if (!isSalesQuotaRole(emp?.quotaRole)) continue
        const companyId = emp?.companyId
        if (!companyId) continue
        const current = map.get(companyId) ?? 0
        map.set(companyId, current + Number(emp?.revenuePerSale || 0))
    }
    return map
})
const companyTotalsCurrentById = computed(() => {
    const map = new Map<string, number>()
    for (const [id, meta] of companyQuotaById.value.entries()) {
        const planned = Number(meta?.numberOfSalesAllocated ?? 0) * Number(meta?.perSale ?? 0)
        const total = resolveTotalRevenue(meta?.total, planned)
        map.set(id, Number(total.toFixed(2)))
    }
    return map
})

const companyTotalsById = computed(() => {
    if (!segmentsChanged.value) {
        return new Map<string, number>(companyTotalsCurrentById.value)
    }
    const map = new Map<string, number>()
    const allCompanyIds = new Set<string>()
    for (const id of companyQuotaById.value.keys()) allCompanyIds.add(id)
    for (const id of existingSalesQuotaByCompany.value.keys()) allCompanyIds.add(id)

    for (const id of allCompanyIds) {
        const meta = companyQuotaById.value.get(id)
        const planned = Number(meta?.numberOfSalesAllocated ?? 0) * Number(meta?.perSale ?? 0)
        const actual = existingSalesQuotaByCompany.value.get(id) ?? 0
        let computed = Math.max(planned, actual)
        // Hỗ trợ (Support): nếu chi nhánh đủ sale, mỗi support cộng 50% chỉ tiêu EC (perSale)
        const supportCount = segments.value.filter(s => s.role === QuotaRole.Support && s.companyId === id).length
        const companyIsFull = Number(meta?.currentSales ?? 0) >= Number(meta?.numberOfSalesAllocated ?? 0)
        if (supportCount > 0 && companyIsFull) {
            computed += supportCount * 0.5 * Number(meta?.perSale ?? 0)
        }
        const total = meta?.hasAdjustment
            ? resolveTotalRevenue(meta?.total, computed)
            : computed
        map.set(id, Number(total.toFixed(2)))
    }
    return map
})

const allocationTotalAfter = computed(() => {
    const total = Array.from(companyTotalsById.value.values()).reduce((sum, value) => sum + value, 0)
    return Number(total.toFixed(2))
})

const companyChangedById = computed(() => {
    const changed = new Set<string>()
    const all = new Set<string>([...companyTotalsById.value.keys(), ...companyTotalsCurrentById.value.keys()])
    for (const id of all) {
        const before = companyTotalsCurrentById.value.get(id) ?? 0
        const after = companyTotalsById.value.get(id) ?? before
        if (Math.abs(after - before) > 0.01) changed.add(id)
    }
    return changed
})

const regionChangedById = computed(() => {
    const changed = new Set<string>()
    const all = new Set<string>([...regionTotalsById.value.keys(), ...regionTotalsCurrentById.value.keys()])
    for (const id of all) {
        const before = regionTotalsCurrentById.value.get(id) ?? 0
        const after = regionTotalsById.value.get(id) ?? before
        if (Math.abs(after - before) > 0.01) changed.add(id)
    }
    return changed
})

const regionTotalsCurrentById = computed(() => {
    const map = new Map<string, number>()
    for (const [companyId, total] of companyTotalsCurrentById.value.entries()) {
        const regionId = companyQuotaById.value.get(companyId)?.regionId ?? resolveRegionIdForCompany(companyId)
        if (!regionId) continue
        map.set(regionId, (map.get(regionId) ?? 0) + total)
    }
    return map
})

const regionTotalsById = computed(() => {
    const map = new Map<string, number>()
    for (const [companyId, total] of companyTotalsById.value.entries()) {
        const regionId = companyQuotaById.value.get(companyId)?.regionId ?? resolveRegionIdForCompany(companyId)
        if (!regionId) continue
        map.set(regionId, (map.get(regionId) ?? 0) + total)
    }
    return map
})

const previewRows = computed(() => {
    const wdMonth = monthWorkingDays.value
    const adjustedWorkDays = adjustWorkDaysForSegments(segments.value)
    return segments.value.map((seg, idx) => {
        const workDays = adjustedWorkDays[idx] ?? workDaysBetween(seg.start, seg.end)
        const baseQuota = getBaseQuotaForSegment(seg)
        const baselineRow = findBaselineEmployeeRow(seg)
        const baselineValRaw = baselineRow?.revenuePerSale ?? findBaselineEmployeeRevenue(seg)
        const ignoreBaseline =
            (seg.role === QuotaRole.BM && seg.companyId && companyChangedById.value.has(seg.companyId)) ||
            (seg.role === QuotaRole.ASM && seg.regionId && regionChangedById.value.has(seg.regionId))
        const baselineVal = ignoreBaseline ? undefined : baselineValRaw
        const estQuota = workDays < 7
            ? 0
            : (baselineVal !== undefined
                ? Number(baselineVal)
                : computeSegmentQuota(seg, workDays, wdMonth))
        return {
            key: seg.key,
            role: seg.role,
            companyId: seg.companyId,
            regionId: seg.regionId,
            start: seg.start,
            end: seg.end,
            workDays,
            baseQuota: Number(baseQuota.toFixed(2)),
            estQuota: Number(estQuota.toFixed(2)),
        }
    })
})
const previewTotal = computed(() =>
    Number(previewRows.value.reduce((sum, row) => sum + row.estQuota, 0).toFixed(2))
)
const previewByKey = computed(() => {
    const map = new Map<string, any>()
    for (const row of previewRows.value) map.set(row.key, row)
    return map
})

const companySummaryRows = computed(() => {
    const impacted = new Set<string>()
    for (const seg of segments.value) {
        if (seg.companyId) impacted.add(seg.companyId)
    }
    for (const id of selectedEmployeeCompanyIds.value) impacted.add(id)
    const rows: Array<{
        companyId: string
        companyName: string
        regionId: string
        regionName: string
        planned: number
        actual: number
        total: number
    }> = []
    for (const companyId of impacted) {
        const meta = companyQuotaById.value.get(companyId)
        const planned = Number(meta?.numberOfSalesAllocated ?? 0) * Number(meta?.perSale ?? 0)
        const actual = existingSalesQuotaByCompany.value.get(companyId) ?? 0
        const total = companyTotalsById.value.get(companyId) ?? Math.max(planned, actual)
        const company = (companyStore.companies || []).find(c => c.id === companyId)
        const regionId = meta?.regionId ?? resolveRegionIdForCompany(companyId)
        const region = (regionStore.regions || []).find(r => r.id === regionId)
        rows.push({
            companyId,
            companyName: company?.companyName ?? companyId,
            regionId: regionId ?? '',
            regionName: region?.regionName ?? regionId ?? '',
            planned: Number(planned.toFixed(2)),
            actual: Number(actual.toFixed(2)),
            total: Number(total.toFixed(2)),
        })
    }
    return rows.sort((a, b) => a.companyName.localeCompare(b.companyName, 'vi'))
})

const regionSummaryRows = computed(() => {
    const impacted = new Set<string>()
    for (const row of companySummaryRows.value) {
        if (row.regionId) impacted.add(row.regionId)
    }
    for (const seg of segments.value) {
        if (seg.regionId) impacted.add(seg.regionId)
    }
    const rows: Array<{ regionId: string; regionName: string; total: number }> = []
    for (const regionId of impacted) {
        const total = regionTotalsById.value.get(regionId) ?? regionQuotaById.value.get(regionId)?.total ?? 0
        const region = (regionStore.regions || []).find(r => r.id === regionId)
        rows.push({
            regionId,
            regionName: region?.regionName ?? regionId,
            total: Number(total.toFixed(2)),
        })
    }
    return rows.sort((a, b) => a.regionName.localeCompare(b.regionName, 'vi'))
})
const impactedCompanyRows = computed(() => {
    const rows: Array<{ companyId: string; companyName: string; regionName: string; before: number; after: number; delta: number }> = []
    const allIds = new Set<string>()
    for (const id of companyTotalsById.value.keys()) allIds.add(id)
    for (const id of companyTotalsCurrentById.value.keys()) allIds.add(id)
    for (const id of allIds) {
        const before = baselineCompanyTotals.value.get(id) ?? 0
        const after = companyTotalsById.value.get(id) ?? before
        const company = (companyStore.companies || []).find(c => c.id === id)
        const regionId = companyQuotaById.value.get(id)?.regionId ?? resolveRegionIdForCompany(id)
        const region = (regionStore.regions || []).find(r => r.id === regionId)
        rows.push({
            companyId: id,
            companyName: company?.companyName ?? id,
            regionName: region?.regionName ?? regionId ?? '',
            before: Number(before.toFixed(2)),
            after: Number(after.toFixed(2)),
            delta: Number((after - before).toFixed(2)),
        })
    }
    return rows.sort((a, b) => a.companyName.localeCompare(b.companyName, 'vi'))
})
const impactedRegionRows = computed(() => {
    const rows: Array<{ regionId: string; regionName: string; before: number; after: number; delta: number }> = []
    const allIds = new Set<string>()
    for (const id of regionTotalsById.value.keys()) allIds.add(id)
    for (const id of regionTotalsCurrentById.value.keys()) allIds.add(id)
    for (const id of regionTouched.value) allIds.add(id)
    for (const id of allIds) {
        const before = baselineRegionTotals.value.get(id) ?? regionTotalsCurrentById.value.get(id) ?? 0
        const after = regionTotalsById.value.get(id) ?? before
        const region = (regionStore.regions || []).find(r => r.id === id)
        rows.push({
            regionId: id,
            regionName: region?.regionName ?? id,
            before: Number(before.toFixed(2)),
            after: Number(after.toFixed(2)),
            delta: Number((after - before).toFixed(2)),
        })
    }
    return rows.sort((a, b) => a.regionName.localeCompare(b.regionName, 'vi'))
})
const impactedEmployeeRows = computed(() => {
    const detail = quotaDetail.value
    if (!detail) return []
    if (!segmentsChanged.value) return [] // chua thay d?i th� kh�ng c� ?nh hu?ng
    const { employeeRows } = buildRowsFromQuota(detail)
    const afterEmployees = updatedEmployees.value
    const keyOf = (row: any) =>
        row?.id
            ? `id:${row.id}`
            : [
                row?.employeeId ?? '',
                row?.quotaRole ?? '',
                row?.companyId ?? '',
                row?.regionId ?? '',
                row?.allocationStartAt ?? '',
                row?.allocationEndAt ?? ''
            ].join('|')
    const beforeMap = new Map<string, any>(baselineEmployeeMap.value)
    for (const row of employeeRows) {
        const key = keyOf(row)
        if (beforeMap.has(key) && !row?.id) continue
        if (!beforeMap.has(key)) beforeMap.set(key, row)
    }
    const afterMap = new Map<string, any>()
    for (const row of afterEmployees) afterMap.set(keyOf(row), row)

    const roleLabel = (role: any) => {
        switch (Number(role)) {
            case QuotaRole.ASM: return t('admissionsQuota.transferDialog.roleASM')
            case QuotaRole.BM: return t('admissionsQuota.transferDialog.roleBM')
            case QuotaRole.SalesLead: return t('admissionsQuota.transferDialog.roleSales')
            case QuotaRole.Sale: return t('admissionsQuota.transferDialog.roleSales')
            case QuotaRole.ProbationEmployee: return t('admissionsQuota.transferDialog.roleSales')
            case QuotaRole.LeavingEmployee: return t('admissionsQuota.transferDialog.roleSales')
            default: return t('admissionsQuota.transferDialog.roleSales')
        }
    }

    const allKeys = new Set<string>([...beforeMap.keys(), ...afterMap.keys()])
    const impactedCompanyIds = new Set<string>()
    for (const [id, val] of companyTotalsById.value.entries()) {
        const before = companyTotalsCurrentById.value.get(id) ?? 0
        if (Math.abs(val - before) > 0.01) impactedCompanyIds.add(id)
    }
    const impactedRegionIds = new Set<string>()
    for (const [id, val] of regionTotalsById.value.entries()) {
        const before = regionTotalsCurrentById.value.get(id) ?? 0
        if (Math.abs(val - before) > 0.01) impactedRegionIds.add(id)
    }
    for (const id of regionTouched.value) impactedRegionIds.add(id)

    const rows: Array<{ key: string; name: string; role: string; companyName: string; regionName: string; before: number; after: number; delta: number }> = []
    for (const key of allKeys) {
        const before = beforeMap.get(key)
        const after = afterMap.get(key)
        const beforeVal = Number(before?.revenuePerSale ?? 0)
        const afterVal = Number(after?.revenuePerSale ?? 0)

        const empId = (after ?? before)?.employeeId
        const emp = (employeeStore.employees || []).find(e => e.id === empId)
        const companyId = (after ?? before)?.companyId ?? ''
        const regionId = (after ?? before)?.regionId ?? ''
        const company = (companyStore.companies || []).find(c => c.id === companyId)
        const region = (regionStore.regions || []).find(r => r.id === regionId)

        const isManagerImpacted =
            (Number((after ?? before)?.quotaRole) === QuotaRole.BM && (impactedCompanyIds.has(companyId) || companyTouched.value.has(companyId))) ||
            (Number((after ?? before)?.quotaRole) === QuotaRole.ASM && impactedRegionIds.has(regionId))
        if (Math.abs(afterVal - beforeVal) < 0.01 && !isManagerImpacted) continue

        // ensure BM/ASM rows show adjusted value even if delta was tiny
        const finalAfter = isManagerImpacted ? afterVal : afterVal
        const finalBefore = isManagerImpacted ? beforeVal : beforeVal

        rows.push({
            key,
            name: emp?.applicationUser?.fullName ?? empId ?? '',
            role: roleLabel((after ?? before)?.quotaRole),
            companyName: company?.companyName ?? companyId,
            regionName: region?.regionName ?? regionId,
            before: Number(finalBefore.toFixed(2)),
            after: Number(finalAfter.toFixed(2)),
            delta: Number((finalAfter - finalBefore).toFixed(2)),
        })
    }
    return rows.sort((a, b) => a.name.localeCompare(b.name, 'vi'))
})

watch([() => companyStore.LogRegionComs, () => quotaDetail.value, year, month], () => {
    for (const seg of segments.value) {
        if ((seg.role === QuotaRole.Sale || seg.role === QuotaRole.SalesLead || seg.role === QuotaRole.ProbationEmployee || seg.role === QuotaRole.LeavingEmployee || seg.role === QuotaRole.Support || seg.role === QuotaRole.BM) && seg.companyId) {
            const regionId = resolveRegionIdForCompany(seg.companyId)
            if (regionId && seg.regionId !== regionId) seg.regionId = regionId
        }
    }
}, { deep: true })

function close() { showDialog.value = false }

function addSegment(role: QuotaRole = QuotaRole.Sale) {
    if (!selectedQuotaId.value || !employeeId.value) {
        errors.value = [
            t('admissionsQuota.transferDialog.validation.selectQuota'),
            t('admissionsQuota.transferDialog.validation.selectEmployee')
        ]
        return
    }
    employeeFilter.value = ''
    const monthStart = monthStartStr.value
    const monthEnd = monthEndStr.value
    let start = monthStart
    let end = monthEnd
    if (segments.value.length) {
        const ordered = [...segments.value].sort((a, b) => {
            const da = toDateLocal(a.end)?.getTime() ?? 0
            const db = toDateLocal(b.end)?.getTime() ?? 0
            return da - db
        })
        const last = ordered[ordered.length - 1]
        const lastStart = last.start
        const lastEnd = last.end
        if (toDateLocal(lastEnd) && toDateLocal(monthEnd) && toDateLocal(lastEnd)! >= toDateLocal(monthEnd)!) {
            if (toDateLocal(lastStart) && toDateLocal(lastStart)! < toDateLocal(monthEnd)!) {
                last.end = addDaysYmd(monthEnd, -1)
            }
        }
        start = addDaysYmd(last.end, 1)
        end = monthEnd
    }
    const seg: Seg = { key: crypto.randomUUID(), role, start, end, companyId: '', regionId: '', sourceId: null }
    segments.value.push(seg)
}

function filterEmployeeOption(query: string, option: any) {
    employeeFilter.value = query || ''
    return true
}

function removeSegment(key: string) {
    segments.value = segments.value.filter(s => s.key !== key)
}

function onRoleChange(seg: Seg) {
    if (seg.role === QuotaRole.ASM) {
        seg.companyId = ''
    } else if (seg.companyId) {
        const regionId = resolveRegionIdForCompany(seg.companyId)
        if (regionId) seg.regionId = regionId
    }
    errors.value = []
}

function onCompanyChange(seg: Seg) {
    if (!seg.companyId) {
        seg.regionId = ''
        return
    }
    const regionId = resolveRegionIdForCompany(seg.companyId)
    if (regionId) seg.regionId = regionId
    errors.value = []
}

function validate(showErrors = true): boolean {
    const list: string[] = []
    if (!selectedQuotaId.value) list.push(t('admissionsQuota.transferDialog.validation.selectQuota'))
    if (!employeeId.value) list.push(t('admissionsQuota.transferDialog.validation.selectEmployee'))
    if (segments.value.length === 0) list.push(t('admissionsQuota.transferDialog.validation.addSegment'))

    const ms = dayjs(monthStartDate())
    const monthStart = monthStartStr.value
    const monthEnd = monthEndStr.value
    const rs = segments.value.map((s, i) => ({
        i,
        role: s.role,
        a: dayjs(s.start),
        b: dayjs(s.end),
        start: s.start,
        end: s.end,
        hasCompany: !!s.companyId,
        hasRegion: !!s.regionId,
    })).sort((x, y) => x.a.valueOf() - y.a.valueOf())

    rs.forEach((r, idx) => {
        if (!r.a.isSame(ms, 'month') || !r.b.isSame(ms, 'month')) {
            list.push(t('admissionsQuota.transferDialog.validation.segmentInMonth', { index: idx + 1 }))
        }
        if (r.a.isAfter(r.b)) {
            list.push(t('admissionsQuota.transferDialog.validation.segmentStartAfterEnd', { index: idx + 1 }))
        }
        if (r.role === QuotaRole.ASM && !r.hasRegion) {
            list.push(t('admissionsQuota.transferDialog.validation.segmentRegionRequired', { index: idx + 1 }))
        }
        if ((r.role === QuotaRole.BM || isSalesSegmentRole(r.role)) && !r.hasCompany) {
            list.push(t('admissionsQuota.transferDialog.validation.segmentCompanyRequired', { index: idx + 1, role: r.role }))
        }
    })
    for (let i = 1; i < rs.length; i++) {
        if (rs[i].a.isBefore(rs[i - 1].b)) {
            list.push(t('admissionsQuota.transferDialog.validation.segmentOverlap', { index: rs[i].i + 1, prevIndex: rs[i - 1].i + 1 }))
        }
    }
    if (rs.length) {
        const firstRequiresMonthStart = rs[0].role !== QuotaRole.ProbationEmployee
        if (firstRequiresMonthStart && rs[0].start !== monthStart) {
            list.push(t('admissionsQuota.transferDialog.validation.firstSegmentStart'))
        }
        if (rs[rs.length - 1].end !== monthEnd) list.push(t('admissionsQuota.transferDialog.validation.lastSegmentEnd'))
        for (let i = 1; i < rs.length; i++) {
            const expectedStart = addDaysYmd(rs[i - 1].end, 1)
            if (rs[i].start !== expectedStart) {
                list.push(t('admissionsQuota.transferDialog.validation.segmentStartNextDay', { index: rs[i].i + 1 }))
            }
        }
    }

    if (showErrors) errors.value = list
    return list.length === 0
}

function previewValidate() {
    validate(true)
}

function resolveTotalRevenue(raw: any, fallback: number) {
    const value = raw === '' || raw === null || raw === undefined ? NaN : Number(raw)
    return Number.isNaN(value) ? fallback : value
}
function toDateOnlyString(date: Date | null) {
    if (!date) return null
    const y = date.getFullYear()
    const m = String(date.getMonth() + 1).padStart(2, '0')
    const d = String(date.getDate()).padStart(2, '0')
    return `${y}-${m}-${d}`
}
function clampAllocationRange(
    y: number,
    m: number,
    startInput?: any,
    endInput?: any
) {
    const monthStart = firstDayOfMonth(y, m)
    const monthEnd = lastDayOfMonth(y, m)
    const startDate = toDateLocal(startInput) ?? monthStart
    const endDate = toDateLocal(endInput) ?? monthEnd
    const allocationStartAt = startDate < monthStart ? monthStart : startDate > monthEnd ? monthEnd : startDate
    const allocationEndAt = endDate < monthStart ? monthStart : endDate > monthEnd ? monthEnd : endDate
    if (allocationEndAt < allocationStartAt) {
        return { allocationStartAt: null, allocationEndAt: null }
    }
    return {
        allocationStartAt: toDateOnlyString(allocationStartAt),
        allocationEndAt: toDateOnlyString(allocationEndAt),
    }
}
function segmentKey(role: QuotaRole, companyId?: string, regionId?: string, start?: string, end?: string) {
    return [role, companyId ?? '', regionId ?? '', start ?? '', end ?? ''].join('|')
}
function segmentGroupKey(role: QuotaRole, companyId?: string, regionId?: string) {
    return [role, companyId ?? '', regionId ?? ''].join('|')
}
function existingGroupKey(row: any) {
    const role = mapQuotaRoleToSegmentRole(row?.quotaRole)
    const companyId = role === QuotaRole.ASM ? '' : (row?.companyId ?? '')
    const regionId = role === QuotaRole.ASM
        ? (row?.regionId ?? '')
        : (row?.regionId ?? resolveRegionIdForCompany(companyId))
    return segmentGroupKey(role, companyId, regionId)
}
function buildRowsFromQuota(detail: AdmissionsQuotaModel) {
    const regionRows: any[] = []
    const companyRows: any[] = []
    const employeeRows: any[] = []
    const employeeByKey = new Map<string, any>()
    const companyIds = new Set<string>()

    const pushEmployee = (emp: any, region: any, company?: any) => {
        const regionId = emp?.regionId ?? region?.regionId ?? region?.region?.id ?? ''
        const companyId = emp?.companyId ?? company?.companyId ?? company?.company?.id ?? ''
        const allocationStartAt = clampToMonth(
            emp?.allocationStartAt ?? emp?.joinedAt ?? emp?.joinDate ?? monthStartStr.value,
            monthStartStr.value,
            monthEndStr.value
        )
        const allocationEndAt = clampToMonth(
            emp?.allocationEndAt ?? emp?.endAt ?? emp?.endDate ?? monthEndStr.value,
            monthStartStr.value,
            monthEndStr.value
        )
        const naturalKey = [
            emp?.employeeId ?? '',
            emp?.quotaRole ?? '',
            companyId,
            regionId,
            allocationStartAt ?? '',
            allocationEndAt ?? ''
        ].join('|')
        const existing = employeeByKey.get(naturalKey)
        if (existing?.id && !emp?.id) return
        const row = {
            id: emp?.id ?? null,
            employeeId: emp?.employeeId ?? null,
            companyId: companyId || null,
            regionId: regionId || null,
            positionId: emp?.positionId ?? emp?.position?.id ?? null,
            quotaRole: emp?.quotaRole ?? QuotaRole.ASM,
            workStage: emp?.workStage ?? null,
            revenuePerSale: Number(emp?.revenuePerSale ?? emp?.revenueQuota ?? 0),
            joinedAt: emp?.joinedAt ?? emp?.joinDate ?? emp?.employee?.employeeStartedDate ?? null,
            endAt: emp?.endAt ?? emp?.endDate ?? emp?.employee?.employeeEndDate ?? null,
            allocationStartAt,
            allocationEndAt,
            probationEnd: emp?.probationEnd ?? emp?.employee?.employeeNewEndDate ?? null,
            note: emp?.note ?? ''
        }
        employeeByKey.set(naturalKey, row)
    }

    for (const reg of detail.admissionsQuotaRegions || []) {
        const regionId = reg.regionId ?? reg.region?.id ?? ''
        regionRows.push({
            id: reg.id ?? null,
            regionId,
            companyCount: Number(reg.companyCount ?? (reg.admissionsQuotaCompanies?.length ?? 0)),
            currentSales: Number(reg.currentSales ?? 0),
            numberOfSalesAllocated: Number(reg.numberOfSalesAllocated ?? 0),
            revenuePerSale: Number(reg.revenuePerSale ?? 0),
            totalRevenue: Number(reg.totalRevenue ?? 0),
            admissionsQuotaAdjustments: reg.admissionsQuotaAdjustments ?? [],
            note: (reg as any).note ?? ''
        })

        for (const comp of reg.admissionsQuotaCompanies || []) {
            const companyId = comp.companyId ?? comp.company?.id
            if (companyId) companyIds.add(companyId)
            companyRows.push({
                id: comp.id ?? null,
                admissionsQuotaRegionId: reg.id ?? null,
                regionId,
                companyId: companyId ?? '',
                currentSales: Number(comp.currentSales ?? 0),
                numberOfSalesAllocated: Number(comp.numberOfSalesAllocated ?? 0),
                numberOfPartTimeSales: Number(comp.numberOfPartTimeSales ?? 0),
                revenuePerSale: Number((comp as any).revenuePerSale ?? (comp as any).revenueQuotaPerSale ?? 0),
                totalRevenue: Number((comp as any).totalRevenue ?? (comp as any).totalQuota ?? 0),
                admissionsQuotaAdjustments: comp.admissionsQuotaAdjustments ?? [],
                note: (comp as any).note ?? ''
            })
            for (const emp of comp.admissionsQuotaEmployees || []) {
                pushEmployee(emp, reg, comp)
            }
        }
        for (const emp of (reg as any).admissionsQuotaEmployees || []) {
            pushEmployee(emp, reg)
        }
    }
    for (const comp of detail.admissionsQuotaCompanies || []) {
        const companyId = comp.companyId ?? comp.company?.id
        if (!companyId || companyIds.has(companyId)) continue
        const regionId = comp.admissionsQuotaRegion?.regionId ?? ''
        companyIds.add(companyId)
        companyRows.push({
            id: comp.id ?? null,
            admissionsQuotaRegionId: comp.admissionsQuotaRegionId ?? null,
            regionId,
            companyId,
            currentSales: Number(comp.currentSales ?? 0),
            numberOfSalesAllocated: Number((comp as any).numberOfSalesAllocated ?? (comp as any).numberOfSales ?? 0),
            numberOfPartTimeSales: Number(comp.numberOfPartTimeSales ?? 0),
            revenuePerSale: Number((comp as any).revenuePerSale ?? (comp as any).revenueQuotaPerSale ?? 0),
            totalRevenue: Number((comp as any).totalRevenue ?? (comp as any).totalQuota ?? 0),
            admissionsQuotaAdjustments: comp.admissionsQuotaAdjustments ?? [],
            note: (comp as any).note ?? ''
        })
        for (const emp of comp.admissionsQuotaEmployees || []) {
            pushEmployee(emp, {}, comp)
        }
    }
    for (const emp of detail.admissionsQuotaEmployees || []) {
        pushEmployee(emp, {})
    }

    employeeRows.push(...employeeByKey.values())

    const regionIds = new Set<string>()
    for (const row of companyRows) {
        if (row.regionId) regionIds.add(row.regionId)
    }
    for (const row of employeeRows) {
        if (row.regionId) regionIds.add(row.regionId)
    }
    for (const regionId of regionIds) {
        if (regionRows.some(r => r.regionId === regionId)) continue
        const region = (regionStore.regions || []).find(r => r.id === regionId)
        regionRows.push({
            id: null,
            regionId,
            companyCount: 0,
            currentSales: 0,
            numberOfSalesAllocated: 0,
            revenuePerSale: 0,
            totalRevenue: 0,
            admissionsQuotaAdjustments: [],
            note: '',
            regionName: region?.regionName ?? ''
        })
    }
    const existingByRegionId = new Map(regionRows.map(r => [r.regionId, r]))
    const grouped = new Map<string, any[]>()
    for (const comp of companyRows) {
        if (!comp.regionId) continue
        if (!grouped.has(comp.regionId)) grouped.set(comp.regionId, [])
        grouped.get(comp.regionId)!.push(comp)
    }
    for (const [regionId, comps] of grouped.entries()) {
        const existing = existingByRegionId.get(regionId)
        const numberOfSalesAllocated = comps.reduce((s, x) => s + Number(x.numberOfSalesAllocated || 0), 0)
        const totalRevenue = comps.reduce((s, x) => {
            const planned = Number(x.numberOfSalesAllocated || 0) * Number(x.revenuePerSale || 0)
            return s + resolveTotalRevenue(x.totalRevenue, planned)
        }, 0)
        const currentSales = comps.reduce((s, x) => s + Number(x.currentSales || 0), 0)
        const revenuePerSale = numberOfSalesAllocated > 0 ? Math.round(totalRevenue / numberOfSalesAllocated) : 0
        const next = {
            regionId,
            regionName: existing?.regionName ?? '',
            companyCount: comps.length,
            currentSales,
            numberOfSalesAllocated,
            revenuePerSale,
            totalRevenue
        }
        if (existing) Object.assign(existing, next)
        else regionRows.push(next)
    }

    return { regionRows, companyRows, employeeRows }
}
function applySegmentsToEmployees(baseEmployees: any[]) {
    if (!employeeId.value) return baseEmployees
    const targetId = String(employeeId.value)
    const existingForSelected = baseEmployees.filter((row: any) => String(row?.employeeId ?? '') === targetId)
    const remaining = baseEmployees.filter((row: any) => String(row?.employeeId ?? '') !== targetId)
    const meta = (employeeStore.employees || []).find(e => String(e.id) === targetId)
    const monthDays = monthWorkingDays.value
    const existingById = new Map<string, any>()
    for (const row of existingForSelected) {
        if (row?.id) existingById.set(String(row.id), row)
    }

    // N?u nh�n s? chuy?n sang BM t?i c�ng ty kh�c, c?t giai do?n BM hi?n t?i c?a c�ng ty d� (n?u c�)
    const adjustedRemaining = [...remaining]
    for (const seg of segments.value) {
        if (seg.role !== QuotaRole.BM || !seg.companyId) continue
        const segStart = toDateLocal(seg.start)
        if (!segStart) continue
        for (const row of adjustedRemaining) {
            if (row?.quotaRole !== QuotaRole.BM) continue
            if (String(row?.companyId ?? '') !== String(seg.companyId)) continue
            const rowStart = toDateLocal(row?.allocationStartAt ?? row?.joinedAt)
            const rowEnd = toDateLocal(row?.allocationEndAt ?? row?.endAt ?? row?.endDate ?? row?.allocationEndAt)
            if (!rowStart || !rowEnd) continue
            if (rowEnd < segStart) continue
            // c?t t?i tru?c ng�y b? nhi?m m?i
            const newEndStr = addDaysYmd(dayjs(segStart).format('YYYY-MM-DD'), -1)
            const newEnd = toDateLocal(newEndStr)
            if (!newEnd || newEnd < rowStart) {
                row.allocationEndAt = null
                row.revenuePerSale = 0
                continue
            }
            row.allocationEndAt = dayjs(newEnd).format('YYYY-MM-DD')
            const wd = workDaysBetween(dayjs(rowStart).format('YYYY-MM-DD'), dayjs(newEnd).format('YYYY-MM-DD'))
            row.revenuePerSale = Number(((row.revenuePerSale / monthDays) * wd).toFixed(2))
        }
    }

    // N?u c� do?n ASM m?i, c?t c�c do?n sales-like tr�ng th?i gian d? tr�nh gi? nguy�n ch? ti�u sales khi d� l�n ASM
    const salesLikeRoles = new Set<QuotaRole>([
        QuotaRole.Sale,
        QuotaRole.SalesLead,
        QuotaRole.ProbationEmployee,
        QuotaRole.LeavingEmployee,
    ])
    const salesRowsToRemove = new Set<any>()
    for (const seg of segments.value) {
        if (seg.role !== QuotaRole.ASM) continue
        const asmStart = toDateLocal(seg.start)
        if (!asmStart) continue
        for (const row of existingForSelected) {
            if (!salesLikeRoles.has(Number(row?.quotaRole) as QuotaRole)) continue
            const rowStart = toDateLocal(row?.allocationStartAt ?? row?.joinedAt ?? monthStartStr.value)
            const rowEnd = toDateLocal(row?.allocationEndAt ?? row?.endAt ?? row?.endDate ?? monthEndStr.value)
            if (!rowStart || !rowEnd) continue
            if (rowEnd < asmStart) continue
            const newEndStr = addDaysYmd(dayjs(seg.start).format('YYYY-MM-DD'), -1)
            const newEnd = toDateLocal(newEndStr)
            if (!newEnd || newEnd < rowStart) {
                salesRowsToRemove.add(row)
                continue
            }
            const wd = workDaysBetween(dayjs(rowStart).format('YYYY-MM-DD'), dayjs(newEnd).format('YYYY-MM-DD'))
            const monthDaysForScale = monthWorkingDays.value || countWorkingDaysBetween(monthStartDate(), monthEndDate())
            row.allocationEndAt = dayjs(newEnd).format('YYYY-MM-DD')
            row.revenuePerSale = monthDaysForScale > 0
                ? Number(((Number(row.revenuePerSale || 0) / monthDaysForScale) * wd).toFixed(2))
                : 0
        }
    }
    if (salesRowsToRemove.size) {
        const filtered = existingForSelected.filter(r => !salesRowsToRemove.has(r))
        existingForSelected.splice(0, existingForSelected.length, ...filtered)
    }

    // N?u nh�n s? BM b? gi�ng xu?ng Sale/SalesLead m� kh�ng khai b�o do?n BM m?i,
    // gi? l?i ph?n BM t?i tru?c ng�y b?t d?u do?n sales-like d?u ti�n d? b?o to�n ch? ti�u BM cu
    const hasBmSegment = segments.value.some(s => s.role === QuotaRole.BM)
    if (!hasBmSegment) {
        const salesSegmentsByCompany = new Map<string, Date>()
        for (const seg of segments.value) {
            if (!isSalesSegmentRole(seg.role)) continue
            if (!seg.companyId) continue
            const start = toDateLocal(seg.start)
            if (!start) continue
            const current = salesSegmentsByCompany.get(seg.companyId)
            if (!current || start < current) salesSegmentsByCompany.set(seg.companyId, start)
        }
        for (const row of existingForSelected) {
            if (row?.quotaRole !== QuotaRole.BM) continue
            const companyId = row?.companyId ?? ''
            const bmStart = toDateLocal(row?.allocationStartAt ?? row?.joinedAt)
            const bmEnd = toDateLocal(row?.allocationEndAt ?? row?.endAt ?? row?.endDate ?? row?.allocationEndAt)
            if (!bmStart || !bmEnd) continue
            const cutoff = salesSegmentsByCompany.get(companyId)
            let effectiveEnd = bmEnd
            if (cutoff && cutoff <= bmEnd) {
                const newEndStr = addDaysYmd(dayjs(cutoff).format('YYYY-MM-DD'), -1)
                const newEnd = toDateLocal(newEndStr)
                if (newEnd && newEnd >= bmStart) effectiveEnd = newEnd
            }
            if (effectiveEnd < bmStart) continue
            const wd = workDaysBetween(dayjs(bmStart).format('YYYY-MM-DD'), dayjs(effectiveEnd).format('YYYY-MM-DD'))
            const revenuePerSale = wd > 0 ? Number(((row.revenuePerSale / monthDays) * wd).toFixed(2)) : 0
            adjustedRemaining.push({
                ...row,
                allocationStartAt: dayjs(bmStart).format('YYYY-MM-DD'),
                allocationEndAt: dayjs(effectiveEnd).format('YYYY-MM-DD'),
                revenuePerSale,
            })
        }
    }
    const existingGroups = new Map<string, any[]>()
    for (const row of existingForSelected) {
        const key = existingGroupKey(row)
        const list = existingGroups.get(key) ?? []
        list.push(row)
        existingGroups.set(key, list)
    }
    for (const list of existingGroups.values()) {
        list.sort((a, b) => {
            const da = toDateLocal(a?.allocationStartAt ?? a?.joinedAt ?? monthStartStr.value)?.getTime() ?? 0
            const db = toDateLocal(b?.allocationStartAt ?? b?.joinedAt ?? monthStartStr.value)?.getTime() ?? 0
            return da - db
        })
    }

    const orderedSegments = [...segments.value].sort((a, b) => {
        const da = toDateLocal(a.start)?.getTime() ?? 0
        const db = toDateLocal(b.start)?.getTime() ?? 0
        return da - db
    })

    const adjustedWorkDays = adjustWorkDaysForSegments(orderedSegments)
    const nextRows = orderedSegments.map((seg, idx) => {
        const companyId = seg.role === QuotaRole.ASM ? '' : (seg.companyId ?? '')
        const regionId = seg.role === QuotaRole.ASM
            ? (seg.regionId ?? '')
            : (seg.regionId ?? resolveRegionIdForCompany(companyId))
        const key = segmentGroupKey(seg.role, companyId, regionId)
        const group = existingGroups.get(key) ?? []
        let existing = seg.sourceId ? existingById.get(String(seg.sourceId)) : undefined
        if (existing) {
            // remove from its group if present so it isn't reused
            const idx = group.findIndex(x => String(x?.id ?? '') === String(seg.sourceId))
            if (idx >= 0) group.splice(idx, 1)
        } else {
            existing = group.shift()
        }
        existingGroups.set(key, group)
        const workDays = idx < adjustedWorkDays.length
            ? adjustedWorkDays[idx]
            : workDaysBetween(seg.start, seg.end)
        let revenuePerSale = 0
        const unchanged =
            !!existing &&
            String(existing.companyId ?? '') === String(companyId) &&
            String(existing.regionId ?? '') === String(regionId) &&
            mapSegmentRoleToQuotaRole(seg.role) === Number(existing.quotaRole) &&
            String(existing.allocationStartAt ?? existing.joinedAt ?? '') === String(seg.start) &&
            String(existing.allocationEndAt ?? existing.endAt ?? existing.endDate ?? '') === String(seg.end)
        if (monthDays > 0) {
            if (isSalesSegmentRole(seg.role)) {
                revenuePerSale = computeSegmentQuota(seg, workDays, monthDays)
            } else {
                const total = seg.role === QuotaRole.BM
                    ? (companyTotalsCurrentById.value.get(companyId) ?? companyQuotaById.value.get(companyId ?? '')?.total ?? 0)
                    : (regionTotalsCurrentById.value.get(regionId) ?? regionQuotaById.value.get(regionId ?? '')?.total ?? 0)
                revenuePerSale = (total / monthDays) * workDays
            }
        }
        if (workDays < 7) revenuePerSale = 0
        return {
            id: existing?.id ?? undefined,
            employeeId: employeeId.value,
            companyId: companyId || null,
            regionId: regionId || null,
            positionId: seg.positionId ?? existing?.positionId ?? (meta as any)?.positionId ?? null,
            quotaRole: mapSegmentRoleToQuotaRole(seg.role),
            workStage: existing?.workStage ?? null,
            revenuePerSale: unchanged ? Number(existing?.revenuePerSale ?? 0) : Number(revenuePerSale.toFixed(2)),
            joinedAt: existing?.joinedAt ?? (meta as any)?.employeeStartedDate ?? null,
            endAt: existing?.endAt ?? (meta as any)?.employeeEndDate ?? null,
            allocationStartAt: unchanged ? (existing?.allocationStartAt ?? seg.start) : seg.start,
            allocationEndAt: unchanged ? (existing?.allocationEndAt ?? seg.end) : seg.end,
            probationEnd: existing?.probationEnd ?? (meta as any)?.employeeNewEndDate ?? null,
            note: existing?.note ?? '',
            keepOriginalQuota: unchanged
        }
    })

    const leftovers: any[] = []
    for (const list of existingGroups.values()) {
        for (const row of list) leftovers.push({ ...row, keepOriginalQuota: true })
    }

    return [...remaining, ...nextRows, ...leftovers]
}
function adjustBmAllocations(rows: any[]) {
    const impactedCompanies = new Set(
        segments.value.filter(seg => seg.role === QuotaRole.BM && seg.companyId).map(seg => seg.companyId as string)
    )
    const impactedRegions = new Set(
        segments.value.filter(seg => seg.role === QuotaRole.ASM && seg.regionId).map(seg => seg.regionId as string)
    )
    if (!impactedCompanies.size && !impactedRegions.size) return rows
    const monthStart = monthStartStr.value
    const monthEnd = monthEndStr.value
    const toTime = (value: any) => toDateLocal(value)?.getTime() ?? 0
    const remove = new Set<any>()

    const groups = new Map<string, any[]>()
    for (const row of rows) {
        if (row?.quotaRole !== QuotaRole.BM) continue
        if (!row?.companyId || !impactedCompanies.has(row.companyId)) continue
        const start = clampToMonth(row.allocationStartAt ?? row.joinedAt ?? monthStart, monthStart, monthEnd)
        const end = clampToMonth(row.allocationEndAt ?? row.endAt ?? monthEnd, monthStart, monthEnd)
        row.allocationStartAt = start
        row.allocationEndAt = end
        if (!groups.has(row.companyId)) groups.set(row.companyId, [])
        groups.get(row.companyId)!.push(row)
    }

    for (const list of groups.values()) {
        list.sort((a, b) => toTime(a.allocationStartAt) - toTime(b.allocationStartAt))
        for (let i = 0; i < list.length - 1; i++) {
            const curr = list[i]
            const next = list[i + 1]
            const currEnd = toDateLocal(curr.allocationEndAt ?? monthEnd)
            const nextStart = toDateLocal(next.allocationStartAt ?? monthStart)
            if (!currEnd || !nextStart) continue
            if (currEnd >= nextStart) {
                const newEnd = addDaysYmd(next.allocationStartAt ?? monthStart, -1)
                curr.allocationEndAt = newEnd
            }
        }
        for (const row of list) {
            const start = toDateLocal(row.allocationStartAt ?? monthStart)
            const end = toDateLocal(row.allocationEndAt ?? monthEnd)
            if (start && end && end < start) remove.add(row)
        }
    }

    // C?t c�c do?n ASM cu khi c� ASM m?i trong c�ng v�ng
    const asmGroups = new Map<string, any[]>()
    for (const row of rows) {
        if (row?.quotaRole !== QuotaRole.ASM) continue
        if (!row?.regionId || !impactedRegions.has(row.regionId)) continue
        const start = clampToMonth(row.allocationStartAt ?? row.joinedAt ?? monthStart, monthStart, monthEnd)
        const end = clampToMonth(row.allocationEndAt ?? row.endAt ?? monthEnd, monthStart, monthEnd)
        row.allocationStartAt = start
        row.allocationEndAt = end
        if (!asmGroups.has(row.regionId)) asmGroups.set(row.regionId, [])
        asmGroups.get(row.regionId)!.push(row)
    }

    for (const list of asmGroups.values()) {
        list.sort((a, b) => toTime(a.allocationStartAt) - toTime(b.allocationStartAt))
        for (let i = 0; i < list.length - 1; i++) {
            const curr = list[i]
            const next = list[i + 1]
            const currEnd = toDateLocal(curr.allocationEndAt ?? monthEnd)
            const nextStart = toDateLocal(next.allocationStartAt ?? monthStart)
            if (!currEnd || !nextStart) continue
            if (currEnd >= nextStart) {
                const newEnd = addDaysYmd(next.allocationStartAt ?? monthStart, -1)
                curr.allocationEndAt = newEnd
            }
        }
        for (const row of list) {
            const start = toDateLocal(row.allocationStartAt ?? monthStart)
            const end = toDateLocal(row.allocationEndAt ?? monthEnd)
            if (start && end && end < start) remove.add(row)
        }
    }

    return rows.filter(row => !remove.has(row))
}
function updateManagerQuotas(rows: any[]) {
    const monthDays = monthWorkingDays.value
    if (!monthDays) return rows

    // Chu?n h�a ng�y c�ng BM theo t?ng c�ng ty: n?u c�c kho?ng BM li�n t?c ph? k�n th�ng, t?ng ng�y c�ng = ng�y c�ng th�ng
    const bmWorkDaysMap = new Map<string, number>()
    const bmGroups = new Map<string, any[]>()
    const bmKey = (r: any) => [
        r.id ?? '',
        r.companyId ?? '',
        r.allocationStartAt ?? '',
        r.allocationEndAt ?? ''
    ].join('|')
    for (const r of rows) {
        if (r.quotaRole !== QuotaRole.BM || !r.companyId) continue
        if (!bmGroups.has(r.companyId)) bmGroups.set(r.companyId, [])
        bmGroups.get(r.companyId)!.push(r)
    }
    for (const [cid, list] of bmGroups.entries()) {
        const ordered = [...list].sort((a, b) =>
            (toDateLocal(a.allocationStartAt ?? a.joinedAt ?? monthStartStr.value)?.getTime() ?? 0) -
            (toDateLocal(b.allocationStartAt ?? b.joinedAt ?? monthStartStr.value)?.getTime() ?? 0)
        )
        const raw: number[] = ordered.map(r => workDaysBetween(
            r.allocationStartAt ?? monthStartStr.value,
            r.allocationEndAt ?? monthEndStr.value
        ))
        const isContiguous = ordered.every((r, idx) => {
            if (idx === 0) return true
            const expected = addDaysYmd(ordered[idx - 1].allocationEndAt ?? monthEndStr.value, 1)
            return (r.allocationStartAt ?? '') === expected
        })
        const coversMonth = (ordered[0]?.allocationStartAt === monthStartStr.value) &&
            (ordered[ordered.length - 1]?.allocationEndAt === monthEndStr.value)
        if (isContiguous && coversMonth) {
            const totalRaw = raw.reduce((s, v) => s + v, 0)
            const diff = totalRaw - monthDays
            if (diff > 0 && raw.length) {
                raw[raw.length - 1] = Math.max(0, raw[raw.length - 1] - diff)
            }
        }
        raw.forEach((wd, idx) => bmWorkDaysMap.set(bmKey(ordered[idx]), wd))
    }

    return rows.map((row: any) => {
        const isBm = row.quotaRole === QuotaRole.BM
        const isAsm = row.quotaRole === QuotaRole.ASM
        const companyChanged = isBm && row.companyId ? companyChangedById.value.has(row.companyId) : false
        const regionChanged = isAsm && row.regionId ? regionChangedById.value.has(row.regionId) : false
        if (row?.keepOriginalQuota && !companyChanged && !regionChanged) return row
        if (row.quotaRole !== QuotaRole.BM && row.quotaRole !== QuotaRole.ASM) return row
        const start = row.allocationStartAt ?? monthStartStr.value
        const end = row.allocationEndAt ?? monthEndStr.value
        let workDays = workDaysBetween(start, end)
        if (isBm && row.companyId) {
            const wd = bmWorkDaysMap.get(bmKey(row))
            if (wd !== undefined) workDays = wd
        }
        if (row.quotaRole === QuotaRole.BM && row.companyId) {
            const total = companyTotalsById.value.get(row.companyId) ?? 0
            row.revenuePerSale = Number(((total / monthDays) * workDays).toFixed(2))
        }
        if (row.quotaRole === QuotaRole.ASM && row.regionId) {
            const total = regionTotalsById.value.get(row.regionId) ?? 0
            row.revenuePerSale = Number(((total / monthDays) * workDays).toFixed(2))
        }
        return row
    })
}
function buildPayloadNested(
    quotaStage: number,
    form: any,
    regionRows: any[],
    companyRows: any[],
    employeeRows: any[]
) {
    const isUpdate = !!form?.id

    const actualByCompany = new Map<string, number>()
    for (const e of (employeeRows || [])) {
        const companyId = e?.companyId
        if (!companyId) continue
        if (isSalesQuotaRole(e?.quotaRole)) {
            const current = actualByCompany.get(companyId) ?? 0
            actualByCompany.set(companyId, current + Number(e.revenuePerSale || 0))
        }
    }

    const regionMap = new Map<string, any>()
    for (const r of (regionRows || [])) {
        const n = Number(r.numberOfSalesAllocated ?? r.numberOfSales ?? 0)
        const rev = Number(r.revenuePerSale || 0)

        const regionDto: any = {
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
        }

        regionMap.set(r.regionId, regionDto)
    }

    for (const c of (companyRows || [])) {
        const rId = c.regionId
        const parentRegion = regionMap.get(rId)
        if (!parentRegion) continue

        const n = Number(c.numberOfSalesAllocated ?? c.numberOfSales ?? 0)
        const rev = Number(c.revenuePerSale || 0)
        const planned = n * rev
        const actual = actualByCompany.get(c.companyId) ?? 0
        const defaultTotal = Math.max(planned, actual)
        const hasAdjustment = Array.isArray(c.admissionsQuotaAdjustments) && c.admissionsQuotaAdjustments.length > 0
        const totalRevenue = hasAdjustment
            ? resolveTotalRevenue(c.totalRevenue, defaultTotal)
            : defaultTotal

        const companyDto: any = {
            id: isUpdate ? (c.id || undefined) : undefined,
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
        }

        parentRegion.admissionsQuotaCompanies.push(companyDto)
    }
    for (const reg of regionMap.values()) {
        const totalRevenue = (reg.admissionsQuotaCompanies || []).reduce(
            (s: number, comp: any) => s + Number(comp.totalRevenue || 0),
            0
        )
        reg.totalRevenue = totalRevenue
        const salesAllocated = Number(reg.numberOfSalesAllocated || 0)
        reg.revenuePerSale = salesAllocated > 0 ? Math.round(totalRevenue / salesAllocated) : 0
    }

    const companyIndex = new Map<string, any>()
    for (const reg of regionMap.values()) {
        for (const comp of reg.admissionsQuotaCompanies) {
            companyIndex.set(`${reg.regionId}_${comp.companyId}`, comp)
        }
    }

    for (const e of (employeeRows || [])) {
        const key = `${e.regionId ?? ''}_${e.companyId ?? ''}`
        const comp = companyIndex.get(key)
        const parentRegion = regionMap.get(e.regionId ?? '')

        const { allocationStartAt, allocationEndAt } = clampAllocationRange(
            Number(form.year),
            Number(form.month),
            e.allocationStartAt ?? e.joinedAt ?? e.joinDate,
            e.allocationEndAt ?? e.endAt ?? e.endDate
        )
        const eDto: any = {
            id: isUpdate ? (e.id || undefined) : undefined,
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
        }

        if (comp) comp.admissionsQuotaEmployees.push(eDto)
        else if (parentRegion) parentRegion.admissionsQuotaEmployees.push(eDto)
    }

    const admissionsQuotaRegions = Array.from(regionMap.values()).map((reg: any) => {
        reg.companyCount = Array.isArray(reg.admissionsQuotaCompanies)
            ? reg.admissionsQuotaCompanies.length
            : 0
        return reg
    })
    const payload: any = {
        month: Number(form.month),
        year: Number(form.year),
        note: form.note ?? '',
        status: 1,
        quotaStage,
        companyCount: admissionsQuotaRegions.reduce(
            (s: number, r: any) => s + (r.admissionsQuotaCompanies?.length ?? 0),
            0
        ),
        currentSales: admissionsQuotaRegions.reduce((s, r) => s + (r.currentSales || 0), 0),
        totalSalesAllocated: admissionsQuotaRegions.reduce((s, r) => s + (r.numberOfSalesAllocated || 0), 0),
        totalQuota: admissionsQuotaRegions.reduce((s, r) => s + (r.totalRevenue || 0), 0),
        admissionsQuotaRegions
    }

    if (isUpdate) {
        payload.id = form.id
    }

    return payload
}
function buildTransferPayload() {
    const detail = quotaDetail.value
    if (!detail) return null
    const { regionRows, companyRows, employeeRows } = buildRowsFromQuota(detail)
    const withSegments = applySegmentsToEmployees(employeeRows)
    const withAdjustedBm = adjustBmAllocations(withSegments)
    const updatedEmployees = updateManagerQuotas(withAdjustedBm)
    return buildPayloadNested(
        Number(detail.quotaStage ?? 0),
        { id: detail.id, month: detail.month, year: detail.year, note: detail.note },
        regionRows,
        companyRows,
        updatedEmployees
    )
}

async function apply() {
    if (!validate(true)) return
    try {
        saving.value = true
        if (selectedQuotaId.value && (!quotaDetail.value || quotaDetail.value.id !== selectedQuotaId.value)) {
            await loadQuotaDetail(selectedQuotaId.value)
        }
        const payload = buildTransferPayload()
        if (!payload) return
        await admissionsQuotaStore.transferRoles(payload)
        emit('applied')
        close()
    } finally {
        saving.value = false
    }
}
</script>

<style scoped>
.quota-transfer {
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.segment-toolbar {
    display: flex;
    align-items: center;
    gap: 8px;
}

.spacer {
    flex: 1;
}

.segment-table :deep(.el-table__cell) {
    vertical-align: top;
}

.total-card {
    margin-top: 8px;
}

.impact-card {
    margin-top: 8px;
}

.total-row-secondary {
    margin-top: 10px;
}

.summary-grid {
    margin-top: 8px;
}

.card-title {
    font-weight: 600;
}

.total-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 12px;
}

.total-label {
    font-size: 12px;
    color: #666;
}

.total-value {
    font-size: 20px;
    font-weight: 600;
}

.total-meta {
    font-size: 12px;
    color: #666;
    text-align: right;
}
</style>






