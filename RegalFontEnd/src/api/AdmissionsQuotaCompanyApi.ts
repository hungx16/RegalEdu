// src/api/AdmissionsQuotaCompanyApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { AdmissionsQuotaAdjustmentModel, AdmissionsQuotaModel } from './AdmissionsQuotaApi';
import type { AdmissionsQuotaRegionModel } from './AdmissionsQuotaRegionApi';
import type { CompanyModel } from './CompanyApi';
import type { EmployeeModel } from './EmployeeApi';
import type { QuotaRole, WorkStage } from '@/types';
import type { PositionModel } from './PositionApi';
import type { RegionModel } from './RegionApi';
// AdmissionsQuotaEmployeeModel.cs
export interface AdmissionsQuotaEmployeeModel extends BaseEntityModel {
  admissionsQuotaId?: string;
  admissionsQuota?: AdmissionsQuotaModel | null;

  employeeId?: string | null;
  employee?: EmployeeModel | null;

  positionId: string | null;
  position?: PositionModel | null;

  revenueQuota?: number | null;   // decimal(18,2)
  joinDate?: string | null;       // ISO date
  //endDate?: string | null;        // ISO date
  allocationStartAt?: string | null; // ISO date
  allocationEndAt?: string | null;   // ISO date

  admissionsQuotaCompanyId?: string;
  admissionsQuotaCompany?: AdmissionsQuotaCompanyModel | null;

  admissionsQuotaRegionId?: string;
  admissionsQuotaRegion?: AdmissionsQuotaRegionModel | null;
  company?: CompanyModel | null;
  region?: RegionModel | null;
  quotaRole: QuotaRole;           // ASM/BM/SalesLead/Sales/Support/Probation/Leaving
  //workStage: WorkStage;           // Normal/Probation/EndingThisMonth
}
export interface AdmissionsQuotaCompanyModel extends BaseEntityModel {
  currentSales?: number | null; // int?

  admissionsQuotaId?: string;
  admissionsQuota?: AdmissionsQuotaModel | null;

  companyId: string;
  company?: CompanyModel | null;

  numberOfSalesAllocated: number;
  numberOfPartTimeSales: number;
  revenueQuotaPerSale: number;      // decimal(18,2)
  totalRevenue: number;             // decimal(18,2)

  admissionsQuotaRegionId?: string;
  admissionsQuotaRegion?: AdmissionsQuotaRegionModel | null;

  admissionsQuotaAdjustments?: AdmissionsQuotaAdjustmentModel[] | null;
  admissionsQuotaEmployees?: AdmissionsQuotaEmployeeModel[] | null;
}

export interface AdmissionsQuotaCompanyQuery {
  page: number;
  pageSize: number;
  filters?: {
    companyName?: string;
    status?: number;
    isDeleted?: boolean;
  };
}

export interface AdmissionsQuotaCompanyPagedResult {
  items: AdmissionsQuotaCompanyModel[];
  total: number;
}

export class AdmissionsQuotaCompanyApi extends ApiClient {

  controller = 'admissionsQuotaCompany';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedCompanies(query: AdmissionsQuotaCompanyQuery): Promise<Result<AdmissionsQuotaCompanyPagedResult>> {
    return await this.get<Result<AdmissionsQuotaCompanyPagedResult>>(`/${this.controller}/GetPagedAdmissionsQuotaCompanies`, { params: query });
  }

  public async getAllCompanies(): Promise<Result<AdmissionsQuotaCompanyModel[]>> {
    return await this.get<Result<AdmissionsQuotaCompanyModel[]>>(`/${this.controller}/GetAllAdmissionsQuotaCompanies`);
  }

  public async addCompany(data: Partial<AdmissionsQuotaCompanyModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddAdmissionsQuotaCompany`, data);
  }

  public async updateCompany(data: Partial<AdmissionsQuotaCompanyModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateAdmissionsQuotaCompany`, data);
  }

  public async deleteCompanies(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListAdmissionsQuotaCompany`, { data: ids });
  }

  public async restoreCompanies(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListAdmissionsQuotaCompany`, { data: ids });
  }

  public async getAdmissionsQuotaCompanyById(id: string): Promise<Result<AdmissionsQuotaCompanyModel>> {
    return await this.get<Result<AdmissionsQuotaCompanyModel>>(`/${this.controller}/GetAdmissionsQuotaCompanyById`, { params: { id } });
  }

  public async getDeletedCompanies(): Promise<Result<AdmissionsQuotaCompanyModel[]>> {
    return await this.get<Result<AdmissionsQuotaCompanyModel[]>>(`/${this.controller}/GetDeletedAdmissionsQuotaCompanies`);
  }
}
