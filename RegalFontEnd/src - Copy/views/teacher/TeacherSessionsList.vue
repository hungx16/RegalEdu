<template>
  <div class="teacher-sessions-page">
    <FilterComponent ref="filterComponentRef" headerTitle="teacherSessions.listTitle"
      headerDesc="teacherSessions.listDesc" @add="openWorkLogDialog" @updateData="openReassignDialog" class="mb-6" />

    <div class="row g-4 mb-8">
      <div class="col-12 col-md-6">
        <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
          <div class="d-flex justify-content-between align-items-start mb-2">
            <span class="fw-semibold fs-5">{{ t('classScheduleStats.totalSessionsTitle') }}</span>
            <i class="bi bi-calendar3 fs-4 text-body-secondary"></i>
          </div>
          <div class="fs-2 fw-bold mb-1">{{ totalSessions }}</div>
          <div class="fs-7 text-body-secondary">{{ t('teacherSessions.listDesc') }}</div>
        </div>
      </div>
      <div class="col-12 col-md-6">
        <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
          <div class="d-flex justify-content-between align-items-start mb-2">
            <span class="fw-semibold fs-5">{{ t('common.TotalTeachers') }}</span>
            <i class="bi bi-person-badge fs-4 text-body-secondary"></i>
          </div>
          <div class="fs-2 fw-bold mb-1">{{ totalTeachers }}</div>
          <div class="fs-7 text-body-secondary">{{ t('teacherSessions.listDesc') }}</div>
        </div>
      </div>
    </div>

    <div class="card mb-10 w-100 mt-5">
      <div class="card-header card-header-stretch">
        <div class="card-title d-flex flex-column">
          <h3 class="fw-bold fs-4 mb-1">{{ t('teacherSessions.listTitle') }}</h3>
          <span class="text-body-secondary fw-light fs-8">{{ t('teacherSessions.listDesc') }}</span>
        </div>
      </div>
      <div class="card-body py-6 px-2 px-md-6">
        <div class="mb-6">
          <el-form label-position="top">
            <el-row :gutter="16">
              <el-col :xs="24" :md="8">
                <el-form-item :label="t('teacherSessions.teacherLabel')">
                  <el-select v-model="filters.teacherId" filterable clearable
                    :placeholder="t('teacherSessions.teacherPlaceholder')" :disabled="teacherLoading">
                    <el-option :label="t('common.all')" value="" />
                    <el-option v-for="teacher in teachers" :key="teacher.id" :label="formatTeacherLabel(teacher)"
                      :value="teacher.id" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :xs="24" :md="8">
                <el-form-item :label="t('classScheduleStats.startDateFrom')">
                  <el-date-picker v-model="filters.fromDate" type="date" placeholder="dd/mm/yyyy" format="DD/MM/YYYY"
                    value-format="YYYY-MM-DD" clearable />
                </el-form-item>
              </el-col>
              <el-col :xs="24" :md="8">
                <el-form-item :label="t('classScheduleStats.startDateTo')">
                  <el-date-picker v-model="filters.toDate" type="date" placeholder="dd/mm/yyyy" format="DD/MM/YYYY"
                    value-format="YYYY-MM-DD" clearable />
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
          <div class="d-flex justify-content-end gap-2">
            <el-button size="small" @click="resetFilters">
              {{ t('common.clearFilters') }}
            </el-button>
            <el-button size="small" type="primary" @click="applyFilters">
              {{ t('common.search') }}
            </el-button>
          </div>
        </div>
        <el-skeleton v-if="loading" animated :rows="4" />
        <el-empty v-else-if="!filteredSessionsAll.length" :description="t('common.noData')" class="mb-4" />
        <BaseTable :show-checkbox-column="false" :columns="columns" :items="filteredSessionsAll"
          :loading="loading" :show-pagination="true" :page="page" :page-size="pageSize" :filter="filter"
          :show-index="true" :show-actions-column="false" @update:filter="onTableFilter" @update:page="onPageChange"
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
            <BaseBadge
              v-if="resolveClassScheduleStatus(item) !== null"
              :label="classScheduleStatusLabel(resolveClassScheduleStatus(item))"
              :type="classScheduleStatusColor(resolveClassScheduleStatus(item))"
              size="small"
            />
            <span v-else>-</span>
          </template>
          <template #cell-branchName="{ item }">
            {{ item.branchName || '-' }}
          </template>
          <template #cell-programName="{ item }">
            {{ item.programName || '-' }}
          </template>
          <template #cell-levelName="{ item }">
            {{ item.levelName || '-' }}
          </template>
          <template #cell-moduleName="{ item }">
            {{ item.moduleName || '-' }}
          </template>
          <template #cell-teacherCode="{ item }">
            {{ item.teacherCode || '-' }}
          </template>
          <template #cell-teacherName="{ item }">
            {{ item.teacherName || '-' }}
          </template>
          <template #cell-teacherEnglishName="{ item }">
            {{ item.teacherEnglishName || '-' }}
          </template>
          <template #cell-classCode="{ item }">
            {{ item.classCode || '-' }}
          </template>
          <template #cell-className="{ item }">
            {{ item.className || '-' }}
          </template>
          <template #cell-activityTypeName="{ item }">
            {{ item.activityTypeName || '-' }}
          </template>
          <template #cell-workTypeLabel="{ item }">
            {{ item.workTypeLabel || '-' }}
          </template>
        </BaseTable>
      </div>
    </div>

    <BaseDialogForm :visible="showReassignDialog" :title="t('teacherSessions.reassignTitle')" mode="edit" width="520px"
      :form-data="reassignForm" :loading="reassignLoading" :show-delete="false" :show-action-buttons="false"
      @update:visible="handleReassignVisibleChange">
      <template #form>
        <div class="reassign-form">
          <el-form-item :label="t('teacherSessions.fromTeacherLabel')" :label-width="reassignLabelWidth">
            <el-select v-model="reassignForm.fromTeacherId" filterable clearable
              :placeholder="t('teacherSessions.fromTeacherPlaceholder')" :disabled="teacherLoading">
              <el-option v-for="teacher in teachers" :key="teacher.id" :label="formatTeacherLabel(teacher)"
                :value="teacher.id" />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('teacherSessions.classLabel')" :label-width="reassignLabelWidth">
            <el-select v-model="reassignForm.classId" filterable clearable
              :placeholder="t('teacherSessions.classPlaceholder')"
              :disabled="classLoading || !reassignForm.fromTeacherId">
              <el-option v-for="cls in classes" :key="cls.id" :label="formatClassLabel(cls)" :value="cls.id" />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('teacherSessions.toTeacherLabel')" :label-width="reassignLabelWidth">
            <el-select v-model="reassignForm.toTeacherId" filterable clearable
              :placeholder="t('teacherSessions.toTeacherPlaceholder')"
              :disabled="teacherLoading || !reassignForm.fromTeacherId">
              <el-option v-for="teacher in availableTeachersForReassign" :key="teacher.id"
                :label="formatTeacherLabel(teacher)" :value="teacher.id" />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('classScheduleStats.startDateFrom')" :label-width="reassignLabelWidth">
            <el-date-picker v-model="reassignForm.fromDate" type="date" placeholder="dd/mm/yyyy" format="DD/MM/YYYY"
              value-format="YYYY-MM-DD" clearable />
          </el-form-item>
          <el-form-item :label="t('classScheduleStats.startDateTo')" :label-width="reassignLabelWidth">
            <el-date-picker v-model="reassignForm.toDate" type="date" placeholder="dd/mm/yyyy" format="DD/MM/YYYY"
              value-format="YYYY-MM-DD" clearable />
          </el-form-item>
        </div>
      </template>
      <template #footer-extra>
        <el-button type="primary" class="btn btn-primary" :loading="reassignLoading" :disabled="!canSubmitReassign"
          @click="submitReassign">
          {{ t('teacherSessions.reassignAction') }}
        </el-button>
      </template>
    </BaseDialogForm>

    <BaseDialogForm :visible="showWorkLogDialog" :title="t('teacherSessions.workLogTitle')" mode="create" width="520px"
      :form-data="workLogForm" :loading="workLogLoading" :show-delete="false" :show-action-buttons="false"
      @update:visible="handleWorkLogVisibleChange">
      <template #form>
        <div class="reassign-form">
          <el-form-item :label="t('teacherSessions.branchLabel')" :label-width="reassignLabelWidth">
            <el-select v-model="workLogForm.companyId" filterable clearable
              :placeholder="t('teacherSessions.branchPlaceholder')" :disabled="companyLoading">
              <el-option v-for="company in companies" :key="company.id" :label="company.companyName"
                :value="company.id" />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('teacherSessions.teacherLabel')" :label-width="reassignLabelWidth">
            <el-select v-model="workLogForm.teacherId" filterable clearable
              :placeholder="t('teacherSessions.teacherPlaceholder')"
              :disabled="teacherLoading || !workLogForm.companyId">
              <el-option v-for="teacher in teachersByCompany" :key="teacher.id" :label="formatTeacherLabel(teacher)"
                :value="teacher.id" />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('teacherSessions.workTypeLabel')" :label-width="reassignLabelWidth">
            <el-select v-model="workLogForm.workType" filterable clearable
              :placeholder="t('teacherSessions.workTypePlaceholder')" :disabled="!workLogForm.teacherId">
              <el-option v-for="opt in workTypeOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('teacherSessions.workDate')" :label-width="reassignLabelWidth">
            <el-date-picker v-model="workLogForm.date" type="date" placeholder="dd/mm/yyyy" format="DD/MM/YYYY"
              value-format="YYYY-MM-DD" clearable :disabled="!workLogForm.teacherId" />
          </el-form-item>
          <el-form-item :label="t('teacherSessions.workingTimeLabel')" :label-width="reassignLabelWidth">
            <el-select v-model="workLogForm.workingTimeId" filterable clearable
              :placeholder="t('teacherSessions.workingTimePlaceholder')"
              :disabled="workingTimeLoading || !workLogForm.teacherId">
              <el-option v-for="workingTime in workingTimes" :key="workingTime.id"
                :label="formatWorkingTimeLabel(workingTime.name, workingTime.dayOfWeek, workingTime.startTime, workingTime.endTime)"
                :value="workingTime.id" />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('teacherSessions.startTime')" :label-width="reassignLabelWidth">
            <el-time-picker v-model="workLogForm.startTime" format="HH:mm" value-format="HH:mm:00"
              :disabled="!workLogForm.teacherId || Boolean(workLogForm.workingTimeId)" />
          </el-form-item>
          <el-form-item :label="t('teacherSessions.endTime')" :label-width="reassignLabelWidth">
            <el-time-picker v-model="workLogForm.endTime" format="HH:mm" value-format="HH:mm:00"
              :disabled="!workLogForm.teacherId || Boolean(workLogForm.workingTimeId)" />
          </el-form-item>
          <el-form-item :label="t('teacherSessions.note')" :label-width="reassignLabelWidth">
            <el-input v-model="workLogForm.note" type="textarea" :rows="3" :placeholder="t('teacherSessions.note')"
              :disabled="!workLogForm.teacherId" />
          </el-form-item>
        </div>
      </template>
      <template #footer-extra>
        <el-button type="primary" class="btn btn-primary" :loading="workLogLoading" :disabled="!canSubmitWorkLog"
          @click="submitWorkLog">
          {{ t('common.save') }}
        </el-button>
      </template>
    </BaseDialogForm>

  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import FilterComponent from '@/components/filter-component/FilterComponent.vue'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { useNotificationStore } from '@/stores/notificationStore'
import { useClassStore } from '@/stores/classStore'
import { useCompanyStore } from '@/stores/companyStore'
import { useTeacherStore } from '@/stores/teacherStore'
import { useTeacherSessionStore } from '@/stores/teacherSessionStore'
import { useWorkingTimeStore } from '@/stores/workingTimeStore'
import type { TeacherModel } from '@/api/TeacherApi'
import type { ClassModel } from '@/api/ClassApi'
import { formatDate as formatDateUtil } from '@/utils/format'
import { getDayOfWeekKey } from '@/types/daysOfWeek'
import { ClassScheduleStatus } from '@/types'

const { t } = useI18n()
const notificationStore = useNotificationStore()
const classStore = useClassStore()
const companyStore = useCompanyStore()
const teacherStore = useTeacherStore()
const teacherSessionStore = useTeacherSessionStore()
const workingTimeStore = useWorkingTimeStore()

const filterComponentRef = ref()
const page = ref(1)
const pageSize = ref(20)
const filter = ref({})
const reassignLabelWidth = '120px'

const filters = reactive({
  teacherId: '',
  fromDate: '',
  toDate: '',
})

const reassignForm = reactive({
  classId: '',
  fromTeacherId: '',
  toTeacherId: '',
  fromDate: '',
  toDate: '',
})

const showReassignDialog = ref(false)
const reassignLoading = ref(false)

const workLogForm = reactive({
  companyId: '',
  teacherId: '',
  workType: null as number | null,
  date: '',
  workingTimeId: '',
  startTime: '',
  endTime: '',
  note: '',
})

const showWorkLogDialog = ref(false)
const workLogLoading = ref(false)

const teachers = computed(() => teacherStore.teachers || [])
const classes = computed(() => classStore.classes || [])
const teacherLoading = computed(() => teacherStore.loading)
const classLoading = computed(() => classStore.loading)
const loading = computed(() => teacherSessionStore.loading)
const companies = computed(() => companyStore.companies || [])
const companyLoading = computed(() => companyStore.loading)
const workingTimes = computed(() => workingTimeStore.workingTimes || [])
const workingTimeLoading = computed(() => workingTimeStore.loading)

const selectedWorkingTime = computed(() => {
  if (!workLogForm.workingTimeId) return null
  return workingTimes.value.find((time) => time.id === workLogForm.workingTimeId) || null
})

const availableTeachersForReassign = computed(() => {
  if (!reassignForm.fromTeacherId) return teachers.value
  return teachers.value.filter((teacher) => teacher.id !== reassignForm.fromTeacherId)
})

const teachersByCompany = computed(() => {
  if (!workLogForm.companyId) return []
  return teachers.value.filter((teacher) => teacher.companyId === workLogForm.companyId)
})

const workTypeFilterOptions = computed(() => [
  { label: t('teacherSessions.workTypeInterview'), value: t('teacherSessions.workTypeInterview'), isLocale: false },
  { label: t('teacherSessions.workTypeEvent'), value: t('teacherSessions.workTypeEvent'), isLocale: false },
])

const columns = computed<BaseTableColumn[]>(() => [
  { key: 'date', labelKey: 'teacherSessions.date', filterType: 'date', width: 120, align: 'center', sticky: true },
  { key: 'branchName', labelKey: 'teacherSessions.branchName', filterType: 'text', width: 180 },
  { key: 'programName', labelKey: 'teacherSessions.programName', filterType: 'text', width: 160 },
  { key: 'levelName', labelKey: 'teacherSessions.levelName', filterType: 'text', width: 140 },
  { key: 'moduleName', labelKey: 'teacherSessions.moduleName', filterType: 'text', width: 140 },
  { key: 'teacherCode', labelKey: 'teacherSessions.teacherCode', filterType: 'text', width: 140 },
  { key: 'teacherName', labelKey: 'teacherSessions.teacherName', filterType: 'text', width: 200 },
  { key: 'teacherEnglishName', labelKey: 'teacherSessions.teacherEnglishName', filterType: 'text', width: 200 },
  { key: 'classCode', labelKey: 'teacherSessions.classCode', filterType: 'text', width: 140 },
  { key: 'className', labelKey: 'teacherSessions.className', filterType: 'text', width: 200 },
  { key: 'activityTypeName', labelKey: 'teacherSessions.activityTypeName', filterType: 'text', width: 180 },
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

const listHeaderParams = {
  listParams: [],
  listBtn: [
    { event: 'add', label: 'teacherSessions.addWorkLog', type: 'add' },
    { event: 'updateData', label: 'teacherSessions.reassignAction', type: 'updateData' },
  ],
}

const displaySessions = computed(() => {
  const itemsWithWorkType = (teacherSessionStore.sessions || []).map((item) => ({
    ...item,
    workTypeLabel: resolveWorkTypeLabel(item),
  }))
  return [...itemsWithWorkType].sort((a, b) => {
    const aTime = new Date(`${a.date}T${a.startTime ?? '00:00'}`).getTime()
    const bTime = new Date(`${b.date}T${b.startTime ?? '00:00'}`).getTime()
    return aTime - bTime
  })
})

const totalSessions = computed(() => displaySessions.value.length)
const totalTeachers = computed(() => {
  const keys = displaySessions.value
    .map((item) => item.teacherId || item.teacherCode || item.teacherName || '')
    .filter((val) => Boolean(val))
  return new Set(keys).size
})

const workTypeOptions = computed(() => [
  { value: 0, label: t('teacherSessions.workTypeInterview') },
  { value: 1, label: t('teacherSessions.workTypeEvent') },
])

const canSubmitReassign = computed(() => {
  return (
    Boolean(reassignForm.classId) &&
    Boolean(reassignForm.fromTeacherId) &&
    Boolean(reassignForm.toTeacherId) &&
    reassignForm.fromTeacherId !== reassignForm.toTeacherId
  )
})

const canSubmitWorkLog = computed(() => {
  if (workLogForm.workingTimeId && !isWorkingTimeDayMatch()) return false
  return Boolean(workLogForm.teacherId) &&
    workLogForm.workType !== null &&
    Boolean(workLogForm.date) &&
    Boolean(workLogForm.startTime) &&
    Boolean(workLogForm.endTime)
})

const filteredSessionsAll = computed(() => {
  let arr = displaySessions.value
  Object.entries(filter.value).forEach(([key, val]) => {
    if (val != null && val !== '') {
      if (key === 'date') {
        arr = arr.filter((item: any) => {
          if (!item[key]) return false
          const dateOnly = String(item[key]).substring(0, 10)
          return dateOnly === val
        })
      } else {
        arr = arr.filter((item: any) =>
          String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase())
        )
      }
    }
  })
  return arr
})

function formatTeacherLabel(teacher: TeacherModel) {
  const name = teacher.applicationUser?.fullName || ''
  const code = teacher.applicationUser?.userCode || ''
  if (name && code) return `${name} (${code})`
  return name || code || '-'
}

function formatClassLabel(cls: ClassModel) {
  if (cls.classCode) return `${cls.className} (${cls.classCode})`
  return cls.className || '-'
}

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

function formatWorkingTimeLabel(name?: string | null, dayOfWeek?: number | null, start?: string | null, end?: string | null) {
  const dayLabel = dayOfWeek ? formatDayOfWeek(dayOfWeek) : '-'
  const timeRange = formatTimeRange(start, end)
  if (name) return `${name} - ${dayLabel} (${timeRange})`
  return `${dayLabel} (${timeRange})`
}

function getDateDayOfWeek(dateValue?: string | null) {
  if (!dateValue) return null
  const date = new Date(`${dateValue}T00:00:00`)
  if (Number.isNaN(date.getTime())) return null
  const day = date.getDay()
  return day === 0 ? 7 : day
}

function isWorkingTimeDayMatch() {
  if (!workLogForm.workingTimeId || !workLogForm.date) return true
  const workingTime = selectedWorkingTime.value
  if (!workingTime) return true
  const dayOfWeek = getDateDayOfWeek(workLogForm.date)
  if (!dayOfWeek) return true
  return dayOfWeek === workingTime.dayOfWeek
}

function validateWorkingTimeDayMatch() {
  if (!workLogForm.workingTimeId || !workLogForm.date) return
  if (!isWorkingTimeDayMatch()) {
    notificationStore.showToast('warning', { key: 'teacherSessions.workLogInvalidDayOfWeek' })
    workLogForm.workingTimeId = ''
    workLogForm.startTime = ''
    workLogForm.endTime = ''
  }
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

async function applyFilters() {
  page.value = 1
  await teacherSessionStore.fetchSessions({
    teacherId: filters.teacherId || undefined,
    fromDate: filters.fromDate || undefined,
    toDate: filters.toDate || undefined,
  })
}

function openReassignDialog() {
  showReassignDialog.value = true
}

function closeReassignDialog() {
  showReassignDialog.value = false
  reassignForm.classId = ''
  reassignForm.fromTeacherId = ''
  reassignForm.toTeacherId = ''
  reassignForm.fromDate = ''
  reassignForm.toDate = ''
}

function openWorkLogDialog() {
  showWorkLogDialog.value = true
}

function closeWorkLogDialog() {
  showWorkLogDialog.value = false
  workLogForm.companyId = ''
  workLogForm.teacherId = ''
  workLogForm.workType = null
  workLogForm.date = ''
  workLogForm.workingTimeId = ''
  workLogForm.startTime = ''
  workLogForm.endTime = ''
  workLogForm.note = ''
}

function handleWorkLogVisibleChange(val: boolean) {
  showWorkLogDialog.value = val
  if (!val) {
    closeWorkLogDialog()
  }
}

function handleReassignVisibleChange(val: boolean) {
  showReassignDialog.value = val
  if (!val) {
    closeReassignDialog()
  }
}

async function submitReassign() {
  if (!canSubmitReassign.value) {
    notificationStore.showToast('warning', { key: 'teacherSessions.reassignInvalid' })
    return
  }
  reassignLoading.value = true
  try {
    const res = await teacherSessionStore.reassignTeacherSessions({
      classId: reassignForm.classId,
      fromTeacherId: reassignForm.fromTeacherId,
      toTeacherId: reassignForm.toTeacherId,
      fromDate: reassignForm.fromDate || undefined,
      toDate: reassignForm.toDate || undefined,
    })
    if (res?.succeeded) {
      notificationStore.showToast('success', { key: 'teacherSessions.reassignSuccess' })
      closeReassignDialog()
      await applyFilters()
    } else {
      notificationStore.showToast('error', { key: 'teacherSessions.reassignFailed' })
    }
  } catch (error) {
    console.error('Error reassigning teacher sessions:', error)
    notificationStore.showToast('error', { key: 'teacherSessions.reassignFailed' })
  } finally {
    reassignLoading.value = false
  }
}

async function submitWorkLog() {
  if (!canSubmitWorkLog.value) {
    notificationStore.showToast('warning', { key: 'teacherSessions.workLogInvalid' })
    return
  }
  workLogLoading.value = true
  try {
    const res = await teacherSessionStore.addTeacherWorkLog({
      teacherWorkLogModel: {
        teacherId: workLogForm.teacherId,
        workType: workLogForm.workType as number,
        date: workLogForm.date,
        startTime: workLogForm.startTime,
        endTime: workLogForm.endTime,
        note: workLogForm.note || undefined,
        workingTimeId: workLogForm.workingTimeId || undefined,
      },
    })
    if (res?.succeeded) {
      notificationStore.showToast('success', { key: 'teacherSessions.workLogSuccess' })
      closeWorkLogDialog()
      await applyFilters()
    } else {
      notificationStore.showToast('error', { key: 'teacherSessions.workLogFailed' })
    }
  } catch (error) {
    console.error('Error adding teacher work log:', error)
    notificationStore.showToast('error', { key: 'teacherSessions.workLogFailed' })
  } finally {
    workLogLoading.value = false
  }
}

function onTableFilter(val: Record<string, any>) {
  filter.value = val
  page.value = 1
}

function resetFilters() {
  filters.teacherId = ''
  filters.fromDate = ''
  filters.toDate = ''
  void applyFilters()
}

function onPageChange(val: number) {
  page.value = val
}

function onPageSizeChange(size: number) {
  pageSize.value = size
  page.value = 1
}

onMounted(async () => {
  filterComponentRef.value?.initListHeaderParams(listHeaderParams)
  await companyStore.fetchAllCompanies()
  await teacherStore.fetchAllTeacher()
  await workingTimeStore.fetchAllWorkingTimes()
  await applyFilters()
})

watch(
  () => reassignForm.fromTeacherId,
  async (teacherId) => {
    reassignForm.classId = ''
    reassignForm.toTeacherId = ''
    if (teacherId) {
      await classStore.fetchAllClassesByTeacherId(teacherId)
    } else {
      classStore.classes = []
    }
  }
)

watch(
  () => reassignForm.toTeacherId,
  (teacherId) => {
    if (teacherId && teacherId === reassignForm.fromTeacherId) {
      reassignForm.toTeacherId = ''
    }
  }
)

watch(
  () => workLogForm.companyId,
  () => {
    workLogForm.teacherId = ''
    workLogForm.workType = null
    workLogForm.date = ''
    workLogForm.workingTimeId = ''
    workLogForm.startTime = ''
    workLogForm.endTime = ''
    workLogForm.note = ''
  }
)

watch(
  () => workLogForm.workingTimeId,
  (value) => {
    if (!value) {
      workLogForm.startTime = ''
      workLogForm.endTime = ''
      return
    }
    const workingTime = selectedWorkingTime.value
    if (workingTime) {
      workLogForm.startTime = workingTime.startTime
      workLogForm.endTime = workingTime.endTime
    }
    validateWorkingTimeDayMatch()
  }
)

watch(
  () => workLogForm.date,
  () => {
    if (workLogForm.workingTimeId) {
      validateWorkingTimeDayMatch()
    }
  }
)
</script>

<style scoped>
.teacher-sessions-page {
  padding: 20px;
}

.reassign-form :deep(.el-form-item__label) {
  text-align: left;
  justify-content: flex-start;
}

.cell-wrap {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.cell-primary {
  font-weight: 600;
}

.cell-sub {
  color: #6b7280;
  font-size: 12px;
}
</style>
