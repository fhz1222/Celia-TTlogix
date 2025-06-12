import { AdjustmentItem } from "./adjustmentItem";
import { Pallet } from "./pallet";
import { ProductWithDescription } from "./product";
import { Supplier } from "./supplier";

export class AdjustmentItemAddMod {
  constructor(item: AdjustmentItem | null = null){
    if(item){
        this.jobNo = item.jobNo;
        this.lineItem = item.lineItem;
        this.remarks = item.remarks;
        this.newQty = 0;
        this.newQtyPerPkg = 0;

        this.pallet = new Pallet();
        this.pallet.product = new ProductWithDescription();
        this.pallet.product.customerSupplier = new Supplier();
        
        this.pallet.product.code = item.productCode;
        this.pallet.product.customerSupplier.supplierId = item.supplierId;
        this.pallet.qty = item.initialQty;
        this.pallet.qtyPerPkg = item.initialQtyPerPkg;
        this.pallet.product.description = '';

        return this;
    } else {
        this.remarks = '';
    }
}

  jobNo: string;
  lineItem: number;
  newQty: number;
  newQtyPerPkg: number;
  remarks: string;
  pallet: Pallet;
}