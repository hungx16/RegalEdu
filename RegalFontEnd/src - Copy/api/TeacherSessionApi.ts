import { ApiClient } from '@/api/ApiClient'
import type { Result } from '@/types/Result'
import type { ClassScheduleStatus } from '@/types'
import type { WorkingTimeModel } from './WorkingTimeApi'

export interface TeacherSessionItemModel {
  scheduleId: string
  classId: string
  classCode?: string | null
  className?: string | null
  branchName?: string | null
  courseId?: string | null
  courseCode?: string | null
  courseName?: string | null
  programName?: string | null
  levelName?: string | null
  moduleName?: string | null
  teacherId?: string | null
  teacherCode?: string | null
  teacherName?: string | null
  teacherEnglishName?: string | null
  date: string
  startTime?: string | null
  endTime?: string | null
  dayOfWeek: number
  scheduleName?: string | null
  activityTypeName?: string | null
  shiftTimeRange?: string | null
  classScheduleStatus: ClassScheduleStatus
}

export interface GetAllTeacherSessionsQuery {
  teacherId?: string
  fromDate?: string
  toDate?: string
}

export interface GetTeacherWorkBoardQuery {
  teacherId: string
  fromDate?: string
  toDate?: string
}

export interface ReassignTeacherSessionsCommand {
  classId: string
  fromTeacherId: string
  toTeacherId: string
  fromDate?: string
  toDate?: string
}

export interface TeacherWorkLogModel {
  teacherId: string
  workType: number
  date: string
  startTime: string
  endTime: string
  note?: string | null
  workingTimeId?: string | null
  workingTime?: WorkingTimeModel | null
}

export interface AddTeacherWorkLogCommand {
  teacherWorkLogModel: TeacherWorkLogModel
}

export class TeacherSessionApi extends ApiClient {
  controller = 'TeacherSessions'

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  async getAllTeacherSessions(
    query: GetAllTeacherSessionsQuery
  ): Promise<Result<TeacherSessionItemModel[]>> {
    return await this.get<Result<TeacherSessionItemModel[]>>(
      `/${this.controller}/GetAllTeacherSessions`,
      { params: query }
    )
  }

  async getTeacherWorkBoard(
    query: GetTeacherWorkBoardQuery
  ): Promise<Result<TeacherSessionItemModel[]>> {
    return await this.get<Result<TeacherSessionItemModel[]>>(
      `/${this.controller}/GetTeacherWorkBoard`,
      { params: query }
    )
  }

  async reassignTeacherSessions(
    command: ReassignTeacherSessionsCommand
  ): Promise<Result<any>> {
    return await this.post<Result<any>>(
      `/${this.controller}/ReassignTeacherSessions`,
      command
    )
  }

  async addTeacherWorkLog(command: AddTeacherWorkLogCommand): Promise<Result<any>> {
    return await this.post<Result<any>>(
      `/${this.controller}/AddTeacherWorkLog`,
      command
    )
  }
}
