import type { ClassTypeModel, ClassTypeQuery } from '@/api/ClassTypeApi';
import type { ClassTypeApi } from '@/api/ClassTypeApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class ClassTypeService {
  private api: ClassTypeApi;
  constructor(apiInstance: ClassTypeApi) {
    this.api = apiInstance;
  }

  fetchPagedClassTypes(query: ClassTypeQuery): Promise<Result<any>> {
    return this.api.getPagedClassTypes(query);
  }

  fetchAllClassTypes(): Promise<Result<any>> {
    return this.api.getAllClassTypes();
  }

  async saveClassType(model: Partial<ClassTypeModel>): Promise<any> {
    let result: any;
    if (model.id) {
      result = await this.api.updateClassType(model);
    } else {
      result = await this.api.addClassType(model);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteClassTypes(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteClassTypes(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
    else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreClassTypes(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreClassTypes(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }
  async fetchDeletedClassTypes(): Promise<Result<any>> {
    return await this.api.getDeletedClassTypes();
  }
  async getClassTypeById(id: string): Promise<ClassTypeModel | null> {
    const result: Result<ClassTypeModel> = await this.api.getClassTypeById(id);
    if (!result.succeeded) throw new Error(result.errors || 'Fetch failed');
    return result.data ?? null;
  }
}

