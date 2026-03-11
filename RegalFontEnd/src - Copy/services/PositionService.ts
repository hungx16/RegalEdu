// src/services/PositionService.ts
import type { PositionModel, PositionQuery } from '@/api/PositionApi';
import type { PositionApi } from '@/api/PositionApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class PositionService {
  private positionApi: PositionApi;

  constructor(positionApiInstance: PositionApi) {
    this.positionApi = positionApiInstance;
  }

  async fetchPagedPositions(query: PositionQuery): Promise<Result<any>> {
    return await this.positionApi.getPagedPositions(query);
  }
  async fetchAllPositions(): Promise<Result<any>> {
    return await this.positionApi.getAllPositions();
  }
  async savePosition(position: Partial<PositionModel>): Promise<any> {
    let result: any;
    if (position.id) {
      result = await this.positionApi.updatePosition(position);
    } else {
      result = await this.positionApi.addPosition(position);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deletePositions(positionIds: string[]): Promise<void> {
    let result: any = await this.positionApi.deletePositions(positionIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restorePositions(positionIds: string[]): Promise<void> {
    let result: any = await this.positionApi.restorePositions(positionIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Restore failed');
    }
  }
  async fetchDeletedPositions(): Promise<Result<any>> {
    return await this.positionApi.getDeletedPositions();
  }
}
