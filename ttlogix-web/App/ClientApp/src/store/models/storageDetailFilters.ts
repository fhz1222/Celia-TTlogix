import { Range } from "@/store/commonModels/Range";
import { BaseFilter } from "@/store/commonModels/baseFilter";

export class StorageDetailFilters extends BaseFilter {
    constructor () {
        super();
        this.customerCode = null;
        this.whsCode = '';
    }

    customerCode: string | null;
    whsCode: string;
    supplierId: string;
    productCode: string | null;

    ownership: number;
    location: string;
    pid: string;
    externalPID: string;
    inboundJobNo: string;
    outboundJobNo: string;
    lineItem: number;
    sequenceNo: number;
    inboundDate: string;
    controlCode1: string;
    controlCode2: string;
    parentId: string;
    status: number;
    bondedStatus: true;
    refNo: string;
    commInvNo: string;
    qty: Range;
    allocatedQty: Range;
}