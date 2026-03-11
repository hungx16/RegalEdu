// src/api/RegisterStudyApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { Result } from '@/types/Result';
import type { ReceiptsModel } from './ReceiptApi';
import type { PaymentMeThod, PaymentMethodType, PaymentStatus, PaymentType } from '@/types';

// --- I. Mẫu dữ liệu con ---

export interface RegisterPromotionListModel extends BaseEntityModel {
    promotionId?: string | null;
    registerStudyId?: string | null;
    discountAmount?: number | null;
    promotionName?: string; // Sẽ được tính trong DTO phản hồi
}
export interface RegisterGiftModel extends BaseEntityModel {
    promotionId?: string | null;
    registerStudyId?: string | null;
    giftId?: string | null;
    giftName?: string | null;
    amount?: number | null;
    // promotionName?: string; // Sẽ được tính trong DTO phản hồi
}
export interface DetailRegisterStudyModel extends BaseEntityModel {
    registerStudyId?: string | null;
    courseId?: string | null;
    classTypeId?: string | null;

    // Thuộc tính hiển thị/tính toán
    courseName?: string;
    classTypeName?: string;
    unit?: number | null;
    quantity?: number | null;
    tuitionFee?: number | null;
    discountAmount?: number | null;
    totalAmount?: number | null;
}

// --- II. Mô hình Form chính cho Wizard ---
// Phản ánh chính xác RegisterStudyRequestDto từ C#

export interface RegisterStudyModel extends BaseEntityModel {
    // RegisterStudy fields
    code?: string | null; // Mã đăng ký học
    type: number;
    couponCode?: string | null;
    studentId?: string | null; // Có thể null nếu là khách hàng mới

    companyId?: string | null;
    regionId?: string | null;
    employeeId?: string | null;     // Nhân viên tư vấn
    teacherManagerId?: string | null; // Giáo viên/CM phụ trách

    // Contact/Student Info (Nếu là Khách hàng mới)
    studentFullName: string; // Tên học viên
    studentPhone?: string;
    studentEmail?: string;
    studentBirthDate?: Date | string;

    contactFullName?: string; // Tên phụ huynh (Contact)
    contactPhone?: string;
    contactEmail?: string;
    contactAddress?: string;
    contactRelationship?: number | null; // Mối quan hệ với học viên
    expectedCompleteDate?: Date | string; // Ngày hoàn thành khóa học gần nhất

    // Khóa học & Khuyến mãi
    detailRegisterStudys?: DetailRegisterStudyModel[] | null;
    registerPromotion?: RegisterPromotionListModel[]; // Dữ liệu chọn khuyến mãi từ dialog
    registerGifts?: RegisterGiftModel[]; // Dữ liệu chọn quà tặng từ dialog

    // Thanh toán (Bước 2)
    totalAmount?: number | null;        // Tổng tiền trước giảm giá
    totalDiscount?: number | null;      // Tổng giảm giá
    totalAfterDiscount?: number | null; // Tổng sau giảm

    firstPaymentAmount?: number;
    paymentType?: PaymentType; // Trả thẳng/Trả góp
    paymentMethodType?: PaymentMethodType; // Thanh toán 1 lần/Nhiều lần
    paymentMethod?: PaymentMeThod; // Tiền mặt/ATM/Credit
    receipts?: ReceiptsModel[] | null; // IDs của phiếu thu liên quan
    paymentStatus?: PaymentStatus; // Trạng thái thanh toán
    amountToBePaid?: number | null; // Số tiền còn phải thu
    tuitionFeesPaid?: number | null; // Số tiền đã thu
    remainingTuitionFees?: number | null; // Số tiền còn lại phải thu

}

// --- III. Query và Paged Result (Dùng cho List) ---

export interface RegisterStudyListModel extends BaseEntityModel {
    code: string;
    studentCode: string; // Từ Student
    companyName: string; // Từ Company
    regionName: string;  // Từ Region
    createdAt: string;   // Ngày đăng ký
    type: number;        // Loại đăng ký
    status: number;      // Trạng thái đăng ký
    paymentStatus: number; // Trạng thái thanh toán
    totalAmount: number; // Tổng thanh toán
}

export interface RegisterStudyQuery {
    page: number;
    pageSize: number;
    searchTerm?: string;
    registrationStatus?: number;
    paymentStatus?: number;
    regionId?: number;
    companyId?: number;
    employeeId?: number;
    studentId?: string;
}

export interface RegisterStudyPagedResult {
    items: RegisterStudyListModel[];
    total: number;
}

// --- IV. API Class ---

export class RegisterStudyApi extends ApiClient {
    controller = 'RegisterStudy';

    constructor() {
        super(import.meta.env.VITE_APP_API_URL as string);
    }

    public async getPagedRegisterStudy(query: RegisterStudyQuery): Promise<Result<RegisterStudyPagedResult>> {
        return await this.get<Result<RegisterStudyPagedResult>>(`/${this.controller}/GetPagedRegisterStudys`, { params: query });
    }
    //lấy toàn bộ đăng ký học
    public async getAllRegisterStudy(): Promise<Result<RegisterStudyModel[]>> {
        return await this.get<Result<RegisterStudyModel[]>>(`/${this.controller}/GetAllRegisterStudy`);
    }

    public async getRegisterStudyById(id: string): Promise<Result<RegisterStudyModel>> {
        return await this.get<Result<RegisterStudyModel>>(`/${this.controller}/GetRegisterStudyById`, { params: { id } });
    }

    // Tạo mới đăng ký học
    public async addRegisterStudy(data: Partial<RegisterStudyModel>): Promise<Result<any>> {
        return await this.post<Result<any>>(`/${this.controller}/AddRegisterStudy`, data);
    }

    // Cập nhật đăng ký học
    public async updateRegisterStudy(data: Partial<RegisterStudyModel>): Promise<Result<any>> {
        return await this.put<Result<any>>(`/${this.controller}/UpdateRegisterStudy`, data);
    }

    public async deleteRegisterStudies(ids: string[]): Promise<Result<any>> {
        return await this.delete<Result<any>>(`/${this.controller}/DeleteListRegisterStudy`, { data: ids });
    }
}