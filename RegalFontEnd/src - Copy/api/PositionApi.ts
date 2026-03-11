// src/api/PositionApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { DepartmentModel } from '@/api/DepartmentApi';

export interface PositionModel extends BaseEntityModel {
    positionCode: string;
    positionName: string;
    description?: string | null;
    // Liên kết phòng ban
    departmentPositions?: DepartmentPositionModel[]; // Nếu trả về đầy đủ
    // departments?: DepartmentModel[]; // Danh sách phòng ban, thường dùng ở FE
    departmentIds?: string[]; // Mảng id, tiện cho binding v-model
    isSale?: boolean;
    isSaleLead?: boolean;
    isSupport?: boolean;
}
export interface DepartmentPositionModel {
    id?: string | null;
    departmentId: string;
    positionId?: string | null;
    department?: DepartmentModel; // có thể trả về object hoặc chỉ id
}

export interface PositionQuery {
    page: number;
    pageSize: number;
    filters?: {
        positionCode?: string;
        positionName?: string;
        status?: number;
        isDeleted?: boolean;
    };
}

export interface PositionPagedResult {
    items: PositionModel[];
    total: number;
}

export class PositionApi extends ApiClient {
    controller = 'position';

    constructor() {
        super(import.meta.env.VITE_APP_API_URL as string);
    }

    public async getPagedPositions(query: PositionQuery): Promise<Result<PositionPagedResult>> {
        return await this.get<Result<PositionPagedResult>>(`/${this.controller}/GetPagedPositions`, { params: query });
    }

    public async getAllPositions(): Promise<Result<PositionModel[]>> {
        return await this.get<Result<PositionModel[]>>(`/${this.controller}/GetAllPositions`);
    }

    public async addPosition(data: Partial<PositionModel>): Promise<Result<any>> {
        return await this.post<Result<any>>(`/${this.controller}/AddPosition`, data);
    }

    public async updatePosition(data: Partial<PositionModel>): Promise<Result<any>> {
        return await this.put<Result<any>>(`/${this.controller}/UpdatePosition`, data);
    }

    public async deletePositions(ids: string[]): Promise<Result<any>> {
        return await this.delete<Result<any>>(`/${this.controller}/DeleteListPosition`, { data: ids });
    }

    public async restorePositions(ids: string[]): Promise<Result<any>> {
        return await this.delete<Result<any>>(`/${this.controller}/RestoreListPosition`, { data: ids });
    }

    public async getPositionById(id: string): Promise<Result<PositionModel>> {
        return await this.get<Result<PositionModel>>(`/${this.controller}/GetPositionById`, { params: { id } });
    }

    public async getDeletedPositions(): Promise<Result<PositionModel[]>> {
        return await this.get<Result<PositionModel[]>>(`/${this.controller}/GetDeletedPositions`);
    }
}
