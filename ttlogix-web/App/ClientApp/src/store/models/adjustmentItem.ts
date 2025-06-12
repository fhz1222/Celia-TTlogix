import { AdjustmentItemAddMod } from "./adjustmentItemAddMod";

export class AdjustmentItems{
    items: Array<AdjustmentItem>;
    jobNo: string;
}

export class AdjustmentItem {
    constructor(preItem: AdjustmentItemAddMod | null = null){
        if(preItem){
            this.jobNo = preItem.jobNo;
            this.lineItem = preItem.lineItem;
            this.productCode = preItem.pallet.product.code;
            this.supplierId = preItem.pallet.product.customerSupplier.supplierId;
            this.pid = preItem.pallet.id;
            this.initialQty = preItem.pallet.qty;
            this.newQty = preItem.newQty;
            this.initialQtyPerPkg = preItem.pallet.qtyPerPkg;
            this.newQtyPerPkg = preItem.newQtyPerPkg;
            this.remarks = preItem.remarks;
            /*this.isPositive = true;
            this.qtyIsValid = true;
            this.isAdjusted = true;*/
            return this;
        }
    }
    jobNo: string;
    lineItem: number;
    productCode: string;
    supplierId: string;
    pid: string;
    initialQty: number;
    newQty: number;
    initialQtyPerPkg: number;
    newQtyPerPkg: number;
    remarks: string;
    /*isPositive: boolean;
    qtyIsValid: boolean;
    isAdjusted: boolean;*/
}