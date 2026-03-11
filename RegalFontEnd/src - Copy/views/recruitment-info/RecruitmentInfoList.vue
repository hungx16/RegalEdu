<template>
    <div>
        <!-- Header FilterComponent -->
        <FilterComponent ref="filterComponentRef" class="mb-6" headerTitle="recruitmentInfo.headerTitle"
            headerDesc="recruitmentInfo.headerDesc" :disabledDelete="getDisableDelete" @add="addModelEvent"
            @delete="onDeleteClicked" />

        <!-- Summary Cards -->
        <div class="row g-4 mb-8">
            <div class="col-12 col-md-6">
                <div class="summary-card bg-body text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('recruitmentInfo.totalJobs') }}</span>
                        <i class="bi bi-briefcase fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ infoStore.recruitmentInfoList.length }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('recruitmentInfo.position') }}</div>
                </div>
            </div>

            <div class="col-12 col-md-6">
                <div class="summary-card bg-dark-1 text-body p-6 rounded-4 shadow-sm h-100">
                    <div class="d-flex justify-content-between align-items-start mb-2">
                        <span class="fw-semibold fs-5">{{ t('recruitmentInfo.totalActive') }}</span>
                        <i class="bi bi-lightning-charge fs-4 text-body-secondary"></i>
                    </div>
                    <div class="fs-2 fw-bold mb-1">{{ totalActive }}</div>
                    <div class="fs-7 text-body-secondary">{{ t('common.active') }}</div>
                </div>
            </div>
        </div>

        <!-- Data Table Card -->
        <div class="card mb-10 w-100 mt-5">
            <div class="card-header card-header-stretch">
                <div class="card-title d-flex flex-column">
                    <h3 class="fw-bold fs-4 mb-1">{{ t('recruitmentInfo.listTitle') }}</h3>
                    <span class="text-body-secondary fw-light fs-8">{{ t('recruitmentInfo.listFunction') }}</span>
                </div>
            </div>

            <div class="card-body py-6 px-2 px-md-6">
                <BaseTable :showCheckboxColumn="true" :showActionsColumn="true" :showEdit="true" :showDelete="true"
                    :showView="true" :columns="columns" :items="pagedItems" :loading="infoStore.loading"
                    :showPagination="true" :page="page" :total="filteredInfosAll.length" :pageSize="pageSize"
                    :filter="filter" @update:filter="onTableFilter" @update:rows="val => selectedRowsData = val"
                    @edit="editModelEvent" @view="viewModelEvent" @delete="handleDelete"
                    @update:page="val => page = val" @update:pageSize="onPageSizeChange">
                    <!-- Status badge -->
                    <template #cell-status="{ item }">
                        <BaseBadge type="boolean" :label="item.status === 0 ? t('common.active') : t('common.inactive')"
                            :value="item.status === 0" />
                    </template>

                    <!-- Province badge -->
                    <template #cell-provinceName="{ item }">
                        <BaseBadge :label="item.provinceName" color="blue" soft bordered />
                    </template>
                    <!-- Department badge -->
                    <template #cell-departmentName="{ item }">
                        <BaseBadge :label="item.departmentName" color="green" soft bordered />
                    </template>
                    <!-- Salary badge -->
                    <template #cell-salary="{ item }">
                        {{ formatCurrency(item.salary) }}
                    </template>
                    <!-- CreatedAt -->
                    <template #cell-createdAt="{ item }">
                        {{ item.createdAt ? formatDate(item.createdAt, 'DD/MM/YYYY HH:mm') : '-' }}
                    </template>
                </BaseTable>
            </div>
        </div>

        <!-- Dialog -->
        <RecruitmentInfoDialog v-model:visible="showFormModal" :mode="dialogMode"
            :recruitment-info-data="infoStore.selectedRecruitmentInfo" :loading="formLoading" @submit="handleSave"
            @delete="handleDelete" />
    </div>
</template>

<script setup lang="ts">
/** ===== IMPORTS ===== */
import { onMounted, ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRecruitmentInfoStore } from '@/stores/recruitmentInfoStore'
import { useNotificationStore } from '@/stores/notificationStore'
import { useCommonStore } from '@/stores/commonStore'
import FilterComponent from '@/components/filter-component/FilterComponent.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import RecruitmentInfoDialog from '@/views/recruitment-info/RecruitmentInfoDialog.vue'
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout'
import { formatCurrency, formatDate } from '@/utils/format';
import { useDepartmentStore } from '@/stores/departmentStore'

/** ===== INIT ===== */
const { t } = useI18n()
const infoStore = useRecruitmentInfoStore()
const notificationStore = useNotificationStore()
const commonStore = useCommonStore()
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
const departmentStore = useDepartmentStore();

const filterComponentRef = ref()
const showFormModal = ref(false)
const dialogMode = ref<'create' | 'edit' | 'view'>('create')

const page = ref(1)
const pageSize = ref(30)
const filter = ref<Record<string, any>>({})
const selectedRowsData = ref<any[]>([])
const departmentFilterOptions = ref<{ label: string; value: any }[]>([]);

/** ===== HEADER BTNS ===== */
const listHeaderParams = {
    listParams: [],
    listBtn: [
        { event: 'add', label: 'recruitmentInfo.add', type: 'add' },
        { event: 'delete', label: 'recruitmentInfo.delete', type: 'delete' },
    ],
}

/** ===== PROVINCE OPTIONS (from commonStore) ===== */
const provinceFilterOptions = ref<{ label: string; value: any }[]>([]);
/** ===== COLUMNS (computed để lấy filterOptions động) ===== */
const columns = computed<BaseTableColumn[]>(() => ([
    { key: 'recruitmentInfoName', labelKey: 'recruitmentInfo.name', filterType: 'text', sortable: true, sticky: true, width: 240, isBold: true },
    {
        key: 'departmentName', labelKey: 'models.Department', filterType: 'select', sortable: true, width: 180, align: 'center',
        filterOptions: departmentFilterOptions.value
    },
    { key: 'position', labelKey: 'recruitmentInfo.position', filterType: 'text', sortable: true, width: 200, align: 'center' },
    { key: 'experience', labelKey: 'recruitmentInfo.experience', filterType: 'text', sortable: false, width: 160, align: 'center' },
    { key: 'salary', labelKey: 'recruitmentInfo.salary', filterType: 'text', sortable: false, width: 140, align: 'center' },
    {
        key: 'provinceName', labelKey: 'employee.province', filterType: 'select', sortable: true, width: 200, align: 'center',
        filterOptions: provinceFilterOptions.value
    },
    {
        key: 'status', labelKey: 'common.status', filterType: 'select', sortable: true, width: 160, align: 'center',
        filterOptions: [
            { label: 'common.all', value: '' },
            { label: 'common.active', value: 0 },
            { label: 'common.inactive', value: 1 },
        ]
    },
    { key: 'createdBy', labelKey: 'common.createdBy', filterType: 'text', sortable: true, width: 220 },
    {
        key: 'createdAt', labelKey: 'common.createdAt', filterType: 'date', sortable: true, width: 200, align: 'center',
    },
    { key: 'actions', labelKey: 'common.actions', width: 220 },
]))
watch(
    () => departmentStore.departments,
    (departments) => {
        departmentFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...departments.map(department => ({
                label: department.departmentName,
                value: department.departmentName,
                isLocale: false,
            })),
            {
                label: t('common.none'),
                value: t('common.none'),
                isLocale: false,
            }
        ];
    },
    { immediate: true }
);
watch(
    () => commonStore.provinces,
    (provinces) => {
        provinceFilterOptions.value = [
            { label: ('common.all'), value: '' },
            ...provinces.map(province => ({
                label: province.provinceName,
                value: province.provinceName,
                isLocale: false,
            }))
            ,
            {
                label: t('common.none'),
                value: t('common.none'),
                isLocale: false,
            }
        ];
    },
    { immediate: true }
);
/** ===== NORMALIZE DATA ===== */
const infosNormalized = computed(() =>
    (infoStore.recruitmentInfoList || []).map((x: any) => ({
        ...x,
        recruitmentInfoName:
            x.recruitmentInfoName ??
            '',
        provinceName: commonStore.provinces.find(p => p.provinceCode === x.provinceCode)?.provinceName ?? t('common.none'),
        departmentName: x.department?.departmentName ?? t('common.none'),
        status: typeof x.status === 'number' ? x.status : 0,
    }))
)

/** ===== SUMMARY ===== */
const totalActive = computed(() => infosNormalized.value.filter(i => i.status === 0).length)

/** ===== FILTER + PAGINATION (client) ===== */
const filteredInfosAll = computed(() => {
    let arr = infosNormalized.value

    Object.entries(filter.value).forEach(([key, val]) => {
        if (val === '' || val == null) return

        if (key === 'createdAt') {
            arr = arr.filter(item => {
                if (!item.createdAt) return false
                const d = String(item.createdAt).substring(0, 10)
                return d === String(val)
            })
        } else if (key === 'status') {
            arr = arr.filter(i => Number(i.status) === Number(val))
        } else {
            arr = arr.filter(item =>
                String((item as any)[key] ?? '').toLowerCase().includes(String(val).toLowerCase())
            )
        }
    })

    return arr
})

const pagedItems = computed(() => {
    const start = (page.value - 1) * pageSize.value
    return filteredInfosAll.value.slice(start, start + pageSize.value)
})

/** ===== UTILS ===== */
function displayProvince(code?: string) {
    if (!code) return '-'
    const f = (commonStore.provinces || []).find(x => x.provinceCode === code)
    return f?.provinceName ?? code
}

/** ===== ACTIONS ===== */
const getDisableDelete = computed(() => selectedRowsData.value.length === 0)

function onTableFilter(val: Record<string, any>) {
    filter.value = val
    page.value = 1
}

function onPageSizeChange(newSize: number) {
    pageSize.value = newSize
    page.value = 1
}

function onDeleteClicked() {
    if (!selectedRowsData.value.length) {
        notificationStore.showToast('warning', { key: 'toast.noSelected', params: { model: t('models.recruitmentInfo') } })
        return
    }
    handleDelete(selectedRowsData.value)
}

function idsFrom(items: any[] | any) {
    const list = Array.isArray(items) ? items : [items]
    return list.filter(i => typeof i.id === 'string' && i.id).map(i => i.id as string)
}

function editModelEvent(item: any) {
    dialogMode.value = 'edit'
    infoStore.selectRecruitmentInfo({ ...item })
    showFormModal.value = true
}

function viewModelEvent(item: any) {
    dialogMode.value = 'view'
    infoStore.selectRecruitmentInfo({ ...item })
    showFormModal.value = true
}

async function addModelEvent() {
    dialogMode.value = 'create'
    infoStore.selectRecruitmentInfo(null)
    showFormModal.value = true
}

function handleDelete(items: any[] | any) {
    const ids = idsFrom(items)
    if (!ids.length) return

    notificationStore.showConfirm(
        { key: 'toast.delete', params: { model: t('models.recruitmentInfo') } },
        async () => {
            startLoading()
            try {
                await infoStore.deleteRecruitmentInfo(ids)
                await infoStore.fetchAllRecruitmentInfo()
                notificationStore.showToast('success', { key: 'toast.deleteSuccess', params: { model: t('models.recruitmentInfo') } })
                showFormModal.value = false
            } catch (err: any) {
                console.error('Delete error:', err?.response?.data?.errors || err)
            } finally {
                stopLoading()
            }
        }
    )
}

async function handleSave(payload: any) {
    startLoading()
    try {
        await infoStore.saveRecruitmentInfo(payload)
        await infoStore.fetchAllRecruitmentInfo()
        const msgKey = payload.id ? 'toast.updateSuccess' : 'toast.createSuccess'
        notificationStore.showToast('success', { key: msgKey, params: { model: t('models.recruitmentInfo') } })
        showFormModal.value = false
    } catch (err: any) {
        console.error('Save error:', err?.response?.data?.errors || err)
    } finally {
        stopLoading()
    }
}

/** ===== LIFECYCLE ===== */
onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams(listHeaderParams)
    // Load provinces từ CommonApi qua commonStore
    if (!commonStore.provinces?.length) {
        await commonStore.fetchProvinces()
    }
    await infoStore.fetchAllRecruitmentInfo()
})
</script>
