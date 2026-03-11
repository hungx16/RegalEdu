<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addRoadmap" @delete="onDeleteClicked"
            headerTitle="learningRoadMap.headerTitle" headerDesc="learningRoadMap.headerDesc"
            :disabledDelete="getDisableDelete" class="mb-6" />
        <!-- ROW cho 2 CARD -->
        <div class="row g-4 mb-8">
            <div class="col-12 col-md-4">
                <div class="summary-card bg-body  text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('learningRoadMap.totalPrograms') }}</span>
                        <i class="bi bi-book fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalPrograms }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.all') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-4">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('common.active') }}</span>
                        <i class="bi bi-bullseye fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ activeLearningRoadMaps.length }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('models.learningRoadMap') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-4">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('learningRoadMap.totalCourses') }}</span>
                        <i class="bi bi-collection fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalCourses }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.activeOverTotal') }}</div>
                </div>
            </div>
        </div>
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('learningRoadMap.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('learningRoadMap.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredLearningRoadMapsAll"
                    :loading="formLoading" :showPagination="true" :page="page"
                    :total="filteredLearningRoadMapsAll.length" :pageSize="pageSize" :filter="filter"
                    @update:filter="onTableFilter" @update:rows="val => selectedRows = val" @edit="editRoadmap"
                    @view="viewRoadmap" :showActionsColumn="true" :showEdit="true" :showDelete="true" :showView="true"
                    @delete="handleDelete" @update:page="val => page = val" @update:pageSize="onPageSizeChange">
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-commitmentOutput="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.commitmentOutput ? t('learningRoadMap.commitmentOutput') : t('learningRoadMap.notCommitmentOutput')" />
                    </template>
                </BaseTable>
            </div>
        </div>
        <LearningRoadMapDialog v-model:visible="showDialog" :mode="dialogMode"
            :learning-road-map-data="learningRoadMapStore.selectedLearningRoadMap" :loading="formLoading"
            @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useLearningRoadMapStore } from '@/stores/learningRoadMapStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import LearningRoadMapDialog from '@/views/learning-road-map/LearningRoadMapDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { formatDate } from '@/utils/format';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { StatusType } from '@/types';
const { t } = useI18n();
const learningRoadMapStore = useLearningRoadMapStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);

const showDialog = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRows = ref([]);

const columns: BaseTableColumn[] = [
    { key: 'learningRoadMapCode', labelKey: 'learningRoadMap.learningRoadMapCode', filterType: 'text', sortable: true },
    { key: 'learningRoadMapName', labelKey: 'learningRoadMap.learningRoadMapName', filterType: 'text', sortable: true },
    {
        key: 'commitmentOutput', labelKey: 'learningRoadMap.commitmentOutput', filterType: 'select', sortable: false, filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'learningRoadMap.commitmentOutput', value: true },
            { label: 'learningRoadMap.notCommitmentOutput', value: false },
        ]
    },
    { key: 'ageGroup', labelKey: 'learningRoadMap.ageGroup', filterType: 'text', sortable: false },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 250 },
    {
        key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 250,
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    { key: 'actions', labelKey: 'common.actions', width: 220 },
];

const getDisableDelete = computed(() => selectedRows.value.length === 0);
// Tính số khoá học cho từng roadmap + chuẩn hoá vài field hiển thị
const learningRoadMapsWithCount = computed(() =>
    learningRoadMapStore.learningRoadMaps.map(item => ({
        ...item,
        // Ưu tiên server trả totalCourses; nếu có mảng courses thì lấy length; không có thì 0
        courseCount: (item as any).totalCourses ?? (item as any).courses?.length ?? 0,
        // Hiển thị tên nhóm tuổi thống nhất cho cột/filter text
        ageGroup: item.ageGroup.categoryName ?? '',
    }))
)
const totalPrograms = computed(() => learningRoadMapStore.learningRoadMaps.length)
const totalCourses = computed(() =>
    learningRoadMapsWithCount.value.reduce((sum, item) => {
        const count = Number((item as any).courseCount ?? 0)
        return sum + (Number.isFinite(count) ? count : 0)
    }, 0)
)
const activeLearningRoadMaps = computed(() =>
    learningRoadMapStore.learningRoadMaps.filter(item => item.status === StatusType.Active)
)
// Lọc data theo filter (tương tự Division)
const filteredLearningRoadMapsAll = computed(() => {
    let arr = learningRoadMapsWithCount.value

    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            if (key === 'createdAt') {
                arr = arr.filter(item => {
                    if (!item[key as keyof typeof item]) return false
                    const dateOnly = String(item[key as keyof typeof item]).substring(0, 10)
                    return dateOnly === String(val)
                })
            } else if (key === 'status') {
                // status thường là số hoặc enum -> so sánh bằng
                const target = Number(val)
                arr = arr.filter(item => Number((item as any).status ?? '') === target)
            } else if (key === 'commitmentOutput') {
                // select true/false: val có thể là '0' | '1' hoặc true/false
                const target = String(val) === '1' || String(val).toLowerCase() === 'true'
                arr = arr.filter(item => Boolean((item as any).commitmentOutput) === target)
            } else {
                // text filter chung
                arr = arr.filter(item =>
                    String((item as any)[key] ?? '')
                        .toLowerCase()
                        .includes(String(val).toLowerCase())
                )
            }
        }
    })

    return arr
})

function onDeleteClicked() {
    if (!selectedRows.value || selectedRows.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.learningRoadMap') } });
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
        { key: 'toast.delete', params: { model: t('models.learningRoadMap') } },
        async () => {
            await learningRoadMapStore.deleteLearningRoadMaps(ids);
            notificationStore.showToast('success', { key: 'toast.deleteSuccess', params: { model: t('models.learningRoadMap') } });
            learningRoadMapStore.fetchAllLearningRoadMaps();
        }
    );
}

async function addRoadmap() {
    dialogMode.value = 'create';
    learningRoadMapStore.selectLearningRoadMap(null);
    showDialog.value = true;
}

function editRoadmap(item: any) {
    dialogMode.value = 'edit';
    learningRoadMapStore.selectLearningRoadMap({ ...item });
    showDialog.value = true;
}

function viewRoadmap(item: any) {
    dialogMode.value = 'view';
    learningRoadMapStore.selectLearningRoadMap({ ...item });
    showDialog.value = true;
}

async function handleSave(item: any) {
    startLoading();
    try {
        await learningRoadMapStore.saveLearningRoadMap(item);
        notificationStore.showToast('success', { key: item.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.learningRoadMap') } });
        learningRoadMapStore.fetchAllLearningRoadMaps();
        showDialog.value = false;
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
        stopLoading()
    } finally {
        stopLoading();
    }
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}

onMounted(() => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'learningRoadMap.add', type: 'add' },
            { event: 'delete', label: 'learningRoadMap.delete', type: 'delete' },
        ],
    });
    learningRoadMapStore.fetchAllLearningRoadMaps();
});
</script>
