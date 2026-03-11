// src/api/ClassScoreBoardApi.ts
import { ApiClient } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export enum ScoreType {
  Midterm = 1,
  EndTerm = 2
}

export interface UpdateScoreItem {
  studentId: string;
  categoryId?: string;
  score?: number | null;
  comment?: string | null;
}

export interface UpdateClassScoreBoardModel {
  classId: string;
  scoreType: ScoreType;
  scores: UpdateScoreItem[];
}

export interface ClassScoreSummaryModel {
  classId: string;
  studentId: string;
  studentName?: string | null;
  nickname?: string | null;
  comment?: string | null;
  rating?: number | null;
  updatedAt?: string | null;
  midtermAvg?: number | null;
  endTermAvg?: number | null;
  summaryScore?: number | null;
  isPass: boolean;
}

export interface ClassScoreBoardModel {
  id?: string;
  classId: string;
  studentId: string;
  categoryId?: string | null;
  scoreType: ScoreType;
  score?: number | null;
  studentName?: string | null;
  categoryName?: string | null;
}

export class ClassScoreBoardApi extends ApiClient {
  controller = 'ClassScoreBoard';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  async updateClassScoreBoard(model: UpdateClassScoreBoardModel): Promise<Result<any>> {
    // Backend expects payload property scoreBoardModel
    return await this.put<Result<any>>(`/${this.controller}/UpdateClassScoreBoard`, {
      scoreBoardModel: model
    });
  }

  async getClassScoreBoardByClassId(classId: string): Promise<Result<ClassScoreSummaryModel[]>> {
    return await this.get<Result<ClassScoreSummaryModel[]>>(
      `/${this.controller}/GetClassScoreBoardByClassId`,
      { params: { classId } }
    );
  }

  async getClassScoresByClassId(classId: string): Promise<Result<ClassScoreBoardModel[]>> {
    return await this.get<Result<ClassScoreBoardModel[]>>(
      `/${this.controller}/GetClassScoresByClassId`,
      { params: { classId } }
    );
  }
}
