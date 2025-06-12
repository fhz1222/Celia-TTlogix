import { ProductWithDescription } from "./product";

export class Pallet {
    id: string;
    whsCode: string;
    location: string;
    inboundDate: string;
    qty: number;
    qtyPerPkg: number;
    product: ProductWithDescription;
    status: number;
    ownership: number;
    isInQuarantine: boolean;
    length: number;
    width: number;
    height: number;
    netWeight: number;
    grossWeight: number;
}