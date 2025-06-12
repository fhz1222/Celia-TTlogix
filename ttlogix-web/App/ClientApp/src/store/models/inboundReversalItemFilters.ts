import { Sorting } from "@/store/commonModels/Sorting";

export class InboundReversalItemFilters {
    constructor() {
        this.productCode = null;
        this.pid = null;
        this.sorting = { by: null, descending: null };
    }

    sorting: Sorting;

    jobNo: string;
    pid: string | null;
    productCode: string | null;
}