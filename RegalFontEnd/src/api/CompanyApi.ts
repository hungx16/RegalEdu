// src/api/CompanyApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { Attachment } from './FileApi';


export interface CompanyModel extends BaseEntityModel {
  companyCode: string;
  companyName: string;
  companyAddress?: string | null;
  companyPhone?: string | null;
  establishmentDate?: string | null;
  provinceCode?: string | null;
  managerId?: string | null;
  manager?: any;
  logRegionComs?: LogRegionComModel[] | null; // Liên kết vùng
  companyImages: ImageModel[];
  regionId?: string | null; // Mã vùng
  region?: any; // Thông tin vùng,
  deletedImageIds?: string[] | null; // <== khi edit, gửi kèm
  employees?: any[] | null; // Danh sách nhân viên,
  wardCode?: string | null;
  isPublish: boolean;
  companyEmail?: string | null;
  numberOfStudents?: number | null;
  convenience?: string | null;
  votingRate?: number | null;
  enCompanyName?: string | null;
  enCompanyAddress?: string | null;
  enConvenience?: string | null;
  enDescription?: string | null;
  description?: string | null;
  workingTime?: string | null;
  enWorkingTime?: string | null;
  isMultilingual?: boolean;
  isHeadQuarters?: boolean;
  companyLearningRoadMaps?: CompanyLearningRoadMapModel[];
  companyLearningRoadMapIds?: string[]; // mảng id, tiện cho binding v-model
  latitude?: number | null;   // thêm
  longitude?: number | null;  // thêm
}
export interface CompanyLearningRoadMapModel {
  id?: string | null;
  companyId?: string;
  learningRoadMapId: string | null;
}
export interface ImageModel extends Attachment {
  caption?: string
  isCover?: boolean
  sortOrder?: number
}
export interface LogRegionComModel extends BaseEntityModel {
  companyId?: string | null; // Guid
  company?: any | null;
  regionId?: string; // Guid
  region?: any | null;
  startedDate: string; // ISO date string (yyyy-MM-dd hoặc yyyy-MM-ddTHH:mm:ss)
  endDate?: string | null;
  description?: string | null;
}

export interface CompanyQuery {
  page: number;
  pageSize: number;
  filters?: {
    companyCode?: string;
    companyName?: string;
    provinceCode?: string;
    regionId?: string;
    managerId?: string;
    isDeleted?: boolean;
  };
}

export interface CompanyPagedResult {
  items: CompanyModel[];
  total: number;
}

export class CompanyApi extends ApiClient {
  controller = 'company';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedCompanies(query: CompanyQuery): Promise<Result<CompanyPagedResult>> {
    return await this.get<Result<CompanyPagedResult>>(`/${this.controller}/GetPagedCompanies`, { params: query });
  }

  public async getAllCompanies(): Promise<Result<CompanyModel[]>> {
    return await this.get<Result<CompanyModel[]>>(`/${this.controller}/GetAllCompanies`);
  }

  public async addCompany(data: Partial<CompanyModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddCompany`, data);
  }

  public async updateCompany(data: Partial<CompanyModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateCompany`, data);
  }

  public async deleteCompanies(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListCompany`, { data: ids });
  }

  public async restoreCompanies(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListCompany`, { data: ids });
  }

  public async getCompanyById(id: string): Promise<Result<CompanyModel>> {
    return await this.get<Result<CompanyModel>>(`/${this.controller}/GetCompanyById`, { params: { id } });
  }

  public async getDeletedCompanies(): Promise<Result<CompanyModel[]>> {
    return await this.get<Result<CompanyModel[]>>(`/${this.controller}/GetDeletedCompanies`);
  }
  public async getAllCompanyRegions(): Promise<Result<LogRegionComModel[]>> {
    return await this.get<Result<LogRegionComModel[]>>(`/${this.controller}/GetAllCompanyRegions`);
  }
  public async createLogRegionCom(data: Partial<LogRegionComModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/CreateLogRegionCom`, data);
  }
}
