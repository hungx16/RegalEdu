// src/api/DivisionApi.ts
import { ApiClient } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface GenerateCodeRequest {
  prefix: string;
  tableName: string;
  columnName: string;
  length?: number; // Optional, default can be set in the backend
  format?: string; // Optional, e.g., "yyyyMMdd"
  year?: number; // Optional, e.g., 2024
  month?: number; // Optional, e.g., 3
}
export interface Province {
  provinceCode: string;
  provinceName: string;
}
export interface DocumentType {
  documentTypeId: number;
  documentTypeName: string;
}
export interface WebsiteKey {
  frequency: number;
  key: string;
}
export interface Ward {
  wardCode: string;
  wardName: string;
  provinceCode: string;
  level: string;
}
export class CommonApi extends ApiClient {

  controller = 'common';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async generateCode(query: GenerateCodeRequest): Promise<Result<string>> {
    return await this.get<Result<string>>(`/${this.controller}/GenerateCode`, { params: query });
  }
  public async fetchProvinces(): Promise<Result<Province[]>> {
    return await this.get<Result<Province[]>>(`/${this.controller}/Provinces`, {});
  }
  public async fetchWards(provinceCode: string): Promise<Result<Ward[]>> {
    return await this.get<Result<Ward[]>>(`/${this.controller}/Wards`, { params: { provinceCode } });
  }
  public async fetchDocumentTypes(): Promise<Result<DocumentType[]>> {
    return await this.get<Result<DocumentType[]>>(`/${this.controller}/DocumentTypes`, {});
  }
  public async fetchWebsiteKeys(): Promise<Result<WebsiteKey[]>> {
    return await this.get<Result<WebsiteKey[]>>(`/${this.controller}/GetWebsiteKeys`, {});
  }
  public async fetchEnWebsiteKeys(): Promise<Result<WebsiteKey[]>> {
    return await this.get<Result<WebsiteKey[]>>(`/${this.controller}/GetEnWebsiteKeys`, {});
  }
}
