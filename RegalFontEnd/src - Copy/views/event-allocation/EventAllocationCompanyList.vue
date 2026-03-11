<template>
  <div class="company-allocation-page">
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-start align-items-md-center mb-6">
      <div>
        <h2 class="fw-bold mb-1">{{ t('allocationEvent.companyListHeader') }}</h2>
        <p class="text-muted mb-0">{{ t('allocationEvent.companyListDesc') }}</p>
      </div>
      <el-button type="primary" plain @click="onExport">
        <el-icon class="me-1">
          <Download />
        </el-icon>
        {{ t('allocationEvent.exportReport') }}
      </el-button>
    </div>

    <el-card class="mb-6">
      <div class="row g-4">
        <div class="col-12 col-md-4">
          <label class="form-label d-block mb-1">{{ t('allocationEvent.companyFilter.searchLabel') }}</label>
          <el-input v-model="searchKeyword" :placeholder="t('allocationEvent.companyFilter.searchPlaceholder')"
            clearable />
        </div>
        <div class="col-12 col-md-4">
          <label class="form-label d-block mb-1">{{ t('allocationEvent.companyFilter.monthLabel') }}</label>
          <el-select v-model="monthFilter" class="w-100">
            <el-option value="all" :label="t('allocationEvent.companyFilter.monthAll')" />
            <el-option v-for="opt in monthOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
          </el-select>
        </div>
        <div class="col-12 col-md-4">
          <label class="form-label d-block mb-1">{{ t('allocationEvent.companyFilter.statusLabel') }}</label>
          <el-select v-model="statusFilter" class="w-100">
            <el-option value="all" :label="t('allocationEvent.companyFilter.statusAll')" />
            <el-option v-for="opt in statusOptions" :key="opt.value" :label="t(opt.label)" :value="opt.value" />
          </el-select>
        </div>
      </div>
    </el-card>

    <el-card class="mb-8">
      <div class="d-flex flex-column flex-md-row justify-content-between align-items-start align-items-md-center mb-4">
        <div>
          <h5 class="fw-semibold mb-1">{{ t('allocationEvent.companyTableTitle') }}</h5>
          <span class="text-muted fs-8">{{ t('allocationEvent.companyTableDesc') }}</span>
        </div>
        <span class="text-muted fs-8">
          {{ t('allocationEvent.eventAllocationListCount', { count: filteredRows.length }) }}
        </span>
      </div>

      <BaseTable :columns="columns" :items="filteredRows" :loading="allocationStore.loading" :showCheckboxColumn="false"
        :showActionsColumn="false" :showIndex="true" :height="500">
        <template #cell-companyName="{ item }">
          <span class="fw-semibold">{{ item.companyName }}</span>
          <div class="text-muted fs-8">{{ item.regionName }}</div>
        </template>

        <template #cell-totalQuantity="{ item }">
          <span class="fw-semibold">
            {{ t('allocationEvent.eventsCount', { count: item.totalQuantity }) }}
          </span>
        </template>

        <!-- Ngân sách phân bổ theo AllocationEvent.eventBudget -->
        <template #cell-eventBudget="{ item }">
          <span class="fw-semibold">{{ formatCurrency(item.eventBudget) }}</span>
        </template>

        <!-- Tổng ngân sách theo chi nhánh (sum Budget của các detail) -->
        <template #cell-totalBudget="{ item }">
          <span class="fw-semibold">{{ formatCurrency(item.totalBudget) }}</span>
        </template>

        <template #cell-status="{ item }">
          <BaseBadge :label="statusLabel(item.status)" :colorByLabelMap="statusColorMap" />
        </template>

        <template #cell-view="{ item }">
          <div class="d-flex justify-content-center gap-2">
            <el-tooltip :content="t('allocationEvent.proposal.open')" placement="top">
              <el-button circle size="small" @click="openProposal(item)">
                <el-icon>
                  <Plus />
                </el-icon>
              </el-button>
            </el-tooltip>

            <el-tooltip :content="t('allocationEvent.viewAllocations')" placement="top">
              <el-button circle size="small" @click="openAllocations(item)">
                <el-icon>
                  <List />
                </el-icon>
              </el-button>
            </el-tooltip>

            <el-tooltip :content="t('common.viewDetail')" placement="top">
              <el-button circle size="small" @click="viewAllocation(item)">
                <el-icon>
                  <View />
                </el-icon>
              </el-button>
            </el-tooltip>
          </div>
        </template>
      </BaseTable>
    </el-card>

    <CompanyAllocationDetailDialog v-model:visible="showDialog" :branch-row="selectedBranch"
      :allocation-event="selectedEvent" />

    <CompanyEventProposalDialog v-model:visible="showProposalDialog" :branch-row="selectedBranch"
      :allocation-event="selectedEvent" :proposal="dialogProposal" :submitting="proposalSubmitting"
      @submit="handleProposalSubmit" />

    <BaseDialogForm :visible="showAllocationsDialog" :title="t('allocationEvent.viewAllocations')"
      :actionsColumnWidth="180" :description="selectedBranch?.companyName" :show-delete="false" :submit-disabled="true"
      mode="view" width="90%" @update:visible="showAllocationsDialog = $event">
      <template #form>
        <div class="allocations-table-wrapper">
          <BaseTable :columns="proposalColumns" :items="proposalRows" :loading="proposalsLoading"
            :showActionsColumn="true" :actionsColumnWidth="180" :showCheckboxColumn="false" :height="360">
            <template #cell-rowNumber="{ item }">
              {{ item.rowNumber }}
            </template>

            <template #cell-eventDate="{ item }">
              {{ formatDate(item.eventDate) }}
            </template>

            <template #cell-totalAmount="{ item }">
              {{ formatCurrency(item.totalAmount) }}
            </template>

            <template #cell-attachmentLabel="{ item }">
              {{ item.attachmentLabel }}
            </template>

            <template #cell-status="{ item }">
              <BaseBadge :label="item.statusLabel" :colorByLabelMap="proposalStatusColorMap" />
            </template>

            <template #actions="{ item }">
              <div class="d-flex justify-content-center gap-2">
                <el-tooltip v-if="canCreateReport(item.raw)" :content="t('allocationEvent.report.create')"
                  placement="top">
                  <el-button circle size="small" @click="createReportFromProposal(item.raw)">
                    <el-icon>
                      <Plus />
                    </el-icon>
                  </el-button>
                </el-tooltip>
                <el-tooltip v-if="canViewReports(item.raw)" :content="t('allocationEvent.report.title')"
                  placement="top">
                  <el-button circle size="small" @click="openReportList(item.raw)">
                    <el-icon>
                      <Document />
                    </el-icon>
                  </el-button>
                </el-tooltip>
                <el-tooltip :content="t('common.edit')" placement="top">
                  <el-button circle size="small" @click="editProposalFromDialog(item.raw)">
                    <el-icon>
                      <Edit />
                    </el-icon>
                  </el-button>
                </el-tooltip>
                <el-tooltip v-if="canMoveToPending(item.raw)" :content="t('allocationEvent.proposal.sendPending')"
                  placement="top">
                  <el-button circle size="small" @click="markProposalPending(item.raw)">
                    <el-icon>
                      <Refresh />
                    </el-icon>
                  </el-button>
                </el-tooltip>
              </div>
            </template>
          </BaseTable>
        </div>
      </template>
    </BaseDialogForm>

    <BaseDialogForm :visible="showReportListDialog" :title="t('allocationEvent.report.title')"
      :description="reportDialogDescription" :show-delete="false" :submit-disabled="true" mode="view" width="80%"
      @update:visible="showReportListDialog = $event">
      <template #form>
        <div class="allocations-table-wrapper">
          <BaseTable :columns="reportColumns" :items="reportRows" :loading="reportStore.loading"
            :showActionsColumn="true" :actionsColumnWidth="120" :showCheckboxColumn="false" :height="320">
            <template #cell-rowNumber="{ item }">
              {{ item.rowNumber }}
            </template>
            <template #cell-eventDate="{ item }">
              {{ formatDate(item.eventDate) }}
            </template>
            <template #cell-reportDate="{ item }">
              {{ formatDate(item.reportDate) }}
            </template>
            <template #cell-costActual="{ item }">
              {{ formatCurrency(item.costActual) }}
            </template>
            <template #cell-status="{ item }">
              <BaseBadge :label="item.statusLabel" :colorByLabelMap="proposalStatusColorMap" />
            </template>
            <template #actions="{ item }">
              <div class="d-flex justify-content-center gap-2">
                <el-tooltip v-if="canEditReport(item.raw)" :content="t('common.edit')" placement="top">
                  <el-button circle size="small" @click="editReportFromDialog(item.raw)">
                    <el-icon>
                      <Edit />
                    </el-icon>
                  </el-button>
                </el-tooltip>
                <el-tooltip v-if="canMoveReportToPending(item.raw)" :content="t('allocationEvent.proposal.sendPending')"
                  placement="top">
                  <el-button circle size="small" @click="markReportPending(item.raw)">
                    <el-icon>
                      <Refresh />
                    </el-icon>
                  </el-button>
                </el-tooltip>
              </div>
            </template>
          </BaseTable>
        </div>
      </template>
    </BaseDialogForm>

    <CompanyEventReportDialog :visible="reportEditDialogVisible" :proposal="reportDialogProposal"
      :report="reportEditDialogReport" :submitting="reportEditDialogSubmitting" :readonly="reportEditDialogReadonly"
      @update:visible="reportEditDialogVisible = $event" @submit="handleReportSubmit" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { Download, View, Plus, List, Edit, Refresh, Document } from '@element-plus/icons-vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import CompanyAllocationDetailDialog from '@/views/event-allocation/CompanyAllocationDetailDialog.vue'
import CompanyEventProposalDialog from '@/views/event-allocation/CompanyEventProposalDialog.vue'
import CompanyEventReportDialog from '@/views/event-allocation/CompanyEventReportDialog.vue'
import { useAllocationEventStore } from '@/stores/allocationEventStore'
import { useCompanyEventReportStore } from '@/stores/companyEventReportStore'
import { useRegionStore } from '@/stores/regionStore'
import { useCompanyStore } from '@/stores/companyStore'
import { useEventStore } from '@/stores/eventStore'
import { useNotificationStore } from '@/stores/notificationStore'
import { formatCurrency, formatDate } from '@/utils/format'
import {
  AllocationEventStatus,
  AllocationEventStatusLabels,
  CompanyEventProposalStatus,
  CompanyEventProposalStatusLabels
} from '@/types'
import type {
  AllocationDetailEventModel,
  AllocationEventModel,
  CompanyEventProposalRequest,
  CompanyEventModel,
  ApproveCompanyEventModel,
  CompanyEventReportModel,
  ApproveCompanyEventReportModel
} from '@/api/AllocationEventApi'
import { CompanyEventService } from '@/services/CompanyEventService'

const { t } = useI18n()
const allocationStore = useAllocationEventStore()
const reportStore = useCompanyEventReportStore()
const regionStore = useRegionStore()
const companyStore = useCompanyStore()
const notificationStore = useNotificationStore()
const eventStore = useEventStore()

const searchKeyword = ref('')
const monthFilter = ref<string | 'all'>('all')
const statusFilter = ref<number | 'all'>('all')

const showDialog = ref(false)
const showProposalDialog = ref(false)
const showAllocationsDialog = ref(false)
const showReportListDialog = ref(false)
const proposalSubmitting = ref(false)
const selectedBranch = ref<BranchAllocationRow | null>(null)
const selectedEvent = ref<AllocationEventModel | null>(null)
const dialogProposal = ref<CompanyEventModel | null>(null)
const reportDialogProposal = ref<CompanyEventModel | null>(null)
const reportEditDialogVisible = ref(false)
const reportEditDialogReport = ref<CompanyEventReportModel | null>(null)
const reportEditDialogSubmitting = ref(false)
const reportEditDialogReadonly = ref(false)
const companyEventService = new CompanyEventService()
const companyProposals = ref<CompanyEventModel[]>([])
const proposalsLoading = ref(false)

interface BranchAllocationRow {
  id: string
  eventId?: string
  allocationCode?: string
  monthLabel: string
  monthKey?: string
  allocationMonth?: number
  allocationYear?: number
  regionId?: string | number | null
  regionName: string
  companyId?: string | number | null
  companyName: string
  noAllocation: boolean
  totalQuantity: number
  // Tổng ngân sách phân bổ cho chi nhánh (sum budget từ các detail)
  totalBudget: number
  // Ngân sách được phân bổ (eventBudget từ AllocationEventModel tương ứng)
  eventBudget: number
  status: number
  details: AllocationDetailEventModel[]
}

const columns: BaseTableColumn[] = [
  { key: 'allocationCode', labelKey: 'allocationEvent.code', isBold: true },
  { key: 'monthLabel', labelKey: 'allocationEvent.month', width: 160 },
  { key: 'companyName', labelKey: 'allocationEvent.branches', width: 220 },
  { key: 'totalQuantity', labelKey: 'allocationEvent.quantity', width: 160, align: 'center' },
  // Cột ngân sách được phân bổ cho chi nhánh (eventBudget)
  { key: 'eventBudget', labelKey: 'allocationEvent.eventBudget', width: 200, align: 'right' },
  // Cột tổng ngân sách theo chi nhánh (sum detail.budget)
  { key: 'totalBudget', labelKey: 'allocationEvent.totalBudget', width: 200, align: 'right' },
  { key: 'status', labelKey: 'allocationEvent.status', width: 160, align: 'center' },
  { key: 'view', labelKey: 'common.actions', width: 220, align: 'center', fixed: 'right' }
]

const proposalColumns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center', sticky: true },
  { key: 'companyEventCode', labelKey: 'allocationEvent.proposal.proposalCode', width: 200, sticky: true },
  { key: 'allocationCode', labelKey: 'allocationEvent.code', width: 160 },
  { key: 'companyEventName', labelKey: 'allocationEvent.proposal.eventName', minWidth: 200 },
  { key: 'companyName', labelKey: 'allocationEvent.branches', width: 180 },
  { key: 'regionName', labelKey: 'allocationEvent.region', width: 140 },
  { key: 'eventDate', labelKey: 'allocationEvent.proposal.eventDate', width: 150, align: 'center' },
  { key: 'totalAmount', labelKey: 'allocationEvent.proposal.totalProjectedCost', width: 160, align: 'right' },
  { key: 'attachmentLabel', labelKey: 'allocationEvent.proposalList.attachments', width: 160, align: 'center' },
  { key: 'status', labelKey: 'common.status', width: 150, align: 'center' }
]

const reportColumns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center' },
  { key: 'proposalCode', labelKey: 'allocationEvent.report.proposalCode', width: 160 },
  { key: 'reportCode', labelKey: 'allocationEvent.report.reportCode', width: 160 },
  { key: 'eventName', labelKey: 'allocationEvent.report.eventName', minWidth: 200 },
  { key: 'eventDate', labelKey: 'allocationEvent.report.eventDate', width: 140, align: 'center' },
  { key: 'costActual', labelKey: 'allocationEvent.report.costActual', width: 140, align: 'right' },
  { key: 'branch', labelKey: 'allocationEvent.report.branch', width: 180 },
  { key: 'region', labelKey: 'allocationEvent.region', width: 140 },
  { key: 'reportDate', labelKey: 'allocationEvent.report.reportDate', width: 150, align: 'center' },
  { key: 'status', labelKey: 'allocationEvent.report.status', width: 150, align: 'center' }
]

const statusOptions = computed(() =>
  Object.values(AllocationEventStatus)
    .filter(v => typeof v === 'number')
    .map(v => ({
      value: v as number,
      label: AllocationEventStatusLabels[v as AllocationEventStatus]
    }))
)

const statusColorMap = computed(() => ({
  [t(AllocationEventStatusLabels[AllocationEventStatus.Draft])]: 'warning',
  [t(AllocationEventStatusLabels[AllocationEventStatus.Allocated])]: 'primary',
  [t(AllocationEventStatusLabels[AllocationEventStatus.Completed])]: 'success',
  [t(AllocationEventStatusLabels[AllocationEventStatus.Cancelled])]: 'danger'
}))

const regionNameMap = computed(() => {
  const map = new Map<string, string>()
  regionStore.regions.forEach(region => map.set(String(region.id), region.regionName ?? '-'))
  return map
})

const companyNameMap = computed(() => {
  const map = new Map<string, { name: string; regionId?: string | number | null }>()
  companyStore.companies.forEach(company => {
    const activeRegionId =
      company.logRegionComs?.find(x => x.companyId === company.id && x.status === 0)?.regionId ?? company.regionId
    map.set(String(company.id), {
      name: company.companyName ?? '-',
      regionId: activeRegionId
    })
  })
  return map
})

/**
 * Tổng hợp dữ liệu theo từng chi nhánh:
 * - totalQuantity: tổng số event được phân bổ (Quantity)
 * - totalBudget: tổng ngân sách phân bổ cho chi nhánh (sum Budget của AllocationDetailEvent)
 * - eventBudget: ngân sách từ AllocationEvent.eventBudget
 */
const branchAllocations = computed<BranchAllocationRow[]>(() => {
  const rows: BranchAllocationRow[] = []
  const events = allocationStore.allocationEvents || []

  events.forEach(event => {
    const details = event.allocationDetails || []

    const perCompany = new Map<
      string,
      {
        quantity: number
        details: AllocationDetailEventModel[]
        noAllocation: boolean
        regionId?: string | number | null
        companyId?: string | number | null
        budget: number
      }
    >()

    details.forEach(detail => {
      if (detail?.companyId == null) return

      const key = String(detail.companyId)

      if (!perCompany.has(key)) {
        perCompany.set(key, {
          quantity: 0,
          details: [],
          noAllocation: detail.noAllocation === 1,
          regionId: detail.regionId ?? null,
          companyId: detail.companyId,
          budget: 0
        })
      }

      const group = perCompany.get(key)!

      // Ngân sách phân bổ cho chi nhánh ở detail này (đã mapping từ AllocationDetailEvent.Budget)
      const detailBudget = Number(detail.budget || 0)

      group.quantity += Number(detail.quantity) || 0
      group.noAllocation = group.noAllocation && detail.noAllocation === 1
      group.regionId = group.regionId ?? detail.regionId
      group.budget += detailBudget
      group.details.push(detail)
    })

    perCompany.forEach((group, key) => {
      const companyInfo = companyNameMap.value.get(key)
      const regionId = group.regionId ?? companyInfo?.regionId ?? null
      const month = event.allocationMonth ? String(event.allocationMonth).padStart(2, '0') : '--'
      const year = event.allocationYear ? String(event.allocationYear) : '--'

      rows.push({
        id: `${event.id}-${key}`,
        eventId: event.id || undefined,
        allocationCode: event.allocationCode,
        monthLabel:
          event.allocationMonth && event.allocationYear
            ? t('allocationEvent.monthLabel', { month, year })
            : '-',
        monthKey: event.allocationMonth && event.allocationYear ? `${event.allocationYear}-${month}` : undefined,
        allocationMonth: event.allocationMonth,
        allocationYear: event.allocationYear,
        regionId,
        regionName: regionNameMap.value.get(String(regionId ?? '')) || t('common.none'),
        companyId: group.companyId,
        companyName: companyInfo?.name || t('common.none'),
        noAllocation: group.noAllocation,
        totalQuantity: group.quantity,
        // Tổng ngân sách phân bổ cho chi nhánh (sum Budget từ các detail)
        totalBudget: group.budget,
        // Ngân sách được phân bổ cho chi nhánh lấy trực tiếp từ AllocationEvent.eventBudget
        eventBudget: Number(event.eventBudget ?? 0),
        status: Number(event.allocationEventStatus ?? AllocationEventStatus.Draft),
        details: group.details
      })
    })
  })

  rows.sort((a, b) => {
    const ay = a.allocationYear ?? 0
    const by = b.allocationYear ?? 0
    if (ay !== by) return by - ay
    const am = a.allocationMonth ?? 0
    const bm = b.allocationMonth ?? 0
    if (am !== bm) return bm - am
    return a.companyName.localeCompare(b.companyName, undefined, { sensitivity: 'base' })
  })

  return rows
})

const monthOptions = computed(() => {
  const set = new Set<string>()
  allocationStore.allocationEvents?.forEach(event => {
    if (event.allocationMonth && event.allocationYear) {
      const key = `${event.allocationYear}-${String(event.allocationMonth).padStart(2, '0')}`
      set.add(key)
    }
  })
  return Array.from(set)
    .sort((a, b) => b.localeCompare(a))
    .map(value => {
      const [year, month] = value.split('-')
      return {
        value,
        label: t('allocationEvent.monthLabel', { month, year })
      }
    })
})

const filteredRows = computed(() => {
  const search = searchKeyword.value.trim().toLowerCase()
  return branchAllocations.value.filter(row => {
    const matchesKeyword =
      !search ||
      (row.allocationCode ?? '').toLowerCase().includes(search) ||
      row.companyName.toLowerCase().includes(search) ||
      row.regionName.toLowerCase().includes(search)

    const matchesMonth = monthFilter.value === 'all' || row.monthKey === monthFilter.value
    const matchesStatus = statusFilter.value === 'all' || row.status === statusFilter.value

    return matchesKeyword && matchesMonth && matchesStatus
  })
})

function statusLabel(status: number) {
  const key = AllocationEventStatusLabels[status as AllocationEventStatus]
  return key ? t(key) : t('common.unknown')
}

function proposalStatusLabel(item: CompanyEventModel) {
  const status = (item.companyEventStatus ?? item.status) as CompanyEventProposalStatus | undefined
  if (status != null && CompanyEventProposalStatusLabels[status]) {
    return t(CompanyEventProposalStatusLabels[status])
  }
  return t('common.unknown')
}

function reportStatusLabel(report: CompanyEventReportModel) {
  const status = (report.companyEventStatus ?? report.status) as CompanyEventProposalStatus | undefined
  if (status != null && CompanyEventProposalStatusLabels[status]) {
    return t(CompanyEventProposalStatusLabels[status])
  }
  return t('common.unknown')
}

const proposalStatusColorMap = computed(() => ({
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Draft])]: 'warning',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.PendingApproval])]: 'primary',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Approved])]: 'success',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Rejected])]: 'red'
}))

const proposalRows = computed(() =>
  companyProposals.value.map((proposal, index) => {
    const branch =
      selectedBranch.value ??
      branchAllocations.value.find(row =>
        String(row.companyId ?? '') === String(proposal.companyId ?? proposal.allocationDetailEvent?.companyId ?? '')
      )

    const allocationCode =
      branch?.allocationCode ?? proposal.allocationDetailEvent?.allocationEvent?.allocationCode ?? '-'
    const companyName = branch?.companyName ?? proposal.allocationDetailEvent?.company?.companyName ?? '-'
    const regionName = branch?.regionName ?? proposal.allocationDetailEvent?.region?.regionName ?? '-'
    const attachmentCount = proposal.attachments?.length ?? 0
    const attachmentLabel = t('allocationEvent.proposalList.fileCount', { count: attachmentCount })

    return {
      rowNumber: index + 1,
      allocationCode,
      companyEventCode: proposal.companyEventCode || '-',
      companyEventName: proposal.companyEventName || '-',
      companyName,
      regionName,
      eventDate: proposal.eventDate,
      totalAmount: proposal.totalAmount ?? 0,
      attachmentLabel,
      statusLabel: proposalStatusLabel(proposal),
      status: proposal.companyEventStatus ?? proposal.status,
      raw: proposal
    }
  })
)

const reportRows = computed(() => {
  const proposal = reportDialogProposal.value
  const branchName =
    selectedBranch.value?.companyName ??
    proposal?.allocationDetailEvent?.company?.companyName ??
    proposal?.companyEventName ??
    '-'
  const regionName =
    selectedBranch.value?.regionName ?? proposal?.allocationDetailEvent?.region?.regionName ?? '-'

  return reportStore.reports.map((report, index) => {
    const reportProposalCode =
      proposal?.companyEventCode ??
      report.companyEvent?.companyEventCode ??
      '-'
    const reportEventName =
      proposal?.companyEventName ??
      report.companyEvent?.companyEventName ??
      '-'
    const reportEventDate =
      proposal?.eventDate ?? report.eventDate ?? report.companyEvent?.eventDate ?? ''

    return {
      rowNumber: index + 1,
      proposalCode: reportProposalCode,
      reportCode: report.companyEventReportCode || '-',
      eventName: reportEventName,
      eventDate: reportEventDate,
      costActual: report.totalAmount ?? 0,
      branch: branchName,
      region: regionName,
      reportDate: report.reportDate ?? report.createdAt ?? '',
      statusLabel: reportStatusLabel(report),
      raw: report
    }
  })
})

const reportDialogDescription = computed(() => {
  if (!reportDialogProposal.value) return ''
  const code = reportDialogProposal.value.companyEventCode ?? '-'
  return t('allocationEvent.report.dialogSubTitle', { code })
})

function viewAllocation(row: BranchAllocationRow) {
  const target = allocationStore.allocationEvents.find(ev => ev.id === row.eventId)
  if (!target) return
  selectedEvent.value = target
  selectedBranch.value = row
  showDialog.value = true
}

function openProposal(row: BranchAllocationRow) {
  const target = allocationStore.allocationEvents.find(ev => ev.id === row.eventId)
  if (!target) return
  selectedEvent.value = target
  selectedBranch.value = row
  dialogProposal.value = null
  showProposalDialog.value = true
}

async function openAllocations(row: BranchAllocationRow) {
  selectedBranch.value = row
  const embedded = (row.details || []).flatMap(d => d.companyEvents || [])
  if (embedded.length) {
    companyProposals.value = embedded
    proposalsLoading.value = false
  } else {
    await loadCompanyProposals(String(row.companyId ?? ''))
  }
  showAllocationsDialog.value = true
}

function canViewReports(proposal: CompanyEventModel) {
  const status = proposal.companyEventStatus ?? proposal.status
  return status === CompanyEventProposalStatus.Approved
}

function canCreateReport(proposal: CompanyEventModel) {
  const status = proposal.companyEventStatus ?? proposal.status
  return status === CompanyEventProposalStatus.Approved
}

function attachDetailToProposal(proposal: CompanyEventModel) {
  if (!proposal) return proposal
  const branch = selectedBranch.value
  const detailId = proposal.allocationDetailEventId ?? proposal.allocationDetailEvent?.id
  const details = branch?.details ?? []
  const detailFromBranch =
    detailId != null
      ? details.find(d => String(d.id ?? '') === String(detailId))
      : details.length === 1
        ? details[0]
        : undefined
  const detail = proposal.allocationDetailEvent ?? detailFromBranch
  if (!detail) return proposal
  const allocationEvent =
    detail.allocationEvent ??
    allocationStore.allocationEvents.find(
      ev => String(ev.id ?? '') === String(branch?.eventId ?? detail.allocationEventId ?? '')
    ) ??
    (branch
      ? {
        id: String(branch.eventId ?? ''),
        allocationCode: branch.allocationCode ?? ''
      }
      : undefined)
  const company =
    detail.company ??
    (branch
      ? { id: branch.companyId, companyName: branch.companyName }
      : undefined)
  const region =
    detail.region ??
    (branch
      ? { id: branch.regionId, regionName: branch.regionName }
      : undefined)
  const detailAny = detail as any
  const event = detailAny.event ?? eventStore.events.find(ev => String(ev.id ?? '') === String(detail.eventId ?? ''))
  const allocationDetailEventId = proposal.allocationDetailEventId ?? detail.id
  return {
    ...proposal,
    allocationDetailEventId: allocationDetailEventId || undefined,
    allocationDetailEvent: {
      ...detailAny,
      eventId: detail.eventId ?? event?.id,
      allocationEvent,
      company,
      region,
      event
    }
  }
}

async function openReportList(proposal: CompanyEventModel) {
  if (!proposal?.id) return
  reportDialogProposal.value = attachDetailToProposal(proposal)
  showReportListDialog.value = true
  await reportStore.fetchByCompanyEventId(String(proposal.id))
}

function createReportFromProposal(proposal: CompanyEventModel) {
  reportDialogProposal.value = attachDetailToProposal(proposal)
  reportEditDialogReport.value = null
  reportEditDialogReadonly.value = false
  reportEditDialogVisible.value = true
}

function canEditReport(report: CompanyEventReportModel) {
  const status = report.companyEventStatus ?? report.status
  return status !== CompanyEventProposalStatus.Approved
}

function canMoveReportToPending(report: CompanyEventReportModel) {
  const status = report.companyEventStatus ?? report.status
  return status === CompanyEventProposalStatus.Draft || status === CompanyEventProposalStatus.Rejected
}

function editReportFromDialog(report: CompanyEventReportModel) {
  reportEditDialogReport.value = report
  reportEditDialogReadonly.value = false
  reportEditDialogVisible.value = true
}

async function markReportPending(report: CompanyEventReportModel) {
  if (!report?.id) return
  const payload: ApproveCompanyEventReportModel = {
    companyEventReportId: report.id,
    approveStatus: CompanyEventProposalStatus.PendingApproval,
    reason: ''
  }
  try {
    await companyEventService.updateReportStatusOnly(payload)
    notificationStore.showToast('success', { key: 'allocationEvent.proposal.pendingSuccess' })
    const companyEventId = report.companyEventId ?? reportDialogProposal.value?.id
    if (companyEventId) {
      await reportStore.fetchByCompanyEventId(String(companyEventId))
    }
  } catch (error) {
    console.error('Failed to move report to pending', error)
    notificationStore.showToast('error', { key: 'allocationEvent.proposal.pendingError' })
  }
}

async function handleReportSubmit(payload: CompanyEventReportModel) {
  if (reportEditDialogSubmitting.value) return
  reportEditDialogSubmitting.value = true
  try {
    const proposal = reportDialogProposal.value
    const companyEventId = payload.companyEventId ?? proposal?.id
    const reportPayload: CompanyEventReportModel = {
      ...payload,
      companyEventId: companyEventId ? String(companyEventId) : payload.companyEventId,
      companyEvent: undefined
    }
    if (payload.id) {
      await companyEventService.updateReport(reportPayload)
      notificationStore.showToast('success', { key: 'allocationEvent.report.updateSuccess' })
    } else {
      await companyEventService.createReport(reportPayload)
      notificationStore.showToast('success', { key: 'allocationEvent.report.createSuccess' })
    }
    reportEditDialogVisible.value = false
    if (companyEventId) {
      await reportStore.fetchByCompanyEventId(String(companyEventId))
    }
  } catch (error) {
    console.error('Report submission failed', error)
    notificationStore.showToast('error', { key: 'common.systemError' })
  } finally {
    reportEditDialogSubmitting.value = false
  }
}

function onExport() {
  notificationStore.showToast('info', { key: 'common.comingSoon' })
}

function canMoveToPending(proposal: CompanyEventModel) {
  const status = proposal.companyEventStatus ?? proposal.status
  return (
    status === CompanyEventProposalStatus.Draft ||
    status === CompanyEventProposalStatus.Rejected ||
    status === undefined ||
    status === null
  )
}

async function loadCompanyProposals(companyId: string) {
  proposalsLoading.value = true
  try {
    if (!companyId) {
      companyProposals.value = []
      return
    }
    const res = await companyEventService.fetchAllProposals()
    const items = res.data || []
    companyProposals.value = items.filter(
      p => String(p.companyId ?? p.allocationDetailEvent?.companyId ?? '') === companyId
    )
  } catch (err) {
    console.error('Failed to load company proposals', err)
    notificationStore.showToast('error', { key: 'common.systemError' })
    companyProposals.value = []
  } finally {
    proposalsLoading.value = false
  }
}

function editProposalFromDialog(proposal: CompanyEventModel) {
  const branch =
    selectedBranch.value ??
    branchAllocations.value.find(row =>
      String(row.companyId ?? '') === String(proposal.companyId ?? proposal.allocationDetailEvent?.companyId ?? '')
    )
  if (!branch) return

  const targetEvent = allocationStore.allocationEvents.find(ev => ev.id === branch.eventId)
  selectedBranch.value = branch
  selectedEvent.value = targetEvent ?? null
  dialogProposal.value = proposal
  showProposalDialog.value = true
}

async function markProposalPending(proposal: CompanyEventModel) {
  if (!proposal?.id) return
  const payload: ApproveCompanyEventModel = {
    companyEventId: proposal.id,
    approveStatus: CompanyEventProposalStatus.PendingApproval,
    reason: ''
  }
  proposalsLoading.value = true
  try {
    await companyEventService.updateStatusOnly(payload)
    notificationStore.showToast('success', { key: 'allocationEvent.proposal.pendingSuccess' })
    const companyId = String(
      selectedBranch.value?.companyId ??
      proposal.companyId ??
      proposal.allocationDetailEvent?.companyId ??
      ''
    )
    await loadCompanyProposals(companyId)
  } catch (err) {
    console.error('Failed to move proposal to pending', err)
    notificationStore.showToast('error', { key: 'allocationEvent.proposal.pendingError' })
  } finally {
    proposalsLoading.value = false
  }
}

async function handleProposalSubmit(payload: CompanyEventProposalRequest) {
  if (proposalSubmitting.value) return
  proposalSubmitting.value = true
  try {
    if (payload.companyEvent?.id) {
      await companyEventService.updateProposal(payload)
      notificationStore.showToast('success', { key: 'allocationEvent.proposalUpdateSuccess' })
    } else {
      await companyEventService.createProposal(payload)
      notificationStore.showToast('success', { key: 'allocationEvent.proposalSuccess' })
    }
    showProposalDialog.value = false
    await allocationStore.fetchAll()
    dialogProposal.value = null
    if (showAllocationsDialog.value && selectedBranch.value?.companyId) {
      await loadCompanyProposals(String(selectedBranch.value.companyId))
    }
  } catch (err) {
    console.error('Failed to create company event proposal', err)
    notificationStore.showToast('error', { key: 'common.systemError' })
  } finally {
    proposalSubmitting.value = false
  }
}

onMounted(async () => {
  if (!allocationStore.allocationEvents.length) {
    await allocationStore.fetchAllForCompany()
  }
  if (!regionStore.regions.length) {
    await regionStore.fetchAllRegions()
  }
  if (!companyStore.companies.length) {
    await companyStore.fetchAllCompanies()
  }
  if (!eventStore.events.length) {
    await eventStore.fetchAllEvents()
  }
})

watch(showProposalDialog, visible => {
  if (!visible) {
    dialogProposal.value = null
  }
})

watch(showReportListDialog, visible => {
  if (!visible) {
    reportStore.clear()
    reportDialogProposal.value = null
  }
})

watch(reportEditDialogVisible, visible => {
  if (!visible) {
    reportEditDialogReport.value = null
  }
})

function buildProposalPayload(
  proposal: CompanyEventModel,
  statusOverride?: CompanyEventProposalStatus
): CompanyEventProposalRequest {
  const detailId = proposal.allocationDetailEventId || proposal.allocationDetailEvent?.id || ''
  const companyId = proposal.companyId ?? proposal.allocationDetailEvent?.companyId
  const regionId = proposal.regionId ?? proposal.allocationDetailEvent?.regionId

  const publications = proposal.eventPublications ?? proposal.publications ?? []
  const cashes = proposal.eventCashes ?? proposal.cashCosts ?? []
  const participants = proposal.eventParticipants ?? proposal.participants ?? []
  const attachments = proposal.attachments ?? (proposal as any).attachmentModels ?? []

  return {
    companyEvent: {
      id: proposal.id,
      allocationDetailEventId: detailId,
      companyEventCode: proposal.companyEventCode,
      companyEventName: proposal.companyEventName,
      eventDate: proposal.eventDate,
      affiliatePartnerId: proposal.affiliatePartnerId,
      companyId: companyId ? String(companyId) : undefined,
      regionId: regionId ? String(regionId) : undefined,
      numberStudents: proposal.numberStudents,
      propose: proposal.propose,
      totalAmount: proposal.totalAmount,
      eventSize: proposal.eventSize,
      companyEventStatus: statusOverride ?? proposal.companyEventStatus ?? CompanyEventProposalStatus.Draft
    },
    publications: publications.map(pub => ({
      id: pub.id,
      itemId: pub.itemId,
      quantity: Number(pub.quantity || 0),
      publicationAmount: Number(pub.publicationAmount || 0),
      totalAmount: Number(pub.totalAmount || 0)
    })),
    cashCosts: cashes.map(cash => ({
      id: cash.id,
      cashName: cash.cashName,
      quantity: Number(cash.quantity || 0),
      amount: Number(cash.amount || 0),
      totalAmount: Number(cash.totalAmount || 0)
    })),
    participants: participants.map(p => ({
      id: p.id ?? undefined,
      isStudent: p.isStudent,
      participantName: p.participantName,
      participantGender: p.participantGender,
      participantDateOfBirth: p.participantDateOfBirth,
      participantAddress: p.participantAddress,
      participantPhoneNumber: p.participantPhoneNumber,
      participantContact: p.participantContact,
      participantEmail: p.participantEmail,
      participantSchool: p.participantSchool,
      participantSourceKnown: p.participantSourceKnown,
      participantJob: p.participantJob,
      employeeId: p.employeeId
    })),
    attachments: attachments.map(att => {
      const resolvedPath = att.filePath ?? (att as any).path ?? att.relativePath ?? ''
      return {
        id: att.id,
        fileName: att.fileName,
        filePath: resolvedPath,
        relativePath: att.relativePath ?? resolvedPath,
        size: att.size,
        contentType: att.contentType
      }
    }),
    deletedAttachmentIds: [],
    deletedPublicationIds: [],
    deletedCashIds: [],
    deletedParticipantIds: [],
    proposalQuantity: (proposal as any).proposalQuantity ?? 1
  }
}
</script>

<style scoped>
.company-allocation-page {
  padding-bottom: 40px;
}

.allocations-table-wrapper {
  pointer-events: auto !important;
  opacity: 1 !important;
}

.allocations-table-wrapper :deep(.el-button) {
  pointer-events: auto !important;
  opacity: 1 !important;
}
</style>
