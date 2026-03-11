// src/services/StudentService.ts
import type { StudentModel, StudentQuery } from '@/api/StudentApi';
import type { StudentApi } from '@/api/StudentApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class StudentService {
  private studentApi: StudentApi;

  constructor(studentApiInstance: StudentApi) {
    this.studentApi = studentApiInstance;
  }

  /** Lấy danh sách học viên có phân trang */
  async fetchPagedStudents(query: StudentQuery): Promise<Result<any>> {
    return await this.studentApi.getPagedStudents(query);
  }

  /** Lấy toàn bộ học viên */
  async fetchAllStudents(): Promise<Result<any>> {
    return await this.studentApi.getAllStudents();
  }
  /** Lấy danh sách học viên có phân trang */
  async fetchPagedCustoms(query: StudentQuery): Promise<Result<any>> {
    return await this.studentApi.getPagedCustoms(query);
  }

  /** Lấy toàn bộ học viên */
  async fetchAllCustoms(): Promise<Result<any>> {
    return await this.studentApi.getAllCustoms();
  }

  /** Lưu hoặc cập nhật học viên */
  async saveStudent(student: Partial<StudentModel>): Promise<any> {
    let result: any;
    if (student.id) {
      // Clear các field quan hệ không cần gửi lên để tránh lỗi
      (student as any).class = null;
      (student as any).advisor = null;

      result = await this.studentApi.updateStudent(student);
    } else {
      result = await this.studentApi.addStudent(student);
    }
    if (!result.succeeded) {
      throw new Error(result.error || 'Save failed');
    }
    return result.data;
  }

  /** Xóa một hoặc nhiều học viên */
  async deleteStudents(studentIds: string[]): Promise<void> {
    let result: any = await this.studentApi.deleteStudents(studentIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Delete failed');
    } else {
      useNotificationStore().showToast('success', { key: result.data });
    }
  }

  /** Khôi phục học viên đã xóa */
  async restoreStudents(studentIds: string[]): Promise<void> {
    let result: any = await this.studentApi.restoreStudents(studentIds);
    if (!result.succeeded) {
      throw new Error(result.error || 'Restore failed');
    }
  }

  /** Lấy danh sách học viên đã xóa */
  async fetchDeletedStudents(): Promise<Result<any>> {
    return await this.studentApi.getDeletedStudents();
  }
  async getStudentById(id: string): Promise<Result<any>> {
    return await this.studentApi.getStudentById(id);
  }

  /** Lấy học viên theo mã học viên */
  async getStudentByCode(code: string): Promise<Result<StudentModel>> {
    return await this.studentApi.getStudentByStudentCode(code);
  }
}
