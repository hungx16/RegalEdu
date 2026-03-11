// =============================
// WorkingTimeApi.ts
// =============================
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface WorkingTimeModel extends BaseEntityModel {

  name: string;
  startTime: string;
  endTime: string;
  dayOfWeek: number;
  isWorkingDay: boolean;
  workingTimeConfigurationId?: string;
}

export interface WorkingTimeQuery {
  page: number;
  pageSize: number;
  filters?: {
    name?: string;
    dayOfWeek?: number;
    isWorkingDay?: boolean;
  };
}

export interface WorkingTimePagedResult {
  items: WorkingTimeModel[];
  total: number;
}

export class WorkingTimeApi extends ApiClient {
  controller = 'workingtime';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async getPagedWorkingTimes(query: WorkingTimeQuery): Promise<Result<WorkingTimePagedResult>> {
    return this.get(`/${this.controller}/GetPagedWorkingTimes`, { params: query });
  }

  async getAllWorkingTimes(): Promise<Result<WorkingTimeModel[]>> {
    return this.get(`/${this.controller}/GetAllWorkingTimes`);
  }

  async addWorkingTime(data: Partial<WorkingTimeModel>): Promise<Result<any>> {
    return this.post(`/${this.controller}/AddWorkingTime`, data);
  }

  async updateWorkingTime(data: Partial<WorkingTimeModel>): Promise<Result<any>> {
    return this.put(`/${this.controller}/UpdateWorkingTime`, data);
  }

  async deleteWorkingTimes(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/DeleteListWorkingTime`, { data: ids });
  }

  async restoreWorkingTimes(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/RestoreListWorkingTime`, { data: ids });
  }

  async getWorkingTimeById(id: string): Promise<Result<WorkingTimeModel>> {
    return this.get(`/${this.controller}/GetWorkingTimeById`, { params: { id } });
  }
  async getAllWorkingTimesByConfigId(configurationId: string): Promise<Result<WorkingTimeModel[]>> {
    return this.get(`/${this.controller}/GetAllWorkingTimesByConfigId`, { params: { configurationId } });
  }
  async getDeletedWorkingTimes(): Promise<Result<WorkingTimeModel[]>> {
    return this.get(`/${this.controller}/GetDeletedWorkingTimes`);
  }
}


