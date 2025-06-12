import { Sorting } from "@/store/commonModels/Sorting";
import { PaginationData } from "@/store/commonModels/paginationData";
import { state } from "@/store/state";

export class LocationFilters {
    constructor() {
        this.sorting = { by: null, descending: null };
        this.pagination = new PaginationData();
        this.pagination.itemsPerPage = state.config.pagination.itemsPerPage[0];
        this.pagination.pageNumber = 1;
    }

    pagination: PaginationData;

    sorting: Sorting;

    code: string | null;
    name: string | null;
    type: number | null;
    warehouseCode: string | null;
    areaCode: string | null;
    isPriority: number | null;
    iLogLocationCategoryId: number | null;
    status: number | null;
}