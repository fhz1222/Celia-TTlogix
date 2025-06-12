import { Supplier } from "./supplier";

export class Product {
    code: string;
    customerSupplier: Supplier;
}

export class ProductWithDescription extends Product{
    constructor () {
        super();
    }
    description: string;
}

export class ProductFull {
    customerCodedecimalNum: number;
    description: string;
    grossWeight: number;
    height: number;
    isDefected: number;
    isStandardPackaging: number;
    length: number;
    netWeight: number;
    packageType: string;
    pkgTypeName: string;
    productCode1: string;
    sizeM3: number;
    spq: number;
    status: number;
    supplierID: string;
    uom: string;
    uomName: string;
    width: number;

    // ---customerCode
    // description
    // isDefected
    // productCode1
    // ---productCode2
    // spq
    // status
    // ---statusString
    // supplierID
    // uom
    // uomName
}
