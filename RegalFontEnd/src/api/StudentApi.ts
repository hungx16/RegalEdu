// src/api/StudentApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { EmployeeModel } from './EmployeeApi';
import type { StudentStatus } from '@/types';
//import { ApplicationUserModel } from './ApplicationUserApi';

export interface StudentNoteModel extends BaseEntityModel {
  studentId?: string | null;
  employeeId?: string | null;
  noteDate?: Date | string | null;
  noteContext?: string | null;
  employeeName?: string | null; // Dành cho hiển thị
}

export interface StudentActivityModel extends BaseEntityModel {
  studentId?: string | null;
  employeeId?: string | null;
  type?: string | null;        // Loại: 0-Gọi điện/ 1-Tin nhắn/...
  activityDate?: Date | string | null;
  results?: string | null;
  nextAction?: string | null;
  content?: string | null;
  //status?: number | null; // Trạng thái thay đổi công ty: 0 - Mới tạo, 1 - Đang xử lý, 2 - Hoàn thành
  // ... các trường CallLog khác
}

export interface StudentCourseModel extends BaseEntityModel {
  studentId?: string | null;
  courseId?: string | null;
  courseName?: string | null;
  interestLevel?: string | null; // Mức độ quan tâm
  reason?: string | null;        // Lý do học
}
export interface EnrollmentModel extends BaseEntityModel {
  studentId?: string | null;
  classId?: string | null;
  classTypeId?: string | null;
  courseId?: string | null;
  fee?: number | null;
  discount?: number | null;
  finalFee?: number | null;
  paymentCourseStatus?: number | null;
  paidAmount?: number | null;
  usableAmount?: number | null;
  startDate?: Date | string | null;
  endDate?: Date | string | null;
  studentCourseStatus?: number | null;
  times?: number | null;
  class?: any | null;
  classType?: any | null;
  course?: any | null;
  registerStudyId?: string | null;
  registerStudy?: any | null;
}
// --- II. Interface Contact (Dựa trên Contact.cs) ---

export interface ContactModel extends BaseEntityModel {
  fullName: string;
  phone?: string | null;
  email?: string | null;
  gender: number; // Gender Enum
  address?: string | null;
  relationship: number; // Relationship Enum
  note?: string | null;
  studentId?: string | null;
  username?: string | null;
  applicationUserId?: string | null;
}

export interface StudentModel extends BaseEntityModel {
  studentCode: string;
  fullName: string;
  gender?: string | null;
  birthDate?: Date | string;
  categoryId?: string | null;
  category?: any | null;
  employeeId?: string | null;
  employee?: EmployeeModel | null;
  address?: string | null;
  email?: string | null;
  phone?: string | null;
  companyId?: string | null;
  company?: any | null;
  reason?: string | null;
  currentLevel?: number | null;
  expectedTime?: number | null;
  expectedWorkingTime?: string | null;
  learningGoal?: string | null;
  englishExperience?: string | null;
  leadSource?: string | null;
  studentStatus: StudentStatus;
  enrollments?: EnrollmentModel[] | null;
  applicationUserId?: string | null;
  applicationUser?: any | null;
  englishName?: string | null;
  identifyNumber?: string | null;
  age?: number | null;
  expectedStartDate?: Date | string | null;
  registerStudys?: any[] | null;
  contacts?: ContactModel[] | null;
  coupons?: any[] | null;
  studentNote?: StudentNoteModel[] | null;
  studentActivity?: StudentActivityModel[] | null;
  studentCourse?: StudentCourseModel[] | null;
  priority?: number | null;
  expectedBudget?: number | null;
  profile?: any | null;
  regionId?: string | null;
  region?: any | null;
  employeeName?: string | null;
  companyName?: string | null;

}

// --- III. Query và Paged Result ---

export interface StudentQuery {
  page: number;
  pageSize: number;
  filters?: {
    searchTerm?: string;
    status?: number; // Trạng thái học viên/Lead
    source?: string; // Nguồn Lead
    priority?: number; // Độ ưu tiên (Giả định được tính toán hoặc là trường trong Student/Contact)
  };
}

export interface StudentPagedResult {
  items: StudentModel[];
  total: number;
}


export class StudentApi extends ApiClient {
  controller = 'student';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  /** Lấy danh sách học viên có phân trang */
  public async getPagedStudents(query: StudentQuery): Promise<Result<StudentPagedResult>> {
    return await this.get<Result<StudentPagedResult>>(`/${this.controller}/GetPagedStudents`, { params: query });
  }

  /** Lấy toàn bộ học viên */
  public async getAllStudents(): Promise<Result<StudentModel[]>> {
    return await this.get<Result<StudentModel[]>>(`/${this.controller}/GetAllStudents`);
  }
  /** Lấy danh sách học viên có phân trang */
  public async getPagedCustoms(query: StudentQuery): Promise<Result<StudentPagedResult>> {
    return await this.get<Result<StudentPagedResult>>(`/${this.controller}/getPagedCustoms`, { params: query });
  }

  /** Lấy toàn bộ học viên */
  public async getAllCustoms(): Promise<Result<StudentModel[]>> {
    return await this.get<Result<StudentModel[]>>(`/${this.controller}/GetAllCustoms`);
  }
  /** Thêm mới học viên */
  public async addStudent(data: Partial<StudentModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddStudent`, data);
  }

  /** Cập nhật học viên */
  public async updateStudent(data: Partial<StudentModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateStudent`, data);
  }

  /** Xóa một hoặc nhiều học viên */
  public async deleteStudents(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListStudent`, { data: ids });
  }

  /** Khôi phục một hoặc nhiều học viên */
  public async restoreStudents(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListStudent`, { data: ids });
  }

  /** Lấy thông tin chi tiết học viên */
  public async getStudentById(id: string): Promise<Result<StudentModel>> {
    return await this.get<Result<StudentModel>>(`/${this.controller}/GetStudentById`, { params: { id } });
  }
  /** Lấy thông tin chi tiết học viên */
  public async getStudentByStudentCode(code: string): Promise<Result<StudentModel>> {
    return await this.get<Result<StudentModel>>(`/${this.controller}/GetStudentByStudentCode`, { params: { code } });
  }

  /** Lấy danh sách học viên đã xóa */
  public async getDeletedStudents(): Promise<Result<StudentModel[]>> {
    return await this.get<Result<StudentModel[]>>(`/${this.controller}/GetDeletedStudents`);
  }
}
