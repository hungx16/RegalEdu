<template>
  <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
    :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete" width="70%"
    @update:visible="emit('update:visible', $event)" @close="closeModal">
    <template #form>
      <el-row :gutter="20">
        <!-- Bật song ngữ -->
        <el-col :span="24">
          <el-form-item>
            <el-checkbox v-model="formData.isMultilingual" :disabled="isView">
              {{ t('common.allowMultilingual') }}
            </el-checkbox>
          </el-form-item>
        </el-col>
        <!-- Loại đối tác -->
        <el-col :span="12">
          <label class="required">{{ t('affiliatePartner.partnerType') }}</label>
          <el-form-item prop="partnerTypeId">
            <el-select v-model="formData.partnerTypeId" :disabled="isView" filterable clearable
              :placeholder="t('affiliatePartner.placeholders.partnerType')">
              <el-option v-for="pt in partnerTypeOptions" :key="pt.id" :label="pt.partnerTypeName" :value="pt.id" />
            </el-select>
          </el-form-item>
        </el-col>

        <!-- Mã đối tác -->
        <el-col :span="12">
          <label class="required">{{ t('affiliatePartner.code') }}</label>
          <el-form-item prop="partnerCode">
            <el-input v-model="formData.partnerCode" :disabled="isView" maxlength="50" show-word-limit
              :placeholder="t('affiliatePartner.placeholders.partnerCode')" />
          </el-form-item>
        </el-col>

        <!-- Tên đối tác -->
        <el-col :span="12">
          <label class="required">{{ t('affiliatePartner.name') }}</label>
          <el-form-item prop="partnerName">
            <el-input v-model="formData.partnerName" :disabled="isView" maxlength="200" show-word-limit
              :placeholder="t('affiliatePartner.placeholders.partnerName')" />
          </el-form-item>
        </el-col>

        <!-- Người liên hệ -->
        <el-col :span="12">
          <label class="required">{{ t('affiliatePartner.contactPerson') }}</label>
          <el-form-item prop="contactPerson">
            <el-input v-model="formData.contactPerson" :disabled="isView" maxlength="100" show-word-limit
              :placeholder="t('affiliatePartner.placeholders.contactPerson')" />
          </el-form-item>
        </el-col>
        <!-- English Partner Name -->
        <el-col :span="24" v-if="formData.isMultilingual">
          <label class="required fs-6 fw-semibold mb-2 d-block">English Partner Name</label>
          <el-form-item prop="enPartnerName">
            <el-input v-model="formData.enPartnerName" :disabled="isView" :maxlength="200" show-word-limit />
          </el-form-item>
        </el-col>
        <!-- Điện thoại -->
        <el-col :span="12">
          <label class="required">{{ t('affiliatePartner.phone') }}</label>
          <el-form-item prop="phone">
            <el-input v-model="formData.phone" :disabled="isView"
              :placeholder="t('affiliatePartner.placeholders.phone')" />
          </el-form-item>
        </el-col>

        <!-- Chức vụ -->
        <el-col :span="12">
          <label class="required">{{ t('affiliatePartner.position') }}</label>
          <el-form-item prop="position">
            <el-input v-model="formData.position" :disabled="isView" maxlength="100" show-word-limit
              :placeholder="t('affiliatePartner.placeholders.position')" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <label class="required">{{ t('affiliatePartner.province') }}</label>
          <el-form-item prop="province">
            <!-- Province -->
            <el-select v-model="formData.province" :disabled="isView" filterable clearable
              :placeholder="t('affiliatePartner.placeholders.province')">
              <el-option v-for="item in commonStore.provinces" :key="item.provinceCode || item"
                :label="item.provinceName || item" :value="item.provinceCode || item" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <label>{{ t('affiliatePartner.schoolLevel') }}</label>
          <el-form-item prop="schoolLevel">
            <el-select v-model="formData.schoolLevel" :disabled="isView" clearable
              :placeholder="t('affiliatePartner.placeholders.schoolLevel')">
              <el-option v-for="level in schoolLevelOptions" :key="level.value" :label="level.label"
                :value="level.value" />
            </el-select>
          </el-form-item>
        </el-col>
        <!-- Vị trí đại lý/phạm vi -->
        <el-col :span="12">
          <label>{{ t('affiliatePartner.agencyLocation') }}</label>
          <el-form-item prop="agencyLocation">
            <el-input v-model="formData.agencyLocation" :disabled="isView" maxlength="200" show-word-limit
              :placeholder="t('affiliatePartner.placeholders.agencyLocation')" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <label>{{ t('affiliatePartner.websiteKeys') }}</label>
          <el-form-item prop="websiteKeys">
            <TagList v-model="websiteKeysModel" :maxVisible="8" :maxTags="20" :distinct-colors="true" :autoColor="true"
              :dismissible="!isView" :placeholder="t('affiliatePartner.placeholders.websiteKeys')" />
          </el-form-item>
        </el-col>
        <el-col v-if="formData.isMultilingual" :span="12">
          <label>{{ t('affiliatePartner.enWebsiteKeys') }}</label>
          <el-form-item prop="enWebsiteKeys">
            <TagList v-model="enWebsiteKeysModel" :maxVisible="8" :maxTags="20" :distinct-colors="true"
              :autoColor="true" :dismissible="!isView"
              :placeholder="t('affiliatePartner.placeholders.enWebsiteKeys') || 'Enter English website keys'" />
          </el-form-item>
        </el-col>

        <!-- Địa chỉ -->
        <el-col :span="24">
          <label>{{ t('affiliatePartner.address') }}</label>
          <el-form-item prop="address">
            <el-input type="textarea" :rows="2" v-model="formData.address" :disabled="isView" maxlength="300"
              show-word-limit :placeholder="t('affiliatePartner.placeholders.address')" />
          </el-form-item>
        </el-col>

        <el-col :span="24">
          <div class="financial-section">
            <div class="financial-header">
              <span class="section-title">{{ t('affiliatePartner.financialInfo') }}</span>
              <el-checkbox v-model="formData.isFinancialCompany" :disabled="isView">
                {{ t('affiliatePartner.isFinancialCompany') }}
              </el-checkbox>
            </div>
            <el-row v-if="formData.isFinancialCompany" :gutter="16">
              <el-col :span="12">
                <label>{{ t('affiliatePartner.interestRate') }}</label>
                <el-form-item prop="interestRate">
                  <el-input-number v-model="formData.interestRate" :disabled="isView" :min="0" :step="0.1"
                    :precision="2" controls-position="right" class="w-100" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <label>{{ t('affiliatePartner.loanTerm') }}</label>
                <el-form-item prop="loanTerm">
                  <el-input-number v-model="formData.loanTerm" :disabled="isView" :min="0" :step="1"
                    controls-position="right" class="w-100" />
                </el-form-item>
              </el-col>
              <el-col :span="24">
                <label>{{ t('affiliatePartner.loanBenefits') }}</label>
                <el-form-item prop="loanBenefits">
                  <TagList v-model="loanBenefitsModel" :maxVisible="10" :maxTags="20" :distinct-colors="true"
                    :autoColor="true" :dismissible="!isView"
                    :placeholder="t('affiliatePartner.placeholders.loanBenefits')" />
                </el-form-item>
              </el-col>
              <el-col v-if="formData.isMultilingual" :span="24">
                <label>{{ t('affiliatePartner.enLoanBenefits') }}</label>
                <el-form-item prop="enLoanBenefits">
                  <TagList v-model="enLoanBenefitsModel" :maxVisible="10" :maxTags="20" :distinct-colors="true"
                    :autoColor="true" :dismissible="!isView"
                    :placeholder="t('affiliatePartner.placeholders.enLoanBenefits') || 'Enter English loan benefits'" />
                </el-form-item>
              </el-col>
            </el-row>
          </div>
        </el-col>

        <!-- Đăng web -->
        <el-col :span="24">
          <el-form-item prop="isPublish">
            <el-checkbox v-model="formData.isPublish" :disabled="isView">
              {{ t('affiliatePartner.isPublish') }}
            </el-checkbox>
          </el-form-item>
        </el-col>

        <!-- Ảnh đại diện -->
        <el-col :span="24">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('supportingDocument.avatar') }}</label>
          <el-form-item prop="image">
            <FileUpload :key="imageUploadKey" ref="representativeImageRef" :file-url="formData.image?.path"
              :original-file-name="formData.image?.fileName" :allowed-groups="['image']" :disabled="isView"
              @file-change="handleAvatarChange" @change="onImgUploadChange" />
          </el-form-item>
        </el-col>

        <el-col v-if="showReportPublicationsSection" :span="24">
          <div class="report-publications">
            <div class="report-publications-header">
              <div>
                <h5 class="fw-semibold mb-1">{{ t('affiliatePartner.reportPublicationsTitle') }}</h5>
                <div class="text-muted fs-8">{{ t('affiliatePartner.reportPublicationsDesc') }}</div>
              </div>
            </div>
            <div class="report-publications-filters">
              <el-row :gutter="12">
                <el-col :span="6">
                  <label class="fs-8 fw-semibold">{{ t('allocationEvent.report.reportCode') }}</label>
                  <el-input v-model="reportFilters.reportCode" clearable
                    :placeholder="t('affiliatePartner.placeholders.reportCode')" />
                </el-col>
                <el-col :span="6">
                  <label class="fs-8 fw-semibold">{{ t('allocationEvent.report.eventName') }}</label>
                  <el-input v-model="reportFilters.eventName" clearable
                    :placeholder="t('affiliatePartner.placeholders.eventName')" />
                </el-col>
                <el-col :span="6">
                  <label class="fs-8 fw-semibold">{{ t('allocationEvent.proposal.stockProduct') }}</label>
                  <el-input v-model="reportFilters.itemName" clearable
                    :placeholder="t('affiliatePartner.placeholders.itemName')" />
                </el-col>
                <el-col :span="6">
                  <label class="fs-8 fw-semibold">{{ t('allocationEvent.report.eventDate') }}</label>
                  <el-date-picker v-model="reportFilters.eventDateRange" type="daterange" unlink-panels
                    range-separator="-" value-format="YYYY-MM-DD" class="w-100"
                    :start-placeholder="t('affiliatePartner.placeholders.eventDateFrom')"
                    :end-placeholder="t('affiliatePartner.placeholders.eventDateTo')" />
                </el-col>
              </el-row>
              <div class="report-publications-actions">
                <el-button size="small" @click="resetReportFilters">{{ t('common.clearFilters') }}</el-button>
              </div>
            </div>
            <BaseTable :columns="reportPublicationColumns" :items="reportPublicationRowsFiltered"
              :loading="reportPublicationsLoading" :showCheckboxColumn="false" :showActionsColumn="false" :height="260">
              <template #cell-rowNumber="{ item }">
                {{ item.rowNumber }}
              </template>
              <template #cell-eventDate="{ item }">
                {{ formatDate(item.eventDate) }}
              </template>
              <template #cell-publicationAmount="{ item }">
                {{ formatCurrency(item.publicationAmount) }}
              </template>
              <template #cell-totalAmount="{ item }">
                {{ formatCurrency(item.totalAmount) }}
              </template>
            </BaseTable>
            <div v-if="!reportPublicationsLoading && !reportPublicationRowsFiltered.length"
              class="text-muted fs-8 text-center py-3">
              {{ t('affiliatePartner.reportPublicationsEmpty') }}
            </div>

            <div class="report-publications-summary">
              <div class="report-publications-header">
                <div>
                  <h5 class="fw-semibold mb-1">{{ t('affiliatePartner.reportPublicationsSummaryTitle') }}</h5>
                  <div class="text-muted fs-8">{{ t('affiliatePartner.reportPublicationsSummaryDesc') }}</div>
                </div>
              </div>
              <BaseTable :columns="reportPublicationSummaryColumns" :items="reportPublicationSummaryRows"
                :loading="reportPublicationsLoading" :showCheckboxColumn="false" :showActionsColumn="false"
                :height="220">
                <template #cell-rowNumber="{ item }">
                  {{ item.rowNumber }}
                </template>
                <template #cell-totalAmount="{ item }">
                  {{ formatCurrency(item.totalAmount) }}
                </template>
              </BaseTable>
              <div v-if="!reportPublicationsLoading && !reportPublicationSummaryRows.length"
                class="text-muted fs-8 text-center py-3">
                {{ t('affiliatePartner.reportPublicationsSummaryEmpty') }}
              </div>
            </div>
          </div>
        </el-col>
      </el-row>
    </template>
  </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, ref, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import { useNotificationStore } from '@/stores/notificationStore'
import type { AffiliatePartnerModel } from '@/api/AffiliatePartnerApi'
import { usePartnerTypeStore } from '@/stores/partnerTypeStore'
import { useCommonStore } from '@/stores/commonStore'
import { useAffiliatePartnerStore } from '@/stores/affiliatePartnerStore'
import FileUpload from '@/components/file-upload/FileUpload.vue'
import TagList from '@/components/tag/TagList.vue'
import { SchoolLevelType, SchoolLevelTypeLabels, StatusType } from '@/types'
import { formatCurrency, formatDate } from '@/utils/format'
const commonStore = useCommonStore()
const imageUploadKey = ref(0)

const props = defineProps<{
  visible: boolean,
  mode?: 'create' | 'edit' | 'view',
  loading: boolean,
  affiliatePartnerData: Partial<AffiliatePartnerModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const isView = computed(() => props.mode === 'view')

const partnerTypeStore = usePartnerTypeStore()
const partnerTypeOptions = computed(() => partnerTypeStore.partnerTypes)
const affiliatePartnerStore = useAffiliatePartnerStore()
const schoolLevelOptions = computed(() =>
  (Object.values(SchoolLevelType).filter((v) => typeof v === 'number') as number[])
    .map(level => ({
      value: level as SchoolLevelType,
      label: t(SchoolLevelTypeLabels[level as SchoolLevelType])
    }))
)

const formData = ref<Partial<AffiliatePartnerModel>>({
  partnerTypeId: '',
  partnerCode: '',
  partnerName: '',
  contactPerson: '',
  phone: '',
  province: '',
  address: '',
  position: '',
  agencyLocation: '',
  isPublish: false,
  image: { path: '', fileName: '' },
  isMultilingual: false,
  status: StatusType.Active,
  sortOrder: 0,
  isFinancialCompany: false,
  schoolLevel: undefined,
  websiteKeys: '',
  interestRate: 0,
  loanTerm: 0,
  loanBenefits: '',
  enLoanBenefits: '',
  enWebsiteKeys: '',
})
const websiteKeysModel = computed<string>({
  get: () => formData.value.websiteKeys ?? '',
  set: (v: string) => { formData.value.websiteKeys = v }
})
const loanBenefitsModel = computed<string>({
  get: () => formData.value.loanBenefits ?? '',
  set: (v: string) => { formData.value.loanBenefits = v }
})
const enWebsiteKeysModel = computed<string>({
  get: () => formData.value.enWebsiteKeys ?? '',
  set: (v: string) => { formData.value.enWebsiteKeys = v }
})
const enLoanBenefitsModel = computed<string>({
  get: () => formData.value.enLoanBenefits ?? '',
  set: (v: string) => { formData.value.enLoanBenefits = v }
})
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')
const showReportPublications = computed(() => !isCreate.value && !!formData.value.id)

type ReportPublicationRow = {
  rowNumber: number
  reportCode: string
  eventName: string
  eventDate: string
  itemCode: string
  itemName: string
  quantity: number
  publicationAmount: number
  totalAmount: number
}

type ReportPublicationSummaryRow = {
  rowNumber: number
  itemCode: string
  itemName: string
  totalQuantity: number
  totalAmount: number
}

const reportPublicationsLoading = computed(() => affiliatePartnerStore.reportPublicationsLoading)
const reportPublicationRows = computed<ReportPublicationRow[]>(() =>
  (affiliatePartnerStore.reportPublications ?? []).map((item, index) => ({
    rowNumber: index + 1,
    reportCode: item.companyEventReportCode ?? '-',
    eventName: item.companyEventName ?? '-',
    eventDate: item.eventDate ?? '',
    itemCode: item.itemCode ?? '-',
    itemName: item.itemName ?? '-',
    quantity: Number(item.quantity || 0),
    publicationAmount: Number(item.publicationAmount || 0),
    totalAmount: Number(item.totalAmount || 0),
  }))
)
const showReportPublicationsSection = computed(() => {
  if (!showReportPublications.value) return false
  if (reportPublicationsLoading.value) return true
  return reportPublicationRows.value.length > 0
})

const reportFilters = ref({
  reportCode: '',
  eventName: '',
  itemName: '',
  eventDateRange: [] as string[],
})

const reportPublicationRowsFiltered = computed<ReportPublicationRow[]>(() => {
  const rows = reportPublicationRows.value
  const reportCode = String(reportFilters.value.reportCode ?? '').trim().toLowerCase()
  const eventName = String(reportFilters.value.eventName ?? '').trim().toLowerCase()
  const itemName = String(reportFilters.value.itemName ?? '').trim().toLowerCase()
  const range = reportFilters.value.eventDateRange ?? []
  const startDate = range.length > 0 ? parseFilterDate(range[0]) : null
  const endDate = range.length > 1 ? parseFilterDate(range[1]) : null

  if (!reportCode && !eventName && !itemName && !startDate && !endDate) return rows

  return rows.filter((row) => {
    if (reportCode && !String(row.reportCode ?? '').toLowerCase().includes(reportCode)) return false
    if (eventName && !String(row.eventName ?? '').toLowerCase().includes(eventName)) return false
    if (
      itemName &&
      !String(row.itemName ?? '').toLowerCase().includes(itemName) &&
      !String(row.itemCode ?? '').toLowerCase().includes(itemName)
    ) {
      return false
    }
    if (startDate || endDate) {
      const rowDate = parseFilterDate(row.eventDate)
      if (!rowDate) return false
      if (startDate && rowDate < startDate) return false
      if (endDate && rowDate > endDate) return false
    }
    return true
  })
})

const reportPublicationSummaryRows = computed<ReportPublicationSummaryRow[]>(() => {
  const map = new Map<string, ReportPublicationSummaryRow>()
  reportPublicationRowsFiltered.value.forEach((row) => {
    const key = `${row.itemCode ?? ''}|${row.itemName ?? ''}`
    const existing = map.get(key) ?? {
      rowNumber: 0,
      itemCode: row.itemCode ?? '-',
      itemName: row.itemName ?? '-',
      totalQuantity: 0,
      totalAmount: 0,
    }
    existing.totalQuantity += Number(row.quantity || 0)
    existing.totalAmount += Number(row.totalAmount || 0)
    map.set(key, existing)
  })
  return Array.from(map.values()).map((row, index) => ({
    ...row,
    rowNumber: index + 1,
  }))
})

const reportPublicationColumns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center' },
  { key: 'reportCode', labelKey: 'allocationEvent.report.reportCode', width: 150 },
  { key: 'eventName', labelKey: 'allocationEvent.report.eventName', minWidth: 200 },
  { key: 'eventDate', labelKey: 'allocationEvent.report.eventDate', width: 140, align: 'center' },
  { key: 'itemCode', labelKey: 'item.code', width: 140 },
  { key: 'itemName', labelKey: 'allocationEvent.proposal.stockProduct', minWidth: 200 },
  { key: 'quantity', labelKey: 'allocationEvent.proposal.quantity', width: 110, align: 'center' },
  { key: 'publicationAmount', labelKey: 'allocationEvent.proposal.unitAmount', width: 140, align: 'right' },
  { key: 'totalAmount', labelKey: 'allocationEvent.proposal.totalAmount', width: 150, align: 'right' }
]

const reportPublicationSummaryColumns: BaseTableColumn[] = [
  { key: 'rowNumber', labelKey: 'common.index', width: 70, align: 'center' },
  { key: 'itemCode', labelKey: 'item.code', width: 160 },
  { key: 'itemName', labelKey: 'item.name', minWidth: 220 },
  { key: 'totalQuantity', labelKey: 'allocationEvent.proposal.quantity', width: 120, align: 'center' },
  { key: 'totalAmount', labelKey: 'allocationEvent.proposal.totalAmount', width: 150, align: 'right' }
]

function parseFilterDate(value?: string | null) {
  if (!value) return null
  const normalized = value.length >= 10 ? value.slice(0, 10) : value
  const parsed = Date.parse(normalized)
  return Number.isNaN(parsed) ? null : parsed
}

function resetReportFilters() {
  reportFilters.value = {
    reportCode: '',
    eventName: '',
    itemName: '',
    eventDateRange: [],
  }
}
async function loadReportPublications(partnerId: string) {
  if (!partnerId) {
    affiliatePartnerStore.clearReportPublications()
    return
  }
  await affiliatePartnerStore.fetchReportPublications(partnerId)
}
watch(
  () => props.affiliatePartnerData,
  (data) => {
    if (data) {
      formData.value = {
        id: data.id ?? '',
        partnerTypeId: data.partnerTypeId ?? '',
        partnerCode: data.partnerCode ?? '',
        partnerName: data.partnerName ?? '',
        agencyLocation: data.agencyLocation ?? '',
        province: data.province ?? '',
        address: data.address ?? '',
        contactPerson: data.contactPerson ?? '',
        phone: data.phone ?? '',
        position: data.position ?? '',
        isPublish: data.isPublish ?? false,
        image: data.image ?? { path: '', fileName: '' },
        createdAt: data.createdAt ?? '',
        createdBy: data.createdBy ?? '',
        status: data.status ?? StatusType.Active,
        sortOrder: data.sortOrder ?? 0,
        isMultilingual: data.isMultilingual ?? false,
        enPartnerName: data.enPartnerName ?? '',
        isFinancialCompany: data.isFinancialCompany ?? false,
        schoolLevel: data.schoolLevel ?? undefined,
        websiteKeys: data.websiteKeys ?? '',
        interestRate: data.interestRate ?? 0,
        loanTerm: data.loanTerm ?? 0,
        loanBenefits: data.loanBenefits ?? '',
        enLoanBenefits: data.enLoanBenefits ?? '',
        enWebsiteKeys: data.enWebsiteKeys ?? '',
      }
    } else {
      formData.value = {
        partnerTypeId: '',
        partnerCode: '',
        partnerName: '',
        agencyLocation: '',
        province: '',
        address: '',
        contactPerson: '',
        phone: '',
        position: '',
        isPublish: false,
        image: { path: '', fileName: '' },
        status: StatusType.Active,
        sortOrder: 0,
        isMultilingual: false,
        isFinancialCompany: false,
        schoolLevel: undefined,
        websiteKeys: '',
        interestRate: 0,
        loanTerm: 0,
        loanBenefits: '',
        enLoanBenefits: '',
        enWebsiteKeys: '',
      }
    }
    imageUploadKey.value++
  },
  { immediate: true }
)
watch(
  [() => props.visible, () => formData.value.id],
  ([visible, partnerId], [prevVisible, prevPartnerId]) => {
    if (!visible) {
      affiliatePartnerStore.clearReportPublications()
      resetReportFilters()
      return
    }
    if (!partnerId) {
      affiliatePartnerStore.clearReportPublications()
      resetReportFilters()
      return
    }
    if (!prevVisible || partnerId !== prevPartnerId) {
      resetReportFilters()
    }
    void loadReportPublications(String(partnerId))
  }
)
const representativeImageRef = ref<InstanceType<typeof FileUpload> | null>(null)


const modeTitle = computed(() => {
  if (isView.value) return t('affiliatePartner.detailTitle')
  if (isEdit.value) return t('affiliatePartner.editTitle')
  if (isCreate.value) {
    formData.value.partnerCode = commonStore.code ?? ''
  }
  return t('affiliatePartner.addTitle')
})
const validateImageFile = async () => {
  const ref = representativeImageRef.value
  const hasPending = !!ref?.getPendingFile?.()
  const cur = ref?.getCurrent?.()
  const hasExisting = !!cur?.fileUrl && !cur?.markedForDelete

  if (!hasPending && !hasExisting) {
    return Promise.reject(new Error(t('validation.requiredFile')))
  }
  return Promise.resolve()
}
// validate theo max length của model + bắt buộc
const rules = {
  partnerTypeId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
  partnerCode: [
    { required: true, message: t('validation.required'), trigger: 'blur' },
    { min: 1, max: 50, message: t('validation.maxLength', { max: 50 }), trigger: 'blur' }
  ],
  partnerName: [
    { required: true, message: t('validation.required'), trigger: 'blur' },
    { min: 1, max: 200, message: t('validation.maxLength', { max: 200 }), trigger: 'blur' }
  ],
  contactPerson: [
    { required: true, message: t('validation.required'), trigger: 'blur' },
    { min: 1, max: 100, message: t('validation.maxLength', { max: 100 }), trigger: 'blur' }
  ],
  phone: [
    { required: true, message: t('validation.required'), trigger: 'blur' },
    {
      pattern: /^(0|\+84)[3-9][0-9]{8}$/,
      message: t('validation.invalidPhone'),
      trigger: ['blur', 'change']
    }
  ],
  position: [
    { required: true, message: t('validation.required'), trigger: 'blur' },
    { min: 1, max: 100, message: t('validation.maxLength', { max: 100 }), trigger: 'blur' }
  ],
  province: [
    { required: true, message: t('validation.required'), trigger: 'blur' },
    { min: 1, max: 100, message: t('validation.maxLength', { max: 100 }), trigger: 'blur' }
  ],
  agencyLocation: [{ max: 200, message: t('validation.maxLength', { max: 200 }), trigger: 'blur' }],
  address: [{ max: 300, message: t('validation.maxLength', { max: 300 }), trigger: 'blur' }],
  imageUrl: [{ validator: validateImageFile, trigger: 'change' }],
  enPartnerName: [
    { required: true, message: t('validation.required'), trigger: 'blur' }
  ]
}


function handleAvatarChange(_file: File | null) { /* ... */ }

function onImgUploadChange() {
  baseDialogRef.value?.formRef?.validateField('imageUrl')
}
const baseDialogRef = ref()
const formRef = ref()

function closeModal() {
  emit('update:visible', false)
  emit('close')
}

async function onSubmit() {
  baseDialogRef.value?.formRef.validate(async (valid: boolean) => {
    if (valid) {
      type UploadRef = {
        uploadPending?: () => Promise<{ relativePath: string; fileName: string } | null>
        uploadPendingToTemp?: () => Promise<{ relativePath: string; fileName: string } | null>
        getCurrent?: () => { fileUrl: string | null; originalFileName: string | null; markedForDelete: boolean }
        getOriginalName?: () => string | null
        getUploadedOriginalName?: () => string | null
      }
      const imgRef = representativeImageRef.value as UploadRef | undefined

      // Ưu tiên upload file vào thư mục tạm, nếu không có pending thì imgUp = null
      const imgUp = await (imgRef?.uploadPending?.() ?? imgRef?.uploadPendingToTemp?.() ?? Promise.resolve(null))
      const imgState = imgRef?.getCurrent?.()
      const payload: any = { ...formData.value }
      // ensure payload.isPublish is a boolean before sending to API
      payload.isPublish = payload.isPublish === true || payload.isPublish === 'true' || payload.isPublish === '1' || payload.isPublish === 1
      // normalize optional numeric fields for backend (Int32)
      const toNumberOrNull = (val: any) => {
        if (val === null || val === undefined || val === '') return null
        const num = Number(val)
        return Number.isFinite(num) ? num : null
      }
      payload.interestRate = toNumberOrNull(payload.interestRate)
      payload.loanTerm = toNumberOrNull(payload.loanTerm)
      if (!payload.isFinancialCompany) {
        payload.interestRate = 0
        payload.loanTerm = 0
        payload.loanBenefits = ''
        payload.enLoanBenefits = ''
      }
      if (props.mode === 'create') {
        if (imgUp) {
          payload.image = {
            path: imgUp.relativePath,
            fileName: imgRef?.getOriginalName?.() ?? imgUp.fileName,
          }
        } else {
          delete payload.image
        }
      } else if (props.mode === 'edit') {
        if (imgUp) {
          // ảnh mới upload
          payload.image = {
            path: imgUp.relativePath,
            fileName: imgRef?.getOriginalName?.() ?? imgUp.fileName,
          }
        } else if (imgState?.markedForDelete && payload.image) {
          // xóa ảnh
          payload.image.path = ''
        }
      }
      emit('submit', payload)
    } else {
      useNotificationStore().showToast('error', { key: 'validation.formInvalid' })
    }
  })
}

function onDelete() {
  emit('delete', formData.value)
}

onMounted(async () => {
  // đảm bảo có data cho dropdown
  if (!partnerTypeStore.partnerTypes?.length) {
    await partnerTypeStore.fetchAllPartnerTypes()
  }
  if (!commonStore.provinces.length) await commonStore.fetchProvinces()

})
</script>

<style scoped>
.financial-section {
  padding: 12px 16px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #f8fafc;
  margin-top: 8px;
}

.financial-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 12px;
  gap: 12px;
}

.section-title {
  font-weight: 600;
  color: #0d6efd;
}

.report-publications {
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 12px 16px;
  background: #f8fafc;
  margin-top: 12px;
}

.report-publications-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 12px;
  gap: 12px;
}

.report-publications-filters {
  margin-bottom: 12px;
}

.report-publications-actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 8px;
}

.report-publications-summary {
  margin-top: 16px;
}
</style>
