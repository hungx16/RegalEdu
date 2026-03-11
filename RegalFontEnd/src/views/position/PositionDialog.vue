<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('position.code') }}</label>
                    <el-form-item prop="positionCode">
                        <el-input v-model="formData.positionCode" :disabled="isView" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('position.name') }}</label>
                    <el-form-item prop="positionName">
                        <el-input v-model="formData.positionName" :disabled="isView"
                            :placeholder="t('position.namePlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col :span="24" v-if="!isView">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('position.belongingDepartments')
                        }}</label>
                    <el-form-item prop="departmentIds">
                        <el-select v-model="formData.departmentIds" multiple clearable filterable :collapse-tags="false"
                            :max-collapse-tags="2" placeholder="Chọn phòng ban liên kết" style="width: 100%"
                            popper-class="department-select-dropdown">
                            <!-- Header: Checkbox Chọn tất cả -->
                            <template #header>
                                <el-checkbox v-model="checkAll" :indeterminate="indeterminate" @change="handleCheckAll">
                                    {{ t('common.selectAll') }}
                                </el-checkbox>
                            </template>
                            <!-- Option: danh sách phòng ban -->
                            <el-option v-for="dep in departmentStore.departments" :key="dep.id"
                                :label="dep.departmentName" :value="dep.id" />
                        </el-select>
                    </el-form-item>


                </el-col>
                <!-- Chọn nhiều phòng ban -->


                <!-- Hiển thị khi xem (view mode) -->
                <el-col :span="24" v-if="isView">
                    <label class="fs-6 fw-semibold mb-2 d-block">
                        {{ t('position.belongingDepartments') }}
                    </label>
                    <div>
                        <BaseBadge v-for="dep in formData.departments || []" :key="dep.id" :label="dep.departmentName"
                            color="blue" class="me-1 mb-1" />
                        <span v-if="!formData.departments || formData.departments.length === 0">-</span>
                    </div>
                </el-col>

                <el-col :span="8">
                    <el-form-item prop="isSale">
                        <el-checkbox v-model="formData.isSale" :disabled="isView">
                            {{ t('position.isSale') }}
                        </el-checkbox>
                    </el-form-item>
                </el-col>
                <el-col :span="8">
                    <el-form-item prop="isSaleLead">
                        <el-checkbox v-model="formData.isSaleLead" :disabled="isView">
                            {{ t('position.isSaleLead') }}
                        </el-checkbox>
                    </el-form-item>
                </el-col>
                <el-col :span="8">
                    <el-form-item prop="isSupport">
                        <el-checkbox v-model="formData.isSupport" :disabled="isView">
                            {{ t('position.isSupport') }}
                        </el-checkbox>
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
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('position.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="2" :disabled="isView"
                            :placeholder="t('position.descriptionPlaceholder')" />
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
import { computed, ref, watch, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { PositionModel } from '@/api/PositionApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useCommonStore } from '@/stores/commonStore'
import { usePositionStore } from '@/stores/positionStore'
import { formatDate } from '@/utils/format'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'
import { useDepartmentStore } from '@/stores/departmentStore'

const departmentStore = useDepartmentStore()
// Chọn tất cả/indeterminate logic
const checkAll = ref(false)
const indeterminate = ref(false)
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    positionData: Partial<PositionModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const notificationStore = useNotificationStore()
const commonStore = useCommonStore()
const positionStore = usePositionStore()
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
    if (isView.value) return t('position.detailTitle')
    if (isEdit.value) return t('position.editTitle')
    if (isCreate.value) {
        formData.value.positionCode = commonStore.code ?? ''
    }
    return t('position.addTitle')
})

// Xử lý chọn tất cả
const handleCheckAll = (val: boolean) => {
    indeterminate.value = false
    if (val) {
        formData.value.departmentIds = departmentStore.departments
            .map(dep => dep.id)
            .filter((id): id is string => typeof id === 'string' && id !== null && id !== undefined)
    } else {
        formData.value.departmentIds = []
    }
}
// Theo dõi danh sách được chọn để update trạng thái "Chọn tất cả"

const formRef = ref()
const loading = ref(false)

const formData = ref<Partial<PositionModel & { departmentIds?: string[], departments?: any[] }>>({
    id: '',
    positionCode: '',
    positionName: '',
    status: 0,
    description: '',
    createdAt: '',
    isDeleted: false,
    departmentIds: [] as string[],
    departments: [],
    departmentPositions: [] // Thêm trường này để lưu thông tin liên kết với phòng ban
})

const parentOptions = ref<PositionModel[]>([])
watch(() => formData.value.departmentIds, (val) => {
    if (!(val?.length)) {
        checkAll.value = false
        indeterminate.value = false
    } else if (val.length === departmentStore.departments.length) {
        checkAll.value = true
        indeterminate.value = false
    } else {
        checkAll.value = false
        indeterminate.value = true
    }
})
watch(
    () => props.positionData,
    (data) => {
        if (data) {
            // Lấy danh sách phòng ban đã liên kết từ departmentIds hoặc departmentPositions
            const ids = data.departmentPositions
                ? data.departmentPositions.map((dp: any) => dp.departmentId)
                : data.departmentIds || [];

            // Lấy object phòng ban từ store cho view mode
            const departments = ids.map((depId: string) =>
                departmentStore.departments.find(dep => dep.id === depId)
            ).filter(Boolean);

            // Lấy thông tin chức vụ cha nếu có


            formData.value = {
                id: data.id ?? '',
                positionCode: data.positionCode ?? '',
                positionName: data.positionName ?? '',
                status: data.status ?? 0,
                description: data.description ?? '',
                createdAt: data.createdAt ?? '',
                createdBy: data.createdBy ?? '',
                departmentIds: ids,
                departmentPositions: data.departmentPositions ?? [],
                departments: departments, // <-- Đảm bảo luôn có mảng object phòng ban cho view mode!,
                isSale: data.isSale ?? false,
                isSaleLead: data.isSaleLead ?? false,
                isSupport: data.isSupport ?? false,
            }
        } else {
            formData.value = {
                positionCode: '',
                positionName: '',
                status: 0,
                description: '',
                departmentIds: [],
                departmentPositions: [],
                departments: [],
                isSale: false,
                isSaleLead: false,
                isSupport: false,
            }
        }
    },
    { immediate: true }
)


onMounted(async () => {
    await departmentStore.fetchAllDepartments();
    await positionStore.fetchAllPositions().then(() => {
        parentOptions.value = positionStore.positions.filter(
            p => !formData.value.id || p.id !== formData.value.id
        )
    })
})

const rules = {
    positionCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    positionName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    status: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    departmentIds: [
        { required: true, message: t('validation.required'), trigger: 'change', type: 'array' }
    ],
}

const baseDialogRef = ref()
function getAllChildIds(node: any): string[] {
    let result: string[] = []
    if (node.children && node.children.length > 0) {
        node.children.forEach((child: any) => {
            result.push(child.id)
            result = result.concat(getAllChildIds(child))
        })
    }
    return result
}
function buildTree(list: any[], parentId: string | null = null): any[] {
    return list
        .filter(item => item.positionParentId === parentId)
        .map(item => ({
            ...item,
            children: buildTree(list, item.id)
        }))
}
// Hoặc đặt vào watch khi thay đổi id chức vụ đang edit:
watch(() => formData.value.id, () => filterParentOptions())

function filterParentOptions() {
    // Build lại cây positions
    const tree = buildTree(positionStore.positions)
    // Tìm node hiện tại đang edit
    const findNodeById = (nodes: any[], id: string): any | null => {
        for (const node of nodes) {
            if (node.id === id) return node
            const found = findNodeById(node.children, id)
            if (found) return found
        }
        return null
    }
    const editingId = formData.value.id
    let excludeIds: string[] = []
    if (editingId) {
        const currentNode = findNodeById(tree, editingId)
        excludeIds = currentNode
            ? [editingId, ...getAllChildIds(currentNode)]
            : [editingId]
    }

    // Filter lại options
    parentOptions.value = positionStore.positions.filter(
        p => !excludeIds.includes(p.id?.toString() || '')
    )
}
function closeModal() {
    emit('update:visible', false)
    emit('close')
}

function onSubmit() {
    const form = baseDialogRef.value?.formRef
    form.validate(async (valid: boolean) => {
        if (valid) {
            loading.value = true
            // Cập nhật lại departments trước khi submit
            // Cập nhật departmentPositions trước khi submit
            formData.value.departmentPositions = (formData.value.departmentIds || []).map(depId => ({
                departmentId: depId,
                positionId: formData.value.id || undefined // thêm nếu cần cho BE, không có cũng được khi thêm mới
            }))
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
</script>
