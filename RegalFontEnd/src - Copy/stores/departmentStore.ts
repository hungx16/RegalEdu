// src/stores/departmentStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { DepartmentModel, DepartmentQuery } from '@/api/DepartmentApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useDepartmentStore = defineStore('department', {
    state: () => ({
        departments: [] as DepartmentModel[],
        total: 0,
        loading: false,
        query: {
            page: 1,
            pageSize: DEFAULT_PAGE_SIZE,
            filters: {
                departmentCode: '',
                departmentName: '',
                divisionId: '',
                status: undefined,
                isDeleted: false,
            },
        } as DepartmentQuery,
        selectedDepartment: null as DepartmentModel | null,
    }),
    actions: {
        async fetchPagedDepartments() {
            const departmentService = serviceFactory.departmentService;
            this.loading = true;
            try {
                const result = await departmentService.fetchPagedDepartments(this.query);
                if (result?.succeeded === true) {
                    this.departments = result.data.items;
                    this.total = result.data.total;
                }
            } catch (error) {
                console.error('Error fetching departments:', error);
            } finally {
                this.loading = false;
            }
        },
        async fetchAllDepartments() {
            const departmentService = serviceFactory.departmentService;
            this.loading = true;
            try {
                const result = await departmentService.fetchAllDepartments();
                if (result?.succeeded === true) {
                    this.departments = result.data;
                }
            } catch (error) {
                console.error('Error fetching departments:', error);
            } finally {
                this.loading = false;
            }
        },
        selectDepartment(department: DepartmentModel | null) {
            this.selectedDepartment = department;
        },
        async saveDepartment(department: Partial<DepartmentModel>) {
            const departmentService = serviceFactory.departmentService;
            await departmentService.saveDepartment(department);
        },
        async deleteDepartments(ids: string[]) {
            const departmentService = serviceFactory.departmentService;
            await departmentService.deleteDepartments(ids);
        },
        async restoreDepartments(ids: string[]) {
            const departmentService = serviceFactory.departmentService;
            await departmentService.restoreDepartments(ids);
        },
        async setPage(page: number) {
            this.query.page = page;
        },
        async setPageSize(pageSize: number) {
            this.query.pageSize = pageSize;
            this.query.page = 1;
        },
        async setFilter(filter) {
            this.query = { ...this.query, ...filter };
            this.query.page = 1;
        },
        async fetchDeletedDepartments() {
            this.loading = true;
            try {
                const result = await serviceFactory.departmentService.fetchDeletedDepartments();
                if (result?.succeeded === true) {
                    this.departments = result.data;
                    this.total = result.data.length;
                }
            } catch (error) {
                console.error('Error fetching deleted departments:', error);
            } finally {
                this.loading = false;
            }
        },
    },
});
