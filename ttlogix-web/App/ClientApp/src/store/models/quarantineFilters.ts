import { Range } from "@/store/commonModels/Range";
import { BaseFilter } from "@/store/commonModels/baseFilter";

export class QuarantineFilters extends BaseFilter {
    constructor () {
        super();
        this.customerCode = null;
    }

    customerCode: string | null;
    whsCode: string;
    supplierId: string;
    productCode: string | null;

    location: string;
    reason: string;
    pid: string;
    createdBy: string;
    quarantineDate: Range;
    qty: Range;
    decimalNum: Range;
}