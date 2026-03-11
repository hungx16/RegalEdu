import type { CouponTypeApi, CouponTypeModel, CouponTypeQuery } from '@/api/CouponTypeApi';
import { useNotificationStore } from '@/stores/notificationStore';
import type { Result } from '@/types/Result';

export class CouponTypeService {
    private api: CouponTypeApi;
    constructor(apiInstance: CouponTypeApi) {
        this.api = apiInstance;
    }

    async fetchPaged(query: CouponTypeQuery): Promise<Result<any>> {
        return await this.api.getPagedCouponTypes(query);
    }

    async fetchAll(): Promise<Result<any>> {
        return await this.api.getAll();
    }

    async save(data: Partial<CouponTypeModel>): Promise<void> {
        let result: any;
        if (data.id) {
            result = await this.api.updateCouponType(data);
        } else {
            result = await this.api.addCouponType(data);
        }
        if (!result.succeeded) throw new Error(result.error || 'Save failed');
        useNotificationStore().showToast('success', { key: result.data });

    }

    async delete(ids: string[]): Promise<void> {
        let result: any = await this.api.deleteCouponTypes(ids);
        if (!result.succeeded) throw new Error(result.error || 'Delete failed');
        else useNotificationStore().showToast('success', { key: result.data });

    }
}
