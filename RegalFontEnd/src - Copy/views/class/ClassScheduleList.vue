<template>
    <div class="class-schedule-card">
        <div class="card-header">
            <div>
                <h5 class="title">
                    {{ t('classSchedule.listTitle') }}
                    <span v-if="schedules.length">({{ schedules.length }})</span>
                </h5>
            </div>
        </div>

        <template v-if="!classId">
            <el-empty :description="t('class.studentTabNeedSave')" />
        </template>
        <template v-else-if="!schedules.length && !loading">
            <el-empty :description="t('common.noData')" />
        </template>
        <BaseTable v-else :columns="columns" :items="displaySchedules" :loading="loading" :showIndex="false"
            :showPagination="false" :showCheckboxColumn="false" :showActionsColumn="true" :actionsColumnWidth="120"
            class="schedule-table">
            <template #cell-date="{ item }">
                {{ formatDateCell(item.date) }}
            </template>
            <template #cell-timeRange="{ item }">
                {{ formatTimeRange(item.startTime, item.endTime) }}
            </template>
            <template #cell-dayOfWeek="{ item }">
                {{ dayOfWeekLabel(item.dayOfWeek) }}
            </template>
            <template #cell-sessionAttendanceStatus="{ item }">
                <BaseBadge :label="attendanceLabel(item.sessionAttendanceStatus)"
                    :type="attendanceColor(item.sessionAttendanceStatus)" size="small" />
            </template>
            <template #cell-classScheduleStatus="{ item }">
                <BaseBadge :label="scheduleLabel(item.classScheduleStatus)"
                    :type="scheduleColor(item.classScheduleStatus)" size="small" />
            </template>
            <!-- sửa tên slot từ #actions sang #cell-actions -->
            <template #actions="{ item }">
                <div class="action-wrapper">
                    <el-tooltip :content="t('common.viewDetail')" placement="top" :teleported="true"
                        :popper-options="{ strategy: 'fixed' }" popper-class="table-tooltip-popper">
                        <el-button text class="action-icon-btn" @click.stop="handleView(item)"
                            :disabled="isFutureSchedule(item)">
                            <el-icon>
                                <View />
                            </el-icon>
                        </el-button>
                    </el-tooltip>
                    <el-tooltip :content="t('common.edit')" placement="top" :teleported="true"
                        :popper-options="{ strategy: 'fixed' }" popper-class="table-tooltip-popper">
                        <el-button text class="action-icon-btn" @click.stop="handleEdit(item)"
                            :disabled="isFutureSchedule(item)">
                            <el-icon>
                                <Edit />
                            </el-icon>
                        </el-button>
                    </el-tooltip>
                    <el-tooltip
                        :content="canCancel(item) ? t('common.cancel') : t('classSchedule.cancelNotAllowedCompleted')"
                        placement="top" :teleported="true" :popper-options="{ strategy: 'fixed' }"
                        popper-class="table-tooltip-popper">
                        <el-button text class="action-icon-btn" :disabled="!canCancel(item)"
                            @click.stop="handleCancel(item)">
                            <el-icon>
                                <CloseBold />
                            </el-icon>
                        </el-button>
                    </el-tooltip>

                </div>
            </template>
        </BaseTable>
        <ClassScheduleDetailDialog :visible="detailVisible" :schedule-id="detailScheduleId"
            @update:visible="detailVisible = $event" />
        <ClassScheduleEditDialog :visible="editVisible" :schedule-id="editScheduleId"
            @update:visible="editVisible = $event" @saved="refreshAfterEdit" />
        <ClassScheduleCancelDialog :visible="cancelVisible" :class-id="props.classId" :schedule="cancelSchedule"
            @update:visible="cancelVisible = $event" @cancelled="refreshAfterEdit" />

    </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { View, Edit, CloseBold } from '@element-plus/icons-vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import type { ClassScheduleModel } from '@/api/ClassScheduleApi'
import { ClassScheduleStatus, SessionAttendanceStatus } from '@/types'
import { formatDate as formatDateUtil } from '@/utils/format'
import { getDayOfWeekKey } from '@/types/daysOfWeek'
import ClassScheduleDetailDialog from './ClassScheduleDetailDialog.vue'
import ClassScheduleEditDialog from './ClassScheduleEditDialog.vue'
import { useClassScheduleStore } from '@/stores/classScheduleStore'
import { storeToRefs } from 'pinia'
import ClassScheduleCancelDialog from './ClassScheduleCancelDialog.vue'
const cancelVisible = ref(false)
const cancelSchedule = ref<ClassScheduleModel | null>(null)
function canCancel(item: ClassScheduleModel) {
    return item.classScheduleStatus !== ClassScheduleStatus.Completed
}

const props = defineProps<{ classId?: string | null; active: boolean }>()
const { t } = useI18n()
const classScheduleStore = useClassScheduleStore()
const { schedules, loading } = storeToRefs(classScheduleStore)
const detailVisible = ref(false)
const detailScheduleId = ref<string | null>(null)
const editVisible = ref(false)
const editScheduleId = ref<string | null>(null)

const displaySchedules = computed(() =>
    (schedules.value || []).map((s, idx) => ({
        ...s,
        sessionIndex: s.sessionIndex ?? idx + 1,
        dayOfWeek: computeDayOfWeek(s.date, s.dayOfWeek),
        timeRange: formatTimeRange(s.startTime, s.endTime)
    }))
)

const columns: BaseTableColumn[] = [
    { key: 'sessionIndex', labelKey: 'classSchedule.sessionIndex', width: 90, align: 'center' },
    { key: 'date', labelKey: 'class.startDate', width: 140 },
    { key: 'timeRange', labelKey: 'class.schedule', width: 180 },
    { key: 'dayOfWeek', labelKey: 'classSchedule.dayOfWeek', width: 120 },
    { key: 'sessionAttendanceStatus', labelKey: 'classSchedule.attendanceStatus', width: 180, align: 'center' },
    { key: 'classScheduleStatus', labelKey: 'classSchedule.classScheduleStatus', align: 'center' },
]

watch(
    () => [props.classId, props.active],
    async ([classId, active]) => {
        if (active && typeof classId === 'string') {
            await fetchSchedules(classId)
        }
    },
    { immediate: true }
)

async function fetchSchedules(classId: string) {
    try {
        await classScheduleStore.fetchByClassId(classId)
    } catch (err) {
        console.error('Failed to fetch schedules', err)
        classScheduleStore.schedules = []
    }
}

function formatDateCell(value?: string | Date | null) {
    if (!value) return '-'
    const result = formatDateUtil(value, 'DD/MM/YYYY')
    return result || '-'
}

function formatTimeRange(start?: string | null, end?: string | null) {
    const s = start ? start.toString().substring(0, 5) : '--:--'
    const e = end ? end.toString().substring(0, 5) : '--:--'
    return `${s} - ${e}`
}

function getScheduleDateTime(item: ClassScheduleModel) {
    if (!item.date) return null
    const parsedDate = new Date(item.date)
    if (Number.isNaN(parsedDate.valueOf())) return null
    if (item.startTime) {
        const [hoursStr, minutesStr] = item.startTime.toString().split(':')
        const hours = Number(hoursStr)
        const minutes = Number(minutesStr)
        if (!Number.isNaN(hours)) {
            parsedDate.setHours(hours, Number.isNaN(minutes) ? 0 : minutes, 0, 0)
        }
    }
    return parsedDate
}

function isFutureSchedule(item: ClassScheduleModel) {
    const scheduleDate = getScheduleDateTime(item)
    if (!scheduleDate) return false
    return scheduleDate > new Date()
}

function dayOfWeekLabel(day?: number) {
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
        const jsDay = d.getDay() // 0 (Sun) - 6 (Sat)
        // Assuming backend uses Monday=1..Sunday=7
        return jsDay === 0 ? 7 : jsDay
    }
    return fallback ?? 0
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
        default: return t('classSchedule.notStarted') || t('classStatus.plan')
    }
}
function scheduleColor(status?: number | null) {
    switch (status) {
        case ClassScheduleStatus.Completed: return 'success'
        case ClassScheduleStatus.Cancelled: return 'danger'
        default: return 'info'
    }
}

function handleView(item: ClassScheduleModel) {
    detailScheduleId.value = item.id || null
    detailVisible.value = true
}

async function refreshAfterEdit() {
    if (props.classId && typeof props.classId === 'string') {
        await fetchSchedules(props.classId)
    }
}

function handleEdit(item: ClassScheduleModel) {
    editScheduleId.value = item.id || null
    editVisible.value = true
}

function handleCancel(item: ClassScheduleModel) {
    cancelSchedule.value = item
    cancelVisible.value = true
}

</script>

<style scoped>
.class-schedule-card {
    background: #fff;
    border-radius: 16px;
    border: 1px solid #f0f0f5;
    padding: 20px;
    min-height: 320px;
}

.card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 12px;
}

.title {
    margin: 0;
    font-size: 16px;
    font-weight: 600;
}

.subtitle {
    margin: 2px 0 0;
    color: #8f8f8f;
    font-size: 13px;
}

.schedule-table {
    font-size: 13px;
}

.action-wrapper {
    display: flex;
    justify-content: center;
    gap: 8px;
}

.action-icon-btn {
    color: #191a1b !important;
    font-size: 14px;
    padding: 4px 6px;
    background: transparent !important;
    border: none !important;
}

.el-icon {
    color: #54585c !important;
}

.action-icon-btn:hover {
    color: #2563eb !important;
}
</style>
