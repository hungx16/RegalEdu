import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';
import type { EventModel, EventQuery } from '@/api/EventApi';

const DEFAULT_PAGE_SIZE = Number(import.meta.env.VITE_DEFAULT_PAGE_SIZE) || 10;

export const useEventStore = defineStore('event', {
  state: () => ({
    events: [] as EventModel[],
    total: 0,
    loading: false,
    query: {
      page: 1,
      pageSize: DEFAULT_PAGE_SIZE,
      filters: {
        eventCode: '',
        eventName: '',
        status: undefined,
        category: undefined,
        isDeleted: false,
      },
    } as EventQuery,
    selectedEvent: null as EventModel | null,
  }),
  actions: {
    async fetchPagedEvents() {
      const eventService = serviceFactory.eventService;
      this.loading = true;
      try {
        const result = await eventService.fetchPagedEvents(this.query);
        if (result?.succeeded === true) {
          this.events = result.data.items;
          this.total = result.data.total;
        }
      } catch (error) {
        console.error('Error fetching events:', error);
      } finally {
        this.loading = false;
      }
    },
    async fetchAllEvents() {
      const eventService = serviceFactory.eventService;
      this.loading = true;
      try {
        const result = await eventService.fetchAllEvents();
        if (result?.succeeded === true) {
          this.events = result.data;
          this.total = result.data.length;
        }
      } catch (error) {
        console.error('Error fetching events:', error);
      } finally {
        this.loading = false;
      }
    },
    selectEvent(event: EventModel | null) {
      this.selectedEvent = event;
    },
    async saveEvent(event: Partial<EventModel>) {
      const eventService = serviceFactory.eventService;
      await eventService.saveEvent(event);
    },
    async deleteEvents(eventIds: string[]) {
      const eventService = serviceFactory.eventService;
      await eventService.deleteEvents(eventIds);
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
  },
});