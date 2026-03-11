<template>
    <div>
        <!-- FilterComponent (đặt ngoài row/card) -->
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="ageGroup.headerTitle" headerDesc="ageGroup.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <!-- ROW cho 4 CARD thống kê -->
        <div class="row g-4 mb-8">
            <div class="col-12 col-md-3">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('ageGroup.totalAgeGroups') }}</span>
                        <i class="bi bi-person-check fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ ageGroupStore.total }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-3">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('ageGroup.totalPrograms') }}</span>
                        <i class="bi bi-window-stack fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalPrograms }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('ageGroup.designedPrograms') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-3">
                <div class="summary-card bg-light text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('ageGroup.totalStudents') }}</span>
                        <i class="bi bi-person-circle fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalStudents }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('ageGroup.studentsPerGroup') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-3">
                <div class="summary-card bg-light text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('common.average') }}</span>
                        <i class="bi bi-distribute-vertical fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalStudents }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('ageGroup.studentsPerGroup') }}</div>
                </div>
            </div>
        </div>

        <!-- BẢNG DỮ LIỆU -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('ageGroup.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('ageGroup.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredAgeGroupsAll"
                    :loading="ageGroupStore.loading" :showPagination="true" :page="page"
                    :total="filteredAgeGroupsAll.length" :pageSize="pageSize" :filter="filter"
                    @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val" @edit="editModelEvent"
                    @view="viewModelEvent" :showActionsColumn="true" :showEdit="true" :showDelete="true"
                    :showView="true" @delete="handleDelete" @update:page="val => page = val"
                    @update:pageSize="onPageSizeChange">
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-ageRange="{ item }">
                        <BaseBadge :label="item.ageRange || t('common.none')" soft :bold="true" bordered
                            :rawLabel="true" />
                    </template>
                </BaseTable>
            </div>
        </div>

        <!-- Dialog Form -->
        <AgeGroupDialog v-model:visible="showFormModal" :mode="dialogMode"
            :age-group-data="ageGroupStore.selectedAgeGroup" :loading="formLoading" @submit="handleSave"
            @delete="handleDelete" />
    </div>
</template>
<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useAgeGroupStore } from '@/stores/ageGroupStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import AgeGroupDialog from '@/views/age-group/AgeGroupDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { useCommonStore } from '@/stores/commonStore';
import { formatDate } from '@/utils/format';
const commonStore = useCommonStore();
const { t } = useI18n();
const ageGroupStore = useAgeGroupStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);

const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRowsData = ref([]);

// Cột cho bảng
const columns: BaseTableColumn[] = [
    { key: 'categoryCode', labelKey: 'ageGroup.code', filterType: 'text', sortable: true },
    { key: 'categoryName', labelKey: 'ageGroup.name', filterType: 'text', sortable: true },
    { key: 'ageRange', labelKey: 'ageGroup.ageRange', filterType: 'text', sortable: true },
    { key: 'description', labelKey: 'ageGroup.description', filterType: 'text', sortable: false },
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
    { key: 'actions', labelKey: 'common.actions', width: 160 }
];

// Tính toán số học viên và chương trình cho từng nhóm tuổi
const ageGroupsWithStats = computed(() => {
    return ageGroupStore.ageGroups.map(item => ({
        ...item,
        programCount: 0,  // Số lượng chương trình
        studentCount: 0,  // Số lượng học viên
        ageRange: `${item.from} - ${item.to} ${t('ageGroup.age')}` // Hiển thị khoảng tuổi
    }));
});

// Lọc dữ liệu theo filter
const filteredAgeGroupsAll = computed(() => {
    let arr = ageGroupsWithStats.value;
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

// Tính tổng số chương trình
const totalPrograms = computed(() => {
    return ageGroupsWithStats.value.reduce((acc, item) => acc + item.programCount, 0);
});

// Tính tổng số học viên
const totalStudents = computed(() => {
    return ageGroupsWithStats.value.reduce((acc, item) => acc + item.studentCount, 0);
});

// Disable delete nếu không có item nào được chọn
const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.ageGroup') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;  // Reset trang khi thay đổi filter
}

function handleDelete(items: any | any[]) {
    const list = Array.isArray(items) ? items : [items];
    const ids = list.map(item => item.id).filter(Boolean);
    notificationStore.showConfirm(
        {
            key: 'toast.delete',
            params: { model: t('models.ageGroup') },
        },
        async () => {
            await ageGroupStore.deleteAgeGroups(ids);
            notificationStore.showToast('success', { key: 'toast.deleteSuccess', params: { model: t('models.ageGroup') } });
            ageGroupStore.fetchAllAgeGroups();
        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    await commonStore.generateCode('AGE', 'Category', 'CategoryCode', 4);
    ageGroupStore.selectAgeGroup(null);
    showFormModal.value = true;
}

function editModelEvent(item: any) {
    dialogMode.value = 'edit';
    ageGroupStore.selectAgeGroup({ ...item });
    showFormModal.value = true;
}

function viewModelEvent(item: any) {
    dialogMode.value = 'view';
    ageGroupStore.selectAgeGroup({ ...item });
    showFormModal.value = true;
}

async function handleSave(item: any) {
    startLoading();
    try {
        await ageGroupStore.saveAgeGroup(item);
        notificationStore.showToast('success', { key: item.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.ageGroup') } });
        ageGroupStore.fetchAllAgeGroups();
        showFormModal.value = false;
    } catch (err) {
        notificationStore.showToast('error', { key: 'toast.saveError', params: { model: t('models.ageGroup') } });
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
            { event: 'add', label: 'ageGroup.add', type: 'add' },
            { event: 'delete', label: 'ageGroup.delete', type: 'delete' },
        ]
    });
    ageGroupStore.fetchAllAgeGroups();
});
</script>
