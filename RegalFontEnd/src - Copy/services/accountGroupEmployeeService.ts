import type { AccountGroupEmployeeApi, AccountGroupEmployeeModel } from '@/api/accountGroupEmployeeApi';
import type { Result } from '@/types/Result';

export class AccountGroupEmployeeService {

  private api: AccountGroupEmployeeApi;

  constructor(apiInstance: AccountGroupEmployeeApi) {
    this.api = apiInstance;
  }

  async fetchByGroupId(groupId: string): Promise<Result<any>> {
    return await this.api.getByGroupId(groupId);
  }

  async getEmployeeNoGroup(): Promise<Result<any>> {
    return await this.api.getEmployeeNoGroup();
  }
  async getAccountGroupEmployeeByGroupId(groupId: string): Promise<Result<any>> {
    return await this.api.getAccountGroupEmployeeByGroupId(groupId);
  }
  async saveEmployee(data: AccountGroupEmployeeModel): Promise<void> {
    let result;
    if (data.id) {
      result = await this.api.save(data);
    } else {
      result = await this.api.add(data);
    }
    if (!result?.succeeded) {
      throw new Error(result?.error || 'Save failed');
    }
  }
  async saveAccountGroupEmployee(model: any) {
    return await this.api.SaveAccountGroupEmployee(model);
  }
  async addAccountGroupEmployee(model: any) {
    return await this.api.AddAccountGroupEmployee(model);
  }
}
