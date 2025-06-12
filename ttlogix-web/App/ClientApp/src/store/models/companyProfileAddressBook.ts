import { CompanyProfileAddressContact } from './companyProfileAddressContact'

export class CompanyProfileAddressBookShort {
    constructor() {
        this.type = 'addressBook';
    }
    code: string;
    companyCode: string;
    status: number;
    address1: string;
    address2: string;
    address3: string;
    address4: string;
    postCode: string;
    country: string;
    telNo: string;
    faxNo: string;

    // local props
    type: string;
}
export class CompanyProfileAddressBook extends CompanyProfileAddressBookShort {
    addressContacts: Array<CompanyProfileAddressContact>;
}

