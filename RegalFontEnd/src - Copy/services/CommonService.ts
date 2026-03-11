// src/services/DivisionService.ts
import type { CommonApi, GenerateCodeRequest } from '@/api/CommonApi';
import type { Result } from '@/types/Result';


export class CommonService {
  private commonApi: CommonApi;

  constructor(commonApiInstance: CommonApi) {
    this.commonApi = commonApiInstance;
  }

  async generateCode(query: GenerateCodeRequest): Promise<Result<any>> {
    // console.log('Generating code with query:', query);
    return await this.commonApi.generateCode(query);
  }
  async fetchProvinces(): Promise<Result<any>> {
    // console.log('Fetching provinces');
    return await this.commonApi.fetchProvinces();
  }
  async fetchWards(provinceCode: string): Promise<Result<any>> {
    // console.log('Fetching wards for provinceCode:', provinceCode);
    return await this.commonApi.fetchWards(provinceCode);
  }
  async fetchDocumentTypes(): Promise<Result<any>> {
    // console.log('Fetching document types');
    return await this.commonApi.fetchDocumentTypes();
  }
  async fetchWebsiteKeys(): Promise<Result<any>> {
    // console.log('Fetching website keys');
    return await this.commonApi.fetchWebsiteKeys();
  }

  async fetchEnWebsiteKeys(): Promise<Result<any>> {
    // console.log('Fetching EN website keys');
    return await this.commonApi.fetchEnWebsiteKeys();
  }
}
