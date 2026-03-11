// src/services/RecruitmentInfoService.ts
import { RecruitmentInfoApi, type RecruitmentInfoModel, type RecruitmentInfoQuery } from '@/api/RecruitmentInfoApi';
import type { Result } from '@/types/Result';

const api = new RecruitmentInfoApi();

export class RecruitmentInfoService {
  async fetchPaged(query: RecruitmentInfoQuery): Promise<Result<any>> {
    return await api.getPagedRecruitmentInfo(query);
  }

  async fetchAll(): Promise<Result<RecruitmentInfoModel[]>> {
    return await api.getAllRecruitmentInfo();
  }

  async add(data: Partial<RecruitmentInfoModel>): Promise<Result<any>> {
    return await api.addRecruitmentInfo(data);
  }

  async update(data: Partial<RecruitmentInfoModel>): Promise<Result<any>> {
    return await api.updateRecruitmentInfo(data);
  }

  async delete(ids: string[]): Promise<Result<any>> {
    return await api.deleteRecruitmentInfo(ids);
  }

  async restore(ids: string[]): Promise<Result<any>> {
    return await api.restoreRecruitmentInfo(ids);
  }

  async getById(id: string): Promise<Result<RecruitmentInfoModel>> {
    return await api.getRecruitmentInfoById(id);
  }

  async fetchDeleted(): Promise<Result<RecruitmentInfoModel[]>> {
    return await api.getDeletedRecruitmentInfo();
  }
}
