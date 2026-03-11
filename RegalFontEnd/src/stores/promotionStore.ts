// src/stores/promotionStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { PromotionModel, PromotionQuery } from '@/api/PromotionApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const usePromotionStore = defineStore('promotion', {
  state: () => ({
    promotions: [] as PromotionModel[],
    promotionGlobal: [] as PromotionModel[],
    promotionAvaliable: [] as PromotionModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        name: '',
        startDate: '',
        endDate: '',
      },
    } as PromotionQuery,
    selectedPromotion: null as PromotionModel | null,
  }),
  actions: {
    /**
     * Lấy danh sách khuyến mãi có phân trang từ API.
     */
    async fetchPagedPromotions() {
      const service = serviceFactory.promotionService;
      this.loading = true;
      try {
        const result = await service.fetchPagedPromotions(this.query);
        if (result?.succeeded) {
          this.promotions = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching promotions:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchPromotionGlobal() {
      const service = serviceFactory.promotionService;
      this.loading = true;
      try {
        const result = await service.fetchGlobalPromotions();
        if (result?.succeeded) {
          this.promotionGlobal = result.data;
        }
      } catch (error) {
        console.error('Error fetching promotions:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchPromotionValues() {
      const service = serviceFactory.promotionService;
      this.loading = true;
      try {
        const result = await service.fetchPromotionValues();
        if (result?.succeeded) {
          this.promotionAvaliable = result.data;
        }
      } catch (error) {
        console.error('Error fetching promotions:', error);
      } finally {
        this.loading = false;
      }
    },

    /**
     * Lấy tất cả khuyến mãi từ API.
     */
    async fetchAllPromotions() {
      const service = serviceFactory.promotionService;
      this.loading = true;
      try {
        const result = await service.fetchAllPromotions();
        if (result?.succeeded) {
          this.promotions = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching all promotions:', error);
      } finally {
        this.loading = false;
      }
    },

    /**
     * Thêm hoặc cập nhật một khuyến mãi.
     * @param promotion Dữ liệu khuyến mãi cần lưu.
     */
    async savePromotion(promotion: Partial<PromotionModel>) {
      const service = serviceFactory.promotionService;
      //   if (promotion.id) {
      await service.savePromotions(promotion);
      //   } else {
      //     await service.addPromotion(promotion);
      //   }
    },

    /**
     * Xóa các khuyến mãi đã chọn.
     * @param ids Mảng các ID của khuyến mãi cần xóa.
     */
    async deletePromotions(ids: string[]) {
      const service = serviceFactory.promotionService;
      await service.deletePromotion(ids);
    },

    /**
     * Khôi phục các khuyến mãi đã xóa.
     * @param ids Mảng các ID của khuyến mãi cần khôi phục.
     */
    // async restorePromotions(ids: string[]) {
    //   const service = serviceFactory.promotionService;
    //   await service.restorePromotions(ids);
    // },

    /**
     * Chọn một khuyến mãi để hiển thị chi tiết hoặc chỉnh sửa.
     * @param promotion Khuyến mãi được chọn.
     */
    selectPromotion(promotion: PromotionModel | null) {
      this.selectedPromotion = promotion;
    },

    /**
     * Cập nhật số trang.
     * @param page Số trang mới.
     */
    setPage(page: number) {
      this.query.page = page;
    },

    /**
     * Cập nhật kích thước trang và reset về trang 1.
     * @param pageSize Kích thước trang mới.
     */
    setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
    },

    /**
     * Cập nhật bộ lọc tìm kiếm và reset về trang 1.
     * @param filter Bộ lọc mới.
     */
    setFilter(filter: Partial<PromotionQuery['filters']>) {
      this.query.filters = { ...this.query.filters, ...filter };
      this.query.page = 1;
    },
  },
});