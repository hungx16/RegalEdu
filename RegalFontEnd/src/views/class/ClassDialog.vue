<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        width="90vw" :mode="mode" :show-action-buttons="showActionButtons" @submit="onSubmit" @delete="onDelete" height="40vw"
        @update:visible="emit('update:visible', $event)">
        <template #form>
            <TabbedComponent v-model="activeTab" :tabs="tabs" />

            <div v-if="activeTab === 'info'">
                <el-row :gutter="20" class="form-group-block">
                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.code')" prop="classCode">
                            <el-input v-model="formData.classCode"
                                placeholder="Auto generated after selecting company & course" disabled />
                        </el-form-item>
                    </el-col>

                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.name')" prop="className" required>
                            <el-input v-model="formData.className" :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.status')" prop="classStatus">
                            <el-select v-model="formData.classStatus" :disabled="isView" clearable>
                                <el-option v-for="status in classStatusOptions" :key="status.value"
                                    :label="status.label" :value="status.value" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.company')" prop="companyId" required>
                            <el-select v-model="formData.companyId" filterable placeholder="Select company"
                                :disabled="isView">
                                <el-option v-for="c in companyOptions" :key="c.value" :label="c.label"
                                    :value="c.value" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.course')" prop="courseId" required>
                            <el-select v-model="formData.courseId" filterable placeholder="Select course"
                                :disabled="isView">
                                <el-option v-for="c in courseOptions" :key="c.value" :label="c.label"
                                    :value="c.value" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.classType')" prop="classTypeId" required>
                            <el-select v-model="formData.classTypeId" filterable placeholder="Select class type"
                                :disabled="isView">
                                <el-option v-for="ct in classTypeOptions" :key="ct.value" :label="ct.label"
                                    :value="ct.value" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.teacher')" prop="teacherId">
                            <el-select v-model="formData.teacherId" filterable clearable placeholder="Select teacher"
                                :disabled="isView">
                                <el-option v-for="t in teacherOptions" :key="t.value" :label="t.label"
                                    :value="t.value" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.employee')" prop="employeeId">
                            <el-select v-model="formData.employeeId" filterable clearable placeholder="Select PIC"
                                :disabled="isView">
                                <el-option v-for="e in employeeOptions" :key="e.value" :label="e.label"
                                    :value="e.value" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.method')" prop="method">
                            <el-radio-group v-model="formData.method" :disabled="isView">
                                <el-radio :value="0">{{ t('class.onsite') }}</el-radio>
                                <el-radio :value="1">{{ t('class.online') }}</el-radio>
                            </el-radio-group>
                        </el-form-item>
                    </el-col>

                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.startDate')" prop="startDate" required>
                            <el-date-picker v-model="formData.startDate" type="date" value-format="YYYY-MM-DD"
                                :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.endDate')" prop="endDate">
                            <el-date-picker v-model="formData.endDate" type="date" value-format="YYYY-MM-DD"
                                :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <el-col :span="24">
                        <el-form-item label-position="top" :label="t('class.schedule')" prop="classSchedule">
                            <el-select v-model="selectedWorkingTimeIds" multiple filterable clearable collapse-tags
                                :disabled="isView">
                                <el-option v-for="wt in workingTimeOptions" :key="wt.value" :label="wt.label"
                                    :value="wt.value" />
                            </el-select>
                        </el-form-item>
                    </el-col>

                    <el-col :xs="24" :md="12" :lg="8">
                        <el-form-item label-position="top" :label="t('class.trialClass')" prop="trialClass">
                            <el-switch v-model="formData.trialClass" :disabled="isView" />
                        </el-form-item>
                    </el-col>

                    <el-col :span="24">
                        <el-form-item label-position="top" :label="t('class.description')" prop="description">
                            <el-input type="textarea" v-model="formData.description" :disabled="isView" :rows="3" />
                        </el-form-item>
                    </el-col>
                </el-row>
            </div>

            <div v-else-if="activeTab === 'students'">
                <ClassStudentList :class-id="formData.id || ''" :active="activeTab === 'students'" />
            </div>

            <div v-else-if="activeTab === 'sessions'">
                <ClassScheduleList :class-id="formData.id || ''" :active="activeTab === 'sessions'" />
            </div>

            <div v-else-if="activeTab === 'attendance'">
                <ClassAttendanceList :class-id="formData.id || ''" :active="activeTab === 'attendance'" />
            </div>

            <div v-else-if="activeTab === 'scores'">
                <ClassScoreBoardList :class-id="formData.id || ''" :course-id="formData.courseId || ''"
                    :active="activeTab === 'scores'" />
            </div>

            <div v-else-if="activeTab === 'notes'">
                <ClassRemarksList :class-id="formData.id || ''" :active="activeTab === 'notes'" />
            </div>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import TabbedComponent from '@/components/tabbed/TabbedComponent.vue'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { ClassModel } from '@/api/ClassApi'
import { useCourseStore } from '@/stores/courseStore'
import { useCompanyStore } from '@/stores/companyStore'
import { useClassTypeStore } from '@/stores/classTypeStore'
import { useTeacherStore } from '@/stores/teacherStore'
import { useEmployeeStore } from '@/stores/employeeStore'
import { useWorkingTimeStore } from '@/stores/workingTimeStore'
import { useCommonStore } from '@/stores/commonStore'
import { getDayOfWeekKey } from '@/types/daysOfWeek'
import { getClassStatusOptions } from '@/utils/makeList'
import ClassStudentList from './ClassStudentList.vue'
import ClassScheduleList from './ClassScheduleList.vue'
import ClassAttendanceList from './ClassAttendanceList.vue'
import ClassRemarksList from './ClassRemarksList.vue'
import ClassScoreBoardList from '@/views/class/ClassScoreBoardList.vue'

const props = defineProps<{ visible: boolean; mode?: 'create' | 'edit' | 'view'; classData: Partial<ClassModel> | null }>()
const emit = defineEmits(['update:visible', 'submit', 'delete'])
const { t } = useI18n()
const SPLIT_TAG = import.meta.env.VITE_SPLIT_TAG || '#$#'

const tabs = computed(() => [
    { name: 'info', label: t('class.infoTab') },
    { name: 'students', label: t('class.studentsTab') },
    { name: 'sessions', label: t('class.sessionsTab') },
    { name: 'attendance', label: t('class.attendanceTab') },
    { name: 'scores', label: t('class.scoresTab') },
    { name: 'notes', label: t('class.notesTab') }
])
const activeTab = ref('info')

const initialFormState = () => ({
    classCode: '',
    className: '',
    companyId: '',
    courseId: '',
    classTypeId: '',
    method: 0,
    startDate: '',
    endDate: null,
    description: '',
    trialClass: false,
    classStatus: 0,
    teacherId: '',
    employeeId: '',
    classSchedule: ''
})

const formData = ref<Partial<ClassModel>>(initialFormState())
const selectedWorkingTimeIds = ref<string[]>([])

watch(() => props.classData, (data) => {
    if (data) {
        formData.value = {
            ...initialFormState(),
            ...data,
        }
        selectedWorkingTimeIds.value = data.classSchedule ? data.classSchedule.split(SPLIT_TAG) : []
    } else {
        formData.value = initialFormState()
        selectedWorkingTimeIds.value = []
    }
}, { immediate: true })

watch(() => props.visible, (visible) => {
    if (visible) {
        activeTab.value = 'info'
    }
})

const companyStore = useCompanyStore()
const courseStore = useCourseStore()
const classTypeStore = useClassTypeStore()
const teacherStore = useTeacherStore()
const employeeStore = useEmployeeStore()
const workingTimeStore = useWorkingTimeStore()
const commonStore = useCommonStore()

const companyOptions = computed(() => companyStore.companies.map(c => ({ label: c.companyName, value: c.id })))
const courseOptions = computed(() => courseStore.courses.map(c => ({ label: c.courseName, value: c.id })))
const classTypeOptions = computed(() => classTypeStore.classTypes.map(ct => ({ label: ct.classTypeName, value: ct.id })))
const teacherOptions = computed(() => teacherStore.teachers.map(t => ({
    label: t.applicationUser?.fullName || '',
    value: t.id
})))
const employeeOptions = computed(() => employeeStore.employees.map(e => ({
    label: e.applicationUser?.fullName || '',
    value: e.id
})))
const classStatusOptions = computed(() => getClassStatusOptions(t))

watch(
    [() => formData.value.companyId, () => formData.value.courseId],
    async ([newCompanyId, newCourseId]) => {
        if (newCompanyId && newCourseId) {
            const company = companyStore.companies.find(c => c.id === newCompanyId)
            const course = courseStore.courses.find(c => c.id === newCourseId)
            if (company && course) {
                try {
                    await commonStore.generateCode(`${company.companyCode}_${course.courseCode}_`, 'Class', 'ClassCode', 4)
                    formData.value.classCode = commonStore.code
                } catch (err) {
                    console.error('Error generating class code:', err)
                }
            }
        }
    }
)

function formatTime(time: string) {
    return time ? time.substring(0, 5) : ''
}

const workingTimeOptions = computed(() =>
    workingTimeStore.workingTimes
        .filter(w => w.isWorkingDay)
        .map(w => ({
            label: `${t(getDayOfWeekKey(w.dayOfWeek))} - ${formatTime(w.startTime)} to ${formatTime(w.endTime)}`,
            value: w.id
        }))
)

watch(selectedWorkingTimeIds, (val) => {
    formData.value.classSchedule = val.join(SPLIT_TAG)
})

const rules = {
    className: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    companyId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    courseId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    classTypeId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    startDate: [{ required: true, message: t('validation.required'), trigger: 'change' }]
}

const isView = computed(() => props.mode === 'view')
const showActionButtons = computed(() => props.mode !== 'edit' || activeTab.value === 'info')
const modeTitle = computed(() =>
    props.mode === 'edit' ? t('class.editTitle') : props.mode === 'view' ? t('class.detailTitle') : t('class.addTitle')
)

onMounted(async () => {
    await Promise.all([
        companyStore.fetchAllCompanies(),
        courseStore.fetchAllCourses(),
        classTypeStore.fetchAllClassTypes(),
        teacherStore.fetchAllTeacher(),
        employeeStore.fetchAllEmployees(),
        workingTimeStore.fetchAllWorkingTimes(),
        teacherStore.checkIsCurrentUserTeacher()
    ])
})

function onSubmit() {
    emit('submit', formData.value)
}
function onDelete() {
    emit('delete', formData.value)
}
</script>

<style scoped>
.form-group-block {
    padding-top: 0.25rem;
}

.tab-placeholder {
    padding: 3rem 0;
    text-align: center;
    color: #8f8f8f;
    font-size: 0.95rem;
}
</style>
