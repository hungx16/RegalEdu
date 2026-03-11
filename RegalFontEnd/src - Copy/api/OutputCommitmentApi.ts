// src/api/OutputCommitmentApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient'
import type { Result } from '@/types/Result'
import { OutputCommitmentStatus } from '@/types'

export interface OutputCommitmentModel extends BaseEntityModel {
  studentCode: string
  beginningLevel?: string | null
  finalLevel?: string | null
  outputCommitmentInfo?: string | null
  outputCommitmentStatus: OutputCommitmentStatus
  studentId?: string | null
  student?: any
}

export class OutputCommitmentApi extends ApiClient {
  controller = 'outputcommitment'

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  public async getAllOutputCommitments(): Promise<Result<OutputCommitmentModel[]>> {
    return await this.get<Result<OutputCommitmentModel[]>>(`/${this.controller}/GetAllOutputCommitments`)
  }

  public async getOutputCommitmentById(id: string): Promise<Result<OutputCommitmentModel>> {
    return await this.get<Result<OutputCommitmentModel>>(`/${this.controller}/GetOutputCommitmentById`, { params: { id } })
  }

  public async addOutputCommitment(data: Partial<OutputCommitmentModel>): Promise<Result<any>> {
    delete data.student
    return await this.post<Result<any>>(`/${this.controller}/AddOutputCommitment`, data)
  }

  public async updateOutputCommitment(data: Partial<OutputCommitmentModel>): Promise<Result<any>> {
    delete data.student
    return await this.put<Result<any>>(`/${this.controller}/UpdateOutputCommitment`, data)
  }

  public async deleteOutputCommitments(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListOutputCommitment`, { data: ids })
  }

  public async getPdfForOutputCommitment(data: Partial<OutputCommitmentModel>) {
    const payload = { ...data }
    delete (payload as any).student
    return await this.get<ArrayBuffer>(`/${this.controller}/GetPdfForOutputCommitment`, {
      params: payload,
      responseType: 'arraybuffer'
    })
  }
}
