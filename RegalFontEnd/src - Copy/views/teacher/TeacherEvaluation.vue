<template>
  <div class="teacher-evaluation">
    <div class="evaluation-filter">
      <el-select v-model="selectedClassId" clearable filterable :placeholder="filterPlaceholder"
        class="evaluation-filter__control">
        <el-option v-for="cls in classOptions" :key="cls.value" :label="cls.label" :value="cls.value" />
      </el-select>
      <el-date-picker v-model="filters.fromDate" type="date" value-format="YYYY-MM-DD" format="DD/MM/YYYY"
        :placeholder="t('classScheduleStats.startDateFrom')" class="evaluation-filter__control" clearable />
      <el-date-picker v-model="filters.toDate" type="date" value-format="YYYY-MM-DD" format="DD/MM/YYYY"
        :placeholder="t('classScheduleStats.startDateTo')" class="evaluation-filter__control" clearable />
      <el-button size="small" :loading="evaluationLoading" @click="loadTeacherEvaluations(true)">
        {{ t('common.search') }}
      </el-button>
    </div>

    <el-skeleton v-if="evaluationLoading" animated :rows="4" />
    <el-empty v-else-if="groupedEvaluations.length === 0" :description="t('common.noData')" />
    <el-collapse v-else v-model="expandedClasses" class="evaluation-accordion">
      <el-collapse-item v-for="group in groupedEvaluations" :key="group.key" :name="group.key">
        <template #title>
          <div class="eval-card__header">
            <div>
              <div class="eval-card__title">{{ group.classLabel }}</div>
              <div class="text-muted fs-7">{{ group.itemsCount }} {{ t('teacher.evaluation') }}</div>
            </div>
          </div>
        </template>

        <el-collapse v-model="expandedSessions[group.key]" class="session-accordion">
          <el-collapse-item v-for="session in group.sessions" :key="session.key" :name="session.key">
            <template #title>
              <div class="session-block__header">
                <!-- <div class="session-title">{{ session.title }}</div> -->
                <div class="session-meta">{{ session.meta }}</div>
              </div>
            </template>

            <div v-for="item in session.items"
              :key="item.id || item.normalizedEvaluateDate || item.evaluateDate || `${item.normalizedClassId}-${item.normalizedClassScheduleId}-${item.normalizedStudentName || ''}`"
              class="evaluation-item">
              <div class="evaluation-item__top">
                <div class="fw-semibold">
                  {{ displayStudentName(item) }}
                  <span v-if="displayEvaluationTime(item)" class="student-time">
                    - {{ displayEvaluationTime(item) }}
                  </span>
                </div>
                <div v-if="item.starRating !== null && item.starRating !== undefined" class="stars">
                  <span class="pill">{{ formatRating(item.starRating) }}</span>
                  <el-rate class="rating-stars" :model-value="item.starRating || 0" disabled :allow-half="true" :max="5"
                    :colors="['#f7ba2a', '#f7ba2a', '#f7ba2a']" :void-color="'#e5e7eb'" :show-score="false" />
                </div>
              </div>
              <div class="evaluation-content" v-if="displayComment(item)">
                {{ displayComment(item) }}
              </div>
              <el-alert v-if="item.responseContent" type="info" :closable="false" show-icon class="mt-2"
                :title="t('common.note')" :description="item.responseContent" />
            </div>
          </el-collapse-item>
        </el-collapse>
      </el-collapse-item>
    </el-collapse>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useEvaluateTeacherStore } from '@/stores/evaluateTeacherStore'
import { ClassApi } from '@/api/ClassApi'
import { ClassScheduleApi } from '@/api/ClassScheduleApi'
import type { EvaluateTeacherModel } from '@/api/EvaluateTeacherApi'
import { formatDate as formatDateUtil } from '@/utils/format'

const props = defineProps<{ teacherId: string }>()
const { t } = useI18n()
const evaluateTeacherStore = useEvaluateTeacherStore()
const classApi = new ClassApi()
const scheduleApi = new ClassScheduleApi()

const selectedClassId = ref<string | null>(null)
const filters = ref({
  fromDate: '',
  toDate: '',
})
const lastEvaluationTeacherId = ref<string | null>(null)
const classInfoMap = ref<Record<string, { className?: string; classCode?: string }>>({})
const scheduleInfoMap = ref<
  Record<
    string,
    {
      sessionIndex?: number | null;
      date?: string | null;
      startTime?: string | null;
      endTime?: string | null;
      classId?: string | null;
      scheduleName?: string | null;
    }
  >
>({})
const expandedClasses = ref<string[]>([])
const expandedSessions = ref<Record<string, string[]>>({})

const evaluationLoading = computed(() => evaluateTeacherStore.loading)

interface NormalizedEvaluation extends EvaluateTeacherModel {
  normalizedClassId: string;
  normalizedClassName: string;
  normalizedClassCode: string;
  normalizedClassScheduleId: string;
  normalizedClassScheduleName: string;
  normalizedSessionIndex?: number | null;
  normalizedSessionDate?: string | null;
  normalizedSessionStartTime?: string | null;
  normalizedSessionEndTime?: string | null;
  normalizedEvaluateDate?: string | null;
  normalizedStudentName?: string | null;
}

const normalizedEvaluations = computed<NormalizedEvaluation[]>(() =>
  evaluateTeacherStore.evaluations.map((ev) => {
    const cls = ev.class || (ev as any).Class || null
    const sched = ev.classSchedule || (ev as any).classSchedule || (ev as any).ClassSchedule || null

    const classScheduleId = ev.classScheduleId || (sched as any)?.id || ''
    const scheduleInfo = classScheduleId ? scheduleInfoMap.value[classScheduleId] : undefined
    const classId =
      ev.classId ||
      cls?.id ||
      sched?.classId ||
      (sched as any)?.ClassId ||
      scheduleInfo?.classId ||
      'unknown'
    const classInfo = classId ? classInfoMap.value[classId] : undefined
    const className =
      ev.className ||
      cls?.className ||
      sched?.className ||
      (sched as any)?.class?.className ||
      classInfo?.className ||
      ''
    const classCode =
      ev.classCode ||
      cls?.classCode ||
      sched?.classCode ||
      (sched as any)?.class?.classCode ||
      classInfo?.classCode ||
      ''
    const classScheduleName =
      ev.classScheduleName ||
      sched?.scheduleName ||
      (sched as any)?.title ||
      sched?.name ||
      scheduleInfo?.scheduleName ||
      ''
    const sessionIndex =
      (ev as any).sessionIndex ??
      sched?.sessionIndex ??
      (sched as any)?.sessionNo ??
      (sched as any)?.sessionNumber ??
      scheduleInfo?.sessionIndex ??
      null
    const studentName = ev.studentName || ev.student?.fullName || ''
    const sessionDate = sched?.date || scheduleInfo?.date || ev.evaluateDate || ''
    const sessionStartTime = sched?.startTime || scheduleInfo?.startTime || ''
    const sessionEndTime = sched?.endTime || scheduleInfo?.endTime || ''
    const evaluateDate = ev.evaluateDate || ''

    const sessionIndexValue =
      typeof sessionIndex === 'number' ? sessionIndex : Number(sessionIndex)

    return {
      ...ev,
      normalizedClassId: classId,
      normalizedClassName: className,
      normalizedClassCode: classCode,
      normalizedClassScheduleId: classScheduleId,
      normalizedClassScheduleName: classScheduleName,
      normalizedSessionIndex: Number.isNaN(sessionIndexValue) ? null : sessionIndexValue,
      normalizedSessionDate: sessionDate,
      normalizedSessionStartTime: sessionStartTime,
      normalizedSessionEndTime: sessionEndTime,
      normalizedEvaluateDate: evaluateDate,
      normalizedStudentName: studentName,
    }
  })
)

const classOptions = computed(() => {
  const map = new Map<string, { value: string; label: string }>()
  const classEntries = Object.entries(classInfoMap.value)
  classEntries.forEach(([id, info]) => {
    const labelParts = [info.className || info.classCode].filter(Boolean)
    map.set(id, { value: id, label: labelParts.join(' - ') || t('common.unknown') })
  })
  normalizedEvaluations.value.forEach((ev) => {
    const key = ev.normalizedClassId || 'unknown'
    if (!map.has(key)) {
      const labelParts = [ev.normalizedClassName || ev.normalizedClassCode].filter(Boolean)
      map.set(key, { value: key, label: labelParts.join(' - ') || t('common.unknown') })
    }
  })
  return Array.from(map.values())
})

const filterPlaceholder = computed(() => {
  const classLabel = t('teacherSessions.className')
  return `${t('common.filter')} ${classLabel}`
})

const groupedEvaluations = computed(() => {
  const toDateValue = (value?: string | null) => {
    const time = value ? new Date(value).getTime() : NaN
    return Number.isNaN(time) ? null : time
  }
  const fromDateValue = filters.value.fromDate
    ? toDateValue(`${filters.value.fromDate}T00:00:00`)
    : null
  const toDateValueFilter = filters.value.toDate
    ? toDateValue(`${filters.value.toDate}T23:59:59`)
    : null

  const list = normalizedEvaluations.value.filter((ev) => {
    if (!selectedClassId.value) return true
    return ev.normalizedClassId === selectedClassId.value
  })

  const filteredList = list.filter((ev) => {
    if (!fromDateValue && !toDateValueFilter) return true
    const timeValue = toDateValue(ev.normalizedEvaluateDate || ev.normalizedSessionDate || '')
    if (!timeValue) return false
    if (fromDateValue && timeValue < fromDateValue) return false
    if (toDateValueFilter && timeValue > toDateValueFilter) return false
    return true
  })

  const classMap = new Map<
    string,
    {
      key: string
      classLabel: string
      evaluations: NormalizedEvaluation[]
    }
  >()

  filteredList.forEach((ev) => {
    const key = ev.normalizedClassId || 'unknown'
    const labelParts = [ev.normalizedClassName || ev.normalizedClassCode].filter(Boolean)
    const classLabel = labelParts.join(' - ') || t('common.unknown')
    if (!classMap.has(key)) {
      classMap.set(key, { key, classLabel, evaluations: [] })
    }
    classMap.get(key)?.evaluations.push(ev)
  })

  const groups = Array.from(classMap.values()).map((group) => {
    const sessionMap = new Map<
      string,
      { key: string; title: string; meta: string; items: NormalizedEvaluation[]; dateValue: number; sessionIndex?: number | null }
    >()
    group.evaluations.forEach((ev) => {
      const sessionKey =
        ev.normalizedClassScheduleId ||
        ev.id ||
        ev.normalizedEvaluateDate ||
        `${ev.normalizedSessionDate}-${ev.normalizedClassId}`
      if (!sessionMap.has(sessionKey)) {
        const sessionTitle = t('teacherSessions.sessionName')
        const sessionOrder =
          ev.normalizedSessionIndex !== null && ev.normalizedSessionIndex !== undefined
            ? `${t('classSchedule.sessionOrder')} ${ev.normalizedSessionIndex}`
            : ''
        const sessionDateText = ev.normalizedSessionDate ? formatDateDisplay(ev.normalizedSessionDate) : ''
        const sessionTimeText = formatTimeRange(
          ev.normalizedSessionStartTime,
          ev.normalizedSessionEndTime
        )
        const metaParts = [sessionOrder, sessionDateText, sessionTimeText].filter(Boolean)
        const meta = metaParts.join(' - ')
        sessionMap.set(sessionKey, {
          key: sessionKey,
          title: sessionTitle,
          meta,
          items: [],
          dateValue: toDateValue(ev.normalizedSessionDate) ?? 0,
          sessionIndex: ev.normalizedSessionIndex ?? null,
        })
      }
      sessionMap.get(sessionKey)?.items.push(ev)
    })

    const sessions = Array.from(sessionMap.values()).map((s) => {
      s.items.sort(
        (a, b) =>
          (toDateValue(b.normalizedEvaluateDate || b.normalizedSessionDate) ?? 0) -
          (toDateValue(a.normalizedEvaluateDate || a.normalizedSessionDate) ?? 0)
      )
      return s
    })

    sessions.sort((a, b) => {
      const aIdx = typeof a.sessionIndex === 'number' ? a.sessionIndex : Number.MAX_SAFE_INTEGER
      const bIdx = typeof b.sessionIndex === 'number' ? b.sessionIndex : Number.MAX_SAFE_INTEGER
      if (aIdx !== bIdx) return aIdx - bIdx
      return a.dateValue - b.dateValue
    })

    return {
      ...group,
      sessions,
      itemsCount: group.evaluations.length,
    }
  })

  return groups.sort((a, b) => a.classLabel.localeCompare(b.classLabel))
})

watch(
  groupedEvaluations,
  (groups) => {
    expandedClasses.value = groups.map((g) => g.key)
    const nextSessions: Record<string, string[]> = {}
    groups.forEach((g) => {
      nextSessions[g.key] = g.sessions.map((s) => s.key)
    })
    expandedSessions.value = nextSessions
  },
  { immediate: true }
)

async function loadTeacherEvaluations(force = false) {
  const teacherId = props.teacherId
  if (!teacherId) {
    evaluateTeacherStore.reset()
    classInfoMap.value = {}
    scheduleInfoMap.value = {}
    return
  }
  if (!force && lastEvaluationTeacherId.value === teacherId && evaluateTeacherStore.evaluations.length) return
  lastEvaluationTeacherId.value = teacherId
  const res = await evaluateTeacherStore.fetchTeacherEvaluations(teacherId)
  const evaluations = res?.data || []
  await loadClassInfo(teacherId)
  await loadScheduleInfo(evaluations)
}

watch(
  () => props.teacherId,
  () => {
    selectedClassId.value = null
    evaluateTeacherStore.reset()
    classInfoMap.value = {}
    scheduleInfoMap.value = {}
    void loadTeacherEvaluations()
  }
)

onMounted(() => {
  void loadTeacherEvaluations()
})

function formatDateDisplay(value?: string | null) {
  return formatDateUtil(value || '', 'DD/MM/YYYY') || '-'
}

function formatDateTimeDisplay(value?: string | null) {
  return formatDateUtil(value || '', 'DD/MM/YYYY HH:mm') || ''
}

function formatTimeRange(start?: string | null, end?: string | null) {
  const startTime = normalizeTime(start)
  const endTime = normalizeTime(end)
  if (startTime && endTime) return `${startTime}-${endTime}`
  return startTime || endTime || ''
}

function normalizeTime(value?: string | null) {
  if (!value) return ''
  return value.length >= 5 ? value.slice(0, 5) : value
}

function displayStudentName(item: NormalizedEvaluation) {
  return (
    item.normalizedStudentName ||
    item.evaluateNick ||
    item.evaluateName ||
    t('common.unknown')
  )
}

function displayEvaluationTime(item: NormalizedEvaluation) {
  const value = item.normalizedEvaluateDate || item.evaluateDate || ''
  if (!value) return ''
  return formatDateTimeDisplay(value)
}

function formatRating(val?: number | null) {
  return formatNumericValue(val)
}

function formatNumericValue(val?: number | null) {
  if (val === null || val === undefined) return ''
  const num = Number(val)
  if (Number.isNaN(num)) return ''
  return num % 1 === 0 ? `${num}` : num.toFixed(1)
}

function displayComment(item: NormalizedEvaluation) {
  const comment = item.evaluateName ? item.evaluateName.toString().trim() : ''
  const name = displayStudentName(item).toString().trim()
  if (!comment || comment === name) return ''
  return comment
}

async function loadClassInfo(teacherId: string) {
  try {
    const res = await classApi.getClassByTeacherId(teacherId)
    const items = res?.data || []
    classInfoMap.value = items.reduce(
      (acc: Record<string, { className?: string; classCode?: string }>, item: any) => {
        if (item?.id) {
          acc[item.id] = { className: item.className, classCode: item.classCode }
        }
        return acc
      },
      {}
    )
  } catch (error) {
    console.error('Failed to load classes for teacher', error)
    classInfoMap.value = {}
  }
}

async function loadScheduleInfo(evaluations: EvaluateTeacherModel[]) {
  const classIds = new Set<string>()
  evaluations.forEach((ev) => {
    const cls = ev.class || (ev as any).Class || null
    const sched = ev.classSchedule || (ev as any).classSchedule || (ev as any).ClassSchedule || null
    const classId =
      ev.classId ||
      cls?.id ||
      sched?.classId ||
      (sched as any)?.ClassId ||
      null
    if (classId) classIds.add(classId)
  })
  Object.keys(classInfoMap.value).forEach((id) => classIds.add(id))
  if (!classIds.size) {
    scheduleInfoMap.value = {}
    return
  }

  try {
    const classIdList = Array.from(classIds)
    const results = await Promise.all(
      classIdList.map((id) => scheduleApi.getSchedulesByClassId(id))
    )
    const nextScheduleMap: Record<
      string,
      {
        sessionIndex?: number | null;
        date?: string | null;
        startTime?: string | null;
        endTime?: string | null;
        classId?: string | null;
        scheduleName?: string | null;
      }
    > = {}
    results.forEach((res, idx) => {
      const classId = classIdList[idx]
      const schedules = res?.data || []
      const sortedSchedules = [...schedules].sort((a: any, b: any) => {
        const aIdx = typeof a.sessionIndex === 'number' ? a.sessionIndex : Number.MAX_SAFE_INTEGER
        const bIdx = typeof b.sessionIndex === 'number' ? b.sessionIndex : Number.MAX_SAFE_INTEGER
        if (aIdx !== bIdx) return aIdx - bIdx
        return new Date(a.date || 0).getTime() - new Date(b.date || 0).getTime()
      })
      const scheduleOrderMap: Record<string, number> = {}
      sortedSchedules.forEach((s: any, index: number) => {
        scheduleOrderMap[s.id] = typeof s.sessionIndex === 'number' ? s.sessionIndex : index + 1
      })

      schedules.forEach((s: any) => {
        const derivedIndex = scheduleOrderMap[s.id] ?? deriveSessionIndex(s.courseLesson)
        const scheduleName =
          s.scheduleName ||
          s.sessionName ||
          s.courseLesson?.sessionName ||
          ''
        nextScheduleMap[s.id] = {
          sessionIndex: typeof derivedIndex === 'number' ? derivedIndex : Number(derivedIndex) || null,
          date: s.date || null,
          startTime: s.startTime || null,
          endTime: s.endTime || null,
          classId: s.classId || classId || null,
          scheduleName
        }
      })
    })
    scheduleInfoMap.value = nextScheduleMap
  } catch (error) {
    console.error('Failed to load class schedules', error)
    scheduleInfoMap.value = {}
  }
}

function deriveSessionIndex(courseLesson: any): string | null {
  if (!courseLesson) return null
  if (courseLesson.sessionName) {
    const m = `${courseLesson.sessionName}`.match(/\d+/)
    if (m) return m[0]
  }
  return courseLesson.sessionIndex || null
}
</script>

<style scoped>
.teacher-evaluation {
  padding: 12px 0;
}

.evaluation-filter {
  display: grid;
  grid-template-columns: minmax(260px, 2fr) repeat(2, minmax(180px, 1fr)) auto;
  align-items: center;
  gap: 12px;
  margin-bottom: 12px;
}

.evaluation-filter__control {
  width: 100%;
}

@media (max-width: 992px) {
  .evaluation-filter {
    grid-template-columns: 1fr;
  }
}

.evaluation-accordion {
  border: none;
}

.session-accordion {
  margin-top: 8px;
}

.eval-card__header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.eval-card__title {
  font-weight: 600;
}

.session-block {
  padding: 8px 0;
  border-bottom: 1px dashed #e5e7eb;
}

.session-block:last-child {
  border-bottom: none;
}

.session-block__header {
  display: flex;
  justify-content: flex-start;
  align-items: center;
  gap: 12px;
}

.session-title {
  font-weight: 600;
  color: #111827;
}

.session-meta {
  font-size: 12px;
  color: #6b7280;
  white-space: nowrap;
}

.evaluation-item {
  padding: 8px 10px;
  border: 1px solid #f0f0f0;
  border-radius: 8px;
  margin-bottom: 8px;
  background: #fafafa;
}

.evaluation-item:last-child {
  margin-bottom: 0;
}

.evaluation-item__top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
  margin-bottom: 6px;
}

.stars {
  display: grid;
  grid-template-columns: auto auto;
  align-items: center;
  column-gap: 8px;
}

.rating-stars :deep(.el-rate) {
  display: inline-flex;
  align-items: center;
  line-height: 1;
}

.rating-stars :deep(.el-rate__item) {
  line-height: 1;
  margin-right: 2px;
}

.rating-stars :deep(.el-rate__item:last-child) {
  margin-right: 0;
}

.rating-stars :deep(.el-rate__icon) {
  font-size: 16px;
}

.rating-stars :deep(.el-rate__decimal) {
  overflow: hidden;
}

.pill {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  height: 26px;
  min-width: 26px;
  padding: 0 6px;
  border-radius: 6px;
  background: #f3f4f6;
  border: 1px solid #e5e7eb;
  font-weight: 600;
  text-align: center;
}

.student-time {
  font-size: 12px;
  color: #6b7280;
}

.evaluation-content {
  font-size: 14px;
  line-height: 1.5;
  color: #1f2937;
}

:deep(.el-rate__icon),
:deep(img),
:deep(svg) {
  vertical-align: baseline !important;
}
</style>
