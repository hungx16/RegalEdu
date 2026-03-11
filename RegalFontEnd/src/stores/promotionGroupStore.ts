import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { PromotionGroupModel, PromotionGroupQuery } from '@/api/PromotionGroupApi';

const promotionGroupService = serviceFactory.promotionGroupService;

export const usePromotionGroupStore = defineStore('promotionGroupStore', {
    state: () => ({
        promotionGroups: [] as PromotionGroupModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: 10,
            filters: {
                groupName: '',
                status: undefined,
                isDeleted: false,
            },
        } as PromotionGroupQuery,
        selectedPromotionGroup: null as PromotionGroupModel | null,
    }),
    actions: {
        async fetchPaged() {
            this.loading = true;
            try {
                const result = await promotionGroupService.fetchPagedPromotionGroups(this.query);
                if (result.succeeded) {
                    this.promotionGroups = result.data.items;
                    this.total = result.data.total;
                }
            } finally {
                this.loading = false;
            }
        },
        async fetchAll() {
            this.loading = true;
            try {
                const result = await promotionGroupService.fetchAllPromotionGroups();
                if (result.succeeded) {
                    this.promotionGroups = result.data;
                }
            } finally {
                this.loading = false;
            }
        },
        selectPromotionGroup(promotionGroup: PromotionGroupModel | null) {
            this.selectedPromotionGroup = promotionGroup;
        },
        async savePromotionGroup(model: Partial<PromotionGroupModel>) {
            return await promotionGroupService.savePromotionGroup(model);
        },
        async deletePromotionGroups(ids: string[]) {
            return await promotionGroupService.deletePromotionGroups(ids);
        },
        async setPage(page: number) {
            this.query.page = page;
            // await this.fetchPaged();
        },
        async setPageSize(pageSize: number) {
            this.query.pageSize = pageSize;
            this.query.page = 1; // Reset to first page
            //await this.fetchPaged();
        },
        async setFilter(filter) {
            this.query = { ...this.query, ...filter };
            this.query.page = 1;
        },
    }
});
