import { Range } from "@/store/commonModels/Range";
import { Sorting } from "@/store/commonModels/Sorting";

export class CustomerClientFilters {
    constructor() {
        this.sorting = { by: null, descending: null };
    }

    sorting: Sorting;

    customerCode: string;
    code: string;
    name: string;
    contactPerson: string;
    telephoneNo: string;
    faxNo: string;
    status: number | null;
}