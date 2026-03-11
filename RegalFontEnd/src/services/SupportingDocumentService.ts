// src/services/SupportingDocumentService.ts
import type { Result } from '@/types/Result';
import type { SupportingDocumentApi } from '../api/SupportingDocumentApi';
import type { DeleteSupportingDocumentRequest, SupportingDocumentModel, SupportingDocumentQuery } from '../api/SupportingDocumentApi';
import { useNotificationStore } from '@/stores/notificationStore';

export class SupportingDocumentService {
  private supportingDocumentApi: SupportingDocumentApi;

  constructor(supportingDocumentApiInstance: SupportingDocumentApi) {
    this.supportingDocumentApi = supportingDocumentApiInstance;
  }

  async fetchPagedSupportingDocuments(query: SupportingDocumentQuery): Promise<Result<any>> {
    return await this.supportingDocumentApi.getPagedSupportingDocuments(query);
  }

  async fetchAllSupportingDocuments(): Promise<Result<any>> {
    return await this.supportingDocumentApi.getAllSupportingDocuments();
  }

  async saveSupportingDocument(supportingDocument: Partial<SupportingDocumentModel>): Promise<any> {
    let result: any;
    if (supportingDocument.id) {
      result = await this.supportingDocumentApi.updateSupportingDocument(supportingDocument);
    } else {
      result = await this.supportingDocumentApi.addSupportingDocument(supportingDocument);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  async deleteSupportingDocuments(request: DeleteSupportingDocumentRequest): Promise<void> {
    const result: any = await this.supportingDocumentApi.deleteSupportingDocuments(request);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  async restoreSupportingDocuments(request: DeleteSupportingDocumentRequest): Promise<void> {
    const result: any = await this.supportingDocumentApi.restoreSupportingDocuments(request);
    if (!result.succeeded) {
      throw new Error(result.error || 'Restore failed');
    }
  }
  async fetchDeletedSupportingDocuments(): Promise<Result<any>> {
    return await this.supportingDocumentApi.getDeletedSupportingDocuments();
  }
}
