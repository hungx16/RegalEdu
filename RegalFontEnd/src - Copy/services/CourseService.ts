// src/services/CourseService.ts
import type { CourseApi, CourseModel, CourseQuery } from '@/api/CourseApi';
import type { Result } from '@/types/Result';

export class CourseService {
  private api: CourseApi;

  constructor(apiInstance: CourseApi) {
    this.api = apiInstance;
  }

  async fetchPagedCourses(query: CourseQuery): Promise<Result<any>> {
    return await this.api.getPagedCourses(query);
  }

  async fetchAllCourses(): Promise<Result<any>> {
    return await this.api.getAllCourses();
  }

  async saveCourse(model: Partial<CourseModel>): Promise<any> {
    let result: any;
    const courseModel = (model as any).courseModel ?? model;
    if (courseModel.id) result = await this.api.updateCourse(courseModel);
    else result = await this.api.addCourse(courseModel);

    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }

  async deleteCourses(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteCourses(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
  }

  async restoreCourses(ids: string[]): Promise<void> {
    const result: any = await this.api.restoreCourses(ids);
    if (!result.succeeded) throw new Error(result.error || 'Restore failed');
  }

  async getCourseById(id: string): Promise<CourseModel> {
    const result: any = await this.api.getCourseById(id);
    if (!result.succeeded) throw new Error(result.error || 'Fetch failed');
    return result.data;
  }

  async getDeletedCourses(): Promise<CourseModel[]> {
    const result: any = await this.api.getDeletedCourses();
    if (!result.succeeded) throw new Error(result.error || 'Fetch failed');
    return result.data;
  }
}
