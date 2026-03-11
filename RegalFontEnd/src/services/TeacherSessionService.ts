import type {
  AddTeacherWorkLogCommand,
  GetAllTeacherSessionsQuery,
  GetTeacherWorkBoardQuery,
  ReassignTeacherSessionsCommand,
} from '@/api/TeacherSessionApi'
import type { TeacherSessionApi } from '@/api/TeacherSessionApi'
import type { Result } from '@/types/Result'
import type { TeacherSessionItemModel } from '@/api/TeacherSessionApi'

export class TeacherSessionService {
  private api: TeacherSessionApi

  constructor(apiInstance: TeacherSessionApi) {
    this.api = apiInstance
  }

  getAllTeacherSessions(
    query: GetAllTeacherSessionsQuery
  ): Promise<Result<TeacherSessionItemModel[]>> {
    return this.api.getAllTeacherSessions(query)
  }

  getTeacherWorkBoard(
    query: GetTeacherWorkBoardQuery
  ): Promise<Result<TeacherSessionItemModel[]>> {
    return this.api.getTeacherWorkBoard(query)
  }

  reassignTeacherSessions(command: ReassignTeacherSessionsCommand): Promise<Result<any>> {
    return this.api.reassignTeacherSessions(command)
  }

  addTeacherWorkLog(command: AddTeacherWorkLogCommand): Promise<Result<any>> {
    return this.api.addTeacherWorkLog(command)
  }
}
