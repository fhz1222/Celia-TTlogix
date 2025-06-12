import { Range } from "@/store/commonModels/Range";
import { PaginationData } from "@/store/commonModels/paginationData";
import { Sorting } from "@/store/commonModels/Sorting";
import { state } from "../state";

export class StockTransferReversalFilters {
    constructor () {
        this.pagination = new PaginationData();
        this.pagination.itemsPerPage = state.config.pagination.itemsPerPage[0];
        this.pagination.pageNumber = 1;
        this.customerCode = null;
        this.whsCode = '';
        this.sorting = { by: null, descending: null};
        this.statuses = [0,1]
    }

    pagination: PaginationData;

    sorting: Sorting;

    customerCode: string | null;
    whsCode: string;
    jobNo: string;
    refNo: string;
    statuses: Array<number> | null;
    reason: string;
    createdDate: Range;
}