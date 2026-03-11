import type { AccountGroupPermissionModel, AccountGroupPermissionRequestModel } from '@/api/accountGroupPermissionApi';
import type { AccountGroupPermissionApi } from '@/api/accountGroupPermissionApi';
import type { Result } from '@/types/Result';

export class AccountGroupPermissionService {
  private api: AccountGroupPermissionApi;

  constructor(api: AccountGroupPermissionApi) {
    this.api = api;
  }

  async fetchPermissions(accountGroupId?: string): Promise<Result> {
    return await this.api.getPermissions(accountGroupId || '');
  }
  async saveAccountGroupPermission(data: Partial<AccountGroupPermissionRequestModel>): Promise<any> {
    var result = await this.api.saveAccountGroupPermission(data);
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }
  async savePermission(data: Partial<AccountGroupPermissionModel>): Promise<any> {
    return await this.api.savePermission(data);
  }
  async getPermissions(accountGroupId: string): Promise<Result<AccountGroupPermissionModel[]>> {
    return await this.api.getPermissions(accountGroupId);
  }
  async getMenuAccept(): Promise<any> {
    return await this.api.getMenuAccept();
  }
  async getMenuAndPermissionAccept(): Promise<any> {
    return await this.api.GetMenuAndPermissionAccept();
  }
}
