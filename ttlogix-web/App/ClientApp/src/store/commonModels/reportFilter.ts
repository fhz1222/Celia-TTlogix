export class ReportFilter {
    dateStart: string | null;
    dateEnd: string | null;
    whsCode: string;
    customerCode: string | null;
    productCode: string | null;
    supplierID: string | null;
    bondedStatus: boolean | null;

    interval: number | null;

    locationCode: string | null;

    date: string;

    isQuantitySelected: boolean;
    isAllProductsSelected: boolean;

    // local props
    reportByProductCode: boolean;
    supplierSelect: boolean;
    productSelect: boolean;
}