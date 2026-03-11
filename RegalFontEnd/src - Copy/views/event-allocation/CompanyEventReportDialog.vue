<template>
  <BaseDialogForm class="company-report-dialog" :visible="visible" :title="t('allocationEvent.report.dialogTitle')"
    :description="t('allocationEvent.report.dialogSubTitle', { code: proposalInfo.allocationCode })" :mode="dialogMode"
    :form-data="form" width="90%" :submit-disabled="props.readonly || !proposal" :loading="isSubmitting"
    :show-delete="false" :height="'80vh'" @update:visible="emit('update:visible', $event)" @submit="submitReport">
    <template #form>
      <div class="report-form">
        <el-form :model="form" label-position="top" :inline="false">
          <template v-if="proposal">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item :label="t('allocationEvent.code')">
                  <el-input :model-value="proposalInfo.allocationCode" disabled />
                </el-form-item>
                <el-form-item :label="t('allocationEvent.proposal.proposalCode')">
                  <el-input :model-value="proposalInfo.proposalCode" disabled />
                </el-form-item>
                <el-form-item :label="t('allocationEvent.report.reportCode')">
                  <el-input v-model="form.reportCode" disabled />
                </el-form-item>
                <el-form-item :label="t('allocationEvent.proposal.companyScope')">
                  <el-input :model-value="proposalInfo.companyName" disabled />
                </el-form-item>
                <el-form-item :label="t('allocationEvent.proposal.eventName')">
                  <el-input :model-value="proposalInfo.eventName" disabled />
                </el-form-item>
                <el-form-item :label="t('allocationEvent.proposal.eventDate')">
                  <el-date-picker :model-value="proposalInfo.eventDate" type="date" value-format="YYYY-MM-DD"
                    disabled />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item :label="t('allocationEvent.proposal.region')">
                  <el-input :model-value="proposalInfo.regionName" disabled />
                </el-form-item>
                <el-form-item :label="t('allocationEvent.proposal.eventSize')">
                  <el-input :model-value="proposalInfo.eventSizeLabel" disabled />
                </el-form-item>
                <el-form-item :label="t('allocationEvent.proposal.totalProjectedCost')">
                  <el-input :model-value="formatCurrency(proposalInfo.projectedTotal)" disabled />
                </el-form-item>
                <el-form-item :label="t('allocationEvent.report.reportDate')">
                  <el-date-picker v-model="form.reportDate" type="date" value-format="YYYY-MM-DD" />
                </el-form-item>
                <el-form-item :label="t('allocationEvent.report.costActual')">
                  <el-input :model-value="formatCurrency(totalActualCost)" disabled />
                </el-form-item>
              </el-col>
            </el-row>

            <el-row :gutter="16">
              <el-col :span="12">
                <el-form-item :label="t('allocationEvent.report.linkContent')">
                  <el-input v-model="form.linkContent" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item :label="t('allocationEvent.report.linkFanpage')">
                  <el-input v-model="form.linkFanpage" />
                </el-form-item>
              </el-col>
            </el-row>
          </template>
        </el-form>

        <section class="mt-4" v-if="proposal">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <h5 class="fw-semibold mb-0">{{ t('allocationEvent.proposal.stockCostTitle') }}</h5>
            <el-button size="small" class="action-btn add" type="primary" text @click="addStockRow">
              <el-icon>
                <Plus />
              </el-icon>
              {{ t('common.add') }}
            </el-button>
          </div>

          <BaseTable :columns="stockColumns" :items="stockCosts" :showCheckboxColumn="false" :showActionsColumn="false"
            :height="200">
            <template #cell-rowNumber="{ item }">
              {{ stockCosts.indexOf(item) + 1 }}
            </template>
            <template #cell-itemId="{ item }">
              <el-select v-model="item.itemId" filterable @change="onPublicationChange(item)">
                <el-option v-for="opt in publicationOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
              </el-select>
            </template>
            <template #cell-quantity="{ item }">
              <el-input-number v-model="item.quantity" :min="0" @change="recalcPublication(item)" />
            </template>
            <template #cell-publicationAmount="{ item }">
              <el-input-number v-model="item.publicationAmount" :min="0" @change="recalcPublication(item)" />
            </template>
            <template #cell-totalAmount="{ item }">
              <el-input v-model="item.totalAmount" disabled />
            </template>
            <template #cell-rowActions="{ item }">
              <el-button class="action-btn delete" text type="danger" @click="removeStockRow(stockCosts.indexOf(item))">
                <el-icon>
                  <Delete />
                </el-icon>
              </el-button>
            </template>
          </BaseTable>
        </section>

        <section class="mt-4" v-if="proposal">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <h5 class="fw-semibold mb-0">{{ t('allocationEvent.proposal.cashCostTitle') }}</h5>
            <el-button size="small" class="action-btn add" type="primary" text @click="addCashRow">
              <el-icon>
                <Plus />
              </el-icon>
              {{ t('common.add') }}
            </el-button>
          </div>

          <BaseTable :columns="cashColumns" :items="cashCosts" :showCheckboxColumn="false" :showActionsColumn="false"
            :height="200">
            <template #cell-rowNumber="{ item }">
              {{ cashCosts.indexOf(item) + 1 }}
            </template>
            <template #cell-cashName="{ item }">
              <el-input v-model="item.cashName" />
            </template>
            <template #cell-quantity="{ item }">
              <el-input-number v-model="item.quantity" :min="0" @change="recalcCash(item)" />
            </template>
            <template #cell-amount="{ item }">
              <el-input-number v-model="item.amount" :min="0" @change="recalcCash(item)" />
            </template>
            <template #cell-totalAmount="{ item }">
              <el-input v-model="item.totalAmount" disabled />
            </template>
            <template #cell-rowActions="{ item }">
              <el-button class="action-btn delete" text type="danger" @click="removeCashRow(cashCosts.indexOf(item))">
                <el-icon>
                  <Delete />
                </el-icon>
              </el-button>
            </template>
          </BaseTable>
        </section>

        <section class="mt-4" v-if="proposal">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <h5 class="fw-semibold mb-0">{{ t('allocationEvent.report.participantActualTitle') }}</h5>
            <div class="d-flex gap-2">
              <el-button size="small" :loading="participantImportLoading" @click="triggerParticipantImport">
                <el-icon>
                  <Plus />
                </el-icon>
                {{ t('allocationEvent.proposal.import') }}
              </el-button>
              <el-button size="small" class="action-btn add" type="primary" text @click="addParticipant">
                <el-icon>
                  <Plus />
                </el-icon>
                {{ t('common.add') }}
              </el-button>
            </div>
          </div>
          <input ref="participantImportRef" type="file" accept=".xlsx,.xls,.csv" class="d-none"
            @change="handleParticipantImportFile" />
          <BaseTable :columns="participantColumns" :items="participants" :showCheckboxColumn="false"
            :showActionsColumn="false" :height="240">
            <template #cell-studentCode="{ item }">
              <el-input v-model="item.studentCode" size="small"
                :placeholder="t('allocationEvent.proposal.participantStudentCode')"
                @keyup.enter="handleStudentCodeEnter(item)" />
            </template>
            <template #cell-fullName="{ item }">
              <el-input v-model="item.participantName" />
            </template>
            <template #cell-gender="{ item }">
              <el-select v-model="item.participantGender" placeholder="-">
                <el-option :label="t('common.male')" value="male" />
                <el-option :label="t('common.female')" value="female" />
              </el-select>
            </template>
            <template #cell-dateOfBirth="{ item }">
              <el-date-picker v-model="item.participantDateOfBirth" type="date" value-format="YYYY-MM-DD"
                placeholder="-" />
            </template>
            <template #cell-address="{ item }">
              <el-input v-model="item.participantAddress" />
            </template>
            <template #cell-isStudent="{ item }">
              <el-select v-model="item.isStudent" placeholder="-">
                <el-option :label="t('common.yes')" :value="true" />
                <el-option :label="t('common.no')" :value="false" />
              </el-select>
            </template>
            <template #cell-phone="{ item }">
              <el-input v-model="item.participantPhoneNumber" />
            </template>
            <template #cell-contact="{ item }">
              <el-input v-model="item.participantContact" />
            </template>
            <template #cell-email="{ item }">
              <el-input v-model="item.participantEmail" />
            </template>
            <template #cell-school="{ item }">
              <el-input v-model="item.participantSchool" />
            </template>
            <template #cell-source="{ item }">
              <el-input v-model="item.participantSourceKnown" />
            </template>
            <template #cell-job="{ item }">
              <el-input v-model="item.participantJob" />
            </template>
            <template #cell-advisor="{ item }">
              <el-select v-model="item.employeeId" filterable placeholder="-">
                <el-option v-for="advisor in advisorOptions" :key="advisor.value" :label="advisor.label"
                  :value="advisor.value" />
              </el-select>
            </template>
            <template #cell-rowActions="{ item }">
              <el-button class="action-btn delete" text type="danger"
                @click="removeParticipant(participants.indexOf(item))">
                <el-icon>
                  <Delete />
                </el-icon>
              </el-button>
            </template>
          </BaseTable>
        </section>

        <section class="mt-4" v-if="proposal">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <h5 class="fw-semibold mb-0">{{ t('allocationEvent.proposal.attachments') }}</h5>
            <el-button size="small" class="action-btn add" type="primary" text @click="openAttachmentPicker">
              <el-icon>
                <Plus />
              </el-icon>
              {{ t('common.add') }}
            </el-button>
          </div>
          <FileManager ref="attachmentManagerRef" v-model="attachments" :fields="attachmentFields" :multiple="true"
            :enableDownload="dialogMode !== 'create'" class="attachments-manager" :itemTitle="''"
            :hideHeader="true" />
          <div class="text-muted fs-8 mt-2">
            {{ t('allocationEvent.proposal.attachmentNote') }}
          </div>
        </section>

        <section class="mt-4" v-if="proposal && approvalHistoryRows.length">
          <h5 class="fw-semibold mb-2">{{ t('allocationEvent.proposal.approvalHistory') }}</h5>
          <BaseTable :columns="approvalHistoryColumns" :items="approvalHistoryRows" :showCheckboxColumn="false"
            :showActionsColumn="false" :height="200">
            <template #cell-rowNumber="{ item }">
              {{ item.rowNumber }}
            </template>
            <template #cell-statusLabel="{ item }">
              <BaseBadge :label="item.statusLabel" :colorByLabelMap="approvalStatusColorMap" />
            </template>
            <template #cell-createdAt="{ item }">
              {{ item.createdAt }}
            </template>
          </BaseTable>
        </section>
      </div>
    </template>
  </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch, onMounted } from 'vue'
import dayjs from 'dayjs'
import * as XLSX from 'xlsx'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import FileManager from '@/components/file-manager/FileManager.vue'
import { Delete, Plus } from '@element-plus/icons-vue'
import { formatCurrency } from '@/utils/format'
import type {
  CompanyEventModel,
  CompanyEventReportModel,
  EventCashModel,
  EventParticipantModel,
  EventPublicationModel,
  ApproveCompanyEventReportModel
} from '@/api/AllocationEventApi'
import { useItemStore } from '@/stores/itemStore'
import { useEventStore } from '@/stores/eventStore'
import { useEmployeeStore } from '@/stores/employeeStore'
import { useStudentStore } from '@/stores/studentStore'
import { useCommonStore } from '@/stores/commonStore'
import { CompanyEventProposalStatus, CompanyEventProposalStatusLabels, EventSize } from '@/types'
import type { Attachment, FieldSchema } from '@/api/FileApi'
import { useNotificationStore } from '@/stores/notificationStore'
import type { StudentModel } from '@/api/StudentApi'
import type { EmployeeModel } from '@/api/EmployeeApi'

const props = defineProps<{
  visible: boolean
  proposal: CompanyEventModel | null
  proposals?: CompanyEventModel[]
  report?: CompanyEventReportModel | null
  submitting?: boolean
  readonly?: boolean
}>()

const emit = defineEmits(['update:visible', 'submit'])
const { t } = useI18n()
const itemStore = useItemStore()
const eventStore = useEventStore()
const employeeStore = useEmployeeStore()
const studentStore = useStudentStore()
const commonStore = useCommonStore()
const notificationStore = useNotificationStore()

const visible = computed(() => props.visible)
const proposal = computed(() => props.proposal)
const isSubmitting = computed(() => props.submitting ?? false)
const dialogMode = computed<'create' | 'edit' | 'view'>(() => {
  if (props.readonly) return 'view'
  return props.report ? 'edit' : 'create'
})

const form = reactive({
  reportCode: '',
  reportDate: dayjs().format('YYYY-MM-DD'),
  linkContent: '',
  linkFanpage: ''
})

type EditablePublication = EventPublicationModel & { itemName?: string; maxQuantity?: number; price?: number }
type ParticipantRow = EventParticipantModel & { studentCode?: string }
const stockCosts = ref<EditablePublication[]>([])
const cashCosts = ref<EventCashModel[]>([])
const participants = ref<ParticipantRow[]>([])
const attachments = ref<Attachment[]>([])
const attachmentManagerRef = ref<any>(null)
const attachmentFields: FieldSchema[] = []
const participantImportRef = ref<HTMLInputElement | null>(null)
const participantImportLoading = ref(false)
const sampleDataApplied = ref(false)

const publicationOptions = computed(() =>
  itemStore.items.map(item => ({
    value: String(item.id),
    label: item.itemName ?? '-'
  }))
)

const eventCodeMap = computed(() => {
  const map = new Map<string, string>()
  eventStore.events.forEach(ev => map.set(String(ev.id), ev.eventCode || ''))
  return map
})

const advisorOptions = computed(() =>
  (employeeStore.employees || [])
    .filter(emp => emp.position?.isSale === true)
    .map(emp => {
      const code = (emp.applicationUser?.userCode || emp.employeeTax || '').trim()
      const name = emp.applicationUser?.fullName || emp.employeeTax || ''
      const label = code && name ? `${name} (${code})` : name || code || '-'
      return {
        value: String(emp.id),
        label
      }
    })
)

const proposalInfo = computed(() => {
  const detail = proposal.value?.allocationDetailEvent
  const sizeLabel = proposal.value?.eventSize != null
    ? t(proposal.value.eventSize === EventSize.Big ? 'allocationEvent.proposal.size.big' : 'allocationEvent.proposal.size.mini')
    : ''
  return {
    allocationCode: detail?.allocationEvent?.allocationCode ?? '',
    proposalCode: proposal.value?.companyEventCode ?? '',
    companyName: proposal.value?.companyEventName ?? '',
    eventName: proposal.value?.companyEventName ?? '',
    eventDate: proposal.value?.eventDate ?? '',
    regionName: detail?.region?.regionName ?? '',
    eventSizeLabel: sizeLabel,
    projectedTotal: proposal.value?.totalAmount ?? 0
  }
})

const totalActualCost = computed(() => {
  const stockTotal = stockCosts.value.reduce((sum, row) => sum + Number(row.totalAmount || 0), 0)
  const cashTotal = cashCosts.value.reduce((sum, row) => sum + Number(row.totalAmount || 0), 0)
  return stockTotal + cashTotal
})

const stockColumns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center' },
  { key: 'itemId', labelKey: 'allocationEvent.proposal.stockProduct' },
  { key: 'quantity', labelKey: 'allocationEvent.proposal.quantity', width: 220, align: 'center' },
  { key: 'publicationAmount', labelKey: 'allocationEvent.proposal.unitAmount', width: 220, align: 'right' },
  { key: 'totalAmount', labelKey: 'allocationEvent.proposal.totalAmount', width: 220, align: 'right' },
  { key: 'rowActions', labelKey: 'common.actions', width: 100, align: 'center' }
]

const cashColumns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center' },
  { key: 'cashName', labelKey: 'allocationEvent.proposal.cashName' },
  { key: 'quantity', labelKey: 'allocationEvent.proposal.quantity', width: 220, align: 'center' },
  { key: 'amount', labelKey: 'allocationEvent.proposal.unitAmount', width: 220, align: 'right' },
  { key: 'totalAmount', labelKey: 'allocationEvent.proposal.totalAmount', width: 220, align: 'right' },
  { key: 'rowActions', labelKey: 'common.actions', width: 100, align: 'center' }
]

const participantColumns: BaseTableColumn[] = [
  { key: 'studentCode', labelKey: 'allocationEvent.proposal.participantStudentCode', width: 160, sticky: true },
  { key: 'fullName', labelKey: 'allocationEvent.proposal.participantName', width: 180, sticky: true },
  { key: 'gender', labelKey: 'allocationEvent.proposal.participantGender', width: 120 },
  { key: 'dateOfBirth', labelKey: 'allocationEvent.proposal.participantDob', width: 150 },
  { key: 'address', labelKey: 'allocationEvent.proposal.participantAddress', width: 180 },
  { key: 'isStudent', labelKey: 'allocationEvent.proposal.participantIsStudent', width: 140 },
  { key: 'phone', labelKey: 'allocationEvent.proposal.participantPhone', width: 140 },
  { key: 'contact', labelKey: 'allocationEvent.proposal.participantContact', width: 160 },
  { key: 'email', labelKey: 'allocationEvent.proposal.participantEmail', width: 200 },
  { key: 'school', labelKey: 'allocationEvent.proposal.participantSchool', width: 160 },
  { key: 'source', labelKey: 'allocationEvent.proposal.participantSource', width: 140 },
  { key: 'job', labelKey: 'allocationEvent.proposal.participantJob', width: 140 },
  { key: 'advisor', labelKey: 'allocationEvent.proposal.participantAdvisor', width: 180 },
  { key: 'rowActions', labelKey: 'common.actions', width: 120, align: 'center' }
]

const approvalHistoryColumns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center' },
  { key: 'statusLabel', labelKey: 'common.status', width: 160, align: 'center' },
  { key: 'createdAt', labelKey: 'allocationEvent.proposal.approveDate', width: 180, align: 'center' },
  { key: 'actor', labelKey: 'allocationEvent.proposal.approveBy', minWidth: 200 }
]

const approvalStatusColorMap = {
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.PendingApproval])]: 'warning',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Draft])]: 'warning',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Approved])]: 'success',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Rejected])]: 'red'
}

const employeeByCode = computed(() => {
  const map = new Map<string, EmployeeModel>()
    ; (employeeStore.employees || []).forEach(emp => {
      const code = (emp.applicationUser?.userCode || emp.employeeTax || '').trim().toLowerCase()
      if (code) {
        map.set(code, emp)
      }
    })
  return map
})

const approvalHistoryRows = computed(() => {
  const rows = (props.report?.approveCompanyEvents ?? []) as ApproveCompanyEventReportModel[]
  return rows.map((item, index) => ({
    rowNumber: index + 1,
    statusLabel: statusLabel(item.approveStatus),
    createdAt: item.createdAt ?? '',
    actor: item.createdBy ?? ''
  }))
})

const generatingReportCode = ref(false)
const lastGeneratedEventId = ref<string | null>(null)

function statusLabel(status?: CompanyEventProposalStatus | number) {
  if (status == null) return t('common.unknown')
  const key = CompanyEventProposalStatusLabels[status as CompanyEventProposalStatus]
  return key ? t(key) : t('common.unknown')
}

function resolveReportEventId() {
  const detail = proposal.value?.allocationDetailEvent as any
  const eventId = detail?.eventId ?? detail?.event?.id
  return eventId != null ? String(eventId) : ''
}

function resolveReportEventCode() {
  const eventId = resolveReportEventId()
  if (eventId) {
    const code = eventCodeMap.value.get(eventId)
    if (code) return code
  }
  const detail = proposal.value?.allocationDetailEvent as any
  return detail?.event?.eventCode ?? ''
}

async function generateReportCode() {
  if (generatingReportCode.value || props.readonly) return
  const eventId = resolveReportEventId()
  const eventCode = resolveReportEventCode().trim()
  if (!eventId || !eventCode) return
  if (lastGeneratedEventId.value === eventId && form.reportCode) return
  generatingReportCode.value = true
  try {
    const code = await commonStore.generateCode(
      `BC_${eventCode}_`,
      'CompanyEventReport',
      'CompanyEventReportCode',
      4
    )
    if (code) {
      form.reportCode = code
      lastGeneratedEventId.value = eventId
    }
  } catch (error) {
    console.error('Generate report code failed', error)
  } finally {
    generatingReportCode.value = false
  }
}

function addStockRow() {
  stockCosts.value = [
    ...stockCosts.value,
    {
      companyEventId: proposal.value?.id ?? '',
      itemId: '',
      quantity: 0,
      publicationAmount: 0,
      totalAmount: 0
    } as EditablePublication
  ]
}

function removeStockRow(index: number) {
  stockCosts.value.splice(index, 1)
}

function onPublicationChange(row: EditablePublication) {
  const item = itemStore.items.find(i => String(i.id) === String(row.itemId))
  row.itemName = item?.itemName ?? row.itemName
  row.publicationAmount = Number(item?.price ?? row.publicationAmount ?? 0)
  recalcPublication(row)
}

function recalcPublication(row: EditablePublication) {
  row.totalAmount = Number(row.quantity || 0) * Number(row.publicationAmount || 0)
}

function addCashRow() {
  cashCosts.value = [
    ...cashCosts.value,
    {
      companyEventId: proposal.value?.id ?? '',
      cashName: '',
      quantity: 0,
      amount: 0,
      totalAmount: 0
    } as EventCashModel
  ]
}

function removeCashRow(index: number) {
  cashCosts.value.splice(index, 1)
}

function recalcCash(row: EventCashModel) {
  row.totalAmount = Number(row.quantity || 0) * Number(row.amount || 0)
}

function addParticipant() {
  participants.value = [
    ...participants.value,
    {
      companyEventId: proposal.value?.id ?? '',
      isStudent: false,
      participantName: '',
      studentCode: ''
    } as ParticipantRow
  ]
}

function removeParticipant(index: number) {
  participants.value.splice(index, 1)
}

async function handleStudentCodeEnter(row: ParticipantRow) {
  const code = (row.studentCode || '').trim()
  if (!code) return
  try {
    const student = await studentStore.fetchStudentByCode(code)
    if (!student) {
      notificationStore.showToast('warning', {
        key: 'allocationEvent.proposal.studentNotFound',
        params: { code }
      })
      return
    }
    row.participantName = student.fullName || row.participantName
    row.participantPhoneNumber = student.phone || row.participantPhoneNumber
    row.participantEmail = student.email || row.participantEmail
    row.participantAddress = student.address || row.participantAddress
    row.participantSchool = student.companyName || row.participantSchool
    row.participantJob = student.reason || row.participantJob
    row.participantContact =
      student.contacts?.[0]?.fullName || student.contacts?.[0]?.phone || row.participantContact
    row.participantSourceKnown = student.leadSource || row.participantSourceKnown
    row.participantGender = student.gender || row.participantGender
    if (student.birthDate) {
      const birth = dayjs(student.birthDate)
      if (birth.isValid()) {
        row.participantDateOfBirth = birth.format('YYYY-MM-DD')
      }
    }
    row.isStudent = true
  } catch (error) {
    console.error('Lookup student by code failed', error)
    //notificationStore.showToast('error', { key: 'common.systemError' })
  }
}

const PARTICIPANT_IMPORT_ALIASES: Record<string, string[]> = {
  studentCode: ['studentcode', 'mahocvien', 'masohocvien', 'studentid', 'mahs', 'student'],
  studentName: ['studentname', 'tenhocvien', 'tenhocvien', 'fullname', 'name'],
  gender: ['gender', 'gioitinh', 'gioitinh'],
  dateOfBirth: ['dateofbirth', 'ngaysinh', 'dob', 'birthdate'],
  address: ['address', 'diachi', 'diachi'],
  phone: ['phone', 'sdt', 'sdt', 'mobile'],
  contact: ['contact', 'contactname', 'tenlienhe', 'tenlienhe'],
  email: ['email', 'thuemail'],
  school: ['school', 'truong', 'diachi', 'department'],
  source: ['source', 'nguon', 'leadsource'],
  job: ['job', 'occupation', 'nghenghiep', 'career'],
  advisorCode: ['advisorcode', 'manhanvien', 'manhanvien', 'manv', 'employeecode', 'ma nv']
}

function normalizeHeaderKey(value?: string) {
  if (!value) return ''
  return value
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .replace(/\s+/g, '')
    .replace(/[^a-z0-9]/gi, '')
    .toLowerCase()
}

function buildNormalizedRow(row: Record<string, any>) {
  const normalized: Record<string, any> = {}
  Object.entries(row).forEach(([key, value]) => {
    const normalizedKey = normalizeHeaderKey(key)
    if (normalizedKey) {
      normalized[normalizedKey] = value
    }
  })
  return normalized
}

function getNormalizedValue(row: Record<string, any>, field: string) {
  const aliases = PARTICIPANT_IMPORT_ALIASES[field] ?? []
  for (const alias of aliases) {
    const normalized = normalizeHeaderKey(alias)
    if (normalized && Object.prototype.hasOwnProperty.call(row, normalized)) {
      return row[normalized]
    }
  }
  return ''
}

function toStringValue(value: unknown) {
  if (value === null || value === undefined) return ''
  if (typeof value === 'number') return String(value)
  return String(value).trim()
}

function toDateString(value: unknown) {
  const parsed = dayjs(value as string | number | Date | null | undefined)
  return parsed.isValid() ? parsed.format('YYYY-MM-DD') : ''
}

function triggerParticipantImport() {
  participantImportRef.value?.click()
}

async function handleParticipantImportFile(event: Event) {
  const input = event.target as HTMLInputElement | null
  const file = input?.files?.[0]
  if (input) input.value = ''
  if (!file) return
  participantImportLoading.value = true
  try {
    const buffer = await file.arrayBuffer()
    const workbook = XLSX.read(buffer, { type: 'array' })
    const sheetName = workbook.SheetNames[0]
    if (!sheetName) {
      notificationStore.showToast('warning', { key: 'allocationEvent.proposal.importEmpty' })
      return
    }
    const sheet = workbook.Sheets[sheetName]
    const rawRows = XLSX.utils.sheet_to_json<Record<string, any>>(sheet, { defval: '' })
    if (!rawRows.length) {
      notificationStore.showToast('warning', { key: 'allocationEvent.proposal.importEmpty' })
      return
    }
    const errors: string[] = []
    const studentCache = new Map<string, StudentModel | null>()
    const newParticipants: ParticipantRow[] = []
    const codesMap = employeeByCode.value
    for (let rowIndex = 0; rowIndex < rawRows.length; rowIndex++) {
      const normalizedRow = buildNormalizedRow(rawRows[rowIndex])
      const studentCodeRaw = getNormalizedValue(normalizedRow, 'studentCode')
      const advisorCodeRaw = getNormalizedValue(normalizedRow, 'advisorCode')
      const studentCode = toStringValue(studentCodeRaw)
      const advisorCode = toStringValue(advisorCodeRaw)
      let rowHasError = false
      let resolvedStudent: StudentModel | null = null
      if (studentCode) {
        const studentKey = studentCode.toLowerCase()
        if (!studentCache.has(studentKey)) {
          const student = await studentStore.fetchStudentByCode(studentCode)
          studentCache.set(studentKey, student)
        }
        resolvedStudent = studentCache.get(studentKey) ?? null
        if (!resolvedStudent) {
          errors.push(t('allocationEvent.proposal.studentNotFound', { code: studentCode }))
          rowHasError = true
        }
      }
      let employeeId: string | undefined
      if (advisorCode) {
        const employee = codesMap.get(advisorCode.toLowerCase())
        if (!employee) {
          errors.push(t('allocationEvent.proposal.advisorNotFound', { code: advisorCode }))
          rowHasError = true
        } else {
          employeeId = employee.id ? String(employee.id) : undefined
        }
      }
      if (rowHasError) {
        continue
      }
      const rowName = toStringValue(getNormalizedValue(normalizedRow, 'studentName'))
      const rowGender = toStringValue(getNormalizedValue(normalizedRow, 'gender'))
      const rowDob = toDateString(getNormalizedValue(normalizedRow, 'dateOfBirth'))
      const rowAddress = toStringValue(getNormalizedValue(normalizedRow, 'address'))
      const rowPhone = toStringValue(getNormalizedValue(normalizedRow, 'phone'))
      const rowContact = toStringValue(getNormalizedValue(normalizedRow, 'contact'))
      const rowEmail = toStringValue(getNormalizedValue(normalizedRow, 'email'))
      const rowSchool = toStringValue(getNormalizedValue(normalizedRow, 'school'))
      const rowSource = toStringValue(getNormalizedValue(normalizedRow, 'source'))
      const rowJob = toStringValue(getNormalizedValue(normalizedRow, 'job'))
      const participant: ParticipantRow = {
        companyEventId: '',
        studentCode,
        isStudent: true,
        participantName: rowName || resolvedStudent?.fullName || '',
        participantGender: rowGender || resolvedStudent?.gender || '',
        participantDateOfBirth:
          rowDob ||
          (resolvedStudent?.birthDate ? dayjs(resolvedStudent.birthDate).format('YYYY-MM-DD') : ''),
        participantAddress: rowAddress || resolvedStudent?.address || '',
        participantPhoneNumber: rowPhone || resolvedStudent?.phone || '',
        participantContact:
          rowContact ||
          resolvedStudent?.contacts?.[0]?.fullName ||
          resolvedStudent?.contacts?.[0]?.phone ||
          '',
        participantEmail: rowEmail || resolvedStudent?.email || '',
        participantSchool: rowSchool || resolvedStudent?.companyName || '',
        participantSourceKnown: rowSource || resolvedStudent?.leadSource || '',
        participantJob: rowJob || resolvedStudent?.reason || '',
        employeeId
      }
      newParticipants.push(participant)
    }
    if (errors.length) {
      notificationStore.showToast('error', {
        key: 'allocationEvent.proposal.importInvalid',
        params: { reason: errors.join(' / ') }
      })
      return
    }
    participants.value = [...participants.value, ...newParticipants]
    notificationStore.showToast('success', {
      key: 'allocationEvent.proposal.importSuccess',
      params: { count: newParticipants.length }
    })
  } catch (error) {
    console.error('Participant import failed', error)
    notificationStore.showToast('error', { key: 'common.systemError' })
  } finally {
    participantImportLoading.value = false
  }
}

function applyReportData() {
  if (!proposal.value) return
  const report = props.report
  form.reportCode = report?.companyEventReportCode ?? ''
  form.reportDate = report?.reportDate ?? report?.createdAt ?? dayjs().format('YYYY-MM-DD')
  form.linkContent = report?.linkContent ?? ''
  form.linkFanpage = report?.linkFanpage ?? ''

  if (!report) {
    form.reportCode = ''
    stockCosts.value = []
    cashCosts.value = []
    participants.value = []
    attachments.value = []
    void generateReportCode()
    applySampleData()
    return
  }

  const reportId = String(report.id ?? '')
  const publications = filterReportItems(report.eventPublications ?? [], reportId)
  stockCosts.value = publications.map(item => ({
    ...item,
    itemId: item.itemId,
    quantity: Number(item.quantity || 0),
    publicationAmount: Number(item.publicationAmount || 0),
    totalAmount: Number(item.totalAmount || 0)
  })) as EditablePublication[]

  const cashList = filterReportItems(report.eventCashes ?? [], reportId)
  cashCosts.value = cashList.map(item => ({
    ...item,
    quantity: Number(item.quantity || 0),
    amount: Number(item.amount || 0),
    totalAmount: Number(item.totalAmount || 0)
  })) as EventCashModel[]

  const participantList = filterReportItems(report.eventParticipants ?? [], reportId)
  participants.value = participantList.map(item => ({
    ...item
  })) as EventParticipantModel[]

  attachments.value = mapAttachments(report.attachments ?? [])
}

function filterReportItems<T extends { companyEventReportId?: string | null; companyEventId?: string | null; id?: string | null }>(
  items: T[],
  reportId: string
) {
  if (!items.length) return []
  if (reportId) {
    const byReport = items.filter(
      item => String(item.companyEventReportId ?? '') === reportId
    )
    if (byReport.length) {
      return dedupeById(byReport)
    }
  }
  const byNullEventId = items.filter(item => item.companyEventId == null)
  if (byNullEventId.length) {
    return dedupeById(byNullEventId)
  }
  return dedupeById(items)
}

function dedupeById<T extends { id?: string | null }>(items: T[]) {
  const seen = new Set<string>()
  return items.filter(item => {
    const key = item.id ? String(item.id) : ''
    if (!key) return true
    if (seen.has(key)) return false
    seen.add(key)
    return true
  })
}

function applySampleData() {
  if (sampleDataApplied.value || !props.visible || !proposal.value) return
  form.reportDate = form.reportDate || dayjs().format('YYYY-MM-DD')
  form.linkContent = form.linkContent || 'https://example.com/events/report'
  form.linkFanpage = form.linkFanpage || 'https://facebook.com/example'

  if (!stockCosts.value.length) {
    const firstItem = itemStore.items[0]
    const qty = 10
    const price = firstItem?.price ?? 50000
    stockCosts.value = [
      {
        companyEventId: proposal.value?.id ?? '',
        itemId: firstItem?.id ?? '',
        itemName: firstItem?.itemName ?? 'Intro brochure',
        quantity: qty,
        maxQuantity: firstItem?.quantity ?? qty,
        publicationAmount: price,
        totalAmount: qty * price
      } as EditablePublication
    ]
  }

  if (!cashCosts.value.length) {
    cashCosts.value = [
      {
        companyEventId: proposal.value?.id ?? '',
        cashName: 'MC fee',
        quantity: 1,
        amount: 1500000,
        totalAmount: 1500000
      } as EventCashModel
    ]
  }

  if (!participants.value.length) {
    participants.value = [
      {
        companyEventId: proposal.value?.id ?? '',
        studentCode: 'SAMPLE001',
        isStudent: true,
        participantName: 'Sample Student A',
        participantGender: 'female',
        participantDateOfBirth: '2008-04-15',
        participantAddress: 'Hanoi',
        participantPhoneNumber: '0912345678',
        participantContact: 'Parent - Mr. An',
        participantEmail: 'student.a@example.com',
        participantSchool: 'THCS Le Loi',
        participantSourceKnown: 'Fanpage',
        participantJob: 'Student',
        employeeId: employeeStore.employees[0]?.id
          ? String(employeeStore.employees[0].id)
          : ''
      } as ParticipantRow,
      {
        companyEventId: proposal.value?.id ?? '',
        studentCode: 'SAMPLE002',
        isStudent: true,
        participantName: 'Sample Student B',
        participantGender: 'male',
        participantDateOfBirth: '2007-11-02',
        participantAddress: 'Hai Phong',
        participantPhoneNumber: '0987654321',
        participantContact: 'Parent - Ms. Huong',
        participantEmail: 'student.b@example.com',
        participantSchool: 'THPT Nguyen Trai',
        participantSourceKnown: 'Referral',
        participantJob: 'Student',
        employeeId: employeeStore.employees[1]?.id
          ? String(employeeStore.employees[1].id)
          : ''
      } as ParticipantRow
    ]
  }

  sampleDataApplied.value = true
}

async function submitReport() {
  if (!proposal.value) return
  const companyEventId = String(proposal.value.id ?? '')
  if (!isGuid(companyEventId)) {
    notificationStore.showToast('error', { key: 'common.systemError' })
    return
  }
  if (attachmentManagerRef.value?.uploadPendingFiles) {
    try {
      await attachmentManagerRef.value.uploadPendingFiles()
    } catch (error) {
      console.error('Upload pending attachments failed', error)
      notificationStore.showToast('error', { key: 'common.systemError' })
      return
    }
  }
  const reportId = normalizeGuid(props.report?.id)
  const packedAttachments =
    attachmentManagerRef.value?.packAttachments?.() ?? attachments.value ?? []
  const payload: CompanyEventReportModel = {
    id: reportId,
    companyEventId: companyEventId,
    companyEventReportCode: form.reportCode,
    eventDate: proposal.value.eventDate ?? '',
    reportDate: form.reportDate,
    numberStudents: participants.value.length,
    totalAmount: totalActualCost.value,
    linkContent: form.linkContent,
    linkFanpage: form.linkFanpage,
    attachments: mapAttachmentPayload(packedAttachments, reportId ?? undefined),
    eventPublications: stockCosts.value
      .filter(row => String(row.itemId ?? '').trim())
      .map(row => ({
        id: normalizeGuid(row.id),
        companyEventId: null,
        companyEventReportId: reportId,
        itemId: row.itemId,
        quantity: Number(row.quantity || 0),
        publicationAmount: Number(row.publicationAmount || 0),
        totalAmount: Number(row.totalAmount || 0)
      })),
    eventCashes: cashCosts.value
      .filter(row => String(row.cashName ?? '').trim())
      .map(row => ({
        id: normalizeGuid(row.id),
        companyEventId: null,
        companyEventReportId: reportId,
        cashName: row.cashName,
        quantity: Number(row.quantity || 0),
        amount: Number(row.amount || 0),
        totalAmount: Number(row.totalAmount || 0)
      })),
    eventParticipants: participants.value
      .filter(row => String(row.participantName ?? '').trim())
      .map(row => ({
        id: normalizeGuid(row.id),
        companyEventId: null,
        companyEventReportId: reportId,
        studentCode: row.studentCode,
        isStudent: row.isStudent ?? false,
        participantName: row.participantName ?? '',
        participantGender: row.participantGender,
        participantDateOfBirth: row.participantDateOfBirth,
        participantAddress: row.participantAddress,
        participantPhoneNumber: row.participantPhoneNumber,
        participantContact: row.participantContact,
        participantEmail: row.participantEmail,
        participantSchool: row.participantSchool,
        participantSourceKnown: row.participantSourceKnown,
        participantJob: row.participantJob,
        employeeId: row.employeeId
      }))
  }
  emit('submit', payload)
}

function openAttachmentPicker() {
  attachmentManagerRef.value?.openFilePicker?.()
}

function mapAttachments(list: any[]) {
  return (list || []).map((a: any) => ({
    uid: a.id || a.uid || crypto.randomUUID(),
    id: a.id,
    fileName: a.fileName || a.name || a.filePath || a.path,
    filePath: a.filePath || a.path || a.relativePath,
    relativePath: a.relativePath || a.filePath || a.path,
    path: a.path || a.filePath || a.relativePath,
    size: a.size,
    contentType: a.contentType
  }))
}

function mapAttachmentPayload(list: Attachment[], reportId?: string) {
  return (list || [])
    .filter(att => att?.path || (att as any)?.filePath || (att as any)?.relativePath)
    .map(att => {
    const resolvedPath = (att as any).path ?? (att as any).filePath ?? att.relativePath ?? ''
    return {
      id: normalizeGuid(att.id),
      companyEventReportId: reportId,
      fileName: att.fileName || undefined,
      filePath: resolvedPath,
      relativePath: att.relativePath ?? resolvedPath,
      path: resolvedPath,
      size: att.size,
      contentType: att.contentType
    }
  })
}

function isGuid(value?: string | null) {
  if (!value) return false
  return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(value)
}

function normalizeGuid(value?: string | null) {
  return isGuid(value) ? value : undefined
}

onMounted(async () => {
  if (!itemStore.items.length) {
    await itemStore.fetchAllItems()
  }
  if (!eventStore.events.length) {
    await eventStore.fetchAllEvents()
  }
  if (!employeeStore.employees.length) {
    await employeeStore.fetchAllEmployees()
  }
})

watch(() => props.visible, visible => {
  if (visible) {
    applyReportData()
  } else {
    sampleDataApplied.value = false
  }
})

watch(() => props.report, report => {
  if (!report) {
    sampleDataApplied.value = false
  }
  if (props.visible) applyReportData()
})

watch(
  [() => props.visible, () => eventStore.events.length, () => proposal.value?.allocationDetailEvent?.eventId, () => props.report],
  ([visible]) => {
    if (!visible || props.report) return
    if (form.reportCode) return
    void generateReportCode()
  }
)
</script>

<style scoped>
.report-form :deep(.el-form-item) {
  margin-bottom: 12px;
}

.company-report-dialog.metronic-dialog {
  max-width: 1200px !important;
  width: min(1200px, 96vw) !important;
}

.action-btn {
  padding: 4px 10px;
  border-radius: 6px;
  color: #fff;
}

.action-btn.add {
  background: #2563eb;
}

.action-btn.delete {
  background: #e40505;
}

.action-btn:disabled,
.action-btn.is-disabled,
:deep(.action-btn.is-disabled) {
  background: #d1d5db !important;
  color: #f3f4f6 !important;
  cursor: not-allowed;
}

.attachments-manager :deep(.item-card) {
  border: 1px solid var(--el-border-color) !important;
}

.attachments-manager :deep(.item-card.is-even),
.attachments-manager :deep(.item-card.is-odd),
.attachments-manager :deep(.item-card.is-even .el-card__body),
.attachments-manager :deep(.item-card.is-odd .el-card__body) {
  background: #f8fafc !important;
}

.attachments-manager :deep(.el-card__body) {
  padding: 10px 14px !important;
}

.attachments-manager :deep(img.upload-thumb) {
  width: 80px;
  height: 80px;
  object-fit: cover;
  border-radius: 8px;
}

.attachments-manager :deep(.upload-thumb:not(img)) {
  width: auto;
  max-width: 100%;
  height: auto;
  min-height: 32px;
  padding: 0;
  margin-left: 0;
  display: inline-block;
  white-space: normal;
  word-break: break-all;
  text-overflow: initial;
  background: transparent;
  border: none;
  box-shadow: none;
  color: inherit;
}
</style>
