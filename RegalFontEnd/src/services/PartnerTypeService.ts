// src/services/PartnerTypeService.ts
import type { PartnerTypeModel, PartnerTypeQuery } from '@/api/PartnerTypeApi';
import type { PartnerTypeApi } from '@/api/PartnerTypeApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class PartnerTypeService {
  private partnerTypeApi: PartnerTypeApi;

  constructor(partnerTypeApiInstance: PartnerTypeApi) {
    this.partnerTypeApi = partnerTypeApiInstance;
  }

  async fetchPagedPartnerTypes(query: PartnerTypeQuery): Promise<Result<any>> {
    return await this.partnerTypeApi.getPagedPartnerTypes(query);
  }

  async fetchAllPartnerTypes(): Promise<Result<any>> {
    return await this.partnerTypeApi.getAllPartnerTypes();
  }

  async savePartnerType(partnerType: Partial<PartnerTypeModel>): Promise<any> {
    let result: any;
    if (partnerType.id) {
      result = await this.partnerTypeApi.updatePartnerType(partnerType);
    } else {
      result = await this.partnerTypeApi.addPartnerType(partnerType);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deletePartnerTypes(partnerTypeIds: string[]): Promise<void> {
    const result: any = await this.partnerTypeApi.deleteListPartnerType(partnerTypeIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }
  async fetchDeletedPartnerTypes(): Promise<Result<any>> {
    return await this.partnerTypeApi.getDeletedPartnerTypes();
  }
}
