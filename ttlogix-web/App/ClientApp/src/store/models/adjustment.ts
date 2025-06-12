import { AdjustmentItem } from "./adjustmentItem";

export class Adjustment {
    constructor(){
        this.items = [];
    }
    jobNo: string;
    whsCode: string;
    customerName: string;
    customerCode: string | null;
    referenceNo: string;
    jobType: number;
    createdDate: string;
    createdBy: string;
    status: number;
    reason: string;
    revisedDate: string;
    revisedBy: string;
    confirmedDate: string;
    confirmedBy: string;
    cancelledDate: string;
    cancelledBy: string;
    items: Array<AdjustmentItem>;
}