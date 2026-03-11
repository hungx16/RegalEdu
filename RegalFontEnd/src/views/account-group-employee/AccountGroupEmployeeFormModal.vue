<template>
  <div class="modal fade show d-block" tabindex="-1" aria-hidden="true" style="background-color: rgba(0, 0, 0, 0.5);"
    v-if="visible">
    <div class="modal-dialog modal-dialog-centered mw-650px">
      <div class="modal-content rounded">
        <!-- Header -->
        <div class="modal-header pb-0 border-0 justify-content-end position-relative"
          :class="isEdit ? 'modal-header-primary' : 'modal-header-success'">
          <h1 class="modal-title text-white w-100 text-center m-0 py-3">
            {{ isEdit ? t('accountGroupEmployee.editTitle') : t('accountGroupEmployee.addTitle') }}
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
            <!-- userCode -->
            <div class="d-flex flex-column mb-4 fv-row">
              <label class="required fs-6 fw-semibold mb-2">{{ t('accountGroupEmployee.userCode') }}</label>
              <el-form-item prop="userCode">
                <el-select v-model="formData.userCode" filterable placeholder="Select employee">
                  <el-option v-for="emp in employeeOptions" :key="emp" :label="emp" :value="emp" />
                </el-select>
              </el-form-item>
            </div>
            <!-- Actions -->
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
              <button v-if="isEdit" type="button" class="btn btn-danger ms-3" @click="onDelete" :disabled="loading">
                {{ t('common.delete') }}
              </button>
            </div>
          </el-form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import type { AccountGroupEmployeeModel } from '@/api/accountGroupEmployeeApi';
import { serviceFactory } from '@/services/ServiceFactory';

const props = defineProps<{ show: boolean; employee?: AccountGroupEmployeeModel | null; groupId: string }>();
const emit = defineEmits(['update:show', 'saved', 'deleted']);
const { t } = useI18n();

const visible = ref(props.show);
watch(() => props.show, v => visible.value = v);
watch(visible, v => emit('update:show', v));

const isEdit = computed(() => !!props.employee?.id);
const formRef = ref();
const loading = ref(false);

const formData = ref<AccountGroupEmployeeModel>({
  id: '',
  accountGroupId: props.groupId,
  userCode: '',
  accountGroup: undefined,
  isApprover: false
});

const employeeOptions = ref<string[]>([]);

onMounted(fetchAvailableEmployees);

async function fetchAvailableEmployees() {
  const result = await serviceFactory.accountGroupEmployeeService.getEmployeeNoGroup();
  if (result?.succeeded) {
    employeeOptions.value = result.data;
  }
}

watch(() => props.employee, (val) => {
  formData.value = val
    ? { ...val }
    : {
      id: '',
      accountGroupId: props.groupId,
      userCode: '',
      accountGroup: undefined,
      isApprover: false
    };
}, { immediate: true });

const rules = {
  userCode: [{ required: true, message: t('validation.required'), trigger: 'change' }]
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

function onDelete() {
  emit('deleted', formData.value.id);
  closeModal();
}
</script>
