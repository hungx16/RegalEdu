import type {
  EvaluateTeacherApi,
  EvaluateTeacherModel,
  EvaluateTeacherPagedResult,
  EvaluateTeacherQuery,
  EvaluateTeacherSummary,
  EvaluateTeacherSummaryQuery,
  RespondEvaluateTeacherCommand,
} from '@/api/EvaluateTeacherApi';
import type { Result } from '@/types/Result';

export class EvaluateTeacherService {
  private api: EvaluateTeacherApi;

  constructor(apiInstance: EvaluateTeacherApi) {
    this.api = apiInstance;
  }

  addEvaluateTeacher(model: EvaluateTeacherModel): Promise<Result<any>> {
    return this.api.addEvaluateTeacher(model);
  }

  updateEvaluateTeacher(model: EvaluateTeacherModel): Promise<Result<any>> {
    return this.api.updateEvaluateTeacher(model);
  }

  saveEvaluateTeacher(model: EvaluateTeacherModel): Promise<Result<any>> {
    return model.id
      ? this.updateEvaluateTeacher(model)
      : this.addEvaluateTeacher(model);
  }

  deleteEvaluateTeacher(id: string): Promise<Result<any>> {
    return this.api.deleteEvaluateTeacher(id);
  }

  respondEvaluateTeacher(
    command: RespondEvaluateTeacherCommand
  ): Promise<Result<any>> {
    return this.api.respondEvaluateTeacher(command);
  }

  getEvaluateTeacherById(id: string): Promise<Result<EvaluateTeacherModel>> {
    return this.api.getEvaluateTeacherById(id);
  }

  getPagedEvaluateTeachers(
    query: EvaluateTeacherQuery
  ): Promise<Result<EvaluateTeacherPagedResult>> {
    return this.api.getPagedEvaluateTeachers(query);
  }

  getAllEvaluateTeachers(): Promise<Result<EvaluateTeacherModel[]>> {
    return this.api.getAllEvaluateTeachers();
  }

  getTeacherEvaluations(
    teacherId: string
  ): Promise<Result<EvaluateTeacherModel[]>> {
    return this.api.getTeacherEvaluations(teacherId);
  }

  getEvaluateTeacherSummary(
    query: EvaluateTeacherSummaryQuery
  ): Promise<Result<EvaluateTeacherSummary>> {
    return this.api.getEvaluateTeacherSummary(query);
  }
}
