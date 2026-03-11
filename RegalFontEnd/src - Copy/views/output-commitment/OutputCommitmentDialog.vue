<template>
  <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
    :mode="props.mode" :loading="submitting || props.loading" @submit="onSubmit" @delete="onDelete"
    @update:visible="emit('update:visible', $event)" @close="closeModal">
    <template #form>
      <el-row :gutter="20">
        <!-- Student Code -->
        <el-col :span="24">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('outputCommitment.studentCode') }}</label>
          <el-form-item prop="studentCode">
            <el-input v-model="formData.studentCode" :disabled="isView" @keyup.enter="onStudentCodeEnter" />
          </el-form-item>
        </el-col>

        <!-- Student selector -->
        <!-- <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('outputCommitment.student') }}</label>
          <el-form-item prop="studentId" v-if="!isView">
            <el-select v-model="formData.studentId" clearable filterable>
              <el-option v-for="opt in studentOptions" :key="opt.value" :label="opt.label" :value="opt.value" />
            </el-select>
          </el-form-item>
          <BaseBadge v-else :label="formData.student?.studentName || '-'" :rawLabel="true" />
        </el-col> -->

        <!-- Student quick info -->
        <el-col :span="24">
          <div v-if="studentInfo" class="student-info-box p-3 rounded-3 border mb-2">
            <el-row :gutter="20">
              <el-col :span="6">
                <label class="fs-6 fw-semibold mb-1 d-block text-body-secondary">{{ t('outputCommitment.studentName')
                }}</label>
                <div class="fw-semibold">{{ studentInfo.fullName || '-' }}</div>
              </el-col>
              <el-col :span="6">
                <label class="fs-6 fw-semibold mb-1 d-block text-body-secondary">{{ t('outputCommitment.branch')
                }}</label>
                <div class="fw-semibold">{{ studentInfo.companyName || '-' }}</div>
              </el-col>
              <el-col :span="6">
                <label class="fs-6 fw-semibold mb-1 d-block text-body-secondary">{{ t('outputCommitment.region')
                }}</label>
                <div class="fw-semibold">{{ studentInfo.regionName || '-' }}</div>
              </el-col>
              <el-col :span="6">
                <label class="fs-6 fw-semibold mb-1 d-block text-body-secondary">{{ t('outputCommitment.advisor')
                }}</label>
                <div class="fw-semibold">{{ studentInfo.advisor || '-' }}</div>
              </el-col>
            </el-row>
          </div>
          <el-alert
            v-else-if="lookupAttempted && lookupCode === (formData.studentCode || '').trim() && formData.studentCode"
            type="warning" :title="t('outputCommitment.studentNotFound')" show-icon :closable="false" />
        </el-col>

        <el-col :span="24" class="d-flex justify-content-end mb-2">
          <el-button type="primary" plain size="small" @click="onDownloadPdf" :loading="pdfLoading">
            PDF
          </el-button>
        </el-col>

        <!-- Beginning Level -->
        <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('outputCommitment.beginningLevel') }}</label>
          <el-form-item prop="beginningLevel">
            <el-input v-model="formData.beginningLevel" :disabled="isView" />
          </el-form-item>
        </el-col>

        <!-- Final Level -->
        <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('outputCommitment.finalLevel') }}</label>
          <el-form-item prop="finalLevel">
            <el-input v-model="formData.finalLevel" :disabled="isView" />
          </el-form-item>
        </el-col>

        <!-- Commitment Info -->
        <el-col :span="24">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('outputCommitment.outputCommitmentInfo') }}</label>
          <el-form-item prop="outputCommitmentInfo">
            <el-input type="textarea" :rows="3" v-model="formData.outputCommitmentInfo" :disabled="isView" />
          </el-form-item>
        </el-col>

        <!-- Status -->
        <el-col :span="24">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('outputCommitment.outputCommitmentStatus') }}</label>
          <el-form-item prop="outputCommitmentStatus">
            <el-radio-group v-model="formData.outputCommitmentStatus" :disabled="isView">
              <el-radio :value="OutputCommitmentStatus.NotFinished">{{ t('outputCommitment.status.notFinished') }}</el-radio>
              <el-radio :value="OutputCommitmentStatus.Finished">{{ t('outputCommitment.status.finished') }}</el-radio>
            </el-radio-group>
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
import type { OutputCommitmentModel } from '@/api/OutputCommitmentApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useStudentStore } from '@/stores/studentStore'
import { OutputCommitmentStatus } from '@/types'
import { useOutputCommitmentStore } from '@/stores/outputCommitmentStore'

const props = defineProps<{
  visible: boolean,
  mode?: 'create' | 'edit' | 'view',
  loading: boolean,
  data: Partial<OutputCommitmentModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const notificationStore = useNotificationStore()
const studentStore = useStudentStore()
const outputCommitmentStore = useOutputCommitmentStore()

const isView = computed(() => props.mode === 'view')
const modeTitle = computed(() => {
  if (isView.value) return t('outputCommitment.detailTitle')
  if (props.mode === 'edit') return t('outputCommitment.editTitle')
  return t('outputCommitment.addTitle')
})

const baseDialogRef = ref()
const submitting = ref(false)
const codeLookupLoading = ref(false)
const lookupAttempted = ref(false)
const lookupCode = ref('')
const pdfLoading = ref(false)

const formData = ref<Partial<OutputCommitmentModel>>({
  id: '',
  studentCode: '',
  studentId: '',
  beginningLevel: '',
  finalLevel: '',
  outputCommitmentInfo: '',
  outputCommitmentStatus: OutputCommitmentStatus.NotFinished,
})

watch(() => props.data, (val) => {
  if (val) formData.value = { ...formData.value, ...val }
  else {
    formData.value = {
      studentCode: '',
      studentId: '',
      beginningLevel: '',
      finalLevel: '',
      outputCommitmentInfo: '',
      outputCommitmentStatus: OutputCommitmentStatus.NotFinished,
    }
  }
}, { immediate: true })

const rules = {
  studentCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
  outputCommitmentStatus: [{ required: true, message: t('validation.required'), trigger: 'change' }],
}



const matchedStudent = computed(() => {
  const selected = studentStore.selectedStudent
  const inlineStudent = formData.value.student as any
  const id = (formData.value.studentId || '').toString()
  const code = (formData.value.studentCode || '').trim().toLowerCase()

  if (selected) {
    if (id && (selected.id || '').toString() === id) return selected
    if (code && (selected.studentCode || '').toLowerCase() === code) return selected
  }
  if (inlineStudent) {
    if (id && (inlineStudent.id || '').toString() === id) return inlineStudent
    if (code && ((inlineStudent.studentCode || inlineStudent.code || '').toLowerCase() === code)) return inlineStudent
  }
  return null
})

const studentInfo = computed(() => {
  if (!matchedStudent.value) return null
  const student = matchedStudent.value
  const fullName = (student as any).studentName || student.fullName || ''
  const companyName = student.company?.companyName || (student as any).companyName || ''
  const regionName = student.region?.regionName || student.region?.name || (student as any).regionName || ''
  const advisor = student.employee?.applicationUser?.fullName || student.employee?.fullName || (student as any).employeeName || ''
  return { fullName, companyName, regionName, advisor }
})

watch(matchedStudent, (student) => {
  if (student) {
    const studentId = student.id || ''
    formData.value.studentId = studentId
    if (!formData.value.studentCode) formData.value.studentCode = student.studentCode || ''
    formData.value.student = { ...(formData.value.student || {}), studentName: student.fullName || (student as any).studentName }
  } else if (formData.value.studentCode) {
    formData.value.studentId = ''
    formData.value.student = undefined
  }
})

watch(() => formData.value.studentCode, (code) => {
  if ((code || '').trim() !== lookupCode.value) {
    lookupAttempted.value = false
  }
  if (!code) {
    formData.value.studentId = ''
    formData.value.student = undefined
  }
})

onMounted(async () => {
  // Không tải toàn bộ danh sách học viên; chỉ fetch theo mã khi người dùng nhập
})

function closeModal() {
  emit('update:visible', false)
  emit('close')
}

async function onStudentCodeEnter() {
  if (isView.value) return
  const code = (formData.value.studentCode || '').trim()
  if (!code) return
  lookupAttempted.value = true
  lookupCode.value = code
  await fetchStudentByCodeAndFill(code)
}

async function fetchStudentByCodeAndFill(code: string) {
  if (!code) return null
  lookupAttempted.value = true
  lookupCode.value = code
  codeLookupLoading.value = true
  try {
    const student = await studentStore.fetchStudentByCode(code)
    if (student) {
      formData.value.studentId = student.id || ''
      formData.value.studentCode = student.studentCode || code
      formData.value.student = {
        ...(formData.value.student || {}),
        studentName: student.fullName || (student as any).studentName,
      }
    } else {
      formData.value.studentId = ''
      formData.value.student = undefined
    }
  } catch (error) {
    console.error('Error lookup student by code:', error)
    //notificationStore.showToast('error', { key: 'toast.fetchError', params: { model: t('models.student') } })
  } finally {
    codeLookupLoading.value = false
  }
}

async function onDownloadPdf() {
  if (pdfLoading.value) return
  pdfLoading.value = true
  try {
    const { student, ...rest } = formData.value
    const res = await outputCommitmentStore.downloadPdf(rest)
    const blob = new Blob([res as any], { type: 'application/pdf' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `CamKetDauRa_${(formData.value.studentCode || 'output').replace(/\\s+/g, '')}.pdf`
    link.click()
    window.URL.revokeObjectURL(url)
  } catch (err) {
    console.error('Error downloading pdf:', err)
    notificationStore.showToast('error', { key: 'toast.fetchError', params: { model: t('models.outputCommitment') } })
  } finally {
    pdfLoading.value = false
  }
}

watch(() => props.visible, async (val) => {
  if (val && formData.value.id && formData.value.studentCode) {
    if (!matchedStudent.value || !hasStudentDetails(matchedStudent.value)) {
      await fetchStudentByCodeAndFill(formData.value.studentCode)
    }
  }
})

function hasStudentDetails(student: any) {
  return Boolean(
    (student.company?.companyName || student.companyName) ||
    (student.region?.regionName || student.region?.name || student.regionName) ||
    (student.employee?.applicationUser?.fullName || student.employee?.fullName || student.employeeName)
  )
}

function onSubmit() {
  const form = (baseDialogRef.value as any)?.formRef
  form.validate((valid: boolean) => {
    if (valid) {
      const { student, ...rest } = formData.value
      emit('submit', { ...rest })
    } else notificationStore.showToast('error', { key: 'validation.formInvalid' })
  })
}
function onDelete() {
  emit('delete', formData.value)
}
</script>
