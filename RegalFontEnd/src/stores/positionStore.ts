// src/stores/positionStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { PositionModel, PositionQuery } from '@/api/PositionApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const usePositionStore = defineStore('position', {
  state: () => ({
    positions: [] as PositionModel[],
    total: 0,
    totalActive: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        positionCode: '',
        positionName: '',
        status: undefined,
        isDeleted: false,
      },
    } as PositionQuery,
    selectedPosition: null as PositionModel | null,
  }),
  actions: {
    async fetchPagedPositions() {
      const positionService = serviceFactory.positionService;
      this.loading = true;
      try {
        const result = await positionService.fetchPagedPositions(this.query);
        if (result?.succeeded === true) {
          this.positions = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching positions:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllPositions() {
      const positionService = serviceFactory.positionService;
      this.loading = true;
      try {
        const result = await positionService.fetchAllPositions();
        if (result?.succeeded === true) {
          this.positions = result.data;
          this.total = result.data.length;
          this.totalActive = Array.isArray(result.data)
            ? result.data.filter(item => item.status === 0).length
            : 0;
        }
      } catch (error) {
        console.error('Error fetching positions:', error);
      } finally {
        this.loading = false;
      }
    },
    selectPosition(position: PositionModel | null) {
      this.selectedPosition = position;
    },
    async savePosition(position: Partial<PositionModel>) {
      const positionService = serviceFactory.positionService;
      await positionService.savePosition(position);
    },
    async deletePositions(positionIds: string[]) {
      const positionService = serviceFactory.positionService;
      await positionService.deletePositions(positionIds);
    },
    async restorePositions(positionIds: string[]) {
      const positionService = serviceFactory.positionService;
      await positionService.restorePositions(positionIds);
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
      const positionService = serviceFactory.positionService;
      this.loading = true;
      try {
        const result = await positionService.fetchDeletedPositions();
        if (result?.succeeded === true) {
          this.positions = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching deleted positions:', error);
      } finally {
        this.loading = false;
      }
    },
  },
});
