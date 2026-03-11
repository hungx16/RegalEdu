<template>
    <div class="p-3">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h5 class="fw-bold mb-0">{{ t('custome.courseListTitle') }}</h5>
            <el-button type="primary" @click="addCourseInterest" :icon="Plus" circle />
        </div>

        <div v-if="newCourse.isAdding" class="card p-4 mb-4 border-primary">
            <h6 class="fw-bold mb-3">{{ t('custome.addCourseInterest') }}</h6>
            <el-row :gutter="20">
                <el-col :span="8">
                    <el-select v-model="newCourse.courseId" :placeholder="t('registerStudy.courseName')" class="w-100">
                        <el-option v-for="course in courseStore.courses" :key="course.id" :label="course.courseName"
                            :value="course.id" />
                    </el-select>
                </el-col>
                <el-col :span="8">
                    <el-select v-model="newCourse.interestLevel" :placeholder="t('custome.interestLevel')"
                        class="w-100">
                        <el-option :label="t('common.low')" :value="0" />
                        <el-option :label="t('common.medium')" :value="1" />
                        <el-option :label="t('common.high')" :value="2" />
                    </el-select>
                </el-col>
                <el-col :span="8">
                    <el-input v-model="newCourse.reason" :placeholder="t('custome.reasonPlaceholder')" />
                </el-col>
            </el-row>
            <div class="d-flex justify-content-end mt-3">
                <el-button @click="newCourse.isAdding = false">{{ t('common.cancel') }}</el-button>
                <el-button type="primary" @click="saveNewCourse">{{ t('common.add') }}</el-button>
            </div>
        </div>

        <div v-for="(course, idx) in internalCourses" :key="course.id ?? idx" class="card p-4 mb-3">
            <div class="d-flex justify-content-between">
                <div>
                    <h6 class="fw-bold">{{ getCourseNameById(course.courseId) || 'Khóa học chưa xác định' }}</h6>
                    <p class="text-body-secondary mb-1">
                        {{ t('custome.interestLevel') }}
                        :
                        <span
                            :class="Number(course.interestLevel ?? InterestLevel.Low) === InterestLevel.High ? 'text-danger' : 'text-warning'">{{
                                getCourseInterestLevelLabel(course.interestLevel ?? undefined) ||
                                t('custome.noInterestLevel') }}</span>

                    </p>
                    <p class="text-body-secondary mt-2 fst-italic">{{ t('custome.reason') }}: {{ course.reason || '-' }}
                    </p>
                </div>
                <!-- <el-button circle size="small" @click="deleteCourse(course.id!)">
                    <el-icon>
                        <Delete />
                    </el-icon>
                </el-button> -->
                <el-button type="danger" :icon="DeleteIcon" circle size="small" @click="deleteCourse(course.id!)"
                    style="background-color: red;" />
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import type { StudentCourseModel } from '@/api/StudentApi';
import { useCourseStore } from '@/stores/courseStore';
import { Plus, Delete as DeleteIcon } from '@element-plus/icons-vue';
const courseStore = useCourseStore();
// Giả định BaseBadge đã được định nghĩa
// import BaseBadge from '@/components/info-badge/BaseBadge.vue'; 

const props = defineProps<{ courses: StudentCourseModel[]; t: Function; }>();
const { t } = props;
const emit = defineEmits(['update:courseInterest', 'change']);

const internalCourses = ref([...props.courses]);
const newCourse = ref({
    isAdding: false,
    courseId: null as string | null,
    courseName: '', // Tên hiển thị (cần lấy từ Store)
    interestLevel: '',
    reason: '',
});
enum InterestLevel {
    Low = 0,
    Medium = 1,
    High = 2,
}
const getCourseNameById = (id: string | null | undefined) => {
    const course = courseStore.courses.find(c => c.id == id);
    console.log("Looking up course for id:", id);
    console.log(course);
    return course ? course.courseName : 'Khóa học chưa xác định';
};
watch(() => props.courses, (newCourses) => {
    internalCourses.value = [...newCourses];
});

const addCourseInterest = () => {
    newCourse.value.isAdding = true;
};
onMounted(() => {

    courseStore.fetchAllCourses();

});
const getCourseInterestLevelLabel = (level: number | string | undefined) => {
    const n = Number(level);
    switch (n) {
        case InterestLevel.High:
            return t('common.high');
        case InterestLevel.Medium:
            return t('common.medium');
        case InterestLevel.Low:
        default:
            return t('common.low');
    }
};
const saveNewCourse = () => {
    console.log("Saving new course interest:", newCourse.value);

    if (newCourse.value.courseId) {
        const newRecord: StudentCourseModel = {
            courseId: newCourse.value.courseId,
            courseName: "",
            interestLevel: newCourse.value.interestLevel.toString(),
            reason: newCourse.value.reason,
            createdAt: new Date().toISOString(),
        };
        internalCourses.value.push(newRecord);
        newCourse.value.isAdding = false;
        emit('update:courseInterest', internalCourses.value); // Báo cho Dialog cha rằng có thay đổi chưa lưu
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