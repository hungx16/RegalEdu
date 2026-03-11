<template>
    <div>
        <!-- FILTER HEADER -->
        <FilterComponent ref="filterComponentRef" :headerTitle="'class.headerTitle'" :headerDesc="'class.headerDesc'"
            @add="addClassEvent" @delete="onDeleteClicked" :disabledDelete="selectedClasses.length === 0"
            class="mb-6" />

        <!-- SUMMARY CARDS -->
        <div class="row g-4 mb-8">
            <div class="col-12 col-md-6">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('class.totalClasses') }}</span>
                        <i class="bi bi-easel2 fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ classStore.classes.length }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>

            <div class="col-12 col-md-6">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('class.totalTrial') }}</span>
                        <i class="bi bi-person-workspace fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalTrialClasses }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('class.trialClassCount') }}</div>
                </div>
            </div>
        </div>

        <!-- MAIN TABLE -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('class.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('class.listFunction') }}</span>
                </div>
            </div>

            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :columns="columns" :items="filteredClasses" :loading="formLoading" :showCheckboxColumn="true"
                    :showActionsColumn="true" :showEdit="employeeStore.isAcademicAffairsEmployee"
                    :disable-row-dbl-click="!employeeStore.isAcademicAffairsEmployee"
                    :showDelete="employeeStore.isAcademicAffairsEmployee" :showView="true" :showPagination="true"
                    @update:rows="val => selectedClasses = val" @edit="editClassEvent" @view="viewClassEvent"
                    @delete="handleDelete" :page="page" :pageSize="pageSize" :total="filteredClasses.length"
                    :filter="filter" @update:filter="onTableFilter" @update:page="val => page = val"
                    @update:pageSize="onPageSizeChange">
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
                    <!-- KHÓA HỌC -->
                    <template #cell-courseName="{ item }">
                        {{ item.courseName }}
                    </template>

                    <!-- GIÁO VIÊN -->
                    <template #cell-teacherName="{ item }">
                        {{ item.teacherName }}
                    </template>

                    <!-- CƠ SỞ -->
                    <template #cell-companyName="{ item }">
                        <BaseBadge :label="item.companyName" color="green" soft bordered />
                    </template>

                </BaseTable>
            </div>
        </div>

        <!-- DIALOG -->
        <ClassDialog v-model:visible="showFormModal" :mode="dialogMode" :class-data="classStore.selectedClass"
            @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useClassStore } from '@/stores/classStore'
import { useNotificationStore } from '@/stores/notificationStore'
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout'
import FilterComponent from '@/components/filter-component/FilterComponent.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import ClassDialog from '@/views/class/ClassDialog.vue'
import type { ClassModel } from '@/api/ClassApi'
import { formatDate } from '@/utils/format'
import { useCourseStore } from '@/stores/courseStore'
import { useTeacherStore } from '@/stores/teacherStore'
import { useCompanyStore } from '@/stores/companyStore'
import { useEmployeeStore } from '@/stores/employeeStore'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import { getClassMethodOptions, getClassStatusOptions } from '@/utils/makeList'
const { t } = useI18n()
const classStore = useClassStore()
const notificationStore = useNotificationStore()
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
const courseStore = useCourseStore()
const teacherStore = useTeacherStore()
const companyStore = useCompanyStore()
const employeeStore = useEmployeeStore()
const showFormModal = ref(false)
const dialogMode = ref<'create' | 'edit' | 'view'>('create')
const selectedClasses = ref<Array<ClassModel>>([])
const page = ref(1)
const pageSize = ref(30)
const filter = ref<Record<string, any>>({})
const filterComponentRef = ref()
const isAcademicAffairs = ref(false);

const classesWithDisplay = computed(() =>
    classStore.classes.map(item => ({
        ...item,
        courseName:
            item.course?.courseName ||
            courseStore.courses.find(c => c.id === item.courseId)?.courseName ||
            t('common.none'),

        teacherName:
            item.teacher?.teacherName ||
            teacherStore.teachers.find(t => t.id === item.teacherId)?.applicationUser.fullName ||
            t('common.none'),

        companyName:
            item.company?.companyName ||
            companyStore.companies.find(c => c.id === item.companyId)?.companyName ||
            t('common.none'),
    }))
)

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

const teacherFilterOptions = ref<{ label: string; value: any }[]>([]);
watch(
    () => teacherStore.teachers,
    (teachers) => {
        teacherFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...teachers.map(teacher => ({
                label: teacher.applicationUser.fullName,
                value: teacher.applicationUser.fullName,
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



// 🔹 CẤU HÌNH CỘT BASETABLE
const columns = computed<BaseTableColumn[]>(() => [
    { key: 'classCode', labelKey: 'class.code', filterType: 'text', sortable: true, sticky: true, width: 200, isBold: true, align: 'center' },
    { key: 'className', labelKey: 'class.name', filterType: 'text', sortable: true, sticky: true, width: 200, isBold: true },
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
        key: 'teacherName',
        labelKey: 'class.teacher',
        filterType: 'select',
        sortable: true,
        align: 'center',
        width: 180,
        filterOptions: teacherFilterOptions.value
    },
    {
        key: 'method',
        labelKey: 'class.method',
        filterType: 'select',
        sortable: true,
        width: 150,
        align: 'center',
        filterOptions: methodFilterOptions.value,
        // formatter: (value: number) =>
        //     value === 0 ? t('class.onsite') : t('class.online')
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
        key: 'classStatus',
        labelKey: 'class.status',
        filterType: 'select',
        sortable: true,
        width: 160,
        align: 'center',
        filterOptions: statusFilterOptions.value,
        // formatter: (value: number) => {
        //     switch (value) {
        //         case 0: return t('class.plan')
        //         case 1: return t('class.active')
        //         case 2: return t('class.finished')
        //         default: return t('common.unknown')
        //     }
        // }
    },
    {
        key: 'startDate',
        labelKey: 'class.startDate',
        filterType: 'date',
        sortable: true,
        width: 200,
        align: 'center',
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    {
        key: 'endDate',
        labelKey: 'class.endDate',
        filterType: 'date',
        sortable: true,
        width: 200,
        align: 'center',
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 200 },
    { key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 200, align: 'center', formatter: (value: string) => formatDate(value, 'DD/MM/YYYY') },
    { key: 'actions', labelKey: 'common.actions', width: 200 }
])


/** ===== COMPUTED ===== */
const totalTrialClasses = computed(() => classStore.classes.filter(x => x.trialClass).length)

const filteredClasses = computed(() => {
    let arr = classesWithDisplay.value
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val) {
            arr = arr.filter(item => String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase()))
        }
    })
    return arr
})

// /** ===== HELPERS ===== */
function statusLabel(status: number) {
    switch (status) {
        case 0: return t('classStatus.plan')
        case 1: return t('classStatus.inProgress')
        case 2: return t('classStatus.completed')
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

/** ===== ACTIONS ===== */
function onTableFilter(val: Record<string, any>) {
    filter.value = val
    page.value = 1
}

async function addClassEvent() {
    dialogMode.value = 'create'
    classStore.selectClass(null)
    showFormModal.value = true
}

function editClassEvent(item: ClassModel) {
    dialogMode.value = 'edit'
    classStore.selectClass({ ...item })
    showFormModal.value = true
}

function viewClassEvent(item: ClassModel) {
    console.log('abc');

    dialogMode.value = 'view'
    classStore.selectClass({ ...item })
    showFormModal.value = true
}

function onDeleteClicked() {
    if (selectedClasses.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.class') } })
        return
    }
    handleDelete(selectedClasses.value)
}

async function handleSave(classData: any) {
    startLoading()
    try {
        await classStore.saveClass(classData)
        notificationStore.showToast('success', {
            key: classData.id ? 'toast.updateSuccess' : 'toast.createSuccess',
            params: { model: t('models.class') }
        })
        await classStore.fetchAllClasses()
        stopLoading()
        showFormModal.value = false
    } catch (err: any) {
        console.error('Error saving class:', err)
        stopLoading()
    }
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

/** ===== LIFECYCLE ===== */
onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'class.add', type: 'add' },
            { event: 'delete', label: 'class.delete', type: 'delete' }
        ]
    })
    await Promise.all([
        classStore.fetchAllClasses(),
        courseStore.fetchAllCourses(),
        teacherStore.fetchAllTeacher(),
        companyStore.fetchAllCompanies(),
        teacherStore.checkIsCurrentUserTeacher(),
        employeeStore.checkIsAcademicAffairsEmployee(),
    ])
})
</script>
