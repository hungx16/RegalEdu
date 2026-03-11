<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('region.code') }}</label>
                    <el-form-item prop="regionCode">
                        <el-input v-model="formData.regionCode" :disabled="isView"
                            :placeholder="t('region.codePlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('region.manager') }}</label>
                    <el-form-item prop="managerId" v-if="!isView">
                        <el-select v-model="formData.managerId" filterable clearable
                            :placeholder="t('company.managerPlaceholder')">
                            <el-option v-for="opt in employeeOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" :disabled="opt.disabled" />
                        </el-select>
                    </el-form-item>
                    <span v-if="isView">{{ formData.manager?.fullName || '-' }}</span>
                </el-col>

                <!-- <el-col :span="12" v-if="!isView">
                    <label class="fs-6 mb-2 d-block">{{ t('region.codePlaceholder') }}</label>
                </el-col> -->
                <!-- <el-col :span="12" v-if="!isView">
                    <label class="fs-6 mb-2 d-block">{{ t('region.managerPlaceholder') }}</label>
                </el-col> -->
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('region.name') }}</label>
                    <el-form-item prop="regionName">
                        <el-input v-model="formData.regionName" :disabled="isView"
                            :placeholder="t('region.namePlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col v-if="isView" :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('region.companyNumber') }}</label>
                    <BaseBadge :label="formData.companies?.length || 0" color="deepPurple" displayType="department" />
                </el-col>
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('region.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="2" :disabled="isView"
                            :placeholder="t('region.descriptionPlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
                    <el-form-item prop="status">
                        <el-radio-group v-model="formData.status" :disabled="isView">
                            <el-radio :value="0">{{ t('common.active') }}</el-radio>
                            <el-radio :value="1">{{ t('common.inactive') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
                <el-col :span="12" v-if="isView">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdBy') }}</label>
                    <BaseBadge :label="formData.createdBy || ''" color="purple" />
                </el-col>
                <el-col :span="12" v-if="isView">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdAt') }}</label>
                    <el-form-item>
                        <el-input :value="formatDate(formData.createdAt || '', 'YYYY-MM-DD HH:mm:ss')"
                            :disabled="true" />
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { RegionModel } from '@/api/RegionApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { formatDate } from '@/utils/format'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { useEmployeeStore } from '@/stores/employeeStore'
import { useCommonStore } from '@/stores/commonStore'
import { buildSelectOptions } from '@/utils/publicFunction'
const employeeStore = useEmployeeStore()

const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    regionData: Partial<RegionModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])

const { t } = useI18n()
const notificationStore = useNotificationStore()
const commonStore = useCommonStore()

const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
    if (isView.value) return t('region.detailTitle')
    if (isEdit.value) return t('region.editTitle')
    if (isCreate.value) {
        formData.value.regionCode = commonStore.code ?? ''
    }
    return t('region.addTitle')
})

const formRef = ref()
const loading = ref(false)

const formData = ref<RegionModel>({
    id: '',
    regionCode: '',
    regionName: '',
    managerId: '',
    manager: null,
    companies: [],
    description: '',
    createdAt: '',
    createdBy: '',
    isDeleted: false,
    status: 0 // Mặc định là active
})

watch(
    () => props.regionData,
    (data) => {
        if (data) {
            formData.value = {
                id: data.id ?? '',
                regionCode: data.regionCode ?? '',
                regionName: data.regionName ?? '',
                managerId: data.managerId ?? null,
                manager: data.manager ?? null,
                companies: data.companies ?? [],
                description: data.description ?? '',
                createdAt: data.createdAt ?? '',
                createdBy: data.createdBy ?? '',
                isDeleted: data.isDeleted ?? false,
                status: data.status ?? 0 // Mặc định là active
            }
        } else {
            formData.value = {
                regionCode: '',
                regionName: '',
                managerId: null,
                manager: null,
                description: '',
                status: 0 // Mặc định là active
            }
        }
    },
    { immediate: true }
)

const rules = {
    regionCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    regionName: [{ required: true, message: t('validation.required'), trigger: 'blur' }]
}

const baseDialogRef = ref()
const employeeOptionsObj = computed(() =>
    buildSelectOptions(
        employeeStore.employees,
        formData.value.managerId,
        // Cách hiển thị label
        e => e.applicationUser.fullName + (e.applicationUser.status === 1 ? ` (${t('common.inactive')})` : ''),
        // Khi nào disable
        e => e.applicationUser.status === 1
    )
);
const employeeOptions = computed(() => employeeOptionsObj.value.options);

function closeModal() {
    emit('update:visible', false)
    emit('close')
}

function onSubmit() {
    const form = baseDialogRef.value?.formRef
    form.validate(async (valid: boolean) => {
        if (valid) {
            loading.value = true
            emit('submit', formData.value)
            loading.value = false
        } else {
            notificationStore.showToast('error', {
                key: 'validation.formInvalid'
            })
        }
    })
}
function onDelete() {
    emit('delete', formData.value)
}
watch(
    () => props.visible,
    async (val) => {
        if (val) {
            await employeeStore.fetchAllEmployees()
        }
    },
    { immediate: true }
)
</script>
