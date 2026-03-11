<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="t('classSchedule.editTitle')" :mode="'edit'"
        :show-delete="false" :loading="loading || saving" :submit-disabled="saving" :form-ref="formRef"
        :form-data="form" :rules="rules" width="880px" @update:visible="emit('update:visible', $event)"
        @submit="onSubmit" @close="handleClose">
        <template #form>
            <el-row :gutter="16">
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('classSchedule.lessonType') }}</label>
                    <el-form-item>
                        <el-input v-model="readonlyFields.lessonType" disabled />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('classSchedule.lessonName') }}</label>
                    <el-form-item>
                        <el-input v-model="readonlyFields.lessonName" disabled />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('classSchedule.date') }}</label>
                    <el-form-item prop="date">
                        <el-date-picker v-model="form.date" disabled type="date" value-format="YYYY-MM-DD"
                            format="DD/MM/YYYY" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('classSchedule.timeRange') }}</label>
                    <el-form-item prop="timeRange">
                        <div class="time-range-wrap">
                            <el-time-picker v-model="form.startTime" format="HH:mm" value-format="HH:mm:ss" disabled
                                :placeholder="t('classSchedule.startTime')" />
                            <span class="time-sep">-</span>
                            <el-time-picker v-model="form.endTime" format="HH:mm" value-format="HH:mm:ss" disabled
                                :placeholder="t('classSchedule.endTime')" />
                        </div>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('classSchedule.classStatus') }}</label>
                    <el-form-item prop="classScheduleStatus">
                        <el-select v-model="form.classScheduleStatus" disabled>
                            <el-option v-for="opt in classStatusOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('classSchedule.attendanceStatus')
                        }}</label>
                    <el-form-item prop="sessionAttendanceStatus">
                        <el-select v-model="form.sessionAttendanceStatus" disabled>
                            <el-option v-for="opt in attendanceOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('classSchedule.plan') }}</label>
                    <el-form-item>
                        <el-input type="textarea" :rows="2" v-model="form.plan" />
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('classSchedule.homeworkExtra') }}</label>
                    <el-form-item>
                        <el-input type="textarea" :rows="4" v-model="form.homeworkPlusContent" />
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('lectureType.fileAttached') }}</label>
                    <FileUpload ref="fileRef" :file-url="form.attachment?.path" :key="fileUploadKey"
                        :original-file-name="form.attachment?.fileName" :allowed-groups="['document']"
                        @file-change="onFileChange" />
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { ClassScheduleModel } from '@/api/ClassScheduleApi'
import { ClassScheduleStatus, SessionAttendanceStatus } from '@/types'
import FileUpload from '@/components/file-upload/FileUpload.vue'
import { useClassScheduleStore } from '@/stores/classScheduleStore'
import { storeToRefs } from 'pinia'

const props = defineProps<{ visible: boolean; scheduleId: string | null }>()
const emit = defineEmits(['update:visible', 'saved'])
const { t } = useI18n()
const classScheduleStore = useClassScheduleStore()
const { selectedSchedule, detailLoading } = storeToRefs(classScheduleStore)
const loading = detailLoading
const saving = ref(false)
const baseDialogRef = ref()
const formRef = ref()
const fileRef = ref<InstanceType<typeof FileUpload> | null>(null)
const fileUploadKey = ref(0)
const tf = (key: string, fallback: string) => {
    const v = t(key)
    return v === key ? fallback : v
}
const form = reactive({
    id: '',
    date: '',
    startTime: '',
    endTime: '',
    content: '',
    homeworkPlusContent: '',
    attachment: null as any,
    plan: '',
    attender: '',
    classScheduleStatus: ClassScheduleStatus.NotStarted,
    sessionAttendanceStatus: SessionAttendanceStatus.NotChecked
})

const readonlyFields = computed(() => ({
    lessonType: selectedSchedule.value?.courseLesson?.lectureType?.lectureName ||
        (selectedSchedule.value?.courseLesson?.lectureType as any)?.LectureName ||
        '',
    lessonName: selectedSchedule.value?.courseLesson?.lessonName ||
        (selectedSchedule.value?.courseLesson as any)?.LessonName ||
        selectedSchedule.value?.courseLesson?.sessionName ||
        (selectedSchedule.value?.courseLesson as any)?.SessionName ||
        ''
}))

const classStatusOptions = [
    { value: ClassScheduleStatus.NotStarted, label: t('classSchedule.notStarted') },
    { value: ClassScheduleStatus.Completed, label: t('classStatus.completed') },
    { value: ClassScheduleStatus.Cancelled, label: t('classStatus.cancelled') }
]

const attendanceOptions = [
    { value: SessionAttendanceStatus.NotChecked, label: t('classSchedule.attendanceNotChecked') },
    { value: SessionAttendanceStatus.Checked, label: t('classSchedule.attendanceChecked') },
    { value: SessionAttendanceStatus.Confirmed, label: t('classSchedule.attendanceConfirmed') }
]

// Cho phép lưu khi chỉ nhập các trường plan/homework/attender, bỏ bắt buộc các field khác
const rules = {}

watch(
    () => [props.visible, props.scheduleId],
    async ([visible, id]) => {
        if (visible && typeof id === 'string') {
            await fetchDetail(id)
        } else if (!visible) {
            resetForm()
        }
    },
    { immediate: true }
)

async function fetchDetail(id: string) {
    try {
        const res = await classScheduleStore.fetchById(id)
        if (res?.succeeded && res.data) {
            populateForm(res.data)
        } else {
            resetForm()
        }
    } catch (err) {
        console.error('Failed to load schedule detail', err)
        resetForm()
    }
}

function populateForm(data: ClassScheduleModel) {
    form.id = data.id || ''
    form.date = data.date || ''
    form.startTime = data.startTime || ''
    form.endTime = data.endTime || ''
    form.content = data.content || data.courseLesson?.content || ''
    form.homeworkPlusContent = data.homeworkPlusContent || ''
    form.plan = data.plan || ''
    form.attender = data.attender || ''
    form.classScheduleStatus = data.classScheduleStatus
    form.sessionAttendanceStatus = data.sessionAttendanceStatus
    form.attachment = data.attachment
}

function resetForm() {
    form.id = ''
    form.date = ''
    form.startTime = ''
    form.endTime = ''
    form.content = ''
    form.homeworkPlusContent = ''
    form.plan = ''
    form.attender = ''
    form.classScheduleStatus = ClassScheduleStatus.NotStarted
    form.sessionAttendanceStatus = SessionAttendanceStatus.NotChecked
    classScheduleStore.resetSelected()
}
function onFileChange(file: any) {
    form.attachment = file
}
function handleClose() {
    resetForm()
    emit('update:visible', false)
}

async function onSubmit() {
    saving.value = true
    try {
        type UploadRef = {
            uploadPending?: () => Promise<{ relativePath: string; fileName: string } | null>
            uploadPendingToTemp?: () => Promise<{ relativePath: string; fileName: string } | null>
            getCurrent?: () => { fileUrl: string | null; originalFileName: string | null; markedForDelete: boolean }
            getOriginalName?: () => string | null
            getUploadedOriginalName?: () => string | null
        }
        const fileUploadRef = fileRef.value as UploadRef | undefined
        const fileUp = await (fileUploadRef?.uploadPending?.() ?? fileUploadRef?.uploadPendingToTemp?.() ?? Promise.resolve(null))
        const fileState = fileUploadRef?.getCurrent?.()

        const payload: Partial<ClassScheduleModel> = {
            id: form.id,
            date: form.date,
            startTime: form.startTime,
            endTime: form.endTime,
            content: form.content,
            plan: form.plan,
            attender: form.attender,
            classScheduleStatus: form.classScheduleStatus,
            sessionAttendanceStatus: form.sessionAttendanceStatus,
            homeworkPlusContent: form.homeworkPlusContent,
            attachment: form.attachment
        }
        delete payload.attender // không cho phép cập nhật attender từ đây
        if (fileUp) {
            payload.attachment = {
                path: fileUp.relativePath,
                fileName: fileUploadRef?.getUploadedOriginalName?.() ?? fileUp.fileName,
            }
        } else if (fileState?.markedForDelete) {
            payload.attachment = { path: '', fileName: '' }
        }
        const res = await classScheduleStore.updateSchedule(payload)
        emit('saved', res?.data || payload)
        emit('update:visible', false)
    } catch (err) {
        console.error('Failed to update schedule', err)
    } finally {
        saving.value = false
    }
}

</script>

<style scoped>
.time-range-wrap {
    display: flex;
    align-items: center;
    gap: 8px;
}

.time-sep {
    color: #9ca3af;
}

.loading-wrap {
    min-height: 200px;
}
</style>
