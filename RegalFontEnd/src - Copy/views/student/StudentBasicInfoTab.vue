<template>
    <el-row :gutter="20">
        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.code') }}</label>
            <el-form-item prop="studentCode">
                <el-input v-model="formData.studentCode" :disabled="isView || isEdit"
                    :placeholder="t('student.codePlaceholder')" />
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.name') }}</label>
            <el-form-item prop="fullName">
                <el-input v-model="formData.fullName" :disabled="isView" :placeholder="t('student.namePlaceholder')" />
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.englishName') }}</label>
            <el-form-item prop="englishName">
                <el-input v-model="formData.englishName" :disabled="isView"
                    :placeholder="t('student.englishNamePlaceholder')" />
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.gender') }}</label>
            <el-form-item prop="gender">
                <el-select v-model="formData.gender" :disabled="isView" class="w-100">
                    <el-option :label="t('common.male')" value="Nam" />
                    <el-option :label="t('common.female')" value="Nữ" />
                    <el-option :label="t('common.other')" value="Khác" />
                </el-select>
            </el-form-item>
        </el-col>

        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.phone') }}</label>
            <el-form-item prop="phone">
                <el-input v-model="formData.phone" :disabled="isView" :placeholder="t('student.phonePlaceholder')" />
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.email') }}</label>
            <el-form-item prop="email">
                <el-input v-model="formData.email" :disabled="isView" :placeholder="t('student.emailPlaceholder')" />
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.birthDate') }}</label>
            <el-form-item prop="birthDate">
                <el-date-picker v-model="formData.birthDate" :min="0" :disabled="isView" type="date" format="DD/MM/YYYY"
                    value-format="YYYY-MM-DD" class="w-100" />
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.identifyNumber') }}</label>
            <el-form-item prop="identifyNumber">
                <el-input v-model="formData.identifyNumber" :disabled="isView"
                    :placeholder="t('student.identifyNumberPlaceholder')" />
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.company') }}</label>
            <el-form-item prop="companyId">
                <el-select v-model="formData.companyId" :disabled="isView" class="w-100">
                    <el-option v-for="company in companyStore.companies" :key="company.id" :label="company.companyName"
                        :value="company.id" />
                </el-select>
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.category') }}</label>
            <el-form-item prop="categoryId">
                <el-select v-model="formData.categoryId" :disabled="isView" class="w-100">
                    <el-option v-for="category in ageGroupStore.ageGroups" :key="category.id"
                        :label="category.categoryName + ' (' + category.from + '-' + category.to + ')'"
                        :value="category.id" />
                </el-select>
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.employee') }}</label>
            <el-form-item prop="employeeId">
                <el-select v-model="formData.employeeId" :placeholder="t('registerStudy.employeeManagerPlaceholder')"
                    class="w-100">
                    <el-option v-for="employee in employeeStore.employees" :key="employee.id"
                        :label="employee.applicationUser?.fullName" :value="employee.id" />
                </el-select>
            </el-form-item>
        </el-col>

        <!-- <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.expectedStartDate') }}</label>
            <el-form-item prop="expectedStartDate">
                <el-date-picker v-model="formData.expectedStartDate" type="date" :disabled="isView" format="DD/MM/YYYY"
                    value-format="YYYY-MM-DD" class="w-100" />
            </el-form-item>
        </el-col>

        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.age') }}</label>
            <el-form-item prop="age">
                <el-input-number v-model="formData.age" :min="0" :disabled="isView" class="w-100" />
            </el-form-item>
        </el-col>

        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('student.advisor') }}</label>
            <el-form-item prop="employeeId">
                <el-select v-model="formData.employeeId" :disabled="isView" clearable filterable class="w-100"
                    @change="employeeChange">
                    <el-option v-for="employee in employeeStore.employees" :key="employee.id"
                        :label="employee.applicationUser?.fullName" :value="employee.id" />
                </el-select>
            </el-form-item>
        </el-col> -->

        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.address') }}</label>
            <el-form-item prop="address">
                <el-input v-model="formData.address" type="textarea" :rows="2" :disabled="isView"
                    :placeholder="t('student.addressPlaceholder')" />
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.reason') }}</label>
            <el-form-item prop="reason">
                <el-input v-model="formData.reason" type="textarea" :rows="2" :disabled="isView"
                    :placeholder="t('student.reasonPlaceholder')" />
            </el-form-item>
        </el-col>

        <!-- <el-col :span="24">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.note') }}</label>
            <el-form-item prop="note">
                <el-input v-model="formData.note" type="textarea" :rows="2" :disabled="isView"
                    :placeholder="t('student.notePlaceholder')" />
            </el-form-item>
        </el-col> -->
    </el-row>
</template>

<script setup lang="ts">
import type { StudentModel } from '@/api/StudentApi';

import { onMounted } from 'vue';
import { useEmployeeStore } from '@/stores/employeeStore'


const employeeStore = useEmployeeStore()
import { useCompanyStore } from '@/stores/companyStore'
const companyStore = useCompanyStore()
import { useAgeGroupStore } from '@/stores/ageGroupStore';
const ageGroupStore = useAgeGroupStore()


// Props nhận dữ liệu và trạng thái từ StudentDialog
const props = defineProps<{
    formData: Partial<StudentModel & { priority: number, expectedBudget: number }>;
    isView: boolean;
    isEdit: boolean;
    t: Function; // Hàm dịch i18n
    rules: object; // Rules (chỉ dùng để tham chiếu, validation chính nằm ở parent)
}>();
onMounted(() => {
    // Load danh sách nhân viên khi component được khởi tạo
    employeeStore.fetchEmployees();
    ageGroupStore.fetchAllAgeGroups();
    companyStore.fetchAllCompanies();
});
// async function employeeChange(value: number) {
//     // Xử lý khi chọn nhân viên tư vấn (nếu cần)

//     const employee = employeeStore.employees.find(emp => emp.id != null && String(emp.id) === String(value));
//     if (employee) {
//         console.log("Selected Employee:", employee.applicationUser?.fullName);
//         (props.formData as any).employeeName = employee.applicationUser?.fullName;
//     }
// }
</script>