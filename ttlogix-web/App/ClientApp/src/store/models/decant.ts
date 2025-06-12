export class Decant {
    jobNo: string;
    customerCode: string;
    customerName: string;
    whsCode: string;
    referenceNo: string;
    remark: string;
    status: number;
    createdBy: string;
    createdDate: string;
    completedBy: string;
    completedDate: string;
    cancelledBy: string;
    cancelledDate: string;
    items: Array<DecantPallet>;
    canComplete: boolean;
    canEdit: boolean;

    // Local properties
    _detailsList: Array<DecantDetailsList>;
    get detailsList() {
        if(!this._detailsList) {
            this._detailsList = [];
            if(this.items && this.items.length > 0){
                this.items.forEach((pallet) => { 
                    if(pallet.newPallets && pallet.newPallets.length > 0){
                        pallet.newPallets.forEach((newPallet) => { this._detailsList.push(new DecantDetailsList(newPallet, pallet.sourcePalletId, pallet.sourceQty)) });
                    }
                }
            )}
        }
        return this._detailsList;
    }
    set detailsList(list:Array<DecantDetailsList>){
        this._detailsList = list;
    }
}

export class DecantPallet {
    sourcePalletId: string;
    sourceQty: number;
    newPallets: Array<DecantNewPallet>;
}

export class DecantNewPallet {
    palletId: string;
    qty: number;
    sequenceNo: number;
    productCode: string;
    supplierId: string;
    length: number;
    width: number;
    height: number;
    netWeight: number;
    grossWeight: number;
}

export class DecantDetailsList {
    constructor(newPallet:DecantNewPallet, origPID: string, origQty: number){
        this.originalPID = origPID;
        this.productCode = newPallet.productCode;
        this.supplierID = newPallet.supplierId;
        this.originalQty = origQty;
        this.sequenceNo = newPallet.sequenceNo;
        this.pid = newPallet.palletId;
        this.newQty = newPallet.qty;
    }

    originalPID: string;
    productCode: string;
    supplierID: string;
    originalQty: number;
    sequenceNo: number;
    pid: string;
    newQty: number;
}