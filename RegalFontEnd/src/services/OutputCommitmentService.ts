// src/services/OutputCommitmentService.ts
import { OutputCommitmentApi, type OutputCommitmentModel } from '@/api/OutputCommitmentApi'
import type { Result } from '@/types/Result'

const api = new OutputCommitmentApi()

export class OutputCommitmentService {
  async fetchAll(): Promise<Result<OutputCommitmentModel[]>> {
    return await api.getAllOutputCommitments()
  }

  async getById(id: string): Promise<Result<OutputCommitmentModel>> {
    return await api.getOutputCommitmentById(id)
  }

  async add(data: Partial<OutputCommitmentModel>): Promise<Result<any>> {
    return await api.addOutputCommitment(data)
  }

  async update(data: Partial<OutputCommitmentModel>): Promise<Result<any>> {
    return await api.updateOutputCommitment(data)
  }

  async delete(ids: string[]): Promise<Result<any>> {
    return await api.deleteOutputCommitments(ids)
  }

  async save(model: Partial<OutputCommitmentModel>) {
    if (model.id) return this.update(model)
    return this.add(model)
  }

  async downloadPdf(model: Partial<OutputCommitmentModel>) {
    return await api.getPdfForOutputCommitment(model)
  }
}
