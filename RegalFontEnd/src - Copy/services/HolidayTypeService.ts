import type { HolidayTypeModel, HolidayTypeQuery } from '@/api/HolidayTypeApi';
import type { HolidayTypeApi } from '@/api/HolidayTypeApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class HolidayTypeService {
  private api: HolidayTypeApi;
  constructor(apiInstance: HolidayTypeApi) {
    this.api = apiInstance;
  }

  fetchPagedHolidayTypes(query: HolidayTypeQuery): Promise<Result<any>> {
    return this.api.getPagedHolidayTypes(query);
  }

  fetchAllHolidayTypes(): Promise<Result<any>> {
    return this.api.getAllHolidayTypes();
  }

  async saveHolidayType(model: Partial<HolidayTypeModel>): Promise<any> {
    let result: any;
    if (model.id) {
      result = await this.api.updateHolidayType(model);
    } else {
      result = await this.api.addHolidayType(model);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteHolidayTypes(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteHolidayTypes(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
    else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreHolidayTypes(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreHolidayTypes(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }
  async fetchDeletedHolidayTypes(): Promise<Result<any>> {
    return await this.api.getDeletedHolidayTypes();
  }
}
