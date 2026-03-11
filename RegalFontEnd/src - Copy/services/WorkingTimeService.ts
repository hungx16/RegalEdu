
// =============================
// WorkingTimeService.ts
// =============================
import type { WorkingTimeModel, WorkingTimeQuery } from '@/api/WorkingTimeApi';
import type { WorkingTimeApi } from '@/api/WorkingTimeApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class WorkingTimeService {
  private workingTimeApi: WorkingTimeApi;

  constructor(workingTimeApiInstance: WorkingTimeApi) {
    this.workingTimeApi = workingTimeApiInstance;
  }

  async fetchPagedWorkingTimes(query: WorkingTimeQuery): Promise<Result<any>> {
    return await this.workingTimeApi.getPagedWorkingTimes(query);
  }

  async fetchAllWorkingTimes(): Promise<Result<any>> {
    return await this.workingTimeApi.getAllWorkingTimes();
  }
  async fetchAllWorkingTimesByConfigId(configurationId: string): Promise<Result<any>> {
    return await this.workingTimeApi.getAllWorkingTimesByConfigId(configurationId);
  }
  async saveWorkingTime(workingTime: Partial<WorkingTimeModel>): Promise<any> {
    let result: any;
    if (workingTime.id) {
      result = await this.workingTimeApi.updateWorkingTime(workingTime);
    } else {
      result = await this.workingTimeApi.addWorkingTime(workingTime);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteWorkingTimes(workingTimeIds: string[]): Promise<void> {
    let result: any = await this.workingTimeApi.deleteWorkingTimes(workingTimeIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreWorkingTimes(workingTimeIds: string[]): Promise<void> {
    let result: any = await this.workingTimeApi.restoreWorkingTimes(workingTimeIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Restore failed');
    }
  }
  async fetchDeletedWorkingTimes(): Promise<Result<any>> {
    return await this.workingTimeApi.getDeletedWorkingTimes();

  }
}
