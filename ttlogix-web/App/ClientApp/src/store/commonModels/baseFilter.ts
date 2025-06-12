import { PaginationData } from "@/store/commonModels/paginationData";
import { Sorting } from "@/store/commonModels/Sorting";
import { Config } from "./config";

export class BaseFilter {
    constructor () {
        this.pagination = new PaginationData();
        this.pagination.itemsPerPage = new Config().pagination.itemsPerPage[0];
        this.pagination.pageNumber = 1;
        this.sorting = { by: null, descending: null };
    }

    pagination: PaginationData;

    sorting: Sorting;

}