<template>
    <div>
        <!-- FilterComponent -->
        <FilterComponent ref="filterComponentRef" @add="addModeEvent" @delete="onDeleteClicked"
            headerTitle="department.headerTitle" headerDesc="department.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <!-- Bảng dữ liệu -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('department.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('department.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredDepartments" :showIndex="true"
                    :loading="store.loading" :showPagination="true" :page="page" :total="filteredDepartments.length"
                    :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
                    @update:rows="val => selectedRowsData = val" :showView="true" @edit="editModelEvent"
                    :showActionsColumn="true" :showEdit="true" :showDelete="true" @delete="handleDelete"
                    @view="viewModelEvent" @update:page="val => page = val" @update:pageSize="onPageSizeChange">
                    <template #cell-currentDivision="{ item }">
                        <BaseBadge :label="item.currentDivision" color="lightYellow" :bold="true" :bordered="true" />
                    </template>
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-departmentCode="{ item }">
                        <BaseBadge :label="item.departmentCode" color="lightGreen" :bold="true" :bordered="true" />
                    </template>
                </BaseTable>
            </div>
        </div>

        <!-- Dialog -->
        <DepartmentDialog v-model:visible="showFormModal" :is-edit="!!store.selectedDepartment?.id" :mode="dialogMode"
            :loading="formLoading" :department-data="store.selectedDepartment" @submit="handleSave"
            @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useDepartmentStore } from '@/stores/departmentStore';
import { useNotificationStore } from '@/stores/notificationStore';
import { useCommonStore } from '@/stores/commonStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import DepartmentDialog from '@/views/department/DepartmentDialog.vue';
import { formatDate } from '@/utils/format';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import type { DepartmentModel } from '@/api/DepartmentApi';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { useDivisionStore } from '@/stores/divisionStore';

const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)

const { t } = useI18n();
const store = useDepartmentStore();
const notificationStore = useNotificationStore();
const commonStore = useCommonStore();
const showFormModal = ref(false);
const filterComponentRef = ref();
const divisionStore = useDivisionStore();
const divisionFilterOptions = ref<{ label: string; value: any }[]>([]);
watch(
    () => divisionStore.divisions,
    (divisions) => {
        divisionFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...divisions.map(division => ({
                label: division.divisionName,
                value: division.divisionName,
                isLocale: false,
            }))
        ];
    },
    { immediate: true }
);

let dialogMode = ref<'create' | 'edit' | 'view'>('create');

const columns = computed<BaseTableColumn[]>(() => [
    { key: 'departmentCode', labelKey: 'department.code', filterType: 'text', sortable: true, sticky: true, width: 160 },
    { key: 'departmentName', labelKey: 'department.name', filterType: 'text', sortable: true, sticky: true, width: 240 },
    { key: 'description', labelKey: 'department.description', filterType: 'text', sortable: false },
    { key: 'currentDivision', labelKey: 'models.Division', filterType: 'select', filterOptions: divisionFilterOptions.value, sortable: false, width: 200 },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },
    {
        key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 250,
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    { key: 'actions', labelKey: 'common.actions', width: 220 },
]);


const selectedRowsData = ref<Array<DepartmentModel>>([]);
const page = ref(1);
const pageSize = ref(30);
const filter = ref({});

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);
const departmentsWithDisplay = computed(() =>
    store.departments.map(item => ({
        ...item,
        currentDivision: divisionStore.divisions.find(d => d.id === item.divisionId)?.divisionName ?? t('common.none'),
    }))
);
const filteredDepartments = computed(() => {
    let arr = departmentsWithDisplay.value;
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

onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: ('department.add'), type: 'add' },
            { event: 'delete', label: ('department.delete'), type: 'delete' }
        ]
    });
    await divisionStore.fetchAllDivisions();

    await store.fetchAllDepartments();
});

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.department') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function handleDelete(departments: DepartmentModel | DepartmentModel[]) {
    const list = Array.isArray(departments) ? departments : [departments];
    const ids = list.filter(item => typeof item.id === 'string' && item.id).map(item => item.id as string);

    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.department') } },
        async () => {
            try {
                startLoading();
                await store.deleteDepartments(ids);
                await store.fetchAllDepartments();

            } catch (err: any) {
                console.error('Error deleting:', err);
            } finally {
                stopLoading();
                showFormModal.value = false;
            }
        }
    );
}

async function addModeEvent() {
    dialogMode.value = 'create';
    await commonStore.generateCode('DE', 'Department', 'DepartmentCode', 4);
    store.selectDepartment(null);
    showFormModal.value = true;
}

function editModelEvent(department: any) {
    dialogMode.value = 'edit';
    store.selectDepartment({ ...department });
    showFormModal.value = true;
}
function viewModelEvent(department: any) {
    dialogMode.value = 'view';
    store.selectDepartment({ ...department });
    showFormModal.value = true;
}
async function handleSave(department: any) {
    try {
        startLoading();
        await store.saveDepartment(department);
        if (department.id) {
            notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.department') } });
        } else {
            notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.department') } });
        }
        await store.fetchAllDepartments();
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
    } finally {
        stopLoading();
        showFormModal.value = false;
    }
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    // Không reset page về 1 ở đây!
}
</script>
