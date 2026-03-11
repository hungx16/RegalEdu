// src/api/EmployeeApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

// ==== INTERFACE EmployeeModel ====
export interface EmployeeModel extends BaseEntityModel {
  applicationUserId?: string | null;
  applicationUser?: any | null;

  companyId: string;
  company?: any | null;

  positionId: string;
  position?: any | null;

  departmentId: string;
  department?: any | null;

  employeeTax?: string | null;
  employeeStartedDate?: string | null;
  employeeEndDate?: string | null;
  employeeNewEndDate?: string | null;
  personalEmail?: string | null;
  regions?: any[];      // List<RegionModel>
  companies?: any[];    // List<CompanyModel>
}

// ==== INTERFACE EmployeeQuery & Result ====
export interface EmployeeQuery {
  page: number;
  pageSize: number;
  filters?: {
    companyId?: string;
    departmentId?: string;
    positionId?: string;
    isSupport?: boolean;
    keyword?: string;
    isDeleted?: boolean;
  };
}

export interface EmployeePagedResult {
  items: EmployeeModel[];
  total: number;
}

// ==== CLASS EmployeeApi ====
export class EmployeeApi extends ApiClient {
  controller = 'employee';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedEmployees(query: EmployeeQuery): Promise<Result<EmployeePagedResult>> {
    return await this.get<Result<EmployeePagedResult>>(`/${this.controller}/GetPagedEmployees`, { params: query });
  }

  public async getAllEmployees(): Promise<Result<EmployeeModel[]>> {
    return await this.get<Result<EmployeeModel[]>>(`/${this.controller}/GetAllEmployees`);
  }

  public async addEmployee(data: Partial<EmployeeModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddEmployee`, data);
  }

  public async updateEmployee(data: Partial<EmployeeModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateEmployee`, data);
  }

  public async deleteEmployees(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListEmployee`, { data: ids });
  }

  public async restoreEmployees(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListEmployee`, { data: ids });
  }

  public async getEmployeeByIdOrEmail(id?: string | null, email?: string | null): Promise<Result<EmployeeModel>> {
    const params: Record<string, string> = {}
    if (id && id.trim()) params.id = id.trim()
    if (email && email.trim()) params.email = email.trim()
    return await this.get<Result<EmployeeModel>>(`/${this.controller}/GetEmployeeByIdOrEmail`, { params });
  }

  public async getDeletedEmployees(): Promise<Result<EmployeeModel[]>> {
    return await this.get<Result<EmployeeModel[]>>(`/${this.controller}/GetDeletedEmployees`);
  }
  public async isRegionManager(): Promise<Result<any>> {
    return await this.get<Result<any>>(`/${this.controller}/IsRegionManager`);
  }
  public async isCompanyManager(): Promise<Result<any>> {
    return await this.get<Result<any>>(`/${this.controller}/IsCompanyManager`);
  }
  public async isAdmissionEmployee(): Promise<Result<any>> {
    return await this.get<Result<any>>(`/${this.controller}/IsAdmissionEmployee`);
  }
  public async isMarketingEmployee(): Promise<Result<any>> {
    return await this.get<Result<any>>(`/${this.controller}/IsMarketingEmployee`);
  }
  public async isAcademicAffairsEmployee(): Promise<Result<any>> {
    return await this.get<Result<any>>(`/${this.controller}/IsAcademicAffairsEmployee`);
  }
  public async updateProfile(data: Partial<EmployeeModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateProfile`, data);
  }

}
