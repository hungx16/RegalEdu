// src/services/AdmissionsQuotaService.ts
// src/services/PromotionService.ts
import type { Result } from '@/types/Result';
import type { PromotionModel, PromotionQuery } from '@/api/PromotionApi';
import type { PromotionApi } from '@/api/PromotionApi';


export class PromotionService {
  private api: PromotionApi;

  constructor(apiInstance: PromotionApi) {
    this.api = apiInstance;
  }

  async fetchPagedPromotions(query: PromotionQuery): Promise<Result<any>> {
    return await this.api.getPagedPromotions(query);
  }

  async fetchAllPromotions(): Promise<Result<any>> {
    return await this.api.GetAllPromotions();
  }
  async fetchGlobalPromotions(): Promise<Result<any>> {
    return await this.api.GetGlobalPromotions();
  }
  async fetchPromotionValues(): Promise<Result<any>> {
    return await this.api.GetPromotionValues();
  }
  async savePromotions(promotion: Partial<PromotionModel>): Promise<any> {
    let result: any;
    if (promotion.id) {
      result = await this.api.updatePromotion(promotion);
    } else {
      result = await this.api.addPromotion(promotion);
    }
    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }
  // async AddPromotion(quota: Partial<AdmissionsQuotaAdjustmentModel>): Promise<any> {
  //   let result: any;
  //   result = await this.api.addAdmissionsQuotaAdjustment(quota);
  //   if (!result.succeeded) throw new Error(result.error || 'Save failed');
  //   return result.data;
  // }
  // async assignSupportStaff(quota: Partial<AdmissionsQuotaEmployeeModel>): Promise<any> {
  //   let result: any;
  //   result = await this.api.assignSupportStaff(quota);
  //   if (!result.succeeded) throw new Error(result.error || 'Save failed');
  //   return result.data;
  // }
  async deletePromotion(ids: string[]): Promise<void> {
    const result: any = await this.api.deletePromotions(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
  }

  // async restoreAdmissionsQuotas(ids: string[]): Promise<void> {
  //   const result: any = await this.api.restoreAdmissionsQuotas(ids);
  //   if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  // }
}
