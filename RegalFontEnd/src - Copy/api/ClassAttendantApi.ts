// src/api/ClassAttendantApi.ts
import axios from 'axios'
import { ApiClient } from '@/api/ApiClient'
import type { Result } from '@/types/Result'
import type { ClassScheduleModel } from './ClassScheduleApi'
import type { StudentModel } from './StudentApi'
import type { StudentHomeworkStatus, StudentParticipationStatus } from '@/types'

export interface ClassAttendantModel {
  id?: string
  classScheduleId: string
  classSchedule?: ClassScheduleModel | null
  studentId: string
  student?: StudentModel | null
  studentParticipationStatus: StudentParticipationStatus
  studentHomeworkStatus: StudentHomeworkStatus
  comment?: string | null
  star?: number | null
  attachment?: string | null
  note?: string | null
  homeworkScore?: number | null
  createdAt?: string
  updatedAt?: string
  createdBy?: string | null,
  participationLevel?: number | null, // mức độ tham gia
  learningAbsorptionLevel?: number | null, // mức độ tiếp thu bài
  disciplineLevel?: number | null, // thời gian học tập & kỷ luật

}

export interface UpdateClassAttendantCommand {
  scheduleId: string
  attendants: ClassAttendantModel[]
}

export class ClassAttendantApi extends ApiClient {
  controller = 'ClassAttendant'
  private rawBaseUrl = import.meta.env.VITE_APP_API_URL as string

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  public async getClassAttendantsByScheduleId(scheduleId: string): Promise<Result<ClassAttendantModel[]>> {
    return await this.get<Result<ClassAttendantModel[]>>(
      `/${this.controller}/GetClassAttendantsByScheduleId`,
      {
        params: { id: scheduleId },
        // Cho phép 404 để coi như không có dữ liệu, tránh toast lỗi
        validateStatus: (status) => (status ?? 500) === 404 || ((status ?? 0) >= 200 && (status ?? 0) < 300)
      }
    )
  }

  public async updateClassAttendant(command: UpdateClassAttendantCommand): Promise<Result<ClassAttendantModel[]>> {
    return await this.post<Result<ClassAttendantModel[]>>(
      `/${this.controller}/UpdateClassAttendent`,
      command.attendants,
      { params: { scheduleId: command.scheduleId } }
    )
  }
}
