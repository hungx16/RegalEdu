// src/stores/partnerTypeStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { PartnerTypeModel, PartnerTypeQuery } from '@/api/PartnerTypeApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const usePartnerTypeStore = defineStore('partnerType', {
  state: () => ({
    partnerTypes: [] as PartnerTypeModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        partnerTypeCode: '',
        partnerTypeName: '',
        status: undefined,
        isDeleted: false,
      },
    } as PartnerTypeQuery,
    selectedPartnerType: null as PartnerTypeModel | null,
    selectedIds: [] as string[],
  }),
  getters: {
    getDisableDelete(state) {
      return state.selectedIds.length === 0;
    },
  },
  actions: {
    async fetchPagedPartnerTypes() {
      const svc = serviceFactory.partnerTypeService;
      this.loading = true;
      try {
        const result = await svc.fetchPagedPartnerTypes(this.query);
        if (result?.succeeded === true) {
          this.partnerTypes = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching partner types:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllPartnerTypes() {
      const svc = serviceFactory.partnerTypeService;
      this.loading = true;
      try {
        const result = await svc.fetchAllPartnerTypes();
        if (result?.succeeded === true) {
          this.partnerTypes = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching partner types:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchDeletedPartnerTypes() {
      const svc = serviceFactory.partnerTypeService;
      this.loading = true;
      try {
        const result = await svc.fetchDeletedPartnerTypes();
        if (result?.succeeded === true) {
          this.partnerTypes = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching partner types:', error);
      } finally {
        this.loading = false;
      }
    },
    selectPartnerType(item: PartnerTypeModel | null) {
      this.selectedPartnerType = item;
    },
    async savePartnerType(item: Partial<PartnerTypeModel>) {
      const svc = serviceFactory.partnerTypeService;
      await svc.savePartnerType(item);
    },
    async deletePartnerTypes(ids: string[]) {
      const svc = serviceFactory.partnerTypeService;
      await svc.deletePartnerTypes(ids);
    },
    setPage(page: number) {
      this.query.page = page;
    },
    setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
    },
    setFilter(filter: Partial<PartnerTypeQuery>) {
      this.query = { ...this.query, ...filter };
    },
    setSelectedIds(ids: string[]) {
      this.selectedIds = ids;
    },
  },
});
