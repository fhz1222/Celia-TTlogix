// import vue stuff
import { ActionTree, ActionContext } from 'vuex'
import { State } from './state'
import { Mutations } from './mutations'
import { ActionTypes } from './action-types'
import { MutationTypes } from './mutation-types'
//import { RootState } from '@/store/state'
import { AxiosInstance, AxiosResponse } from 'axios'
import axios from '@/axios-instance'
import qs from 'qs'

// import types
import { Inventory } from './models/inventory'
import { PaginatedList } from './commonModels/paginatedList'
import { PaginationData } from './commonModels/paginationData'
import { InventoryFilters } from './models/inventoryFilters'
import { Adjustment } from './models/adjustment'
import { StorageDetail } from './models/storageDetail'
import { StorageDetailFilters } from './models/storageDetailFilters'
import { AdjustmentFilters } from './models/adjustmentFilters'
import { AdjustmentItem } from './models/adjustmentItem'
import { Sorting } from './commonModels/Sorting'
import { AdjustmentItemAddMod } from './models/adjustmentItemAddMod'
import { DecantFilters } from './models/decantFilters'
import { Decant, DecantDetailsList } from './models/decant'
import { Quarantine } from './models/quarantine'
import { QuarantineFilters } from './models/quarantineFilters'
import { RelocationFilters } from './models/relocationFilters'
import { Relocation } from './models/relocation'
import { SupplierRegular } from './models/supplier'
import { ProductFull } from './models/product'
import { ReportFilter } from './commonModels/reportFilter'
import { Pallet } from './models/pallet'
import { Report } from './commonModels/report'
import { InvoiceBatch } from './models/invoiceBatch'
import { InvoiceBatchFilters } from './models/invoiceBatchFilters'
import { InboundReversal } from './models/inboundReversal'
import { InboundReversalFilters } from './models/InboundReversalFilters'
import { ReversibleInbound } from './models/reversibleInbound'
import { InboundReversalItem } from './models/inboundReversalItem'
import { InboundReversalItemFilters } from './models/inboundReversalItemFilters'
import { StockTransferReversal } from './models/stockTransferReversal'
import { StockTransferReversalFilters } from './models/stockTransferReversalFilters'
import { StockTransferReversalItem } from './models/stockTransferReversalItem'
import { StockTransferReversalItemFilters } from './models/stockTransferReversalItemFilters'
import { CompanyProfile } from './models/companyProfile'
import { CompanyProfileShort } from './models/companyProfile'
import { CompanyProfileAddressBookShort } from './models/companyProfileAddressBook'
import { CompanyProfileAddressContact } from './models/companyProfileAddressContact'
import { Customer, CustomerFull } from './models/customer'
import { CustomerFilters } from './models/customerFilters'
import { ControlProductCode } from './models/controlProductCode'
import { CustomerInventory } from './models/customerInventory'
import { CustomerUom, GlobalUom } from './models/customerUom'
import { CustomerUomFilters } from './models/customerUomFilters'
import { CustomerClient } from './models/customerClient'
import { CustomerClientFilters } from './models/customerClientFilters'
import { StockTake } from './models/stockTake'
import { StandbyLocation } from './models/standbyLocation'
import { StockTakeFilters } from './models/stockTakeFilters'
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
import { SettingsComboBox } from './models/settings/settingsComboBox'
import { LocationLabel, LocationQrCode } from '@/store/models/settings/locationLabel'

// import common logic
import { fromJSON } from './commonFunctions/miscellaneous'
import { ReversibleInboundItemFilters } from './models/reversibleInboundItemFilters'
import { ReversibleInboundItem } from './models/reversibleInboundItem'
import { StockTakeDetail } from './models/stockTakeDetail'

let apiAxios: AxiosInstance = axios.$axios;

type AugmentedActionContext = {
    commit<K extends keyof Mutations>(
        key: K,
        payload: Parameters<Mutations[K]>[1]
    ): ReturnType<Mutations[K]>
} & Omit<ActionContext<State, State>, 'commit'>

export interface Actions {
    // Inventory
    [ActionTypes.INV_LOAD_LIST]({ commit }: AugmentedActionContext, payload: { filters: InventoryFilters }): Promise<void>;

    //Adjustment
    [ActionTypes.INV_LOAD_ADJ_LIST]({ commit }: AugmentedActionContext, payload: { filters: AdjustmentFilters }): Promise<void>;
    [ActionTypes.INV_ADD_ADJ]({ commit }: AugmentedActionContext, payload: { customerCode: string, isUndoZeroOut: boolean }): Promise<Adjustment>;
    [ActionTypes.INV_UPDATE_ADJ]({ commit }: AugmentedActionContext, payload: { jobNo: string, referenceNo: string, reason: string }): Promise<Adjustment>;
    [ActionTypes.INV_CANCEL_ADJ]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<void>;
    [ActionTypes.INV_COMPLETE_ADJ]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<void>;
    [ActionTypes.INV_ADJ_ITEM_PREPARE_NEW]({ commit }: AugmentedActionContext, payload: { pid: string, jobNo: string }): Promise<AdjustmentItemAddMod>;
    [ActionTypes.INV_ADJ_ITEM_GET]({ commit }: AugmentedActionContext, payload: { jobNo: string, lineItem: number }): Promise<AdjustmentItemAddMod>;
    [ActionTypes.INV_ADJ_ITEM_SAVE]({ commit }: AugmentedActionContext, payload: { adjItem: AdjustmentItem }): Promise<void>;
    [ActionTypes.INV_DEL_ITEM]({ commit }: AugmentedActionContext, payload: { jobNo: string, lineItem: number }): Promise<void>;
    [ActionTypes.INV_LOAD_ADJ_ITEMS]({ commit }: AugmentedActionContext, payload: { jobNo: string, filters: Sorting }): Promise<Array<AdjustmentItem>>;
    [ActionTypes.INV_ADJ_GET_DETAILS]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<Adjustment>;

    // Quarantine
    [ActionTypes.INV_QNE_LOAD_LIST]({ commit }: AugmentedActionContext, payload: { filters: QuarantineFilters }): Promise<void>;
    [ActionTypes.INV_QNE_REASON]({ commit }: AugmentedActionContext, payload: { pids: Array<string>, reason: string }): Promise<void>;
    [ActionTypes.REPORT_QUARANTINE]({ commit }: AugmentedActionContext, payload: { filter: ReportFilter }): Promise<ArrayBuffer>;

    // Relocation
    [ActionTypes.INV_REL_LOAD_LIST]({ commit }: AugmentedActionContext, payload: { filters: RelocationFilters }): Promise<void>;

    // Decant
    [ActionTypes.INV_DEC_LOAD_LIST]({ commit }: AugmentedActionContext, payload: { filters: DecantFilters }): Promise<void>;
    [ActionTypes.INV_DEC_ADD]({ commit }: AugmentedActionContext, payload: { customerCode: string }): Promise<string>;
    [ActionTypes.INV_DEC_UPDATE]({ commit }: AugmentedActionContext, payload: { jobNo: string, referenceNo: string, remark: string }): Promise<void>;
    [ActionTypes.INV_DEC_CANCEL]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<void>;
    [ActionTypes.INV_DEC_COMPLETE]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<void>;
    [ActionTypes.INV_DEC_DETAILS]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<void>;
    [ActionTypes.INV_DEC_ITEM_PREPARE_NEW]({ commit }: AugmentedActionContext, payload: { jobNo: string, pid: string }): Promise<Pallet>;
    [ActionTypes.INV_DEC_ADD_ITEMS]({ commit }: AugmentedActionContext, payload: { jobNo: string, pid: string, newQuantities: Array<number> }): Promise<void>;
    [ActionTypes.INV_DEC_DEL_ITEMS]({ commit }: AugmentedActionContext, payload: { jobNo: string, pid: string }): Promise<void>;
    [ActionTypes.INV_DEC_REPORT]({ commit }: AugmentedActionContext, payload: { decantDetails: Array<DecantDetailsList> }): Promise<Blob>;

    // Storage Details
    [ActionTypes.INV_SD_LOAD_LIST]({ commit }: AugmentedActionContext, payload: { filters: StorageDetailFilters }): Promise<void>;

    // Common
    [ActionTypes.GET_SUPPLIERS]({ commit }: AugmentedActionContext, payload: { factoryId: string }): Promise<Array<SupplierRegular>>;
    [ActionTypes.GET_PRODUCTS]({ commit }: AugmentedActionContext, payload: { customerCode: string, supplierID: string }): Promise<Array<ProductFull>>;
    [ActionTypes.GET_PRODUCTS_BY_CUSTOMER]({ commit }: AugmentedActionContext, payload: { customerCode: string }): Promise<Array<ProductFull>>;
    [ActionTypes.INV_LOAD_CUSTOMERS]({ commit }: AugmentedActionContext): Promise<void>;
    [ActionTypes.LOAD_DICT]({ commit }: AugmentedActionContext): Promise<void>;
    [ActionTypes.INV_REPORT]({ commit }: AugmentedActionContext, payload: { filter: ReportFilter | null, report: Report }): Promise<ArrayBuffer>;
    [ActionTypes.LOAD_LOCATIONS]({ commit }: AugmentedActionContext, payload: { whsCode: string }): Promise<void>;

    // Invoicing
    [ActionTypes.INVC_LOAD_FACTORIES]({ commit }: AugmentedActionContext): Promise<void>;
    [ActionTypes.INVC_LOAD_LIST]({ commit }: AugmentedActionContext, payload: { filters: InvoiceBatchFilters }): Promise<void>;
    [ActionTypes.INVC_LOAD_MORE]({ commit, state }: AugmentedActionContext, payload: { filters: InvoiceBatchFilters }): Promise<void>;
    [ActionTypes.INVC_UPDATE_BATCH]({ commit }: AugmentedActionContext, payload: { batchId: number, hour: number, comment: string }): Promise<void>;
    [ActionTypes.CONFIRM_INVC]({ commit }: AugmentedActionContext, payload: { batchId: number }): Promise<void>;
    [ActionTypes.REJECT_INVC]({ commit }: AugmentedActionContext, payload: { batchId: number }): Promise<void>;
    [ActionTypes.INVC_GET_INVOICE_PDF]({ commit }: AugmentedActionContext, payload: { invoiceId: string }): Promise<AxiosResponse<Blob>>;
    [ActionTypes.INV_RELOAD_ILOG_ENABLED]({ commit }: AugmentedActionContext): Promise<void>;
    [ActionTypes.INV_RELOAD_ILOG_PIDS]({ commit }: AugmentedActionContext): Promise<void>;
    [ActionTypes.REQUEST_FROM_ILOG]({ commit }: AugmentedActionContext, payload: { pids: Array<string> }): Promise<void>;

    // Inbound Reversal
    [ActionTypes.IR_LOAD_LIST]({ commit }: AugmentedActionContext, payload: { filters: InboundReversalFilters }): Promise<void>;
    [ActionTypes.IR_LOAD_REVERSIBLE_INBOUND]({ commit }: AugmentedActionContext, payload: { filters: InboundReversalFilters }): Promise<ReversibleInbound>;
    [ActionTypes.IR_ADD_REVERSAL]({ commit }: AugmentedActionContext, payload: { inJobNo: string }): Promise<InboundReversal>;
    [ActionTypes.IR_COMPLETE]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<void>;
    [ActionTypes.IR_CANCEL]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<void>;
    [ActionTypes.IR_LOAD_ITEMS]({ commit }: AugmentedActionContext, payload: { filters: InboundReversalItemFilters }): Promise<Array<InboundReversalItem>>;
    [ActionTypes.IR_GET_DETAILS]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<InboundReversal>;
    [ActionTypes.IR_LOAD_REVERSIBLE_INBOUND_ITEMS]({ commit }: AugmentedActionContext, payload: { filters: ReversibleInboundItemFilters }): Promise<Array<ReversibleInboundItem>>;
    [ActionTypes.IR_ADD_INBOUND_REVERSAL_ITEMS]({ commit }: AugmentedActionContext, payload: { jobNo: string, pids: Array<string> }): Promise<void>;
    [ActionTypes.IR_DELETE_ITEM]({ commit }: AugmentedActionContext, payload: { jobNo: string, pid: string }): Promise<void>;
    [ActionTypes.IR_UPDATE]({ commit }: AugmentedActionContext, payload: { jobNo: string, refNo: string, reason: string }): Promise<void>;

    // Stock Transfer Reversal
    [ActionTypes.STR_LOAD_LIST]({ commit }: AugmentedActionContext, payload: { filters: StockTransferReversalFilters }): Promise<void>;
    [ActionTypes.STR_LOAD_REVERSIBLE_STOCK_TRANSFER]({ commit }: AugmentedActionContext, payload: { filters: StockTransferReversalFilters }): Promise<ReversibleInbound>;
    [ActionTypes.STR_ADD_REVERSAL]({ commit }: AugmentedActionContext, payload: { stfJobNo: string }): Promise<StockTransferReversal>;
    [ActionTypes.STR_COMPLETE]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<void>;
    [ActionTypes.STR_CANCEL]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<void>;
    [ActionTypes.STR_LOAD_ITEMS]({ commit }: AugmentedActionContext, payload: { filters: StockTransferReversalItemFilters }): Promise<Array<StockTransferReversalItem>>;
    [ActionTypes.STR_GET_DETAILS]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<StockTransferReversal>;
    [ActionTypes.STR_LOAD_REVERSIBLE_STOCK_TRANSFER_ITEMS]({ commit }: AugmentedActionContext, payload: { filters: ReversibleInboundItemFilters }): Promise<Array<ReversibleInboundItem>>;
    [ActionTypes.STR_ADD_STOCK_TRANSFER_REVERSAL_ITEMS]({ commit }: AugmentedActionContext, payload: { jobNo: string, pids: Array<string> }): Promise<void>;
    [ActionTypes.STR_DELETE_ITEM]({ commit }: AugmentedActionContext, payload: { jobNo: string, pid: string }): Promise<void>;
    [ActionTypes.STR_UPDATE]({ commit }: AugmentedActionContext, payload: { jobNo: string, refNo: string, reason: string }): Promise<void>;
    [ActionTypes.STR_REPORT]({ commit }: AugmentedActionContext, payload: { report: Report }): Promise<ArrayBuffer>;

    // Company Profile
    [ActionTypes.CP_LOAD_LIST]({ commit }: AugmentedActionContext): Promise<void>;
    [ActionTypes.CP_TOGGLE_COMPANY]({ commit }: AugmentedActionContext, payload: { code: string }): Promise<CompanyProfileShort>;
    [ActionTypes.CP_TOGGLE_ADDRESS_BOOK]({ commit }: AugmentedActionContext, payload: { code: string }): Promise<CompanyProfileAddressBookShort>;
    [ActionTypes.CP_TOGGLE_ADDRESS_CONTACT]({ commit }: AugmentedActionContext, payload: { companyCode: string, code: string }): Promise<CompanyProfileAddressContact>;
    [ActionTypes.CP_UPDATE_COMPANY]({ commit }: AugmentedActionContext, payload: { companyProfile: CompanyProfileShort }): Promise<CompanyProfileShort>;
    [ActionTypes.CP_UPDATE_ADDRESS_BOOK]({ commit }: AugmentedActionContext, payload: { addressBook: CompanyProfileAddressBookShort }): Promise<CompanyProfileAddressBookShort>;
    [ActionTypes.CP_UPDATE_ADDRESS_CONTACT]({ commit }: AugmentedActionContext, payload: { companyCode: string, addressContact: CompanyProfileAddressContact }): Promise<CompanyProfileAddressContact>;

    // Customers
    [ActionTypes.CS_LOAD_LIST]({ commit }: AugmentedActionContext, payload: { filters: CustomerFilters }): Promise<void>;
    [ActionTypes.CS_GET_PRODUCT_CODES]({ commit }: AugmentedActionContext): Promise<void>;
    [ActionTypes.CS_GET_CONTROL_CODES]({ commit }: AugmentedActionContext): Promise<void>;
    [ActionTypes.CS_GET_INVENTORY]({ commit }: AugmentedActionContext, payload: { customerCode: string }): Promise<CustomerInventory>;
    [ActionTypes.CS_UPDATE_INVENTORY_CONTROL]({ commit }: AugmentedActionContext, payload: { inventoryControl: CustomerInventory }): Promise<void>;
    [ActionTypes.CS_LOAD_UOM_LIST]({ commit }: AugmentedActionContext, payload: { filters: CustomerUomFilters }): Promise<Array<CustomerUom>>;
    [ActionTypes.CS_LOAD_CLIENTS]({ commit }: AugmentedActionContext, payload: { filters: CustomerClientFilters }): Promise<Array<CustomerClient>>;
    [ActionTypes.CS_GET_CUSTOMER]({ commit }: AugmentedActionContext, payload: { customerCode: string }): Promise<CustomerFull>;
    [ActionTypes.CS_UPDATE_CUSTOMER]({ commit }: AugmentedActionContext, payload: { customer: CustomerFull }): Promise<void>;
    [ActionTypes.CS_UPDATE_UOM]({ commit }: AugmentedActionContext, payload: { customerUom: CustomerUom }): Promise<CustomerUom>;
    [ActionTypes.CS_LOAD_GLOBAL_UOM]({ commit }: AugmentedActionContext): Promise<void>;
    [ActionTypes.CS_TOGGLE_UOM]({ commit }: AugmentedActionContext, payload: { customerCode: string, code: string }): Promise<CustomerUom>;
    [ActionTypes.CS_UPDATE_CUSTOMER_CLIENT]({ commit }: AugmentedActionContext, payload: { customerClient: CustomerClient }): Promise<CustomerFull>;
    [ActionTypes.CS_GET_CUSTOMER_CLIENT]({ commit }: AugmentedActionContext, payload: { customerClientCode: string }): Promise<CustomerClient>;
    [ActionTypes.CS_TOGGLE_CUSTOMER_CLIENT]({ commit }: AugmentedActionContext, payload: { customerClientCode: string }): Promise<CustomerClient>;

    // Stock Take
    [ActionTypes.STL_LOAD_LIST]({ commit }: AugmentedActionContext, payload: { filters: StockTakeFilters }): Promise<void>;
    [ActionTypes.STL_CANCEL_JOB]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<void>;
    [ActionTypes.STL_COMPLETE_JOB]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<void>;
    [ActionTypes.STL_LOAD_JOB]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<StockTake>;
    [ActionTypes.STL_LOAD_UPLOADED_LIST]({ commit }: AugmentedActionContext, payload: { jobNo: string, sorting: Sorting }): Promise<Array<StockTakeDetail>>;
    [ActionTypes.STL_LOAD_PID_ANOTHER_LOC_LIST]({ commit }: AugmentedActionContext, payload: { jobNo: string, sorting: Sorting }): Promise<Array<StockTakeDetail>>;
    [ActionTypes.STL_LOAD_PID_INVALID_LIST]({ commit }: AugmentedActionContext, payload: { jobNo: string, sorting: Sorting }): Promise<Array<StockTakeDetail>>;
    [ActionTypes.STL_LOAD_PID_MISSING_LIST]({ commit }: AugmentedActionContext, payload: { jobNo: string, sorting: Sorting }): Promise<Array<StockTakeDetail>>;
    [ActionTypes.STL_UPDATE_JOB]({ commit }: AugmentedActionContext, payload: { stockTake: StockTake }): Promise<void>;
    [ActionTypes.STL_LOAD_STANDBY_LOCATIONS]({ commit }: AugmentedActionContext, payload: { jobNo: string }): Promise<Array<StandbyLocation>>;
    [ActionTypes.STL_SEND_TO_STANDBY_NEGATIVE]({ commit }: AugmentedActionContext, payload: { jobNo: string, locationCode: string, whsCode: string }): Promise<void>;

    // Settings
    [ActionTypes.SETT_LOAD_AREA_LIST]({ commit }: AugmentedActionContext, payload: { filters: AreaFilters }): Promise<void>;
    [ActionTypes.SETT_LOAD_AREATYPE_LIST]({ commit }: AugmentedActionContext, payload: { filters: AreaTypeFilters }): Promise<void>;
    [ActionTypes.SETT_LOAD_CC_LIST]({ commit }: AugmentedActionContext, payload: { filters: ControlCodeFilters }): Promise<void>;
    [ActionTypes.SETT_LOAD_LOC_LIST]({ commit }: AugmentedActionContext, payload: { filters: LocationFilters }): Promise<void>;
    [ActionTypes.SETT_LOAD_PACKTYPE_LIST]({ commit }: AugmentedActionContext, payload: { filters: PackageTypeFilters }): Promise<void>;
    [ActionTypes.SETT_LOAD_PC_LIST]({ commit }: AugmentedActionContext, payload: { filters: ProductCodeFilters }): Promise<void>;
    [ActionTypes.SETT_LOAD_SATO_LIST]({ commit }: AugmentedActionContext, payload: { filters: SatoPrinterFilters }): Promise<void>;
    [ActionTypes.SETT_LOAD_UOM_LIST]({ commit }: AugmentedActionContext, payload: { filters: UomSettingsFilters }): Promise<void>;
    [ActionTypes.SETT_LOAD_WAREHOUSE_LIST]({ commit }: AugmentedActionContext, payload: { filters: WarehouseFilters }): Promise<void>;
    [ActionTypes.SETT_ADD_AREA]({ commit }: AugmentedActionContext, payload: { area: Area }): Promise<void>;
    [ActionTypes.SETT_ADD_AREATYPE]({ commit }: AugmentedActionContext, payload: { areaType: AreaType }): Promise<void>;
    [ActionTypes.SETT_ADD_CC]({ commit }: AugmentedActionContext, payload: { controlCode: ControlCode }): Promise<void>;
    [ActionTypes.SETT_ADD_LOCATION]({ commit }: AugmentedActionContext, payload: { location: LocationSettings }): Promise<void>;
    [ActionTypes.SETT_ADD_PACKAGETYPE]({ commit }: AugmentedActionContext, payload: { packageType: PackageType }): Promise<void>;
    [ActionTypes.SETT_ADD_PC]({ commit }: AugmentedActionContext, payload: { productCode: ProductCode }): Promise<void>;
    [ActionTypes.SETT_ADD_SATO]({ commit }: AugmentedActionContext, payload: { satoPrinter: SatoPrinter }): Promise<void>;
    [ActionTypes.SETT_ADD_UOM]({ commit }: AugmentedActionContext, payload: { uom: UomSettings }): Promise<void>;
    [ActionTypes.SETT_ADD_WAREHOUSE]({ commit }: AugmentedActionContext, payload: { warehouse: Warehouse }): Promise<void>;
    [ActionTypes.SETT_UPDATE_AREA]({ commit }: AugmentedActionContext, payload: { area: Area }): Promise<void>;
    [ActionTypes.SETT_UPDATE_AREATYPE]({ commit }: AugmentedActionContext, payload: { areaType: AreaType }): Promise<void>;
    [ActionTypes.SETT_UPDATE_CC]({ commit }: AugmentedActionContext, payload: { controlCode: ControlCode }): Promise<void>;
    [ActionTypes.SETT_UPDATE_LOCATION]({ commit }: AugmentedActionContext, payload: { location: LocationSettings }): Promise<void>;
    [ActionTypes.SETT_UPDATE_PACKAGETYPE]({ commit }: AugmentedActionContext, payload: { packageType: PackageType }): Promise<void>;
    [ActionTypes.SETT_UPDATE_PC]({ commit }: AugmentedActionContext, payload: { productCode: ProductCode }): Promise<void>;
    [ActionTypes.SETT_UPDATE_SATO]({ commit }: AugmentedActionContext, payload: { satoPrinter: SatoPrinter }): Promise<void>;
    [ActionTypes.SETT_UPDATE_UOM]({ commit }: AugmentedActionContext, payload: { uom: UomSettings }): Promise<void>;
    [ActionTypes.SETT_UPDATE_WAREHOUSE]({ commit }: AugmentedActionContext, payload: { warehouse: Warehouse }): Promise<void>;
    [ActionTypes.SETT_LOAD_WAREHOUSE]({ commit }: AugmentedActionContext, payload: { code: string }): Promise<Warehouse>;
    [ActionTypes.SETT_CB_LOAD_ACTIVE_WAREHOUSES]({ commit }: AugmentedActionContext): Promise<Array<SettingsComboBox>>;
    [ActionTypes.SETT_CB_LOAD_ACTIVE_AREAS]({ commit }: AugmentedActionContext, payload: { whsCode: string }): Promise<Array<SettingsComboBox>>;
    [ActionTypes.SETT_CB_LOAD_ACTIVE_AREA_TYPES]({ commit }: AugmentedActionContext): Promise<Array<SettingsComboBox>>;
    [ActionTypes.SETT_CB_LOAD_ILOG_LOC_CAT]({ commit }: AugmentedActionContext): Promise<Array<SettingsComboBox>>;
    [ActionTypes.SETT_LOAD_AREA]({ commit }: AugmentedActionContext, payload: { code: string, whsCode: string }): Promise<Area>;
    [ActionTypes.SETT_LOAD_LOCATION]({ commit }: AugmentedActionContext, payload: { code: string, whsCode: string }): Promise<LocationSettings>;
    [ActionTypes.SETT_LOAD_LOC_MORE]({ commit }: AugmentedActionContext, payload: { filters: LocationFilters }): Promise<void>;
    [ActionTypes.SETT_TOGGLE_ACTIVE_LOCATION]({ commit }: AugmentedActionContext, payload: { code: string, whsCode: string }): Promise<void>;
    [ActionTypes.SETT_PRINT_LOC_LABELS]({ commit }: AugmentedActionContext, payload: { locations: LocationLabel }): Promise<void>;
    [ActionTypes.SETT_GET_LOC_LABELS]({ commit }: AugmentedActionContext, payload: { locations: LocationLabel }): Promise<Array<LocationQrCode>>;
    [ActionTypes.SETT_DELETE_SATO]({ commit }: AugmentedActionContext, payload: { ip: string }): Promise<void>;
}

export const actions: ActionTree<State, State> & Actions = {
    // Inventory
    async [ActionTypes.INV_LOAD_LIST]({ commit }, { filters }) {
        var result = new PaginatedList<Inventory>();
        result.items = new Array<Inventory>();
        result.pagination = new PaginationData();
        if (!filters.customerCode) {
            commit(MutationTypes.SET_LIST_INV, result);
        } else {
            let data: PaginatedList<Inventory> = (await apiAxios.get('new-api/inventory', {
                params: { filter: filters }
            })).data;

            result.items = data.items;
            result.pagination = data.pagination;
            commit(MutationTypes.SET_LIST_INV, result);
        }
    },

    // Adjustment
    async [ActionTypes.INV_LOAD_ADJ_LIST]({ commit }, { filters }) {
        var result = new PaginatedList<Adjustment>();
        result.items = new Array<Adjustment>();
        result.pagination = new PaginationData();
        let data: PaginatedList<Adjustment> = (await apiAxios.get('new-api/adjustment', {
            params: { filter: filters }
        })).data;

        result.items = data.items;
        result.pagination = data.pagination;
        commit(MutationTypes.SET_LIST_ADJ, result);
    },
    async [ActionTypes.INV_LOAD_ADJ_ITEMS]({ commit }, { jobNo, filters }) {
        let items: Array<AdjustmentItem> = (await apiAxios.get('new-api/adjustmentItems', {
            params: { jobNo: jobNo, filters }
        })).data;
        commit(MutationTypes.SET_ADJ_ITEMS, { jobNo, items });
        return items;
    },
    async [ActionTypes.INV_ADD_ADJ]({ commit }, { customerCode, isUndoZeroOut }) {
        let newAdjustment: Adjustment = (await apiAxios.get('new-api/adjustment/add', {
            params: { customerCode: customerCode, isUndoZeroOut: isUndoZeroOut }
        })).data;
        newAdjustment.items = new Array<AdjustmentItem>();
        commit(MutationTypes.SET_NEW_ADJ, newAdjustment);
        return newAdjustment;
    },
    async [ActionTypes.INV_UPDATE_ADJ]({ commit }, { jobNo, referenceNo, reason }) {
        let updatedItem: Adjustment = await (await apiAxios.post('new-api/adjustment/update', { jobNo: jobNo, referenceNo: referenceNo, reason: reason })).data;
        commit(MutationTypes.UPDATE_ADJ, updatedItem);
        return updatedItem;
    },
    async [ActionTypes.INV_CANCEL_ADJ]({ commit }, { jobNo }) {
        let updatedItem: Adjustment = await (await apiAxios.post('new-api/adjustment/cancel?jobNo=' + jobNo, null)).data;
        commit(MutationTypes.CANCEL_ADJ_ITEM, updatedItem);
    },
    async [ActionTypes.INV_COMPLETE_ADJ]({ commit }, { jobNo }) {
        let updatedItem: Adjustment = await (await apiAxios.post('new-api/adjustment/complete?jobNo=' + jobNo, null)).data;
        commit(MutationTypes.UPDATE_ADJ, updatedItem);
    },
    async [ActionTypes.INV_ADJ_ITEM_PREPARE_NEW]({ commit }, { pid, jobNo }) {
        let adjustmentDetail: AdjustmentItemAddMod = await (await apiAxios.get('new-api/adjustmentItems/prepareNew', { params: { pid: pid, jobNo: jobNo } })).data;
        return adjustmentDetail;
    },
    async [ActionTypes.INV_ADJ_ITEM_GET]({ commit }, { jobNo, lineItem }) {
        let adjustmentItem: AdjustmentItemAddMod = (await apiAxios.get('new-api/adjustmentItems/details', { params: { jobNo: jobNo, lineItem: lineItem } })).data;
        return adjustmentItem;
    },
    async [ActionTypes.INV_ADJ_ITEM_SAVE]({ commit }, { adjItem }) {
        let result = await (await apiAxios.post('new-api/adjustmentItems', adjItem)).data;
        let addNew = adjItem.lineItem == 0 ? true : false;
        commit(MutationTypes.ADD_ADJ_ITEM, { adjItem, addNew });
    },
    async [ActionTypes.INV_DEL_ITEM]({ commit }, { jobNo, lineItem }) {
        let result = await (await apiAxios.delete('new-api/AdjustmentItems?jobNo=' + jobNo + '&lineItem=' + lineItem)).data;
        commit(MutationTypes.DEL_ADJ_ITEM, { jobNo, lineItem });
    },
    async [ActionTypes.INV_ADJ_GET_DETAILS]({ commit }, { jobNo }) {
        return (await apiAxios.get('new-api/adjustment/details', { params: { jobNo } })).data;
    },

    // Quarantine
    async [ActionTypes.INV_QNE_LOAD_LIST]({ commit }, { filters }) {
        var result = new PaginatedList<Quarantine>();
        result.items = new Array<Quarantine>();
        result.pagination = new PaginationData();

        let data: PaginatedList<Quarantine> = (await apiAxios.get('new-api/quarantine', { params: { filter: filters } })).data;

        result.items = data.items;
        result.pagination = data.pagination;
        commit(MutationTypes.INV_QNE_SET_LIST, result);
    },
    async [ActionTypes.INV_QNE_REASON]({ commit }, { pids, reason }) {
        let updateReason = (await apiAxios.post('new-api/quarantine/updateReason', { pids, reason })).data;
        commit(MutationTypes.INV_QNE_UPDATE_REASON, { pids, reason });
    },
    async [ActionTypes.REPORT_QUARANTINE]({ commit }, { filter }) {
        let reportFile: ArrayBuffer = (await apiAxios.get(('report/quarantine/' + (filter.reportByProductCode ? 'quarantine' : 'quarantinebypid')),
            {
                params: { customerCode: filter.customerCode, productCode: filter.productCode, supplierID: filter.supplierID },
                responseType: 'arraybuffer'
            })).data;
        return reportFile;
    },

    // Relocation
    async [ActionTypes.INV_REL_LOAD_LIST]({ commit }, { filters }) {
        var result = new PaginatedList<Relocation>();
        result.items = new Array<Relocation>();
        result.pagination = new PaginationData();

        let data: PaginatedList<Relocation> = await (await apiAxios.get('new-api/relocationLog', { params: { filter: filters } })).data;

        result.items = data.items;
        result.pagination = data.pagination;
        commit(MutationTypes.INV_REL_SET_LIST, result);
    },

    // Decant
    async [ActionTypes.INV_DEC_LOAD_LIST]({ commit }, { filters }) {
        var result = new PaginatedList<Decant>();
        result.items = new Array<Decant>();
        result.pagination = new PaginationData();

        let data: PaginatedList<Decant> = await (await apiAxios.get('new-api/decant', { params: { filter: filters } })).data;

        data.items.forEach((dec) => { result.items.push(dec) });
        result.pagination = data.pagination;
        commit(MutationTypes.INV_DEC_SET_LIST, result);
    },
    async [ActionTypes.INV_DEC_ADD]({ commit }, { customerCode }) {
        let newDecant: Decant = (await apiAxios.get('new-api/decant/add', { params: { customerCode: customerCode } })).data;
        commit(MutationTypes.INV_DEC_SET_DECANT, newDecant);
        return newDecant.jobNo;
    },
    async [ActionTypes.INV_DEC_UPDATE]({ commit }, { jobNo, referenceNo, remark }) {
        let updatedDecant: Decant = (await apiAxios.post('new-api/decant/update?jobNo=' + jobNo + '&referenceNo=' + referenceNo + '&remark=' + remark, {})).data;
        commit(MutationTypes.INV_DEC_UPDATE, fromJSON(Decant, updatedDecant));
    },
    async [ActionTypes.INV_DEC_CANCEL]({ commit }, { jobNo }) {
        let updatedItem: Decant = await (await apiAxios.post('new-api/decant/cancel?jobNo=' + jobNo, null)).data;
        commit(MutationTypes.INV_DEC_UPDATE, updatedItem);
    },
    async [ActionTypes.INV_DEC_COMPLETE]({ commit }, { jobNo }) {
        let updatedItem: Decant = await (await apiAxios.post('new-api/decant/complete?jobNo=' + jobNo, null)).data;
        commit(MutationTypes.INV_DEC_UPDATE, updatedItem);
    },
    async [ActionTypes.INV_DEC_DETAILS]({ commit }, { jobNo }) {
        let decant: Decant = await (await apiAxios.get('new-api/decant/details?jobNo=' + jobNo)).data;
        commit(MutationTypes.INV_DEC_UPDATE, fromJSON(Decant, decant));
    },
    async [ActionTypes.INV_DEC_ITEM_PREPARE_NEW]({ commit }, { jobNo, pid }) {
        let pallet: Pallet = (await apiAxios.get('new-api/decant/pallet', { params: { jobNo, pid } })).data;
        return pallet;
    },
    async [ActionTypes.INV_DEC_ADD_ITEMS]({ commit }, { jobNo, pid, newQuantities }) {
        let updatedDecant: Decant = (await apiAxios.post('new-api/decant/addItem', { jobNo, pid, newQuantities })).data;
        commit(MutationTypes.INV_DEC_UPDATE, fromJSON(Decant, updatedDecant));
    },
    async [ActionTypes.INV_DEC_DEL_ITEMS]({ commit }, { jobNo, pid }) {
        let updatedDecant: Decant = (await apiAxios.delete('new-api/decant/deleteItem', { params: { jobNo, pid } })).data;
        commit(MutationTypes.INV_DEC_UPDATE, fromJSON(Decant, updatedDecant));
    },
    async [ActionTypes.INV_DEC_REPORT]({ commit }, { decantDetails }) {
        let report = (await apiAxios.post('report/inventory/decantdetail?displayRowNo=true', decantDetails, { responseType: 'arraybuffer' })).data;
        return report;
    },

    // Storage Detail
    async [ActionTypes.INV_SD_LOAD_LIST]({ commit }, { filters }) {
        var result = new PaginatedList<StorageDetail>();
        result.items = new Array<StorageDetail>();
        result.pagination = new PaginationData();

        let data: PaginatedList<StorageDetail> = (await apiAxios.get('new-api/storageDetail', {
            params: { filter: filters }
        })).data;

        result.items = data.items;
        result.pagination = data.pagination;
        result.totalQty = data.totalQty;
        commit(MutationTypes.SET_LIST_SD, result);
    },

    // Common
    async [ActionTypes.GET_SUPPLIERS]({ commit }, { factoryId }) {
        let suppliers: Array<SupplierRegular> = (await apiAxios.get('supplierMasters/getList', { params: { factoryId: factoryId } })).data;
        return suppliers;
    },
    async [ActionTypes.GET_PRODUCTS]({ commit }, { customerCode, supplierID }) {
        let products: Array<ProductFull> = (await apiAxios.get('partMasters/getPartMasterListBySupplier', { params: { customerCode: customerCode, supplierID: supplierID } })).data;
        return products;
    },
    async [ActionTypes.GET_PRODUCTS_BY_CUSTOMER]({ commit }, { customerCode }) {
        let products: Array<ProductFull> = ((await apiAxios.get('partMasters/getPartMasterList', { params: { customerCode: customerCode, pageSize: 9999 } })).data).data;
        return products;
    },
    async [ActionTypes.LOAD_DICT]({ commit }) {
        await this.INV_LOAD_CUSTOMERS
        return new Promise((resolve) => {
            setTimeout(() => {
                commit(MutationTypes.SET_DICT, {
                    ownership: { 0: 'Supplier', 1: 'EHP' },
                    storageDetailStatus: {
                        0: 'Incoming', 1: 'Putaway', 2: 'Allocated', 3: 'Picked', 4: 'Packed', 5: 'InTransit', 6: 'Dispatched', 7: 'Splitted', 9: 'Quarantine',
                        //Temporary status before Allocated
                        10: 'Allocating', 11: 'Kitting', 12: 'Splitting', 13: 'Restricted',
                        //Status 21 - 30 is reserved for item locked by warehouse operation
                        21: 'Transferring', 22: 'Decant', 23: 'Discrepancy',
                        96: 'DiscrepancyFixed', 97: 'Reversal', 98: 'ZeroOut', 99: 'Cancelled'
                    },
                    adjustmentStatus: { 0: 'New', 1: 'Processing', 2: 'Completed', 3: 'Outstanding', 10: 'Cancelled', 11: 'All' },
                    adjustmentJobType: { 0: 'Normal', 1: 'Undo Zero Out' },
                    decantStatus: { 0: 'New', 1: 'Processing', 7: 'Outstanding', 8: 'Completed', 9: 'Cancelled', 10: 'All' },
                    scannerType: { 0: 'Batch Scanner', 1: 'RF Scanner' },
                    bondedStatus: { true: 'Bonded', false: 'Non-Bonded' },
                    inboundReversalStatus: { 0: 'New', 1: 'Processing', 2: 'Outstanding', 8: 'Completed', 9: 'Cancelled', 10: 'All' },
                    stockTransferReversalStatus: { 0: 'New', 1: 'Processing', 2: 'Outstanding', 8: 'Completed', 9: 'Cancelled', 10: 'All' },
                    customerStatus: { 0: 'Inactive', 1: 'Active', 3: 'All' },
                    customerUomStatus: { 0: 'Inactive', 1: 'Active', 3: 'All' },
                    customerClientStatus: { 0: 'Inactive', 1: 'Active', 3: 'All' },
                    stockTakeStatus: { 0: 'New Job', 1: 'Processing', 2: 'Outstanding', 8: 'Completed', 9: 'Cancelled', 10: 'All' },
                    areaStatus: { 0: 'Inactive', 1: 'Active', 3: 'All' },
                    areaTypeStatus: { 0: 'Inactive', 1: 'Active', 3: 'All' },
                    areaTypeType: { 0: 'System Defined', 1: 'User Defined', 3: 'All' },
                    locationStatus: { 0: 'Inactive', 1: 'Active', 3: 'All' },
                    locationType: { 0: 'Normal', 1: 'Quarantine', 2: 'Cross Dock', 3: 'Stand By', 4: 'Ext System', 10: 'All'},
                    locationPriority: { 0: 'Non Priority', 1: 'Priority', 3: 'All' },
                    controlCodeStatus: { 0: 'Inactive', 1: 'Active', 3: 'All' },
                    controlCodeType: { 0: 'System Defined', 1: 'User Defined', 3: 'All' },
                    productCodeStatus: { 0: 'Inactive', 1: 'Active', 3: 'All' },
                    productCodeType: { 0: 'System Defined', 1: 'User Defined', 3: 'All' },
                    packageTypeStatus: { 0: 'Inactive', 1: 'Active', 3: 'All' },
                    packageTypeType: { 0: 'System Defined', 1: 'User Defined', 3: 'All' },
                    uomStatus: { 0: 'Inactive', 1: 'Active', 3: 'All' },
                    uomType: { 0: 'System Defined', 1: 'User Defined', 3: 'All' },
                    warehouseStatus: { 0: 'Inactive', 1: 'Active', 3: 'All' },
                    warehouseType: { 0: 'System Defined', 1: 'User Defined', 3: 'All' },
                    satoPrinterType: { 0: 'CL412', 1: 'CL4NX', 3: 'All' }
                }); resolve()
            }, 250)
        })
    },
    async [ActionTypes.INV_LOAD_CUSTOMERS]({ commit }) {
        const customers = (await apiAxios.get('customers')).data;
        commit(MutationTypes.SET_CUSTOMERS, customers);
    },
    async [ActionTypes.LOAD_LOCATIONS]({ commit }, { whsCode }) {
        const locations = (await apiAxios.get('warehouses/locations?whsCode=' + whsCode)).data;
        commit(MutationTypes.SET_LOCATIONS, locations);
    },
    async [ActionTypes.INV_REPORT]({ commit }, { filter, report }) {
        let reportFile: ArrayBuffer = (await apiAxios.get(report.alias,
            {
                params: {
                    customerCode: filter?.customerCode, productCode: filter?.productCode, supplierID: filter?.supplierID,
                    dateStart: filter?.dateStart, dateEnd: filter?.dateEnd, bondedStatus: filter?.bondedStatus, interval: filter?.interval,
                    date: filter?.date, isAllProductsSelected: filter?.isAllProductsSelected,
                    isQuantitySelected: filter?.isQuantitySelected, locationCode: filter?.locationCode ?? 'All Location'
                },
                responseType: 'arraybuffer'
            })).data;
        return reportFile;
    },

    // Invoicing
    async [ActionTypes.INVC_LOAD_FACTORIES]({ commit }) {
        const res = (await apiAxios.get<Array<Customer>>('new-api/InvoiceRequest/factories')).data;
        commit(MutationTypes.SET_FACTORIES, res);
    },

    async [ActionTypes.INVC_LOAD_LIST]({ commit }, { filters }) {
        const pagination = {
            pageNumber: 1,
            itemsPerPage: 20
        }
        const res = (await apiAxios.get<PaginatedList<InvoiceBatch>>('new-api/InvoiceRequest/batches', { params: { ...filters, pagination } }));
        const flow = (await apiAxios.get<"Standard" | "CustomsClearance" | "None">('new-api/InvoiceRequest/flow')).data;
        if (res && res.status == 200) {
            commit(MutationTypes.SET_LIST_INVC, { list: res.data, flow });
        } else {
            throw res;
        }
    },
    async [ActionTypes.INVC_LOAD_MORE]({ commit, state }, { filters }) {
        const pagination = {
            pageNumber: state.invoiceRequestList.pagination.pageNumber + 1,
            itemsPerPage: state.invoiceRequestList.pagination.itemsPerPage
        }
        const res = (await apiAxios.get<PaginatedList<InvoiceBatch>>('new-api/InvoiceRequest/batches', { params: { ...filters, pagination } }));
        if (res && res.status == 200) {
            commit(MutationTypes.SET_LIST_INVC, {
                list: {
                    items: state.invoiceRequestList.items.concat(res.data.items),
                    pagination: res.data.pagination,
                    totalQty: res.data.totalQty
                },
                flow: state.invoiceRequestFlow
            });
        } else {
            throw res;
        }
    },
    async [ActionTypes.INVC_UPDATE_BATCH]({ commit, state }, { batchId, comment, hour }) {
        const res = await apiAxios.post(`new-api/InvoiceRequest/saveDataForCustomsAgency/${batchId}`, { comment, hour });
        if (!res || res.status != 200) {
            throw res;
        }
    },
    async [ActionTypes.CONFIRM_INVC]({ commit }, { batchId }) {
        const res = await apiAxios.post('new-api/InvoiceRequest/approve/' + batchId);
        if (!res || res.status != 200) {
            throw res;
        }
    },
    async [ActionTypes.REJECT_INVC]({ commit }, { batchId }) {
        const res = await apiAxios.post('new-api/InvoiceRequest/reject/' + batchId);
        if (!res || res.status != 200) {
            throw res;
        }
    },
    async [ActionTypes.INVC_GET_INVOICE_PDF]({ commit }, { invoiceId }) {
        const res = await apiAxios.get<Blob>('new-api/InvoiceRequest/file/' + invoiceId, {
            params: {}, responseType: 'blob', headers: {
                Accept: 'application/*',
            }
        });
        if (res && res.status == 200) {
            return res;
        } else {
            throw res;
        }
    },
    async [ActionTypes.INV_RELOAD_ILOG_ENABLED]({ commit }) {
        const ilogEnabled = (await apiAxios.get<boolean>('new-api/ILogIntegration/isActiveForWarehouse')).data;
        commit(MutationTypes.SET_ILOG_ENABLED, ilogEnabled);
    },
    async [ActionTypes.INV_RELOAD_ILOG_PIDS]({ commit }) {
        const pidsRequestedFromILog = (await apiAxios.get<Array<{ pid: string }>>('new-api/PalletTransferRequest/ongoing')).data.map(x => x.pid);
        commit(MutationTypes.SET_PIDS_REQUESTED_FROM_ILOG, pidsRequestedFromILog);
    },
    async [ActionTypes.REQUEST_FROM_ILOG](_, { pids }) {
        await apiAxios.post('new-api/PalletTransferRequest/add', { pids })
    },

    // Inbound Reversal
    async [ActionTypes.IR_LOAD_LIST]({ commit }, { filters }) {
        var result = new PaginatedList<InboundReversal>();
        result.items = new Array<InboundReversal>();
        result.pagination = new PaginationData();
   
        let data: PaginatedList<InboundReversal> = (await apiAxios.get('new-api/inboundReversal', { params: { parameters: filters } })).data;
        result.items = data.items;
        result.pagination = data.pagination;
        commit(MutationTypes.SET_LIST_IR, result);
    },
    async [ActionTypes.IR_LOAD_REVERSIBLE_INBOUND]({ commit }, { filters }) {
        const result = (await apiAxios.get('new-api/inboundReversal/getReversibleInbounds', { params: { filter: filters } })).data;
        if (result) {
            return result.items[0];
        } else {
            throw result.items;
        }
    },
    async [ActionTypes.IR_ADD_REVERSAL]({ commit }, { inJobNo }) {
        let newReversal: InboundReversal = (await apiAxios.post('new-api/inboundReversal/add?inJobNo=' + inJobNo, null)).data;
        newReversal.items = new Array<InboundReversalItem>();
        commit(MutationTypes.SET_NEW_IR, newReversal);
        return newReversal;
    },
    async [ActionTypes.IR_COMPLETE]({ commit }, { jobNo }) {
        let updatedItem: InboundReversal = await (await apiAxios.post('new-api/inboundReversal/complete?jobNo=' + jobNo, null)).data;
        commit(MutationTypes.COMPLETE_OR_CANCEL_IR, updatedItem);
    },
    async [ActionTypes.IR_CANCEL]({ commit }, { jobNo }) {
        let updatedItem: InboundReversal = await (await apiAxios.post('new-api/inboundReversal/cancel?jobNo=' + jobNo, null)).data;
        commit(MutationTypes.COMPLETE_OR_CANCEL_IR, updatedItem);
    },
    async [ActionTypes.IR_LOAD_ITEMS]({ commit }, { filters }) {
        let items: Array<InboundReversalItem> = (await apiAxios.get('new-api/inboundReversalItems/getInboundReversalItems', { params: { parameters: filters } })).data;
        const jobNo = filters.jobNo;
        commit(MutationTypes.SET_IR_ITEMS, { jobNo, items });
        return items;
    },
    async [ActionTypes.IR_GET_DETAILS]({ commit }, { jobNo }) {
        return (await apiAxios.get('new-api/inboundReversal/details', { params: { jobNo } })).data;
    },
    async [ActionTypes.IR_LOAD_REVERSIBLE_INBOUND_ITEMS]({ commit }, { filters }) {
        return (await apiAxios.get('new-api/inboundReversalItems/getReversibleInboundItems', { params: { parameters: filters } })).data;
    },
    async [ActionTypes.IR_ADD_INBOUND_REVERSAL_ITEMS]({ commit }, { jobNo, pids }) {
        let updatedIR: InboundReversalItem = (await apiAxios.post('new-api/inboundReversalItems/add', { jobNo, pids })).data;
        const status = 1;
        commit(MutationTypes.UPDATE_IR_STATUS, { jobNo, status });
    },
    async [ActionTypes.IR_DELETE_ITEM]({ commit }, { jobNo, pid }) {
        return (await apiAxios.delete('new-api/inboundReversalItems/delete', { params: { jobNo, pid } })).data;
    },
    async [ActionTypes.IR_UPDATE]({ commit }, { jobNo, refNo, reason }) {
        let updatedIR: InboundReversal = (await apiAxios.post('new-api/inboundReversal/update?jobNo='+jobNo+'&refNo='+refNo+'&reason='+reason, null)).data;
        commit(MutationTypes.UPDATE_IR, { updatedIR });
    },

    // Stock Transfer Reversal
    async [ActionTypes.STR_LOAD_LIST]({ commit }, { filters }) {
        var result = new PaginatedList<StockTransferReversal>();
        result.items = new Array<StockTransferReversal>();
        result.pagination = new PaginationData();

        let data: PaginatedList<StockTransferReversal> = (await apiAxios.get('new-api/stockTransferReversal', { params: { parameters: filters } })).data;
        result.items = data.items;
        result.pagination = data.pagination;
        commit(MutationTypes.SET_LIST_STR, result);
    },
    async [ActionTypes.STR_LOAD_REVERSIBLE_STOCK_TRANSFER]({ commit }, { filters }) {
        const result = (await apiAxios.get('new-api/stockTransferReversal/getReversibleStockTransfers', { params: { filter: filters } })).data;
        if (result) {
            return result.items[0];
        } else {
            throw result.items;
        }
    },
    async [ActionTypes.STR_ADD_REVERSAL]({ commit }, { stfJobNo }) {
        let newReversal: StockTransferReversal = (await apiAxios.post('new-api/stockTransferReversal/add?stfJobNo=' + stfJobNo, null)).data;
        newReversal.items = new Array<StockTransferReversalItem>();
        commit(MutationTypes.SET_NEW_STR, newReversal);
        return newReversal;
    },
    async [ActionTypes.STR_COMPLETE]({ commit }, { jobNo }) {
        let updatedItem: StockTransferReversal = await (await apiAxios.post('new-api/stockTransferReversal/complete?jobNo=' + jobNo, null)).data;
        commit(MutationTypes.COMPLETE_OR_CANCEL_STR, updatedItem);
    },
    async [ActionTypes.STR_CANCEL]({ commit }, { jobNo }) {
        let updatedItem: StockTransferReversal = await (await apiAxios.post('new-api/stockTransferReversal/cancel?jobNo=' + jobNo, null)).data;
        commit(MutationTypes.COMPLETE_OR_CANCEL_STR, updatedItem);
    },
    async [ActionTypes.STR_LOAD_ITEMS]({ commit }, { filters }) {
        let items: Array<StockTransferReversalItem> = (await apiAxios.get('new-api/stockTransferReversalItems/getStockTransferReversalItems', { params: { parameters: filters } })).data;
        const jobNo = filters.jobNo;
        commit(MutationTypes.SET_STR_ITEMS, { jobNo, items });
        return items;
    },
    async [ActionTypes.STR_GET_DETAILS]({ commit }, { jobNo }) {
        return (await apiAxios.get('new-api/stockTransferReversal/details', { params: { jobNo } })).data;
    },
    async [ActionTypes.STR_LOAD_REVERSIBLE_STOCK_TRANSFER_ITEMS]({ commit }, { filters }) {
        return (await apiAxios.get('new-api/stockTransferReversalItems/getReversibleStockTransferItems', { params: { parameters: filters } })).data;
    },
    async [ActionTypes.STR_ADD_STOCK_TRANSFER_REVERSAL_ITEMS]({ commit }, { jobNo, pids }) {
        let updatedSTR: StockTransferReversalItem = (await apiAxios.post('new-api/stockTransferReversalItems/add', { jobNo, pids })).data;
        const status = 1;
        commit(MutationTypes.UPDATE_STR_STATUS, { jobNo, status });
    },
    async [ActionTypes.STR_DELETE_ITEM]({ commit }, { jobNo, pid }) {
        return (await apiAxios.delete('new-api/stockTransferReversalItems/delete', { params: { jobNo, pid } })).data;
    },
    async [ActionTypes.STR_UPDATE]({ commit }, { jobNo, refNo, reason }) {
        let updatedSTR: StockTransferReversal = (await apiAxios.post('new-api/stockTransferReversal/update?jobNo=' + jobNo + '&refNo=' + refNo + '&reason=' + reason, null)).data;
        commit(MutationTypes.UPDATE_STR, { updatedSTR });
    },
    async [ActionTypes.STR_REPORT]({ commit }, { report }) {
        let reportFile: ArrayBuffer = (await apiAxios.get(report.alias,
            {
                params: { jobNo: report.props.jobNo, whsCode: report.props.whsCode },
                responseType: 'arraybuffer'
            })).data;
        return reportFile;
    },

    // Company Profile
    async [ActionTypes.CP_LOAD_LIST]({ commit }) {
        let companyProfiles: Array<CompanyProfile> = (await apiAxios.get('new-api/companyProfile/getAddressTree')).data.companyProfiles;
        commit(MutationTypes.SET_LIST_CP, { companyProfiles });
    },
    async [ActionTypes.CP_TOGGLE_COMPANY]({ commit }, { code }) {
        let updatedCompany = (await apiAxios.post('new-api/companyProfile/toggleActiveCompanyProfile?code=' + code)).data
        commit(MutationTypes.UPDATE_COMPANY_PROFILE, { updatedCompany });
        return updatedCompany;
    },
    async [ActionTypes.CP_TOGGLE_ADDRESS_BOOK]({ commit }, { code }) {
        let updatedAddressBook = (await apiAxios.post('new-api/companyProfile/toggleActiveAddressBook?code=' + code)).data
        commit(MutationTypes.UPDATE_ADDRESS_BOOK, { updatedAddressBook });
        return updatedAddressBook;
    },
    async [ActionTypes.CP_TOGGLE_ADDRESS_CONTACT]({ commit }, { companyCode, code }) {
        let updatedAddressContact = (await apiAxios.post('new-api/companyProfile/toggleActiveAddressContact?code=' + code)).data
        commit(MutationTypes.UPDATE_ADDRESS_CONTACT, { companyCode, updatedAddressContact });
        return updatedAddressContact;
    },
    async [ActionTypes.CP_UPDATE_COMPANY]({ commit }, { companyProfile }) {
        let updatedCompany = (await apiAxios.post('new-api/companyProfile/update', { code: companyProfile.code, name: companyProfile.name })).data
        commit(MutationTypes.UPDATE_COMPANY_PROFILE, { updatedCompany });
        return updatedCompany;
    },
    async [ActionTypes.CP_UPDATE_ADDRESS_BOOK]({ commit }, { addressBook }) {
        const addUpdateUrl = !addressBook.code ? 'addAddressBook' : 'updateAddressBook';
        let updatedAddressBook = (await apiAxios.post(('new-api/companyProfile/' + addUpdateUrl), addressBook)).data;
        commit(MutationTypes.UPDATE_ADDRESS_BOOK, { updatedAddressBook });
        return updatedAddressBook;
    },
    async [ActionTypes.CP_UPDATE_ADDRESS_CONTACT]({ commit }, { companyCode, addressContact }) {
        const addUpdateUrl = !addressContact.code ? 'addAddressContact' : 'updateAddressContact';
        let updatedAddressContact = (await apiAxios.post(('new-api/companyProfile/' + addUpdateUrl), addressContact)).data
        commit(MutationTypes.UPDATE_ADDRESS_CONTACT, { companyCode, updatedAddressContact });
        return updatedAddressContact;
    },

    // Customers
    async [ActionTypes.CS_LOAD_LIST]({ commit }, { filters }) {
        let customers: Array<CustomerFull> = (await apiAxios.get('new-api/customer/getCustomerList', { params: filters})).data;
        commit(MutationTypes.SET_LIST_CS, { customers });
    },
    async [ActionTypes.CS_GET_PRODUCT_CODES]({ commit }) {
        let productCodes = (await apiAxios.get('new-api/customer/getActiveProductCodes')).data;
        commit(MutationTypes.SET_PRODUCT_CODES, { productCodes });
    },
    async [ActionTypes.CS_GET_CONTROL_CODES]({ commit }) {
        let controlCodes = (await apiAxios.get('new-api/customer/getActiveControlCodes')).data;
        commit(MutationTypes.SET_CONTROL_CODES, { controlCodes });
    },
    async [ActionTypes.CS_GET_INVENTORY]({ commit }, { customerCode }) {
        let csInventory = (await apiAxios.get('new-api/customer/getInventoryControl', { params: { customerCode } })).data;
        return csInventory;
    },
    async [ActionTypes.CS_UPDATE_INVENTORY_CONTROL]({ commit }, { inventoryControl }) {
        let updatedInventory = (await apiAxios.post('new-api/customer/updateInventoryControl', inventoryControl)).data;
        return;
    },
    async [ActionTypes.CS_LOAD_UOM_LIST]({ commit }, { filters }) {
        let uomList: Array<CustomerUom> = (await apiAxios.get('new-api/customer/getUomDecimalList', { params: { parameters: filters } })).data;
        return uomList;
    },
    async [ActionTypes.CS_LOAD_CLIENTS]({ commit }, { filters }) {
        let clientList: Array<CustomerClient> = (await apiAxios.get('new-api/customer/getCustomerClientList', { params: { parameters: filters } })).data;
        return clientList;
    },
    async [ActionTypes.CS_GET_CUSTOMER]({ commit }, { customerCode }) {
        let customer: CustomerFull = (await apiAxios.get('new-api/customer/getCustomer', { params: { code: customerCode } })).data;
        return customer;
    },
    async [ActionTypes.CS_UPDATE_CUSTOMER]({ commit }, { customer }) {
        let updatedCustomer: CustomerFull = (await apiAxios.post('new-api/customer/update', customer)).data;
        commit(MutationTypes.UPDATE_CUSTOMER, { updatedCustomer });
    },
    async [ActionTypes.CS_UPDATE_UOM]({ commit }, { customerUom }) {
        let updatedCustomerUom: CustomerUom = (await apiAxios.post('new-api/customer/updateUomDecimal', customerUom)).data;
        return updatedCustomerUom;
    },
    async [ActionTypes.CS_LOAD_GLOBAL_UOM]({ commit }) {
        let globalUomList: Array<GlobalUom> = (await apiAxios.get('new-api/customer/getActiveUomList')).data;
        commit(MutationTypes.SET_GLOBAL_UOM, { globalUomList })
    },
    async [ActionTypes.CS_TOGGLE_UOM]({ commit }, { customerCode, code }) {
        let updatedUom: CustomerUom = (await apiAxios.post('new-api/customer/toggleActiveUomDecimal?customerCode='+customerCode+'&code='+code)).data;
        return updatedUom;
    },
    async [ActionTypes.CS_UPDATE_CUSTOMER_CLIENT]({ commit }, { customerClient }) {
        let updatedCustomerClient: CustomerFull = (await apiAxios.post('new-api/customer/updateCustomerClient', customerClient)).data;
        return updatedCustomerClient;
    },
    async [ActionTypes.CS_GET_CUSTOMER_CLIENT]({ commit }, { customerClientCode }) {
        let customer: CustomerClient = (await apiAxios.get('new-api/customer/getCustomerClient', { params: { code: customerClientCode } })).data;
        return customer;
    },
    async [ActionTypes.CS_TOGGLE_CUSTOMER_CLIENT]({ commit }, { customerClientCode }) {
        let updatedCustomerClient: CustomerClient = (await apiAxios.post('new-api/customer/toggleActiveCustomerClient?code='+customerClientCode)).data;
        return updatedCustomerClient;
    },

    // Stock Take
    async [ActionTypes.STL_LOAD_LIST]({ commit }, { filters }) {
        let result = new PaginatedList<StockTake>();
        result.items = new Array<StockTake>();
        result.pagination = new PaginationData();

        let data: PaginatedList<StockTake> = (await apiAxios.get('new-api/StockTake', { params: { parameters: filters } })).data;
        result.items = data.items;
        result.pagination = data.pagination;
        commit(MutationTypes.SET_LIST_STL, result);
    },
    async [ActionTypes.STL_CANCEL_JOB]({ commit }, { jobNo }) {
        let updatedStockTake: StockTake = (await apiAxios.post('new-api/StockTake/cancelStockTake?jobNo=' + jobNo, null)).data;
        commit(MutationTypes.UPDATE_STL, { updatedStockTake });
    },
    async [ActionTypes.STL_COMPLETE_JOB]({ commit }, { jobNo }) {
        let updatedStockTake: StockTake = (await apiAxios.post('new-api/StockTake/completeStockTake?jobNo=' + jobNo, null)).data;
        commit(MutationTypes.UPDATE_STL, { updatedStockTake });
    },
    async [ActionTypes.STL_LOAD_JOB]({ commit }, { jobNo }) {
        let stockTakeJob = (await apiAxios.get('new-api/StockTake/getStockTake?jobNo=' + jobNo)).data;
        return stockTakeJob;
    },
    async [ActionTypes.STL_LOAD_UPLOADED_LIST]({ commit }, { jobNo, sorting }) {
        let uploadedList = (await apiAxios.get('new-api/StockTake/getUploadedList', { params: { jobNo: jobNo, sorting: sorting } })).data;
        return uploadedList;
    },
    async [ActionTypes.STL_LOAD_PID_ANOTHER_LOC_LIST]({ commit }, { jobNo, sorting }) {
        let anotherLocList = (await apiAxios.get('new-api/StockTake/getAnotherLocPid', { params: { jobNo: jobNo, sorting: sorting } })).data;
        return anotherLocList;
    },
    async [ActionTypes.STL_LOAD_PID_INVALID_LIST]({ commit }, { jobNo, sorting }) {
        let invalidPid = (await apiAxios.get('new-api/StockTake/getInvalidPid', { params: { jobNo: jobNo, sorting: sorting } })).data;
        return invalidPid;
    },
    async [ActionTypes.STL_LOAD_PID_MISSING_LIST]({ commit }, { jobNo, sorting }) {
        let missingPid = (await apiAxios.get('new-api/StockTake/getMissingPid', { params: { jobNo: jobNo, sorting: sorting } })).data;
        return missingPid;
    },
    async [ActionTypes.STL_UPDATE_JOB]({ commit }, { stockTake }) {
        let updatedStockTake: StockTake = (await apiAxios.post('new-api/StockTake/updateStockTake', stockTake)).data;
        commit(MutationTypes.UPDATE_STL, { updatedStockTake });
    },
    async [ActionTypes.STL_LOAD_STANDBY_LOCATIONS]({ commit }, { jobNo }) {
        let standbyLocations: Array<StandbyLocation> = (await apiAxios.get('new-api/StockTake/getStandByLocations?jobNo=' + jobNo)).data;
        return standbyLocations;
    },
    async [ActionTypes.STL_SEND_TO_STANDBY_NEGATIVE]({ commit }, { jobNo, locationCode, whsCode }) {
        let standbyLocations = (await apiAxios.post('new-api/StockTake/sendStandByNegative?jobNo=' + jobNo + '&locationCode=' + locationCode + '&whsCode=' + whsCode)).data;
    },
    
    // Settings
    async [ActionTypes.SETT_LOAD_AREA_LIST]({ commit }, { filters }) {
        let areaList: Array<Area> = (await apiAxios.get('new-api/Registration/getAreaList', { params: { parameters: filters }})).data;
        commit(MutationTypes.SET_AREA_LIST, { areaList });
    },
    async [ActionTypes.SETT_LOAD_AREATYPE_LIST]({ commit }, { filters }) {
        let areaTypeList: Array<AreaType> = (await apiAxios.get('new-api/Registration/getAreaTypeList', { params: { parameters: filters }})).data;
        commit(MutationTypes.SET_AREATYPE_LIST, { areaTypeList });
    },
    async [ActionTypes.SETT_LOAD_CC_LIST]({ commit }, { filters }) {
        let ccList: Array<ControlCode> = (await apiAxios.get('new-api/Registration/getControlCodeList', { params: { parameters: filters }})).data;
        commit(MutationTypes.SET_CC_LIST, { ccList });
    },
    async [ActionTypes.SETT_LOAD_LOC_LIST]({ commit }, { filters }) {
        let result = new PaginatedList<LocationSettings>();
        result.items = new Array<LocationSettings>();
        result.pagination = new PaginationData();

        let data: PaginatedList<LocationSettings> = (await apiAxios.get('new-api/Registration/getLocationList', { params: { parameters: filters } })).data;
        result.items = data.items;
        result.pagination = data.pagination;
        commit(MutationTypes.SET_LOCATION_LIST, result);
    },
    async [ActionTypes.SETT_LOAD_PACKTYPE_LIST]({ commit }, { filters }) {
        let packageTypeList: Array<PackageType> = (await apiAxios.get('new-api/Registration/getPackageTypeList', { params: { parameters: filters }})).data;
        commit(MutationTypes.SET_PACKAGETYPE_LIST, { packageTypeList });
    },
    async [ActionTypes.SETT_LOAD_PC_LIST]({ commit }, { filters }) {
        let pcList: Array<ProductCode> = (await apiAxios.get('new-api/Registration/getProductCodeList', { params: { parameters: filters }})).data;
        commit(MutationTypes.SET_PC_LIST, { pcList });
    },
    async [ActionTypes.SETT_LOAD_SATO_LIST]({ commit }, { filters }) {
        let satoPrinterList: Array<SatoPrinter> = (await apiAxios.get('new-api/Registration/getLabelPrinterList', { params: { parameters: filters }})).data;
        commit(MutationTypes.SET_SATO_LIST, { satoPrinterList });
    },
    async [ActionTypes.SETT_LOAD_UOM_LIST]({ commit }, { filters }) {
        let uomList: Array<UomSettings> = (await apiAxios.get('new-api/Registration/getUomList', { params: { parameters: filters }})).data;
        commit(MutationTypes.SET_UOM_LIST, { uomList });
    },
    async [ActionTypes.SETT_LOAD_WAREHOUSE_LIST]({ commit }, { filters }) {
        let warehouseList: Array<Warehouse> = (await apiAxios.get('new-api/Registration/getWarehouseList', { params: { parameters: filters }})).data;
        commit(MutationTypes.SET_WAREHOUSE_LIST, { warehouseList });
    },
    async [ActionTypes.SETT_ADD_AREA]({ commit }, { area }) {
        let updatedArea = (await apiAxios.post('new-api/Registration/addArea', area)).data;
    },
    async [ActionTypes.SETT_ADD_AREATYPE]({ commit }, { areaType }) {
        let updatedAreaType = (await apiAxios.post('new-api/Registration/addAreaType', areaType)).data;
    },
    async [ActionTypes.SETT_ADD_CC]({ commit }, { controlCode }) {
        let updatedControlCode = (await apiAxios.post('new-api/Registration/addControlCode', controlCode)).data;
    },
    async [ActionTypes.SETT_ADD_LOCATION]({ commit }, { location }) {
        let updatedLocation = (await apiAxios.post('new-api/Registration/addLocation', location)).data;
    },
    async [ActionTypes.SETT_ADD_PACKAGETYPE]({ commit }, { packageType }) {
        let updatedPackageType = (await apiAxios.post('new-api/Registration/addPackageType', packageType)).data;
    },
    async [ActionTypes.SETT_ADD_PC]({ commit }, { productCode }) {
        let updatedProductCode = (await apiAxios.post('new-api/Registration/addProductCode', productCode)).data;
    },
    async [ActionTypes.SETT_ADD_SATO]({ commit }, { satoPrinter }) {
        let updatedSatoPrinter = (await apiAxios.post('new-api/Registration/addLabelPrinter', satoPrinter)).data;
    },
    async [ActionTypes.SETT_ADD_UOM]({ commit }, { uom }) {
        let updatedUom = (await apiAxios.post('new-api/Registration/addUom', uom)).data;
    },
    async [ActionTypes.SETT_ADD_WAREHOUSE]({ commit }, { warehouse }) {
        let updatedWarehouse = (await apiAxios.post('new-api/Registration/addWarehouse', warehouse)).data;
    },
    async [ActionTypes.SETT_UPDATE_AREA]({ commit }, { area }) {
        let updatedArea = (await apiAxios.post('new-api/Registration/updateArea', area)).data;
    },
    async [ActionTypes.SETT_UPDATE_AREATYPE]({ commit }, { areaType }) {
        let updatedAreaType = (await apiAxios.post('new-api/Registration/updateAreaType', areaType)).data;
    },
    async [ActionTypes.SETT_UPDATE_CC]({ commit }, { controlCode }) {
        let updatedProductCode = (await apiAxios.post('new-api/Registration/updateControlCode', controlCode)).data;
    },
    async [ActionTypes.SETT_UPDATE_LOCATION]({ commit }, { location }) {
        let updatedLocation = (await apiAxios.post('new-api/Registration/updateLocation', location)).data;
        commit(MutationTypes.UPDATE_LOCATION_SETTINGS, updatedLocation);
    },
    async [ActionTypes.SETT_UPDATE_PACKAGETYPE]({ commit }, { packageType }) {
        let updatedPackageType = (await apiAxios.post('new-api/Registration/updatePackageType', packageType)).data;
    },
    async [ActionTypes.SETT_UPDATE_PC]({ commit }, { productCode }) {
        let updatedProductCode = (await apiAxios.post('new-api/Registration/updateProductCode', productCode)).data;
    },
    async [ActionTypes.SETT_UPDATE_SATO]({ commit }, { satoPrinter }) {
        let updatedSatoPrinter = (await apiAxios.post('new-api/Registration/updateLabelPrinter', satoPrinter)).data;
    },
    async [ActionTypes.SETT_UPDATE_UOM]({ commit }, { uom }) {
        let updatedPackageType = (await apiAxios.post('new-api/Registration/updateUom', uom)).data;
    },
    async [ActionTypes.SETT_UPDATE_WAREHOUSE]({ commit }, { warehouse }) {
        let updatedWarehouse = (await apiAxios.post('new-api/Registration/updateWarehouse', warehouse)).data;
    },
    async [ActionTypes.SETT_LOAD_WAREHOUSE]({ commit }, { code }) {
        let warehouse: Warehouse = (await apiAxios.get('new-api/Registration/getWarehouse', { params: { code: code } })).data;
        return warehouse;
    },
    async [ActionTypes.SETT_CB_LOAD_ACTIVE_WAREHOUSES]({ commit }) {
        let activeWarehouses = (await apiAxios.get('new-api/Registration/getActiveWarehouses')).data;
        return activeWarehouses;
    },
    async [ActionTypes.SETT_CB_LOAD_ACTIVE_AREAS]({ commit }, { whsCode }) {
        let activeWarehouses = (await apiAxios.get('new-api/Registration/getActiveAreas', { params: { warehouseCode: whsCode } })).data;
        return activeWarehouses;
    },
    async [ActionTypes.SETT_CB_LOAD_ACTIVE_AREA_TYPES]({ commit }) {
        let activeWarehouses = (await apiAxios.get('new-api/Registration/getActiveAreaTypes')).data;
        return activeWarehouses;
    },
    async [ActionTypes.SETT_CB_LOAD_ILOG_LOC_CAT]({ commit }) {
        let activeWarehouses = (await apiAxios.get('new-api/Registration/getILogLocationCategories')).data;
        return activeWarehouses;
    },
    async [ActionTypes.SETT_LOAD_AREA]({ commit }, { code, whsCode }) {
        let area = (await apiAxios.get('new-api/Registration/getArea', { params: { code: code, areaWhsCode: whsCode } })).data;
        return area;
    },
    async [ActionTypes.SETT_LOAD_LOCATION]({ commit }, { code, whsCode }) {
        let location = (await apiAxios.get('new-api/Registration/getLocation', { params: { code: code, warehouseCode: whsCode } })).data;
        return location;
    },
    async [ActionTypes.SETT_LOAD_LOC_MORE]({ commit, state }, { filters }) {
        const pagination = {
            pageNumber: state.settingsLocationList.pagination.pageNumber + 1,
            itemsPerPage: state.settingsLocationList.pagination.itemsPerPage
        }
        const res = (await apiAxios.get<PaginatedList<LocationSettings>>('new-api/Registration/getLocationList', { params: { parameters: { ...filters, pagination } } }));
        if (res && res.status == 200) {
            let result = new PaginatedList<LocationSettings>();
            result.items = state.settingsLocationList.items.concat(res.data.items);
            result.pagination = res.data.pagination;
            commit(MutationTypes.SET_LOCATION_LIST, result);
        } else {
            throw res;
        }
    },
    async [ActionTypes.SETT_TOGGLE_ACTIVE_LOCATION]({ commit }, { code, whsCode }) {
        let updatedLocation = (await apiAxios.post('new-api/Registration/toggleActiveLocation?code=' + code + '&warehouseCode=' + whsCode, null)).data;
        commit(MutationTypes.UPDATE_LOCATION_SETTINGS, updatedLocation);
    },
    async [ActionTypes.SETT_PRINT_LOC_LABELS]({ commit }, { locations }) {
        let printLabels = (await apiAxios.post('Registration/printLocationLabels', locations))
    },
    async [ActionTypes.SETT_GET_LOC_LABELS]({ commit }, { locations }) {
        let printedLabels = (await apiAxios.post('Registration/getLocationLabels', locations)).data
        return printedLabels;
    },
    async [ActionTypes.SETT_DELETE_SATO]({ commit }, { ip }) {
        let deleteSato = (await apiAxios.delete('new-api/Registration/deleteLabelPrinter?ip='+ip)).data
    }
}
