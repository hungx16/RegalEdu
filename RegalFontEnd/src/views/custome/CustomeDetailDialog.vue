<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="loading" @submit="onSubmit" @delete="onDelete" :width="computedDialogWidth"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-tabs v-model="activeTab" class="detail-tabs">
                <el-tab-pane :label="t('custome.basicInfoTab')" name="basic">
                    <DetailTab_BasicInfo :formData="formData" :t="t" />
                </el-tab-pane>

                <el-tab-pane :label="t('custome.courseInterestTab')" name="courses">
                    <DetailTab_CourseInterest :courses="formData?.studentCourse || []" :t="t"
                        @update:courseInterest="formData.studentCourse = $event" />
                </el-tab-pane>

                <el-tab-pane :label="t('custome.activityLogTab')" name="activities">
                    <DetailTab_ActivityLog :activities="formData?.studentActivity || []" :t="t"
                        @update:activities="formData.studentActivity = $event" />
                </el-tab-pane>

                <el-tab-pane :label="t('custome.notesTab')" name="notes">
                    <DetailTab_Notes :notes="formData?.studentNote || []" :t="t"
                        @update:notes="formData.studentNote = $event" />
                </el-tab-pane>
            </el-tabs>

            <!-- <el-row v-if="activeTab === 'basic'">
                <el-col :span="24">
                    <label class="d-none required"></label>
                    <el-form-item prop="fullName" class="d-none" />
                    <el-form-item prop="phone" class="d-none" />
                </el-col>
            </el-row> -->
        </template>

    </BaseDialogForm>
</template>

<script setup lang="ts">

import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { StudentModel, ContactModel } from '@/api/StudentApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useStudentStore } from '@/stores/studentStore'
import StudentBasicInfoTab from './StudentBasicInfoTab.vue' // Component Tab 1
import StudentContactsTab from './StudentContactsTab.vue'   // Component Tab 2
import DetailTab_BasicInfo from './detailTabs/DetailTab_BasicInfo.vue';
import DetailTab_CourseInterest from './detailTabs/DetailTab_CourseInterest.vue';
import DetailTab_ActivityLog from './detailTabs/DetailTab_ActivityLog.vue';
import DetailTab_Notes from './detailTabs/DetailTab_Notes.vue';
const windowWidth = ref(window.innerWidth);
const hasUnsavedChanges = ref(false);
const computedDialogWidth = computed(() => windowWidth.value < 700 ? '100%' : '80%');
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    studentData: Partial<StudentModel & { priority: number, expectedBudget: number }> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const studentStore = useStudentStore()
const notificationStore = useNotificationStore()

const activeTab = ref('basic'); // Mặc định mở tab Thông tin cơ bản
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')

const modeTitle = computed(() => {
    if (isView.value) return t('custome.detailTitle')
    if (isEdit.value) return t('custome.editTitle')
    return t('custome.detailTitle')
})

const baseDialogRef = ref()
const loading = ref(false)

const defaultFormData: Partial<StudentModel & { priority: number, expectedBudget: number }> = {
    studentCode: '',
    fullName: '',
    phone: '',
    gender: 'Nam',
    leadSource: 'Website',
    employeeId: null,
    expectedBudget: 0,
    priority: 1,
    englishName: 'None',
    contacts: [], // Đảm bảo contacts luôn là mảng
}
const formData = ref<Partial<StudentModel & { priority: number, expectedBudget: number }>>({ ...defaultFormData })
//const student = computed<StudentModel | null>(() => studentStore.selectedStudent);
watch(
    () => props.studentData,
    (data) => {
        if (data && data.id) {
            // Đảm bảo contacts là mảng khi load data
            formData.value = {
                ...data,
                contacts: data.contacts || [],
                priority: 1,
                expectedBudget: 0,
                englishName: data.englishName || 'None',
            } as any;
        } else {
            formData.value = { ...defaultFormData }
        }
        activeTab.value = 'basic'; // Reset về tab đầu tiên
    },
    { immediate: true }
)

const rules = {
    // studentCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    // fullName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    // phone: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    // leadSource: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    // employeeId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
}

// // Logic thêm Contact mới
// function addEmptyContact() {
//     if (!formData.value.contacts) {
//         formData.value.contacts = [];
//     }
//     const newContact: Partial<ContactModel> = {
//         fullName: 'Người liên hệ mới',
//         relationship: 0, // Cha
//         phone: '',
//         email: '',
//         gender: 0, // Nam
//     };
//     formData.value.contacts.push(newContact as ContactModel);
// }

// --- Logic CRUD/Validation ---
function closeModal() {
    emit('update:visible', false)
    emit('close')
}

function onSubmit() {
    const form = baseDialogRef.value?.formRef


    form.validate(async (valid: boolean) => {
        if (valid) {
            loading.value = true
            console.log('Submitting form data:', formData.value);
            // Cần validate cả dữ liệu trong tab Contacts nếu cần
            emit('submit', formData.value)
            loading.value = false
        } else {
            notificationStore.showToast('error', { key: 'validation.formInvalid' })
            activeTab.value = 'basic'; // Chuyển về tab có lỗi (nếu lỗi là ở tab basic)
        }
    })
}
function onDelete() {
    emit('delete', formData.value)
}
</script>
<style>
/* Điều chỉnh kiểu cho dialog lớn */
.student-detail-dialog .el-dialog__body {
    padding-top: 10px !important;
}
</style>