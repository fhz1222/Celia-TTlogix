export class CompanyProfileAddressContact {
    code: string;
    addressCode: string;
    name: string;
    telNo: string;
    faxNo: string;
    email: string;
    status: number;

    // local props
    type: string = 'address';
    companyProfileCode: string;
}