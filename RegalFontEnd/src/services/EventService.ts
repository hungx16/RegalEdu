import type { EventModel, EventQuery } from '@/api/EventApi';
import type { EventApi } from '@/api/EventApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class EventService {
  private eventApi: EventApi;

  constructor(eventApiInstance: EventApi) {
    this.eventApi = eventApiInstance;
  }

  async fetchPagedEvents(query: EventQuery): Promise<Result<any>> {
    return await this.eventApi.getPagedEvents(query);
  }

  async fetchAllEvents(): Promise<Result<any>> {
    return await this.eventApi.getAllEvents();
  }

  async saveEvent(event: Partial<EventModel>): Promise<any> {
    let result: any;
    if (event.id) {
      result = await this.eventApi.updateEvent(event);
    } else {
      result = await this.eventApi.addEvent(event);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteEvents(eventIds: string[]): Promise<void> {
    let result: any = await this.eventApi.deleteEvents(eventIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }
}