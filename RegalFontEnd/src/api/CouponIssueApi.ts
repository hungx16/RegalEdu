// src/api/CouponIssueApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { CouponStatus, IssueStatus, IssueType } from '@/types';
import type { Result } from '@/types/Result';

// --- I. Interfaces cho Coupon ---
export interface CouponModel extends BaseEntityModel {
    code?: string | null;
    createdDate?: Date | string;
    expiredDate?: Date | string | null;
    registerStudyId?: string | null;
    couponIssueId?: string | null;
    studentId?: string | null;
    // Thuộc tính hiển thị mở rộng (dùng cho List)
    couponTypeName?: string | null;
    createdBy?: string | null;
    usageCount?: number;
    // status?: number; // 0: Issued, 1: Used, 2: Expired, etc.
    couponStatus?: CouponStatus | null;
}

// --- II. Interfaces cho Học viên áp dụng trong đợt phát hành ---
export interface CouponIssueStudentModel extends BaseEntityModel {
    couponIssueId?: string | null;
    studentId?: string | null;
    studentName?: string | null; // Cần thiết khi chọn thủ công
}

// --- III. Interfaces cho Đợt phát hành Coupon (CouponIssue) ---
export interface CouponIssueModel extends BaseEntityModel {
    couponTypeId?: string | null;
    issueType?: IssueType | null;      // Loại phát hành (ví dụ: "Số lượng phát hành")
    quantity?: number | null;       // Số lượng phát hành
    issueDate?: Date | string | null; // Ngày phát hành
    isForAllStudents?: boolean | null; // Áp dụng cho tất cả học viên

    // Mối quan hệ chi tiết (dùng khi tạo/cập nhật)
    couponIssueStudent?: CouponIssueStudentModel[] | null;
    coupons?: CouponModel[] | null;
    issueStatus?: IssueStatus | null;
}

// --- IV. Query và Paged Result ---

export interface CouponIssueQuery {
    couponTypeId?: string;
    issueType?: string;
    issueDateFrom?: string;
    issueDateTo?: string;
    page: number;
    pageSize: number;
}

export interface CouponIssuePagedResult {
    items: CouponIssueModel[];
    total: number;
}

export class CouponIssueApi extends ApiClient {
    controller = 'CouponIssue';

    constructor() {
        super(import.meta.env.VITE_APP_API_URL as string);
    }

    /** Tạo mới một đợt phát hành (và các mã coupon liên quan) */
    public async addCouponIssue(data: Partial<CouponIssueModel>): Promise<Result<any>> {
        // Endpoint thường là CreateCouponIssue hoặc AddCouponIssue
        return await this.post<Result<any>>(`/${this.controller}/AddCouponIssue`, data);
    }

    // Cập nhật một đợt phát hành
    public async updateCouponIssue(data: Partial<CouponIssueModel>): Promise<Result<any>> {
        return await this.put<Result<any>>(`/${this.controller}/UpdateCouponIssue`, data);
    }
    // Lấy đợt phát hành theo ID
    public async getCouponIssueById(id: string): Promise<Result<CouponIssueModel>> {
        return await this.get<Result<CouponIssueModel>>(`/${this.controller}/GetCouponIssueById`, { params: { id } });
    }
    //lấy tất cả đợt phát hành
    public async getAll(): Promise<Result<CouponIssueModel[]>> {
        return await this.get<Result<CouponIssueModel[]>>(`/${this.controller}/GetAllCouponIssues`);
    }
    // Xoá đợt phát hành
    public async deleteCouponIssues(ids: string[]): Promise<Result<any>> {
        return await this.delete<Result<any>>(`/${this.controller}/DeleteCouponIssues`, { data: ids });
    }
    //lấy các đợt phát hành có phân trang
    public async getPagedCouponIssues(query: CouponIssueQuery): Promise<Result<CouponIssuePagedResult>> {
        return await this.get<Result<CouponIssuePagedResult>>(`/${this.controller}/GetPagedCouponIssues`, { params: query });
    }
    //getAllCoupons
    public async getAllCoupons(): Promise<Result<CouponModel[]>> {
        return await this.get<Result<CouponModel[]>>(`/${this.controller}/GetAllCoupons`);
    }
    // ... Thêm các API khác (GetPagedIssues, DeleteIssue) nếu cần
}