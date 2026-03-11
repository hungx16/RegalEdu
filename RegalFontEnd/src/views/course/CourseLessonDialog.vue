<template>
    <BaseDialogForm :visible="visible" :title="dialogTitle" :loading="loading" :rules="rules" :form-data="{ lessons }"
        width="800px" ref="baseCourseLessonDialogRef" @update:visible="emit('update:visible', $event)"
        @submit="handleBaseSubmit" @confirm="handleBaseSubmit">
        <template #form>
            <div class="p-4 bg-light rounded-4 shadow-sm">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <div class="fw-semibold">
                        {{ t('courseLesson.toolbar.title') }}
                    </div>
                    <el-button type="primary" size="small" @click="addLesson">
                        <i class="bi bi-plus-circle me-2"></i>{{ t('courseLesson.actions.addSession') }}
                    </el-button>
                </div>

                <el-collapse v-model="activeLesson" accordion class="lesson-list">
                    <el-collapse-item v-for="(lesson, index) in lessons"
                        :key="getLessonKey(lesson, index)"
                        :name="getLessonKey(lesson, index)"
                        class="lesson-collapse-item mb-3">
                        <template #title>
                            <div class="lesson-accordion-title d-flex justify-content-between align-items-center w-100">
                                <div class="fw-semibold">
                                    {{ t('courseLesson.sessionLabel', { index: String(index + 1).padStart(2, '0') }) }}
                                </div>
                                <el-popconfirm :title="t('courseLesson.confirm.deleteOne')"
                                    :confirm-button-text="t('common.ok')" :cancel-button-text="t('common.cancel')"
                                    @confirm="removeLesson(index)">
                                    <template #reference>
                                        <el-button type="danger" size="small" plain @click.stop>
                                            {{ t('courseLesson.actions.delete') }}
                                        </el-button>
                                    </template>
                                </el-popconfirm>
                            </div>
                        </template>

                        <div class="lesson-item p-3 border rounded-3"
                            :class="{ 'is-odd': index % 2 === 0, 'is-even': index % 2 === 1 }">
                            <div class="row g-2">
                                <div class="col-12 col-md-4">
                                    <div class="text-muted small mb-1">{{ t('courseLesson.columns.lectureType') }}</div>
                                    <el-select v-model="lesson.lectureTypeId"
                                        :placeholder="t('courseLesson.placeholders.lectureType')" filterable clearable
                                        style="width: 100%">
                                        <el-option v-for="lt in lectureTypeStore.lectureTypes" :key="lt.id"
                                            :label="lt.lectureName" :value="lt.id" />
                                    </el-select>
                                </div>

                                <div class="col-12 col-md-8">
                                    <div class="text-muted small mb-1">{{ t('courseLesson.columns.lessonName') }}</div>
                                    <el-input v-model="lesson.lessonName"
                                        :placeholder="t('courseLesson.placeholders.lessonName')" />
                                </div>

                                <div class="col-12 col-md-6">
                                    <div class="text-muted small mb-1">{{ t('courseLesson.columns.objective') }}</div>
                                    <el-input v-model="lesson.objective" type="textarea" :rows="3"
                                        :placeholder="t('courseLesson.placeholders.objective')" />
                                </div>

                                <div class="col-12 col-md-6">
                                    <div class="text-muted small mb-1">{{ t('courseLesson.columns.content') }}</div>
                                    <el-input v-model="lesson.content" type="textarea" :rows="3"
                                        :placeholder="t('courseLesson.placeholders.content')" />
                                </div>

                                <div class="col-12 col-md-6">
                                    <div class="text-muted small mb-1">{{ t('courseLesson.columns.homework') }}</div>
                                    <el-input v-model="lesson.homework" type="textarea" :rows="3"
                                        :placeholder="t('courseLesson.placeholders.homework')" />
                                </div>

                                <div class="col-12 col-md-6">
                                    <div class="text-muted small mb-1">{{ t('courseLesson.columns.reference') }}</div>
                                    <el-input v-model="lesson.reference" type="textarea" :rows="3"
                                        :placeholder="t('courseLesson.placeholders.reference')" />
                                </div>

                                <div class="col-12">
                                    <div class="text-muted small mb-1">
                                        {{ t('courseLesson.columns.homeworkAttachments') }}
                                    </div>
                                    <FileManager ref="homeworkFileRefs" v-model="lesson.homeworkAttachments"
                                        :fields="attachmentFields" :multiple="true" :enableDownload="true" />
                                </div>

                                <div class="col-12">
                                    <div class="text-muted small mb-1">
                                        {{ t('courseLesson.columns.referenceAttachments') }}
                                    </div>
                                    <FileManager ref="referenceFileRefs" v-model="lesson.referenceAttachments"
                                        :fields="attachmentFields" :multiple="true" :enableDownload="true" />
                                </div>
                            </div>
                        </div>
                    </el-collapse-item>
                </el-collapse>

            </div>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, ref, watch, onMounted, onBeforeUpdate } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { CourseLessonModel } from '@/api/CourseApi'
import type { TuitionModel } from '@/api/TuitionApi'
import type { FieldSchema } from '@/api/FileApi'
import { useClassTypeStore } from '@/stores/classTypeStore'
import { useLectureTypeStore } from '@/stores/lectureTypeStore'
import FileManager from '@/components/file-manager/FileManager.vue'

const { t } = useI18n()
const classTypeStore = useClassTypeStore()
const lectureTypeStore = useLectureTypeStore()
const baseCourseLessonDialogRef = ref()

const props = defineProps<{
    visible: boolean
    tuition: TuitionModel | null
}>()

const emit = defineEmits(['update:visible', 'submit'])

const dialogTitle = computed(() =>
    props.tuition
        ? t('courseLesson.dialog.titleWithTuition', { tuitionName: props.tuition.tuitionName })
        : t('courseLesson.dialog.titleCreate')
)

const loading = ref(false)
const lessons = ref<CourseLessonModel[]>([])
const activeLesson = ref('')
const homeworkFileRefs = ref<any[]>([])
const referenceFileRefs = ref<any[]>([])
const attachmentFields: FieldSchema[] = []

onBeforeUpdate(() => {
    homeworkFileRefs.value = []
    referenceFileRefs.value = []
})

function normalizeAttachments(list?: any[]) {
    return (list || []).map((a: any) => ({
        ...a,
        uid: a.uid || a.id || crypto.randomUUID(),
        fileName: a.fileName || a.name || a.filePath || a.path,
        path: a.path || a.filePath || a.relativePath,
        filePath: a.filePath || a.path || a.relativePath,
        relativePath: a.relativePath || a.filePath || a.path
    }))
}

function renumberSessions() {
    lessons.value.forEach((ls, idx) => {
        const order = String(idx + 1).padStart(2, '0')
        ls.sessionName = t('courseLesson.sessionLabel', { index: order })
    })
}

function getLessonKey(lesson: CourseLessonModel, index: number) {
    return String(lesson.id ?? lesson.sessionName ?? index)
}

function setActiveLessonByIndex(index: number) {
    if (!lessons.value.length) {
        activeLesson.value = ''
        return
    }
    const safeIndex = Math.min(Math.max(index, 0), lessons.value.length - 1)
    activeLesson.value = getLessonKey(lessons.value[safeIndex], safeIndex)
}

function ensureActiveLesson() {
    if (!lessons.value.length) {
        activeLesson.value = ''
        return
    }
    const exists = lessons.value.some((lesson, idx) => getLessonKey(lesson, idx) === activeLesson.value)
    if (!exists) setActiveLessonByIndex(0)
}

function addLesson() {
    const nextIndex = lessons.value.length + 1
    lessons.value.push({
        id: crypto.randomUUID(),
        lectureTypeId: '',
        lessonName: '',
        objective: '',
        content: '',
        homework: '',
        reference: '',
        homeworkAttachments: [],
        referenceAttachments: [],
        sessionName: t('courseLesson.sessionLabel', { index: String(nextIndex).padStart(2, '0') })
    })
    renumberSessions()
    setActiveLessonByIndex(lessons.value.length - 1)
}

function removeLesson(rowIndex: number) {
    if (rowIndex < 0 || rowIndex >= lessons.value.length) return
    const removedKey = getLessonKey(lessons.value[rowIndex], rowIndex)
    lessons.value.splice(rowIndex, 1)
    renumberSessions()
    if (activeLesson.value === removedKey) {
        setActiveLessonByIndex(rowIndex)
    } else {
        ensureActiveLesson()
    }
}

watch(
    () => props.tuition,
    async (tuition) => {
        if (!tuition) return

        // Nếu có sẵn lessons thì load lại để chỉnh sửa
        if (tuition.courseLessons && tuition.courseLessons.length > 0) {
            lessons.value = tuition.courseLessons.map(ls => ({
                ...ls,
                homeworkAttachments: normalizeAttachments(ls.homeworkAttachments),
                referenceAttachments: normalizeAttachments(ls.referenceAttachments)
            }))
            renumberSessions()
            setActiveLessonByIndex(0)
            return
        }

        // Tạo mới danh sách buổi học nếu chưa có
        const cls = classTypeStore.classTypes.find(c => c.id === tuition.classTypeId)
        const hoursPerSession = cls?.hoursPerSession || 1
        const sessionCount = Math.floor((tuition.durationHours ?? 0) / hoursPerSession)

        const list: CourseLessonModel[] = []
        for (let i = 1; i <= sessionCount; i++) {
            list.push({
                //id: crypto.randomUUID(),
                // tuitionId: tuition.id ?? '',
                sessionName: t('courseLesson.sessionLabel', { index: String(i).padStart(2, '0') }),
                lectureTypeId: '',
                lessonName: '',
                objective: '',
                content: '',
                homework: '',
                reference: '',
                homeworkAttachments: [],
                referenceAttachments: []
            })
        }

        lessons.value = list
        setActiveLessonByIndex(0)
    },
    { immediate: true }
)

// 🧩 Tải dữ liệu loại bài giảng
onMounted(async () => {
    if (!lectureTypeStore.lectureTypes.length) {
        await lectureTypeStore.fetchAllLectureTypes()
    }
})

const rules = {
    lessons: [{ required: true, message: t('validation.required'), trigger: 'blur' }]
}

async function syncLessonAttachments() {
    const managers = [
        ...(homeworkFileRefs.value || []),
        ...(referenceFileRefs.value || [])
    ].filter(Boolean)

    for (const mgr of managers) {
        await mgr?.uploadPendingFiles?.()
    }

    lessons.value.forEach((lesson, idx) => {
        const homeworkMgr = homeworkFileRefs.value?.[idx]
        const referenceMgr = referenceFileRefs.value?.[idx]
        if (homeworkMgr?.packAttachments) {
            lesson.homeworkAttachments = normalizeAttachments(homeworkMgr.packAttachments())
        }
        if (referenceMgr?.packAttachments) {
            lesson.referenceAttachments = normalizeAttachments(referenceMgr.packAttachments())
        }
    })
}

async function handleBaseSubmit() {
    try {
        loading.value = true
        await syncLessonAttachments()
        emit('submit', lessons.value)
        emit('update:visible', false)
    } catch (error) {
        console.error('❌ Form validation failed:', error)
    } finally {
        loading.value = false
    }
}
</script>

<style scoped>
.lesson-list {
    max-height: 450px;
    overflow-y: auto;
}

.lesson-list :deep(.el-collapse-item__header) {
    padding: 0;
    border: none;
    border-radius: 0.5rem;
    box-shadow: none;
}

.lesson-list :deep(.el-collapse-item__content) {
    padding: 0 0 1rem;
}

.lesson-list :deep(.el-collapse-item__wrap) {
    border-bottom: none;
}

.lesson-list :deep(.el-collapse-item__header:hover),
.lesson-list :deep(.el-collapse-item__header:focus),
.lesson-list :deep(.el-collapse-item__header:focus-visible) {
    outline: none;
    box-shadow: none;
}

.lesson-accordion-title {
    background: #e7f1ff;
    border: 1px solid #d6e6ff;
    border-radius: 0.5rem;
    padding: 0.5rem 0.75rem;
}

.lesson-item.is-odd {
    background: #ffffff;
}

.lesson-item.is-even {
    background: #f1f5f9;
}

.lesson-collapse-item:last-child {
    margin-bottom: 0;
}
</style>
