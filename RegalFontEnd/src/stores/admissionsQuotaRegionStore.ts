// src/stores/admissionsQuotaRegionStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { AdmissionsQuotaRegionModel, AdmissionsQuotaRegionQuery } from '@/api/AdmissionsQuotaRegionApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useAdmissionsQuotaRegionStore = defineStore('admissionsQuotaRegion', {
  state: () => ({
    regions: [] as AdmissionsQuotaRegionModel[],
    total: 0,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        regionName: '',
        status: undefined,
        isDeleted: false,
      },
    } as AdmissionsQuotaRegionQuery,
    selectedRegion: null as AdmissionsQuotaRegionModel | null,
  }),
  actions: {
    async fetchRegions() {
      const service = serviceFactory.admissionsQuotaRegionService;
      try {
        const result = await service.fetchPagedRegions(this.query);
        if (result?.succeeded) {
          this.regions = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching regions:', error);
      }
    },
    async fetchAllRegions() {
      const service = serviceFactory.admissionsQuotaRegionService;
      try {
        const result = await service.fetchAllRegions();
        if (result?.succeeded) {
          this.regions = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching all regions:', error);
      }
    },
    selectRegion(region: AdmissionsQuotaRegionModel | null) {
      this.selectedRegion = region;
    },
    async saveRegion(region: Partial<AdmissionsQuotaRegionModel>) {
      await serviceFactory.admissionsQuotaRegionService.saveRegion(region);
      await this.fetchRegions();
    },
    async deleteRegions(ids: string[]) {
      await serviceFactory.admissionsQuotaRegionService.deleteRegions(ids);
      await this.fetchRegions();
    },
    async restoreRegions(ids: string[]) {
      await serviceFactory.admissionsQuotaRegionService.restoreRegions(ids);
      await this.fetchRegions();
    },
    async setPage(page: number) {
      this.query.page = page;
      await this.fetchRegions();
    },
    async setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
      await this.fetchRegions();
    },
    async setFilter(filter) {
      this.query = { ...this.query, ...filter };
      this.query.page = 1;
      await this.fetchRegions();
    },
    async FetchAdmissionsQuotaRegionByAdmissionsQuotaId(id: string) {
      const service = serviceFactory.admissionsQuotaRegionService;
      try {
        const result = await service.fetchAdmissionsQuotaRegionByAdmissionsQuotaId(id);
        if (result?.succeeded) {
          this.regions = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching all regions:', error);
      }
    },
    async fetchAdmissionsQuotaRegionById(id: string) {
      const service = serviceFactory.admissionsQuotaRegionService;
      try {
        const result = await service.fetchAdmissionsQuotaRegionById(id);
        if (result?.succeeded) {
          this.selectedRegion = result.data;
        }
      } catch (error) {
        console.error('Error fetching all regions:', error);
      }
    },
  },
});
