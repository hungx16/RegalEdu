// src/services/ItemService.ts
import type { ItemApi, ItemModel, ItemQuery } from '@/api/ItemApi';
import type { Result } from '@/types/Result';

export class ItemService {
  private api: ItemApi;

  constructor(apiInstance: ItemApi) {
    this.api = apiInstance;
  }

  async fetchPagedItems(query: ItemQuery): Promise<Result<any>> {
    return await this.api.getPagedItems(query);
  }

  async fetchAllItems(): Promise<Result<any>> {
    return await this.api.getAllItems();
  }

  async saveItem(model: Partial<ItemModel>): Promise<any> {
    let result: any;
    if (model.id) result = await this.api.updateItem(model);
    else result = await this.api.addItem(model);
    if (!result.succeeded) throw new Error(result.error || 'Save failed');
    return result.data;
  }

  async deleteItems(ids: string[]): Promise<void> {
    const result: any = await this.api.deleteItems(ids);
    if (!result.succeeded) throw new Error(result.error || 'Delete failed');
  }

  async getItemById(id: string): Promise<ItemModel> {
    const result: any = await this.api.getItemById(id);
    if (!result.succeeded) throw new Error(result.error || 'Fetch failed');
    return result.data;
  }

  async getDeletedItems(): Promise<ItemModel[]> {
    const result: any = await this.api.getDeletedItems();
    if (!result.succeeded) throw new Error(result.error || 'Fetch failed');
    return result.data;
  }
}
