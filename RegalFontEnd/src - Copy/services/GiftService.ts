import type { GiftModel, GiftQuery } from '@/api/GiftApi';
import type { GiftApi } from '@/api/GiftApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class GiftService {
    private giftApi: GiftApi;

    constructor(apiInstance: GiftApi) {
        this.giftApi = apiInstance;
    }

    async fetchPagedGifts(query: GiftQuery): Promise<Result<any>> {
        return await this.giftApi.getPagedGifts(query);
    }

    async fetchAllGifts(): Promise<Result<any>> {
        return await this.giftApi.getAllGifts();
    }

    async saveGift(model: Partial<GiftModel>): Promise<any> {
        let result: any;
        if (model.id) {
            result = await this.giftApi.updateGift(model);
        } else {
            result = await this.giftApi.addGift(model);
        }
        if (!result.succeeded) throw new Error(result.error || 'Save failed');
        return result.data;
    }

    async deleteGifts(ids: string[]): Promise<void> {
        let result: any = await this.giftApi.deleteGifts(ids);
        if (!result.succeeded) throw new Error(result.error || 'Delete failed');
        else useNotificationStore().showToast('success', { key: result.data });
    }
}
