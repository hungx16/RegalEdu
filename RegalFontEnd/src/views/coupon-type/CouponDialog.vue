<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="t('coupon.createCoupon')" :form-data="formData"
        :rules="rules" :width="'40%'" @update:visible="emit('update:visible', $event)" @submit="onSubmit">

        <template #form>
            <h5 class="fw-semibold mb-4">{{ t('coupon.issueInfo') }}</h5>
            <el-row :gutter="20">
                <el-col :span="24">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('coupon.issueType') }}</label>
                    <el-form-item prop="issueType">
                        <el-select v-model="formData.issueType" class="w-100">
                            <el-option :label="t('coupon.type.quantity')" :value="IssueType.Quantity" />
                            <el-option :label="t('coupon.type.selectedStudent')" :value="IssueType.SelectedStudent" />
                        </el-select>
                    </el-form-item>

                </el-col>

                <el-col :span="24" v-if="formData.issueType === IssueType.Quantity">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('coupon.quantity') }}</label>
                    <el-form-item prop="quantity">
                        <el-input-number v-model="formData.quantity" :min="1" :controls="false" class="w-100" />
                    </el-form-item>
                </el-col>

                <el-col :span="24">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('coupon.issueDate') }}</label>
                    <el-form-item prop="issueDate">
                        <el-date-picker v-model="formData.issueDate" type="date" format="DD/MM/YYYY"
                            value-format="YYYY-MM-DD" class="w-100" />
                    </el-form-item>
                </el-col>

                <el-col :span="24" v-if="formData.issueType === IssueType.SelectedStudent">
                    <el-divider class="my-4" />
                    <h5 class="fw-semibold mb-3">{{ t('coupon.studentScope') }}</h5>
                </el-col>

                <el-col :span="24" v-if="formData.issueType === IssueType.SelectedStudent">
                    <el-checkbox v-model="formData.isForAllStudents" :disabled="true">
                        {{ t('coupon.allStudents') }}
                    </el-checkbox>
                    <span class="ms-3 text-primary">{{ t('coupon.studentsMatched') }}</span>
                </el-col>

                <el-col :span="24" v-if="!props.isForAllStudents && formData.issueType === IssueType.SelectedStudent">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('coupon.selectStudents') }}</label>
                    <el-form-item prop="selectedStudents">
                        <el-select v-model="formData.selectedStudents" multiple filterable class="w-100">
                            <el-option v-for="student in studentStore.students" :key="student.id"
                                :label="student.fullName + ' (' + student.email + ')'" :value="student.id" />
                        </el-select>
                    </el-form-item>
                </el-col>
            </el-row>
        </template>

        <!-- <template #footer>
            <el-button @click="emit('update:visible', false)">{{ t('common.cancel') }}</el-button>
            <el-button type="primary" :loading="issueStore.loading" @click="onSubmit">
                {{ t('common.confirm') }}
            </el-button>
        </template> -->
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue';
// Fallback placeholder for the coupon issue store when the actual module is missing.
// Replace with: import { useCouponIssueStore } from '@/stores/couponIssueStore';
import { useNotificationStore } from '@/stores/notificationStore';
//khai báo coupon issue model
import type { CouponIssueModel } from '@/api/CouponIssueApi';
import { IssueType } from '@/types';
//sử dụng student store
import { useStudentStore } from '@/stores/studentStore';
import { useCouponIssueStore } from '@/stores/couponIssueStore';
const issueStore = useCouponIssueStore();
const studentStore = useStudentStore();

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close', 'success', 'activated', 'opennewdialog']);

const props = defineProps<{
    visible: boolean;
    couponTypeId: string | null; // ID của CouponType
    isForAllStudents: boolean; // Phạm vi áp dụng từ CouponType
    studentIds?: string | null; // Danh sách học viên cụ thể (nếu có)
}>();
const { t } = useI18n();

// Local fallback issueStore to avoid build errors when the actual store module is missing.
// This provides the minimal API used by this component: `loading` and `createIssue`.
// Replace this block with `const issueStore = useCouponIssueStore();` when the real store exists.

onMounted(() => {
    // Nếu cần load dữ liệu học viên cụ thể
    studentStore.fetchAllStudents();
    if (!props.isForAllStudents && props.studentIds) {
        const ids = props.studentIds.split(',');
        //lọc danh sách sin viên thỏa mã điều kiện từ studentStore.students
        studentStore.students = studentStore.students.filter(student => ids.includes(student.id as string));
    }
});
const notificationStore = useNotificationStore();

const formRef = ref();

const defaultFormData = (): Partial<CouponIssueModel & { selectedStudents: string[] }> => {
    const selectedStudents = props.studentIds ? props.studentIds.split(',') : [];
    return {
        couponTypeId: props.couponTypeId,
        issueType: IssueType.Quantity,
        quantity: 50,
        issueDate: new Date().toISOString().substring(0, 10), // Ngày hôm nay
        isForAllStudents: props.isForAllStudents,
        selectedStudents: selectedStudents,
        couponIssueStudent: selectedStudents.map(studentId => ({ studentId }))
    };
};

const formData = ref(defaultFormData());

watch(() => props.visible, (val) => {
    if (val) {
        formData.value = defaultFormData();
        console.log('CouponDialog opened, formData reset:', formData.value);
        studentStore.fetchAllStudents();
        if (!props.isForAllStudents && props.studentIds) {
            const ids = props.studentIds.split(',');
            //lọc danh sách sin viên thỏa mã điều kiện từ studentStore.students
            studentStore.students = studentStore.students.filter(student => ids.includes(student.id as string));
        }
    }
});

const rules = {
    quantity: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    issueDate: [{ required: true, message: t('validation.required'), trigger: 'change' }],
};
async function onSubmit() {
    console.log('Submitting form with data:', formData.value);

    // formRef.value?.validate(async (valid: boolean) => {
    //     if (valid) {
    console.log('Form is valid, preparing to submit...');
    try {
        const payload: Partial<CouponIssueModel> = {
            ...formData.value,
            couponTypeId: props.couponTypeId,
            isForAllStudents: props.isForAllStudents,
            // Nếu là học viên cụ thể, cần tạo mảng CouponIssueStudent
            couponIssueStudent: !props.isForAllStudents && formData.value.selectedStudents
                ? formData.value.selectedStudents.map(studentId => ({ studentId }))
                : null
        };

        await issueStore.saveCouponIssue(payload);

        emit('update:visible', false);
        emit('success'); // Thông báo cho CouponTypeDialog refresh
    } catch (error: any) {
        notificationStore.showToast('error', { key: 'toast.error', params: { message: error.message || 'Lỗi phát hành coupon' } });
    }
    // }
    // });
}
</script>