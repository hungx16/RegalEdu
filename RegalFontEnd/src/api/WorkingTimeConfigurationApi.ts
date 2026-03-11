import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface WorkingTimeConfigurationCompanyModel extends BaseEntityModel {
  id?: string | null;
  companyId: string;
  company?: any;
  workingTimeConfigurationId?: string | null;
}

export interface WorkingTimeConfigurationModel extends BaseEntityModel {
  nameConfiguration: string;
  description?: string;
  workingTimes: any[]; // WorkingTimeModel[]
  holidays: any[];     // HolidayModel[]
  applyToSystem: boolean;
  isDefault: boolean;
  workingTimeConfigurationCompanies?: WorkingTimeConfigurationCompanyModel[];
  companyIds?: string[]; // Array of company IDs for binding
}

export interface WorkingTimeConfigurationQuery {
  page: number;
  pageSize: number;
  filters?: {
    nameConfiguration?: string;
    applyToSystem?: boolean;
  };
}

export interface WorkingTimeConfigurationPagedResult {
  items: WorkingTimeConfigurationModel[];
  total: number;
}

export class WorkingTimeConfigurationApi extends ApiClient {
  controller = 'workingtimeconfiguration';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async getPagedWorkingTimeConfigurations(query: WorkingTimeConfigurationQuery): Promise<Result<WorkingTimeConfigurationPagedResult>> {
    return this.get(`/${this.controller}/GetPagedWorkingTimeConfigurations`, { params: query });
  }

  async getAllWorkingTimeConfigurations(): Promise<Result<WorkingTimeConfigurationModel[]>> {
    return this.get(`/${this.controller}/GetAllWorkingTimeConfigurations`);
  }

  async addWorkingTimeConfiguration(data: Partial<WorkingTimeConfigurationModel>): Promise<Result<any>> {
    return this.post(`/${this.controller}/AddWorkingTimeConfiguration`, data);
  }

  async updateWorkingTimeConfiguration(data: Partial<WorkingTimeConfigurationModel>): Promise<Result<any>> {
    return this.put(`/${this.controller}/UpdateWorkingTimeConfiguration`, data);
  }

  async deleteWorkingTimeConfigurations(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/DeleteListWorkingTimeConfiguration`, { data: ids });
  }

  async restoreWorkingTimeConfigurations(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/RestoreListWorkingTimeConfiguration`, { data: ids });
  }

  async getWorkingTimeConfigurationById(id: string): Promise<Result<WorkingTimeConfigurationModel>> {
    return this.get(`/${this.controller}/GetWorkingTimeConfigurationById`, { params: { id } });
  }

  async getDeletedWorkingTimeConfigurations(): Promise<Result<WorkingTimeConfigurationModel[]>> {
    return this.get(`/${this.controller}/GetDeletedWorkingTimeConfigurations`);
  }
}
