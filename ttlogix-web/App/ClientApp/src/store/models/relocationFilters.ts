import { Range } from "@/store/commonModels/Range";
import { BaseFilter } from "@/store/commonModels/baseFilter";

export class RelocationFilters extends BaseFilter {
    constructor () {
        super();
        this.customerCode = null;
    }

    customerCode: string | null;
    supplierId: string;
    productCode: string | null;

    pid: string;
    externalPID: string;
    oldWhsCode: string;
    oldLocation: string;
    newWhsCode: string;
    newLocation: string;
    scannerType: number;
    relocatedBy: string;
    relocationDate: Range;
    qty: Range;
}