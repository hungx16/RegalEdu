<template>
    <el-row :gutter="20">
        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('custome.code') }}</label>
            <el-form-item prop="studentCode">
                <el-input v-model="formData.studentCode" :disabled="isView || isEdit"
                    :placeholder="t('custome.codePlaceholder')" />
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('custome.name') }}</label>
            <el-form-item prop="fullName">
                <el-input v-model="formData.fullName" :disabled="isView" :placeholder="t('custome.namePlaceholder')" />
            </el-form-item>
        </el-col>

        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('custome.phone') }}</label>
            <el-form-item prop="phone">
                <el-input v-model="formData.phone" :disabled="isView" :placeholder="t('custome.phonePlaceholder')" />
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('custome.email') }}</label>
            <el-form-item prop="email">
                <el-input v-model="formData.email" :disabled="isView" :placeholder="t('custome.emailPlaceholder')" />
            </el-form-item>
        </el-col>

        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('custome.source') }}</label>
            <el-form-item prop="leadSource">
                <el-select v-model="formData.leadSource" :disabled="isView" class="w-100">
                    <el-option :label="t('source.zalo')" value="Zalo" />
                    <el-option :label="t('source.social')" value="Social" />
                    <el-option :label="t('source.event')" value="Event" />
                    <el-option :label="t('source.tiktok')" value="Tiktok" />
                    <el-option :label="t('source.support')" value="Support" />
                    <el-option :label="t('source.website')" value="Website" />
                    <el-option :label="t('source.facebook')" value="Facebook" />
                    <el-option :label="t('source.other')" value="Other" />
                </el-select>
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('custome.priority') }}</label>
            <el-form-item prop="priority">
                <el-select v-model="formData.priority" :disabled="isView" class="w-100">
                    <el-option :label="t('common.low')" :value="0" />
                    <el-option :label="t('common.medium')" :value="1" />
                    <el-option :label="t('common.high')" :value="2" />
                </el-select>
            </el-form-item>
        </el-col>

        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('custome.expectedBudget') }}</label>
            <el-form-item prop="expectedBudget">
                <el-input-number v-model="formData.expectedBudget" :min="0" :disabled="isView" class="w-100" />
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('custome.expectedStartDate') }}</label>
            <el-form-item prop="expectedStartDate">
                <el-date-picker v-model="formData.expectedStartDate" type="date" :disabled="isView" format="DD/MM/YYYY"
                    value-format="YYYY-MM-DD" class="w-100" />
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
        <!-- <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('custome.age') }}</label>
            <el-form-item prop="age">
                <el-input-number v-model="formData.age" :min="0" :disabled="isView" class="w-100" />
            </el-form-item>
        </el-col> -->
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('custome.gender') }}</label>
            <el-form-item prop="gender">
                <el-select v-model="formData.gender" :disabled="isView" class="w-100">
                    <el-option :label="t('common.male')" value="Nam" />
                    <el-option :label="t('common.female')" value="Nữ" />
                    <el-option :label="t('common.other')" value="Khác" />
                </el-select>
            </el-form-item>
        </el-col>

        <el-col :span="12">
            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('custome.advisor') }}</label>
            <el-form-item prop="employeeId">
                <el-select v-model="formData.employeeId" :disabled="isView" clearable filterable class="w-100"
                    @change="employeeChange">
                    <el-option v-for="employee in employeeStore.employees" :key="employee.id"
                        :label="employee.applicationUser?.fullName" :value="employee.id" />
                </el-select>
            </el-form-item>
        </el-col>
        <!-- Thêm cột lịch học mong muốn là một combobox -->
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('custome.expectedTime') }}</label>
            <el-form-item prop="selectedWorkingTimeIds">
                <el-select v-model="selectedWorkingTimeIds" :disabled="isView" class="w-100" multiple
                    @change="handleWorkingTimeChange" placeholder="-- Select Expected Working Times --">
                    <!-- đọc tất cả các thời gian trong workingtime -->
                    <el-option v-for="time in workingTimeStore.workingTimes.sort((a, b) => a.dayOfWeek - b.dayOfWeek)"
                        :key="time.id"
                        :label="(getDayNames(time.dayOfWeek)) + ' (' + time.startTime + '-' + time.endTime + ')'"
                        :value="time.id" />
                    <!-- <el-option :label="t('time.morning')" :value="ExpectedTime.Morning" />
                    <el-option :label="t('time.afternoon')" :value="ExpectedTime.Afternoon" />
                    <el-option :label="t('time.evening')" :value="ExpectedTime.Evening" />
                    <el-option :label="t('time.weekend')" :value="ExpectedTime.Weekend" />
                    <el-option :label="t('time.other')" :value="ExpectedTime.Other" /> -->
                </el-select>
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('custome.currentLevel') }}</label>
            <el-form-item prop="currentLevel">
                <el-select v-model="formData.currentLevel" :disabled="isView" class="w-100">
                    <el-option :label="t('level.beginner')" :value="CurrentLevel.PreIntermediate" />
                    <el-option :label="t('level.intermediate')" :value="CurrentLevel.Intermediate" />
                    <el-option :label="t('level.upperIntermediate')" :value="CurrentLevel.UpperIntermediate" />
                    <el-option :label="t('level.advanced')" :value="CurrentLevel.Advanced" />
                    <el-option :label="t('level.proficient')" :value="CurrentLevel.Unknown" />
                </el-select>
            </el-form-item>
        </el-col>
        <!-- bổ sung cột trạng thái  -->
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.status') }}</label>
            <el-form-item prop="status">
                <el-select v-model="formData.status" :disabled="isView" class="w-100">
                    <!--gồm các trạng thái tiếp cận, tìm hiểu, tiềm năng -->
                    <el-option :label="t('custome.prospecting')" :value="0" />
                    <el-option :label="t('custome.considering')" :value="1" />
                    <el-option :label="t('custome.potential')" :value="2" />

                </el-select>
            </el-form-item>
        </el-col>
        <el-col :span="12">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('custome.address') }}</label>
            <el-form-item prop="address">
                <el-input v-model="formData.address" type="textarea" :rows="2" :disabled="isView"
                    :placeholder="t('custome.addressPlaceholder')" />
            </el-form-item>
        </el-col>

        <el-col :span="24">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.note') }}</label>
            <el-form-item prop="reason">
                <el-input v-model="formData.reason" type="textarea" :rows="2" :disabled="isView"
                    :placeholder="t('student.notePlaceholder')" />
            </el-form-item>
        </el-col>
        <!-- <el-col :span="24">
            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('custome.note') }}</label>
            <el-form-item prop="reason">
                <el-input v-model="formData.reason" type="textarea" :rows="2" :disabled="isView"
                    :placeholder="t('custome.notePlaceholder')" />
            </el-form-item>
        </el-col> -->
    </el-row>
</template>

<script setup lang="ts">
import type { StudentModel } from '@/api/StudentApi';
import { useCompanyStore } from '@/stores/companyStore'
const companyStore = useCompanyStore()
import { onMounted, ref } from 'vue';
import { useEmployeeStore } from '@/stores/employeeStore'
const employeeStore = useEmployeeStore()
import { useAgeGroupStore } from '@/stores/ageGroupStore';
const ageGroupStore = useAgeGroupStore()
//khai báo sử dụng WorkingTimeStore
import { useWorkingTimeStore } from '@/stores/workingTimeStore';
const workingTimeStore = useWorkingTimeStore();
import { ExpectedTime, CurrentLevel, StudentStatus } from '@/types';
import { useI18n } from 'vue-i18n';
const { t } = useI18n();
//khai báo một biến để lưu trữ danh sách các id cảc working time đã chọn
const selectedWorkingTimeIds = ref<string[]>([]);

// helper to format day names for working times (accepts number or array of numbers)
function getDayNames(dayOfWeek: number | number[] | undefined): string {
    if (dayOfWeek == null) return '';
    const names = [t('weekdays.sunday'), t('weekdays.monday'), t('weekdays.tuesday'), t('weekdays.wednesday'), t('weekdays.thursday'), t('weekdays.friday'), t('weekdays.saturday')];
    const days = Array.isArray(dayOfWeek) ? dayOfWeek : [dayOfWeek];
    return days.map(d => {
        const idx = Number(d) % 7;
        return names[idx] ?? String(d);
    }).join(', ');
}
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
    employeeStore.fetchAllEmployees();
    ageGroupStore.fetchAllAgeGroups();
    companyStore.fetchAllCompanies();
    workingTimeStore.fetchAllWorkingTimes();

    // Initialize selectedWorkingTimeIds from incoming formData (if present)
    if ((props.formData as any).expectedWorkingTime) {
        selectedWorkingTimeIds.value = (props.formData as any).expectedWorkingTime.split('#$#').map((id: string) => id.trim());
    }
});

// keep local selectedWorkingTimeIds and parent formData in sync when user changes selection
function handleWorkingTimeChange(value: (string | number)[]) {
    // normalize to string ids for the local ref (template v-model uses selectedWorkingTimeIds)
    selectedWorkingTimeIds.value = Array.isArray(value) ? value.map(v => String(v)) : [];
    // store the raw value (numbers or strings) back to the parent formData
    (props.formData as any).expectedWorkingTime = Array.isArray(value) ? value.join('#$#') : '';
    console.log("Updated expectedWorkingTime:", (props.formData as any).expectedWorkingTime);

}

async function employeeChange(value: number) {
    // Xử lý khi chọn nhân viên tư vấn (nếu cần)

    const employee = employeeStore.employees.find(emp => emp.id != null && String(emp.id) === String(value));
    if (employee) {
        console.log("Selected Employee:", employee.applicationUser?.fullName);
        (props.formData as any).employeeName = employee.applicationUser?.fullName;
    }
}
</script>