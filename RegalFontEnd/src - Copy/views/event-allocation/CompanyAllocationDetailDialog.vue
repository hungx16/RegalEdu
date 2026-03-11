<template>
  <el-dialog :model-value="visible" width="960px" class="company-allocation-dialog" @close="close">
    <template #header>
      <div class="d-flex justify-content-between align-items-center w-100">
        <div>
          <div class="fw-semibold text-muted fs-8 mb-1">{{ t('allocationEvent.companyDetail.title') }}</div>
          <div class="d-flex align-items-center gap-2">
            <h4 class="mb-0">{{ branchRow?.companyName || '-' }}</h4>
            <BaseBadge v-if="allocationEvent" :label="statusLabel(allocationEvent.allocationEventStatus)"
              :colorByLabelMap="statusColorMap" />
          </div>
        </div>
        <span class="text-muted fs-8">{{ t('allocationEvent.companyDetail.subtitle') }}</span>
      </div>
    </template>

    <div v-if="branchRow && allocationEvent" class="detail-body">
      <div class="detail-grid mb-5">
        <div>
          <label>{{ t('allocationEvent.code') }}</label>
          <div class="value">{{ allocationEvent.allocationCode || '-' }}</div>
        </div>
        <div>
          <label>{{ t('allocationEvent.companyDetail.month') }}</label>
          <div class="value">{{ branchRow.monthLabel }}</div>
        </div>
        <div>
          <label>{{ t('allocationEvent.companyDetail.company') }}</label>
          <div class="value">{{ branchRow.companyName }}</div>
        </div>
        <div>
          <label>{{ t('allocationEvent.companyDetail.region') }}</label>
          <div class="value">{{ branchRow.regionName }}</div>
        </div>
        <div>
          <label>{{ t('allocationEvent.companyDetail.noAllocation') }}</label>
          <div class="value">
            <el-tag :type="branchRow.noAllocation ? 'danger' : 'success'" effect="plain">
              {{ branchRow.noAllocation ? t('common.no') : t('common.yes') }}
            </el-tag>
          </div>
        </div>
        <div class="text-end">
          <label>{{ t('allocationEvent.companyDetail.totalBudget') }}</label>
          <div class="value text-primary fw-bold">{{ formatCurrency(allocationBudget) }}</div>
        </div>
        <div class="text-end">
          <label>{{ t('allocationEvent.companyDetail.proposedBudget') }}</label>
          <div class="value">{{ formatCurrency(proposedBudget) }}</div>
        </div>
      </div>

      <div class="mb-4">
        <div class="d-flex justify-content-between align-items-center mb-2">
          <h5 class="fw-semibold mb-0">{{ t('allocationEvent.companyDetail.eventTableTitle') }}</h5>
          <span class="text-muted fs-8">{{ t('allocationEvent.companyDetail.eventTableNote') }}</span>
        </div>
        <BaseTable :columns="eventColumns" :items="eventRows" :showCheckboxColumn="false" :showActionsColumn="false"
          :height="260" :loading="false" />
      </div>

      <!-- <div class="mb-4">
        <div class="fw-semibold mb-1">{{ t('common.note') }}</div>
        <div class="text-muted">
          {{ t('allocationEvent.noteForMonth', { month: allocationEvent.allocationMonth, year: allocationEvent.allocationYear }) }}
        </div>
      </div> -->

      <div>
        <div class="fw-semibold mb-2">{{ t('allocationEvent.companyDetail.historyTitle') }}</div>
        <BaseTable :columns="historyColumns" :items="historyRows" :showCheckboxColumn="false" :showActionsColumn="false"
          :height="260" />
        <div v-if="historyRows.length === 0" class="text-center text-muted py-3 fs-8">
          {{ t('common.noData') }}
        </div>
      </div>
    </div>
    <template #footer>
      <div class="d-flex justify-content-end gap-3">
        <el-button @click="close">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" plain>{{ t('allocationEvent.companyDetail.proposeButton') }}</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { formatCurrency } from '@/utils/format'
import { AllocationEventStatus, AllocationEventStatusLabels } from '@/types'
import { useEventStore } from '@/stores/eventStore'
import type { AllocationDetailEventModel, AllocationEventModel } from '@/api/AllocationEventApi'
import dayjs from 'dayjs'

interface BranchRow {
  id: string
  companyName: string
  regionName: string
  monthLabel: string
  noAllocation: boolean
  details: AllocationDetailEventModel[]
}

const props = defineProps<{
  visible: boolean
  branchRow: BranchRow | null
  allocationEvent: AllocationEventModel | null
}>()

const emit = defineEmits(['update:visible'])
const { t } = useI18n()
const eventStore = useEventStore()

const statusColorMap = computed(() => ({
  [t(AllocationEventStatusLabels[AllocationEventStatus.Draft])]: 'warning',
  [t(AllocationEventStatusLabels[AllocationEventStatus.Allocated])]: 'primary',
  [t(AllocationEventStatusLabels[AllocationEventStatus.Completed])]: 'success',
  [t(AllocationEventStatusLabels[AllocationEventStatus.Cancelled])]: 'danger'
}))

const visible = computed(() => props.visible)

const eventColumns: BaseTableColumn[] = [
  { key: 'index', labelKey: 'common.index', width: 80, align: 'center' },
  { key: 'eventName', labelKey: 'allocationEvent.companyDetail.eventName' },
  { key: 'quantity', labelKey: 'allocationEvent.companyDetail.quantity', width: 140, align: 'center' },
  { key: 'budget', labelKey: 'allocationEvent.companyDetail.budget', width: 180, align: 'right' }
]

const historyColumns: BaseTableColumn[] = [
  { key: 'time', labelKey: 'allocationEvent.history.time', width: 200 },
  { key: 'target', labelKey: 'allocationEvent.history.target' },
  { key: 'action', labelKey: 'allocationEvent.history.action', width: 180 },
  { key: 'description', labelKey: 'allocationEvent.history.description' },
  { key: 'actor', labelKey: 'allocationEvent.history.actor', width: 160 }
]

const eventRows = computed(() => {
  if (!props.branchRow) return []
  const eventNameMap = new Map<string, string>()
  eventStore.events.forEach(ev => eventNameMap.set(String(ev.id), ev.eventName))
  return props.branchRow.details.map((detail, idx) => ({
    index: idx + 1,
    eventName: eventNameMap.get(String(detail.eventId)) || '-',
    quantity: detail.quantity ?? 0,
    budget: formatCurrency(detail.budget ?? 0)
  }))
})

const allocationBudget = computed(() => props.allocationEvent?.eventBudget ?? 0)
const proposedBudget = computed(() => {
  if (!props.branchRow) return 0
  return props.branchRow.details.reduce((sum, detail) => sum + Number(detail.budget || 0), 0)
})

const historyRows = computed(() => {
  if (!props.allocationEvent) return []
  const branchName = props.branchRow?.companyName ?? ''
  return (props.allocationEvent.allocationEventHistories || [])
    .filter(h => !branchName || !h.targetName || h.targetName === branchName)
    .slice()
    .sort((a, b) => {
      const av = a.createdAt ? dayjs(a.createdAt).valueOf() : 0
      const bv = b.createdAt ? dayjs(b.createdAt).valueOf() : 0
      return av - bv
    })
    .map(h => ({
      time: h.createdAt ? dayjs(h.createdAt).format('HH:mm:ss DD/MM/YYYY') : '-',
      target: h.targetName || '-',
      action: h.actionName || '-',
      description: h.description || '-',
      actor: h.createdByName || h.createdBy || '-'
    }))
})

function statusLabel(status?: number | null) {
  const key = AllocationEventStatusLabels[status as AllocationEventStatus]
  return key ? t(key) : t('common.unknown')
}

function close() {
  emit('update:visible', false)
}
</script>

<style scoped>
.company-allocation-dialog :deep(.el-dialog__body) {
  padding: 10px 24px 5px;
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
}

.detail-grid label {
  font-size: 12px;
  color: var(--el-text-color-secondary);
  text-transform: uppercase;
}

.detail-grid .value {
  font-weight: 600;
  color: var(--el-text-color-primary);
}
</style>
