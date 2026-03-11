// src/stores/registerStudyStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { RegisterStudyModel, RegisterStudyListModel, RegisterStudyQuery } from '@/api/RegisterStudyApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

const getDefaultForm = (): Partial<RegisterStudyModel> => ({
    type: 1, // Đăng ký mới
    detailRegisterStudys: [],
    registerPromotion: [],
    firstPaymentAmount: 0,
    paymentType: 0,
    paymentMethodType: 0,
    paymentMethod: 0,
    totalAmount: 0,
    totalDiscount: 0,
    totalAfterDiscount: 0,
});

export const useRegisterStudyStore = defineStore('registerStudy', {
    state: () => ({
        // Quản lý Wizard
        currentStep: 1 as 1 | 2 | 3 | 4,
        formData: getDefaultForm(),
        // Quản lý Promotion
        registerPromotion: [] as RegisterStudyListModel[],
        // Quản lý List
        registerStudies: [] as RegisterStudyModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            searchTerm: '',
            registrationStatus: undefined,
            paymentStatus: undefined,
        } as RegisterStudyQuery,
        // allow either the list item shape or the detailed model for selectedRegisterStudy
        selectedRegisterStudy: null as RegisterStudyModel | null,
    }),
    getters: {
        totalAmount(state) {
            // Tính Tổng tiền học phí (TuitionFee)
            return state.formData.detailRegisterStudys?.reduce((sum, detail) => sum + (detail.tuitionFee || 0), 0) || 0;
        },
        totalDiscount(state) {
            // Tính tổng giảm giá từ Khóa học (DiscountAmount)
            const courseDiscount = state.formData.registerPromotion?.reduce((sum, detail) => sum + (detail.discountAmount || 0), 0) || 0;
            // Giả định thêm logic tính giảm giá từ promotionSelections/couponCode nếu cần
            return courseDiscount;
        },
        totalAfterDiscount(): number {
            return Number(this.totalAmount ?? 0) - Number(this.totalDiscount ?? 0);
        }
    },
    actions: {
        // --- Actions cho Wizard ---
        nextStep() { if (this.currentStep < 3) this.currentStep++; },
        prevStep() { if (this.currentStep > 1) this.currentStep--; },
        // Hành động mới: Chuyển đến một bước cụ thể
        goToStep(step: 1 | 2 | 3 | 4) {
            this.currentStep = step;
        },
        resetForm() { this.formData = getDefaultForm(); this.currentStep = 1; },
        updateFormData(data: Partial<RegisterStudyModel>) {
            console.log("Updating form data with từ ....:", data);

            this.formData = { ...this.formData, ...data };

            console.log("Updated form data là ....:", this.formData);
            // Cập nhật các trường tính toán
            this.formData.totalAmount = this.totalAmount;
            this.formData.totalDiscount = this.totalDiscount;
            this.formData.totalAfterDiscount = this.totalAfterDiscount;
        },

        // --- Actions cho List ---
        async fetchPagedRegisterStudies() {
            const service = serviceFactory.registerStudyService;
            this.loading = true;
            try {
                const result = await service.fetchPagedRegisterStudy(this.query);
                if (result?.succeeded) {
                    this.registerStudies = result.data.items;
                    this.total = result.data.total;
                }
            } catch (error) {
                console.error('Error fetching register studies:', error);
            } finally {
                this.loading = false;
            }
        },
        async fetchAllRegisterStudies() {
            const service = serviceFactory.registerStudyService;
            this.loading = true;
            try {
                const result = await service.fetchAllRegisterStudys();
                if (result?.succeeded) {
                    this.registerStudies = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching all register studies:', error);
            } finally {
                this.loading = false;
            }
        },
        async fetchRegisterStudyDetails(id: string) {
            const service = serviceFactory.registerStudyService;
            this.loading = true;
            try {
                const result = await service.getRegisterStudyById(id);
                if (result?.succeeded && result.data) {
                    // Lưu dữ liệu chi tiết vào selectedRegisterStudy (giả định dùng lại field này)
                    this.selectedRegisterStudy = result.data;
                }
            } catch (error) {
                console.error('Error fetching register study details:', error);
            } finally {
                this.loading = false;
            }
        },
        selectRegisterStudy(registerStudy: RegisterStudyModel | null) {
            this.selectedRegisterStudy = registerStudy;
        },
        async setPage(page: number) {
            this.query.page = page;
            await this.fetchPagedRegisterStudies();
        },
        async setPageSize(pageSize: number) {
            this.query.pageSize = pageSize;
            this.query.page = 1; // Reset to first page
            await this.fetchPagedRegisterStudies();
        },
        // --- Actions CRUD ---
        async saveRegisterStudy(registerStudy: Partial<RegisterStudyModel>) {
            await serviceFactory.registerStudyService.saveRegisterStudys(registerStudy);
        },
        async deleteRegisterStudies(ids: string[]) {
            await serviceFactory.registerStudyService.deleteRegisterStudys(ids);
        },

        // --- Filters ---
        setQuery(query: Partial<RegisterStudyQuery>) {
            this.query = { ...this.query, ...query };
            this.query.page = 1;
        }
    },
});