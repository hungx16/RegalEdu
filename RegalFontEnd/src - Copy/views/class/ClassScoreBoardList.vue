<template>
  <div class="scoreboard-wrapper">
    <div class="scoreboard-header">
      <div>
        <h5 class="title">{{ t('classScoreBoard.title') }}</h5>
      </div>
      <el-button v-if="teacherStore.isCurrentUserTeacher" type="primary" size="small" plain :loading="saving"
        @click="openDialog">
        {{ t('classScoreBoard.updateScore') }}
      </el-button>
    </div>

    <el-skeleton v-if="loading" animated :rows="4" />
    <el-empty v-else-if="!rows.length" :description="t('common.noData')" />
    <BaseTable v-else :columns="columns" :items="rows" :showActionsColumn="false" :showCheckboxColumn="false"
      :showPagination="false" height="360px">
      <template #cell-student="{ item }">
        <div class="student-name">
          <strong>{{ item.student }}</strong>
          <span v-if="item.nickname" class="nickname">({{ item.nickname }})</span>
        </div>
      </template>
      <template #cell-midtermAvg="{ item }">
        {{ formatScore(item.midtermAvg) }}
      </template>
      <template #cell-endTermAvg="{ item }">
        {{ formatScore(item.endTermAvg) }}
      </template>
      <template #cell-summaryScore="{ item }">
        {{ formatScore(item.summaryScore) }}
      </template>
      <template #cell-result="{ item }">
        <BaseBadge :type="item.isPass ? 'success' : 'danger'"
          :label="item.isPass ? t('classScoreBoard.pass') : t('classScoreBoard.fail')" size="small" />
      </template>
    </BaseTable>
  </div>
  <ClassScoreBoardDialog v-if="dialogVisible" :visible="dialogVisible" :class-id="props.classId"
    :course-id="props.courseId" @update:visible="dialogVisible = $event" @saved="reload" />
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { useClassScoreBoardStore } from '@/stores/classScoreBoardStore'
import ClassScoreBoardDialog from './ClassScoreBoardDialog.vue'
import { useTeacherStore } from '@/stores/teacherStore'
import { useAssignedClassStore } from '@/stores/assignedClassStore'
const teacherStore = useTeacherStore()
const assignedStore = useAssignedClassStore()

const props = defineProps<{ classId: string; courseId?: string; active: boolean }>()
const emit = defineEmits<{ (e: 'update'): void }>()
const { t } = useI18n()
const store = useClassScoreBoardStore()
const loading = computed(() => store.loading)
const saving = computed(() => store.saving)

const columns: BaseTableColumn[] = [
  { key: 'student', labelKey: 'classScoreBoard.student', minWidth: 200, sticky: true },
  { key: 'midtermAvg', labelKey: 'classScoreBoard.midterm', minWidth: 140, align: 'center' },
  { key: 'endTermAvg', labelKey: 'classScoreBoard.endterm', minWidth: 140, align: 'center' },
  { key: 'summaryScore', labelKey: 'classScoreBoard.summary', minWidth: 140, align: 'center' },
  { key: 'result', labelKey: 'classScoreBoard.result', minWidth: 120, align: 'center' }
]

const rows = computed(() => store.scores.map(s => {
  const display = formatScoreBoardStudent(s)
  return {
    student: display.name,
    nickname: display.nickname,
    midtermAvg: s.midtermAvg,
    endTermAvg: s.endTermAvg,
    summaryScore: s.summaryScore,
    isPass: !!s.isPass
  }
}))

function formatScoreBoardStudent(score: any) {
  const fallbackFields = [
    score.studentName,
    score.studentFullName,
    score.studentCode,
    score.student || '',
    score.studentId || ''
  ]
  const assigned = assignedStore.assignedStudents?.find((stu: any) =>
    stu.id === score.studentId || stu.studentId === score.studentId || stu.studentId === score.studentId
  )
  if (assigned) {
    return {
      name: assigned.fullName || assigned.studentCode || assigned.id || '-',
      nickname: assigned.englishName || ''
    }
  }
  const first = fallbackFields.find(v => v && String(v).trim())
  return {
    name: first ? String(first).trim() : '-',
    nickname: ''
  }
}

const dialogVisible = ref(false)

watch(
  () => [props.classId, props.active],
  async ([id, active]) => {
    if (active && typeof id === 'string') {
      await loadData(id)
    }
  },
  { immediate: true }
)

onMounted(async () => {
  if (props.active && props.classId) {
    await loadData(props.classId)
  }
})

async function loadData(id: string) {
  if (!id) return
  await store.fetchByClassId(id)
  try {
    await assignedStore.fetchAssignedStudents(id)
  } catch (err) {
    console.error('Failed to fetch assigned students for scoreboard list', err)
  }
}

async function reload() {
  if (props.classId) await loadData(props.classId)
}

function openDialog() {
  dialogVisible.value = true
}

function formatScore(val?: number | null) {
  if (val === null || val === undefined) return ''
  const num = Number(val)
  if (Number.isNaN(num)) return ''
  return num.toFixed(2)
}
</script>

<style scoped>
.scoreboard-wrapper {
  margin-top: 8px;
}

.scoreboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
  gap: 8px;
}

.title {
  margin: 0;
  font-weight: 600;
}

.student-name {
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.nickname {
  color: #6b7280;
}
</style>
