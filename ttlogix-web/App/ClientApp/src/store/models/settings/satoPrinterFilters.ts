import { Sorting } from "@/store/commonModels/Sorting";

export class SatoPrinterFilters {
    constructor() {
        this.sorting = { by: null, descending: null };
    }

    sorting: Sorting;

    ip: string | null;
    name: string | null;
    description: string | null;
    type: number | null;
}