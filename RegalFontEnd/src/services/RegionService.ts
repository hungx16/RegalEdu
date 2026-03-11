// src/services/RegionService.ts
import type { RegionModel, RegionQuery } from '@/api/RegionApi';
import type { RegionApi } from '@/api/RegionApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class RegionService {
  private regionApi: RegionApi;

  constructor(regionApiInstance: RegionApi) {
    this.regionApi = regionApiInstance;
  }

  async fetchPagedRegions(query: RegionQuery): Promise<Result<any>> {
    return await this.regionApi.getPagedRegions(query);
  }
  async fetchAllRegions(): Promise<Result<any>> {
    return await this.regionApi.getAllRegions();
  }
  async saveRegion(region: Partial<RegionModel>): Promise<any> {
    let result: any;
    if (region.id) {
      region.companies = undefined; // Exclude companies from update
      region.manager = undefined; // Exclude manager from update
      result = await this.regionApi.updateRegion(region);
    } else {
      result = await this.regionApi.addRegion(region);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteRegions(regionIds: string[]): Promise<void> {
    let result: any = await this.regionApi.deleteRegions(regionIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreRegions(regionIds: string[]): Promise<void> {
    let result: any = await this.regionApi.restoreRegions(regionIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Restore failed');
    }
  }
  async fetchDeletedRegions(): Promise<Result<any>> {
    return await this.regionApi.getDeletedRegions();
  }
}
