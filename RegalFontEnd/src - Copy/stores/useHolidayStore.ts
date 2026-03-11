import { defineStore } from 'pinia';
import type { HolidayModel, HolidayQuery } from '@/api/HolidayApi';
import { serviceFactory } from '@/services/ServiceFactory';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useHolidayStore = defineStore('holiday', {
    state: () => ({
        holidays: [] as HolidayModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                name: '',
                date: undefined,
                frequency: undefined,
                regionId: undefined,
                type: undefined,
            },
        } as HolidayQuery,
        selectedHoliday: null as HolidayModel | null,
    }),
    actions: {
        async fetchAllHolidays() {
            this.loading = true;
            try {
                const res = await serviceFactory.holidayService.fetchAllHolidays();
                if (res?.succeeded) {
                    this.holidays = res.data;
                    this.total = res.data.length;
                }
            } finally {
                this.loading = false;
            }
        },

        async fetchPagedHolidays() {
            this.loading = true;
            try {
                const res = await serviceFactory.holidayService.fetchPagedHolidays(this.query);
                if (res?.succeeded) {
                    this.holidays = res.data.items;
                    this.total = res.data.total;
                }
            } finally {
                this.loading = false;
            }
        },

        async saveHoliday(model: Partial<HolidayModel>) {
            await serviceFactory.holidayService.saveHoliday(model);
        },

        async deleteHolidays(ids: string[]) {
            await serviceFactory.holidayService.deleteHolidays(ids);
        },

        async restoreHolidays(ids: string[]) {
            await serviceFactory.holidayService.restoreHolidays(ids);
        },

        selectHoliday(model: HolidayModel | null) {
            this.selectedHoliday = model;
        },

        async setPage(page: number) {
            this.query.page = page;
            await this.fetchPagedHolidays();
        },

        async setPageSize(size: number) {
            this.query.pageSize = size;
            this.query.page = 1;
            await this.fetchPagedHolidays();
        },

        async setFilter(filter: Partial<HolidayQuery["filters"]>) {
            this.query.filters = { ...this.query.filters, ...filter };
            this.query.page = 1;
            await this.fetchPagedHolidays();
        },
        async fetchDeletedHolidays() {
            this.loading = true;
            try {
                const result = await serviceFactory.holidayService.fetchDeletedHolidays();
                if (result?.succeeded === true) {
                    this.holidays = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted holidays:', error);
            } finally {
                this.loading = false;
            }
        },
    },
});
