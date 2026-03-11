// src/api/CouponTypeApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { CouponTypeStatus, DueType, PromotionType } from '@/types';
import type { Result } from '@/types/Result';

// --- Interfaces cho Chi tiết Khuyến mãi (Nested Models) ---

export interface CouponTypeGiftDetailModel extends BaseEntityModel {
    giftName?: number | null;     // tên quà tặng (int vì có thể là enum/lookup)
    quantityGift?: number | null; // Số lượng quà tặng
    couponTypeGiftId?: string | null;
}

export interface CouponTypeGiftModel extends BaseEntityModel {
    giftType?: number | null;     // phương thức quà tặng (int)
    giftCount?: number | null;    // Số lượng quà tặng (int)
    couponTypeId?: string | null;
    couponTypeGiftDetail?: CouponTypeGiftDetailModel[] | null;
}

export interface CouponTypeDiscountDetailModel extends BaseEntityModel {
    minAmount?: number | null;    // Số tiền chiết khấu tối thiểu (hoặc MinQty)
    limit: number;                // Giới hạn số lần sử dụng (0 là không giới hạn)
    discountType: number;         // Loại chiết khấu (Percentage/FixedAmount)
    discountAmount: number;       // Giá trị chiết khấu
    couponTypeDiscountId?: string | null;
}

export interface CouponTypeDiscountModel extends BaseEntityModel {
    method?: number | null;       // phương thức chiết khấu (theo tổng đơn/số lượng)
    discountMax?: number | null;  // Số tiền chiết khấu tối đa
    couponTypeId?: string | null;
    couponTypeDiscountDetail?: CouponTypeDiscountDetailModel[] | null;
}

export interface CouponTypeFixedPriceModel extends BaseEntityModel {
    minPrice?: number | null;     // giá trị đơn hàng tối thiểu
    limit?: number | null;        // giới hạn số lượng áp dụng (0 là không giới hạn)
    priceSale?: number | null;    // giá cố định sau khi áp dụng
    couponTypeId?: string | null;
}

export interface CouponTypeCouponModel extends BaseEntityModel {
    minQuantity?: number | null;  // Số lượng tối thiểu
    limit?: number | null;        // Giới hạn số lượng
    couponCode?: string | null;   // Mã coupon
    couponTypeID?: string | null;
    coupontTypeDiscountDi
}
export interface CouponStudentModel extends BaseEntityModel {
    StudentName?: string | null;        // Giới hạn số lượng
    StudentId?: string | null;   // Mã coupon
    couponTypeID?: string | null;
}
// --- Interface CouponType Chính ---

export interface CouponTypeModel extends BaseEntityModel {
    name?: string | null;
    code?: string | null;
    status?: number; // Cần thêm status để theo dõi trạng thái kích hoạt

    // Hiệu lực
    dueType?: DueType | null; // Loại hiệu lực (ví dụ: "Theo thời hạn", "Theo ngày tạo") 

    durationInDays?: number | null;
    startDate?: Date | string | null;
    endDate?: Date | string | null;

    // Cấu trúc sinh coupon
    prefix?: string | null;
    suffix?: string | null;
    characterCount?: number | null;

    // Điều kiện áp dụng
    applyWith?: boolean | null;
    isForAllCompany?: boolean | null;
    isForAllCourse?: boolean | null;
    companyIds?: string | null;
    courseIds?: string | null;
    minQuantity?: number | null; // Số lượng tối thiểu (minQty)
    type?: PromotionType | null; // Loại coupon (ví dụ: "Chiết khấu", "Gia cứng", "Quà tặng")
    // Học viên áp dụng
    isForAllStudents?: boolean | null;
    // (Giả định studentId/StudentGroupId nếu không phải All Students)

    note?: string | null;
    description?: string | null;

    // Mối quan hệ chi tiết
    // couponTypeFixedPrices?: CouponTypeFixedPriceModel[] | null;`
    couponTypeGifts?: CouponTypeGiftModel[] | null;
    // couponTypeCoupons?: CouponTypeCouponModel[] | null;
    couponTypeDiscounts?: CouponTypeDiscountModel[] | null;
    // couponStudent?: CouponStudentModel[] | null;
    // Các trường khác như CouponIssues, CouponStudents... có thể thêm nếu cần
    studentIds?: string | null;
    couponTypeStatus?: CouponTypeStatus | null;
}

export interface CouponTypeQuery {
    name?: string;
    code?: string;
    status?: number;
    type?: string;
    page: number;
    pageSize: number;
}

export interface CouponTypePagedResult {
    items: CouponTypeModel[];
    total: number;
}

export class CouponTypeApi extends ApiClient {
    controller = 'CouponType';

    constructor() {
        super(import.meta.env.VITE_APP_API_URL as string);
    }

    public async getPagedCouponTypes(query: CouponTypeQuery): Promise<Result<CouponTypePagedResult>> {
        return await this.get<Result<CouponTypePagedResult>>(`/${this.controller}/GetPagedCouponTypes`, { params: query });
    }

    public async addCouponType(data: Partial<CouponTypeModel>): Promise<Result<any>> {
        return await this.post<Result<any>>(`/${this.controller}/AddCouponType`, data);
    }

    public async updateCouponType(data: Partial<CouponTypeModel>): Promise<Result<any>> {
        return await this.put<Result<any>>(`/${this.controller}/UpdateCouponType`, data);
    }
    // Lấy toàn bộ
    public async getAll(): Promise<Result<CouponTypeModel[]>> {
        return await this.get<Result<CouponTypeModel[]>>(`/${this.controller}/GetAllCouponTypes`);
    }
    public async deleteCouponTypes(ids: string[]): Promise<Result<any>> {
        return await this.delete<Result<any>>(`/${this.controller}/DeleteListCouponType`, { data: ids });
    }
}