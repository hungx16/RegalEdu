<template>
  <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
    :width="computedDialogWidth" :mode="mode" :loading="submitting" :submit-disabled="actionsDisabled"
    :delete-disabled="actionsDisabled" @submit="onSubmit" @delete="onDelete"
    @update:visible="emit('update:visible', $event)">
    <template #form>
      <!-- ===== THÔNG TIN CƠ BẢN ===== -->
      <el-row :gutter="20" class="pb-4 border-bottom mb-4">
        <!-- Mã phân bổ: tự sinh, luôn disabled -->
        <el-col :span="6">
          <label class="required fw-semibold">{{ t('allocationEvent.code') }}</label>
          <el-form-item prop="allocationCode" class="mb-0">
            <el-input v-model="formData.allocationCode" :disabled="true"
              :placeholder="t('allocationEvent.autoCodeAfterPick')" />
          </el-form-item>
        </el-col>

        <!-- Tháng -->
        <el-col :span="6">
          <label class="required fw-semibold">{{ t('allocationEvent.month') }}</label>
          <el-form-item prop="allocationMonth" class="mb-0">
            <el-input-number v-model="formData.allocationMonth" :min="1" :max="12" :disabled="isView" />
          </el-form-item>
        </el-col>

        <!-- Năm -->
        <el-col :span="6">
          <label class="required fw-semibold">{{ t('allocationEvent.year') }}</label>
          <el-form-item prop="allocationYear" class="mb-0">
            <el-input-number v-model="formData.allocationYear" :min="2000" :max="2100" :disabled="isView" />
          </el-form-item>
        </el-col>

        <!-- Ngân sách / chi nhánh (cố định cho mỗi chi nhánh) -->
        <el-col :span="6">
          <label class="required fw-semibold">{{ t('allocationEvent.eventBudget') }}</label>
          <el-form-item prop="eventBudget" class="mb-0">
            <CurrencyInput v-model="formData.eventBudget" :disabled="isView" currency="VND" locale="vi-VN" bordered
              background="#fff" textColor="#111" :placeholder="t('allocationEvent.enterBudget')" />
          </el-form-item>
        </el-col>
      </el-row>

      <!-- === HIỂN THỊ BẢNG KHI ĐÃ CHỌN THÁNG + NĂM === -->
      <template v-if="showMatrix">
        <!-- ===== THỐNG KÊ THEO VÙNG ===== -->
        <div class="p-4 bg-light rounded-3 shadow-sm mb-6">
          <BaseTable :columns="regionColumns" :items="regionRows" :showCheckboxColumn="false" :showActionsColumn="false"
            :showIndex="true" :height="300">
            <!-- Cột vùng -->
            <template #cell-regionName="{ item }">
              <span class="fw-semibold">{{ item.regionName }}</span>
            </template>
            <!-- Nếu muốn chắc chắn hiển thị đúng field -->
            <template #cell-appliedBranchCount="{ item }">
              <span class="fw-semibold">{{ item.appliedBranchCount }}</span>
            </template>

            <!-- Cột động: từng sự kiện (tổng theo vùng) -->
            <template v-for="ev in eventOptions" :key="'region-cell-' + ev.value"
              v-slot:[`cell-r-${ev.value}`]="{ item }">
              <span class="fw-semibold">{{ item.eventTotals[String(ev.value)] ?? 0 }}</span>
            </template>

            <!-- Tổng sự kiện vùng -->
            <template #cell-regionTotalEvents="{ item }">
              <span class="fw-bold text-primary">{{ item.totalEvents }}</span>
            </template>

            <!-- Tổng ngân sách vùng (theo số chi nhánh 'được áp') -->
            <template #cell-regionTotalBudget="{ item }">
              <CurrencyInput :model-value="item.totalBudget" currency="VND" locale="vi-VN" bordered disabled
                background="#f5f7fa" textColor="#2e7d32" rounded="sm" />
            </template>
          </BaseTable>
        </div>

        <!-- ===== PHÂN BỔ CHO CHI NHÁNH ===== -->
        <div class="p-4 bg-light rounded-3 shadow-sm">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <h5 class="fw-semibold mb-0">{{ t('allocationEvent.companyAllocation') }}</h5>
            <small class="text-muted">{{ t('allocationEvent.matrixGuide') }}</small>
          </div>

          <BaseTable :columns="tableColumns" :items="filteredCompanies" :showCheckboxColumn="false"
            :showActionsColumn="false" :height="550" :loading="submitting">
            <!-- Cột Chi nhánh -->
            <template #cell-company="{ item }">
              <div class="fw-semibold">{{ item.companyName }}</div>
              <div class="text-muted fs-8">
                {{ t('models.Region') }}: {{ item.regionName }}
              </div>
            </template>

            <!-- Cột Không áp (checkbox) -->
            <template #cell-noAlloc="{ item }">
              <el-checkbox v-model="item.noAllocation" :disabled="isView || item.lockedDueToDate"
                @change="(val) => onNoAllocChange(item, val)" />
            </template>

            <!-- Cột động: từng sự kiện -->
            <template v-for="event in eventOptions" :key="'cell-' + event.value"
              v-slot:[`cell-${event.value}`]="{ item }">
              <el-input-number v-if="matrixReady && matrix[String(item.id)]"
                v-model="matrix[String(item.id)][String(event.value)]" :min="0"
                :disabled="item.noAllocation || item.lockedDueToDate || isView" @change="updateDetails(item, event)"
                controls-position="right" class="matrix-input" />
            </template>


            <!-- Cột động: từng sự kiện -->
            <template v-for="event in eventOptions" :key="'cell-' + event.value"
              v-slot:[`cell-${event.value}`]="{ item }">
              <el-input-number v-if="matrixReady && matrix[String(item.id)]"
                v-model="matrix[String(item.id)][String(event.value)]" :min="0" :disabled="item.noAllocation || isView"
                @change="updateDetails(item, event)" controls-position="right" class="matrix-input" />
            </template>

            <!-- Tổng số sự kiện -->
            <template #cell-total="{ item }">
              <span class="fw-bold text-primary">{{ getRowTotal(item.id) }}</span>
            </template>

            <!-- Tổng ngân sách (cố định theo chi nhánh) -->
            <template #cell-totalBudget="{ item }">
              <CurrencyInput :model-value="getRowTotalBudget(item.id)" currency="VND" locale="vi-VN" bordered disabled
                background="#f5f7fa" textColor="#2e7d32" rounded="sm" />
            </template>
          </BaseTable>
        </div>

        <!-- Ghi chú -->
        <!-- <div class="mt-4 text-muted">
          <div class="fw-semibold mb-1">{{ t('common.note') }}</div>
          <div>
            {{ t('allocationEvent.noteForMonth', { month: formData.allocationMonth, year: formData.allocationYear }) }}
          </div>
        </div> -->
      </template>

      <template v-if="mode !== 'create' && !regionScopeId">
        <div class="p-4 bg-light rounded-3 shadow-sm mt-6">
          <div class="d-flex justify-content-between align-items-center mb-3">
            <h5 class="fw-semibold mb-0">{{ t('allocationEvent.historyTitle') }}</h5>
          </div>

          <BaseTable :columns="historyColumns" :items="historyRows" :loading="historyLoading"
            :showCheckboxColumn="false" :showActionsColumn="false" :height="320" />
          <div v-if="!historyLoading && historyRows.length === 0" class="text-center text-muted py-3 fs-8">
            {{ t('common.noData') }}
          </div>
        </div>
      </template>
    </template>
  </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, nextTick } from 'vue'
import { useI18n } from 'vue-i18n'
import dayjs from 'dayjs'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import CurrencyInput from '@/components/currency-input/CurrencyInput.vue'
import type {
  AllocationEventModel,
  AllocationEventHistoryModel,
  AllocationEventHistoryChange,
  AllocationDetailEventModel
} from '@/api/AllocationEventApi'
import { useCompanyStore } from '@/stores/companyStore'
import { useRegionStore } from '@/stores/regionStore'
import { useEventStore } from '@/stores/eventStore'
import { useCommonStore } from '@/stores/commonStore'
import { EventCategory, AllocationEventStatus, AllocationEventStatusLabels } from '@/types'
import { debounce } from 'lodash-es'

const props = defineProps<{
  visible: boolean
  mode?: 'create' | 'edit' | 'view'
  allocationEventData: Partial<AllocationEventModel> | null
  regionIdScope?: string | number | null
}>()

const emit = defineEmits(['update:visible', 'submit', 'delete'])
const { t } = useI18n()

const companyStore = useCompanyStore()
const regionStore = useRegionStore()
const eventStore = useEventStore()
const commonStore = useCommonStore()

const submitting = ref(false)
const isHydrating = ref(false)
const storesReady = ref(false)
const matrixReady = ref(false)
const historyLoading = ref(false)
const histories = ref<AllocationEventHistoryModel[]>([])

const windowWidth = ref(window.innerWidth)
const computedDialogWidth = computed(() => (windowWidth.value < 768 ? '80%' : '90%'))

/* ---------------- FORM DATA ---------------- */
const formData = ref<Partial<AllocationEventModel>>({
  allocationCode: '',
  allocationMonth: (new Date().getMonth() + 1) as unknown as number,
  allocationYear: new Date().getFullYear() as unknown as number,
  eventBudget: 0, // <-- ngân sách cố định cho MỘT chi nhánh
  allocationEventStatus: 0,
  allocationDetails: []
})
const originalData = ref<Partial<AllocationEventModel> | null>(null)

const isView = computed(() => props.mode === 'view')
const modeTitle = computed(() => {
  if (isView.value) return t('allocationEvent.detailTitle')
  if (props.mode === 'edit') return t('allocationEvent.editTitle')
  return t('allocationEvent.addTitle')
})
const isDraftStatus = computed(
  () =>
    Number(formData.value.allocationEventStatus ?? AllocationEventStatus.Draft) === AllocationEventStatus.Draft
)
const actionsDisabled = computed(() => !isDraftStatus.value)
const historyRows = computed(() => histories.value
  .slice()
  .sort((a, b) => {
    const av = a.createdAt ? dayjs(a.createdAt).valueOf() : 0
    const bv = b.createdAt ? dayjs(b.createdAt).valueOf() : 0
    return av - bv
  })
  .map(h => ({
    time: h.createdAt ? dayjs(h.createdAt).format('HH:mm:ss DD/MM/YYYY') : '-',
    target: h.targetName || '-',
    action: h.actionName || '-',
    description: h.description || '-',
    actor: h.createdByName || h.createdBy || '-'
  })))
const historyColumns: BaseTableColumn[] = [
  { key: 'time', labelKey: 'allocationEvent.history.time', width: 200 },
  { key: 'target', labelKey: 'allocationEvent.history.target' },
  { key: 'action', labelKey: 'allocationEvent.history.action', width: 180 },
  { key: 'description', labelKey: 'allocationEvent.history.description' },
  { key: 'actor', labelKey: 'allocationEvent.history.actor', width: 160 }
]

function syncHistoriesFromEvent(event?: Partial<AllocationEventModel> | null) {
  histories.value = event?.allocationEventHistories
    ? [...event.allocationEventHistories]
    : []
}

/* Hiển thị bảng khi đã có tháng & năm */
const showMatrix = computed(() => !!formData.value.allocationMonth && !!formData.value.allocationYear)
const regionScopeId = computed(() => {
  if (props.regionIdScope === undefined || props.regionIdScope === null) return null
  return String(props.regionIdScope)
})

/* ---------------- VALIDATION ---------------- */
const rules = {
  allocationCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
  allocationMonth: [{ required: true, message: t('validation.required'), trigger: 'change' }],
  allocationYear: [{ required: true, message: t('validation.required'), trigger: 'change' }],
  eventBudget: [{ required: true, message: t('validation.required'), trigger: 'blur' }]
}

/* ---------------- EVENTS ---------------- */
const eventOptions = computed(() =>
  eventStore.events
    .filter(e => e.category === EventCategory.Event)
    .map(e => ({ label: e.eventName, value: e.id }))
)

/* ====== LOAD trạng thái 'không áp' từ allocationDetails (khi edit) ====== */
const savedNoAllocByCompany = computed<Map<string, boolean>>(() => {
  const m = new Map<string, boolean>()
  const details = formData.value.allocationDetails || []
  details.forEach(d => {
    const key = String(d.companyId)
    if (d.noAllocation === 1) m.set(key, true)
  })
  return m
})

/* Thứ tự vùng theo thứ tự trong regionStore.regions */
const regionOrderMap = computed(() => {
  const m = new Map<string, number>()
  regionStore.regions.forEach((r, idx) => m.set(String(r.id), idx))
  return m
})

/* ---------------- COMPANIES (lọc theo tháng/năm) ---------------- */
// Set công ty đang có chỉ tiêu (quantity > 0) trong details
const assignedCompanyIds = computed(() => {
  const set = new Set<string>()
    ; (formData.value.allocationDetails || []).forEach(d => {
      if (d?.companyId != null && (Number(d.quantity) || 0) > 0) {
        set.add(String(d.companyId))
      }
    })
  return set
})

const filteredCompanies = computed(() => {
  if (!showMatrix.value) return []
  const allocMonth = formData.value.allocationMonth!
  const allocYear = formData.value.allocationYear!

  const list = companyStore.companies
    .filter(c => c.status === 0)
    // KHÔNG filter cứng nữa — union với các chi nhánh đang có chỉ tiêu
    .map(c => {
      const est = c.establishmentDate ? dayjs(c.establishmentDate) : null
      const eligibleByDate = !est
        || est.year() < allocYear
        || (est.year() === allocYear && est.month() + 1 <= allocMonth)

      const equalMonth = !!(est && est.year() === allocYear && est.month() + 1 === allocMonth)
      const hasAssigned = assignedCompanyIds.value.has(String(c.id))

      const activeRegionId = c.logRegionComs?.find(x => x.companyId === c.id && x.status === 0)?.regionId
      const regionName = regionStore.regions.find(r => r.id === activeRegionId)?.regionName ?? t('common.none')

      if (regionScopeId.value && String(activeRegionId ?? '') !== regionScopeId.value) {
        return null
      }

      // Nếu không còn hợp lệ nhưng đang có chỉ tiêu => ép "không áp" & khóa
      const lockedDueToDate = !eligibleByDate && hasAssigned

      const savedNo = savedNoAllocByCompany.value.get(String(c.id)) ?? false
      return {
        ...c,
        regionName,
        regionId: activeRegionId,
        // auto "không áp" nếu bằng tháng thành lập, hoặc bị khóa do không hợp lệ
        noAllocation: savedNo || equalMonth || lockedDueToDate,
        lockedDueToDate
      }
    })
    .filter((c): c is typeof c & { regionId?: string | number | null } => c !== null)
    // chỉ hiển thị: hợp lệ hoặc đang có chỉ tiêu
    .filter(c => {
      const est = c.establishmentDate ? dayjs(c.establishmentDate) : null
      const eligibleByDate = !est
        || est.year() < allocYear
        || (est.year() === allocYear && est.month() + 1 <= allocMonth)
      return eligibleByDate || assignedCompanyIds.value.has(String(c.id))
    })

  // sort theo vùng → tên vùng → tên chi nhánh
  const BIG = Number.MAX_SAFE_INTEGER
  list.sort((a: any, b: any) => {
    const ai = regionOrderMap.value.get(String(a.regionId ?? '')) ?? BIG
    const bi = regionOrderMap.value.get(String(b.regionId ?? '')) ?? BIG
    if (ai !== bi) return ai - bi
    const rn = String(a.regionName || '').localeCompare(String(b.regionName || ''), undefined, { sensitivity: 'base' })
    if (rn !== 0) return rn
    return String(a.companyName || '').localeCompare(String(b.companyName || ''), undefined, { sensitivity: 'base' })
  })

  return list
})
function enforceForcedNoAllocOnDetails() {
  const details = formData.value.allocationDetails || []
  const perBranchBudget = formData.value.eventBudget ?? 0

  filteredCompanies.value.forEach(c => {
    if (!c?.id) return
    const cid = String(c.id)

    if (c.lockedDueToDate) {
      // ép tick không áp & quantity = 0 cho mọi sự kiện
      eventOptions.value.forEach(ev => {
        const eid = String(ev.value)
        // ma trận hiển thị = 0
        if (matrix.value[cid]) matrix.value[cid][eid] = 0

        // details
        const existing = details.find(x => String(x.companyId) === cid && String(x.eventId) === eid)
        if (!existing) {
          const newDetail = {
            companyId: c.id,
            regionId: c.regionId ?? c.logRegionComs?.[0]?.regionId ?? '',
            eventId: String(ev.value),
            quantity: 0,
            budget: perBranchBudget,
            noAllocation: 1
          } as any
          details.push(newDetail)
        } else {
          existing.quantity = 0
          existing.noAllocation = 1
          existing.budget = perBranchBudget
        }
      })
    }
  })

  formData.value.allocationDetails = [...details]
}

function onNoAllocChange(company: any, newVal: boolean) {
  if (company?.lockedDueToDate) return // Skip if locked

  const noAllocation = !!newVal // ensure boolean
  const cid = String(company.id)

  // Update company model
  company.noAllocation = noAllocation

  // Reset matrix if needed
  if (!matrix.value[cid]) {
    matrix.value[cid] = {}
    eventOptions.value.forEach(ev => {
      matrix.value[cid][String(ev.value)] = 0
    })
  }

  // Reset quantities if noAllocation
  if (noAllocation) {
    Object.keys(matrix.value[cid]).forEach(eid => {
      matrix.value[cid][eid] = 0
    })
  }

  // Update or create details
  const details = formData.value.allocationDetails || []
  const perBranchBudget = formData.value.eventBudget ?? 0

  // Important: Update ALL existing details for this company
  details.forEach(detail => {
    if (String(detail.companyId) === cid) {
      detail.noAllocation = noAllocation ? 1 : 0 // Convert to number for DB
      if (noAllocation) {
        detail.quantity = 0
      }
      detail.budget = perBranchBudget
    }
  })

  // Create missing details if any
  eventOptions.value.forEach(ev => {
    const eidStr = String(ev.value)
    const existingDetail = details.find(d =>
      String(d.companyId) === cid &&
      String(d.eventId) === eidStr
    )

    if (!existingDetail) {
      details.push({
        companyId: company.id,
        regionId: company.regionId ?? company.logRegionComs?.[0]?.regionId ?? '',
        eventId: String(ev.value),
        quantity: 0,
        budget: perBranchBudget,
        noAllocation: noAllocation ? 1 : 0, // Convert to number for DB
        allocationEventId: formData.value.id || '' // Thêm allocationEventId
      })
    }
  })

  // Trigger reactivity
  formData.value.allocationDetails = [...details]
}

/* ---------------- MÃ PHÂN BỔ TỰ SINH ---------------- */
async function generateAllocationCode() {
  await commonStore.generateCode(
    'PB',
    'AllocationEvent',
    'AllocationCode',
    4,
    '{0}-{1:D4}-{2:D2}-{3}',
    formData.value.allocationYear!,
    formData.value.allocationMonth!
  )
  formData.value.allocationCode = commonStore.code
}

watch(
  [() => formData.value.allocationMonth, () => formData.value.allocationYear],
  async ([m, y]) => {
    if (!m || !y || isHydrating.value || isView.value) return
    await generateAllocationCode()

    if (storesReady.value) {
      isHydrating.value = true // Thêm flag để prevent infinite loop
      try {
        await hydrate()
      } finally {
        isHydrating.value = false
      }
    }
  }
)


/* ---------------- BẢNG CỘT CHI NHÁNH ---------------- */
const tableColumns = computed<BaseTableColumn[]>(() => {
  const dynamicCols: BaseTableColumn[] = eventOptions.value.map(ev => ({
    key: String(ev.value),
    labelKey: ev.label,
    isLocale: false,
    width: 200,
    align: 'center',
    headerAlign: 'center'
  }))
  return [
    { key: 'company', labelKey: 'models.Company' },
    { key: 'noAlloc', labelKey: 'allocationEvent.noTarget', width: 140, align: 'center' },
    ...dynamicCols,
    { key: 'total', labelKey: 'allocationEvent.totalEvents', width: 140, align: 'center' },
    { key: 'totalBudget', labelKey: 'allocationEvent.totalBudget', width: 180, align: 'center' }
  ]
})

/* ---------------- BẢNG CỘT VÙNG ---------------- */
// regionColumns
const regionColumns = computed<BaseTableColumn[]>(() => {
  const dynamicCols: BaseTableColumn[] = eventOptions.value.map(ev => ({
    key: `r-${String(ev.value)}`,
    labelKey: ev.label,
    isLocale: false,
    width: 160,
    align: 'center',
    headerAlign: 'center'
  }))
  return [
    { key: 'regionName', labelKey: 'models.Region' },
    // ⬇️ Đổi từ 'branchCount' -> 'appliedBranchCount'
    { key: 'appliedBranchCount', labelKey: 'allocationEvent.branchCount', width: 140, align: 'center' },
    ...dynamicCols,
    { key: 'regionTotalEvents', labelKey: 'allocationEvent.totalEvents', width: 140, align: 'center' },
    { key: 'regionTotalBudget', labelKey: 'allocationEvent.totalBudget', width: 180, align: 'center' }
  ]
})

/* Dữ liệu cho bảng vùng: tổng ngân sách = số chi nhánh 'được áp' × eventBudget */
const computeRegionRows = debounce(() => {
  if (!showMatrix.value || !storesReady.value || !matrixReady.value) return []
  const map: Record<string, {
    regionId: string | null
    regionName: string
    branchCount: number
    appliedBranchCount: number
    eventTotals: Record<string, number>
    totalEvents: number
    totalBudget: number
  }> = {}

  const companies = filteredCompanies.value
  const evs = eventOptions.value

  companies.forEach(c => {
    const ridKey = String(c.regionId ?? 'none')
    if (!map[ridKey]) {
      map[ridKey] = {
        regionId: c.regionId ?? null,
        regionName: c.regionName,
        branchCount: 0,
        appliedBranchCount: 0,
        eventTotals: {},
        totalEvents: 0,
        totalBudget: 0
      }
      evs.forEach(e => (map[ridKey].eventTotals[String(e.value)] = 0))
    }

    map[ridKey].branchCount += 1
    const applied = !(c.noAllocation || c.lockedDueToDate)
    if (applied) map[ridKey].appliedBranchCount += 1

    const cid = String(c.id)
    evs.forEach(e => {
      const eid = String(e.value)
      const qty = applied ? Number(matrix.value[cid]?.[eid] ?? 0) : 0
      map[ridKey].eventTotals[eid] = (map[ridKey].eventTotals[eid] || 0) + qty
    })
  })

  Object.values(map).forEach(row => {
    row.totalEvents = Object.values(row.eventTotals).reduce((s, n) => s + Number(n || 0), 0)
    row.totalBudget = row.appliedBranchCount * (formData.value.eventBudget ?? 0)
  })

  return Object.values(map)
}, 100)

const regionRows = computed(() => {
  if (!showMatrix.value || !storesReady.value || !matrixReady.value) return []

  const map: Record<string, {
    regionId: string | null
    regionName: string
    branchCount: number
    appliedBranchCount: number
    eventTotals: Record<string, number>
    totalEvents: number
    totalBudget: number
  }> = {}

  const companies = filteredCompanies.value
  const evs = eventOptions.value

  companies.forEach(c => {
    const ridKey = String(c.regionId ?? 'none')
    if (!map[ridKey]) {
      map[ridKey] = {
        regionId: c.regionId ?? null,
        regionName: c.regionName,
        branchCount: 0,
        appliedBranchCount: 0,
        eventTotals: {},
        totalEvents: 0,
        totalBudget: 0
      }
      evs.forEach(e => (map[ridKey].eventTotals[String(e.value)] = 0))
    }

    map[ridKey].branchCount += 1
    const applied = !(c.noAllocation || c.lockedDueToDate)
    if (applied) map[ridKey].appliedBranchCount += 1

    const cid = String(c.id)
    evs.forEach(e => {
      const eid = String(e.value)
      const qty = applied ? Number(matrix.value[cid]?.[eid] ?? 0) : 0
      map[ridKey].eventTotals[eid] = (map[ridKey].eventTotals[eid] || 0) + qty
    })
  })

  Object.values(map).forEach(row => {
    row.totalEvents = Object.values(row.eventTotals).reduce((s, n) => s + Number(n || 0), 0)
    row.totalBudget = row.appliedBranchCount * (formData.value.eventBudget ?? 0)
  })

  return Object.values(map)
})

/* ---------------- MATRIX ---------------- */
const matrix = ref<Record<string, Record<string, number>>>({})

function buildEmptyMatrix(companies: any[], events: any[]) {
  const newMatrix: Record<string, Record<string, number>> = {}
  companies.forEach(c => {
    if (!c?.id) return
    const cid = String(c.id)
    newMatrix[cid] = {}
    events.forEach(e => {
      const eid = String(e.value)
      newMatrix[cid][eid] = 0
    })
  })
  matrix.value = newMatrix
}

function applyDetailsToMatrix() {
  const details = formData.value.allocationDetails || []
  details.forEach(d => {
    const cid = String(d.companyId)
    const eid = String(d.eventId)
    if (matrix.value[cid] && typeof matrix.value[cid][eid] !== 'undefined') {
      matrix.value[cid][eid] = d.quantity ?? 0
    }
  })
}

async function hydrate() {
  // prevent re-entrancy from other watchers
  if (isHydrating.value) return
  if (!storesReady.value || !showMatrix.value) return

  isHydrating.value = true
  matrixReady.value = false
  try {
    // Build matrix mới
    buildEmptyMatrix(filteredCompanies.value, eventOptions.value)

    // Đợi 1 frame để Vue cập nhật DOM
    await nextTick()

    // Áp dụng details vào matrix
    applyDetailsToMatrix()

    // Force update matrix references
    Object.keys(matrix.value).forEach(k => {
      matrix.value[k] = { ...matrix.value[k] }
    })

    // Enforce noAlloc CUỐI CÙNG sau khi matrix đã sẵn sàng
    enforceForcedNoAllocOnDetails()
  } finally {
    matrixReady.value = true
    // release lock after microtask to allow other updates to settle
    await nextTick()
    isHydrating.value = false
  }
}

// Replace watch for filteredCompanies,eventOptions with debounced caller
const debouncedHydrate = debounce(async () => {
  if (!storesReady.value || !showMatrix.value || isHydrating.value) return
  await hydrate()
}, 120)

watch([filteredCompanies, eventOptions], () => {
  debouncedHydrate()
}, { immediate: true })

watch(() => formData.value.allocationDetails, async () => {
  if (!storesReady.value || !showMatrix.value || isHydrating.value) return // Thêm check isHydrating
  await nextTick()
  applyDetailsToMatrix()
}, { deep: true })

/* ---------------- LOGIC ---------------- */
function updateDetails(company: any, event: any) {
  const qty = matrix.value[String(company.id)]?.[String(event.value)] ?? 0
  const details = formData.value.allocationDetails || []
  const exist = details.find(
    d => String(d.companyId) === String(company.id) && String(d.eventId) === String(event.value)
  )

  if (exist) {
    exist.quantity = qty
    // giữ lại budget ở detail nếu backend cần, nhưng tổng hiển thị theo chi nhánh là fixed
    exist.budget = formData.value.eventBudget ?? 0
  } else {
    details.push({
      companyId: company.id,
      regionId: company.regionId ?? company.logRegionComs?.[0]?.regionId,
      eventId: event.value,
      quantity: qty,
      budget: formData.value.eventBudget ?? 0, // budget/branch
      noAllocation: company.noAllocation ? 1 : 0
    } as any)
  }

  formData.value.allocationDetails = [...details]
}

/* ---------------- TÍNH TỔNG ---------------- */
function getRowTotal(companyId: string) {
  return (
    formData.value.allocationDetails
      ?.filter(d => String(d.companyId) === String(companyId))
      .reduce((sum, d) => sum + Number(d.quantity || 0), 0) ?? 0
  )
}

function getRowTotalBudget(companyId: string) {
  const company = filteredCompanies.value.find(c => String(c.id) === String(companyId))
  if (!company || company.noAllocation || company.lockedDueToDate) return 0
  return formData.value.eventBudget ?? 0
}

/* ---------------- HISTORY CHANGE BUILDER ---------------- */
function getStatusLabel(val?: number | null) {
  const key = AllocationEventStatusLabels[val as AllocationEventStatus]
  return key ? t(key) : String(val ?? '')
}
function formatNumber(val: number | undefined | null) {
  const num = Number(val ?? 0)
  return Number.isNaN(num) ? '0' : num.toLocaleString('vi-VN')
}
function getCompanyNameById(companyId: string | number | undefined) {
  if (companyId == null) return '-'
  const found = companyStore.companies.find(c => String(c.id) === String(companyId))
  return found?.companyName ?? '-'
}
function getEventNameById(eventId: string | number | undefined) {
  if (eventId == null) return '-'
  const found = eventStore.events.find(e => String(e.id) === String(eventId))
  return found?.eventName ?? '-'
}
function buildHistoryChanges(): AllocationEventHistoryChange[] {
  const current = formData.value
  const original = originalData.value
  if (!current) return []
  // Create: chỉ cần 1 log tạo mới
  if (!original || !original.id) {
    return [{
      actionName: 'Tạo phiếu phân bổ',
      description: '',
      targetName: t('allocationEvent.detailTitle')
    }]
  }

  const defaultTarget = t('allocationEvent.detailTitle')
  const pushGeneralChange = (actionName: string, description: string) => {
    changes.push({
      actionName,
      description,
      targetName: defaultTarget
    })
  }

  const changes: AllocationEventHistoryChange[] = []

  // Đổi trạng thái
  if (original.allocationEventStatus !== current.allocationEventStatus) {
    pushGeneralChange(
      'Đổi trạng thái',
      `${getStatusLabel(original.allocationEventStatus)} -> ${getStatusLabel(current.allocationEventStatus)}`
    )
  }

  // Đổi tháng
  if (original.allocationMonth !== current.allocationMonth) {
    pushGeneralChange(
      'Điều chỉnh tháng',
      `${String(original.allocationMonth ?? '-')} -> ${String(current.allocationMonth ?? '-')}`
    )
  }

  // Đổi năm
  if (original.allocationYear !== current.allocationYear) {
    pushGeneralChange(
      'Điều chỉnh năm',
      `${String(original.allocationYear ?? '-')} -> ${String(current.allocationYear ?? '-')}`
    )
  }

  // Đổi ngân sách / chi nhánh
  if (original.eventBudget !== current.eventBudget) {
    pushGeneralChange(
      'Điều chỉnh ngân sách',
      `${formatNumber(original.eventBudget)} -> ${formatNumber(current.eventBudget)}`
    )
  }

  // Chi tiết chỉ tiêu
  const oldDetails = (original.allocationDetails || []) as AllocationDetailEventModel[]
  const newDetails = (current.allocationDetails || []) as AllocationDetailEventModel[]

  const oldMap = new Map<string, AllocationDetailEventModel>()
  oldDetails.forEach(d => {
    const key = `${String(d.companyId)}-${String(d.eventId)}`
    oldMap.set(key, d)
  })

  const visited = new Set<string>()
  newDetails.forEach(d => {
    const key = `${String(d.companyId)}-${String(d.eventId)}`
    visited.add(key)
    const old = oldMap.get(key)
    const targetName = getCompanyNameById(d.companyId)

    if (!old) {
      changes.push({
        actionName: 'Thêm chỉ tiêu',
        targetName,
        description: `Sự kiện ${getEventNameById(d.eventId)} - Số lượng: ${formatNumber(d.quantity)}`
      })
      return
    }

    // quantity change
    if ((old.quantity ?? 0) !== (d.quantity ?? 0)) {
      changes.push({
        actionName: 'Điều chỉnh chỉ tiêu',
        targetName,
        description: `Sự kiện ${getEventNameById(d.eventId)}: ${formatNumber(old.quantity)} -> ${formatNumber(d.quantity)}`
      })
    }

    // noAllocation change
    if ((old.noAllocation ?? 0) !== (d.noAllocation ?? 0)) {
      changes.push({
        actionName: 'Điều chỉnh chỉ tiêu',
        targetName,
        description: `Sự kiện ${getEventNameById(d.eventId)}: ` +
          (old.noAllocation === 1 ? 'Không áp chỉ tiêu' : 'Áp chỉ tiêu') +
          ' -> ' + (d.noAllocation === 1 ? 'Không áp chỉ tiêu' : 'Áp chỉ tiêu')
      })
    }
  })

  // deleted details
  oldDetails.forEach(d => {
    const key = `${String(d.companyId)}-${String(d.eventId)}`
    if (!visited.has(key)) {
      changes.push({
        actionName: 'Xoá chỉ tiêu',
        targetName: getCompanyNameById(d.companyId),
        description: `Sự kiện ${getEventNameById(d.eventId)} - Số lượng: ${formatNumber(d.quantity)}`
      })
    }
  })

  return changes
}

/* ---------------- SUBMIT ---------------- */
function onSubmit() {
  const payload: Partial<AllocationEventModel> = {
    ...formData.value,
    historyChanges: buildHistoryChanges()
  }
  console.log(payload.historyChanges);

  emit('submit', payload)
}
function onDelete() {
  emit('delete', formData.value)
}

/* ---------------- INIT ---------------- */
onMounted(async () => {
  await Promise.all([
    eventStore.events.length ? Promise.resolve() : eventStore.fetchAllEvents(),
    companyStore.companies.length ? Promise.resolve() : companyStore.fetchAllCompanies(),
    regionStore.regions.length ? Promise.resolve() : regionStore.fetchAllRegions()
  ])
  storesReady.value = true
})

watch(
  () => props.allocationEventData,
  async val => {
    if (!val) {
      originalData.value = null
      histories.value = []
      historyLoading.value = false
      return
    }
    isHydrating.value = true
    historyLoading.value = true
    formData.value = { ...val }
    originalData.value = JSON.parse(JSON.stringify(val))
    await nextTick()
    if (storesReady.value && showMatrix.value) {
      await hydrate()
    }
    syncHistoriesFromEvent(val)
    historyLoading.value = false
    isHydrating.value = false
  },
  { immediate: true }
)
</script>

<style scoped>
.matrix-input {
  width: 120px;
  text-align: center;
}
</style>
