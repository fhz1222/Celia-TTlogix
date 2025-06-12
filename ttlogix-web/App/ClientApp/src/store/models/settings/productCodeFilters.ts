import { Sorting } from "@/store/commonModels/Sorting";

export class ProductCodeFilters {
    constructor() {
        this.sorting = { by: null, descending: null };
    }

    sorting: Sorting;

    code: string | null;
    productCode: string | null;
    type: number | null;
    status: number | null;
}