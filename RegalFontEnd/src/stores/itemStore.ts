// src/stores/itemStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { ItemModel, ItemQuery } from '@/api/ItemApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useItemStore = defineStore('item', {
  state: () => ({
    items: [] as ItemModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {},
    } as ItemQuery,
    selectedItem: null as ItemModel | null,
  }),
  actions: {
    async fetchItems() {
      const service = serviceFactory.itemService;
      this.loading = true;
      try {
        const result = await service.fetchPagedItems(this.query);
        if (result?.succeeded) {
          this.items = result.data.items;
          this.total = result.data.total;
        }
      } finally {
        this.loading = false;
      }
    },
    async fetchAllItems() {
      const service = serviceFactory.itemService;
      this.loading = true;
      try {
        const result = await service.fetchAllItems();
        if (result?.succeeded) this.items = result.data;
      } finally {
        this.loading = false;
      }
    },
    selectItem(model: ItemModel | null) {
      this.selectedItem = model;
    },
    async saveItem(model: Partial<ItemModel>) {
      await serviceFactory.itemService.saveItem(model);
      await this.fetchItems();
    },
    async deleteItems(ids: string[]) {
      await serviceFactory.itemService.deleteItems(ids);
      await this.fetchItems();
    },
    async setPage(page: number) {
      this.query.page = page;
      await this.fetchItems();
    },
    async setPageSize(pageSize: number) {
      this.query.pageSize = pageSize;
      this.query.page = 1;
      await this.fetchItems();
    },
    async setFilter(filter: Record<string, any>) {
      this.query.filters = { ...this.query.filters, ...filter };
      this.query.page = 1;
      await this.fetchItems();
    },
  },
});
