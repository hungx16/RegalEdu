// src/api/DivisionApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface DivisionModel extends BaseEntityModel {
  divisionCode: string;
  divisionName: string;
  divisionLevel: number;
  description?: string | null;
  // status: number;
  departments?: any[];
}

export interface DivisionQuery {
  page: number;
  pageSize: number;
  filters?: {
    divisionCode?: string;
    divisionName?: string;
    status?: number;
    isDeleted?: boolean;
  };
}

export interface DivisionPagedResult {
  items: DivisionModel[];
  total: number;
}

export class DivisionApi extends ApiClient {
  controller = 'division';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedDivisions(query: DivisionQuery): Promise<Result<DivisionPagedResult>> {
    return await this.get<Result<DivisionPagedResult>>(`/${this.controller}/GetPagedDivisions`, { params: query });
  }

  public async getAllDivisions(): Promise<Result<DivisionModel[]>> {
    return await this.get<Result<DivisionModel[]>>(`/${this.controller}/GetAllDivisions`);
  }

  public async addDivision(data: Partial<DivisionModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddDivision`, data);
  }

  public async updateDivision(data: Partial<DivisionModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateDivision`, data);
  }

  public async deleteDivisions(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListDivision`, { data: ids });
  }

  public async restoreDivisions(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListDivision`, { data: ids });
  }

  public async getDivisionById(id: string): Promise<Result<DivisionModel>> {
    return await this.get<Result<DivisionModel>>(`/${this.controller}/GetDivisionById`, { params: { id } });
  }

  public async getDeletedDivisions(): Promise<Result<DivisionModel[]>> {
    return await this.get<Result<DivisionModel[]>>(`/${this.controller}/GetDeletedDivisions`);
  }
}
