import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface HolidayModel extends BaseEntityModel {
  name: string;
  date: string; // ISO string
  categoryId?: string;
  category?: any;
  description?: string;
  frequency: number; // 0 = yearly, 1 = once
  workingTimeConfigurationId?: string;

}

export interface HolidayQuery {
  page: number;
  pageSize: number;
  filters?: {
    name?: string;
    date?: string;
    frequency?: number;
    categoryId?: string;
  };
}

export interface HolidayPagedResult {
  items: HolidayModel[];
  total: number;
}

export class HolidayApi extends ApiClient {
  controller = 'holiday';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async getPagedHolidays(query: HolidayQuery): Promise<Result<HolidayPagedResult>> {
    return this.get(`/${this.controller}/GetPagedHolidays`, { params: query });
  }

  async getAllHolidays(): Promise<Result<HolidayModel[]>> {
    return this.get(`/${this.controller}/GetAllHolidays`);
  }

  async addHoliday(data: Partial<HolidayModel>): Promise<Result<any>> {
    return this.post(`/${this.controller}/AddHoliday`, data);
  }

  async updateHoliday(data: Partial<HolidayModel>): Promise<Result<any>> {
    return this.put(`/${this.controller}/UpdateHoliday`, data);
  }

  async deleteHolidays(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/DeleteListHoliday`, { data: ids });
  }

  async restoreHolidays(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/RestoreListHoliday`, { data: ids });
  }

  async getHolidayById(id: string): Promise<Result<HolidayModel>> {
    return this.get(`/${this.controller}/GetHolidayById`, { params: { id } });
  }

  async getDeletedHolidays(): Promise<Result<HolidayModel[]>> {
    return this.get(`/${this.controller}/GetDeletedHolidays`);
  }
}
