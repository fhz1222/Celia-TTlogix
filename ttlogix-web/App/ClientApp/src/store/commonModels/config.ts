export class Config {
    constructor() {
        this.images = { x: getImgUrl('x') };
        this.pagination = { itemsPerPage: new Array(25, 50, 100, 150, 200, 300, 500) };
        this.dictionaries = {
            ownership: {}, storageDetailStatus: {}, adjustmentStatus: {}, adjustmentJobType: {}, decantStatus: {}, scannerType: {}, bondedStatus: {}, inboundReversalStatus: {}, stockTransferReversalStatus: {}, customerStatus: {}, customerUomStatus: {}, customerClientStatus: {}, stockTakeStatus: {},
            areaStatus: {}, areaTypeStatus: {}, locationStatus: {}, controlCodeStatus: {}, productCodeStatus: {}, packageTypeStatus: {}, uomStatus: {}, warehouseStatus: {}, satoPrinterType: {},
            areaTypeType: {}, warehouseType: {}, packageTypeType: {}, uomType: {}, controlCodeType: {}, productCodeType: {}, locationType: {}, locationPriority: {}
        };
    }
    pagination: {
        itemsPerPage: number[];
    };
    dictionaries: Dictionaries;
    images: {
        x: string;
    };
}

export class Dictionaries {
    ownership: { [id: number]: string };
    storageDetailStatus: { [id: number]: string };
    adjustmentStatus: { [id: number]: string };
    adjustmentJobType: { [id: number]: string };
    decantStatus: { [id: number]: string };
    scannerType: { [id: number]: string };
    bondedStatus: { [id: string]: string };
    inboundReversalStatus: { [id: number]: string };
    stockTransferReversalStatus: { [id: number]: string };
    customerStatus: { [id: number]: string };
    customerUomStatus: { [id: number]: string };
    customerClientStatus: { [id: number]: string };
    stockTakeStatus: { [id: number]: string };
    areaStatus: { [id: number]: string };
    areaTypeStatus: { [id: number]: string };
    areaTypeType: { [id: number]: string };
    locationStatus: { [id: number]: string };
    controlCodeStatus: { [id: number]: string };
    controlCodeType: { [id: number]: string };
    productCodeStatus: { [id: number]: string };
    packageTypeStatus: { [id: number]: string };
    packageTypeType: { [id: number]: string };
    uomStatus: { [id: number]: string };
    warehouseStatus: { [id: number]: string };
    warehouseType: { [id: number]: string };
    satoPrinterType: { [id: number]: string };
    uomType: { [id: number]: string };
    productCodeType: { [id: number]: string };
    locationType: { [id: number]: string };
    locationPriority: { [id: number]: string };
}

export type MenuItem = MenuItemBasic | MenuItemFloat

class MenuItemBasic {
    state: string | null;
    title: string | null;
    function: ((...args: any[]) => void) | null;
    icon: string;
    stateParams: any;
}

class MenuItemFloat extends MenuItemBasic {
    float: 'end';
}

export type TableColumn = TableColumnBasic | TableColumnWithFilterName | TableColumnWithF2ResetValue | TableColumnWithForceResetToDefault

class TableColumnBasic {
    alias: string;
    dropdown: any;
    columnType: string | null;
    sortable: boolean;
    f1: any;
    f2: any;
    resetTo: any;
    specialClasses?: Array<string>;
}

class TableColumnWithFilterName extends TableColumnBasic {
    filterName: string;
    filterNameForSorting: string;
}

class TableColumnWithF2ResetValue extends TableColumnBasic {
    resetF2To: any;
}

class TableColumnWithForceResetToDefault extends TableColumnBasic {
    resetToDefault: boolean;
}


function getImgUrl(img: string) {
    var images = require.context('../../assets/images/', false, /\.svg$/);
    return images('./' + img + ".svg");
}