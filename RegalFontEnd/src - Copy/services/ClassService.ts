// src/services/ClassService.ts
import type { ClassApi, ClassModel, ClassQuery } from '@/api/ClassApi'
import type { ClassScheduleModel } from '@/api/ClassScheduleApi'
import { useNotificationStore } from '@/stores/notificationStore'
import type { Result } from '@/types/Result'


export class ClassService {
  private api: ClassApi;
  constructor(apiInstance: ClassApi) {
    this.api = apiInstance;
  }

  async fetchPaged(query: ClassQuery): Promise<Result<any>> {
    return await this.api.getPagedClass(query)
  }

  async fetchAll(): Promise<Result<ClassModel[]>> {
    return await this.api.getAllClass()
  }

  async add(data: Partial<ClassModel>): Promise<Result<any>> {
    return await this.api.addClass(data)
  }

  async update(data: Partial<ClassModel>): Promise<Result<any>> {
    return await this.api.updateClass(data)
  }

  async delete(ids: string[]): Promise<void> {
    let result: any = await this.api.deleteClasses(ids)
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async getById(id: string): Promise<Result<ClassModel>> {
    return await this.api.getClassById(id)
  }
  async getByTeacherId(id: string): Promise<Result<ClassModel[]>> {
    return await this.api.getClassByTeacherId(id)
  }

  async getSchedulesByHomeTeacher(): Promise<Result<ClassScheduleModel[]>> {
    return await this.api.getClassSchedulesByHomeTeacher()
  }
}
