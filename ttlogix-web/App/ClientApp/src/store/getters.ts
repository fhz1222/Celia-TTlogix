//import { RootState } from '../root-state'
import { GetterTree } from 'vuex'
import { State } from './state'
import { Adjustment } from './models/adjustment';

export type Getters = {
    getOwnership(state: State): (ownershipId: number) => string;
    getStorageDetailStatus(state:State): (statusId: number) => string;
    getAdjustmentStatus(state:State): (statusId: number) => string;
    getAdjustmentJobType(state:State): (jobTypeId: number) => string;
    getAdjustmentDetail(state:State): (jobNo: string) => Adjustment;
    getDecantStatus(state:State): (statusId: number) => string;
    getScannerType(state:State): (scannerId: number) => string;
    getBondedStatus(state:State): (bondedStatus: boolean) => string;
    getInboundReversalStatus(state: State): (statusId: number) => string;
    getStockTransferReversalStatus(state: State): (statusId: number) => string;
    getCustomerStatus(state: State): (statusId: number) => string;
    getCustomerUomStatus(state: State): (statusId: number) => string;
    getCustomerClientStatus(state: State): (statusId: number) => string;
    getStockTakeStatus(state: State): (statusId: number) => string;
    getAreaStatus(state: State): (statusId: number) => string;
    getAreaTypeStatus(state: State): (statusId: number) => string;
    getAreaTypeType(state: State): (statusId: number) => string;
    getWarehouseStatus(state: State): (statusId: number) => string;
    getWarehouseType(state: State): (statusId: number) => string;
    getPackageTypeStatus(state: State): (statusId: number) => string;
    getPackageTypeType(state: State): (statusId: number) => string;
    getUomStatus(state: State): (statusId: number) => string;
    getUomType(state: State): (statusId: number) => string;
    getControlCodeStatus(state: State): (statusId: number) => string;
    getControlCodeType(state: State): (statusId: number) => string;
    getProductCodeStatus(state: State): (statusId: number) => string;
    getProductCodeType(state: State): (statusId: number) => string;
    getLocationStatus(state: State): (statusId: number) => string;
    getLocationType(state: State): (statusId: number) => string;
    getLocationPriority(state: State): (statusId: number) => string;
    getSatoPrinterType(state: State): (statusId: number) => string;
}

export const getters: Getters = {
    getOwnership:(state) => (ownershipId) => state.config.dictionaries.ownership[ownershipId],
    getStorageDetailStatus:(state) => (statusId) => state.config.dictionaries.storageDetailStatus[statusId],
    getAdjustmentStatus:(state) => (statusId) => state.config.dictionaries.adjustmentStatus[statusId],
    getAdjustmentJobType:(state) => (jobTypeId) => state.config.dictionaries.adjustmentJobType[jobTypeId],
    getAdjustmentDetail:(state) => (jobNo) => { 
        return state.adjustmentList.items.find(ad => { return ad.jobNo == jobNo }) ?? new Adjustment();
    },
    getDecantStatus:(state) => (statusId) => state.config.dictionaries.decantStatus[statusId],
    getScannerType:(state) => (scannerId) => state.config.dictionaries.scannerType[scannerId],
    getBondedStatus:(state) => (bondedStatus) => state.config.dictionaries.bondedStatus[bondedStatus ? "true" : "false"],
    getInboundReversalStatus: (state) => (statusId) => state.config.dictionaries.inboundReversalStatus[statusId],
    getStockTransferReversalStatus: (state) => (statusId) => state.config.dictionaries.stockTransferReversalStatus[statusId],
    getCustomerStatus: (state) => (statusId) => state.config.dictionaries.customerStatus[statusId],
    getCustomerUomStatus: (state) => (statusId) => state.config.dictionaries.customerUomStatus[statusId],
    getCustomerClientStatus: (state) => (statusId) => state.config.dictionaries.customerClientStatus[statusId],
    getStockTakeStatus: (state) => (statusId) => state.config.dictionaries.stockTakeStatus[statusId],
    getAreaStatus: (state) => (statusId) => state.config.dictionaries.areaStatus[statusId],
    getAreaTypeStatus: (state) => (statusId) => state.config.dictionaries.areaTypeStatus[statusId],
    getAreaTypeType: (state) => (statusId) => state.config.dictionaries.areaTypeType[statusId],
    getWarehouseStatus: (state) => (statusId) => state.config.dictionaries.warehouseStatus[statusId],
    getWarehouseType: (state) => (statusId) => state.config.dictionaries.warehouseType[statusId],
    getPackageTypeStatus: (state) => (statusId) => state.config.dictionaries.packageTypeStatus[statusId],
    getPackageTypeType: (state) => (statusId) => state.config.dictionaries.packageTypeType[statusId],
    getUomStatus: (state) => (statusId) => state.config.dictionaries.uomStatus[statusId],
    getUomType: (state) => (statusId) => state.config.dictionaries.uomType[statusId],
    getControlCodeStatus: (state) => (statusId) => state.config.dictionaries.controlCodeStatus[statusId],
    getControlCodeType: (state) => (statusId) => state.config.dictionaries.controlCodeType[statusId],
    getProductCodeStatus: (state) => (statusId) => state.config.dictionaries.productCodeStatus[statusId],
    getProductCodeType: (state) => (statusId) => state.config.dictionaries.productCodeType[statusId],
    getLocationStatus: (state) => (statusId) => state.config.dictionaries.locationStatus[statusId],
    getLocationType: (state) => (statusId) => state.config.dictionaries.locationType[statusId],
    getLocationPriority: (state) => (statusId) => state.config.dictionaries.locationPriority[statusId],
    getSatoPrinterType: (state) => (statusId) => state.config.dictionaries.satoPrinterType[statusId]
}
