// src/stores/accountGroupStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { AccountGroupModel, AccountGroupQuery } from '@/api/AccountGroupApi';

export const useAccountGroupStore = defineStore('accountGroup', {
  state: () => ({
    groups: [] as AccountGroupModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: 10,
      filters: {
        name: '',
        enable: undefined,
        useDefault: undefined,
        createdAt: undefined,
      },
    } as AccountGroupQuery,
    selectedGroup: null as AccountGroupModel | null,
  }),
  actions: {
    async fetchAccountGroups() {
      const accountGroupService = serviceFactory.accountGroupService;
      this.loading = true;
      try {
        const result = await accountGroupService.fetchAccountGroups(this.query);
        if (result?.succeeded === true) {
          this.groups = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching account groups:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllAccountGroups() {
      const accountGroupService = serviceFactory.accountGroupService;
      this.loading = true;
      try {
        const result = await accountGroupService.fetchAllAccountGroups();
        if (result?.succeeded === true) {
          this.groups = result.data
        }
      } catch (error) {
        console.error('Error fetching account groups:', error);
      } finally {
        this.loading = false;
      }
    },
    selectGroup(group: AccountGroupModel | null) {
      this.selectedGroup = group;
    },
    async saveAccountGroup(group: Partial<AccountGroupModel>) {
      const accountGroupService = serviceFactory.accountGroupService;
      await accountGroupService.saveAccountGroup(group);
      await this.fetchAccountGroups();
    },
    async deleteAccountGroup(groupId: string) {
      const accountGroupService = serviceFactory.accountGroupService;
      await accountGroupService.deleteAccountGroup(groupId);
    },
    async setPage(page: number) {
      this.query.page = page;
      await this.fetchAccountGroups();
    },
    async setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
      await this.fetchAccountGroups();
    },
    async setFilter(filter) {
      this.query = { ...this.query, ...filter };
      this.query.page = 1;
      await this.fetchAccountGroups();
    },
  },
});
