import type { TeacherModel, TeacherQuery } from '@/api/TeacherApi';
import type { TeacherApi } from '@/api/TeacherApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class TeacherService {
  private api: TeacherApi;
  constructor(apiInstance: TeacherApi) {
    this.api = apiInstance;
  }

  async isCurrentUserTeacher(): Promise<Result<boolean>> {
    return this.api.isCurrentUserTeacher();
  }

  fetchPagedTeacher(query: TeacherQuery): Promise<Result<any>> {
    return this.api.getPagedTeacher(query);
  }

  fetchAllTeacher(): Promise<Result<any>> {
    return this.api.getAllTeachers();
  }

  async saveTeacher(model: Partial<TeacherModel>): Promise<any> {
    let result: any;


    try {
      if (model.id) {
        result = await this.api.updateTeacher(model);
      } else {
        result = await this.api.addTeacher(model);
      }

      console.log('Save result:', result);

      if (!result.succeeded) {
        // Hiển thị lỗi chi tiết từ backend
        const errorDetails = result.errors ? result.errors.join(', ') : result.message || result.error || 'Save failed';
        console.error('Backend error details:', errorDetails);
        throw new Error(errorDetails);
      }
      return result.data;
    } catch (error: any) {
      console.error('Error in saveTeacher:', error);
      // Hiển thị lỗi chi tiết cho người dùng
      const errorMessage = error.message || 'Lỗi không xác định khi lưu giáo viên';
      throw new Error(errorMessage);
    }
  }

  async deleteTeacher(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteTeacher(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
    else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreTeacher(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreTeacher(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }

  async fetchDeletedTeacher(): Promise<Result<any>> {
    return await this.api.getDeletedTeacher();
  }
}
