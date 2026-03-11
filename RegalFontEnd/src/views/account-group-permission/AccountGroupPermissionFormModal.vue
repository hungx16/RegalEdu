<template>
  <div class="modal fade show d-block" tabindex="-1" aria-hidden="true" style="background-color: rgba(0, 0, 0, 0.5);"
    v-if="visible">
    <div class="modal-dialog modal-dialog-centered mw-650px">
      <div class="modal-content rounded">
        <div class="modal-header pb-0 border-0 justify-content-end position-relative"
          :class="isEdit ? 'modal-header-primary' : 'modal-header-success'">
          <h1 class="modal-title text-white w-100 text-center m-0 py-3">
            {{ isEdit ? t('accountGroupPermission.editTitle') : t('accountGroupPermission.addTitle') }}
          </h1>
          <button type="button"
            class="btn btn-sm btn-icon btn-active-color-primary position-absolute top-0 end-0 mt-2 me-2"
            @click="closeModal" style="z-index:2">
            <i class="bi bi-x fs-1 text-white"></i>
          </button>
        </div>
        <hr class="modal-title-divider" />
        <div class="modal-body scroll-y px-10 px-lg-15 pt-0 pb-15">
          <el-form ref="formRef" :model="formData" :rules="rules" @submit.prevent="onSubmit" class="form">
            <div class="d-flex flex-column mb-4 fv-row">
              <label class="required fs-6 fw-semibold mb-2">{{ t('accountGroupPermission.formName') }}</label>
              <el-form-item prop="formName">
                <el-input v-model="formData.formName" />
              </el-form-item>
            </div>
            <div class="d-flex flex-column mb-4 fv-row">
              <label class="required fs-6 fw-semibold mb-2">{{ t('accountGroupPermission.action') }}</label>
              <el-form-item prop="action">
                <el-input v-model="formData.action" />
              </el-form-item>
            </div>
            <div class="mb-4">
              <el-checkbox v-model="formData.allowAction">{{ t('accountGroupPermission.allowAction') }}</el-checkbox>
            </div>
            <div class="text-center">
              <button type="button" class="btn btn-light me-3" @click="closeModal">
                {{ t('common.cancel') }}
              </button>
              <button class="btn btn-primary" type="submit" :disabled="loading">
                <span v-if="!loading">{{ t('common.save') }}</span>
                <span v-else>
                  <span class="spinner-border spinner-border-sm align-middle me-2"></span>
                  {{ t('common.loading') }}
                </span>
              </button>
            </div>
          </el-form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import type { AccountGroupPermissionModel } from '@/api/accountGroupPermissionApi';

const props = defineProps<{ show: boolean; permission?: AccountGroupPermissionModel | null }>();
const emit = defineEmits(['update:show', 'saved']);

const { t } = useI18n();
const visible = ref(props.show);
watch(() => props.show, v => visible.value = v);
watch(visible, v => emit('update:show', v));

const isEdit = computed(() => !!props.permission && !!props.permission.id);
const formRef = ref();
const loading = ref(false);

const formData = ref<AccountGroupPermissionModel>({
  id: '',
  formName: '',
  action: '',
  allowAction: false
});

watch(() => props.permission, (val) => {
  formData.value = val ? { ...val } : { formName: '', action: '', allowAction: false };
}, { immediate: true });

const rules = {
  formName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
  action: [{ required: true, message: t('validation.required'), trigger: 'blur' }]
};

function closeModal() {
  visible.value = false;
}

function onSubmit() {
  if (!formRef.value) return;
  formRef.value.validate(async (valid: boolean) => {
    if (valid) {
      loading.value = true;
      emit('saved', { ...formData.value });
      loading.value = false;
      closeModal();
    }
  });
}
</script>
