import type { WorkingTimeConfigurationModel, WorkingTimeConfigurationQuery } from '@/api/WorkingTimeConfigurationApi';
import type { WorkingTimeConfigurationApi } from '@/api/WorkingTimeConfigurationApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class WorkingTimeConfigurationService {
  private api: WorkingTimeConfigurationApi;
  constructor(apiInstance: WorkingTimeConfigurationApi) {
    this.api = apiInstance;
  }

  async fetchPagedWorkingTimeConfigurations(query: WorkingTimeConfigurationQuery): Promise<Result<any>> {
    return await this.api.getPagedWorkingTimeConfigurations(query);
  }

  async fetchAllWorkingTimeConfigurations(): Promise<Result<any>> {
    return await this.api.getAllWorkingTimeConfigurations();
  }

  async saveWorkingTimeConfiguration(model: Partial<WorkingTimeConfigurationModel>): Promise<any> {
    let result: any;
    if (model.id) {
      result = await this.api.updateWorkingTimeConfiguration(model);
    } else {
      result = await this.api.addWorkingTimeConfiguration(model);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteWorkingTimeConfigurations(ids: string[]): Promise<void> {
    let result: any = await this.api.deleteWorkingTimeConfigurations(ids);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreWorkingTimeConfigurations(ids: string[]): Promise<void> {
    let result: any = await this.api.restoreWorkingTimeConfigurations(ids);
    if (!result.succeeded) {
      throw new Error(result.error || 'Restore failed');
    }
  }
  async fetchDeletedWorkingTimeConfigurations(): Promise<Result<any>> {
    return await this.api.getDeletedWorkingTimeConfigurations();
  }

}
