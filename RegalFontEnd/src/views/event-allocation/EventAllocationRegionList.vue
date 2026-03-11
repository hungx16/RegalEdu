<template>
  <div class="region-allocation-page">
    <div class="d-flex flex-column flex-md-row justify-content-between align-items-start align-items-md-center mb-6">
      <div>
        <h2 class="fw-bold mb-1">{{ t('allocationEvent.regionListHeader') }}</h2>
        <p class="text-muted mb-0">{{ t('allocationEvent.regionListDesc') }}</p>
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
          <label class="form-label d-block mb-1">{{ t('allocationEvent.regionFilter.searchLabel') }}</label>
          <el-input v-model="searchKeyword" :placeholder="t('allocationEvent.regionFilter.searchPlaceholder')"
            clearable />
        </div>
        <div class="col-12 col-md-4">
          <label class="form-label d-block mb-1">{{ t('allocationEvent.regionFilter.monthLabel') }}</label>
          <el-select v-model="monthFilter" class="w-100">
            <el-option value="all" :label="t('allocationEvent.regionFilter.monthAll')" />
            <el-option v-for="opt in monthOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
          </el-select>
        </div>
        <div class="col-12 col-md-4">
          <label class="form-label d-block mb-1">{{ t('allocationEvent.regionFilter.statusLabel') }}</label>
          <el-select v-model="statusFilter" class="w-100">
            <el-option value="all" :label="t('allocationEvent.regionFilter.statusAll')" />
            <el-option v-for="opt in statusOptions" :key="opt.value" :label="t(opt.label)" :value="opt.value" />
          </el-select>
        </div>
      </div>
    </el-card>

    <el-card class="mb-8">
      <div class="d-flex flex-column flex-md-row justify-content-between align-items-start align-items-md-center mb-4">
        <div>
          <h5 class="fw-semibold mb-1">{{ t('allocationEvent.regionTableTitle') }}</h5>
          <span class="text-muted fs-8">{{ t('allocationEvent.regionTableDesc') }}</span>
        </div>
        <span class="text-muted fs-8">{{ t('allocationEvent.regionListCount', { count: filteredRows.length }) }}</span>
      </div>

      <BaseTable :columns="columns" :items="filteredRows" :loading="allocationStore.loading" :showCheckboxColumn="false"
        :showActionsColumn="false" :height="600">
        <template #cell-companyCount="{ item }">
          <span class="fw-semibold">{{ t('allocationEvent.branchesCount', { count: item.companyCount }) }}</span>
        </template>
        <template #cell-totalQuantity="{ item }">
          <span class="fw-semibold">{{ t('allocationEvent.eventsCount', { count: item.totalQuantity }) }}</span>
        </template>
        <template #cell-totalBudget="{ item }">
          <span class="fw-semibold">{{ formatCurrency(item.totalBudget) }}</span>
        </template>
        <template #cell-status="{ item }">
          <BaseBadge :label="statusLabel(item.status)" :colorByLabelMap="statusColorMap" />
        </template>
        <template #cell-view="{ item }">
          <el-tooltip :content="t('common.viewDetail')" placement="top">
            <el-button circle size="small" @click="viewAllocation(item)">
              <el-icon>
                <View />
              </el-icon>
            </el-button>
          </el-tooltip>
        </template>
      </BaseTable>
    </el-card>

    <EventAllocationDialog v-if="showDialog" v-model:visible="showDialog" :mode="dialogMode"
      :allocation-event-data="allocationStore.selectedEvent" :region-id-scope="dialogRegionId" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { Download, View } from '@element-plus/icons-vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import EventAllocationDialog from './EventAllocationDialog.vue'
import { useAllocationEventStore } from '@/stores/allocationEventStore'
import { useRegionStore } from '@/stores/regionStore'
import { useNotificationStore } from '@/stores/notificationStore'
import { formatCurrency } from '@/utils/format'
import { AllocationEventStatus, AllocationEventStatusLabels } from '@/types'

const { t } = useI18n()
const allocationStore = useAllocationEventStore()
const regionStore = useRegionStore()
const notificationStore = useNotificationStore()

const searchKeyword = ref('')
const monthFilter = ref<string | 'all'>('all')
const statusFilter = ref<number | 'all'>('all')

const showDialog = ref(false)
const dialogMode = ref<'view'>('view')
const dialogRegionId = ref<string | null>(null)

interface RegionAllocationRow {
  id: string
  eventId?: string
  allocationCode?: string
  monthLabel: string
  monthKey?: string
  allocationMonth?: number
  allocationYear?: number
  regionId?: string | number | null
  regionName: string
  companyCount: number
  totalQuantity: number
  totalBudget: number
  status: number
}

const columns: BaseTableColumn[] = [
  { key: 'allocationCode', labelKey: 'allocationEvent.code', isBold: true },
  { key: 'monthLabel', labelKey: 'allocationEvent.month', width: 160 },
  { key: 'regionName', labelKey: 'allocationEvent.region', width: 160 },
  { key: 'companyCount', labelKey: 'allocationEvent.branches', width: 160, align: 'center' },
  { key: 'totalQuantity', labelKey: 'allocationEvent.quantity', width: 160, align: 'center' },
  { key: 'totalBudget', labelKey: 'allocationEvent.totalBudget', width: 200, align: 'right' },
  { key: 'status', labelKey: 'allocationEvent.status', width: 160, align: 'center' },
  { key: 'view', labelKey: 'common.actions', width: 100, align: 'center' }
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

const regionAllocations = computed<RegionAllocationRow[]>(() => {
  const rows: RegionAllocationRow[] = []
  const regionNameMap = new Map<string, string>()
  regionStore.regions.forEach(region => regionNameMap.set(String(region.id), region.regionName ?? '-'))

  const events = allocationStore.allocationEvents || []
  events.forEach(event => {
    const details = event.allocationDetails || []
    const perRegion = new Map<string, { quantity: number; companyIds: Set<string>; regionId?: string | number | null }>()

    details.forEach(detail => {
      if (detail?.regionId == null) return
      const key = String(detail.regionId)
      if (!perRegion.has(key)) {
        perRegion.set(key, { quantity: 0, companyIds: new Set<string>(), regionId: detail.regionId })
      }
      const group = perRegion.get(key)!
      group.quantity += Number(detail.quantity) || 0
      if (detail.companyId != null) group.companyIds.add(String(detail.companyId))
    })

    perRegion.forEach(group => {
      const month = event.allocationMonth ? String(event.allocationMonth).padStart(2, '0') : '--'
      const year = event.allocationYear ? String(event.allocationYear) : '--'
      rows.push({
        id: `${event.id}-${group.regionId}`,
        eventId: event.id || undefined,
        allocationCode: event.allocationCode,
        monthLabel: event.allocationMonth && event.allocationYear
          ? t('allocationEvent.monthLabel', { month, year })
          : '-',
        monthKey: event.allocationMonth && event.allocationYear ? `${event.allocationYear}-${month}` : undefined,
        allocationMonth: event.allocationMonth,
        allocationYear: event.allocationYear,
        regionId: group.regionId,
        regionName: regionNameMap.get(String(group.regionId)) || t('common.none'),
        companyCount: group.companyIds.size,
        totalQuantity: group.quantity,
        totalBudget: group.companyIds.size * (Number(event.eventBudget) || 0),
        status: Number(event.allocationEventStatus ?? AllocationEventStatus.Draft)
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
    return a.regionName.localeCompare(b.regionName, undefined, { sensitivity: 'base' })
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
        label: t('allocationEvent.monthLabel', {
          month,
          year
        })
      }
    })
})

const filteredRows = computed(() => {
  const search = searchKeyword.value.trim().toLowerCase()
  return regionAllocations.value.filter(row => {
    const matchesKeyword =
      !search ||
      (row.allocationCode ?? '').toLowerCase().includes(search) ||
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

function viewAllocation(row: RegionAllocationRow) {
  const target = allocationStore.allocationEvents.find(ev => ev.id === row.eventId)
  if (target) {
    allocationStore.selectEvent(target)
    dialogRegionId.value = row.regionId != null ? String(row.regionId) : null
    showDialog.value = true
  }
}

function onExport() {
  notificationStore.showToast('info', { key: 'common.comingSoon' })
}

onMounted(async () => {
  if (!allocationStore.allocationEvents.length) {
    await allocationStore.fetchAllForRegion()
  }
  if (!regionStore.regions.length) {
    await regionStore.fetchAllRegions()
  }
})
</script>

<style scoped>
.region-allocation-page {
  padding-bottom: 40px;
}
</style>
