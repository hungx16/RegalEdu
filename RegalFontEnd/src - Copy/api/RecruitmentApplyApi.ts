import { ApiClient, type BaseEntityModel } from '@/api/ApiClient'
import type { Result } from '@/types/Result'

export interface RecruitmentApplyModel extends BaseEntityModel {
  candidateName: string
  candidateEmail: string
  candidatePhone: string
  candidateExperience?: string | null
  candidateCV?: string | null
  recruitmentInfoId: string
  recruitmentInfo?: any
  attachment?: any
}

export interface RecruitmentApplyQuery {
  page: number
  pageSize: number
  filters?: {
    candidateName?: string
    candidateEmail?: string
    candidatePhone?: string
    recruitmentInfoId?: string
    isDeleted?: boolean
  }
}

export interface RecruitmentApplyPagedResult {
  items: RecruitmentApplyModel[]
  total: number
}

export class RecruitmentApplyApi extends ApiClient {
  controller = 'recruitmentapply'

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  public async getPagedRecruitmentApplies(query: RecruitmentApplyQuery): Promise<Result<RecruitmentApplyPagedResult>> {
    return await this.get<Result<RecruitmentApplyPagedResult>>(`/${this.controller}/GetPagedRecruitmentApplies`, { params: query })
  }

  public async getAllRecruitmentApplies(): Promise<Result<RecruitmentApplyModel[]>> {
    return await this.get<Result<RecruitmentApplyModel[]>>(`/${this.controller}/GetAllRecruitmentApplies`)
  }

  public async addRecruitmentApply(data: Partial<RecruitmentApplyModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddRecruitmentApply`, data)
  }

  public async updateRecruitmentApply(data: Partial<RecruitmentApplyModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateRecruitmentApply`, data)
  }

  public async deleteRecruitmentApply(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListRecruitmentApply`, { data: ids })
  }

  public async restoreRecruitmentApply(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListRecruitmentApply`, { data: ids })
  }

  public async getRecruitmentApplyById(id: string): Promise<Result<RecruitmentApplyModel>> {
    return await this.get<Result<RecruitmentApplyModel>>(`/${this.controller}/GetRecruitmentApplyById`, { params: { id } })
  }

  public async getDeletedRecruitmentApplies(): Promise<Result<RecruitmentApplyModel[]>> {
    return await this.get<Result<RecruitmentApplyModel[]>>(`/${this.controller}/GetDeletedRecruitmentApplies`)
  }
}
