import type { DegreeModel, DegreeQuery } from '@/api/DegreeApi';
import type { DegreeApi } from '@/api/DegreeApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class DegreeService {
  private api: DegreeApi;
  constructor(apiInstance: DegreeApi) {
    this.api = apiInstance;
  }

  fetchPagedDegrees(query: DegreeQuery): Promise<Result<any>> {
    return this.api.getPagedDegrees(query);
  }

  fetchAllDegrees(): Promise<Result<any>> {
    return this.api.getAllDegrees();
  }

  async saveDegree(model: Partial<DegreeModel>): Promise<any> {
    let result: any;
    if (model.id) {
      result = await this.api.updateDegree(model);
    } else {
      result = await this.api.addDegree(model);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteDegrees(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteDegrees(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
    else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreDegrees(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreDegrees(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }
  async fetchDeletedDegrees(): Promise<Result<any>> {
    return await this.api.getDeletedDegrees();
  }
}
