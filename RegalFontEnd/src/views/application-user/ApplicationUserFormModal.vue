<template>
  <div class="modal fade show d-block" tabindex="-1" aria-hidden="true" style="background-color: rgba(0, 0, 0, 0.5);"
    v-if="visible">
    <div class="modal-dialog modal-dialog-centered mw-650px">
      <div class="modal-content rounded">
        <!-- Header với màu động -->
        <div class="modal-header pb-0 border-0 justify-content-end position-relative"
          :class="isEdit ? 'modal-header-primary' : 'modal-header-success'">
          <h1 class="modal-title text-white w-100 text-center m-0 py-3">
            {{ isEdit ? t('applicationUser.editTitle') : t('applicationUser.addTitle') }}
          </h1>
          <button type="button"
            class="btn btn-sm btn-icon btn-active-color-primary position-absolute top-0 end-0 mt-2 me-2"
            @click="closeModal" style="z-index:2">
            <i class="bi bi-x fs-1 text-white"></i>
          </button>
        </div>
        <hr class="modal-title-divider" />
        <!-- Body -->
        <div class="modal-body scroll-y px-10 px-lg-15 pt-0 pb-15">

          <el-form ref="formRef" :model="formData" :rules="rules" @submit.prevent="onSubmit" class="form">
            <div class="mb-13 text-center">

            </div>
            <!-- Full Name -->
            <div class="d-flex flex-column mb-4 fv-row">
              <label class="required fs-6 fw-semibold mb-2">{{ t('applicationUser.fullName') }}</label>
              <el-form-item prop="fullName">
                <el-input v-model="formData.fullName" placeholder="" />
              </el-form-item>
            </div>
            <!-- Username -->
            <div class="d-flex flex-column mb-4 fv-row">
              <label class="required fs-6 fw-semibold mb-2">{{ t('applicationUser.userName') }}</label>
              <el-form-item prop="userName">
                <el-input v-model="formData.userName" placeholder="" />
              </el-form-item>
            </div>
            <!-- Email -->
            <div class="d-flex flex-column mb-4 fv-row">
              <label class="required fs-6 fw-semibold mb-2">{{ t('applicationUser.email') }}</label>
              <el-form-item prop="email">
                <el-input v-model="formData.email" placeholder="" />
              </el-form-item>
            </div>
            <!-- Phone Number -->
            <div class="d-flex flex-column mb-4 fv-row">
              <label class="fs-6 fw-semibold mb-2">{{ t('applicationUser.phoneNumber') }}</label>
              <el-input v-model="formData.phoneNumber" />
            </div>
            <!-- Gender -->
            <div class="d-flex flex-column mb-4 fv-row">
              <label class="fs-6 fw-semibold mb-2">{{ t('applicationUser.gender') }}</label>
              <el-radio-group v-model="formData.gender">
                <el-radio :value="true">{{ t('applicationUser.male') }}</el-radio>
                <el-radio :value="false">{{ t('applicationUser.female') }}</el-radio>
              </el-radio-group>
            </div>
            <!-- Employee ATID -->
            <div class="d-flex flex-column mb-4 fv-row">
              <label class="fs-6 fw-semibold mb-2">{{ t('applicationUser.userCode') }}</label>
              <el-input v-model="formData.userCode" />
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
                  {{ t('common.saving') }}
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

<script lang="ts" setup>
import { ref, watch, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import type { ApplicationUserModel } from '@/api/ApplicationUserApi'

const props = defineProps<{
  show: boolean
  user?: ApplicationUserModel | null
}>()
const emit = defineEmits(['update:show', 'saved', 'deleted'])

const { t } = useI18n()
const visible = ref(props.show)
watch(() => props.show, v => visible.value = v)
watch(visible, v => emit('update:show', v))

const isEdit = computed(() => !!props.user && !!props.user.id)

const formRef = ref()
const loading = ref(false)

const formData = ref<Partial<ApplicationUserModel>>({
  id: '',
  fullName: '',
  userName: '',
  email: '',
  phoneNumber: '',
  gender: true,
  avatar: null,
  avatarString: null,
  password: '',
  passwordConfirm: '',
  isDeleted: false,
  createdAt: null,
  lastModified: null,
  createdBy: null,
  lastModifiedBy: null,
  genderText: null,
  userCode: '',
  dateOfBirth: null,
  provinceCode: null
})

const DEFAULT_PASSWORD = import.meta.env.VITE_PASSWORD_DEFAULT || ''

watch(
  () => props.user,
  (user) => {
    if (user) {
      formData.value = { ...user }
      formData.value.password = ''
      formData.value.passwordConfirm = ''
    } else {
      formData.value = {
        id: '',
        fullName: 'Họ Tên',
        userName: 'Tên Đăng Nhập',
        email: 'email@example.com',
        phoneNumber: '0901234567',
        gender: true,
        avatar: null,
        avatarString: null,
        password: DEFAULT_PASSWORD, // Lấy từ biến môi trường
        passwordConfirm: DEFAULT_PASSWORD,
        isDeleted: false,
        createdAt: null,
        lastModified: null,
        createdBy: null,
        lastModifiedBy: null,
        genderText: null,
        userCode: '',
        dateOfBirth: null,
        provinceCode: null
      }
    }
  },
  { immediate: true }
)

const rules = {
  fullName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
  userName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
  email: [
    { required: true, message: t('validation.required'), trigger: 'blur' },
    { type: 'email', message: t('validation.email'), trigger: 'blur' }
  ],
  password: [
    { required: !isEdit.value, message: t('validation.required'), trigger: 'blur' }
  ],
  passwordConfirm: [
    { required: !isEdit.value, message: t('validation.required'), trigger: 'blur' },
    {
      validator: (_: any, value: string) => value === formData.value.password,
      message: t('validation.passwordConfirm'),
      trigger: 'blur'
    }
  ]
}

function closeModal() {
  visible.value = false
}

function onSubmit() {
  if (!formRef.value) return
  formRef.value.validate(async (valid: boolean) => {
    if (valid) {
      loading.value = true
      console.log('Form data:', formData.value);

      // Gọi API lưu hoặc emit event
      emit('saved', { ...formData.value })
      loading.value = false
      closeModal()
    }
  })
}

function onDelete() {
  emit('deleted', formData.value.id)
  closeModal()
}
</script>
