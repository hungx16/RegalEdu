// src/api/AllocationEventApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient'
import type { EventSize, CompanyEventProposalStatus } from '@/types'
import type { Result } from '@/types/Result'
import type { ItemModel } from './ItemApi'

export interface AllocationDetailEventModel extends BaseEntityModel {
  allocationEventId: string
  eventId: string
  quantity: number
  budget: number
  companyId: string
  regionId: string
  noAllocation: number
  allocationEvent?: AllocationEventModel
  company?: any
  region?: any
  companyEvents?: CompanyEventModel[]
}

export interface AllocationEventModel extends BaseEntityModel {
  allocationCode: string
  allocationMonth: number
  allocationYear: number
  eventBudget: number
  allocationEventStatus: number
  allocationDetails?: AllocationDetailEventModel[]
  allocationEventHistories?: AllocationEventHistoryModel[]
  historyChanges?: AllocationEventHistoryChange[]
}

export interface AllocationEventHistoryModel extends BaseEntityModel {
  allocationEventId: string
  targetName?: string | null
  actionName?: string | null
  description?: string | null
  createdByName?: string | null
}

export interface AllocationEventHistoryChange {
  actionName: string
  description?: string | null
  targetName?: string | null
}

export interface AllocationEventQuery {
  page: number
  pageSize: number
  filters?: {
    allocationCode?: string
    allocationYear?: number
    allocationMonth?: number
    allocationEventStatus?: number
    isDeleted?: boolean
  }
}

export interface AllocationEventPagedResult {
  items: AllocationEventModel[]
  total: number
}
export interface CompanyEventModel extends BaseEntityModel {
  allocationDetailEventId?: string;
  allocationDetailEvent?: AllocationDetailEventModel;
  companyEventCode: string;
  companyEventName: string;
  eventDate: string;
  affiliatePartnerId?: string;
  companyId?: string;
  regionId?: string;
  numberStudents?: number;
  propose?: string;
  totalAmount?: number;
  eventSize: EventSize;
  companyEventStatus?: CompanyEventProposalStatus;
  status?: number;
  // Backend collections
  eventPublications?: EventPublicationModel[]
  eventCashes?: EventCashModel[]
  eventParticipants?: EventParticipantModel[]
  // Legacy/front-end aliases (keep for compatibility)
  publications?: EventPublicationModel[]
  cashCosts?: EventCashModel[]
  participants?: EventParticipantModel[]
  attachments?: AttachmentModel[]
  approveCompanyEvents?: ApproveCompanyEventModel[]
}
export interface CompanyEventReportModel extends BaseEntityModel {
  companyEventId?: string | null;
  companyEvent?: CompanyEventModel;
  companyEventReportCode: string;
  eventDate: string;
  reportDate?: string;
  numberStudents?: number;
  totalAmount?: number;
  companyEventStatus?: CompanyEventProposalStatus;
  status?: number;
  linkContent?: string;
  linkFanpage?: string;
  // Backend collections
  eventPublications?: EventPublicationModel[]
  eventCashes?: EventCashModel[]
  eventParticipants?: EventParticipantModel[]
  // Legacy/front-end aliases (keep for compatibility)
  publications?: EventPublicationModel[]
  cashCosts?: EventCashModel[]
  participants?: EventParticipantModel[]
  attachments?: AttachmentModel[]
  approveCompanyEvents?: ApproveCompanyEventReportModel[]
}

export interface EventParticipantModel extends BaseEntityModel {
  companyEventId?: string | null;
  companyEventReportId?: string | null;
  studentCode?: string;
  isStudent: boolean;
  participantName: string;
  participantGender?: string;
  participantDateOfBirth?: string;
  participantAddress?: string;
  participantPhoneNumber?: string;
  participantContact?: string;
  participantEmail?: string;
  participantSchool?: string;
  participantSourceKnown?: string;
  participantJob?: string;
  employeeId?: string;
}

export interface EventPublicationModel extends BaseEntityModel {
  companyEventId?: string | null;
  companyEventReportId?: string | null;
  itemId: string;
  item?: ItemModel;
  quantity: number;
  publicationAmount: number;
  totalAmount: number;
}

export interface EventCashModel extends BaseEntityModel {
  companyEventId?: string | null;
  companyEventReportId?: string | null;
  cashName: string;
  quantity: number;
  amount: number;
  totalAmount: number;
}

export interface AttachmentModel extends BaseEntityModel {
  companyEventId?: string;
  companyEventReportId?: string;
  fileName?: string;
  filePath?: string;
  contentType?: string;
  size?: number;
  relativePath?: string;
}

export interface CompanyEventProposalRequest {
  companyEvent: CompanyEventModel;
  publications?: Array<Pick<EventPublicationModel, 'id' | 'itemId' | 'quantity' | 'publicationAmount' | 'totalAmount'>>;
  cashCosts?: Array<Pick<EventCashModel, 'id' | 'cashName' | 'quantity' | 'amount' | 'totalAmount'>>;
  participants?: Array<Partial<Omit<EventParticipantModel, 'companyEventId'> & { id?: string }>>;
  attachments?: Array<Pick<AttachmentModel, 'id' | 'fileName' | 'filePath' | 'relativePath' | 'size' | 'contentType'>>;
  deletedAttachmentIds?: string[];
  deletedPublicationIds?: string[];
  deletedCashIds?: string[];
  deletedParticipantIds?: string[];
  proposalQuantity: number;
}

export interface CreateCompanyEventProposalCommand {
  companyEventProposalRequest: CompanyEventProposalRequest;
}

export interface ApproveCompanyEventModel extends BaseEntityModel {
  companyEventId: string;
  reason?: string;
  approveStatus: CompanyEventProposalStatus;
  employeeId?: string;
}
export interface ApproveCompanyEventReportModel extends BaseEntityModel {
  companyEventReportId: string;
  reason?: string;
  approveStatus: CompanyEventProposalStatus;
  employeeId?: string;
}

export class AllocationEventApi extends ApiClient {
  controller = 'allocationevent'

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  async getPagedAllocationEvents(query: AllocationEventQuery): Promise<Result<AllocationEventPagedResult>> {
    return await this.get<Result<AllocationEventPagedResult>>(`/${this.controller}/GetPagedAllocationEvents`, { params: query })
  }

  async getAllAllocationEvents(): Promise<Result<AllocationEventModel[]>> {
    return await this.get<Result<AllocationEventModel[]>>(`/${this.controller}/GetAllAllocationEvents`)
  }
  async getAllAllocationEventsForRegion(): Promise<Result<AllocationEventModel[]>> {
    return await this.get<Result<AllocationEventModel[]>>(`/${this.controller}/GetAllAllocationEventsForRegion`)
  }
  async getAllAllocationEventsForCompany(): Promise<Result<AllocationEventModel[]>> {
    return await this.get<Result<AllocationEventModel[]>>(`/${this.controller}/GetAllAllocationEventsForCompany`)
  }

  async getAllocationEventById(id: string): Promise<Result<AllocationEventModel>> {
    return await this.get<Result<AllocationEventModel>>(`/${this.controller}/GetAllocationEventById`, { params: { id } })
  }

  async addAllocationEventWithDetails(data: Partial<AllocationEventModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddAllocationEventWithDetails`, data)
  }

  async updateAllocationEventWithDetails(data: Partial<AllocationEventModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateAllocationEventWithDetails`, data)
  }

  async deleteListAllocationEventWithDetails(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListAllocationEventWithDetails`, { data: ids })
  }

  async getAllocationEventSummaries(): Promise<Result<any>> {
    return await this.get<Result<any>>(`/${this.controller}/GetAllocationEventSummaries`)
  }

  async createCompanyEventProposal(data: CompanyEventProposalRequest): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/CreateCompanyEventProposal`, data)
  }

  async updateCompanyEventProposal(data: CompanyEventProposalRequest): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateCompanyEventProposal`, data)
  }
  async updateStatusOfCompanyEventProposal(data: ApproveCompanyEventModel): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/UpdateStatusOfCompanyEventProposal`, data)
  }

  async getAllCompanyEventProposals(): Promise<Result<CompanyEventModel[]>> {
    return await this.get<Result<CompanyEventModel[]>>(`/${this.controller}/GetAllCompanyEventProposal`)
  }

  async getAllCompanyEventReports(): Promise<Result<CompanyEventReportModel[]>> {
    return await this.get<Result<CompanyEventReportModel[]>>(`/${this.controller}/GetAllCompanyEventReports`)
  }

  async getCompanyEventReportsByCompanyEventId(companyEventId: string): Promise<Result<CompanyEventReportModel[]>> {
    return await this.get<Result<CompanyEventReportModel[]>>(
      `/${this.controller}/GetCompanyEventReportsByCompanyEventId`,
      { params: { companyEventId } }
    )
  }

  async createCompanyEventReport(data: CompanyEventReportModel): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/CreateCompanyEventReport`, data)
  }

  async updateCompanyEventReport(data: CompanyEventReportModel): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateCompanyEventReport`, data)
  }

  async approveCompanyEventProposal(data: ApproveCompanyEventModel): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/ApproveCompanyEventProposal`, data)
  }

  async approveCompanyEventReport(data: ApproveCompanyEventReportModel): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/ApproveCompanyEventReport`, data)
  }

  async updateStatusOfCompanyEventReport(data: ApproveCompanyEventReportModel): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/UpdateStatusOfCompanyEventReport`, data)
  }
}
