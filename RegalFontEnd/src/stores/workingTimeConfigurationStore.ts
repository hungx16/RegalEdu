import { defineStore } from 'pinia';
import type { WorkingTimeConfigurationModel, WorkingTimeConfigurationQuery } from '@/api/WorkingTimeConfigurationApi';
import { serviceFactory } from '@/services/ServiceFactory';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useWorkingTimeConfigurationStore = defineStore('workingTimeConfiguration', {
    state: () => ({
        configurations: [] as WorkingTimeConfigurationModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                nameConfiguration: '',
                applyToSystem: undefined,
            },
        } as WorkingTimeConfigurationQuery,
        selectedConfiguration: null as WorkingTimeConfigurationModel | null,
    }),
    actions: {
        async fetchAllWorkingTimeConfigurations() {
            this.loading = true;
            try {
                const res = await serviceFactory.workingTimeConfigurationService.fetchAllWorkingTimeConfigurations();
                if (res?.succeeded) {
                    this.configurations = res.data;
                    this.total = res.data.length;
                }
            } finally {
                this.loading = false;
            }
        },

        async fetchPagedWorkingTimeConfigurations() {
            this.loading = true;
            try {
                const res = await serviceFactory.workingTimeConfigurationService.fetchPagedWorkingTimeConfigurations(this.query);
                if (res?.succeeded) {
                    this.configurations = res.data.items;
                    this.total = res.data.total;
                }
            } finally {
                this.loading = false;
            }
        },

        async saveWorkingTimeConfiguration(model: Partial<WorkingTimeConfigurationModel>) {
            await serviceFactory.workingTimeConfigurationService.saveWorkingTimeConfiguration(model);
        },

        async deleteWorkingTimeConfigurations(ids: string[]) {
            await serviceFactory.workingTimeConfigurationService.deleteWorkingTimeConfigurations(ids);
        },

        async restoreWorkingTimeConfigurations(ids: string[]) {
            await serviceFactory.workingTimeConfigurationService.restoreWorkingTimeConfigurations(ids);
        },

        selectConfiguration(model: WorkingTimeConfigurationModel | null) {
            this.selectedConfiguration = model;
        },

        async setPage(page: number) {
            this.query.page = page;
            await this.fetchPagedWorkingTimeConfigurations();
        },

        async setPageSize(size: number) {
            this.query.pageSize = size;
            this.query.page = 1;
            await this.fetchPagedWorkingTimeConfigurations();
        },

        async setFilter(filter: Partial<WorkingTimeConfigurationQuery["filters"]>) {
            this.query.filters = { ...this.query.filters, ...filter };
            this.query.page = 1;
            await this.fetchPagedWorkingTimeConfigurations();
        },

        async fetchDeletedWorkingTimeConfigurations() {
            this.loading = true;
            try {
                const result = await serviceFactory.workingTimeConfigurationService.fetchDeletedWorkingTimeConfigurations();
                if (result?.succeeded === true) {
                    this.configurations = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted working time configurations:', error);
            } finally {
                this.loading = false;
            }
        },
    },
});
