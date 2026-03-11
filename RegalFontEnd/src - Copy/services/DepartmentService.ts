// src/services/DepartmentService.ts
import type { DepartmentModel, DepartmentQuery } from '@/api/DepartmentApi';
import type { DepartmentApi } from '@/api/DepartmentApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class DepartmentService {
    private departmentApi: DepartmentApi;

    constructor(departmentApiInstance: DepartmentApi) {
        this.departmentApi = departmentApiInstance;
    }

    async fetchPagedDepartments(query: DepartmentQuery): Promise<Result<any>> {
        return await this.departmentApi.getPagedDepartments(query);
    }

    async fetchAllDepartments(): Promise<Result<any>> {
        return await this.departmentApi.getAllDepartments();
    }

    async saveDepartment(department: Partial<DepartmentModel>): Promise<any> {
        let result: any;
        if (department.id) {
            result = await this.departmentApi.updateDepartment(department);
        } else {
            result = await this.departmentApi.addDepartment(department);
        }
        if (!result.succeeded) {
            throw new Error(result.error || 'Save failed');
        }
        return result.data;
    }

    async deleteDepartments(ids: string[]): Promise<void> {
        let result: any = await this.departmentApi.deleteDepartments(ids);
        if (!result.succeeded) {
            throw new Error(result.error || 'Delete failed');
        } else {
            useNotificationStore().showToast('success', { key: result.data });
        }
    }

    async restoreDepartments(ids: string[]): Promise<void> {
        let result: any = await this.departmentApi.restoreDepartments(ids);
        if (!result.succeeded) {
            throw new Error(result.error || 'Restore failed');
        }
    }
    async fetchDeletedDepartments(): Promise<Result<any>> {
        return await this.departmentApi.getDeletedDepartments();
    }
}
