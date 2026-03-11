// src/api/ClassScheduleApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient'
import type { Result } from '@/types/Result'
import type { ClassScheduleStatus, SessionAttendanceStatus, SessionAttendanceLockStatus } from '@/types'
import type { CourseLessonModel } from './CourseApi'
import type { Attachment } from './FileApi'

export interface ClassScheduleModel extends BaseEntityModel {
  classId: string
  teacherId?: string | null
  courseLessonId?: string | null
  courseLesson?: CourseLessonModel | null
  teacher?: { fullName?: string; applicationUser?: { fullName?: string } } | null
  date: string
  startTime?: string | null
  endTime?: string | null
  dayOfWeek: number
  classScheduleStatus: ClassScheduleStatus
  content?: string | null
  sessionAttendanceStatus: SessionAttendanceStatus
  attender?: string | null
  plan?: string | null
  homeworkPlusName?: string | null
  homeworkPlusContent?: string | null
  sessionAttendanceLockStatus: SessionAttendanceLockStatus
  // client side helper
  sessionIndex?: number
  attachment?: Attachment | null;
}
export interface UpdateSessionAttendanceLockStatusesCommand {
  sessionIds: string[]
  newStatus: SessionAttendanceLockStatus
}

export interface CancelClassScheduleByShiftingCommand {
  classId: string
  classScheduleId: string
  cancelReason: string
}
export interface CancelClassScheduleWithSubstitutionCommand {
  classId: string
  classScheduleId: string
  cancelReason: string
  substitutionDate: string
  substitutionStartTime: string
  substituteTeacherId?: string | null
}
export class ClassScheduleApi extends ApiClient {
  controller = 'classschedule'

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  async getScheduleById(id: string): Promise<Result<ClassScheduleModel>> {
    return await this.get<Result<ClassScheduleModel>>(`/${this.controller}/GetScheduleById`, { params: { id } })
  }

  async getSchedulesByClassId(classId: string): Promise<Result<ClassScheduleModel[]>> {
    return await this.get<Result<ClassScheduleModel[]>>(`/${this.controller}/GetSchedulesByClassId`, { params: { classId } })
  }

  async updateSchedule(data: Partial<ClassScheduleModel>): Promise<Result<ClassScheduleModel>> {
    return await this.put<Result<ClassScheduleModel>>(`/${this.controller}/UpdateSchedule`, data)
  }

  async updateSessionAttendanceLockStatus(sessionId: string, newStatus: SessionAttendanceLockStatus): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateSessionAttendanceLockStatus`, {
      sessionId,
      newStatus
    })
  }

  async updateSessionAttendanceLockStatuses(command: UpdateSessionAttendanceLockStatusesCommand): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateSessionAttendanceLockStatuses`, command)
  }

  async confirmSessionAttendance(sessionId: string): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/ConfirmSessionAttendance`, { sessionId })
  }
  async cancelAndShift(command: CancelClassScheduleByShiftingCommand): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/CancelAndShift`, command)
  }
  async cancelAndSubstitute(command: CancelClassScheduleWithSubstitutionCommand): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/CancelAndSubstitute`, command)
  }
}
