// src/services/AdmissionsQuotaRegionService.ts
import type { AdmissionsQuotaRegionApi } from '@/api/AdmissionsQuotaRegionApi';
import type { AdmissionsQuotaRegionModel, AdmissionsQuotaRegionQuery } from '@/api/AdmissionsQuotaRegionApi';
import type { Result } from '@/types/Result';

export class AdmissionsQuotaRegionService {
  private api: AdmissionsQuotaRegionApi;

  constructor(apiInstance: AdmissionsQuotaRegionApi) {
    this.api = apiInstance;
  }

  async fetchPagedRegions(query: AdmissionsQuotaRegionQuery): Promise<Result<any>> {
    return await this.api.getPagedRegions(query);
  }

  async fetchAllRegions(): Promise<Result<any>> {
    return await this.api.getAllRegions();
  }

  async saveRegion(region: Partial<AdmissionsQuotaRegionModel>): Promise<any> {
    let result: any;
    if (region.id) {
      result = await this.api.updateRegion(region);
    } else {
      result = await this.api.addRegion(region);
    }
    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }

  async deleteRegions(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteRegions(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
  }

  async restoreRegions(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreRegions(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }
  async fetchAdmissionsQuotaRegionByAdmissionsQuotaId(id: string): Promise<any> {
    return await this.api.getAdmissionsQuotaRegionByAdmissionsQuotaId(id);
  }
  async fetchAdmissionsQuotaRegionById(id: string): Promise<any> {
    return await this.api.getAdmissionsQuotaRegionById(id);
  }
}
