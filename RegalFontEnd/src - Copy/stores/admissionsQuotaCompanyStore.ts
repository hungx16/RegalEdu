// src/stores/admissionsQuotaCompanyStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { AdmissionsQuotaCompanyModel, AdmissionsQuotaCompanyQuery } from '@/api/AdmissionsQuotaCompanyApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useAdmissionsQuotaCompanyStore = defineStore('admissionsQuotaCompany', {
  state: () => ({
    companies: [] as AdmissionsQuotaCompanyModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        companyName: '',
        status: undefined,
        isDeleted: false,
      },
    } as AdmissionsQuotaCompanyQuery,
    selectedCompany: null as AdmissionsQuotaCompanyModel | null,
  }),
  actions: {
    async fetchCompanies() {
      const service = serviceFactory.admissionsQuotaCompanyService;
      this.loading = true;
      try {
        const result = await service.fetchPagedCompanies(this.query);
        if (result?.succeeded) {
          this.companies = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching companies:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllAdmissionsQuotaCompanies() {
      const service = serviceFactory.admissionsQuotaCompanyService;
      this.loading = true;
      try {
        const result = await service.fetchAllCompanies();
        if (result?.succeeded) {
          this.companies = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching all companies:', error);
      } finally {
        this.loading = false;
      }
    },
    selectCompany(company: AdmissionsQuotaCompanyModel | null) {
      this.selectedCompany = company;
    },
    async saveCompany(company: Partial<AdmissionsQuotaCompanyModel>) {
      await serviceFactory.admissionsQuotaCompanyService.saveCompany(company);
      await this.fetchCompanies();
    },
    async deleteCompanies(ids: string[]) {
      await serviceFactory.admissionsQuotaCompanyService.deleteCompanies(ids);
      await this.fetchCompanies();
    },
    async restoreCompanies(ids: string[]) {
      await serviceFactory.admissionsQuotaCompanyService.restoreCompanies(ids);
      await this.fetchCompanies();
    },
    async setPage(page: number) {
      this.query.page = page;
      await this.fetchCompanies();
    },
    async setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
      await this.fetchCompanies();
    },
    async setFilter(filter) {
      this.query = { ...this.query, ...filter };
      this.query.page = 1;
      await this.fetchCompanies();
    },
    async fetchAdmissionsQuotaCompanyById(id: string) {
      const service = serviceFactory.admissionsQuotaCompanyService;
      try {
        const result = await service.fetchAdmissionsQuotaCompanyById(id);
        if (result?.succeeded) {
          this.selectedCompany = result.data;
        }
      } catch (error) {
        console.error('Error fetching all companies:', error);
      }
    },
  },
});
