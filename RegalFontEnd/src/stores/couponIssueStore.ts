//tạo couponIssueStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { CouponIssueModel, CouponIssueQuery, CouponModel } from '@/api/CouponIssueApi';
const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

const service = serviceFactory.couponIssueService;
export const useCouponIssueStore = defineStore('couponIssue', {
    state: () => ({
        couponIssues: [] as CouponIssueModel[],
        allCoupons: [] as CouponModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            search: '',
        } as CouponIssueQuery,
        selectedCouponIssue: null as CouponIssueModel | null,
    }),
    actions: {
        async fetchPaged(this: any) {
            this.loading = true;
            try {
                const result = await service.fetchPaged(this.query);
                if (result.succeeded) {
                    this.couponIssues = result.data.items;
                    this.total = result.data.total;
                }
            } catch (error) {
                console.error('Error fetching paged coupon issues:', error);
            } finally {
                this.loading = false;
            }
        },
        async fetchAll(this: any) {
            this.loading = true;
            try {
                const result = await service.fetchAll();
                if (result.succeeded) {
                    this.couponIssues = result.data.items;
                    this.total = result.data.total;
                }
            } catch (error) {
                console.error('Error fetching all coupon issues:', error);
            } finally {
                this.loading = false;
            }
        },
        async fetchAllCoupons(this: any) {
            this.loading = true;
            try {
                const result = await service.fetchAllCoupons();
                if (result.succeeded) {
                    this.allCoupons = result.data;
                    this.total = result.data.total;
                }
            } catch (error) {
                console.error('Error fetching all coupon issues:', error);
            } finally {
                this.loading = false;
            }
        },
        selectCouponIssue(couponIssue: CouponIssueModel | null) {
            this.selectedCouponIssue = couponIssue;
        },
        async saveCouponIssue(model: Partial<CouponIssueModel>) {
            return await service.save(model);
        },
        async deleteCouponIssues(ids: string[]) {
            return await service.delete(ids);
        },
        async setPage(page: number) {
            this.query.page = page;
        },
        async setPageSize(pageSize: number) {
            this.query.pageSize = pageSize;
        },
    },
});