// src/services/AdmissionsQuotaCompanyService.ts
import type { AdmissionsQuotaCompanyApi } from '@/api/AdmissionsQuotaCompanyApi';
import type { AdmissionsQuotaCompanyModel, AdmissionsQuotaCompanyQuery } from '@/api/AdmissionsQuotaCompanyApi';
import type { Result } from '@/types/Result';

export class AdmissionsQuotaCompanyService {
  private api: AdmissionsQuotaCompanyApi;

  constructor(apiInstance: AdmissionsQuotaCompanyApi) {
    this.api = apiInstance;
  }

  async fetchPagedCompanies(query: AdmissionsQuotaCompanyQuery): Promise<Result<any>> {
    return await this.api.getPagedCompanies(query);
  }

  async fetchAllCompanies(): Promise<Result<any>> {
    return await this.api.getAllCompanies();
  }

  async saveCompany(company: Partial<AdmissionsQuotaCompanyModel>): Promise<any> {
    let result: any;
    if (company.id) {
      result = await this.api.updateCompany(company);
    } else {
      result = await this.api.addCompany(company);
    }
    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }

  async deleteCompanies(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteCompanies(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
  }

  async restoreCompanies(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreCompanies(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }
  async fetchAdmissionsQuotaCompanyById(id: string): Promise<any> {
    return await this.api.getAdmissionsQuotaCompanyById(id);
  }
}
