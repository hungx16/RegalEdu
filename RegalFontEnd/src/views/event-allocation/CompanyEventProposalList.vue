<template>
  <div class="company-proposal-list">
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-start align-items-md-center mb-6">
      <div>
        <h2 class="fw-bold mb-1">{{ t('allocationEvent.proposalList.header') }}</h2>
        <p class="text-muted mb-0">{{ t('allocationEvent.proposalList.desc') }}</p>
      </div>
      <el-button type="primary" @click="emitCreate">
        <el-icon class="me-1">
          <Plus />
        </el-icon>
        {{ t('allocationEvent.proposalList.createButton') }}
      </el-button>
    </div>

    <el-card>
      <div class="d-flex justify-content-between align-items-center mb-3">
        <div>
          <h5 class="fw-semibold mb-1">{{ t('allocationEvent.proposalList.tableTitle') }}</h5>
          <span class="text-muted fs-8">{{ t('allocationEvent.proposalList.tableDesc') }}</span>
        </div>
        <el-input v-model="searchKeyword" :placeholder="t('allocationEvent.proposalList.searchPlaceholder')"
          suffix-icon="Search" style="max-width: 260px;" />
      </div>

      <BaseTable :columns="columns" :items="filteredItems" :loading="loading" :showCheckboxColumn="false"
        :disable-row-dbl-click="true" :showActionsColumn="true" :showView="true" :showEdit="false" :height="520"
        :actionsColumnWidth="180" @view="viewProposal" @edit="editProposal">
        <template #cell-eventDate="{ item }">
          {{ formatDate(item.eventDate) }}
        </template>
        <template #cell-totalAmount="{ item }">
          {{ formatCurrency(item.totalAmount) }}
        </template>
        <template #cell-attachments="{ item }">
          {{ item.attachmentCount || 0 }} file(s)
        </template>
        <template #cell-status="{ item }">
          <BaseBadge :label="statusLabel(item)" :colorByLabelMap="statusColorMap" />
        </template>
        <template #actions="{ item }">
          <el-tooltip v-if="canApprove(item) && !isCompanyManager" :content="t('common.approve')" placement="top">
            <el-button circle size="small" type="success"
              @click.stop="openApprove(item, CompanyEventProposalStatus.Approved)">
              <el-icon>
                <CircleCheck />
              </el-icon>
            </el-button>
          </el-tooltip>
          <el-tooltip v-if="canApprove(item) && !isCompanyManager" :content="t('common.reject')" placement="top">
            <el-button circle size="small" type="danger"
              @click.stop="openApprove(item, CompanyEventProposalStatus.Rejected)">
              <el-icon>
                <CircleClose />
              </el-icon>
            </el-button>
          </el-tooltip>
        </template>
      </BaseTable>
    </el-card>
    <CompanyEventProposalDialog :visible="dialogVisible" :allocation-event="dialogAllocationEvent"
      :branch-row="dialogBranchRow" :proposal="dialogProposal" :submitting="dialogSubmitting" :readonly="dialogReadonly"
      :show-approval-actions="true" @update:visible="dialogVisible = $event" @submit="onDialogSubmit" />

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
import { ref, computed, onMounted, reactive } from 'vue'
import { useI18n } from 'vue-i18n'
import { Plus, CircleCheck, CircleClose } from '@element-plus/icons-vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { CompanyEventService } from '@/services/CompanyEventService'
import type { CompanyEventModel, AllocationEventModel, AllocationDetailEventModel } from '@/api/AllocationEventApi'
import { CompanyEventProposalStatus, CompanyEventProposalStatusLabels } from '@/types'
import { formatCurrency, formatDate } from '@/utils/format'
import CompanyEventProposalDialog from './CompanyEventProposalDialog.vue'
import { useNotificationStore } from '@/stores/notificationStore'
import type { CompanyEventProposalRequest } from '@/api/AllocationEventApi'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import { useEmployeeStore } from '@/stores/employeeStore'
const employeeStore = useEmployeeStore()
const { t } = useI18n()
const emit = defineEmits(['create', 'view', 'edit'])

const service = new CompanyEventService()
const loading = ref(false)
const proposals = ref<CompanyEventModel[]>([])
const searchKeyword = ref('')
const dialogVisible = ref(false)
const dialogAllocationEvent = ref<AllocationEventModel | null>(null)
const dialogBranchRow = ref<any>(null)
const dialogProposal = ref<CompanyEventModel | null>(null)
const dialogSubmitting = ref(false)
const dialogReadonly = ref(false)
const approveDialogVisible = ref(false)
const approveForm = reactive({ reason: '' })
const approveTarget = ref<CompanyEventModel | null>(null)
const approveStatus = ref<CompanyEventProposalStatus | null>(null)
const notificationStore = useNotificationStore()
const approveLoading = ref(false)
const isCompanyManager = ref(false)
const approveSubmitDisabled = computed(() => {
  if (approveStatus.value === CompanyEventProposalStatus.Rejected) {
    return !approveForm.reason.trim()
  }
  return false
})

const columns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center' },
  { key: 'allocationCodeDisplay', labelKey: 'allocationEvent.code', width: 180 },
  { key: 'proposalCodeDisplay', labelKey: 'allocationEvent.proposal.proposalCode' },
  { key: 'companyEventNameDisplay', labelKey: 'allocationEvent.proposalList.eventName' },
  { key: 'companyNameDisplay', labelKey: 'allocationEvent.branches', width: 180 },
  { key: 'regionNameDisplay', labelKey: 'allocationEvent.region', width: 140 },
  { key: 'eventDate', labelKey: 'allocationEvent.proposal.eventDate', width: 140 },
  { key: 'totalAmount', labelKey: 'allocationEvent.proposal.totalProjectedCost', width: 160, align: 'right' },
  { key: 'attachments', labelKey: 'allocationEvent.proposalList.attachments', width: 160, align: 'center' },
  { key: 'status', labelKey: 'common.status', width: 160, align: 'center' }
]

const statusColorMap = computed(() => ({
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Draft])]: 'warning',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.PendingApproval])]: 'primary',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Approved])]: 'success',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Rejected])]: 'red'
}))

const normalizedItems = computed(() =>
  proposals.value.map((item, idx) => {
    const allocationCode = item.allocationDetailEvent?.allocationEvent?.allocationCode
    const companyName = item.allocationDetailEvent?.company?.companyName
    const regionName = item.allocationDetailEvent?.region?.regionName
    const attachmentCount = (item as any).attachmentModels?.length ?? item.attachments?.length ?? 0
    return {
      ...item,
      rowNumber: idx + 1,
      allocationCodeDisplay: allocationCode || '-',
      proposalCodeDisplay: item.companyEventCode || '-',
      companyEventNameDisplay: item.companyEventName || '-',
      companyNameDisplay: companyName || '-',
      regionNameDisplay: regionName || '-',
      attachmentCount
    }
  })
)

const filteredItems = computed(() => {
  const keyword = searchKeyword.value.trim().toLowerCase()
  if (!keyword) return normalizedItems.value
  return normalizedItems.value.filter(item =>
    [item.allocationCodeDisplay, item.proposalCodeDisplay, item.companyEventNameDisplay, item.companyNameDisplay, item.regionNameDisplay]
      .filter(Boolean)
      .some(val => String(val).toLowerCase().includes(keyword))
  )
})

function statusLabel(item: CompanyEventModel) {
  const status = (item.companyEventStatus ?? item.status) as CompanyEventProposalStatus | undefined
  if (status != null && CompanyEventProposalStatusLabels[status]) {
    return t(CompanyEventProposalStatusLabels[status])
  }
  return t('common.unknown')
}

function emitCreate() {
  emit('create')
}
function viewProposal(item: CompanyEventModel) {
  emit('view', item)
  dialogReadonly.value = true
  openDialog(item)
}
function editProposal(item: CompanyEventModel) {
  emit('edit', item)
  dialogReadonly.value = false
  openDialog(item)
}

function openDialog(item: CompanyEventModel) {
  // Prefer the original data from service (includes publications/cashes/participants)
  const original = proposals.value.find(p => p.id === item.id) || item
  dialogProposal.value = { ...original }
  const detail = item.allocationDetailEvent as AllocationDetailEventModel | undefined
  if (!detail) {
    dialogAllocationEvent.value = null
    dialogBranchRow.value = null
    dialogVisible.value = true
    return
  }
  const allocation = detail.allocationEvent as AllocationEventModel | undefined
  dialogAllocationEvent.value = allocation
    ? {
      ...allocation,
      allocationDetails: allocation.allocationDetails || [detail]
    }
    : {
      id: detail.allocationEventId,
      allocationCode: (allocation as AllocationEventModel | undefined)?.allocationCode || '',
      allocationMonth: (allocation as AllocationEventModel | undefined)?.allocationMonth || 0,
      allocationYear: (allocation as AllocationEventModel | undefined)?.allocationYear || 0,
      eventBudget: (allocation as AllocationEventModel | undefined)?.eventBudget || detail.budget || 0,
      allocationEventStatus: (allocation as AllocationEventModel | undefined)?.allocationEventStatus || 0,
      allocationDetails: [detail]
    } as AllocationEventModel

  dialogBranchRow.value = {
    companyId: detail.companyId,
    companyName: detail.company?.companyName || '',
    regionId: detail.regionId,
    regionName: detail.region?.regionName || '',
    allocationCode: dialogAllocationEvent.value?.allocationCode || '',
    totalBudget: detail.budget || dialogAllocationEvent.value?.eventBudget || 0,
    details: dialogAllocationEvent.value?.allocationDetails || [detail]
  }
  dialogVisible.value = true
}

function openApprove(item: CompanyEventModel, status: CompanyEventProposalStatus) {
  approveTarget.value = item
  approveStatus.value = status
  approveForm.reason = ''
  approveDialogVisible.value = true
}

async function onDialogSubmit(payload: CompanyEventProposalRequest) {
  if (!payload?.companyEvent?.id) {
    dialogVisible.value = false
    return
  }
  dialogSubmitting.value = true
  try {
    await service.updateProposal(payload)
    notificationStore.showToast('success', { key: 'allocationEvent.proposalUpdateSuccess' })
    dialogVisible.value = false
    const res = await service.fetchAllProposals()
    proposals.value = res.data || []
  } catch (err) {
    console.error('Failed to update proposal', err)
    notificationStore.showToast('error', { key: 'allocationEvent.proposal.updateError' })
  } finally {
    dialogSubmitting.value = false
  }
}

const canApprove = (item: CompanyEventModel) => {
  const status = (item.companyEventStatus ?? item.status) as CompanyEventProposalStatus | undefined
  return status === CompanyEventProposalStatus.PendingApproval || status === CompanyEventProposalStatus.Draft
}

async function submitApproval() {
  if (!approveTarget.value || approveStatus.value == null) return
  try {
    approveLoading.value = true
    await service.approveProposal({
      companyEventId: approveTarget.value.id!,
      approveStatus: approveStatus.value,
      reason: approveForm.reason
    })
    notificationStore.showToast('success', {
      key: approveStatus.value === CompanyEventProposalStatus.Approved
        ? 'allocationEvent.proposal.approveSuccess'
        : 'allocationEvent.proposal.rejectSuccess'
    })
    approveDialogVisible.value = false
    const res = await service.fetchAllProposals()
    proposals.value = res.data || []
  } catch (err) {
    console.error('Failed to approve/reject', err)
    notificationStore.showToast('error', {
      key: approveStatus.value === CompanyEventProposalStatus.Approved
        ? 'allocationEvent.proposal.approveError'
        : 'allocationEvent.proposal.rejectError'
    })
  } finally {
    approveLoading.value = false
  }
}

onMounted(async () => {
  loading.value = true
  try {
    const res = await service.fetchAllProposals()
    proposals.value = res.data || []
    isCompanyManager.value = await employeeStore.checkIsCompanyManager()
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.company-proposal-list {
  padding-bottom: 40px;
}
</style>
