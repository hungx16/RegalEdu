import { ApiClient } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface AccountGroupPermissionModel {
  id?: string;
  formName: string;
  action: string;
  allowAction: boolean;
}
export interface FormPermissionDTO {
  formName: string;
  listAction: Array<string>;
}
export enum PermissionAction {
  view = "view",
  add = "add",
  edit = "edit",
  delete = "delete",
  // approval = "approval",
  // book = "book",
  // buy = "buy",
  // accept = "accept"
}
export interface AccountGroupPermissionRequestModel {
  accountGroupId: string;
  listGroupPermission?: Array<AccountGroupPermissionModel>;
}
export class AccountGroupPermissionApi extends ApiClient {
  async getMenuAccept(): Promise<any> {
    return await this.get(`/${this.controller}/GetMenuAccept`);
  }
  async GetMenuAndPermissionAccept(): Promise<any> {
    return this.get(`/${this.controller}/GetMenuAndPermissionAccept`);
  }
  async saveAccountGroupPermission(data: Partial<AccountGroupPermissionRequestModel>): Promise<any> {
    return await this.post(`/${this.controller}/SaveAccountGroupPermission`, data);
  }
  controller = 'AccountGroupPermission';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async getPermissions(accountGroupId: string): Promise<Result<AccountGroupPermissionModel[]>> {
    return await this.get(`/${this.controller}/GetAccountGroupPermissionByAccountGroupId`, {
      params: { accountGroupId }
    });
  }

  async savePermission(data: Partial<AccountGroupPermissionModel>): Promise<Result<any>> {
    return await this.post(`/${this.controller}/SaveAccountGroupPermission`, data);
  }
}
