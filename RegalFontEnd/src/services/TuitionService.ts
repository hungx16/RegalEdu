// src/services/TuitionService.ts
import type { TuitionApi } from '@/api/TuitionApi';
import type { TuitionModel, TuitionQuery } from '@/api/TuitionApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class TuitionService {
  private api: TuitionApi;

  constructor(apiInstance: TuitionApi) {
    this.api = apiInstance;
  }

  async fetchPagedTuitions(query: TuitionQuery): Promise<Result<any>> {
    return await this.api.getPagedTuitions(query);
  }

  async fetchAllTuitions(): Promise<Result<any>> {
    return await this.api.getAllTuitions();
  }

  async saveTuition(Tuition: Partial<TuitionModel>): Promise<any> {
    let result: any;
    if (Tuition.id) {
      result = await this.api.updateTuition(Tuition);
    } else {
      result = await this.api.addTuition(Tuition);
    }
    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }

  async deleteTuitions(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteTuitions(ids);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreTuitions(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreTuitions(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }
}
