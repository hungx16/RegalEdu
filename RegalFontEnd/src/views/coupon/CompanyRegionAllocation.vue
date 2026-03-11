<template>
    <BaseDialogForm v-model:visible="showAllocationDialog" :title="t('company.monthlyCompanyAllocation')"
        :width="computedDialogWidth" :description="t('company.monthlyCompanyAllocationDesc')" @close="closeModal"
        :mode="'view'" @update:visible="emit('update:visible', $event)">
        <template #icon>
            <i class="bi bi-graph-up text-primary"></i>
        </template>
        <template #form>
            <MonthSwitcher v-model="currentMonth" :min="minDate" :max="maxDate" @update:modelValue="onMonthChange" />

            <BaseTable :columns="columns" :items="filteredRegionCompaniesAll" :loading="companyStore.loading"
                :showPagination="true" :page="page" :total="filteredRegionCompaniesAll.length" :pageSize="pageSize"
                :filter="filter" @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val"
                :showActionsColumn="true" @update:page="val => page = val" @update:pageSize="onPageSizeChange">
                <template #cell-regionName="{ item }">
                    <BaseBadge :label="item.region?.regionName || t('common.none')" />
                </template>
                <template #cell-description="{ item }">
                    {{ item.description || '-' }}
                </template>
                <template #cell-companyName="{ item }">
                    {{ item.company?.companyName || '-' }}
                </template>
                <template #cell-status="{ item }">
                    <BaseBadge type="boolean" :label="item.status === 0 ? t('common.active') : t('common.inactive')" />
                </template>
                <template #actions="{ item }">
                    <el-tooltip :content="t('common.allocate')" placement="top">
                        <el-button class="btn-allocate" circle size="small" @click="handleAllocate(item)">
                            <el-icon>
                                <i class="bi bi-arrow-left-right" />
                            </el-icon>
                        </el-button>
                    </el-tooltip>
                </template>
            </BaseTable>
        </template>
    </BaseDialogForm>

    <NewAllocateDialog v-model:visible="showAllocationActionDialog" :data="selectedAllocation" @submit="handleSave"
        @close="showAllocationActionDialog = false" @update:visible="showAllocationActionDialog = $event" />
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useCompanyStore } from '@/stores/companyStore'
const { t } = useI18n();
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue';
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { formatDate } from '@/utils/format'
import type { LogRegionComModel } from '@/api/CompanyApi';
import NewAllocateDialog from './NewAllocateDialog.vue'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import MonthSwitcher from '@/components/month-switcher/MonthSwitcher.vue'
const props = defineProps<{ visible: boolean; companyId: string | null }>()
const emit = defineEmits(['update:visible', 'close'])
const companyStore = useCompanyStore()
const notificationStore = useNotificationStore();
const windowWidth = ref(window.innerWidth)
const currentMonth = ref(new Date());
const minDate = new Date(2022, 0, 1); // Tùy chỉnh
const maxDate = new Date(2026, 11, 31);
const filter = ref({});
const page = ref(1);
const pageSize = ref(30);
const showAllocationDialog = computed({
    get: () => props.visible,
    set: (val) => emit('update:visible', val),
})
const selectedRowsData = ref([]);
const selectedAllocation = ref<LogRegionComModel | null>(null)
const showAllocationActionDialog = ref(false)

const computedDialogWidth = computed(() => {
    return windowWidth.value < 768 ? '100%' : '80%'
})
const columns = computed<BaseTableColumn[]>(() => [
    { key: 'companyName', labelKey: 'company.name', minWidth: 200, filterType: 'text', sortable: true },
    { key: 'regionName', labelKey: 'company.region', minWidth: 150, filterType: 'text', sortable: true },
    { key: 'startedDate', labelKey: 'company.startedDate', minWidth: 160, formatter: (val: string) => formatDate(val, 'DD/MM/YYYY'), filterType: 'date', sortable: true },
    { key: 'endDate', labelKey: 'company.endDate', minWidth: 160, formatter: (val: string) => formatDate(val, 'DD/MM/YYYY'), filterType: 'date', sortable: true },
    { key: 'description', labelKey: 'company.description', minWidth: 200 },
    { key: 'createdBy', labelKey: 'common.createdBy', minWidth: 150, filterType: 'text', sortable: true },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },
])
const filteredByMonthRegionCompanies = computed(() => {
    const month = currentMonth.value.getMonth();
    const year = currentMonth.value.getFullYear();

    return companyStore.LogRegionComs.filter(item => {
        const started = new Date(item.startedDate);
        return started.getMonth() === month && started.getFullYear() === year;
    });
});
const regionCompaniesWithDisplay = computed(() =>
    filteredByMonthRegionCompanies.value.map(item => ({
        ...item,
        regionName: item.region?.regionName ?? t('common.none'),
        companyName: item.company?.companyName ?? t('common.none'),
    }))
);
const filteredRegionCompaniesAll = computed(() => {
    let arr = regionCompaniesWithDisplay.value;
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            if (key === 'establishmentDate' || key === 'createdAt') {
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
function updateWindowWidth() {
    windowWidth.value = window.innerWidth
}
onMounted(() => {
    window.addEventListener('resize', updateWindowWidth)
    updateWindowWidth()
})

function onMonthChange(val: Date) {
    // Xử lý khi đổi tháng
    console.log('Selected month changed:', val);
    currentMonth.value = val;

}

function onTableFilter(val: Record<string, any>) {
    filter.value = val;
    page.value = 1;
}
async function handleSave(company) {
    try {
        await companyStore.createLogRegionCom(company);
        showAllocationActionDialog.value = false;
        notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.company') } });
        await companyStore.fetchAllCompanyRegions();
    } catch (err: any) {
        console.error('Error saving:', err?.response?.data?.errors || err);
    }
}
function onPageSizeChange(newSize: number) {
    pageSize.value = newSize;
    page.value = 1;
}
function closeModal() {
    emit('update:visible', false)
    emit('close')
}
function handleAllocate(item: LogRegionComModel) {
    selectedAllocation.value = item;
    showAllocationActionDialog.value = true;
}
</script>
<style>
.btn-allocate:hover,
.btn-allocate:focus {
    background-color: #ffd740 !important;
    color: #212121 !important;
    border: none;
}

.btn-allocate {
    background: transparent !important;
    border: none !important;
    box-shadow: none !important;
    border-radius: 12px !important;
    min-width: 30px;
    height: 30px;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    padding: 0 !important;
    margin: 0 4px;
    transition: background 0.2s, color 0.2s;
    position: relative;
    overflow: visible;
    color: var(--kt-menu-text-color) !important;
}
</style>