// src/api/PromotionApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
// import { S } from 'node_modules/@fullcalendar/core/internal-common';
import type { StudentModel } from './StudentApi';
import type { PromotionGroupModel } from './PromotionGroupApi';
export interface PromotionModel extends BaseEntityModel {
  name: string;
  description?: string | null;
  startDate: Date;
  endDate: Date;
  applyWith?: boolean | null;
  companyId?: string | null;
  courseId?: string | null;
  qtymonth?: number | null;
  // studentId?: string | null;
  codeUsage?: boolean | null;
  promoCode?: string | null;
  type?: number | null;
  discounts?: DiscountModel[] | null;
  registerStudys?: any[] | null; // Cần tạo interface cho RegisterStudyModel nếu có
  courseGifts?: CourseGiftModel[] | null;
  promotionCoupon?: PromotionCouponModel[] | null;
  promotionGift?: PromotionGiftModel[] | null;
  promotionFixedPrice?: PromotionFixedPriceModel[] | null;
  // timeRange?: Date[];
  allCompany?: boolean | null;
  allCourse?: boolean | null;
  allStudent?: boolean | null;
  code?: string | null;
  promotionGroupId?: string | null;
  promotionGroup?: PromotionGroupModel | null;
  promotionStudent?: PromotionStudentModel[] | null;
}
export interface PromotionGiftDetailModel extends BaseEntityModel {
  giftName?: number | null;
  quantityGift?: number | null;
  promotionGiftId?: string | null;
}

export interface PromotionGiftModel extends BaseEntityModel {
  giftType?: number | null;
  giftCount?: number | null;
  promotionId?: string | null;
  promotionGiftDetails?: PromotionGiftDetailModel[] | null;
}

export interface CourseGiftDetailModel extends BaseEntityModel {
  courseId?: string | null;
  minQty?: number | null;
  courseGiftId?: string | null;
}

export interface CourseGiftModel extends BaseEntityModel {
  allCourse?: boolean | null;
  minQty?: number | null;
  promotionId?: string | null;
  courseGiftDetails?: CourseGiftDetailModel[] | null;
}

export interface DiscountDetailModel extends BaseEntityModel {
  minAmount?: number | null;
  limit: number;
  discountType: number;
  discountAmount: number;
  discountId?: string | null;
}

export interface DiscountModel extends BaseEntityModel {
  method?: number | null;
  discountMax?: number | null;
  promotionId?: string | null;
  discountDetails?: DiscountDetailModel[] | null;
}

export interface PromotionFixedPriceModel extends BaseEntityModel {
  minPrice?: number | null;
  limit?: number | null;
  priceSale?: number | null;
  promotionId?: string | null;
}

export interface PromotionCouponModel extends BaseEntityModel {
  minQuantity?: number | null;
  limit?: number | null;
  couponCode?: string | null;
  couponTypeId?: string | null;
  promotionId?: string | null;
}

export interface PromotionStudentModel extends BaseEntityModel {
  studentId?: string | null;
  studentCode?: string | null;
  promotionId?: string | null;
  student: StudentModel | null;
}

export interface PromotionQuery {
  page: number;
  pageSize: number;
  filters?: {
    name?: string;
    startDate?: string;
    endDate?: string;
  };
}

export interface PromotionPagedResult {
  items: PromotionModel[];
  total: number;
}
// ... (các interface đã tạo ở trên)

export class PromotionApi extends ApiClient {

  controller = 'promotion';

  constructor() {
    super(import.meta.env.VITE_APP_API_URL as string);
  }

  // Lấy danh sách khuyến mãi có phân trang
  public async getPagedPromotions(query: PromotionQuery): Promise<Result<PromotionPagedResult>> {
    return await this.get<Result<PromotionPagedResult>>(`/${this.controller}/GetPagedPromotions`, { params: query });
  }

  // Lấy tất cả khuyến mãi
  public async GetAllPromotions(): Promise<Result<PromotionModel[]>> {
    return await this.get<Result<PromotionModel[]>>(`/${this.controller}/GetAllPromotions`);
  }
  // Lấy tất cả khuyến mãi
  public async GetGlobalPromotions(): Promise<Result<PromotionModel[]>> {
    return await this.get<Result<PromotionModel[]>>(`/${this.controller}/GetGlobalPromotions`);
  }
  // Lấy tất cả khuyến mãi
  public async GetPromotionValues(): Promise<Result<PromotionModel[]>> {
    return await this.get<Result<PromotionModel[]>>(`/${this.controller}/GetPromotionValues`);
  }
  // Lấy khuyến mãi theo ID
  public async getPromotionById(id: string): Promise<Result<PromotionModel>> {
    return await this.get<Result<PromotionModel>>(`/${this.controller}/GetPromotionById`, { params: { id } });
  }

  // Thêm mới một khuyến mãi
  public async addPromotion(data: Partial<PromotionModel>): Promise<Result<any>> {
    return await this.post<Result<any>>(`/${this.controller}/AddPromotion`, data);
  }

  // Cập nhật một khuyến mãi
  public async updatePromotion(data: Partial<PromotionModel>): Promise<Result<any>> {
    return await this.put<Result<any>>(`/${this.controller}/UpdatePromotion`, data);
  }

  // Xóa danh sách khuyến mãi
  public async deletePromotions(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/DeleteListPromotion`, { data: ids });
  }

  // Khôi phục danh sách khuyến mãi đã xóa mềm
  public async restorePromotions(ids: string[]): Promise<Result<any>> {
    return await this.delete<Result<any>>(`/${this.controller}/RestoreListPromotion`, { data: ids });
  }
}