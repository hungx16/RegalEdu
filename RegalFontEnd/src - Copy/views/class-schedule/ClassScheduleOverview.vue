<template>
  <div class="schedule-overview-page">
    <FilterComponent ref="filterComponentRef" header-title="classSchedule.overviewTitle"
      header-desc="classSchedule.overviewDescription" :disabled-lock="!canLockSelected"
      :disabled-unlock="!canUnlockSelected" @lock="handleFilterLock" @unlock="handleFilterUnlock" class="mb-6" />

    <div class="row g-4 mb-6">
      <div class="col-12 col-md-6">
        <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
          <div class="d-flex justify-content-between align-items-start mb-2">
            <span class="fw-semibold fs-5">{{ t('classScheduleStats.totalSessionsTitle') }}</span>
            <i class="bi bi-calendar3 fs-4 text-body-secondary"></i>
          </div>
          <div class="fs-2 fw-bold mb-1">{{ totalSessions }}</div>
          <div class="fs-7 text-body-secondary">{{ t('classSchedule.overviewDescription') }}</div>
        </div>
      </div>
      <div class="col-12 col-md-6">
        <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
          <div class="d-flex justify-content-between align-items-start mb-2">
            <span class="fw-semibold fs-5">{{ t('classScheduleStats.lockedSessionsTitle') }}</span>
            <i class="bi bi-lock fs-4 text-body-secondary"></i>
          </div>
          <div class="fs-2 fw-bold mb-1">{{ lockedSessions }}</div>
          <div class="fs-7 text-body-secondary">{{ t('classScheduleStats.lockedSessionsDesc') }}</div>
        </div>
      </div>
    </div>

    <div class="filters">
      <el-row :gutter="16" type="flex" align="middle">
        <el-col :xs="24" :md="8">
          <el-form label-position="top">
            <el-form-item :label="t('classSchedule.classFilterLabel')">
              <el-select v-model="selectedClassId" filterable :placeholder="t('classSchedule.selectClassPlaceholder')"
                :disabled="classLoading || !classes.length">
                <el-option :label="t('classSchedule.allClasses')" :value="ALL_CLASSES_KEY"></el-option>
                <el-option v-for="cls in classes" :key="cls.id" :label="formatClassLabel(cls)" :value="cls.id" />
              </el-select>
            </el-form-item>
          </el-form>
        </el-col>
        <el-col :xs="24" :md="8">
          <el-form label-position="top">
            <el-form-item :label="t('classScheduleStats.startDateFrom')">
              <el-date-picker v-model="filterCriteria.startDate" type="date"
                :placeholder="t('classSchedule.datePlaceholder')" format="DD/MM/YYYY" value-format="YYYY-MM-DD"
                clearable :disabled="scheduleLoading" />
            </el-form-item>
          </el-form>
        </el-col>
        <el-col :xs="24" :md="8">
          <el-form label-position="top">
            <el-form-item :label="t('classScheduleStats.startDateTo')">
              <el-date-picker v-model="filterCriteria.endDate" type="date"
                :placeholder="t('classSchedule.datePlaceholder')" format="DD/MM/YYYY" value-format="YYYY-MM-DD"
                clearable :disabled="scheduleLoading" />
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>
    </div>
    <div v-if="selectedIds.length" class="selection-summary">
      {{ t('classSchedule.selectedSummary', { count: selectedIds.length }) }}
    </div>

    <!-- BẢNG DỮ LIỆU -->
    <div class="card mb-10 w-100 mt-5">
      <div class="card-header card-header-stretch">
        <div class="card-title d-flex flex-column">
          <h3 class="fw-bold fs-4 mb-1">{{ t('classSchedule.listTitle') }}</h3>
          <span class="text-body-secondary fw-light fs-8">{{ t('classSchedule.listFunction') }}</span>
        </div>
      </div>
      <div class="card-body py-6 px-2 px-md-6">
        <el-skeleton v-if="scheduleLoading" animated :rows="4" />
        <el-empty v-else-if="!filteredRows.length" :description="t('classSchedule.emptyState')" />
        <BaseTable v-else :columns="columns" :items="paginatedRows" :loading="scheduleLoading" :show-index="false"
          :show-pagination="true" :page="page" :page-size="pageSize" :total="filteredRows.length"
          :show-checkbox-column="true" @update:rows="handleSelectionChange" @update:page="onPageChange"
          @update:pageSize="onPageSizeChange" :show-actions-column="true" :actions-column-width="160" height="520px"
          class="schedule-table">
          <template #cell-date="{ item }">
            {{ formatDate(item.date) }}
          </template>
          <template #cell-timeRange="{ item }">
            {{ formatTimeRange(item.startTime, item.endTime) }}
          </template>
          <template #cell-className="{ item }">
            <div class="class-label">
              <span class="class-name">{{ item.className }}</span>
              <span class="class-code" v-if="item.classCode">({{ item.classCode }})</span>
            </div>
          </template>
          <template #cell-classStatus="{ item }">
            <BaseBadge :label="classStatusLabel(item.classStatus)" :type="classStatusColor(item.classStatus)"
              size="small" />
          </template>
          <template #cell-classScheduleStatus="{ item }">
            <BaseBadge :label="classScheduleStatusLabel(item.classScheduleStatus)"
              :type="classScheduleStatusColor(item.classScheduleStatus)" size="small" />
          </template>
          <template #cell-sessionAttendanceStatus="{ item }">
            <BaseBadge :label="sessionAttendanceStatusLabel(item.sessionAttendanceStatus)"
              :type="sessionAttendanceStatusColor(item.sessionAttendanceStatus)" size="small" />
          </template>
          <template #cell-sessionAttendanceLockStatus="{ item }">
            <BaseBadge :label="sessionLockStatusLabel(item.sessionAttendanceLockStatus)"
              :type="sessionLockStatusColor(item.sessionAttendanceLockStatus)" size="small" />
          </template>
          <template #actions="{ item }">
            <el-button size="small" type="primary" :plain="true" :disabled="lockLoading[item.id] || !item.id"
              @click="toggleLockStatus(item)">
              {{ item.sessionAttendanceLockStatus === SessionAttendanceLockStatus.Locked ?
                t('classSchedule.unlockAttendance') : t('classSchedule.lockAttendance') }}
            </el-button>
          </template>
        </BaseTable>
      </div>
    </div>

  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import FilterComponent from '@/components/filter-component/FilterComponent.vue'
import { useClassStore } from '@/stores/classStore'
import { serviceFactory } from '@/services/ServiceFactory'
import type { ClassScheduleModel } from '@/api/ClassScheduleApi'
import type { ClassModel } from '@/api/ClassApi'
import { storeToRefs } from 'pinia'
import { useNotificationStore } from '@/stores/notificationStore'
import type { ListHeaderParams } from '@/types'
import {
  ClassScheduleStatus,
  ClassStatus,
  SessionAttendanceStatus,
  SessionAttendanceLockStatus,
} from '@/types'

const ALL_CLASSES_KEY = 'all'

type ScheduleRow = ClassScheduleModel & {
  className: string
  classCode: string
  classStatus: ClassStatus
}

const classStore = useClassStore()
const notificationStore = useNotificationStore()
const { classes, loading: classLoading } = storeToRefs(classStore)
const { t } = useI18n()

const filterComponentRef = ref<InstanceType<typeof FilterComponent> | null>(null)

const selectedClassId = ref<string>(ALL_CLASSES_KEY)
const scheduleRows = ref<ScheduleRow[]>([])
const scheduleCache = reactive<Record<string, ClassScheduleModel[]>>({})
const scheduleLoading = ref(false)
const lockLoading = reactive<Record<string, boolean>>({})
const selectedRows = ref<ScheduleRow[]>([])
const bulkActionLoading = ref(false)
const page = ref(1)
const pageSize = ref(30)
const filterCriteria = reactive<{ startDate: string; endDate: string }>({
  startDate: '',
  endDate: '',
})

const selectedIds = computed(() =>
  selectedRows.value.map((row) => row.id).filter(Boolean) as string[]
)
const canLockSelected = computed(() =>
  selectedRows.value.some((row) => row.sessionAttendanceLockStatus !== SessionAttendanceLockStatus.Locked)
)
const canUnlockSelected = computed(() =>
  selectedRows.value.some((row) => row.sessionAttendanceLockStatus === SessionAttendanceLockStatus.Locked)
)

const listHeaderParams: ListHeaderParams = {
  listParams: [],
  listBtn: [
    { type: 'lock', label: 'classSchedule.lockAttendance', event: 'lock' },
    { type: 'unlock', label: 'classSchedule.unlockAttendance', event: 'unlock' },
  ],
}

const handleFilterLock = () => {
  confirmLockAction(SessionAttendanceLockStatus.Locked, () =>
    bulkUpdateLockStatus(SessionAttendanceLockStatus.Locked)
  )
}
const handleFilterUnlock = () => {
  confirmLockAction(SessionAttendanceLockStatus.Unlocked, () =>
    bulkUpdateLockStatus(SessionAttendanceLockStatus.Unlocked)
  )
}

const columns: BaseTableColumn[] = [
  { key: 'className', labelKey: 'classSchedule.classLabel', sticky: true },

  { key: 'sessionIndex', labelKey: 'classSchedule.sessionIndex', width: 90, align: 'center', sticky: true },
  { key: 'date', labelKey: 'classSchedule.date', width: 120, align: 'center' },
  { key: 'timeRange', labelKey: 'classSchedule.timeRange', width: 140, align: 'center' },
  { key: 'classStatus', labelKey: 'classSchedule.classStatus', width: 140, align: 'center' },
  { key: 'classScheduleStatus', labelKey: 'classSchedule.classScheduleStatus', width: 150, align: 'center' },
  { key: 'sessionAttendanceStatus', labelKey: 'classSchedule.attendanceStatus', width: 170, align: 'center' },
  // { key: 'sessionAttendanceLockStatus', labelKey: 'classSchedule.lockStatus', width: 150, align: 'center' },
  { key: 'actions', labelKey: 'common.actions', width: 160, align: 'center' },
]

const displayRows = computed(() => scheduleRows.value.slice().sort((a, b) => compareSchedules(a, b)))
const filteredRows = computed(() => {
  const start = filterCriteria.startDate ? new Date(filterCriteria.startDate) : null
  const end = filterCriteria.endDate ? new Date(filterCriteria.endDate) : null
  if (!start && !end) return displayRows.value
  return displayRows.value.filter((row) => {
    if (!row.date) return false
    const rowDate = new Date(row.date.substring(0, 10))
    if (start && rowDate < start) return false
    if (end && rowDate > end) return false
    return true
  })
})
const paginatedRows = computed(() => {
  const startIdx = (page.value - 1) * pageSize.value
  return filteredRows.value.slice(startIdx, startIdx + pageSize.value)
})
const totalSessions = computed(() => scheduleRows.value.length)
const lockedSessions = computed(() =>
  scheduleRows.value.filter((row) => row.sessionAttendanceLockStatus === SessionAttendanceLockStatus.Locked)
    .length
)

onMounted(async () => {
  await classStore.fetchAllClasses()
  filterComponentRef.value?.initListHeaderParams(listHeaderParams, { skipPermission: true })
})

watch(
  () => classes.value.length,
  (length) => {
    if (length) {
      void loadSchedules(selectedClassId.value)
    }
  },
  { immediate: true }
)

watch(selectedClassId, (newId) => {
  page.value = 1
  if (classes.value.length) {
    void loadSchedules(newId)
  }
})

watch(
  () => [filterCriteria.startDate, filterCriteria.endDate],
  () => {
    page.value = 1
  }
)

function formatClassLabel(cls: ClassModel) {
  return cls.classCode ? `${cls.className} (${cls.classCode})` : cls.className
}

function formatDate(value?: string | Date | null) {
  if (!value) return '-'
  const date = new Date(value)
  return `${String(date.getDate()).padStart(2, '0')}/${String(date.getMonth() + 1).padStart(2, '0')}/${date.getFullYear()}`
}

function formatTimeRange(start?: string | null, end?: string | null) {
  const startTime = start ? start.substring(0, 5) : '--:--'
  const endTime = end ? end.substring(0, 5) : '--:--'
  return `${startTime} - ${endTime}`
}

function compareSchedules(a: ScheduleRow, b: ScheduleRow) {
  const dateA = new Date(`${a.date}T${a.startTime ?? '00:00'}:00`).getTime()
  const dateB = new Date(`${b.date}T${b.startTime ?? '00:00'}:00`).getTime()
  return dateA - dateB
}

function handleSelectionChange(rows: ScheduleRow[]) {
  selectedRows.value = rows || []
}

async function loadSchedules(classId: string) {
  if (!classes.value.length) {
    return
  }
  scheduleLoading.value = true
  try {
    const targetClasses =
      classId === ALL_CLASSES_KEY ? classes.value : classes.value.filter((cls) => cls.id === classId)
    const aggregated: ScheduleRow[] = []
    for (const cls of targetClasses) {
      if (!cls?.id) continue
      const schedules = await resolveSchedules(cls.id)
      aggregated.push(
        ...schedules.map((schedule) => ({
          ...schedule,
          className: cls.className,
          classCode: cls.classCode,
          classStatus: cls.classStatus ?? ClassStatus.Plan,
        }))
      )
    }
    scheduleRows.value = aggregated.map((row, idx) => ({
      ...row,
      sessionIndex: row.sessionIndex ?? idx + 1,
    }))
    selectedRows.value = []
  } catch (err) {
    console.error('Failed to load schedules', err)
    scheduleRows.value = []
  } finally {
    scheduleLoading.value = false
  }
}

function onPageChange(val: number) {
  page.value = val
}

function onPageSizeChange(size: number) {
  page.value = 1
  pageSize.value = size
}

async function resolveSchedules(classId: string) {
  if (!scheduleCache[classId]) {
    const res = await serviceFactory.classScheduleService.getSchedulesByClassId(classId)
    scheduleCache[classId] = res.data || []
  }
  return scheduleCache[classId]
}

async function refresh() {
  if (selectedClassId.value === ALL_CLASSES_KEY) {
    Object.keys(scheduleCache).forEach((key) => delete scheduleCache[key])
  } else {
    delete scheduleCache[selectedClassId.value]
  }
  selectedRows.value = []
  await loadSchedules(selectedClassId.value)
}

function updateLockStatusForRows(ids: string[], newStatus: SessionAttendanceLockStatus) {
  if (!ids.length) return
  const idSet = new Set(ids)
  scheduleRows.value = scheduleRows.value.map((row) =>
    row.id && idSet.has(row.id) ? { ...row, sessionAttendanceLockStatus: newStatus } : row
  )
  Object.keys(scheduleCache).forEach((key) => {
    scheduleCache[key] = scheduleCache[key].map((row) =>
      row.id && idSet.has(row.id) ? { ...row, sessionAttendanceLockStatus: newStatus } : row
    )
  })
  selectedRows.value = selectedRows.value.map((row) =>
    row.id && idSet.has(row.id) ? { ...row, sessionAttendanceLockStatus: newStatus } : row
  )
}

async function bulkUpdateLockStatus(newStatus: SessionAttendanceLockStatus) {
  if (!selectedIds.value.length) return
  bulkActionLoading.value = true
  try {
    await serviceFactory.classScheduleService.updateSessionAttendanceLockStatuses({
      sessionIds: selectedIds.value,
      newStatus,
    })
    updateLockStatusForRows(selectedIds.value, newStatus)
    notificationStore.showToast('success', {
      key:
        newStatus === SessionAttendanceLockStatus.Locked
          ? 'classSchedule.lockAttendanceSuccess'
          : 'classSchedule.unlockAttendanceSuccess',
    })
  } catch (err) {
    console.error('Bulk lock update failed', err)
    notificationStore.showToast('error', {
      key:
        newStatus === SessionAttendanceLockStatus.Locked
          ? 'classSchedule.lockAttendanceFailed'
          : 'classSchedule.unlockAttendanceFailed',
    })
  } finally {
    bulkActionLoading.value = false
  }
}

function confirmLockAction(
  newStatus: SessionAttendanceLockStatus,
  action: () => Promise<void>
) {
  const messageKey =
    newStatus === SessionAttendanceLockStatus.Locked
      ? 'classSchedule.confirmLockAttendance'
      : 'classSchedule.confirmUnlockAttendance'
  notificationStore.showConfirm(
    { key: messageKey },
    async () => {
      await action()
    }
  )
}

function toggleLockStatus(item: ScheduleRow) {
  if (!item.id) return
  const newStatus =
    item.sessionAttendanceLockStatus === SessionAttendanceLockStatus.Locked
      ? SessionAttendanceLockStatus.Unlocked
      : SessionAttendanceLockStatus.Locked
  confirmLockAction(newStatus, async () => {
    const itemId = item.id as string
    lockLoading[itemId] = true
    try {
      await serviceFactory.classScheduleService.updateSessionAttendanceLockStatus(itemId, newStatus)
      updateLockStatusForRows([itemId], newStatus)
      notificationStore.showToast('success', {
        key: newStatus === SessionAttendanceLockStatus.Unlocked ? 'classSchedule.unlockAttendanceSuccess' : 'classSchedule.lockAttendanceSuccess',
      })
    } catch (err) {
      console.error('Failed to toggle session lock status', err)
      notificationStore.showToast('error', {
        key: newStatus === SessionAttendanceLockStatus.Unlocked ? 'classSchedule.unlockAttendanceFailed' : 'classSchedule.lockAttendanceFailed',
      })
    } finally {
      lockLoading[itemId] = false
    }
  })
}

function classStatusLabel(status?: ClassStatus | null) {
  switch (status) {
    case ClassStatus.InProgress:
      return t('classStatus.inProgress')
    case ClassStatus.Completed:
      return t('classStatus.completed')
    default:
      return t('classStatus.plan')
  }
}

function classStatusColor(status?: ClassStatus | null) {
  switch (status) {
    case ClassStatus.InProgress:
      return 'success'
    case ClassStatus.Completed:
      return 'info'
    default:
      return 'warning'
  }
}

function classScheduleStatusLabel(status?: ClassScheduleStatus | null) {
  switch (status) {
    case ClassScheduleStatus.Completed:
      return t('classStatus.completed')
    case ClassScheduleStatus.Cancelled:
      return t('classStatus.cancelled')
    default:
      return t('classSchedule.notStarted')
  }
}

function classScheduleStatusColor(status?: ClassScheduleStatus | null) {
  switch (status) {
    case ClassScheduleStatus.Completed:
      return 'success'
    case ClassScheduleStatus.Cancelled:
      return 'danger'
    default:
      return 'info'
  }
}

function sessionAttendanceStatusLabel(status?: SessionAttendanceStatus | null) {
  switch (status) {
    case SessionAttendanceStatus.Checked:
      return t('classSchedule.attendanceChecked')
    case SessionAttendanceStatus.Confirmed:
      return t('classSchedule.attendanceConfirmed')
    default:
      return t('classSchedule.attendanceNotChecked')
  }
}

function sessionAttendanceStatusColor(status?: SessionAttendanceStatus | null) {
  switch (status) {
    case SessionAttendanceStatus.Checked:
      return 'warning'
    case SessionAttendanceStatus.Confirmed:
      return 'success'
    default:
      return 'info'
  }
}

function sessionLockStatusLabel(status?: SessionAttendanceLockStatus | null) {
  return status === SessionAttendanceLockStatus.Locked ? t('classSchedule.locked') : t('classSchedule.unlocked')
}

function sessionLockStatusColor(status?: SessionAttendanceLockStatus | null) {
  return status === SessionAttendanceLockStatus.Locked ? 'danger' : 'success'
}
</script>

<style scoped>
.schedule-overview-page {
  padding: 20px;
}

.page-header {
  margin-bottom: 16px;
}

.page-subtitle {
  margin: 4px 0 0;
  color: #6c7482;
}

.filters {
  margin-bottom: 12px;
}

.selection-summary {
  font-size: 0.85rem;
  color: #6c7482;
}

.summary-card {
  border: 1px solid #f0f0f5;
}

.schedule-table-wrapper {
  background: #fff;
  border-radius: 16px;
  border: 1px solid #f0f0f5;
  padding: 16px;
}

.class-label {
  display: flex;
  align-items: center;
  gap: 6px;
}

.class-code {
  color: #6c7482;
  font-size: 0.85rem;
}

.schedule-table {
  font-size: 13px;
}
</style>
