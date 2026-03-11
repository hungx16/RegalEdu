<template>
  <div class="w-lg-500px p-10">
    <VForm id="kt_login_signin_form" class="form w-100" :validation-schema="login" :initial-values="{
      userName: 'admin@regaledu.vn',
      password: 'Admin@123',
      email: '',
      surname: '',
      accessToken: '',
      rememberMe: true
    }" @submit="onSubmitLogin">
      <div class="text-center mb-10">
        <h1 class="text-gray-900 mb-3">{{ t('auth.sign_in') }}</h1>
      </div>

      <!-- Username -->
      <div class="fv-row mb-10">
        <label class="form-label fs-6 fw-bold text-gray-900">{{ t('auth.username') }}</label>
        <Field tabindex="1" name="userName" type="text" autocomplete="off"
          class="form-control form-control-lg form-control-solid" />
        <ErrorMessage name="userName" class="fv-help-block text-danger mt-2" />
      </div>

      <!-- Password -->
      <div class="fv-row mb-10">
        <div class="d-flex flex-stack mb-2">
          <label class="form-label fw-bold text-gray-900 fs-6 mb-0">{{ t('auth.password') }}</label>
          <router-link to="/password-reset" class="link-primary fs-6 fw-bold">
            {{ t('auth.forgot_password') }}
          </router-link>
        </div>
        <Field tabindex="2" name="password" type="password" autocomplete="off"
          class="form-control form-control-lg form-control-solid" />
        <ErrorMessage name="password" class="fv-help-block text-danger mt-2" />
      </div>

      <!-- Submit -->
      <div class="text-center">
        <button id="kt_sign_in_submit" tabindex="3" type="submit"
          class="el-button el-button-login w-100 mb-5 h-10 btn-custom" :data-kt-indicator="isSubmitting ? 'on' : null"
          :disabled="isSubmitting">
          <span class="indicator-label">{{ t('auth.sign_in') }}</span>
          <span class="indicator-progress">
            {{ t('auth.please_wait') }}
            <span class="spinner-border spinner-border-sm align-middle ms-2"></span>
          </span>
        </button>
      </div>
    </VForm>
  </div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { ErrorMessage, Field, Form as VForm } from 'vee-validate';
import * as Yup from 'yup';
import Swal from 'sweetalert2';
import { useI18n } from 'vue-i18n';
import { useAuthStore } from '@/stores/useAuthStore';
import type { User } from '@/api/AuthApi';
import { userPermissionStore } from '@/stores/permissionStore';

export default defineComponent({
  name: 'sign-in',
  components: { VForm, Field, ErrorMessage },
  setup() {
    const { t } = useI18n();
    const router = useRouter();
    const route = useRoute();
    const store = useAuthStore();
    const isSubmitting = ref(false);

    const login = Yup.object().shape({
      userName: Yup.string().required(t('auth.username_required')),
      password: Yup.string().required(t('auth.password_required')),
    });

    const onSubmitLogin = async (values: any) => {
      if (isSubmitting.value) return;
      isSubmitting.value = true;

      try {
        store.logout();
        await store.login(values as User);

        const errorMsg = store.error;
        if (!errorMsg) {
          const permissionStore = userPermissionStore();
          await permissionStore.loadResource();

          await Swal.fire({
            text: t('auth.login_success'),
            icon: 'success',
            buttonsStyling: false,
            confirmButtonText: t('common.ok'),
            heightAuto: false,
            customClass: { confirmButton: 'el-button fw-semibold btn-light-primary' },
          });

          const returnUrl = (route.query.returnUrl as string) || '/dashboard';
          router.push(returnUrl);
        } else {
          await Swal.fire({
            text: String(errorMsg),
            icon: 'error',
            buttonsStyling: false,
            confirmButtonText: t('common.retry'),
            heightAuto: false,
            customClass: { confirmButton: 'el-button fw-semibold btn-light-danger' },
          });
          store.error = null;
        }
      } finally {
        isSubmitting.value = false;
      }
    };

    onMounted(() => {
      localStorage.clear();
      sessionStorage.clear();
      t;
    });

    return { t, login, onSubmitLogin, isSubmitting };
  },
});
</script>

<style scoped>
/* Ẩn progress mặc định; bật khi có data-kt-indicator="on" */
.indicator-progress {
  display: none;
}

[data-kt-indicator="on"] .indicator-label {
  display: none !important;
}

[data-kt-indicator="on"] .indicator-progress {
  display: inline-flex !important;
  align-items: center;
}
</style>
