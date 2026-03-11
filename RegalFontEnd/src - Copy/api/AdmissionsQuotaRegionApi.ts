// src/api/AdmissionsQuotaRegionApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { AdmissionsQuotaAdjustmentModel, AdmissionsQuotaModel } from './AdmissionsQuotaApi';
import type { AdmissionsQuotaCompanyModel, AdmissionsQuotaEmployeeModel } from './AdmissionsQuotaCompanyApi';
import type { RegionModel } from './RegionApi';

// AdmissionsQuotaRegion (vùng)
export interface AdmissionsQuotaRegionModel extends BaseEntityModel {
  admissionsQuotaId?: string;
  admissionsQuota?: AdmissionsQuotaModel | null;

  currentSales: number;
  numberOfSalesAllocated: number;

  regionId: string;
  region?: RegionModel | null;
  companyCount: number;
  revenuePerSale: number;                  // decimal(18,2)
  totalRevenue: number;             // decimal(18,2)

  admissionsQuotaCompanies?: AdmissionsQuotaCompanyModel[] | null;
  admissionsQuotaAdjustments?: AdmissionsQuotaAdjustmentModel[] | null;
  admissionsQuotaEmployees?: AdmissionsQuotaEmployeeModel[] | null;
}
export interface AdmissionsQuotaRegionQuery {
  page: number;
  pageSize: number;
  filters?: {
    regionName?: string;
    status?: number;
    isDeleted?: boolean;
  };
}

export interface AdmissionsQuotaRegionPagedResult {
  items: AdmissionsQuotaRegionModel[];
  total: number;
}

export class AdmissionsQuotaRegionApi extends ApiClient {
  controller = 'admissionsQuotaRegion';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedRegions(query: AdmissionsQuotaRegionQuery): Promise<Result<AdmissionsQuotaRegionPagedResult>> {
    return await this.get<Result<AdmissionsQuotaRegionPagedResult>>(`/${this.controller}/GetPagedAdmissionsQuotaRegions`, { params: query });
  }

  public async getAllRegions(): Promise<Result<AdmissionsQuotaRegionModel[]>> {
    return await this.get<Result<AdmissionsQuotaRegionModel[]>>(`/${this.controller}/GetAllAdmissionsQuotaRegions`);
  }

  public async addRegion(data: Partial<AdmissionsQuotaRegionModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddAdmissionsQuotaRegion`, data);
  }

  public async updateRegion(data: Partial<AdmissionsQuotaRegionModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateAdmissionsQuotaRegion`, data);
  }

  public async deleteRegions(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListAdmissionsQuotaRegion`, { data: ids });
  }

  public async restoreRegions(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListAdmissionsQuotaRegion`, { data: ids });
  }

  public async getAdmissionsQuotaRegionById(id: string): Promise<Result<AdmissionsQuotaRegionModel>> {
    return await this.get<Result<AdmissionsQuotaRegionModel>>(`/${this.controller}/GetAdmissionsQuotaRegionById`, { params: { id } });
  }
  public async getAdmissionsQuotaRegionByAdmissionsQuotaId(id: string): Promise<Result<AdmissionsQuotaRegionModel[]>> {
    return await this.get<Result<AdmissionsQuotaRegionModel[]>>(`/${this.controller}/GetAdmissionsQuotaRegionByAdmissionsQuotaId`, { params: { id } });
  }

  public async getDeletedRegions(): Promise<Result<AdmissionsQuotaRegionModel[]>> {
    return await this.get<Result<AdmissionsQuotaRegionModel[]>>(`/${this.controller}/GetDeletedAdmissionsQuotaRegions`);
  }
}
