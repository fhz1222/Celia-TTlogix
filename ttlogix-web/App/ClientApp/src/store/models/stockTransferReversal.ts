import { StockTransferReversalItem } from "./stockTransferReversalItem"

export class StockTransferReversal {
    constructor() {
        this.items = [];
    }

    jobNo: string;
    stfJobNo: string;
    customerCode: string;
    whsCode: string;
    refNo: string;
    status: number;
    createdDate: string;
    reason: string;
    items: Array<StockTransferReversalItem>;
}