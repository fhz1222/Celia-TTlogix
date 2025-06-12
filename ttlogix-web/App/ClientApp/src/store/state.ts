import { Inventory } from './models/inventory'
import { PaginatedList } from './commonModels/paginatedList'
import { Customer } from './models/customer'
import { Adjustment } from './models/adjustment'
import { PaginationData } from './commonModels/paginationData'
import { Config } from './commonModels/config'
import { StorageDetail } from './models/storageDetail'
import { Decant } from './models/decant'
import { Quarantine } from './models/quarantine'
import { Relocation } from './models/relocation'
import { Supplier } from './models/supplier'
import { Location } from './commonModels/location'
import { InvoiceBatch } from './models/invoiceBatch'
import { InboundReversal } from './models/inboundReversal'
import { StockTransferReversal } from './models/stockTransferReversal'
import { CompanyProfile } from './models/companyProfile'
import { Country } from './models/country'
import { CustomerFull } from './models/customer'
import { ControlProductCode } from './models/controlProductCode'
import { GlobalUom } from './models/customerUom'
import { StockTake } from './models/stockTake'
import { Settings } from './models/settings/settings'
import { LocationSettings } from './models/settings/location'

export type State = {
    config: Config,
    whsCode: string,
    inventoryList: PaginatedList<Inventory>,
    adjustmentList: PaginatedList<Adjustment>,
    storageDetailList: PaginatedList<StorageDetail>,
    decantList: PaginatedList<Decant>,
    quarantineList: PaginatedList<Quarantine>,
    relocationList: PaginatedList<Relocation>,
    suppliersList: Array<Supplier>,
    customers: Array<Customer>,
    locations: Array<Location>,
    factories: Array<Customer>,
    invoiceRequestList: PaginatedList<InvoiceBatch>,
    invoiceRequestFlow: "Standard" | "CustomsClearance" | "None",
    pidsRequestedFromILog: Array<string>,
    isILogEnabled: boolean,
    canEditUnloadingPoint: boolean,
    inboundReversalList: PaginatedList<InboundReversal>,
    stockTransferReversalList: PaginatedList<StockTransferReversal>,
    companyProfileList: Array<CompanyProfile>,
    customersFullList: Array<CustomerFull>,
    controlCodes: Array<ControlProductCode>,
    productCodes: Array<ControlProductCode>,
    globalUomList: Array<GlobalUom>,
    stockTakeList: PaginatedList<StockTake>
    settings: Settings,
    settingsLocationList: PaginatedList<LocationSettings>
}

const config: Config = new Config();

export const state: State = {
    config,
    whsCode: 'PL',
    inventoryList: initList<Inventory>(),
    adjustmentList: initList<Adjustment>(),
    storageDetailList: initList<StorageDetail>(),
    decantList: initList<Decant>(),
    quarantineList: initList<Quarantine>(),
    relocationList: initList<Relocation>(),
    suppliersList: new Array<Supplier>(),
    customers: [],
    locations: [],
    factories: [],
    invoiceRequestList: initList<InvoiceBatch>(),
    invoiceRequestFlow: "None",
    pidsRequestedFromILog: [],
    isILogEnabled: false,
    canEditUnloadingPoint: false,
    inboundReversalList: initList<InboundReversal>(),
    stockTransferReversalList: initList<StockTransferReversal>(),
    companyProfileList: new Array<CompanyProfile>(),
    customersFullList: new Array<CustomerFull>(),
    controlCodes: new Array<ControlProductCode>(),
    productCodes: new Array<ControlProductCode>(),
    globalUomList: new Array<GlobalUom>(),
    stockTakeList: initList<StockTake>(),
    settings: new Settings(),
    settingsLocationList: initList<LocationSettings>()
}

function initList<T>() {
    var newList = new PaginatedList<T>();
    newList.items = new Array<T>();
    newList.pagination = new PaginationData();
    newList.pagination.hasNextPage = false;
    newList.pagination.hasPreviousPage = false;
    newList.pagination.itemsPerPage = config.pagination.itemsPerPage[0];
    newList.pagination.pageNumber = 1;
    newList.pagination.totalCount = 0;
    newList.pagination.totalPages = 1;
    return newList;
}