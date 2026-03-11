// src/api/RegionApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface RegionModel extends BaseEntityModel {
    regionCode: string;
    regionName: string;
    description?: string | null;
    companies?: any[]; // CompanyDto[]
    managerId?: string | null;
    manager?: any; // EmployeeModel
}

export interface RegionQuery {
    page: number;
    pageSize: number;
    filters?: {
        regionCode?: string;
        regionName?: string;
        isDeleted?: boolean;
    };
}

export interface RegionPagedResult {
    items: RegionModel[];
    total: number;
}

export class RegionApi extends ApiClient {
    controller = 'region';

    constructor() {
        super(import.meta.env.VITE_APP_API_URL as string);
    }

    public async getPagedRegions(query: RegionQuery): Promise<Result<RegionPagedResult>> {
        return await this.get<Result<RegionPagedResult>>(`/${this.controller}/GetPagedRegions`, { params: query });
    }

    public async getAllRegions(): Promise<Result<RegionModel[]>> {
        return await this.get<Result<RegionModel[]>>(`/${this.controller}/GetAllRegions`);
    }

    public async addRegion(data: Partial<RegionModel>): Promise<Result<any>> {
        return await this.post<Result<any>>(`/${this.controller}/AddRegion`, data);
    }

    public async updateRegion(data: Partial<RegionModel>): Promise<Result<any>> {
        return await this.put<Result<any>>(`/${this.controller}/UpdateRegion`, data);
    }

    public async deleteRegions(ids: string[]): Promise<Result<any>> {
        return await this.delete<Result<any>>(`/${this.controller}/DeleteListRegion`, { data: ids });
    }

    public async restoreRegions(ids: string[]): Promise<Result<any>> {
        return await this.delete<Result<any>>(`/${this.controller}/RestoreListRegion`, { data: ids });
    }

    public async getRegionById(id: string): Promise<Result<RegionModel>> {
        return await this.get<Result<RegionModel>>(`/${this.controller}/GetRegionById`, { params: { id } });
    }

    public async getDeletedRegions(): Promise<Result<RegionModel[]>> {
        return await this.get<Result<RegionModel[]>>(`/${this.controller}/GetDeletedRegions`);
    }
}
