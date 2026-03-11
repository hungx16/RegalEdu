// src/services/DivisionService.ts
import type { FileApi, UploadedFileDto } from '@/api/FileApi';


export class FileService {
  private fileApi: FileApi;

  constructor(FileApiInstance: FileApi) {
    this.fileApi = FileApiInstance;
  }

  async downloadFile(fileName: string): Promise<Blob> {
    // console.log('Downloading file with name:', fileName);
    return await this.fileApi.downloadFile(fileName);
  }
  // Upload TEMP (chung thư mục temp/)
  async uploadTemp(files: File[]): Promise<UploadedFileDto[]> {

    return await this.fileApi.uploadTemp(files);
  }
  async deleteTemp(path: string): Promise<void> {
    return await this.fileApi.deleteTemp(path);
  }
}
