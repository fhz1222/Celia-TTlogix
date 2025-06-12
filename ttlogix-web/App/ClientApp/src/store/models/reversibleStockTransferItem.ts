export class ReversibleStockTransferItem {
    pid: string;
    productCode: string;
    qty: number;
    locationCode: string;
    originalLocationCode: string;

    // local props
    selected: boolean;
}