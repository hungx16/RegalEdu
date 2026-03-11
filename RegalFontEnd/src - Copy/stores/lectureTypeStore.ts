import { defineStore } from 'pinia';
import type { LectureTypeModel, LectureTypeQuery } from '@/api/LectureTypeApi';
import { serviceFactory } from '@/services/ServiceFactory';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useLectureTypeStore = defineStore('lectureType', {
    state: () => ({
        lectureTypes: [] as LectureTypeModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                lectureName: '',
                description: '',
            },
        } as LectureTypeQuery,
        selectedLectureType: null as LectureTypeModel | null,
    }),
    actions: {
        async fetchAllLectureTypes() {
            this.loading = true;
            try {
                const res = await serviceFactory.lectureTypeService.fetchAllLectureTypes();
                if (res?.succeeded) {
                    this.lectureTypes = res.data;
                    this.total = res.data.length;
                }
            } finally {
                this.loading = false;
            }
        },

        async fetchPagedLectureTypes() {
            this.loading = true;
            try {
                const res = await serviceFactory.lectureTypeService.fetchPagedLectureTypes(this.query);
                if (res?.succeeded) {
                    this.lectureTypes = res.data.items;
                    this.total = res.data.total;
                }
            } finally {
                this.loading = false;
            }
        },

        async saveLectureType(model: Partial<LectureTypeModel>) {
            await serviceFactory.lectureTypeService.saveLectureType(model);
        },

        async deleteLectureTypes(ids: string[]) {
            await serviceFactory.lectureTypeService.deleteLectureTypes(ids);
        },

        async restoreLectureTypes(ids: string[]) {
            await serviceFactory.lectureTypeService.restoreLectureTypes(ids);
        },

        selectLectureType(model: LectureTypeModel | null) {
            this.selectedLectureType = model;
        },

        async setPage(page: number) {
            this.query.page = page;
            await this.fetchPagedLectureTypes();
        },

        async setPageSize(size: number) {
            this.query.pageSize = size;
            this.query.page = 1;
            await this.fetchPagedLectureTypes();
        },

        async setFilter(filter: Partial<LectureTypeQuery["filters"]>) {
            this.query.filters = { ...this.query.filters, ...filter };
            this.query.page = 1;
            await this.fetchPagedLectureTypes();
        },

        async fetchDeletedLectureTypes() {
            this.loading = true;
            try {
                const result = await serviceFactory.lectureTypeService.fetchDeletedLectureTypes();
                if (result?.succeeded === true) {
                    this.lectureTypes = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted lecture types:', error);
            } finally {
                this.loading = false;
            }
        },
    },
});
