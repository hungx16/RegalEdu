// =============================
// workingTimeStore.ts
// =============================
import { defineStore } from 'pinia';
import type { WorkingTimeModel, WorkingTimeQuery } from '@/api/WorkingTimeApi';
import { serviceFactory } from '@/services/ServiceFactory';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useWorkingTimeStore = defineStore('workingTime', {
    state: () => ({
        workingTimes: [] as WorkingTimeModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                name: '',
                dayOfWeek: undefined,
                isWorkingDay: undefined,
            },
        } as WorkingTimeQuery,
        selectedWorkingTime: null as WorkingTimeModel | null,
    }),
    actions: {
        async fetchAllWorkingTimes() {
            this.loading = true;
            try {
                const res = await serviceFactory.workingTimeService.fetchAllWorkingTimes();
                if (res?.succeeded) {
                    this.workingTimes = res.data;
                    this.total = res.data.length;
                }
            } finally {
                this.loading = false;
            }
        },

        async fetchPagedWorkingTimes() {
            this.loading = true;
            try {
                const res = await serviceFactory.workingTimeService.fetchPagedWorkingTimes(this.query);
                if (res?.succeeded) {
                    this.workingTimes = res.data.items;
                    this.total = res.data.total;
                }
            } finally {
                this.loading = false;
            }
        },
        async fetchAllWorkingTimesByConfigId(configurationId: string) {
            this.loading = true;
            try {
                const res = await serviceFactory.workingTimeService.fetchAllWorkingTimesByConfigId(configurationId);
                if (res?.succeeded) {
                    this.workingTimes = res.data;
                    this.total = res.data.length;
                }
            } finally {
                this.loading = false;
            }
        },
        async saveWorkingTime(model: Partial<WorkingTimeModel>) {
            await serviceFactory.workingTimeService.saveWorkingTime(model);
        },

        async deleteWorkingTimes(ids: string[]) {
            await serviceFactory.workingTimeService.deleteWorkingTimes(ids);
        },

        async restoreWorkingTimes(ids: string[]) {
            await serviceFactory.workingTimeService.restoreWorkingTimes(ids);
        },

        selectWorkingTime(model: WorkingTimeModel | null) {
            this.selectedWorkingTime = model;
        },

        async fetchDeletedWorkingTimes() {
            this.loading = true;
            try {
                const result = await serviceFactory.workingTimeService.fetchDeletedWorkingTimes();
                if (result?.succeeded === true) {
                    this.workingTimes = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted working times:', error);
            } finally {
                this.loading = false;
            }
        },
    },
});
