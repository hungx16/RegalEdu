<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addTuition" @delete="onDeleteClicked"
            headerTitle="tuition.headerTitle" headerDesc="tuition.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />
        <!-- SUMMARY CARDS -->
        <div class="row g-4 mb-8">
            <div class="col-12 col-md-6">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('tuition.totalTuitions') }}</span>
                        <i class="bi bi-collection fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ store.total }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('models.tuition') }}</div>
                </div>
            </div>
            <div class="col-12 col-md-6">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('tuition.activeTuitions') }}</span>
                        <i class="bi bi-check2-circle fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{store.Tuitions.filter(t => t.status === 0).length}}</div>
                    <div class="fs-7 text-body-secondary">{{ t('models.tuition') }}</div>
                </div>
            </div>
        </div>
        <!-- BẢNG DỮ LIỆU -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('tuition.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('tuition.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="store.Tuitions"
                    :loading="store.loading" :showPagination="true" :page="page" :total="store.total"
                    :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
                    @update:rows="val => selectedRows = val" @edit="editTuition" @view="viewTuition"
                    :showActionsColumn="true" :showEdit="true" :showDelete="true" :showView="true"
                    @delete="handleDelete" @update:page="val => page = val" @update:pageSize="onPageSizeChange">
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-tuitionFee="{ item }">
                        {{ formatCurrency(item.tuitionFee) }}
                    </template>
                </BaseTable>
            </div>
        </div>
        <TuitionDialog v-model:visible="showDialog" :mode="dialogMode" :tuition-data="store.selectedTuition"
            :loading="formLoading" @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useTuitionStore } from '@/stores/tuitionStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import TuitionDialog from '@/views/tuition/TuitionDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { formatCurrency, formatDate } from '@/utils/format';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
const { t } = useI18n();
const store = useTuitionStore();
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
    { key: 'tuitionCode', labelKey: 'tuition.tuitionCode', filterType: 'text', sortable: true },
    { key: 'tuitionName', labelKey: 'tuition.tuitionName', filterType: 'text', sortable: true },
    { key: 'tuitionFee', labelKey: 'tuition.tuitionFee', filterType: 'number', sortable: true },
    { key: 'startDate', labelKey: 'tuition.startDate', filterType: 'date', sortable: true },
    { key: 'endDate', labelKey: 'tuition.endDate', filterType: 'date', sortable: true },
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
    { key: 'actions', labelKey: 'common.actions', width: 220 },];

const getDisableDelete = computed(() => selectedRows.value.length === 0);

function onDeleteClicked() {
    if (!selectedRows.value || selectedRows.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.tuition') } });
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
        { key: 'toast.delete', params: { model: t('models.tuition') } },
        async () => {
            await store.deleteTuitions(ids);
            store.fetchAllTuitions();
        }
    );
}

async function addTuition() {
    dialogMode.value = 'create';
    store.selectTuition(null);
    showDialog.value = true;
}

function editTuition(item: any) {
    dialogMode.value = 'edit';
    store.selectTuition({ ...item });
    showDialog.value = true;
}

function viewTuition(item: any) {
    dialogMode.value = 'view';
    store.selectTuition({ ...item });
    showDialog.value = true;
}

async function handleSave(item: any) {
    startLoading();
    try {
        await store.saveTuition(item);
        notificationStore.showToast('success', { key: item.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.tuition') } });
        store.fetchAllTuitions();
        showDialog.value = false;
    } catch (error) {
        console.log(error);

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
            { event: 'add', label: 'tuition.add', type: 'add' },
            { event: 'delete', label: 'tuition.delete', type: 'delete' },
        ],
    });
    store.fetchAllTuitions();
});
</script>
