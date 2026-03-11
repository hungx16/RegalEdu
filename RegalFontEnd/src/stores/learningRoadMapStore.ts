// src/stores/learningRoadMapStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { LearningRoadMapModel, LearningRoadMapQuery } from '@/api/LearningRoadMapApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useLearningRoadMapStore = defineStore('learningRoadMap', {
  state: () => ({
    learningRoadMaps: [] as LearningRoadMapModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {},
    } as LearningRoadMapQuery,
    selectedLearningRoadMap: null as LearningRoadMapModel | null,
  }),
  actions: {
    async fetchLearningRoadMaps() {
      const service = serviceFactory.learningRoadMapService;
      this.loading = true;
      try {
        const result = await service.fetchPagedLearningRoadMaps(this.query);
        if (result?.succeeded) {
          this.learningRoadMaps = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllLearningRoadMaps() {
      const service = serviceFactory.learningRoadMapService;
      this.loading = true;
      try {
        const result = await service.fetchAllLearningRoadMaps();
        if (result?.succeeded) this.learningRoadMaps = result.data;
      } finally {
        this.loading = false;
      }
    },
    selectLearningRoadMap(model: LearningRoadMapModel | null) {
      this.selectedLearningRoadMap = model;
    },
    async saveLearningRoadMap(model: Partial<LearningRoadMapModel>) {
      await serviceFactory.learningRoadMapService.saveLearningRoadMap(model);
      await this.fetchLearningRoadMaps();
    },
    async deleteLearningRoadMaps(ids: string[]) {
      await serviceFactory.learningRoadMapService.deleteLearningRoadMaps(ids);
      await this.fetchLearningRoadMaps();
    },
    async restoreLearningRoadMaps(ids: string[]) {
      await serviceFactory.learningRoadMapService.restoreLearningRoadMaps(ids);
      await this.fetchLearningRoadMaps();
    },
    async setPage(page: number) {
      this.query.page = page;
      await this.fetchLearningRoadMaps();
    },
    async setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
      await this.fetchLearningRoadMaps();
    },
    async setFilter(filter: Record<string, any>) {
      this.query.filters = { ...this.query.filters, ...filter };
      this.query.page = 1;
      await this.fetchLearningRoadMaps();
    },
  },
});
