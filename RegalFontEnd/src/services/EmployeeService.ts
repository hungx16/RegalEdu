// src/services/EmployeeService.ts
import type { EmployeeModel, EmployeeQuery } from '@/api/EmployeeApi';
import type { EmployeeApi } from '@/api/EmployeeApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class EmployeeService {
  private employeeApi: EmployeeApi;

  constructor(employeeApiInstance: EmployeeApi) {
    this.employeeApi = employeeApiInstance;
  }

  async fetchPagedEmployees(query: EmployeeQuery): Promise<Result<any>> {
    return await this.employeeApi.getPagedEmployees(query);
  }
  async getEmployeeByIdOrEmail(id?: string | null, email?: string | null): Promise<Result<any>> {
    return await this.employeeApi.getEmployeeByIdOrEmail(id, email);
  }
  async fetchAllEmployees(): Promise<Result<any>> {
    return await this.employeeApi.getAllEmployees();
  }

  async saveEmployee(employee: Partial<EmployeeModel>): Promise<any> {
    let result: any;
    if (employee.id) {
      employee.company = undefined; // Clear company to avoid sending it if not needed
      employee.position = undefined; // Clear position to avoid sending it if not needed
      employee.department = undefined; // Clear department to avoid sending it if not needed
      employee.regions = undefined; // Clear regions to avoid sending it if not needed
      employee.companies = undefined; // Clear companies to avoid sending it if not needed
      result = await this.employeeApi.updateEmployee(employee);
    } else {
      result = await this.employeeApi.addEmployee(employee);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }
  async updateProfile(employee: Partial<EmployeeModel>): Promise<any> {
    let result: any;
    if (employee.id) {
      employee.company = undefined; // Clear company to avoid sending it if not needed
      employee.position = undefined; // Clear position to avoid sending it if not needed
      employee.department = undefined; // Clear department to avoid sending it if not needed
      employee.regions = undefined; // Clear regions to avoid sending it if not needed
      employee.companies = undefined; // Clear companies to avoid sending it if not needed
      result = await this.employeeApi.updateProfile(employee);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }
  async deleteEmployees(employeeIds: string[]): Promise<void> {
    let result: any = await this.employeeApi.deleteEmployees(employeeIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreEmployees(employeeIds: string[]): Promise<void> {
    let result: any = await this.employeeApi.restoreEmployees(employeeIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Restore failed');
    }
  }
  async fetchDeletedEmployees(): Promise<Result<any>> {
    return await this.employeeApi.getDeletedEmployees();
  }
  async checkIsRegionManager(): Promise<Result<any>> {
    return await this.employeeApi.isRegionManager();
  }
  async checkIsCompanyManager(): Promise<Result<any>> {
    return await this.employeeApi.isCompanyManager();
  }
  async checkIsAdmissionEmployee(): Promise<Result<any>> {
    return await this.employeeApi.isAdmissionEmployee();
  }
  async checkIsMarketingEmployee(): Promise<Result<any>> {
    return await this.employeeApi.isMarketingEmployee();
  }
  async checkIsAcademicAffairsEmployee(): Promise<Result<any>> {
    return await this.employeeApi.isAcademicAffairsEmployee();
  }
}
