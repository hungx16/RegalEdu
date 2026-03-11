<template>
  <div class="my-quota-page">
    <el-card class="mb-6 shadow-sm" body-class="p-0">
      <div class="card-header px-4 py-4 d-flex align-items-center flex-wrap gap-3">
        <div>
          <h3 class="m-0">{{ t('myQuota.title') }}</h3>
          <div class="text-muted small">{{ t('myQuota.subtitle') }}</div>
        </div>
        <div class="d-flex align-items-center gap-3 header-actions">
          <el-select v-model="allocationFilter" filterable clearable class="alloc-select"
            :placeholder="t('myQuota.filterPlaceholder')">
            <el-option v-for="opt in allocationOptions" :key="opt.id" :label="opt.label" :value="opt.id" />
          </el-select>
        </div>
      </div>

      <div class="px-4 pb-4">
        <el-alert v-if="errorMsg" type="error" :closable="false" show-icon class="mb-3">
          {{ errorMsg }}
        </el-alert>
        <div v-if="allocationFilter && allocationTotals.length" class="mb-3">
          <div class="fw-semibold mb-2">{{ t('myQuota.totalAllocation') }}</div>
          <el-table :data="allocationTotals" size="small" border class="total-table">
            <el-table-column prop="label" :label="t('myQuota.allocation')" />
            <el-table-column :label="t('myQuota.quota')" align="right">
              <template #default="{ row }">
                {{ formatCurrency(row.total) }}
              </template>
            </el-table-column>
          </el-table>
        </div>
        <BaseTable class="my-quota-table" :loading="loading" :items="tableRows" :columns="columns"
          :showPagination="false" :showIndex="false" :showActionsColumn="false">
          <template #cell-stt="{ item }">
            {{ item.stt }}
          </template>
          <template #cell-role="{ item }">
            {{ item.position?.positionName || item.quotaRole || '' }}
          </template>
          <template #cell-company="{ item }">
            {{ companyName(item) }}
          </template>
          <template #cell-start="{ item }">
            {{ formatDate(item.start) }}
          </template>
          <template #cell-end="{ item }">
            {{ formatDate(item.end) }}
          </template>
          <template #cell-quota="{ item }">
            {{ formatCurrency(item.quota) }}
          </template>
          <template #cell-note="{ item }">
            {{ item.note }}
          </template>
        </BaseTable>
      </div>
    </el-card>
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import dayjs from 'dayjs'
import { formatCurrency } from '@/utils/format'
import { AdmissionsQuotaApi } from '@/api/AdmissionsQuotaApi'
import type { AdmissionsQuotaEmployeeModel } from '@/api/AdmissionsQuotaCompanyApi'
import BaseTable from '@/components/table/BaseTable.vue'
import type { BaseTableColumn } from '@/components/table/BaseTable.vue'

const { t } = useI18n()
const loading = ref(false)
const rows = ref<AdmissionsQuotaEmployeeModel[]>([])
const errorMsg = ref<string | null>(null)
const api = new AdmissionsQuotaApi()
const allocationFilter = ref<string | null>(null)

const allocationOptions = computed(() => {
  const seen = new Set<string>()
  return rows.value
    .map((r) => {
      const id = (r as any).admissionsQuotaId || r.admissionsQuota?.id || ''
      const label = r.admissionsQuota?.month && r.admissionsQuota?.year
        ? `${String(r.admissionsQuota.month).padStart(2, '0')}/${r.admissionsQuota.year}`
        : ''
      return { id, label }
    })
    .filter((opt) => {
      if (!opt.id || !opt.label) return false
      if (seen.has(opt.id)) return false
      seen.add(opt.id)
      return true
    })
})

const tableRows = computed(() => {
  const data = rows.value
    .filter((r) => {
      if (allocationFilter.value && String(allocationFilter.value) !== String((r as any).admissionsQuotaId ?? r.admissionsQuota?.id ?? '')) return false
      return true
    })
    .map((r) => ({
      ...r,
      monthYear: r.admissionsQuota?.month && r.admissionsQuota?.year
        ? `${String(r.admissionsQuota.month).padStart(2, '0')}/${r.admissionsQuota.year}`
        : '',
      regionDisplay: regionName(r),
      companyDisplay: companyName(r),
      start: r.allocationStartAt,
      end: r.allocationEndAt,
      quota: toQuotaValue(r),
    }))
    .sort((a, b) => (b.admissionsQuota?.year ?? 0) - (a.admissionsQuota?.year ?? 0) || (b.admissionsQuota?.month ?? 0) - (a.admissionsQuota?.month ?? 0))
    .map((item, index) => ({ ...item, stt: index + 1 }))
  return data
})

const allocationTotals = computed(() => {
  const map = new Map<string, { id: string; label: string; total: number }>()
  tableRows.value.forEach((r: any) => {
    const id = String(r.admissionsQuotaId ?? r.admissionsQuota?.id ?? '')
    if (!id) return
    const label = r.monthYear || ''
    const found = map.get(id) ?? { id, label, total: 0 }
    found.total += Number(r.quota ?? 0)
    map.set(id, found)
  })
  return Array.from(map.values())
})

const columns = computed<BaseTableColumn[]>(() => [
  { key: 'stt', labelKey: 'common.index', width: 70, align: 'center', headerAlign: 'center' },
  { key: 'monthYear', labelKey: 'myQuota.monthYear', width: 140, align: 'center', sortable: true },
  { key: 'regionDisplay', labelKey: 'models.Region', minWidth: 160, },
  { key: 'companyDisplay', labelKey: 'models.Company', minWidth: 180, },
  { key: 'role', labelKey: 'myQuota.role', minWidth: 160 },
  { key: 'quota', labelKey: 'myQuota.quota', width: 160, align: 'right' },

  { key: 'start', labelKey: 'common.startDate', width: 130 },
  { key: 'end', labelKey: 'common.endDate', width: 130 },
  { key: 'note', labelKey: 'common.note', minWidth: 180 },
])

async function loadData() {
  errorMsg.value = null
  loading.value = true
  try {
    const res = await api.getMyAdmissionsQuota()
    if (res?.succeeded && res.data) {
      rows.value = res.data
    } else {
      errorMsg.value = res?.message || t('common.somethingWentWrong')
    }
  } catch (err: any) {
    errorMsg.value = err?.message || t('common.somethingWentWrong')
  } finally {
    loading.value = false
  }
}

function formatDate(val?: string | null) {
  if (!val) return ''
  return dayjs(val).format('YYYY-MM-DD')
}

function toQuotaValue(row: AdmissionsQuotaEmployeeModel) {
  return Number(row.revenueQuota ?? (row as any).revenuePerSale ?? 0)
}

function companyName(row: AdmissionsQuotaEmployeeModel) {
  return (
    row.company?.companyName ||
    row.admissionsQuotaCompany?.company?.companyName ||
    (row.admissionsQuotaCompany as any)?.companyName ||
    ''
  )
}

function regionName(row: AdmissionsQuotaEmployeeModel) {
  return (
    row.region?.regionName ||
    row.admissionsQuotaRegion?.region?.regionName ||
    (row.admissionsQuotaRegion as any)?.regionName ||
    ''
  )
}

onMounted(loadData)
</script>


<style scoped>
.my-quota-page {
  padding: 0 0.25rem;
}

.my-quota-table :deep(.el-table__cell) {
  padding: 10px 12px;
}

.alloc-select {
  min-width: 200px;
}
.header-actions {
  margin-left: auto;
}
.total-table :deep(.el-table__cell) {
  padding: 6px 8px;
}
</style>

