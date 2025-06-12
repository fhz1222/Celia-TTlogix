import { Product } from "./product"
import { InboundReversalItem } from "./inboundReversalItem"

export class InboundReversal {
    constructor() {
        this.items = [];
    }

    jobNo: string;
    inJobNo: string;
    customerCode: string;
    whsCode: string;
    refNo: string;
    status: number;
    createdDate: string;
    reason: string;
    items: Array<InboundReversalItem>;
}