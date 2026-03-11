<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible"
        :title="isEdit ? t('applicationUser.editTitle') : t('applicationUser.addTitle')" :form-data="formData"
        :rules="rules" :is-edit="isEdit" :loading="loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <!-- Full Name -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">
                        {{ t('applicationUser.fullName') }}
                    </label>
                    <el-form-item prop="fullName">
                        <el-input v-model="formData.fullName" placeholder="" />
                    </el-form-item>
                </el-col>

                <!-- Username -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">
                        {{ t('applicationUser.userName') }}
                    </label>
                    <el-form-item prop="userName">
                        <el-input v-model="formData.userName" placeholder="" />
                    </el-form-item>
                </el-col>

                <!-- Email -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">
                        {{ t('applicationUser.email') }}
                    </label>
                    <el-form-item prop="email">
                        <el-input v-model="formData.email" placeholder="" />
                    </el-form-item>
                </el-col>

                <!-- Phone -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">
                        {{ t('applicationUser.phoneNumber') }}
                    </label>
                    <el-form-item prop="phoneNumber">
                        <el-input v-model="formData.phoneNumber" placeholder="" />
                    </el-form-item>
                </el-col>

                <!-- Gender -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">
                        {{ t('applicationUser.gender') }}
                    </label>
                    <el-form-item prop="gender">
                        <el-radio-group v-model="formData.gender">
                            <el-radio :value="true">{{ t('applicationUser.male') }}</el-radio>
                            <el-radio :value="false">{{ t('applicationUser.female') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>

                <!-- Employee ATID -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">
                        {{ t('applicationUser.userCode') }}
                    </label>
                    <el-form-item prop="userCode">
                        <el-input v-model="formData.userCode" placeholder="" />
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, reactive, ref, toRaw, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { ApplicationUserModel } from '@/api/ApplicationUserApi';
import { useNotificationStore } from '@/stores/notificationStore'

// Props from parent
const props = defineProps({
    visible: Boolean,
    isEdit: Boolean,
    loading: Boolean,
    userData: Object as () => Partial<ApplicationUserModel> | null
});
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close']);
const isEdit = computed(() => !!props.userData && !!props.userData.id)
const formRef = ref()
const loading = ref(false)
const notificationStore = useNotificationStore()
const DEFAULT_PASSWORD = import.meta.env.VITE_PASSWORD_DEFAULT || ''

const { t } = useI18n()

// 1. Khai báo formData trước
const formData = ref<ApplicationUserModel>({
    id: '',
    fullName: '',
    userName: '',
    email: '',
    phoneNumber: '',
    gender: true,
    avatar: null,
    avatarString: null,
    password: '',
    passwordConfirm: '',
    isDeleted: false,
    createdAt: null,
    lastModified: null,
    createdBy: null,
    lastModifiedBy: null,
    genderText: null,
    userCode: '',
    dateOfBirth: null,
    provinceCode: null,
    teacher: null,
    employee: null,
    identityNumber: null
})

// 2. Sau đó mới đến watch sử dụng formData
watch(
    () => props.userData,
    (userData) => {
        if (userData) {
            formData.value = {
                id: userData.id ?? '',
                fullName: userData.fullName ?? null,
                userName: userData.userName ?? null,
                email: userData.email ?? null,
                phoneNumber: userData.phoneNumber ?? null,
                gender: userData.gender ?? true,
                avatar: userData.avatar ?? null,
                avatarString: userData.avatarString ?? null,
                password: '',
                passwordConfirm: '',
                isDeleted: userData.isDeleted ?? false,
                createdAt: userData.createdAt ?? null,
                lastModified: userData.lastModified ?? null,
                createdBy: userData.createdBy ?? null,
                lastModifiedBy: userData.lastModifiedBy ?? null,
                genderText: userData.genderText ?? null,
                userCode: userData.userCode ?? null,
                dateOfBirth: userData.dateOfBirth ?? null,
                provinceCode: userData.provinceCode ?? null,
                teacher: userData.teacher ?? null,
                employee: userData.employee ?? null,
                identityNumber: userData.identityNumber ?? null
            }
        } else {
            formData.value = {
                id: '',
                fullName: 'Họ Tên',
                userName: 'Tên Đăng Nhập',
                email: 'email@example.com',
                phoneNumber: '0901234567',
                gender: true,
                avatar: null,
                avatarString: null,
                password: DEFAULT_PASSWORD, // Lấy từ biến môi trường
                passwordConfirm: DEFAULT_PASSWORD,
                isDeleted: false,
                createdAt: null,
                lastModified: null,
                createdBy: null,
                lastModifiedBy: null,
                genderText: null,
                userCode: '',
                dateOfBirth: null,
                provinceCode: null,
                teacher: null,
                employee: null,
                identityNumber: null
            }
        }
    },
    { immediate: true }
)

// Rules
const rules = {
    fullName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    userName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    email: [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        { type: 'email', message: t('validation.email'), trigger: 'blur' }
    ],
    password: [
        { required: !isEdit.value, message: t('validation.required'), trigger: 'blur' }
    ],
    passwordConfirm: [
        { required: !isEdit.value, message: t('validation.required'), trigger: 'blur' },
        {
            validator: (_: any, value: string) => value === formData.value.password,
            message: t('validation.passwordConfirm'),
            trigger: 'blur'
        }
    ]
}
// Emits
function closeModal() {
    emit('update:visible', false)
    emit('close')
}

const baseDialogRef = ref()

function onSubmit() {
    // Lấy formRef từ component con qua expose
    const formRef = baseDialogRef.value?.formRef
    formRef.validate(async (valid: boolean) => {
        if (valid) {
            loading.value = true
            emit('submit', formData.value) // Truyền formData về cha
            loading.value = false
        } else {
            notificationStore.showToast('error', {
                key: 'validation.formInvalid'
            })
            // KHÔNG gọi closeModal() ở đây
        }
    })
}
function onDelete() {
    emit('delete', toRaw(formData)) // Truyền user đang xoá về cha
}
</script>
