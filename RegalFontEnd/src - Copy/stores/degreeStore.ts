import { defineStore } from 'pinia';
import type { DegreeModel, DegreeQuery } from '@/api/DegreeApi';
import { serviceFactory } from '@/services/ServiceFactory';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useDegreeStore = defineStore('degree', {
    state: () => ({
        degrees: [] as DegreeModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,

            filters: {
                degreeName: '',
                description: '',
            },
        } as DegreeQuery,
        selectedDegree: null as DegreeModel | null,
    }),
    actions: {
        async fetchAllDegrees() {
            this.loading = true;
            try {
                const res = await serviceFactory.degreeService.fetchAllDegrees();
                if (res?.succeeded) {
                    this.degrees = res.data;
                    this.total = res.data.length;
                }
            } finally {
                this.loading = false;
            }
        },

        async fetchPagedDegrees() {
            this.loading = true;
            try {
                const res = await serviceFactory.degreeService.fetchPagedDegrees(this.query);
                if (res?.succeeded) {
                    this.degrees = res.data.items;
                    this.total = res.data.total;
                }
            } finally {
                this.loading = false;
            }
        },

        async saveDegree(model: Partial<DegreeModel>) {
            await serviceFactory.degreeService.saveDegree(model);
        },

        async deleteDegrees(ids: string[]) {
            await serviceFactory.degreeService.deleteDegrees(ids);
        },

        async restoreDegrees(ids: string[]) {
            await serviceFactory.degreeService.restoreDegrees(ids);
        },

        selectDegree(model: DegreeModel | null) {
            this.selectedDegree = model;
        },

        async setPage(page: number) {
            this.query.page = page;
            await this.fetchPagedDegrees();
        },

        async setPageSize(size: number) {
            this.query.pageSize = size;
            this.query.page = 1;
            await this.fetchPagedDegrees();
        },

        async setFilter(filter: Partial<DegreeQuery["filters"]>) {
            this.query.filters = { ...this.query.filters, ...filter };
            this.query.page = 1;
            await this.fetchPagedDegrees();
        },
        async fetchDeletedDegrees() {
            this.loading = true;
            try {
                const result = await serviceFactory.degreeService.fetchDeletedDegrees();
                if (result?.succeeded === true) {
                    this.degrees = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted degrees:', error);
            } finally {
                this.loading = false;
            }
        },
    },
});
