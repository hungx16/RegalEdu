import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { GiftModel, GiftQuery } from '@/api/GiftApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

const giftService = serviceFactory.giftService;

export const useGiftStore = defineStore('giftStore', {
    state: () => ({
        gifts: [] as GiftModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                giftCode: '',
                giftName: '',
                status: undefined,
                isDeleted: false,
            },
        } as GiftQuery,
        selectedGift: null as GiftModel | null,
    }),
    actions: {
        async fetchPaged(p0: { page: number; pageSize: number; }) {
            this.loading = true;
            try {
                const result = await giftService.fetchPagedGifts(this.query);
                if (result.succeeded) {
                    this.gifts = result.data.items;
                    this.total = result.data.total;
                }
            } finally {
                this.loading = false;
            }
        },
        async fetchAll() {
            this.loading = true;
            try {
                const result = await giftService.fetchAllGifts();
                if (result.succeeded) {
                    this.gifts = result.data;
                }
            } finally {
                this.loading = false;
            }
        },
        selectGift(gift: GiftModel | null) {
            this.selectedGift = gift;
        },
        async saveGift(model: Partial<GiftModel>) {
            return await giftService.saveGift(model);
        },
        async deleteGifts(ids: string[]) {
            return await giftService.deleteGifts(ids);
        },
        async setPage(page: number) {
            this.query.page = page;
            //await this.fetchPaged();
        },
        async setPageSize(pageSize: number) {
            this.query.pageSize = pageSize;
            this.query.page = 1; // Reset to first page
            // await this.fetchPaged();
        },
        async setFilter(filter) {
            this.query = { ...this.query, ...filter };
            this.query.page = 1;
        },
    }
});
