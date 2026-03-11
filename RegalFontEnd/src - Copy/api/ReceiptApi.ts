// src/api/ReceiptApi.ts
import { ApiClient, type BaseEntityModel } from '@/api/ApiClient';
import type { PaymentMeThod, PaymentMethodType, PaymentType } from '@/types';
import type { Result } from '@/types/Result';
import type { EmployeeModel } from './EmployeeApi';

export interface ReceiptsModel extends BaseEntityModel {
    receiptType?: number | null;     // Loại phiếu thu (Ví dụ: "Phiếu theo đơn")
    receiptCode?: string | null;     // Mã phiếu thu

    registerStudyId?: string | null; // Mã đăng ký học
    studentId?: string | null;
    courseId?: string | null;
    employeeId?: string | null;      // Nhân viên tư vấn
    employee: EmployeeModel | null;
    paymentType?: PaymentType | null; // Loại thanh toán (Ví dụ: "Trả thẳng", "Trả góp")
    paymentMethodType?: PaymentMethodType | null; // Hình thức thanh toán (Ví dụ: "Thanh toán một lần", "Nhiều lần")
    paymentMethod?: PaymentMeThod | null;   // Phương thức thanh toán (Ví dụ: "Chuyển khoản", "Tiền mặt")
    totalAmount?: number | null;     // Tổng tiền

    note?: string | null;
    discountCode?: string | null;    // Mã thấu chi

    // Thuộc tính hiển thị mở rộng (dùng cho List)`
    studentName?: string | null;
    companyName?: string | null;
    regionName?: string | null;
    registerCode?: string | null;
    courseName?: string | null;
    employeeName?: string | null;
    status?: number; // Trạng thái phiếu thu (Giả định: 1: Đã xác nhận, 2: Chờ xử lý)
}

export interface ReceiptQuery {
    page: number;
    pageSize: number;
    filters?: {
        searchTerm?: string;
        type?: string;
        status?: number;
        paymentMethodType?: string;
    };
}

export interface ReceiptPagedResult {
    items: ReceiptsModel[];
    total: number;
}

export class ReceiptApi extends ApiClient {
    controller = 'Receipt';

    constructor() {
        super(import.meta.env.VITE_APP_API_URL as string);
    }

    public async getPagedReceipts(query: ReceiptQuery): Promise<Result<ReceiptPagedResult>> {
        return await this.get<Result<ReceiptPagedResult>>(`/${this.controller}/GetPagedReceipts`, { params: query });
    }

    public async addReceipt(data: Partial<ReceiptsModel>): Promise<Result<any>> {
        return await this.post<Result<any>>(`/${this.controller}/AddReceipt`, data);
    }

    public async updateReceipt(data: Partial<ReceiptsModel>): Promise<Result<any>> {
        return await this.put<Result<any>>(`/${this.controller}/UpdateReceipt`, data);
    }

    public async deleteReceipts(ids: string[]): Promise<Result<any>> {
        return await this.delete<Result<any>>(`/${this.controller}/DeleteListReceipt`, { data: ids });
    }
    public async getReceiptById(id: string): Promise<Result<ReceiptsModel>> {
        return await this.get<Result<ReceiptsModel>>(`/${this.controller}/GetReceiptById`, { params: { id } });
    }

    public async getAllReceipts(): Promise<Result<ReceiptsModel[]>> {
        return this.get<Result<ReceiptsModel[]>>(`/${this.controller}/GetAllReceipts`);
    }
}