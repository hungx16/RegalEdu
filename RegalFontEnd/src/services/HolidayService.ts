import type { HolidayModel, HolidayQuery } from '@/api/HolidayApi';
import type { HolidayApi } from '@/api/HolidayApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class HolidayService {
  private holidayApi: HolidayApi;

  constructor(holidayApiInstance: HolidayApi) {
    this.holidayApi = holidayApiInstance;
  }

  async fetchPagedHolidays(query: HolidayQuery): Promise<Result<any>> {
    return await this.holidayApi.getPagedHolidays(query);
  }

  async fetchAllHolidays(): Promise<Result<any>> {
    return await this.holidayApi.getAllHolidays();
  }

  async saveHoliday(holiday: Partial<HolidayModel>): Promise<any> {
    let result: any;
    if (holiday.id) {
      result = await this.holidayApi.updateHoliday(holiday);
    } else {
      result = await this.holidayApi.addHoliday(holiday);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteHolidays(holidayIds: string[]): Promise<void> {
    let result: any = await this.holidayApi.deleteHolidays(holidayIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreHolidays(holidayIds: string[]): Promise<void> {
    let result: any = await this.holidayApi.restoreHolidays(holidayIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Restore failed');
    }
  }
  async fetchDeletedHolidays(): Promise<Result<any>> {
    return await this.holidayApi.getDeletedHolidays();
  }
}
