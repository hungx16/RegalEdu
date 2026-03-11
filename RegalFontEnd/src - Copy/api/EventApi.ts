import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface EventModel extends BaseEntityModel {
  eventCode: string;
  eventName: string;
  description?: string;
  category: number;
}

export interface EventQuery {
  page: number;
  pageSize: number;
  filters?: {
    eventCode?: string;
    eventName?: string;
    category?: number;
    status?: number;
    isDeleted?: boolean;
  };
}

export interface EventPagedResult {
  items: EventModel[];
  total: number;
}

export class EventApi extends ApiClient {
  controller = 'event';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedEvents(query: EventQuery): Promise<Result<EventPagedResult>> {
    return await this.get<Result<EventPagedResult>>(`/${this.controller}/GetPagedEvents`, { params: query });
  }

  public async getAllEvents(): Promise<Result<EventModel[]>> {
    return await this.get<Result<EventModel[]>>(`/${this.controller}/GetAllEvents`);
  }

  public async addEvent(data: Partial<EventModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddEvent`, data);
  }

  public async updateEvent(data: Partial<EventModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateEvent`, data);
  }

  public async deleteEvents(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListEvents`, { data: ids });
  }

  public async getEventById(id: string): Promise<Result<EventModel>> {
    return await this.get<Result<EventModel>>(`/${this.controller}/GetEventById`, { params: { id } });
  }
}
