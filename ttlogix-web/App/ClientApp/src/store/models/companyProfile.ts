import { CompanyProfileAddressBook } from './CompanyProfileAddressBook'
export class CompanyProfileShort {
    code: string;
    name: string;
    status: number;

    // local props
    type: string = 'company';
}

export class CompanyProfile extends CompanyProfileShort {
    addressBooks: Array<CompanyProfileAddressBook>;
}
