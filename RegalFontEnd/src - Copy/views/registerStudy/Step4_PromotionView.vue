<template>
    <div class="p-3">
        <h3 class="fw-bold mb-4">{{ t('registerStudy.selectPromotion') }}</h3>

        <div class="mb-5">
            <div class="d-flex align-items-center mb-3">
                <i class="el-icon-arrow-down fs-5 me-2"></i>
                <h5 class="fw-bold mb-0">{{ t('promotion.globalPromotion') }}</h5>
            </div>

            <div class="ms-4">
                <el-checkbox-group v-model="selectedGlobalPromotions">
                    <div v-for="promo in globalPromotions" :key="String(promo.id)" class="mb-2">
                        <el-checkbox :label="promo.id">
                            {{ promo.name }} ({{ promo.description }})
                        </el-checkbox>
                    </div>
                </el-checkbox-group>
                <div v-if="globalPromotions.length === 0" class="text-body-secondary fst-italic">
                    {{ t('promotion.noGlobalPromotion') }}
                </div>
            </div>
        </div>

        <el-divider />

        <div class="mb-3">
            <div class="d-flex align-items-center mb-3">
                <i class="el-icon-arrow-down fs-5 me-2"></i>
                <h5 class="fw-bold mb-0">{{ t('promotion.productPromotion') }}</h5>
            </div>

            <div class="card p-3">
                <div v-for="course in courses" :key="course.id" class="mb-4">
                    <label class="fw-semibold">{{ course.code }} - {{ course.name }}</label>
                    <el-select v-model="selectedProductPromotions[course.id]"
                        :placeholder="t('promotion.selectPromotionPlaceholder')" class="w-100 mt-1" clearable>
                        <el-option :label="t('common.noPromotion')" value="" />

                        <el-option-group :label="t('promotion.availablePromotions')">
                            <el-option v-for="promo in course.availablePromotions" :key="promo.id" :label="promo.id"
                                :value="promo.id">
                                <span style="float: left; white-space: normal;">{{ promo.name }}</span>
                                <span style="float: right; color: var(--el-text-color-secondary); font-size: 13px;">({{
                                    promo.description }})</span>
                            </el-option>
                        </el-option-group>

                        <!-- <el-option-group v-if="course.availableGifts && course.availableGifts.length > 0"
                            :label="t('promotion.availableGifts')">
                            <el-option v-for="gift in course.availableGifts" :key="gift.id" :label="gift.id"
                                :value="gift.id">
                                <span style="float: left;">
                                    <i v-if="gift.isApplied" class="el-icon-check text-success me-1"></i>
                                    {{ gift.name }}
                                </span>
                            </el-option>
                        </el-option-group> -->
                    </el-select>
                </div>
                <div v-if="courses.length === 0" class="text-center text-body-secondary fst-italic">
                    {{ t('promotion.noCoursesSelected') }}
                </div>
            </div>
        </div>

        <div class="d-flex justify-content-end mt-5">
            <el-button @click="handleCancel">{{ t('common.cancel') }}</el-button>
            <el-button type="primary" @click="handleConfirm">{{ t('common.confirm') }}</el-button>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, watch, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRegisterStudyStore } from '@/stores/registerStudyStore';
import { usePromotionStore } from '@/stores/promotionStore';
//import type { PromotionSelectionDto } from '@/api/RegisterStudyApi';
import { ElMessage, linkEmits } from 'element-plus';
import { defineStore } from 'pinia';
import type { PromotionModel } from '@/api/PromotionApi';
import type { RegisterPromotionListModel } from '@/api/RegisterStudyApi';



const emit = defineEmits(['confirm', 'cancel']);
const { t } = useI18n();
const store = useRegisterStudyStore();

const PromoItem = ref<PromotionModel[]>([]);
const promotionStore = usePromotionStore();
// ------------------------------------


// --- STATE CỤC BỘ ---
const selectedGlobalPromotions = ref<string[]>([]);
const selectedProductPromotions = ref<Record<string, string>>({});

// --- DỮ LIỆU HIỂN THỊ ---

// Lấy các danh sách khuyến mãi toàn cục từ Store
const globalPromotions = computed(() => {
    //lấy danh sách khuyến mãi khả dụng từ store thỏa mã đi điều kiện allCompany = true, allCourse = true, allstudent = true
    return promotionStore.promotionAvaliable.filter(p => (p.allCompany || p.companyId === store.formData.companyId) && p.allCourse && p.allStudent);
});
console.log('Global Promotions:', globalPromotions.value);

// Lấy danh sách khóa học đang được chọn và khuyến mãi khả dụng
const courses = computed(() => {
    return store.formData.detailRegisterStudys?.filter(d => d.courseId).map(d => {
        // Lấy danh sách khuyến mãi theo khóa học
        const promoData = promotionStore.promotions.filter(p => p.courseId === d.courseId);
        return {
            id: d.courseId || 'temp',
            code: d.courseName || '',
            name: d.courseName || 'Khóa học',
            availablePromotions: promoData,
            // availableGifts: promoData.gifts,
        };
    }) || [];
});

// --- WATCH LOGIC: Load trạng thái ban đầu từ Store ---
watch(() => store.formData.registerPromotion, (initialSelection) => {
    selectedGlobalPromotions.value = [];
    selectedProductPromotions.value = {};
}, { immediate: true, deep: true });

// --- Xử lý sự kiện ---

const handleCancel = () => {
    emit('cancel'); // Gọi hàm cancel trong RegisterStudyWizard để quay về Bước 1
};
onMounted(() => {
    promotionStore.fetchPromotionValues();
});
const handleConfirm = () => {
    // Tính tổng tiền của các khóa học đã đăng ký
    var totalAmount = store.formData.detailRegisterStudys?.reduce((acc, d) => acc + (d.totalAmount ?? 0), 0) || 0;

    var registerPromoList: RegisterPromotionListModel[] = [];
    //Lấy danh sách các khuyến mãi được chọn theo mã chọn selectedGlobalPromotions.value từ danh sách globalPromotion
    var i = 0;
    var tongkhuyenmai = 0;
    for (const promoId of selectedGlobalPromotions.value) {
        const promo = globalPromotions.value.filter(p => p.id === promoId).map(p => p)[0];
        console.log('promotion thứ ' + (i++));

        if (promo) {
            //Xử lý khuyến mãi theo loại - loại =0: giảm giá, loại =1: quà tặn
            let disAmount = 0;
            if (promo.type === 0) {
                const discount = promo.discounts?.filter(d => d.discountDetails && d.discountDetails.length > 0);
                discount?.forEach(d => {
                    console.log('Discount Max: ' + (d.discountMax ?? 'N/A'));

                    const dis = computed(() => discount.filter(detail => detail.promotionId === d.promotionId));
                    dis.value.forEach(detail => {
                        //lấy thông tin về discountDetail từ promotion.discounts để hiển thị thông tin khuyến mãi được chọn
                        var detailInfo = detail.discountDetails?.map(d => ({
                            type: d.discountType,
                            minAmount: d.minAmount,
                            limit: d.limit,
                            amount: d.discountAmount,
                        })) ?? [];
                        //tính tổng các giá trị khuến mại theo từng discountDetail
                        //biết rằng nếu tổng tiền >= minAmount thì mới áp dụng khuyến mãi và chỉ tính một lần với giá trị lớn nhất
                        //nếu giá trị tổng tiền mà lớn hơn giá trị maxAmount thì lấy giá trị maxAmount để tính khuyến mãi
                        //nếu discountType = 0 thì là tiền mặt, nếu discountType = 1 thì là phần trăm

                        detailInfo.forEach(di => {
                            let discountAmountMax = 0;
                            if (totalAmount >= (di.minAmount ?? 0)) {

                                if (di.type === 0) {
                                    discountAmountMax = di.amount ?? 0;
                                } else if (di.type === 1) {
                                    discountAmountMax = totalAmount * ((di.amount ?? 0) / 100);
                                }
                                //kiểm tra giá trị khuyến mãi không vượt quá limit
                                if (di.limit && discountAmountMax > di.limit) {
                                    discountAmountMax = di.limit;
                                }

                            } else {
                                console.log('Tổng tiền không đủ điều kiện áp dụng khuyến mãi: ' + (di.minAmount ?? 0));
                            }
                            disAmount = Math.max(disAmount, discountAmountMax);
                        });
                        disAmount = Math.min(disAmount, d.discountMax ?? disAmount);
                        tongkhuyenmai += disAmount;
                        console.log('Áp dụng khuyến mãi: ' + disAmount);
                    });
                });

            }
            else if (promo.type === 1) {
                console.log('Selected Global Gift Promotion: ' + promo.name + ' (' + promo.description + ')');
            }
            console.log('Tổng khuyến mại : ' + (i++) + ' ' + disAmount);
            registerPromoList.push({
                promotionId: promo.id || null,
                registerStudyId: store.formData.id || null,
                discountAmount: disAmount, // Sẽ được tính sau
                promotionName: promo.name,
            });

        }
    }
    store.formData.registerPromotion = registerPromoList;

    store.updateFormData({ registerPromotion: registerPromoList });
    // store.updateFormData({ totalDiscountAmount: tongkhuyenmai });


    // 1. Tạo DTO kết quả
    // const selectionResult: PromotionSelectionDto = {
    //     global: selectedGlobalPromotions.value,
    //     product: selectedProductPromotions.value,
    // };

    // // 2. Cập nhật Store (chủ yếu là promotionSelections)
    // store.updateFormData({ promotionSelections: selectionResult });

    // 3. Thông báo
    ElMessage.success(t('promotion.appliedSuccess'));

    // 4. Chuyển về Bước 1
    emit('confirm');
};
</script>