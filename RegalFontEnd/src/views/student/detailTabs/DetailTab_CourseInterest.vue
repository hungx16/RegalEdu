<template>
    <div class="p-3">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h5 class="fw-bold mb-0">{{ t('student.courseTitle') }}</h5>
            <!-- <el-button type="primary" @click="addCourseInterest" icon="el-icon-plus" circle /> -->
        </div>

        <!-- <div v-if="newCourse.isAdding" class="card p-4 mb-4 border-primary">
            <h6 class="fw-bold mb-3">{{ t('student.addCourseInterest') }}</h6>
            <el-row :gutter="20">
                <el-col :span="8">
                    <el-select v-model="newCourse.courseId" :placeholder="t('registerStudy.courseName')" class="w-100">
                    </el-select>
                </el-col>
                <el-col :span="8">
                    <el-select v-model="newCourse.interestLevel" :placeholder="t('student.interestLevel')"
                        class="w-100">
                        <el-option :label="t('common.medium')" value="Trung bình" />
                        <el-option :label="t('common.high')" value="Cao" />
                    </el-select>
                </el-col>
                <el-col :span="8">
                    <el-input v-model="newCourse.reason" :placeholder="t('student.reasonPlaceholder')" />
                </el-col>
            </el-row>
            <div class="d-flex justify-content-end mt-3">
                <el-button @click="newCourse.isAdding = false">{{ t('common.cancel') }}</el-button>
                <el-button type="primary" @click="saveNewCourse">{{ t('common.add') }}</el-button>
            </div>
        </div> -->

        <div v-for="(course, idx) in internalCourses" :key="course.id ?? idx" class="card p-4 mb-3">
            <div class="d-flex justify-content-between">
                <div>
                    <h6 class="fw-bold">{{ course.courseName || 'Khóa học chưa xác định' }}</h6>
                    <p class="text-body-secondary mb-1">
                        {{ t('student.interestLevel') }}:
                        <BaseBadge :label="course.interestLevel || 'Thấp'"
                            :color="course.interestLevel === 'Cao' ? 'danger' : 'warning'" />
                    </p>
                    <p class="text-body-secondary mt-2 fst-italic">{{ t('student.reason') }}: {{ course.reason || '-' }}
                    </p>
                </div>
                <el-button type="danger" icon="el-icon-delete" circle size="small" @click="deleteCourse(course.id!)" />
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type { StudentCourseModel } from '@/api/StudentApi';
// Giả định BaseBadge đã được định nghĩa
// import BaseBadge from '@/components/info-badge/BaseBadge.vue'; 

const props = defineProps<{ courses: StudentCourseModel[]; t: Function; }>();
const emit = defineEmits(['change']);

const internalCourses = ref([...props.courses]);
const newCourse = ref({
    isAdding: false,
    courseId: null,
    courseName: '', // Tên hiển thị (cần lấy từ Store)
    interestLevel: 'Trung bình',
    reason: '',
});

watch(() => props.courses, (newCourses) => {
    internalCourses.value = [...newCourses];
});

const addCourseInterest = () => {
    newCourse.value.isAdding = true;
};

const saveNewCourse = () => {
    if (newCourse.value.courseId) {
        const newRecord: StudentCourseModel = {
            id: 'temp_' + Date.now(),
            courseId: newCourse.value.courseId,
            courseName: 'Khóa học mới', // Tên cần được resolve
            interestLevel: newCourse.value.interestLevel,
            reason: newCourse.value.reason,
            createdAt: new Date().toISOString(),
        };
        internalCourses.value.push(newRecord);
        newCourse.value.isAdding = false;
        emit('change'); // Báo cho Dialog cha rằng có thay đổi chưa lưu
    }
};

const deleteCourse = (id: string) => {
    const index = internalCourses.value.findIndex(c => c.id === id);
    if (index !== -1) {
        internalCourses.value.splice(index, 1);
        emit('change');
    }
};
</script>