// src/stores/receiptStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { ReceiptsModel, ReceiptQuery } from '@/api/ReceiptApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useReceiptStore = defineStore('receipt', {
    state: () => ({
        receipts: [] as ReceiptsModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                searchTerm: '',
                type: undefined,
                status: undefined,
                paymentMethodType: undefined,
            },
        } as ReceiptQuery,
        selectedReceipt: null as ReceiptsModel | null,
    }),
    actions: {
        async fetchPagedReceipts() {
            const service = serviceFactory.receiptService;
            this.loading = true;
            try {
                const result = await service.fetchPagedReceipts(this.query);
                if (result?.succeeded && result.data) {
                    this.receipts = result.data.items;
                    this.total = result.data.total;
                }
            } catch (error) {
                console.error('Error fetching receipts:', error);
            } finally {
                this.loading = false;
            }
        },
        //tạo fetchAllReceipts
        async fetchAllReceipts() {
            const service = serviceFactory.receiptService;
            this.loading = true;
            try {
                const result = await service.fetchAllReceipts();
                if (result?.succeeded && result.data) {
                    this.receipts = result.data;
                }
            } catch (error) {
                console.error('Error fetching all receipts:', error);
            } finally {
                this.loading = false;
            }
        },
        async fetchReceiptById(id: string) {
            return serviceFactory.receiptService.getReceiptById(id);
        },
        selectReceipt(receipt: ReceiptsModel | null) {
            this.selectedReceipt = receipt;
        },
        async saveReceipt(receipt: Partial<ReceiptsModel>) {
            await serviceFactory.receiptService.saveReceipt(receipt);
        },
        async deleteReceipts(ids: string[]) {
            await serviceFactory.receiptService.deleteReceipts(ids);
        },
        setPage(page: number) { this.query.page = page; },
        setPageSize(pageSize: number) { this.query.pageSize = pageSize; this.query.page = 1; },
        setQueryFilters(filters: Partial<ReceiptQuery['filters']>) {
            this.query.filters = { ...this.query.filters, ...filters };
            this.query.page = 1;
        },
    },
});