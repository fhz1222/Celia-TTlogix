import { Range } from "@/store/commonModels/Range";
import { PaginationData } from "@/store/commonModels/paginationData";
import { Sorting } from "@/store/commonModels/Sorting";
import { state } from "../state";

export class StockTakeFilters {
    constructor () {
        this.whsCode = '';
        this.sorting = { by: null, descending: null };
        this.pagination = new PaginationData();
        this.pagination.itemsPerPage = state.config.pagination.itemsPerPage[0];
        this.pagination.pageNumber = 1;
        this.statuses = [0, 1];
    }

    sorting: Sorting;
    pagination: PaginationData;

    whsCode: string;

    jobNo: string;
    refNo: string;
    locationCode: string;
    createdDate: Range;
    statuses: Array<number> | null;;
    remark: string;
}