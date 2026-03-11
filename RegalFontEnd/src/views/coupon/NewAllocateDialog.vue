<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="t('company.monthlyCompanyAllocation')"
        :width="computedDialogWidth" :mode="'create'" :form-data="formData" :form-ref="formRef" :loading="loading"
        :rules="rules" @submit="onSubmit" @update:visible="emit('update:visible', $event)" @close="emit('close')">
        <template #icon>
            <i class="bi bi-arrow-left-right text-primary"></i>
        </template>
        <template #form>
            <div class="bg-light rounded p-4 mb-4">
                <el-row :gutter="20">
                    <el-col :span="8">
                        <div class="fw-semibold mb-1">{{ t('company.name') }}:</div>
                        <div>{{ currentInfo.companyName || '-' }}</div>
                    </el-col>
                    <el-col :span="8">
                        <div class="fw-semibold mb-1">{{ t('company.currentRegion') }}:</div>
                        <div>{{ currentInfo.regionName || '-' }}</div>
                    </el-col>
                    <el-col :span="8">
                        <div class="fw-semibold mb-1">{{ t('company.startedDate') }}:</div>
                        <div>{{ currentInfo.startedDate || '-' }}</div>
                    </el-col>
                </el-row>
            </div>

            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">
                        {{ t('company.region') }}
                    </label>
                    <el-form-item prop="regionId">
                        <el-select v-model="formData.regionId" filterable clearable
                            :placeholder="t('company.regionPlaceholder')">
                            <el-option v-for="opt in regionOptions" :key="opt.value" :label="opt.label"
                                :value="opt.value" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">
                        {{ t('company.startedDate') }}
                    </label>
                    <el-form-item prop="startedDate">
                        <el-date-picker v-model="formData.startedDate" type="date" format="DD/MM/YYYY"
                            value-format="YYYY-MM-DD" :clearable="true"
                            :placeholder="t('company.startedDatePlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">
                        {{ t('company.description') }}
                    </label>
                    <el-form-item prop="description">
                        <el-input v-model="formData.description" type="textarea" :rows="2"
                            :placeholder="t('company.reasonChange')" />
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, watch, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import type { LogRegionComModel } from '@/api/CompanyApi'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import { useRegionStore } from '@/stores/regionStore'
import { buildSelectOptions } from '@/utils/publicFunction'
import { formatDate } from '@/utils/format'
import { useCompanyStore } from '@/stores/companyStore'
const companyStore = useCompanyStore()

const props = defineProps<{
    visible: boolean
    data: LogRegionComModel | null
}>()
const emit = defineEmits(['update:visible', 'close', 'submit'])
const { t } = useI18n()
const windowWidth = ref(window.innerWidth)

const baseDialogRef = ref()
const formRef = ref()
const regionStore = useRegionStore()
const loading = ref(false)

const formData = ref<Partial<LogRegionComModel>>({
    regionId: '',
    startedDate: '',
    description: ''
})
const computedDialogWidth = computed(() => {
    return windowWidth.value < 768 ? '100%' : '50%'
})
function updateWindowWidth() {
    windowWidth.value = window.innerWidth
}
onMounted(() => {
    window.addEventListener('resize', updateWindowWidth)
    updateWindowWidth()
})
const currentInfo = computed(() => {
    console.log('currentInfo', companyStore.LogRegionComs);

    const log = companyStore.LogRegionComs.find(l =>
        l.companyId === props.data?.companyId &&
        l.status === 0 &&
        l.endDate == null
    )
    return {
        companyName: props.data?.company?.companyName ?? '-',
        regionName: log?.region?.regionName ?? '-',
        startedDate: formatDate(log?.startedDate ?? '', 'DD/MM/YYYY')
    }
})

watch(() => props.data, (val) => {
    if (val) {
        formData.value = {
            companyId: val.companyId,
            regionId: '',
            startedDate: '',
            description: '',
            status: 0,
        }
    }
}, { immediate: true })

watch(() => props.visible, async (val) => {
    if (val) {
        await regionStore.fetchAllRegions()
    }
}, { immediate: true })

const regionOptions = computed(() =>
    buildSelectOptions(regionStore.regions, formData.value.regionId, r => r.regionName, r => false).options
)

const rules = {
    regionId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    startedDate: [{ required: true, message: t('validation.required'), trigger: 'change' }],
}

function onSubmit() {
    baseDialogRef.value?.formRef?.validate((valid: boolean) => {
        if (!valid) return

        console.log('formData', formData.value);

        emit('submit', formData.value)
    })
}
</script>
