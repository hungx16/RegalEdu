// src/api/ItemApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface ItemModel extends BaseEntityModel {
  itemCode: string;   // Mã ấn phẩm
  itemName: string;   // Tên ấn phẩm
  price: number;      // Đơn giá
  quantity: number;   // Số lượng còn lại
}

export interface ItemQuery {
  page: number;
  pageSize: number;
  filters?: {
    itemCode?: string;
    itemName?: string;
    isDeleted?: boolean;
  };
}

export interface ItemPagedResult {
  items: ItemModel[];
  total: number;
}

export class ItemApi extends ApiClient {
  controller = 'item';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedItems(query: ItemQuery): Promise<Result<ItemPagedResult>> {
    return await this.get<Result<ItemPagedResult>>(`/${this.controller}/GetPagedItems`, { params: query });
  }

  public async getAllItems(): Promise<Result<ItemModel[]>> {
    return await this.get<Result<ItemModel[]>>(`/${this.controller}/GetAllItems`);
  }

  public async addItem(data: Partial<ItemModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddItem`, data);
  }

  public async updateItem(data: Partial<ItemModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateItem`, data);
  }

  public async deleteItems(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListItem`, { data: ids });
  }

  public async getItemById(id: string): Promise<Result<ItemModel>> {
    return await this.get<Result<ItemModel>>(`/${this.controller}/GetItemById`, { params: { id } });
  }

  public async getDeletedItems(): Promise<Result<ItemModel[]>> {
    return await this.get<Result<ItemModel[]>>(`/${this.controller}/GetDeletedItems`);
  }
}
