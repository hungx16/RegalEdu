import { ApiClient } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface AccountGroupEmployeeModel {
  id?: string;
  accountGroupId: string;
  userCode: string;
  accountGroup?: any;
  isApprover: boolean;
}
export interface AccountGroupEmployeeRequestModel {
  accountGroupId: string;
  listuserCode: Array<string>;
  listIsApprover: Array<boolean>;
}
export class AccountGroupEmployeeApi extends ApiClient {
  controller = 'AccountGroupEmployee';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getByGroupId(groupId: string): Promise<Result<AccountGroupEmployeeModel[]>> {
    return await this.get(`/${this.controller}/GetAccountGroupEmployeeByGroupId`, {
      params: { accountGroupId: groupId }
    });
  }

  public async getEmployeeNoGroup(): Promise<Result<string[]>> {
    return await this.get(`/${this.controller}/GetEmployeeNoGroup`);
  }

  public async add(data: AccountGroupEmployeeModel): Promise<Result<any>> {
    return await this.post(`/${this.controller}/AddAccountGroupEmployee`, data);
  }

  public async save(data: AccountGroupEmployeeModel): Promise<Result<any>> {
    return await this.post(`/${this.controller}/SaveAccountGroupEmployee`, data);
  }

  /**
   * GetAccountGroupEmployeeByGroupId
   */
  public async getAccountGroupEmployeeByGroupId(groupId: string): Promise<Result<AccountGroupEmployeeModel[]>> {
    return await this.get(`/${this.controller}/GetAccountGroupEmployeeByGroupId`, {
      params: { accountGroupId: groupId }
    });
  }
  public async AddAccountGroupEmployee(model: AccountGroupEmployeeRequestModel): Promise<Result<any>> {
    return this.post(`/${this.controller}/AddAccountGroupEmployee`, model);
  }
  public async SaveAccountGroupEmployee(model: AccountGroupEmployeeRequestModel): Promise<Result<any>> {
    return this.post(`/${this.controller}/SaveAccountGroupEmployee`, model);
  }
}
