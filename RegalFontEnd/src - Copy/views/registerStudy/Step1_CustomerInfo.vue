<template>
    <el-form ref="formRef" :model="store.formData" :rules="rules" label-position="top">

        <div class="mb-5">
            <h5 class="fw-bold mb-3">{{ t('registerStudy.customerType') }}</h5>

            <el-input v-model="searchQuery" :placeholder="t('registerStudy.searchStudentPlaceholder')"
                class="w-100 mb-3" @keyup.enter="handleSearch">
                <template #prepend>
                    <el-select v-model="customerType" style="width: 150px;">
                        <el-option :label="t('registerStudy.customerTypeOld')" value="old" />
                        <el-option :label="t('registerStudy.customerTypeNew')" value="new" />
                    </el-select>
                </template>
                <template #suffix>
                    <el-button v-if="customerType === 'old'" @click="handleSearch" type="primary" link :icon="Search">{{
                        t('common.search') }}</el-button>
                </template>
            </el-input>

            <div v-if="searchResult && customerType === 'old'" class="alert alert-info py-2 px-3">
                {{ t('registerStudy.studentFound', {
                    name: searchResult.studentFullName, phone:
                        searchResult.studentPhone
                }) }}
            </div>
            <el-divider class="my-3" v-if="customerType === 'old'" />
        </div>

        <div class="card p-4 mb-5">
            <h5 class="fw-bold mb-4">{{ t('registerStudy.newCustomerInfo') }}</h5>

            <el-row :gutter="20">
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.employeeManager')
                    }}</label>
                    <el-form-item prop="employeeId">
                        <el-select v-model="store.formData.employeeId"
                            :placeholder="t('registerStudy.employeeManagerPlaceholder')" class="w-100">
                            <el-option v-for="employee in employeeStore.employees" :key="employee.id"
                                :label="employee.applicationUser?.fullName" :value="employee.id" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.studentPhone') }}</label>
                    <el-form-item prop="studentPhone">
                        <el-input v-model="store.formData.studentPhone"
                            :placeholder="t('registerStudy.studentPhonePlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.studentName') }}</label>
                    <el-form-item prop="studentFullName">
                        <el-input v-model="store.formData.studentFullName"
                            :placeholder="t('registerStudy.studentNamePlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.studentBirthDate') }}</label>
                    <el-form-item prop="studentBirthDate">
                        <el-date-picker v-model="store.formData.studentBirthDate" type="date" class="w-100" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.studentEmail') }}</label>
                    <el-form-item prop="studentEmail">
                        <el-input v-model="store.formData.studentEmail"
                            :placeholder="t('registerStudy.studentEmailPlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.registrationType') }}</label>
                    <el-form-item prop="type">
                        <el-select v-model="store.formData.type" class="w-100">
                            <el-option :label="t('registerStudy.type.new')" :value="1" />
                            <el-option :label="t('registerStudy.type.renewal')" :value="2" />
                        </el-select>
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.studentAddress') }}</label>
                    <el-form-item prop="contactAddress">
                        <el-input v-model="store.formData.contactAddress"
                            :placeholder="t('registerStudy.studentAddressPlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.parentName') }}</label>
                    <el-form-item prop="contactFullName">
                        <el-input v-model="store.formData.contactFullName"
                            :placeholder="t('registerStudy.parentNamePlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.parentEmail') }}</label>
                    <el-form-item prop="contactEmail">
                        <el-input v-model="store.formData.contactEmail"
                            :placeholder="t('registerStudy.parentEmailPlaceholder')" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.parentPhone') }}</label>
                    <el-form-item prop="contactPhone">
                        <el-input v-model="store.formData.contactPhone"
                            :placeholder="t('registerStudy.parentPhonePlaceholder')" />
                    </el-form-item>
                </el-col>

                <el-col :span="12" v-if="store.formData.type === 2">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.teacherManager') }}</label>
                    <el-form-item prop="teacherManagerId">
                        <el-select v-model="store.formData.teacherManagerId"
                            :placeholder="t('registerStudy.teacherManagerPlaceholder')" class="w-100">
                            <el-option v-for="(teacher, idx) in teacherStore.teachers" :key="teacher.id ?? idx"
                                :label="teacher.applicationUser.fullName" :value="teacher.id" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12" v-if="store.formData.type === 2">
                    <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.expectedCompleteDate')
                    }}</label>
                    <el-form-item prop="expectedCompleteDate">
                        <el-date-picker v-model="store.formData.expectedCompleteDate" type="date" class="w-100" />
                    </el-form-item>
                </el-col>

                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.region') }}</label>
                    <el-form-item prop="regionId">
                        <el-select v-model="store.formData.regionId" :placeholder="t('registerStudy.regionPlaceholder')"
                            class="w-100">
                            <el-option v-for="region in regionStore.regions" :key="region.id ?? region.regionName"
                                :label="region.regionName" :value="region.id" />
                        </el-select>
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('registerStudy.company') }}</label>
                    <el-form-item prop="companyId">
                        <el-select v-model="store.formData.companyId"
                            :placeholder="t('registerStudy.companyPlaceholder')" class="w-100">
                            <el-option v-for="company in companyStore.companies" :key="company.id"
                                :label="company.companyName" :value="company.id" />
                        </el-select>
                    </el-form-item>
                </el-col>
            </el-row>
        </div>

        <div class="mb-5">
            <div class="d-flex justify-content-between align-items-center mb-3">
                <h5 class="fw-bold mb-0">{{ t('registerStudy.selectCourse') }}</h5>
                <el-button type="primary" plain @click="addEmptyCourse" icon="el-icon-plus">{{
                    t('registerStudy.addCourse')
                    }}</el-button>
            </div>

            <el-table :data="store.formData.detailRegisterStudys">

                <el-table-column :label="t('registerStudy.courseCode')" prop="courseCode" width="100" />

                <el-table-column :label="t('registerStudy.classType')" width="150" :required="true">
                    <template #default="{ row, $index }">
                        <el-form-item :prop="'detailRegisterStudys.' + $index + '.classTypeId'" :rules="courseRules">
                            <el-select v-model="row.classTypeId" filterable clearable
                                :placeholder="t('registerStudy.classTypePlaceholder')"
                                @change="val => handleCourseChange(val, row, 'classType')">
                                <el-option v-for="type in classTypeStore.classTypes" :key="type.id"
                                    :label="type.classTypeName" :value="type.id" />
                            </el-select>
                        </el-form-item>
                    </template>
                </el-table-column>

                <el-table-column :label="t('registerStudy.courseName')" width="200" :required="true">
                    <template #default="{ row, $index }">
                        <el-form-item :prop="'detailRegisterStudys.' + $index + '.courseId'" :rules="courseRules">
                            <el-select v-model="row.courseId" filterable clearable
                                :placeholder="t('registerStudy.coursePlaceholder')"
                                @change="val => handleCourseChange(val, row, 'course')">
                                <el-option v-for="course in courseStore.courses" :key="course.id"
                                    :label="course.courseName" :value="course.id" />
                            </el-select>
                        </el-form-item>
                    </template>
                </el-table-column>

                <!-- <el-table-column :label="t('registerStudy.program')" prop="program" width="100" /> -->

                <el-table-column :label="t('registerStudy.courseHours')" prop="courseHours" width="100" />
                <el-table-column :label="t('registerStudy.tuitionFee')" prop="tuitionFee" width="100" align="right" />
                <el-table-column :label="t('registerStudy.monthNumber')" prop="monthNumber" width="150" align="right">
                    <template #default="{ row }">
                        <el-input-number v-model="row.monthNumber" :min="0" disabled />
                    </template>
                </el-table-column>
                <el-table-column :label="t('registerStudy.unit')" prop="unit" width="80">
                    <template #default="{ row }">
                        <span v-if="row.unit === 0">{{ t('unit.hours') }}</span>
                        <span v-else-if="row.unit === 1">{{ t('unit.session') }}</span>
                        <span v-else-if="row.unit === 2">{{ t('unit.month') }}</span>
                        <span v-else>{{ t('unit.course') }}</span>
                    </template>
                </el-table-column>
                <el-table-column :label="t('registerStudy.quantity')" prop="quantity" width="80" />
                <!-- <el-table-column :label="t('registerStudy.discount')" prop="discountAmount" width="100" align="right" /> -->
                <el-table-column :label="t('registerStudy.totalAmount')" prop="totalAmount" width="100" align="right" />

                <el-table-column :label="t('common.actions')" width="80" fixed="right">
                    <template #default="{ $index }">
                        <el-button type="danger" size="small" :icon="DeleteIcon" circle @click="removeCourse($index)" />
                    </template>
                </el-table-column>
            </el-table>
            <!-- Hiển thị thông tin khuyến mại lấy từ store.formData.registerPromotion -->
            <el-table v-if="store.formData.registerPromotion && store.formData.registerPromotion.length > 0"
                :data="store.formData.registerPromotion" :show-header="false" class="mt-4" style="width: 100%;">
                <!-- <el-table-column :label="t('registerStudy.promotionCode')" prop="" width="150" /> -->
                <el-table-column :label="t('registerStudy.promotionName')" prop="promotionName" width="200" />
                <el-table-column :label="t('registerStudy.discountAmount')" prop="discountAmount" width="900"
                    align="right" />
            </el-table>
        </div>

        <div class="d-flex justify-content-end mb-5">
            <div class="text-end me-5">
                <p class="mb-1 fw-bold fs-5">{{ t('registerStudy.totalAmount') }}:</p>
            </div>
            <div>
                <p class="mb-1 fw-bold fs-5 text-danger">{{ store.totalAfterDiscount.toLocaleString('vi-VN') }} VND</p>
            </div>
        </div>


        <div class="mb-5 d-flex align-items-center">
            <h5 class="fw-bold mb-0 me-3">{{ t('registerStudy.discountCode') }}</h5>
            <el-input v-model="store.formData.couponCode" :placeholder="t('registerStudy.discountCodePlaceholder')"
                class="w-25" />
            <el-button type="success" class="ms-2">{{ t('registerStudy.apply') }}</el-button>
            <el-button type="warning" @click="emit('viewPromotion')">{{ t('registerStudy.viewPromotion') }}</el-button>
        </div>

        <div class="d-flex justify-content-end">
            <el-button @click="$emit('exit')">{{ t('common.close') }}</el-button>
            <el-button type="primary" @click="handleNext">{{ t('common.nextStep') }}</el-button>
        </div>
    </el-form>

    <PromotionSelectionDialog v-model:visible="showPromotionModal" :courses="mockAvailableCourses"
        @selectPromotions="handlePromotionSelection" />
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';

import { useRegisterStudyStore } from '@/stores/registerStudyStore';
import type { DetailRegisterStudyModel, RegisterPromotionListModel } from '@/api/RegisterStudyApi';

import { defineStore } from 'pinia';
import { ElMessage } from 'element-plus';
import type { RegisterStudyModel } from '@/api/RegisterStudyApi';
import { useEmployeeStore } from '@/stores/employeeStore'
import { useTeacherStore } from '@/stores/teacherStore'
import { useRegionStore } from '@/stores/regionStore'
import { useCompanyStore } from '@/stores/companyStore'
import { useCourseStore } from '@/stores/courseStore'
import type { CourseModel } from '@/api/CourseApi';
import { useClassTypeStore } from '@/stores/classTypeStore'
import type { ClassTypeModel } from '@/api/ClassTypeApi';
import { Plus, Delete as DeleteIcon, Search } from '@element-plus/icons-vue';
const teacherStore = useTeacherStore()
const employeeStore = useEmployeeStore()

const companyStore = useCompanyStore()
const regionStore = useRegionStore()
const courseStore = useCourseStore()

// --- GIẢ LẬP StudentStore, CourseStore, ClassTypeStore ---
interface StudentInfo {
    studentId: string;
    studentFullName: string;
    studentPhone: string;
    contactFullName: string;
    contactPhone: string;
    email?: string;
    address?: string;
    code?: string;
    contactAddress?: string;
    contactEmail?: string;
    employerId?: string;
    teacherId?: string;
    companyId?: string;
    regionId?: string;
    birthDate?: string;
    gender?: string;

    // ... các trường thông tin khác
}
import { useStudentStore } from '@/stores/studentStore';



const studentStore = useStudentStore();
// const courseStore = useCourseStore();
const classTypeStore = useClassTypeStore();
// ***************************************

const emit = defineEmits(['next', 'exit', 'viewPromotion']);
const { t } = useI18n();
const store = useRegisterStudyStore();
const formRef = ref();

// --- STATE CỤC BỘ ---
const customerType = ref('new');
const showPromotionModal = ref(false);
const searchQuery = ref('');
const searchResult = ref<StudentInfo | null>(null);

// --- RULES & VALIDATION ---
const rules = {
    employeeId: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    studentFullName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    contactFullName: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    contactPhone: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    // expectedCompleteDate: [{ required: true, message: t('validation.required'), trigger: 'change' }],

};

const courseRules = { required: true, message: t('validation.required'), trigger: 'change' };

// --- LOGIC TÌM KIẾM HỌC VIÊN ---
const handleSearch = async () => {
    if (customerType.value !== 'old' || !searchQuery.value) return;

    const result = await studentStore.searchStudentOrParent(searchQuery.value);

    if (result) {
        // result may be an array or a single object; normalize to a single student object
        const studentRaw = Array.isArray(result) ? result[0] : result;
        console.log('Raw student data from search:', studentRaw);

        // Map available properties from the API/mock to our StudentInfo shape with safe fallbacks
        const mapped: StudentInfo = {
            studentId: (studentRaw as any).studentId ?? (studentRaw as any).id ?? '',
            studentFullName: (studentRaw as any).studentFullName ?? (studentRaw as any).fullName ?? (studentRaw as any).name ?? '',
            studentPhone: (studentRaw as any).studentPhone ?? (studentRaw as any).phone ?? (studentRaw as any).mobile ?? '',
            contactFullName: (studentRaw as any).contacts?.[0]?.fullName ?? (studentRaw as any).parentName ?? (studentRaw as any).contactName ?? '',
            contactPhone: (studentRaw as any).contacts?.[0]?.phone ?? (studentRaw as any).contactPhone ?? (studentRaw as any).parentPhone ?? '',
            //mapping tất các trường trong studentModel và contactModel nếu có
            email: (studentRaw as any).email ?? '',
            address: (studentRaw as any).contacts?.[0]?.address ?? '',
            code: (studentRaw as any).code ?? '',
            contactAddress: (studentRaw as any).contacts?.[0]?.address ?? '',
            contactEmail: (studentRaw as any).contacts?.[0]?.email ?? '',
            employerId: (studentRaw as any).employerId ?? '',
            teacherId: (studentRaw as any).teacherId ?? '',
            companyId: (studentRaw as any).companyId ?? '',
            regionId: (studentRaw as any).regionId ?? '',
            //ngày sinh, giới tính, ... có thể thêm sau
            birthDate: (studentRaw as any).birthDate ?? '',
            gender: (studentRaw as any).gender ?? '',


        };

        searchResult.value = mapped;
        ElMessage.success(t('registerStudy.searchSuccess'));

        // Tự động điền thông tin vào formData
        store.updateFormData({
            studentId: mapped.studentId,
            studentFullName: mapped.studentFullName,
            studentPhone: mapped.studentPhone,
            contactFullName: mapped.contactFullName,
            contactPhone: mapped.contactPhone,
            studentEmail: mapped.email,
            contactEmail: mapped.contactEmail,
            contactAddress: mapped.contactAddress,
            companyId: mapped.companyId,
            regionId: mapped.regionId,
            teacherManagerId: mapped.teacherId,
            gender: mapped.gender,
            studentBirthDate: mapped.birthDate,
            type: 2,

            // ... có thể điền các trường khác như email, địa chỉ nếu có trong result
        } as Partial<RegisterStudyModel>);
    } else {
        searchResult.value = null;
        ElMessage.warning(t('registerStudy.searchNotFound'));
        // Reset các trường liên quan đến học viên nếu không tìm thấy
        store.updateFormData({
            studentId: null,
            // Giữ nguyên các trường để người dùng tự nhập
        } as Partial<RegisterStudyModel>);
    }
};

// --- LOGIC CHỌN KHÓA HỌC/LỚP HỌC ---

const addEmptyCourse = () => {
    const emptyCourse = {
        courseId: null,
        classTypeId: null,
        discountAmount: 0,
        tuitionFee: 0,
        totalAmount: 0,
        quantity: 1,
    };
    store.formData.detailRegisterStudys?.push(emptyCourse as DetailRegisterStudyModel);
    store.updateFormData({});
};

const handleCourseChange = async (id: string | null, row: any, type: 'course' | 'classType') => {
    // console.log(`Selected ${type} ID:`, row.courseId);

    let selectedCourse: CourseModel | undefined;
    let selectedClassType: ClassTypeModel | undefined;

    // Lấy dữ liệu hiện tại
    if (row.courseId) selectedCourse = await courseStore.getCourseById(row.courseId);
    if (row.classTypeId) selectedClassType = (await classTypeStore.getClassTypeById(row.classTypeId)) ?? undefined;

    if (type === 'course') {
        selectedCourse = await courseStore.getCourseById(id || '');
        console.log(`Lấy giá trị của Couse`, selectedCourse);

        if (selectedCourse) {
            var quantity = selectedCourse.tuitions?.[0]?.unit == 0 ? selectedCourse.tuitions?.[0]?.durationHours : 1;
            // var unit = selectedCourse.tuitions?.[0]?.unit == 0 ? t('unit.hours') : selectedCourse.tuitions?.[0]?.unit == 1 ? t('unit.session') : selectedCourse.tuitions?.[0]?.unit == 2 ? t('unit.month') : t('unit.course');
            var tuitionFee = selectedCourse.tuitions?.[0]?.tuitionFee ?? 0;
            // var program = selectedCourse.learningRoadMap?.learningRoadMapName ?? 'null';
            // row.program = program;
            var courseHours = selectedCourse.tuitions?.[0]?.durationHours ?? 0;
            row.courseHours = courseHours;

            // Use flexible access to handle possible differences in property names between mocks and real API models
            row.courseName = (selectedCourse as any).name ?? (selectedCourse as any).courseName ?? null;
            row.courseCode = (selectedCourse as any).code ?? (selectedCourse as any).courseCode ?? null;
            row.tuitionFee = tuitionFee;
            // row.courseHours = (selectedCourse as any).courseHours ?? (selectedCourse as any).hours ?? null;
            row.unit = selectedCourse.tuitions?.[0]?.unit;
            row.program = (selectedCourse as any).program ?? null;
            row.quantity = quantity;
        } else {
            // Reset fields
            Object.assign(row, { courseName: null, courseCode: null, tuitionFee: 0, courseHours: null, unit: null, program: null });
        }
    } else if (type === 'classType') {
        selectedClassType = (await classTypeStore.getClassTypeById(id || '')) ?? undefined;
    }

    // TÍNH TOÁN LẠI TỔNG TIỀN
    row.discountAmount = row.discountAmount || 0;
    row.totalAmount = row.tuitionFee - row.discountAmount;

    store.updateFormData({});
};

const removeCourse = (index: number) => {
    store.formData.detailRegisterStudys?.splice(index, 1);
    store.updateFormData({});
};

// --- LOGIC KHUYẾN MÃI ---
const mockAvailableCourses = computed(() => {
    return store.formData.detailRegisterStudys?.filter(d => d.courseId && d.classTypeId).map(d => ({
        id: d.courseId || 'temp',
        code: d.courseName || '',
        name: d.courseName || 'Khóa học',
        availablePromotions: [
            { id: 'GLOBAL_PROMO_1', name: t('promotion.discount5Percent'), description: t('promotion.discount500k') },
            { id: 'PRODUCT_PROMO_1', name: t('promotion.buy1Get1'), description: t('promotion.gift500k') },
        ]
    })) || [];
});

const handlePromotionSelection = (selection: RegisterPromotionListModel) => {
    store.updateFormData({ registerPromotion: [selection] });

    // Logic: GỌI API/Function tính toán tổng giảm giá dựa trên selection
    // Mock: Giảm 5% nếu chọn Global_PROMO_1
    // const globalDiscountRate = selection? 0.05 : 0;
    // const calculatedDiscount = store.totalAmount * globalDiscountRate;

    // store.formData.totalDiscount = calculatedDiscount;

    ElMessage.success(t('promotion.appliedSuccess'));
    store.updateFormData({});
};

const handleNext = () => {
    formRef.value?.validate((valid: boolean) => {
        if (valid) {
            if (!store.formData.detailRegisterStudys || store.formData.detailRegisterStudys.length === 0) {
                ElMessage.error(t('registerStudy.validation.courseRequired'));
                return;
            }
            store.updateFormData({});
            emit('next');
        }
    });
};

onMounted(() => {
    regionStore.fetchAllRegions();
    companyStore.fetchAllCompanies();
    teacherStore.fetchAllTeacher();
    employeeStore.fetchAllEmployees();
    courseStore.fetchAllCourses();
    classTypeStore.fetchAllClassTypes();

    // Khởi tạo CustomerType dựa trên dữ liệu hiện tại trong store (nếu có)
    if (store.formData.studentId) {
        customerType.value = 'old';
    } else {
        customerType.value = 'new';
    }
});
async function ShowPromotion() {
    console.log("Show Promotion clicked");

    // dialogMode.value = 'create';
    // await commonStore.generateCode('RE', 'Region', 'RegionCode', 4);
    // regionStore.selectRegion(null);
    emit('exit');
    showPromotionModal.value = true;
}
</script>