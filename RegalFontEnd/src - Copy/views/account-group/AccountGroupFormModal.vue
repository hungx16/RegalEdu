<template>
  <BaseDialogForm :visible="show" @update:visible="(v) => emit('update:show', v)"
    :title="isEdit ? t('accountGroup.editTitle') : t('accountGroup.addTitle')" :mode="isEdit ? 'edit' : 'create'"
    :description="''" :formData="formData" :rules="rules" :loading="loading" :submitDisabled="false"
    :showDelete="isEdit" :height="480" @submit="onSubmit" @delete="onDelete" @close="closeModal">
    <!-- BODY FORM theo slot API: { mode, formData } -->
    <template #form="{ formData: fd = {} }">
      <!-- Name -->
      <el-col :span="24">
        <label class="required fs-6 fw-semibold mb-2">{{ t('accountGroup.name') }}</label>
        <el-form-item prop="name" class="w-100">
          <el-input v-model="fd.name" placeholder="" />
        </el-form-item>
      </el-col>
      <el-col :span="24">
        <label class="fs-6 fw-semibold mb-2 d-block">{{ t('accountGroup.enable') }}</label>
        <el-form-item prop="status">
          <el-radio-group v-model="fd.enable">
            <el-radio :value="true">{{ t('accountGroup.isEnabled') }}</el-radio>
            <el-radio :value="false">{{ t('accountGroup.isNotEnabled') }}</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-col>
      <!-- Use Default -->

      <el-col :span="24">
        <label class="fs-6 fw-semibold mb-2 d-block">{{ t('accountGroup.useDefault') }}</label>
        <el-form-item prop="status">
          <el-radio-group v-model="fd.useDefault">
            <el-radio :value="true">{{ t('accountGroup.isDefault') }}</el-radio>
            <el-radio :value="false">{{ t('accountGroup.isNotDefault') }}</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-col>
      <!-- Note -->
      <el-col :span="24">
        <label class="fs-6 fw-semibold mb-2">{{ t('accountGroup.note') }}</label>
        <el-input type="textarea" v-model="fd.note" />
      </el-col>
    </template>
  </BaseDialogForm>
</template>

<script lang="ts" setup>
import { ref, watch, computed } from 'vue'
import { useI18n } from 'vue-i18n'
// ⚠️ Đổi path cho đúng vị trí file của anh:
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { AccountGroupModel } from '@/api/AccountGroupApi'

const props = defineProps<{
  show: boolean
  group?: AccountGroupModel | null
}>()

const emit = defineEmits(['update:show', 'saved', 'deleted'])
const { t } = useI18n()

const isEdit = computed(() => !!props.group?.id)
const loading = ref(false)

/** Dữ liệu form truyền vào BaseDialogForm */
type GroupForm = {
  id?: string
  name: string
  enable: boolean
  useDefault: boolean
  note: string
}
const formData = ref<GroupForm>({
  id: '',
  name: '',
  enable: true,
  useDefault: false,
  note: '',
})

/** Đồng bộ từ props.group (giữ đúng hành vi bản cũ) */
watch(
  () => props.group,
  (group) => {
    if (group) {
      formData.value = {
        id: group.id ?? '',
        name: group.name ?? '',
        enable: group.enable ?? true,
        useDefault: group.useDefault ?? false,
        note: group.note ?? '',
      }
    } else {
      formData.value = {
        name: '',
        enable: true,
        useDefault: false,
        note: '',
      }
    }
  },
  { immediate: true }
)

/** Rules validate */
const rules = {
  name: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
}

/** BaseDialogForm đã validate trước khi emit @submit */
async function onSubmit(fd: GroupForm) {
  try {
    loading.value = true
    emit('saved', { ...fd } as AccountGroupModel)
    // Không tự close ở đây để cha chủ động đóng sau khi save OK (giữ nguyên flow hiện tại).
    // Nếu muốn tự đóng ngay: emit('update:show', false)
  } finally {
    loading.value = false
  }
}

function onDelete() {
  if (!formData.value.id) return
  emit('deleted', formData.value.id)
}

function closeModal() {
  emit('update:show', false)
}
</script>


<style scoped>
.required::after {
  content: ' *';
  color: var(--el-color-danger);
}

.w-100 {
  width: 100%;
}
</style>
