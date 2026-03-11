<template>
    <div>
        <!-- HEADER LOCALIZE -->
        <div class="d-flex align-items-center justify-content-between mb-4">
            <div class="fw-semibold fs-5">
                {{ t('holiday.headerTitle') }}
                <span class="text-muted">({{ holidayStore.holidays.length }})</span>
            </div>
            <div class="d-flex gap-2">
                <el-button size="small" @click="onImport">
                    <i class="bi bi-upload me-1 text-primary"></i>
                    {{ t('holiday.importVN') }}
                </el-button>
                <el-button size="small" @click="onExport">
                    <i class="bi bi-download me-1 text-primary"></i>
                    {{ t('holiday.export') }}
                </el-button>
                <el-button type="primary" size="small" @click="addModelEvent">
                    <i class="bi bi-plus-lg me-1 text-white"></i>
                    {{ t('holiday.add') }}
                </el-button>
                <el-button type="danger" size="small" @click="handleDelete(selectedRowsData)"
                    :disabled="getDisableDelete">
                    <i class="bi bi-plus-lg me-1 text-white"></i>
                    {{ t('holiday.delete') }}
                </el-button>
            </div>
        </div>

        <!-- Table -->
        <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredHolidaysAll"
            :loading="holidayStore.loading" :showPagination="true" :page="page" :total="filteredHolidaysAll.length"
            :pageSize="pageSize" :filter="filter" @update:filter="onTableFilter"
            @update:rows="val => selectedRowsData = val" @edit="editModelEvent" @view="viewModelEvent"
            :showActionsColumn="true" :showEdit="true" :showDelete="true" :showView="true" @delete="handleDelete"
            @update:page="val => page = val" @update:pageSize="onPageSizeChange">
            <!-- Type column: badge tím -->
            <template #cell-category="{ item }">
                <BaseBadge :label="item.category || '-'" :rawLabel="true" />
            </template>
            <!-- Frequency column: badge xanh/xám -->
            <template #cell-frequency="{ item }">
                <BaseBadge :label="item.frequency === 0 ? t('holiday.frequencyYearly') : t('holiday.frequencyOnce')"
                    :rawLabel="true" :color="item.frequency === 0 ? '#18a1e4' : '#d6dee8'"
                    :textColor="item.frequency === 0 ? '#fff' : '#333'" />
            </template>
            <!-- Date column: format -->
            <template #cell-date="{ item }">
                <span>{{ formatDate(item.date, 'DD/MM/YYYY') }}</span>
            </template>
        </BaseTable>

        <!-- Popup -->
        <HolidayDialog v-model:visible="showFormModal" :mode="dialogMode" :holiday-data="holidayStore.selectedHoliday"
            :configurationName="selectedConfigName" :configurationId="props.configurationId" :loading="formLoading"
            @submit="handleSave" @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import { useHolidayStore } from '@/stores/useHolidayStore';
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import HolidayDialog from './HolidayDialog.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout';
import { formatDate } from '@/utils/format';
import { useWorkingTimeConfigurationStore } from '@/stores/workingTimeConfigurationStore';
import { useHolidayTypeStore } from '@/stores/useHolidayTypeStore';

const { t } = useI18n();
const holidayStore = useHolidayStore();
const notificationStore = useNotificationStore();
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000);
const props = defineProps<{ configurationId: string }>();
const showFormModal = ref(false);
const dialogMode = ref<'create' | 'edit' | 'view'>('create');
const page = ref(1);
const pageSize = ref(20);
const filter = ref({});
const selectedRowsData = ref([]);
const workingTimeConfigurationsStore = useWorkingTimeConfigurationStore();
const holidayTypeStore = useHolidayTypeStore();
const holidayTypeFilterOptions = ref<{ label: string; value: any }[]>([]);

const selectedConfig = computed(() => workingTimeConfigurationsStore.configurations.find(c => c.id === props.configurationId));
const selectedConfigName = computed(() => selectedConfig.value?.nameConfiguration || '');
const columns = computed<BaseTableColumn[]>(() => [
    { key: 'name', labelKey: 'holiday.name', filterType: 'text', sortable: true },
    { key: 'date', labelKey: 'holiday.date', filterType: 'date', sortable: true },
    {
        key: 'category',
        labelKey: 'holiday.type',
        filterType: 'select',
        sortable: true,
        filterOptions: holidayTypeFilterOptions.value,
    },
    {
        key: 'frequency', labelKey: 'holiday.frequency', filterType: 'select', sortable: true,
        filterOptions: [
            { label: t('common.All'), value: '', isLocale: false },
            { label: t('holiday.frequencyYearly'), value: 0, isLocale: false },
            { label: t('holiday.frequencyOnce'), value: 1, isLocale: false }
        ]
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 250 },
    {
        key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 250,
        formatter: (value: string) => formatDate(value, 'DD/MM/YYYY')
    },
    { key: 'description', labelKey: 'holiday.description', filterType: 'text', sortable: false },
]);


const getDisableDelete = computed(() => selectedRowsData.value.length === 0);

function onDeleteClicked() {
    if (!selectedRowsData.value || selectedRowsData.value.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.holiday') } });
        return;
    }
    handleDelete(selectedRowsData.value);
}

watch(
    () => holidayTypeStore.holidayTypes,
    (holidayTypes) => {
        holidayTypeFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...holidayTypes.map(holidayType => ({
                label: holidayType.categoryName,
                value: holidayType.categoryName,
                isLocale: false,
            }))
        ];
    },
    { immediate: true }
);

const holidaysWithCount = computed(() =>
    holidayStore.holidays.map(item => ({
        ...item,
        category: item?.category?.categoryName ?? t('common.none'),
    }))
);
// Lọc data theo filter
const filteredHolidaysAll = computed(() => {
    let arr = holidaysWithCount.value;
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
            params: { model: t('models.holiday') },
        },
        async () => {
            try {
                startLoading();
                await holidayStore.deleteHolidays(ids);
                await holidayStore.fetchAllHolidays();
            } catch (error: any) {
                console.error('Error deleting:', error?.response?.data?.errors || error);
            } finally {
                stopLoading();
            }
        }
    );
}

async function addModelEvent() {
    dialogMode.value = 'create';
    holidayStore.selectHoliday(null);
    showFormModal.value = true;
}

function editModelEvent(item: any) {
    dialogMode.value = 'edit';
    holidayStore.selectHoliday({ ...item });
    showFormModal.value = true;
}

function viewModelEvent(item: any) {
    dialogMode.value = 'view';
    holidayStore.selectHoliday({ ...item });
    showFormModal.value = true;
}

async function handleSave(item: any) {
    startLoading();
    try {
        await holidayStore.saveHoliday(item);
        notificationStore.showToast('success', { key: item.id ? 'toast.updateSuccess' : 'toast.createSuccess', params: { model: t('models.holiday') } });
        holidayStore.fetchAllHolidays();
        showFormModal.value = false;
    } catch (err) {
        notificationStore.showToast('error', { key: 'toast.saveError', params: { model: t('models.holiday') } });
    } finally {
        stopLoading();
    }
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}

function onImport() {
    notificationStore.showToast('info', { key: 'toast.comingSoon', params: {} });
}
function onExport() {
    notificationStore.showToast('info', { key: 'toast.comingSoon', params: {} });
}

onMounted(async () => {
    await holidayStore.fetchAllHolidays();

    await holidayTypeStore.fetchAllHolidayTypes();
});
</script>

<style scoped>
@media (max-width: 600px) {
    .d-flex.align-items-center.justify-content-between.mb-4 {
        flex-direction: column !important;
        align-items: stretch !important;
        gap: 12px;
    }

    .d-flex.gap-2 {
        flex-wrap: wrap;
        justify-content: flex-start;
    }

    .fw-semibold.fs-5 {
        text-align: left;
        word-break: break-word;
    }
}
</style>
