import { DecantItemAddMod } from "./decantItemAddMod copy";
import { DecantItemPallet } from "./decantItemPallet";

export class DecantItems{
    items: Array<DecantItem>;
    jobNo: string;
}

export class DecantItem {
    constructor(preItem: DecantItemAddMod | null = null){
        if(preItem){
            this.id = 0;
            this.sourcePalletId = '';
            this.sourceQty = 0;
            this.newPallets = new Array<DecantItemPallet>();
            return this;
        }
    }
    id: number;
    sourcePalletId: string;
    sourceQty: number;
    newPallets: Array<DecantItemPallet>;
}