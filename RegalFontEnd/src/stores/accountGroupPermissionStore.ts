import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { AccountGroupPermissionModel, AccountGroupPermissionRequestModel, FormPermissionDTO } from '@/api/accountGroupPermissionApi';
const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useAccountGroupPermissionStore = defineStore('accountGroupPermission', {
  state: () => ({
    permissions: [] as AccountGroupPermissionModel[],
    listFormPermissionArray: [] as FormPermissionDTO[],
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        formName: '',
        action: ''
      }
    },
    selectedPermission: null as AccountGroupPermissionModel | null,
  }),
  actions: {
    async fetchAccountGroupPermissions(accountGroupId?: string) {
      this.loading = true;
      try {
        const result = await serviceFactory.accountGroupPermissionService.fetchPermissions(accountGroupId);
        console.log("Fetched account group permissions:", result);

        if (result?.succeeded === true) {
          this.permissions = result.data ?? [];
        }
      } finally {
        this.loading = false;
      }
    },
    async getAccountGroupPermissionByAccountGroupId(accountGroupId: string) {
      this.loading = true;
      try {
        const result = await serviceFactory.accountGroupPermissionService.getPermissions(accountGroupId);
        if (result?.succeeded === true) {
          this.permissions = result.data ?? [];
        }
      } catch (error) {
        console.error('Error fetching account group permissions:', error);
      }
      this.loading = false;
    },
    selectPermission(permission: AccountGroupPermissionModel | null) {
      this.selectedPermission = permission;
    },
    async saveAccountGroupPermission(permission: Partial<AccountGroupPermissionRequestModel>) {
      await serviceFactory.accountGroupPermissionService.saveAccountGroupPermission(permission);
    },
    setFilter(filter) {
      this.query = { ...this.query, ...filter };
    },
    setPage(page: number) {
      this.query.page = page;
    },
    setPageSize(size: number) {
      this.query.pageSize = size;
    },
    async getMenuAccept() {
      this.loading = true;
      try {
        const result = await serviceFactory.accountGroupPermissionService.getMenuAccept();
        if (result?.succeeded === true) {
          return result.data;
        }
        throw new Error(result.error || 'Failed to fetch menu accept');
      } finally {
        this.loading = false;
      }
    },
    async getMenuAndPermissionAccept() {
      this.loading = true;
      try {
        const result = await serviceFactory.accountGroupPermissionService.getMenuAndPermissionAccept();
        if (result?.succeeded === true) {
          this.listFormPermissionArray = result.data || [];
          return result.data;
        }
      } catch (error) {
        console.error('Error fetching menu and permission accept:', error);
      } finally {
        this.loading = false;
      }
    }
  }
});
