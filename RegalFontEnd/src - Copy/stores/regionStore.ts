// src/stores/regionStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { RegionModel, RegionQuery } from '@/api/RegionApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useRegionStore = defineStore('region', {
  state: () => ({
    regions: [] as RegionModel[],
    total: 0,
    totalActiveRegions: 0,
    totalCompanies: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        regionCode: '',
        regionName: '',
        isDeleted: false,
      },
    } as RegionQuery,
    selectedRegion: null as RegionModel | null,
  }),
  actions: {
    async fetchPagedRegions() {
      const regionService = serviceFactory.regionService;
      this.loading = true;
      try {
        const result = await regionService.fetchPagedRegions(this.query);
        if (result?.succeeded === true) {
          this.regions = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching regions:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllRegions() {
      const regionService = serviceFactory.regionService;
      this.loading = true;
      try {
        const result = await regionService.fetchAllRegions();
        if (result?.succeeded === true) {
          this.regions = result.data;
          this.total = result.data.length;
          this.totalActiveRegions = result.data.filter(region => region.status === 0).length;
          this.totalCompanies = result.data.reduce((acc, region) => acc + (region.companies?.length || 0), 0);
        }
      } catch (error) {
        console.error('Error fetching regions:', error);
      } finally {
        this.loading = false;
      }
    },
    selectRegion(region: RegionModel | null) {
      this.selectedRegion = region;
    },
    async saveRegion(region: Partial<RegionModel>) {
      const regionService = serviceFactory.regionService;
      await regionService.saveRegion(region);
    },
    async deleteRegions(regionIds: string[]) {
      const regionService = serviceFactory.regionService;
      await regionService.deleteRegions(regionIds);
    },
    async restoreRegions(regionIds: string[]) {
      const regionService = serviceFactory.regionService;
      await regionService.restoreRegions(regionIds);
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
    async fetchDeletedRegions() {
      const regionService = serviceFactory.regionService;
      this.loading = true;
      try {
        const result = await regionService.fetchDeletedRegions();
        if (result?.succeeded === true) {
          this.regions = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching deleted regions:', error);
      } finally {
        this.loading = false;
      }
    },
  },
});
