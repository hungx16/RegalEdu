// src/stores/companyStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { CompanyModel, CompanyQuery, LogRegionComModel } from '@/api/CompanyApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useCompanyStore = defineStore('company', {
  state: () => ({
    companies: [] as CompanyModel[],
    LogRegionComs: [] as LogRegionComModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {},
    } as CompanyQuery,
    selectedCompany: null as CompanyModel | null,
  }),
  actions: {
    async fetchPagedCompanies() {
      const companyService = serviceFactory.companyService;
      this.loading = true;
      try {
        const result = await companyService.fetchPagedCompanies(this.query);
        if (result?.succeeded === true) {
          this.companies = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching companies:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllCompanies() {
      const companyService = serviceFactory.companyService;
      this.loading = true;
      try {
        const result = await companyService.fetchAllCompanies();
        if (result?.succeeded === true) {
          this.companies = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching companies:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllCompanyRegions() {
      const companyService = serviceFactory.companyService;
      this.loading = true;
      try {
        const result = await companyService.getAllCompanyRegions();
        if (result?.succeeded === true) {
          this.LogRegionComs = result.data;
        }
      } catch (error) {
        console.error('Error fetching companies:', error);
      } finally {
        this.loading = false;
      }
    },
    selectCompany(company: CompanyModel | null) {
      this.selectedCompany = company;
    },
    async saveCompany(company: Partial<CompanyModel>) {
      const companyService = serviceFactory.companyService;
      await companyService.saveCompany(company);
    },
    async deleteCompanies(companyIds: string[]) {
      const companyService = serviceFactory.companyService;
      await companyService.deleteCompanies(companyIds);
    },
    async restoreCompanies(companyIds: string[]) {
      const companyService = serviceFactory.companyService;
      await companyService.restoreCompanies(companyIds);
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
    async createLogRegionCom(data: Partial<LogRegionComModel>) {
      const companyService = serviceFactory.companyService;
      await companyService.createLogRegionCom(data);
    },
    async fetchDeletedCompanies() {
      const companyService = serviceFactory.companyService;
      this.loading = true;
      try {
        const result = await companyService.fetchDeletedCompanies();
        if (result?.succeeded === true) {
          this.companies = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching deleted companies:', error);
      } finally {
        this.loading = false;
      }
    },
  },
});
