import {
  AllocationEventApi,
  type CompanyEventProposalRequest,
  type CompanyEventModel,
  type CompanyEventReportModel,
  type ApproveCompanyEventModel,
  type ApproveCompanyEventReportModel
} from '@/api/AllocationEventApi'
import type { Result } from '@/types/Result'

export class CompanyEventService {
  private api = new AllocationEventApi()

  async createProposal(data: CompanyEventProposalRequest): Promise<Result<any>> {
    return await this.api.createCompanyEventProposal(data)
  }

  async updateProposal(data: CompanyEventProposalRequest): Promise<Result<any>> {
    return await this.api.updateCompanyEventProposal(data)
  }
  async fetchAllProposals(): Promise<Result<CompanyEventModel[]>> {
    return await this.api.getAllCompanyEventProposals()
  }

  async fetchAllReports(): Promise<Result<CompanyEventReportModel[]>> {
    return await this.api.getAllCompanyEventReports()
  }

  async fetchReportsByCompanyEventId(companyEventId: string): Promise<Result<CompanyEventReportModel[]>> {
    return await this.api.getCompanyEventReportsByCompanyEventId(companyEventId)
  }

  async createReport(data: CompanyEventReportModel): Promise<Result<any>> {
    return await this.api.createCompanyEventReport(data)
  }

  async updateReport(data: CompanyEventReportModel): Promise<Result<any>> {
    return await this.api.updateCompanyEventReport(data)
  }

  async approveProposal(data: ApproveCompanyEventModel): Promise<Result<any>> {
    return await this.api.approveCompanyEventProposal(data)
  }

  async updateStatusOnly(data: ApproveCompanyEventModel): Promise<Result<any>> {
    return await this.api.updateStatusOfCompanyEventProposal(data)
  }

  async approveReport(data: ApproveCompanyEventReportModel): Promise<Result<any>> {
    return await this.api.approveCompanyEventReport(data)
  }

  async updateReportStatusOnly(data: ApproveCompanyEventReportModel): Promise<Result<any>> {
    return await this.api.updateStatusOfCompanyEventReport(data)
  }
}
