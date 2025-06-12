import { Range } from "@/store/commonModels/Range";
import { Sorting } from "@/store/commonModels/Sorting";

export class CustomerUomFilters {
    constructor() {
        this.sorting = { by: null, descending: null };
    }

    sorting: Sorting;

    status: number | null;
    name: string | null;
    decimalPoint: Range;
    customerCode: string;
}