import { defineStore } from 'pinia';
import type { AgeGroupModel, AgeGroupQuery } from '@/api/AgeGroupApi';
import { serviceFactory } from '@/services/ServiceFactory';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useAgeGroupStore = defineStore('ageGroup', {
    state: () => ({
        ageGroups: [] as AgeGroupModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                categoryCode: '',
                categoryName: '',
            },
        } as AgeGroupQuery,
        selectedAgeGroup: null as AgeGroupModel | null,
    }),
    actions: {
        async fetchAllAgeGroups() {
            this.loading = true;
            try {
                const res = await serviceFactory.ageGroupService.fetchAllAgeGroups();
                if (res?.succeeded) {
                    this.ageGroups = res.data;
                    this.total = res.data.length;
                }
            } finally {
                this.loading = false;
            }
        },

        async fetchPagedAgeGroups() {
            this.loading = true;
            try {
                const res = await serviceFactory.ageGroupService.fetchPagedAgeGroups(this.query);
                if (res?.succeeded) {
                    this.ageGroups = res.data.items;
                    this.total = res.data.total;
                }
            } finally {
                this.loading = false;
            }
        },

        async saveAgeGroup(model: Partial<AgeGroupModel>) {
            await serviceFactory.ageGroupService.saveAgeGroup(model);
        },

        async deleteAgeGroups(ids: string[]) {
            await serviceFactory.ageGroupService.deleteAgeGroups(ids);
        },

        async restoreAgeGroups(ids: string[]) {
            await serviceFactory.ageGroupService.restoreAgeGroups(ids);
        },

        selectAgeGroup(model: AgeGroupModel | null) {
            this.selectedAgeGroup = model;
        },

        async setPage(page: number) {
            this.query.page = page;
            await this.fetchPagedAgeGroups();
        },

        async setPageSize(size: number) {
            this.query.pageSize = size;
            this.query.page = 1;
            await this.fetchPagedAgeGroups();
        },

        async setFilter(filter: Partial<AgeGroupQuery["filters"]>) {
            this.query.filters = { ...this.query.filters, ...filter };
            this.query.page = 1;
            await this.fetchPagedAgeGroups();
        },
        async fetchDeletedAgeGroups() {
            this.loading = true;
            try {
                const result = await serviceFactory.ageGroupService.fetchDeletedAgeGroups();
                if (result?.succeeded === true) {
                    this.ageGroups = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted age groups:', error);
            } finally {
                this.loading = false;
            }
        },
    },
});
