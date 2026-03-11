<template>
  <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
    :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
    @update:visible="emit('update:visible', $event)" @close="closeModal">
    <template #form>
      <el-row :gutter="20">
        <!-- Bật song ngữ -->
        <el-col :span="24">
          <el-form-item>
            <el-checkbox v-model="formData.isMultilingual" :disabled="isView">
              {{ t('common.allowMultilingual') }}
            </el-checkbox>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('partnerType.code') }}</label>
          <el-form-item prop="partnerTypeCode">
            <el-input v-model="formData.partnerTypeCode" :disabled="isView" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('partnerType.name') }}</label>
          <el-form-item prop="partnerTypeName">
            <el-input v-model="formData.partnerTypeName" :disabled="isView"
              :placeholder="t('partnerType.namePlaceholder')" />
          </el-form-item>
        </el-col>
        <!-- English Partner Name -->
        <el-col :span="24" v-if="formData.isMultilingual">
          <label class="required fs-6 fw-semibold mb-2 d-block">English Partner Name</label>
          <el-form-item prop="enPartnerTypeName">
            <el-input v-model="formData.enPartnerTypeName" :disabled="isView" :maxlength="200" show-word-limit />
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
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('partnerType.description') }}</label>
          <el-form-item prop="description">
            <el-input type="textarea" v-model="formData.description" :rows="2" :disabled="isView"
              :placeholder="t('partnerType.descriptionPlaceholder')" />
          </el-form-item>
        </el-col>

        <el-col :span="12" v-if="isView">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdBy') }}</label>
          <BaseBadge :label="formData.createdBy || ''" color="purple" />
        </el-col>

        <el-col :span="12" v-if="isView">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.createdAt') }}</label>
          <el-form-item>
            <el-input :value="formatDate(formData.createdAt || '', 'YYYY-MM-DD HH:mm:ss')" :disabled="true" />
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
import type { PartnerTypeModel } from '@/api/PartnerTypeApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useCommonStore } from '@/stores/commonStore'
import { formatDate } from '@/utils/format'
import BaseBadge from '@/components/info-badge/BaseBadge.vue'

const props = defineProps<{
  visible: boolean,
  mode?: 'create' | 'edit' | 'view',
  loading: boolean,
  partnerTypeData: Partial<PartnerTypeModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const notificationStore = useNotificationStore()
const commonStore = useCommonStore()
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')

const modeTitle = computed(() => {
  if (isView.value) return t('partnerType.detailTitle')
  if (isEdit.value) return t('partnerType.editTitle')
  if (isCreate.value) {
    formData.value.partnerTypeCode = commonStore.code ?? ''
  }
  return t('partnerType.addTitle')
})

const formRef = ref()

const formData = ref<PartnerTypeModel>({
  id: '',
  partnerTypeCode: '',
  partnerTypeName: '',
  status: 0,
  description: '',
  createdAt: '',
  isDeleted: false,
  isMultilingual: false,
  enPartnerTypeName: '',
})

watch(
  () => props.partnerTypeData,
  (data) => {
    if (data) {
      formData.value = {
        id: data.id ?? '',
        partnerTypeCode: data.partnerTypeCode ?? '',
        partnerTypeName: data.partnerTypeName ?? '',
        status: data.status ?? 0,
        description: data.description ?? '',
        createdAt: data.createdAt ?? '',
        createdBy: data.createdBy ?? '',
        enPartnerTypeName: data.enPartnerTypeName ?? '',
        isMultilingual: data.isMultilingual ?? false,
      }
    } else {
      formData.value = {
        partnerTypeCode: '',
        partnerTypeName: '',
        status: 0,
        description: '',
        isMultilingual: false,
        enPartnerTypeName: '',
      }
    }
  },
  { immediate: true }
)

const rules = {
  partnerTypeCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
  partnerTypeName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
  status: [{ required: true, message: t('validation.required'), trigger: 'change' }],
  enPartnerTypeName: [
    {
      required: true,
      message: t('validation.required'),
      trigger: 'blur'
    }
  ]
}

const baseDialogRef = ref()

function closeModal() {
  emit('update:visible', false)
  emit('close')
}

function onSubmit() {
  const form = baseDialogRef.value?.formRef
  form.validate(async (valid: boolean) => {
    if (valid) {
      emit('submit', formData.value)
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
