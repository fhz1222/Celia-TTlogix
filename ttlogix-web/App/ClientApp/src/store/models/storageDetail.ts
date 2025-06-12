import { Product } from "./product"

export class StorageDetail {
    product: Product;
    whsCode: string;
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
    qty: number;
    allocatedQty: number;
    iLogLocationCategory: string;

    // local attributes
    selected: boolean;
}