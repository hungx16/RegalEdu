// src/api/PartnerTypeApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface PartnerTypeModel extends BaseEntityModel {
  partnerTypeCode: string;
  partnerTypeName: string;
  enPartnerTypeName?: string;
  description?: string | null;
  isMultilingual?: boolean | null;
}

export interface PartnerTypeQuery {
  page: number;
  pageSize: number;
  filters?: {
    partnerTypeCode?: string;
    partnerTypeName?: string;
    status?: number | undefined;
    isDeleted?: boolean | undefined;
  };
}

export class PartnerTypeApi extends ApiClient {
  private controller = 'PartnerType';
  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedPartnerTypes(query: PartnerTypeQuery): Promise<Result<any>> {
    return await this.get<Result<any>>(`/${this.controller}/GetPagedPartnerTypes`, { params: query });
  }

  public async addPartnerType(model: Partial<PartnerTypeModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddPartnerType`, model);
  }

  public async updatePartnerType(model: Partial<PartnerTypeModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdatePartnerType`, model);
  }

  public async deleteListPartnerType(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListPartnerType`, { data: ids });
  }

  public async getPartnerTypeById(id: string): Promise<Result<PartnerTypeModel>> {
    // Controller signature uses [HttpGet], we'll pass id as query param
    return await this.get<Result<PartnerTypeModel>>(`/${this.controller}/GetPartnerTypeById`, { params: { id } });
  }

  public async getAllPartnerTypes(): Promise<Result<PartnerTypeModel[]>> {
    return await this.get<Result<PartnerTypeModel[]>>(`/${this.controller}/GetAllPartnerTypes`);
  }

  public async getDeletedPartnerTypes(): Promise<Result<PartnerTypeModel[]>> {
    return await this.get<Result<PartnerTypeModel[]>>(`/${this.controller}/GetDeletedPartnerTypes`);
  }
}
