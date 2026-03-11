<template>
    <div>
        <!-- 🔹 FILTER HEADER -->
        <FilterComponent ref="filterComponentRef" headerTitle="allocationEvent.headerTitle"
            headerDesc="allocationEvent.headerDesc" @add="addEvent" @delete="onDeleteClicked" @summary="openSummary"
            :disabledDelete="selectedEvents.length === 0" class="mb-6" />

        <!-- 🔹 SUMMARY CARDS -->
        <div class="row g-4 mb-8">
            <div class="col-12 col-md-4">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('allocationEvent.totalEvents') }}</span>
                        <i class="bi bi-calendar-event fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ store.allocationEvents.length }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>

            <div class="col-12 col-md-4">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('allocationEvent.totalBudget') }}</span>
                        <i class="bi bi-cash-stack fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalBudgetFormatted }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('allocationEvent.totalBudgetDesc') }}</div>
                </div>
            </div>

            <div class="col-12 col-md-4">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('allocationEvent.completedEvents') }}</span>
                        <i class="bi bi-check2-circle fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ allocatedCount }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('allocationEvent.completedDesc') }}</div>
                </div>
            </div>
        </div>

        <!-- 🔹 TABLE -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('allocationEvent.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('allocationEvent.listFunction') }}</span>
                </div>
            </div>

            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :columns="columns" :items="filteredEvents" :loading="formLoading"
                    :showPagination="true" :page="page" :total="filteredEvents.length" :pageSize="pageSize"
                    :filter="filter" @update:filter="onTableFilter" @update:rows="val => (selectedEvents = val)"
                    @edit="editEvent" @view="viewEvent" @delete="handleDelete" :showActionsColumn="true"
                    :actionsColumnWidth="200" :showEdit="true" :showDelete="true" :showView="true"
                    @update:page="val => (page = val)" @update:pageSize="onPageSizeChange">
                    <!-- Tháng -->
                    <template #cell-monthLabel="{ item }">
                        <span>{{ item.monthLabel }}</span>
                    </template>

                    <!-- Vùng -->
                    <template #cell-regionCount="{ item }">
                        <span class="fw-semibold">{{ item.regionCount }}</span>
                    </template>

                    <!-- Chi nhánh -->
                    <template #cell-companyCount="{ item }">
                        <span class="company-chip">
                            <i class="bi bi-building me-1"></i>
                            {{ t('allocationEvent.branchesCount', { count: item.companyCount }) }}
                        </span>
                    </template>

                    <!-- Số lượng -->
                    <template #cell-totalQuantity="{ item }">
                        <span class="soft-pill">
                            {{ t('allocationEvent.eventsCount', { count: item.totalQuantity }) }}
                        </span>
                    </template>

                    <!-- Tổng ngân sách -->
                    <template #cell-totalBudget="{ item }">
                        <span class="fw-semibold">{{ formatCurrency(item.totalBudget) }}</span>
                    </template>

                    <!-- Trạng thái -->
                    <template #cell-allocationEventStatus="{ item }">
                        <BaseBadge :label="statusLabel(item.allocationEventStatus)" :colorByLabelMap="statusColorMap" />
                    </template>

                    <!-- Action phụ: Đánh dấu hoàn tất -->
                    <template #actions="{ item }">
                        <el-tooltip :content="t('allocationEvent.markAllocated')" placement="top" :teleported="true"
                            :popper-options="{ strategy: 'fixed' }" popper-class="table-tooltip-popper">
                            <el-button circle size="small" :disabled="!canMarkAllocated(item)" class="markAllocated"
                                @click="markAllocated(item)">
                                <el-icon>
                                    <Check />
                                </el-icon>
                            </el-button>
                        </el-tooltip>
                        <el-tooltip :content="t('allocationEvent.markCompleted')" placement="top" :teleported="true"
                            :popper-options="{ strategy: 'fixed' }" popper-class="table-tooltip-popper">
                            <el-button circle size="small" :disabled="!canMarkCompleted(item)" class="markComplete"
                                @click="markCompleted(item)">
                                <el-icon>
                                    <Refresh />
                                </el-icon>
                            </el-button>
                        </el-tooltip>
                    </template>
                </BaseTable>
            </div>
        </div>

        <!-- 🔹 DIALOG -->
        <EventAllocationDialog v-if="showDialog" v-model:visible="showDialog" :mode="dialogMode"
            :loading="formLoading" :allocation-event-data="store.selectedEvent" @submit="handleSave"
            @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import FilterComponent from '@/components/filter-component/FilterComponent.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import EventAllocationDialog from './EventAllocationDialog.vue'
import { useAllocationEventStore } from '@/stores/allocationEventStore'
import { useNotificationStore } from '@/stores/notificationStore'
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout'
import { formatCurrency, formatDate } from '@/utils/format'
import { AllocationEventStatus, AllocationEventStatusLabels } from '@/types'
import { Refresh, Check } from '@element-plus/icons-vue'
import {
    getAllocationEventStatusOptions
} from '@/utils/makeList'

const { t } = useI18n()
const store = useAllocationEventStore()
const notificationStore = useNotificationStore()
const filterComponentRef = ref()

const showDialog = ref(false)
const dialogMode = ref<'create' | 'edit' | 'view'>('create')
const filter = ref<Record<string, any>>({})
const selectedEvents = ref<any[]>([])
const page = ref(1)
const pageSize = ref(30)
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)

/** ==== Decorate items (ngân sách theo chi nhánh được áp) ==== **/
/** ==== Decorate items (ngân sách = số CN được áp chỉ tiêu × budget/branch) ==== **/
const decoratedEvents = computed(() => {
    return (store.allocationEvents || []).map(ev => {
        const details = Array.isArray(ev.allocationDetails) ? ev.allocationDetails : []

        // Tổng số sự kiện (giữ nguyên cho cột "Số lượng")
        const totalQuantity = details.reduce((s, d) => s + (Number(d.quantity) || 0), 0)

        // Đếm vùng & chi nhánh (tổng quan)
        const companyIds = new Set(details.map(d => d.companyId).filter(Boolean))
        const regionIds = new Set(details.map(d => d.regionId).filter(Boolean))

        // ✅ CN "được áp chỉ tiêu" = có ÍT NHẤT 1 detail (noAllocation ≠ 1) VÀ quantity > 0
        const appliedCompanyIds = new Set<string>()
        details.forEach(d => {
            if (d?.companyId == null) return
            const qty = Number(d.quantity) || 0
            if (d.noAllocation !== 1 && qty > 0) {
                appliedCompanyIds.add(String(d.companyId))
            }
        })

        // ✅ Tổng ngân sách = số CN được áp × ngân sách/chi nhánh
        const perBranchBudget = Number(ev.eventBudget) || 0
        const totalBudget = appliedCompanyIds.size * perBranchBudget

        // Nhãn tháng
        const m = String(ev.allocationMonth ?? '').padStart(2, '0')
        const y = ev.allocationYear ?? ''
        const monthLabel = t('allocationEvent.monthLabel', { month: m, year: y })

        return {
            ...ev,
            monthLabel,
            regionCount: regionIds.size || 0,
            companyCount: companyIds.size || 0,
            totalQuantity,
            totalBudget,
            appliedBranchCount: appliedCompanyIds.size
        }
    })
})

/** ===== Columns ===== **/
const columns = computed<BaseTableColumn[]>(() => [
    { key: 'allocationCode', labelKey: 'allocationEvent.code', sortable: true, isBold: true, align: 'center', filterType: 'text' },
    { key: 'monthLabel', labelKey: 'allocationEvent.month', sortable: false, align: 'left', width: 220, filterType: 'text' },
    { key: 'regionCount', labelKey: 'allocationEvent.region', sortable: true, align: 'center', width: 150 },
    { key: 'companyCount', labelKey: 'allocationEvent.branches', sortable: true, align: 'left', width: 180 },
    { key: 'totalQuantity', labelKey: 'allocationEvent.quantity', sortable: true, align: 'center', width: 150 },
    { key: 'totalBudget', labelKey: 'allocationEvent.totalBudget', sortable: true, align: 'right', width: 180 },
    {
        key: 'allocationEventStatus',
        labelKey: 'allocationEvent.status',
        sortable: true,
        align: 'center',
        width: 200,
        filterType: 'select',
        filterOptions: statusFilterOptions.value
    },
    { key: 'createdBy', labelKey: 'common.createdBy', width: 140 },
    { key: 'createdAt', labelKey: 'common.createdAt', width: 140, formatter: (value: string) => formatDate(value, 'DD/MM/YYYY'), align: 'center' },
    { key: 'actions', labelKey: 'common.actions', width: 250 }
])

/** ===== Summary cards ===== **/
const totalBudgetFormatted = computed(() =>
    formatCurrency(decoratedEvents.value.reduce((sum, x) => sum + (x.totalBudget || 0), 0))
)
const allocatedCount = computed(() =>
    (store.allocationEvents || []).filter(x => x.allocationEventStatus === AllocationEventStatus.Completed).length
)

/** ===== Filtering ===== **/
const filteredEvents = computed(() => {
    let arr = decoratedEvents.value
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val != null && val !== '') {
            arr = arr.filter(item => String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase()))
        }
    })
    return arr
})

function statusLabel(status: number) {
    const key = AllocationEventStatusLabels[status as AllocationEventStatus]
    return key ? t(key) : t('common.unknown')
}

const statusFilterOptions = ref<{ label: string; value: any; isLocale?: boolean }[]>([])
watch(
    () => getAllocationEventStatusOptions(t),
    () => {
        statusFilterOptions.value = [
            { label: 'common.all', value: '' },
            ...getAllocationEventStatusOptions(t).map(o => ({ label: o.label, value: o.value, isLocale: false }))
        ]
    },
    { immediate: true }
)
const statusColorMap = computed(() => ({
    [t(AllocationEventStatusLabels[AllocationEventStatus.Draft])]: 'warning',
    [t(AllocationEventStatusLabels[AllocationEventStatus.Allocated])]: 'success',
    [t(AllocationEventStatusLabels[AllocationEventStatus.Completed])]: 'primary',
    [t(AllocationEventStatusLabels[AllocationEventStatus.Cancelled])]: 'red'
}))

/** ===== Table events ===== **/
function onTableFilter(val: Record<string, any>) {
    filter.value = val
    page.value = 1
}
function onPageSizeChange(newSize: number) {
    pageSize.value = newSize
    page.value = 1
}

/** ===== CRUD ===== **/
function addEvent() {
    store.selectEvent(null)
    dialogMode.value = 'create'
    showDialog.value = true
}
function editEvent(item: any) {
    store.selectEvent(item)
    dialogMode.value = 'edit'
    showDialog.value = true
}
function viewEvent(item: any) {
    store.selectEvent(item)
    dialogMode.value = 'view'
    showDialog.value = true
}
async function handleSave(data: any) {
    startLoading()
    await store.saveEvent(data)
    await store.fetchAll()
    stopLoading()
    showDialog.value = false
    notificationStore.showToast('success', { key: 'toast.saveSuccess', params: { model: t('models.eventAllocation') } })
}

/* ✅ NEW: Xoá theo mẫu CompanyList */
function onDeleteClicked(items?: any[]) {
    const list = items && items.length ? items : selectedEvents.value
    if (!list || list.length === 0) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.eventAllocation') } })
        return
    }
    handleDelete(list)
}

async function handleDelete(events: any | any[]) {
    const arr = Array.isArray(events) ? events : [events]
    const ids = arr
        .filter((it: any) => typeof it?.id === 'string' && it.id)
        .map((it: any) => it.id as string)

    if (!ids.length) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.eventAllocation') } })
        return
    }

    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.eventAllocation') } },
        async () => {
            startLoading()
            try {
                await store.deleteEvents(ids)
                await store.fetchAll()
                showDialog.value = false
                notificationStore.showToast('success', { key: 'toast.deleteSuccess', params: { model: t('models.eventAllocation') } })
            } catch (err) {
                console.error('Error deleting allocation events:', err)
                notificationStore.showToast('error', { key: 'toast.deleteError', params: { model: t('models.eventAllocation') } })
            } finally {
                stopLoading()
            }
        }
    )
}

function openSummary() {
    notificationStore.showToast('info', { key: 'allocationEvent.summaryInfo' })
}

/** ===== Complete/Status ===== **/
function canMarkCompleted(item: any) {
    return item?.allocationEventStatus !== AllocationEventStatus.Completed
        && item?.allocationEventStatus !== AllocationEventStatus.Cancelled && item?.allocationEventStatus == AllocationEventStatus.Allocated
}
function canMarkAllocated(item: any) {
    return item?.allocationEventStatus == AllocationEventStatus.Draft

}
async function markCompleted(item: any) {
    notificationStore.showConfirm(
        { key: 'allocationEvent.confirmComplete' },
        async () => {
            startLoading()
            try {
                const payload: any = {
                    id: item.id,
                    allocationCode: item.allocationCode,
                    allocationMonth: item.allocationMonth,
                    allocationYear: item.allocationYear,
                    eventBudget: item.eventBudget,
                    allocationEventStatus: AllocationEventStatus.Completed,
                    allocationDetails: item.allocationDetails ?? []
                }
                await store.saveEvent(payload)
                await store.fetchAll()
                notificationStore.showToast('success', { key: 'allocationEvent.completedSuccess' })
            } finally {
                stopLoading()
            }
        }
    )
}
async function markAllocated(item: any) {
    notificationStore.showConfirm(
        { key: 'allocationEvent.confirmAllocated' },
        async () => {
            startLoading()
            try {
                const payload: any = {
                    id: item.id,
                    allocationCode: item.allocationCode,
                    allocationMonth: item.allocationMonth,
                    allocationYear: item.allocationYear,
                    eventBudget: item.eventBudget,
                    allocationEventStatus: AllocationEventStatus.Allocated,
                    allocationDetails: item.allocationDetails ?? []
                }
                await store.saveEvent(payload)
                await store.fetchAll()
                notificationStore.showToast('success', { key: 'allocationEvent.allocatedSuccess' })
            } finally {
                stopLoading()
            }
        }
    )
}
/** ===== INIT ===== **/
onMounted(async () => {
    await store.fetchAll()
    filterComponentRef.value?.initListHeaderParams({
        listParams: [],
        listBtn: [
            { event: 'add', label: 'allocationEvent.add', type: 'add' },
            { event: 'delete', label: 'allocationEvent.delete', type: 'delete' },
            { event: 'summary', label: 'allocationEvent.summary', type: 'summary' }
        ]
    })
})
</script>

<style scoped>
.company-chip {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    padding: 6px 10px;
    border-radius: 999px;
    background: var(--el-fill-color-light);
    color: var(--el-text-color-primary);
    font-weight: 600;
    font-size: 12px;
}

.soft-pill {
    display: inline-block;
    padding: 4px 10px;
    border-radius: 999px;
    background: var(--el-fill-color-dark);
    color: var(--el-text-color-secondary);
    font-weight: 600;
    font-size: 12px;
}

.markAllocated {
    background-color: var(--el-bg-color-white);
    border: none;
}

.markAllocated .el-icon {
    color: #28a745;
    font-size: larger;
}

.markComplete {
    background-color: var(--el-bg-color-white);
    border: none;
}

.markComplete .el-icon {
    color: #28a745;
    font-size: larger;
}
</style>
