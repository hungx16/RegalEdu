import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { WorkType } from '@/types';

export interface TeacherModel extends BaseEntityModel {
  teacherNickname?: string | null;
  teacherQualifications?: string | null;
  teacherSpecialization?: string | null;
  workType: WorkType;
  joinDate?: string | null;
  preferLevel?: string | null;
  teachingOutside: boolean;
  teacherAssistant: boolean;
  isOnline: boolean;
  applicationUserId?: string | null;
  applicationUser?: any | null;
  degreeId?: string | null;
  companyId?: string | null;
  subCompanyIds?: string | null;
}

export interface TeacherQuery {
  page: number;
  pageSize: number;
  filters?: {
    teacherCode?: string;
    teacherName?: string;
  };
}

export interface TeacherPagedResult {
  items: TeacherModel[];
  total: number;
}

export class TeacherApi extends ApiClient {
  controller = 'Teacher';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async getPagedTeacher(query: TeacherQuery): Promise<Result<TeacherPagedResult>> {
    return this.get(`/${this.controller}/GetPagedTeachers`, { params: query }); // Sửa thành GetPagedTeachers
  }

  public async getAllTeachers(): Promise<Result<TeacherModel[]>> {
    return await this.get(`/${this.controller}/GetAllTeachers`);
  }

  public async addTeacher(data: Partial<TeacherModel>): Promise<Result<any>> {
    return await this.post(`/${this.controller}/AddTeacher`, data);
  }

  public async updateTeacher(data: Partial<TeacherModel>): Promise<Result<any>> {
    return await this.put(`/${this.controller}/UpdateTeacher`, data);
  }

  public async deleteTeacher(ids: string[]): Promise<Result<any>> {
    return await this.delete(`/${this.controller}/DeleteListTeacher`, { data: ids });
  }

  public async restoreTeacher(ids: string[]): Promise<Result<any>> {
    return await this.delete(`/${this.controller}/RestoreListTeacher`, { data: ids });
  }

  public async getTeacherById(id: string): Promise<Result<TeacherModel>> {
    return await this.get(`/${this.controller}/GetTeacherById`, { params: { id } });
  }

  public async getDeletedTeacher(): Promise<Result<TeacherModel[]>> {
    return await this.get(`/${this.controller}/GetDeletedTeachers`); // Sửa thành GetDeletedTeachers
  }

  public async isCurrentUserTeacher(): Promise<Result<boolean>> {
    return await this.get(`/${this.controller}/IsCurrentUserTeacher`);
  }
}
