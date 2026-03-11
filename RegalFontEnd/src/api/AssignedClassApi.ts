// src/api/AssignedClassApi.ts
import { ApiClient } from '@/api/ApiClient'
import type { Result } from '@/types/Result'
import type { StudentModel } from './StudentApi'

export interface ManualAssignStudentCommand {
  classId: string
  studentId: string
  confirmOverrideClassStatus?: boolean
  confirmOverrideMaxStudents?: boolean
  confirmOverrideScheduleConflict?: boolean
}

export interface ManualUnassignStudentCommand {
  classId: string
  studentId: string
}

export interface AutoAssignStudentsCommand {
  classId: string
}

export class AssignedClassApi extends ApiClient {
  controller = 'assignedclass'

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  public async getAssignableStudents(classId: string): Promise<Result<StudentModel[]>> {
    return await this.get<Result<StudentModel[]>>(`/${this.controller}/GetAssignableStudentsByClassId`, { params: { classId } })
  }

  public async getAssignedStudentsByClassId(classId: string): Promise<Result<StudentModel[]>> {
    return await this.get<Result<StudentModel[]>>(`/${this.controller}/GetAssignedStudentsByClassId`, { params: { classId } })
  }

  public async manualAssignStudent(command: ManualAssignStudentCommand): Promise<Result<StudentModel>> {
    return await this.post<Result<StudentModel>>(`/${this.controller}/ManualAssignStudent`, command)
  }

  public async manualUnassignStudent(command: ManualUnassignStudentCommand): Promise<Result<StudentModel>> {
    return await this.post<Result<StudentModel>>(`/${this.controller}/ManualUnassignStudent`, command)
  }

  public async autoAssignStudents(command: AutoAssignStudentsCommand): Promise<Result<StudentModel[]>> {
    return await this.post<Result<StudentModel[]>>(`/${this.controller}/AutoAssignStudents`, command)
  }
}
