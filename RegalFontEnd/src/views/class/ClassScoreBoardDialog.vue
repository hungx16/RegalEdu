<template>
  <BaseDialogForm :visible="visible" :title="t('classScoreBoard.updateScore')" mode="edit" width="1200px"
    :show-delete="false" :loading="saving" :form-data="formState" @update:visible="emit('update:visible', $event)"
    @submit="onSave">
    <template #form>
      <div class="form-section">
        <el-form-item :label="t('classScoreBoard.scoreType')">
          <el-select v-model="formState.scoreType" class="score-type-select">
            <el-option v-for="opt in scoreTypeOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
          </el-select>
        </el-form-item>
      </div>
      <BaseTable :key="tableKey" :columns="columns" :items="rows" :showActionsColumn="false" :showCheckboxColumn="false"
        :showPagination="false" height="360px">
        <template #cell-student="{ item }">
          <div class="student-name">
            <strong>{{ item.student }}</strong>
            <span v-if="item.nickname" class="nickname">({{ item.nickname }})</span>
          </div>
        </template>
        <template v-for="skill in displaySkills" :key="skill.key" #[`cell-${skill.key}`]="{ item }">
          <el-input-number v-model="item.scores[skill.key]" :min="0" :max="10" :step="0.25" controls-position="right"
            size="small" />
        </template>
        <template #cell-average="{ item }">
          <strong>{{ formatScore(item.average) }}</strong>
        </template>
      </BaseTable>

    </template>
  </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, reactive, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import { ScoreType } from '@/api/ClassScoreBoardApi'
import { useClassScoreBoardStore } from '@/stores/classScoreBoardStore'
import { useNotificationStore } from '@/stores/notificationStore'
import { useSkillStore } from '@/stores/skillStore'
import { useAssignedClassStore } from '@/stores/assignedClassStore'
import { useCourseStore } from '@/stores/courseStore'

const props = defineProps<{
  visible: boolean
  classId: string
  courseId?: string
}>()
const emit = defineEmits<{
  (e: 'update:visible', val: boolean): void
  (e: 'saved'): void
}>()

const { t } = useI18n()
const store = useClassScoreBoardStore()
const notificationStore = useNotificationStore()
const skillStore = useSkillStore()
const assignedStore = useAssignedClassStore()
const courseStore = useCourseStore()
const lastLoadedClassId = reactive<{ value: string | null }>({ value: null })
const examSkillIds = reactive<{ mid: string[]; final: string[] }>({ mid: [], final: [] })

const saving = computed(() => store.saving)
const skillColumns = computed<Array<{ key: string; label: string; source: string }>>(() => {
  // lấy trực tiếp danh sách kỹ năng từ skillStore (categoryName)
  const list = skillStore.skills || []
  return list.map((s) => ({
    key: s.id || '',
    label: s.categoryName || s.categoryCode || '',
    source: s.id || s.categoryCode || ''
  }))
})
const detailSkillColumns = computed<Array<{ key: string; label: string; source: string }>>(() => {
  const categories: Record<string, { key: string; label: string; source: string }> = {} as Record<string, { key: string; label: string; source: string }>
  (store.scoreDetails || []).forEach((d) => {
    if (d.categoryId) {
      categories[d.categoryId] = {
        key: d.categoryId,
        label: d.categoryName || d.categoryId,
        source: d.categoryId
      }
    }
  })
  return Object.values(categories)
})
const defaultSkillColumns = computed<Array<{ key: string; label: string; source: string }>>(() => ([
  { key: 'midtermAvg', label: t('classScoreBoard.midterm'), source: 'midtermAvg' },
  { key: 'endTermAvg', label: t('classScoreBoard.endterm'), source: 'endTermAvg' },
  { key: 'summaryScore', label: t('classScoreBoard.summary'), source: 'summaryScore' }
]))
const displaySkills = computed<Array<{ key: string; label: string; source: string }>>(() => {
  // ưu tiên kỹ năng của khóa học theo scoreType
  const selectedExamIds = formState.scoreType === ScoreType.Midterm ? examSkillIds.mid : examSkillIds.final
  if (selectedExamIds.length) {
    const mapped = selectedExamIds
      .map(id => {
        const found = skillColumns.value.find(sk => sk.key === id)
        if (found) return found
        // nếu chưa có trong skill store vẫn tạo cột với key, label là id để user thấy
        return { key: id, label: id, source: id }
      })
    if (mapped.length) return mapped
  }
  if (detailSkillColumns.value.length) return detailSkillColumns.value
  return defaultSkillColumns.value
})

const columns = computed<BaseTableColumn[]>(() => {
  const base: BaseTableColumn[] = [
    { key: 'student', labelKey: 'classScoreBoard.student', minWidth: 200, sticky: true }
  ]
  const skillCols = displaySkills.value.map((s) => ({
    key: String(s.key ?? ''),
    labelKey: s.label || t('common.unknown'),
    isLocale: false,
    minWidth: 140,
    align: 'center' as 'center'
  }))
  return [
    ...base,
    ...skillCols,
    { key: 'average', labelKey: 'classScoreBoard.summary', minWidth: 140, align: 'center' as 'center' }
  ]
})

const formState = reactive({
  scoreType: ScoreType.Midterm
})

const rows = reactive<any[]>([])
const tableKey = computed(() => `${formState.scoreType}-${displaySkills.value.map(s => s.key).join(',')}`)

const scoreTypeOptions = computed(() => [
  { value: ScoreType.Midterm, label: t('classScoreBoard.midterm') },
  { value: ScoreType.EndTerm, label: t('classScoreBoard.endterm') }
])

watch(
  () => props.visible,
  (v) => {
    if (v) {
      loadData()
    }
  },
  { immediate: true }
)

watch(
  () => [props.classId, store.scores],
  () => {
    if (props.visible) hydrateRows()
  },
  { deep: true }
)

watch(
  () => formState.scoreType,
  () => {
    if (props.visible) hydrateRows()
  }
)

async function ensureSkills() {
  if (skillStore.skills.length) return
  try {
    await skillStore.fetchAllSkills()
  } catch (err) {
    console.error('Failed to load skills/category for scores', err)
  }
}

async function loadData() {
  await ensureSkills()
  await ensureCourseSkillIds()
  await ensureScoreSummary()
  await ensureScores()
  await ensureStudents()
  hydrateRows()
}

async function ensureScoreSummary() {
  if (!props.classId) return
  try {
    await store.fetchByClassId(props.classId)
  } catch (err) {
    console.error('Failed to load score summary', err)
  }
}

async function ensureCourseSkillIds() {
  examSkillIds.mid = []
  examSkillIds.final = []
  if (!props.courseId) return
  try {
    const found = courseStore.courses.find(c => c.id === props.courseId)
    let course = found
    if (!course) {
      course = await courseStore.getCourseById(props.courseId)
    }
    if (course) {
      const parseIds = (val?: string[] | string | null) => {
        if (!val) return []
        if (Array.isArray(val)) return val.filter(Boolean).map(String)
        return val.split(/[#\$#,]+/).map(v => v.trim()).filter(Boolean)
      }
      examSkillIds.mid = parseIds((course as any).midExamIds)
      examSkillIds.final = parseIds((course as any).finalExamIds)
    }
  } catch (err) {
    console.error('Failed to load course exam skill ids', err)
  }
}
async function ensureScores() {
  if (!props.classId) return
  try {
    await store.fetchDetailsByClassId(props.classId)
    lastLoadedClassId.value = props.classId
  } catch (err) {
    console.error('Failed to load score board data', err)
  }
}

async function ensureStudents() {
  if (!props.classId) return
  try {
    await assignedStore.fetchAssignedStudents(props.classId)
  } catch (err) {
    console.error('Failed to load assigned students', err)
  }
}

function hydrateRows() {
  rows.splice(0, rows.length)
  const studentsFromAssigned = assignedStore.assignedStudents || []
  const studentsFromScores = store.scoreDetails
    .filter(s => s.studentId)
    .map(s => ({
      id: s.studentId,
      fullName: s.studentName || '',
      englishName: ''
    }))
  const merged = new Map<string, any>()
  studentsFromScores.forEach(stu => merged.set(stu.id, stu))
  studentsFromAssigned.forEach(stu => merged.set(stu.id ?? '', stu))

  Array.from(merged.values()).forEach((stu) => {
    const scoreEntry = store.scoreDetails.find((s) => s.studentId === (stu.id || stu.id))
    const detailEntries = store.scoreDetails.filter((d) =>
      d.studentId === (stu.id || stu.id) &&
      (!d.scoreType || d.scoreType === formState.scoreType)
    )
    const scoreMap: Record<string, number | null> = {}
    displaySkills.value.forEach((skill) => {
      const key = skill.source || skill.key
      let value: number | null = null
      if (detailEntries.length && key) {
        const found = detailEntries.find(d => d.categoryId === key)
        value = found?.score ?? null
      }
      // fallback cho cột mặc định
      if (value === null && scoreEntry) {
        if (skill.key === 'midtermAvg') value = (scoreEntry as any).midtermAvg ?? null
        else if (skill.key === 'endTermAvg') value = (scoreEntry as any).endTermAvg ?? null
        else if (skill.key === 'summaryScore') value = (scoreEntry as any).summaryScore ?? null
      }
      scoreMap[String(skill.key ?? '')] = value
    })
    const display = formatDialogStudent(stu, scoreEntry?.studentId)
    rows.push({
      studentId: stu.id || '',
      student: display.name,
      nickname: display.nickname,
      scores: scoreMap,
      get average() {
        const nums = Object.values(this.scores || {}).map((n: any) => Number(n)).filter((n: number) => !Number.isNaN(n))
        if (!nums.length) return null
        return nums.reduce((a: number, b: number) => a + b, 0) / nums.length
      }
    })
  })
}


async function onSave() {
  if (!props.classId) return
  try {
    const payload = {
      classId: props.classId,
      scoreType: formState.scoreType,
      scores: rows.flatMap((r) =>
        displaySkills.value.map((skill) => ({
          studentId: r.studentId,
          categoryId: skill.key ? String(skill.key) : undefined,
          score: (skill.key !== null && skill.key !== undefined) ? r.scores?.[skill.key] ?? null : null
        }))
      )
    }
    await store.updateScoreBoard(payload)
    notificationStore.showToast('success', { key: 'classScoreBoard.updateSuccess' })
    emit('saved')
    emit('update:visible', false)
  } catch (err) {
    console.error('Failed to update score board', err)
    notificationStore.showToast('error', { key: 'classScoreBoard.updateFailed' })
  }
}

function formatScore(val?: number | null) {
  if (val === null || val === undefined) return ''
  const num = Number(val)
  if (Number.isNaN(num)) return ''
  return num.toFixed(2)
}

function formatDialogStudent(student: any, studentId?: string) {
  const summary = store.scores.find(s => s.studentId === studentId)
  const assigned = assignedStore.assignedStudents?.find((stu: any) => {
    const stuId = stu.studentId || stu.id || (stu.student || {}).id
    const targetId = studentId || student?.studentId || student?.id
    return stuId && targetId && String(stuId) === String(targetId)
  })
  if (assigned) {
    return {
      name: assigned.fullName || assigned.studentCode || assigned.id || '-',
      nickname: assigned.englishName || ''
    }
  }
  const fallback = student || {}
  const fallbackFields = [
    summary?.studentName,
    fallback.fullName,
    fallback.studentName,
    fallback.studentCode,
    fallback.student,
    (fallback.student || {}).fullName,
    studentId,
    fallback.id
  ]
  const first = fallbackFields.find(v => v && String(v).trim())
  return {
    name: first ? String(first).trim() : '-',
    nickname: summary?.nickname || ''
  }
}
</script>

<style scoped>
.form-section {
  margin-bottom: 12px;
}

.score-type-select {
  width: 260px;
}

.student-cell .name {
  font-weight: 600;
}

.student-cell .nickname {
  color: #6b7280;
}

.student-name {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.nickname {
  color: #6b7280;
}

.footer {
  display: flex;
  justify-content: flex-end;
  margin-top: 14px;
}
</style>
