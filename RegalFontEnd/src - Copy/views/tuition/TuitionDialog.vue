<template>
  <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
    :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
    @update:visible="emit('update:visible', $event)" @close="closeModal">
    <template #form>
      <el-row :gutter="20">
        <!-- Tuition Code -->
        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('tuition.tuitionCode') }}</label>
          <el-form-item prop="tuitionCode">
            <el-input v-model="formData.tuitionCode" :disabled="isView" maxlength="10" show-word-limit clearable />
          </el-form-item>
        </el-col>

        <!-- Tuition Name -->
        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('tuition.tuitionName') }}</label>
          <el-form-item prop="tuitionName">
            <el-input v-model="formData.tuitionName" :disabled="isView" maxlength="100" show-word-limit clearable />
          </el-form-item>
        </el-col>

        <!-- Course -->
        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('tuition.course') }}</label>
          <el-form-item prop="courseId">
            <el-select v-model="formData.courseId" filterable :disabled="isView" style="width: 100%;">
              <el-option v-for="c in courseOptions" :key="c.value" :label="c.label" :value="c.value" />
            </el-select>
          </el-form-item>
        </el-col>

        <!-- Class Type -->
        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('tuition.classType') }}</label>
          <el-form-item prop="classTypeId">
            <el-select v-model="formData.classTypeId" filterable :disabled="isView" style="width: 100%;">
              <el-option v-for="ct in classTypeOptions" :key="ct.value" :label="ct.label" :value="ct.value" />
            </el-select>
          </el-form-item>
        </el-col>
        <!-- Unit -->
        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('tuition.unit') }}</label>
          <el-form-item prop="unit">
            <el-select v-model="formData.unit" :disabled="isView" style="width: 100%;">
              <el-option :label="t('unit.hour')" :value="UnitType.Hour" />
              <el-option :label="t('unit.session')" :value="UnitType.Session" />
              <el-option :label="t('unit.month')" :value="UnitType.Month" />
              <el-option :label="t('unit.course')" :value="UnitType.Course" />
            </el-select>
          </el-form-item>
        </el-col>

        <!-- Tuition Fee -->
        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('tuition.tuitionFee') }}</label>
          <el-form-item prop="tuitionFee">
            <CurrencyInput v-model="formData.tuitionFee" :disabled="isView" locale="vi-VN" currency="VND" />

          </el-form-item>
        </el-col>
        <!-- Duration Hours -->
        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('tuition.durationHours') }}</label>
          <el-form-item prop="durationHours">
            <el-input-number v-model="formData.durationHours" :disabled="isView" :min="1" :max="1000" />
          </el-form-item>
        </el-col>

        <!-- Min Hours -->
        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('tuition.minHours') }}</label>
          <el-form-item prop="minHours">
            <el-input-number v-model="formData.minHours" :disabled="isView" :min="1"
              :max="formData.durationHours || 1" />
          </el-form-item>
        </el-col>

        <!-- Total Months -->
        <el-col :span="10">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('tuition.totalMonths') }}</label>
          <el-form-item prop="totalMonths">
            <el-input-number v-model="formData.totalMonths" :disabled="true" :min="0" :max="100" :precision="2" />
          </el-form-item>
        </el-col>



        <!-- Status -->
        <el-col :span="14" :xs="24">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
          <el-form-item prop="status">
            <el-radio-group v-model="formData.status" :disabled="isView">
              <el-radio :value="0">{{ t('common.active') }}</el-radio>
              <el-radio :value="1">{{ t('common.inactive') }}</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <!-- Start Date -->
        <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('tuition.startDate') }}</label>
          <el-form-item prop="startDate">
            <el-date-picker v-model="formData.startDate" :disabled="isView" type="date" value-format="YYYY-MM-DD"
              format="DD/MM/YYYY" clearable style="width: 100%;" :disabled-date="disableStartDates" />
          </el-form-item>
        </el-col>

        <!-- End Date -->
        <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('tuition.endDate') }}</label>
          <el-form-item prop="endDate">
            <el-date-picker v-model="formData.endDate" :disabled="isView" type="date" value-format="YYYY-MM-DD"
              format="DD/MM/YYYY" clearable style="width: 100%;" :disabled-date="disableEndDates" />
          </el-form-item>
        </el-col>

        <!-- View-only audit fields -->
        <el-col :span="12" v-if="isView">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdBy') }}</label>
          <el-form-item>
            <el-input :value="formData.createdBy || ''" disabled />
          </el-form-item>
        </el-col>
        <el-col :span="12" v-if="isView">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdAt') }}</label>
          <el-form-item>
            <el-input :value="formatDate(formData.createdAt || '', 'YYYY-MM-DD HH:mm:ss')" disabled />
          </el-form-item>
        </el-col>
      </el-row>
    </template>
  </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, ref, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import { useCourseStore } from '@/stores/courseStore'
import { useClassTypeStore } from '@/stores/classTypeStore'
import { UnitType, StatusType } from '@/types'
import type { TuitionModel } from '@/api/TuitionApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { formatDate } from '@/utils/format';

import CurrencyInput from '@/components/currency-input/CurrencyInput.vue'
const props = defineProps<{
  visible: boolean
  mode?: 'create' | 'edit' | 'view'
  loading: boolean
  tuitionData: Partial<TuitionModel> | null
}>()

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const notificationStore = useNotificationStore()

const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
  if (isView.value) return t('tuition.detailTitle')
  if (isEdit.value) return t('tuition.editTitle')
  if (isCreate.value) return t('tuition.addTitle')
  return ''
})

const formRef = ref()
const baseDialogRef = ref()

const defaults: TuitionModel = {
  // id: '',
  tuitionCode: '',
  tuitionName: '',
  courseId: '',
  classTypeId: '',
  durationHours: 1,
  minHours: 1,
  totalMonths: 0,
  unit: UnitType.Hour,
  tuitionFee: 0,
  status: StatusType.Active,
  startDate: null, // NEW
  endDate: null,   // NEW
}

const formData = ref<TuitionModel>({ ...defaults })

const courseStore = useCourseStore()
const classTypeStore = useClassTypeStore()

const selectedClassType = computed(() =>
  (classTypeStore.classTypes ?? []).find(ct => ct.id === formData.value.classTypeId) || null
)

const courseOptions = computed(() =>
  (courseStore.courses ?? []).map((x: any) => ({
    value: x.id,
    label: `${x.code ?? ''} - ${x.courseName ?? x.name ?? ''}`.replace(/^ - /, '')
  }))
)

const classTypeOptions = computed(() =>
  (classTypeStore.classTypes ?? []).map((x: any) => ({
    value: x.id,
    label: `${x.classTypeCode ?? ''} - ${x.classTypeName ?? ''}`.replace(/^ - /, '')
  }))
)

watch(
  () => props.tuitionData,
  (data) => {
    if (data) {
      formData.value = {
        ...defaults,
        ...data,
        durationHours: toNumber(data.durationHours, 1),
        minHours: toNumber(data.minHours, 1),
        totalMonths: toNumber(data.totalMonths, 0),
        tuitionFee: toNumber(data.tuitionFee, 0),
        status: toNumber(data.status, StatusType.Active),
        unit: toNumber(data.unit, UnitType.Hour),
        startDate: (data.startDate as string | null) ?? null,
        endDate: (data.endDate as string | null) ?? null,
      } as TuitionModel
    } else {
      formData.value = { ...defaults }
    }
  },
  { immediate: true }
)

function toNumber(val: any, fallback = 0) {
  const n = Number(val)
  return Number.isFinite(n) ? n : fallback
}
function disableStartDates(date: Date) {
  // Nếu đã chọn endDate, không cho chọn startDate >= endDate
  const end = formData.value.endDate ? new Date(formData.value.endDate) : null
  return end ? date >= end : false
}

function disableEndDates(date: Date) {
  // Nếu đã chọn startDate, không cho chọn endDate <= startDate
  const start = formData.value.startDate ? new Date(formData.value.startDate) : null
  return start ? date <= start : false
}
watch(
  () => [formData.value.durationHours, formData.value.classTypeId],
  () => recalculateTotalMonths(),
  { immediate: true }
)

watch(
  () => classTypeStore.classTypes,
  () => recalculateTotalMonths()
)

function recalculateTotalMonths() {
  const classType = selectedClassType.value
  const durationHours = Number(formData.value.durationHours) || 0
  const hoursPerSession = Number(classType?.hoursPerSession) || 0
  const sessionsPerWeek = Number(classType?.sessionsPerWeek) || 0
  const denominator = hoursPerSession * sessionsPerWeek * 4

  if (!classType || denominator <= 0 || durationHours <= 0) {
    formData.value.totalMonths = 0
    return
  }

  const total = durationHours / denominator
  formData.value.totalMonths = Number(total.toFixed(2))
}

watch(
  () => formData.value.startDate,
  (newStart) => {
    const end = formData.value.endDate
    if (!newStart || !end) {
      // Re-validate để xoá lỗi cũ nếu có
      baseDialogRef.value?.formRef?.validateField?.(['startDate', 'endDate'])
      return
    }
    const s = new Date(newStart)
    const e = new Date(end)
    if (!isNaN(s.getTime()) && !isNaN(e.getTime()) && s >= e) {
      // Xoá endDate để tránh trạng thái không hợp lệ
      formData.value.endDate = ''
      // Có thể cảnh báo nhẹ (tuỳ thích)
      // notificationStore.showToast('warning', { key: 'validation.adjustedEndDate' })
      // Re-validate 2 field
      baseDialogRef.value?.formRef?.validateField?.(['startDate', 'endDate'])
    }
  }
)

const rules = {
  tuitionCode: [
    { required: true, message: t('validation.required'), trigger: 'blur' },
    { min: 1, max: 20, message: t('validation.maxLength', { max: 20 }), trigger: 'blur' }
  ],
  tuitionName: [
    { required: true, message: t('validation.required'), trigger: 'blur' },
    { min: 1, max: 100, message: t('validation.maxLength', { max: 100 }), trigger: 'blur' }
  ],
  courseId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
  classTypeId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
  durationHours: [
    { required: true, message: t('validation.required'), trigger: 'change' },
    {
      validator: (_: any, value: number, cb: any) => {
        if (value < 1) return cb(new Error(t('validation.min', { min: 1 })))
        cb()
      },
      trigger: 'change'
    }
  ],
  minHours: [
    {
      validator: (_: any, value: number, cb: any) => {
        if (value < 1) return cb(new Error(t('validation.min', { min: 1 })))
        if (value > formData.value.durationHours)
          return cb(new Error(t('validation.minHoursNotExceed')))
        cb()
      },
      trigger: 'change'
    }
  ],
  tuitionFee: [
    { required: true, message: t('validation.required'), trigger: 'change' },
    {
      validator: (_: any, value: number, cb: any) => {
        if (value < 0) return cb(new Error(t('validation.min', { min: 0 })))
        cb()
      },
      trigger: 'change'
    }
  ],
  unit: [{ required: true, message: t('validation.required'), trigger: 'change' }],
  status: [{ required: true, message: t('validation.required'), trigger: 'change' }],
  startDate: [
    {
      validator: (_: any, value: string, cb: any) => {
        const e = formData.value.endDate
        // Nếu thiếu 1 trong 2 -> OK
        if (!value || !e) return cb()
        const sDate = new Date(value)
        const eDate = new Date(e)
        if (isNaN(sDate.getTime()) || isNaN(eDate.getTime())) return cb()
        if (sDate >= eDate) {
          return cb(new Error(t('validation.startDateBeforeEnd')))
        }
        cb()
      },
      trigger: 'change'
    }
  ],
  endDate: [
    {
      validator: (_: any, value: string, cb: any) => {
        const s = formData.value.startDate
        // nếu không nhập endDate hoặc không nhập startDate -> OK
        if (!value || !s) return cb()
        // so sánh theo 'YYYY-MM-DD'
        const sDate = new Date(s)
        const eDate = new Date(value)
        if (isNaN(sDate.getTime()) || isNaN(eDate.getTime())) return cb()
        if (eDate <= sDate) {
          return cb(new Error(t('validation.endDateAfterStart')))
        }
        cb()
      },
      trigger: 'change'
    }
  ],

}

function closeModal() {
  emit('update:visible', false)
  emit('close')
}

onMounted(async () => {
  await Promise.all([
    courseStore.fetchAllCourses(),
    classTypeStore.fetchAllClassTypes(),
  ])
})

function onSubmit() {
  const form = baseDialogRef.value?.formRef
  form?.validate((valid: boolean) => {
    if (valid) {
      const payload: TuitionModel = {
        ...formData.value,
        tuitionCode: (formData.value.tuitionCode || '').trim().toUpperCase(),
        tuitionName: (formData.value.tuitionName || '').trim(),
      }
      emit('submit', payload)
    } else {
      notificationStore.showToast('error', { key: 'validation.formInvalid' })
    }
  })
}

function onDelete() {
  emit('delete', formData.value)
}
</script>
