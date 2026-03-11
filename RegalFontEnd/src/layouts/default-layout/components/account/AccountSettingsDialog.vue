<template>
    <BaseDialogForm :visible="model" @update:visible="emit('update:modelValue', $event)"
        :title="t('menu.account_settings')" mode="edit" :formData="form" :rules="rules" :loading="loading"
        @submit="save">
        <template #icon><i class="ki-duotone ki-setting-2"></i></template>

        <template #form>
            <el-form-item :label="t('employee.fullName')" prop="fullName">
                <el-input v-model="form.fullName" />
            </el-form-item>
            <el-form-item :label="t('employee.email')" prop="email">
                <el-input v-model="form.email" />
            </el-form-item>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { reactive, ref, watch } from 'vue'
import { useVModel } from '@vueuse/core'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'

type UserLS = {
    userName?: string
    originalUserName?: string
    avatarUrl?: string
    roles?: string[]
    accessToken?: string
    refreshToken?: string
    succeeded?: boolean
    userStatus?: number
}

const props = defineProps<{ modelValue: boolean; user?: UserLS | null }>()
const emit = defineEmits<{ (e: 'update:modelValue', v: boolean): void }>()
const model = useVModel(props, 'modelValue', emit)
const { t } = useI18n()

const loading = ref(false)
const form = reactive({ fullName: '', email: '' })
const rules = {
    fullName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    email: [{ required: true, message: t('validation.required'), trigger: 'blur' }]
}

/** Mỗi lần mở dialog -> load lại từ props.user/localStorage */
watch(model, (open) => {
    if (open) {
        const u = props.user || readUserFromLS()
        form.fullName = u?.originalUserName || u?.userName || ''
        form.email = u?.userName || ''
    }
})

function readUserFromLS(): UserLS | null {
    try {
        const raw = localStorage.getItem('userData')
        return raw ? (JSON.parse(raw) as UserLS) : null
    } catch { return null }
}

function writeUserToLS(next: UserLS) {
    localStorage.setItem('userData', JSON.stringify(next))
}

async function save() {
    loading.value = true
    try {
        // Nếu có API update profile, gọi ở đây; nếu chưa, cập nhật localStorage:
        const current = readUserFromLS() || {}
        const next: UserLS = {
            ...current,
            originalUserName: form.fullName,
            userName: form.email
        }
        writeUserToLS(next)
        ElMessage.success(t('common.updated'))
        model.value = false
        // Nếu muốn các nơi khác nhận update same-tab, có thể phát event:
        window.dispatchEvent(new StorageEvent('storage', { key: 'userData', newValue: JSON.stringify(next) }))
    } finally {
        loading.value = false
    }
}
</script>
