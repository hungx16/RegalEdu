// src/api/DepartmentApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface DepartmentModel extends BaseEntityModel {
    departmentCode: string;
    departmentName: string;
    enDepartmentName?: string | null;
    divisionId: string;
    division?: any; // type DivisionModel nếu cần
    departmentParentId?: string | null;
    departmentParent?: any;
    description?: string | null;
    // status: number;
}

export interface DepartmentQuery {
    page: number;
    pageSize: number;
    filters?: {
        departmentCode?: string;
        departmentName?: string;
        divisionId?: string;
        status?: number;
    };
}

export interface DepartmentPagedResult {
    items: DepartmentModel[];
    total: number;
}

export class DepartmentApi extends ApiClient {
    controller = 'department';

    constructor() {
        super(import.meta.env.VITE_APP_API_URL as string);
    }

    public async getPagedDepartments(query: DepartmentQuery): Promise<Result<DepartmentPagedResult>> {
        return await this.get<Result<DepartmentPagedResult>>(`/${this.controller}/GetPagedDepartments`, { params: query });
    }

    public async getAllDepartments(): Promise<Result<DepartmentModel[]>> {
        return await this.get<Result<DepartmentModel[]>>(`/${this.controller}/GetAllDepartments`);
    }

    public async addDepartment(data: Partial<DepartmentModel>): Promise<Result<any>> {
        return await this.post<Result<any>>(`/${this.controller}/AddDepartment`, data);
    }

    public async updateDepartment(data: Partial<DepartmentModel>): Promise<Result<any>> {
        return await this.put<Result<any>>(`/${this.controller}/UpdateDepartment`, data);
    }

    public async deleteDepartments(ids: string[]): Promise<Result<any>> {
        return await this.delete<Result<any>>(`/${this.controller}/DeleteListDepartment`, { data: ids });
    }

    public async restoreDepartments(ids: string[]): Promise<Result<any>> {
        return await this.delete<Result<any>>(`/${this.controller}/RestoreListDepartment`, { data: ids });
    }

    public async getDepartmentById(id: string): Promise<Result<DepartmentModel>> {
        return await this.get<Result<DepartmentModel>>(`/${this.controller}/GetDepartmentById`, { params: { id } });
    }

    public async getDeletedDepartments(): Promise<Result<DepartmentModel[]>> {
        return await this.get<Result<DepartmentModel[]>>(`/${this.controller}/GetDeletedDepartments`);
    }
}
