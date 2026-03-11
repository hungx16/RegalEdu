<template>
    <div>
        <!-- FilterComponent (đặt ngoài row/card) -->
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="skill.headerTitle" headerDesc="skill.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <!-- ROW cho 4 CARD thống kê -->
        <div class="row g-4 mb-8">
            <div class="col-12 col-md-6">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('skill.totalSkills') }}</span>
                        <i class="bi bi-ui-checks-grid fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ skillStore.total }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.allStatus') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-6">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('skill.totalSkills') }}</span>
                        <i class="bi bi-check2-circle fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ numberOfActiveSkills }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>
        </div>

        <!-- BẢNG DỮ LIỆU -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('skill.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('skill.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredSkillsAll"
                    :loading="skillStore.loading" :showPagination="true" :page="page" :total="filteredSkillsAll.length"
                    :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
                    @update:rows="val => selectedRowsData = val" @edit="editModelEvent" @view="viewModelEvent"
                    :showActionsColumn="true" :showEdit="true" :showDelete="true" :showView="true"
                    @delete="handleDelete" @update:page="val => page = val" @update:pageSize="onPageSizeChange">
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
        <SkillDialog v-model:visible="showFormModal" :mode="dialogMode" :skill-data="skillStore.selectedSkill"
            :loading="formLoading" @submit="handleSave" @delete="handleDelete" />
    </div>
</template>
<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useSkillStore } from '@/stores/skillStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import SkillDialog from '@/views/skill/SkillDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { formatDate } from '@/utils/format';
import { StatusType } from '@/types';
const { t } = useI18n();
const skillStore = useSkillStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(1000);

const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRowsData = ref([]);
const numberOfActiveSkills = computed(() => skillStore.skills.filter(item => item.status === StatusType.Active).length);
// Cột cho bảng
const columns: BaseTableColumn[] = [
    { key: 'categoryCode', labelKey: 'skill.code', filterType: 'text', sortable: true },
    { key: 'categoryName', labelKey: 'skill.name', filterType: 'text', sortable: true },
    { key: 'description', labelKey: 'skill.description', filterType: 'text', sortable: false },
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
const skillsWithStats = computed(() => {
    return skillStore.skills.map(item => ({
        ...item,
        programCount: 0,  // Số lượng chương trình
        studentCount: 0,  // Số lượng học viên
    }));
});

// Lọc dữ liệu theo filter
const filteredSkillsAll = computed(() => {
    let arr = skillsWithStats.value;
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
    return skillsWithStats.value.reduce((acc, item) => acc + item.programCount, 0);
});

// Tính tổng số học viên
const totalStudents = computed(() => {
    return skillsWithStats.value.reduce((acc, item) => acc + item.studentCount, 0);
});

// Disable delete nếu không có item nào được chọn
const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.skill') } });
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
            params: { model: t('models.skill') },
        },
        async () => {
            await skillStore.deleteSkills(ids);
            notificationStore.showToast('success', { key: 'toast.deleteSuccess', params: { model: t('models.skill') } });
            skillStore.fetchAllSkills();
        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    // await commonStore.generateCode('CT', 'Category', 'CategoryCode', 4);
    skillStore.selectSkill(null);
    showFormModal.value = true;
}

function editModelEvent(item: any) {
    dialogMode.value = 'edit';
    skillStore.selectSkill({ ...item });
    showFormModal.value = true;
}

function viewModelEvent(item: any) {
    dialogMode.value = 'view';
    skillStore.selectSkill({ ...item });
    showFormModal.value = true;
}

async function handleSave(item: any) {

    startLoading()
    try {
        await skillStore.saveSkill(item);
        if (item.id) {
            notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.skill') } });
        } else {
            notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.skill') } });
        }
        await skillStore.fetchAllSkills();
        stopLoading() // Hoặc chỉ cần đóng dialog, watcher sẽ reset loading về false
        showFormModal.value = false;

    } catch (err: any) {
        stopLoading()
        showFormModal.value = false;
        console.error('Error saving:', err?.response?.data?.errors || err);

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
            { event: 'add', label: 'skill.add', type: 'add' },
            { event: 'delete', label: 'skill.delete', type: 'delete' },
        ]
    });
    skillStore.fetchAllSkills();
});
</script>
