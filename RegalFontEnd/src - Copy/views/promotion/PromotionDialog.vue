<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="loading" :width="computedDialogWidth" :form-ref="formRef" @submit="onSubmit"
        @delete="onDelete" @update:visible="emit('update:visible', $event)" @close="closeModal">
        <template #form>
            <div class="row border rounded-3 bg-white">
                <div class="col-12 mb-4" style="margin-top: 10px;">
                    <h5 class="fw-bold">{{ t('promotion.generalInfo') }}</h5>
                    <el-divider class="my-2" />
                    <div class="col-md-24">
                        <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('promotion.group') }}</label>
                        <el-form-item prop="promotionGroupId">
                            <el-select v-model="formData.promotionGroupId" :disabled="isView"
                                :placeholder="t('promotion.groupPlaceholder')">
                                <el-option v-for="group in promotionGroupStore.promotionGroups" :key="group.id"
                                    :label="group.name" :value="group.id" />
                            </el-select>
                        </el-form-item>
                    </div>
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('promotion.name') }}</label>
                            <el-form-item prop="name">
                                <el-input v-model="formData.name" :disabled="isView"
                                    :placeholder="t('promotion.namePlaceholder')" />
                            </el-form-item>
                        </div>
                        <div class="col-md-3">
                            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('promotion.timeRange') }}</label>
                            <el-form-item prop="timeRange">
                                <el-date-picker v-model="formData.startDate" type="date" :disabled="isView"
                                    :range-separator="t('common.to')"
                                    :start-placeholder="t('promotion.startDatePlaceholder')" />
                            </el-form-item>

                        </div>
                        <div class="col-md-3">
                            <label class="fs-6 fw-semibold mb-2 d-block"> &nbsp;</label>
                            <el-form-item>
                                <el-date-picker v-model="formData.endDate" type="date" :disabled="isView"
                                    :range-separator="t('common.to')"
                                    :end-placeholder="t('promotion.endDatePlaceholder')" />
                            </el-form-item>
                        </div>
                        <div class="col-md-6">
                            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('promotion.code') }}</label>
                            <el-form-item prop="code">
                                <el-input v-model="formData.code" :disabled="isView"
                                    :placeholder="t('promotion.codePlaceholder')" />
                            </el-form-item>
                        </div>
                        <div class="col-md-6">
                            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
                            <el-form-item prop="quotaStage">
                                <el-select v-model="formData.status" :disabled="isView"
                                    :placeholder="t('promotion.statusPlaceholder')">
                                    <el-option :value="0" :label="t('common.active')" />
                                    <el-option :value="1" :label="t('common.inactive')" />
                                </el-select>
                            </el-form-item>
                        </div>

                    </div>
                </div>
                <el-divider class="my-2" />
                <div class="col-12 mb-4">
                    <h5 class="fw-bold">{{ t('promotion.applicationConditions') }}</h5>
                    <el-divider class="my-2" />
                    <div class="row g-3">

                        <div class="col-12">
                            <el-checkbox v-model="formData.applyWith" :disabled="isView">
                                {{ t('promotion.applyWithOther') }}
                            </el-checkbox>
                        </div>

                        <div class="col-md-12 d-flex align-items-center">
                            <label class="fs-6 fw-semibold me-3" style="min-width: 120px;">{{ t('promotion.company')
                            }}</label>
                            <el-form-item prop="companyId" class="w-100">
                                <el-select v-model="formData.companyId" filterable clearable :disabled="isView"
                                    :placeholder="t('promotion.companyPlaceholder')">
                                    <el-option v-for="comp in companyStore.companies" :key="comp.id"
                                        :label="comp.companyName" :value="comp.id" />
                                </el-select>
                            </el-form-item>
                            <el-checkbox v-model="formData.allCompany" :disabled="isView" class="ms-2">All</el-checkbox>
                        </div>

                        <div class="col-md-12 d-flex align-items-center">
                            <label class="fs-6 fw-semibold me-3" style="min-width: 120px;">{{ t('promotion.course')
                            }}</label>
                            <el-form-item prop="courseId" class="w-100">
                                <el-select v-model="formData.courseId" filterable clearable :disabled="isView"
                                    :placeholder="t('promotion.coursePlaceholder')">
                                    <el-option v-for="course in courseStore.courses" :key="course.id"
                                        :label="course.courseName" :value="course.id" />
                                </el-select>
                            </el-form-item>
                            <el-checkbox v-model="formData.allCourse" :disabled="isView" class="ms-2">All</el-checkbox>
                        </div>

                        <div class="col-md-6 d-flex align-items-center">
                            <label class="fs-6 fw-semibold me-3" style="min-width: 120px;">{{ t('promotion.minMonth')
                            }}</label>
                            <el-form-item prop="qtymonth" class="w-100">
                                <el-input-number v-model="formData.qtymonth" :min="0" :disabled="isView"
                                    :placeholder="t('promotion.minMonthPlaceholder')" class="w-100" />
                            </el-form-item>
                        </div>
                        <div class="col-md-6 d-flex align-items-center">
                            <label class="fs-6 fw-semibold me-3" style="min-width: 120px;">{{ t('promotion.maxMonth')
                            }}</label>
                            <el-form-item prop="qtymonth" class="w-100">
                                <el-input-number v-model="formData.qtymonth" :min="0" :disabled="isView"
                                    :placeholder="t('promotion.minMonthPlaceholder')" class="w-100" />
                            </el-form-item>
                        </div>
                    </div>
                </div>
                <el-divider class="my-2" />
                <div class="col-12 mb-4">
                    <h5 class="fw-bold">{{ t('promotion.studentScope') }}</h5>
                    <el-divider class="my-2" />
                    <div class="row g-3">
                        <div class="col-12">
                            <el-radio-group v-model="formData.allStudent" :disabled="isView">
                                <el-radio :value="true">{{ t('promotion.allStudents') }} <span
                                        v-if="formData.allStudent == true" class="mx-4"> ({{
                                            getStudentNumberQualified()
                                        }} {{ t('promotion.studentsQualified') }})</span></el-radio>
                                <el-radio :value="false">{{ t('promotion.specificStudents') }}</el-radio>
                            </el-radio-group>
                            <!-- viết chương trình sao cho khi người dùng  sẽ thêm người dùng vào promotionStudent -->
                            <el-form-item v-if="formData.allStudent === false" prop="studentId" class="mt-2">
                                <el-select v-model="studentIds" multiple filterable :disabled="isView"
                                    :placeholder="t('promotion.selectStudentsPlaceholder')" class="w-100"
                                    @change="addPromotionStudent">
                                    <el-option v-for="student in studentStore.students" :key="student.id"
                                        :label="student.fullName" :value="student.id" />
                                </el-select>
                                <el-button class="mt-2" type="primary" plain
                                    @click="emit('import-students', studentIds)">
                                    {{ t('promotion.importStudents', { count: studentIds.length }) }}
                                </el-button>
                            </el-form-item>
                        </div>
                    </div>
                </div>
                <el-divider class="my-2" />
                <div class="col-12 mb-4">
                    <h5 class="fw-bold">{{ t('promotion.details') }}</h5>
                    <el-divider class="my-2" />
                    <div class="row g-3">
                        <!-- <div class="col-12 col-md-12 d-flex">
                            <div class="col-4">
                                <el-checkbox v-model="formData.codeUsage" :disabled="isView">
                                    {{ t('promotion.codeUsage') }}
                                </el-checkbox>
                            </div>
                            <div class="col-6" v-if="formData.codeUsage">
                                
                                <el-form-item prop="name" class="col-6">
                                    <el-input v-model="formData.code" :disabled="isView"
                                        :placeholder="t('promotion.promoCodePlaceholder')" />
                                </el-form-item>
                            </div>
                        </div> -->
                        <div class="col-md-12">
                            <!-- <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('promotion.type') }}</label> -->
                            <el-form-item prop="type">
                                <!-- <el-select v-model="formData.type" :disabled="isView"
                                    :placeholder="t('promotion.typePlaceholder')">
                                    <el-option v-for="type in promotiontypeOptions" :key="type.value"
                                        :label="type.label" :value="type.value" />
                                </el-select> -->
                                <el-checkbox v-model="formData.allCourse" :disabled="isView" class="ms-3">Chiết
                                    khấu(%)</el-checkbox>
                                <!-- <el-form-item prop="discounts.discountMax"> -->
                                <el-input-number class="ms-3" v-model="discounts0.discountMax" :disabled="isView"
                                    :min="0" />
                                <!-- </el-form-item> -->
                                <!-- <div class="col-md-6">
                                    <label class="fs-6 fw-semibold mb-2 d-block">% {{ t('promotion.discountAmount')
                                        }}</label>

                                </div> -->
                            </el-form-item>
                        </div>
                        <el-divider class="my-2" />
                        <div class="col-md-12">
                            <!-- <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('promotion.type') }}</label> -->
                            <el-form-item prop="type">
                                <!-- <el-select v-model="formData.type" :disabled="isView"
                                    :placeholder="t('promotion.typePlaceholder')">
                                    <el-option v-for="type in promotiontypeOptions" :key="type.value"
                                        :label="type.label" :value="type.value" />
                                </el-select> -->
                                <el-checkbox v-model="formData.allCourse" :disabled="isView" class="col-md-2">Tặng
                                    tiền</el-checkbox>
                                <!-- <el-form-item prop="discounts.discountMax"> -->
                                <div class="col-md-3">
                                    <!-- <label class="fs-6 fw-semibold mb-2 d-block">{{ t('promotion.couponCode') }}</label> -->
                                    <el-input class="fs-6 md-3" v-model="discounts0.fixedPrice" :disabled="isView"
                                        placeholder="Nhập số tiền tặng" />
                                </div>
                                <!-- </el-form-item> -->
                                <!-- <div class="col-md-6">
                                    <label class="fs-6 fw-semibold mb-2 d-block">% {{ t('promotion.discountAmount')
                                        }}</label>

                                </div> -->
                            </el-form-item>
                        </div>
                        <el-divider class="my-2" />
                        <div class="col-md-12">
                            <!-- <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('promotion.type') }}</label> -->
                            <el-form-item prop="type">
                                <!-- <el-select v-model="formData.type" :disabled="isView"
                                    :placeholder="t('promotion.typePlaceholder')">
                                    <el-option v-for="type in promotiontypeOptions" :key="type.value"
                                        :label="type.label" :value="type.value" />
                                </el-select> -->
                                <el-checkbox v-model="formData.allCourse" :disabled="isView" class="col-md-2">Tặng mã
                                    giảm
                                    giá</el-checkbox>
                                <!-- <el-form-item prop="discounts.discountMax"> -->
                                <div class="col-md-3">
                                    <!-- <label class="fs-6 fw-semibold mb-2 d-block">{{ t('promotion.couponCode') }}</label> -->
                                    <el-input class="col-md-3" v-model="discounts0.couponCode" :disabled="isView"
                                        placeholder="Nhập mã giảm giá" />
                                </div>
                                <!-- </el-form-item> -->
                                <!-- <div class="col-md-6">
                                    <label class="fs-6 fw-semibold mb-2 d-block">% {{ t('promotion.discountAmount')
                                        }}</label>

                                </div> -->
                            </el-form-item>
                        </div>
                        <el-divider class="my-2" />
                        <div class="col-md-12">
                            <!-- <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('promotion.type') }}</label> -->
                            <el-form-item prop="type">
                                <!-- <el-select v-model="formData.type" :disabled="isView"
                                    :placeholder="t('promotion.typePlaceholder')">
                                    <el-option v-for="type in promotiontypeOptions" :key="type.value"
                                        :label="type.label" :value="type.value" />
                                </el-select> -->
                                <el-checkbox v-model="formData.allCourse" :disabled="isView" class="col-md-2">Tháng
                                    học</el-checkbox>
                                <!-- <el-form-item prop="discounts.discountMax"> -->
                                <div class="col-md-3">
                                    <!-- <label class="fs-6 fw-semibold mb-2 d-block">{{ t('promotion.couponCode') }}</label> -->
                                    <el-input class="col-md-3" v-model="discounts0.couponCode" :disabled="isView"
                                        placeholder="Nhập số tháng tặng" />
                                </div>
                                <!-- </el-form-item> -->
                                <!-- <div class="col-md-6">
                                    <label class="fs-6 fw-semibold mb-2 d-block">% {{ t('promotion.discountAmount')
                                        }}</label>

                                </div> -->
                            </el-form-item>
                        </div>
                        <el-divider class="my-2" />
                        <div class="col-md-12">
                            <!-- <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('promotion.type') }}</label> -->
                            <el-form-item prop="type">
                                <!-- <el-select v-model="formData.type" :disabled="isView"
                                    :placeholder="t('promotion.typePlaceholder')">
                                    <el-option v-for="type in promotiontypeOptions" :key="type.value"
                                        :label="type.label" :value="type.value" />
                                </el-select> -->
                                <el-checkbox v-model="formData.allCourse" :disabled="isView" class="col-md-2">Học
                                    bổng</el-checkbox>
                                <!-- <el-form-item prop="discounts.discountMax"> -->
                                <div class="col-md-3">
                                    <!-- <label class="fs-6 fw-semibold mb-2 d-block">{{ t('promotion.couponCode') }}</label> -->
                                    <el-input class="col-md-3" v-model="discounts0.couponCode" :disabled="isView"
                                        placeholder="nhập % học bổng" />
                                </div>
                                <!-- cách ra một khoảng -->
                                <span class="col-md-1"></span>
                                <div class="fs-6 col-md-3">
                                    <!-- <label class="fs-6 fw-semibold mb-2 d-block">{{ t('promotion.couponCode') }}</label> -->
                                    <el-input class="col-md-3" v-model="discounts0.couponCode" :disabled="isView"
                                        placeholder="Nhập số tháng tặng" />
                                </div>
                                <!-- </el-form-item> -->
                                <!-- <div class="col-md-6">
                                    <label class="fs-6 fw-semibold mb-2 d-block">% {{ t('promotion.discountAmount')
                                        }}</label>

                                </div> -->
                            </el-form-item>
                        </div>
                        <el-divider class="my-2" />
                        <!--chiết khấu-->
                        <!-- <div v-if="formData.type === 0" class="col-12">
                            <div class="row g-3">
                                <div class="col-md-6">
                                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('promotion.discountMethod')
                                        }}</label>
                                    <el-form-item prop="discounts.method">
                                        <el-select v-model="discounts0.method" :disabled="isView"
                                            :placeholder="t('promotion.discountMethodPlaceholder')">
                                            <el-option :value="0" :label="t('discount.onOrderTotal')" />
                                            <el-option :value="1" :label="t('discount.onQuantity')" />
                                        </el-select>
                                    </el-form-item>
                                </div>
                                <div class="col-md-6">
                                    <label class="fs-6 fw-semibold mb-2 d-block">% {{ t('promotion.discountAmount')
                                        }}</label>
                                    <el-form-item prop="discounts.discountMax">
                                        <el-input-number v-model="discounts0.discountMax" :disabled="isView" :min="0" />
                                    </el-form-item>
                                </div>
                            </div>
                            <h6 class="fw-bold mt-4">{{ t('promotion.discountDetails') }}</h6>
                            <el-divider class="my-2" />
                            <el-table :data="discounts0.discountDetails">
                                <el-table-column :label="t('promotion.minAmount')" width="180">
                                    <template #default="{ row }">
                                        <el-input-number v-model="row.minAmount" :disabled="isView" :min="0" />
                                    </template>
</el-table-column>
<el-table-column :label="t('promotion.limit')" width="180">
    <template #default="{ row }">
                                        <el-input-number v-model="row.limit" :disabled="isView" :min="0" />
                                    </template>
</el-table-column>
<el-table-column :label="t('promotion.discountAmount')" width="180">
    <template #default="{ row }">
                                        <el-input-number v-model="row.discountAmount" :disabled="isView" :min="0" />
                                    </template>
</el-table-column>
<el-table-column :label="t('promotion.discountType')" width="180">
    <template #default="{ row }">
                                        <el-select v-model="row.discountType" :disabled="isView">
                                            <el-option :value="0" label="VND" />
                                            <el-option :value="1" label="%" />
                                        </el-select>
                                    </template>
</el-table-column>
<el-table-column v-if="!isView" :label="t('common.actions')">
    <template #default="{ $index }">
                                        <el-button @click="removeDiscountDetail($index)" type="danger" circle
                                            :icon="Delete"></el-button>
                                    </template>
</el-table-column>
</el-table>
<el-button v-if="!isView" class="mt-2" @click="addDiscountDetail" type="primary" plain>
    {{ t('promotion.addDetail') }}
</el-button>
</div> -->
                        <!-- Loại tặng quà -->
                        <div class="col-12">
                            <!-- <h6 class="fw-bold"> </h6> -->
                            <el-checkbox v-model="formData.allCourse" :disabled="isView" class="ms-3">Quà
                                tặng</el-checkbox>
                            <!-- <el-divider class="my-2" /> -->
                            <el-table :data="formData.promotionGift?.[0]?.promotionGiftDetails || []">
                                <el-table-column :label="t('promotion.giftName')" width="180">
                                    <template #default="{ row }">
                                        <!-- tạo danh sách quà tặng để lựa chọn từ bảng quà tặng -->
                                        <el-select v-model="row.giftName" :disabled="isView">
                                            <el-option v-for="gift in giftList" :key="gift.id" :value="gift.name"
                                                :label="gift.name" />
                                        </el-select>
                                    </template>
                                </el-table-column>
                                <el-table-column :label="t('promotion.giftQuantity')" width="180">
                                    <template #default="{ row }">
                                        <el-input-number v-model="row.quantityGift" :disabled="isView" :min="0" />
                                    </template>
                                </el-table-column>
                                <el-table-column v-if="!isView" :label="t('common.actions')">
                                    <template #default="{ $index }">
                                        <el-button @click="removePromotionGiftDetail($index)" type="danger" circle
                                            :icon="Minus"></el-button>
                                    </template>
                                </el-table-column>
                            </el-table>
                            <el-button v-if="!isView" class="mt-2" @click="addPromotionGiftDetail" type="primary" plain>
                                {{ t('promotion.addDetail') }}
                            </el-button>
                        </div>
                        <!-- Loại giảm giá -->


                    </div>
                </div>

                <div class="col-12 mb-4">
                    <h5 class="fw-bold">{{ t('promotion.description') }}</h5>
                    <!-- <el-divider class="my-2" /> -->
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :disabled="isView"
                            :placeholder="t('promotion.descriptionPlaceholder')" :rows="4" />
                    </el-form-item>
                </div>
            </div>
        </template>
    </BaseDialogForm>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue';
import type { PromotionModel, DiscountDetailModel, PromotionGiftDetailModel } from '@/api/PromotionApi';
import { useNotificationStore } from '@/stores/notificationStore';
import { formatDate } from '@/utils/format';
import { usePromotionStore } from '@/stores/promotionStore';
import { ElMessage } from 'element-plus';
import { Delete, Minus, Plus } from '@element-plus/icons-vue';
import { useRegionStore } from '@/stores/regionStore';
import { useCompanyStore } from '@/stores/companyStore';
import { useCourseStore } from '@/stores/courseStore';
import { useStudentStore } from '@/stores/studentStore';
import { usePromotionGroupStore } from '@/stores/promotionGroupStore';


const courseStore = useCourseStore();
const studentStore = useStudentStore();
const promotionGroupStore = usePromotionGroupStore();
const props = defineProps<{
    visible: boolean;
    mode?: 'create' | 'edit' | 'view';
    loading: boolean;
    promotionData: Partial<PromotionModel> | null;
}>();

const emit = defineEmits(['update:visible', 'submit', 'delete', 'close', 'import-students']);
const windowWidth = ref(window.innerWidth);
const { t } = useI18n();
const notificationStore = useNotificationStore();
const promotionStore = usePromotionStore();
const computedDialogWidth = computed(() => windowWidth.value < 700 ? '100%' : '60%');
const isView = computed(() => props.mode === 'view');
const isEdit = computed(() => props.mode === 'edit');
const isCreate = computed(() => props.mode === 'create');
const promotiontypeOptions = [
    { label: t('promotiontype.discount'), value: 0 },
    { label: t('promotiontype.gift'), value: 1 },
    { label: t('promotiontype.coupon'), value: 2 },
    { label: t('promotiontype.fixedPrice'), value: 3 },
];
const formRef = ref();
const baseDialogRef = ref();
const loading = ref(false);
const regionStore = useRegionStore()
const companyStore = useCompanyStore()
// simple reactive list of gifts used by the template (populate from API/store if available)
const giftList = ref<{ id: number; name: string }[]>([]);
// Options and UI state used by the template




const studentScopeType = ref<number>(0);
const studentIds = ref<any[]>([]);

const modeTitle = computed(() => {
    if (isView.value) return t('promotion.detailTitle');
    if (isEdit.value) return t('promotion.editTitle');
    return t('promotion.addTitle');
});

const defaultFormData: Partial<PromotionModel> = {
    name: '',
    startDate: new Date(),
    endDate: new Date(),
    promoCode: '',
    type: 0,
    discounts: [{
        discountDetails: [{ minAmount: null, limit: 0, discountType: 0, discountAmount: 0 }]
    }],
    promotionGift: [{
        promotionGiftDetails: [{ giftName: null, quantityGift: null }]
    }],
    promotionCoupon: [{
        minQuantity: null, limit: null, couponCode: null
    }],
    promotionFixedPrice: [{
        minPrice: null, limit: null, priceSale: null
    }],
};

const formData = ref<Partial<PromotionModel>>({ ...defaultFormData });

const discounts0 = computed({
    get() {
        if (!formData.value.discounts) {
            formData.value.discounts = [{
                discountDetails: [{ minAmount: null, limit: 0, discountType: 0, discountAmount: 0 }],
            }];
        }
        if (!formData.value.discounts[0]) {
            formData.value.discounts[0] = {
                discountDetails: [{ minAmount: null, limit: 0, discountType: 0, discountAmount: 0 }],
            };
        }
        return formData.value.discounts[0] as any;
    },
    set(val: any) {
        if (!formData.value.discounts) {
            formData.value.discounts = [val];
        } else {
            formData.value.discounts[0] = val;
        }
    },
});

const promotionCoupon0 = computed({
    get() {
        if (!formData.value.promotionCoupon) {
            formData.value.promotionCoupon = [{ minQuantity: null, limit: null, couponCode: null }];
        }
        if (!formData.value.promotionCoupon[0]) {
            formData.value.promotionCoupon[0] = { minQuantity: null, limit: null, couponCode: null };
        }
        return formData.value.promotionCoupon[0] as any;
    },
    set(val: any) {
        if (!formData.value.promotionCoupon) {
            formData.value.promotionCoupon = [val];
        } else {
            formData.value.promotionCoupon[0] = val;
        }
    },
});

const promotionFixedPrice0 = computed({
    get() {
        if (!formData.value.promotionFixedPrice) {
            formData.value.promotionFixedPrice = [{ minPrice: null, limit: null, priceSale: null }];
        }
        if (!formData.value.promotionFixedPrice[0]) {
            formData.value.promotionFixedPrice[0] = { minPrice: null, limit: null, priceSale: null };
        }
        return formData.value.promotionFixedPrice[0] as any;
    },
    set(val: any) {
        if (!formData.value.promotionFixedPrice) {
            formData.value.promotionFixedPrice = [val];
        } else {
            formData.value.promotionFixedPrice[0] = val;
        }
    },
});

const rules = {
    name: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    startDate: [{ required: true, message: t('validation.required'), trigger: 'change' }],
    endDate: [{
        required: true, message: t('validation.required'), trigger: 'change',
        validator: (rule: any, value: Date) => {
            if (formData.value.startDate && value < formData.value.startDate) {
                return new Error(t('validation.endDateAfterStartDate'));
            }
            return true;
        }
    }],
    code: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    type: [{ required: true, message: t('validation.required'), trigger: 'change' }],
};
onMounted(() => {
    companyStore.fetchAllCompanies();
    courseStore.fetchAllCourses();
    studentStore.fetchAllStudents();
    promotionGroupStore.fetchAll();
});
watch(
    () => props.promotionData,
    (data) => {
        if (data && data.id) {
            formData.value = {
                ...data,
                //timeRange: [new Date(data.startDate!), new Date(data.endDate!)],
            };
            // sync studentIds with incoming promotionData (handle cases where promotionStudent may be array of ids or objects)
            const existing = formData.value.promotionStudent || [];
            studentIds.value = existing.map((s: any) => {
                if (s == null) return s;
                // if s is a number/string id
                if (typeof s === 'number' || typeof s === 'string') return s;
                // if s is an object with studentId or id
                return s.studentId ?? s.id ?? s;
            });
        } else {
            formData.value = { ...defaultFormData };
            studentIds.value = [];
        }
    },
    { immediate: true }
);


function onSubmit() {
    const form = baseDialogRef.value?.formRef;
    form.validate(async (valid: boolean) => {
        if (valid) {
            loading.value = true;
            const payload = { ...formData.value };
            //  payload.startDate = payload.timeRange[0]!= undefined ?  payload.timeRange[0]: new Date();
            //   payload.endDate = payload.timeRange[1];
            // delete payload.timeRange;
            console.log('Dữ liệu đang gửi:', payload);

            emit('submit', payload);
            loading.value = false;
        } else {
            notificationStore.showToast('error', { key: 'validation.formInvalid' });
        }
    });
}

function onDelete() {
    emit('delete', formData.value);
}

function closeModal() {
    emit('update:visible', false);
    emit('close');
}

// Discount Details
function addDiscountDetail() {
    if (!formData.value.discounts) {
        formData.value.discounts = [{ discountDetails: [] }];
    }
    if (!formData.value.discounts[0].discountDetails) {
        formData.value.discounts[0].discountDetails = [];
    }
    formData.value.discounts[0].discountDetails.push({ minAmount: null, limit: 0, discountType: 0, discountAmount: 0 });
}

function removeDiscountDetail(index: number) {
    formData.value.discounts?.[0].discountDetails?.splice(index, 1);
}

// Promotion Gift Details
function addPromotionGiftDetail() {
    // if (!formData.value.promotionGift) {
    //     function addPromotionStudent(selected: any) {
    //         // 'selected' comes from the multiple el-select and is an array of selected student ids
    //         studentIds.value = Array.isArray(selected) ? selected : [selected];
    //         // store simple IDs into formData.promotionStudent so backend can process them; adjust if backend expects objects
    //         formData.value.promotionStudent = studentIds.value.slice();
    //         console.log('Selected students:', formData.value.promotionStudent);
    //     }
    // }
    formData.value.promotionGift![0]!.promotionGiftDetails!.push({ giftName: null, quantityGift: null });
}

function addPromotionStudent(selected: any) {
    // store the selected students (could be array of student objects) into formData
    // keeps template v-model and @change handler consistent with TypeScript
    studentIds.value = Array.isArray(selected) ? selected : [selected];
    // store simple IDs into formData.promotionStudent so backend can process them; adjust if backend expects objects
    formData.value.promotionStudent = studentIds.value.slice();
    console.log('Selected students:', formData.value.promotionStudent);

}

function getStudentNumberQualified() {
    const students = studentStore.students || [];
    return students.filter((s: any) => {
        if (!formData.value.allCompany && formData.value.companyId && s.companyId !== formData.value.companyId) return false;
        if (!formData.value.allCourse && formData.value.courseId && s.courseId !== formData.value.courseId) return false;
        return true;
    }).length;
}

function removePromotionGiftDetail(index: number) {
    formData.value.promotionGift?.[0].promotionGiftDetails?.splice(index, 1);
}
</script>