// src/services/learningRoadMapService.ts
import type { LearningRoadMapApi, LearningRoadMapModel, LearningRoadMapQuery } from '@/api/LearningRoadMapApi'
import type { Result } from '@/types/Result';

export class LearningRoadMapService {
  private api: LearningRoadMapApi;

  constructor(apiInstance: LearningRoadMapApi) {
    this.api = apiInstance;
  }

  async fetchPagedLearningRoadMaps(query: LearningRoadMapQuery): Promise<Result<any>> {
    return await this.api.getPagedLearningRoadMaps(query);
  }

  async fetchAllLearningRoadMaps(): Promise<Result<any>> {
    return await this.api.getAllLearningRoadMaps();
  }

  async saveLearningRoadMap(model: Partial<LearningRoadMapModel>): Promise<any> {
    let result: any;
    if (model.id) {
      model.ageGroup = undefined; // tránh gửi ageGroup khi cập nhật
      result = await this.api.updateLearningRoadMap(model);
    } else {
      result = await this.api.addLearningRoadMap(model);
    }
    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }

  async deleteLearningRoadMaps(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteLearningRoadMaps(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
  }

  async restoreLearningRoadMaps(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreLearningRoadMaps(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }

  async getLearningRoadMapById(id: string): Promise<LearningRoadMapModel> {
    const result: any = await this.api.getLearningRoadMapById(id);
    if (!result.succeeded) throw new Error(result.error || 'Fetch failed');
    return result.data;
  }

  async getDeletedLearningRoadMaps(): Promise<LearningRoadMapModel[]> {
    const result: any = await this.api.getDeletedLearningRoadMaps();
    if (!result.succeeded) throw new Error(result.error || 'Fetch failed');
    return result.data;
  }
}
