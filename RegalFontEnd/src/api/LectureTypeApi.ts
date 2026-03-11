import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { Attachment } from './FileApi';

export interface LectureTypeModel extends BaseEntityModel {
  lectureName: string;
  description?: string;
  attachment?: Attachment;
}

export interface LectureTypeQuery {
  page: number;
  pageSize: number;
  filters?: {
    lectureName?: string;
    description?: string;
  };
}

export interface LectureTypePagedResult {
  items: LectureTypeModel[];
  total: number;
}

export class LectureTypeApi extends ApiClient {
  controller = 'lecturetype';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async getPagedLectureTypes(query: LectureTypeQuery): Promise<Result<LectureTypePagedResult>> {
    return this.get(`/${this.controller}/GetPagedLectureTypes`, { params: query });
  }

  async getAllLectureTypes(): Promise<Result<LectureTypeModel[]>> {
    return this.get(`/${this.controller}/GetAllLectureTypes`);
  }


  async addLectureType(data: Partial<LectureTypeModel>): Promise<Result<any>> {
    return this.post(`/${this.controller}/AddLectureType`, data);
  }
  async updateLectureType(data: Partial<LectureTypeModel>): Promise<Result<any>> {
    return this.put(`/${this.controller}/UpdateLectureType`, data);
  }

  async deleteLectureTypes(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/DeleteListLectureType`, { data: ids });
  }

  async restoreLectureTypes(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/RestoreListLectureType`, { data: ids });
  }

  async getLectureTypeById(id: string): Promise<Result<LectureTypeModel>> {
    return this.get(`/${this.controller}/GetLectureTypeById`, { params: { id } });
  }

  async getDeletedLectureTypes(): Promise<Result<LectureTypeModel[]>> {
    return this.get(`/${this.controller}/GetDeletedLectureTypes`);
  }
}
