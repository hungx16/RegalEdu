<template>
  <div class="home-teacher-schedule-page">
    <FilterComponent
      ref="filterComponentRef"
      header-title="models.HomeTeacherSessions"
      header-desc="classSchedule.listFunction"
      class="mb-6"
    />

    <div class="card mb-10 w-100">
      <div class="card-header card-header-stretch">
        <div class="card-title d-flex flex-column">
          <h3 class="fw-bold fs-4 mb-1">{{ t('classSchedule.listTitle') }}</h3>
          <span class="text-body-secondary fw-light fs-8">
            {{ t('classSchedule.listFunction') }}
          </span>
        </div>
      </div>
      <div class="card-body py-6 px-2 px-md-6">
        <div class="filters mb-5">
          <el-form label-position="top">
            <el-row :gutter="16">
              <el-col :xs="24" :md="8">
                <el-form-item :label="t('classSchedule.classFilterLabel')">
                  <el-select
                    v-model="selectedClassId"
                    filterable
                    :placeholder="t('classSchedule.selectClassPlaceholder')"
                    :disabled="loading || !classOptions.length"
                  >
                    <el-option :label="t('classSchedule.allClasses')" :value="ALL_CLASSES_KEY" />
                    <el-option
                      v-for="option in classOptions"
                      :key="option.id"
                      :label="option.label"
                      :value="option.id"
                    />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :xs="24" :md="8">
                <el-form-item :label="t('classScheduleStats.startDateFrom')">
                  <el-date-picker
                    v-model="filterCriteria.startDate"
                    type="date"
                    :placeholder="t('classSchedule.datePlaceholder')"
                    format="DD/MM/YYYY"
                    value-format="YYYY-MM-DD"
                    clearable
                    :disabled="loading"
                  />
                </el-form-item>
              </el-col>
              <el-col :xs="24" :md="8">
                <el-form-item :label="t('classScheduleStats.startDateTo')">
                  <el-date-picker
                    v-model="filterCriteria.endDate"
                    type="date"
                    :placeholder="t('classSchedule.datePlaceholder')"
                    format="DD/MM/YYYY"
                    value-format="YYYY-MM-DD"
                    clearable
                    :disabled="loading"
                  />
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
          <div class="d-flex justify-content-end gap-2">
            <el-button size="small" @click="resetFilters">
              {{ t('common.clearFilters') }}
            </el-button>
          </div>
        </div>

        <el-skeleton v-if="loading" animated :rows="4" />
        <el-empty v-else-if="!filteredRows.length" :description="t('common.noData')" />
        <BaseTable
          v-else
          :columns="columns"
          :items="paginatedRows"
          :loading="loading"
          :show-index="true"
          :show-pagination="true"
          :page="page"
          :page-size="pageSize"
          :total="filteredRows.length"
          :show-checkbox-column="false"
          :show-actions-column="true"
          :show-view="true"
          height="520px"
          @view="handleView"
          @update:page="onPageChange"
          @update:pageSize="onPageSizeChange"
        >
          <template #cell-className="{ item }">
            <div class="class-label">
              <span class="class-name">{{ item.className || '-' }}</span>
              <span v-if="item.classCode" class="class-code">({{ item.classCode }})</span>
            </div>
          </template>
          <template #cell-date="{ item }">
            {{ formatDate(item.date) }}
          </template>
          <template #cell-timeRange="{ item }">
            {{ formatTimeRange(item.startTime, item.endTime) }}
          </template>
          <template #cell-dayOfWeek="{ item }">
            {{ dayOfWeekLabel(item.dayOfWeek) }}
          </template>
          <template #cell-sessionAttendanceStatus="{ item }">
            <BaseBadge
              :label="attendanceLabel(item.sessionAttendanceStatus)"
              :type="attendanceColor(item.sessionAttendanceStatus)"
              size="small"
            />
          </template>
          <template #cell-classScheduleStatus="{ item }">
            <BaseBadge
              :label="scheduleLabel(item.classScheduleStatus)"
              :type="scheduleColor(item.classScheduleStatus)"
              size="small"
            />
          </template>
        </BaseTable>
      </div>
    </div>

    <ClassScheduleDetailDialog
      :visible="detailVisible"
      :schedule-id="detailScheduleId"
      @update:visible="detailVisible = $event"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'
import FilterComponent from '@/components/filter-component/FilterComponent.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import ClassScheduleDetailDialog from '@/views/class/ClassScheduleDetailDialog.vue'
import type { ClassScheduleModel } from '@/api/ClassScheduleApi'
import { ClassScheduleStatus, SessionAttendanceStatus } from '@/types'
import { getDayOfWeekKey } from '@/types/daysOfWeek'
import { formatDate as formatDateUtil } from '@/utils/format'
import { useClassStore } from '@/stores/classStore'

type HomeTeacherScheduleRow = ClassScheduleModel & {
  class?: {
    id?: string
    className?: string
    classCode?: string
    classStatus?: number
  } | null
  className?: string
  classCode?: string
}

const ALL_CLASSES_KEY = 'all'
const { t } = useI18n()
const classStore = useClassStore()
const filterComponentRef = ref<InstanceType<typeof FilterComponent> | null>(null)

const { homeTeacherSchedules, homeTeacherScheduleLoading } = storeToRefs(classStore)
const selectedClassId = ref<string>(ALL_CLASSES_KEY)
const filterCriteria = reactive({ startDate: '', endDate: '' })
const page = ref(1)
const pageSize = ref(20)
const detailVisible = ref(false)
const detailScheduleId = ref<string | null>(null)

const columns: BaseTableColumn[] = [
  { key: 'className', labelKey: 'classSchedule.classLabel', minWidth: 220, sticky: true },
  { key: 'date', labelKey: 'classSchedule.date', width: 120, align: 'center' },
  { key: 'timeRange', labelKey: 'classSchedule.timeRange', width: 140, align: 'center' },
  { key: 'dayOfWeek', labelKey: 'classSchedule.dayOfWeek', width: 120, align: 'center' },
  { key: 'sessionAttendanceStatus', labelKey: 'classSchedule.attendanceStatus', width: 170, align: 'center' },
  { key: 'classScheduleStatus', labelKey: 'classSchedule.classScheduleStatus', width: 150, align: 'center' },
]

const loading = computed(() => homeTeacherScheduleLoading.value)

const displayRows = computed<HomeTeacherScheduleRow[]>(() => {
  const rows = (homeTeacherSchedules.value || []).map((item) => ({
    ...item,
    className: resolveClassName(item),
    classCode: resolveClassCode(item),
    dayOfWeek: computeDayOfWeek(item.date, item.dayOfWeek),
  }))
  return rows.sort(compareSchedules)
})

const classOptions = computed(() => {
  const map = new Map<string, string>()
  displayRows.value.forEach((row) => {
    const id = resolveClassId(row)
    if (!id || map.has(id)) return
    map.set(id, formatClassLabel(row))
  })
  return Array.from(map.entries())
    .map(([id, label]) => ({ id, label }))
    .sort((a, b) => a.label.localeCompare(b.label))
})

const filteredRows = computed(() => {
  let rows = displayRows.value
  if (selectedClassId.value !== ALL_CLASSES_KEY) {
    rows = rows.filter((row) => resolveClassId(row) === selectedClassId.value)
  }
  const start = filterCriteria.startDate ? new Date(filterCriteria.startDate) : null
  const end = filterCriteria.endDate ? new Date(filterCriteria.endDate) : null
  if (start || end) {
    rows = rows.filter((row) => {
      if (!row.date) return false
      const dateOnly = String(row.date).substring(0, 10)
      const rowDate = new Date(dateOnly)
      if (Number.isNaN(rowDate.valueOf())) return false
      if (start && rowDate < start) return false
      if (end && rowDate > end) return false
      return true
    })
  }
  return rows
})

const paginatedRows = computed(() => {
  const startIdx = (page.value - 1) * pageSize.value
  return filteredRows.value.slice(startIdx, startIdx + pageSize.value)
})

watch(selectedClassId, () => {
  page.value = 1
})

watch(
  () => [filterCriteria.startDate, filterCriteria.endDate],
  () => {
    page.value = 1
  }
)

onMounted(async () => {
  filterComponentRef.value?.initListHeaderParams({ listParams: [], listBtn: [] }, { skipPermission: true })
  await classStore.fetchHomeTeacherSchedules()
})

function resetFilters() {
  selectedClassId.value = ALL_CLASSES_KEY
  filterCriteria.startDate = ''
  filterCriteria.endDate = ''
}

function onPageChange(val: number) {
  page.value = val
}

function onPageSizeChange(size: number) {
  page.value = 1
  pageSize.value = size
}

function compareSchedules(a: HomeTeacherScheduleRow, b: HomeTeacherScheduleRow) {
  const dateA = new Date(`${a.date}T${a.startTime ?? '00:00'}:00`).getTime()
  const dateB = new Date(`${b.date}T${b.startTime ?? '00:00'}:00`).getTime()
  const safeA = Number.isNaN(dateA) ? 0 : dateA
  const safeB = Number.isNaN(dateB) ? 0 : dateB
  return safeA - safeB
}

function resolveClassId(item: HomeTeacherScheduleRow) {
  return item.classId || item.class?.id || ''
}

function resolveClassName(item: HomeTeacherScheduleRow) {
  return item.class?.className || (item as any).className || ''
}

function resolveClassCode(item: HomeTeacherScheduleRow) {
  return item.class?.classCode || (item as any).classCode || ''
}

function formatClassLabel(item: HomeTeacherScheduleRow) {
  const name = resolveClassName(item)
  const code = resolveClassCode(item)
  if (name && code) return `${name} (${code})`
  return name || code || '-'
}

function formatDate(value?: string | Date | null) {
  if (!value) return '-'
  return formatDateUtil(value, 'DD/MM/YYYY') || '-'
}

function formatTimeRange(start?: string | null, end?: string | null) {
  const s = start ? start.toString().substring(0, 5) : '--:--'
  const e = end ? end.toString().substring(0, 5) : '--:--'
  return `${s} - ${e}`
}

function dayOfWeekLabel(day?: number | null) {
  if (day === null || day === undefined) return '-'
  try {
    return t(getDayOfWeekKey(day))
  } catch {
    return '-'
  }
}

function computeDayOfWeek(date?: string | Date | null, fallback?: number | null) {
  if (date) {
    const d = new Date(date)
    if (Number.isNaN(d.getTime())) return fallback ?? 0
    const jsDay = d.getDay()
    return jsDay === 0 ? 7 : jsDay
  }
  return fallback ?? 0
}

function attendanceLabel(status?: number | null) {
  switch (status) {
    case SessionAttendanceStatus.Checked:
      return t('classSchedule.attendanceChecked')
    case SessionAttendanceStatus.Confirmed:
      return t('classSchedule.attendanceConfirmed')
    default:
      return t('classSchedule.attendanceNotChecked')
  }
}

function attendanceColor(status?: number | null) {
  switch (status) {
    case SessionAttendanceStatus.Checked:
      return 'warning'
    case SessionAttendanceStatus.Confirmed:
      return 'success'
    default:
      return 'info'
  }
}

function scheduleLabel(status?: number | null) {
  switch (status) {
    case ClassScheduleStatus.Completed:
      return t('classStatus.completed')
    case ClassScheduleStatus.Cancelled:
      return t('classStatus.cancelled')
    default:
      return t('classSchedule.notStarted')
  }
}

function scheduleColor(status?: number | null) {
  switch (status) {
    case ClassScheduleStatus.Completed:
      return 'success'
    case ClassScheduleStatus.Cancelled:
      return 'danger'
    default:
      return 'info'
  }
}

function handleView(item: HomeTeacherScheduleRow) {
  if (!item.id) return
  detailScheduleId.value = item.id
  detailVisible.value = true
}
</script>

<style scoped>
.home-teacher-schedule-page {
  padding: 20px;
}

.filters {
  background: #fff;
  border-radius: 12px;
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
</style>
