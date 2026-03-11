<template>
    <BaseDialogForm :visible="visible" :title="dialogTitleText" :description="dialogSubtitleText" :mode="'view'"
        :show-delete="false" :loading="loading" :submit-disabled="true" width="90%"
        @update:visible="emit('update:visible', $event)">
        <template #form>
            <div v-if="loading" class="loading-wrap" v-loading="loading" />
            <template v-else>
                <TabbedComponent v-model="activeTab" :tabs="tabs" />
                <div v-if="activeTab === 'info'">
                    <el-card shadow="never" class="info-card">
                        <div class="section-title">{{ t('classSchedule.sessionInfo') }}</div>
                        <el-row :gutter="16" class="mt-2">
                            <el-col :xs="24" :md="12" :lg="8">
                                <div class="field"><span class="label">{{ t('classSchedule.lessonType') }}</span><span
                                        class="value">{{ lessonType }}</span></div>
                                <div class="field"><span class="label">{{ t('classSchedule.lessonName') }}</span><span
                                        class="value">{{ lessonName }}</span></div>
                                <div class="field"><span class="label">{{ t('classSchedule.lessonContent')
                                }}</span><span class="value">{{ lessonContent }}</span></div>
                                <div class="field"><span class="label">{{ t('classSchedule.plan') }}</span><span
                                        class="value">{{ schedule?.plan || '-'
                                        }}</span>
                                </div>

                                <div class="field"><span class="label">{{ t('classSchedule.teacherName') }}</span><span
                                        class="value">{{ teacherName
                                        }}</span></div>
                            </el-col>
                            <el-col :xs="24" :md="12" :lg="8">
                                <div class="field"><span class="label">{{ t('classSchedule.objective') }}</span><span
                                        class="value">{{ lessonObjective }}</span></div>
                                <div class="field"><span class="label">{{ t('classSchedule.reference') }}</span><span
                                        class="value">
                                        <a v-if="referenceLink" :href="referenceLink" target="_blank" rel="noreferrer"
                                            class="link-text">{{ referenceLink }}</a>
                                        <span v-else>{{ referenceText }}</span>
                                    </span></div>
                                <div class="field"><span class="label">{{ t('classSchedule.scheduleStatus')
                                }}</span><span class="value badge-holder">
                                        <BaseBadge v-if="schedule" :label="scheduleLabel(schedule.classScheduleStatus)"
                                            :type="scheduleColor(schedule.classScheduleStatus)" size="small" />
                                        <span v-else>-</span>
                                    </span></div>
                                <div class="field"><span class="label">{{ t('classSchedule.attendanceStatus')
                                }}</span><span class="value badge-holder">
                                        <BaseBadge v-if="schedule"
                                            :label="attendanceLabel(schedule.sessionAttendanceStatus)"
                                            :type="attendanceColor(schedule.sessionAttendanceStatus)" size="small" />
                                        <span v-else>-</span>
                                    </span></div>
                            </el-col>
                            <el-col :span="24">
                                <div class="field homework-field">
                                    <span class="label">{{ t('classSchedule.homeworkMain') }}</span>
                                    <span class="value multiline full-width">{{ homeworkMain }}</span>
                                </div>
                                <div class="field homework-field">
                                    <span class="label">{{ t('classSchedule.homeworkExtra') }}</span>
                                    <span class="value multiline full-width">
                                        {{ homeworkExtra || '-' }}
                                    </span>
                                </div>
                                <div class="field homework-field">
                                    <span class="label">{{ t('classSchedule.homeworkAttachments') }}</span>
                                    <i class="bi bi-paperclip me-1" v-if="homeworkExtra && homeworkExtra !== '-'" />
                                    <span class="value full-width">
                                        <template v-if="schedule?.attachment?.path">
                                            <a href="javascript:void(0)" class="link-text"
                                                @click="downloadAttachment(schedule.attachment)">
                                                {{ schedule.attachment.fileName || t('common.download') }}
                                            </a>
                                        </template>
                                        <span v-else>-</span>
                                    </span>
                                </div>
                                <div class="field homework-field">
                                    <span class="label">{{ t('courseLesson.columns.homeworkAttachments') }}</span>
                                    <span class="value full-width">
                                        <template v-if="courseLessonHomeworkAttachments.length">
                                            <span v-for="(att, idx) in courseLessonHomeworkAttachments"
                                                :key="att.path || idx">
                                                <a href="javascript:void(0)" class="link-text"
                                                    @click="downloadAttachment(att)">
                                                    {{ att.fileName || t('common.download') }}
                                                </a>
                                                <span v-if="idx < courseLessonHomeworkAttachments.length - 1">, </span>
                                            </span>
                                        </template>
                                        <span v-else>-</span>
                                    </span>
                                </div>
                                <div class="field homework-field">
                                    <span class="label">{{ t('courseLesson.columns.referenceAttachments') }}</span>
                                    <span class="value full-width">
                                        <template v-if="courseLessonReferenceAttachments.length">
                                            <span v-for="(att, idx) in courseLessonReferenceAttachments"
                                                :key="att.path || idx">
                                                <a href="javascript:void(0)" class="link-text"
                                                    @click="downloadAttachment(att)">
                                                    {{ att.fileName || t('common.download') }}
                                                </a>
                                                <span v-if="idx < courseLessonReferenceAttachments.length - 1">, </span>
                                            </span>
                                        </template>
                                        <span v-else>-</span>
                                    </span>
                                </div>
                                <!-- <div class="field"><span class="label">{{ t('classSchedule.syllabus') }}</span><span
                                        class="value"><i class="bi bi-paperclip me-1" />{{ syllabusText }}</span></div> -->
                            </el-col>
                        </el-row>
                    </el-card>
                </div>
                <div v-else-if="activeTab === 'attendance'" class="attendance-tab">
                    <div class="attendance-header">
                        <div class="attendance-title">
                            <h5 class="fw-semibold mb-1">{{ t('classSchedule.attendanceTitle') }}</h5>
                            <div class="text-muted">{{ attendanceDescText }}</div>
                        </div>
                        <div class="attendance-actions">
                            <!-- <el-button type="danger" size="small" :loading="unlockingAttendance"
                                v-if="!teacherStore.isCurrentUserTeacher" :disabled="!props.scheduleId"
                                @click="unlockAttendance">
                                {{ t('classSchedule.unlockAttendance') }}
                            </el-button> -->
                            <el-button type="success" size="small" :loading="confirmingAttendance"
                                v-if="!teacherStore.isCurrentUserTeacher && !employeeStore.isAcademicAffairsEmployee"
                                @click="confirmAttendance">
                                {{ t('classSchedule.confirmAttendance') }}
                            </el-button>
                            <el-button type="primary" size="small" :loading="savingAttendance"
                                v-if="teacherStore.isCurrentUserTeacher"
                                :disabled="!canSaveAttendance || isSessionLocked" @click="saveAttendance">
                                {{ t('classSchedule.saveAttendance') }}
                            </el-button>
                        </div>
                    </div>
                    <el-card shadow="never">
                        <BaseTable :columns="attendanceColumns" :items="attendants" :loading="attendanceLoading"
                            :showActionsColumn="false" :showCheckboxColumn="false" :showIndex="false" height="400px"
                            :showPagination="false">
                            <template #cell-studentCode="{ item }">
                                {{ item.student?.studentCode || '-' }}
                            </template>
                            <template #cell-fullName="{ item }">
                                {{ item.student?.fullName || '-' }}
                            </template>
                            <template #cell-nickname="{ item }">
                                <span class="link-text" v-if="item.student?.englishName">{{ item.student.englishName
                                    }}</span>
                                <span v-else>-</span>
                            </template>
                            <template #cell-participationStatus="{ item }">
                                <el-radio-group size="small" :model-value="item.studentParticipationStatus"
                                    @change="val => updateParticipation(item, val)" class="inline-radio-group">
                                    <el-radio-button v-for="opt in participationOptions" :key="opt.value"
                                        :label="opt.value">
                                        {{ opt.label }}
                                    </el-radio-button>
                                </el-radio-group>
                            </template>
                            <template #cell-homeworkStatus="{ item }">
                                {{ homeworkStatusLabel(item.studentHomeworkStatus) }}
                            </template>
                            <template #cell-attachment="{ item }">
                                <template v-if="item.attachment?.path">
                                    <a href="javascript:void(0)" class="link-text"
                                        @click="downloadAttachment(item.attachment)">
                                        {{ item.attachment.fileName || t('common.download') }}
                                    </a>
                                </template>
                                <span v-else>-</span>
                            </template>
                            <template #cell-comment="{ item }">
                                {{ item.comment || '-' }}
                            </template>
                        </BaseTable>
                    </el-card>
                </div>
                <div v-else-if="activeTab === 'notes'" class="remarks-tab">
                    <div class="remarks-header">
                        <div>
                            <h5 class="fw-semibold mb-1">{{ t('classSchedule.remarksTitle') }}</h5>
                            <div class="text-muted">{{ t('classSchedule.remarksDesc') }}</div>
                        </div>
                        <el-button v-if="teacherStore.isCurrentUserTeacher" type="primary" size="small"
                            :loading="savingRemarks" :disabled="!canSaveRemarks || isSessionLocked"
                            @click="saveRemarks">
                            {{ t('classSchedule.saveRemarks') }}
                        </el-button>
                    </div>
                    <el-skeleton v-if="attendanceLoading" animated :rows="3" />
                    <div v-else class="remarks-list">
                        <el-empty v-if="!remarks.length" :description="t('common.noData')" />
                        <el-card v-for="item in remarks" :key="item.studentId" shadow="never" class="remark-card">
                            <div class="remark-top">
                                <div class="remark-name">
                                    <strong>{{ item.student?.fullName || '-' }}</strong>
                                    <span v-if="item.student?.englishName"> - {{ item.student.englishName }}</span>
                                </div>
                                <div class="remark-date" v-if="hasRemarkData(item)">
                                    <div class="remark-rate remark-rate-align">
                                        <span class="rating-number pill" v-if="averageRating(getDraft(item)) !== null">
                                            {{ formatRating(averageRating(getDraft(item))) }}
                                        </span>
                                        <el-rate class="rating-stars" :model-value="averageRating(getDraft(item))"
                                            :allow-half="true" :max="5" :colors="['#f7ba2a', '#f7ba2a', '#f7ba2a']"
                                            :void-color="'#e5e7eb'" :show-score="false" :disabled="true" />

                                    </div>
                                    <!-- <span class="muted-text" v-if="getRemarkDate(item)">{{
                                        formatDateCell(getRemarkDate(item)) }}</span> -->
                                </div>
                            </div>
                            <div class="remark-body">
                                <div class="remark-content">
                                    <div class="remark-left">
                                        <el-input v-model="getDraft(item).comment" type="textarea" :rows="3"
                                            :placeholder="t('classSchedule.remarkPlaceholder')" />
                                        <div class="homework-row">
                                            <el-checkbox v-model="getDraft(item).didHomework"
                                                :disabled="item.studentParticipationStatus === StudentParticipationStatus.Absent"
                                                @change="val => onHomeworkToggle(getDraft(item), Boolean(val))">
                                                {{ t('classSchedule.homeworkDoneToggle') }}
                                            </el-checkbox>
                                            <div class="remark-score-editor">
                                                <span class="label">{{ t('classSchedule.homeworkScore') }}</span>
                                                <el-input-number v-model="getDraft(item).homeworkScore" size="small"
                                                    :min="0" :max="10" :step="0.5" :precision="1" :controls="false"
                                                    :disabled="item.studentParticipationStatus === StudentParticipationStatus.Absent || !getDraft(item).didHomework" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="remark-right">
                                        <div class="remark-rating-grid">
                                            <div class="remark-rating-item">
                                                <span class="label">{{ t('classSchedule.participationLevel') }}</span>
                                                <el-rate class="rating-stars"
                                                    v-model="getDraft(item).participationLevel" :allow-half="true"
                                                    :max="5" :colors="['#f7ba2a', '#f7ba2a', '#f7ba2a']"
                                                    :void-color="'#e5e7eb'" :show-score="false"
                                                    :disabled="item.studentParticipationStatus === StudentParticipationStatus.Absent" />
                                            </div>
                                            <div class="remark-rating-item">
                                                <span class="label">{{ t('classSchedule.learningAbsorptionLevel')
                                                }}</span>
                                                <el-rate class="rating-stars"
                                                    v-model="getDraft(item).learningAbsorptionLevel" :allow-half="true"
                                                    :max="5" :colors="['#f7ba2a', '#f7ba2a', '#f7ba2a']"
                                                    :void-color="'#e5e7eb'" :show-score="false"
                                                    :disabled="item.studentParticipationStatus === StudentParticipationStatus.Absent" />
                                            </div>
                                            <div class="remark-rating-item">
                                                <span class="label">{{ t('classSchedule.disciplineLevel') }}</span>
                                                <el-rate class="rating-stars" v-model="getDraft(item).disciplineLevel"
                                                    :allow-half="true" :max="5"
                                                    :colors="['#f7ba2a', '#f7ba2a', '#f7ba2a']" :void-color="'#e5e7eb'"
                                                    :show-score="false"
                                                    :disabled="item.studentParticipationStatus === StudentParticipationStatus.Absent" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </el-card>
                    </div>
                </div>
            </template>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import TabbedComponent from '@/components/tabbed/TabbedComponent.vue'
import type { ClassScheduleModel } from '@/api/ClassScheduleApi'
import { ClassScheduleStatus, SessionAttendanceStatus, SessionAttendanceLockStatus } from '@/types'
import { formatDate as formatDateUtil } from '@/utils/format'
import { getDayOfWeekKey } from '@/types/daysOfWeek'
import { useClassScheduleStore } from '@/stores/classScheduleStore'
import { useClassAttendantStore } from '@/stores/classAttendantStore'
import { storeToRefs } from 'pinia'
import { useFileStore } from '@/stores/fileStore'
import type { ClassAttendantModel } from '@/api/ClassAttendantApi'
import { StudentHomeworkStatus, StudentParticipationStatus } from '@/types'
import { useNotificationStore } from '@/stores/notificationStore'
import { ClassScheduleApi } from '@/api/ClassScheduleApi'
import { useTeacherStore } from '@/stores/teacherStore'
import { useEmployeeStore } from '@/stores/employeeStore'
const teacherStore = useTeacherStore()
const employeeStore = useEmployeeStore()
const props = defineProps<{ visible: boolean; scheduleId: string | null }>()
const emit = defineEmits(['update:visible'])
const { t } = useI18n()
const classScheduleStore = useClassScheduleStore()
const classAttendantStore = useClassAttendantStore()
const { selectedSchedule, detailLoading } = storeToRefs(classScheduleStore)
const { attendantsMap, loading: attendantsLoadingState, saving: attendantsSavingState } = storeToRefs(classAttendantStore)
const loading = detailLoading
const schedule = computed<ClassScheduleModel | null>(() => selectedSchedule.value)
const activeTab = ref('info')
const fileStore = useFileStore()
const scheduleApi = new ClassScheduleApi()
const notificationStore = useNotificationStore()
const attendants = computed<ClassAttendantModel[]>(() => attendantsMap.value[props.scheduleId ?? ''] || [])
const attendanceLoading = computed(() => attendantsLoadingState.value)
const savingAttendance = computed(() => attendantsSavingState.value)
const unlockingAttendance = ref(false)
const confirmingAttendance = ref(false)
const savingRemarks = ref(false)
type RemarkDraft = {
    comment: string
    participationLevel: number | null
    learningAbsorptionLevel: number | null
    disciplineLevel: number | null
    didHomework: boolean
    homeworkScore: number | null
}
const remarkDrafts = ref<Record<string, RemarkDraft>>({})
const attendanceColumns: BaseTableColumn[] = [
    { key: 'studentCode', labelKey: 'student.studentCode', width: 140, sticky: true, filterType: 'text' },
    { key: 'fullName', labelKey: 'student.name', minWidth: 150, sticky: true, filterType: 'text' },
    { key: 'nickname', labelKey: 'student.nickName', minWidth: 140 },
    { key: 'participationStatus', labelKey: 'classSchedule.attendanceStatus', minWidth: 130, align: 'center' },
    { key: 'homeworkStatus', labelKey: 'classSchedule.homeworkMain', minWidth: 120 },
    { key: 'attachment', labelKey: 'classSchedule.homeworkAttachments', minWidth: 160 },
    { key: 'comment', labelKey: 'classSchedule.attendanceNote', minWidth: 160 },
]
const participationOptions = computed(() => [
    { value: StudentParticipationStatus.Present, label: t('classSchedule.participationPresent') },
    { value: StudentParticipationStatus.Absent, label: t('classSchedule.participationAbsent') }
])
const canSaveAttendance = computed(() => !!props.scheduleId && attendants.value.length > 0)
const sessionLockStatus = computed(() => {
    const raw =
        schedule.value?.sessionAttendanceLockStatus ??
        (schedule.value as any)?.SessionAttendanceLockStatus ??
        null
    if (raw === null || raw === undefined) return null
    if (typeof raw === 'string') {
        const asNumber = Number(raw)
        if (!Number.isNaN(asNumber)) return asNumber
        const normalized = raw.toLowerCase()
        if (normalized === 'locked') return SessionAttendanceLockStatus.Locked
        if (normalized === 'unlocked') return SessionAttendanceLockStatus.Unlocked
    }
    return raw as number
})
const isSessionLocked = computed(() => sessionLockStatus.value === SessionAttendanceLockStatus.Locked)
const tabs = computed(() => [
    { name: 'info', label: t('class.infoTab') },
    { name: 'attendance', label: t('class.attendanceTab') },
    { name: 'notes', label: t('class.notesTab') }
])

const dialogTitleText = computed(() => {
    const no = scheduleNumberDisplay.value ? ` #${scheduleNumberDisplay.value}` : ''
    return `${t('classSchedule.detailTitle')}${no}`
})
const dialogSubtitleText = computed(() => {
    const parts = [dayOfWeekText.value, dateText.value].filter(Boolean)
    return parts.join(' - ')
})
const attendanceDescText = computed(() => {
    const base = t('classSchedule.attendanceDesc')
        .replace('#{num}', scheduleNumberDisplay.value || '?')
    return `${base} · ${t('classSchedule.presentCount')}: ${presentCount.value} · ${t('classSchedule.absentCount')}: ${absentCount.value}`
})

const scheduleNumber = computed(() => {
    if (schedule.value?.sessionIndex) return schedule.value.sessionIndex
    const sessionName = schedule.value?.courseLesson?.sessionName || ''
    const match = sessionName.match(/\d+/)
    return match ? match[0] : ''
})
const scheduleNumberDisplay = computed(() => {
    const num = scheduleNumber.value
    if (num !== undefined && num !== null && num !== '') return String(num)
    return '?'
})
const dateText = computed(() => formatDateCell(schedule.value?.date))
const dayOfWeekText = computed(() => {
    if (!schedule.value) return ''
    const fromDate = computeDayOfWeekFromDate(schedule.value.date)
    const day = fromDate ?? schedule.value.dayOfWeek
    try {
        return t(getDayOfWeekKey(day))
    } catch {
        return ''
    }
})

const courseLesson = computed(
    () => schedule.value?.courseLesson || (schedule.value as any)?.CourseLesson || null
)
const courseLessonHomeworkAttachments = computed(() =>
    normalizeAttachments(
        courseLesson.value?.homeworkAttachments ||
        (courseLesson.value as any)?.HomeworkAttachments
    )
)
const courseLessonReferenceAttachments = computed(() =>
    normalizeAttachments(
        courseLesson.value?.referenceAttachments ||
        (courseLesson.value as any)?.ReferenceAttachments
    )
)
const lessonType = computed(() =>
    courseLesson.value?.lectureType?.lectureName ||
    (courseLesson.value?.lectureType as any)?.LectureName ||
    '-'
)
const lessonName = computed(() =>
    courseLesson.value?.lessonName ||
    (courseLesson.value as any)?.LessonName ||
    courseLesson.value?.sessionName ||
    (courseLesson.value as any)?.SessionName ||
    '-'
)
const lessonContent = computed(() =>
    courseLesson.value?.content ||
    (courseLesson.value as any)?.Content ||
    schedule.value?.content ||
    '-'
)
const lessonObjective = computed(() =>
    courseLesson.value?.objective ||
    (courseLesson.value as any)?.Objective ||
    '-'
)
const referenceText = computed(() =>
    courseLesson.value?.reference ||
    (courseLesson.value as any)?.Reference ||
    '-'
)
const referenceLink = computed(() => {
    const ref =
        courseLesson.value?.reference ||
        (courseLesson.value as any)?.Reference ||
        ''
    if (!ref) return ''
    if (ref.startsWith('http')) return ref
    if (ref.startsWith('www.')) return `https://${ref}`
    return ''
})
const homeworkMain = computed(() =>
    courseLesson.value?.homework ||
    (courseLesson.value as any)?.Homework ||
    '-'
)
const homeworkExtra = computed(() => schedule.value?.homeworkPlusContent || (schedule.value as any)?.HomeworkPlusContent || '-')
const teacherName = computed(() =>
    schedule.value?.teacher?.applicationUser?.fullName ||
    schedule.value?.teacher?.fullName ||
    (schedule.value as any)?.teacherName ||
    '-'
)
const syllabusText = computed(() =>
    schedule.value?.homeworkPlusName ||
    (schedule.value as any)?.HomeworkPlusName ||
    courseLesson.value?.lectureType?.attachment?.fileName ||
    (courseLesson.value?.lectureType as any)?.Attachment?.fileName ||
    '-'
)
const presentCount = computed(() => attendants.value.filter(a => a.studentParticipationStatus === StudentParticipationStatus.Present).length)
const absentCount = computed(() => attendants.value.filter(a => a.studentParticipationStatus === StudentParticipationStatus.Absent).length)
const remarks = computed<ClassAttendantModel[]>(() => (attendants.value || []).filter(hasRemarkData))
const canSaveRemarks = computed(() => !!props.scheduleId && remarks.value.length > 0)

function getDraft(item: ClassAttendantModel) {
    const existing = remarkDrafts.value[item.studentId]
    if (existing) return existing
    const fallbackStar = item.star ?? null
    const draft = {
        comment: item.comment || '',
        participationLevel: item.participationLevel ?? fallbackStar,
        learningAbsorptionLevel: item.learningAbsorptionLevel ?? fallbackStar,
        disciplineLevel: item.disciplineLevel ?? fallbackStar,
        didHomework: item.homeworkScore !== null && item.homeworkScore !== undefined,
        homeworkScore: item.homeworkScore ?? null
    }
    remarkDrafts.value = { ...remarkDrafts.value, [item.studentId]: draft }
    return remarkDrafts.value[item.studentId]
}

function homeworkStatusLabel(status?: number | null) {
    switch (status) {
        case StudentHomeworkStatus.Done: return t('classSchedule.homeworkDone')
        case StudentHomeworkStatus.NotDone: return t('classSchedule.homeworkNotDone')
        default: return '-'
    }
}

watch(
    () => [props.scheduleId, props.visible, activeTab.value],
    async ([id, visible, tab]) => {
        if (visible && typeof id === 'string') {
            await fetchSchedule(id)
            if (tab === 'attendance') {
                await fetchAttendants(id)
            } else if (tab === 'notes' && id) {
                await fetchRemarks(id)
            }
        } else if (!visible) {
            classScheduleStore.resetSelected()
            activeTab.value = 'info'
            remarkDrafts.value = {}
        }
    },
    { immediate: true }
)

async function fetchSchedule(id: string) {
    try {
        const res = await classScheduleStore.fetchById(id)
        if (!res?.succeeded) classScheduleStore.resetSelected()
    } catch (err) {
        console.error('Failed to fetch schedule detail', err)
        classScheduleStore.resetSelected()
    }
}

async function fetchAttendants(scheduleId: string) {
    try {
        await classAttendantStore.fetchByScheduleId(scheduleId)
    } catch (err) {
        console.error('Failed to fetch attendants', err)
    }
}

async function fetchRemarks(scheduleId: string) {
    try {
        await fetchAttendants(scheduleId)
        const map: Record<string, RemarkDraft> = { ...remarkDrafts.value }
        attendants.value.forEach((a) => {
            const existing = map[a.studentId] || {}
            const fallbackStar = a.star ?? null
            const homeworkScore = (existing as any).homeworkScore ?? a.homeworkScore ?? null
            map[a.studentId] = {
                comment: (existing as any).comment ?? a.comment ?? '',
                participationLevel: (existing as any).participationLevel ?? a.participationLevel ?? fallbackStar,
                learningAbsorptionLevel: (existing as any).learningAbsorptionLevel ?? a.learningAbsorptionLevel ?? fallbackStar,
                disciplineLevel: (existing as any).disciplineLevel ?? a.disciplineLevel ?? fallbackStar,
                didHomework: (existing as any).didHomework ?? (homeworkScore !== null && homeworkScore !== undefined),
                homeworkScore
            }
        })
        remarkDrafts.value = map
    } catch (err) {
        console.error('Failed to fetch remarks', err)
        if (!Object.keys(remarkDrafts.value).length) remarkDrafts.value = {}
    }
}

function formatDateCell(value?: string | Date | null) {
    if (!value) return ''
    return formatDateUtil(value, 'DD/MM/YYYY') || ''
}

function hasRemarkData(item: ClassAttendantModel) {
    const draft = getDraft(item)
    const comment = draft.comment?.trim()
    return Boolean(
        comment && comment.length ||
        (draft.participationLevel !== null && draft.participationLevel !== undefined) ||
        (draft.learningAbsorptionLevel !== null && draft.learningAbsorptionLevel !== undefined) ||
        (draft.disciplineLevel !== null && draft.disciplineLevel !== undefined) ||
        (draft.homeworkScore !== null && draft.homeworkScore !== undefined)
    )
}

function getRemarkDate(item: ClassAttendantModel) {
    if (!hasRemarkData(item)) return ''
    return item.updatedAt || item.createdAt || ''
}

function computeDayOfWeekFromDate(value?: string | Date | null) {
    if (!value) return null
    const d = new Date(value)
    if (Number.isNaN(d.getTime())) return null
    const jsDay = d.getDay() // 0 Sunday - 6 Saturday
    return jsDay === 0 ? 7 : jsDay // backend uses Monday=1..Sunday=7
}

function attendanceLabel(status?: number | null) {
    switch (status) {
        case SessionAttendanceStatus.Checked: return t('classSchedule.attendanceChecked')
        case SessionAttendanceStatus.Confirmed: return t('classSchedule.attendanceConfirmed')
        default: return t('classSchedule.attendanceNotChecked')
    }
}
function attendanceColor(status?: number | null) {
    switch (status) {
        case SessionAttendanceStatus.Checked: return 'warning'
        case SessionAttendanceStatus.Confirmed: return 'success'
        default: return 'info'
    }
}
function scheduleLabel(status?: number | null) {
    switch (status) {
        case ClassScheduleStatus.Completed: return t('classStatus.completed')
        case ClassScheduleStatus.Cancelled: return t('classStatus.cancelled') || t('common.cancelled')
        default: return t('classSchedule.notStarted')
    }
}
function scheduleColor(status?: number | null) {
    switch (status) {
        case ClassScheduleStatus.Completed: return 'success'
        case ClassScheduleStatus.Cancelled: return 'danger'
        default: return 'info'
    }
}

function downloadAttachment(att?: any) {
    if (!att) return
    const path =
        typeof att === 'string'
            ? att
            : att.path || att.Path || att.filePath || att.FilePath || att.relativePath || att.RelativePath || ''
    if (!path) return
    const fallbackName = path.split('/').pop() || t('common.download')
    const fileName =
        typeof att === 'string'
            ? fallbackName
            : att.fileName || att.FileName || att.name || att.Name || fallbackName
    fileStore.downloadFile(path, fileName)
}

function updateParticipation(item: ClassAttendantModel, val: any) {
    const numVal = typeof val === 'string' ? Number(val) : val
    item.studentParticipationStatus = numVal as StudentParticipationStatus
}

function formatRating(val?: number | null) {
    if (val === null || val === undefined) return ''
    const num = Number(val)
    if (Number.isNaN(num)) return ''
    return num % 1 === 0 ? `${num}` : num.toFixed(1)
}

async function saveAttendance() {
    if (!props.scheduleId) return
    try {
        const payload = attendants.value.map((a) => ({
            ...a,
            attachment: (a as any).attachment?.path || (a as any).attachment || null
        }))
        await classAttendantStore.updateAttendants(props.scheduleId, payload as any)
        await fetchAttendants(props.scheduleId)
        notify('success', 'classSchedule.saveAttendanceSuccess', 'Lưu điểm danh thành công')
    } catch (err) {
        console.error('Failed to update attendance', err)
        // notify('error', 'classSchedule.saveAttendanceFailed', 'Lưu điểm danh thất bại')
    }
}

async function unlockAttendance() {
    if (!props.scheduleId) return
    unlockingAttendance.value = true
    try {
        await scheduleApi.updateSessionAttendanceLockStatus(props.scheduleId, SessionAttendanceLockStatus.Unlocked)
        await fetchSchedule(props.scheduleId)
        notify('success', 'classSchedule.unlockAttendanceSuccess', 'Mở khóa điểm danh thành công')
    } catch (err) {
        console.error('Failed to unlock attendance', err)
        notify('error', 'classSchedule.unlockAttendanceFailed', 'Mở khóa điểm danh thất bại')
    } finally {
        unlockingAttendance.value = false
    }
}

async function confirmAttendance() {
    if (!props.scheduleId) return
    confirmingAttendance.value = true
    try {
        await scheduleApi.confirmSessionAttendance(props.scheduleId)
        await fetchSchedule(props.scheduleId)
        notify('success', 'classSchedule.confirmAttendanceSuccess', 'Xác nhận điểm danh thành công')
    } catch (err) {
        console.error('Failed to confirm attendance', err)
        notify('error', 'classSchedule.confirmAttendanceFailed', 'Xác nhận điểm danh thất bại')
    } finally {
        confirmingAttendance.value = false
    }
}

async function saveRemarks() {
    if (!props.scheduleId) return
    // không lưu khi tất cả vắng mặt
    const hasActive = attendants.value.some(a => a.studentParticipationStatus !== StudentParticipationStatus.Absent)
    if (!hasActive) {
        notify('warning', 'classSchedule.noActiveStudents', 'Không có học viên tham gia để đánh giá')
        return
    }
    const requiresComment = attendants.value.some((a) => {
        if (a.studentParticipationStatus === StudentParticipationStatus.Absent) return false
        const draft = getDraft(a)
        const participationLevel = normalizeNumber(draft.participationLevel)
        const learningAbsorptionLevel = normalizeNumber(draft.learningAbsorptionLevel)
        const disciplineLevel = normalizeNumber(draft.disciplineLevel)
        const hasLowRating = [participationLevel, learningAbsorptionLevel, disciplineLevel].some(
            (val) => val !== null && val < 4
        )
        if (!hasLowRating) return false
        return !draft.comment || !draft.comment.trim()
    })
    if (requiresComment) {
        notify('warning', 'classSchedule.remarkRequired', 'Cần nhập nhận xét khi có tiêu chí dưới 4 sao')
        return
    }
    savingRemarks.value = true
    const payload = attendants.value.map((a) => {
        const draft = getDraft(a)
        const participationLevel = normalizeNumber(draft.participationLevel)
        const learningAbsorptionLevel = normalizeNumber(draft.learningAbsorptionLevel)
        const disciplineLevel = normalizeNumber(draft.disciplineLevel)
        const rawRating = averageLevels(participationLevel, learningAbsorptionLevel, disciplineLevel)
        const homeworkScore = normalizeNumber(draft.homeworkScore)
        return {
            ...a,
            comment: draft.comment || '',
            // Lưu điểm sao vào star, điểm BTVN vào homeworkScore
            star: rawRating,
            participationLevel,
            learningAbsorptionLevel,
            disciplineLevel,
            homeworkScore
        }
    })
    try {
        await classAttendantStore.updateAttendants(props.scheduleId, payload as any)
        await fetchAttendants(props.scheduleId)
        notify('success', 'classSchedule.saveRemarksSuccess', 'Lưu nhận xét thành công')
    } catch (err) {
        console.error('Failed to save remarks', err)
        //notify('error', 'classSchedule.saveRemarksFailed', 'Lưu nhận xét thất bại')
    } finally {
        savingRemarks.value = false
    }
}

function normalizeNumber(val: number | string | null | undefined): number | null {
    if (val === null || val === undefined || val === '') return null
    const num = Number(val)
    return Number.isNaN(num) ? null : num
}

function normalizeAttachments(list?: any): Array<{ path: string; fileName: string }> {
    if (!Array.isArray(list)) return []
    return list
        .map((item) => {
            if (!item) return null
            if (typeof item === 'string') {
                const name = item.split('/').pop() || item
                return { path: item, fileName: name }
            }
            const path =
                item.path ||
                item.Path ||
                item.filePath ||
                item.FilePath ||
                item.relativePath ||
                item.RelativePath ||
                item.url ||
                item.Url ||
                ''
            if (!path) return null
            const fileName =
                item.fileName ||
                item.FileName ||
                item.name ||
                item.Name ||
                (path.split('/').pop() || t('common.download'))
            return { path, fileName }
        })
        .filter((item): item is { path: string; fileName: string } => Boolean(item))
}

function onHomeworkToggle(draft: RemarkDraft, enabled: boolean) {
    if (!enabled) {
        draft.homeworkScore = null
        return
    }
    if (draft.homeworkScore === null || draft.homeworkScore === undefined) {
        draft.homeworkScore = 0
    }
}

function averageLevels(
    participationLevel: number | null,
    learningAbsorptionLevel: number | null,
    disciplineLevel: number | null
): number | null {
    if (participationLevel === null || learningAbsorptionLevel === null || disciplineLevel === null) return null
    const avg = (participationLevel + learningAbsorptionLevel + disciplineLevel) / 3
    return Number(avg.toFixed(1))
}

function averageRating(draft: RemarkDraft) {
    return averageLevels(
        normalizeNumber(draft.participationLevel),
        normalizeNumber(draft.learningAbsorptionLevel),
        normalizeNumber(draft.disciplineLevel)
    )
}

function notify(type: 'success' | 'error' | 'warning', key: string, fallback: string) {
    notificationStore.showToast(type, { key })
}
</script>

<style scoped>
.loading-wrap {
    min-height: 200px;
}

.dialog-header {
    margin-bottom: 0px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 12px;
}

.title-wrap {
    display: flex;
    gap: 10px;
    align-items: center;
}

.title-text {
    font-weight: 600;
    font-size: 16px;
}

.sub-text {
    color: #6b7280;
    display: flex;
    gap: 6px;
    align-items: center;
    flex-wrap: wrap;
}

.dot-sep {
    color: #d1d5db;
}

.badges {
    display: flex;
    gap: 6px;
}

.info-card {
    border-radius: 12px;
}

.info-card .field {
    margin-bottom: 12px;
    line-height: 1.5;
}

.info-card .field .label {
    display: inline-block;
    min-width: 140px;
}

.info-card .field .value {
    display: inline-block;
    line-height: 1.5;
}

.homework-field .value.multiline {
    white-space: pre-line;
}

.secondary-meta {
    margin-bottom: 4px;
}

.section-title {
    margin: 0 0 12px;
    font-weight: 600;
}

.link-text {
    color: #2563eb;
    text-decoration: underline;
}

.badge-holder {
    display: inline-flex;
    align-items: center;
    gap: 4px;
}

.placeholder {
    padding: 16px;
    color: #6b7280;
}

.footer-actions {
    display: flex;
    justify-content: flex-end;
    gap: 8px;
}

.inline-radio-group :deep(.el-radio-button__inner) {
    padding: 4px 10px;
}

.attendance-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 12px;
    margin-bottom: 12px;
}

.attendance-title {
    flex: 1;
    min-width: 0;
}

.attendance-actions {
    display: flex;
    align-items: center;
    gap: 8px;
}

.remarks-tab {
    display: flex;
    flex-direction: column;
    gap: 12px;
}

.remarks-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
}

.remarks-list {
    display: flex;
    flex-direction: column;
    gap: 10px;
}

.remark-card {
    border: 1px solid #eef2f7;
    --rating-label-width: 170px;
}

.remark-top {
    display: grid;
    grid-template-columns: minmax(0, 1.2fr) minmax(0, 1fr);
    gap: 16px;
    align-items: center;
    margin-bottom: 8px;
}

.remark-name {
    font-size: 14px;
}

.remark-date {
    display: grid;
    grid-template-columns: var(--rating-label-width) auto;
    align-items: center;
    column-gap: 8px;
    color: #6b7280;
    width: 100%;
}

.remark-date :deep(.el-rate) {
    display: inline-flex;
}

.remark-body {
    margin-bottom: 8px;
}

.remark-content {
    display: grid;
    grid-template-columns: minmax(0, 1fr) auto;
    gap: 16px;
    align-items: start;
}

.remark-left,
.remark-right {
    display: flex;
    flex-direction: column;
    gap: 10px;
}

.remark-right {
    width: 320px;
    justify-self: end;
}

.homework-row {
    display: flex;
    align-items: center;
    gap: 12px;
    flex-wrap: wrap;
}

.remark-rating-grid {
    display: flex;
    flex-direction: column;
    gap: 10px;
}

.remark-rating-item {
    display: grid;
    grid-template-columns: var(--rating-label-width) auto;
    align-items: center;
    column-gap: 8px;
}

.remark-rating-item .rating-stars {
    justify-self: end;
}

.remark-rating-item .label {
    color: #6b7280;
    font-size: 13px;
    padding-left: 12px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.remark-rate {
    display: inline-flex;
    align-items: center;
    gap: 6px;
}

.remark-rate-align {
    grid-column: 2;
    justify-self: end;
}

.remark-score-editor {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-top: 0;
}

.remark-score-editor .label {
    color: #6b7280;
    font-size: 13px;
}

.remark-score-editor :deep(.el-input-number) {
    width: 120px;
}

.rating-number {
    min-width: 24px;
    text-align: center;
    color: #374151;
}

.rating-number.pill {
    display: inline-block;
    padding: 2px 8px;
    border-radius: 12px;
    background: #f3f4f6;
    border: 1px solid #e5e7eb;
    font-weight: 600;
    min-width: 32px;
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
    vertical-align: middle;
    transform: translateY(0);
    font-size: 18px;
}

.muted-text {
    color: #6b7280;
}

:deep(.el-rate__icon),
:deep(img),
:deep(svg) {
    vertical-align: baseline !important;
}
</style>
