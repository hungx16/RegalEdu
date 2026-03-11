import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface DegreeModel extends BaseEntityModel {
  degreeName: string;
  description?: string;
}

export interface DegreeQuery {
  page: number;
  pageSize: number;
  filters?: {
    degreeName?: string;
    description?: string;
  };
}

export interface DegreePagedResult {
  items: DegreeModel[];
  total: number;
}

export class DegreeApi extends ApiClient {
  controller = 'degree';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async getPagedDegrees(query: DegreeQuery): Promise<Result<DegreePagedResult>> {
    return this.get(`/${this.controller}/GetPagedDegrees`, { params: query });
  }

  async getAllDegrees(): Promise<Result<DegreeModel[]>> {
    return this.get(`/${this.controller}/GetAllDegrees`);
  }

  async addDegree(data: Partial<DegreeModel>): Promise<Result<any>> {
    return this.post(`/${this.controller}/AddDegree`, data);
  }

  async updateDegree(data: Partial<DegreeModel>): Promise<Result<any>> {
    return this.put(`/${this.controller}/UpdateDegree`, data);
  }

  async deleteDegrees(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/DeleteListDegree`, { data: ids });
  }

  async restoreDegrees(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/RestoreListDegree`, { data: ids });
  }

  async getDegreeById(id: string): Promise<Result<DegreeModel>> {
    return this.get(`/${this.controller}/GetDegreeById`, { params: { id } });
  }

  async getDeletedDegrees(): Promise<Result<DegreeModel[]>> {
    return this.get(`/${this.controller}/GetDeletedDegrees`);
  }
}
