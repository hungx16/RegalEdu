<template>
    <BaseDialogForm :visible="model" @update:visible="emit('update:modelValue', $event)" :showDelete="false"
        :title="t('menu.change_password')" mode="edit" :formData="form" :rules="rules" :loading="formLoading"
        :height="200" @submit="onSubmit">
        <template #icon><i class="ki-duotone ki-key-square"></i></template>

        <template #form>
            <el-row :gutter="20" class="stackable-form">
                <!-- Current password -->
                <el-col :span="24">
                    <el-row :gutter="20" class="field-row">
                        <el-col :xs="24" :sm="6" class="label-col">
                            <label class="required fs-6 fw-semibold mb-2 d-block">
                                {{ t('auth.current_password') }}
                            </label>
                        </el-col>
                        <el-col :xs="24" :sm="18" class="control-col">
                            <el-form-item prop="currentPassword">
                                <el-input v-model="form.currentPassword" type="password" show-password />
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-col>

                <!-- New password -->
                <el-col :span="24">
                    <el-row :gutter="20" class="field-row">
                        <el-col :xs="24" :sm="6" class="label-col">
                            <label class="required fs-6 fw-semibold mb-2 d-block">
                                {{ t('auth.new_password') }}
                            </label>
                        </el-col>
                        <el-col :xs="24" :sm="18" class="control-col">
                            <el-form-item prop="newPassword">
                                <el-input v-model="form.newPassword" type="password" show-password />
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-col>

                <!-- Confirm password -->
                <el-col :span="24">
                    <el-row :gutter="20" class="field-row">
                        <el-col :xs="24" :sm="6" class="label-col">
                            <label class="required fs-6 fw-semibold mb-2 d-block">
                                {{ t('auth.confirm_password') }}
                            </label>
                        </el-col>
                        <el-col :xs="24" :sm="18" class="control-col">
                            <el-form-item prop="confirmPassword">
                                <el-input v-model="form.confirmPassword" type="password" show-password />
                            </el-form-item>
                        </el-col>
                    </el-row>
                </el-col>
            </el-row>

        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useVModel } from '@vueuse/core'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import { useAuthStore } from '@/stores/useAuthStore'
import { useLoadingWithTimeout } from '@/composables/useLoadingWithTimeout'
import { useNotificationStore } from '@/stores/notificationStore'
import { useRouter } from 'vue-router'
import { isStrongPassword } from '@/utils/publicFunction'

const { loading: formLoading, start: startLoading, stop: stopLoading } = useLoadingWithTimeout(10000)
const notificationStore = useNotificationStore()
const authStore = useAuthStore()
const props = defineProps<{ modelValue: boolean }>()
const emit = defineEmits<{ (e: 'update:modelValue', v: boolean): void }>()
const model = useVModel(props, 'modelValue', emit)
const { t } = useI18n()
const router = useRouter();

const loading = ref(false)
const form = reactive({ currentPassword: '', newPassword: '', confirmPassword: '' })
const rules = {
    currentPassword: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    newPassword: [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        {
            validator: (_: any, v: string, cb: any) =>
                cb(
                    isStrongPassword(v)
                        ? undefined
                        : new Error(
                            t?.('validation.password_strength')
                        )
                ),
            trigger: 'blur'
        }
    ], confirmPassword: [
        { required: true, message: t('validation.required'), trigger: 'blur' },
        { validator: (_: any, v: string, cb: any) => cb(v === form.newPassword ? undefined : new Error(t('validation.password_not_match'))) }
    ]
}

async function onSubmit() {
    loading.value = true
    startLoading()
    try {
        await authStore.changePassword({
            userName: authStore.user?.userName || '',
            oldPassword: form.currentPassword,
            newPassword: form.newPassword
        })
        notificationStore.showToast('success', { key: 'toast.updateSuccess', params: { model: t('auth.password') } });
        model.value = false
        authStore.logout()
        router.push("/sign-in");
    } catch (error: any) {
        console.log(error)
    } finally {
        loading.value = false
        stopLoading()
    }
}
</script>
<style scoped>
:deep(.el-form-item__error) {
    position: static !important;
    margin-top: 4px;
    line-height: 1.2;
    white-space: normal;
}

/* đảm bảo control ăn full chiều ngang cột phải */
:deep(.control-col .el-form-item),
:deep(.control-col .el-form-item__content),
:deep(.control-col .el-input),
:deep(.control-col .el-input__wrapper) {
    width: 100%;
    box-sizing: border-box;
}

/* mobile: label một dòng, input một dòng, gap gọn */
@media (max-width: 768px) {
    .label-col {
        margin-bottom: 4px;
    }

    /* khoảng giữa label và input */
    :deep(.control-col .el-form-item) {
        margin-bottom: 10px;
    }
}
</style>
