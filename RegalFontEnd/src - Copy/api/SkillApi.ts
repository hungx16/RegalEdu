import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface SkillModel extends BaseEntityModel {
  categoryCode: string;
  categoryName: string;
  description?: string;
}

export interface SkillQuery {
  page: number;
  pageSize: number;
  filters?: {
    categoryCode?: string;
    categoryName?: string;
  };
}

export interface SkillPagedResult {
  items: SkillModel[];
  total: number;
}

export class SkillApi extends ApiClient {
  controller = 'skill';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async getPagedSkills(query: SkillQuery): Promise<Result<SkillPagedResult>> {
    return this.get(`/${this.controller}/GetPagedSkills`, { params: query });
  }

  async getAllSkills(): Promise<Result<SkillModel[]>> {
    return this.get(`/${this.controller}/GetAllSkills`);
  }

  async addSkill(data: Partial<SkillModel>): Promise<Result<any>> {
    return this.post(`/${this.controller}/AddSkill`, data);
  }

  async updateSkill(data: Partial<SkillModel>): Promise<Result<any>> {
    return this.put(`/${this.controller}/UpdateSkill`, data);
  }

  async deleteSkills(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/DeleteListSkills`, { data: ids });
  }

  async restoreSkills(ids: string[]): Promise<Result<any>> {
    return this.delete(`/${this.controller}/RestoreListSkills`, { data: ids });
  }

  async getSkillById(id: string): Promise<Result<SkillModel>> {
    return this.get(`/${this.controller}/GetSkillById`, { params: { id } });
  }

  async getDeletedSkills(): Promise<Result<SkillModel[]>> {
    return this.get(`/${this.controller}/GetDeletedSkills`);
  }
}
