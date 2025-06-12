import { Range } from "@/store/commonModels/Range";
import { PaginationData } from "@/store/commonModels/paginationData";
import { Sorting } from "@/store/commonModels/Sorting";
import { state } from "../state";

export class ReversibleStockTransferItemFilters {
    constructor () {
        this.sorting = { by: null, descending: null };
        this.pid = null;
        this.productCode = null;
        this.locationCode = null;
    }
    sorting: Sorting;

    stfJobNo: string;
    pid: string | null;
    productCode: string | null;
    locationCode: string | null;
}

export class ReversibleStockTransferItemFiltersLocal {
    constructor() {
        this.pid = null;
        this.productCode = null;
        this.locationCode = null;
    }

    pid: string | null;
    productCode: string | null;
    locationCode: string | null;
}