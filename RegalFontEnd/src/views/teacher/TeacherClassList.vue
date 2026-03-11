<template>
    <div>
        <BaseTable :columns="columns" :items="filteredClasses" :loading="formLoading" :showCheckboxColumn="true"
            :showPagination="true" :showActionsColumn="false" :showView="true" height="300px"
            @update:rows="val => selectedClasses = val" @view="viewClassEvent" @delete="handleDelete" :page="page"
            :pageSize="pageSize" :total="filteredClasses.length" :filter="filter" @update:filter="onTableFilter"
            @update:page="val => page = val" @update:pageSize="onPageSizeChange">
            <!-- TRẠNG THÁI -->
            <template #cell-classStatus="{ item }">
                <BaseBadge :type="statusColor(item.classStatus)" :label="statusLabel(item.classStatus)" />
            </template>

            <!-- HÌNH THỨC HỌC -->
            <template #cell-method="{ item }">
                <BaseBadge :type="item.method === 0 ? 'blue' : 'purple'"
                    :label="item.method === 0 ? t('class.onsite') : t('class.online')" bordered />
            </template>

            <!-- MÃ LỚP -->
            <template #cell-classCode="{ item }">
                <BaseBadge :label="item.classCode" color="blue" soft bold bordered />
            </template>

            <!-- TÊN LỚP -->
            <template #cell-className="{ item }">
                <span class="fw-semibold text-primary">{{ item.className }}</span>
            </template>

            <template #cell-studentCount="{ item }">
                <BaseBadge v-if="item.studentCount != null" :label="item.studentCount" color="cyan"
                    displayType="studentCountAlt" soft bordered />
                <span v-else class="text-muted">-</span>
            </template>

            <template #cell-teachingSchedule="{ item }">
                <div class="d-flex flex-column gap-1" style="white-space: normal;">
                    <template v-if="item.teachingScheduleItems?.length">
                        <span v-for="sched in item.teachingScheduleItems" :key="sched.id || sched.label">
                            {{ sched.label }}
                        </span>
                    </template>
                    <span v-else class="text-muted">{{ t('common.none') }}</span>
                </div>
            </template>

            <!-- CƠ SỞ -->
            <template #cell-companyName="{ item }">
                <BaseBadge :label="item.companyName" color="green" soft bordered />
            </template>
        </BaseTable>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useClassStore } from '@/stores/classStore'
import { useCourseStore } from '@/stores/courseStore'
import { useCompanyStore } from '@/stores/companyStore'
import { useWorkingTimeStore } from '@/stores/workingTimeStore'
import { useNotificationStore } from '@/stores/notificationStore'
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { formatDate } from '@/utils/format'
import type { ClassModel } from '@/api/ClassApi'
import type { WorkingTimeModel } from '@/api/WorkingTimeApi'
import { getDayOfWeekKey } from '@/types/daysOfWeek'
import { getClassMethodOptions, getClassStatusOptions } from '@/utils/makeList'

const props = defineProps<{ teacherId: string }>()
const { t } = useI18n()
const classStore = useClassStore()
const courseStore = useCourseStore()
const companyStore = useCompanyStore()
const workingTimeStore = useWorkingTimeStore()
const notificationStore = useNotificationStore()
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(8000)

const SPLIT_TAG = import.meta.env.VITE_SPLIT_TAG || '#$#'

const filter = ref<Record<string, any>>({})
const filterComponentRef = ref()
const selectedClasses = ref<Array<ClassModel>>([])
const page = ref(1)
const pageSize = ref(30)

function resolveStudentCount(item: ClassModel & Record<string, any>) {
    const enrollments = (item as any).enrollments
    if (Array.isArray(enrollments)) return enrollments.length
    const students = (item as any).students
    if (Array.isArray(students)) return students.length
    const count = (item as any).studentCount ?? (item as any).totalStudents
    const num = Number(count)
    return Number.isFinite(num) ? num : null
}

const workingTimesById = computed<Record<string, WorkingTimeModel>>(() => {
    const map: Record<string, WorkingTimeModel> = {}
    workingTimeStore.workingTimes.forEach((wt) => {
        if (wt.id) map[String(wt.id)] = wt
    })
    return map
})

function formatTime(value?: string | null) {
    return value ? value.substring(0, 5) : ''
}

function buildScheduleItems(classSchedule?: string | null) {
    if (!classSchedule) return []
    const ids = String(classSchedule)
        .split(SPLIT_TAG)
        .map((id) => id.trim())
        .filter(Boolean)

    const items = ids
        .map((id) => workingTimesById.value[id])
        .filter((wt): wt is WorkingTimeModel => Boolean(wt))
        .map((wt) => ({
            id: wt.id,
            dayOfWeek: wt.dayOfWeek,
            startTime: wt.startTime,
            endTime: wt.endTime,
            label: `${t(getDayOfWeekKey(wt.dayOfWeek))} ${formatTime(wt.startTime)}-${formatTime(wt.endTime)}`
        }))

    return items.sort((a, b) => {
        if (a.dayOfWeek !== b.dayOfWeek) return a.dayOfWeek - b.dayOfWeek
        return a.startTime.localeCompare(b.startTime)
    })
}

const classesWithDisplay = computed(() =>
    classStore.classes
        // .filter(c => c.teacherId === props.teacherId)
        .map(item => {
            const scheduleItems = buildScheduleItems(item.classSchedule)
            const studentCount = resolveStudentCount(item)
            return {
                ...item,
                courseName:
                    item.course?.courseName ||
                    courseStore.courses.find(c => c.id === item.courseId)?.courseName ||
                    t('common.none'),
                companyName:
                    item.company?.companyName ||
                    companyStore.companies.find(c => c.id === item.companyId)?.companyName ||
                    t('common.none'),
                studentCount,
                teachingScheduleItems: scheduleItems,
                teachingSchedule: scheduleItems.map((sched) => sched.label).join(', ') || t('common.none'),
            }
        })
)

/** ===== Filter Options ===== */
const companyFilterOptions = ref<{ label: string; value: any }[]>([]);

watch(
    () => companyStore.companies,
    (companies) => {
        companyFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...companies.map(company => ({
                label: company.companyName,
                value: company.companyName,
                isLocale: false,
            })),
            {
                label: t('common.none'),
                value: t('common.none'),
                isLocale: false,
            }
        ];
    },
    { immediate: true }
);
const courseFilterOptions = ref<{ label: string; value: any }[]>([]);
watch(
    () => courseStore.courses,
    (courses) => {
        courseFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...courses.map(course => ({
                label: course.courseName,
                value: course.courseName,
                isLocale: false,
            })),
            {
                label: t('common.none'),
                value: t('common.none'),
                isLocale: false,
            }
        ];
    },
    { immediate: true }
);


const methodFilterOptions = ref<{ label: string; value: any; isLocale?: boolean }[]>([]);

watch(
    () => getClassMethodOptions(t),
    () => {
        methodFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...getClassMethodOptions(t).map(option => ({
                label: option.label,
                value: option.value,
                isLocale: false,
            })),
        ]
    },
    { immediate: true }
)
const statusFilterOptions = ref<{ label: string; value: any; isLocale?: boolean }[]>([]);

watch(
    () => getClassStatusOptions(t),
    () => {
        statusFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...getClassStatusOptions(t).map(option => ({
                label: option.label,
                value: option.value,
                isLocale: false,
            })),
        ]
    },
    { immediate: true }
)




/** ===== Cột bảng ===== */
const columns = computed<BaseTableColumn[]>(() => [
    { key: 'classCode', labelKey: 'class.code', filterType: 'text', sortable: true, sticky: true, width: 180, isBold: true, align: 'center' },
    { key: 'className', labelKey: 'class.name', filterType: 'text', sortable: true, width: 200, isBold: true },
    {
        key: 'companyName',
        labelKey: 'class.company',
        filterType: 'select',
        sortable: true,
        align: 'center',
        width: 180,
        filterOptions: companyFilterOptions.value
    },
    {
        key: 'courseName',
        labelKey: 'class.course',
        filterType: 'select',
        sortable: true,
        align: 'center',
        width: 180,
        filterOptions: courseFilterOptions.value
    },
    {
        key: 'studentCount',
        labelKey: 'class.studentCount',
        filterType: 'text',
        sortable: true,
        width: 160,
        align: 'center'
    },
    {
        key: 'teachingSchedule',
        labelKey: 'teacher.teachingSchedule',
        filterType: 'text',
        sortable: false,
        minWidth: 220
    },
    {
        key: 'classStatus',
        labelKey: 'class.status',
        filterType: 'select',
        sortable: true,
        width: 160,
        align: 'center',
        filterOptions: statusFilterOptions.value,
        formatter: (value: number) => statusLabel(value)
    },

    {
        key: 'method',
        labelKey: 'class.method',
        filterType: 'select',
        sortable: true,
        width: 150,
        align: 'center',
        filterOptions: methodFilterOptions.value,
        formatter: (value: number) =>
            value === 0 ? t('class.onsite') : t('class.online')
    },
    {
        key: 'trialClass',
        labelKey: 'class.trialClass',
        filterType: 'select',
        sortable: true,
        width: 150,
        align: 'center',
        filterOptions: [
            { label: t('common.all'), value: '', isLocale: false },
            { label: t('common.yes'), value: true, isLocale: false },
            { label: t('common.no'), value: false, isLocale: false },
        ],
        formatter: (value: boolean) => (value ? t('common.yes') : t('common.no'))
    },
    {
        key: 'startDate',
        labelKey: 'class.startDate',
        filterType: 'date',
        sortable: true,
        width: 200,
        align: 'center',
        formatter: (v: string) => formatDate(v, 'DD/MM/YYYY')
    },
    {
        key: 'endDate',
        labelKey: 'class.endDate',
        filterType: 'date',
        sortable: true,
        width: 200,
        align: 'center',
        formatter: (v: string) => formatDate(v, 'DD/MM/YYYY')
    },
    { key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 180, align: 'center', formatter: (v: string) => formatDate(v, 'DD/MM/YYYY') },
    { key: 'actions', labelKey: 'common.actions', width: 180, align: 'center' },
])

/** ===== Filter + Helpers ===== */
const filteredClasses = computed(() => {
    let arr = classesWithDisplay.value
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            arr = arr.filter(item =>
                String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase())
            )
        }
    })
    return arr
})

const totalTrialClasses = computed(() => filteredClasses.value.filter(x => x.trialClass).length)

function onTableFilter(val: Record<string, any>) {
    filter.value = val
    page.value = 1
}


async function handleDelete(classes: ClassModel | ClassModel[]) {
    const list = Array.isArray(classes) ? classes : [classes]
    const ids = list.map(c => c.id as string)
    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.class') } },
        async () => {
            startLoading()
            try {
                await classStore.deleteClasses(ids)
                await classStore.fetchAllClasses()
                stopLoading()
            } catch (err: any) {
                console.error('Error deleting classes:', err)
                stopLoading()
            }
        }
    )
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize
    page.value = 1
}

function statusLabel(status: number) {
    switch (status) {
        case 0: return t('class.plan')
        case 1: return t('class.active')
        case 2: return t('class.finished')
        default: return t('common.unknown')
    }
}

function statusColor(status: number) {
    switch (status) {
        case 0: return 'info'
        case 1: return 'success'
        case 2: return 'danger'
        default: return 'gray'
    }
}

function viewClassEvent(item: ClassModel) {
    console.log('View class detail:', item)
    // anh có thể mở dialog hoặc router.push(`/class/detail/${item.id}`)
}

/** ===== LIFECYCLE ===== */
onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'delete', label: 'class.delete', type: 'delete' }
        ]
    })
    await Promise.all([
        classStore.fetchAllClassesByTeacherId(props.teacherId),
        courseStore.fetchAllCourses(),
        companyStore.fetchAllCompanies(),
        workingTimeStore.fetchAllWorkingTimes()
    ])
})
</script>
