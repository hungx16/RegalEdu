<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addItem" @delete="onDeleteClicked"
            headerTitle="item.headerTitle" headerDesc="item.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />
        <!-- ROW cho 2 CARD -->
        <div class="row g-4 mb-8">
            <div class="col-6 col-md-3">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('item.totalItems') }}</span>
                        <i class="bi bi-journal-text fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ itemStore.items.length }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('item.totalQuantity') }}</span>
                        <i class="bi bi-list-ol fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalQuantity }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('item.inStock', {
                        number: 0
                    }) }}</div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('item.totalValue') }}</span>
                        <i class="bi bi-cash-stack fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ formatCurrency(totalValue) }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('item.valueStock') }}</div>
                </div>
            </div>
            <div class="col-6 col-md-3">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('item.lowStock') }}</span>
                        <i class="bi bi-exclamation-triangle-fill fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalUnderStock }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('item.under50Item') }}</div>
                </div>
            </div>
        </div>

        <!-- BẢNG DỮ LIỆU -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('item.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('item.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="itemStore.items"
                    :loading="itemStore.loading" :showPagination="true" :page="page" :total="itemStore.total"
                    :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
                    @update:rows="val => selectedRows = val" @edit="editItem" @view="viewItem" :showActionsColumn="true"
                    :showEdit="true" :showDelete="true" :showView="true" @delete="handleDelete"
                    @update:page="val => page = val" @update:pageSize="onPageSizeChange">
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-price="{ item }">
                        {{ formatCurrency(item.price) }}
                    </template>
                </BaseTable>
            </div>
        </div>

        <ItemDialog v-model:visible="showDialog" :mode="dialogMode" :item-data="itemStore.selectedItem"
            :loading="formLoading" @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useItemStore } from '@/stores/itemStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import ItemDialog from './ItemDialog.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { formatCurrency, formatDate } from '@/utils/format';
import { useCommonStore } from '@/stores/commonStore';

const commonStore = useCommonStore();

const { t } = useI18n();
const itemStore = useItemStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);

const showDialog = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRows = ref([]);
const totalQuantity = computed(() => {
    return itemStore.items.reduce((sum, item) => sum + (item.quantity || 0), 0);
});
const totalValue = computed(() => {
    return itemStore.items.reduce((sum, item) => sum + ((item.price || 0) * (item.quantity || 0)), 0);
});
const totalUnderStock = computed(() => {
    return itemStore.items.filter(item => (item.quantity || 0) < 50).length;
});
const columns: BaseTableColumn[] = [
    { key: 'itemCode', labelKey: 'item.code', filterType: 'text', sortable: true },
    { key: 'itemName', labelKey: 'item.name', filterType: 'text', sortable: true },
    { key: 'price', labelKey: 'item.price', filterType: 'number', sortable: true, align: 'right' },
    { key: 'quantity', labelKey: 'item.quantity', filterType: 'number', sortable: true, align: 'center' },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 180 },
    {
        key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 180,
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    { key: 'actions', labelKey: 'common.actions', width: 220 },
];

const getDisableDelete = computed(() => selectedRows.value.length === 0);

function onDeleteClicked() {
    if (!selectedRows.value || selectedRows.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.item') } });
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
        { key: 'toast.delete', params: { model: t('models.item') } },
        async () => {
            await itemStore.deleteItems(ids);
            notificationStore.showToast('success', { key: 'toast.deleteSuccess', params: { model: t('models.item') } });
            itemStore.fetchAllItems();
        }
    );
}

async function addItem() {
    dialogMode.value = 'create';
    await commonStore.generateCode('AP', 'Item', 'ItemCode', 4);

    itemStore.selectItem(null);
    showDialog.value = true;
}

function editItem(item: any) {
    dialogMode.value = 'edit';
    itemStore.selectItem({ ...item });
    showDialog.value = true;
}

function viewItem(item: any) {
    dialogMode.value = 'view';
    itemStore.selectItem({ ...item });
    showDialog.value = true;
}

async function handleSave(item: any) {
    startLoading();
    try {
        await itemStore.saveItem(item);
        notificationStore.showToast('success', { key: item.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.item') } });
        itemStore.fetchAllItems();
        showDialog.value = false;
    } catch {
        notificationStore.showToast('error', { key: 'toast.saveError', params: { model: t('models.item') } });
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
            { event: 'add', label: 'item.add', type: 'add' },
            { event: 'delete', label: 'item.delete', type: 'delete' },
        ],
    });
    itemStore.fetchAllItems();
});
</script>
