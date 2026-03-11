import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface ClassTypeModel extends BaseEntityModel {
  classTypeCode: string;
  classTypeName: string;
  description?: string;
  sessionsPerWeek: number;
  hoursPerSession: number;
  maxStudents?: number;
  minStudents?: number;
}

export interface ClassTypeQuery {
  page: number;
  pageSize: number;
  filters?: {
    classTypeCode?: string;
    classTypeName?: string;
  };
}

export interface ClassTypePagedResult {
  items: ClassTypeModel[];
  total: number;
}

export class ClassTypeApi extends ApiClient {
  controller = 'classtype';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async getPagedClassTypes(query: ClassTypeQuery): Promise<Result<ClassTypePagedResult>> {
    return this.get(`/${this.controller}/GetPagedClassTypes`, { params: query });
  }

  async getAllClassTypes(): Promise<Result<ClassTypeModel[]>> {
    return this.get(`/${this.controller}/GetAllClassTypes`);
  }

  async addClassType(data: Partial<ClassTypeModel>): Promise<Result<any>> {
    return this.post(`/${this.controller}/AddClassType`, data);
  }

  async updateClassType(data: Partial<ClassTypeModel>): Promise<Result<any>> {
    return this.put(`/${this.controller}/UpdateClassType`, data);
  }

  async deleteClassTypes(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/DeleteListClassType`, { data: ids });
  }

  async restoreClassTypes(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/RestoreListClassType`, { data: ids });
  }

  async getClassTypeById(id: string): Promise<Result<ClassTypeModel>> {
    return this.get(`/${this.controller}/GetClassTypeById`, { params: { id } });
  }

  async getDeletedClassTypes(): Promise<Result<ClassTypeModel[]>> {
    return this.get(`/${this.controller}/GetDeletedClassTypes`);
  }
}
