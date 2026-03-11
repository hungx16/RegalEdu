import { ApiClient, type BaseEntityModel } from '@/api/ApiClient'
import type { Result } from '@/types/Result'

export interface RecruitmentInfoModel extends BaseEntityModel {
  recruitmentInfoName: string
  description?: string | null
  experience?: string | null
  salary?: number
  position: string
  departmentId?: string | null
  provinceCode?: string | null
  department?: any,
  enDescription?: string | null
  enExperience?: string | null
  enPosition?: string | null
  workType?: number | null
  enRecruitmentInfoName?: string | null
  isPublish?: boolean | null
  isMultilingual?: boolean | null
}

export interface RecruitmentInfoQuery {
  page: number
  pageSize: number
  filters?: {
    recruitmentInfoName?: string
    position?: string
    provinceCode?: string
    isDeleted?: boolean
  }
}

export interface RecruitmentInfoPagedResult {
  items: RecruitmentInfoModel[]
  total: number
}

export class RecruitmentInfoApi extends ApiClient {
  controller = 'recruitmentinfo'

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  public async getPagedRecruitmentInfo(query: RecruitmentInfoQuery): Promise<Result<RecruitmentInfoPagedResult>> {
    return await this.get<Result<RecruitmentInfoPagedResult>>(`/${this.controller}/GetPagedRecruitmentInfo`, { params: query })
  }

  public async getAllRecruitmentInfo(): Promise<Result<RecruitmentInfoModel[]>> {
    return await this.get<Result<RecruitmentInfoModel[]>>(`/${this.controller}/GetAllRecruitmentInfo`)
  }

  public async addRecruitmentInfo(data: Partial<RecruitmentInfoModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddRecruitmentInfo`, data)
  }

  public async updateRecruitmentInfo(data: Partial<RecruitmentInfoModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateRecruitmentInfo`, data)
  }

  public async deleteRecruitmentInfo(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListRecruitmentInfo`, { data: ids })
  }

  public async restoreRecruitmentInfo(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListRecruitmentInfo`, { data: ids })
  }

  public async getRecruitmentInfoById(id: string): Promise<Result<RecruitmentInfoModel>> {
    return await this.get<Result<RecruitmentInfoModel>>(`/${this.controller}/GetRecruitmentInfoById`, { params: { id } })
  }

  public async getDeletedRecruitmentInfo(): Promise<Result<RecruitmentInfoModel[]>> {
    return await this.get<Result<RecruitmentInfoModel[]>>(`/${this.controller}/GetDeletedRecruitmentInfo`)
  }
}
