// src/api/AffiliatePartnerApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { PartnerTypeModel } from './PartnerTypeApi';
import type { ImageModel } from './CompanyApi';
import { SchoolLevelType } from '@/types';

export interface AffiliatePartnerModel extends BaseEntityModel {
  partnerTypeId: string;
  partnerType?: PartnerTypeModel | null;
  isMultilingual: boolean;
  /** Mã định danh duy nhất (VD: TH001) - MaxLength(50) */
  partnerCode: string;

  /** Tên đầy đủ đối tác - MaxLength(200) */
  partnerName: string;
  /** Tên đầy đủ đối tác (Tiếng Anh) - MaxLength(200) */
  enPartnerName?: string;
  /** Vị trí địa lý/phạm vi (VD: bán kính dưới 5km) - MaxLength(200) */
  agencyLocation?: string | null;

  /** Tỉnh/Thành phố - MaxLength(100) */
  province: string;

  /** Địa chỉ chi tiết - MaxLength(300) */
  address?: string | null;

  /** Họ tên người đại diện - MaxLength(100) */
  contactPerson: string;

  /** SĐT (varchar(15)) - MaxLength(15) */
  phone: string;

  /** Chức vụ người đại diện - MaxLength(100) */
  position: string;

  /** Đăng lên website hay không */
  isPublish: boolean;

  /** Ảnh liên kết (nếu có) */
  image?: ImageModel;
  sortOrder: 0,
  isFinancialCompany?: boolean;
  schoolLevel?: SchoolLevelType;
  websiteKeys?: string;
  interestRate?: number;
  loanTerm?: number;
  loanBenefits?: string;
  enLoanBenefits?: string;
  enWebsiteKeys?: string;
}

export interface AffiliatePartnerQuery {
  page: number;
  pageSize: number;
  filters?: {
    affiliatePartnerCode?: string;
    affiliatePartnerName?: string;
    status?: number;
    isDeleted?: boolean;
  };
}

export interface AffiliatePartnerPagedResult {
  items: AffiliatePartnerModel[];
  total: number;
}

export interface EventReportPublicationItemModel {
  companyEventReportId: string;
  companyEventReportCode?: string | null;
  companyEventId?: string | null;
  companyEventName?: string | null;
  eventDate?: string | null;
  itemId?: string | null;
  itemCode?: string | null;
  itemName?: string | null;
  quantity?: number | null;
  publicationAmount?: number | null;
  totalAmount?: number | null;
}

export class AffiliatePartnerApi extends ApiClient {
  controller = 'affiliatePartner';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedAffiliatePartners(query: AffiliatePartnerQuery): Promise<Result<AffiliatePartnerPagedResult>> {
    return await this.get<Result<AffiliatePartnerPagedResult>>(`/${this.controller}/GetPagedAffiliatePartners`, { params: query });
  }

  public async getAllAffiliatePartners(): Promise<Result<AffiliatePartnerModel[]>> {
    return await this.get<Result<AffiliatePartnerModel[]>>(`/${this.controller}/GetAllAffiliatePartners`);
  }

  public async addAffiliatePartner(data: Partial<AffiliatePartnerModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddAffiliatePartner`, data);
  }

  public async updateAffiliatePartner(data: Partial<AffiliatePartnerModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateAffiliatePartner`, data);
  }

  public async deleteAffiliatePartners(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListAffiliatePartner`, { data: ids });
  }

  public async restoreAffiliatePartners(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListAffiliatePartner`, { data: ids });
  }

  public async getAffiliatePartnerById(id: string): Promise<Result<AffiliatePartnerModel>> {
    return await this.get<Result<AffiliatePartnerModel>>(`/${this.controller}/GetAffiliatePartnerById`, { params: { id } });
  }

  public async getDeletedAffiliatePartners(): Promise<Result<AffiliatePartnerModel[]>> {
    return await this.get<Result<AffiliatePartnerModel[]>>(`/${this.controller}/GetDeletedAffiliatePartners`);
  }

  public async getEventReportPublicationsByAffiliatePartnerId(
    affiliatePartnerId: string,
  ): Promise<Result<EventReportPublicationItemModel[]>> {
    return await this.get<Result<EventReportPublicationItemModel[]>>(`/${this.controller}/GetEventReportPublicationsByAffiliatePartnerId`, {
      params: { affiliatePartnerId },
    });
  }
}
