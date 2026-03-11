// src/api/SupportingDocumentApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { WebsiteKey } from './CommonApi';
import type { ImageModel } from './CompanyApi';
import type { Attachment } from './FileApi';
import type { FormatType, LevelType, TopicType } from '@/types';

export interface SupportingDocumentModel extends BaseEntityModel {
  documentName: string;
  enDocumentName?: string;
  description: string;
  enDescription?: string;
  documentTypeId: number;
  websiteKeys?: string;
  enWebsiteKeys?: string;
  startDate?: Date;
  endDate?: Date;
  authorName?: string;
  enAuthorName?: string;
  listWebsiteKeys?: WebsiteKey[];
  listEnWebsiteKeys?: WebsiteKey[];
  image?: ImageModel | null;
  attachment?: Attachment | null;
  isPublish: boolean;
  format: FormatType;
  topic: TopicType;
  yearRelease?: number;
  isMultilingual: boolean; // true = có bản dịch EN
  level?: LevelType;
  link?: string;
}
export interface DeleteSupportingDocumentRequest {
  ids: string[];
  websiteKeys: WebsiteKey[];
  enWebsiteKeys: WebsiteKey[];
}
export interface SupportingDocumentQuery {
  page: number;
  pageSize: number;
  filters?: {
    documentName?: string;
    documentTypeId?: number;
    status?: number;
    isDeleted?: boolean;
  };
}

export interface SupportingDocumentPagedResult {
  items: SupportingDocumentModel[];
  total: number;
}

export class SupportingDocumentApi extends ApiClient {
  controller = 'supportingDocument';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  // API để lấy danh sách tài liệu hỗ trợ theo phân trang
  public async getPagedSupportingDocuments(query: SupportingDocumentQuery): Promise<Result<SupportingDocumentPagedResult>> {
    return await this.get<Result<SupportingDocumentPagedResult>>(`/${this.controller}/GetPagedSupportingDocuments`, { params: query });
  }

  // API để lấy tất cả tài liệu hỗ trợ
  public async getAllSupportingDocuments(): Promise<Result<SupportingDocumentModel[]>> {
    return await this.get<Result<SupportingDocumentModel[]>>(`/${this.controller}/GetAllSupportingDocuments`);
  }

  // API thêm tài liệu hỗ trợ
  public async addSupportingDocument(data: Partial<SupportingDocumentModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddSupportingDocument`, data);
  }

  // API cập nhật tài liệu hỗ trợ
  public async updateSupportingDocument(data: Partial<SupportingDocumentModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateSupportingDocument`, data);
  }

  // API xóa danh sách tài liệu hỗ trợ
  public async deleteSupportingDocuments(request: DeleteSupportingDocumentRequest): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListSupportingDocument`, { data: request });
  }

  // API khôi phục danh sách tài liệu hỗ trợ
  public async restoreSupportingDocuments(request: DeleteSupportingDocumentRequest): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListSupportingDocument`, { data: request });
  }

  // API lấy tài liệu hỗ trợ theo ID
  public async getSupportingDocumentById(id: string): Promise<Result<SupportingDocumentModel>> {
    return await this.get<Result<SupportingDocumentModel>>(`/${this.controller}/GetSupportingDocumentById`, { params: { id } });
  }

  // API lấy danh sách tài liệu hỗ trợ đã xóa
  public async getDeletedSupportingDocuments(): Promise<Result<SupportingDocumentModel[]>> {
    return await this.get<Result<SupportingDocumentModel[]>>(`/${this.controller}/GetDeletedSupportingDocuments`);
  }
}
