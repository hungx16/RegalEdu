// src/services/AllocationEventService.ts
import {
  AllocationEventApi,
  type AllocationEventModel,
  type AllocationEventQuery
} from '@/api/AllocationEventApi'
import type { Result } from '@/types/Result'

const api = new AllocationEventApi()

export class AllocationEventService {
  async fetchPaged(query: AllocationEventQuery): Promise<Result<any>> {
    return await api.getPagedAllocationEvents(query)
  }

  async fetchAll(): Promise<Result<AllocationEventModel[]>> {
    return await api.getAllAllocationEvents()
  }
  async fetchAllForRegion(): Promise<Result<AllocationEventModel[]>> {
    return await api.getAllAllocationEventsForRegion()
  }
  async fetchAllForCompany(): Promise<Result<AllocationEventModel[]>> {
    return await api.getAllAllocationEventsForCompany()
  }

  async add(data: Partial<AllocationEventModel>): Promise<Result<any>> {
    return await api.addAllocationEventWithDetails(data)
  }

  async update(data: Partial<AllocationEventModel>): Promise<Result<any>> {
    return await api.updateAllocationEventWithDetails(data)
  }

  async delete(ids: string[]): Promise<Result<any>> {
    return await api.deleteListAllocationEventWithDetails(ids)
  }

  async getById(id: string): Promise<Result<AllocationEventModel>> {
    return await api.getAllocationEventById(id)
  }

  async fetchSummary(): Promise<Result<any>> {
    return await api.getAllocationEventSummaries()
  }
}
