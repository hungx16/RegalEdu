<template>
    <div class="p-3">
        <div class="card p-4 mb-4 border-primary">
            <h6 class="fw-bold mb-3">{{ t('student.requestCompanyChange') }}</h6>
            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="d-block mb-2">{{ t('student.selectCompanyChange') }}</label>
                    <el-select v-model="companyName" :placeholder="t('student.selectCompanyChange')" class="w-100">

                        <!-- hiển thị danh sách các chi nhánh (company) tạo placeholder -->
                        <el-option v-for="branch in companyList" :key="branch.id" :label="branch.companyName"
                            :placeholder="t('custome.selectCompany')" :value="branch.id" />
                    </el-select>
                </el-col>
                <el-col :span="6">
                    <label class="d-block mb-2">{{ t('student.saleName') }}</label>
                    <el-select v-model="employeeIds" :placeholder="t('student.saleName')" class="w-100">
                        <!-- hiển thị danh sách các employee -->
                        <el-option v-for="employee in employeeList" :key="employee.id"
                            :label="employee.applicationUser?.fullName" :value="employee.id" />
                    </el-select>
                </el-col>
                <el-col :span="6">
                    <label class="d-block mb-2">{{ t('student.changeDate') }}</label>
                    <el-date-picker v-model="dateChange" type="date" :placeholder="t('student.selectDate')"
                        class="w-100" />
                </el-col>

                <el-col :span="24" class="mt-3">
                    <label class="d-block mb-2">{{ t('student.changeReason') }}</label>
                    <el-input type="textarea" v-model="newActivity.content" :placeholder="t('custome.reason')" />
                </el-col>
                <el-col :span="24" class="mt-3">
                    <el-checkbox v-model="newActivity.nextAction">
                        {{ t('student.requestPaymentChange') }}
                    </el-checkbox>

                </el-col>
            </el-row>
            <div class="d-flex justify-content-end mt-3">
                <el-button type="primary" @click="saveNewActivity">{{ t('student.createRequest') }}</el-button>
            </div>
        </div>

        <h5 class="fw-bold mb-3">{{ t('student.requestListChangeCompany') }}</h5>
        <div v-for="(activity, idx) in internalActivities" :key="activity.id ?? idx" class="card p-4 mb-3">
            <div class="d-flex justify-content-between">
                <h6 class="fw-bold">{{ t(`customeActivity.${activity.type === '0' ? 'call' : 'sms'}`) }}</h6>
                <span class="text-body-secondary">
                    {{ ((typeof activity.activityDate === 'string' ? activity.activityDate :
                        activity.activityDate?.toISOString()) || '').substring(0, 10) }}
                    {{ ((typeof activity.activityDate === 'string' ? activity.activityDate :
                        activity.activityDate?.toISOString()) || '').substring(11, 16) }}
                </span>
            </div>
            <p class="text-body-secondary mt-2 mb-1">{{ activity.content }}</p>
            <p class="text-primary mt-2">
                {{ t('custome.nextAction') }}: <span class="fw-semibold">{{ activity.nextAction }}</span>
            </p>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, watch, computed, onMounted } from 'vue';
import type { StudentActivityModel } from '@/api/StudentApi';
//import company list from store or api
import { useCompanyStore } from '@/stores/companyStore';
const companyStore = useCompanyStore();
const companyList = computed(() => companyStore.companies);
// import employee list from store or api
import { useEmployeeStore } from '@/stores/employeeStore';
const employeeStore = useEmployeeStore();

const employeeList = computed(() => employeeStore.employees); // added to avoid compile error in template
// reactive model for the company select used in the template
const companyName = ref<string | number | null>(null);
// selected employee id for the employee select (fixes missing property error)
const employeeIds = ref<string | number | null>(null);
// selected date/change value used by the template
const dateChange = ref<string | number | null>(null);

const props = defineProps<{ activities: StudentActivityModel[]; t: Function; branches?: { id: string; name: string }[] }>();
const emit = defineEmits(['update:activities']);

const branches = computed(() => props.branches ?? []);

const internalActivities = ref([...props.activities]);
const newActivity = ref({
    type: '0', // Gọi điện
    results: 'Trung bình',
    content: '',
    nextAction: '',
});

watch(() => props.activities, (newActivities) => {
    internalActivities.value = [...newActivities];
});
//thiết lập onMounted để load danh sách chi nhánh nếu cần
onMounted(() => {
    if (companyList.value.length === 0) {
        companyStore.fetchAllCompanies();
    }
    if (employeeList.value.length === 0) {
        employeeStore.fetchAllEmployees();
    }
});

const saveNewActivity = () => {
    if (newActivity.value.content && newActivity.value.nextAction) {
        const newRecord: StudentActivityModel = {
            type: newActivity.value.type,
            results: newActivity.value.results,
            content: newActivity.value.content,
            nextAction: newActivity.value.nextAction,
            activityDate: new Date().toISOString(),
            //  employeeId: null,
        };
        internalActivities.value.unshift(newRecord);
        emit('update:activities', internalActivities.value);
    }
};
</script>