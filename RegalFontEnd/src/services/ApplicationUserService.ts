// src/services/ApplicationUserService.ts
import type { ApplicationUserModel, ApplicationUserQuery } from '@/api/ApplicationUserApi';
import type { ApplicationUserApi } from '@/api/ApplicationUserApi';
import type { Result } from '@/types/Result';

export class ApplicationUserService {
  private applicationUserApi: ApplicationUserApi;

  constructor(applicationUserApiInstance: ApplicationUserApi) {
    this.applicationUserApi = applicationUserApiInstance;
  }

  async fetchPagedApplicationUser(query: ApplicationUserQuery): Promise<Result> {
    return await this.applicationUserApi.GetPagedApplicationUsers(query);
  }
  async fetchAllApplicationUsers(): Promise<Result> {
    return await this.applicationUserApi.GetAllApplicationUsers();
  }
  async saveApplicationUser(user: Partial<ApplicationUserModel>): Promise<any> {
    let result: any;
    if (user.id) {
      result = await this.applicationUserApi.updateApplicationUser(user);
    } else {
      result = await this.applicationUserApi.addApplicationUser(user);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteApplicationUser(userIds: string[]): Promise<void> {
    let result: any = await this.applicationUserApi.deleteListUser(userIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    }
  }
}
