import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface HolidayTypeModel extends BaseEntityModel {
  categoryCode: string;
  categoryName: string;
  description?: string;
}

export interface HolidayTypeQuery {
  page: number;
  pageSize: number;
  filters?: {
    categoryCode?: string;
    categoryName?: string;
  };
}

export interface HolidayTypePagedResult {
  items: HolidayTypeModel[];
  total: number;
}

export class HolidayTypeApi extends ApiClient {
  controller = 'holidaytype';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async getPagedHolidayTypes(query: HolidayTypeQuery): Promise<Result<HolidayTypePagedResult>> {
    return this.get(`/${this.controller}/GetPagedHolidayTypes`, { params: query });
  }

  async getAllHolidayTypes(): Promise<Result<HolidayTypeModel[]>> {
    return this.get(`/${this.controller}/GetAllHolidayTypes`);
  }

  async addHolidayType(data: Partial<HolidayTypeModel>): Promise<Result<any>> {
    return this.post(`/${this.controller}/AddHolidayType`, data);
  }

  async updateHolidayType(data: Partial<HolidayTypeModel>): Promise<Result<any>> {
    return this.put(`/${this.controller}/UpdateHolidayType`, data);
  }

  async deleteHolidayTypes(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/DeleteListHolidayTypes`, { data: ids });
  }

  async restoreHolidayTypes(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/RestoreListHolidayTypes`, { data: ids });
  }

  async getHolidayTypeById(id: string): Promise<Result<HolidayTypeModel>> {
    return this.get(`/${this.controller}/GetHolidayTypeById`, { params: { id } });
  }

  async getDeletedHolidayTypes(): Promise<Result<HolidayTypeModel[]>> {
    return this.get(`/${this.controller}/GetDeletedHolidayTypes`);
  }
}
