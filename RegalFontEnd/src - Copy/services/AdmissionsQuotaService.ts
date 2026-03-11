// src/services/AdmissionsQuotaService.ts
import type { AdmissionsQuotaEmployeeModel } from '@/api/AdmissionsQuotaCompanyApi';
import type { AdmissionsQuotaApi } from '../api/AdmissionsQuotaApi';
import type { AdmissionsQuotaAdjustmentModel, AdmissionsQuotaModel, AdmissionsQuotaQuery, TransferRolesRequest } from '../api/AdmissionsQuotaApi';
import type { Result } from '@/types/Result';
import { useNotificationStore } from '@/stores/notificationStore';

export class AdmissionsQuotaService {
  private api: AdmissionsQuotaApi;

  constructor(apiInstance: AdmissionsQuotaApi) {
    this.api = apiInstance;
  }

  async fetchPagedAdmissionsQuotas(query: AdmissionsQuotaQuery): Promise<Result<any>> {
    return await this.api.getPagedAdmissionsQuotas(query);
  }

  async fetchAllAdmissionsQuotas(): Promise<Result<any>> {
    return await this.api.getAllAdmissionsQuotas();
  }

  async saveAdmissionsQuota(quota: Partial<AdmissionsQuotaModel>): Promise<any> {
    let result: any;
    if (quota.id) {
      result = await this.api.updateAdmissionsQuota(quota);
    } else {
      result = await this.api.addAdmissionsQuota(quota);
    }
    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }
  async getMyAdmissionsQuota() {
    return await this.api.getMyAdmissionsQuota();
  }
  async addAdmissionsQuotaAdjustment(quota: Partial<AdmissionsQuotaAdjustmentModel>): Promise<any> {
    let result: any;
    result = await this.api.addAdmissionsQuotaAdjustment(quota);
    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }
  async assignSupportStaff(quota: Partial<AdmissionsQuotaEmployeeModel>): Promise<any> {
    let result: any;
    result = await this.api.assignSupportStaff(quota);
    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }
  async transferRoles(data: TransferRolesRequest): Promise<any> {
    const result: any = await this.api.transferRoles(data);
    if (!result.succeeded) throw new Error(result.error || 'Transfer failed');
    return result.data;
  }
  async deleteAdmissionsQuotas(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteAdmissionsQuotas(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
    else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreAdmissionsQuotas(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreAdmissionsQuotas(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }
}
