import { ApiClient } from '@/api/ApiClient'
export interface Attachment {
  id?: string | null
  path?: string | null
  file?: File | null
  fileName?: string | null
  [key: string]: any
  uid?: string
}
export interface FieldOption {
  label: string
  value: string | number | boolean
  disabled?: boolean
}

export interface FieldSchema {
  key: string
  label: string
  type: 'text' | 'textarea' | 'number' | 'switch' | 'select'
  placeholder?: string
  activeText?: string
  inactiveText?: string
  span?: number
  options?: FieldOption[]
  disabled?: boolean
  defaultValue?: string | number | boolean
}

export interface UploadedFileDto {
  relativePath: string
  fileName: string
  size: number
  contentType?: string
}
export class FileApi extends ApiClient {
  controller: string = 'file'
  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  public async downloadFile(fileUrl: string): Promise<Blob> {
    return this.get(`/${this.controller}/download`, {
      params: { file: fileUrl },
      responseType: 'blob'
    });
  }
  public async downloadByAttachmentId(id: string): Promise<Blob> {
    return this.get(`/${this.controller}/download-by-attachment`, {
      params: { id },
      responseType: 'blob'
    });
  }
  // Upload TEMP (chung thư mục temp/)
  async uploadTemp(files: File[]): Promise<UploadedFileDto[]> {
    const fd = new FormData()
    files.forEach(f => fd.append('files', f))

    const res = await this.put(`/${this.controller}/temp`, fd, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
    const payload = (res as any)?.data ?? res
    return (payload?.data ?? payload) as UploadedFileDto[]
  }

  async uploadTempOne(file: File): Promise<UploadedFileDto> {
    const list = await this.uploadTemp([file])
    return list[0]
  }

  async deleteTemp(path: string): Promise<void> {
    await this.delete(`/${this.controller}/temp`, { params: { path } })
  }
}
