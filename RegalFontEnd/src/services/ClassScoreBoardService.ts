import type {
  ClassScoreBoardApi,
  ClassScoreSummaryModel,
  UpdateClassScoreBoardModel
} from '@/api/ClassScoreBoardApi';
import type { Result } from '@/types/Result';
import { useNotificationStore } from '@/stores/notificationStore';

export class ClassScoreBoardService {
  private api: ClassScoreBoardApi;

  constructor(apiInstance: ClassScoreBoardApi) {
    this.api = apiInstance;
  }

  async getByClassId(classId: string): Promise<Result<ClassScoreSummaryModel[]>> {
    return await this.api.getClassScoreBoardByClassId(classId);
  }

  async getScoresByClassId(classId: string) {
    return await this.api.getClassScoresByClassId(classId);
  }

  async updateScoreBoard(model: UpdateClassScoreBoardModel): Promise<Result<any>> {
    const result: any = await this.api.updateClassScoreBoard(model);
    if (!result.succeeded) {
      throw new Error(result.error || 'Update failed');
    }
    //useNotificationStore().showToast('success', { key: result.message || 'toast.updateSuccess' });
    return result;
  }
}
