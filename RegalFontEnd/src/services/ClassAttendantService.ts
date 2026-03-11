import type { ClassAttendantApi, ClassAttendantModel, UpdateClassAttendantCommand } from '@/api/ClassAttendantApi';
import type { Result } from '@/types/Result';
// import { useNotificationStore } from '@/stores/notificationStore';

export class ClassAttendantService {
  private api: ClassAttendantApi;

  constructor(apiInstance: ClassAttendantApi) {
    this.api = apiInstance;
  }

  async getByScheduleId(scheduleId: string): Promise<Result<ClassAttendantModel[]>> {
    return await this.api.getClassAttendantsByScheduleId(scheduleId);
  }

  async update(command: UpdateClassAttendantCommand): Promise<Result<ClassAttendantModel[]>> {
    const result: any = await this.api.updateClassAttendant(command);
    if (!result.succeeded) {
      throw new Error(result.error || 'Update failed');
    }
    // useNotificationStore().showToast('success', { key: result.message || 'toast.updateSuccess' });
    return result;
  }
}
