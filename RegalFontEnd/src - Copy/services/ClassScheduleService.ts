import type { CancelClassScheduleByShiftingCommand, CancelClassScheduleWithSubstitutionCommand, ClassScheduleApi, ClassScheduleModel, UpdateSessionAttendanceLockStatusesCommand, SessionAttendanceLockStatus } from '@/api/ClassScheduleApi';
import type { Result } from '@/types/Result';
import { useNotificationStore } from '@/stores/notificationStore';

export class ClassScheduleService {
  private api: ClassScheduleApi;

  constructor(apiInstance: ClassScheduleApi) {
    this.api = apiInstance;
  }

  async getScheduleById(id: string): Promise<Result<ClassScheduleModel>> {
    return await this.api.getScheduleById(id);
  }

  async getSchedulesByClassId(classId: string): Promise<Result<ClassScheduleModel[]>> {
    return await this.api.getSchedulesByClassId(classId);
  }

  async updateSchedule(data: Partial<ClassScheduleModel>): Promise<Result<ClassScheduleModel>> {
    const result: any = await this.api.updateSchedule(data);
    if (!result.succeeded) {
      throw new Error(result.error || 'Update failed');
    }
    useNotificationStore().showToast('success', { key: result.message || 'toast.updateSuccess' });
    return result;
  }
  async updateSessionAttendanceLockStatus(sessionId: string, newStatus: SessionAttendanceLockStatus): Promise<Result<any>> {
    const result: any = await this.api.updateSessionAttendanceLockStatus(sessionId, newStatus);
    if (!result.succeeded) {
      throw new Error(result.error || 'Update failed');
    }
    return result;
  }
  async updateSessionAttendanceLockStatuses(command: UpdateSessionAttendanceLockStatusesCommand): Promise<Result<any>> {
    const result: any = await this.api.updateSessionAttendanceLockStatuses(command);
    if (!result.succeeded) {
      throw new Error(result.error || 'Update failed');
    }
    return result;
  }
  async cancelAndShift(data: CancelClassScheduleByShiftingCommand): Promise<Result<any>> {
    const result: any = await this.api.cancelAndShift(data);
    if (!result.succeeded) {
      throw new Error(result.error || 'Update failed');
    }
    useNotificationStore().showToast('success', { key: result.message || 'toast.updateSuccess' });
    return result;
  }
  async cancelAndSubstitute(data: CancelClassScheduleWithSubstitutionCommand): Promise<Result<any>> {
    const result: any = await this.api.cancelAndSubstitute(data);
    if (!result.succeeded) {
      throw new Error(result.error || 'Update failed');
    }
    useNotificationStore().showToast('success', { key: result.message || 'toast.updateSuccess' });
    return result;
  }
}
