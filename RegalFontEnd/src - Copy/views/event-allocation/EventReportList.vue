<template>
  <div class="event-report-list">
    <div class="d-flex justify-content-between align-items-start mb-3 flex-wrap gap-3">
      <div>
        <h3 class="fw-semibold mb-1">{{ t('allocationEvent.report.title') }}</h3>
        <div class="text-muted fs-8">{{ t('allocationEvent.report.subTitle') }}</div>
      </div>
    </div>

    <div class="d-flex justify-content-start mb-3 gap-3 flex-wrap">
      <el-input size="small" class="flex-grow-1" v-model="searchKeyword"
        :placeholder="t('allocationEvent.report.searchPlaceholder')" clearable>
        <template #prefix>
          <el-icon>
            <Search />
          </el-icon>
        </template>
      </el-input>
    </div>

    <BaseTable :columns="reportColumns" :items="filteredReports" :loading="loading" :showCheckboxColumn="false"
      :showActionsColumn="true" :showView="true" :showEdit="false" :disable-row-dbl-click="true"
      :actionsColumnWidth="180" :height="520" @view="viewReport">
      <template #cell-rowNumber="{ item }">
        {{ filteredReports.indexOf(item) + 1 }}
      </template>
      <template #cell-proposalCode="{ item }">
        {{ item.companyEvent?.companyEventCode ?? item.companyEventCode }}
      </template>
      <template #cell-reportCode="{ item }">
        {{ formatReportCode(item) }}
      </template>
      <template #cell-eventName="{ item }">
        {{ item.companyEvent?.companyEventName ?? item.companyEventName }}
      </template>
      <template #cell-eventDate="{ item }">
        {{ formatDate(item.eventDate ?? item.companyEvent?.eventDate) }}
      </template>
      <template #cell-costActual="{ item }">
        {{ formatCurrency(item.totalAmount ?? item.companyEvent?.totalAmount ?? 0) }}
      </template>
      <template #cell-branch="{ item }">
        {{ resolveBranchName(item) }}
      </template>
      <template #cell-region="{ item }">
        {{ resolveRegionName(item) }}
      </template>
      <template #cell-reportDate="{ item }">
        {{ formatDate(item.reportDate ?? item.createdAt) }}
      </template>
      <template #cell-status="{ item }">
        <BaseBadge :label="statusLabel(item.companyEventStatus ?? item.status ?? item.companyEvent?.companyEventStatus)"
          :color-by-label-map="statusColorMap" />
      </template>
      <template #actions="{ item }">
        <el-tooltip v-if="canApprove(item) && !isCompanyManager" :content="t('common.approve')" placement="top">
          <el-button circle size="small"
            @click.stop="openApprove(item, CompanyEventProposalStatus.Approved)">
            <el-icon>
              <CircleCheck />
            </el-icon>
          </el-button>
        </el-tooltip>
        <el-tooltip v-if="canApprove(item) && !isCompanyManager" :content="t('common.reject')" placement="top">
          <el-button circle size="small"
            @click.stop="openApprove(item, CompanyEventProposalStatus.Rejected)">
            <el-icon>
              <CircleClose />
            </el-icon>
          </el-button>
        </el-tooltip>
      </template>
    </BaseTable>
    <CompanyEventReportDialog :visible="reportDialogVisible" :proposal="reportDialogProposal"
      :proposals="approvedProposals" :report="reportDialogReport" :submitting="reportDialogSubmitting"
      :readonly="reportDialogReadonly" @update:visible="reportDialogVisible = $event" @submit="handleReportSubmit" />
    <BaseDialogForm :visible="approveDialogVisible"
      :title="approveStatus === CompanyEventProposalStatus.Approved ? t('common.approve') : t('common.reject')"
      :mode="'edit'" :form-data="approveForm" :show-delete="false" :loading="approveLoading"
      :submit-disabled="approveSubmitDisabled" width="520" :height="'auto'"
      @update:visible="approveDialogVisible = $event" @submit="submitApproval">
      <template #form>
        <el-form label-position="top">
          <el-form-item :label="t('allocationEvent.proposal.approveReason')"
            :required="approveStatus === CompanyEventProposalStatus.Rejected">
            <el-input type="textarea" :rows="3" v-model="approveForm.reason"
              :placeholder="t('allocationEvent.proposal.approveReasonPlaceholder')" />
          </el-form-item>
        </el-form>
      </template>
    </BaseDialogForm>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { CircleCheck, CircleClose, Search } from '@element-plus/icons-vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import CompanyEventReportDialog from '@/views/event-allocation/CompanyEventReportDialog.vue'
import { CompanyEventService } from '@/services/CompanyEventService'
import type { CompanyEventModel, CompanyEventReportModel, ApproveCompanyEventReportModel } from '@/api/AllocationEventApi'
import { formatCurrency } from '@/utils/format'
import { useNotificationStore } from '@/stores/notificationStore'
import { CompanyEventProposalStatus, CompanyEventProposalStatusLabels } from '@/types'
import { useEmployeeStore } from '@/stores/employeeStore'

const { t } = useI18n()
const companyEventService = new CompanyEventService()
const notificationStore = useNotificationStore()
const employeeStore = useEmployeeStore()

const reports = ref<CompanyEventReportModel[]>([])
const proposals = ref<CompanyEventModel[]>([])
const loading = ref(false)
const searchKeyword = ref('')
const reportDialogVisible = ref(false)
const reportDialogProposal = ref<CompanyEventModel | null>(null)
const reportDialogReport = ref<CompanyEventReportModel | null>(null)
const reportDialogSubmitting = ref(false)
const reportDialogReadonly = ref(false)
const approveDialogVisible = ref(false)
const approveForm = reactive({ reason: '' })
const approveTarget = ref<CompanyEventReportModel | null>(null)
const approveStatus = ref<CompanyEventProposalStatus | null>(null)
const approveLoading = ref(false)
const isCompanyManager = ref(false)
const approveSubmitDisabled = computed(() => {
  if (approveStatus.value === CompanyEventProposalStatus.Rejected) {
    return !approveForm.reason.trim()
  }
  return false
})

const reportColumns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center', sticky: true },
  { key: 'reportCode', labelKey: 'allocationEvent.report.reportCode', width: 140, align: 'center', sticky: true },

  { key: 'proposalCode', labelKey: 'allocationEvent.report.proposalCode', width: 220 },
  { key: 'eventName', labelKey: 'allocationEvent.report.eventName' },
  { key: 'eventDate', labelKey: 'allocationEvent.report.eventDate', width: 140, align: 'center' },
  { key: 'costActual', labelKey: 'allocationEvent.report.costActual', width: 140, align: 'right' },
  { key: 'branch', labelKey: 'allocationEvent.report.branch', width: 200 },
  { key: 'region', labelKey: 'allocationEvent.region', width: 160 },
  { key: 'reportDate', labelKey: 'allocationEvent.report.reportDate', width: 160, align: 'center' },
  { key: 'status', labelKey: 'allocationEvent.report.status', width: 140, align: 'center' }
]

const approvedProposals = computed(() =>
  proposals.value.filter(
    proposal =>
      (proposal.companyEventStatus ?? proposal.status) === CompanyEventProposalStatus.Approved
  )
)

const filteredReports = computed(() => {
  const query = searchKeyword.value.trim().toLowerCase()
  if (!query) return reports.value
  return reports.value.filter(report => {
    const proposal = report.companyEvent
    const reportCode = formatReportCode(report)
    const branchName = resolveBranchName(report)
    const regionName = resolveRegionName(report)
    const status = statusLabel(report.companyEventStatus ?? report.status ?? proposal?.companyEventStatus)
    return (
      (proposal?.companyEventCode ?? '').toLowerCase().includes(query) ||
      reportCode.toLowerCase().includes(query) ||
      (proposal?.companyEventName ?? '').toLowerCase().includes(query) ||
      branchName.toLowerCase().includes(query) ||
      regionName.toLowerCase().includes(query) ||
      status.toLowerCase().includes(query)
    )
  })
})

const statusColorMap = {
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.PendingApproval])]: 'warning',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Draft])]: 'warning',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Approved])]: 'success',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Rejected])]: 'red'
}

function statusLabel(status: CompanyEventProposalStatus | number | undefined) {
  const key = status != null ? CompanyEventProposalStatusLabels[status as CompanyEventProposalStatus] : undefined
  if (!key) return t('common.unknown')
  return t(key)
}

function formatReportCode(report: CompanyEventReportModel) {
  return report.companyEventReportCode ?? '-'
}

function resolveProposalForReport(report: CompanyEventReportModel) {
  if (report.companyEvent) return report.companyEvent
  const reportEventId = String(report.companyEventId ?? '')
  if (!reportEventId) return null
  return proposals.value.find(p => String(p.id ?? '') === reportEventId) ?? null
}

function resolveBranchName(report: CompanyEventReportModel) {
  const proposal = resolveProposalForReport(report)
  return (
    proposal?.allocationDetailEvent?.company?.companyName ??
    (proposal as any)?.company?.companyName ??
    (report.companyEvent as any)?.company?.companyName ??
    (report as any)?.company?.companyName ??
    '-'
  )
}

function resolveRegionName(report: CompanyEventReportModel) {
  const proposal = resolveProposalForReport(report)
  return (
    proposal?.allocationDetailEvent?.region?.regionName ??
    (proposal as any)?.region?.regionName ??
    (report.companyEvent as any)?.region?.regionName ??
    (report as any)?.region?.regionName ??
    '-'
  )
}

function formatDate(value?: string) {
  if (!value) return ''
  return new Date(value).toLocaleDateString()
}
async function loadReports() {
  loading.value = true
  try {
    const res = await companyEventService.fetchAllReports()
    reports.value = res.data ?? []
  } catch (error) {
    console.error('Load event reports failed', error)
    notificationStore.showToast('error', { key: 'common.systemError' })
  } finally {
    loading.value = false
  }
}

async function loadProposals() {
  try {
    const res = await companyEventService.fetchAllProposals()
    proposals.value = res.data ?? []
  } catch (error) {
    console.error('Load event proposals failed', error)
    notificationStore.showToast('error', { key: 'common.systemError' })
  }
}

function openReportDialogFromRow(report: CompanyEventReportModel, readonly = false) {
  reportDialogReport.value = report
  const proposalFromList = proposals.value.find(
    p => String(p.id ?? '') === String(report.companyEventId ?? report.companyEvent?.id ?? '')
  )
  reportDialogProposal.value = proposalFromList ?? report.companyEvent ?? null
  reportDialogReadonly.value = readonly
  reportDialogVisible.value = true
}

function viewReport(report: CompanyEventReportModel) {
  openReportDialogFromRow(report, true)
}

const canApprove = (item: CompanyEventReportModel) => {
  const status = (item.companyEventStatus ?? item.status ?? item.companyEvent?.companyEventStatus) as
    | CompanyEventProposalStatus
    | undefined
  return status === CompanyEventProposalStatus.PendingApproval || status === CompanyEventProposalStatus.Draft
}

function openApprove(item: CompanyEventReportModel, status: CompanyEventProposalStatus) {
  approveTarget.value = item
  approveStatus.value = status
  approveForm.reason = ''
  approveDialogVisible.value = true
}

async function submitApproval() {
  if (!approveTarget.value || approveStatus.value == null) return
  try {
    approveLoading.value = true
    await companyEventService.approveReport({
      companyEventReportId: approveTarget.value.id!,
      approveStatus: approveStatus.value,
      reason: approveForm.reason
    } as ApproveCompanyEventReportModel)
    notificationStore.showToast('success', {
      key: approveStatus.value === CompanyEventProposalStatus.Approved
        ? 'allocationEvent.proposal.approveSuccess'
        : 'allocationEvent.proposal.rejectSuccess'
    })
    approveDialogVisible.value = false
    await loadReports()
  } catch (err) {
    console.error('Failed to approve/reject report', err)
    notificationStore.showToast('error', {
      key: approveStatus.value === CompanyEventProposalStatus.Approved
        ? 'allocationEvent.proposal.approveError'
        : 'allocationEvent.proposal.rejectError'
    })
  } finally {
    approveLoading.value = false
  }
}

async function handleReportSubmit(payload: CompanyEventReportModel) {
  if (reportDialogSubmitting.value) return
  reportDialogSubmitting.value = true
  try {
    const resolvedProposalId =
      payload.companyEventId || reportDialogProposal.value?.id || reportDialogReport.value?.companyEventId
    const reportPayload: CompanyEventReportModel = {
      ...payload,
      companyEventId: String(resolvedProposalId ?? ''),
      companyEvent: undefined
    }
    if (payload.id) {
      await companyEventService.updateReport(reportPayload)
      notificationStore.showToast('success', { key: 'allocationEvent.report.updateSuccess' })
    } else {
      await companyEventService.createReport(reportPayload)
      notificationStore.showToast('success', { key: 'allocationEvent.report.createSuccess' })
    }
    reportDialogVisible.value = false
    await loadReports()
  } catch (error) {
    console.error('Report submission failed', error)
    notificationStore.showToast('error', { key: 'common.systemError' })
  } finally {
    reportDialogSubmitting.value = false
  }
}

watch(reportDialogVisible, visible => {
  if (!visible) {
    reportDialogProposal.value = null
    reportDialogReadonly.value = false
    reportDialogReport.value = null
  }
})

onMounted(() => {
  loadReports()
  loadProposals()
  employeeStore.checkIsCompanyManager().then(result => {
    isCompanyManager.value = result
  })
})
</script>

<style scoped>
.event-report-list {
  background: #fff;
  padding: 24px;
  border-radius: 12px;
}

.report-header-actions {
  gap: 0.75rem;
}
</style>
