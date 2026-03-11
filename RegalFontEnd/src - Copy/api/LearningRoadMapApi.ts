// src/api/LearningRoadMapApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { ImageModel } from './CompanyApi';

export interface LearningRoadMapModel extends BaseEntityModel {
  learningRoadMapCode: string;
  learningRoadMapName: string;
  description?: string;
  ageGrId: string;
  ageGroup?: any; // để hiển thị tên AgeGroup
  commitmentOutput: boolean;
  order: number;
  isPublish?: boolean;
  numberOfStudents?: number;
  numberOfSatisfiedStudents?: number;
  votingRate?: number;
  enDescription?: string;
  enLearningRoadMapName?: string;
  isMultilingual?: boolean;
  deletedImageIds?: string[] | null; // <== khi edit, gửi kèm
  images: ImageModel[];
}

export interface LearningRoadMapQuery {
  page: number;
  pageSize: number;
  filters?: {
    learningRoadMapCode?: string;
    learningRoadMapName?: string;
    ageGrId?: string;
    status?: number;
    isDeleted?: boolean;
  };
}

export interface LearningRoadMapPagedResult {
  items: LearningRoadMapModel[];
  total: number;
}

export class LearningRoadMapApi extends ApiClient {
  controller = 'learningRoadMap';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedLearningRoadMaps(query: LearningRoadMapQuery): Promise<Result<LearningRoadMapPagedResult>> {
    return await this.get<Result<LearningRoadMapPagedResult>>(`/${this.controller}/GetPagedLearningRoadMaps`, { params: query });
  }

  public async getAllLearningRoadMaps(): Promise<Result<LearningRoadMapModel[]>> {
    return await this.get<Result<LearningRoadMapModel[]>>(`/${this.controller}/GetAllLearningRoadMaps`);
  }

  public async addLearningRoadMap(data: Partial<LearningRoadMapModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddLearningRoadMap`, data);
  }

  public async updateLearningRoadMap(data: Partial<LearningRoadMapModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateLearningRoadMap`, data);
  }

  public async deleteLearningRoadMaps(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListLearningRoadMaps`, { data: ids });
  }

  public async restoreLearningRoadMaps(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListLearningRoadMaps`, { data: ids });
  }

  public async getLearningRoadMapById(id: string): Promise<Result<LearningRoadMapModel>> {
    return await this.get<Result<LearningRoadMapModel>>(`/${this.controller}/GetLearningRoadMapById`, { params: { id } });
  }

  public async getDeletedLearningRoadMaps(): Promise<Result<LearningRoadMapModel[]>> {
    return await this.get<Result<LearningRoadMapModel[]>>(`/${this.controller}/GetDeletedLearningRoadMaps`);
  }
}
