<template>
  <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
    :mode="props.mode" :loading="props.loading" :form-ref="formRef" @submit="onSubmit" @delete="onDelete"
    @update:visible="emit('update:visible', $event)" @close="closeModal">
    <template #form>
      <el-row :gutter="20">
        <el-col :span="24">
          <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('luckyDraw.name') }}</label>
          <el-form-item prop="name">
            <el-input v-model="formData.name" :disabled="isView" />
          </el-form-item>
        </el-col>

        <el-col :span="8">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('luckyDraw.branch') }}</label>
          <el-form-item prop="branch">
            <el-input v-model="formData.branch" :disabled="isView" />
          </el-form-item>
        </el-col>

        <el-col :span="8">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('luckyDraw.region') }}</label>
          <el-form-item prop="region">
            <el-input v-model="formData.region" :disabled="isView" />
          </el-form-item>
        </el-col>

        <el-col :span="8">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('luckyDraw.reportDate') }}</label>
          <el-form-item prop="reportDate">
            <el-date-picker v-model="formData.reportDate" type="date" placeholder="YYYY-MM-DD" :disabled="isView" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('luckyDraw.reporter') }}</label>
          <el-form-item prop="reporter">
            <el-input v-model="formData.reporter" :disabled="isView" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('luckyDraw.startDate') }}</label>
          <el-form-item prop="startDate">
            <el-date-picker v-model="formData.startDate" type="date" placeholder="YYYY-MM-DD" :disabled="isView" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('luckyDraw.endDate') }}</label>
          <el-form-item prop="endDate">
            <el-date-picker v-model="formData.endDate" type="date" placeholder="YYYY-MM-DD" :disabled="isView" />
          </el-form-item>
        </el-col>

        <el-col :span="12">
          <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
          <el-form-item prop="status">
            <el-radio-group v-model="formData.status" :disabled="isView">
              <el-radio :value="1">{{ t('common.active') }}</el-radio>
              <el-radio :value="0">{{ t('common.inactive') }}</el-radio>
            </el-radio-group>
          </el-form-item>
        </el-col>
      </el-row>
    </template>
  </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue';
import { useNotificationStore } from '@/stores/notificationStore';
import type { LuckyDrawModel } from '@/api/LuckyDrawApi';

const props = defineProps<{
  visible: boolean,
  mode?: 'create' | 'edit' | 'view',
  loading: boolean,
  luckyDrawData: Partial<LuckyDrawModel> | null
}>();

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close']);
const { t } = useI18n();
const notificationStore = useNotificationStore();
const formRef = ref();
const baseDialogRef = ref();

const isView = computed(() => props.mode === 'view');
const isEdit = computed(() => props.mode === 'edit');
const isCreate = computed(() => props.mode === 'create');

const formData = ref<Partial<LuckyDrawModel>>({
  id: '',
  name: '',
  branch: '',
  region: '',
  reportDate: '',
  reporter: '',
  startDate: '',
  endDate: '',
  status: 1,
});

const modeTitle = computed(() => {
  if (isView.value) return t('luckyDraw.detailTitle');
  if (isEdit.value) return t('luckyDraw.editTitle');
  return t('luckyDraw.addTitle');
});

const rules = {
  name: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
};

watch(
  () => props.luckyDrawData,
  (data) => {
    if (data) {
      formData.value = { ...data };
    } else {
      formData.value = {
        name: '', branch: '', region: '', reportDate: '', reporter: '', startDate: '', endDate: '', status: 1,
      };
    }
  },
  { immediate: true }
);

function onSubmit() {
  const form = baseDialogRef.value?.formRef;
  form.validate((valid: boolean) => {
    if (valid) {
      emit('submit', formData.value);
    } else {
      notificationStore.showToast('error', { key: 'validation.formInvalid' });
    }
  });
}

function onDelete() {
  emit('delete', formData.value);
}

function closeModal() {
  emit('update:visible', false);
  emit('close');
}
</script>
