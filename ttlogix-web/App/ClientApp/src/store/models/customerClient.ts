export class CustomerClient {
    code: string;
    name: string;
    contactPerson: string;
    telephoneNo: string;
    faxNo: string;
    status: number;

    customerCode: string;
    companyCode: string;
    primaryAddress: string | null;
    billingAddress: string | null;
    shippingAddress: string | null;
    pic1: string | null;
    pic2: string | null;
}