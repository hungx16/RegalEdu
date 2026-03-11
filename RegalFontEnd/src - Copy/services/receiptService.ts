import type { ReceiptApi } from "@/api/ReceiptApi";
import type { ReceiptsModel } from "@/api/ReceiptApi";
import type { Result } from "@/types/Result";

export class ReceiptService {
    private api: ReceiptApi;

    constructor(apiInstance: ReceiptApi) {
        this.api = apiInstance;
    }

    async fetchPagedReceipts(query: any): Promise<Result<any>> {
        return await this.api.getPagedReceipts(query);
    }
    async fetchAllReceipts(): Promise<Result<ReceiptsModel[]>> {
        return await this.api.getAllReceipts();
    }
    async saveReceipt(receipt: Partial<ReceiptsModel>): Promise<any> {

        let result: any;
        if (receipt.id) {
            result = await this.api.updateReceipt(receipt);
        } else {
            result = await this.api.addReceipt(receipt);
        }
        if (!result.succeeded) throw new Error(result.error || 'Save failed');
        return result.data;
    }

    async updateReceipt(receipt: Partial<ReceiptsModel>): Promise<any> {
        return await this.api.updateReceipt(receipt);
    }

    async deleteReceipts(ids: string[]): Promise<any> {
        return await this.api.deleteReceipts(ids);
    }
    async getReceiptById(id: string): Promise<Result<ReceiptsModel>> {
        return await this.api.getReceiptById(id);
    }
}