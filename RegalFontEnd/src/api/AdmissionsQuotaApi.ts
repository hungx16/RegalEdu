// src/api/AdmissionsQuotaApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { AdmissionsQuotaRegionModel } from './AdmissionsQuotaRegionApi';
import type { AdmissionsQuotaCompanyModel, AdmissionsQuotaEmployeeModel } from './AdmissionsQuotaCompanyApi';
import type { AdjustmentScope, QuotaStatus } from '@/types';


export interface AdmissionsQuotaAdjustmentModel extends BaseEntityModel {
  scope: AdjustmentScope;         // Region / Company

  admissionsQuotaRegionId?: string;
  admissionsQuotaRegion?: AdmissionsQuotaRegionModel | null;

  admissionsQuotaCompanyId?: string;
  admissionsQuotaCompany?: AdmissionsQuotaCompanyModel | null;

  totalQuotaBefore?: number | null; // decimal(18,2)
  totalQuotaAfter?: number | null;  // decimal(18,2)
  reason?: string | null;
}
export interface AdmissionsQuotaModel extends BaseEntityModel {
  year: number;
  month: number;
  currentSales: number;
  totalSalesAllocated: number;
  quotaStage: QuotaStatus;
  companyCount: number;
  totalQuota: number;             // decimal(18,2)
  note?: string | null;

  admissionsQuotaCompanies?: AdmissionsQuotaCompanyModel[] | null;
  admissionsQuotaRegions?: AdmissionsQuotaRegionModel[] | null;
  admissionsQuotaEmployees?: AdmissionsQuotaEmployeeModel[] | null;
}
export type TransferRole = 'Sales' | 'BM' | 'ASM'

export interface TransferSegment {
  role: TransferRole          // vai trò của đoạn
  start: string               // YYYY-MM-DD (nằm trong tháng của đợt)
  end: string                 // YYYY-MM-DD (>= start)
  companyId?: string          // bắt buộc nếu role = 'Sales' | 'BM'
  regionId?: string           // bắt buộc nếu role = 'ASM'
}
export type TransferRolesRequest = Partial<AdmissionsQuotaModel> & { status?: number }


export interface AdmissionsQuotaQuery {
  page: number;
  pageSize: number;
  filters?: {
    quotaName?: string;
    academicYear?: string;
    status?: number;
    isDeleted?: boolean;
  };
}

export interface AdmissionsQuotaPagedResult {
  items: AdmissionsQuotaModel[];
  total: number;
}

export class AdmissionsQuotaApi extends ApiClient {

  controller = 'admissionsQuota';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedAdmissionsQuotas(query: AdmissionsQuotaQuery): Promise<Result<AdmissionsQuotaPagedResult>> {
    return await this.get<Result<AdmissionsQuotaPagedResult>>(`/${this.controller}/GetPagedAdmissionsQuotas`, { params: query });
  }

  public async getAllAdmissionsQuotas(): Promise<Result<AdmissionsQuotaModel[]>> {
    return await this.get<Result<AdmissionsQuotaModel[]>>(`/${this.controller}/GetAllAdmissionsQuotas`);
  }

  public async addAdmissionsQuota(data: Partial<AdmissionsQuotaModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddAdmissionsQuota`, data);
  }
  public async addAdmissionsQuotaAdjustment(data: Partial<AdmissionsQuotaAdjustmentModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddAdmissionsQuotaAdjustment`, data);
  }
  public async assignSupportStaff(quota: Partial<AdmissionsQuotaEmployeeModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AssignSupportStaff`, quota);
  }
  public async updateAdmissionsQuota(data: Partial<AdmissionsQuotaModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateAdmissionsQuota`, data);
  }
  public async getMyAdmissionsQuota(): Promise<Result<AdmissionsQuotaEmployeeModel[]>> {
    return await this.get<Result<AdmissionsQuotaEmployeeModel[]>>(`/${this.controller}/GetMyAdmissionsQuota`);
  }

  public async deleteAdmissionsQuotas(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListAdmissionsQuota`, { data: ids });
  }

  public async restoreAdmissionsQuotas(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListAdmissionsQuota`, { data: ids });
  }

  public async getAdmissionsQuotaById(id: string): Promise<Result<AdmissionsQuotaModel>> {
    return await this.get<Result<AdmissionsQuotaModel>>(`/${this.controller}/GetAdmissionsQuotaById`, { params: { id } });
  }

  public async getDeletedAdmissionsQuotas(): Promise<Result<AdmissionsQuotaModel[]>> {
    return await this.get<Result<AdmissionsQuotaModel[]>>(`/${this.controller}/GetDeletedAdmissionsQuotas`);
  }

  public async transferRoles(data: TransferRolesRequest): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/TransferRoles`, data);
  }
}
