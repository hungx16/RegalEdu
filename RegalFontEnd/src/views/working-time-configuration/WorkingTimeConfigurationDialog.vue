<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="props.loading" @submit="onSubmit" @update:visible="emit('update:visible', $event)"
        @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{
                        t('workingTimeConfiguration.nameConfiguration') }}</label>
                    <el-form-item prop="nameConfiguration">
                        <el-input v-model="formData.nameConfiguration" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('workingTimeConfiguration.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="2" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('workingTimeConfiguration.applyToSystem')
                        }}</label>
                    <el-form-item prop="applyToSystem">
                        <el-radio-group v-model="formData.applyToSystem" :disabled="isView">
                            <el-radio :value="true">{{ t('common.yes') }}</el-radio>
                            <el-radio :value="false">{{ t('common.no') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
                <el-col :span="24" v-if="!isView && formData.applyToSystem === false">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('position.belongingDepartments')
                    }}</label>
                    <el-form-item prop="companyIds">
                        <el-select v-model="formData.companyIds" multiple clearable filterable :collapse-tags="false"
                            :max-collapse-tags="2" placeholder="Chọn công ty liên kết" style="width: 100%"
                            popper-class="department-select-dropdown">
                            <!-- Header: Checkbox Chọn tất cả -->
                            <template #header>
                                <el-checkbox v-model="checkAll" :indeterminate="indeterminate" @change="handleCheckAll">
                                    {{ t('common.selectAll') }}
                                </el-checkbox>
                            </template>
                            <!-- Option: danh sách công ty -->
                            <el-option v-for="comp in companyStore.companies" :key="comp.id" :label="comp.companyName"
                                :value="comp.id" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('workingTimeConfiguration.isDefault')
                        }}</label>
                    <el-form-item prop="isDefault">
                        <el-radio-group v-model="formData.isDefault" :disabled="isView">
                            <el-radio :value="true">{{ t('common.yes') }}</el-radio>
                            <el-radio :value="false">{{ t('common.no') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('workingTimeConfiguration.status')
                        }}</label>
                    <el-form-item prop="status">
                        <el-radio-group v-model="formData.status" :disabled="isView">
                            <el-radio :value="StatusType.Active">{{ t('common.active') }}</el-radio>
                            <el-radio :value="StatusType.Inactive">{{ t('common.inactive') }}</el-radio>
                        </el-radio-group>
                    </el-form-item>
                </el-col>
            </el-row>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { WorkingTimeConfigurationModel } from '@/api/WorkingTimeConfigurationApi'
import { StatusType } from '@/types';
import { useCompanyStore } from '@/stores/companyStore';
import { useNotificationStore } from '@/stores/notificationStore';

const notificationStore = useNotificationStore()

const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    workingTimeConfigurationData: Partial<WorkingTimeConfigurationModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')
const checkAll = ref(false)
const indeterminate = ref(false)
const companyStore = useCompanyStore()
const modeTitle = computed(() => {
    if (isView.value) return t('workingTimeConfiguration.detailTitle')
    if (isEdit.value) return t('workingTimeConfiguration.editTitle')
    if (isCreate.value) return t('workingTimeConfiguration.addTitle')
    return ''
})

const baseDialogRef = ref()
const formData = ref<Partial<WorkingTimeConfigurationModel & { companyIds?: string[], companies?: any[] }>>({
    id: '',
    nameConfiguration: '',
    description: '',
    applyToSystem: true,
    workingTimes: [],
    holidays: [],
    isDefault: false, // Thêm trường isDefault nếu cần thiết
    companyIds: [] as string[],
    status: StatusType.Active,// Giả sử có trường status,
    companies: [], // Thêm trường companies để lưu trữ danh sách công ty
})
watch(
    () => props.workingTimeConfigurationData,
    (data) => {
        if (data) {
            // Lấy danh sách phòng ban đã liên kết từ departmentIds hoặc departmentPositions
            const ids = data.workingTimeConfigurationCompanies
                ? data.workingTimeConfigurationCompanies.map((dp: any) => dp.companyId)
                : data.companyIds || [];

            // Lấy object công ty từ store cho view mode
            const companies = ids.map((depId: string) =>
                companyStore.companies.find(dep => dep.id === depId)
            ).filter(Boolean);
            formData.value = {
                id: data.id ?? '',
                nameConfiguration: data.nameConfiguration ?? '',
                description: data.description ?? '',
                applyToSystem: data.applyToSystem ?? true,
                workingTimes: data.workingTimes ?? [],
                holidays: data.holidays ?? [],
                isDefault: data.isDefault ?? false, // Cập nhật trường isDefault nếu có
                status: data.status ?? StatusType.Active,// Giả sử có trường status
                companyIds: ids,
                companies: companies, // Lưu trữ danh sách công ty,
                workingTimeConfigurationCompanies: data.workingTimeConfigurationCompanies || []
            }
        } else {
            formData.value = {
                nameConfiguration: '',
                description: '',
                applyToSystem: true,
                workingTimes: [],
                holidays: [],
                isDefault: false, // Đặt giá trị mặc định cho trường isDefault
                status: StatusType.Active, // Đặt giá trị mặc định cho trường status,
                companyIds: [],
                companies: [], // Khởi tạo danh sách công ty rỗng,
                workingTimeConfigurationCompanies: []
            }
        }
    },
    { immediate: true }
)

const rules = {
    nameConfiguration: [{ required: true, message: t('validation.required'), trigger: 'blur' }]
}
onMounted(async () => {
    // Lấy danh sách công ty từ store khi component được mount
    await companyStore.fetchAllCompanies()
})
function closeModal() {
    emit('update:visible', false)
    emit('close')
}
function onSubmit() {
    const form = baseDialogRef.value.formRef
    form.validate((valid: boolean) => {
        if (valid) {
            if (formData.value.applyToSystem === true) {
                formData.value.workingTimeConfigurationCompanies = [] // Nếu applyToSystem là true, không cần công ty liên kết
            } else {
                formData.value.workingTimeConfigurationCompanies = (formData.value.companyIds || []).map(compId => ({
                    companyId: compId,
                    workingTimeConfigurationId: formData.value.id || undefined // đảm bảo luôn là string hoặc null
                }))
            }

            // Chỉ gửi dữ liệu nếu form hợp lệ
            emit('submit', formData.value)
        } else {
            notificationStore.showToast('error', {
                key: 'validation.formInvalid'
            })
        }
    })
}
// Xử lý chọn tất cả
const handleCheckAll = (val: boolean) => {
    indeterminate.value = false
    if (val) {
        formData.value.companyIds = companyStore.companies
            .map(comp => comp.id)
            .filter((id): id is string => typeof id === 'string' && id !== null && id !== undefined)
    } else {
        formData.value.companyIds = []
    }
}
</script>
