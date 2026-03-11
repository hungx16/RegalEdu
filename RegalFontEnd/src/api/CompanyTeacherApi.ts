// src/api/CompanyTeacherApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface CompanyTeacherModel extends BaseEntityModel {
  companyId: string;
  teacherId: string;
  companyCenter: number; // 0: chi nhánh chính, 1: chi nhánh phụ
  company?: any;
  teacher?: any;
}

export interface TeacherCompanyModel {
  teacherId: string;
  companies: CompanyTeacherModel[];
}

export interface UpdateTeacherCompaniesRequest {
  teacherId: string;
  companyTeachers: Partial<CompanyTeacherModel>[];
}

export class CompanyTeacherApi extends ApiClient {
  controller = 'CompanyTeacher';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  // Lấy tất cả chi nhánh của giáo viên
  public async getTeacherCompanies(teacherId: string): Promise<Result<CompanyTeacherModel[]>> {
    return await this.get<Result<CompanyTeacherModel[]>>(`/${this.controller}/GetTeacherCompanies`, { 
      params: { teacherId } 
    });
  }

  // Lấy chi nhánh chính của giáo viên
  public async getMainCompany(teacherId: string): Promise<Result<CompanyTeacherModel>> {
    return await this.get<Result<CompanyTeacherModel>>(`/${this.controller}/GetMainCompany`, { 
      params: { teacherId } 
    });
  }

  // Cập nhật chi nhánh cho giáo viên
  public async updateTeacherCompanies(data: UpdateTeacherCompaniesRequest): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateTeacherCompanies`, data);
  }

  // Thêm chi nhánh cho giáo viên
  public async addCompanyToTeacher(data: Partial<CompanyTeacherModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddCompanyToTeacher`, data);
  }

  // Xóa chi nhánh khỏi giáo viên
  public async removeCompanyFromTeacher(companyTeacherId: string): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RemoveCompanyFromTeacher`, { 
      params: { id: companyTeacherId } 
    });
  }

  // Lấy tất cả giáo viên của chi nhánh
  public async getCompanyTeachers(companyId: string): Promise<Result<CompanyTeacherModel[]>> {
    return await this.get<Result<CompanyTeacherModel[]>>(`/${this.controller}/GetCompanyTeachers`, { 
      params: { companyId } 
    });
  }
}