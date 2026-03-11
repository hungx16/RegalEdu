<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="promotionGroup.headerTitle" headerDesc="promotionGroup.headerDesc"
            :disabledDelete="getDisableDelete" class="mb-6" />

        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('promotionGroup.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('promotionGroup.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filterPromotionGroups"
                    :showIndex="true" :loading="groupStore.loading" :showPagination="true" :page="groupStore.query.page"
                    :total="filterPromotionGroups.length" :pageSize="groupStore.query.pageSize"
                    :filter="(groupStore.query as any).filters" @update:filter="onTableFilter"
                    @update:rows="val => selectedRowsData = val" :showView="true" @edit="editModelEvent"
                    :showActionsColumn="true" :showEdit="true" :showDelete="true" @delete="handleDelete"
                    @view="viewModelEvent" @update:page="groupStore.setPage"
                    @update:pageSize="groupStore.setPageSize" />
            </div>
        </div>

        <PromotionGroupDialog :visible="showFormModal" :mode="dialogMode" @update:visible="showFormModal = $event"
            @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { usePromotionGroupStore } from '@/stores/promotionGroupStore';
import { useCommonStore } from '@/stores/commonStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import PromotionGroupDialog from '@/views/promotion-group/PromotionGroupDialog.vue';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import type { PromotionGroupApi, PromotionGroupModel } from '@/api/PromotionGroupApi';

const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
const { t } = useI18n();
const groupStore = usePromotionGroupStore();
const commonStore = useCommonStore();
const notificationStore = useNotificationStore();
const filter = ref({});
const filterComponentRef = ref<any>(null);
const showFormModal = ref(false);
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const selectedRowsData = ref<any[]>([]);
const filterPromotionGroups = computed(() => {
    let arr = groupStore.promotionGroups;
    console.log("arr before filter:", arr);

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
onMounted(() => {
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: ('promotionGroup.add'), type: 'add' },
            { event: 'delete', label: ('promotionGroup.delete'), type: 'delete' }
        ]
    });
    groupStore.fetchAll();
});


watch(
    () => [groupStore.query.page, groupStore.query.pageSize, (groupStore.query as any).filters],
    () => {
        fetchGroups();
    },
    { deep: true }
);

// --- Table Setup ---
function formatDateValue(value: any) {
    const anyCommon = commonStore as any;
    if (typeof anyCommon.formatDate === 'function') {
        return anyCommon.formatDate(value);
    }
    if (value == null || value === '') return '';
    try {
        return new Date(value).toLocaleString();
    } catch {
        return String(value);
    }
}

const columns = computed(() => [
    { key: 'name', labelKey: 'promotionGroup.name', label: t('promotionGroup.name'), prop: 'name', minWidth: 200, sortable: true },
    { key: 'description', labelKey: 'promotionGroup.description', label: t('promotionGroup.description'), prop: 'description', minWidth: 250 },
    { key: 'status', labelKey: 'common.status', label: t('common.status'), prop: 'status', minWidth: 100, formatter: (row: any) => row.status === 0 ? t('common.active') : t('common.inactive') },
    { key: 'createdAt', labelKey: 'common.createdAt', label: t('common.createdAt'), prop: 'createdAt', minWidth: 150, formatter: (row: any) => formatDateValue(row.createdAt) },
]);

async function fetchGroups() {
    const anyStore = groupStore as any;
    if (typeof anyStore.fetchPagedPromotionGroups === 'function') {
        return await anyStore.fetchPagedPromotionGroups();
    }
    if (typeof anyStore.fetchPromotionGroups === 'function') {
        return await anyStore.fetchPromotionGroups();
    }
    if (typeof anyStore.fetchList === 'function') {
        return await anyStore.fetchList();
    }
    console.warn('No fetch method found on groupStore');
    return Promise.resolve();
}

// --- Actions & Handlers ---
const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

function onTableFilter(newFilters: any) {
    (groupStore.query as any).filters = { ...(groupStore.query as any).filters, ...newFilters };
    groupStore.setPage(1);
}
function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.department') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function addModelEvent() {
    dialogMode.value = 'create';
    groupStore.selectedPromotionGroup = null;
    showFormModal.value = true;
}

function editModelEvent(group: any) {
    dialogMode.value = 'edit';
    groupStore.selectedPromotionGroup = { ...group };
    showFormModal.value = true;
}

function viewModelEvent(group: any) {
    dialogMode.value = 'view';
    groupStore.selectedPromotionGroup = { ...group };
    showFormModal.value = true;
}

async function handleSave(group: any) {
    try {
        startLoading();
        await groupStore.savePromotionGroup(group);
        if (group.id) {
            notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.promtionGroup') } });
        } else {
            notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.department') } });
        }
        await groupStore.fetchAll();
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
    } finally {
        stopLoading();
        showFormModal.value = false;
    }
}

async function handleDelete(groups: PromotionGroupModel | PromotionGroupModel[]) {
    const list = Array.isArray(groups) ? groups : [groups];
    console.log("list", list);

    const ids = list.filter(item => typeof item.id === 'string' && item.id).map(item => item.id as string);

    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.promotionGroup') } },
        async () => {
            try {
                startLoading();
                await groupStore.deletePromotionGroups(ids);
                await groupStore.fetchAll();

            } catch (err: any) {
                console.error('Error deleting:', err);
            } finally {
                stopLoading();
                showFormModal.value = false;
            }
        }
    );
}
</script>