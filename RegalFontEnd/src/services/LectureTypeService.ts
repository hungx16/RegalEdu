import type { LectureTypeModel, LectureTypeQuery } from '@/api/LectureTypeApi';
import type { LectureTypeApi } from '@/api/LectureTypeApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class LectureTypeService {
  private api: LectureTypeApi;
  constructor(apiInstance: LectureTypeApi) {
    this.api = apiInstance;
  }

  fetchPagedLectureTypes(query: LectureTypeQuery): Promise<Result<any>> {
    return this.api.getPagedLectureTypes(query);
  }

  fetchAllLectureTypes(): Promise<Result<any>> {
    return this.api.getAllLectureTypes();
  }

  async saveLectureType(model: Partial<LectureTypeModel>): Promise<any> {
    let result: any;
    if (model.id) {
      result = await this.api.updateLectureType(model);
    } else {
      result = await this.api.addLectureType(model);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteLectureTypes(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteLectureTypes(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
    else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreLectureTypes(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreLectureTypes(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }

  async fetchDeletedLectureTypes(): Promise<Result<any>> {
    return await this.api.getDeletedLectureTypes();
  }

}
