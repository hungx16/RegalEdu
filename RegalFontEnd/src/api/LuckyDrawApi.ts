import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';

export interface LuckyDrawModel extends BaseEntityModel {
    name: string;
    branch?: string;
    region?: string;
    reportDate?: string;
    reporter?: string;
    status: number;
    startDate: string;
    endDate: string;
    description?: string;
}

export interface LuckyDrawQuery {
    name?: string;
    branch?: string;
    region?: string;
    page: number;
    pageSize: number;
}

export interface LuckyDrawPagedResult {
    items: LuckyDrawModel[];
    total: number;
}

export interface CustomerRewardModel extends BaseEntityModel {
    wonDate: string;
    prize: string;
    phone: string;
    fullName: string;
    birthDate?: string;
    companyId?: string;
    regionId?: string;
    luckyDrawId?: string;
    rewardId?: string;
    imageId?: string;
    receiveStatus: number;
    acceptanceStatus: number;
    note?: string;
}

export interface CustomerRewardQuery {
    luckyDrawId?: string;
    companyId?: string;
    regionId?: string;
    receiveStatus?: number;
    acceptanceStatus?: number;
    phoneOrName?: string;
    page: number;
    pageSize: number;
}

export interface CustomerRewardPagedResult {
    items: CustomerRewardModel[];
    total: number;
}

export interface RewardModel extends BaseEntityModel {
    name: string;
    type?: string;
    promoCode?: string;
    status: number;
    description?: string;
}

export interface RewardQuery {
    name?: string;
    type?: string;
    page: number;
    pageSize: number;
}

export interface RewardPagedResult {
    items: RewardModel[];
    total: number;
}

export class LuckyDrawApi extends ApiClient {
    luckyDrawController = 'LuckyDraw';
    customerRewardController = 'CustomerReward';
    rewardController = 'Reward';

    constructor() {
        super(import.meta.env.VITE_APP_API_URL as string);
    }

    public async getPagedLuckyDraws(query: LuckyDrawQuery): Promise<Result<LuckyDrawPagedResult>> {
        return await this.get<Result<LuckyDrawPagedResult>>(`/${this.luckyDrawController}/GetPagedLuckyDraws`, { params: query });
    }

    public async getAllActiveLuckyDraws(): Promise<Result<LuckyDrawModel[]>> {
        return await this.get<Result<LuckyDrawModel[]>>(`/${this.luckyDrawController}/GetAllActiveLuckyDraws`);
    }

    public async getLuckyDrawById(id: string): Promise<Result<LuckyDrawModel>> {
        return await this.get<Result<LuckyDrawModel>>(`/${this.luckyDrawController}/GetLuckyDrawById/${id}`);
    }

    public async addLuckyDraw(data: Partial<LuckyDrawModel>): Promise<Result<any>> {
        return await this.post<Result<any>>(`/${this.luckyDrawController}/CreateLuckyDraw`, data);
    }

    public async updateLuckyDraw(data: Partial<LuckyDrawModel>): Promise<Result<any>> {
        return await this.put<Result<any>>(`/${this.luckyDrawController}/UpdateLuckyDraw`, data);
    }

    public async deleteLuckyDraw(id: string): Promise<Result<any>> {
        return await this.delete<Result<any>>(`/${this.luckyDrawController}/DeleteLuckyDraw`, { data: id });
    }

    public async getPagedCustomerRewards(query: CustomerRewardQuery): Promise<Result<CustomerRewardPagedResult>> {
        return await this.get<Result<CustomerRewardPagedResult>>(`/${this.customerRewardController}/GetPagedCustomerRewards`, { params: query });
    }

    public async confirmReceiveCustomerReward(payload: { id: string; note?: string }): Promise<Result<any>> {
        return await this.post<Result<any>>(`/${this.customerRewardController}/ConfirmReceiveCustomerReward`, payload);
    }

    public async confirmAcceptanceCustomerReward(payload: { id: string; note?: string }): Promise<Result<any>> {
        return await this.post<Result<any>>(`/${this.customerRewardController}/ConfirmAcceptanceCustomerReward`, payload);
    }

    public async getPagedRewards(query: RewardQuery): Promise<Result<RewardPagedResult>> {
        return await this.get<Result<RewardPagedResult>>(`/${this.rewardController}/GetPagedRewards`, { params: query });
    }
}
