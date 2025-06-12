<template>
    
    <div class="wrap">
        <div class="wait-window" :hidden="!inProgress"><loading class="loader" color="#2C5A81" /></div>

        <alert v-if="alert" @close="alert = null" :text="alert" />
        <!-- Page title -->
        <div class="row toolbar-new">
            <div class="col-md-8">
                <h2 class="mt-3 mb-3 display-6 float-start">{{$t('stockTransferJobNo__', {'jobNo': jobNo}) }}</h2>
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
            <div class="stocktransfer-detail">
                <stock-transfer-header v-model="header"
                                       :errors="errors"
                                       :detailsLength="details.length"
                                       @save="(callback) => { save().finally(callback) }"
                                       @complete="(callback) => { complete().then(callback) }"
                                       :key="headerKey" />
            </div>

            
            <div class="row mt-5 mb-5">
                <div :class="invoicingAvailable ? 'col-9' : 'col'">
                    <!-- Reports -->
                        <div class="card">
                            <div class="card-header">
                                <h5>{{$t('documentationHeader')}}</h5>
                            </div>
                            <div class="card-body">
                                <button class="btn btn-sm btn-primary me-2" @click="report()"><i class="las la-file-invoice"></i> 1. {{$t('stockTransferReportButton')}}</button>
                                <button class="btn btn-sm btn-primary" @click="downloadEDT()"><i class="las la-file-invoice"></i> 4. {{$t('stockTransferEDTButton')}}</button>
                            </div>
                        </div>
                </div>
                <div class="col-3" v-if="invoicingAvailable">
                    <InvoicingWidget v-if="invoiceStatus" v-bind="invoiceStatus" 
                        @block="(c) => block().finally(() => {c(); refreshInvStatus()})" 
                        @unblock="(c) => unblock().finally(() => {c(); refreshInvStatus()})"
                        @request-now="(c) => requestNow().finally(() => {c(); refreshInvStatus()})"></InvoicingWidget>
                </div>
            </div>

            <!-- Stock Transfer Details -->
            <div class="row mt-4">
                <div class="col-md-12">
                    <nav>
                        <div class="nav nav-tabs" id="nav-tab" role="tablist">
                            <button class="nav-link active"
                                    id="nav-details-tab"
                                    data-bs-toggle="tab"
                                    data-bs-target="#nav-details"
                                    type="button"
                                    role="tab"
                                    aria-controls="nav-details"
                                    :aria-selected="true">
                                {{$t('stockTransfer.detail.stockTransferDetails')}}
                            </button>
                            <button class="nav-link"
                                    id="nav-summary-tab"
                                    data-bs-toggle="tab"
                                    data-bs-target="#nav-summary"
                                    type="button"
                                    role="tab"
                                    aria-controls="nav-summary"
                                    aria-selected="false"
                                    v-if="showEStockTransferTab">
                                {{$t('stockTransfer.detail.stockTransferSummary')}}
                            </button>
                        </div>
                    </nav>

                    <div class="tab-content" id="nav-tabContent">


                        <!-- Stock Transfer Details -->
                        <div class="tab-pane fade show active" id="nav-details" role="tabpanel" aria-labelledby="nav-details-tab">
                            <!-- Table -->
                            <table class="table table-sm table-striped table-hover align-middle selectable" v-if="details.length">
                                <thead>
                                    <tr>
                                        <th></th>
                                        <th class="num pointer" @click.prevent.stop="sort(row => row.lineItem,'lineItem')">
                                            {{$t('noHeader')}}
                                            <i v-if="sortedBy == 'lineItem'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sort(row => row.pid,'pid')">
                                            {{$t('pidHeader')}}
                                            <i v-if="sortedBy == 'pid'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sort(row => row.originalSupplierID,'originalSupplierID')">
                                            {{$t('originalSupplierHeader')}}
                                            <i v-if="sortedBy == 'originalSupplierID'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sort(row => row.productCode1,'productCode1')">
                                            {{$t('productCodeHeader')}}
                                            <i v-if="sortedBy == 'productCode1'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sort(row => row.description,'description')">
                                            {{$t('descriptionHeader')}}
                                            <i v-if="sortedBy == 'description'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sort(row => row.qty,'qty')">
                                            {{$t('qtyHeader')}}
                                            <i v-if="sortedBy == 'qty'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sort(row => row.wHSCode,'wHSCode')">
                                            {{$t('newWHSHeader')}}
                                            <i v-if="sortedBy == 'wHSCode'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sort(row => row.locationCode,'locationCode')">
                                            {{$t('newLocationHeader')}}
                                            <i v-if="sortedBy == 'locationCode'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sort(row => row.inboundDate,'inboundDate')">
                                            {{$t('receivedDateHeader')}}
                                            <i v-if="sortedBy == 'inboundDate'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sort(row => row.currency,'currency')">
                                            {{$t('currencyHeader')}}
                                            <i v-if="sortedBy == 'currency'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="num pointer" @click.prevent.stop="sort(row => row.price,'price')">
                                            {{$t('unitPriceHeader')}}
                                            <i v-if="sortedBy == 'price'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="num pointer" @click.prevent.stop="sort(row => row.pidValue,'pidValue')">
                                            {{$t('pidValueHeader')}}
                                            <i v-if="sortedBy == 'pidValue'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th></th>
                                    </tr>
                                    <tr>
                                        <th></th>
                                        <th></th>
                                        <th>
                                            <div class="d-flex">
                                                <input type="text" class="tab-filter-input in-group" v-model="tableFilter.pid" placeholder="filter..." v-on:input="filter()" />
                                                <button class="btn btn-xs btn-secondary in-group" type="button" @click.stop="tableFilter.pid='';filter()"><i class="las la-times-circle"></i></button>
                                            </div>
                                        </th>
                                        <th>
                                            <div class="d-flex">
                                                <input type="text" class="tab-filter-input in-group" v-model="tableFilter.originalSupplierID" placeholder="filter..." v-on:input="filter()" />
                                                <button class="btn btn-xs btn-secondary in-group" type="button" @click.stop="tableFilter.originalSupplierID='';filter()"><i class="las la-times-circle"></i></button>
                                            </div>
                                        </th>
                                        <th>
                                            <div class="d-flex">
                                                <input type="text" class="tab-filter-input in-group" v-model="tableFilter.productCode1" placeholder="filter..." v-on:input="filter()" />
                                                <button class="btn btn-xs btn-secondary in-group" type="button" @click.stop="tableFilter.productCode1='';filter()"><i class="las la-times-circle"></i></button>
                                            </div>
                                        </th>
                                        <th>
                                            <div class="d-flex">
                                                <input type="text" class="tab-filter-input in-group" v-model="tableFilter.description" placeholder="filter..." v-on:input="filter()" />
                                                <button class="btn btn-xs btn-secondary in-group" type="button" @click.stop="tableFilter.description='';filter()"><i class="las la-times-circle"></i></button>
                                            </div>
                                        </th>
                                        <th></th>
                                        <th></th>
                                        <th>
                                            <div class="d-flex">
                                                <input type="text" class="tab-filter-input in-group" v-model="tableFilter.locationCode" placeholder="filter..." v-on:input="filter()" />
                                                <button class="btn btn-xs btn-secondary in-group" type="button" @click.stop="tableFilter.locationCode='';filter()"><i class="las la-times-circle"></i></button>
                                            </div>
                                        </th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                    </tr>

                                </thead>
                                <tbody>
                                    <tr v-for="(row, index) in detailsFiltered"
                                        :key="index"
                                        @click.stop="onDetailsListClick(row)"
                                        :class="getSelectedClass(detailsSelected,row)">
                                        <td>
                                            <input type="checkbox" @click.stop v-model="detailsSelected" :value="row" />
                                        </td>
                                        <td class="num">{{row.lineItem}}</td>
                                        <td>{{row.pid}}</td>
                                        <td>{{row.originalSupplierID}}</td>
                                        <td>{{row.productCode1}}</td>
                                        <td>{{row.description}}</td>
                                        <td>{{row.qty}}</td>
                                        <td>{{row.wHSCode}}</td>
                                        <td>{{row.locationCode}}</td>
                                        <td><date :value="row.inboundDate" /></td>
                                        <td>{{row.currency}}</td>
                                        <td>
                                            <input type="text" class="form-control form-control-sm text-end" v-model="row.price" step="0.01" />
                                        </td>
                                        <td>{{row.pidValue}}</td>
                                        <td>
                                            <button class="btn btn-danger btn-sm no-line-break" @click.stop="modal = { type: 'confirm', pid: row.pid, lineItem: row.lineItem }" v-if="isRemoveVisible">
                                                <i class="las la-trash-alt"></i> {{$t('outbound.operation.delete')}}
                                            </button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>

                            <!-- Butons -->
                            <div class="row mt-3">
                                <div class="col-md-6 text-start">
                                    <button class="btn btn-sm btn-primary me-2" @click.stop="add()" :disabled="isAddPIDDisabled">
                                        <i class="las la-plus-circle"></i> {{$t('addPIDButton')}}
                                    </button>
                                    <button class="btn btn-sm btn-primary me-2" @click.stop="splitByInDate()" :disabled="isSplitDisabled">
                                        <i class="las la-stream"></i> {{$t('stockTransfer.detail.splitByInDate')}}
                                    </button>
                                    <button class="btn btn-primary btn-sm me-2" v-if="isPrintLabelVisible"
                                            @click.prevent="printLabel()">
                                        <i class="las la-print"></i> {{$t('inbound.operation.printLabel')}}
                                    </button>
                                </div>
                                <div class="col-md-6 text-end">
                                    <button class="btn btn-sm btn-primary me-2" @click.stop="calcSTF()" :disabled="details.length == 0">
                                        <i class="las la-calculator"></i> {{$t('calculateSTFButton')}}
                                    </button>
                                    <button class="btn btn-sm btn-primary me-2" @click.stop="updatePriceMaster()" :disabled="details.length == 0">
                                        <i class="las la-upload"></i> {{$t('outbound.operation.updateStockValue')}}
                                    </button>
                                </div>
                            </div>
                        </div>

                        <!-- Stock Transfer Summary -->
                        <div class="tab-pane fade" id="nav-summary" role="tabpanel" aria-labelledby="nav-summary-tab" v-if="showEStockTransferTab">
                            <!-- Table -->
                            <table class="table table-sm table-striped table-hover align-middle selectable" v-if="summary.length">
                                <thead>
                                    <tr>
                                        <th>{{$t('stockTransfer.detail.columnNo')}}</th>
                                        <th>{{$t('stockTransfer.detail.columnProductCode')}}</th>
                                        <th>{{$t('stockTransfer.detail.columnSupplierId')}}</th>
                                        <th>{{$t('stockTransfer.detail.columnOrderQty')}}</th>
                                        <th>{{$t('stockTransfer.detail.columnPickedQty')}}</th>
                                        <th>{{$t('stockTransfer.detail.columnOrderPkg')}}</th>
                                        <th>{{$t('stockTransfer.detail.columnPickedPkg')}}</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(row, index) in summary" :key="index">
                                        <td>{{row.orderNo}}</td>
                                        <td>{{row.productCode}}</td>
                                        <td>{{row.supplierID}}</td>
                                        <td>{{row.quantity}}</td>
                                        <td>{{row.pickedQty}}</td>
                                        <td>{{row.pkg}}</td>
                                        <td>{{row.pickedPkg}}</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>

                    </div>
                </div>
            </div>



        </div>

        <!-- Features -->
        <alert v-if="alert" @close="alert = null" :text="alert" />
        <pdf-preview v-if="modal == 'report'" :name="pdfName" :pdf="pdf" @close="pdfName = pdf = null; modal = null"></pdf-preview>
        <add-pid-feature v-if="modal == 'addPid'" :jobNo="jobNo" :customerCode="header.customerCode" :supplier="supplierID" @close="modal=null, refresh()"></add-pid-feature>
        <print-label-feature v-if="modal != null && modal.type == 'printLabel'"
                             :all="modal.allPids"
                             :selected="modal.selectedPids"
                             :labelType="modal.labelType"
                             @done="refresh()"
                             @close="modal=null">
        </print-label-feature>
        <confirm v-if="modal != null && modal.type == 'confirm'" @ok="remove(modal.lineItem), modal = 'processing'" @close="modal = null">{{$t('operation.doYouWantToRemoveLineFromSTF', { 'pid': modal.pid, 'jobNo': jobNo }, )}}</confirm>
        <confirm v-if="modal == 'complete-with-discrepancies'" @close="modal = null" @ok="modal=null, goComplete(jobNo)">
            {{ $t('stockTransfer.detail.discrepanciesConfirm') }}
        </confirm>
        <processing v-if="modal !=null && modal == 'processing'">tesssds</processing>
    </div>
</template>
<script>
    import StockTransferHeader from './stocktransfer/Header'
    import PDFPreview from '@/widgets/PDFPreview'
    import AddPidFeature from './stocktransfer/AddPid'
    import DateField from '@/widgets/Date'
    import PrintLabelFeature from './print/PrintLabelPID'
    import Loading from '@/widgets/Loading'
    import { setIntervalAsync } from 'set-interval-async/dynamic'
    import { clearIntervalAsync } from 'set-interval-async'
    import { onRouteExit } from '@/route-exit-jobs'
    import { defineComponent } from 'vue';
    import Confirm from '@/widgets/Confirm'
    import Processing from '@/widgets/Processing'
    import InvoicingWidget from './invoicing/InvoicingWidget.vue'

    export default defineComponent({
        components: { StockTransferHeader, 'pdf-preview': PDFPreview, AddPidFeature, date: DateField, PrintLabelFeature, Loading, Confirm, Processing, InvoicingWidget },
        props: ['jobNo'],
        async created() {
            this.settings = await this.$dataProvider.settings.get()
            this.lockingMesage = this.$t('document.locking')
            const lockJob = async() => {
                const r = await this.$dataProvider.locks.tryLock(this.jobNo, "StkXfr");
                if (r) {
                    this.changed = false;
                    this.locked = true;
                }
                else {
                    this.locked = false;
                    this.changed = false;
                    const r1 = await this.$dataProvider.locks.getLock(this.jobNo)
                    this.lockingMesage = this.$t('document.locked', { userCode: r1.userCode})
                }
                return r;
            }
            if (await lockJob()) this.refresh();
            this.lockInterval = setIntervalAsync(lockJob, 30000)
            onRouteExit(this.$route.path, async () => {
                if (this.lockInterval) await clearIntervalAsync(this.lockInterval);
                await this.$dataProvider.locks.tryUnlock(this.jobNo)
            })
        },
        data() {
            return {
                lockInterval: null,
                modal: null,
                pdfName: null,
                pdf: null,
                header: null,
                details: [],
                detailsSelected: [],
                errors: [],
                summary: [],
                locked: false,
                headerKey: 0,
                settings: null,
                alert: null,
                supplierID: null,
                inProgress: false,
                lockingMesage: "",
                tableFilter: {
                    'originalSupplierID':'',
                    'productCode1': '',
                    'description': '',
                    'locationCode': ''
                },
                sortedAsc: false,
                sortedBy: 'pid',
                invoiceStatus: null,
                invoicingAvailable: false
            }
        },
        computed: {
            isPrintLabelVisible() { return this.header && this.header.statusString == 'Completed' },
            isAddPIDDisabled() { return this.header && ['Completed', 'Cancelled'].includes(this.header.statusString) },
            isCompleteDisabled() { return this.header && ['Completed', 'Cancelled'].includes(this.header.statusString) },
            isSplitDisabled() { return this.header && ['Completed', 'Cancelled'].includes(this.header.statusString) || (this.details && this.details.length == 0) },
            isRemoveVisible() { return this.header && !['Completed', 'Cancelled'].includes(this.header.statusString) },
            showEStockTransferTab() { return this.header && this.header.transferType == 2 }
        },
        methods: {
            refresh() {
                this.$dataProvider.stockTransfers.getStockTransfer(this.jobNo).then(headResp => {
                    this.header = headResp;
                })

                this.$dataProvider.stockTransfers.getStockTransferDetails(this.jobNo).then(resp => {
                    this.details = resp;
                    this.detailsFiltered = resp;
                    this.supplierID = resp && resp.length > 0 ? resp[0].supplierID : null
                })
                this.$dataProvider.stockTransfers.getStockTransferSummary(this.jobNo).then((resp) => {
                    this.summary = resp;
                })
                this.refreshInvStatus();
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
            save() {
                this.errors = []
                return this.$dataProvider.stockTransfers.updateStockTransfer(this.jobNo, this.header).then(() => {
                    this.$notify({
                        title: this.$t('success.Done'),
                        text: this.$t('success.Saved'),
                        type: 'success',
                        group: 'temporary'
                    })
                }).catch(e => {
                    if (e.response && e.response.data.errors) {
                        this.errors = e.response.data.errors
                    }
                })
            },
            report() {
                this.modal = 'report'
                //todo move this to data provider
                this.$dataProvider.apiAxios.get(`stockTransfers/getStockTransferReport`,
                    {
                        params: {
                            jobNo: this.jobNo
                        },
                        responseType: 'blob',
                        headers: {
                            Accept: 'application/pdf'
                        }
                    }).then(resp => {
                        this.pdf = resp.data;
                        let headerLine = resp.headers['Content-Disposition'] || resp.headers['content-disposition'];
                        const regExpFilename = /filename="?(?<filename>[^;"]*)/;
                        this.pdfName = regExpFilename.exec(headerLine)?.groups?.filename ?? null;
                    }).catch((e) => {
                        this.$dataProvider.errorHandlers.reports(e)
                        this.modal = this.pdf = this.pdfName = null
                    });
            },
            downloadEDT() {
                this.$dataProvider.stockTransfers.downloadEDTToCSV(this.jobNo)
            },
            add() {
                this.modal = 'addPid';
            },
            remove(lineItem){
                this.$dataProvider.stockTransfers.deleteStockTransferDetail(this.jobNo, lineItem).then(() => {
                    this.refresh();
                    this.modal = null;
                })
            },
            splitByInDate() {
                this.$dataProvider.stockTransfers.splitByInboundDate(this.jobNo).then(() => {
                    this.refresh();
                    this.alert = this.$t('success.SplitCompleted')
                });
            },
            calcSTF() {
                this.inProgress = true
                this.$dataProvider.storage.updateSellingPrice(
                    {
                        data: this.details.map(d => ({ pid: d.pid, price: d.price }))
                    })
                    .then(() => {
                        this.headerKey += 1;
                        this.refresh();
                    })
                    .finally(() => this.inProgress = false)
            },
            updatePriceMaster() {
                this.inProgress = true
                this.errors = {}
                this.$dataProvider.priceMasters.updatePriceMasterOutbound({
                    customerCode: this.header.customerCode,
                    jobNo: this.jobNo,
                    pickingListForPriceMasterDtos: this.details.map(d => ({ pid: d.pid, supplierId: d.originalSupplierID, productCode: d.productCode1, price: d.price }))
                })
                    .then(() => this.refresh())
                    .catch(e => {
                        if (e.response && e.response.data.errors) {
                            this.errors = e.response.data.errors
                        }
                    })
                    .finally(() => this.inProgress = false)
            },
            complete() {
                if (this.header.transferType == 2) {
                    this.$dataProvider.eStockTransfers.hasAnyEStockTransferDiscrepancy(this.jobNo).then((discrepanciesPresent) => {
                        if (discrepanciesPresent) {
                            this.modal = 'complete-with-discrepancies';
                        } else {
                            this.goComplete(this.jobNo);
                        }
                    });
                } else {
                    this.goComplete(this.jobNo);
                }
            },
            goComplete(jobNo) {
                this.inProgress = true
                this.$dataProvider.stockTransfers.complete(jobNo).then(() => {
                    this.refresh();
                }).finally(() => this.inProgress = false);
            },
            printLabel() {
                this.modal = {
                    type: 'printLabel',
                    allPids: this.details.map(ld => ld.pid),
                    selectedPids: this.detailsSelected.map(ld => ld.pid),
                    labelType: 'QRPRINT_STOCKTRANSFERLABEL'
                }
            },
            onDetailsListClick(row) {
                var index = this.detailsSelected.indexOf(row);
                if (index !== -1) {
                    this.detailsSelected.splice(index, 1);
                }
                else {
                    this.detailsSelected.push(row);
                }
            },
            getSelectedClass(array, row) {
                return array.indexOf(row) > -1 ? 'selected' : '';
            },
            sort(selector, by, mode) {
                if (mode) this.sortedAsc = mode == 'asc';
                else if (this.sortedBy != by) this.sortedAsc = true;
                else this.sortedAsc = !this.sortedAsc;
                this.sortedBy = by;
                this.detailsFiltered = this.detailsFiltered.sort((a, b) => {
                    if (typeof (selector(a)) === 'string' || selector(a) instanceof String) {
                        return selector(a).localeCompare(selector(b)) * (this.sortedAsc ? 1 : -1);
                    } else {
                        var x = selector(a); var y = selector(b); var d = this.sortedAsc ? -1 : 1;
                        return ((x < y) ? d : ((x > y) ? (-d) : 0));
                    }
                });
            }, sortIcon() {
                return this.sortedAsc ? 'las la-sort-amount-up-alt' : 'las la-sort-amount-down';
            },
            filter() {
                const filters = this.tableFilter;
                this.detailsFiltered = this.details.filter(el => el.pid.toLowerCase().includes(filters.pid.toLowerCase()));
                this.detailsFiltered = this.detailsFiltered.filter(el => el.originalSupplierID.toString().includes(filters.originalSupplierID.toLowerCase()));
                this.detailsFiltered = this.detailsFiltered.filter(el => el.productCode1.toLowerCase().includes(filters.productCode1.toLowerCase()));
                this.detailsFiltered = this.detailsFiltered.filter(el => el.description.toLowerCase().includes(filters.description.toLowerCase()));
                this.detailsFiltered = this.detailsFiltered.filter(el => el.locationCode.toLowerCase().includes(filters.locationCode.toLowerCase()));
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
                this.$dataProvider.outbounds.requestInvoiceNow(this.header.jobNo, true).then(() => {
                    this.$notify({ title: this.$t('success.Done'), text: this.$t('success.InvoiceRequested'), type: 'success', group: 'temporary' });
                    this.refresh();
                }).finally(() => this.inProgress = false)
            }
        }
    })
</script>