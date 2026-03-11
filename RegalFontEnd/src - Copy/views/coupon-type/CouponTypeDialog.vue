<template>
    <BaseDialogForm ref="baseDialogRef" :visible="visible" :title="modeTitle" :form-data="formData" :rules="rules"
        :mode="props.mode" :loading="loading" @submit="onSubmit" @delete="onDelete" :width="computedDialogWidth"
        @update:visible="emit('update:visible', $event)" @close="closeModal" @activated="onActivated"
        @opennewdialog="onOpennewdialog" :showActive="showActived" :showOpenNewDialog="showOpenNewDialog">
        <template #form>
            <div class="row g-3">
                <div class="col-12">
                    <h5 class="fw-bold">{{ t('couponType.generalInfo') }}</h5>
                    <el-divider class="my-2" />
                    <el-row :gutter="20">
                        <el-col :span="12">
                            <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('couponType.name') }}</label>
                            <el-form-item prop="name">
                                <el-input v-model="formData.name" :disabled="isView" />
                            </el-form-item>
                        </el-col>
                        <el-col :span="12">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.code') }}</label>
                            <el-form-item prop="code">
                                <el-input v-model="formData.code" :disabled="isView" />
                            </el-form-item>
                        </el-col>
                        <el-col :span="12">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('common.status') }}</label>
                            <el-form-item prop="status">
                                <el-select v-model="formData.status" :disabled="isView">
                                    <el-option :label="t('common.couponTypeStatusInActive')" :value="0" />
                                    <el-option :label="t('common.couponTypeStatusActive')" :value="1" />
                                    <el-option :label="t('common.couponTypeStatusDiActive')" :value="2" />
                                </el-select>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </div>

                <div class="col-12">
                    <h5 class="fw-bold">{{ t('couponType.validity') }}</h5>
                    <el-divider class="my-2" />
                    <el-row :gutter="20">
                        <el-col :span="12">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.dueType') }}</label>
                            <el-form-item prop="type">
                                <el-select v-model="formData.dueType" :disabled="isView"
                                    @change="handleValidityTypeChange">
                                    <el-option :label="t('couponType.dueTypes.duration')" :value="DueType.duration" />
                                    <el-option :label="t('couponType.dueTypes.dateRange')" :value="DueType.dateRange" />
                                </el-select>
                            </el-form-item>
                        </el-col>
                        <el-col :span="12" v-if="formData.dueType == DueType.duration">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.durationInDays') }}</label>
                            <el-form-item prop="durationInDays">
                                <el-input-number v-model="formData.durationInDays" :min="1" :disabled="isView" />
                            </el-form-item>
                        </el-col>
                        <el-col :span="6" v-if="formData.dueType == DueType.dateRange">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.startDate') }}</label>
                            <el-form-item prop="startDate">
                                <el-date-picker v-model="formData.startDate" type="date" :disabled="isView"
                                    format="DD-MM-YYYY" value-format="YYYY-MM-DD" />
                            </el-form-item>
                        </el-col>
                        <el-col :span="6" v-if="formData.dueType === DueType.dateRange">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.endDate') }}</label>
                            <el-form-item prop="endDate">
                                <el-date-picker v-model="formData.endDate" type="date" :disabled="isView"
                                    format="DD-MM-YYYY" value-format="YYYY-MM-DD" />
                            </el-form-item>
                        </el-col>
                    </el-row>
                </div>

                <div class="col-12">
                    <h5 class="fw-bold">{{ t('couponType.generationStructure') }}</h5>
                    <el-divider class="my-2" />
                    <el-row :gutter="20">
                        <el-col :span="8">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.prefix') }}</label>
                            <el-form-item prop="prefix">
                                <el-input v-model="formData.prefix" :disabled="isView" />
                            </el-form-item>
                        </el-col>
                        <el-col :span="8">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.suffix') }}</label>
                            <el-form-item prop="suffix">
                                <el-input v-model="formData.suffix" :disabled="isView" />
                            </el-form-item>
                        </el-col>
                        <el-col :span="8">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.characterCount') }}</label>
                            <el-form-item prop="characterCount">
                                <el-input-number v-model="formData.characterCount" :min="1" :disabled="isView" />
                            </el-form-item>
                        </el-col>
                    </el-row>
                </div>

                <div class="col-12">
                    <h5 class="fw-bold">{{ t('couponType.applicationConditions') }}</h5>
                    <el-divider class="my-2" />
                    <el-row :gutter="20">
                        <el-col :span="24">
                            <el-checkbox v-model="formData.applyWith" :disabled="isView">
                                {{ t('couponType.applyWithOther') }}
                            </el-checkbox>
                        </el-col>

                        <el-col :span="24">
                            <label class="fs-6 fw-semibold mb-2 d-block me-3" style="min-width: 160px;">{{
                                t('couponType.company') }}</label>
                            <div class="d-flex align-items-center">
                                <el-form-item prop="companyIds" class="w-100">
                                    <el-select v-model="companyIds" filterable clearable multiple
                                        :disabled="isView || formData.isForAllCompany"
                                        @change="handleSelectCompanyChange">
                                        <el-option v-for="c in companyStore.companies" :key="c.id"
                                            :label="c.companyName" :value="c.id" />
                                    </el-select>
                                </el-form-item>
                                <el-checkbox v-model="formData.isForAllCompany" :disabled="isView" class="ms-2">{{
                                    t('common.all') }}</el-checkbox>
                            </div>
                        </el-col>

                        <el-col :span="24">
                            <label class="fs-6 fw-semibold mb-2 d-block" style="min-width: 160px;">{{
                                t('couponType.course') }}</label>
                            <div class="d-flex align-items-center">
                                <el-form-item prop="courseIds" class="w-100">
                                    <el-select v-model="courseIds" filterable clearable
                                        :disabled="isView || formData.isForAllCourse" @change="handleSelectCourseChange"
                                        multiple>
                                        <el-option v-for="c in courseStore.courses" :key="c.id" :label="c.courseName"
                                            :value="c.id" />
                                    </el-select>
                                </el-form-item>
                                <el-checkbox v-model="formData.isForAllCourse" :disabled="isView" class="ms-2">{{
                                    t('common.all') }}</el-checkbox>
                            </div>
                        </el-col>

                        <el-col :span="12">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.minMonth') }}</label>
                            <el-form-item prop="minQuantity">
                                <el-input-number v-model="formData.minQuantity" :min="0" :disabled="isView" />
                            </el-form-item>
                        </el-col>
                        <el-col :span="12">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.maxMonth') }}</label>
                            <el-form-item prop="minQuantity">
                                <el-input-number v-model="formData.minQuantity" :min="0" :disabled="isView" />
                            </el-form-item>
                        </el-col>
                    </el-row>
                </div>

                <div class="col-12">
                    <h5 class="fw-bold">{{ t('couponType.studentScope') }}</h5>
                    <el-divider class="my-2" />
                    <el-row :gutter="20">
                        <el-col :span="24">
                            <el-radio-group v-model="formData.isForAllStudents" :disabled="isView">
                                <el-radio :value="true">{{ t('couponType.allStudents') }}</el-radio>
                                <el-radio :value="false">{{ t('couponType.specificStudents') }}</el-radio>
                            </el-radio-group>
                            <span class="ms-3 text-primary">{{ t('couponType.studentsMatched') }}</span>
                        </el-col>
                        <el-col :span="24" v-if="!formData.isForAllStudents">
                            <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.selectStudents')
                            }}</label>
                            <el-form-item prop="studentIds">
                                <el-select v-model="studentIds" multiple filterable clearable :disabled="isView"
                                    @change="handleSelectStudentChange">
                                    <!-- Giả lập options, thay thế bằng dữ liệu thực tế -->
                                    <el-option v-for="std in studentStore.students" :key="std.id" :label="std.fullName"
                                        :value="std.id" />

                                </el-select>
                                <el-button class="mt-2" type="primary" plain
                                    @click="emit('import-students', studentIds)">
                                    {{ t('promotion.importStudents', { count: studentIds.length }) }}
                                </el-button>
                            </el-form-item>
                        </el-col>
                    </el-row>
                </div>

                <div class="col-12">
                    <h5 class="fw-bold">{{ t('couponType.promotionDetails') }}</h5>
                    <el-divider class="my-2" />
                    <!-- <el-row gutter="20" class="mb-3">
                        <el-col :span="24">
                            <label class="fs-6 fw-semibold mb-2 d-block me-3">{{ t('couponType.type')
                                }}</label>
                            <el-form-item prop="type">
                                <el-select v-model="formData.type" :disabled="isView"
                                    @change="handleValidityTypeChange">
                                    <el-option v-for="type in typeOptions" :key="type.value" :label="type.label"
                                        :value="type.value" />
                                </el-select>
                            </el-form-item>
                        </el-col>
                    </el-row> -->
                    <!-- Bắt đầu khuyến mại -->
                    <div class="col-md-12">
                        <!-- <label class="required fs-6 fw-semibold mb-2 d-block">{{ t('promotion.type') }}</label> -->
                        <el-form-item prop="type">
                            <!-- <el-select v-model="formData.type" :disabled="isView"
                                    :placeholder="t('promotion.typePlaceholder')">
                                    <el-option v-for="type in promotiontypeOptions" :key="type.value"
                                        :label="type.label" :value="type.value" />
                                </el-select> -->
                            <el-checkbox v-model="formData.isForAllCourse" :disabled="isView" class="ms-3">Chiết
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

                            <el-checkbox v-model="formData.isForAllCourse" :disabled="isView" class="col-md-2">Tặng
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
                            <el-checkbox v-model="formData.isForAllCourse" :disabled="isView" class="col-md-2">Tặng mã
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
                            <el-checkbox v-model="formData.isForAllCourse" :disabled="isView" class="col-md-2">Tháng
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
                            <el-checkbox v-model="formData.isForAllCourse" :disabled="isView" class="col-md-2">Học
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

                    <!-- Loại tặng quà -->
                    <div class="col-12">
                        <!-- <h6 class="fw-bold"> </h6> -->
                        <el-checkbox v-model="formData.isForAllCourse" :disabled="isView" class="ms-3">Quà
                            tặng</el-checkbox>
                        <!-- <el-divider class="my-2" /> -->
                        <el-table :data="formData.couponTypeGifts?.[0]?.couponTypeGiftDetail || []">
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
                                    <el-button @click="removeCouponTypeGiftDetail($index)" type="danger" circle
                                        :icon="Minus"></el-button>
                                </template>
                            </el-table-column>
                        </el-table>
                        <el-button v-if="!isView" class="mt-2" @click="addCouponTypeGiftDetail" type="primary" plain>
                            {{ t('promotion.addDetail') }}
                        </el-button>
                    </div>

                    <!-- Kết thúc khuyến mại -->
                    <!-- <div class="col-12">
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
                                <label class="fs-6 fw-semibold mb-2 d-block">{{ t('promotion.maxDiscount')
                                }}</label>
                                <el-form-item prop="discounts.discountMax">
                                    <CurrencyInput v-model="discounts0.discountMax" :disabled="isView" locale="vi-VN"
                                        currency="VND" />
                                </el-form-item>
                            </div>
                        </div>
                        <h6 class="fw-bold mt-4">{{ t('promotion.discountDetail') }}</h6>
                        <el-divider class="my-2" />
                        <el-table :data="discounts0.couponTypeDiscountDetail">
                            <el-table-column
                                :label="discounts0.method == 0 ? t('promotion.minAmount') : t('promotion.minQuantity')"
                                width="180">
                                <template #default="{ row }">
                                    <CurrencyInput v-if="discounts0.method == 0" v-model="row.minAmount"
                                        :disabled="isView" :min="0" locale="vi-VN" currency="VND" />
                                    <el-input-number v-if="discounts0.method == 1" v-model="row.minAmount"
                                        :disabled="isView" :min="0" />
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('promotion.limit')" width="180">
                                <template #default="{ row }">
                                    <el-input-number v-model="row.limit" :disabled="isView" :min="0" />
                                </template>
                            </el-table-column>
                            <el-table-column :label="labelForDiscountAmount" width="180">
                                <template #default="{ row }">
                                    <CurrencyInput v-if="row.discountType == 1" v-model="row.discountAmount"
                                        :disabled="isView" :min="0" locale="vi-VN" currency="VND" />
                                    <el-input-number v-else v-model="row.discountAmount" :disabled="isView" :min="0" />
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('promotion.discountType')" width="180">
                                <template #default="{ row }">
                                    <el-select v-model="row.discountType" :disabled="isView">
                                        <el-option :value="1" label="Giảm tiền" />
                                        <el-option :value="0" label="%" />
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
                        <el-button class="mt-2" @click="addDiscountDetail" type="primary" plain>
                            {{ t('promotion.addDetail') }}
                        </el-button>
                    </div> -->
                    <!-- <div v-for="(discount, index) in formData.couponTypeDiscounts" :key="index">
                        <el-row :gutter="20">
                            <el-col :span="12">
                                <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.discountMethod')
                                }}</label>
                                <el-form-item prop="couponTypeDiscounts[index].method">
                                    <el-select v-model="discount.method" :disabled="isView">
                                        <el-option v-for="method in discountMethodOption" :label="method.label"
                                            :value="method.value" :key="method.value" />

                                    </el-select>
                                </el-form-item>
                            </el-col>
                            <el-col :span="12">
                                <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.discountMax') }}</label>
                                <el-form-item prop="couponTypeDiscounts[index].discountMax">
                                    <el-input-number v-model="discount.discountMax" :min="0" :disabled="isView" />
                                </el-form-item>
                            </el-col>
                        </el-row>

                        <el-table :data="discount.couponTypeDiscountDetail" class="mt-3">
                            <el-table-column :label="t('couponType.minAmount')" width="150">
                                <template #default="{ row }">
                                    <el-input-number v-model="row.minAmount" :min="0" :disabled="isView" />
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('couponType.discountAmount')" width="150">
                                <template #default="{ row }">
                                    <el-input-number v-model="row.discountAmount" :min="0" :disabled="isView" />
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('couponType.discountType')" width="150">
                                <template #default="{ row }">
                                    <el-select v-model="row.discountType" :disabled="isView">
                                        <el-option label="%" :value="0" />
                                        <el-option :label="t('couponType.value')" :value="1" />
                                    </el-select>
                                </template>
                            </el-table-column>
                            <el-table-column :label="t('couponType.limit')" width="150">
                                <template #default="{ row }">
                                    <el-input-number v-model="row.limit" :min="0" :disabled="isView" />
                                </template>
                            </el-table-column>
                            <el-table-column v-if="!isView" :label="t('common.actions')">
                                <template #default="{ $index }">
                                    <el-button size="small" style="background: none;border: none;"
                                        @click="removeDiscountDetail(index, $index)" round>
                                        <i class="bi bi-trash3 text-danger"></i>
                                    </el-button>
                                </template>
                            </el-table-column>
                        </el-table>
                        <el-button v-if="!isView" class="mt-2" @click="addDiscountDetail(index)" type="primary" plain>
                            {{ t('couponType.addDetail') }}
                        </el-button>
                    </div> -->
                </div>

                <div class="col-12">
                    <label class="fs-6 fw-semibold mb-2 d-block">{{ t('couponType.description') }}</label>
                    <el-form-item prop="description">
                        <el-input type="textarea" v-model="formData.description" :rows="3" :disabled="isView"
                            :placeholder="t('couponType.descriptionPlaceholder')" />
                    </el-form-item>
                </div>
            </div>

        </template>
        <!-- bổ sung thêm nú phát hành coupon -->

    </BaseDialogForm>
    <CouponDialog v-model:visible="showIssueModal" :coupon-type-id="formData.id ?? null"
        :is-for-all-students="Boolean(formData.isForAllStudents)" :student-ids="formData.studentIds ?? null"
        @success="handleIssueSuccess" />

</template>

<script setup lang="ts">
import { computed, h, onMounted, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import BaseDialogForm from '@/components/base-dialog-form/BaseDialogForm.vue'
import type { CouponTypeModel, CouponTypeDiscountDetailModel, CouponTypeDiscountModel } from '@/api/CouponTypeApi'
import { useNotificationStore } from '@/stores/notificationStore'
import { useCouponTypeStore } from '@/stores/couponTypeStore'
import { useRegionStore } from '@/stores/regionStore';
import { useCompanyStore } from '@/stores/companyStore';
import { useCourseStore } from '@/stores/courseStore';
import { useStudentStore } from '@/stores/studentStore';
import { Delete, Plus, Minus } from '@element-plus/icons-vue';
import { DiscountMeThod, DueType, PromotionType } from '@/types'
import { useCouponIssueStore } from '@/stores/couponIssueStore';
import CurrencyInput from '@/components/currency-input/CurrencyInput.vue'
import CouponDialog from './CouponDialog.vue';
//tạo một biến CompanyIds khởi tạo rỗng; sẽ được thiết lập trong onMounted hoặc khi props.couponTypeData thay đổi
const companyIds = ref<string[]>([]);
// danh sách quà tặng được dùng trong template (khi chưa có store/backend, khởi tạo rỗng để tránh lỗi TS)
const giftList = ref<{ id: string | number; name: string }[]>([]);
//tạo một biến StudentIds
const studentIds = ref<string[]>([]);
//tạo một biến CourseIds
const courseIds = ref<string[]>([]);
// Giả định các store cần thiết khác
// import { useCompanyStore } from '@/stores/companyStore'
// import { useCourseStore } from '@/stores/courseStore'
//khai báo biến showActivedModal
// const showActived = ref(true);
// const showOpenNewDialog = ref(false);
const windowWidth = ref(window.innerWidth);
const props = defineProps<{
    visible: boolean,
    mode?: 'create' | 'edit' | 'view',
    loading: boolean,
    showActived?: boolean,
    showOpenNewDialog?: boolean,
    couponTypeData: Partial<CouponTypeModel> | null
}>()
const emit = defineEmits(['update:visible', 'submit', 'delete', 'close', 'activated', 'import-students', 'open-new-dialog'])
const { t } = useI18n()
const computedDialogWidth = computed(() => windowWidth.value < 700 ? '100%' : '50%');
const couponTypeStore = useCouponTypeStore()
const notificationStore = useNotificationStore()
const isView = computed(() => props.mode === 'view')
const isEdit = computed(() => props.mode === 'edit')
const isCreate = computed(() => props.mode === 'create')
const companyStore = useCompanyStore()
const courseStore = useCourseStore()
const studentStore = useStudentStore()
const modeTitle = computed(() => {
    if (isView.value) return t('couponType.detailTitle')
    if (isEdit.value) return t('couponType.editTitle')
    return t('couponType.addTitle')
})

const baseDialogRef = ref()
const loading = ref(false)
const showIssueModal = ref(false)

// HÀM MỚI: Mở dialog phát hành coupon
function openIssueDialog() {
    if (!formData.value.id) {
        notificationStore.showToast('error', { key: 'coupon.saveTypeFirst' });
        return;
    }
    showIssueModal.value = true;
}
// HÀM MỚI: Xử lý sau khi phát hành thành công
function handleIssueSuccess() {
    // Sau khi phát hành thành công, có thể làm mới danh sách coupon type hoặc chuyển tab
    notificationStore.showToast('success', { key: 'toast.createSuccess', params: { model: t('models.couponIssue') } });
    // Nếu CouponTypeDialog có tab hiển thị danh sách CouponIssue, ta sẽ refresh nó ở đây.
}
const typeOptions = ref([
    { value: PromotionType.Discount, label: t('typeOptions.Duration') },
    { value: PromotionType.FixedPrice, label: t('typeOptions.FixedPrice') },
    { value: PromotionType.Gift, label: t('typeOptions.Gift') },
    { value: PromotionType.Coupon, label: t('typeOptions.Coupon') }
]);
const discountMethodOption = ref([
    { value: DiscountMeThod.OrderTotal, label: t('DiscountMethodOption.OrderTotal') },
    { value: DiscountMeThod.Quantity, label: t('DiscountMethodOption.Quantity') },

]);

// Data mặc định cho CouponTypeDiscountDetail
// const defaultDiscountDetail: Partial<CouponTypeDiscountDetailModel> = {
//     minAmount: null, limit: 0, discountType: 0, discountAmount: 0
// };
// // Data mặc định cho CouponTypeDiscount
// const defaultDiscount: Partial<CouponTypeDiscountModel> = {
//     method: 0, discountMax: null, couponTypeDiscountDetail: [{ ...defaultDiscountDetail } as CouponTypeDiscountDetailModel]
// };
const discounts0 = computed({
    get() {
        if (!formData.value.couponTypeDiscounts) {
            formData.value.couponTypeDiscounts = [{
                couponTypeDiscountDetail: [{ minAmount: null, limit: 0, discountType: 0, discountAmount: 0 }],
            }];
        }
        if (!formData.value.couponTypeDiscounts[0]) {
            formData.value.couponTypeDiscounts[0] = {
                couponTypeDiscountDetail: [{ minAmount: null, limit: 0, discountType: 0, discountAmount: 0 }],
            };
        }
        return formData.value.couponTypeDiscounts[0] as any;
    },
    set(val: any) {
        if (!formData.value.couponTypeDiscounts) {
            formData.value.couponTypeDiscounts = [val];
        } else {
            formData.value.couponTypeDiscounts[0] = val;
        }
    },
});

// Derived label for the Discount Amount column (can't use `row` in the column's label expression)
const labelForDiscountAmount = computed(() => {
    const firstDetail = discounts0.value?.couponTypeDiscountDetail?.[0];
    return firstDetail && firstDetail.discountType === 1
        ? t('promotion.discountFixedAmount')
        : t('promotion.discountPercent');
});

// Discount Details
// function addDiscountDetail() {
//     if (!formData.value.discounts) {
//         formData.value.discounts = [{ discountDetails: [] }];
//     }
//     if (!formData.value.discounts[0].discountDetails) {
//         formData.value.discounts[0].discountDetails = [];
//     }
//     formData.value.discounts[0].discountDetails.push({ minAmount: null, limit: 0, discountType: 0, discountAmount: 0 });
// }


const defaultFormData: Partial<CouponTypeModel> = {
    name: '',
    code: '',
    status: 0,
    type: PromotionType.Discount, // Mặc định theo hình
    durationInDays: 30,
    startDate: null,
    endDate: null,
    prefix: '',
    suffix: '',
    characterCount: 5,
    applyWith: false,
    isForAllCompany: true,
    isForAllCourse: true,
    companyIds: null,
    courseIds: null,
    studentIds: null,
    minQuantity: 0,
    isForAllStudents: true,
    description: '',
    dueType: DueType.duration,
    // Chi tiết khuyến mãi (Khởi tạo Discount mặc định để hiện bảng)
    //  couponTypeDiscounts: [{ ...defaultDiscount }],
    couponTypeDiscounts: [{
        couponTypeDiscountDetail: [{ minAmount: null, limit: 0, discountType: 0, discountAmount: 0 }]
    }],
    // timeRange: [], // Dùng để binding date range picker
};
const formData = ref<Partial<CouponTypeModel>>({ ...defaultFormData });

watch(

    () => props.visible,
    (newVal) => {
        if (newVal) {

            // companyStore.fetchAllCompanies();
            //courseStore.fetchCourses();
            if (props.mode === 'edit' && props.couponTypeData) {
                formData.value = { ...props.couponTypeData };
                //khởi tạo companyIds/studentIds/courseIds từ formData nếu có


            } else {
                // Chế độ create hoặc view, reset về mặc định
                formData.value = { ...props.couponTypeData };
            }
            if (formData.value.companyIds) {
                companyIds.value = String(formData.value.companyIds).split(',');
            } else {
                companyIds.value = [];
            }
            if (formData.value.studentIds) {
                studentIds.value = String(formData.value.studentIds).split(',');
            } else {
                studentIds.value = [];
            }
            if (formData.value.courseIds) {
                courseIds.value = String(formData.value.courseIds).split(',');
            } else {
                courseIds.value = [];
            }
        }
    },
    { immediate: true },


);

const rules = {
    name: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    code: [{ required: true, message: t('validation.required'), trigger: 'blur' }],
    // ... Thêm rules cho các trường khác nếu cần
};

function handleValidityTypeChange() {
    formData.value.startDate = null;
    formData.value.endDate = null;
    formData.value.durationInDays = null;
    // formData.value.timeRange = [];
    console.log("Changed dueType to:", formData.value.dueType);

}
onMounted(() => {

    useCompanyStore().fetchAllCompanies();
    useCourseStore().fetchCourses();
    studentStore.fetchAllStudents();
    //khởi tạo companyIds từ formData nếu có
    if (formData.value.companyIds) {
        companyIds.value = formData.value.companyIds.split(',');
        // console.log("Selected companyIds:", companyIds.value);

    }
    //khởi tạo studentIds từ formData nếu có
    if (formData.value.studentIds) {
        studentIds.value = formData.value.studentIds.split(',');
    }
    //khởi tạo courseIds từ formData nếu có
    if (formData.value.courseIds) {
        courseIds.value = formData.value.courseIds.split(',');
    }
    window.addEventListener('resize', () => {
        windowWidth.value = window.innerWidth;
    });
});

// Handlers for events emitted by BaseDialogForm used in the template
function onActivated(): void {
    const form = baseDialogRef.value?.formRef
    form.validate(async (valid: boolean) => {
        if (valid) {
            loading.value = true
            const payload = { ...formData.value };

            console.log("Payload trước khi gửi:", payload);

            // Xử lý logic 'All'
            if (payload.isForAllCompany) payload.companyIds = null;
            if (payload.isForAllCourse) payload.courseIds = null;
            if (payload.isForAllStudents) payload.studentIds = null;
            payload.status = 1; // Kích hoạt khi kích hoạt
            if (payload.dueType === DueType.duration) {
                payload.startDate = new Date().toISOString().split('T')[0]; // Ngày hiện tại
                const days = Number(payload.durationInDays) || 0;
                const endDate = new Date();
                endDate.setDate(endDate.getDate() + days);
                payload.endDate = endDate.toISOString().split('T')[0];
            }
            emit('submit', payload)
            loading.value = false
        } else {
            notificationStore.showToast('error', {
                key: 'validation.formInvalid'
            })
        }
    })
}

function onOpennewdialog(...args: any[]): void {
    // no-op: keep for template binding to avoid TS error; add logic here if needed
    if (!formData.value.id) {
        notificationStore.showToast('error', { key: 'coupon.saveTypeFirst' });
        return;
    }
    showIssueModal.value = true;
}
/**
 * Khi người dùng thay đổi chọn company (trên select ở form), đồng bộ giá trị vào cả companyIds và formData.companyIds
 * @param val danh sách id (string | number) được chọn
 */

function handleSelectCompanyChange(val: (string | number)[]) {
    companyIds.value = Array.isArray(val) ? val.map(v => String(v)) : [];
    formData.value.companyIds = companyIds.value.join(',');
}

function handleSelectCourseChange(val: (string | number)[]) {
    courseIds.value = Array.isArray(val) ? val.map(v => String(v)) : [];
    formData.value.courseIds = courseIds.value.join(',');
    console.log("Selected courseIds:", formData.value.courseIds);

}

function handleSelectStudentChange(val: (string | number)[]) {
    studentIds.value = Array.isArray(val) ? val.map(v => String(v)) : [];
    formData.value.studentIds = studentIds.value.join(',');
    console.log("Selected studentIds:", formData.value.studentIds);

}

// Logic cho bảng Chi tiết Chiết khấu
// function addDiscountDetail(discountIndex: number) {
//     formData.value.couponTypeDiscounts?.[discountIndex].couponTypeDiscountDetail?.push({ ...defaultDiscountDetail } as CouponTypeDiscountDetailModel);
// }
// Discount Details
function addDiscountDetail() {

    if (!formData.value.couponTypeDiscounts) {
        formData.value.couponTypeDiscounts = [{ couponTypeDiscountDetail: [] }];
    }
    if (!formData.value.couponTypeDiscounts[0].couponTypeDiscountDetail) {
        formData.value.couponTypeDiscounts[0].couponTypeDiscountDetail = [];
    }
    formData.value.couponTypeDiscounts[0].couponTypeDiscountDetail.push({ minAmount: null, limit: 0, discountType: 0, discountAmount: 0 });
    console.log("Thêm chi tiết khuyến mãi:", formData.value.couponTypeDiscounts[0].couponTypeDiscountDetail);

}

// function removeDiscountDetail(discountIndex: number, detailIndex: number) {
//     formData.value.couponTypeDiscounts?.[discountIndex].couponTypeDiscountDetail?.splice(detailIndex, 1);
// }
function removeDiscountDetail(index: number) {
    formData.value.couponTypeDiscounts?.[0].couponTypeDiscountDetail?.splice(index, 1);
}

// Gift detail helpers for couponTypeGifts table
function addCouponTypeGiftDetail() {
    if (!formData.value.couponTypeGifts) {
        formData.value.couponTypeGifts = [{ couponTypeGiftDetail: [] } as any];
    }
    if (!formData.value.couponTypeGifts[0].couponTypeGiftDetail) {
        formData.value.couponTypeGifts[0].couponTypeGiftDetail = [];
    }
    // default gift detail structure
    formData.value.couponTypeGifts[0].couponTypeGiftDetail.push({ giftName: null, quantityGift: 1 });
    console.log("Thêm chi tiết quà tặng:", formData.value.couponTypeGifts[0].couponTypeGiftDetail);
}

function removeCouponTypeGiftDetail(index: number) {
    formData.value.couponTypeGifts?.[0].couponTypeGiftDetail?.splice(index, 1);
    console.log("Xóa chi tiết quà tặng tại index:", index);
}

function closeModal() {
    emit('update:visible', false)
    emit('close')
}

function onSubmit() {
    const form = baseDialogRef.value?.formRef
    form.validate(async (valid: boolean) => {
        if (valid) {
            loading.value = true
            const payload = { ...formData.value };

            console.log("Payload trước khi gửi:", payload);

            // Xử lý logic 'All'
            if (payload.isForAllCompany) payload.companyIds = null;
            if (payload.isForAllCourse) payload.courseIds = null;
            if (payload.isForAllStudents) payload.studentIds = null;
            emit('submit', payload)
            loading.value = false
        } else {
            notificationStore.showToast('error', {
                key: 'validation.formInvalid'
            })
        }
    })
}
function onDelete() {
    emit('delete', formData.value)
}
</script>