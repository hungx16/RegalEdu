<template>
    <BaseDialogForm :visible="model" @update:visible="emit('update:modelValue', $event)" :title="t('menu.profile')"
        :showDelete="false" mode="edit" :formData="form" :rules="rules" :loading="formLoading" @submit="onSubmit">
        <template #icon><i class="ki-duotone ki-profile-circle hide-delete"></i></template>

        <template #form>
            <el-row :gutter="20" class="w-100 form-responsive">
                <!-- Avatar -->
                <el-col :xs="24" :sm="8" class="text-center mb-4 mb-sm-0">
                    <div class="mb-2 fw-semibold">{{ t('profile.avatar') }}</div>
                    <el-upload class="avatar-uploader" action="#" :auto-upload="false" :show-file-list="false"
                        :before-upload="beforeAvatarUpload" :on-change="onAvatarChange"
                        accept="image/png,image/jpeg,image/webp">
                        <div class="avatar-wrapper">
                            <img v-if="avatarPreview" :src="avatarPreview" class="avatar-img" alt="avatar" />
                            <div v-else class="avatar-placeholder">
                                <i class="bi bi-person fs-1"></i>
                            </div>
                        </div>
                    </el-upload>
                    <div class="mt-2 d-flex justify-content-center gap-2 avatar-actions">
                        <el-button size="small" @click="removeAvatar">{{ t('common.remove') }}</el-button>
                        <el-button size="small" type="primary" @click="triggerFileInput">{{ t('common.change')
                        }}</el-button>
                    </div>
                    <input ref="fileInputRef" type="file" class="d-none" accept="image/png,image/jpeg,image/webp"
                        @change="manualFilePicked" />
                </el-col>

                <!-- Fields -->
                <el-col :xs="24" :sm="16">
                    <el-form-item label-position="left" :label="t('profile.full_name')" prop="fullName">
                        <el-input v-model="form.fullName" />
                    </el-form-item>

                    <el-form-item label-position="left" :label="t('profile.company_email')" prop="companyEmail">
                        <el-input v-model="form.companyEmail" />
                    </el-form-item>

                    <el-form-item label-position="left" :label="t('profile.personal_email')" prop="personalEmail">
                        <el-input v-model="form.personalEmail" />
                    </el-form-item>

                    <el-form-item label-position="left" :label="t('profile.gender')" prop="gender">
                        <el-radio-group v-model="form.gender">
                            <el-radio :value="true">{{ t('profile.male') }}</el-radio>
                            <el-radio :value="false">{{ t('profile.female') }}</el-radio>
                        </el-radio-group>

                    </el-form-item>

                    <el-form-item label-position="left" :label="t('profile.date_of_birth')" prop="dateOfBirth">
                        <el-date-picker v-model="form.dateOfBirth" type="date" placeholder="YYYY-MM-DD"
                            format="YYYY-MM-DD" value-format="YYYY-MM-DD" style="width: 100%" />
                    </el-form-item>

                    <el-form-item label-position="left" :label="t('profile.phone_number')" prop="phoneNumber">
                        <el-input v-model="form.phoneNumber" />
                    </el-form-item>

                    <el-form-item label-position="left" :label="t('profile.address')" prop="address">
                        <el-input v-model="form.address" />
                    </el-form-item>


                    <el-alert v-if="noteLoginEmail" type="info" :closable="false" class="mt-2">
                        {{ noteLoginEmail }}
                    </el-alert>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { reactive, ref, computed, watch } from 'vue'
import { useVModel } from '@vueuse/core'
import { ElMessage } from 'element-plus'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import { useEmployeeStore } from '@/stores/employeeStore'
import type { EmployeeModel } from '@/api/EmployeeApi'
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout'
import { useNotificationStore } from '@/stores/notificationStore'

const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(30000)
const notificationStore = useNotificationStore()
type UserLS = {
    userName?: string            // email đăng nhập
    originalUserName?: string    // full name hiển thị
    companyEmail?: string
    personalEmail?: string
    avatarUrl?: string           // raw base64 (không header)
}

const props = defineProps<{ modelValue: boolean; user?: UserLS | null }>()
const emit = defineEmits<{ (e: 'update:modelValue', v: boolean): void }>()
const { t } = useI18n()
const model = useVModel(props, 'modelValue', emit)

const employeeStore = useEmployeeStore()

const loading = ref(false)
const fileInputRef = ref<HTMLInputElement | null>(null)

/** ====== FORM STATE ====== */
const form = reactive({
    fullName: '',
    companyEmail: '',
    personalEmail: '',
    gender: true as boolean | null,
    dateOfBirth: '' as string | null, // 'YYYY-MM-DD'
    phoneNumber: '',
    provinceCode: '',
    avatarBase64: '' as string,       // dataURL preview
    avatarMime: 'image/jpeg' as 'image/jpeg' | 'image/png' | 'image/webp',
    address: '' as string | null
})

/** ===== LocalStorage helpers ===== */
function readUserFromLS(): UserLS | null {
    try {
        const raw = localStorage.getItem('userData')
        return raw ? (JSON.parse(raw) as UserLS) : null
    } catch { return null }
}
function writeUserToLS(next: UserLS) {
    localStorage.setItem('userData', JSON.stringify(next))
    window.dispatchEvent(new StorageEvent('storage', { key: 'userData', newValue: JSON.stringify(next) }))
}

/** ===== Load employee theo email khi mở dialog ===== */
watch(model, async (open) => {
    if (!open) return
    loading.value = true
    try {
        const loginEmail = props.user?.userName ?? readUserFromLS()?.userName ?? ''
        if (loginEmail) {
            await employeeStore.getEmployeeByIdOrEmail(undefined, loginEmail) // chỉ truyền email
        }
        const emp: EmployeeModel | null = employeeStore.selectedEmployee

        // Map -> form
        form.fullName = (emp as any)?.applicationUser?.fullName
            || props.user?.originalUserName || props.user?.userName
            || readUserFromLS()?.originalUserName || readUserFromLS()?.userName || ''

        form.companyEmail = (emp as any)?.applicationUser?.email
            || props.user?.companyEmail || props.user?.userName
            || readUserFromLS()?.companyEmail || readUserFromLS()?.userName || ''

        form.personalEmail = emp?.personalEmail || props.user?.personalEmail || readUserFromLS()?.personalEmail || ''

        form.gender = (emp as any)?.applicationUser?.gender ?? true
        form.dateOfBirth = normalizeDate((emp as any)?.applicationUser?.dateOfBirth)
        form.phoneNumber = (emp as any)?.applicationUser?.phoneNumber || ''
        form.address = (emp as any)?.applicationUser?.address || ''

        // Avatar: ưu tiên applicationUser.avatarString; nếu không có, lấy từ LS
        const avatarFromEmp = (emp as any)?.applicationUser?.avatarString as string | null
        const avatarRawFromLS = props.user?.avatarUrl || readUserFromLS()?.avatarUrl
        if (avatarFromEmp) {
            const hasHeader = avatarFromEmp.startsWith('data:')
            form.avatarBase64 = hasHeader ? avatarFromEmp : `data:${guessMimeFromBase64(avatarFromEmp)};base64,${avatarFromEmp}`
            form.avatarMime = hasHeader ? (avatarFromEmp.split(';')[0].replace('data:', '') as any) : guessMimeFromBase64(avatarFromEmp)
        } else if (avatarRawFromLS) {
            const mime = guessMimeFromBase64(avatarRawFromLS)
            form.avatarBase64 = `data:${mime};base64,${avatarRawFromLS}`
            form.avatarMime = mime
        } else {
            form.avatarBase64 = ''
            form.avatarMime = 'image/jpeg'
        }
    } finally {
        loading.value = false
    }
}, { immediate: false })

/** ===== Preview avatar ===== */
const avatarPreview = computed(() => form.avatarBase64 || '')

/** ===== Validators ===== */
const emailRule = { type: 'email', message: t('validation.email'), trigger: 'blur' } as const
const rules = {
    fullName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    companyEmail: [{ required: true, message: t('validation.required'), trigger: 'blur' }, emailRule],
    personalEmail: [emailRule],
    phoneNumber: [{ validator: (_: any, v: string, cb: any) => cb(!v || /^[0-9+\-\s()]{6,}$/.test(v) ? undefined : new Error(t('validation.phone'))), trigger: 'blur' }],
    dateOfBirth: [{ validator: (_: any, v: string, cb: any) => cb(!v || /^\d{4}-\d{2}-\d{2}$/.test(v) ? undefined : new Error(t('validation.date'))), trigger: 'change' }]
}

/** ===== Note login email ===== */
const noteLoginEmail = computed(() => {
    const loginEmail = props.user?.userName ?? readUserFromLS()?.userName ?? ''
    return loginEmail ? t('profile.login_email_note', { email: loginEmail }) : ''
})

/** ===== Avatar upload ===== */
function beforeAvatarUpload(file: File) {
    const isOkType = ['image/jpeg', 'image/png', 'image/webp'].includes(file.type)
    const isLt3M = file.size / 1024 / 1024 < 3
    if (!isOkType) ElMessage.error(t('validation.image_type'))
    if (!isLt3M) ElMessage.error(t('validation.image_size'))
    return isOkType && isLt3M
}
async function onAvatarChange(file: any) {
    const raw: File | undefined = file?.raw
    if (!raw || !beforeAvatarUpload(raw)) return
    form.avatarBase64 = await fileToDataURL(raw)
    form.avatarMime = (raw.type as any) || 'image/jpeg'
}
function triggerFileInput() { fileInputRef.value?.click() }
async function manualFilePicked(e: Event) {
    const input = e.target as HTMLInputElement
    if (!input.files || !input.files[0]) return
    const f = input.files[0]
    if (!beforeAvatarUpload(f)) return
    form.avatarBase64 = await fileToDataURL(f)
    form.avatarMime = (f.type as any) || 'image/jpeg'
    input.value = ''
}
function removeAvatar() { form.avatarBase64 = '' }

/** ===== Submit ===== */
async function onSubmit() {
    loading.value = true
    try {
        // Cập nhật localStorage ngay
        const current = readUserFromLS() || {}
        const next: UserLS = {
            ...current,
            originalUserName: form.fullName,
            companyEmail: form.companyEmail,
            personalEmail: form.personalEmail,
            avatarUrl: form.avatarBase64 ? stripDataHeader(form.avatarBase64) : undefined
        }
        writeUserToLS(next)

        // Gọi API cập nhật Employee + ApplicationUser (nested)
        const emp = employeeStore.selectedEmployee
        const payload: Partial<EmployeeModel> = {
            id: emp?.id,
            personalEmail: form.personalEmail,
            applicationUserId: (emp as any)?.applicationUser?.id ?? emp?.applicationUserId,
            applicationUser: {
                id: (emp as any)?.applicationUser?.id ?? null,
                fullName: form.fullName,
                email: form.companyEmail,
                gender: form.gender === null ? true : form.gender,
                phoneNumber: form.phoneNumber || null,
                dateOfBirth: form.dateOfBirth || null,
                provinceCode: form.provinceCode || null,
                avatar: form.avatarBase64 ? stripDataHeader(form.avatarBase64) : null,
                address: form.address || null
            } as any
        }

        try {
            startLoading()
            await employeeStore.updateProfile(payload)
            notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('models.profile') } });

        } catch (e) {
            console.warn('Save employee failed, kept local changes only.', e)
        }
        finally {
            stopLoading()
        }
        model.value = false
    } finally {
        loading.value = false
    }
}

/** ===== Utils ===== */
function fileToDataURL(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
        const reader = new FileReader()
        reader.onload = () => resolve(reader.result as string)
        reader.onerror = reject
        reader.readAsDataURL(file)
    })
}
function stripDataHeader(dataUrl: string) {
    const idx = dataUrl.indexOf('base64,')
    return idx >= 0 ? dataUrl.substring(idx + 7) : dataUrl
}
function guessMimeFromBase64(raw: string): 'image/png' | 'image/jpeg' | 'image/webp' {
    if (!raw) return 'image/jpeg'
    if (raw.startsWith('data:')) {
        const m = raw.slice(5, raw.indexOf(';')) as any
        return (m || 'image/jpeg')
    }
    if (raw.startsWith('iVBOR')) return 'image/png'
    if (raw.startsWith('/9j/')) return 'image/jpeg'
    return 'image/jpeg'
}
function normalizeDate(v?: string | null) {
    if (!v) return ''
    // nhận 'YYYY-MM-DD' hoặc ISO -> trả 'YYYY-MM-DD'
    const m = v.match(/^\d{4}-\d{2}-\d{2}/)
    return m ? m[0] : ''
}
</script>
<style scoped>
.form-responsive {
    --form-label-width: 120px;
    max-width: 800px;
    margin: 0 auto;
    padding: 0 8px;
}

:deep(.el-form-item) {
    display: flex;
    margin-bottom: 4px !important;
    /* Giảm xuống 4px */
    align-items: center;
    gap: 4px;
    /* Giảm gap xuống 4px */
}

:deep(.el-form-item__label) {
    width: var(--form-label-width);
    padding: 0;
    margin: 0;
    text-align: left;
    /* line-height: 1; */
    /* Giảm line-height */
    color: #606266;
    font-size: 14px;
}

:deep(.el-form-item__content) {
    flex: 1;
    min-width: 0;
    margin-left: 4px !important;
    /* Giảm margin */
}

/* Input styles */
:deep(.el-input__wrapper),
:deep(.el-select__wrapper),
:deep(.el-date-editor.el-input__wrapper) {
    width: 100%;
    box-shadow: 0 0 0 1px #e6e9ef inset;
    padding: 4px 8px !important;
    /* Giảm padding */
    border-radius: 4px;
    min-height: 32px !important;
    /* Giảm height */
}

:deep(.el-input__inner) {
    height: 32px !important;
    /* Giảm height */
    padding: 4px 8px !important;
    font-size: 14px;
}

.el-form-item {
    margin-bottom: 8px !important;
    padding-bottom: 10px !important
}

/* Mobile styles */
@media (max-width: 768px) {
    .form-responsive {
        padding: 0 8px;
    }

    :deep(.el-form-item) {
        flex-direction: column;
        align-items: flex-start;
        margin-bottom: 8px !important;
        /* Giảm margin mobile */
        gap: 2px;
        /* Giảm gap trên mobile */
    }

    :deep(.el-form-item__label) {
        width: 100%;
        margin-bottom: 2px !important;
        /* Giảm margin label */
        font-size: 14px;
    }

    :deep(.el-form-item__content) {
        width: 100%;
        margin-left: 0 !important;
    }

    :deep(.el-input__wrapper),
    :deep(.el-select__wrapper),
    :deep(.el-date-editor.el-input__wrapper) {
        padding: 4px 8px !important;
        min-height: 36px !important;
    }
}

/* Avatar styles */
.avatar-wrapper {
    width: 96px;
    height: 96px;
    /* Sửa lại height bằng width */
    border-radius: 50%;
    background-color: #f5f8fa;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0 auto;
    overflow: hidden;
}

.avatar-placeholder {
    width: 100%;
    height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #99a2ab;
}

.avatar-placeholder i {
    font-size: 40px !important;
}

.avatar-img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.avatar-uploader {
    text-align: center;
    margin-bottom: 16px;
}

.avatar-actions {
    justify-content: center;
    gap: 8px;
    margin-top: 16px;
}

.avatar-actions .el-button {
    min-width: 72px;
    padding: 4px 8px;
}
</style>
