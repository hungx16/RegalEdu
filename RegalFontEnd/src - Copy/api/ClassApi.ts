// src/api/ClassApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient'
import type { Result } from '@/types/Result'
import type { ClassScheduleModel } from '@/api/ClassScheduleApi'

export interface ClassModel extends BaseEntityModel {
  classCode: string
  className: string
  courseId: string
  companyId: string
  method: number
  startDate: string
  endDate?: string | null
  description?: string | null
  trialClass: boolean
  classStatus: number
  classTypeId: string
  teacherId?: string | null
  employeeId?: string | null
  classSchedule?: string | null
  course?: any
  company?: any
  teacher?: any
  classType?: any
  employee?: any
}

export interface ClassQuery {
  page: number
  pageSize: number
  filters?: {
    classCode?: string
    className?: string
    classStatus?: number
    method?: number
    companyId?: string
    courseId?: string
    isDeleted?: boolean
  }
}

export interface ClassPagedResult {
  items: ClassModel[]
  total: number
}

export class ClassApi extends ApiClient {
  controller = 'class'

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string)
  }

  public async getPagedClass(query: ClassQuery): Promise<Result<ClassPagedResult>> {
    return await this.get<Result<ClassPagedResult>>(`/${this.controller}/GetPagedClass`, { params: query })
  }

  public async getAllClass(): Promise<Result<ClassModel[]>> {
    return await this.get<Result<ClassModel[]>>(`/${this.controller}/GetAllClass`)
  }

  public async addClass(data: Partial<ClassModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddClass`, data)
  }

  public async updateClass(data: Partial<ClassModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdateClass`, data)
  }

  public async deleteClasses(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListClass`, { data: ids })
  }

  public async getClassById(id: string): Promise<Result<ClassModel>> {
    return await this.get<Result<ClassModel>>(`/${this.controller}/GetClassById`, { params: { id } })
  }
  public async getClassByTeacherId(id: string): Promise<Result<ClassModel[]>> {
    return await this.get<Result<ClassModel[]>>(`/${this.controller}/GetClassByTeacherId`, { params: { id } })
  }

  public async getClassSchedulesByHomeTeacher(): Promise<Result<ClassScheduleModel[]>> {
    return await this.get<Result<ClassScheduleModel[]>>(`/${this.controller}/GetClassSchedulesByHomeTeacher`)
  }
}
