import { defineStore } from 'pinia';
import type { ClassTypeModel, ClassTypeQuery } from '@/api/ClassTypeApi';
import { serviceFactory } from '@/services/ServiceFactory';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useClassTypeStore = defineStore('classType', {
    state: () => ({
        classTypes: [] as ClassTypeModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                classTypeCode: '',
                classTypeName: '',
            },
        } as ClassTypeQuery,
        selectedClassType: null as ClassTypeModel | null,
    }),
    actions: {
        async fetchAllClassTypes() {
            this.loading = true;
            try {
                const res = await serviceFactory.classTypeService.fetchAllClassTypes();
                if (res?.succeeded) {
                    this.classTypes = res.data;
                    this.total = res.data.length;
                }
            } finally {
                this.loading = false;
            }
        },
        async getClassTypeById(id: string) {
            const service = serviceFactory.classTypeService;
            this.loading = true;
            try {
                const classType = await service.getClassTypeById(id);
                return classType;
            } finally {
                this.loading = false;
            }
        },

        async fetchPagedClassTypes() {
            this.loading = true;
            try {
                const res = await serviceFactory.classTypeService.fetchPagedClassTypes(this.query);
                if (res?.succeeded) {
                    this.classTypes = res.data.items;
                    this.total = res.data.total;
                }
            } finally {
                this.loading = false;
            }
        },

        async saveClassType(model: Partial<ClassTypeModel>) {
            await serviceFactory.classTypeService.saveClassType(model);
        },

        async deleteClassTypes(ids: string[]) {
            await serviceFactory.classTypeService.deleteClassTypes(ids);
        },

        async restoreClassTypes(ids: string[]) {
            await serviceFactory.classTypeService.restoreClassTypes(ids);
        },

        selectClassType(model: ClassTypeModel | null) {
            this.selectedClassType = model;
        },

        async setPage(page: number) {
            this.query.page = page;
            await this.fetchPagedClassTypes();
        },

        async setPageSize(size: number) {
            this.query.pageSize = size;
            this.query.page = 1;
            await this.fetchPagedClassTypes();
        },

        async setFilter(filter: Partial<ClassTypeQuery["filters"]>) {
            this.query.filters = { ...this.query.filters, ...filter };
            this.query.page = 1;
            await this.fetchPagedClassTypes();
        },
        async fetchDeletedClassTypes() {
            this.loading = true;
            try {
                const result = await serviceFactory.classTypeService.fetchDeletedClassTypes();
                if (result?.succeeded === true) {
                    this.classTypes = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted class types:', error);
            } finally {
                this.loading = false;
            }
        },
    },
});
