// src/stores/divisionStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { DivisionModel, DivisionQuery } from '@/api/DivisionApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useDivisionStore = defineStore('division', {
  state: () => ({
    divisions: [] as DivisionModel[],
    totalDepartments: 0,
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        divisionCode: '',
        divisionName: '',
        status: undefined,
        isDeleted: false,
      },
    } as DivisionQuery,
    selectedDivision: null as DivisionModel | null,
  }),
  actions: {
    async fetchPagedDivisions() {
      const divisionService = serviceFactory.divisionService;
      this.loading = true;
      try {
        const result = await divisionService.fetchPagedDivisions(this.query);
        if (result?.succeeded === true) {
          this.divisions = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching divisions:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllDivisions() {
      const divisionService = serviceFactory.divisionService;
      this.loading = true;
      try {
        const result = await divisionService.fetchAllDivisions();
        if (result?.succeeded === true) {
          this.divisions = result.data;
          this.total = result.data.length;
          this.totalDepartments = result.data.reduce((acc, division) => acc + (division.departments?.length || 0), 0);
        }
      } catch (error) {
        console.error('Error fetching divisions:', error);
      } finally {
        this.loading = false;
      }
    },
    selectDivision(division: DivisionModel | null) {
      this.selectedDivision = division;
    },
    async saveDivision(division: Partial<DivisionModel>) {
      const divisionService = serviceFactory.divisionService;
      await divisionService.saveDivision(division);
    },
    async deleteDivisions(divisionIds: string[]) {
      const divisionService = serviceFactory.divisionService;
      await divisionService.deleteDivisions(divisionIds);
    },
    async restoreDivisions(divisionIds: string[]) {
      const divisionService = serviceFactory.divisionService;
      await divisionService.restoreDivisions(divisionIds);
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
    async fetchDeletedPositions() {
      const divisionService = serviceFactory.divisionService;
      this.loading = true;
      try {
        const result = await divisionService.fetchDeletedPositions();
        if (result?.succeeded === true) {
          this.divisions = result.data;
          this.total = result.data.length;
          this.totalDepartments = result.data.reduce((acc, division) => acc + (division.departments?.length || 0), 0);
        }
      } catch (error) {
        console.error('Error fetching divisions:', error);
      } finally {
        this.loading = false;
      }
    },
  },
});
