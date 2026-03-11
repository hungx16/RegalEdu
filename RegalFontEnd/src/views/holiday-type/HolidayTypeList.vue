<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="holidayType.headerTitle" headerDesc="holidayType.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <BaseTable :showCheckboxColumn="true" :columns="columns" :items="holidayTypeStore.holidayTypes"
            :loading="holidayTypeStore.loading" :showPagination="true" :page="page" :total="holidayTypeStore.total"
            :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
            @update:rows="val => selectedRowsData = val" @edit="editModelEvent" @view="viewModelEvent"
            :showActionsColumn="true" :showEdit="true" :showDelete="true" :showView="true" @delete="handleDelete"
            @update:page="val => page = val" @update:pageSize="onPageSizeChange">
            <template #cell-status="{ item }">
                <BaseBadge type="boolean" :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
            </template>
        </BaseTable>

        <HolidayTypeDialog v-model:visible="showFormModal" :mode="dialogMode"
            :holiday-type-data="holidayTypeStore.selectedHolidayType" :loading="formLoading" @submit="handleSave"
            @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useHolidayTypeStore } from '@/stores/useHolidayTypeStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import HolidayTypeDialog from '@/views/holiday-type/HolidayTypeDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { useCommonStore } from '@/stores/commonStore';
import { formatDate } from '@/utils/format';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
const { t } = useI18n();
const holidayTypeStore = useHolidayTypeStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);
const commonStore = useCommonStore();

const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRowsData = ref([]);

const columns: BaseTableColumn[] = [
    { key: 'categoryCode', labelKey: 'holidayType.categoryCode', filterType: 'text', sortable: true },
    { key: 'categoryName', labelKey: 'holidayType.categoryName', filterType: 'text', sortable: true },
    { key: 'description', labelKey: 'holidayType.description', filterType: 'text', sortable: false },
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

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.holidayType') } });
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
            params: { model: t('models.holidayType') },
        },
        async () => {
            try {
                startLoading();
                await holidayTypeStore.deleteHolidayTypes(ids);
                await holidayTypeStore.fetchAllHolidayTypes();
            } catch (error) {
                console.error(error);
            } finally {
                stopLoading();
                showFormModal.value = false;
            }

        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    await commonStore.generateCode('HT', 'Category', 'CategoryCode', 4);
    holidayTypeStore.selectHolidayType(null);
    showFormModal.value = true;
}

function editModelEvent(item: any) {
    dialogMode.value = 'edit';
    holidayTypeStore.selectHolidayType({ ...item });
    showFormModal.value = true;
}

function viewModelEvent(item: any) {
    dialogMode.value = 'view';
    holidayTypeStore.selectHolidayType({ ...item });
    showFormModal.value = true;
}

async function handleSave(item: any) {
    startLoading();
    try {
        await holidayTypeStore.saveHolidayType(item);
        notificationStore.showToast('success', { key: item.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.holidayType') } });
        holidayTypeStore.fetchAllHolidayTypes();
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
            { event: 'add', label: 'holidayType.add', type: 'add' },
            { event: 'delete', label: 'holidayType.delete', type: 'delete' },
        ]
    });
    holidayTypeStore.fetchAllHolidayTypes();
});
</script>
