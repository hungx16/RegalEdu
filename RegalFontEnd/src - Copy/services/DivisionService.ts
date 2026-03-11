// src/services/DivisionService.ts
import type { DivisionModel, DivisionQuery } from '@/api/DivisionApi';
import type { DivisionApi } from '@/api/DivisionApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class DivisionService {
  private divisionApi: DivisionApi;

  constructor(divisionApiInstance: DivisionApi) {
    this.divisionApi = divisionApiInstance;
  }

  async fetchPagedDivisions(query: DivisionQuery): Promise<Result<any>> {
    return await this.divisionApi.getPagedDivisions(query);
  }
  async fetchAllDivisions(): Promise<Result<any>> {
    return await this.divisionApi.getAllDivisions();
  }
  async saveDivision(division: Partial<DivisionModel>): Promise<any> {
    let result: any;
    if (division.id) {
      result = await this.divisionApi.updateDivision(division);
    } else {
      result = await this.divisionApi.addDivision(division);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteDivisions(divisionIds: string[]): Promise<void> {
    let result: any = await this.divisionApi.deleteDivisions(divisionIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreDivisions(divisionIds: string[]): Promise<void> {
    let result: any = await this.divisionApi.restoreDivisions(divisionIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Restore failed');
    }
  }
  async fetchDeletedPositions(): Promise<Result<any>> {
    return await this.divisionApi.getDeletedDivisions();
  }
}
