// src/stores/TuitionStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { TuitionModel, TuitionQuery } from '@/api/TuitionApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useTuitionStore = defineStore('Tuition', {
  state: () => ({
    Tuitions: [] as TuitionModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        priceName: '',
        status: undefined,
        isDeleted: false,
      },
    } as TuitionQuery,
    selectedTuition: null as TuitionModel | null,
  }),
  actions: {
    async fetchTuitions() {
      const service = serviceFactory.TuitionService;
      this.loading = true;
      try {
        const result = await service.fetchPagedTuitions(this.query);
        if (result?.succeeded) {
          this.Tuitions = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching price lists:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllTuitions() {
      const service = serviceFactory.TuitionService;
      this.loading = true;
      try {
        const result = await service.fetchAllTuitions();
        if (result?.succeeded) {
          this.Tuitions = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching all price lists:', error);
      } finally {
        this.loading = false;
      }
    },
    selectTuition(Tuition: TuitionModel | null) {
      this.selectedTuition = Tuition;
    },
    async saveTuition(Tuition: Partial<TuitionModel>) {
      await serviceFactory.TuitionService.saveTuition(Tuition);
    },
    async deleteTuitions(ids: string[]) {
      await serviceFactory.TuitionService.deleteTuitions(ids);
    },
    async restoreTuitions(ids: string[]) {
      await serviceFactory.TuitionService.restoreTuitions(ids);
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
  },
});
