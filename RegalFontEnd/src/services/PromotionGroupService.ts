import type { PromotionGroupModel, PromotionGroupQuery } from '@/api/PromotionGroupApi';
import type { PromotionGroupApi } from '@/api/PromotionGroupApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class PromotionGroupService {
  private promotionGroupApi: PromotionGroupApi;

  constructor(apiInstance: PromotionGroupApi) {
    this.promotionGroupApi = apiInstance;
  }

  async fetchPagedPromotionGroups(query: PromotionGroupQuery): Promise<Result<any>> {
    return await this.promotionGroupApi.getPagedPromotionGroups(query);
  }

  async fetchAllPromotionGroups(): Promise<Result<any>> {
    return await this.promotionGroupApi.getAllPromotionGroups();
  }

  async savePromotionGroup(model: Partial<PromotionGroupModel>): Promise<any> {
    let result: any;
    if (model.id) {
      result = await this.promotionGroupApi.updatePromotionGroup(model);
    } else {
      result = await this.promotionGroupApi.addPromotionGroup(model);
    }
    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }

  async deletePromotionGroups(ids: string[]): Promise<void> {
    let result: any = await this.promotionGroupApi.deletePromotionGroups(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
    else useNotificationStore().showToast('success', { key: result.data });
  }
}
