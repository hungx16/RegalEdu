// src/stores/supportingDocumentStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { DeleteSupportingDocumentRequest, SupportingDocumentModel, SupportingDocumentQuery } from '@/api/SupportingDocumentApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useSupportingDocumentStore = defineStore('supportingDocument', {
    state: () => ({
        supportingDocuments: [] as SupportingDocumentModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                documentName: '',
                documentTypeId: undefined,
                status: undefined,
                isDeleted: false,
            },
        } as SupportingDocumentQuery,
        selectedSupportingDocument: null as SupportingDocumentModel | null,
    }),
    actions: {
        async fetchSupportingDocuments() {
            const supportingDocumentService = serviceFactory.supportingDocumentService;
            this.loading = true;
            try {
                const result = await supportingDocumentService.fetchPagedSupportingDocuments(this.query);
                if (result?.succeeded === true) {
                    this.supportingDocuments = result.data.items;
                    this.total = result.data.total;
                }
            } catch (error) {
                console.error('Error fetching supporting documents:', error);
            } finally {
                this.loading = false;
            }
        },
        async fetchAllSupportingDocuments() {
            const supportingDocumentService = serviceFactory.supportingDocumentService;
            this.loading = true;
            try {
                const result = await supportingDocumentService.fetchAllSupportingDocuments();
                if (result?.succeeded === true) {
                    this.supportingDocuments = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching all supporting documents:', error);
            } finally {
                this.loading = false;
            }
        },
        selectSupportingDocument(supportingDocument: SupportingDocumentModel | null) {
            this.selectedSupportingDocument = supportingDocument;
        },
        async saveSupportingDocument(supportingDocument: Partial<SupportingDocumentModel>) {
            const supportingDocumentService = serviceFactory.supportingDocumentService;
            await supportingDocumentService.saveSupportingDocument(supportingDocument);
            await this.fetchSupportingDocuments();
        },
        async deleteSupportingDocuments(request: DeleteSupportingDocumentRequest) {
            const supportingDocumentService = serviceFactory.supportingDocumentService;
            await supportingDocumentService.deleteSupportingDocuments(request);
            await this.fetchSupportingDocuments();
        },
        async restoreSupportingDocuments(request: DeleteSupportingDocumentRequest) {
            const supportingDocumentService = serviceFactory.supportingDocumentService;
            await supportingDocumentService.restoreSupportingDocuments(request);
            await this.fetchSupportingDocuments();
        },
        async setPage(page: number) {
            this.query.page = page;
            await this.fetchSupportingDocuments();
        },
        async setPageSize(pageSize: number) {
            this.query.pageSize = pageSize;
            this.query.page = 1;
            await this.fetchSupportingDocuments();
        },
        async setFilter(filter) {
            this.query = { ...this.query, ...filter };
            this.query.page = 1;
            await this.fetchSupportingDocuments();
        },
        async fetchDeletedSupportingDocuments() {
            const supportingDocumentService = serviceFactory.supportingDocumentService;
            this.loading = true;
            try {
                const result = await supportingDocumentService.fetchDeletedSupportingDocuments();
                if (result?.succeeded === true) {
                    this.supportingDocuments = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted supporting documents:', error);
            } finally {
                this.loading = false;
            }
        },
    },
});
