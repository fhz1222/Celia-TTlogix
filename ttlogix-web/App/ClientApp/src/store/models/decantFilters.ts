import { Range } from "@/store/commonModels/Range";
import { PaginationData } from "@/store/commonModels/paginationData";
import { Sorting } from "@/store/commonModels/Sorting";
import { state } from "../state";

export class DecantFilters {
    constructor () {
        this.pagination = new PaginationData();
        this.pagination.itemsPerPage = state.config.pagination.itemsPerPage[0];
        this.pagination.pageNumber = 1;
        this.customerCode = null;
        this.whsCode = '';
        this.sorting = { by: null, descending: null };
        this.productCode = null;
    }

    pagination: PaginationData;

    sorting: Sorting;

    customerCode: string | null;
    whsCode: string;
    supplierId: string;
    productCode: string | null;

    ownership: number;
    location: string;
    pid: string;
    externalPID: string;
    inboundJobNo: string;
    outboundJobNo: string;
    lineItem: number;
    sequenceNo: number;
    inboundDate: string;
    controlCode1: string;
    controlCode2: string;
    parentId: string;
    status: number;
    bondedStatus: true;
    refNo: string;
    commInvNo: string;
    qty: Range;
    allocatedQty: Range;
}