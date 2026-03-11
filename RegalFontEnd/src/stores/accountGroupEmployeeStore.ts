import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { AccountGroupEmployeeModel, AccountGroupEmployeeRequestModel } from '@/api/accountGroupEmployeeApi';

export const useAccountGroupEmployeeStore = defineStore('accountGroupEmployee', {
  state: () => ({
    accountGroupEmployees: [] as AccountGroupEmployeeModel[],
    loading: false,
    selectedEmployee: null as AccountGroupEmployeeModel | null,
    listEmpNoGroup: [] as string[],
  }),
  actions: {
    async fetchEmployees(groupId: string) {
      this.loading = true;
      try {
        const result = await serviceFactory.accountGroupEmployeeService.fetchByGroupId(groupId);
        if (result?.succeeded) {
          this.accountGroupEmployees = result.data;
        }
      } catch (err) {
        console.error('Error fetching employees:', err);
      } finally {
        this.loading = false;
      }
    },
    selectEmployee(employee: AccountGroupEmployeeModel | null) {
      this.selectedEmployee = employee;
    },
    async saveEmployee(employee: AccountGroupEmployeeModel) {
      await serviceFactory.accountGroupEmployeeService.saveEmployee(employee);
    },
    async deleteEmployee(employeeId: string) {
      this.accountGroupEmployees = this.accountGroupEmployees.filter(e => e.id !== employeeId);
    },
    async fetchEmployeeNoGroup() {
      try {
        const result = await serviceFactory.accountGroupEmployeeService.getEmployeeNoGroup();
        if (result?.succeeded) {
          this.listEmpNoGroup = result.data;
        }
      } catch (err) {
        console.error('Error fetching employees without group:', err);
      }
      return [];
    },
    async fetchAccountGroupEmployeeByGroupId(groupId: any) {
      try {
        const result = await serviceFactory.accountGroupEmployeeService.getAccountGroupEmployeeByGroupId(groupId);
        if (result?.succeeded) {
          this.accountGroupEmployees = result.data;
        }
      } catch (err) {
        console.error('Error fetching account group employees by group ID:', err);
      }
    },
    async addAccountGroupEmployee(model: AccountGroupEmployeeRequestModel) {
      try {
        const result = await serviceFactory.accountGroupEmployeeService.addAccountGroupEmployee(model);
        if (result?.succeeded) {
          this.accountGroupEmployees.push(result.data);
        }
      } catch (err) {
        console.error('Error adding account group employee:', err);
      }
    },
    async saveAccountGroupEmployee(model: AccountGroupEmployeeRequestModel) {
      try {
        const result = await serviceFactory.accountGroupEmployeeService.saveAccountGroupEmployee(model);
        if (result?.succeeded) {
          // Update or add each employee in the listuserCode array
          if (Array.isArray(model.listuserCode)) {
            model.listuserCode.forEach((userCode: string) => {
              const index = this.accountGroupEmployees.findIndex(e => e.userCode === userCode);
              if (index !== -1) {
                this.accountGroupEmployees[index] = result.data;
              } else {
                this.accountGroupEmployees.push(result.data);
              }
            });
          }
        }
      } catch (err) {
        console.error('Error saving account group employee:', err);
      }
    }

  }
});
