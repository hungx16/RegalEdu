<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addCourse" @delete="onDeleteClicked"
            headerTitle="course.headerTitle" headerDesc="course.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <div class="row g-4 mb-8">
            <div class="col-12 col-md-4">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('course.totalCourses') }}</span>
                        <i class="bi bi-book fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ store.courses.length }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.allStatus') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-4">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('common.active') }}</span>
                        <i class="bi bi-geo-alt fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{store.courses.filter(course => course.status ===
                        StatusType.Active).length}}</div>
                    <div class="fs-7 text-body-secondary">{{ t('models.Course') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-4">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('common.TotalStudents') }}</span>
                        <i class="bi bi-people fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{store.courses.reduce((total, course) => total +
                        (course.numberOfStudents ?? 0), 0)}}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.inStudying') }}</div>
                </div>
            </div>
        </div>
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('course.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('course.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredCoursesAll"
                    :loading="store.loading" :showPagination="true" :page="page" :total="filteredCoursesAll.length"
                    :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
                    @update:rows="val => selectedRows = val" @edit="editCourse" @view="viewCourse"
                    :showActionsColumn="true" :showEdit="true" :showDelete="true" :showView="true"
                    @delete="handleDelete" @update:page="val => page = val" @update:pageSize="onPageSizeChange">
                    <template #cell-courseCode="{ item }">
                        <BaseBadge :label="item.courseCode" color="blue" soft :bold="true" bordered />
                    </template>
                    <template #cell-status="{ item }">
                    <BaseBadge type="boolean"
                        :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-currentLearningRoadMap="{ item }">
                        {{ item.learningRoadMap?.learningRoadMapName || '' }}
                    </template>
                    <template #cell-ordinalNumber="{ item }">
                        {{ formatOrdinalNumber(item.ordinalNumber) }}
                    </template>
                </BaseTable>
            </div>
        </div>
        <CourseDialog v-model:visible="showDialog" :mode="dialogMode" :course-data="store.selectedCourse"
            :loading="formLoading" @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, onBeforeMount } from 'vue';
import { useI18n } from 'vue-i18n';
import { useCourseStore } from '@/stores/courseStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import CourseDialog from './CourseDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { StatusType } from '@/types';
import { useLearningRoadMapStore } from '@/stores/learningRoadMapStore';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
const learningRoadMapStore = useLearningRoadMapStore();
const { t } = useI18n();
const store = useCourseStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);

const showDialog = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRows = ref<any[]>([]);
const learningRoadMapFilterOptions = ref<{ label: string; value: any; isLocale?: boolean }[]>([]);

const columns = computed<BaseTableColumn[]>(() => [
    { key: 'courseCode', labelKey: 'course.code', filterType: 'text', sortable: true },
    { key: 'courseName', labelKey: 'course.name', filterType: 'text', sortable: true },
    {
        key: 'currentLearningRoadMap',
        labelKey: 'course.learningRoadMap',
        filterType: 'select',
        sortable: false,
        filterOptions: learningRoadMapFilterOptions.value,
        width: 260
    },
    { key: 'sequence', labelKey: 'course.sequence', filterType: 'text', sortable: true, width: 160 },
    { key: 'ordinalNumber', labelKey: 'course.ordinalNumber', filterType: 'text', sortable: true, width: 160, align: 'center' },
    { key: 'duration', labelKey: 'course.duration', filterType: 'text', sortable: true, width: 140 },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: t('common.all'), value: '' },
            { label: t('common.active'), value: 0 },
            { label: t('common.inactive'), value: 1 },
        ]
    },
    { key: 'actions', labelKey: 'common.actions', width: 160 },
])

const getDisableDelete = computed(() => selectedRows.value.length === 0);
const coursesWithCount = computed(() =>
    store.courses.map(item => ({
        ...item,
        currentLearningRoadMap: learningRoadMapStore.learningRoadMaps.find(lr => lr.id === item.learningRoadMapId)?.learningRoadMapName ?? t('common.none'),
    }))
);
watch(
    () => learningRoadMapStore.learningRoadMaps,
    (learningRoadMaps) => {
        learningRoadMapFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...learningRoadMaps.map(learningRoadMap => ({
                label: learningRoadMap.learningRoadMapName,
                value: learningRoadMap.learningRoadMapName,
                isLocale: false,
            }))
        ];
    },
    { immediate: true }
);

// Lọc data theo filter
const filteredCoursesAll = computed(() => {
    let arr = coursesWithCount.value;
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            if (key === 'createdAt') {
                arr = arr.filter(item => {
                    if (!item[key]) return false;
                    const dateOnly = String(item[key]).substring(0, 10);
                    return dateOnly === val;
                });
            } else {
                arr = arr.filter(item =>
                    String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase())
                );
            }
        }
    });
    return arr;
});
function onDeleteClicked() {
    if (!selectedRows.value || selectedRows.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.course') } });
        return;
    }
    handleDelete(selectedRows.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function handleDelete(items: any | any[]) {
    const list = Array.isArray(items) ? items : [items];
    const ids = list.map(item => item.id).filter(Boolean);
    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.course') } },
        async () => {
            startLoading();
            try {
                await store.deleteCourses(ids);
                notificationStore.showToast('success', { key: 'toast.deleteSuccess', params: { model: t('models.course') } });
                store.fetchAllCourses();
                selectedRows.value = [];
            } catch (error) {
                console.log('Error deleting courses', error);
            } finally {
                stopLoading();
            }
            showDialog.value = false;

        }
    );
}

async function addCourse() {
    dialogMode.value = 'create';
    store.selectCourse(null);
    showDialog.value = true;
}

function editCourse(item: any) {
    dialogMode.value = 'edit';
    store.selectCourse({ ...item });
    showDialog.value = true;
}

function viewCourse(item: any) {
    dialogMode.value = 'view';
    store.selectCourse({ ...item });
    showDialog.value = true;
}

async function handleSave(item: any) {
    startLoading();
    try {
        await store.saveCourse(item);
        notificationStore.showToast('success', { key: item.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.course') } });
        store.fetchAllCourses();
        showDialog.value = false;
    } catch (error) {
        console.log('Error saving course', error);
    } finally {
        stopLoading();
    }
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}

function formatOrdinalNumber(val: any) {
    const num = Number(val);
    if (!Number.isFinite(num)) return '';
    return num.toLocaleString('en-US', {
        useGrouping: false,
        maximumFractionDigits: 3,
        minimumFractionDigits: 0
    });
}
onBeforeMount(async () => {
    await store.fetchAllCourses();
});
onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'course.add', type: 'add' },
            { event: 'delete', label: 'course.delete', type: 'delete' },
        ],
    });

    await learningRoadMapStore.fetchAllLearningRoadMaps();

});
</script>
