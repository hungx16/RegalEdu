<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="loading" @submit="onSubmit" @delete="onDelete"
        @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <h5 class="mb-4">{{ t('receipt.formDesc') }}</h5>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('receipt.type') }}</label>
                    <el-form-item prop="receiptType">
                        <el-select v-model="formData.receiptType" :disabled="isView" class="w-100">
                            <el-option :label="t('receipt.receiptType.order')" :value="ReceiptType.Order" />
                            <el-option :label="t('receipt.receiptType.additional')" :value="ReceiptType.Additional" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class=" required fs-6 fw-semibold mb-2 d-block">{{ t('receipt.code') }}</label>
                    <el-form-item prop="receiptCode">
                        <el-input v-model="formData.receiptCode" :disabled="isView"
                            :placeholder="t('receipt.codePlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('receipt.registrationCode') }}</label>
                    <el-form-item prop="registerCode">
                        <el-input v-model="formData.registerCode" :disabled="isView"
                            :placeholder="t('receipt.registrationCodePlaceholder')" @change="onRegisterCodeChange" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('receipt.student') }}</label>
                    <el-form-item prop="studentId">
                        <el-select v-model="formData.studentId" :disabled="isView" filterable clearable
                            :placeholder="t('receipt.studentPlaceholder')" class="w-100">
                            <el-option v-for="student in studentList" :key="student.id" :label="student.name"
                                :value="student.id" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('receipt.course') }}</label>
                    <el-form-item prop="courseId">
                        <el-select v-model="formData.courseId" :disabled="isView" filterable clearable
                            :placeholder="t('receipt.coursePlaceholder')" class="w-100">
                            <el-option v-for="course in courseList" :key="course.id" :label="course.name"
                                :value="course.id" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('receipt.paymentTermType') }}</label>
                    <el-form-item prop="paymentTermType">
                        <el-select v-model="formData.paymentType" :disabled="isView" class="w-100">
                            <el-option :label="t('registerStudy.payDirect')" :value="PaymentType.Direct" />
                            <el-option :label="t('registerStudy.payInstallment')" :value="PaymentType.Installment" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('receipt.paymentMethodType') }}</label>
                    <el-form-item prop="paymentMethodType">
                        <el-select v-model="formData.paymentMethodType" :disabled="isView" class="w-100">
                            <el-option :label="t('registerStudy.payOnce')" :value="PaymentMethodType.OneTime" />
                            <el-option :label="t('registerStudy.payMultiple')" :value="PaymentMethodType.Multiple" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('receipt.paymentMethod') }}</label>
                    <el-form-item prop="paymentMethod">
                        <el-select v-model="formData.paymentMethod" :disabled="isView" class="w-100">
                            <el-option :label="t('receipt.PaymentMethod.cash')" :value="PaymentMeThod.Cash" />
                            <el-option :label="t('receipt.PaymentMethod.vnPay')" :value="PaymentMeThod.VnPay" />
                            <el-option :label="t('receipt.PaymentMethod.transfer')" :value="PaymentMeThod.Transfer" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('receipt.totalAmount') }}</label>
                    <el-form-item prop="totalAmount">
                        <el-input-number v-model="formData.totalAmount" :min="0" :disabled="isView" :controls="false"
                            :placeholder="t('receipt.totalAmountPlaceholder')" class="w-100"
                            @change="onTotalAmountChange" />
                    </el-form-item>
                </el-col>
                <!-- Bổ sung một cột mã thấu chi vào đây -->
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('receipt.discountCode') }}</label>
                    <el-form-item prop="discountCode">
                        <el-input v-model="formData.discountCode" :disabled="isView" placeholder="Nhập mã thấu chi" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('receipt.advisor') }}</label>
                    <el-form-item prop="employeeId">
                        <el-select v-model="formData.employeeId" :disabled="isView" filterable clearable
                            :placeholder="t('receipt.advisorPlaceholder')" class="w-100">
                            <el-option v-for="employee in employeeList" :key="employee.id" :label="employee.name"
                                :value="employee.id" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('receipt.note') }}</label>
                    <el-form-item prop="note">
                        <el-input v-model="formData.note" type="textarea" :rows="2" :disabled="isView"
                            :placeholder="t('receipt.notePlaceholder')" />
                    </el-form-item>
                </el-col>
            </el-row>
        </template>

        <template #footer>
            <el-button @click="emit('update:visible', false)">{{ t('common.cancel') }}</el-button>
            <el-button v-if="!isView" type="primary" @click="onSubmit">{{ t('common.create') }}</el-button>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { ReceiptsModel } from '@/api/ReceiptApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useReceiptStore } from '@/stores/receiptStore'
import { PaymentMeThod, PaymentMethodType, PaymentStatus, PaymentType, ReceiptType } from '@/types'
//import đăng ký
import { useRegisterStudyStore } from '@/stores/registerStudyStore'
//import khóa học
import { useCourseStore } from '@/stores/courseStore'
//import học viên
import { useStudentStore } from '@/stores/studentStore'
//import nhân viên
import { useEmployeeStore } from '@/stores/employeeStore'
//lấy danh sách nhân viên
const employeeStore = useEmployeeStore();
const employeeList = computed(() => employeeStore.employees.map(employee => ({
    id: employee.id,
    name: employee.applicationUser?.fullName || ''
})));

//lấy danh sách học viên
const studentStore = useStudentStore();
const studentList = computed(() => studentStore.students.map(student => ({
    id: student.id,
    name: student.fullName
})));
//hãy viết phương thức mở  form dialog

//lấy danh sách các khóa học
const courseStore = useCourseStore();
const courseList = computed(() => courseStore.courses.map(course => ({
    id: course.id,
    name: course.courseName
})));
//khai báo store đăng ký
const registerStudyStore = useRegisterStudyStore();
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    receiptData: Partial<ReceiptsModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close'])
const { t } = useI18n()
const receiptStore = useReceiptStore()
const notificationStore = useNotificationStore()

const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')

const modeTitle = computed(() => {
    if (isView.value) return t('receipt.detailTitle')
    if (isEdit.value) return t('receipt.editTitle')
    return t('receipt.addTitle')
})

const baseDialogRef = ref()
const loading = ref(false)

const defaultFormData: Partial<ReceiptsModel> = {
    receiptType: ReceiptType.Order,
    paymentType: PaymentType.Direct,
    paymentMethodType: PaymentMethodType.OneTime,
    paymentMethod: PaymentMeThod.Cash,
    totalAmount: null,
    status: PaymentStatus.Paid, // Mặc định là Đã xác nhận/Mới tạo
    discountCode: '',
}

const formData = ref<Partial<ReceiptsModel>>({ ...defaultFormData })

onMounted(() => {
    //fetch danh sách học viên
    studentStore.fetchAllStudents();
    //fetch danh sách đăng ký học
    registerStudyStore.fetchAllRegisterStudies();
    //fetch danh sách khóa học
    courseStore.fetchAllCourses();
    //fetch danh sách nhân viên
    employeeStore.fetchAllEmployees();
})
watch(
    () => props.receiptData,
    (data) => {
        if (data && data.id) {
            formData.value = { ...data } as Partial<ReceiptsModel>;
        } else {
            formData.value = { ...defaultFormData }
        }
    },
    { immediate: true }
)
const remainingTuitionFees = ref<number | null>(null);
// Handler for registration code input change (defined so the template @change="onChange" is valid)
function onRegisterCodeChange(value?: string): void {
    // v-model already updates formData.registerStudyId; keep in sync if value provided
    if (value !== undefined) {
        //tìm mã đăng ký học trong registerStore nếu thấy thì trả lại id nếu không có thì báo lỗi yêu cầu nhập lại
        const found = registerStudyStore.registerStudies.find(item => item.code === value);
        if (found) {
            formData.value.registerStudyId = found.id;
            formData.value.studentName = found.studentFullName;
            formData.value.studentId = found.studentId;
            formData.value.courseId = found.detailRegisterStudys?.[0]?.courseId;
            formData.value.employeeId = found.employeeId;
            formData.value.paymentMethod = found.paymentMethod;
            formData.value.paymentType = found.paymentType;
            formData.value.registerCode = found.code;
            formData.value.paymentMethodType = found.paymentMethodType;
            formData.value.totalAmount = found.remainingTuitionFees ?? 0;
            remainingTuitionFees.value = found.remainingTuitionFees ?? 0;
            formData.value.note = `Thanh toán cho đăng ký học mã ${found.code}`;
            formData.value.receiptType = ReceiptType.Order;
            //nếu số tiền thanh toán bằng 0 thì thống báo không thể tạo phiếu thu
            if ((found.remainingTuitionFees ?? 0) <= 0) {
                notificationStore.showToast('error', { key: 'validation.receiptCannotBeCreated' });
                //trả lại valider để yêu cầu nhập lại
                formData.value.registerCode = undefined;
            }

        } else {

            notificationStore.showToast('error', { key: 'validation.registerCodeNotFound' });
            //trả lại valider để yêu cầu nhập lại
            formData.value.registerCode = undefined;
        }
    }
    // Add additional side effects here if needed (e.g., fetch student list)
}

function onTotalAmountChange(value?: number): void {
    // Keep formData.totalAmount in sync and prevent setting a value larger than remaining tuition fees.
    if (typeof value === 'number') {
        if (remainingTuitionFees.value !== null && value > remainingTuitionFees.value) {
            notificationStore.showToast('error', { key: 'validation.totalAmountExceedsRemainingTuitionFees' });
            // Reset to the allowed remaining amount to avoid invalid state
            formData.value.totalAmount = remainingTuitionFees.value;
            formData.value.status = PaymentStatus.Paid;
        } else if (value === remainingTuitionFees.value) {
            formData.value.status = PaymentStatus.Paid;
            formData.value.totalAmount = value;
        } else {
            formData.value.status = PaymentStatus.PartiallyPaid;
            formData.value.totalAmount = value;
        }
    } else {
        formData.value.totalAmount = value ?? null;
    }
}

const rules = {
    //yêu cầu phải nhập mã phiếu thu
    receiptCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    receiptType: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    registerCode: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    studentId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    paymentType: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    paymentMethodType: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    paymentMethod: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    totalAmount: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
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
            //kiểm tra nếu số tiền thanh toán mà lớn hơn số tiền còn lại thì báo lỗi
            if (remainingTuitionFees.value !== null && typeof formData.value.totalAmount === 'number') {
                if (formData.value.totalAmount > remainingTuitionFees.value) {
                    notificationStore.showToast('error', { key: 'validation.totalAmountExceedsRemainingTuitionFees' })
                    loading.value = false
                    return
                }
            }
            emit('submit', formData.value)
            loading.value = false
        } else {
            notificationStore.showToast('error', { key: 'validation.formInvalid' })
        }
    })
}
function onDelete() {
    emit('delete', formData.value)
}
</script>