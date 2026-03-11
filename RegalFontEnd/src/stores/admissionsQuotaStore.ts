// src/stores/admissionsQuotaStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { AdmissionsQuotaAdjustmentModel, AdmissionsQuotaModel, AdmissionsQuotaQuery, TransferRolesRequest } from '@/api/AdmissionsQuotaApi';
import type { AdmissionsQuotaEmployeeModel } from '@/api/AdmissionsQuotaCompanyApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useAdmissionsQuotaStore = defineStore('admissionsQuota', {
  state: () => ({
    quotas: [] as AdmissionsQuotaModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        quotaName: '',
        academicYear: '',
        status: undefined,
        isDeleted: false,
      },
    } as AdmissionsQuotaQuery,
    selectedQuota: null as AdmissionsQuotaModel | null,
  }),
  actions: {
    async fetchPagedAdmissionsQuotas() {
      const service = serviceFactory.admissionsQuotaService;
      this.loading = true;
      try {
        const result = await service.fetchPagedAdmissionsQuotas(this.query);
        if (result?.succeeded) {
          this.quotas = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching quotas:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllAdmissionsQuotas() {
      const service = serviceFactory.admissionsQuotaService;
      this.loading = true;
      try {
        const result = await service.fetchAllAdmissionsQuotas();
        if (result?.succeeded) {
          this.quotas = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching all quotas:', error);
      } finally {
        this.loading = false;
      }
    },
    selectQuota(quota: AdmissionsQuotaModel | null) {
      this.selectedQuota = quota;
    },
    async saveQuota(quota: Partial<AdmissionsQuotaModel>) {
      await serviceFactory.admissionsQuotaService.saveAdmissionsQuota(quota);
    },
    async addAdmissionsQuotaAdjustment(quota: Partial<AdmissionsQuotaAdjustmentModel>) {
      await serviceFactory.admissionsQuotaService.addAdmissionsQuotaAdjustment(quota);
    },
    async assignSupportStaff(quota: Partial<AdmissionsQuotaEmployeeModel>) {
      await serviceFactory.admissionsQuotaService.assignSupportStaff(quota);
    },
    async transferRoles(data: TransferRolesRequest) {
      await serviceFactory.admissionsQuotaService.transferRoles(data);
    },
    async deleteQuotas(ids: string[]) {
      await serviceFactory.admissionsQuotaService.deleteAdmissionsQuotas(ids);
    },
    async restoreQuotas(ids: string[]) {
      await serviceFactory.admissionsQuotaService.restoreAdmissionsQuotas(ids);
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
  },
});
