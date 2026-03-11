<template>
  <BaseDialogForm class="company-proposal-dialog" :visible="visible" :title="t('allocationEvent.proposal.title')"
    :description="t('allocationEvent.proposal.subTitle', { code: branchRow?.allocationCode ?? '' })" :mode="dialogMode"
    :form-data="form" width="90%" :submit-disabled="!canSubmit || props.readonly" :loading="isSubmitting"
    :show-delete="false" :height="'80vh'" @update:visible="emit('update:visible', $event)" @submit="submitProposal"
    @close="close">
    <template #form>
      <div v-if="allocationEvent && branchRow" class="proposal-form">
        <el-form :model="form" label-position="top" :inline="false">
          <el-row :gutter="20">
            <el-col :span="12">
              <el-form-item :label="t('allocationEvent.code')">
                <el-input v-model="form.allocationCode" disabled />
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.region')">
                <el-input v-model="branchRow.regionName" disabled />
              </el-form-item>

              <el-form-item :label="t('allocationEvent.proposal.branchEvent')">
                <el-select v-model="form.selectedDetailId" filterable>
                  <el-option v-for="opt in branchEventOptions" :key="opt.id" :label="opt.label" :value="opt.id" />
                </el-select>
              </el-form-item>
              <el-row :gutter="16">
                <el-col :span="8">
                  <el-form-item :label="t('allocationEvent.proposal.eventUsage')">
                    <el-input-number v-model="form.usedEvents" :min="0" :disabled="true" />
                  </el-form-item>
                </el-col>
                <el-col :span="8">
                  <el-form-item :label="t('allocationEvent.proposal.eventRemain')">
                    <el-input-number v-model="form.remainingEvents" :min="0" :disabled="true" />
                  </el-form-item>
                </el-col>
                <el-col :span="8">
                  <el-form-item :label="t('allocationEvent.proposal.proposalQuantity')">
                    <el-input-number v-model="form.proposalQuantity" :min="form.remainingEvents > 0 ? 1 : 0"
                      :max="form.remainingEvents" :disabled="form.remainingEvents <= 0" />
                  </el-form-item>
                </el-col>
              </el-row>
              <el-form-item :label="t('allocationEvent.proposal.proposalDate')">
                <el-date-picker v-model="form.proposalDate" type="date" value-format="YYYY-MM-DD" />
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.eventDate')">
                <el-date-picker v-model="form.eventDate" type="date" value-format="YYYY-MM-DD" />
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.schoolName')">
                <el-select v-model="form.affiliatePartnerId" filterable>
                  <el-option v-for="opt in affiliatePartnerOptions" :key="opt.value" :label="opt.label"
                    :value="opt.value" />
                </el-select>
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.educationLevel')">
                <el-input :model-value="gradeLabel" disabled />
              </el-form-item>
              <!-- <el-form-item :label="t('allocationEvent.proposal.partnerCode')">
                <el-input :model-value="affiliatePartnerCodeLabel" disabled />
              </el-form-item> -->
              <!-- <el-form-item
                v-if="affiliatePartnerSchoolLevelLabel"
                :label="t('allocationEvent.proposal.partnerSchoolLevel')"
              >
                <el-input :model-value="affiliatePartnerSchoolLevelLabel" disabled />
              </el-form-item> -->
              <el-form-item :label="t('allocationEvent.proposal.eventName')">
                <el-input v-model="form.eventName" />
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.proposalContent')">
                <el-input v-model="form.proposalContent" type="textarea" :rows="3" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('allocationEvent.proposal.proposalCode')">
                <el-input v-model="form.proposalCode" />
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.companyScope')">
                <el-select v-model="form.company" disabled>
                  <el-option :label="branchRow.companyName" :value="branchRow.companyId" />
                </el-select>
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.eventSize')">
                <el-select v-model="form.eventSize">
                  <el-option v-for="opt in eventSizeOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
                </el-select>
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.allocatedBudget')">
                <el-input v-model="formattedAllocatedBudget" disabled />
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.usedBudget')">
                <el-input-number v-model="form.usedBudget" :min="0" :disabled="true" />
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.remainingBudget')">
                <el-input v-model="formattedBudgetRemain" disabled />
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.totalProjectedCost')">
                <el-input :model-value="formatCurrency(totalProjectedCost)" disabled />
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.budgetRate')">
                <el-input v-model="form.budgetRate" readonly>
                  <template #suffix>%</template>
                </el-input>
              </el-form-item>

              <el-form-item :label="t('allocationEvent.proposal.ruleNote')">
                <el-input v-model="form.ruleNote" />
              </el-form-item>
              <el-form-item :label="t('allocationEvent.proposal.proposalDescription')">
                <el-input type="textarea" :rows="3" v-model="form.proposalDescription" />
              </el-form-item>

            </el-col>
          </el-row>
        </el-form>

        <!-- STOCK COSTS -->
        <section class="mt-4">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <h5 class="fw-semibold mb-0">{{ t('allocationEvent.proposal.stockCostTitle') }}</h5>
            <div class="d-flex gap-2">
              <!-- <el-button size="small" class="action-btn delete" type="danger" text @click="removeLastStockRow"
                :disabled="stockCosts.length === 0">
                {{ t('common.delete') }}
              </el-button> -->
              <el-button size="small" class="action-btn add" type="primary" text @click="addStockRow">
                <el-icon>
                  <Plus />
                </el-icon>
                {{ t('common.add') }}
              </el-button>
            </div>
          </div>
          <input ref="participantImportRef" type="file" accept=".xlsx,.xls,.csv" class="d-none"
            @change="handleParticipantImportFile" />

          <BaseTable :columns="stockColumns" :items="stockCosts" :showCheckboxColumn="false" :showActionsColumn="false"
            :height="220">
            <template #cell-rowNumber="{ item }">
              {{ stockCosts.indexOf(item) + 1 }}
            </template>
            <template #cell-itemId="{ item }">
              <el-select v-model="item.itemId" filterable @change="onPublicationChange(item)">
                <el-option v-for="opt in getPublicationOptionsForRow(item)" :key="opt.value" :label="opt.label"
                  :value="opt.value" />
              </el-select>
            </template>
            <template #cell-quantity="{ item }">
              <el-input-number v-model="item.quantity" :min="0" :max="item.maxQuantity"
                @change="recalcPublication(item)" />
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

        <!-- CASH COSTS -->
        <section class="mt-4">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <h5 class="fw-semibold mb-0">{{ t('allocationEvent.proposal.cashCostTitle') }}</h5>
            <div class="d-flex gap-2">
              <!-- <el-button size="small" class="action-btn delete" type="danger" text @click="removeLastCashRow"
                :disabled="cashCosts.length === 0">
                {{ t('common.delete') }}
              </el-button> -->
              <el-button size="small" class="action-btn add" type="primary" text @click="addCashRow">
                <el-icon>
                  <Plus />
                </el-icon>
                {{ t('common.add') }}
              </el-button>
            </div>
          </div>
          <BaseTable :columns="cashColumns" :items="cashCosts" :showCheckboxColumn="false" :showActionsColumn="false"
            :height="220">
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

        <!-- PARTICIPANTS -->
        <section class="mt-4">
          <div class="d-flex justify-content-between align-items-center mb-2">
            <h5 class="fw-semibold mb-0">
              {{ t('allocationEvent.proposal.participantTitle') }}
            </h5>
            <div class="d-flex gap-2">
              <el-button size="small" :loading="participantImportLoading" @click="triggerParticipantImport">
                <el-icon>
                  <Upload />
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

        <!-- ATTACHMENTS -->
        <section class="mt-4">
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
            :removed-ids="removedAttachmentIds" @update:removedIds="removedAttachmentIds = $event"
            :enableDownload="isEditMode" class="attachments-manager" :itemTitle="''" :hideHeader="true" />
          <div class="text-muted fs-8 mt-2">
            {{ t('allocationEvent.proposal.attachmentNote') }}
          </div>
        </section>

        <!-- APPROVAL HISTORY -->
        <section class="mt-4" v-if="isEditMode">
          <h5 class="fw-semibold mb-2">
            {{ t('allocationEvent.proposal.approvalHistory') }}
          </h5>
          <BaseTable :columns="approvalHistoryColumns" :items="approvalHistoryRows" :showCheckboxColumn="false"
            :showActionsColumn="false" :height="220" :loading="usageLoading">
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
          <div v-if="!usageLoading && approvalHistoryRows.length === 0" class="text-center text-muted py-3 fs-8">
            {{ t('common.noData') }}
          </div>
        </section>
      </div>
    </template>

    <template #footer-extra>
      <div v-if="canApproveCurrent" class="d-inline-flex gap-2 align-items-center">
        <el-button type="success" plain size="default" :loading="approving"
          @click="openApproval(CompanyEventProposalStatus.Approved)">
          {{ t('common.approve') }}
        </el-button>
        <el-button type="danger" plain size="default" :loading="approving"
          @click="openApproval(CompanyEventProposalStatus.Rejected)">
          {{ t('common.reject') }}
        </el-button>
      </div>
    </template>
  </BaseDialogForm>

  <!-- APPROVAL DIALOG -->
  <el-dialog v-model="approvalDialogVisible"
    :title="approvalStatus === CompanyEventProposalStatus.Approved ? t('common.approve') : t('common.reject')"
    width="480px">
    <el-form label-position="top">
      <el-form-item :label="t('allocationEvent.proposal.approveReason')">
        <el-input v-model="approvalReason" type="textarea" :rows="3"
          :placeholder="t('allocationEvent.proposal.approveReasonPlaceholder')" />
      </el-form-item>
    </el-form>
    <template #footer>
      <div class="text-end">
        <el-button @click="approvalDialogVisible = false">
          {{ t('common.cancel') }}
        </el-button>
        <el-button type="primary" :loading="approving" @click="submitApproval">
          {{ approvalStatus === CompanyEventProposalStatus.Approved ? t('common.approve') : t('common.reject') }}
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch, onMounted } from 'vue'
import dayjs from 'dayjs'
import * as XLSX from 'xlsx'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { Delete, Plus, Upload } from '@element-plus/icons-vue'
import { formatCurrency } from '@/utils/format'
import type {
  AllocationEventModel,
  AllocationDetailEventModel,
  EventCashModel,
  EventParticipantModel,
  EventPublicationModel,
  CompanyEventProposalRequest,
  CompanyEventModel,
  ApproveCompanyEventModel
} from '@/api/AllocationEventApi'
import { useItemStore } from '@/stores/itemStore'
import { useEventStore } from '@/stores/eventStore'
import { useCommonStore } from '@/stores/commonStore'
import { useAffiliatePartnerStore } from '@/stores/affiliatePartnerStore'
import { useEmployeeStore } from '@/stores/employeeStore'
import { useStudentStore } from '@/stores/studentStore'
import { CompanyEventService } from '@/services/CompanyEventService'
import type { ItemModel } from '@/api/ItemApi'
import type { EmployeeModel } from '@/api/EmployeeApi'
import type { StudentModel } from '@/api/StudentApi'
import { EventSize, CompanyEventProposalStatus, CompanyEventProposalStatusLabels, SchoolLevelTypeLabels } from '@/types'
import FileManager from '@/components/file-manager/FileManager.vue'
import type { Attachment, FieldSchema } from '@/api/FileApi'
import { useNotificationStore } from '@/stores/notificationStore'

interface BranchRow {
  companyId?: string | number | null
  companyName: string
  regionName: string
  regionId?: string | number | null
  totalBudget: number
  allocationCode?: string
  totalQuantity?: number
  details?: AllocationDetailEventModel[]
}

const props = defineProps<{
  visible: boolean
  allocationEvent: AllocationEventModel | null
  branchRow: BranchRow | null
  submitting?: boolean
  proposal?: CompanyEventModel | null
  readonly?: boolean
  showApprovalActions?: boolean
}>()
const emit = defineEmits(['update:visible', 'submit'])
const { t } = useI18n()
const itemStore = useItemStore()
const eventStore = useEventStore()
const commonStore = useCommonStore()
const affiliatePartnerStore = useAffiliatePartnerStore()
const employeeStore = useEmployeeStore()
const studentStore = useStudentStore()
const companyEventService = new CompanyEventService()
const notificationStore = useNotificationStore()

const visible = computed(() => props.visible)
const isSubmitting = computed(() => props.submitting ?? false)
const isEditMode = computed(() => !!props.proposal)
const dialogMode = computed<'create' | 'edit' | 'view'>(() => {
  if (props.readonly) return 'view'
  return isEditMode.value ? 'edit' : 'create'
})

const form = reactive({
  allocationCode: '',
  proposalCode: '',
  company: '',
  selectedDetailId: '',
  proposalQuantity: 1,
  usedEvents: 0,
  remainingEvents: 0,
  proposalDate: dayjs().format('YYYY-MM-DD'),
  eventDate: '',
  affiliatePartnerId: '',
  ruleNote: '',
  budgetRate: '0',
  usedBudget: 0,
  eventName: '',
  proposalContent: '',
  grade: null as number | null,
  proposalDescription: '',
  eventSize: EventSize.Mini,
  attachments: ['Form đề xuất', 'Ấn phẩm cần thiết kế']
})

const eventSizeOptions = computed(() => [
  { label: t('allocationEvent.proposal.size.mini'), value: EventSize.Mini },
  { label: t('allocationEvent.proposal.size.big'), value: EventSize.Big }
])

const gradeLabel = computed(() => {
  if (form.grade == null) return ''
  const key = SchoolLevelTypeLabels[form.grade]
  return key ? t(key) : ''
})

type EditablePublication = EventPublicationModel & { itemName?: string; maxQuantity?: number; price?: number }
type ParticipantRow = EventParticipantModel & { studentCode?: string }

const stockCosts = ref<EditablePublication[]>([])
const cashCosts = ref<EventCashModel[]>([])
const participants = ref<ParticipantRow[]>([])
const attachments = ref<Attachment[]>([])
const participantImportRef = ref<HTMLInputElement | null>(null)
const participantImportLoading = ref(false)
const attachmentManagerRef = ref<any>(null)
const attachmentFields: FieldSchema[] = []
const existingProposals = ref<CompanyEventModel[]>([])
const allProposals = ref<CompanyEventModel[]>([])
const usageByDetail = ref<Map<string, { quantity: number; budget: number }>>(new Map())
const companyBudgetUsage = ref<Map<string, number>>(new Map())
const usageLoading = ref(false)
const attachmentsLoading = ref(false)
const removedAttachmentIds = ref<string[]>([])
const removedPublicationIds = ref<string[]>([])
const removedCashIds = ref<string[]>([])
const removedParticipantIds = ref<string[]>([])
const sampleDataApplied = ref(false)
const approvalDialogVisible = ref(false)
const approvalStatus = ref<CompanyEventProposalStatus | null>(null)
const approvalReason = ref('')
const approving = ref(false)
const isCompanyManager = ref(false)

const participantColumns: BaseTableColumn[] = [
  { key: 'studentCode', labelKey: 'allocationEvent.proposal.participantStudentCode', width: 160 },
  { key: 'fullName', labelKey: 'allocationEvent.proposal.participantName', width: 180 },
  { key: 'gender', labelKey: 'allocationEvent.proposal.participantGender', width: 120 },
  { key: 'dateOfBirth', labelKey: 'allocationEvent.proposal.participantDob', width: 150 },
  { key: 'address', labelKey: 'allocationEvent.proposal.participantAddress', width: 180 },
  { key: 'isStudent', labelKey: 'allocationEvent.proposal.participantIsStudent', width: 160 },
  { key: 'phone', labelKey: 'allocationEvent.proposal.participantPhone', width: 150 },
  { key: 'contact', labelKey: 'allocationEvent.proposal.participantContact', width: 150 },
  { key: 'email', labelKey: 'allocationEvent.proposal.participantEmail', width: 200 },
  { key: 'school', labelKey: 'allocationEvent.proposal.participantSchool', width: 180 },
  { key: 'source', labelKey: 'allocationEvent.proposal.participantSource', width: 160 },
  { key: 'job', labelKey: 'allocationEvent.proposal.participantJob', width: 150 },
  { key: 'advisor', labelKey: 'allocationEvent.proposal.participantAdvisor', width: 200 },
  { key: 'rowActions', labelKey: 'common.actions', width: 120, align: 'center' }
]

const stockColumns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center' },
  { key: 'itemId', labelKey: 'allocationEvent.proposal.stockProduct', minWidth: 260 },
  { key: 'quantity', labelKey: 'allocationEvent.proposal.quantity', width: 220, align: 'center' },
  { key: 'publicationAmount', labelKey: 'allocationEvent.proposal.unitAmount', width: 220, align: 'right' },
  { key: 'totalAmount', labelKey: 'allocationEvent.proposal.totalAmount', width: 220, align: 'right' },
  { key: 'rowActions', labelKey: 'common.actions', width: 100, align: 'center' }
]

const cashColumns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center' },
  { key: 'cashName', labelKey: 'allocationEvent.proposal.cashName' },
  { key: 'quantity', labelKey: 'allocationEvent.proposal.quantity', width: 180, align: 'center' },
  { key: 'amount', labelKey: 'allocationEvent.proposal.unitAmount', width: 180, align: 'right' },
  { key: 'totalAmount', labelKey: 'allocationEvent.proposal.totalAmount', width: 180, align: 'right' },
  { key: 'rowActions', labelKey: 'common.actions', width: 100, align: 'center' }
]

/* ===== APPROVAL HISTORY ===== */

const approvalHistoryColumns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center' },
  { key: 'statusLabel', labelKey: 'common.status', width: 140 },
  { key: 'reason', labelKey: 'allocationEvent.proposal.approveReason', minWidth: 220 },
  { key: 'createdByName', labelKey: 'allocationEvent.proposal.approveBy', width: 160 },
  { key: 'createdAt', labelKey: 'allocationEvent.proposal.approveDate', width: 180, align: 'center' }
]

const approvalStatusColorMap = computed(() => ({
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Draft])]: 'warning',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.PendingApproval])]: 'primary',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Approved])]: 'success',
  [t(CompanyEventProposalStatusLabels[CompanyEventProposalStatus.Rejected])]: 'red'
}))

function approvalStatusText(status: CompanyEventProposalStatus | number | null | undefined) {
  const s = status as CompanyEventProposalStatus | undefined
  if (s != null && CompanyEventProposalStatusLabels[s]) {
    return t(CompanyEventProposalStatusLabels[s])
  }
  return t('common.unknown')
}

const approvalHistorySource = computed(() => {
  const proposalId = props.proposal?.id
  if (!proposalId) return []

  const direct = props.proposal?.approveCompanyEvents
  if (direct?.length) return direct

  const fromAll = allProposals.value.find(p => String(p.id) === String(proposalId))?.approveCompanyEvents
  return fromAll ?? []
})

const approvalHistoryRows = computed(() => {
  const items = approvalHistorySource.value || []
  if (!items.length) return []

  return items
    .slice()
    .sort((a, b) => {
      const at = a.createdAt ? new Date(a.createdAt).getTime() : 0
      const bt = b.createdAt ? new Date(b.createdAt).getTime() : 0
      return at - bt
    })
    .map((h, index) => ({
      rowNumber: index + 1,
      statusLabel: approvalStatusText(h.approveStatus),
      reason: h.reason || '',
      createdByName: h.createdBy || '',
      createdAt: h.createdAt ? dayjs(h.createdAt).format('YYYY-MM-DD HH:mm') : ''
    }))
})

/* ===== EXISTING LOGIC (giữ nguyên) ===== */

// Use `allocationEvent.eventBudget` as the source of allocated budget for the branch.
// Fallback to `branchRow.totalBudget` if `allocationEvent` is not provided.
const totalBranchBudget = computed(() =>
  Number(props.allocationEvent?.eventBudget ?? props.branchRow?.totalBudget ?? 0)
)
const formattedAllocatedBudget = computed(() => formatCurrency(totalBranchBudget.value))

const totalStockCost = computed(() =>
  stockCosts.value.reduce((sum, row) => sum + Number(row.totalAmount || 0), 0)
)
const totalCashCost = computed(() =>
  cashCosts.value.reduce((sum, row) => sum + Number(row.totalAmount || 0), 0)
)
const totalProjectedCost = computed(() => totalStockCost.value + totalCashCost.value)

const formattedBudgetRemain = computed(() => {
  const used = Number(form.usedBudget || 0)
  const projected = Number(totalProjectedCost.value || 0)
  const remain = totalBranchBudget.value - (used + projected)
  console.log('Formatted budget remain:', { used, projected, remain })
  return formatCurrency(Math.max(remain, 0))
})
function recalcBudgetRate() {
  const allocated = totalBranchBudget.value || 0
  if (!allocated) {
    form.budgetRate = '0'
    return
  }
  // Số ngân sách đã phân bổ cho công ty/chi nhánh (từ `form.usedBudget`)
  const usedAllocated = Number(form.usedBudget || 0)
  // Ngân sách dự kiến của proposal hiện tại
  const projected = Number(totalProjectedCost.value || 0)
  console.log('Recalc budget rate:', { allocated, usedAllocated, projected })
  const rate = ((usedAllocated + projected) / allocated) * 100
  // Hiển thị 2 chữ số thập phân
  form.budgetRate = isFinite(rate) ? rate.toFixed(2) : '0'
}
watch(
  [() => form.usedBudget, () => totalProjectedCost.value, () => totalBranchBudget.value],
  () => {
    recalcBudgetRate()
  },
  { immediate: true }
)

const eventNameMap = computed(() => {
  const map = new Map<string, string>()
  eventStore.events.forEach(ev => map.set(String(ev.id), ev.eventName))
  return map
})
const branchCompanyKey = computed(() => String(props.branchRow?.companyId ?? ''))

const affiliatePartnerOptions = computed(() =>
  (affiliatePartnerStore.affiliatePartners || []).map(partner => {
    const codeSuffix = partner.partnerCode ? ` (${partner.partnerCode})` : ''
    const levelKey = partner.schoolLevel != null ? SchoolLevelTypeLabels[partner.schoolLevel] : ''
    const levelSuffix = levelKey ? ` - ${t(levelKey)}` : ''
    const displayName = partner.partnerName || partner.partnerCode || ''
    return {
      value: String(partner.id),
      label: `${displayName}${codeSuffix}${levelSuffix}`
    }
  })
)

const selectedAffiliatePartner = computed(() =>
  (affiliatePartnerStore.affiliatePartners || []).find(partner => String(partner.id) === form.affiliatePartnerId) || null
)

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

const eventCodeMap = computed(() => {
  const map = new Map<string, string>()
  eventStore.events.forEach(ev => map.set(String(ev.id), ev.eventCode || ''))
  return map
})

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

const branchEventOptions = computed(() => {
  const details = props.branchRow?.details || []
  return details.map(detail => {
    const eventId = String(detail.eventId)
    const name = eventNameMap.value.get(eventId) || t('common.unknown')
    const allocated = Number(detail.quantity) || 0
    const id = getDetailKey(detail)
    const usage = usageByDetail.value.get(id) || { quantity: 0, budget: 0 }
    return {
      id,
      eventId,
      name,
      allocated,
      used: usage.quantity,
      detail,
      label: `${name} (${usage.quantity}/${allocated})`
    }
  })
})

const selectedDetail = computed(
  () => branchEventOptions.value.find(opt => opt.id === form.selectedDetailId) || null
)

const canSubmit = computed(
  () =>
    !!selectedDetail.value &&
    (isEditMode.value || form.proposalQuantity > 0) &&
    !!form.affiliatePartnerId &&
    !!form.eventDate
)

const canApproveCurrent = computed(() => {
  if (!props.showApprovalActions || !isEditMode.value || !props.proposal?.id) return false
  const status = getProposalStatus(props.proposal)
  return (
    (status === CompanyEventProposalStatus.PendingApproval ||
      status === CompanyEventProposalStatus.Draft) && !isCompanyManager.value
  )
})

const lastGeneratedDetailId = ref<string | null>(null)
const generatingProposalCode = ref(false)
const isHydratingProposal = ref(false)

function resolveEventCode(detailId?: string | null) {
  if (!detailId) return ''
  const detail = branchEventOptions.value.find(opt => opt.id === detailId)
  const eventId = detail?.eventId || String(detail?.detail?.eventId ?? '')
  if (!eventId) return ''
  return eventCodeMap.value.get(String(eventId)) || ''
}

async function generateProposalCode(detailId?: string | null) {
  if (!detailId || generatingProposalCode.value || props.readonly) return
  const eventCode = resolveEventCode(detailId).trim()
  if (!eventCode) return
  generatingProposalCode.value = true
  try {
    const generated = await commonStore.generateCode(`DX_${eventCode}_`, 'CompanyEvent', 'CompanyEventCode', 4)
    if (generated) {
      form.proposalCode = generated
      lastGeneratedDetailId.value = detailId
    }
  } catch (err) {
    console.error('Error generating proposal code:', err)
  } finally {
    generatingProposalCode.value = false
  }
}

const publicationOptions = computed(() =>
  (itemStore.items || [])
    .filter(item => Number(item.quantity || 0) > 0)
    .map(item => ({
      value: item.id,
      label: `${item.itemCode} - ${item.itemName} (${item.quantity})`,
      model: item
    }))
)

const selectedStockItemIds = computed(() => {
  const set = new Set<string | number>()
  stockCosts.value.forEach(row => {
    if (row.itemId) set.add(row.itemId)
  })
  return set
})

function getPublicationOptionsForRow(row: EditablePublication) {
  return publicationOptions.value.filter(opt => {
    if (!opt.value) return true
    if (String(opt.value) === String(row.itemId)) return true
    return !selectedStockItemIds.value.has(opt.value)
  })
}

function recalcPublication(row: EditablePublication) {
  if (row.maxQuantity != null && Number(row.quantity || 0) > Number(row.maxQuantity)) {
    row.quantity = row.maxQuantity
  }
  row.totalAmount = Number(row.quantity || 0) * Number(row.publicationAmount || 0)
}
function recalcCash(row: EventCashModel) {
  row.totalAmount = Number(row.quantity || 0) * Number(row.amount || 0)
}
function addStockRow() {
  stockCosts.value.push({
    companyEventId: '',
    itemId: '',
    item: undefined,
    itemName: '',
    maxQuantity: undefined,
    quantity: 0,
    publicationAmount: 0,
    totalAmount: 0
  } as any)
}
function removeStockRow(idx: number) {
  const removed = stockCosts.value[idx]
  if (removed?.id) removedPublicationIds.value.push(String(removed.id))
  stockCosts.value.splice(idx, 1)
}
function removeLastStockRow() {
  if (stockCosts.value.length > 0) {
    const removed = stockCosts.value.pop()
    if (removed?.id) removedPublicationIds.value.push(String(removed.id))
  }
}
function addCashRow() {
  cashCosts.value.push({
    companyEventId: '',
    cashName: '',
    quantity: 0,
    amount: 0,
    totalAmount: 0
  } as any)
}
function removeCashRow(idx: number) {
  const removed = cashCosts.value[idx]
  if (removed?.id) removedCashIds.value.push(String(removed.id))
  cashCosts.value.splice(idx, 1)
}
function removeLastCashRow() {
  if (cashCosts.value.length > 0) {
    const removed = cashCosts.value.pop()
    if (removed?.id) removedCashIds.value.push(String(removed.id))
  }
}

function addParticipant() {
  participants.value.push({
    companyEventId: '',
    studentCode: '',
    isStudent: true,
    participantName: '',
    participantGender: '',
    participantDateOfBirth: '',
    participantAddress: '',
    participantPhoneNumber: '',
    participantContact: '',
    participantEmail: '',
    participantSchool: '',
    participantSourceKnown: '',
    participantJob: '',
    employeeId: ''
  } as any)
}
function removeParticipant(idx: number) {
  const removed = participants.value[idx]
  if (removed?.id) removedParticipantIds.value.push(String(removed.id))
  participants.value.splice(idx, 1)
}
function removeLastParticipant() {
  if (participants.value.length > 0) {
    const removed = participants.value.pop()
    if (removed?.id) removedParticipantIds.value.push(String(removed.id))
  }
}

function onPublicationChange(row: EditablePublication) {
  const option = publicationOptions.value.find(opt => opt.value === row.itemId)
  if (!option) {
    row.itemName = ''
    row.maxQuantity = undefined
    row.publicationAmount = 0
    row.quantity = 0
    row.totalAmount = 0
    return
  }
  const item = option.model as ItemModel
  row.itemName = item.itemName
  row.maxQuantity = item.quantity
  row.publicationAmount = item.price ?? 0
  row.quantity = Math.min(row.quantity || 0, Number(item.quantity || 0))
  recalcPublication(row)
}

watch(
  [() => props.branchRow, () => props.allocationEvent],
  ([branch]) => {
    if (!branch) return
    form.allocationCode = branch.allocationCode || ''
    form.company = String(branch.companyId ?? '')
    form.usedBudget = 0
    form.eventName = branch.companyName
    form.proposalContent = ''
    form.eventDate = dayjs().format('YYYY-MM-DD')
    form.selectedDetailId = branchEventOptions.value[0]?.id || ''
  },
  { immediate: true }
)

watch(
  () => props.visible,
  visible => {
    if (visible) {
      if (!props.proposal) {
        applySampleData()
        removedAttachmentIds.value = []
        removedPublicationIds.value = []
        removedCashIds.value = []
        removedParticipantIds.value = []
      }
      if (props.proposal && props.proposal.attachments?.length) {
        attachments.value = mapAttachments(props.proposal.attachments as any)
        removedAttachmentIds.value = []
      }
    } else {
      removedAttachmentIds.value = []
      removedPublicationIds.value = []
      removedCashIds.value = []
      removedParticipantIds.value = []
      sampleDataApplied.value = false
    }
  }
)

watch(
  () => props.proposal,
  proposal => {
    if (!proposal) {
      stockCosts.value = []
      cashCosts.value = []
      participants.value = []
      attachments.value = []
      removedAttachmentIds.value = []
      removedPublicationIds.value = []
      removedCashIds.value = []
      removedParticipantIds.value = []
      lastGeneratedDetailId.value = null
      isHydratingProposal.value = false
      return
    }
    isHydratingProposal.value = true

    console.log('[ProposalDialog] incoming proposal', proposal)

    form.proposalCode = proposal.companyEventCode || form.proposalCode
    form.eventName = proposal.companyEventName || form.eventName
    form.eventDate = proposal.eventDate || form.eventDate
    form.affiliatePartnerId = proposal.affiliatePartnerId
      ? String(proposal.affiliatePartnerId)
      : form.affiliatePartnerId
    form.eventSize = proposal.eventSize ?? form.eventSize
    form.proposalContent = proposal.propose || ''
    form.proposalQuantity = Number((proposal as any).proposalQuantity ?? 1)

    const detailKey = getProposalDetailKey(proposal)
    if (detailKey) {
      form.selectedDetailId = detailKey
    }
    lastGeneratedDetailId.value = detailKey || null

    const pubs = dedupeById(
      proposal.eventPublications ??
        (proposal as any).eventPublications ??
        (proposal as any).eventPublication ??
        proposal.publications ??
        []
    )
    stockCosts.value = pubs.map((pub: any) => {
      const item = itemStore.items.find(i => String(i.id) === String(pub.itemId))
      const maxQuantity = item?.quantity
      const unit = pub.publicationAmount ?? item?.price ?? 0
      const total = pub.totalAmount ?? Number(pub.quantity || 0) * Number(unit || 0)
      return {
        ...pub,
        itemName: item?.itemName,
        maxQuantity,
        publicationAmount: unit,
        totalAmount: total
      }
    })

    const cashList = dedupeById(
      proposal.eventCashes ?? (proposal as any).eventCashes ?? proposal.cashCosts ?? []
    )
    cashCosts.value = cashList.map((c: any) => ({
      ...c,
      quantity: Number(c.quantity || 0),
      amount: Number(c.amount || 0),
      totalAmount: Number(c.totalAmount || 0)
    }))

    const participantList = dedupeById(
      proposal.eventParticipants ??
        (proposal as any).eventParticipants ??
        proposal.participants ??
        []
    )
    participants.value = participantList.map((p: any) => ({
      ...p,
      studentCode: p.studentCode ?? ''
    }))

    const attachList = (proposal as any).attachments ?? []
    attachments.value = mapAttachments(attachList)
    removedAttachmentIds.value = []
    removedPublicationIds.value = []
    removedCashIds.value = []
    removedParticipantIds.value = []

    updateUsage(selectedDetail.value)
    isHydratingProposal.value = false
  },
  { immediate: true }
)

watch(
  [() => form.selectedDetailId, () => eventStore.events.length],
  async ([detailId]) => {
    if (!detailId || props.readonly || isHydratingProposal.value) return
    if (lastGeneratedDetailId.value === detailId) return
    await generateProposalCode(detailId)
  }
)

watch(
  () => props.branchRow?.companyId,
  async () => {
    if (visible.value) {
      await ensureUsage()
    }
  }
)

watch(branchEventOptions, options => {
  if (!form.selectedDetailId && options.length) {
    form.selectedDetailId = options[0].id
  }
})

watch(affiliatePartnerOptions, options => {
  if (!form.affiliatePartnerId && options.length) {
    form.affiliatePartnerId = options[0].value
  }
})

watch(selectedDetail, detail => {
  if (detail?.detail) {
    console.log('[Proposal] selected detail:', {
      detailId: detail.detail.id,
      eventId: detail.detail.eventId,
      companyId: detail.detail.companyId,
      quantity: detail.detail.quantity,
      used: usageByDetail.value.get(getDetailKey(detail.detail))?.quantity,
      budgetUsed: companyBudgetUsage.value.get(branchCompanyKey.value),
      key: getDetailKey(detail.detail)
    })
  }
  updateUsage(detail)
})

watch(
  () => selectedAffiliatePartner.value,
  partner => {
    form.grade = partner?.schoolLevel ?? null
  },
  { immediate: true }
)

watch(allProposals, () => {
  updateUsage(selectedDetail.value)
})

watch(
  () => props.visible,
  async val => {
    if (val) {
      await ensureUsage()
    }
  }
)

onMounted(async () => {
  await ensureData()
  await ensureUsage()
  isCompanyManager.value = await employeeStore.checkIsCompanyManager()
})

function close() {
  emit('update:visible', false)
  removedAttachmentIds.value = []
  removedPublicationIds.value = []
  removedCashIds.value = []
  removedParticipantIds.value = []
  sampleDataApplied.value = false
  approvalDialogVisible.value = false
}

async function submitProposal() {
  if (props.readonly) return
  if (!canSubmit.value || !selectedDetail.value) return

  if (attachmentManagerRef.value?.uploadPendingFiles) {
    attachmentsLoading.value = true
    try {
      await attachmentManagerRef.value.uploadPendingFiles()
    } catch (err) {
      console.error('Upload pending attachments failed', err)
      attachmentsLoading.value = false
      return
    }
    attachmentsLoading.value = false
  }

  const packedAttachments =
    attachmentManagerRef.value?.packAttachments?.() ?? attachments.value ?? []

  const payload: CompanyEventProposalRequest = {
    companyEvent: {
      id: props.proposal?.id || undefined,
      allocationDetailEventId: selectedDetail.value.detail.id || '',
      companyEventCode: form.proposalCode,
      companyEventName: form.eventName,
      eventDate: form.eventDate,
      affiliatePartnerId: form.affiliatePartnerId,
      companyId: props.branchRow?.companyId ? String(props.branchRow.companyId) : undefined,
      regionId: props.branchRow?.regionId ? String(props.branchRow.regionId) : undefined,
      numberStudents: participants.value.length,
      propose: form.proposalContent,
      totalAmount: totalProjectedCost.value,
      eventSize: form.eventSize,
      companyEventStatus: CompanyEventProposalStatus.Draft
    },
    publications: stockCosts.value
      .filter(row => row.itemId)
      .map(row => ({
        id: row.id,
        itemId: row.itemId,
        quantity: Number(row.quantity || 0),
        publicationAmount: Number(row.publicationAmount || 0),
        totalAmount: Number(row.totalAmount || 0)
      })),
    cashCosts: cashCosts.value.map(row => ({
      id: (row as any).id,
      cashName: row.cashName,
      quantity: Number(row.quantity || 0),
      amount: Number(row.amount || 0),
      totalAmount: Number(row.totalAmount || 0)
    })),
    participants: participants.value.map(p => ({
      id: (p as any).id,
      studentCode: p.studentCode,
      isStudent: p.isStudent,
      participantName: p.participantName,
      participantGender: p.participantGender,
      participantDateOfBirth: p.participantDateOfBirth,
      participantAddress: p.participantAddress,
      participantPhoneNumber: p.participantPhoneNumber,
      participantContact: p.participantContact,
      participantEmail: p.participantEmail,
      participantSchool: p.participantSchool,
      participantSourceKnown: p.participantSourceKnown,
      participantJob: p.participantJob,
      employeeId: p.employeeId
    })),
    attachments: packedAttachments
      .filter((a: any) => a?.path || a?.filePath || a?.relativePath)
      .map((a: any) => ({
        id: a.id,
        fileName: a.fileName || a.relativePath || a.filePath || a.path,
        filePath: a.filePath || a.path || a.relativePath,
        relativePath: a.relativePath || a.filePath || a.path,
        path: a.path || a.filePath || a.relativePath,
        size: a.size,
        contentType: a.contentType
      })),
    deletedAttachmentIds: removedAttachmentIds.value,
    deletedPublicationIds: removedPublicationIds.value,
    deletedCashIds: removedCashIds.value,
    deletedParticipantIds: removedParticipantIds.value,
    proposalQuantity: form.proposalQuantity
  }
  emit('submit', payload)
}

function getProposalsForDetail(detailId: string) {
  return (allProposals.value || []).filter(
    p =>
      getProposalKeys(p).includes(detailId) &&
      getProposalStatus(p) !== CompanyEventProposalStatus.Rejected
  )
}

async function updateUsage(detail: any) {
  if (!detail) {
    form.usedEvents = 0
    form.remainingEvents = 0
    form.proposalQuantity = 0
    form.usedBudget = companyBudgetUsage.value.get(branchCompanyKey.value) ?? 0
    existingProposals.value = []
    return
  }
  const detailId = getDetailKey(detail.detail)
  if (!detailId) return
  const proposals = getProposalsForDetail(detailId)
  console.log(proposals)

  existingProposals.value = proposals
  const usedQty = proposals.length
  const usedBudget = proposals.reduce((sum, p) => sum + Number(p.totalAmount || 0), 0)

  console.log('[Proposal] updateUsage:', {
    detailId,
    usedQty,
    usedBudget,
    companyBudgetUsed: companyBudgetUsage.value.get(branchCompanyKey.value)
  })

  form.usedEvents = usedQty
  form.usedBudget = companyBudgetUsage.value.get(branchCompanyKey.value) ?? usedBudget
  form.remainingEvents = Math.max((detail.detail.quantity || 0) - usedQty, 0)
  if (form.remainingEvents <= 0) {
    form.proposalQuantity = 0
  } else {
    if (form.proposalQuantity <= 0) form.proposalQuantity = 1
    if (form.proposalQuantity > form.remainingEvents) {
      form.proposalQuantity = form.remainingEvents
    }
  }
}

async function ensureData() {
  if (!itemStore.items.length) {
    await itemStore.fetchAllItems()
  }
  if (!eventStore.events.length) {
    await eventStore.fetchAllEvents()
  }
  if (!affiliatePartnerStore.affiliatePartners.length) {
    await affiliatePartnerStore.fetchAllAffiliatePartners()
  }
  if (!employeeStore.employees.length) {
    await employeeStore.fetchAllEmployees()
  }
}

async function ensureUsage() {
  usageLoading.value = true
  try {
    const res = await companyEventService.fetchAllProposals()
    allProposals.value = res.data || []
    usageByDetail.value = buildUsageMap(allProposals.value || [])
    companyBudgetUsage.value = buildCompanyBudgetMap(allProposals.value || [])
    updateUsage(selectedDetail.value)
  } catch (err) {
    console.error('Failed to load existing proposals', err)
  } finally {
    usageLoading.value = false
  }
}

function buildUsageMap(items: CompanyEventModel[]) {
  const map = new Map<string, { quantity: number; budget: number }>()
  items.forEach(p => {
    if (getProposalStatus(p) === CompanyEventProposalStatus.Rejected) return
    const keys = getProposalKeys(p)
    keys.forEach(k => {
      if (!k) return
      const entry = map.get(k) || { quantity: 0, budget: 0 }
      entry.quantity += 1
      entry.budget += Number(p.totalAmount || 0)
      map.set(k, entry)
    })
  })
  return map
}

function buildCompanyBudgetMap(items: CompanyEventModel[]) {
  const map = new Map<string, number>()
  items.forEach(p => {
    if (getProposalStatus(p) === CompanyEventProposalStatus.Rejected) return
    const companyKey = String(p.companyId ?? p.allocationDetailEvent?.companyId ?? '')
    if (!companyKey) return
    map.set(companyKey, (map.get(companyKey) || 0) + Number(p.totalAmount || 0))
  })
  return map
}

function getDetailKey(detail: AllocationDetailEventModel) {
  if (detail?.id) return String(detail.id)
  const eventId = detail?.eventId
  const companyId = detail?.companyId ?? props.branchRow?.companyId
  if (eventId != null && companyId != null) return `${String(companyId)}-${String(eventId)}`
  return ''
}

function getProposalDetailKey(proposal: CompanyEventModel) {
  if (proposal.allocationDetailEventId) return String(proposal.allocationDetailEventId)
  if (proposal.allocationDetailEvent?.id) return String(proposal.allocationDetailEvent.id)
  const eventId = proposal.allocationDetailEvent?.eventId
  const companyId = proposal.allocationDetailEvent?.companyId ?? proposal.companyId
  if (eventId != null && companyId != null) return `${String(companyId)}-${String(eventId)}`
  return ''
}

function getProposalKeys(proposal: CompanyEventModel) {
  const keys = new Set<string>()
  if (proposal.allocationDetailEventId) keys.add(String(proposal.allocationDetailEventId))
  if (proposal.allocationDetailEvent?.id) keys.add(String(proposal.allocationDetailEvent.id))
  const eventId = proposal.allocationDetailEvent?.eventId
  const companyId = proposal.allocationDetailEvent?.companyId ?? proposal.companyId
  if (eventId != null && companyId != null) keys.add(`${String(companyId)}-${String(eventId)}`)
  return Array.from(keys).filter(Boolean)
}

function getProposalStatus(proposal: CompanyEventModel) {
  return proposal.companyEventStatus ?? (proposal.status as CompanyEventProposalStatus | undefined)
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
  if (sampleDataApplied.value || !props.visible) return
  form.eventName = form.eventName || 'Sự kiện trải nghiệm STEM'
  form.eventDate = form.eventDate || dayjs().add(10, 'day').format('YYYY-MM-DD')
  form.proposalContent =
    form.proposalContent || 'Nội dung đề xuất demo để kiểm thử upload tệp tin.'
  form.ruleNote = form.ruleNote || 'Giới hạn 25 học sinh'
  removedAttachmentIds.value = []

  if (!stockCosts.value.length) {
    const firstItem = itemStore.items[0]
    const qty = 10
    const price = firstItem?.price ?? 50000
    stockCosts.value = [
      {
        companyEventId: '',
        itemId: firstItem?.id ?? '',
        itemName: firstItem?.itemName ?? 'Brochure giới thiệu',
        quantity: qty,
        maxQuantity: firstItem?.quantity ?? qty,
        publicationAmount: price,
        totalAmount: qty * price
      } as any
    ]
  }

  if (!cashCosts.value.length) {
    cashCosts.value = [
      {
        companyEventId: '',
        cashName: 'Chi phí thuê MC',
        quantity: 1,
        amount: 1500000,
        totalAmount: 1500000
      } as any
    ]
  }

  if (!participants.value.length) {
    participants.value = [
      {
        companyEventId: '',
        studentCode: 'SAMPLE001',
        isStudent: true,
        participantName: 'Nguyễn Minh Anh',
        participantGender: 'female',
        participantDateOfBirth: '2008-04-15',
        participantAddress: 'Hà Nội',
        participantPhoneNumber: '0912345678',
        participantContact: 'Phụ huynh: Anh Nam',
        participantEmail: 'minhanh@example.com',
        participantSchool: 'THCS Lê Lợi',
        participantSourceKnown: 'Fanpage',
        participantJob: 'Học sinh',
        employeeId: employeeStore.employees[0]?.id
          ? String(employeeStore.employees[0].id)
          : ''
      } as any,
      {
        companyEventId: '',
        studentCode: 'SAMPLE002',
        isStudent: true,
        participantName: 'Trần Quốc Khánh',
        participantGender: 'male',
        participantDateOfBirth: '2007-11-02',
        participantAddress: 'Hải Phòng',
        participantPhoneNumber: '0987654321',
        participantContact: 'Phụ huynh: Chị Hương',
        participantEmail: 'quockhanh@example.com',
        participantSchool: 'THPT Nguyễn Trãi',
        participantSourceKnown: 'Giới thiệu từ bạn',
        participantJob: 'Học sinh',
        employeeId: employeeStore.employees[1]?.id
          ? String(employeeStore.employees[1].id)
          : ''
      } as any
    ]
  }

  sampleDataApplied.value = true
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
    notificationStore.showToast('error', { key: 'common.systemError' })
  }
}

const PARTICIPANT_IMPORT_ALIASES: Record<string, string[]> = {
  studentCode: ['studentcode', 'mahocvien', 'masohocvien', 'studentid', 'mahs', 'student'],
  studentName: ['studentname', 'tenhocvien', 'tênhọcviên', 'fullname', 'name'],
  gender: ['gender', 'gioitinh', 'giớitính'],
  dateOfBirth: ['dateofbirth', 'ngaysinh', 'dob', 'birthdate'],
  address: ['address', 'diachi', 'địachi'],
  phone: ['phone', 'sdt', 'sđt', 'mobile'],
  contact: ['contact', 'contactname', 'tenlienhe', 'tênliênhệ'],
  email: ['email', 'thuemail'],
  school: ['school', 'truong', 'điạchi', 'department'],
  source: ['source', 'nguon', 'leadsource'],
  job: ['job', 'occupation', 'nghenghiep', 'career'],
  advisorCode: ['advisorcode', 'manhanvien', 'mãnhânviên', 'manv', 'employeecode', 'ma nv']
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

function openAttachmentPicker() {
  attachmentManagerRef.value?.openFilePicker?.()
}

function openApproval(status: CompanyEventProposalStatus) {
  approvalStatus.value = status
  approvalReason.value = ''
  approvalDialogVisible.value = true
}

async function submitApproval() {
  if (!props.proposal?.id || approvalStatus.value == null) return
  try {
    approving.value = true
    await companyEventService.approveProposal({
      companyEventId: props.proposal.id,
      approveStatus: approvalStatus.value,
      reason: approvalReason.value
    } as ApproveCompanyEventModel)
    notificationStore.showToast('success', {
      key:
        approvalStatus.value === CompanyEventProposalStatus.Approved
          ? 'allocationEvent.proposal.approveSuccess'
          : 'allocationEvent.proposal.rejectSuccess'
    })
    approvalDialogVisible.value = false
      ; (props.proposal as any).companyEventStatus = approvalStatus.value
  } catch (err) {
    console.error('Approve/reject failed', err)
    notificationStore.showToast('error', { key: 'common.systemError' })
  } finally {
    approving.value = false
  }
}
</script>

<style scoped>
.proposal-form :deep(.el-form-item) {
  margin-bottom: 12px;
}

.company-proposal-dialog.metronic-dialog {
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
