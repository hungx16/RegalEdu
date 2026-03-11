<template>
  <div class="remarks-wrapper">
    <h5 class="title">{{ t('classSchedule.remarksTitle') }}</h5>
    <p class="subtitle">{{ t('classSchedule.remarksDesc') }}</p>

    <el-skeleton v-if="loading" animated :rows="4" />
    <el-empty v-else-if="!students.length" :description="t('common.noData')" />
    <div v-else class="students-list">
      <el-collapse v-model="expanded" class="student-accordion">
        <el-collapse-item v-for="student in students" :key="student.studentId" :name="student.studentId">
          <template #title>
            <div class="student-header">
              <div class="student-name">
                <strong>{{ student.fullName || '-' }}</strong>
                <span v-if="student.nickname" class="nickname">({{ student.nickname }})</span>
              </div>
              <div class="student-rating" v-if="student.avgRating !== null">
                <el-rate v-model="student.avgRating" disabled :max="5" :void-color="'#fff'" />
                <span class="pill">{{ formatRating(student.avgRating) }}</span>
              </div>

            </div>
          </template>

          <div class="student-body">
            <div class="remarks-group" v-for="(remark, index) in student.remarks"
              :key="remark.id || remark.date || index">
              <div class="remark-row">
                <div class="remark-left">
                  <div class="session-info">
                    <span class="session-order"
                      v-if="remark.sessionIndex !== null && remark.sessionIndex !== undefined">
                      {{ t('classSchedule.sessionOrder') }} {{ remark.sessionIndex }}
                    </span>
                    <span class="session-day" v-if="remark.date">
                      {{ formatWeekday(remark.date) }}
                    </span>
                    <span class="date" v-if="remark.date">{{ formatDate(remark.date) }}</span>
                  </div>
                  <div class="remark-comment-wrapper">
                    <div v-if="remark.comment" class="remark-text">{{ remark.comment }}</div>
                    <div class="homework-row" v-if="showHomeworkStatus(remark)">
                      <div class="homework-status" :class="homeworkStatusClass(remark)">
                        <i :class="homeworkStatusIcon(remark)" />
                        <span>{{ homeworkStatusLabel(remark) }}</span>
                      </div>
                      <div class="homework-score"
                        v-if="remark.homeworkScore !== null && remark.homeworkScore !== undefined">
                        <span class="score-label">{{ t('classSchedule.homeworkScore') }}</span>
                        <span class="score-value">{{ formatRating(remark.homeworkScore) }}</span>
                      </div>
                    </div>
                    <div class="remark-by">{{ t('classSchedule.remarkBy') }}: {{ remark.attender || '-' }}</div>
                  </div>

                </div>
                <div class="remark-right">
                  <div class="stars" v-if="remark.star !== null">
                    <span class="pill">{{ formatRating(remark.star) }}</span>
                    <el-rate class="rating-stars" :model-value="remark.star" disabled :allow-half="true" :max="5"
                      :colors="['#f7ba2a', '#f7ba2a', '#f7ba2a']" :void-color="'#e5e7eb'" :show-score="false" />
                  </div>
                  <div v-if="hasCriteria(remark)" class="criteria-list">
                    <div class="criteria-item" v-if="remark.participationLevel !== null">
                      <span class="criteria-label">{{ t('classSchedule.participationLevel') }}</span>
                      <el-rate class="rating-stars" :model-value="remark.participationLevel" disabled :allow-half="true"
                        :max="5" :colors="['#f7ba2a', '#f7ba2a', '#f7ba2a']" :void-color="'#e5e7eb'"
                        :show-score="false" />
                    </div>
                    <div class="criteria-item" v-if="remark.learningAbsorptionLevel !== null">
                      <span class="criteria-label">{{ t('classSchedule.learningAbsorptionLevel') }}</span>
                      <el-rate class="rating-stars" :model-value="remark.learningAbsorptionLevel" disabled
                        :allow-half="true" :max="5" :colors="['#f7ba2a', '#f7ba2a', '#f7ba2a']" :void-color="'#e5e7eb'"
                        :show-score="false" />
                    </div>
                    <div class="criteria-item" v-if="remark.disciplineLevel !== null">
                      <span class="criteria-label">{{ t('classSchedule.disciplineLevel') }}</span>
                      <el-rate class="rating-stars" :model-value="remark.disciplineLevel" disabled :allow-half="true"
                        :max="5" :colors="['#f7ba2a', '#f7ba2a', '#f7ba2a']" :void-color="'#e5e7eb'"
                        :show-score="false" />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </el-collapse-item>
      </el-collapse>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ClassScheduleApi } from '@/api/ClassScheduleApi'
import { useClassAttendantStore } from '@/stores/classAttendantStore'
import { storeToRefs } from 'pinia'
import { StudentHomeworkStatus, StudentParticipationStatus } from '@/types'
import { formatDate as formatDateUtil } from '@/utils/format'
import { getDayOfWeekKey } from '@/types/daysOfWeek'

const props = defineProps<{ classId: string; active: boolean }>()
const { t } = useI18n()
const scheduleApi = new ClassScheduleApi()
const attendantStore = useClassAttendantStore()
const { attendantsMap } = storeToRefs(attendantStore)
const loading = ref(false)
const expanded = ref<string[]>([])

interface RemarkItem {
  id?: string
  comment: string
  star: number | null
  participationLevel: number | null
  learningAbsorptionLevel: number | null
  disciplineLevel: number | null
  studentHomeworkStatus?: StudentHomeworkStatus | null
  homeworkScore: number | null
  date: string | null
  sessionIndex?: number | string | null
  attender?: string | null
}
interface StudentRemarks {
  studentId: string
  fullName: string
  nickname?: string | null
  avgRating: number | null
  remarks: RemarkItem[]
}

const students = computed<StudentRemarks[]>(() => {
  const map: Record<string, StudentRemarks> = {}
  Object.values(attendantsMap.value).forEach((list) => {
    list.forEach((att) => {
      if (att.studentParticipationStatus !== StudentParticipationStatus.Present) return
      if (!att.studentId) return
      const commentText = (att.comment ?? '').toString().trim()
      const participationLevel = readLevel(att, 'participationLevel', 'ParticipationLevel')
      const learningAbsorptionLevel = readLevel(att, 'learningAbsorptionLevel', 'LearningAbsorptionLevel')
      const disciplineLevel = readLevel(att, 'disciplineLevel', 'DisciplineLevel')
      const hasRemark =
        commentText.length > 0 ||
        (att.star !== null && att.star !== undefined) ||
        participationLevel !== null ||
        learningAbsorptionLevel !== null ||
        disciplineLevel !== null ||
        (att.homeworkScore !== null && att.homeworkScore !== undefined)
      if (!hasRemark) return
      if (!map[att.studentId]) {
        map[att.studentId] = {
          studentId: att.studentId,
          fullName: att.student?.fullName || '',
          nickname: att.student?.englishName || null,
          avgRating: null,
          remarks: []
        }
      }
      map[att.studentId].remarks.push({
        id: att.id,
        comment: att.comment || '',
        star: att.star ?? null,
        participationLevel,
        learningAbsorptionLevel,
        disciplineLevel,
        studentHomeworkStatus: att.studentHomeworkStatus ?? null,
        homeworkScore: att.homeworkScore ?? null,
        date: (att as any).sessionDate || att.updatedAt || att.createdAt || null,
        sessionIndex: (att as any).sessionIndex || null,
        attender: att.createdBy || ''
      })
    })
  })
  Object.values(map).forEach((s) => {
    s.remarks.sort((a, b) => {
      const aTime = a.date ? new Date(a.date).getTime() : 0
      const bTime = b.date ? new Date(b.date).getTime() : 0
      if (aTime !== bTime) return aTime - bTime
      const aIndex = typeof a.sessionIndex === 'number' ? a.sessionIndex : Number(a.sessionIndex)
      const bIndex = typeof b.sessionIndex === 'number' ? b.sessionIndex : Number(b.sessionIndex)
      if (!Number.isNaN(aIndex) && !Number.isNaN(bIndex) && aIndex !== bIndex) return aIndex - bIndex
      return 0
    })
    const stars = s.remarks.map(r => r.star).filter((n): n is number => n !== null && n !== undefined)
    s.avgRating = stars.length ? (stars.reduce((a, b) => a + b, 0) / stars.length) : null
  })
  return Object.values(map).sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''))
})

watch(
  () => props.classId,
  async (id) => {
    if (props.active && id) {
      await loadData(id)
      expanded.value = students.value.map((s) => s.studentId)
    }
  },
  { immediate: true }
)

async function loadData(classId: string) {
  loading.value = true
  try {
    const res = await scheduleApi.getSchedulesByClassId(classId)
    const schedules = (res.data || []).filter((s: any) => s.classId === classId)
    await Promise.all(
      schedules.map((s: any) => attendantStore.fetchByScheduleId(s.id as string))
    )
    const sortedSchedules = [...schedules].sort((a: any, b: any) => {
      const aIdx = typeof a.sessionIndex === 'number' ? a.sessionIndex : Number.MAX_SAFE_INTEGER
      const bIdx = typeof b.sessionIndex === 'number' ? b.sessionIndex : Number.MAX_SAFE_INTEGER
      if (aIdx !== bIdx) return aIdx - bIdx
      return new Date(a.date).getTime() - new Date(b.date).getTime()
    })
    const scheduleOrderMap: Record<string, number> = {}
    sortedSchedules.forEach((s: any, index) => {
      scheduleOrderMap[s.id] = typeof s.sessionIndex === 'number' ? s.sessionIndex : index + 1
    })
    // attach session date & index to attendants
    schedules.forEach((s: any) => {
      const arr = attendantsMap.value[s.id] || []
      const sessionOrder = scheduleOrderMap[s.id] ?? deriveSessionIndex(s.courseLesson)
      arr.forEach((att) => {
        (att as any).sessionDate = s.date
          ; (att as any).sessionIndex = sessionOrder
      })
    })
  } catch (err) {
    console.error('Failed to load remarks by class', err)
  } finally {
    loading.value = false
  }
}

function deriveSessionIndex(courseLesson: any): string | null {
  if (!courseLesson) return null
  if (courseLesson.sessionName) {
    const m = `${courseLesson.sessionName}`.match(/\\d+/)
    if (m) return m[0]
  }
  return courseLesson.sessionIndex || null
}

function formatDate(val?: string | null) {
  if (!val) return ''
  return formatDateUtil(val, 'DD/MM/YYYY') || ''
}

function formatWeekday(val?: string | null) {
  if (!val) return ''
  const date = new Date(val)
  if (Number.isNaN(date.getTime())) return ''
  const jsDay = date.getDay()
  const normalizedDay = jsDay === 0 ? 7 : jsDay
  const key = getDayOfWeekKey(normalizedDay)
  return key ? t(key) : ''
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

function hasCriteria(remark: RemarkItem) {
  return (
    remark.participationLevel !== null ||
    remark.learningAbsorptionLevel !== null ||
    remark.disciplineLevel !== null
  )
}

function readLevel(source: any, camelKey: string, pascalKey: string) {
  const raw = source?.[camelKey] ?? source?.[pascalKey] ?? null
  if (raw === null || raw === undefined) return null
  const num = Number(raw)
  return Number.isNaN(num) ? null : num
}

function showHomeworkStatus(remark: RemarkItem) {
  return (
    remark.studentHomeworkStatus !== null &&
    remark.studentHomeworkStatus !== undefined
  ) || (remark.homeworkScore !== null && remark.homeworkScore !== undefined)
}

function homeworkStatusLabel(remark: RemarkItem) {
  if (remark.homeworkScore !== null && remark.homeworkScore !== undefined) {
    return t('classSchedule.homeworkDone')
  }
  if (remark.studentHomeworkStatus === StudentHomeworkStatus.Done) {
    return t('classSchedule.homeworkDone')
  }
  if (remark.studentHomeworkStatus === StudentHomeworkStatus.NotDone) {
    return t('classSchedule.homeworkNotDone')
  }
  return t('classSchedule.homeworkNotDone')
}

function isHomeworkDone(remark: RemarkItem) {
  if (remark.homeworkScore !== null && remark.homeworkScore !== undefined) return true
  if (remark.studentHomeworkStatus === StudentHomeworkStatus.Done) return true
  if (remark.studentHomeworkStatus === StudentHomeworkStatus.NotDone) return false
  return false
}

function homeworkStatusClass(remark: RemarkItem) {
  return isHomeworkDone(remark) ? 'done' : 'not-done'
}

function homeworkStatusIcon(remark: RemarkItem) {
  return isHomeworkDone(remark) ? 'bi bi-check-square-fill' : 'bi bi-x-square'
}
</script>

<style scoped>
.remarks-wrapper {
  padding-top: 8px;
}

.title {
  margin: 0 0 4px;
  font-weight: 600;
}

.subtitle {
  margin: 0 0 12px;
  color: #6b7280;
}

.students-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.student-accordion :deep(.el-collapse-item__header) {
  padding: 6px 10px;
  background: #f6f7fb;
  border: 1px solid #e2e8f0;
  font-weight: 600;
  margin-top: 10px;
  border-top-left-radius: 12px;
  border-top-right-radius: 12px;
}

.student-accordion :deep(.el-collapse-item__wrap) {
  border: 1px solid #e2e8f0;
  border-top: none;
  border-radius: 0 0 12px 12px;
}

.student-body {
  padding: 12px 14px 14px;
  background: #fff;
}

.student-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 8px;
}

.student-name {
  font-size: 14px;
}

.nickname {
  color: #6b7280;
  margin-left: 6px;
}

.student-rating {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 2px 4px;
}

.remarks-group {
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  padding: 12px;
  margin-bottom: 10px;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.03);
  background: #fafbff;
}

.remarks-group:last-child {
  margin-bottom: 0;
}

.homework-row {
  display: inline-flex;
  align-items: center;
  gap: 12px;
}

.homework-status {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  font-weight: 600;
  line-height: 1;
}

.homework-status i {
  font-size: 14px;
}

.homework-status.done {
  color: #3b82f6;
}

.homework-status.not-done {
  color: #9ca3af;
}

.homework-score {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #6b7280;
}

.homework-score .score-label {
  line-height: 1;
}

.homework-score .score-value {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 56px;
  height: 30px;
  padding: 0 12px;
  border-radius: 6px;
  border: 1px solid #d1d5db;
  background: #fff;
  color: #111827;
  font-weight: 600;
}

.remark-row {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 12px;
}

.session-info {
  display: flex;
  flex-direction: row;
  align-items: baseline;
  gap: 8px;
  flex-wrap: wrap;
  color: #4b5563;
}

.remark-left {
  flex: 1;
  min-width: 0;
}

.remark-comment-wrapper {
  margin-top: 6px;
}

.remark-right {
  display: flex;
  flex-direction: column;
  align-items: stretch;
  gap: 6px;
  --criteria-label-width: 160px;
  min-width: 280px;
}

.stars {
  display: grid;
  grid-template-columns: var(--criteria-label-width) auto;
  align-items: center;
  column-gap: 8px;
}

.stars .pill {
  justify-self: end;
}

.criteria-list {
  display: flex;
  flex-direction: column;
  gap: 6px;
  align-items: stretch;
}

.criteria-item {
  display: grid;
  grid-template-columns: var(--criteria-label-width) auto;
  align-items: center;
  column-gap: 8px;
}

.criteria-label {
  color: #6b7280;
  font-size: 12px;
  text-align: left;
  white-space: nowrap;
}

.student-rating :deep(.el-rate__icon) {
  font-size: 16px;
  transform: translateY(0);
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

.session-day {
  font-weight: 600;
  color: #4b5563;
}

.session-order {
  font-weight: 600;
  color: #111827;
  font-size: 13px;
}

.date {
  color: #6b7280;
  min-width: 90px;
  text-align: right;
}

.remark-text {
  margin-bottom: 6px;
  white-space: pre-line;
}

.remark-by {
  color: #6b7280;
  font-size: 13px;
}

:deep(.el-rate__icon),
:deep(img),
:deep(svg) {
  vertical-align: baseline !important;
}
</style>
