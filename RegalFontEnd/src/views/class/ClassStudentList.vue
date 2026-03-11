<template>
    <div class="class-student-card">
        <div class="card-header">
            <div>
                <h5 class="title">{{ t('class.studentListTitle') }} ({{ students.length }})</h5>
                <p class="subtitle">{{ t('class.studentListSubtitle') }}</p>
            </div>
            <div class="header-actions">
                <el-button v-if="employeeStore.isAcademicAffairsEmployee" size="small" type="primary" plain
                    :disabled="!classId || assigning" :loading="assigning" @click="handleManualAssign">
                    {{ t('class.manualAssign') }}
                </el-button>
                <el-button v-if="employeeStore.isAcademicAffairsEmployee" size="small" type="primary" plain
                    :disabled="!classId || assigning" :loading="assigning" @click="handleAutoAssign">
                    {{ t('class.autoAssign') }}
                </el-button>
            </div>
        </div>

        <template v-if="!classId">
            <el-empty :description="t('class.studentTabNeedSave')" />
        </template>
        <template v-else-if="!students.length && !loading">
            <el-empty :description="t('class.studentTabNoData')" />
        </template>
        <BaseTable v-else :columns="columns" :items="students" :loading="loading" :showIndex="true"
            :showPagination="false" :showCheckboxColumn="false" :showActionsColumn="showActionsColumn"
            class="student-table" height="500px" :show-delete="true" @delete="handleRemove">
            <template #cell-englishName="{ item }">
                {{ item.englishName || '-' }}
            </template>
            <template #cell-birthDate="{ item }">
                {{ formatDateCell(item.birthDate) }}
            </template>
            <template #cell-totalCourseFee="{ item }">
                {{ formatCurrencyCell(item.totalCourseFee) }}
            </template>
            <template #cell-paidAmount="{ item }">
                {{ formatCurrencyCell(item.paidAmount) }}
            </template>
            <template #cell-remainingAmount="{ item }">
                {{ formatCurrencyCell(item.remainingAmount) }}
            </template>
            <template #cell-unpaidAmount="{ item }">
                {{ formatCurrencyCell(item.unpaidAmount) }}
            </template>
            <template #cell-totalSessions="{ item }">
                {{ item.totalSessions ?? 0 }}
            </template>
            <template #cell-paidSessions="{ item }">
                {{ item.paidSessions ?? 0 }}
            </template>
            <template #cell-attendedSessions="{ item }">
                {{ item.attendedSessions ?? 0 }}
            </template>
            <template #cell-remainingSessions="{ item }">
                {{ item.remainingSessions ?? 0 }}
            </template>
            <template #cell-unpaidSessions="{ item }">
                {{ item.unpaidSessions ?? 0 }}
            </template>
            <template #cell-startDate="{ item }">
                {{ formatDateCell(item.startDate) }}
            </template>
            <template #cell-endDate="{ item }">
                {{ formatDateCell(item.endDate) }}
            </template>
            <template #cell-studentType="{ item }">
                {{ item.studentType || '-' }}
            </template>
            <template #cell-studentStatus="{ item }">
                <BaseBadge :label="statusLabel(item.studentStatus)" :type="statusColor(item.studentStatus)"
                    size="small" />
            </template>
            <template #cell-actions="{ item }">
                <div class="action-wrapper">
                    <el-tooltip :content="t('common.delete')" placement="top" :teleported="true"
                        :popper-options="{ strategy: 'fixed' }" popper-class="table-tooltip-popper">
                        <el-button link class="action-icon-btn danger" aria-label="delete" @click="handleRemove(item)">
                            <i class="bi bi-trash" />
                        </el-button>
                    </el-tooltip>
                </div>
            </template>
        </BaseTable>
        <BaseDialogForm :visible="manualDialogVisible" :title="t('class.manualAssign')" :show-delete="false"
            :mode="'edit'" width="720px" :submit-disabled="!selectedAssignableIds.length" :loading="assigning"
            :form-data="manualForm" :rules="{}" @update:visible="manualDialogVisible = $event" @submit="assignSelected">
            <template #form>
                <el-skeleton v-if="assignableLoading" animated :rows="4" />
                <el-empty v-else-if="!assignableStudents.length" :description="t('common.noData')" />
                <BaseTable v-else :columns="assignableColumns" :items="assignableStudents" :showActionsColumn="false"
                    :showCheckboxColumn="true" :showPagination="false" height="360px"
                    @update:rows="onSelectionChange" />
            </template>

            <!-- use footer-extra because BaseDialogForm exposes slot footer-extra -->
            <!-- <template #footer-extra>
                <el-button @click="manualDialogVisible = false">{{ t('common.cancel') }}</el-button>
                <el-button type="primary" :disabled="!selectedAssignableIds.length" :loading="assigning"
                    @click="assignSelected">
                    {{ t('common.save') }}
                </el-button>
            </template> -->
        </BaseDialogForm>
    </div>

</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { StudentModel } from '@/api/StudentApi'
import { StudentParticipationStatus, StudentStatus } from '@/types'
import { formatDate as formatDateUtil, formatCurrency as baseFormatCurrency } from '@/utils/format'
import { useAssignedClassStore } from '@/stores/assignedClassStore'
import { useClassAttendantStore } from '@/stores/classAttendantStore'
import { useClassScheduleStore } from '@/stores/classScheduleStore'
import { storeToRefs } from 'pinia'
import { useNotificationStore } from '@/stores/notificationStore'
import type { ManualUnassignStudentCommand } from '@/api/AssignedClassApi'
import { useTeacherStore } from '@/stores/teacherStore'
import { useEmployeeStore } from '@/stores/employeeStore'
const employeeStore = useEmployeeStore()
const teacherStore = useTeacherStore()
const props = defineProps<{
    classId?: string | null
    active: boolean
}>()
const emit = defineEmits(['add-student', 'submit'])

const { t } = useI18n()
const assignedClassStore = useAssignedClassStore()
const classScheduleStore = useClassScheduleStore()
const classAttendantStore = useClassAttendantStore()
const notificationStore = useNotificationStore()
const { assignedStudents, assignableStudents, loading, assigning } = storeToRefs(assignedClassStore)
const { schedules } = storeToRefs(classScheduleStore)
const { attendantsMap } = storeToRefs(classAttendantStore)
const students = computed<Array<StudentModel & Record<string, any>>>(() =>
    (assignedStudents.value || []).map(enrichStudentWithEnrollment)
)
const assignableLoading = computed(() => assignedClassStore.loading && !assignableStudents.value.length)
const manualDialogVisible = ref(false)
const manualForm = ref<Record<string, any>>({})
const selectedAssignableIds = ref<string[]>([])
const assignableColumns: BaseTableColumn[] = [
    { key: 'studentCode', labelKey: 'student.code', width: 140, sticky: true },
    { key: 'fullName', labelKey: 'student.name', minWidth: 180 },
    { key: 'englishName', labelKey: 'student.englishName', minWidth: 140 },
]
const showActionsColumn = computed(() => !teacherStore.isCurrentUserTeacher)
const attendanceStatsByStudentId = computed(() => {
    const stats: Record<string, { attended: number; totalAssigned: number }> = {}
    schedules.value.forEach((schedule) => {
        const list = attendantsMap.value[schedule.id || ''] || []
        list.forEach((att) => {
            if (!att.studentId) return
            const current = stats[att.studentId] || { attended: 0, totalAssigned: 0 }
            current.totalAssigned += 1
            if (att.studentParticipationStatus === StudentParticipationStatus.Present) {
                current.attended += 1
            }
            stats[att.studentId] = current
        })
    })
    return stats
})

const columns: BaseTableColumn[] = [
    { key: 'studentCode', labelKey: 'student.code', width: 120, sticky: true },
    { key: 'fullName', labelKey: 'student.name', width: 180, sticky: true },
    { key: 'englishName', labelKey: 'student.englishName', width: 140 },
    { key: 'birthDate', labelKey: 'student.birthDate', width: 140 },
    { key: 'totalCourseFee', labelKey: 'class.studentTotalFee', width: 140, align: 'right' },
    { key: 'paidAmount', labelKey: 'class.studentPaidAmount', width: 140, align: 'right' },
    { key: 'remainingAmount', labelKey: 'class.studentRemainingAmount', width: 140, align: 'right' },
    { key: 'unpaidAmount', labelKey: 'class.studentUnpaidAmount', width: 140, align: 'right' },
    { key: 'totalSessions', labelKey: 'class.studentTotalSessions', width: 140, align: 'center' },
    { key: 'paidSessions', labelKey: 'class.studentPaidSessions', width: 150, align: 'center' },
    { key: 'attendedSessions', labelKey: 'class.studentAttendedSessions', width: 150, align: 'center' },
    { key: 'remainingSessions', labelKey: 'class.studentRemainingSessions', width: 150, align: 'center' },
    { key: 'unpaidSessions', labelKey: 'class.studentUnpaidSessions', width: 150, align: 'center' },
    { key: 'startDate', labelKey: 'class.startDate', width: 140 },
    { key: 'endDate', labelKey: 'class.endDate', width: 140 },
    { key: 'studentType', labelKey: 'class.studentType', width: 140 },
    { key: 'studentStatus', labelKey: 'student.status', width: 140 },
    { key: 'actions', labelKey: 'common.actions', width: 100, align: 'center' },
]

watch(
    () => [props.classId, props.active],
    async ([classId, active]) => {
        if (active && classId && typeof classId === 'string') {
            await Promise.all([fetchStudents(classId), fetchAttendanceData(classId)])
        }
    },
    { immediate: true }
)

async function fetchStudents(classId: string) {
    try {
        const res = await assignedClassStore.fetchAssignedStudents(classId)
        if (!res?.succeeded || !res.data) assignedClassStore.assignedStudents = []
    } catch (err) {
        console.error('Failed to fetch assigned students', err)
        assignedClassStore.assignedStudents = []
    }
}

async function fetchAttendanceData(classId: string) {
    try {
        await classScheduleStore.fetchByClassId(classId)
        await Promise.all(
            (schedules.value || [])
                .filter((s) => s.id)
                .map((s) => classAttendantStore.fetchByScheduleId(s.id as string))
        )
    } catch (err) {
        console.error('Failed to fetch attendance stats', err)
    }
}

async function handleManualAssign() {
    if (!props.classId) return
    manualDialogVisible.value = true
    selectedAssignableIds.value = []
    try {
        await assignedClassStore.fetchAssignableStudents(props.classId)
    } catch (err) {
        console.error('Failed to fetch assignable students', err)
    }
}


function onSelectionChange(selected: StudentModel[]) {
    const ids = (selected || [])
        .map(s => (s as any).id || (s as any).studentId || '')
        .filter(Boolean)
    selectedAssignableIds.value = Array.from(new Set(ids))
}

async function assignSelected() {
    if (!props.classId || !selectedAssignableIds.value.length) return
    try {
        for (const id of selectedAssignableIds.value) {
            await assignedClassStore.manualAssign({
                classId: props.classId,
                studentId: id
            } as any)
        }
        notificationStore.showToast('success', { key: 'class.manualAssignSuccess' })
        await fetchStudents(props.classId)
        manualDialogVisible.value = false
    } catch (error) {
        console.error('Manual assign failed', error)
        //notificationStore.showToast('error', { key: 'class.manualAssignFailed' })
    }
}

async function handleAutoAssign() {
    if (!props.classId) return
    notificationStore.showConfirm(
        { key: 'class.confirmAutoAssign' },
        async () => {
            try {
                const res = await assignedClassStore.autoAssign(props.classId as string)
                if (res?.succeeded) {
                    notificationStore.showToast('success', { key: 'class.autoAssignSuccess' })
                }
                await fetchStudents(props.classId as string)
                emit('add-student')
            } catch (error) {
                console.error('Auto assign students failed', error)
                // notificationStore.showToast('error', { key: 'class.autoAssignFailed' })
            }
        }
    )
}

async function handleRemove(student: StudentModel) {
    if (!props.classId || !student.id) return
    const payload: ManualUnassignStudentCommand = {
        classId: props.classId,
        studentId: student.id
    }
    notificationStore.showConfirm(
        { key: t('class.confirmUnassign') },
        async () => {
            try {
                const res = await assignedClassStore.manualUnassign(payload as any)
                await fetchStudents(props.classId as string)
            } catch (error) {
                console.error('Manual unassign failed', error)
                //notificationStore.showToast('error', { key: 'class.manualUnassignFailed' })
            }
        }
    )
}

function enrichStudentWithEnrollment(student: StudentModel & Record<string, any>) {
    const enrollment = student.enrollments?.[0]
    const fee = enrollment?.fee ?? 0
    const paidAmount = enrollment?.finalFee ?? 0
    const discount = enrollment?.discount ?? 0
    const delta = paidAmount + discount - fee
    const unpaid = delta < 0 ? Math.abs(delta) : 0
    const courseStatus = enrollment?.studentCourseStatus ?? student.studentStatus
    const startDate = enrollment?.startDate ?? student.startDate
    const endDate = enrollment?.endDate ?? student.endDate

    const totalSessions =
        (student.id && attendanceStatsByStudentId.value[student.id]?.totalAssigned) ??
        normalizePositiveNumber(enrollment?.times) ??
        normalizePositiveNumber((student as any).totalSessions) ??
        schedules.value.length ??
        0
    const attendedSessions =
        (student.id && attendanceStatsByStudentId.value[student.id]?.attended) ??
        (student as any).attendedSessions ??
        0
    const remainingSessions = Math.max(Number(totalSessions) - Number(attendedSessions), 0)
    const remainingAmount =
        Number(totalSessions) > 0
            ? Math.max(paidAmount - (paidAmount / Number(totalSessions)) * attendedSessions, 0)
            : 0

    return {
        ...student,
        studentStatus: courseStatus,
        totalCourseFee: fee,
        paidAmount,
        remainingAmount,
        unpaidAmount: unpaid,
        startDate,
        endDate,
        totalSessions,
        attendedSessions,
        remainingSessions,
        studentType: enrollment?.classType?.classTypeName ?? student.studentType,
        _enrollment: enrollment,
    }
}

function normalizePositiveNumber(value?: number | null) {
    if (value === null || value === undefined) return null
    const num = Number(value)
    if (!Number.isFinite(num) || num <= 0) return null
    return num
}

function formatDateCell(value?: string | Date | null) {
    if (!value) return '-'
    const result = formatDateUtil(value, 'DD/MM/YYYY')
    return result || '-'
}

function formatCurrencyCell(value?: number | null) {
    return baseFormatCurrency(value ?? 0)
}

function statusLabel(status?: StudentStatus | null) {
    switch (status) {
        case StudentStatus.Prospect: return t('studentStatus.prospect')
        case StudentStatus.Enrolled: return t('studentStatus.enrolled')
        case StudentStatus.Paused: return t('studentStatus.paused')
        case StudentStatus.Dropped: return t('studentStatus.dropped')
        case StudentStatus.Graduated: return t('studentStatus.graduated')
        default: return t('common.unknown')
    }
}

function statusColor(status?: StudentStatus | null) {
    switch (status) {
        case StudentStatus.Enrolled: return 'success'
        case StudentStatus.Prospect: return 'info'
        case StudentStatus.Paused: return 'warning'
        case StudentStatus.Dropped: return 'danger'
        case StudentStatus.Graduated: return 'purple'
        default: return 'gray'
    }
}
</script>

<style scoped>
.class-student-card {
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

.header-actions {
    display: flex;
    gap: 8px;
}

.subtitle {
    margin: 2px 0 0;
    color: #8f8f8f;
    font-size: 13px;
}

.student-table {
    font-size: 13px;
}

.action-wrapper {
    display: flex;
    justify-content: center;
    gap: 8px;
}

.action-icon-btn {
    color: #4b5563 !important;
    font-size: 16px;
    padding: 4px 6px;
}
</style>
