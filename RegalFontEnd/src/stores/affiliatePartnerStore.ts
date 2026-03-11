// src/stores/affiliatePartnerStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { AffiliatePartnerModel, AffiliatePartnerQuery, EventReportPublicationItemModel } from '@/api/AffiliatePartnerApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useAffiliatePartnerStore = defineStore('affiliatePartner', {
  state: () => ({
    affiliatePartners: [] as AffiliatePartnerModel[],
    totalDepartments: 0,
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        affiliatePartnerCode: '',
        affiliatePartnerName: '',
        status: undefined,
        isDeleted: false,
      },
    } as AffiliatePartnerQuery,
    selectedAffiliatePartner: null as AffiliatePartnerModel | null,
    reportPublications: [] as EventReportPublicationItemModel[],
    reportPublicationsLoading: false,
  }),
  actions: {
    async fetchPagedAffiliatePartners() {
      const affiliatePartnerService = serviceFactory.affiliatePartnerService;
      this.loading = true;
      try {
        const result = await affiliatePartnerService.fetchPagedAffiliatePartners(this.query);
        if (result?.succeeded === true) {
          this.affiliatePartners = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching affiliatePartners:', error);
      } finally {
        this.loading = false;
      }
    },

    async fetchAllAffiliatePartners() {
      const affiliatePartnerService = serviceFactory.affiliatePartnerService;
      this.loading = true;
      try {
        const result = await affiliatePartnerService.fetchAllAffiliatePartners();
        if (result?.succeeded === true) {
          this.affiliatePartners = result.data;
          this.total = result.data.length;
          this.totalDepartments = result.data.reduce((acc, affiliatePartner) => acc + (affiliatePartner.image?.length || 0), 0);
        }
      } catch (error) {
        console.error('Error fetching affiliatePartners:', error);
      } finally {
        this.loading = false;
      }
    },

    selectAffiliatePartner(affiliatePartner: AffiliatePartnerModel | null) {
      this.selectedAffiliatePartner = affiliatePartner;
    },

    async saveAffiliatePartner(affiliatePartner: Partial<AffiliatePartnerModel>) {
      const affiliatePartnerService = serviceFactory.affiliatePartnerService;
      await affiliatePartnerService.saveAffiliatePartner(affiliatePartner);
    },

    async deleteAffiliatePartners(affiliatePartnerIds: string[]) {
      const affiliatePartnerService = serviceFactory.affiliatePartnerService;
      await affiliatePartnerService.deleteAffiliatePartners(affiliatePartnerIds);
    },

    async restoreAffiliatePartners(affiliatePartnerIds: string[]) {
      const affiliatePartnerService = serviceFactory.affiliatePartnerService;
      await affiliatePartnerService.restoreAffiliatePartners(affiliatePartnerIds);
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

    async fetchDeletedPositions() {
      const affiliatePartnerService = serviceFactory.affiliatePartnerService;
      this.loading = true;
      try {
        const result = await affiliatePartnerService.fetchDeletedPositions();
        if (result?.succeeded === true) {
          this.affiliatePartners = result.data;
          this.total = result.data.length;
          this.totalDepartments = result.data.reduce((acc, affiliatePartner) => acc + (affiliatePartner.image?.length || 0), 0);
        }
      } catch (error) {
        console.error('Error fetching affiliatePartners:', error);
      } finally {
        this.loading = false;
      }
    },

    async fetchReportPublications(affiliatePartnerId: string) {
      const affiliatePartnerService = serviceFactory.affiliatePartnerService;
      this.reportPublicationsLoading = true;
      try {
        const result = await affiliatePartnerService.fetchEventReportPublicationsByAffiliatePartnerId(affiliatePartnerId);
        if (result?.succeeded === true) {
          this.reportPublications = result.data ?? [];
        } else {
          this.reportPublications = [];
        }
      } catch (error) {
        console.error('Error fetching report publications:', error);
        this.reportPublications = [];
      } finally {
        this.reportPublicationsLoading = false;
      }
    },

    clearReportPublications() {
      this.reportPublications = [];
    },
  },
});
