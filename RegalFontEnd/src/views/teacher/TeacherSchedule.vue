<template>
    <div class="teacher-schedule-page">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h3 class="fw-bold fs-3">{{ t('teacher.teachingSchedule') }}</h3>

            <div class="d-flex align-items-center gap-2">
                <el-button size="small" @click="prevWeek">{{ t('schedule.prevWeek') }}</el-button>
                <span class="fw-semibold">{{ formatWeekRange(currentWeekStart) }}</span>
                <el-button size="small" @click="nextWeek">{{ t('schedule.nextWeek') }}</el-button>
                <el-button size="small" type="primary" plain @click="resetToThisWeek">
                    {{ t('schedule.thisWeek') }}
                </el-button>
            </div>
        </div>

        <div v-if="loading" class="text-center py-6">
            <el-icon class="is-loading"><i class="bi bi-arrow-repeat"></i></el-icon>
        </div>

        <div v-else class="schedule-container">
            <table class="schedule-table w-100">
                <thead>
                    <tr>
                        <th>{{ t('schedule.time') }}</th>
                        <th v-for="day in weekDays" :key="day.value">
                            {{ t(getDayOfWeekKey(day.value)) }}<br />
                            <small>{{ day.label }}</small>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="slot in timeSlots" :key="slot">
                        <td class="time-col">{{ slot }}</td>

                        <td v-for="day in weekDays" :key="day.value" class="schedule-cell">
                            <div v-for="cls in getClassesFor(day.value, slot)" :key="cls.id + cls.date"
                                class="class-card">
                                <div class="fw-bold mb-1">{{ cls.className }}</div>
                                <div class="text-xs text-gray-200 mb-1">
                                    <i class="bi bi-tag"></i> {{ cls.classCode }}
                                </div>
                                <div class="text-xs text-gray-200 mb-1">
                                    <i class="bi bi-clock"></i> {{ cls.startTime }}–{{ cls.endTime }}
                                </div>
                                <div class="text-xs text-gray-200 mb-1">
                                    <i class="bi bi-book"></i> {{ cls.courseName }}
                                </div>
                                <div class="text-xs text-gray-200 mb-2">
                                    <i class="bi bi-people"></i>
                                    <span v-if="cls.studentCount != null">{{ cls.studentCount }} {{ t('schedule.students') }}</span>
                                    <span v-else>-</span>
                                </div>
                                <el-button size="small" class="border-0 w-100" @click="viewDetail(cls)">
                                    {{ t('schedule.detail') }}
                                </el-button>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div class="mt-6 d-flex flex-column align-items-end">
            <div>{{ t('schedule.totalSessions') }}: {{ summary.total }}</div>
            <div>{{ t('schedule.taughtSessions') }}: {{ summary.taught }}</div>
            <div>{{ t('schedule.upcomingSessions') }}: {{ summary.upcoming }}</div>
            <div>{{ t('schedule.cancelledSessions') }}: {{ summary.cancelled }}</div>
        </div>

        <ClassScheduleDetailDialog :visible="detailVisible" :schedule-id="detailScheduleId"
            @update:visible="detailVisible = $event" />

    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'
import dayjs from 'dayjs'
import { serviceFactory } from '@/services/ServiceFactory'
import { useClassAttendantStore } from '@/stores/classAttendantStore'
import type { TeacherSessionItemModel } from '@/api/TeacherSessionApi'
import { DayOfWeek, DAY_OF_WEEK_OPTIONS, getDayOfWeekKey } from '@/types/daysOfWeek'
import { ClassScheduleStatus } from '@/types'
import ClassScheduleDetailDialog from '@/views/class/ClassScheduleDetailDialog.vue'

const { t } = useI18n()
const classAttendantStore = useClassAttendantStore()
const { attendantsMap } = storeToRefs(classAttendantStore)

const props = defineProps<{ teacherId: string }>()

const loading = ref(false)
const sessions = ref<TeacherSessionItemModel[]>([])
const detailVisible = ref(false)
const detailScheduleId = ref<string | null>(null)

// 🗓️ Tuần hiện tại (tính từ thứ Hai)
const currentWeekStart = ref(dayjs().startOf('week').add(1, 'day'))
const weekDays = computed(() =>
    DAY_OF_WEEK_OPTIONS.map(o => ({
        value: o.value,
        label: currentWeekStart.value.add(o.value - 1, 'day').format('DD/MM')
    }))
)

function formatTime(value?: string | null) {
    return value ? value.substring(0, 5) : '--:--'
}

function formatTimeRange(start?: string | null, end?: string | null) {
    if (!start && !end) return ''
    return `${formatTime(start)}-${formatTime(end)}`
}

function resolveDayOfWeek(item: TeacherSessionItemModel) {
    if (typeof item.dayOfWeek === 'number' && item.dayOfWeek > 0) return item.dayOfWeek
    if (item.date) {
        const jsDay = dayjs(item.date).day()
        return jsDay === 0 ? 7 : jsDay
    }
    return null
}

function normalizeClassScheduleStatus(status: unknown) {
    if (status === null || status === undefined) return null
    if (typeof status === 'number') return status
    if (typeof status === 'string') {
        const normalized = status.trim().toLowerCase()
        const asNumber = Number(normalized)
        if (!Number.isNaN(asNumber)) return asNumber
        if (normalized === 'cancelled' || normalized === 'canceled') return ClassScheduleStatus.Cancelled
        if (normalized === 'completed') return ClassScheduleStatus.Completed
        if (normalized === 'notstarted' || normalized === 'not started') return ClassScheduleStatus.NotStarted
    }
    return null
}

function resolveScheduleId(item: TeacherSessionItemModel) {
    return (
        item.scheduleId ||
        (item as any).classScheduleId ||
        (item as any).ClassScheduleId ||
        ''
    )
}

function resolveStudentCount(item: TeacherSessionItemModel) {
    const scheduleId = resolveScheduleId(item)
    if (!scheduleId) return null
    if (!Object.prototype.hasOwnProperty.call(attendantsMap.value, scheduleId)) return null
    return attendantsMap.value[scheduleId]?.length ?? 0
}

const sessionsWithDisplay = computed(() =>
    sessions.value.map((item) => {
        const scheduleId = resolveScheduleId(item)
        return {
            ...item,
            scheduleId,
            id: scheduleId || item.classId || `${item.classCode ?? ''}-${item.date ?? ''}-${item.startTime ?? ''}`,
            resolvedDayOfWeek: resolveDayOfWeek(item),
            timeSlot: formatTimeRange(item.startTime, item.endTime),
            startTime: formatTime(item.startTime),
            endTime: formatTime(item.endTime),
            courseName: item.courseName || item.programName || '-',
            studentCount: resolveStudentCount(item),
        }
    })
)

const timeSlots = computed(() => {
    const slots = new Set<string>()
    sessionsWithDisplay.value.forEach((item) => {
        if (item.timeSlot) slots.add(item.timeSlot)
    })
    return Array.from(slots).sort((a, b) => {
        const [aStart] = a.split('-')
        const [bStart] = b.split('-')
        return aStart.localeCompare(bStart)
    })
})

function formatWeekRange(start: dayjs.Dayjs) {
    const end = start.add(6, 'day')
    return `${start.format('DD/MM')} - ${end.format('DD/MM/YYYY')}`
}

function nextWeek() {
    currentWeekStart.value = currentWeekStart.value.add(7, 'day')
}
function prevWeek() {
    currentWeekStart.value = currentWeekStart.value.subtract(7, 'day')
}
function resetToThisWeek() {
    currentWeekStart.value = dayjs().startOf('week').add(1, 'day')
}

// 🟣 Tải dữ liệu thật
async function fetchAttendantsForSessions(items: TeacherSessionItemModel[]) {
    const ids = Array.from(new Set(items.map((item) => resolveScheduleId(item)).filter(Boolean)))
    if (!ids.length) return
    await Promise.all(ids.map((id) => classAttendantStore.fetchByScheduleId(id)))
}

async function loadSchedule() {
    if (!props.teacherId) {
        sessions.value = []
        return
    }
    loading.value = true
    try {
        const fromDate = currentWeekStart.value.format('YYYY-MM-DD')
        const toDate = currentWeekStart.value.add(6, 'day').format('YYYY-MM-DD')
        const res = await serviceFactory.teacherSessionService.getTeacherWorkBoard({
            teacherId: props.teacherId,
            fromDate,
            toDate
        })
        sessions.value = res?.succeeded ? (res.data || []) : []
        await fetchAttendantsForSessions(sessions.value)
    } catch (err) {
        console.error('Failed to fetch teacher schedule', err)
        sessions.value = []
    } finally {
        loading.value = false
    }
}

// 🟣 Hàm lọc lớp theo thứ và khung giờ (chỉ nếu nằm trong tuần hợp lệ)
function getClassesFor(day: DayOfWeek, slot: string) {
    return sessionsWithDisplay.value.filter(
        (item) => item.resolvedDayOfWeek === day && item.timeSlot === slot
    )
}

function calculateSummary() {
    const today = dayjs().startOf('day')
    let totalSessions = 0
    let taughtSessions = 0
    let upcomingSessions = 0
    let cancelledSessions = 0

    sessionsWithDisplay.value.forEach((session) => {
        totalSessions += 1
        const date = session.date ? dayjs(session.date).startOf('day') : null
        if (date) {
            if (date.isBefore(today, 'day')) taughtSessions += 1
            else if (date.isAfter(today, 'day')) upcomingSessions += 1
            else taughtSessions += 1
        }
        const status = normalizeClassScheduleStatus(
            (session as any).classScheduleStatus ?? (session as any).ClassScheduleStatus
        )
        if (status === ClassScheduleStatus.Cancelled) {
            cancelledSessions += 1
        }
    })

    return {
        total: totalSessions,
        taught: taughtSessions,
        upcoming: upcomingSessions,
        cancelled: cancelledSessions
    }
}

const summary = computed(() => calculateSummary())





function viewDetail(cls: any) {
    const scheduleId = cls?.scheduleId || resolveScheduleId(cls)
    if (!scheduleId) return
    detailScheduleId.value = String(scheduleId)
    detailVisible.value = true
}

onMounted(loadSchedule)
watch([() => props.teacherId, () => currentWeekStart.value], () => {
    void loadSchedule()
})
</script>

<style scoped>
.teacher-schedule-page {
    background: #fff;
    border-radius: 12px;
    padding: 24px;
}

.schedule-table {
    border-collapse: collapse;
    width: 100%;
    table-layout: fixed;
    /* Giúp cột đều nhau và không bị phình */
    text-align: center;
}

.schedule-table th,
.schedule-table td {
    border: 1px solid #e6e6e6;
    padding: 4px;
    /* Giảm padding */
    vertical-align: top;
    height: 1px;
    /* Cho phép cell co lại tối đa */
}

.time-col {
    background: #fafafa;
    font-weight: 600;
    width: 110px;
    white-space: nowrap;
}

.schedule-cell {
    min-width: 80px;
    min-height: 40px;
    height: 100%;
    vertical-align: top;
    padding-right: 12px !important;
}

.class-card {
    background: #50005a;
    color: #fff;
    border-radius: 6px;
    padding: 6px 8px;
    font-size: 11px;
    margin: 6px 4px 4px 4px;
    line-height: 1.3;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.15);
    transition: all 0.2s ease;
    width: 100%;
    box-sizing: border-box;
    text-align: left;
    display: block;
}

.class-card .fw-bold {
    font-size: 11.5px;
    line-height: 1.2;
    margin-bottom: 2px;
    word-break: break-all;
}

.class-card i {
    font-size: 10px;
    margin-right: 3px;
}

.class-card .el-button {
    font-size: 10px;
    padding: 2px 6px;
    height: 18px;
    line-height: 16px;
    border-radius: 4px;
    margin-top: 4px;
    width: 100%;
    box-sizing: border-box;
}

.class-card .text-xs {
    font-size: 10px;
    line-height: 1.2;
    margin-bottom: 2px;
    word-break: break-all;
}
</style>
