// src/api/DepartmentApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface GiftModel {
  id?: string;
  name: string;
  prices: number;
  description?: string;
  status?: number;
  createdAt?: string;
}

export interface GiftQuery {
  name?: string;
  minPrice?: number;
  maxPrice?: number;
  status?: number;
  page: number;
  pageSize: number;
}

export interface GiftPagedResult {
  items: GiftModel[];
  total: number;
}
export class GiftApi extends ApiClient {
  controller = 'Gift';
  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getAllGifts(): Promise<Result<GiftModel[]>> {
    return await this.get<Result<GiftModel[]>>(`/${this.controller}/GetAllGifts`);
    //return await this.get('GetAll');
  }

  public async getGiftById(id: string): Promise<Result<GiftModel>> {
    return await this.get<Result<GiftModel>>(`/${this.controller}/GetGiftById`, { params: { id } });
    //return await this.get(`${id}`);
  }


  public async getPagedGifts(query: GiftQuery): Promise<Result<GiftPagedResult>> {
    return await this.get<Result<GiftPagedResult>>(`/${this.controller}/GetPagedGifts`, { params: query });
  }

  public async addGift(data: Partial<GiftModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddGift`, data);
    // return await this.post('', model);
  }

  public async updateGift(data: Partial<GiftModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateGift`, data);
    // return await this.put('', model);
  }

  public async deleteGifts(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListGift`, { data: ids });
    //return await this.delete('', ids);
  }
}
