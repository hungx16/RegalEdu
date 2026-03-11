// src/api/CourseApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { LearningRoadMapModel } from './LearningRoadMapApi';
import type { TuitionModel } from '@/api/TuitionApi'
import type { LectureTypeModel } from './LectureTypeApi';
import type { Attachment } from './FileApi';
import { CommitmentOutputType } from '@/types';

export interface CourseModel extends BaseEntityModel {
  courseCode: string;
  courseName: string;
  description?: string | null;
  sequence?: number;
  minAvgScore?: number;
  commitmentOutputType?: CommitmentOutputType;
  commitmentLevel?: string;
  learningRoadMapId?: string;
  learningRoadMap?: LearningRoadMapModel;
  midExamIds?: string[];
  finalExamIds?: string[];
  reference?: string;
  isPublish?: boolean;
  courseContent?: string;
  courseKey?: string;
  isMultilingual?: boolean;
  enCourseName?: string;
  enDescription?: string;
  enCourseContent?: string;
  enCourseKey?: string;
  duration?: string;
  enDuration?: string;
  numberOfStudents?: number;
  votingRate?: number;
  status?: number;
  ordinalNumber?: number;
  tuitions?: TuitionModel[] | null;
}
export interface CourseLessonModel extends BaseEntityModel {
  /** FK: Course/Tuition */
  tuitionId?: string;

  /** Tên buổi học (vd: "Buổi 01") - Required, max 50 */
  sessionName: string;

  /** FK: LectureType */
  lectureTypeId: string;

  /** Tên bài học trong buổi - max 255 */
  lessonName?: string | null;

  /** Mục tiêu sau buổi học - max 255 */
  objective?: string | null;

  /** Nội dung chi tiết giảng dạy - max 255 */
  content?: string | null;

  /** BTVN - max 500 */
  homework?: string | null;

  /** Tài liệu tham khảo (tên/link) - max 255 */
  reference?: string | null;

  /** Homework attachments */
  homeworkAttachments?: Attachment[];

  /** Reference attachments */
  referenceAttachments?: Attachment[];

  // Navigation (optional)
  tuition?: TuitionModel | null;
  lectureType?: LectureTypeModel | null;
}


export interface CourseQuery {
  page: number;
  pageSize: number;
  filters?: {
    courseCode?: string;
    courseName?: string;
    status?: number;
    isDeleted?: boolean;
  };
}

export interface CoursePagedResult {
  items: CourseModel[];
  total: number;
}

export class CourseApi extends ApiClient {
  controller = 'course';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  public async getPagedCourses(query: CourseQuery): Promise<Result<CoursePagedResult>> {
    return await this.get<Result<CoursePagedResult>>(
      `/${this.controller}/GetPagedCourses`,
      { params: query }
    );
  }

  public async getAllCourses(): Promise<Result<CourseModel[]>> {
    return await this.get<Result<CourseModel[]>>(`/${this.controller}/GetAllCourses`);
  }

  public async addCourse(data: Partial<CourseModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddCourse`, data);
  }

  public async updateCourse(data: Partial<CourseModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateCourse`, data);
  }

  public async deleteCourses(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListCourses`, { data: ids });
  }

  public async restoreCourses(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListCourses`, { data: ids });
  }

  public async getCourseById(id: string): Promise<Result<CourseModel>> {
    return await this.get<Result<CourseModel>>(`/${this.controller}/GetCourseById`, { params: { id } });
  }

  public async getDeletedCourses(): Promise<Result<CourseModel[]>> {
    return await this.get<Result<CourseModel[]>>(`/${this.controller}/GetDeletedCourses`);
  }
}
