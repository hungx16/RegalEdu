<template>
  <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
    width="800" :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
    @update:visible="emit('update:visible', $event)" @close="closeModal">
    <template #form>
      <el-row :gutter="20">
        <!-- Bật đa ngôn ngữ -->
        <el-col :span="12">
          <el-form-item>
            <el-checkbox v-model="formData.isMultilingual" :disabled="isView">
              {{ t('common.allowMultilingual') }}
            </el-checkbox>
          </el-form-item>
        </el-col>
        <!-- Hiển thị công khai -->
        <el-col :span="12">
          <el-form-item>
            <el-checkbox v-model="formData.isPublish" :disabled="isView">
              {{ t('common.isPublish') }}
            </el-checkbox>
          </el-form-item>
        </el-col>
        <!-- Code -->
        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('learningRoadMap.learningRoadMapCode') }}</label>
          <el-form-item prop="learningRoadMapCode">
            <el-input v-model="formData.learningRoadMapCode" :disabled="isView"
              :placeholder="t('learningRoadMap.codePlaceholder')" />
          </el-form-item>
        </el-col>
        <!-- Age Group -->
        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('learningRoadMap.ageGroup') }}</label>
          <el-form-item prop="ageGrId">
            <el-select v-model="formData.ageGrId" :disabled="isView" filterable clearable
              :placeholder="t('learningRoadMap.ageGroupPlaceholder')">
              <el-option v-for="ag in ageGroupStore.ageGroups" :key="ag.id" :label="ag.categoryName" :value="ag.id" />
            </el-select>
          </el-form-item>
        </el-col>
        <!-- Name -->
        <el-col :span="24">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('learningRoadMap.learningRoadMapName') }}</label>
          <el-form-item prop="learningRoadMapName">
            <el-input v-model="formData.learningRoadMapName" :disabled="isView"
              :placeholder="t('learningRoadMap.namePlaceholder')" />
          </el-form-item>
        </el-col>
        <el-col :span="24" v-if="formData.isMultilingual">
          <label class="required fs-6 fw-semibold mb-2 d-block">English program name</label>
          <el-form-item prop="enLearningRoadMapName">
            <el-input v-model="formData.enLearningRoadMapName" :disabled="isView" placeholder="English program name">
            </el-input>
          </el-form-item>
        </el-col>
        <!-- Description -->
        <el-col :span="24">
          <label :class="'fs-6 fw-semibold mb-2 d-block  required'"> {{
            t('learningRoadMap.description')
          }}</label>
          <el-form-item prop="description">
            <el-input type="textarea" v-model="formData.description" :rows="2" :disabled="isView" />
          </el-form-item>
        </el-col>
        <!-- EnDescription -->
        <el-col :span="24" v-if="formData.isMultilingual">
          <label :class="'fs-6 fw-semibold mb-2 d-block  required'">English Description</label>
          <el-form-item prop="enDescription">
            <el-input type="textarea" v-model="formData.enDescription" :rows="2" :disabled="isView" />
          </el-form-item>
        </el-col>


        <!-- VotingRate -->
        <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('learningRoadMap.votingRate') }}</label>
          <el-form-item prop="votingRate">
            <div class="d-flex align-items-center gap-3">
              <el-input-number v-model="formData.votingRate" :min="0" :max="5" :step="0.1" :precision="1"
                :disabled="isView" />
            </div>
          </el-form-item>
        </el-col>
        <!-- NumberOfStudents -->
        <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('learningRoadMap.numberOfStudents') }}</label>
          <el-form-item prop="numberOfStudents">
            <div class="d-flex align-items-center gap-3">
              <el-input-number v-model="formData.numberOfStudents" :disabled="isView" :min="0" />
            </div>
          </el-form-item>
        </el-col>
        <!-- NumberOfSatisfiedStudents -->
        <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('learningRoadMap.numberOfSatisfiedStudents') }}</label>
          <el-form-item prop="numberOfSatisfiedStudents">
            <div class="d-flex align-items-center gap-3">
              <el-input-number v-model="formData.numberOfSatisfiedStudents" :disabled="isView" :min="0" />
            </div>
          </el-form-item>
        </el-col>
        <!-- Order -->
        <el-col :xs="24" :sm="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('learningRoadMap.order') }}</label>
          <el-form-item prop="order" v-if="!isView">
            <el-input-number v-model="formData.order" :min="0" :disabled="isView" />
          </el-form-item>
          <el-form-item v-else>
            <el-input-number v-model="formData.order" :min="0" :disabled="true" />
          </el-form-item>
        </el-col>

        <!-- Commitment Output -->
        <el-col :xs="24" :sm="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('learningRoadMap.commitmentOutput') }}</label>
          <el-form-item prop="commitmentOutput">
            <el-radio-group v-model="formData.commitmentOutput" :disabled="isView">
              <el-radio :value="true">{{ t('learningRoadMap.commitmentOutput') }}</el-radio>
              <el-radio :value="false">{{ t('learningRoadMap.notCommitmentOutput') }}</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <el-col :xs="24" :sm="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
          <el-form-item prop="status">
            <el-radio-group v-model="formData.status" :disabled="isView">
              <el-radio :value="0">{{ t('common.active') }}</el-radio>
              <el-radio :value="1">{{ t('common.inactive') }}</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
        <!-- Images -->
        <el-col :span="24">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('learningRoadMap.image') }}</label>
          <FileManager ref="fileMgrRef" v-model="(formData as any).images"
            accept="image/svg+xml,image/png,image/jpeg,image/jpg,image/webp,image/gif,image/bmp,image/tiff,.svg"
            v-model:removedIds="removedImageIds" :fields="imageFields" unique-boolean-key="isCover"
            :item-title="t('learningRoadMap.image')" :multiple="true" :disabled="isView" />

        </el-col>
        <el-col :span="24" v-if="hasLearningRoadMapId">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('learningRoadMap.courseListTitle') }}</label>
          <BaseTable :columns="courseColumns" :items="courseRows" :showCheckboxColumn="false"
            :showActionsColumn="false" :showPagination="false" height="240" />
        </el-col>
      </el-row>
    </template>
  </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import BaseTable, { type BaseTableColumn } from '@/components/table/BaseTable.vue'
import { useNotificationStore } from '@/stores/notificationStore'
import type { LearningRoadMapModel } from '@/api/LearningRoadMapApi'
import { useAgeGroupStore } from '@/stores/ageGroupStore'
import { useCourseStore } from '@/stores/courseStore'
import { useSkillStore } from '@/stores/skillStore'
import { CommitmentOutputType, CommitmentOutputTypeLabels, StatusType } from '@/types'
import type { FieldSchema } from '@/api/FileApi'
import FileManager from '@/components/file-manager/FileManager.vue'
const removedImageIds = ref<string[]>([])

const props = defineProps<{
  visible: boolean
  mode?: 'create' | 'edit' | 'view'
  loading: boolean
  learningRoadMapData: Partial<LearningRoadMapModel> | null
}>()
const imageUploadKey = ref(0)

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const notificationStore = useNotificationStore()
const ageGroupStore = useAgeGroupStore()
const courseStore = useCourseStore()
const skillStore = useSkillStore()

const baseDialogRef = ref()
const formRef = ref()

const imageFields: FieldSchema[] = [
  { key: 'isCover', label: t('image.cover'), type: 'switch', span: 4, activeText: t('image.coverYes'), inactiveText: t('image.coverNo') },
]
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
  if (isView.value) return t('learningRoadMap.detailTitle')
  if (isEdit.value) return t('learningRoadMap.editTitle')
  if (isCreate.value) return t('learningRoadMap.addTitle')
  return ''
})

const formData = ref<Partial<LearningRoadMapModel>>({
  learningRoadMapCode: '',
  learningRoadMapName: '',
  description: '',
  ageGrId: '',
  commitmentOutput: false,
  order: 0,
  createdAt: undefined as any,
  createdBy: '',
  updatedAt: undefined as any,
  updatedBy: '',
  isDeleted: false,
  status: 0,
  votingRate: 0,
  numberOfStudents: 0,
  numberOfSatisfiedStudents: 0,
  isMultilingual: false,
  enLearningRoadMapName: '',
  enDescription: '',
  isPublish: false,
  images: [],
  deletedImageIds: []
})
const fileMgrRef = ref<any>(null)
const hasLearningRoadMapId = computed(() => Boolean(formData.value.id))
const skillNameMap = computed<Record<string, string>>(() => {
  const map: Record<string, string> = {}
  ;(skillStore.skills ?? []).forEach((skill) => {
    if (!skill.id) return
    map[String(skill.id)] = skill.categoryName || skill.categoryCode || String(skill.id)
  })
  return map
})
const courseColumns: BaseTableColumn[] = [
  { key: 'courseCode', labelKey: 'course.code', minWidth: 130 },
  { key: 'courseName', labelKey: 'course.name', minWidth: 200 },
  { key: 'sequence', labelKey: 'course.sequence', minWidth: 160, align: 'center' },
  { key: 'minAvgScore', labelKey: 'course.minAvgScore', minWidth: 200, align: 'center' },
  { key: 'midExamNames', labelKey: 'course.midExamIds', minWidth: 220 },
  { key: 'finalExamNames', labelKey: 'course.finalExamIds', minWidth: 220 },
  { key: 'commitmentOutputDisplay', labelKey: 'course.commitmentOutputType', minWidth: 200 },
]

function parseIds(val?: string[] | string | null) {
  if (!val) return []
  if (Array.isArray(val)) return val.filter(Boolean).map(String)
  return String(val).split(/[#\$#,]+/).map(v => v.trim()).filter(Boolean)
}

function resolveCourseCommitmentOutputType(course: any): CommitmentOutputType {
  if (course?.commitmentOutputType !== undefined && course.commitmentOutputType !== null) {
    return Number(course.commitmentOutputType) as CommitmentOutputType
  }
  if ((course as any)?.isCommitmentBased) return CommitmentOutputType.Included
  if ((course as any)?.commitmentOutput) return CommitmentOutputType.SelfCommitment
  return CommitmentOutputType.None
}

const courseRows = computed(() => {
  const roadmapId = formData.value.id
  if (!roadmapId) return []
  const hasInlineCourses = Array.isArray((props.learningRoadMapData as any)?.courses)
  const rawCourses = hasInlineCourses
    ? (props.learningRoadMapData as any)?.courses
    : courseStore.courses
  const filteredCourses = hasInlineCourses
    ? (rawCourses ?? [])
    : (rawCourses ?? []).filter((course: any) => String(course.learningRoadMapId ?? '') === String(roadmapId))
  return filteredCourses
    .map((course: any) => {
      const midIds = parseIds(course.midExamIds)
      const finalIds = parseIds(course.finalExamIds)
      const midExamNames = midIds.map(id => skillNameMap.value[id] ?? id).join(', ')
      const finalExamNames = finalIds.map(id => skillNameMap.value[id] ?? id).join(', ')
      const commitmentType = resolveCourseCommitmentOutputType(course)
      const commitmentLabelKey = CommitmentOutputTypeLabels[commitmentType] ?? CommitmentOutputTypeLabels[CommitmentOutputType.None]
      const commitmentOutputDisplay = commitmentType === CommitmentOutputType.None
        ? t(commitmentLabelKey)
        : (course.commitmentLevel?.trim() || t(commitmentLabelKey))
      return {
        ...course,
        sequence: course.sequence ?? course.ordinalNumber ?? 0,
        minAvgScore: course.minAvgScore ?? '',
        midExamNames: midExamNames || '-',
        finalExamNames: finalExamNames || '-',
        commitmentOutputDisplay,
      }
    })
})

function normalizeImages(arr: any[] | undefined | null) {
  return (arr || []).map((x: any, i: number) => {
    const base: any = {
      path: x.path ?? '',
      isCover: !!x.isCover,
      file: null
    }
    const id = x.id ?? x.imageId
    if (id) base.id = id
    return base
  })
}
watch(
  () => props.learningRoadMapData,
  (data) => {
    if (data) {
      formData.value = {
        id: data.id ?? '',
        learningRoadMapCode: data.learningRoadMapCode ?? '',
        learningRoadMapName: data.learningRoadMapName ?? '',
        description: data.description ?? '',
        ageGrId: data.ageGrId ?? '',
        ageGroup: data.ageGroup ?? null,
        commitmentOutput: data.commitmentOutput ?? false,
        order: data.order ?? 0,
        createdAt: data.createdAt ?? '',
        createdBy: data.createdBy ?? '',
        status: data.status ?? StatusType.Active,
        isPublish: data.isPublish ?? false,
        votingRate: data.votingRate ?? 0,
        numberOfStudents: data.numberOfStudents ?? 0,
        numberOfSatisfiedStudents: data.numberOfSatisfiedStudents ?? 0,
        isMultilingual: data.isMultilingual ?? false,
        enLearningRoadMapName: data.enLearningRoadMapName ?? '',
        enDescription: data.enDescription ?? '',
        images: normalizeImages(data.images),
      }
    } else {
      formData.value = {
        learningRoadMapCode: '',
        learningRoadMapName: '',
        description: '',
        ageGrId: '',
        commitmentOutput: false,
        order: 0,
        status: StatusType.Active,
        votingRate: 0,
        numberOfStudents: 0,
        numberOfSatisfiedStudents: 0,
        isMultilingual: false,
        enLearningRoadMapName: '',
        enDescription: '',
        images: [],
      }
      removedImageIds.value = []

    }
    imageUploadKey.value++

  },
  { immediate: true }
)

async function ensureCoursesAndSkills() {
  if (!hasLearningRoadMapId.value) return
  if (!courseStore.courses.length) {
    await courseStore.fetchAllCourses()
  }
  if (!skillStore.skills.length) {
    await skillStore.fetchAllSkills()
  }
}

watch(
  () => [props.visible, hasLearningRoadMapId.value],
  ([visible, hasId]) => {
    if (visible && hasId) {
      void ensureCoursesAndSkills()
    }
  },
  { immediate: true }
)

const rules = {
  learningRoadMapCode: [
    { required: true, message: t('validation.required'), trigger: 'blur' },
    { min: 2, max: 50, message: t('validation.length', { min: 2, max: 50 }), trigger: 'blur' }
  ],
  learningRoadMapName: [
    { required: true, message: t('validation.required'), trigger: 'blur' },
    { min: 2, max: 255, message: t('validation.length', { min: 2, max: 255 }), trigger: 'blur' }
  ],
  ageGrId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
  order: [
    {
      validator: (_: any, val: number, cb: (err?: Error) => void) => {
        if (val === null || val === undefined || val < 0) return cb(new Error(t('validation.min', { min: 0 })))
        cb()
      },
      trigger: 'change'
    }
  ]
}

function resetForm() {
  const form = baseDialogRef.value?.formRef
  form?.resetFields?.()
}

function closeModal() {
  emit('update:visible', false)
  emit('close')
  resetForm()
}
function onSubmit() {
  const form = baseDialogRef.value?.formRef
  form.validate(async (valid: boolean) => {
    if (!valid) {
      notificationStore.showToast('error', { key: 'validation.formInvalid' })
      return
    }
    // 1) Upload toàn bộ file mới ngay trong FileManager
    await fileMgrRef.value?.uploadPendingFiles()
    // Content đảm bảo string
    if (props.mode === 'create') {

    } else if (props.mode === 'edit') {
      (formData.value as any).deletedImageIds = removedImageIds.value
    }
    const attachments = fileMgrRef.value?.packImages(props.mode === 'create') ?? []
    const payload: any = {
      ...formData.value,

      images: attachments
    }
    emit('submit', payload)
  })
}

function onDelete() {
  emit('delete', formData.value)
}

onMounted(async () => {
  // đảm bảo có dữ liệu AgeGroup để đổ select
  await ageGroupStore.fetchAllAgeGroups();
})
</script>
