// src/services/CompanyService.ts
import type { CompanyModel, CompanyQuery, LogRegionComModel } from '@/api/CompanyApi';
import type { CompanyApi } from '@/api/CompanyApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class CompanyService {
  private companyApi: CompanyApi;

  constructor(companyApiInstance: CompanyApi) {
    this.companyApi = companyApiInstance;
  }

  async fetchPagedCompanies(query: CompanyQuery): Promise<Result<any>> {
    return await this.companyApi.getPagedCompanies(query);
  }

  async fetchAllCompanies(): Promise<Result<any>> {
    return await this.companyApi.getAllCompanies();
  }

  async saveCompany(company: Partial<CompanyModel>): Promise<any> {
    let result: any;
    if (company.id) {
      company.manager = null; // Clear manager to avoid sending unnecessary data
      company.region = null; // Clear region to avoid sending unnecessary data

      result = await this.companyApi.updateCompany(company);
    } else {
      result = await this.companyApi.addCompany(company);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteCompanies(companyIds: string[]): Promise<void> {
    let result: any = await this.companyApi.deleteCompanies(companyIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    }
    else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreCompanies(companyIds: string[]): Promise<void> {
    let result: any = await this.companyApi.restoreCompanies(companyIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Restore failed');
    }
  }
  async getAllCompanyRegions(): Promise<Result<any>> {
    return await this.companyApi.getAllCompanyRegions();
  }
  async createLogRegionCom(data: Partial<LogRegionComModel>): Promise<any> {
    let result: any;
    result = await this.companyApi.createLogRegionCom(data);
    if (!result.succeeded) {
      throw new Error(result.error || 'Create failed');
    }
    return result.data;
  }
  async fetchDeletedCompanies(): Promise<Result<any>> {
    return await this.companyApi.getDeletedCompanies();
  }

}
