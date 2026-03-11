// src/stores/couponTypeStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { CouponTypeModel, CouponTypeQuery } from '@/api/CouponTypeApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

const service = serviceFactory.couponTypeService;
export const useCouponTypeStore = defineStore('couponType', {
    state: () => ({
        couponTypes: [] as CouponTypeModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                name: '',
                code: '',
                status: undefined,
                type: undefined,
            },
        } as CouponTypeQuery,
        selectedCouponType: null as CouponTypeModel | null,
    }),
    actions: {
        async fetchPaged() {
            this.loading = true;
            try {
                const result = await service.fetchPaged(this.query);
                if (result.succeeded) {
                    this.couponTypes = result.data.items;
                    this.total = result.data.total;
                }
            } finally {
                this.loading = false;
            }
        },
        async fetchAll() {
            this.loading = true;
            try {
                const result = await service.fetchAll();
                if (result.succeeded) {
                    this.couponTypes = result.data;
                    this.total = result.data.total;
                }
            } finally {
                this.loading = false;
            }
        },
        selectCouponType(couponType: CouponTypeModel | null) {
            this.selectedCouponType = couponType;
        },
        async saveCouponType(model: Partial<CouponTypeModel>) {
            return await serviceFactory.couponTypeService.save(model);
        },
        async deleteCouponTypes(ids: string[]) {
            return await serviceFactory.couponTypeService.delete(ids);
        },
        async setPage(page: number) {
            this.query.page = page;
            await this.fetchPaged();
        },
        async setPageSize(pageSize: number) {
            this.query.pageSize = pageSize;
            this.query.page = 1; // Reset to first page
            await this.fetchPaged();
        }
    },
});