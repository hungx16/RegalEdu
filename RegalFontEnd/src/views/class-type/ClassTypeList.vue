<template>
    <div>
        <!-- FilterComponent -->
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="classType.headerTitle" headerDesc="classType.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <!-- 4 CARD thống kê -->
        <div class="row g-4 mb-8">
            <!-- Tổng loại lớp -->
            <div class="col-12 col-md-3">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('classType.totalClassTypes') }}</span>
                        <i class="bi bi-collection fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalClassTypes }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('classType.allTypes') }}</div>
                </div>
            </div>
            <!-- Đang sử dụng -->
            <div class="col-12 col-md-3">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('classType.activeTypes') }}</span>
                        <i class="bi bi-check2-circle fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ activeClassTypes }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('classType.activeTypesDesc') }}</div>
                </div>
            </div>
            <!-- Tổng lớp học -->
            <div class="col-12 col-md-3">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('classType.totalClasses') }}</span>
                        <i class="bi bi-people fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalClasses }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('classType.totalClassesDesc') }}</div>
                </div>
            </div>
            <!-- Trung bình sĩ số -->
            <div class="col-12 col-md-3">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('classType.avgStudentPerClass') }}</span>
                        <i class="bi bi-bar-chart-line fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ averageStudentsPerClass }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('classType.avgStudentPerClassDesc') }}</div>
                </div>
            </div>
        </div>


        <!-- TABLE DỮ LIỆU -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('classType.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('classType.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredClassTypesAll"
                    :loading="classTypeStore.loading" :showPagination="true" :page="page"
                    :total="filteredClassTypesAll.length" :pageSize="pageSize" :filter="filter"
                    @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val" @edit="editModelEvent"
                    @view="viewModelEvent" :showActionsColumn="true" :showEdit="true" :showDelete="true"
                    :showView="true" @delete="handleDelete" @update:page="val => page = val"
                    @update:pageSize="onPageSizeChange">
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-studentCountRange="{ item }">
                        <BaseBadge :label="item.studentCountRange || t('common.none')" soft :bold="true" bordered
                            :rawLabel="true" />
                    </template>
                    <template #cell-numberOfClasses="{ item }">
                        <BaseBadge :label="item.numberOfClasses || t('common.none')" soft :bold="true" bordered
                            :rawLabel="true" />
                    </template>
                </BaseTable>
            </div>
        </div>

        <!-- Dialog Form -->
        <ClassTypeDialog v-model:visible="showFormModal" :mode="dialogMode"
            :class-type-data="classTypeStore.selectedClassType" :loading="formLoading" @submit="handleSave"
            @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useClassTypeStore } from '@/stores/classTypeStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import ClassTypeDialog from '@/views/class-type/ClassTypeDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { useCommonStore } from '@/stores/commonStore';
import { formatDate } from '@/utils/format';
const commonStore = useCommonStore();
const { t } = useI18n();
const classTypeStore = useClassTypeStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);
const totalClassTypes = computed(() => classTypeStore.classTypes.length);
const activeClassTypes = computed(() => classTypeStore.classTypes.filter(x => x.status === 0).length);
const totalClasses = 0// computed(() => classTypeStore.classTypes.reduce((acc, x) => acc + (x.classCount || 0), 0));
// const totalStudents = 0// computed(() => classTypeStore.classTypes.reduce((acc, x) => acc + (x.studentCount || 0), 0));
// const averageStudentsPerClass = 0//computed(() =>
//     totalClasses.value > 0 ? Math.round(totalStudents.value / totalClasses.value) : 0
// );
const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRowsData = ref([]);

// Định nghĩa columns cho BaseTable
const columns: BaseTableColumn[] = [
    { key: 'classTypeCode', labelKey: 'classType.code', filterType: 'text', sortable: true, width: 180 },
    { key: 'classTypeName', labelKey: 'classType.name', filterType: 'text', sortable: true, width: 180 },
    { key: 'description', labelKey: 'classType.description', filterType: 'text', sortable: true },
    { key: 'sessionsPerWeek', labelKey: 'classType.sessionsPerWeek', filterType: 'text', sortable: true, width: 150 },
    { key: 'hoursPerSession', labelKey: 'classType.hoursPerSession', filterType: 'text', sortable: true, width: 150 },
    { key: 'numberOfClasses', labelKey: 'classType.numberOfClasses', filterType: 'text', sortable: true },
    { key: 'studentCountRange', labelKey: 'classType.studentCountRange', filterType: 'text', sortable: true },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },

    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 200 },
    {
        key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 200,
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    { key: 'actions', labelKey: 'common.actions', width: 160 }
];

// Xử lý thống kê, filter, CRUD
const classTypesWithStats = computed(() => {
    return classTypeStore.classTypes.map(item => ({
        ...item,
        studentCountRange: `${item.minStudents} - ${item.maxStudents} ${t('models.student')}`, // Hiển thị khoảng tuổi
        numberOfClasses: `0 ${t('models.class')}`, // Hiển thị số lớp
    }));
});

// Lọc theo filter
const filteredClassTypesAll = computed(() => {
    let arr = classTypesWithStats.value;
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            arr = arr.filter(item =>
                String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase())
            );
        }
    });
    return arr;
});

// Tổng số học viên của tất cả loại lớp
const totalStudents = computed(() => {
    return 0;// classTypesWithStats.value.reduce((acc, item) => acc + Number(item.studentCount || 0), 0);
});
// Số học viên trung bình trên mỗi loại lớp
const averageStudentsPerClass = computed(() => {
    if (!classTypesWithStats.value.length) return 0;
    return Math.round(totalStudents.value / classTypesWithStats.value.length);
});

// Disable delete nếu không chọn item
const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.classType') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function handleDelete(items: any | any[]) {
    const list = Array.isArray(items) ? items : [items];
    const ids = list.map(item => item.id).filter(Boolean);
    notificationStore.showConfirm(
        {
            key: 'toast.delete',
            params: { model: t('models.classType') },
        },
        async () => {
            startLoading();
            try {
                await classTypeStore.deleteClassTypes(ids);
                await classTypeStore.fetchAllClassTypes();
            } catch (error: any) {
                console.error('Error deleting:', error);
            } finally {
                stopLoading();
                showFormModal.value = false;
            }
        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    await commonStore.generateCode('CT', 'ClassType', 'ClassTypeCode', 4);
    classTypeStore.selectClassType(null);
    showFormModal.value = true;
}

function editModelEvent(item: any) {
    dialogMode.value = 'edit';
    classTypeStore.selectClassType({ ...item });
    showFormModal.value = true;
}

function viewModelEvent(item: any) {
    dialogMode.value = 'view';
    classTypeStore.selectClassType({ ...item });
    showFormModal.value = true;
}

async function handleSave(item: any) {
    startLoading();
    try {
        await classTypeStore.saveClassType(item);
        notificationStore.showToast('success', { key: item.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.classType') } });
        await classTypeStore.fetchAllClassTypes();
        showFormModal.value = false;
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
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
            { event: 'add', label: 'classType.add', type: 'add' },
            { event: 'delete', label: 'classType.delete', type: 'delete' },
        ]
    });
    classTypeStore.fetchAllClassTypes();
});
</script>
