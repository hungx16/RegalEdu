import type {
    CustomerRewardQuery,
    LuckyDrawApi,
    LuckyDrawModel,
    LuckyDrawQuery,
    RewardQuery,
} from '@/api/LuckyDrawApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class LuckyDrawService {
    private luckyDrawApi: LuckyDrawApi;

    constructor(apiInstance: LuckyDrawApi) {
        this.luckyDrawApi = apiInstance;
    }

    async fetchPagedLuckyDraws(query: LuckyDrawQuery): Promise<Result<any>> {
        return await this.luckyDrawApi.getPagedLuckyDraws(query);
    }

    async fetchAllActiveLuckyDraws(): Promise<Result<any>> {
        return await this.luckyDrawApi.getAllActiveLuckyDraws();
    }

    async fetchPagedCustomerRewards(query: CustomerRewardQuery): Promise<Result<any>> {
        return await this.luckyDrawApi.getPagedCustomerRewards(query);
    }

    async fetchPagedRewards(query: RewardQuery): Promise<Result<any>> {
        return await this.luckyDrawApi.getPagedRewards(query);
    }

    async saveLuckyDraw(model: Partial<LuckyDrawModel>): Promise<any> {
        const result = model.id
            ? await this.luckyDrawApi.updateLuckyDraw(model)
            : await this.luckyDrawApi.addLuckyDraw(model);

        if (!result.succeeded) {
            throw new Error(result.errors || 'Save failed');
        }

        return result.data;
    }

    async deleteLuckyDraw(id: string): Promise<void> {
        const result = await this.luckyDrawApi.deleteLuckyDraw(id);

        if (!result.succeeded) {
            throw new Error(result.errors || 'Delete failed');
        }

        useNotificationStore().showToast('success', { key: result.data });
    }

    async confirmReceiveCustomerReward(id: string, note?: string): Promise<void> {
        const result = await this.luckyDrawApi.confirmReceiveCustomerReward({ id, note });

        if (!result.succeeded) {
            throw new Error(result.errors || 'Confirm receive failed');
        }
    }

    async confirmAcceptanceCustomerReward(id: string, note?: string): Promise<void> {
        const result = await this.luckyDrawApi.confirmAcceptanceCustomerReward({ id, note });

        if (!result.succeeded) {
            throw new Error(result.errors || 'Confirm acceptance failed');
        }
    }
}
