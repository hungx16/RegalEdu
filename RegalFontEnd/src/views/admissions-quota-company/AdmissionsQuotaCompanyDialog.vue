<template>
    <BaseDialogForm :visible="visible" :title="modeTitle" :mode="mode" :form-data="formData" :rules="rules"
        :loading="loading" :submit-disabled="true" :width="computedDialogWidth" class="admissions-quota-company-dialog"
        @update:visible="$emit('update:visible', $event)" @close="$emit('close')">
        <template #form>
            <!-- TOP: Month/Year + Note (view only) -->
            <div class="row g-4 align-items-end">
                <div class="col-12 col-md-3">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('admissionsQuota.month') }}</label>
                    <el-form-item prop="month">
                        <el-input :model-value="formData.month" disabled />
                    </el-form-item>
                </div>
                <div class="col-12 col-md-3">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('admissionsQuota.year') }}</label>
                    <el-form-item prop="year">
                        <el-input :model-value="formData.year" disabled />
                    </el-form-item>
                </div>
                <div class="col-12 col-md-6">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.note') }}</label>
                    <el-form-item prop="note">
                        <el-input :model-value="formData.note" :placeholder="t('admissionsQuota.notePlaceholder')"
                            disabled />
                    </el-form-item>
                </div>
            </div>

            <!-- SUMMARY -->
            <div class="row g-4 mt-2 mb-6">
                <div class="col-12 col-xl-6">
                    <div class="summary-card p-6 rounded-4 shadow-sm bg-body h-100">
                        <div class="d-flex justify-content-between align-items-start mb-1">
                            <span class="fw-semibold fs-6">{{ t('admissionsQuota.totalAllocatedRevenue') }}</span>
                            <i class="bi bi-cash-coin fs-4 text-body-secondary"></i>
                        </div>
                        <div class="fs-2 fw-bold">{{ formatCurrency(totalRevenue) }}</div>
                        <div class="fs-8 text-body-secondary">{{ t('admissionsQuota.totalAllocatedRevenueHint') }}</div>
                    </div>
                </div>
                <div class="col-12 col-xl-6">
                    <div class="summary-card p-6 rounded-4 shadow-sm bg-body h-100">
                        <div class="d-flex justify-content-between align-items-start mb-1">
                            <span class="fw-semibold fs-6">{{ t('admissionsQuota.allocationInfo') }}</span>
                            <i class="bi bi-info-circle fs-4 text-body-secondary"></i>
                        </div>
                        <div class="d-flex gap-8 flex-wrap">
                            <div>
                                <div class="fs-3 fw-bold">{{ currentSales }}</div>
                                <div class="fs-8 text-body-secondary">{{ t('admissionsQuota.currentSales') }}</div>
                            </div>
                            <div>
                                <div class="fs-3 fw-bold">{{ numberOfSalesAllocated }}</div>
                                <div class="fs-8 text-body-secondary">{{ t('admissionsQuota.salesTotal') }}</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- COMPANY HEADER -->
            <div class="card mb-6">
                <div class="card-header d-flex align-items-center justify-content-between">
                    <div class="card-title">
                        <h3 class="fs-5 fw-bold m-0">{{ t('admissionsQuota.tableCompany') }}</h3>
                    </div>
                    <div class="d-flex align-items-center gap-6 pe-2">
                        <div class="text-muted fs-8" v-if="companyName || regionName">
                            <div v-if="companyName">
                                {{ t('company.name') }}: <strong>{{ companyName }}</strong>
                            </div>
                            <div v-if="regionName">
                                {{ t('region.name') }}: <strong>{{ regionName }}</strong>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- EMPLOYEE TABLE (view only) -->
                <div class="card-body p-2">
                    <QuotaEmployeeTable :quota-id="formData.admissionsQuotaId" :month="formData.month"
                        :year="formData.year" :rows="employeeRows" :disabled="true" />
                </div>
            </div>
        </template>

        <template #footer-extra />
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import QuotaEmployeeTable from '@/components/admissions-quota-table/QuotaEmployeeTable.vue'
import { formatCurrency } from '@/utils/format'
import { useAdmissionsQuotaCompanyStore } from '@/stores/admissionsQuotaCompanyStore'

interface Props {
    visible: boolean
    mode: 'create' | 'edit' | 'view'
    companyId?: string | null
}
const props = defineProps<Props>()
const emit = defineEmits(['update:visible', 'close'])

const { t } = useI18n()
const loading = ref(false)

const formData = ref<any>({
    admissionsQuotaId: '',
    month: 0,
    year: 0,
    note: '',
    quotaStage: 0
})
const rules = {}
const computedDialogWidth = computed(() => (window.innerWidth < 768 ? '100%' : '80%'))
const modeTitle = computed(() => t('admissionsQuota.viewTitle'))

// header info
const companyName = ref<string>('')
const regionName = ref<string>('')

// summary metrics
const currentSales = ref<number>(0)
const numberOfSalesAllocated = ref<number>(0)
const revenuePerSale = ref<number>(0)
const totalRevenue = ref<number>(0)

// rows
const employeeRows = ref<any[]>([])

// store
const admissionsQuotaCompanyStore = useAdmissionsQuotaCompanyStore()

// normalize employee row
function normalizeEmp(e: any, comp: any, region?: any) {
    return {
        id: e.id ?? null,
        employeeId: e.employeeId,
        employeeCode: e.employee?.applicationUser?.userCode ?? e.employeeCode ?? '',
        fullName: e.employee?.applicationUser?.fullName ?? e.fullName ?? '',
        positionId: e.positionId,
        positionName: e.position?.positionName ?? e.positionName ?? '',
        regionId: region?.regionId ?? e.regionId ?? null,
        regionName: region?.region?.regionName ?? e.regionName ?? '',
        companyId: comp?.companyId ?? e.companyId ?? null,
        companyName: comp?.company?.companyName ?? e.company?.companyName ?? '',
        quotaRole: e.quotaRole ?? null,
        workStage: e.workStage ?? null,
        revenuePerSale: Number(e.revenuePerSale ?? 0),
        joinedAt: e.joinAt ?? e.joinedAt ?? null,
        endAt: e.endAt ?? null,
        probationEnd: e.probationEnd ?? null,
        note: e.note ?? ''
    }
}

async function loadCompanyDetail(id?: string | null) {
    if (!id) return
    loading.value = true
    try {
        // gọi theo tên hàm có sẵn trong store của bạn
        await admissionsQuotaCompanyStore.fetchAdmissionsQuotaCompanyById(id)
        console.log('admissionsQuotaCompanyStore.selectedCompany', admissionsQuotaCompanyStore.selectedCompany);

        const company =
            admissionsQuotaCompanyStore.selectedCompany ||

            null

        if (!company) return

        // header
        companyName.value = company?.company?.companyName ?? ''
        regionName.value = company?.admissionsQuotaRegion?.region?.regionName ?? ''

        // tháng/năm/note
        const aq = company?.admissionsQuota ??
            company?.admissionsQuotaRegion?.admissionsQuota ??
            null

        formData.value = {
            admissionsQuotaId: aq?.id ?? company?.admissionsQuotaId ?? company?.admissionsQuotaRegion?.admissionsQuotaId ?? '',
            month: aq?.month ?? '',
            year: aq?.year ?? '',
            note: aq?.note ?? '',
            quotaStage: aq?.quotaStage ?? 0
        }

        // metrics
        currentSales.value = Number(company?.currentSales ?? 0)
        numberOfSalesAllocated.value = Number(company?.numberOfSalesAllocated ?? 0)
        revenuePerSale.value = Number(company?.revenueQuotaPerSale ?? 0)
        totalRevenue.value =
            Number(company?.totalRevenue ??
                (Number(company?.numberOfSalesAllocated ?? 0) * Number(company?.revenueQuotaPerSale ?? 0)))

        // employees
        const fromCompany = (company?.admissionsQuotaEmployees ?? []).map((e: any) =>
            normalizeEmp(e, company, company?.admissionsQuotaRegion)
        )
        employeeRows.value = fromCompany
    } finally {
        loading.value = false
    }
}

watch(
    () => [props.visible, props.mode, props.companyId],
    async ([v, m, id]) => {
        if (v && m === 'view' && typeof id === 'string') {
            await loadCompanyDetail(id)
        }
    },
    { immediate: false }
)
</script>

<style scoped>
.summary-card {
    min-height: 120px;
}

.fw-600 {
    font-weight: 600;
}
</style>
