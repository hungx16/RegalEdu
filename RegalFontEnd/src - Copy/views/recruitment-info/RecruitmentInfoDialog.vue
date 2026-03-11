<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="mode" @submit="onSubmit" @delete="onDelete" @update:visible="emit('update:visible', $event)">
        <template #form>
            <el-row :gutter="20">
                <!-- Multilingual + Publish -->
                <el-col :span="12">
                    <el-form-item>
                        <el-checkbox v-model="formData.isMultilingual" :disabled="isView">
                            {{ t('common.allowMultilingual') }}
                        </el-checkbox>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item>
                        <el-checkbox v-model="formData.isPublish" :disabled="isView">
                            {{ t('common.isPublish') }}
                        </el-checkbox>
                    </el-form-item>
                </el-col>

                <!-- Name -->
                <el-col :span="12">
                    <label class="required">{{ t('recruitmentInfo.name') }}</label>
                    <el-form-item prop="recruitmentInfoName">
                        <el-input v-model="formData.recruitmentInfoName" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12" v-if="formData.isMultilingual">
                    <label class="required">English Name</label>
                    <el-form-item prop="enRecruitmentInfoName">
                        <el-input v-model="formData.enRecruitmentInfoName" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Position -->
                <el-col :span="12">
                    <label class="required">{{ t('recruitmentInfo.position') }}</label>
                    <el-form-item prop="position">
                        <el-input v-model="formData.position" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12" v-if="formData.isMultilingual">
                    <label class="required">English Position</label>
                    <el-form-item prop="enPosition">
                        <el-input v-model="formData.enPosition" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Department -->
                <el-col :span="12">
                    <label class="required">{{ t('models.Department') }}</label>
                    <el-form-item prop="departmentId">
                        <el-select v-model="formData.departmentId" :disabled="isView || deptStore.loading" class="w-100"
                            filterable clearable :placeholder="t('common.select')">
                            <el-option v-for="d in deptOptions" :key="d.value" :label="d.label" :value="d.value" />
                        </el-select>
                    </el-form-item>
                </el-col>


                <!-- WorkType -->
                <el-col :span="12">
                    <label class="required">{{ t('recruitmentInfo.workType') }}</label>
                    <el-form-item prop="workType">
                        <el-select v-model="formData.workType" :disabled="isView" class="w-100">
                            <el-option v-for="w in workTypeOptions" :key="w.value" :label="w.label" :value="w.value" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <!-- Experience -->
                <el-col :span="12">
                    <label>{{ t('recruitmentInfo.experience') }}</label>
                    <el-form-item prop="experience">
                        <el-input v-model="formData.experience" :disabled="isView" />
                    </el-form-item>
                </el-col>


                <el-col :span="12" v-if="formData.isMultilingual">
                    <label>English Experience</label>
                    <el-form-item prop="enExperience">
                        <el-input v-model="formData.enExperience" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <!-- Salary -->
                <el-col :span="12">
                    <label>{{ t('recruitmentInfo.salary') }}</label>
                    <el-form-item prop="salary">
                        <CurrencyInput v-model="formData.salary" :disabled="isView" locale="vi-VN" currency="VND" />
                    </el-form-item>
                </el-col>

                <!-- Province -->
                <el-col :span="12">
                    <label class="required">{{ t('employee.province') }}</label>
                    <el-form-item prop="provinceCode">
                        <el-select v-model="formData.provinceCode" :disabled="isView" class="w-100" filterable clearable
                            :placeholder="t('common.select')">
                            <el-option v-for="p in provinceOptions" :key="p.value" :label="p.label" :value="p.value" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <!-- Description -->
                <el-col :span="24">
                    <label>{{ t('recruitmentInfo.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :disabled="isView" rows="3" />
                    </el-form-item>
                </el-col>

                <el-col :span="24" v-if="formData.isMultilingual">
                    <label>English Description</label>
                    <el-form-item prop="enDescription">
                        <el-input type="textarea" v-model="formData.enDescription" :disabled="isView" rows="3" />
                    </el-form-item>
                </el-col>

                <!-- Status -->
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
                    <el-form-item prop="status">
                        <el-radio-group v-model="formData.status" :disabled="isView">
                            <el-radio :value="0">{{ t('common.active') }}</el-radio>
                            <el-radio :value="1">{{ t('common.inactive') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, ref, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { RecruitmentInfoModel } from '@/api/RecruitmentInfoApi'
import { useCommonStore } from '@/stores/commonStore'
import { useDepartmentStore } from '@/stores/departmentStore'
import CurrencyInput from '@/components/currency-input/CurrencyInput.vue'
import { getWorkTypeOptions } from '@/utils/makeList'

const props = defineProps<{
    visible: boolean
    mode?: 'create' | 'edit' | 'view'
    recruitmentInfoData: Partial<RecruitmentInfoModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete'])
const { t } = useI18n()
const commonStore = useCommonStore()
const deptStore = useDepartmentStore()
const baseDialogRef = ref()

/** ===== FORM MODEL ===== */
const formData = ref<RecruitmentInfoModel>({
    recruitmentInfoName: '',
    description: '',
    experience: '',
    salary: 0,
    position: '',
    departmentId: '',        // giữ dạng string để bind an toàn
    provinceCode: '',
    // bổ sung song ngữ + publish + work type
    isMultilingual: false,
    isPublish: false,
    enRecruitmentInfoName: '',
    enDescription: '',
    enExperience: '',
    enPosition: '',
    workType: 0 as any,      // FullTime
    status: 0,
} as any)

const isView = computed(() => props.mode === 'view')
const modeTitle = computed(() => {
    if (isView.value) return t('recruitmentInfo.detailTitle')
    if (props.mode === 'edit') return t('recruitmentInfo.editTitle')
    return t('recruitmentInfo.addTitle')
})

/** ===== OPTIONS ===== */
const deptOptions = computed(() =>
    (deptStore.departments || [])
        .map((d: any) => ({
            label: d.departmentName ?? d.name ?? d.departmentCode ?? '',
            value: d.id ?? d.departmentId ?? d.code ?? '',
        }))
        .filter(x => x.value !== '')
)

const provinceOptions = computed(() =>
    (commonStore.provinces || [])
        .map(p => ({ label: p.provinceName, value: p.provinceCode }))
        .filter(x => x.value !== '')
)

// Nếu anh đã có getWorkTypeOptions thì thay bằng hàm đó
const workTypeOptions = getWorkTypeOptions(t)

/** ===== RULES ===== */
const rules = computed(() => {
    const base: any = {
        recruitmentInfoName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
        position: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
        departmentId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
        provinceCode: [{ required: true, message: t('validation.required'), trigger: 'change' }],
        workType: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    }
    if (formData.value.isMultilingual) {
        base.enRecruitmentInfoName = [{ required: true, message: t('validation.required'), trigger: 'blur' }]
        base.enPosition = [{ required: true, message: t('validation.required'), trigger: 'blur' }]
    }
    return base
})

/** ===== WATCH/INIT ===== */
watch(
    () => props.recruitmentInfoData,
    (val) => {
        if (!val) return
        formData.value = {
            ...formData.value,
            ...val,
            // chuẩn hoá key
            departmentId: (val as any).departmentId ?? (val as any).departmentID ?? formData.value.departmentId,
            provinceCode: (val as any).provinceCode ?? (val as any).province ?? formData.value.provinceCode,
            // đảm bảo có default cho các field mới
            isMultilingual: val.isMultilingual ?? false,
            isPublish: val.isPublish ?? false,
            enRecruitmentInfoName: val.enRecruitmentInfoName ?? '',
            enDescription: val.enDescription ?? '',
            enExperience: val.enExperience ?? '',
            enPosition: val.enPosition ?? '',
            workType: (val as any).workType ?? 0,
        } as any
    },
    { immediate: true }
)

watch(
    () => props.visible,
    async (v) => {
        if (v) {
            if (!commonStore.provinces?.length) await commonStore.fetchProvinces()
            if (!deptStore.departments?.length) await deptStore.fetchAllDepartments()
        }
    }
)

/** ===== HANDLERS ===== */
function onSubmit() {
    emit('submit', { ...formData.value })
}
function onDelete() {
    emit('delete', { ...formData.value })
}

/** ===== MOUNT ===== */
onMounted(async () => {
    if (!commonStore.provinces?.length) await commonStore.fetchProvinces()
    if (!deptStore.departments?.length) await deptStore.fetchAllDepartments()
})
</script>
