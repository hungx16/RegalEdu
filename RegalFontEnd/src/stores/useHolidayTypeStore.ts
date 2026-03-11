import { defineStore } from 'pinia';
import type { HolidayTypeModel, HolidayTypeQuery } from '@/api/HolidayTypeApi';
import { serviceFactory } from '@/services/ServiceFactory';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useHolidayTypeStore = defineStore('holidayType', {
    state: () => ({
        holidayTypes: [] as HolidayTypeModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                categoryCode: '',
                categoryName: '',
            },
        } as HolidayTypeQuery,
        selectedHolidayType: null as HolidayTypeModel | null,
    }),
    actions: {
        async fetchAllHolidayTypes() {
            this.loading = true;
            try {
                const res = await serviceFactory.holidayTypeService.fetchAllHolidayTypes();
                if (res?.succeeded) {
                    this.holidayTypes = res.data;
                    this.total = res.data.length;
                }
            } finally {
                this.loading = false;
            }
        },

        async fetchPagedHolidayTypes() {
            this.loading = true;
            try {
                const res = await serviceFactory.holidayTypeService.fetchPagedHolidayTypes(this.query);
                if (res?.succeeded) {
                    this.holidayTypes = res.data.items;
                    this.total = res.data.total;
                }
            } finally {
                this.loading = false;
            }
        },

        async saveHolidayType(model: Partial<HolidayTypeModel>) {
            await serviceFactory.holidayTypeService.saveHolidayType(model);
        },

        async deleteHolidayTypes(ids: string[]) {
            await serviceFactory.holidayTypeService.deleteHolidayTypes(ids);
        },

        async restoreHolidayTypes(ids: string[]) {
            await serviceFactory.holidayTypeService.restoreHolidayTypes(ids);
        },

        selectHolidayType(model: HolidayTypeModel | null) {
            this.selectedHolidayType = model;
        },

        async setPage(page: number) {
            this.query.page = page;
            await this.fetchPagedHolidayTypes();
        },

        async setPageSize(size: number) {
            this.query.pageSize = size;
            this.query.page = 1;
            await this.fetchPagedHolidayTypes();
        },

        async setFilter(filter: Partial<HolidayTypeQuery["filters"]>) {
            this.query.filters = { ...this.query.filters, ...filter };
            this.query.page = 1;
            await this.fetchPagedHolidayTypes();
        },

        async fetchDeletedHolidayTypes() {
            this.loading = true;
            try {
                const result = await serviceFactory.holidayTypeService.fetchDeletedHolidayTypes();
                if (result?.succeeded === true) {
                    this.holidayTypes = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted holiday types:', error);
            } finally {
                this.loading = false;
            }
        },
    },
});
