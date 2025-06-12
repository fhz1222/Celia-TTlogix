import { Product } from "./product"

export class Inventory {
    product: Product;
    ownership: number;
    onHandQty: number;
    quarantineQty: number;
    allocatedQty: number;
    incomingQty: number;
    pickableQty: number;
    bondedQty: number;
    nonBondedQty: number;
}