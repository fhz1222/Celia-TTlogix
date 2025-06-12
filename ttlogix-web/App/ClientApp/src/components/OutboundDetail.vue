<template>
    <div class="wrap position-relative">
        <div class="wait-window" :hidden="!inProgress">
            <loading class="loader" color="#2C5A81" />
        </div>
        <!-- Page title -->
        <div class="row toolbar-new">
            <div class="col-md-8">
                <h2 class="mt-3 mb-3 display-6">{{ $t('outbound.detail.outboundJobNo__', { 'jobNo': jobNo }) }}</h2>
            </div>
            <div class="col-md-4 text-end pe-4 main-action-icons">
                <div class="d-inline-block link-box">
                    <a href="#" @click.stop.prevent="refresh()" class="text-center">
                        <div><i class="las la-redo-alt"></i></div>
                        <div>Refresh</div>
                    </a>
                </div>
            </div>
        </div>

        <div class="" v-if="!locked">
            {{ lockingMesage }}
        </div>
        <div v-else>
            <!-- Outbound header -->
            <div class="outbound-deail">
                <outbound-header v-model="header" :errors="errors" :pickingList="pickingList" @save="(callback) => { save().finally(callback) }"
                                 @eKanbanDownload="alert = 'not available'" @cargoOut="(callback) => { cargoOut().finally(callback) }"
                                 @releaseBondedStock="(callback) => { releaseBondedStock().finally(callback) }" @input="setChanged()"
                                 @truckDeparture="(callback) => { truckDeparture().finally(callback) }" @autoallocateAll="(callback) => { autoallocateAll().finally(callback) }"
                                 @undoAutoallocateAll="(callback) => { undoAutoallocateAll().finally(callback) }" @releaseForPicking="modal='request_picking'" />
            </div>

            <div class="row mt-5 mb-5">
                <div :class="invoicingAvailable ? 'col-9' : 'col'">
                    <div class="card">
                        <div class="card-header">
                            <h5>{{ $t('outbound.detail.documentationTitle') }}</h5>
                        </div>
                        <div class="card-body">
                            <report-button v-if="header.customerCode != 'VRN'"
                                        :tooltip="isDeliveryDocketAvailable ? null : $t('tooltip.completeOutboundFirst')"
                                        :disabled="!isDeliveryDocketAvailable" @click="report('DeliveryDocketReport')"
                                        :lastPrinted="getLastPrintText(reportNames.DeliveryDocketReportEuro)">
                                1.1 {{ $t('outbound.report.deliveryDocket') }}
                            </report-button>

                            <report-button @click="report('OutboundReport')" :lastPrinted="getLastPrintText(reportNames.LoadingOutboundReportEuroWHS)">
                                2. {{ $t('outbound.report.outboundReport') }}
                            </report-button>

                            <report-button v-if="header.customerCode != 'VRN'"
                                        :tooltip="isDeliveryDocketAvailable ? null : $t('tooltip.completeOutboundFirst')"
                                        :disabled="!isDeliveryDocketAvailable" @click="report('DeliveryDocketWithPIDReport')"
                                        :lastPrinted="getLastPrintText(reportNames.DeliveryDocketReportWithPIDEuro)">
                                1.2 {{ $t('outbound.report.deliveryDocketWithPID') }}
                            </report-button>

                            <report-button @click="report('PickingListReport')" :lastPrinted="getLastPrintText(reportNames.PickingListReportEU)">
                                4. {{ $t('outbound.report.pickingList') }}
                            </report-button>

                            <report-button @click="report('PickingInstructionReport')" :lastPrinted="getLastPrintText(reportNames.PickingInstructionReportRFEuro)">
                                5. {{ $t('outbound.report.pickingInstruction') }}
                            </report-button>

                            <report-button @click="report('PackingListReport')" :lastPrinted="getLastPrintText(reportNames.PackingListReport)">
                                6. {{ $t('outbound.report.packingList') }}
                            </report-button>

                            <report-button @click="downloadEDTToCSV()" :lastPrinted="getLastPrintText(reportNames.OutboundEDTToCSV)">
                                5. {{ $t('outbound.report.downloadEDTToCSV') }}
                            </report-button>
                        </div>
                    </div>
                </div>
                <div v-if="invoicingAvailable" class="col-3">
                    <InvoicingWidget v-bind="invoiceStatus" 
                        @block="(c) => block().finally(() => {c(); refreshInvStatus()})" 
                        @unblock="(c) => unblock().finally(() => {c(); refreshInvStatus()})"
                        @request-now="(c) => requestNow().finally(() => {c(); refreshInvStatus()})"></InvoicingWidget>
                </div>
            </div>
                

            <!-- Tabs -->
            <div class="row mt-4">
                <div class="col-md-12 pb-5">
                    <nav>
                        <div class="nav nav-tabs" id="nav-tab" role="tablist">
                            <button class="nav-link active" id="nav-home-tab" data-bs-toggle="tab"
                                    data-bs-target="#nav-outbound-details" type="button" role="tab"
                                    aria-controls="nav-outbound-details" aria-selected="true">
                                {{ $t('outbound.detail.outboundDetail') }}
                            </button>
                            <button class="nav-link" id="nav-profile-tab" data-bs-toggle="tab"
                                    data-bs-target="#nav-packing-list" type="button" role="tab"
                                    aria-controls="nav-packing-list" aria-selected="false">
                                {{ $t('outbound.detail.pickingList') }}
                            </button>
                            <button class="nav-link" id="nav-order-summary" data-bs-toggle="tab"
                                    data-bs-target="#nav-outbound-summary" type="button" role="tab"
                                    aria-controls="nav-outbound-summary" aria-selected="false" v-if="header.showOrderSummary">
                                {{ $t('outbound.detail.orderSummary') }}
                            </button>
                            <button class="nav-link" id="nav-order-delivery" data-bs-toggle="tab"
                                    data-bs-target="#nav-outbound-delivery" type="button" role="tab"
                                    aria-controls="nav-outbound-delivery" aria-selected="false" v-if="$auth.user().whsCode == 'IT'">
                                {{ $t('outbound.detail.deliveryInformation') }}
                            </button>
                        </div>
                    </nav>
                    <div class="tab-content" id="nav-tabContent">

                        <!-- Outbound detail tab -->
                        <div class="tab-pane fade show active" id="nav-outbound-details" role="tabpanel"
                             aria-labelledby="nav-home-tab">
                            <table class="table table-sm table-striped table-hover align-middle">
                                <thead>
                                    <tr>
                                        <th @click.prevent.stop="sortDetails(row => row.lineItem, 'lineItem')"
                                            class="text-end pointer">
                                            {{ $t('outbound.detail.grid_No') }}
                                            <i v-if="sortedBy_details == 'lineItem'" :class="sortIcon()"></i>
                                        </th>
                                        <th @click.prevent.stop="sortDetails(row => row.productCode, 'productCode')"
                                            class="pointer">
                                            {{ $t('outbound.detail.grid_ANC') }}
                                            <i v-if="sortedBy_details == 'productCode'" :class="sortIcon()"></i>
                                        </th>
                                        <th @click.prevent.stop="sortDetails(row => row.supplierID, 'supplierID')"
                                            class="pointer">
                                            {{ $t('outbound.detail.grid_SupplierId') }}
                                            <i v-if="sortedBy_details == 'supplierID'" :class="sortIcon()"></i>
                                        </th>
                                        <th @click.prevent.stop="sortDetails(row => row.qty, 'qty')"
                                            class="text-end pointer">
                                            {{ $t('outbound.detail.grid_OrderQty') }}
                                            <i v-if="sortedBy_details == 'qty'" :class="sortIcon()"></i>
                                        </th>
                                        <th @click.prevent.stop="sortDetails(row => row.pickedQty, 'pickedQty')"
                                            class="text-end pointer">
                                            {{ $t('outbound.detail.grid_AllocatedQty') }}
                                            <i v-if="sortedBy_details == 'pickedQty'" :class="sortIcon()"></i>
                                        </th>
                                        <th @click.prevent.stop="sortDetails(row => row.uom, 'uom')" class="pointer">
                                            {{ $t('outbound.detail.grid_UOM') }}
                                            <i v-if="sortedBy_details == 'uom'" :class="sortIcon()"></i>
                                        </th>
                                        <th @click.prevent.stop="sortDetails(row => row.pkg, 'pkg')"
                                            v-if="header.transType == 2" class="num pointer">
                                            {{ $t('outbound.detail.grid_OrderPKG') }}
                                            <i v-if="sortedBy_details == 'pkg'" :class="sortIcon()"></i>
                                        </th>
                                        <th @click.prevent.stop="sortDetails(row => row.pickedPkg, 'pickedPkg')"
                                            class="text-end pointer">
                                            {{ $t('outbound.detail.grid_AllocatedPkg') }}
                                            <i v-if="sortedBy_details == 'pickedPkg'" :class="sortIcon()"></i>
                                        </th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(row, index) in details" :key="index">
                                        <td class="text-end">{{ row.lineItem }}</td>
                                        <td>{{ row.productCode }}</td>
                                        <td>{{ row.supplierID }}</td>
                                        <td class="text-end">{{ row.qty }}</td>
                                        <td class="text-end">{{ row.pickedQty }}</td>
                                        <td>{{ row.uom }}</td>
                                        <td v-if="header.transType == 2" class="num">{{ row.pkg }}</td>
                                        <td class="text-end">{{ row.pickedPkg }}</td>
                                        <td class="text-end">
                                            <button @click="cancelAllocation(row)"
                                                    class="btn btn-danger btn-sm me-2" v-if="isCancelAllocationVisible" :disabled="isCancelAllocationDisabled || batchAllocationInProgress">
                                                <i class="las la-ban"></i>
                                                {{ $t('outbound.operation.cancelAllocation') }}
                                            </button>
                                            <button class="btn btn-primary btn-sm me-2" @click="onProductCodeClick(row)" v-if="isPickingListBtnVisible"  :disabled="batchAllocationInProgress">
                                                <i class="las la-hand-pointer"></i>
                                                {{ $t('outbound.operation.manualAllocation') }}
                                            </button>

                                            <awaiting-button v-if="header.allowAutoallocation" :disabled="batchAllocationInProgress"
                                                             v-bind:id="'btnAutoAll_' + row.lineItem"
                                                             @proceed="(callback) => { allocate(row).finally(callback) }"
                                                             :btnText="'outbound.operation.autoAllocation'"
                                                             :btnIcon="'las la-project-diagram'"
                                                             :btnClass="'btn-primary'">
                                            </awaiting-button>

                                            <awaiting-button v-if="header.allowAutoallocation" :disabled="batchAllocationInProgress"
                                                             v-bind:id="'btnAutoDeal_' + row.lineItem"
                                                             @proceed="(callback) => { deallocate(row).finally(callback) }"
                                                             :btnText="'outbound.operation.undoAutoAllocation'"
                                                             :btnIcon="'las la-reply'"
                                                             :btnClass="'btn-primary'">
                                            </awaiting-button>

                                            <awaiting-button v-if="header.transType != 2 && row.status != 2" :disabled="batchAllocationInProgress"
                                                             @proceed="(callback) => { remove(row).finally(callback) }"
                                                             :btnText="'outbound.operation.delete'"
                                                             :btnIcon="'las la-trash-alt'"
                                                             :btnClass="'btn-danger'">
                                            </awaiting-button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="row mt-4">
                                <div class="col-md-12">
                                    <button @click="modal = 'pick'" v-if="isPickEntryVisible"
                                            class="btn btn-primary btn-sm me-2">
                                        <i class="las la-hand-pointer"></i> {{$t('outbound.operation.pickEntry')}}
                                    </button>
                                    <template v-if="details.length > 0">
                                        <button class="btn btn-primary btn-sm me-2" @click="modal = 'split_injob'"
                                                v-if="header.transType != 3 && header.transType != 4" :disabled="isSplitByCategoryDisabled">
                                            <i class="las la-stream"></i> {{ $t('outbound.operation.splitByInboundJob') }}
                                        </button>
                                        <button class="btn btn-primary btn-sm" @click="modal = 'split_ownership'"
                                                v-if="header.transType != 3 && header.transType != 4" :disabled="isSplitByCategoryDisabled">
                                            <i class="las la-stream"></i> {{ $t('outbound.operation.splitByOwnership') }}
                                        </button>
                                    </template>
                                </div>
                            </div>
                        </div>

                        <!-- Packing list tab -->
                        <div class="tab-pane fade" id="nav-packing-list" role="tabpanel" aria-labelledby="nav-home-tab">
                            <table class="table table-sm table-striped table-hover align-middle">
                                <thead>
                                    <tr>
                                        <th></th>
                                        <!--<th class="pointer" @click.prevent.stop="sortPicking(row => row.index,'index')">-->
                                        <th>
                                            {{ $t('outbound.detail.grid_No') }}
                                            <!--<i v-if="sortedBy_picking == 'index'" :class="sortIcon('picking')"></i>-->
                                        </th>
                                        <th class="pointer"
                                            @click.prevent.stop="sortPicking(row => row.productCode, 'productCode')">
                                            {{ $t('outbound.detail.grid_ANC') }}
                                            <i v-if="sortedBy_picking == 'productCode'"
                                               :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortPicking(row => row.qty, 'qty')">
                                            {{ $t('outbound.detail.grid_Quantity') }}
                                            <i v-if="sortedBy_picking == 'qty'" :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer"
                                            @click.prevent.stop="sortPicking(row => row.whsCode, 'whsCode')">
                                            {{ $t('outbound.detail.grid_WHS') }}
                                            <i v-if="sortedBy_picking == 'whsCode'" :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer"
                                            @click.prevent.stop="sortPicking(row => row.locationCode, 'locationCode')">
                                            {{ $t('outbound.detail.grid_Location') }}
                                            <i v-if="sortedBy_picking == 'locationCode'"
                                               :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortPicking(row => row.pid, 'pid')">
                                            {{ $t('outbound.detail.grid_PID') }}
                                            <i v-if="sortedBy_picking == 'pid'" :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer"
                                            @click.prevent.stop="sortPicking(row => row.externalPID, 'externalPID')">
                                            {{ $t('outbound.detail.grid_ExternalPID') }}
                                            <i v-if="sortedBy_picking == 'externalPID'"
                                               :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer"
                                            @click.prevent.stop="sortPicking(row => row.ownershipString, 'ownershipString')">
                                            {{ $t('outbound.detail.grid_Ownership') }}
                                            <i v-if="sortedBy_picking == 'ownershipString'"
                                               :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer"
                                            @click.prevent.stop="sortPicking(row => row.inboundDate, 'inboundDate')">
                                            {{ $t('outbound.detail.grid_InboundDate') }}
                                            <i v-if="sortedBy_picking == 'inboundDate'"
                                               :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer"
                                            @click.prevent.stop="sortPicking(row => row.pickedBy, 'pickedBy')">
                                            {{ $t('outbound.detail.grid_PickedBy') }}
                                            <i v-if="sortedBy_picking == 'pickedBy'" :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer"
                                            @click.prevent.stop="sortPicking(row => row.pickedDate, 'pickedDate')">
                                            {{ $t('outbound.detail.grid_PickedDate') }}
                                            <i v-if="sortedBy_picking == 'pickedDate'" :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer"
                                            @click.prevent.stop="sortPicking(row => row.currency, 'currency')">
                                            {{ $t('outbound.detail.grid_Currency') }}
                                            <i v-if="sortedBy_picking == 'currency'" :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortPicking(row => row.price, 'price')">
                                            {{ $t('outbound.detail.grid_UnitPrice') }}
                                            <i v-if="sortedBy_picking == 'price'" :class="sortIcon('picking')"></i>
                                        </th>
                                        <th class="pointer"
                                            @click.prevent.stop="sortPicking(row => row.pidValue, 'pidValue')">
                                            {{ $t('outbound.detail.grid_PIDValue') }}
                                            <i v-if="sortedBy_picking == 'pidValue'" :class="sortIcon('picking')"></i>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(row, index) in pickingList" :key="index"
                                        :class="{ 'selected': isSelected(row) }">
                                        <td>
                                            <input type="checkbox" @click.stop v-model="pickingListItemsSelected"
                                                   :value="row" />
                                        </td>
                                        <td class="num">{{ index + 1 }}</td>
                                        <td>{{ row.productCode }}</td>
                                        <td>{{ row.qty }}</td>
                                        <td>{{ row.whsCode }}</td>
                                        <td>{{ row.locationCode }}</td>
                                        <td>{{ row.pid }}</td>
                                        <td>{{ row.externalPID }}</td>
                                        <td>{{ row.ownershipString }}</td>
                                        <td>
                                            <date :value="row.inboundDate" />
                                        </td>
                                        <td>{{ row.pickedBy }}</td>
                                        <td>
                                            <date :value="row.pickedDate" />
                                        </td>
                                        <td>{{ row.currency }}</td>
                                        <td>
                                            <input type="text" class="form-control form-control-sm"
                                                   v-model="row.price" />
                                        </td>
                                        <td class="num">{{ row.pidValue }}</td>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="row mt-4">
                                <div class="col-md-12">
                                    <button class="btn btn-primary btn-sm me-2" @click.stop="calc_total()">
                                        <i class="las la-calculator"></i>
                                        {{ $t('outbound.operation.calculateOutboundValue') }}
                                    </button>
                                    <button class="btn btn-primary btn-sm me-2" @click.stop="update_price_master()">
                                        <i class="las la-upload"></i> {{ $t('outbound.operation.updateStockValue') }}
                                    </button>
                                    <button class="btn btn-primary btn-sm me-2" v-if="header.transType != 4"
                                            :disabled="pickingListItemsSelected.length == 0 || isSplitOutboundDisabled"
                                            @click="modal = 'split'">
                                        <i class="las la-stream"></i> {{ $t('outbound.operation.splitOutbound') }}
                                    </button>
                                    <button class="btn btn-primary btn-sm me-2"
                                            :disabled="pickingListItemsSelected.length == 0 || isUndoPickDisabled"
                                            @click.stop="undoPicking()">
                                        <i class="las la-stream"></i> {{ $t('outbound.operation.undoPick') }}
                                    </button>
                                </div>
                            </div>
                        </div>

                        <div class="tab-pane fade" id="nav-outbound-summary" role="tabpanel" aria-labelledby="nav-order-summary">
                            <table class="table table-sm table-striped table-hover align-middle">
                                <thead>
                                    <tr>
                                        <th class="pointer" @click.prevent.stop="sortSummary(row => row.productCode, 'productCode')">
                                            {{ $t('outbound.detail.productCode') }}
                                            <i v-if="sortedBy_summary == 'productCode'" :class="sortIcon('summary')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortSummary(row => row.supplierID, 'supplierID')">
                                            {{ $t('outbound.detail.supplierID') }}
                                            <i v-if="sortedBy_summary == 'supplierID'" :class="sortIcon('summary')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortSummary(row => row.orderQty, 'orderQty')">
                                            {{ $t('outbound.detail.orderQty') }}
                                            <i v-if="sortedBy_summary == 'orderQty'" :class="sortIcon('summary')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortSummary(row => row.outboundQty, 'outboundQty')">
                                            {{ $t('outbound.detail.outboundQty') }}
                                            <i v-if="sortedBy_summary == 'outboundQty'" :class="sortIcon('summary')"></i>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(row, index) in orderSummary" :key="index">
                                        <td>{{ row.productCode }}</td>
                                        <td>{{ row.supplierID }}</td>
                                        <td>{{ row.orderQty }}</td>
                                        <td>{{ row.outboundQty }}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>

                        <div class="tab-pane fade" id="nav-outbound-delivery" role="tabpanel" aria-labelledby="nav-order-delivery">
                            <div class="row mt-4">
                                <div class="col-md-6">
                                    <div class="row">
                                        <div class="col-md-3 text-end no-right-padding">{{$t('outbound.detail.selfDelivery')}}</div>
                                        <div class="col-md-9"><input type="checkbox" class="form-check-input" v-model="selfDelivery" @change="selfDeliveryChange()" /></div>
                                    </div>
                                    <div class="row mt-3">
                                        <div class="col-md-3 text-end no-right-padding pt-1">{{$t('outbound.detail.companyCode')}}:</div>
                                        <div class="col-md-8">
                                            <select class="form-select form-select-sm" v-model="header.deliveryTo" @change="currentDeliveryChange()" :disabled="selfDelivery">
                                                <option v-for="ac in activeCustomers" :key="ac.code" :value="ac.code">{{ac.code + ' - ' + ac.name}}</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="row mt-2">
                                        <div class="col-md-3 text-end no-right-padding">{{$t('outbound.detail.companyName')}}:</div>
                                        <div class="col-md-9">{{currentDelivery.name}}</div>
                                    </div>
                                    <div class="row mt-2">
                                        <div class="col-md-3 text-end no-right-padding">{{$t('outbound.detail.address')}}:</div>
                                        <div class="col-md-9">{{currentDelivery.address1}}</div>
                                    </div>
                                    <div class="row mt-1" v-if="currentDelivery.address2">
                                        <div class="col-md-3 text-end no-right-padding"></div>
                                        <div class="col-md-9">{{currentDelivery.address2}}</div>
                                    </div>
                                    <div class="row mt-1" v-if="currentDelivery.address3">
                                        <div class="col-md-3 text-end no-right-padding"></div>
                                        <div class="col-md-9">{{currentDelivery.address3}}</div>
                                    </div>
                                    <div class="row mt-1" v-if="currentDelivery.address4">
                                        <div class="col-md-3 text-end no-right-padding"></div>
                                        <div class="col-md-9">{{currentDelivery.address4}}</div>
                                    </div>
                                    <div class="row mt-2">
                                        <div class="col-md-3 text-end no-right-padding">{{$t('outbound.detail.zone')}}:</div>
                                        <div class="col-md-9">{{currentDelivery.zone}}</div>
                                    </div>
                                    <div class="row mt-2">
                                        <div class="col-md-3 text-end no-right-padding">{{$t('outbound.detail.postalCode')}}:</div>
                                        <div class="col-md-9">{{currentDelivery.postCode}}</div>
                                    </div>
                                    <div class="row mt-2">
                                        <div class="col-md-3 text-end no-right-padding">{{$t('outbound.detail.country')}}:</div>
                                        <div class="col-md-9">{{currentDelivery.country}}</div>
                                    </div>
                                    <div class="row mt-3 text-center" v-if="deliveryChanged">
                                        <h5 class="text-center mt-2 blue-text weight-normal">{{$t('outbound.detail.saveReminder')}}</h5>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="row">
                                        <div class="col-md-3 text-end no-right-padding pt-1">Note:</div>
                                        <div class="col-md-8"><input class="form-control form-control-sm" v-model="deliveryDocketExtFields[0]" /></div>
                                    </div>
                                    <div class="row mt-3">
                                        <div class="col-md-3 text-end no-right-padding pt-1">Annotazioni:</div>
                                        <div class="col-md-8"><input class="form-control form-control-sm" v-model="deliveryDocketExtFields[1]" /></div>
                                    </div>
                                    <div class="row mt-3">
                                        <div class="col-md-3 text-end no-right-padding pt-1">N&deg; Colli:</div>
                                        <div class="col-md-8"><input class="form-control form-control-sm" v-model="deliveryDocketExtFields[2]" /></div>
                                    </div>
                                    <div class="row mt-3">
                                        <div class="col-md-3 text-end no-right-padding pt-1">Aspetto dei beni:</div>
                                        <div class="col-md-8"><input class="form-control form-control-sm" v-model="deliveryDocketExtFields[3]" /></div>
                                    </div>
                                    <div class="row mt-3">
                                        <div class="col-md-3 text-end no-right-padding pt-1">Cau Sale Del Tra Sporto:</div>
                                        <div class="col-md-8"><input class="form-control form-control-sm" v-model="deliveryDocketExtFields[4]" /></div>
                                    </div>
                                    <div class="row mt-3">
                                        <div class="col-md-3 text-end"></div>
                                        <div class="col-md-4">
                                            <button class="btn btn-sm btn-primary" @click="printDeliveryDocketExt()" :disabled="deliveryStatusErrorInfo">
                                                <i class="las la-file-invoice"></i> &nbsp;{{ $t('outbound.report.deliveryDocketExt') }}
                                            </button>
                                        </div>
                                    </div>
                                    <div class="row mt-3" v-if="deliveryStatusErrorInfo">
                                        <div class="col-md-9 offset-3 text-danger">
                                            Delivery Docket can ONLY be printed after the Outbound Job is &lt;Cargo Out&gt;!
                                            <br />You have to pick all the Part Numbers and CARGO OUT
                                            <br />before you can print the Delivery Docket!
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div v-if="batchAllocationInProgress" class="bg-success p-3 text-white" style="position:fixed; bottom:10px; top: auto;">
            <div>
                <div>{{ $t(batchAllocationOperation == true ? 'outbound.operation.batchAllocationInfo' : 'outbound.operation.batchDeallocationInfo') }}</div>
                <div>{{ $t(batchAllocationOperation == true ? 'outbound.operation.allocating' : 'outbound.operation.deallocating') }}: <span>{{batchAllocationCount}}</span></div>
            </div>
        </div>

        <alert v-if="alert" @close="alert = null" :text="alert" />

        <picking-list v-if="modal == 'pickinglist'"
                      :jobNo="jobNo"
                      :lineItem="lineItem"
                      :qtyToPick="qtyToPickPerLine"
                      :supplierId="supplierId"
                      :productCode="productCode"
                      :whsCode="whsCode"
                      @close="modal = null, lineItem = null, refresh()" />
        <pick-entry v-if="modal == 'pick'" :job="header" :details="details" @close="modal = null"
                    @done="modal = null, refresh()" />
        <pdf-preview v-if="modal == 'report'" :name="pdfName" :pdf="pdf"
                     @close="pdfName = pdf = null; modal = null; refresh()"></pdf-preview>
        <cancel-allocation v-if="modal == 'cancelallocation'" :jobNo="jobNo" :lineItem="lineItem" @close="modal = null"
                           @done="modal = null, refresh()" />

        <confirm v-if="modal == 'split_injob'" @close="modal = null" @ok="splitOutboundByInboundJobNo(false)">
            {{ $t('outbound.operation.splitOutboundByInboundJob__', { 'jobNo': jobNo }) }}
        </confirm>
        <confirm v-if="modal == 'split_ownership'" @close="modal = null" @ok="splitOutboundByOwnership()">
            {{ $t('outbound.operation.splitOutboundByOwnership__', { 'jobNo': jobNo }) }}
        </confirm>
        <confirm v-if="modal == 'split'" @close="modal = null" @ok="splitOutbound()">
            {{ $t('outbound.operation.areYouSureYouWantToSplitOutbound') }}
        </confirm>
        <confirm v-if="modal == 'request_picking'" @close="modal = null" @ok="releaseForPicking()">
            {{ $t('outbound.operation.areYouSureYouWantToRequestPicking') }}
        </confirm>
    </div>

</template>
<script>

import OutboundHeader from './outbound/Header'
import DateField from '@/widgets/Date'
import ReportButton from '@/widgets/ReportButtonLastPrinted'
import PickEntry from './outbound/PickEntry'
import PickingList from './outbound/PickingList'
import PDFPreview from '@/widgets/PDFPreview'
import moment from 'moment'
import reportNames from '@/enum/report_names.js'
import CancelAllocation from './outbound/CancelAllocation'
import Loading from '@/widgets/Loading'
import { setIntervalAsync } from 'set-interval-async/dynamic'
import { clearIntervalAsync } from 'set-interval-async'
import AwaitingButton from '@/widgets/AwaitingButton'
import { onRouteExit } from '@/route-exit-jobs'

import { defineComponent } from 'vue';
import InvoicingWidget from './invoicing/InvoicingWidget.vue'
export default defineComponent({
    components: { ReportButton, OutboundHeader, date: DateField, PickEntry, 'pdf-preview': PDFPreview, PickingList, CancelAllocation, Loading, AwaitingButton, InvoicingWidget },
    props: ['jobNo'],
    async created() {
        this.lockingMesage = this.$t('document.locking')
        this.changed = false;
        const lockJob = async () => {
            const r = await this.$dataProvider.locks.tryLock(this.jobNo, "Outbound");
            if (r) {
                this.locked = true;
            }
            else {
                this.locked = false;
                const r1 = await this.$dataProvider.locks.getLock(this.jobNo)
                this.lockingMesage = this.$t('document.locked', { userCode: r1.userCode })
            }
            return r;
        }
        if (await lockJob()) this.refresh(false);
        this.lockInterval = setIntervalAsync(lockJob, 30000);
        onRouteExit(this.$route.path, async () => {
            if (this.lockInterval) await clearIntervalAsync(this.lockInterval);
            await this.$dataProvider.locks.tryUnlock(this.jobNo)
        })
    },
    beforePageLeave(tab, type) {
        if (type === 'unload') { return }
        if (!this.changed) { return }
        var promise = this.$dataProvider.outbounds.updateOutboundStatus;
        var jobNo = this.jobNo;

        return new Promise((resolve) => {
            this.$confirm(
                {
                    message: this.$t('operation.doYouWantToUpdateStatusBeforeClose'),
                    button: { no: this.$t('operation.general.close'), yes: this.$t('operation.general.updateAndClose') },
                    callback: confirm => {
                        if (confirm) {
                            promise(jobNo).then(() => {
                                this.changed = false;
                                resolve()
                            })
                        }
                        else {
                            resolve()
                            this.changed = false;
                        }
                    }
                }
            )
        });
    },
    data() {
        return {
            lockInterval: null,
            reportNames: reportNames,
            pdfName: null,
            pdf: null,
            header: { jobNo: this.jobNo, status: 8, hasBondedStock: false, showBatchAllocationButtons: false, invRequest: {} },
            details: [],
            orderSummary: [],
            pickingList: [],
            errors: [],
            tab: 'detail',
            bonded: true,
            modal: null,
            alert: null,
            reportLogs: {},
            lineItem: null,
            qtyToPickPerLine: 0,
            pickingListItemsSelected: [],
            locked: false,
            changed: false,
            inProgress: false,
            lockingMesage: "",
            sortedAsc_picking: false,
            sortedAsc_details: false,
            sortedAsc_summary: false,
            sortedBy_picking: "",
            sortedBy_details: "",
            sortedBy_summary: "",
            loading: true,
            batchAllocationInProgress: false,
            batchAllocationCount: "0/0",
            batchAllocationOperation: true,
            invoiceStatus: null,
            invoicingAvailable: false,
            currentDelivery: { code: '', name: '', address1: '', address2: '', address3: '', address4: '', postCode: '', country: '' },
            activeCustomers: [{}],
            selfDelivery: true,
            deliveryDocketExtFields: [],
            deliveryChanged: false,
            deliveryStatusErrorInfo: false
        }
    },
    computed: {
        isPickingListBtnVisible() {
            var allowedTypes = [3, 0, 7, 4]
            var allowedTypesForPartial = [0, 7, 4]
            var notAllowedStatuses = [4, 5, 7, 8, 9, 10]
            return this.header.status === 3 && allowedTypesForPartial.includes(this.header.transType)
                || (allowedTypes.includes(this.header.transType) && !notAllowedStatuses.includes(this.header.status))
        },
        isSplitOutboundDisabled() {
            var allowedStatuses = [3, 4, 5]
            return [3,7].includes(this.header.transType) || !allowedStatuses.includes(this.header.status)
        },  
        isSplitByCategoryDisabled() {
            var notAllowedStatuses = [7, 8, 9, 10]
            return this.header && [3,7].includes(this.header.transType) || notAllowedStatuses.includes(this.header.status)
        },
        isUndoPickDisabled() {
            var allowedStatuses = [3, 4, 5]
            return this.header && !allowedStatuses.includes(this.header.status)
        },
        isDeliveryDocketAvailable() {
            return this.header && (this.header.transType != 0 || this.header.status == 8)
        },
        isCancelAllocationVisible() {
            var notAllowedTransTypes = [0, 3, 4, 7];
            return this.header && !notAllowedTransTypes.includes(this.header.transType) && this.header.status != 10
        },
        isCancelAllocationDisabled() {
            var notAllowedStatuses = [4, 5, 7, 8, 9, 10]
            return this.header && notAllowedStatuses.includes(this.header.status)
        },
        isPickEntryVisible() {
            return this.header && ![2, 7].includes(this.header.transType) && ![8, 9, 4].includes(this.header.status);
        }
    },
    methods: {
        refresh() {
            this.loading = true;
            Promise.all([this.getHeader(), this.getOutboundDetails(), this.getPickingListWithUOM(), this.refreshInvStatus()])
                .then(values => {
                    // header
                    this.header = { hasBondedStock: false, ...values[0] }
                    
                    if (this.header.status == 4 || this.header.status == 5) {
                        /*this.$dataProvider.storage.hasBondedStock(this.jobNo).then(bonded => {
                            this.header.hasBondedStock = bonded
                        })*/
                    }
                    this.deliveryChanged = false;
                    this.reportLogs = {}
                    this.header.reportsPrinted.forEach(r => {
                        let t = {
                            by: r.printedBy,
                            on: moment(r.printedDate, 'YYYY-MM-DD\\THH:mm:ss').format('DD/MM/YYYY HH:mm')
                        }
                        if (this.reportLogs[r.reportName]) {
                            this.reportLogs[r.reportName].push(t)
                        }
                        else {
                            this.reportLogs[r.reportName] = [t]
                        }
                    })
                    // details
                    this.details = values[1];
                    this.header.showBatchAllocationButtons = this.details.length > 0 ? true : false;
                    // picking list
                    this.pickingList = values[2];
                    if (this.header.showOrderSummary) {
                        this.getOrderSummary().then(summary => {
                            this.orderSummary = summary;
                            this.changed = false;
                            this.loading = false;
                        });
                    } else {
                        this.changed = false;
                        this.loading = false;
                    }
                    this.getActiveCustomerClients(this.header.customerCode).then((ac) => {
                        this.activeCustomers = ac;
                        this.selfDelivery = this.header.deliveryTo == '' ? true : false;
                        if (!this.selfDelivery) this.currentDeliveryChange(false)
                    });
                    this.deliveryStatusErrorInfo = [7, 8, 10].includes(this.header.status) ? false : true;
                })
        },
        async getHeader() {
            // return {}
            const resp = this.$dataProvider.outbounds.getOutbound(this.jobNo);
            return resp;
        },
        async getOutboundDetails() {
            const resp = this.$dataProvider.outbounds.getOutboundDetails(this.jobNo);
            return resp;
        },
        async getPickingListWithUOM() {
            const resp = this.$dataProvider.pickingLists.getPickingListWithUOM(this.jobNo);
            return resp;
        },
        async getActiveCustomerClients(customerCode) {
            const resp = this.$dataProvider.delivery.getDeliveryCustomerClients(customerCode);
            return resp;
        },
        async refreshInvStatus() {
            this.invoicingAvailable = await this.$dataProvider.invoicing.isActive();
            if (this.invoicingAvailable){
                this.invoiceStatus = await this.$dataProvider.invoicing.getStatus(this.jobNo);
            }
        },
        async block() {
            return this.$dataProvider.invoicing.block(this.jobNo);
        },
        async unblock() {
            return this.$dataProvider.invoicing.unblock(this.jobNo);
        },
        async requestNow() {
            return this.$dataProvider.invoicing.requestNow(this.jobNo);
        },
        async getOrderSummary() {
            const resp = this.$dataProvider.outbounds.getOrderSummary(this.jobNo);
            return resp;
        },
        save() {
            this.errors = []
            return this.$dataProvider.outbounds.patchOutbound(this.jobNo, this.header).then(() => {
                //this.$emit('done', resp.data)
                this.$notify({
                    title: this.$t('success.Done'),
                    text: this.$t('success.Saved'),
                    type: 'success',
                    group: 'temporary'
                })
                //this.changed = false
                this.refresh(false)
            }).catch(e => {
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                }
            })
        },
        allocate(detail, refresh = true, notify = true, singleDisable = true) {
            this.errors = []
            if (singleDisable) this.toggleAllocationButtons(detail.lineItem, true);
            return this.$dataProvider.pickingLists.autoAllocate(this.jobNo, detail.lineItem).then((resp) => {
                if (singleDisable) this.toggleAllocationButtons(detail.lineItem, false);
                if (notify) {
                    this.$notify({
                        title: this.$t('success.Done'),
                        text: this.$t(resp ? 'success.ANCAllocated' : 'success.ANCAlreadyAllocated', { productCode: detail.productCode }),
                        type: 'success',
                        group: 'temporary'
                    });
                }
                if (refresh) { this.refresh(); }
            }).catch(e => {
                if (singleDisable) this.toggleAllocationButtons(detail.lineItem, false);
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                }
            })
        },
        async autoallocateAll() {
            if (this.details.length > 0) {
                var result = Promise.resolve();
                var current = 1;
                this.$emit('batch-allocation-in-progress', true);
                this.batchAllocationCount = current + '/' + this.details.length;
                this.batchAllocationInProgress = true;
                this.batchAllocationOperation = true;
                this.details.forEach((row) => {
                    result = result.then(() => this.allocate(row, false, false, false)).finally(() => {
                        current++;
                        this.batchAllocationCount = current + '/' + this.details.length;
                        if (current > this.details.length) {
                            this.batchAllocationInProgress = false;
                            this.$notify({
                                title: this.$t('success.Done'),
                                text: this.$t('success.ItemsAllocated'),
                                type: 'success',
                                group: 'temporary'
                            });
                            this.getOutboundDetails().then((resp) => this.details = resp);
                        }
                    });
                });
                return result;
            }
        },
        remove(detail) {
            this.errors = []
            return this.$dataProvider.outbounds.deleteOutboundDetail(this.jobNo, detail.lineItem).then(() => {
                //this.$emit('done', resp.data)
                this.$notify({
                    title: this.$t('success.Done'),
                    text: this.$t('success.ItemsDeleted'),
                    type: 'success',
                    group: 'temporary'
                })
                this.refresh()
            })
        },
        deallocate(detail, refresh = true, notify = true, singleDisable = true) {
            this.errors = []
            if (singleDisable) this.toggleAllocationButtons(detail.lineItem, true);
            return this.$dataProvider.pickingLists.unAllocate([{ jobNo: this.jobNo, lineItem: detail.lineItem }]).then(() => {
                if (singleDisable) this.toggleAllocationButtons(detail.lineItem, false);
                if (notify) {
                    this.$notify({
                        title: this.$t('success.Done'),
                        text: this.$t('success.ANCUnallocated', { productCode: detail.productCode }),
                        type: 'success',
                        group: 'temporary'
                    })
                }
                if (refresh) { this.refresh(); }
            }).catch(e => {
                if (singleDisable) this.toggleAllocationButtons(detail.lineItem, false);
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                }
            })
        },
        async undoAutoallocateAll() {
            if (this.details.length > 0) {
                var result = Promise.resolve();
                var current = 1;
                this.$emit('batch-allocation-in-progress', true);
                this.batchAllocationCount = current + '/' + this.details.length;
                this.batchAllocationInProgress = true;
                this.batchAllocationOperation = true;
                this.details.forEach((row) => {
                    result = result.then(() => this.deallocate(row, false, false, false)).finally(() => {
                        current++;
                        this.batchAllocationCount = current + '/' + this.details.length;
                        if (current > this.details.length) {
                            this.batchAllocationInProgress = false;
                            this.$notify({
                                title: this.$t('success.Done'),
                                text: this.$t('success.ItemsDeallocated'),
                                type: 'success',
                                group: 'temporary'
                            });
                            this.getOutboundDetails().then((resp) => this.details = resp);
                        }
                    });
                });
                return result;
            }
        },
        isSelected(row) {
            return this.pickingListItemsSelected.includes(row)
        },
        splitOutbound() {
            if (this.pickingListItemsSelected) {
                this.errors = []
                let pickingListItems = this.pickingListItemsSelected.map(i => ({ jobNo: this.jobNo, lineItem: i.lineItem, seqNo: i.seqNo }))
                this.$dataProvider.outbounds.splitOutbound({
                    jobNo: this.jobNo,
                    ownershipSplit: false,
                    pickingListItemIds: pickingListItems
                }).then((resp) => {
                    this.$notify({
                        title: this.$t('success.Done'),
                        text: this.$t('success.Completed'),
                        type: 'success',
                        group: 'temporary'
                    })
                    this.refresh();
                    resp.filter(x => x != this.jobNo).forEach(x => this.$router.push({name : 'outbound_detail', params: {jobNo: x}}));
                }).catch(e => {
                    if (e.response && e.response.data.errors) {
                        this.errors = e.response.data.errors
                    }
                }).finally(() => { this.modal = null });
            }
        },
        splitOutboundByInboundJobNo() {
            this.errors = []
            this.$dataProvider.outbounds.splitOutboundByInboundJobNo(this.jobNo).then((resp) => {
                this.$notify({
                    title: this.$t('success.Done'),
                    text: this.$t('success.Completed'),
                    type: 'success',
                    group: 'temporary'
                })
                this.refresh();
                resp.filter(x => x != this.jobNo).forEach(x => this.$router.push({ name: 'outbound_detail', params: { jobNo: x } }));
            }).catch(e => {
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                }
            }).finally(() => { this.modal = null });
        },
        splitOutboundByOwnership() {
            this.errors = []
            this.$dataProvider.outbounds.splitOutboundByOwnership(this.jobNo).then((resp) => {
                this.$notify({
                    title: this.$t('success.Done'),
                    text: this.$t('success.Completed'),
                    type: 'success',
                    group: 'temporary'
                })
                this.refresh();
                resp.filter(x => x != this.jobNo).forEach(x => this.$router.push({name : 'outbound_detail', params: {jobNo: x}}));
            }).catch(e => {
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                }
            }).finally(() => { this.modal = null });
        },
        undoPicking() {
            if (this.pickingListItemsSelected) {
                this.errors = []
                this.modal = true
                let pids = this.pickingListItemsSelected.map(i => (i.pid))
                this.$dataProvider.outbounds.undoPicking({
                    OutJobNo: this.jobNo,
                    PIDs: pids
                }).then(() => {
                    this.$notify({
                        title: this.$t('success.Done'),
                        text: this.$t('success.Completed'),
                        type: 'success',
                        group: 'temporary'
                    })
                    this.refresh()
                }).catch(e => {
                    if (e.response && e.response.data.errors) {
                        this.errors = e.response.data.errors
                    }
                })
            }
        },
        releaseBondedStock() {
            this.errors = []
            this.modal = true
            return this.$dataProvider.outbounds.releaseBondedStock(this.jobNo, this.header).then(() => {
                this.$notify({
                    title: this.$t('success.Done'),
                    text: this.$t('success.Completed'),
                    type: 'success',
                    group: 'temporary'
                })
                this.refresh()
            }).catch(e => {
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                }
            })
        },
        async releaseForPicking() {
            this.inProgress = true;
            const resp = await this.$dataProvider.ilogIntegration.releaseOutboundForPicking(this.header.jobNo).finally(() => { this.inProgress = false; this.modal = null });
            this.$notify({
                title: this.$t('success.Done'),
                text: `Sent picking request to iLog: ${resp.pickingRequestId} (rev. ${resp.revision}). Requested ${resp.palletsCount} pallets.`,
                type: 'success',
                group: 'temporary'
            })
        },
        cargoOut() {
            this.inProgress = true
            let type = null
            switch (this.header.transType) {

                case 2:
                    type = 'OutboundEurope'
                    break;
                case 3:
                    type = 'OutboundReturn'
                    break
                case 4:
                    type = 'WHSTransfer'
                    break;
                case 0:
                case 7:
                    type = 'OutboundManual'
                    break;
                case 1://cross dock
                default:
                    return;
            }
            return this.$dataProvider.outbounds.complete(type, this.header.jobNo).then(() => {
                this.$notify({
                    title: this.$t('success.Done'),
                    text: this.$t('success.Completed'),
                    type: 'success',
                    group: 'temporary'
                })
                this.refresh()
            }).finally(() => this.inProgress = false)
        },
        whitelist() {
            this.inProgress = true;
            this.$dataProvider.outbounds.whitelist(this.header.jobNo).then(() => {
                this.$notify({ title: this.$t('success.Done'), text: this.$t('success.Whitelisted'), type: 'success', group: 'temporary' });
                this.refresh();
            }).finally(() => this.inProgress = false)
        },
        blacklist() {
            this.inProgress = true;
            this.$dataProvider.outbounds.blacklist(this.header.jobNo).then(() => {
                this.$notify({ title: this.$t('success.Done'), text: this.$t('success.Blacklisted'), type: 'success', group: 'temporary' });
                this.refresh();
            }).finally(() => this.inProgress = false)
        },
        requestInvoiceNow() {
            this.inProgress = true;
            this.$dataProvider.outbounds.requestInvoiceNow(this.header.jobNo, false).then(() => {
                this.$notify({ title: this.$t('success.Done'), text: this.$t('success.InvoiceRequested'), type: 'success', group: 'temporary' });
                this.refresh();
            }).finally(() => this.inProgress = false)
        },
        report(type) {
            this.modal = 'report'
            this.$dataProvider.apiAxios.get(`outbounds/get${type}`, {
                params: {
                    jobNo: this.jobNo
                },
                responseType: 'blob',
                headers: {
                    Accept: 'application/pdf'
                }
            }).then(resp => {
                
                this.pdf = resp.data
                let headerLine = resp.headers['Content-Disposition'] || resp.headers['content-disposition'];
                const regExpFilename = /filename="?(?<filename>[^;"]*)/;

                this.pdfName = regExpFilename.exec(headerLine)?.groups?.filename ?? null;
            }).catch((e) => {
                this.$dataProvider.errorHandlers.reports(e)
                this.modal = this.pdf = this.pdfName = null
            });
        },
        cancelAllocation(row) {
            this.modal = 'cancelallocation'
            this.lineItem = row
        },
        downloadEDTToCSV() {
            this.$dataProvider.outbounds.downloadEDTToCSV(this.jobNo).then(() => this.refresh())
        },
        getLastPrintText(reportName) {
            if (this.reportLogs[reportName] && this.reportLogs[reportName].length > 0) {
                return this.$t('reportLog.lastPrinted', this.reportLogs[reportName][0])
            }
            return null
        },
        calc_total() {
            this.errors = {}
            this.inProgress = true
            this.$dataProvider.storage.updateSellingPrice(
                {
                    data: this.pickingList.map(d => ({ pid: d.pid, price: d.price }))
                })
                .then(() => this.refresh())
                .catch(e => {
                    if (e.response && e.response.data.errors) {
                        this.errors = e.response.data.errors
                    }}).finally(() => this.inProgress = false)
        },
        update_price_master() {
            this.errors = {}
            this.inProgress = true
            this.$dataProvider.priceMasters.updatePriceMasterOutbound({
                customerCode: this.header.customerCode,
                jobNo: this.jobNo,
                pickingListForPriceMasterDtos: this.pickingList.map(d => ({ pid: d.pid, supplierId: d.supplierID, productCode: d.productCode, price: d.price }))
            })
                .then(() => this.refresh())
                .catch(e => {
                    if (e.response && e.response.data.errors) {
                        this.errors = e.response.data.errors
                    }
                }).finally(() => this.inProgress = false)
        },
        onProductCodeClick(row) {
            this.modal = 'pickinglist';
            this.lineItem = row.lineItem;
            this.qtyToPickPerLine = row.qty;
            this.supplierId = row.supplierID;
            this.productCode = row.productCode;
            this.whsCode = this.header.whsCode;
        },
        sortPicking(selector, by, mode) {
            if (mode) this.sortedAsc_picking = mode == 'asc';
            else if (this.sortedBy_picking != by) this.sortedAsc_picking = true;
            else this.sortedAsc_picking = !this.sortedAsc_picking;
            this.sortedBy_picking = by;
            this.pickingList = this.pickingList.sort((a, b) => {
                if (typeof (selector(a)) === 'string' || selector(a) instanceof String) {
                    return selector(a).localeCompare(selector(b)) * (this.sortedAsc_picking ? 1 : -1);
                } else {
                    var x = selector(a); var y = selector(b); var d = this.sortedAsc_picking ? -1 : 1;
                    return ((x < y) ? d : ((x > y) ? (-d) : 0));
                }
            });
        },
        truckDeparture(){
            this.inProgress = true;
            return this.$dataProvider.outbounds.truckDeparture(this.header.jobNo).then(() => {
                this.$notify({
                    title: this.$t('success.Done'),
                    text: this.$t('success.Completed'),
                    type: 'success',
                    group: 'temporary'
                })
                this.refresh()
            }).finally(() => this.inProgress = false);

        },
        sortDetails(selector, by, mode) {
            if (mode) this.sortedAsc_details = mode == 'asc';
            else if (this.sortedBy_details != by) this.sortedAsc_details = true;
            else this.sortedAsc_details = !this.sortedAsc_details;
            this.sortedBy_details = by;
            this.details = this.details.sort((a, b) => {
                if (typeof (selector(a)) === 'string' || selector(a) instanceof String) {
                    return selector(a).localeCompare(selector(b)) * (this.sortedAsc_details ? 1 : -1);
                } else {
                    var x = selector(a); var y = selector(b); var d = this.sortedAsc_details ? -1 : 1;
                    return ((x < y) ? d : ((x > y) ? (-d) : 0));
                }
            });
        },
        sortSummary(selector, by, mode) {
            if (mode) this.sortedAsc_summary = mode == 'asc';
            else if (this.sortedBy_summary != by) this.sortedAsc_summarys = true;
            else this.sortedAsc_summary = !this.sortedAsc_summary;
            this.sortedBy_summary = by;
            this.orderSummary = this.orderSummary.sort((a, b) => {
                if (typeof (selector(a)) === 'string' || selector(a) instanceof String) {
                    return selector(a).localeCompare(selector(b)) * (this.sortedAsc_summary ? 1 : -1);
                } else {
                    var x = selector(a); var y = selector(b); var d = this.sortedAsc_summary ? -1 : 1;
                    return ((x < y) ? d : ((x > y) ? (-d) : 0));
                }
            });
        },
        sortIcon(table) {
            if (!table) {
                return this.sortedAsc_details ? 'las la-sort-amount-up-alt' : 'las la-sort-amount-down';
            } else if (table == 'picking') {
                return this.sortedAsc_picking ? 'las la-sort-amount-up-alt' : 'las la-sort-amount-down';
            } else if (table == 'summary') {
                return this.sortedAsc_summary ? 'las la-sort-amount-up-alt' : 'las la-sort-amount-down';
            }
        },
        setChanged() {
            this.changed = !this.loading;
        },
        toggleAllocationButtons(id, disable) {
            document.getElementById('btnAutoAll_' + id).disabled = disable ? true : false;
            document.getElementById('btnAutoDeal_' + id).disabled = disable ? true : false;
        },
        currentDeliveryChange(changed = true) {
            if (!this.selfDelivery) this.currentDelivery = this.activeCustomers.find((ac) => ac.code == this.header.deliveryTo)
            else { this.currentDelivery = {}; this.header.deliveryTo = ''; }
            this.deliveryChanged = changed;
        },
        selfDeliveryChange() {
            this.header.deliveryTo = this.selfDelivery ? '' : this.header.deliveryTo;
            if (this.selfDelivery) this.currentDelivery = {}; this.header.deliveryTo = '';
            this.deliveryChanged = true;
        },
        printDeliveryDocketExt() {
            this.modal = 'report'
            const whsCode = this.header.whsCode;
            const jobNo = this.header.jobNo;
            const refNo = this.header.refNo;
            const param1 = this.deliveryDocketExtFields[0] ?? '';
            const param2 = this.deliveryDocketExtFields[1] ?? '';
            const param3 = this.deliveryDocketExtFields[2] ?? '';
            const param4 = this.deliveryDocketExtFields[3] ?? '';
            const param5 = this.deliveryDocketExtFields[4] ?? '';
            const url = this.header.transType == 4 ? 'unitec_whs' : 'unitec';
            this.$dataProvider.apiAxios.get(`report/outbound/${url}?whsCode=${whsCode}&jobNo=${jobNo}&refNo=${refNo}&parameters[0]=${param1}&parameters[1]=${param2}&parameters[2]=${param3}&parameters[3]=${param4}&parameters[4]=${param5}`, {
                responseType: 'blob',
                headers: {
                    Accept: 'application/pdf'
                }
            }).then(resp => {
                
                this.pdf = resp.data
                let headerLine = resp.headers['Content-Disposition'] || resp.headers['content-disposition'];
                const regExpFilename = /filename="?(?<filename>[^;"]*)/;

                this.pdfName = regExpFilename.exec(headerLine)?.groups?.filename ?? null;
            }).catch((e) => {
                this.$dataProvider.errorHandlers.reports(e)
                this.modal = this.pdf = this.pdfName = null
            });
        }
    }
})
</script>
<style lang="scss">
.tabs {
    .tab-content {
        max-height: 400px;
        overflow: auto;
    }
}

h1 {
    padding-bottom: 20px;
}
</style>