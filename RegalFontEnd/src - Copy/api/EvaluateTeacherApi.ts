import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { TeacherModel } from './TeacherApi';
import type { ClassScheduleModel } from './ClassScheduleApi';

export interface EvaluateTeacherModel extends BaseEntityModel {
  teacherId: string;
  teacher?: TeacherModel | null;
  classId?: string | null;
  classCode?: string | null;
  className?: string | null;
  class?: { id?: string; classCode?: string | null; className?: string | null } | null;
  classScheduleId?: string | null;
  classScheduleName?: string | null;
  classSchedule?: ClassScheduleModel | null;
  studentId?: string | null;
  studentName?: string | null;
  student?: { id?: string; fullName?: string | null } | null;
  evaluateName?: string | null;
  evaluateNick?: string | null;
  evaluateDate?: string | null;
  evaluateType?: string | null;
  starRating?: number | null;
  responseContent?: string | null;
}

export interface EvaluateTeacherQuery {
  page: number;
  pageSize: number;
  teacherId?: string;
  studentId?: string;
  classId?: string;
  classScheduleId?: string;
  fromDate?: string;
  toDate?: string;
  evaluateType?: string;
  keyword?: string;
}

export interface EvaluateTeacherPagedResult {
  items: EvaluateTeacherModel[];
  total: number;
}

export interface RespondEvaluateTeacherCommand {
  id?: string;
  evaluateTeacherId?: string;
  responseContent: string;
}

export interface EvaluateTeacherSummaryQuery {
  teacherId?: string;
  fromDate?: string;
  toDate?: string;
}

export interface EvaluateTeacherSummary {
  totalEvaluations?: number;
  averageRating?: number | null;
  [key: string]: any;
}

export class EvaluateTeacherApi extends ApiClient {
  controller = 'EvaluateTeacher';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async addEvaluateTeacher(model: EvaluateTeacherModel): Promise<Result<any>> {
    return await this.post<Result<any>>(
      `/${this.controller}/AddEvaluateTeacher`,
      model
    );
  }

  async updateEvaluateTeacher(model: EvaluateTeacherModel): Promise<Result<any>> {
    return await this.put<Result<any>>(
      `/${this.controller}/UpdateEvaluateTeacher`,
      model
    );
  }

  async deleteEvaluateTeacher(id: string): Promise<Result<any>> {
    return await this.delete<Result<any>>(
      `/${this.controller}/DeleteEvaluateTeacher`,
      { params: { id } }
    );
  }

  async respondEvaluateTeacher(
    command: RespondEvaluateTeacherCommand
  ): Promise<Result<any>> {
    return await this.post<Result<any>>(
      `/${this.controller}/RespondEvaluateTeacher`,
      command
    );
  }

  async getEvaluateTeacherById(id: string): Promise<Result<EvaluateTeacherModel>> {
    return await this.get<Result<EvaluateTeacherModel>>(
      `/${this.controller}/GetEvaluateTeacherById`,
      { params: { id } }
    );
  }

  async getPagedEvaluateTeachers(
    query: EvaluateTeacherQuery
  ): Promise<Result<EvaluateTeacherPagedResult>> {
    return await this.get<Result<EvaluateTeacherPagedResult>>(
      `/${this.controller}/GetPagedEvaluateTeachers`,
      { params: query }
    );
  }

  async getAllEvaluateTeachers(): Promise<Result<EvaluateTeacherModel[]>> {
    return await this.get<Result<EvaluateTeacherModel[]>>(
      `/${this.controller}/GetAllEvaluateTeachers`
    );
  }

  async getTeacherEvaluations(
    teacherId: string
  ): Promise<Result<EvaluateTeacherModel[]>> {
    return await this.get<Result<EvaluateTeacherModel[]>>(
      `/${this.controller}/GetTeacherEvaluations`,
      { params: { teacherId } }
    );
  }

  async getEvaluateTeacherSummary(
    query: EvaluateTeacherSummaryQuery
  ): Promise<Result<EvaluateTeacherSummary>> {
    return await this.get<Result<EvaluateTeacherSummary>>(
      `/${this.controller}/GetEvaluateTeacherSummary`,
      { params: query }
    );
  }
}
