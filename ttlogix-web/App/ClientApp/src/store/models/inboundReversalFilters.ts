import { Range } from "@/store/commonModels/Range";
import { PaginationData } from "@/store/commonModels/paginationData";
import { Sorting } from "@/store/commonModels/Sorting";
import { state } from "../state";

export class InboundReversalFilters {
    constructor () {
        this.pagination = new PaginationData();
        this.pagination.itemsPerPage = state.config.pagination.itemsPerPage[0];
        this.pagination.pageNumber = 1;
        this.customerCode = null;
        this.whsCode = '';
        this.sorting = { by: null, descending: null};
        this.productCode = null;
        this.sorting = { by: null, descending: null };
        this.statuses = [0,1]
    }

    pagination: PaginationData;

    sorting: Sorting;

    customerCode: string | null;
    whsCode: string;
    supplierId: string;
    productCode: string | null;

    jobNo: string;
    refNo: string;
    statuses: Array<number> | null;
    createdDate: string;
    reason: string;
}