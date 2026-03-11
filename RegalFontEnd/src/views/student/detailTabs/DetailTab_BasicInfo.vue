<template>
    <div class="p-3">
        <el-row :gutter="20">
            <el-col :span="8">
                <div class="detail-card p-4 rounded border mb-4">
                    <h5 class="fw-bold mb-3">{{ t('custome.personalInfo') }}</h5>
                    <p class="text-body-secondary mb-1">{{ t('student.code') }}: <span class="fw-semibold text-dark">{{
                        formData?.studentCode || '-' }}</span></p>
                    <p class="text-body-secondary mb-1">{{ t('custome.name') }}: <span class="fw-semibold text-dark">{{
                        formData?.fullName || '-' }}</span></p>
                    <p class="text-body-secondary mb-1">{{ t('custome.age') }}: <span class="fw-semibold text-dark">{{
                        formData?.age || '-' }}</span></p>
                    <p class="text-body-secondary mb-1">{{ t('custome.gender') }}: <span
                            class="fw-semibold text-dark">{{ formData?.gender || '-' }}</span></p>
                </div>

                <div class="detail-card p-4 rounded border">
                    <h5 class="fw-bold mb-3">{{ t('custome.financialInfo') }}</h5>
                    <p class="text-body-secondary mb-1">{{ t('custome.expectedBudget') }}: <span
                            class="fw-semibold text-success">{{ formData?.expectedBudget?.toLocaleString('vi-VN') || 0
                            }}
                            đ</span></p>
                    <p class="text-body-secondary mb-1">{{ t('custome.expectedStartDate') }}: <span
                            class="fw-semibold text-dark">{{ formData?.expectedStartDate ? t('common.formatDate', {
                                date:
                                    formData.expectedStartDate
                            }) : '-' }}</span></p>
                </div>
            </el-col>

            <el-col :span="8">
                <div class="detail-card p-4 rounded border mb-4">
                    <h5 class="fw-bold mb-3">{{ t('custome.contactInfo') }}</h5>
                    <p class="text-body-secondary mb-1">{{ t('custome.phone') }}: <span class="fw-semibold text-dark">{{
                        formData?.phone || '-' }}</span></p>
                    <p class="text-body-secondary mb-1">{{ t('custome.email') }}: <span class="fw-semibold text-dark">{{
                        formData?.email || '-' }}</span></p>
                    <p class="text-body-secondary mb-1">{{ t('custome.address') }}: <span
                            class="fw-semibold text-dark">{{ formData?.address || '-' }}</span></p>
                </div>
            </el-col>

            <el-col :span="8">
                <div class="detail-card p-4 rounded border">
                    <h5 class="fw-bold mb-3">{{ t('custome.salesInfo') }}</h5>
                    <p class="text-body-secondary mb-1">{{ t('custome.source') }}:
                        <BaseBadge :label="formData?.leadSource || '-'" color="primary" />
                    </p>
                    <p class="text-body-secondary mb-1">{{ t('custome.status') }}:
                        <BaseBadge :label="getStatusLabel(formData?.studentStatus) || '-'" color="warning" />
                    </p>
                    <p class="text-body-secondary mb-1">{{ t('custome.priority') }}:
                        <BaseBadge :label="getPriorityLabel(formData?.priority) || '-'"
                            :color="getPriorityColor(formData?.priority)" />
                    </p>
                    <p class="text-body-secondary mb-1">{{ t('custome.advisor') }}: <span
                            class="fw-semibold text-dark">{{ getEmployee(formData?.employeeId) || 'Nguyễn Văn A'
                            }}</span></p>
                </div>
            </el-col>
        </el-row>
    </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import type { StudentModel } from '@/api/StudentApi';
import BaseBadge from '@/components/info-badge/BaseBadge.vue';
import { useEmployeeStore } from '@/stores/employeeStore';
import { StudentStatus } from '@/types';
const employeeStore = useEmployeeStore();
const { t } = useI18n();
const StudentStatusEnum = StudentStatus

const normalizeStatus = (status?: number | string | null) => {
    if (status === null || status === undefined) return null
    const numeric = typeof status === 'string' ? Number(status) : status
    if (Number.isNaN(numeric)) return null
    if (numeric === 6) return StudentStatusEnum.NewRegister
    return numeric as StudentStatus
}

const getStatusLabel = (status?: number | string | null) => {
    const normalized = normalizeStatus(status)
    if (normalized === StudentStatusEnum.Prospect) return t('status.prospect')
    if (normalized === StudentStatusEnum.Enrolled) return t('status.enrolled')
    if (normalized === StudentStatusEnum.Paused) return t('status.paused')
    if (normalized === StudentStatusEnum.Dropped) return t('status.dropped')
    if (normalized === StudentStatusEnum.Graduated) return t('status.graduated')
    if (normalized === StudentStatusEnum.NewRegister) return t('status.newRegister')
    return t('common.unknown')
};
const getPriorityLabel = (priority: number | any) => {
    if (priority === 2) return t('common.high'); // Cao
    if (priority === 1) return t('common.medium'); // Trung bình
    return t('common.low'); // Thấp
};
const getPriorityColor = (priority: number | any) => {
    if (priority === 2) return 'danger';
    if (priority === 1) return 'warning';
    return 'info';
};
const getEmployee = (employeeId: string | number | null | undefined): string | null => {

    //  console.log('Getting employee for ID:', employeeId);

    if (employeeId === null || employeeId === undefined) return null;

    return employeeStore.employees.find(emp => emp.id === employeeId)?.applicationUser?.fullName || null;
};

const props = defineProps<{
    formData: Partial<StudentModel & { priority: number, expectedBudget: number }>;
}>();
</script>
