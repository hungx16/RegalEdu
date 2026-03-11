// src/stores/applicationUserStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { ApplicationUserModel, ApplicationUserQuery } from '@/api/ApplicationUserApi';
const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useApplicationUserStore = defineStore('applicationUser', {
  state: () => ({
    users: [] as ApplicationUserModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        fullName: '',
        userName: '',
        email: '',
        phoneNumber: '',
        isDeleted: false,
      },
    } as ApplicationUserQuery,
    selectedUser: null as ApplicationUserModel | null,
  }),
  actions: {
    async fetchApplicationUsers() {
      const applicationUserService = serviceFactory.applicationUserService;
      this.loading = true;
      try {
        const result = await applicationUserService.fetchPagedApplicationUser(this.query);
        if (result?.succeeded === true) {
          this.users = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching application users:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllApplicationUsers() {
      const applicationUserService = serviceFactory.applicationUserService;
      this.loading = true;
      try {
        const result = await applicationUserService.fetchAllApplicationUsers();
        if (result?.succeeded === true) {
          this.users = result.data;
          this.total = this.users.length; // Assuming total is the length of the fetched users
          console.log('Fetched all application users:', this.users);

        }
      } catch (error) {
        console.error('Error fetching application users:', error);
      } finally {
        this.loading = false;
      }
    },
    selectUser(user: ApplicationUserModel | null) {
      this.selectedUser = user;
    },
    async saveApplicationUser(user: Partial<ApplicationUserModel>) {
      const applicationUserService = serviceFactory.applicationUserService;
      await applicationUserService.saveApplicationUser(user);
      await this.fetchApplicationUsers();
    },
    async deleteApplicationUser(userIds: string[]) {
      const applicationUserService = serviceFactory.applicationUserService;
      await applicationUserService.deleteApplicationUser(userIds);
    },
    async setPage(page: number) {

      this.query.page = page;
      await this.fetchApplicationUsers();
    },
    async setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
      await this.fetchApplicationUsers();
    },
    async setFilter(filter) {
      this.query = { ...this.query, ...filter };
      this.query.page = 1;
      await this.fetchApplicationUsers();
    },
  },
});
