// src/stores/employeeStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { EmployeeModel, EmployeeQuery } from '@/api/EmployeeApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useEmployeeStore = defineStore('employee', {
  state: () => ({
    employees: [] as EmployeeModel[],
    total: 0,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {},
    } as EmployeeQuery,
    selectedEmployee: null as EmployeeModel | null,
    isRegionManager: false,
    isCompanyManager: false,
    isAdmissionEmployee: false,
    isMarketingEmployee: false,
    isAcademicAffairsEmployee: false,
  }),
  actions: {
    async fetchEmployees() {
      const employeeService = serviceFactory.employeeService;
      try {
        const result = await employeeService.fetchPagedEmployees(this.query);
        if (result?.succeeded === true) {
          this.employees = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching employees:', error);
      }
    },
    async getEmployeeByIdOrEmail(id?: string | null, email?: string | null) {
      const employeeService = serviceFactory.employeeService;
      try {
        const result = await employeeService.getEmployeeByIdOrEmail(id, email);
        if (result?.succeeded === true) {
          this.selectedEmployee = result.data;
        }
      } catch (error) {
        console.error('Error fetching employee by ID or email:', error);
      }
    },
    async fetchAllEmployees() {
      const employeeService = serviceFactory.employeeService;
      try {
        const result = await employeeService.fetchAllEmployees();
        if (result?.succeeded === true) {
          this.employees = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching employees:', error);
      }
    },
    selectEmployee(employee: EmployeeModel | null) {
      this.selectedEmployee = employee;
    },
    async saveEmployee(employee: Partial<EmployeeModel>) {
      const employeeService = serviceFactory.employeeService;
      await employeeService.saveEmployee(employee);
    },
    async updateProfile(employee: Partial<EmployeeModel>) {
      const employeeService = serviceFactory.employeeService;
      await employeeService.updateProfile(employee);
    },
    async deleteEmployees(employeeIds: string[]) {
      const employeeService = serviceFactory.employeeService;
      await employeeService.deleteEmployees(employeeIds);
    },
    async restoreEmployees(employeeIds: string[]) {
      const employeeService = serviceFactory.employeeService;
      await employeeService.restoreEmployees(employeeIds);
    },
    async setPage(page: number) {
      this.query.page = page;
    },
    async setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
    },
    async setFilter(filter) {
      this.query = { ...this.query, ...filter };
      this.query.page = 1;
    },
    async fetchDeletedEmployees() {
      try {
        const result = await serviceFactory.employeeService.fetchDeletedEmployees();
        if (result?.succeeded === true) {
          this.employees = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching deleted employees:', error);
      }
    },
    async checkIsRegionManager() {
      try {
        const result = await serviceFactory.employeeService.checkIsRegionManager();
        if (result?.succeeded === true) {
          this.isRegionManager = !!result.data;
          return this.isRegionManager;
        }
      } catch (error) {
        console.error('Error checking region manager status:', error);
      }
      this.isRegionManager = false;
      return false;
    },
    async checkIsCompanyManager() {
      try {
        const result = await serviceFactory.employeeService.checkIsCompanyManager();
        if (result?.succeeded === true) {
          this.isCompanyManager = !!result.data;
          return this.isCompanyManager;
        }
      } catch (error) {
        console.error('Error checking company manager status:', error);
      }
      this.isCompanyManager = false;
      return false;
    },
    async checkIsAdmissionEmployee() {
      try {
        const result = await serviceFactory.employeeService.checkIsAdmissionEmployee();
        if (result?.succeeded === true) {
          this.isAdmissionEmployee = !!result.data;
          return this.isAdmissionEmployee;
        }
      } catch (error) {
        console.error('Error checking admission employee status:', error);
      }
      this.isAdmissionEmployee = false;
      return false;
    },
    async checkIsMarketingEmployee() {
      try {
        const result = await serviceFactory.employeeService.checkIsMarketingEmployee();
        if (result?.succeeded === true) {
          this.isMarketingEmployee = !!result.data;
          return this.isMarketingEmployee;
        }
      } catch (error) {
        console.error('Error checking marketing employee status:', error);
      }
      this.isMarketingEmployee = false;
      return false;
    },
    async checkIsAcademicAffairsEmployee() {
      try {
        const result = await serviceFactory.employeeService.checkIsAcademicAffairsEmployee();
        if (result?.succeeded === true) {
          this.isAcademicAffairsEmployee = !!result.data;
          return this.isAcademicAffairsEmployee;
        }
      } catch (error) {
        console.error('Error checking academic affairs employee status:', error);
      }
      this.isAcademicAffairsEmployee = false;
      return false;
    }
  },
});
