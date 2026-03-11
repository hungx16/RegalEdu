// src/services/AccountGroupService.ts
import type { AccountGroupModel, AccountGroupQuery } from '@/api/AccountGroupApi';
import type { AccountGroupApi } from '@/api/AccountGroupApi';
import type { Result } from '@/types/Result';

export class AccountGroupService {
  private accountGroupApi: AccountGroupApi;

  constructor(accountGroupApiInstance: AccountGroupApi) {
    this.accountGroupApi = accountGroupApiInstance;
  }

  async fetchAccountGroups(query: AccountGroupQuery): Promise<Result> {
    return await this.accountGroupApi.getAccountGroups(query);
  }
  async fetchAllAccountGroups(): Promise<Result> {
    return await this.accountGroupApi.fetchAllAccountGroups();
  }
  async saveAccountGroup(group: Partial<AccountGroupModel>): Promise<any> {
    let result: any;
    if (group.id) {
      result = await this.accountGroupApi.updateAccountGroup(group);
    } else {
      result = await this.accountGroupApi.addAccountGroup(group);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteAccountGroup(groupId: string): Promise<void> {
    await this.accountGroupApi.deleteListAccountGroup([groupId]);
  }
}
