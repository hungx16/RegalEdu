// src/stores/fileStore.ts
import { defineStore } from 'pinia';
import { serviceFactory } from '@/services/ServiceFactory';

export const useFileStore = defineStore('file', {
  state: () => ({}),
  actions: {
    // fileUrl: đường dẫn file trong DB, originalFileName: tên gốc muốn hiển thị cho user khi download
    async downloadFile(fileUrl: string, originalFileName?: string) {
      try {
        const blob = await serviceFactory.fileService.downloadFile(fileUrl);
        const fileName =
          originalFileName ||
          (fileUrl.split('/').pop() || 'file_download');
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
      } catch (error) {
        // Có thể show toast ở đây
        console.error('Error downloading file:', error);
      }
    },
    async uploadTemp(files: File[]) {
      try {
        return await serviceFactory.fileService.uploadTemp(files);
      } catch (error) {
        console.error('Error uploading files:', error);
        throw error; // Ném lại lỗi để xử lý ở component nếu cần
      }
    },
    async deleteTemp(path: string) {
      try {
        return await serviceFactory.fileService.deleteTemp(path);
      } catch (error) {
        console.error('Error deleting temp file:', error);
        throw error;
      }
    },
  },
});
