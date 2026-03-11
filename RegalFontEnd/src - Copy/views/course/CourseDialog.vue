<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        width="1300" :mode="props.mode" :loading="submitting || props.loading" @submit="onSubmit" @delete="onDelete"
        :actionsColumnWidth="180" @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <!-- Multilingual + Publish -->
                <el-col :span="12">
                    <el-form-item>
                        <el-checkbox v-model="formData.isMultilingual" :disabled="isView">
                            {{ t('common.allowMultilingual') }}
                        </el-checkbox>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item>
                        <el-checkbox v-model="formData.isPublish" :disabled="isView">
                            {{ t('common.isPublish') }}
                        </el-checkbox>
                    </el-form-item>
                </el-col>
                <!-- CourseCode -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('course.courseCode') }}</label>
                    <el-form-item prop="courseCode">
                        <el-input v-model="formData.courseCode" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- CourseName -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('course.courseName') }}</label>
                    <el-form-item prop="courseName">
                        <el-input v-model="formData.courseName" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- English CourseName -->
                <el-col :span="24" v-if="formData.isMultilingual">
                    <label class="required fs-6 fw-semibold mb-2 d-block">English Name</label>
                    <el-form-item prop="enCourseName">
                        <el-input v-model="formData.enCourseName" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- Description -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('course.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="5" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- English Description -->
                <el-col :span="24" v-if="formData.isMultilingual">
                    <label class="required fs-6 fw-semibold mb-2 d-block">English Description</label>
                    <el-form-item prop="enDescription">
                        <el-input type="textarea" v-model="formData.enDescription" :rows="5" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- CourseContent -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('course.courseContent') }}</label>
                    <el-form-item prop="courseContent">
                        <TagList v-model="formData.courseContent" :maxVisible="10" :maxTags="8" :dismissible="true"
                            :distinct-colors="true" :autoColor="true"
                            @update:modelValue="() => validateField('courseContent')" />
                    </el-form-item>
                </el-col>

                <!-- English CourseContent -->
                <el-col :span="24" v-if="formData.isMultilingual">
                    <label class="required fs-6 fw-semibold mb-2 d-block">English Content</label>
                    <el-form-item prop="enCourseContent">
                        <TagList v-model="formData.enCourseContent" :maxVisible="10" :maxTags="8" :dismissible="true"
                            :distinct-colors="true" :autoColor="true"
                            @update:modelValue="() => validateField('enCourseContent')" />
                    </el-form-item>
                </el-col>

                <!-- CourseKey -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('course.courseKey') }}</label>
                    <el-form-item prop="courseKey">
                        <TagList v-model="formData.courseKey" :maxVisible="10" :maxTags="8" :dismissible="true"
                            :distinct-colors="true" :autoColor="true"
                            @update:modelValue="() => validateField('courseKey')" />
                    </el-form-item>
                </el-col>

                <!-- English CourseKey -->
                <el-col :span="24" v-if="formData.isMultilingual">
                    <label class="required fs-6 fw-semibold mb-2 d-block">English Course Key</label>
                    <el-form-item prop="enCourseKey">
                        <TagList v-model="formData.enCourseKey" :maxVisible="10" :maxTags="8" :dismissible="true"
                            :distinct-colors="true" :autoColor="true"
                            @update:modelValue="() => validateField('enCourseKey')" />
                    </el-form-item>
                </el-col>


                <!-- Mid Exam IDs (Kỹ năng kiểm tra giữa kỳ) -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">
                        {{ t('course.midExamIds') || 'Danh sách nội dung kiểm tra giữa kỳ' }}
                    </label>
                    <el-form-item prop="midExamIds">
                        <el-select v-model="formData.midExamIds" multiple filterable clearable :disabled="isView"
                            :placeholder="t('course.midExamIdsPlaceholder')">
                            <el-option v-for="sk in skillStore.skills" :key="sk.id" :label="sk.categoryName"
                                :value="String(sk.id)" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <!-- Final Exam IDs (Kỹ năng kiểm tra cuối kỳ) -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">
                        {{ t('course.finalExamIds') }}
                    </label>
                    <el-form-item prop="finalExamIds">
                        <el-select v-model="formData.finalExamIds" multiple filterable clearable :disabled="isView"
                            :placeholder="t('course.finalExamIdsPlaceholder')">
                            <el-option v-for="sk in skillStore.skills" :key="sk.id" :label="sk.categoryName"
                                :value="String(sk.id)" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <!-- Sequence -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('course.sequence') }}</label>
                    <el-form-item prop="sequence">
                        <el-input-number v-model="formData.sequence" :min="0" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- MinAvgScore -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('course.minAvgScore') }}</label>
                    <el-form-item prop="minAvgScore">
                        <el-input-number v-model="formData.minAvgScore" :min="0" :disabled="isView" />
                    </el-form-item>
                </el-col>



                <!-- Learning Roadmap -->
                <el-col :span="24" v-if="!isView">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('course.learningRoadMapId') }}</label>
                    <el-form-item prop="learningRoadMapId">
                        <el-select v-model="formData.learningRoadMapId" clearable filterable>
                            <el-option v-for="roadmap in learningRoadMapStore.learningRoadMaps" :key="roadmap.id"
                                :label="roadmap.learningRoadMapName" :value="roadmap.id" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('course.ordinalNumber') }}</label>
                    <el-form-item>
                        <el-input :model-value="formData.ordinalNumber ?? 0" disabled />
                    </el-form-item>
                </el-col>

                <!-- Duration -->
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('course.duration') }}</label>
                    <el-form-item prop="duration">
                        <el-input v-model="formData.duration" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- English Duration -->
                <el-col :span="24" v-if="formData.isMultilingual">
                    <label class="required fs-6 fw-semibold mb-2 d-block">English Duration</label>
                    <el-form-item prop="enDuration">
                        <el-input v-model="formData.enDuration" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- NumberOfStudents -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('course.numberOfStudents') }}</label>
                    <el-form-item prop="numberOfStudents">
                        <el-input-number v-model="formData.numberOfStudents" :min="0" :disabled="isView" />
                    </el-form-item>
                </el-col>

                <!-- VotingRate -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('course.votingRate') }}</label>
                    <el-form-item prop="votingRate">
                        <el-input-number v-model="formData.votingRate" :min="0" :max="5" :step="0.1"
                            :disabled="isView" />
                    </el-form-item>
                </el-col>

                <el-col :xs="24" :sm="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('course.commitmentOutputType') }}</label>
                    <el-form-item prop="commitmentOutputType">
                        <el-radio-group v-model="formData.commitmentOutputType" :disabled="isView">
                            <el-radio :value="CommitmentOutputType.None">
                                {{ t(CommitmentOutputTypeLabels[CommitmentOutputType.None]) }}
                            </el-radio>
                            <el-radio :value="CommitmentOutputType.Included">
                                {{ t(CommitmentOutputTypeLabels[CommitmentOutputType.Included]) }}
                            </el-radio>
                            <el-radio :value="CommitmentOutputType.SelfCommitment">
                                {{ t(CommitmentOutputTypeLabels[CommitmentOutputType.SelfCommitment]) }}
                            </el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
                <!-- Status -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
                    <el-form-item prop="status">
                        <el-radio-group v-model="formData.status" :disabled="isView">
                            <el-radio :value="0">{{ t('common.active') }}</el-radio>
                            <el-radio :value="1">{{ t('common.inactive') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
                <!-- CommitmentLevel -->
                <el-col :span="24" v-if="formData.commitmentOutputType !== CommitmentOutputType.None">
                    <label
                        :class="[
                            'fs-6 fw-semibold mb-2 d-block',
                            { required: formData.commitmentOutputType === CommitmentOutputType.SelfCommitment }
                        ]">
                        {{ t('course.commitmentLevel') }}
                    </label>
                    <el-form-item prop="commitmentLevel">
                        <el-input v-model="formData.commitmentLevel" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Reference -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('course.reference') }}</label>
                    <el-form-item prop="reference">
                        <el-input v-model="formData.reference" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Tuition table -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('course.tuitionList') }}</label>

                    <div class="p-4 bg-light rounded-4 shadow-sm">
                        <!-- Nút thêm -->
                        <div class="d-flex justify-content-end mb-3">
                            <el-button type="primary" :icon="Plus" @click="addTuition" :disabled="isView">
                                {{ t('common.add') }}
                            </el-button>
                        </div>

                        <!-- Bảng học phí -->
                        <BaseTable :columns="tuitionColumns" :items="formData.tuitions ?? []" :showPagination="false"
                            :showActionsColumn="true" :showEdit="false" :showView="false" :showDelete="false"
                            height="250" @delete="removeTuition">
                            <!-- Cột: Tên học phí -->
                            <template #cell-tuitionName="{ item }">
                                <el-input v-model="item.tuitionName" :disabled="isView" />
                            </template>

                            <!-- Cột: Loại lớp -->
                            <template #cell-classTypeId="{ item }">
                                <el-select v-model="item.classTypeId" :disabled="isView"
                                    :placeholder="t('course.tuition.selectClassType')"
                                    @change="handleTuitionClassTypeChange(item)">
                                    <el-option v-for="cls in classTypeStore.classTypes" :key="cls.id"
                                        :label="cls.classTypeName" :value="cls.id"
                                        :disabled="isClassTypeOptionDisabled(cls.id, item)" />
                                </el-select>
                            </template>


                            <!-- Column: Unit -->
                            <template #cell-unit="{ item }">
                                <el-select v-model="item.unit" :disabled="isView" style="width: 100%;">
                                    <el-option :label="t('unit.hour')" :value="UnitType.Hour" />
                                    <el-option :label="t('unit.session')" :value="UnitType.Session" />
                                    <el-option :label="t('unit.month')" :value="UnitType.Month" />
                                    <el-option :label="t('unit.course')" :value="UnitType.Course" />
                                </el-select>
                            </template>

                            <!-- Column: Price -->
                            <template #cell-tuitionFee="{ item }">
                                <CurrencyInput v-model="item.tuitionFee" :disabled="isView" locale="vi-VN"
                                    currency="VND" />
                            </template>
                            <template #cell-durationHours="{ item }">
                                <el-input-number v-model="item.durationHours" :disabled="isView" :min="0"
                                    @change="handleTuitionDurationChange(item)" />
                            </template>

                            <!-- Cột: Giờ tối thiểu -->
                            <template #cell-minHours="{ item }">
                                <el-input-number v-model="item.minHours" :disabled="isView" :min="0" />
                            </template>

                            <!-- Cột: Số tháng -->
                            <template #cell-totalMonths="{ item }">
                                <el-input-number v-model="item.totalMonths" :disabled="true" :min="0" :precision="2" />
                            </template>
                            <template #actions="{ item }">
                                <el-tooltip :content="t('course.tuition.addLesson')" placement="top">
                                    <el-button circle size="small" type="success" @click="openLessonDialog(item)"
                                        :disabled="isView">
                                        <el-icon>
                                            <Plus />
                                        </el-icon>
                                    </el-button>
                                </el-tooltip>
                                <!-- Nút: Xoá tuition với el-popconfirm -->
                                <el-popconfirm
                                    :title="t('course.tuition.confirm.deleteOne', { name: item.tuitionName ?? '' })"
                                    :confirm-button-text="t('common.ok')" :cancel-button-text="t('common.cancel')"
                                    @confirm="removeTuition(item)">
                                    <template #reference>
                                        <el-button circle size="small" type="danger" plain :disabled="isView"
                                            class="ms-2">
                                            <el-icon><i class="bi bi-trash"></i></el-icon>
                                            <!-- hoặc dùng icon el-icon Delete -->
                                        </el-button>
                                    </template>
                                </el-popconfirm>
                            </template>

                        </BaseTable>
                    </div>
                </el-col>

            </el-row>
        </template>
    </BaseDialogForm>
    <CourseLessonDialog v-if="isLessonDialogVisible" :visible="isLessonDialogVisible" :tuition="selectedTuition"
        @update:visible="isLessonDialogVisible = $event" @submit="handleLessonSubmit" />


</template>

<script setup lang="ts">
import { computed, ref, watch, onMounted, nextTick } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { CourseLessonModel, CourseModel } from '@/api/CourseApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useCommonStore } from '@/stores/commonStore'
import { useLearningRoadMapStore } from '@/stores/learningRoadMapStore'
import TagList from '@/components/tag/TagList.vue'
import { useSkillStore } from '@/stores/skillStore'
import { useClassTypeStore } from '@/stores/classTypeStore'
import { Plus } from '@element-plus/icons-vue'
import BaseTable from '@/components/table/BaseTable.vue'
import type { TuitionModel } from '@/api/TuitionApi'
import type { ClassTypeModel } from '@/api/ClassTypeApi'
import { UnitType, CommitmentOutputType, CommitmentOutputTypeLabels } from '@/types'
import CurrencyInput from '@/components/currency-input/CurrencyInput.vue'
import CourseLessonDialog from './CourseLessonDialog.vue'

const classTypeStore = useClassTypeStore()
const classTypeMap = computed<Record<string, ClassTypeModel>>(() => {
    const map: Record<string, ClassTypeModel> = {}
        ; (classTypeStore.classTypes ?? []).forEach(ct => {
            if (ct.id) map[String(ct.id)] = ct
        })
    return map
})
const isLessonDialogVisible = ref(false)
const selectedTuition = ref<TuitionModel | null>(null)
const selectedTuitionIndex = ref<number | null>(null)
function openLessonDialog(item: TuitionModel) {
    if (!item.tuitionName?.trim() || !item.classTypeId || !item.durationHours || item.durationHours <= 0) {
        notificationStore.showToast('warning', {
            key: 'course.tuition.missingFields',
            params: {
                message: t('course.tuition.requireBeforeAddLesson')
            }
        })
        return
    }

    const index = (formData.value.tuitions ?? []).findIndex(t => t === item)
    if (index === -1) return
    selectedTuition.value = item
    selectedTuitionIndex.value = index
    isLessonDialogVisible.value = true
}


function handleLessonSubmit(lessons: CourseLessonModel[]) {
    console.log(selectedTuition.value);

    if (!selectedTuition.value) return

    // Gán lessons vào đúng tuition đang được chọn
    selectedTuition.value.courseLessons = lessons

    // Cập nhật lại mảng tuition trong form
    formData.value.tuitions = (formData.value.tuitions ?? []).map(t =>
        t === selectedTuition.value ? selectedTuition.value : t
    )

    isLessonDialogVisible.value = false
}
const tuitionColumns = [
    { key: 'tuitionName', labelKey: 'tuition.tuitionName', minWidth: 200 },
    { key: 'classTypeId', labelKey: 'tuition.classType', minWidth: 180 },
    { key: 'unit', labelKey: 'tuition.unit', minWidth: 140 },
    { key: 'tuitionFee', labelKey: 'tuition.tuitionFee', minWidth: 160 },
    { key: 'durationHours', labelKey: 'tuition.durationHours', width: 180, align: 'center' as const },
    { key: 'minHours', labelKey: 'tuition.minHours', width: 180, align: 'center' as const },
    { key: 'totalMonths', labelKey: 'tuition.totalMonths', width: 180, align: 'center' as const },
];

function resolveCommitmentOutputType(data: any): CommitmentOutputType {
    if (data?.commitmentOutputType !== undefined && data.commitmentOutputType !== null) {
        return Number(data.commitmentOutputType) as CommitmentOutputType
    }
    if ((data as any)?.isCommitmentBased) return CommitmentOutputType.Included
    if ((data as any)?.commitmentOutput) return CommitmentOutputType.SelfCommitment
    return CommitmentOutputType.None
}

const learningRoadMapStore = useLearningRoadMapStore()
const skillStore = useSkillStore()
const props = defineProps<{
    visible: boolean
    mode?: 'create' | 'edit' | 'view'
    loading: boolean
    courseData: Partial<CourseModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const commonStore = useCommonStore()
const notificationStore = useNotificationStore()

const baseDialogRef = ref()
const submitting = ref(false)
const isView = computed(() => props.mode === 'view')

const modeTitle = computed(() => {
    if (isView.value) return t('course.detailTitle')
    if (props.mode === 'edit') return t('course.editTitle')
    if (props.mode === 'create') formData.value.courseCode = commonStore.code ?? ''
    return t('course.addTitle')
})

const formData = ref<Partial<CourseModel> & {
    courseContent: string | string[],
    enCourseContent: string | string[],
    courseKey: string | string[],
    enCourseKey: string | string[],
    deletedTuitionIds?: string[]
}>({
    id: '',
    courseCode: '',
    courseName: '',
    enCourseName: '',
    description: '',
    enDescription: '',
    sequence: 0,
    minAvgScore: 0,
    commitmentOutputType: CommitmentOutputType.None,
    commitmentLevel: '',
    learningRoadMapId: '',
    courseContent: '',
    enCourseContent: '',
    courseKey: '',
    enCourseKey: '',
    reference: '',
    isPublish: false,
    isMultilingual: false,
    duration: '',
    enDuration: '',
    numberOfStudents: 0,
    votingRate: 0,
    status: 0,
    createdAt: '',
    updatedAt: '',
    isDeleted: false,
    midExamIds: [] as string[],
    finalExamIds: [] as string[],
    ordinalNumber: 0,
    tuitions: [] as TuitionModel[],
    deletedTuitionIds: []

})


watch(
    () => props.courseData,
    (data) => {
        if (data) {
            const toArray = (v: any) => {
                if (Array.isArray(v)) return v
                if (!v) return []
                return String(v)
                    .split('#$#')
                    .map(part => part.split(','))
                    .flat()
                    .map(s => s.trim())
                    .filter(Boolean)
            }
            const safeTag = (v: any) => Array.isArray(v) ? v : (v ?? '')
            const commitmentOutputType = resolveCommitmentOutputType(data)
            const { commitmentOutput: _commitmentOutput, isCommitmentBased: _isCommitmentBased, ...rest } = data as any
            formData.value = {
                ...formData.value,
                ...rest,
                commitmentOutputType,
                courseContent: safeTag((data as any).courseContent),
                enCourseContent: safeTag((data as any).enCourseContent),
                courseKey: safeTag((data as any).courseKey),
                enCourseKey: safeTag((data as any).enCourseKey),
                midExamIds: toArray((data as any).midExamIds),
                finalExamIds: toArray((data as any).finalExamIds),
                tuitions: (data as any).tuitions ?? []
            }
            const fallbackId = (data as any).id ?? (data as any).Id
            if (fallbackId) formData.value.id = fallbackId
            if (!formData.value.courseCode && (data as any).code) formData.value.courseCode = (data as any).code
            if (!formData.value.courseName && (data as any).name) formData.value.courseName = (data as any).name
            nextTick(() => syncAllTuitionTotals())
        } else {
            formData.value = {
                courseCode: '',
                courseName: '',
                description: '',
                courseContent: '',
                enCourseContent: '',
                courseKey: '',
                enCourseKey: '',
                sequence: 1,
                minAvgScore: 3,
                commitmentOutputType: CommitmentOutputType.None,
                commitmentLevel: '',
                learningRoadMapId: '',
                duration: '1',
                enDuration: '',
                numberOfStudents: 2,
                votingRate: 2,
                isPublish: false,
                isMultilingual: false,
                status: 0,
                midExamIds: [] as string[],
                finalExamIds: [] as string[],
                ordinalNumber: 0,
                tuitions: [] as any[],

            }
            nextTick(() => syncAllTuitionTotals())
        }
    },
    { immediate: true }
)
watch(
    () => formData.value.commitmentOutputType,
    (val) => {
        if (val === CommitmentOutputType.None) {
            formData.value.commitmentLevel = ''
        }
    }
)
const rules = {
    courseCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    courseName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    enCourseName: [
        { required: formData.value.isMultilingual, message: t('validation.required'), trigger: 'blur' }
    ],
    description: [{ required: false, message: t('validation.required'), trigger: 'blur' }],
    enDescription: [
        { required: formData.value.isMultilingual, message: t('validation.required'), trigger: 'blur' }
    ],
    commitmentLevel: [{
        validator: (_: any, val: any, cb: any) => {
            const isRequired = formData.value.commitmentOutputType === CommitmentOutputType.SelfCommitment
            if (!isRequired) return cb()
            if (!String(val ?? '').trim()) return cb(new Error(t('validation.required')))
            cb()
        }, trigger: ['blur', 'change']
    }],
    duration: [{ required: false, message: t('validation.required'), trigger: 'blur' }],
    enDuration: [
        { required: formData.value.isMultilingual, message: t('validation.required'), trigger: 'blur' }
    ],
    votingRate: [
        {
            validator: (_: any, val: any, cb: any) => {
                const v = Number(val)
                if (!Number.isFinite(v) || v < 0 || v > 5) return cb(new Error(t('validation.range', { min: 0, max: 5 })))
                cb()
            }, trigger: 'change'
        }],
    learningRoadMapId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    sequence: [{ type: 'number', min: 0, message: t('validation.min', { min: 0 }), trigger: 'change' }],
    minAvgScore: [
        { type: 'number', required: true, message: t('validation.required'), trigger: 'blur' },
        { type: 'number', min: 0, max: 100, message: t('validation.range', { min: 0, max: 100 }), trigger: ['change', 'blur'] },
    ],
    numberOfStudents: [{ type: 'number', min: 0, message: t('validation.min', { min: 0 }), trigger: 'change' }],
    courseContent: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    enCourseContent: [
        { required: formData.value.isMultilingual, message: t('validation.required'), trigger: 'blur' }
    ],
    courseKey: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    enCourseKey: [
        { required: formData.value.isMultilingual, message: t('validation.required'), trigger: 'blur' }
    ],
    midExamIds: [{ required: true, type: 'array', min: 1, message: t('validation.required'), trigger: 'change' }],
    finalExamIds: [{ required: true, type: 'array', min: 1, message: t('validation.required'), trigger: 'change' }],
    reference: [{ required: false, message: t('validation.required'), trigger: 'blur' }],
}
function toNum(v: any): number | null {
    const n = typeof v === 'string' ? Number(v) : v
    return Number.isFinite(n) ? n : null
}
function addTuition() {
    formData.value.tuitions = formData.value.tuitions ?? []
    const newTuition: TuitionModel = {
        tuitionCode: '',
        tuitionName: '',
        classTypeId: '',
        courseId: null,
        durationHours: 1,
        minHours: 1,
        totalMonths: 1,
        unit: UnitType.Hour, // mặc định
        tuitionFee: 1,
        courseLessons: []
    }
    formData.value.tuitions.push(newTuition)
}

function validateField(prop: string) {
    const form = (baseDialogRef.value as any)?.formRef
    form?.validateField?.(prop)
}

const removedTuitionIds = ref<string[]>([])

function removeTuition(item: TuitionModel) {
    if (!formData.value.tuitions) return

    // Nếu có id thật trong DB thì đưa vào danh sách xóa
    if (item.id && item.id.length === 36) {
        removedTuitionIds.value.push(item.id)

        // 🔥 đồng thời update reactive field
        formData.value.deletedTuitionIds = [
            ...(formData.value.deletedTuitionIds ?? []),
            item.id
        ]
    }

    // Nếu item mới (chưa có id) → so sánh reference
    formData.value.tuitions = formData.value.tuitions.filter(t =>
        t.id ? t.id !== item.id : t !== item
    )
}

function calculateTuitionTotalMonths(tuition: TuitionModel): number {
    const classType = tuition.classTypeId ? classTypeMap.value?.[String(tuition.classTypeId)] : null
    const durationHours = Number(tuition.durationHours) || 0
    const hoursPerSession = Number(classType?.hoursPerSession) || 0
    const sessionsPerWeek = Number(classType?.sessionsPerWeek) || 0
    const denominator = hoursPerSession * sessionsPerWeek * 4
    if (!classType || denominator <= 0 || durationHours <= 0) return 0
    return Number((durationHours / denominator).toFixed(2))
}

function updateTuitionTotalMonths(tuition: TuitionModel) {
    if (!tuition) return
    tuition.totalMonths = calculateTuitionTotalMonths(tuition)
}

function syncAllTuitionTotals() {
    (formData.value.tuitions ?? []).forEach(updateTuitionTotalMonths)
}

function isClassTypeOptionDisabled(classTypeId: ClassTypeModel['id'], currentTuition: TuitionModel) {
    if (!classTypeId) return false
    return (formData.value.tuitions ?? []).some(t =>
        t !== currentTuition && String(t.classTypeId ?? '') === String(classTypeId)
    )
}

function handleTuitionClassTypeChange(tuition: TuitionModel) {
    updateTuitionTotalMonths(tuition)
}

function handleTuitionDurationChange(tuition: TuitionModel) {
    updateTuitionTotalMonths(tuition)
}

function hasDuplicateTuitionClassTypes() {
    const seen = new Set<string>()
    for (const tuition of formData.value.tuitions ?? []) {
        if (!tuition.classTypeId) continue
        const key = String(tuition.classTypeId)
        if (seen.has(key)) return true
        seen.add(key)
    }
    return false
}

function normalizeLectureTypeId(value: any): string {
    // Handle array case (sometimes el-select might return array)
    if (Array.isArray(value)) {
        const val = value[0] ?? ''
        return val != null ? String(val).trim() : ''
    }
    // Handle string case
    if (typeof value === 'string') {
        return value.trim()
    }
    // Handle other cases
    return value != null ? String(value).trim() : ''
}

function toGuidArray(value: any): string[] {
    const guidPattern = /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/
    const arr = Array.isArray(value)
        ? value
        : (typeof value === 'string'
            ? value
                .split('#$#')
                .map(part => part.split(','))
                .flat()
            : [])
    return arr
        .map(v => String(v || '').trim())
        .filter(v => v && guidPattern.test(v))
}



function recomputeOrdinalNumber() {
    // lấy LRM sequence theo learningRoadMapId
    const lm = learningRoadMapStore.learningRoadMaps
        ?.find(x => String(x.id) === String(formData.value.learningRoadMapId))
    const lmSeq = toNum(lm?.order)
    const cSeq = toNum(formData.value.sequence)

    if (lmSeq == null || cSeq == null) {
        formData.value.ordinalNumber = 0
        return
    }
    // GHÉP thành "lmSeq.cSeq" -> number (vd: 2 và 5 -> 2.5; 3 và 12 -> 3.12)
    formData.value.ordinalNumber = Number(`${lmSeq}.${cSeq}`)
}
// Khi đổi sequence hoặc learningRoadMapId
watch(
    () => [formData.value.sequence, formData.value.learningRoadMapId],
    () => recomputeOrdinalNumber()
)

// Khi danh sách LRM tải xong (tránh tính trước khi có data)
watch(
    () => learningRoadMapStore.learningRoadMaps?.length,
    () => recomputeOrdinalNumber()
)

// Sau khi nhận courseData lần đầu
watch(
    () => props.courseData,
    () => recomputeOrdinalNumber(),
    { immediate: true }
)

watch(
    () => classTypeStore.classTypes,
    () => syncAllTuitionTotals()
)

onMounted(async () => {
    if (!learningRoadMapStore.learningRoadMaps?.length) {
        await learningRoadMapStore.fetchAllLearningRoadMaps()
    }
    if (!skillStore.skills?.length) {
        await skillStore.fetchAllSkills()
    }

    if (!classTypeStore.classTypes?.length) {
        await classTypeStore.fetchAllClassTypes()
    }

})

function closeModal() {
    emit('update:visible', false)
    emit('close')
}


async function onSubmit() {
    const form = (baseDialogRef.value as any)?.formRef
    form.validate(async (valid: boolean) => {
        if (!valid) {
            notificationStore.showToast('error', { key: 'validation.formInvalid' })
            return
        }
        const invalidTuition = (formData.value.tuitions ?? []).find(t =>
            !t.tuitionName?.trim() || !t.classTypeId || !t.durationHours || t.durationHours <= 0
        )

        if (invalidTuition) {
            notificationStore.showToast('error', {
                key: 'course.tuition.missingFields',
                params: {
                    message: t('course.tuition.missingFieldsMessage')
                }
            })
            return
        }
        if (hasDuplicateTuitionClassTypes()) {
            notificationStore.showToast('error', { key: 'course.tuition.duplicateClassType' })
            return
        }
        submitting.value = true
        try {
            // 🧩 1. Giữ nguyên danh sách ID thi giữa kỳ & cuối kỳ dưới dạng mảng (backend cần List<Guid>)
            const midGuidList = toGuidArray(formData.value.midExamIds)
            const finalGuidList = toGuidArray(formData.value.finalExamIds)
            const midIds = midGuidList.join(',')
            const finalIds = finalGuidList.join(',')

            // 🧩 2. Xử lý danh sách học phí (Tuition)
            let tuitions = (formData.value.tuitions ?? []).map(t => {
                const isGuid =
                    typeof t.id === 'string' && t.id.length === 36 // GUID thật
                return {
                    ...t,
                    id: isGuid ? t.id : null, // nếu là randomUUID -> để null để backend hiểu là Add
                    courseId:
                        formData.value.id && formData.value.id !== ''
                            ? formData.value.id
                            : null, // null thật, không chuỗi rỗng
                    tuitionCode: t.tuitionCode?.trim() || '',
                    tuitionName: t.tuitionName?.trim() || '',
                    classTypeId: t.classTypeId || '',
                    durationHours: Number(t.durationHours) || 0,
                    minHours: Number(t.minHours) || 0,
                    totalMonths: Number(t.totalMonths) || 0,
                    unit: t.unit ?? 'Hour',
                    tuitionFee: Number(t.tuitionFee) || 0,
                    courseLessons: (t.courseLessons ?? []).map(lesson => ({
                        ...lesson,
                        // // Backend expects Guid: null => Guid.Empty (new), valid Guid stays, array -> first element
                        // id: typeof lesson.id === 'string' && lesson.id.length === 36 ? lesson.id : null,
                        // lectureTypeId: toGuidArray(normalizeLectureTypeId(lesson.lectureTypeId))[0] || null
                    }))
                }
            })

            const deletedTuitionIds = (removedTuitionIds?.value ?? [])


            const courseModel: any = {
                ...formData.value,
                midExamIds: midIds,
                finalExamIds: finalIds,
                tuitions,
                deletedTuitionIds, // thêm nếu có
            }

            const commitmentType = courseModel.commitmentOutputType ?? CommitmentOutputType.None
            courseModel.isCommitmentBased = commitmentType !== CommitmentOutputType.None
            courseModel.commitmentOutput = commitmentType === CommitmentOutputType.SelfCommitment
            courseModel.commitmentLevel = commitmentType === CommitmentOutputType.SelfCommitment
                ? String(courseModel.commitmentLevel ?? '').trim()
                : (String(courseModel.commitmentLevel ?? '').trim() || null)

            courseModel.courseCode = String(courseModel.courseCode ?? '').trim()
            courseModel.courseName = String(courseModel.courseName ?? '').trim()
            delete (courseModel as any).learningRoadMap
            console.log(courseModel);

            emit('submit', courseModel)
        } finally {
            submitting.value = false
        }
    })
}

function onDelete() {
    emit('delete', formData.value)
}
</script>
<style>
.select-multi-fulltags .el-select__tags {
    flex-wrap: wrap;
    /* cho phép xuống dòng */
    max-height: none;
    /* bỏ giới hạn chiều cao */
}

.select-multi-fulltags .el-select__input {
    flex-basis: 120px;
    /* để input tìm kiếm không bóp méo tags, tùy chỉnh */
}

.select-multi-fulltags .el-select__wrapper {
    min-height: 40px;
    /* giữ chiều cao đẹp khi ít tag */
    align-items: flex-start;
    /* canh trên khi nhiều dòng tags */
    padding-top: 6px;
    padding-bottom: 6px;
}
</style>
