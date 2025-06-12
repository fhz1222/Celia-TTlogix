import { Pallet } from "./pallet";

export class DecantItemAddMod {
  constructor(){
    this.remarks = '';
  }

  jobNo: string;
  lineItem: number;
  newQty: number;
  newQtyPerPkg: number;
  remarks: string;
  pallet: Pallet;
}