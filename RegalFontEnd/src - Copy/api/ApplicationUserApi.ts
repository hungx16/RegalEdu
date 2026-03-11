// src/api/ApplicationUserApi.ts
import { ApiClient } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { TeacherModel } from './TeacherApi';
import type { EmployeeModel } from './EmployeeApi';

export interface ApplicationUserModel {
  id?: string | null;
  fullName: string | null;
  userCode: string | null;
  userName: string | null;
  email: string | null;
  gender: boolean;
  avatar: Uint8Array | null; // hoặc string nếu bạn muốn truyền base64
  avatarString: string | null;
  password: string | null;
  passwordConfirm: string | null;
  isDeleted: boolean;
  createdAt: string | null; // DateTime → string ISO
  lastModified: string | null;
  createdBy: string | null;
  lastModifiedBy: string | null;
  genderText: string | null;
  phoneNumber: string | null;
  dateOfBirth: string | null; // DateTime → string ISO
  provinceCode: string | null;
  teacher: TeacherModel | null;
  employee: EmployeeModel | null;
  identityNumber: string | null;
}

export interface ApplicationUserQuery {
  page: number;
  pageSize: number;
  filters?: {
    fullName?: string;
    userName?: string;
    email?: string;
    phoneNumber?: string;
    isDeleted?: boolean;
  };
}

export interface ApplicationUserPagedResult {
  items: ApplicationUserModel[];
  total: number;
}

export class ApplicationUserApi extends ApiClient {

  /**
   * Khởi tạo API client cho ApplicationUserApi.
   * Sử dụng URL từ biến môi trường VITE_APP_API_URL.
   */
  controller: string = 'user';
  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async GetPagedApplicationUsers(query: ApplicationUserQuery): Promise<Result<ApplicationUserPagedResult>> {
    const res = await this.get<Result<ApplicationUserPagedResult>>(`/${this.controller}/GetPagedApplicationUsers`, { params: query });
    return res;
  }
  public async GetAllApplicationUsers(): Promise<Result<ApplicationUserModel[]>> {
    const res = await this.get<Result<ApplicationUserModel[]>>(`/${this.controller}/GetAllApplicationUsers`);
    return res;
  }
  public async addApplicationUser(data: Partial<ApplicationUserModel>): Promise<Result<any>> {
    const res = await this.post<Result<any>>(`/${this.controller}/AddApplicationUser`, data);
    return res; // Đúng, vì res đã là Result<any>
  }

  public async updateApplicationUser(data: Partial<ApplicationUserModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateApplicationUser/`, data);
  }

  public async deleteListUser(id: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListUser`, { data: id });
  }
}
