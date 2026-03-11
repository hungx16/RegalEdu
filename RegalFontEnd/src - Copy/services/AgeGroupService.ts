import type { AgeGroupModel, AgeGroupQuery } from '@/api/AgeGroupApi';
import type { AgeGroupApi } from '@/api/AgeGroupApi';
import type { Result } from '@/types/Result';

export class AgeGroupService {
  private api: AgeGroupApi;
  constructor(apiInstance: AgeGroupApi) {
    this.api = apiInstance;
  }

  fetchPagedAgeGroups(query: AgeGroupQuery): Promise<Result<any>> {
    return this.api.getPagedAgeGroups(query);
  }

  fetchAllAgeGroups(): Promise<Result<any>> {
    return this.api.getAllAgeGroups();
  }

  async saveAgeGroup(model: Partial<AgeGroupModel>): Promise<any> {
    let result: any;
    if (model.id) {
      result = await this.api.updateAgeGroup(model);
    } else {
      result = await this.api.addAgeGroup(model);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteAgeGroups(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteAgeGroups(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
  }

  async restoreAgeGroups(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreAgeGroups(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }

  async fetchDeletedAgeGroups(): Promise<Result<any>> {
    return await this.api.getDeletedAgeGroups();
  }
}
