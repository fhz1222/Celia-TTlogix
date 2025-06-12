export class Customer {
    code: string | null;
    name: string;
    fullName: string;
}

export class CustomerFull {
    code: string;
    name: string;
    contactPerson: string;
    telephoneNo: string;
    faxNo: string;
    status: number;

    companyCode: string;
    bizUnit: string;
    buname: string;
    primaryAddress: string | null;
    billingAddress: string | null;
    shippingAddress: string | null;
    pic1: string | null;
    pic2: string | null;
    whsCode: string;
}