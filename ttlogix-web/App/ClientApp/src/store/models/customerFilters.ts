import { Range } from "@/store/commonModels/Range";
import { PaginationData } from "@/store/commonModels/paginationData";
import { Sorting } from "@/store/commonModels/Sorting";
import { state } from "../state";

export class CustomerFilters {
    constructor () {
        this.whsCode = '';
        this.sorting = { by: null, descending: null };
    }

    sorting: Sorting;

    whsCode: string;
    code: string;
    name: string;
    contactPerson: string;
    telephoneNo: string;
    faxNo: string;
    status: number | null;
}