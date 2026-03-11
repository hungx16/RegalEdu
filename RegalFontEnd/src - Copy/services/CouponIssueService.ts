//tạo service CouponIssueService.ts
import type { CouponIssueApi, CouponIssueModel, CouponIssueQuery } from '@/api/CouponIssueApi';
import type { Result } from '@/types/Result';
export class CouponIssueService {
    private api: CouponIssueApi;
    constructor(apiInstance: CouponIssueApi) {
        this.api = apiInstance;
    }
    async fetchPaged(query: CouponIssueQuery): Promise<Result<any>> {
        return await this.api.getPagedCouponIssues(query);
    }
    async fetchAll(): Promise<Result<any>> {
        return await this.api.getAll();
    }

    async fetchAllCoupons(): Promise<Result<any>> {
        return await this.api.getAllCoupons();
    }

    async save(data: Partial<CouponIssueModel>): Promise<void> {
        let result: any;
        if (data.id) {
            result = await this.api.updateCouponIssue(data);
        } else {
            result = await this.api.addCouponIssue(data);
        }
        if (!result.succeeded) throw new Error(result.error || 'Save failed');
    }
    async delete(ids: string[]): Promise<void> {
        let result: any = await this.api.deleteCouponIssues(ids);
        if (!result.succeeded) throw new Error(result.error || 'Delete failed');
    }

}