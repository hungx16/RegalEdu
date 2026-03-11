<template>
    <div class="p-3">
        <div class="card p-4 mb-4 border-primary">
            <h6 class="fw-bold mb-3">{{ t('custome.addActivity') }}</h6>
            <el-row :gutter="20">
                <el-col :span="12">
                    <el-select v-model="newActivity.type" :placeholder="t('custome.activityType')" class="w-100">
                        <el-option :label="t('customeActivity.call')" value="0" />
                        <el-option :label="t('customeActivity.sms')" value="1" />
                        <el-option :label="t('customeActivity.zalo')" value="2" />
                        <el-option :label="t('customeActivity.email')" value="3" />
                        <el-option :label="t('customeActivity.event')" value="4" />
                        <el-option :label="t('customeActivity.other')" value="5" />
                    </el-select>
                </el-col>
                <el-col :span="12">
                    <el-select v-model="newActivity.results" :placeholder="t('custome.activityResult')" class="w-100">
                        <el-option :label="t('common.medium')" value="Trung bình" />
                        <el-option :label="t('common.good')" value="Tốt" />
                        <el-option :label="t('common.excellent')" value="Xuất sắc" />
                    </el-select>
                </el-col>
                <el-col :span="24" class="mt-3">
                    <el-input type="textarea" v-model="newActivity.content"
                        :placeholder="t('custome.activityContent')" />
                </el-col>
                <el-col :span="24" class="mt-3">
                    <el-input v-model="newActivity.nextAction" :placeholder="t('custome.nextAction')" />
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('student.status') }}</label>
                    <el-form-item prop="status">
                        <el-select v-model="newActivity.status" class="w-100">
                            <!--gồm các trạng thái tiếp cận, tìm hiểu, tiềm năng -->
                            <el-option :label="t('custome.prospecting')" :value="0" />
                            <el-option :label="t('custome.considering')" :value="1" />
                            <el-option :label="t('custome.potential')" :value="2" />

                        </el-select>
                    </el-form-item>
                </el-col>
            </el-row>
            <div class="d-flex justify-content-end mt-3">
                <el-button type="primary" @click="saveNewActivity">{{ t('custome.addInteraction') }}</el-button>
            </div>
        </div>

        <h5 class="fw-bold mb-3">{{ t('custome.activityLogList') }}</h5>
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
import { ref, watch } from 'vue';
import type { StudentActivityModel } from '@/api/StudentApi';

const props = defineProps<{ activities: StudentActivityModel[]; t: Function; }>();
const emit = defineEmits(['update:activities']);

const internalActivities = ref([...props.activities]);
const newActivity = ref({
    type: '0', // Gọi điện
    results: 'Trung bình',
    content: '',
    nextAction: '',
    status: 0,
});

watch(() => props.activities, (newActivities) => {
    internalActivities.value = [...newActivities];
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