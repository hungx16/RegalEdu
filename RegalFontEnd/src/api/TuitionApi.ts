// src/api/TuitionApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { PriceTargetType, UnitType } from '@/types';
import type { Result } from '@/types/Result';
import type { CourseLessonModel, CourseModel } from './CourseApi';
import type { ClassTypeModel } from './ClassTypeApi';

export interface TuitionModel extends BaseEntityModel {
  tuitionCode: string;
  /** Tên học phí */
  tuitionName: string

  /** Khóa ngoại: CourseId */
  courseId?: string | null
  course?: CourseModel | null

  /** Khóa ngoại: ClassTypeId */
  classTypeId: string
  classType?: ClassTypeModel | null

  /** Tổng số giờ học (1–1000) */
  durationHours: number

  /** Số giờ đăng ký tối thiểu (≤ durationHours) */
  minHours: number

  /** Số tháng học (≤100) */
  totalMonths: number

  /** Đơn vị tính (Hour/Session/Month/Course) */
  unit?: UnitType

  /** Học phí (decimal(18,2) bên backend) */
  tuitionFee?: number
  courseLessons?: CourseLessonModel[],
  startDate?: string | null;
  endDate?: string | null;
}
export interface TuitionDetailModel extends BaseEntityModel {
  tuitionId: string;

  targetType: PriceTargetType; // default: Course

  //courseId?: string | null;
  //course?: CourseModel | null;

  courseId?: string | null;
  classTypeId?: string | null;
  course: CourseModel | null;
  classType: ClassTypeModel | null;
  quantity?: number | null;  // decimal(18,2)
  minQty?: number | null;    // decimal(18,2)

  monthQty?: number | null;

  unit: UnitType;            // default: Hour

  fee: number;               // decimal(18,2), required
}

export interface TuitionQuery {
  page: number;
  pageSize: number;
  filters?: {
    priceName?: string;
    status?: number;
    isDeleted?: boolean;
  };
}

export interface TuitionPagedResult {
  items: TuitionModel[];
  total: number;
}

export class TuitionApi extends ApiClient {
  controller = 'tuition';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedTuitions(query: TuitionQuery): Promise<Result<TuitionPagedResult>> {
    return await this.get<Result<TuitionPagedResult>>(`/${this.controller}/GetPagedTuitions`, { params: query });
  }

  public async getAllTuitions(): Promise<Result<TuitionModel[]>> {
    return await this.get<Result<TuitionModel[]>>(`/${this.controller}/GetAllTuitions`);
  }

  public async addTuition(data: Partial<TuitionModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddTuition`, data);
  }

  public async updateTuition(data: Partial<TuitionModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateTuition`, data);
  }

  public async deleteTuitions(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListTuition`, { data: ids });
  }

  public async restoreTuitions(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListTuition`, { data: ids });
  }

  public async getTuitionById(id: string): Promise<Result<TuitionModel>> {
    return await this.get<Result<TuitionModel>>(`/${this.controller}/GetTuitionById`, { params: { id } });
  }

  public async getDeletedTuitions(): Promise<Result<TuitionModel[]>> {
    return await this.get<Result<TuitionModel[]>>(`/${this.controller}/GetDeletedTuitions`);
  }
}
