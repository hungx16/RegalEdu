<template>
    <div>
        <!-- Header -->
        <FilterComponent ref="filterComponentRef" headerTitle="admissionsQuotaCompany.headerTitle"
            headerDesc="admissionsQuotaCompany.headerDesc" :disabledDelete="true" class="mb-6" />

        <!-- Card: Danh sách chỉ tiêu theo tháng (Company) -->
        <el-card class="mb-6 shadow-sm" body-class="p-0">
            <div class="card-header px-4 py-4 d-flex align-items-center justify-between">
                <div>
                    <h3 class="m-0">{{ t('admissionsQuotaCompany.listTitle') }}</h3>
                    <div class="text-muted small">{{ t('admissionsQuotaCompany.listFunction') }}</div>
                </div>
            </div>

            <!-- Bảng -->
            <div class="px-4 pb-4">
                <BaseTable :columns="columns" :items="filteredCompanies" :loading="admissionsQuotaCompanyStore.loading"
                    :showPagination="true" :page="page" :total="filteredCompanies.length" :pageSize="pageSize"
                    :filter="filter" @update:filter="onTableFilter" @update:rows="val => (selectedRows = val)"
                    @view="viewCompany" :showActionsColumn="true" :showEdit="false" :showDelete="false" :showView="true"
                    @update:page="val => (page = val)" @update:pageSize="onPageSizeChange">
                    <!-- Stage badge -->
                    <template #cell-quotaStage="{ item }">
                        <BaseBadge :label="t(statusKeyMap[item.quotaStage] ?? 'common.unknown')"
                            :color="statusColorMap[item.quotaStage] ?? 'gray'" bordered bold />
                    </template>

                    <!-- Tổng chỉ tiêu (link xanh) -->
                    <template #cell-totalRevenue="{ item }">
                        <el-link type="primary">{{ formatCurrency(item.totalRevenue) }}</el-link>
                    </template>

                    <!-- Thực hiện -->
                    <template #cell-actual="{ item }">
                        <span :class="item.actual ? 'text-primary fw-600' : 'text-muted'">
                            {{ formatCurrency(0) }}
                        </span>
                    </template>

                    <!-- Sale hiện tại + chênh lệch -->
                    <template #cell-currentSales="{ item }">
                        <div class="d-flex flex-column">
                            <span class="fw-semibold">
                                {{ Number(item.currentSales || 0) }} {{ t('admissionsQuota.salesUnit') }}
                            </span>
                            <span v-if="salesDiff(item) !== 0" class="fs-8"
                                :class="salesDiff(item) > 0 ? 'text-success' : 'text-danger'">
                                ({{ salesDiff(item) > 0 ? '+' : '−' }}{{ Math.abs(salesDiff(item)) }}
                                {{ t(salesDiff(item) > 0 ? 'admissionsQuota.newSales' : 'admissionsQuota.reducedSales')
                                }})
                            </span>
                        </div>
                    </template>

                    <!-- Tổng sale của chi nhánh (đậm) -->
                    <template #cell-numberOfSalesAllocated="{ item }">
                        <div class="fw-600">
                            {{ item.numberOfSalesAllocated }} {{ t('admissionsQuota.salesUnit') }}
                        </div>
                    </template>

                    <!-- Doanh thu / sale -->
                    <template #cell-revenuePerSale="{ item }">
                        <div>{{ formatCurrency(item.revenuePerSale) }}</div>
                    </template>
                    <template #actions="{ item }">
                        <div v-if="showSupport">
                            <el-tooltip :content="t('admissionsQuota.assignSupportTitle')" placement="top">
                                <el-button circle size="small" class="btn-no-background" @click="openSupport(item)">
                                    <el-icon>
                                        <User />
                                    </el-icon>
                                </el-button>
                            </el-tooltip>
                        </div>
                    </template>
                </BaseTable>
            </div>
        </el-card>
    </div>
    <QuotaSupportStaffDialog :visible="supportDlg.visible" :loading="formLoading" :target-name="supportDlg.targetName"
        :company-id="supportDlg.companyId" :quota-id="quotaId" :region-id="supportDlg.regionId"
        :admissions-quota-company-id="supportDlg.admissionsQuotaCompanyId"
        :admissions-quota-region-id="supportDlg.admissionsQuotaRegionId"
        :admissions-quota-id="supportDlg.admissionsQuotaId" @update:visible="v => (supportDlg.visible = v)"
        @submit="onSaveSupport" />
    <!-- Dialog xem chi tiết Company -->
    <AdmissionsQuotaCompanyDialog :visible="showCompanyDialog" mode="view" :company-id="selectedCompanyId"
        @update:visible="v => (showCompanyDialog = v)" />
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import FilterComponent from '@/components/filter-component/FilterComponent.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import AdmissionsQuotaCompanyDialog from './AdmissionsQuotaCompanyDialog.vue'
import { useAdmissionsQuotaCompanyStore } from '@/stores/admissionsQuotaCompanyStore'
import { formatCurrency, formatDate } from '@/utils/format'
import { QuotaStatus } from '@/types'
import { User } from '@element-plus/icons-vue'
import QuotaSupportStaffDialog from '@/components/admissions-quota-table/QuotaSupportStaffDialog.vue'
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout'
import { useAdmissionsQuotaStore } from '@/stores/admissionsQuotaStore'
import { useNotificationStore } from '@/stores/notificationStore'
import { useEmployeeStore } from '@/stores/employeeStore'
const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
const admissionsQuotaStore = useAdmissionsQuotaStore();
const notificationStore = useNotificationStore();
const employeeStore = useEmployeeStore();
const isRegionManager = ref(false);
const { t } = useI18n()
const admissionsQuotaCompanyStore = useAdmissionsQuotaCompanyStore()

// ===== refs / states =====
const filterComponentRef = ref()
const page = ref(1)
const pageSize = ref(20)
const filter = ref<Record<string, any>>({})
const selectedRows = ref<any[]>([])
const showCompanyDialog = ref(false)
const selectedCompanyId = ref<string | null>(null)

// ===== status maps =====
const statusKeyMap: Record<number, string> = {
    0: 'admissionsQuota.Draft',
    1: 'admissionsQuota.Allocated',
    2: 'admissionsQuota.InProgress',
    3: 'admissionsQuota.Completed'
}
const statusColorMap: Record<number, string> = {
    0: 'gray',
    1: 'blue',
    2: 'orange',
    3: 'green'
}
const filterOptions = [
    { label: 'common.all', value: '' },
    { label: 'admissionsQuota.Allocated', value: QuotaStatus.Allocated },
    { label: 'admissionsQuota.InProgress', value: QuotaStatus.InProgress },
    { label: 'admissionsQuota.Completed', value: QuotaStatus.Completed }
]
const supportDlg = ref({
    visible: false,
    companyId: '' as string,
    targetName: '' as string,
    admissionsQuotaCompanyId: '' as string,
    admissionsQuotaRegionId: '' as string,
    admissionsQuotaId: '' as string,
    regionId: '' as string
})
const quotaId = computed(() => supportDlg.value.admissionsQuotaId)
// ===== columns =====
const columns: BaseTableColumn[] = [
    { key: 'monthYear', labelKey: 'admissionsQuota.monthYear', width: 180, filterType: 'text', sortable: true, align: 'center' },
    { key: 'regionName', labelKey: 'region.name', width: 200, align: 'center' },
    { key: 'companyName', labelKey: 'company.name', width: 220 },
    { key: 'totalRevenue', labelKey: 'admissionsQuotaCompany.totalTarget', width: 160, align: 'right' },
    { key: 'actual', labelKey: 'admissionsQuotaCompany.actual', width: 140, align: 'right' },
    { key: 'currentSales', labelKey: 'admissionsQuotaCompany.currentSales', width: 180, align: 'center' },
    { key: 'numberOfSalesAllocated', labelKey: 'admissionsQuotaCompany.totalSalesInCompany', width: 180, align: 'center' },
    { key: 'revenuePerSale', labelKey: 'admissionsQuotaCompany.revenuePerSale', width: 160, align: 'right' },
    { key: 'quotaStage', labelKey: 'common.status', width: 160, filterType: 'select', filterOptions, sortable: true },
    { key: 'createdBy', labelKey: 'common.createdBy', width: 140 },
    { key: 'updatedAt', labelKey: 'common.updatedAt', width: 140, formatter: (value: string) => formatDate(value, 'DD/MM/YYYY'), align: 'center' },
    { key: 'actions', labelKey: 'common.actions', width: 100, align: 'right' }
]
const showSupport = computed(() => isRegionManager.value)
// ===== normalized data =====
const companiesNormalized = computed(() =>
    (admissionsQuotaCompanyStore.companies || []).map((x: any) => ({
        id: x.id,
        monthYear: (x?.admissionsQuota?.month && x?.admissionsQuota?.year)
            ? `${String(x.admissionsQuota.month).padStart(2, '0')}/${x.admissionsQuota.year}`
            : '',
        companyName: x?.company?.companyName ?? x.companyName ?? '',
        regionName: x?.admissionsQuotaRegion?.region?.regionName ?? '',
        totalRevenue: x.totalRevenue ?? 0,
        currentSales: x.currentSales ?? 0,
        numberOfSalesAllocated: x.numberOfSalesAllocated ?? 0,
        revenuePerSale: x.revenuePerSale ?? 0,
        createdBy: x.createdBy ?? '—',
        createdAt: x.createdAt ?? '',
        updatedAt: x.updatedAt ?? '',
        quotaStage: x?.admissionsQuota?.quotaStage ?? 0,
        admissionsQuotaEmployees: x.admissionsQuotaEmployees || [],
        admissionsQuotaAdjustments: x.admissionsQuotaAdjustments || [],
        admissionsQuota: x.admissionsQuota || {},
        regionId: x?.admissionsQuotaRegion?.region?.id ?? '',
        admissionsQuotaRegionId: x?.admissionsQuotaRegion?.id ?? '',
        company: x.company || {}
    }))
)

const filteredCompanies = computed(() => {
    let arr = companiesNormalized.value
    Object.entries(filter.value).forEach(([key, val]) => {
        if (val !== '' && val != null) {
            if (key === 'createdAt') {
                arr = arr.filter((item: any) => String(item.createdAt ?? '').startsWith(String(val)))
            } else if (key === 'status') {
                arr = arr.filter((item: any) => String(item.status) === String(val))
            } else {
                arr = arr.filter((item: any) =>
                    String(item[key] ?? '').toLowerCase().includes(String(val).toLowerCase())
                )
            }
        }
    })
    return arr
})

// ===== lifecycle =====
onMounted(async () => {
    filterComponentRef.value?.initListHeaderParams({ listParams: [], listBtn: [] })
    // có thể dùng fetchCompanies (paged) nếu muốn
    await admissionsQuotaCompanyStore.fetchAllAdmissionsQuotaCompanies()
    isRegionManager.value = await employeeStore.checkIsRegionManager()
    console.log(isRegionManager.value);

})

// ===== handlers =====
function onPageSizeChange(newSize: number) {
    pageSize.value = newSize
    page.value = 1
}

function onTableFilter(val: Record<string, any>) {
    filter.value = val
    page.value = 1
}

function viewCompany(item: any) {
    selectedCompanyId.value = item.id // AdmissionsQuotaCompanyId
    showCompanyDialog.value = true
}

const salesDiff = (row: any) =>
    Number(row.numberOfSalesAllocated || 0) - Number(row.currentSales || 0)

function openSupport(item: any) {
    console.log(item);

    supportDlg.value.companyId = item?.company.id || '';
    supportDlg.value.targetName = item?.companyName || '';
    supportDlg.value.admissionsQuotaCompanyId = item?.id || '';
    supportDlg.value.admissionsQuotaId = item?.admissionsQuota.id || '';
    supportDlg.value.regionId = item?.regionId || '';
    supportDlg.value.admissionsQuotaRegionId = item?.admissionsQuotaRegionId || '';
    supportDlg.value.visible = true;
}
async function onSaveSupport(payload: any) {
    try {
        startLoading()
        await admissionsQuotaStore.assignSupportStaff(payload)
        notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('common.employee') } })
        await admissionsQuotaCompanyStore.fetchAllAdmissionsQuotaCompanies()
    } catch (e: any) {
        console.error('Error saving:', e?.response?.data?.errors || e);
    } finally {
        stopLoading()
        supportDlg.value.visible = false

    }
}
</script>

<style scoped>
.card-header {
    border-bottom: 1px solid rgba(0, 0, 0, .06);
}

.fw-600 {
    font-weight: 600;
}

.text-12 {
    font-size: 12px;
}

.leading-tight {
    line-height: 1.2;
}

.w-44 {
    width: 176px;
}
</style>
