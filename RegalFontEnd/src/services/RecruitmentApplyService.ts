// src/services/RecruitmentApplyService.ts
import { RecruitmentApplyApi, type RecruitmentApplyModel, type RecruitmentApplyQuery } from '@/api/RecruitmentApplyApi'
import type { Result } from '@/types/Result'

const api = new RecruitmentApplyApi()

export class RecruitmentApplyService {
  async fetchPaged(query: RecruitmentApplyQuery): Promise<Result<any>> {
    return await api.getPagedRecruitmentApplies(query)
  }

  async fetchAll(): Promise<Result<RecruitmentApplyModel[]>> {
    return await api.getAllRecruitmentApplies()
  }

  async add(data: Partial<RecruitmentApplyModel>): Promise<Result<any>> {
    return await api.addRecruitmentApply(data)
  }

  async update(data: Partial<RecruitmentApplyModel>): Promise<Result<any>> {
    return await api.updateRecruitmentApply(data)
  }

  async delete(ids: string[]): Promise<Result<any>> {
    return await api.deleteRecruitmentApply(ids)
  }

  async restore(ids: string[]): Promise<Result<any>> {
    return await api.restoreRecruitmentApply(ids)
  }

  async getById(id: string): Promise<Result<RecruitmentApplyModel>> {
    return await api.getRecruitmentApplyById(id)
  }

  async fetchDeleted(): Promise<Result<RecruitmentApplyModel[]>> {
    return await api.getDeletedRecruitmentApplies()
  }
}
