import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface AgeGroupModel extends BaseEntityModel {
  categoryCode: string;
  categoryName: string;
  enCategoryName?: string;
  description?: string;
  from?: number;
  to?: number;
}

export interface AgeGroupQuery {
  page: number;
  pageSize: number;
  filters?: {
    categoryCode?: string;
    categoryName?: string;
  };
}

export interface AgeGroupPagedResult {
  items: AgeGroupModel[];
  total: number;
}

export class AgeGroupApi extends ApiClient {
  controller = 'agegroup';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async getPagedAgeGroups(query: AgeGroupQuery): Promise<Result<AgeGroupPagedResult>> {
    return this.get(`/${this.controller}/GetPagedAgeGroups`, { params: query });
  }

  async getAllAgeGroups(): Promise<Result<AgeGroupModel[]>> {
    return this.get(`/${this.controller}/GetAllAgeGroups`);
  }

  async addAgeGroup(data: Partial<AgeGroupModel>): Promise<Result<any>> {
    return this.post(`/${this.controller}/AddAgeGroup`, data);
  }

  async updateAgeGroup(data: Partial<AgeGroupModel>): Promise<Result<any>> {
    return this.put(`/${this.controller}/UpdateAgeGroup`, data);
  }

  async deleteAgeGroups(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/DeleteListAgeGroups`, { data: ids });
  }

  async restoreAgeGroups(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/RestoreListAgeGroups`, { data: ids });
  }

  async getAgeGroupById(id: string): Promise<Result<AgeGroupModel>> {
    return this.get(`/${this.controller}/GetAgeGroupById`, { params: { id } });
  }

  async getDeletedAgeGroups(): Promise<Result<AgeGroupModel[]>> {
    return this.get(`/${this.controller}/GetDeletedAgeGroups`);
  }
}
