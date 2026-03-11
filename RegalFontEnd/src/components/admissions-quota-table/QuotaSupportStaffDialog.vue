<template>
    <BaseDialogForm :visible="visible" :title="t('admissionsQuota.assignSupportTitle')"
        :description="t('admissionsQuota.assignSupportDesc')" :mode="'create'" :form-data="formData" :rules="rules"
        :loading="loading" :submit-disabled="!canSubmit" @update:visible="$emit('update:visible', $event)"
        @submit="onSubmit" @close="$emit('update:visible', false)">
        <template #form>
            <div class="row g-4">
                <div class="col-12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('admissionsQuota.chooseEmployee')
                        }}</label>
                    <el-form-item prop="employeeId">
                        <el-select v-model="formData.employeeId" filterable clearable style="width:100%"
                            :placeholder="t('admissionsQuota.chooseEmployeePlaceholder')">
                            <el-option v-for="e in employeeOptions" :key="e.id" :label="e.label" :value="e.id" />
                        </el-select>
                    </el-form-item>
                </div>

                <div class="col-12 col-md-6">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('admissionsQuota.position') }}</label>
                    <el-form-item>
                        <el-input :model-value="selectedPositionName" disabled />
                    </el-form-item>
                </div>

                <div class="col-12 col-md-6">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('admissionsQuota.targetCompany') }}</label>
                    <el-form-item>
                        <el-input :model-value="targetName" disabled />
                    </el-form-item>
                </div>

                <div class="col-12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('admissionsQuota.reason') }}</label>
                    <el-form-item prop="reason">
                        <el-input v-model="formData.reason" type="textarea" :rows="3"
                            :placeholder="t('admissionsQuota.reasonPlaceholder')" />
                    </el-form-item>
                </div>
            </div>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import { useEmployeeStore } from '@/stores/employeeStore'
import { QuotaRole, StatusType } from '@/types'
import { isResigned } from '@/utils/dateUtils'

const { t } = useI18n()
const employeeStore = useEmployeeStore()

interface Props {
    visible: boolean
    loading?: boolean
    targetName: string            // Tên chi nhánh hiển thị
    companyId: string             // Company gán hỗ trợ
    admissionsQuotaCompanyId: string,
    admissionsQuotaRegionId: string,
    admissionsQuotaId: string,
    regionId: string
}
const props = withDefaults(defineProps<Props>(), { loading: false })
const emit = defineEmits(['update:visible', 'submit'])

const formData = ref({
    employeeId: '' as string | null,
    reason: '' as string
})

const rules = {
    employeeId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    reason: [{ required: true, message: t('validation.required'), trigger: 'blur' }]
}

const canSubmit = computed(() => !!formData.value.employeeId && !!formData.value.reason?.trim())

const employeeOptions = computed(() => {
    const list = employeeStore.employees || []
    return list
        .filter((e: any) => (e.status ?? 0) === StatusType.Active && e.position?.isSupport && e.companyId === props.companyId && isResigned(e.employeeEndDate) == false)
        .map((e: any) => ({
            id: e.id,
            label: t('admissionsQuota.supportStaffOption', { name: e.applicationUser?.fullName ?? '', position: e.position?.positionName ?? '' }),
            positionName: e.position?.positionName ?? '',
            positionId: e.position?.id ?? null,
            employeeStartedDate: e.employeeStartedDate ?? null,
            employeeEndDate: e.employeeEndDate ?? null,
            employeeNewEndDate: e.employeeNewEndDate ?? null
        }))
})

const selectedPositionName = computed(() => {
    const found = employeeOptions.value.find(x => x.id === formData.value.employeeId)
    return found?.positionName ?? ''
})
onMounted(async () => {
    if (!employeeStore.employees?.length) {
        await employeeStore.fetchAllEmployees()
    }
})

watch(() => props.visible, v => {
    if (v) {
        formData.value = { employeeId: null, reason: '' }
    }
})

function onSubmit() {
    const emp = employeeOptions.value.find(x => x.id === formData.value.employeeId)
    emit('submit', {
        admissionsQuotaCompanyId: props.admissionsQuotaCompanyId,
        companyId: props.companyId,
        admissionsQuotaRegionId: props.admissionsQuotaRegionId,
        regionId: props.regionId,
        admissionsQuotaId: props.admissionsQuotaId,
        employeeId: formData.value.employeeId,
        positionId: emp?.positionId ?? null,
        joinAt: emp?.employeeStartedDate ?? null,
        endAt: emp?.employeeEndDate ?? null,
        probationEnd: emp?.employeeNewEndDate ?? null,
        reason: formData.value.reason?.trim(),
        quotaRole: QuotaRole.Support
    })
}
</script>
