// src/services/AffiliatePartnerService.ts
import type { AffiliatePartnerModel, AffiliatePartnerQuery, EventReportPublicationItemModel } from '@/api/AffiliatePartnerApi';
import type { AffiliatePartnerApi } from '@/api/AffiliatePartnerApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class AffiliatePartnerService {
  private affiliatePartnerApi: AffiliatePartnerApi;

  constructor(affiliatePartnerApiInstance: AffiliatePartnerApi) {
    this.affiliatePartnerApi = affiliatePartnerApiInstance;
  }

  async fetchPagedAffiliatePartners(query: AffiliatePartnerQuery): Promise<Result<any>> {
    return await this.affiliatePartnerApi.getPagedAffiliatePartners(query);
  }

  async fetchAllAffiliatePartners(): Promise<Result<any>> {
    return await this.affiliatePartnerApi.getAllAffiliatePartners();
  }

  async saveAffiliatePartner(affiliatePartner: Partial<AffiliatePartnerModel>): Promise<any> {
    let result: any;
    if (affiliatePartner.id) {
      result = await this.affiliatePartnerApi.updateAffiliatePartner(affiliatePartner);
    } else {
      result = await this.affiliatePartnerApi.addAffiliatePartner(affiliatePartner);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteAffiliatePartners(affiliatePartnerIds: string[]): Promise<void> {
    const result: any = await this.affiliatePartnerApi.deleteAffiliatePartners(affiliatePartnerIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreAffiliatePartners(affiliatePartnerIds: string[]): Promise<void> {
    const result: any = await this.affiliatePartnerApi.restoreAffiliatePartners(affiliatePartnerIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Restore failed');
    }
  }

  async fetchDeletedPositions(): Promise<Result<any>> {
    return await this.affiliatePartnerApi.getDeletedAffiliatePartners();
  }

  async fetchEventReportPublicationsByAffiliatePartnerId(
    affiliatePartnerId: string,
  ): Promise<Result<EventReportPublicationItemModel[]>> {
    return await this.affiliatePartnerApi.getEventReportPublicationsByAffiliatePartnerId(affiliatePartnerId);
  }
}
