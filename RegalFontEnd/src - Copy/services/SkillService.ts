import type { SkillModel, SkillQuery } from '@/api/SkillApi';
import type { SkillApi } from '@/api/SkillApi';
import type { Result } from '@/types/Result';

export class SkillService {
  private api: SkillApi;
  constructor(apiInstance: SkillApi) {
    this.api = apiInstance;
  }

  fetchPagedSkills(query: SkillQuery): Promise<Result<any>> {
    return this.api.getPagedSkills(query);
  }

  fetchAllSkills(): Promise<Result<any>> {
    return this.api.getAllSkills();
  }

  async saveSkill(model: Partial<SkillModel>): Promise<any> {
    let result: any;
    if (model.id) {
      result = await this.api.updateSkill(model);
    } else {
      result = await this.api.addSkill(model);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteSkills(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteSkills(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
  }

  async restoreSkills(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreSkills(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }

  async fetchDeletedSkills(): Promise<Result<any>> {
    return await this.api.getDeletedSkills();
  }
}
