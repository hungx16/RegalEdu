<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="loading" @submit="onSubmit" @delete="onDelete" :width="computedDialogWidth"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-tabs v-model="activeTab" type="card">
                <el-tab-pane :label="t('student.basicInfoTab')" name="basic">
                    <StudentBasicInfoTab :formData="formData" :isView="isView" :isEdit="isEdit" :t="t" :rules="rules" />
                </el-tab-pane>

                <el-tab-pane :label="t('student.contactsTab')" name="contacts">
                    <StudentContactsTab :contacts="formData.contacts || []" :isView="isView" :t="t"
                        @add-contact="addEmptyContact" @update:contacts="formData.contacts = $event" />
                </el-tab-pane>

            </el-tabs>

            <el-row v-if="activeTab === 'basic'">
                <el-col :span="24">
                    <label class="d-none required"></label>
                    <el-form-item prop="fullName" class="d-none" />
                    <el-form-item prop="phone" class="d-none" />
                </el-col>
            </el-row>
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
const windowWidth = ref(window.innerWidth);
const computedDialogWidth = computed(() => windowWidth.value < 700 ? '100%' : '60%');
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
    if (isView.value) return t('student.detailTitle')
    if (isEdit.value) return t('student.editTitle')
    return t('student.addTitle')
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
    applicationUser: {
        userCode: '',
        fullName: '',
        dateOfBirth: '',
        phoneNumber: '',
        email: '',
        gender: false,
        provinceCode: '',
        address: '',
        note: '',
    },
}
const formData = ref<Partial<StudentModel & { priority: number, expectedBudget: number }>>({ ...defaultFormData })

watch(

    () => props.studentData,
    (data) => {
        if (data && data.id) {
            // Đảm bảo contacts là mảng khi load data
            formData.value = {
                ...data,
                applicationUser: {
                    id: data.applicationUserId ?? '',
                    userName: data.applicationUser?.userName ?? '',
                    userCode: data.applicationUser?.userCode ?? '',
                    fullName: data.applicationUser?.fullName ?? '',
                    dateOfBirth: data.applicationUser?.dateOfBirth ?? null,
                    phoneNumber: data.applicationUser?.phoneNumber ?? '',
                    email: data.applicationUser?.email ?? '',
                    gender: data.applicationUser?.gender ?? false,
                    provinceCode: data.applicationUser?.provinceCode ?? '',
                    address: data.applicationUser?.address ?? '',
                    note: data.applicationUser?.note ?? '',
                },
                applicationUserId: data.applicationUserId ?? '',
                contacts: data.contacts || [],
                priority: 1,
                expectedBudget: 15000000,
            } as any;
        } else {
            formData.value = { ...defaultFormData }
        }
        activeTab.value = 'basic'; // Reset về tab đầu tiên
    },
    { immediate: true }
)

const rules = {
    studentCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    fullName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    phone: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    leadSource: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    employeeId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
}

// Logic thêm Contact mới
function addEmptyContact() {
    if (!formData.value.contacts) {
        formData.value.contacts = [];
    }
    const newContact: Partial<ContactModel> = {
        fullName: 'Người liên hệ mới',
        relationship: 0, // Cha
        phone: '',
        email: '',
        gender: 0, // Nam
    };
    formData.value.contacts.push(newContact as ContactModel);
}

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
            //gán các thông tin liên quan đến applicationUser từ các thông tin có trong formdata
            formData.value.applicationUser.userName = formData.value.email;
            formData.value.applicationUser.userCode = formData.value.studentCode;
            formData.value.applicationUser.phoneNumber = formData.value.phone;
            formData.value.applicationUser.email = formData.value.email;
            formData.value.applicationUser.gender = formData.value.gender == "Nam" ? true : false;
            formData.value.applicationUser.address = formData.value.address;
            formData.value.applicationUser.dateOfBirth = formData.value.birthDate;
            formData.value.applicationUser.fullName = formData.value.fullName;
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