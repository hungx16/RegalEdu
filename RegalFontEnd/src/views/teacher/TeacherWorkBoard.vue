<template>
  <div class="teacher-work-board">
    <div class="d-flex flex-wrap align-items-end gap-3 mb-4">
      <div class="flex-grow-1">
        <label class="form-label fw-semibold">{{ t('classScheduleStats.startDateFrom') }}</label>
        <el-date-picker v-model="filters.fromDate" type="date" placeholder="dd/mm/yyyy" format="DD/MM/YYYY"
          value-format="YYYY-MM-DD" clearable class="w-100" />
      </div>
      <div class="flex-grow-1">
        <label class="form-label fw-semibold">{{ t('classScheduleStats.startDateTo') }}</label>
        <el-date-picker v-model="filters.toDate" type="date" placeholder="dd/mm/yyyy" format="DD/MM/YYYY"
          value-format="YYYY-MM-DD" clearable class="w-100" />
      </div>
      <div class="d-flex gap-2">
        <el-button size="small" @click="resetFilters">{{ t('common.clearFilters') }}</el-button>
        <el-button size="small" type="primary" :loading="loading" @click="fetchWorkBoard">
          {{ t('common.search') }}
        </el-button>
      </div>
    </div>

    <el-skeleton v-if="loading" animated :rows="4" />
    <el-empty v-else-if="!displaySessions.length" :description="t('common.noData')" />
    <BaseTable :show-checkbox-column="false" :columns="columns" :items="displaySessions" :loading="loading"
      :show-pagination="true" :page="page" :page-size="pageSize" :filter="filter" :show-index="true"
      :show-actions-column="false" @update:filter="onTableFilter" @update:page="onPageChange"
      @update:pageSize="onPageSizeChange" height="520px">
      <template #cell-date="{ item }">
        {{ formatDate(item.date) }}
      </template>
      <template #cell-scheduleName="{ item }">
        {{ item.scheduleName || formatDayOfWeek(item.dayOfWeek) }}
      </template>
      <template #cell-shiftTimeRange="{ item }">
        {{ item.shiftTimeRange || formatTimeRange(item.startTime, item.endTime) }}
      </template>
      <template #cell-classScheduleStatus="{ item }">
        <BaseBadge v-if="resolveClassScheduleStatus(item) !== null"
          :label="classScheduleStatusLabel(resolveClassScheduleStatus(item))"
          :type="classScheduleStatusColor(resolveClassScheduleStatus(item))" size="small" />
        <span v-else>-</span>
      </template>
      <template #cell-workTypeLabel="{ item }">
        {{ item.workTypeLabel || '-' }}
      </template>
    </BaseTable>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { serviceFactory } from '@/services/ServiceFactory'
import { useNotificationStore } from '@/stores/notificationStore'
import type { TeacherSessionItemModel } from '@/api/TeacherSessionApi'
import { formatDate as formatDateUtil } from '@/utils/format'
import { getDayOfWeekKey } from '@/types/daysOfWeek'
import { ClassScheduleStatus } from '@/types'

const props = defineProps<{ teacherId: string }>()
const { t } = useI18n()
const notificationStore = useNotificationStore()

const loading = ref(false)
const sessions = ref<TeacherSessionItemModel[]>([])
const page = ref(1)
const pageSize = ref(20)
const filter = ref<Record<string, any>>({})

const filters = reactive({
  fromDate: '',
  toDate: '',
})

const workTypeFilterOptions = computed(() => [
  { label: t('teacherSessions.workTypeInterview'), value: t('teacherSessions.workTypeInterview'), isLocale: false },
  { label: t('teacherSessions.workTypeEvent'), value: t('teacherSessions.workTypeEvent'), isLocale: false },
])

const columns = computed<BaseTableColumn[]>(() => [
  { key: 'date', labelKey: 'teacherSessions.date', filterType: 'date', width: 120, align: 'center', sticky: true },
  { key: 'branchName', labelKey: 'teacherSessions.branchName', filterType: 'text', width: 180 },
  { key: 'programName', labelKey: 'teacherSessions.courseName', filterType: 'text', width: 160 },
  { key: 'levelName', labelKey: 'teacherSessions.classTypeName', filterType: 'text', width: 140 },
  { key: 'moduleName', labelKey: 'teacherSessions.sessionName', filterType: 'text', width: 140 },
  { key: 'teacherCode', labelKey: 'teacherSessions.teacherCode', filterType: 'text', width: 140 },
  { key: 'teacherName', labelKey: 'teacherSessions.teacherName', filterType: 'text', width: 200 },
  { key: 'teacherEnglishName', labelKey: 'teacherSessions.teacherEnglishName', filterType: 'text', width: 200 },
  { key: 'classCode', labelKey: 'teacherSessions.classCode', filterType: 'text', width: 140 },
  { key: 'className', labelKey: 'teacherSessions.className', filterType: 'text', width: 200 },
  {
    key: 'workTypeLabel',
    labelKey: 'teacherSessions.workTypeLabel',
    filterType: 'select',
    filterOptions: workTypeFilterOptions.value,
    width: 170,
  },
  { key: 'scheduleName', labelKey: 'teacherSessions.scheduleDay', filterType: 'text', width: 160 },
  { key: 'shiftTimeRange', labelKey: 'teacherSessions.shiftTimeRange', width: 150, align: 'center' },
  { key: 'classScheduleStatus', labelKey: 'classSchedule.classScheduleStatus', width: 150, align: 'center' },
])

const displaySessions = computed(() => {
  const itemsWithWorkType = sessions.value.map((item) => ({
    ...item,
    workTypeLabel: resolveWorkTypeLabel(item),
  }))
  return [...itemsWithWorkType].sort((a, b) => {
    const aTime = new Date(`${a.date}T${a.startTime ?? '00:00'}`).getTime()
    const bTime = new Date(`${b.date}T${b.startTime ?? '00:00'}`).getTime()
    return aTime - bTime
  })
})

function formatDate(value?: string | null) {
  if (!value) return '-'
  return formatDateUtil(value, 'DD/MM/YYYY') || '-'
}

function formatDayOfWeek(value?: number | null) {
  if (!value) return '-'
  const key = getDayOfWeekKey(value)
  return key ? t(key) : '-'
}

function formatTimeRange(start?: string | null, end?: string | null) {
  const startTime = start ? start.substring(0, 5) : '--:--'
  const endTime = end ? end.substring(0, 5) : '--:--'
  return `${startTime} - ${endTime}`
}

function resolveWorkTypeLabel(item: any) {
  const name = item?.workTypeName ?? item?.WorkTypeName
  if (name) return name
  const raw = item?.workType ?? item?.WorkType ?? item?.teacherWorkType ?? item?.TeacherWorkType
  if (raw === 0) return t('teacherSessions.workTypeInterview')
  if (raw === 1) return t('teacherSessions.workTypeEvent')
  if (typeof raw === 'string') {
    const normalized = raw.trim().toLowerCase()
    if (normalized.includes('interview')) return t('teacherSessions.workTypeInterview')
    if (normalized.includes('event')) return t('teacherSessions.workTypeEvent')
  }
  return '-'
}

function normalizeClassScheduleStatus(status: unknown) {
  if (status === null || status === undefined) return null
  if (typeof status === 'number') return status
  if (typeof status === 'string') {
    const trimmed = status.trim()
    const asNumber = Number(trimmed)
    if (!Number.isNaN(asNumber)) return asNumber
    const lowered = trimmed.toLowerCase()
    if (lowered === 'completed') return ClassScheduleStatus.Completed
    if (lowered === 'cancelled' || lowered === 'canceled') return ClassScheduleStatus.Cancelled
    if (lowered === 'notstarted' || lowered === 'not_started' || lowered === 'not started') {
      return ClassScheduleStatus.NotStarted
    }
  }
  return null
}

function resolveClassScheduleStatus(item: any) {
  return normalizeClassScheduleStatus(
    item?.classScheduleStatus ??
    item?.ClassScheduleStatus ??
    item?.scheduleStatus ??
    item?.ScheduleStatus
  )
}

function classScheduleStatusLabel(status?: ClassScheduleStatus | string | null) {
  const normalized = normalizeClassScheduleStatus(status)
  switch (normalized) {
    case ClassScheduleStatus.Completed:
      return t('classStatus.completed')
    case ClassScheduleStatus.Cancelled:
      return t('classStatus.cancelled')
    case ClassScheduleStatus.NotStarted:
      return t('classSchedule.notStarted')
    default:
      return t('common.unknown')
  }
}

function classScheduleStatusColor(status?: ClassScheduleStatus | string | null) {
  const normalized = normalizeClassScheduleStatus(status)
  switch (normalized) {
    case ClassScheduleStatus.Completed:
      return 'success'
    case ClassScheduleStatus.Cancelled:
      return 'danger'
    case ClassScheduleStatus.NotStarted:
      return 'info'
    default:
      return 'secondary'
  }
}

async function fetchWorkBoard() {
  if (!props.teacherId) {
    sessions.value = []
    return
  }
  loading.value = true
  try {
    const res = await serviceFactory.teacherSessionService.getTeacherWorkBoard({
      teacherId: props.teacherId,
      fromDate: filters.fromDate || undefined,
      toDate: filters.toDate || undefined,
    })
    if (res?.succeeded) {
      sessions.value = res.data || []
    } else {
      sessions.value = []
      notificationStore.showToast('error', { key: 'common.error' })
    }
  } catch (error) {
    console.error('Error fetching teacher work board:', error)
    sessions.value = []
    notificationStore.showToast('error', { key: 'common.error' })
  } finally {
    loading.value = false
  }
}

function resetFilters() {
  filters.fromDate = ''
  filters.toDate = ''
  void fetchWorkBoard()
}

function onTableFilter(val: Record<string, any>) {
  filter.value = val
  page.value = 1
}

function onPageChange(val: number) {
  page.value = val
}

function onPageSizeChange(size: number) {
  pageSize.value = size
  page.value = 1
}

onMounted(fetchWorkBoard)

watch(
  () => props.teacherId,
  () => {
    void fetchWorkBoard()
  }
)
</script>

<style scoped>
.teacher-work-board {
  padding: 12px 0;
}
</style>
