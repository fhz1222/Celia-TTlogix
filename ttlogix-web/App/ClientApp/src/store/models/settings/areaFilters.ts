import { Sorting } from "@/store/commonModels/Sorting";

export class AreaFilters {
    constructor() {
        this.sorting = { by: null, descending: null };
    }

    sorting: Sorting;

    code: string | null;
    name: string | null;
    type: string | null;
    whsCode: string | null;
    status: number | null;
}