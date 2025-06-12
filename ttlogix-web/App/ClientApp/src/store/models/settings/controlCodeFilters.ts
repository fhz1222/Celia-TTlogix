import { Sorting } from "@/store/commonModels/Sorting";

export class ControlCodeFilters {
    constructor() {
        this.sorting = { by: null, descending: null };
    }

    sorting: Sorting;

    code: string | null;
    controlCode: string | null;
    type: number | null;
    status: number | null;
}