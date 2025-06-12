import { Sorting } from "@/store/commonModels/Sorting";
import { PaginationData } from "../commonModels/paginationData";
import { state } from "../state";

export class ReversibleStockTransferFilters {
    constructor() {
        this.pagination = new PaginationData();
        this.pagination.itemsPerPage = state.config.pagination.itemsPerPage[0];
        this.pagination.pageNumber = 1;
        this.newerThan = null;
    }
    pagination: PaginationData;

    whsCode: string;
    stfJobNo: string;
    newerThan: string | null;
}