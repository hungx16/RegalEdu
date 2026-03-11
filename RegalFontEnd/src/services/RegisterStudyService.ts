
//tạo RegisterStudyService.ts
import type { Result } from '@/types/Result';
import type { RegisterStudyModel, RegisterStudyQuery } from '@/api/RegisterStudyApi';
import type { RegisterStudyApi } from '@/api/RegisterStudyApi';
export class RegisterStudyService {
  private api: RegisterStudyApi;
  constructor(apiInstance: RegisterStudyApi) {
    this.api = apiInstance;
  }
  async fetchPagedRegisterStudy(query: RegisterStudyQuery): Promise<Result<any>> {
    return await this.api.getPagedRegisterStudy(query);
  }
  async fetchAllRegisterStudys(): Promise<Result<any>> {
    return await this.api.getAllRegisterStudy();
  }
  async getRegisterStudyById(id: string): Promise<Result<RegisterStudyModel>> {
    return await this.api.getRegisterStudyById(id);
  }
  async saveRegisterStudys(registerStudy: Partial<RegisterStudyModel>): Promise<any> {
    let result: any;
    if (registerStudy.id) {
      result = await this.api.updateRegisterStudy(registerStudy);
    } else {
      result = await this.api.addRegisterStudy(registerStudy);
    }
    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }
  async deleteRegisterStudys(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteRegisterStudies(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
  }
}