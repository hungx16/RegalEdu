<template>
    <div>
        <FilterComponent ref="filterComponentRef" @add="addModelEvent" @delete="onDeleteClicked"
            headerTitle="event.headerTitle" headerDesc="event.headerDesc" :disabledDelete="getDisableDelete"
            class="mb-6" />

        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('event.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('event.listFunction') }}</span>
                </div>
            </div>
            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredEventsAll"
                    :loading="eventStore.loading" :showPagination="true" :page="page" :total="filteredEventsAll.length"
                    :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
                    @update:rows="val => selectedRowsData = val" @edit="editModelEvent" @view="viewModelEvent"
                    :showActionsColumn="true" :showEdit="true" :showDelete="true" :showView="true"
                    @delete="handleDelete" @update:page="val => page = val" @update:pageSize="onPageSizeChange">

                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean"
                            :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                    </template>
                    <template #cell-category="{ item }">
                        <BaseBadge :label="item.categoryName" :rawLabel="true" />
                    </template>
                    <template #cell-eventCode="{ item }">
                        <BaseBadge :label="item.eventCode" color="blue" soft :bold="true" bordered />
                    </template>
                </BaseTable>
            </div>
        </div>

        <EventDialog v-model:visible="showFormModal" :mode="dialogMode" :event-data="eventStore.selectedEvent"
            :loading="formLoading" @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useEventStore } from '@/stores/eventStore';
import { useCommonStore } from '@/stores/commonStore';
import { useNotificationStore } from '@/stores/notificationStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import FilterComponent from '@/components/filter-component/FilterComponent.vue';
import EventDialog from './EventDialog.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { formatDate } from '@/utils/format';
import { getEventCategoryOptions } from '@/utils/makeList';

const { t } = useI18n();
const eventStore = useEventStore();
const commonStore = useCommonStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);

const showFormModal = ref(false);
const filterComponentRef = ref();
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(30);
const filter = ref({});
const selectedRowsData = ref([]);

const getDisableDelete = computed(() => selectedRowsData.value.length === 0);
const eventsWithDisplay = computed(() =>
    eventStore.events.map(item => ({
        ...item,
        categoryName: getEventCategoryOptions(t).find(cat => cat.value === item.category)?.label || '',
    }))
);
const filteredEventsAll = computed(() => {
    let arr = eventsWithDisplay.value;
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            arr = arr.filter(item => {
                if (key === 'createdAt') {
                    const dateOnly = String(item[key]).substring(0, 10);
                    return dateOnly === val;
                } else {
                    return String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase());
                }
            });
        }
    });
    return arr;
});

const columns: BaseTableColumn[] = [
    { key: 'eventCode', labelKey: 'event.code', filterType: 'text', sortable: true, sticky: true, align: 'center' },
    { key: 'eventName', labelKey: 'event.name', filterType: 'text', sortable: true, sticky: true, isBold: true },
    {
        key: 'category', labelKey: 'event.category', filterType: 'select', sortable: true, filterOptions: [
            { label: 'common.all', value: '' },
            ...getEventCategoryOptions(t),
        ], width: 180, align: 'center'
    },
    { key: 'description', labelKey: 'event.description', filterType: 'text', sortable: true },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, align: 'center', filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true },
    { key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, formatter: (value: string) => formatDate(value, 'DD/MM/YYYY') },
    { key: 'actions', labelKey: 'common.actions', width: 200 },
];

function onDeleteClicked() {
    if (selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.event') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}

function handleDelete(events) {
    const ids = (Array.isArray(events) ? events : [events]).map(item => item.id);
    notificationStore.showConfirm({ key: 'toast.delete', params: { model: t('models.event') } }, async () => {
        startLoading();
        try {
            await eventStore.deleteEvents(ids);
            await eventStore.fetchAllEvents();
            stopLoading();
            showFormModal.value = false;
        } catch (err) {
            stopLoading();
        }
    });
}

async function addModelEvent() {
    dialogMode.value = 'create';
    await commonStore.generateCode('SK', 'Event', 'EventCode', 4);
    eventStore.selectEvent(null);
    showFormModal.value = true;
}

function editModelEvent(event: any) {
    dialogMode.value = 'edit';
    eventStore.selectEvent({ ...event });
    showFormModal.value = true;
}

function viewModelEvent(event: any) {
    dialogMode.value = 'view';
    eventStore.selectEvent({ ...event });
    showFormModal.value = true;
}

async function handleSave(event: any) {
    startLoading();
    try {
        await eventStore.saveEvent(event);
        notificationStore.showToast('success', { key: event.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.event') } });
        await eventStore.fetchAllEvents();
        stopLoading();
        showFormModal.value = false;
    } catch (err) {
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
            { event: 'add', label: 'event.add', type: 'add' },
            { event: 'delete', label: 'event.delete', type: 'delete' },
        ],
    });
    eventStore.fetchAllEvents();
});
</script>
