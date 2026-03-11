// src/api/AccountGroupApi.ts
import { ApiClient } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface AccountGroupModel {
  id?: string | null;
  name: string;
  enable: boolean;
  useDefault: boolean;
  note?: string | null;
  created?: string | null;
  lastModified?: string | null;
  createdBy?: string | null;
  lastModifiedBy?: string | null;
}

export interface AccountGroupQuery {
  page: number;
  pageSize: number;
  filters?: {
    name?: string;
    enable?: boolean;
    useDefault?: boolean;
  };
}

export interface AccountGroupPagedResult {
  items: AccountGroupModel[];
  total: number;
}

export class AccountGroupApi extends ApiClient {
  controller = 'AccountGroup';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getAccountGroups(query: AccountGroupQuery): Promise<Result<AccountGroupPagedResult>> {
    return await this.get<Result<AccountGroupPagedResult>>(`/${this.controller}/GetAccountGroups`, { params: query });
  }
  public async fetchAllAccountGroups(): Promise<Result<AccountGroupPagedResult>> {
    return await this.get<Result<AccountGroupPagedResult>>(`/${this.controller}/GetAllAccountGroups`);
  }
  public async addAccountGroup(data: Partial<AccountGroupModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddAccountGroup`, data);
  }

  public async updateAccountGroup(data: Partial<AccountGroupModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateAccountGroup`, data);
  }

  public async deleteListAccountGroup(id: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteAccountGroup`, { data: id });
  }
}
