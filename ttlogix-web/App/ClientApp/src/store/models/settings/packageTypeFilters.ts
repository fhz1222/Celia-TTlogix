import { Sorting } from "@/store/commonModels/Sorting";

export class PackageTypeFilters {
    constructor() {
        this.sorting = { by: null, descending: null };
    }

    sorting: Sorting;

    code: string;
    name: string;
    type: number | null;
    status: number | null;
}