import { Sorting } from "@/store/commonModels/Sorting";

export class UomSettingsFilters {
    constructor() {
        this.sorting = { by: null, descending: null };
    }

    sorting: Sorting;

    code: string | null;
    name: string | null;
    type: number | null;
    status: number | null;
}