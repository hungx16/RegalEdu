<template>
    <div class="attendance-card">
        <div class="attendance-header">
            <h5 class="title">{{ t('classAttendance.title') }}</h5>
            <div class="legend">
                <span><i class="bi bi-check-lg icon present" /> {{ t('classAttendance.presentDone') }}</span>
                <span><i class="bi bi-exclamation-triangle-fill icon warn" /> {{ t('classAttendance.presentNotDone')
                    }}</span>
                <span><i class="bi bi-x-lg icon absent" /> {{ t('classAttendance.absent') }}</span>
                <span><i class="bi bi-circle icon empty" /> {{ t('classAttendance.notJoined') }}</span>
            </div>
        </div>

        <template v-if="!classId">
            <el-empty :description="t('class.studentTabNeedSave')" />
        </template>
        <template v-else-if="!schedules.length && !loading">
            <el-empty :description="t('common.noData')" />
        </template>
        <div v-else class="table-wrapper" v-loading="loading">
            <div class="table-responsive">
                <table class="attendance-table">
                    <thead>
                        <tr>
                            <th class="student-col">{{ t('classAttendance.student') }}</th>
                            <th v-for="(schedule, idx) in schedules" :key="schedule.id || idx" class="session-col">
                                <div class="session-title">
                                    {{ t('classSchedule.sessionIndex') }} {{ idx + 1 }}
                                </div>
                                <div class="session-date">
                                    {{ formatDateCell(schedule.date) }}
                                </div>
                            </th>
                            <th class="total-col">{{ t('classAttendance.total') }}</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(student, stuIdx) in students" :key="student.id || stuIdx">
                            <td class="student-cell">
                                <div class="name">{{ student.fullName || t('common.unknown') }}</div>
                                <div class="alias" v-if="student.englishName">({{ student.englishName }})</div>
                            </td>
                            <td v-for="(schedule, sesIdx) in schedules" :key="schedule.id || sesIdx"
                                class="status-cell">
                                <div :class="['status-icon', statusClass(schedule.id, student.id)]">
                                    <i :class="statusIcon(schedule.id, student.id)" />
                                </div>
                            </td>
                            <td class="total-cell">
                                <div class="count">{{ presentCount(student.id || '') }}/{{ schedules.length }}</div>
                                <div class="percent">({{ presentPercent(student.id || '') }}%)</div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import type { ClassAttendantModel } from '@/api/ClassAttendantApi'
import type { StudentModel } from '@/api/StudentApi'
import { formatDate as formatDateUtil } from '@/utils/format'
import { StudentHomeworkStatus, StudentParticipationStatus } from '@/types'
import { useClassScheduleStore } from '@/stores/classScheduleStore'
import { useClassAttendantStore } from '@/stores/classAttendantStore'
import { storeToRefs } from 'pinia'

const props = defineProps<{ classId?: string | null; active: boolean }>()
const { t } = useI18n()
const classScheduleStore = useClassScheduleStore()
const classAttendantStore = useClassAttendantStore()
const { schedules } = storeToRefs(classScheduleStore)
const { attendantsMap } = storeToRefs(classAttendantStore)
const loading = ref(false)

watch(
    () => [props.classId, props.active],
    async ([classId, active]) => {
        if (active && typeof classId === 'string') {
            await fetchData(classId)
        }
    },
    { immediate: true }
)

const students = computed<StudentModel[]>(() => {
    const map = new Map<string, StudentModel>()
    Object.values(attendantsMap.value).forEach((attList) => {
        attList.forEach((a) => {
            if (a.student && !map.has(a.studentId)) {
                map.set(a.studentId, a.student)
            }
        })
    })
    // sort by name for stable ordering
    return Array.from(map.values())
        .filter((s) => s.id)
        .sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''))
})

async function fetchData(classId: string) {
    loading.value = true
    try {
        await classScheduleStore.fetchByClassId(classId)
        const attMap: Record<string, ClassAttendantModel[]> = {}
        await Promise.all(
            (schedules.value || []).map(async (s) => {
                try {
                    const res = await classAttendantStore.fetchByScheduleId(s.id || '')
                    attMap[s.id || ''] = res?.data || []
                } catch (err) {
                    console.error('Failed to fetch attendants for schedule', s.id, err)
                    attMap[s.id || ''] = []
                }
            })
        )
        classAttendantStore.attendantsMap = attMap
    } catch (err) {
        console.error('Failed to fetch attendance data', err)
        classScheduleStore.schedules = []
        classAttendantStore.attendantsMap = {}
    } finally {
        loading.value = false
    }
}

function formatDateCell(value?: string | Date | null) {
    if (!value) return ''
    return formatDateUtil(value, 'DD/MM/YYYY') || ''
}

function findAttendance(scheduleId?: string | null, studentId?: string | null) {
    if (!scheduleId || !studentId) return undefined
    const list = attendantsMap.value[scheduleId] || []
    return list.find((a) => a.studentId === studentId)
}

function statusClass(scheduleId?: string | null, studentId?: string | null) {
    const att = findAttendance(scheduleId, studentId)
    if (!att) return 'empty'
    if (att.studentParticipationStatus === StudentParticipationStatus.Present) {
        return isHomeworkDone(att) ? 'present' : 'warn'
    }
    if (att.studentParticipationStatus === StudentParticipationStatus.Absent) return 'absent'
    return 'empty'
}

function statusIcon(scheduleId?: string | null, studentId?: string | null) {
    const att = findAttendance(scheduleId, studentId)
    if (!att) return 'bi bi-circle'
    if (att.studentParticipationStatus === StudentParticipationStatus.Present) {
        return isHomeworkDone(att) ? 'bi bi-check-lg' : 'bi bi-exclamation-triangle-fill'
    }
    if (att.studentParticipationStatus === StudentParticipationStatus.Absent) return 'bi bi-x-lg'
    return 'bi bi-circle'
}

function isHomeworkDone(att: ClassAttendantModel) {
    if (att.studentHomeworkStatus === StudentHomeworkStatus.Done) return true
    if (att.studentHomeworkStatus === StudentHomeworkStatus.NotDone) {
        return att.homeworkScore !== null && att.homeworkScore !== undefined
    }
    return att.homeworkScore !== null && att.homeworkScore !== undefined
}

function presentCount(studentId: string) {
    let count = 0
    schedules.value.forEach((s) => {
        const att = findAttendance(s.id || '', studentId)
        if (att && att.studentParticipationStatus === StudentParticipationStatus.Present) {
            count += 1
        }
    })
    return count
}

function presentPercent(studentId: string) {
    if (!schedules.value.length) return 0
    return Math.round((presentCount(studentId) / schedules.value.length) * 100)
}
</script>

<style scoped>
.attendance-card {
    background: #fff;
    border: 1px solid #f0f0f5;
    border-radius: 16px;
    padding: 16px;
}

.attendance-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    gap: 12px;
    margin-bottom: 12px;
}

.title {
    margin: 0;
    font-size: 16px;
    font-weight: 600;
}

.legend {
    display: flex;
    flex-wrap: wrap;
    gap: 12px;
    color: #6b7280;
    font-size: 13px;
}

.legend .icon {
    margin-right: 4px;
}

.table-wrapper {
    width: 100%;
}

.table-responsive {
    overflow-x: auto;
}

.attendance-table {
    width: 100%;
    border-collapse: collapse;
    font-size: 13px;
}

.attendance-table th,
.attendance-table td {
    border: 1px solid #f1f2f5;
    text-align: center;
    padding: 10px 8px;
    white-space: nowrap;
}

.attendance-table thead th {
    background: #fafbff;
    font-weight: 600;
}

.student-col {
    text-align: left;
    min-width: 180px;
}

.student-cell {
    text-align: left;
}

.student-cell .name {
    font-weight: 600;
}

.student-cell .alias {
    color: #6b7280;
    font-size: 12px;
}

.session-col .session-title {
    font-weight: 600;
}

.session-col .session-date {
    color: #6b7280;
    font-size: 12px;
}

.status-cell {
    padding: 8px 4px;
}

.status-icon {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 22px;
    height: 22px;
    border-radius: 50%;
    border: 1px solid transparent;
}

.status-icon.present {
    color: #16a34a;
}

.status-icon.warn {
    color: #d97706;
}

.status-icon.absent {
    color: #dc2626;
}

.status-icon.empty {
    color: #9ca3af;
}

.total-col {
    min-width: 90px;
}

.total-cell .count {
    font-weight: 600;
}

.total-cell .percent {
    color: #6b7280;
    font-size: 12px;
}
</style>
