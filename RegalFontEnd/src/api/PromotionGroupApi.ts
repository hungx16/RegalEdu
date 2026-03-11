// src/api/DepartmentApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';


export interface PromotionGroupModel {
  id?: string;
  name: string;
  description?: string;
  status?: number;
  createdAt?: string;
}

export interface PromotionGroupQuery {
  page: number;
  pageSize: number;
  filters?: {
    groupName?: string;
    status?: number;
  };
}

export interface PagedResult<T> {
  items: PromotionGroupModel[];
  total: number;
  page: number;
  pageSize: number;
}
export class PromotionGroupApi extends ApiClient {
  controller = 'promotiongroup';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getAllPromotionGroups(): Promise<Result<PromotionGroupModel[]>> {
    return await this.get<Result<PromotionGroupModel[]>>(`/${this.controller}/GetAllPromotionGroups`);
    // return await this.get('GetAll');
  }

  public async getPromotionGroupById(id: string): Promise<Result<PromotionGroupModel>> {
    return await this.get<Result<PromotionGroupModel>>(`/${this.controller}/GetPromotionGroupById`, { params: { id } });
    //return await this.get(`${id}`);
  }

  public async getPagedPromotionGroups(query: PromotionGroupQuery): Promise<Result<PagedResult<PromotionGroupModel>>> {
    return await this.get<Result<PagedResult<PromotionGroupModel>>>(`/${this.controller}/GetPagedPromotionGroups`, { params: query });
    //return await this.post('paged', query);
  }

  public async addPromotionGroup(model: Partial<PromotionGroupModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddPromotionGroup`, model);
    //return await this.post('', model);
  }

  public async updatePromotionGroup(model: Partial<PromotionGroupModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdatePromotionGroup`, model);
    //return await this.put('', model);
  }

  public async deletePromotionGroups(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListPromotionGroup`, { data: ids });
    //return await this.delete('', ids);
  }
}