import { MutationTree } from 'vuex'
import { MutationTypes } from './mutation-types'
import { State } from './state'
import { Inventory } from './models/inventory'
import { Customer } from './models/customer'
import { Adjustment } from './models/adjustment'
import { PaginatedList } from './commonModels/paginatedList'
import { Dictionaries } from './commonModels/config'
import { StorageDetail } from './models/storageDetail'
import { AdjustmentItem } from './models/adjustmentItem'
import { Decant } from './models/decant'
import { Quarantine } from './models/quarantine'
import { Relocation } from './models/relocation'
import { Location } from './commonModels/location'
import { InvoiceBatch } from './models/invoiceBatch'
import { InboundReversal } from './models/inboundReversal'
import { InboundReversalItem } from './models/inboundReversalItem'
import { StockTransferReversal } from './models/stockTransferReversal'
import { StockTransferReversalFilters } from './models/stockTransferReversalFilters'
import { StockTransferReversalItem } from './models/stockTransferReversalItem'
import { StockTransferReversalItemFilters } from './models/stockTransferReversalItemFilters'
import { CompanyProfile, CompanyProfileShort } from './models/companyProfile'
import { CompanyProfileAddressBookShort } from './models/CompanyProfileAddressBook'
import { CompanyProfileAddressContact } from './models/companyProfileAddressContact'
import { Country } from './models/country'
import { CustomerFull } from './models/customer'
import { ControlProductCode } from './models/controlProductCode'
import { GlobalUom } from './models/customerUom'
import { StockTake } from './models/stockTake'
import { store } from '.'
import { Area } from './models/settings/area'
import { AreaFilters } from './models/settings/areaFilters'
import { AreaType } from './models/settings/areaType'
import { AreaTypeFilters } from './models/settings/areaTypeFilters'
import { ControlCode } from './models/settings/controlCode'
import { ControlCodeFilters } from './models/settings/controlCodeFilters'
import { LocationSettings } from './models/settings/location'
import { LocationFilters } from './models/settings/locationFilters'
import { PackageType } from './models/settings/packageType'
import { PackageTypeFilters } from './models/settings/packageTypeFilters'
import { ProductCode } from './models/settings/productCode'
import { ProductCodeFilters } from './models/settings/productCodeFilters'
import { SatoPrinter } from './models/settings/satoPrinter'
import { SatoPrinterFilters } from './models/settings/satoPrinterFilters'
import { UomSettings } from './models/settings/uom'
import { UomSettingsFilters } from './models/settings/uomFilters'
import { Warehouse } from './models/settings/warehouse'
import { WarehouseFilters } from './models/settings/warehouseFilters'

export type Mutations<S = State> = {
  [MutationTypes.SET_DICT](state: S, data: Dictionaries): void;
  [MutationTypes.SET_LIST_INV](state: S, data: PaginatedList<Inventory>): void;
  [MutationTypes.SET_LIST_ADJ](state: S, data: PaginatedList<Adjustment>): void;
  [MutationTypes.SET_ADJ_ITEMS](state: S, data: { jobNo: string, items: Array<AdjustmentItem> }): void;
  [MutationTypes.SET_NEW_ADJ](state: S, data: Adjustment): void;
  [MutationTypes.SET_LIST_SD](state: S, data: PaginatedList<StorageDetail>): void;
  [MutationTypes.SET_CUSTOMERS](state: S, data: Customer[]): void;
  [MutationTypes.SET_WHS](state: S, data: string): void;
    [MutationTypes.UPDATE_ADJ](state: S, data: Adjustment): void;
    [MutationTypes.CANCEL_ADJ_ITEM](state: S, data: Adjustment): void;
  [MutationTypes.ADD_ADJ_ITEM](state: S, data: {adjItem: AdjustmentItem, addNew: boolean}): void;
  [MutationTypes.DEL_ADJ_ITEM](state: S, data: {jobNo: string, lineItem: number}): void;

  // Decant
  [MutationTypes.INV_DEC_SET_LIST](state: S, data: PaginatedList<Decant>): void;
  [MutationTypes.INV_DEC_UPDATE](state: S, data: Decant): void;
  [MutationTypes.INV_DEC_SET_DECANT](state: S, data: Decant): void;

  // Quarantine
  [MutationTypes.INV_QNE_SET_LIST](state: S, data: PaginatedList<Quarantine>): void;
  [MutationTypes.INV_QNE_UPDATE_REASON](state: S, data: {pids: Array<string>, reason: string}): void;

  // Relocation
  [MutationTypes.INV_REL_SET_LIST](state: S, data: PaginatedList<Relocation>): void;

  // Common
  [MutationTypes.SET_LOCATIONS](state: S, data: Array<Location>): void;

    // Invoicing
    [MutationTypes.SET_FACTORIES](state: S, data: Array<Customer>): void;
    [MutationTypes.SET_LIST_INVC](state: S, data: {list: PaginatedList<InvoiceBatch>, flow: "Standard" | "CustomsClearance" | "None"}): void;
    [MutationTypes.INVC_SHOW_INVOICE_PDF](state: S, data: any): void;
    [MutationTypes.SET_ILOG_ENABLED](state: S, data: boolean): void;
    [MutationTypes.SET_PIDS_REQUESTED_FROM_ILOG](state: S, data: Array<string>): void;
    [MutationTypes.SET_CAN_EDIT_UNLOADING_POINT](state: S, data: boolean): void;

    // Inbound Reversal
    [MutationTypes.SET_LIST_IR](state: S, data: PaginatedList<InboundReversal>): void;
    [MutationTypes.SET_NEW_IR](state: S, data: InboundReversal): void;
    [MutationTypes.COMPLETE_OR_CANCEL_IR](state: S, data: InboundReversal): void;
    [MutationTypes.SET_IR_ITEMS](state: S, data: { jobNo: string, items: Array<InboundReversalItem> }): void;
    [MutationTypes.UPDATE_IR_STATUS](state: S, data: { jobNo: string, status: number }): void;
    [MutationTypes.UPDATE_IR](state: S, data: { updatedIR: InboundReversal }): void;

    // Stock Transfer Reversal
    [MutationTypes.SET_LIST_STR](state: S, data: PaginatedList<StockTransferReversal>): void;
    [MutationTypes.SET_NEW_STR](state: S, data: StockTransferReversal): void;
    [MutationTypes.COMPLETE_OR_CANCEL_STR](state: S, data: StockTransferReversal): void;
    [MutationTypes.SET_STR_ITEMS](state: S, data: { jobNo: string, items: Array<StockTransferReversalItem> }): void;
    [MutationTypes.UPDATE_STR_STATUS](state: S, data: { jobNo: string, status: number }): void;
    [MutationTypes.UPDATE_STR](state: S, data: { updatedSTR: StockTransferReversal }): void;

    // Company Profile
    [MutationTypes.SET_LIST_CP](state: S, data: { companyProfiles: Array<CompanyProfile> }): void;
    [MutationTypes.UPDATE_COMPANY_PROFILE](state: S, data: { updatedCompany: CompanyProfileShort }): void;
    [MutationTypes.UPDATE_ADDRESS_BOOK](state: S, data: { updatedAddressBook: CompanyProfileAddressBookShort }): void;
    [MutationTypes.UPDATE_ADDRESS_CONTACT](state: S, data: { companyCode: string, updatedAddressContact: CompanyProfileAddressContact }): void;

    // Customers
    [MutationTypes.SET_LIST_CS](state: S, data: { customers: Array<CustomerFull> }): void;
    [MutationTypes.UPDATE_CUSTOMER](state: S, data: { updatedCustomer: CustomerFull }): void;
    [MutationTypes.SET_PRODUCT_CODES](state: S, data: { productCodes: Array<ControlProductCode> }): void;
    [MutationTypes.SET_CONTROL_CODES](state: S, data: { controlCodes: Array<ControlProductCode> }): void;
    [MutationTypes.SET_GLOBAL_UOM](state: S, data: { globalUomList: Array<GlobalUom> }): void;

    // Stock Take
    [MutationTypes.SET_LIST_STL](state: S, data: PaginatedList<StockTake>): void;
    [MutationTypes.UPDATE_STL](state: S, data: { updatedStockTake: StockTake }): void;
    // Settings
    [MutationTypes.SET_AREA_LIST](state: S, data: { areaList: Array<Area> }): void;
    [MutationTypes.SET_AREATYPE_LIST](state: S, data: { areaTypeList: Array<AreaType> }): void;
    [MutationTypes.SET_CC_LIST](state: S, data: { ccList: Array<ControlCode> }): void;
    [MutationTypes.SET_LOCATION_LIST](state: S, data: PaginatedList<LocationSettings>): void;
    [MutationTypes.SET_PACKAGETYPE_LIST](state: S, data: { packageTypeList: Array<PackageType> }): void;
    [MutationTypes.SET_PC_LIST](state: S, data: { pcList: Array<ProductCode> }): void;
    [MutationTypes.SET_SATO_LIST](state: S, data: { satoPrinterList: Array<SatoPrinter> }): void;
    [MutationTypes.SET_UOM_LIST](state: S, data: { uomList: Array<UomSettings> }): void;
    [MutationTypes.SET_WAREHOUSE_LIST](state: S, data: { warehouseList: Array<Warehouse> }): void;
    [MutationTypes.UPDATE_LOCATION_SETTINGS](state: S, data: LocationSettings): void;
}

export const mutations: MutationTree<State> & Mutations = {
    [MutationTypes.SET_DICT](state, data) { state.config.dictionaries = data; },
    [MutationTypes.SET_LIST_INV](state, data) { state.inventoryList = data; },
    [MutationTypes.SET_LIST_ADJ](state, data) { state.adjustmentList = data; },
    [MutationTypes.SET_LIST_SD](state, data) { state.storageDetailList = data; },
    [MutationTypes.SET_CUSTOMERS](state, data) { state.customers = data; },
    [MutationTypes.SET_WHS](state, data) { state.whsCode = data; },
    [MutationTypes.SET_CAN_EDIT_UNLOADING_POINT](state, data) { state.canEditUnloadingPoint = data; },
    [MutationTypes.SET_ADJ_ITEMS](state, data) {
        for (var ai = 0; ai < state.adjustmentList.items.length; ai++) {
            if (state.adjustmentList.items[ai].jobNo == data.jobNo) {
                state.adjustmentList.items[ai].items = data.items;
            }
        }
    },
    [MutationTypes.SET_NEW_ADJ](state, data) { state.adjustmentList.items.push(data) },
    [MutationTypes.UPDATE_ADJ](state, data) {
        for (var ai = 0; ai < state.adjustmentList.items.length; ai++) {
            if (state.adjustmentList.items[ai].jobNo == data.jobNo) {
                var items = state.adjustmentList.items[ai].items;
                state.adjustmentList.items[ai] = data;
                state.adjustmentList.items[ai].items = items;
            }
        }
    },
    [MutationTypes.CANCEL_ADJ_ITEM](state, data) {
        const objIndex = state.adjustmentList.items.findIndex(el => el.jobNo == data.jobNo);
        if (objIndex > -1) state.adjustmentList.items[objIndex] = data;
    },
    [MutationTypes.ADD_ADJ_ITEM](state, data) {
        for (var ai = 0; ai < state.adjustmentList.items.length; ai++) {
            if (state.adjustmentList.items[ai].jobNo == data.adjItem.jobNo) {
                if (data.addNew) {
                    state.adjustmentList.items[ai].items.push(data.adjItem);
                } else {
                    for (var ii = 0; ii < state.adjustmentList.items[ai].items.length; ii++) {
                        if (state.adjustmentList.items[ai].items[ii].lineItem == data.adjItem.lineItem) {
                            state.adjustmentList.items[ai].items[ii] = data.adjItem;
                        }
                    }
                }
            }
        }
    },
    [MutationTypes.DEL_ADJ_ITEM](state, data) {
        for (var ai = 0; ai < state.adjustmentList.items.length; ai++) {
            if (state.adjustmentList.items[ai].jobNo == data.jobNo) {
                for (var it = 0; it < state.adjustmentList.items[ai].items.length; it++) {
                    if (state.adjustmentList.items[ai].items[it].lineItem == data.lineItem) {
                        state.adjustmentList.items[ai].items.splice(it, 1);
                    }
                }
            }
        }
    },

    // Decant
    [MutationTypes.INV_DEC_SET_LIST](state, data) { state.decantList = data },
    [MutationTypes.INV_DEC_UPDATE](state, data) {
        for (var dec = 0; dec < state.decantList.items.length; dec++) {
            if (state.decantList.items[dec].jobNo == data.jobNo) {
                state.decantList.items[dec] = data;
                /*var items = state.decantList.items[dec].items;
                state.decantList.items[dec] = data;
                state.decantList.items[dec].items = items;*/
            }
        }
    },
    [MutationTypes.INV_DEC_SET_DECANT](state, data) {
        state.decantList.items.push(data);
    },

    // Quarantine
    [MutationTypes.INV_QNE_SET_LIST](state, data) { state.quarantineList = data },
    [MutationTypes.INV_QNE_UPDATE_REASON](state, { pids, reason }) {
        for (var p = 0; p < pids.length; p++) {
            const objIndex = state.quarantineList.items.findIndex(el => el.pid == pids[p]);
            if (objIndex > -1) state.quarantineList.items[objIndex].reason = reason;
        }
    },

    // Relocation
    [MutationTypes.INV_REL_SET_LIST](state, data) { state.relocationList = data },

    // Common
    [MutationTypes.SET_LOCATIONS](state, data) { state.locations = data },

    // Invoicing
    [MutationTypes.SET_FACTORIES](state, data) { state.factories = data },
    [MutationTypes.SET_LIST_INVC](state, { list, flow }) { state.invoiceRequestList = list; state.invoiceRequestFlow = flow },
    [MutationTypes.INVC_SHOW_INVOICE_PDF](state, data) {
        const url = URL.createObjectURL(new Blob([data.data], {
            type: data.type
        }));
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', (data.invoiceId + '.pdf'));
        document.body.appendChild(link);
        link.click();
    },

    [MutationTypes.SET_ILOG_ENABLED](state, data) { state.isILogEnabled = data },
    [MutationTypes.SET_PIDS_REQUESTED_FROM_ILOG](state, data) { state.pidsRequestedFromILog = data },

    // Inbound Reversal
    [MutationTypes.SET_LIST_IR](state, data) { state.inboundReversalList = data },
    [MutationTypes.SET_NEW_IR](state, data) { state.inboundReversalList.items.push(data) },
    [MutationTypes.COMPLETE_OR_CANCEL_IR](state, data) {
        const objIndex = state.inboundReversalList.items.findIndex(el => el.jobNo == data.jobNo);
        if (objIndex > -1) state.inboundReversalList.items[objIndex] = data;
    },
    [MutationTypes.SET_IR_ITEMS](state, data) {
        for (var iri = 0; iri < state.inboundReversalList.items.length; iri++) {
            if (state.inboundReversalList.items[iri].jobNo == data.jobNo) {
                state.inboundReversalList.items[iri].items = data.items;
            }
        }
    },
    [MutationTypes.UPDATE_IR_STATUS](state, data) {
        const objIndex = state.inboundReversalList.items.findIndex(el => el.jobNo == data.jobNo);
        if (objIndex > -1) state.inboundReversalList.items[objIndex].status = data.status;
    },
    [MutationTypes.UPDATE_IR](state, data) {
        const objIndex = state.inboundReversalList.items.findIndex(el => el.jobNo == data.updatedIR.jobNo);
        if (objIndex > -1) state.inboundReversalList.items[objIndex] = data.updatedIR;
    },

    // Stock Transfer Reversal
    [MutationTypes.SET_LIST_STR](state, data) { state.stockTransferReversalList = data },
    [MutationTypes.SET_NEW_STR](state, data) { state.stockTransferReversalList.items.push(data) },
    [MutationTypes.COMPLETE_OR_CANCEL_STR](state, data) {
        const objIndex = state.stockTransferReversalList.items.findIndex(el => el.jobNo == data.jobNo);
        if (objIndex > -1) state.stockTransferReversalList.items[objIndex] = data;
    },
    [MutationTypes.SET_STR_ITEMS](state, data) {
        for (var iri = 0; iri < state.stockTransferReversalList.items.length; iri++) {
            if (state.stockTransferReversalList.items[iri].jobNo == data.jobNo) {
                state.stockTransferReversalList.items[iri].items = data.items;
            }
        }
    },
    [MutationTypes.UPDATE_STR_STATUS](state, data) {
        const objIndex = state.stockTransferReversalList.items.findIndex(el => el.jobNo == data.jobNo);
        if (objIndex > -1) state.stockTransferReversalList.items[objIndex].status = data.status;
    },
    [MutationTypes.UPDATE_STR](state, data) {
        const objIndex = state.stockTransferReversalList.items.findIndex(el => el.jobNo == data.updatedSTR.jobNo);
        if (objIndex > -1) state.stockTransferReversalList.items[objIndex] = data.updatedSTR;
    },

    // Company Profile
    [MutationTypes.SET_LIST_CP](state, data) { state.companyProfileList = data.companyProfiles },
    [MutationTypes.UPDATE_COMPANY_PROFILE](state, data) {
        const cpindex = state.companyProfileList.findIndex((cp) => cp.code == data.updatedCompany.code);
        state.companyProfileList[cpindex] = { ...data.updatedCompany, addressBooks: state.companyProfileList[cpindex].addressBooks }
    },
    [MutationTypes.UPDATE_ADDRESS_BOOK](state, data) {
        const cpindex = state.companyProfileList.findIndex((cp) => cp.code == data.updatedAddressBook.companyCode);
        const abindex = state.companyProfileList[cpindex].addressBooks.findIndex((ab) => ab.code == data.updatedAddressBook.code);
        // new
        if (!state.companyProfileList[cpindex].addressBooks.some((ab) => ab.code == data.updatedAddressBook.code)) {
            state.companyProfileList[cpindex].addressBooks.push({ ...data.updatedAddressBook, addressContacts: new Array<CompanyProfileAddressContact>() })
        }
        // update
        else {
            state.companyProfileList[cpindex].addressBooks[abindex] = { ...data.updatedAddressBook, addressContacts: state.companyProfileList[cpindex].addressBooks[abindex].addressContacts }
        }
    },
    [MutationTypes.UPDATE_ADDRESS_CONTACT](state, data) {
        const cpindex = state.companyProfileList.findIndex((cp) => cp.code == data.companyCode);
        const abindex = state.companyProfileList[cpindex].addressBooks.findIndex((ab) => ab.code == data.updatedAddressContact.addressCode);
        const acindex = state.companyProfileList[cpindex].addressBooks[abindex].addressContacts.findIndex((ac) => ac.code == data.updatedAddressContact.code);

        // new
        if (!state.companyProfileList[cpindex].addressBooks[abindex].addressContacts.some((ac) => ac.code == data.updatedAddressContact.code)) {
            state.companyProfileList[cpindex].addressBooks[abindex].addressContacts.push(data.updatedAddressContact)
        }
        // update
        else {
            state.companyProfileList[cpindex].addressBooks[abindex].addressContacts[acindex] = data.updatedAddressContact;
        }
    },

    // Customers
    [MutationTypes.SET_LIST_CS](state, data) { state.customersFullList = data.customers },
    [MutationTypes.UPDATE_CUSTOMER](state, data) {
        // new
        if (!state.customers.some((cs) => cs.code == data.updatedCustomer.code)) {
            let newCustShort = new Customer();
            newCustShort.code = data.updatedCustomer.code;
            newCustShort.name = data.updatedCustomer.name;
            state.customers.push(newCustShort);
        }
        // update
        else {
            const cssIndex = state.customers.findIndex((css) => css.code == data.updatedCustomer.code);
            state.customers[cssIndex].name = data.updatedCustomer.name;
        }
    },
    [MutationTypes.SET_PRODUCT_CODES](state, data) { state.productCodes = data.productCodes },
    [MutationTypes.SET_CONTROL_CODES](state, data) { state.controlCodes = data.controlCodes },
    [MutationTypes.SET_GLOBAL_UOM](state, data) { state.globalUomList = data.globalUomList },

    // Stock Take
    [MutationTypes.SET_LIST_STL](state, data) { state.stockTakeList = data },
    [MutationTypes.UPDATE_STL](state, data) {
        let stlIndex = store.state.stockTakeList.items.findIndex((stl) => stl.jobNo == data.updatedStockTake.jobNo);
        state.stockTakeList.items[stlIndex] = data.updatedStockTake;
    },

    // Settings
    [MutationTypes.SET_AREA_LIST](state, data) { state.settings.area = data.areaList; },
    [MutationTypes.SET_AREATYPE_LIST](state, data) { state.settings.areaType = data.areaTypeList },
    [MutationTypes.SET_CC_LIST](state, data) { state.settings.controlCode = data.ccList },
    [MutationTypes.SET_LOCATION_LIST](state, data) { state.settingsLocationList = data },
    [MutationTypes.SET_PACKAGETYPE_LIST](state, data) { state.settings.packageType = data.packageTypeList; },
    [MutationTypes.SET_PC_LIST](state, data) { state.settings.productCode = data.pcList },
    [MutationTypes.SET_SATO_LIST](state, data) { state.settings.satoPrinter = data.satoPrinterList },
    [MutationTypes.SET_UOM_LIST](state, data) { state.settings.uom = data.uomList },
    [MutationTypes.SET_WAREHOUSE_LIST](state, data) { state.settings.warehouse = data.warehouseList },
    [MutationTypes.UPDATE_LOCATION_SETTINGS](state, data) {
        const updatedLocIndex = state.settingsLocationList.items.findIndex((l) => l.code == data.code);
        state.settingsLocationList.items[updatedLocIndex].status = data.status;
        state.settingsLocationList.items[updatedLocIndex].name = data.name;
        state.settingsLocationList.items[updatedLocIndex].isPriority = data.isPriority;
        state.settingsLocationList.items[updatedLocIndex].m3 = data.m3;
    },
}
