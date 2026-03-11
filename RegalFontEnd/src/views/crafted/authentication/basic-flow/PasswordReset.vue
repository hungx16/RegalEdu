<template>
  <!--begin::Wrapper-->
  <div class="w-lg-500px p-10">
    <!--begin::Form-->
    <VForm id="kt_login_password_reset_form" class="form w-100 fv-plugins-bootstrap5 fv-plugins-framework"
      :validation-schema="forgotPassword" :initial-values="{ email: '' }" @submit="onSubmitForgotPassword">
      <!--begin::Heading-->
      <div class="text-center mb-10">
        <h1 class="text-gray-900 mb-3">{{ t('auth.forgot_password') }}</h1>
        <div class="text-gray-500 fw-semibold fs-4">
          {{ t('auth.enter_email_reset') }}
        </div>
      </div>
      <!--end::Heading-->

      <!--begin::Input group-->
      <div class="fv-row mb-10">
        <label class="form-label fw-bold text-gray-900 fs-6">{{ t('auth.email') }}</label>
        <Field class="form-control form-control-solid" type="email" name="email" autocomplete="off" />
        <div class="fv-plugins-message-container">
          <div class="fv-help-block">
            <ErrorMessage name="email" />
          </div>
        </div>
      </div>
      <!--end::Input group-->

      <!--begin::Actions-->
      <div class="d-flex flex-wrap justify-content-center pb-lg-0">
        <button id="kt_password_reset_submit" type="submit" class="btn-custom el-button el-button-login me-4 w-50"
          :data-kt-indicator="isSubmitting ? 'on' : null" :disabled="isSubmitting">
          <span class="indicator-label">{{ t('auth.submit') }}</span>
          <span class="indicator-progress">
            {{ t('auth.please_wait') }}
            <span class="spinner-border spinner-border-sm align-middle ms-2"></span>
          </span>
        </button>

        <router-link to="/sign-in" class="btn-custom el-button el-button-login w-25">
          {{ t('auth.cancel') }}
        </router-link>
      </div>
      <!--end::Actions-->
    </VForm>
    <!--end::Form-->
  </div>
  <!--end::Wrapper-->
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import { ErrorMessage, Field, Form as VForm } from "vee-validate";
import * as Yup from "yup";
import Swal from "sweetalert2/dist/sweetalert2.js";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/useAuthStore";
import { useI18n } from "vue-i18n";
export default defineComponent({
  name: "password-reset",
  components: { Field, VForm, ErrorMessage },
  setup() {
    const store = useAuthStore();
    const router = useRouter();
    const isSubmitting = ref(false);
    const { t } = useI18n();
    const forgotPassword = Yup.object({
      email: Yup.string().email().required(t('auth.email_required')),
    });

    const onSubmitForgotPassword = async (values) => {
      if (isSubmitting.value) return;
      isSubmitting.value = true;

      await store.forgotPassword({ email: values.email });
      const error = store.error;

      if (!error) {
        await Swal.fire({
          text: t('auth.please_check_email'),
          icon: "success",
          buttonsStyling: false,
          confirmButtonText: t('auth.ok'),
          heightAuto: false,
          customClass: { confirmButton: "el-button fw-semibold btn-light-primary" },
        });
        router.push("/sign-in");
      } else {
        await Swal.fire({
          text: String(error),
          icon: "error",
          buttonsStyling: false,
          confirmButtonText: t('auth.try_again'),
          heightAuto: false,
          customClass: { confirmButton: "el-button fw-semibold btn-light-danger" },
        });
      }

      isSubmitting.value = false;
    };

    return { forgotPassword, onSubmitForgotPassword, isSubmitting, t };
  },
});
</script>

<style scoped>
/* Căn giữa nội dung trong button, đồng nhất chiều cao */


/* ------ Indicator (ẩn/hiện đúng chuẩn) ------ */
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
