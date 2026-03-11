// src/services/CompanyTeacherService.ts
import type {
  CompanyTeacherModel,
  TeacherCompanyModel,
  UpdateTeacherCompaniesRequest
} from '@/api/CompanyTeacherApi';
import type { CompanyTeacherApi } from '@/api/CompanyTeacherApi';
import type { Result } from '@/types/Result';

export class CompanyTeacherService {
  private companyTeacherApi: CompanyTeacherApi;

  constructor(companyTeacherApiInstance: CompanyTeacherApi) {
    this.companyTeacherApi = companyTeacherApiInstance;
  }

  // Lấy tất cả chi nhánh của giáo viên
  async getTeacherCompanies(teacherId: string): Promise<Result<CompanyTeacherModel[]>> {
    return await this.companyTeacherApi.getTeacherCompanies(teacherId);
  }

  // Lấy chi nhánh chính của giáo viên
  async getMainCompany(teacherId: string): Promise<Result<CompanyTeacherModel>> {
    return await this.companyTeacherApi.getMainCompany(teacherId);
  }

  // Cập nhật chi nhánh cho giáo viên
  async updateTeacherCompanies(teacherId: string, companies: Partial<CompanyTeacherModel>[]): Promise<any> {
    const request: UpdateTeacherCompaniesRequest = {
      teacherId,
      companyTeachers: companies
    };

    const result = await this.companyTeacherApi.updateTeacherCompanies(request);
    if (!result.succeeded) {
      throw new Error(result.errors || 'Update teacher companies failed');
    }
    return result.data;
  }

  // Thêm chi nhánh cho giáo viên
  async addCompanyToTeacher(data: Partial<CompanyTeacherModel>): Promise<any> {
    const result = await this.companyTeacherApi.addCompanyToTeacher(data);
    if (!result.succeeded) {
      throw new Error(result.errors || 'Add company to teacher failed');
    }
    return result.data;
  }

  // Xóa chi nhánh khỏi giáo viên
  async removeCompanyFromTeacher(companyTeacherId: string): Promise<void> {
    const result = await this.companyTeacherApi.removeCompanyFromTeacher(companyTeacherId);
    if (!result.succeeded) {
      throw new Error(result.errors || 'Remove company from teacher failed');
    }
  }

  // Lấy tất cả giáo viên của chi nhánh
  async getCompanyTeachers(companyId: string): Promise<Result<CompanyTeacherModel[]>> {
    return await this.companyTeacherApi.getCompanyTeachers(companyId);
  }
}