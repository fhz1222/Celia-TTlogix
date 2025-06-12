<template>
    <div class="wrap">
        <!-- Page title -->
        <div class="row toolbar-new">
            <div class="col-md-8">
                <h2 class="mt-3 mb-3 display-6">{{$t('inbound.detail.inboundJobNo__', {'jobNo': jobNo}) }}</h2>
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
            <!-- Inbound header -->
            <div class="loading-detail">
                <inbound-header v-model="header" :errors="errors" @save="(callback) => { saveInbound().finally(callback) }" @cancel="cancel()" />
            </div>
            
            <div class="card mt-5 mb-5">
                <div class="card-header">
                    <h5>Documentation</h5>
                </div>
                <div class="card-body">
                    <button class="btn btn-primary btn-sm me-2" @click="report('LocationReport')"><i class="las la-file-invoice"></i> 1. {{$t('inbound.report.locationReport')}}</button>
                    <button class="btn btn-primary btn-sm me-2" @click="report('InboundReport')"><i class="las la-file-invoice"></i> 2. {{$t('inbound.report.inboundReport')}}</button>
                    <button class="btn btn-primary btn-sm me-2" @click="report('DiscrepancyReport')"><i class="las la-file-invoice"></i> 3. {{$t('inbound.report.discrepancyReport')}}</button>
                    <button class="btn btn-primary btn-sm me-2" @click="csvReport()"><i class="las la-file-invoice"></i> 4. {{$t('inbound.report.inboundIDT')}}</button>
                    <button class="btn btn-primary btn-sm" @click="report('WarehouseInNoteReport')"><i class="las la-file-invoice"></i> 1. {{$t('inbound.report.warehouseInNote')}}</button>
                </div>
            </div>

            <div class="row mt-4">
                <div class="col-md-12">
                    <nav>
                        <div class="nav nav-tabs" id="nav-tab" role="tablist">
                            <button class="nav-link active"
                                    id="nav-home-tab"
                                    data-bs-toggle="tab"
                                    data-bs-target="#nav-package-details"
                                    type="button"
                                    role="tab"
                                    aria-controls="nav-package-details"
                                    aria-selected="true">
                                {{$t('inbound.detail.packageDetail')}}
                            </button>
                            <button class="nav-link"
                                    id="nav-profile-tab"
                                    data-bs-toggle="tab"
                                    data-bs-target="#nav-profile"
                                    type="button"
                                    role="tab"
                                    aria-controls="nav-profile"
                                    aria-selected="false">
                                {{$t('inbound.detail.locationDetail')}}
                            </button>
                        </div>
                    </nav>
                    <div class="tab-content" id="nav-tabContent">

                        <!-- Package Details Tab -->
                        <div class="tab-pane fade show active" id="nav-package-details" role="tabpanel" aria-labelledby="nav-home-tab">
                            <table class="table table-sm table-striped table-hover align-middle">
                                <thead>
                                    <tr>
                                        <th></th>
                                        <th class="num pointer" @click.prevent.stop="sortDetails(row => row.lineItem,'lineItem')">
                                            {{$t('inbound.detail.lineItem')}}
                                            <i v-if="sortedBy_details == 'lineItem'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortDetails(row => row.productCode,'productCode')">
                                            {{$t('inbound.detail.partNo')}}
                                            <i v-if="sortedBy_details == 'productCode'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortDetails(row => row.qty,'qty')">
                                            {{$t('inbound.detail.quantity')}}
                                            <i v-if="sortedBy_details == 'qty'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortDetails(row => row.uomName,'uomName')">
                                            {{$t('inbound.detail.uom')}}
                                            <i v-if="sortedBy_details == 'uomName'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortDetails(row => row.noOfPackage,'noOfPackage')">
                                            {{$t('inbound.detail.noOfPckgs')}}
                                            <i v-if="sortedBy_details == 'noOfPackage'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th v-if="controlCodes.cC1Name" class="pointer" @click.prevent.stop="sortDetails(row => row.controlCode1,'controlCode1')">
                                            {{controlCodes.cC1Name}}
                                            <i v-if="sortedBy_details == 'controlCode1'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th v-if="controlCodes.cC2Name" class="pointer" @click.prevent.stop="sortDetails(row => row.controlCode2,'controlCode2')">
                                            {{controlCodes.cC2Name}}
                                            <i v-if="sortedBy_details == 'controlCode2'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th v-if="controlCodes.cC3Name" class="pointer" @click.prevent.stop="sortDetails(row => row.controlCode3,'controlCode3')">
                                            {{controlCodes.cC3Name}}
                                            <i v-if="sortedBy_details == 'controlCode3'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th v-if="controlCodes.cC4Name" class="pointer" @click.prevent.stop="sortDetails(row => row.controlCode4,'controlCode4')">
                                            {{controlCodes.cC4Name}}
                                            <i v-if="sortedBy_details == 'controlCode4'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th v-if="controlCodes.cC5Name" class="pointer" @click.prevent.stop="sortDetails(row => row.controlCode5,'controlCode5')">
                                            {{controlCodes.cC5Name}}
                                            <i v-if="sortedBy_details == 'controlCode5'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th v-if="controlCodes.cC6Name" class="pointer" @click.prevent.stop="sortDetails(row => row.controlCode6,'controlCode6')">
                                            {{controlCodes.cC6Name}}
                                            <i v-if="sortedBy_details == 'controlCode6'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortDetails(row => row.remark,'remark')">
                                            {{$t('inbound.detail.remark')}}
                                            <i v-if="sortedBy_details == 'remark'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortDetails(row => row.isDefected,'isDefected')">
                                            {{$t('inbound.detail.isDefected')}}
                                            <i v-if="sortedBy_details == 'isDefected'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortDetails(row => row.buyingPrice,'buyingPrice')">
                                            {{$t('inbound.detail.unitPrice')}}
                                            <i v-if="sortedBy_details == 'buyingPrice'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortDetails(row => row.lineValue,'lineValue')">
                                            {{$t('inbound.detail.lineValue')}}
                                            <i v-if="sortedBy_details == 'lineValue'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortDetails(row => row.residualValue,'residualValue')">
                                            {{$t('inbound.detail.residualValue')}}
                                            <i v-if="sortedBy_details == 'residualValue'" :class="sortIcon('details')"></i>
                                        </th>
                                        <th v-if="!isIncreaseDisabled"></th>
                                        <th v-if="!isDecreaseDisabled"></th>
                                        <th v-if="!isModifyPkgEntryDisabled"></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(row, index) in details" :key="index" @click.stop="onPackageDetailClick(row)">
                                        <td>
                                            <input type="checkbox" @click.stop v-model="packageDetailsSelected" :value="row" />
                                        </td>
                                        <td class="num">{{row.lineItem}}</td>
                                        <td>{{row.productCode}}</td>
                                        <td>{{row.qty}}</td>
                                        <td>{{row.uomName}}</td>
                                        <td>{{row.noOfPackage}}</td>
                                        <td v-if="controlCodes.cC1Name">{{row.controlCode1}}</td>
                                        <td v-if="controlCodes.cC2Name">{{row.controlCode2}}</td>
                                        <td v-if="controlCodes.cC3Name">{{row.controlCode3}}</td>
                                        <td v-if="controlCodes.cC4Name">{{row.controlCode4}}</td>
                                        <td v-if="controlCodes.cC5Name">{{row.controlCode5}}</td>
                                        <td v-if="controlCodes.cC6Name">{{row.controlCode6}}</td>
                                        <td>{{row.remark}}</td>
                                        <td>{{$t(row.isDefected ? 'YES' : 'NO')}}</td>
                                        <td><input type="text" class="form-control form-control-sm" v-model="row.buyingPrice" /></td>
                                        <td>{{row.lineValue}}</td>
                                        <td>{{row.residualValue}}</td>
                                        <td class="action-icon" v-if="!isIncreaseDisabled">
                                            <i class="las la-plus-circle" @click.stop="inbound_increase(row)"></i>
                                        </td>
                                        <td class="action-icon" v-if="!isDecreaseDisabled">
                                            <i class="las la-minus-circle" @click.stop="inbound_decrease(row)"></i>
                                        </td>
                                        <td class="action-icon" v-if="!isModifyPkgEntryDisabled">
                                            <i class="las la-pen" @click.stop="inbound_modify(row)"></i>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <!-- Buttons -->
                            <div class="row">
                                <div class="col-md-12">
                                    <button class="btn btn-primary btn-sm me-2"
                                            @click.stop="add_pkg()"
                                            :disabled="isAddPkgEntryDisabled">
                                        <i class="las la-plus-circle"></i> {{$t('inbound.operation.addPkgEntry')}}
                                    </button>
                                    <button class="btn btn-primary btn-sm me-2"
                                            @click.stop="printLabelPackageDetail()">
                                        <i class="las la-print"></i> {{$t('inbound.operation.printLabel')}}
                                    </button>
                                    <button class="btn btn-primary btn-sm me-2"
                                            @click.stop="calc_total()">
                                        <i class="las la-calculator"></i> {{$t('inbound.operation.calculateTotal')}}
                                    </button>
                                    <button class="btn btn-primary btn-sm"
                                            @click.stop="update_price_master()">
                                        <i class="las la-upload"></i> {{$t('inbound.operation.updatePriceMaster')}}
                                    </button>
                                </div>
                            </div>
                        </div>

                        <!-- Location Details -->
                        <div class="tab-pane fade" id="nav-profile" role="tabpanel" aria-labelledby="nav-profile-tab">
                            <table class="table table-striped table-hover align-middle">
                                <thead>
                                    <tr>
                                        <th></th>
                                        <th class="num pointer" @click.prevent.stop="sortLocation(row => row.lineItem,'lineItem')">
                                            {{$t('inbound.detail.item')}}
                                            <i v-if="sortedBy == 'lineItem'" :class="sortIcon()"></i>
                                        </th>
                                        <th class="num pointer" @click.prevent.stop="sortLocation(row => row.seqNo,'seqNo')">
                                            {{$t('inbound.detail.seq')}}
                                            <i v-if="sortedBy == 'seqNo'" :class="sortIcon()"></i>
                                        </th>
                                        <th class="num pointer" @click.prevent.stop="sortLocation(row => row.productCode,'productCode')">
                                            {{$t('inbound.detail.partNo')}}
                                            <i v-if="sortedBy == 'productCode'" :class="sortIcon()"></i>
                                        </th>
                                        <th class="num pointer" @click.prevent.stop="sortLocation(row => row.pid,'pid')">
                                            {{$t('inbound.detail.pid')}}
                                            <i v-if="sortedBy == 'pid'" :class="sortIcon()"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortLocation(row => row.externalPid,'externalPid')">
                                            {{$t('inbound.detail.externalPid')}}
                                            <i v-if="sortedBy == 'externalPid'" :class="sortIcon()"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortLocation(row => row.qty,'qty')">
                                            {{$t('inbound.detail.quantity')}}
                                            <i v-if="sortedBy == 'qty'" :class="sortIcon()"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortLocation(row => row.locationCode,'locationCode')">
                                            {{$t('inbound.detail.location')}}
                                            <i v-if="sortedBy == 'locationCode'" :class="sortIcon()"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortLocation(row => row.putawayBy,'putawayBy')">
                                            {{$t('inbound.detail.putawayBy')}}
                                            <i v-if="sortedBy == 'putawayBy'" :class="sortIcon()"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortLocation(row => row.putawayDate,'putawayDate')">
                                            {{$t('inbound.detail.putawayDate')}}
                                            <i v-if="sortedBy == 'putawayDate'" :class="sortIcon()"></i>
                                        </th>
                                        <th class="pointer" @click.prevent.stop="sortLocation(row => row.groupId,'groupID')">
                                            {{$t('inbound.detail.groupId')}}
                                            <i v-if="sortedBy == 'groupID'" :class="sortIcon()"></i>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr v-for="(row, index) in locationDetails" :key="index" @click.stop="onLocationDetailClick(row)">
                                        <td>
                                            <input type="checkbox" @click.stop v-model="locationDetailsSelected" :value="row" />
                                        </td>
                                        <td class="num">{{row.lineItem}}</td>
                                        <td class="num">{{row.seqNo}}</td>
                                        <td>{{row.productCode}}</td>
                                        <td>{{row.pid}}</td>
                                        <td>{{row.externalPID}}</td>
                                        <td>{{row.qty}}</td>
                                        <td>{{row.locationCode}}</td>
                                        <td>{{row.putawayBy}}</td>
                                        <td>{{row.putawayDate}}</td>
                                        <td>{{row.groupID}}</td>
                                    </tr>
                                </tbody>
                            </table>
                            <!-- Buttons -->
                            <div class="row">
                                <div class="col-md-12">
                                    <button class="btn btn-primary btn-sm me-2"
                                            @click.stop="printLabelLocationDetail()">
                                        <i class="las la-print"></i> {{$t('inbound.operation.printLabel')}}
                                    </button>
                                    <button class="btn btn-primary btn-sm me-2"
                                            @click.stop="printExternalPIDLabel()"
                                            :disabled="!printExternalDisabled">
                                        <i class="las la-print"></i>  {{ printing ? 'Printing label in progress, please wait' : 'Print ExternalPID Label' }}
                                    </button>
                                    <button class="btn btn-primary btn-sm me-2"
                                            :disabled="locationDetailsSelected.length == 0 || !locationDetailsSelected.every(d => d.putawayDate) || !isPartialPutaway"
                                            @click.stop="undoPutaway()">
                                        <i class="las la-stream"></i> {{$t('inbound.operation.undoPutaway')}}
                                    </button>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>


            <cancel-feature v-if="modal != null && modal.type == 'cancel'" :jobNo="modal.jobNo" @done="modal=null, backToList()" @close="modal=null" />
            <pdf-preview v-if="modal != null && modal.type == 'report'" :name="modal.pdfName" :pdf="modal.pdf" @close="modal = null"></pdf-preview>
            <modify-feature v-if="modal != null && (modal.type == 'modify' || modal.type == 'add')"
                            :controlCodes="modal.controlCodes"
                            :customerCode="modal.customerCode"
                            :supplierID="modal.supplierID"
                            :jobNo="modal.jobNo"
                            :modifyModel="modal.modifyModel"
                            @done="refresh(), modal = null" @close="modal=null" />
            <increase-feature v-if="modal != null && modal.type == 'increase'" :model="modal.row" :jobNo="modal.jobNo" @done="modal = null, refresh()" @close="modal=null" />

            <decrease-feature v-if="modal != null && modal.type == 'decrease'"
                              :header="modal.header"
                              :inJobNo="modal.inJobNo"
                              @done="refresh(), modal = null" @close="modal=null" />
            <print-label-feature v-if="modal != null && modal.type == 'printLabel'"
                                 :all="modal.allPids"
                                 :selected="modal.selectedPids"
                                 :labelType="modal.labelType"
                                 @done="refresh()"
                                 @close="modal=null" />
            <undo-putaway-feature v-if="modal != null && modal.type == 'undoPutaway'" :pids="modal.pids" @done="modal=null, refresh()" @close="modal=null" />
        </div>
    </div>

</template>
<script>
    import InboundHeader from './inbound/Header'
    import IncreaseFeature from './inbound/InboundDetailsIncrease'
    import DecreaseFeature from './inbound/InboundDetailsDecrease'
    import ModifyFeature from './inbound/InboundDetailsModify'
    import PrintLabelFeature from './print/PrintLabelPID'
    import PDFPreview from '@/widgets/PDFPreview'
    import { jsPDF } from "jspdf";
    import toBase64QR from '@/qrcode-generator'
    import { setIntervalAsync } from 'set-interval-async/dynamic'
    import { clearIntervalAsync } from 'set-interval-async'
    import CancelFeature from './inbound/Cancel'
    import { onRouteExit } from '@/route-exit-jobs'
    import UndoPutawayFeature from './inbound/UndoPutaway'


    import { defineComponent } from 'vue';
export default defineComponent({
        components: { InboundHeader, IncreaseFeature, DecreaseFeature, ModifyFeature, PrintLabelFeature, 'pdf-preview': PDFPreview, CancelFeature, UndoPutawayFeature },
        props: ['jobNo'],
        async created() {
            this.lockingMesage = this.$t('document.locking')
            const lockJob = async() => {
                const r = await this.$dataProvider.locks.tryLock(this.jobNo, "Inbound");
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
                //tab: 'package',
                modal: null,
                header: null,
                printing: false,
                errors: [],
                details: [],
                locationDetails: [],
                controlCodes: {},
                locked: false,
                locationDetailsSelected: [],
                packageDetailsSelected: [],
                sortedAsc: false,
                sortedBy: null,
                sortedAsc_details: false,
                sortedBy_details: null,
                lockingMesage: "",
                printExternalDisabled: false
            }
        },
        computed: {
            isAddPkgEntryDisabled() { return this.header && (['PartialPutaway', 'Completed', 'Cancelled'].includes(this.header.statusString) || this.header.transTypeString == 'ASN') },
            isDecreaseDisabled() { return this.header && ['Completed', 'Cancelled'].includes(this.header.statusString) },
            isIncreaseDisabled() { return this.header && ['Completed', 'Cancelled'].includes(this.header.statusString) },
            isModifyPkgEntryDisabled() { return this.header && (!['NewJob', 'PartialDownload', 'Downloaded'].includes(this.header.statusString) || this.header.transTypeString != 'ASN') },
            isPartialPutaway() { return this.header && this.header.statusString == "PartialPutaway" }
        },
        methods: {
            refresh() {
                this.$dataProvider.inbounds.getInbound(this.jobNo)
                    .then(resp => {
                        this.header = resp
                        return this.$dataProvider.inventory.getCustomerInventoryControlCodeMap(this.header.customerCode)
                    })
                    .then(resp => this.controlCodes = resp)
                this.$dataProvider.inbounds.getInboundDetails(this.jobNo)
                    .then(resp => {
                        this.details = resp
                        this.sortDetails(row => row.lineItem, 'lineItem', 'asc');
                    })
                this.packageDetailsSelected = []
                this.$dataProvider.storage.getStoragePutawayList(this.jobNo)
                    .then(resp => {
                        this.locationDetails = resp;
                        this.sortLocation(row => row.pid, 'pid', 'asc');
                        this.printExternalDisabled = this.locationDetails.every(obj => obj.externalPID);
                    })
                this.locationDetailsSelected = []
            },
            inbound_increase(row) {
                this.modal = { type: 'increase', row: row, jobNo: this.jobNo }
            },
            inbound_decrease(row) {
                this.modal = {
                    type: 'decrease',
                    header: row,
                    inJobNo: this.jobNo
                }
            },
            inbound_modify(row) {
                this.modal = { type: 'modify', controlCodes: this.controlCodes, customerCode: this.header.customerCode, supplierID: this.header.supplierID, jobNo: this.jobNo, modifyModel: row }
            },
            printLabelPackageDetail() {
                this.modal = {
                    type: 'printLabel',
                    allPids: this.locationDetails.map(ld => ld.pid),
                    selectedPids: this.locationDetails.filter(ld => this.packageDetailsSelected.find(pd => pd.lineItem == ld.lineItem)).map(ld => ld.pid),
                    labelType: 'QRPRINT_STORAGELABEL'
                }
            },
            printLabelLocationDetail() {
                this.modal = {
                    type: 'printLabel',
                    allPids: this.locationDetails.map(ld => ld.pid),
                    selectedPids: this.locationDetailsSelected.map(ld => ld.pid),
                    labelType: 'QRPRINT_STORAGELABEL'
                }
            },

            add_pkg() {
                this.modal = { type: 'add', controlCodes: this.controlCodes, customerCode: this.header.customerCode, supplierID: this.header.supplierID, jobNo: this.jobNo }
            },
            report(type) {
                this.modal = { type: 'report' }
                //todo move this to data provider
                this.$dataProvider.apiAxios.get(`inbounds/get${type}`,
                    {
                        params: {
                            jobNo: this.jobNo
                        },
                        responseType: 'blob',
                        headers: {
                            Accept: 'application/pdf'
                        }
                    }).then(resp => {
                        let pdf = resp.data;
                        let headerLine = resp.headers['Content-Disposition'] || resp.headers['content-disposition'];
                        const regExpFilename = /filename="?(?<filename>[^;"]*)/;
                        let pdfName = regExpFilename.exec(headerLine)?.groups?.filename ?? null;
                        this.modal = { type: 'report', pdfName, pdf }
                    }).catch((e) => {
                        this.$dataProvider.errorHandlers.reports(e)
                        this.modal = null
                    });
            },
            csvReport() {
                this.$dataProvider.inbounds.downloadIDTToCSV(this.jobNo)
            },
            calc_total() {
                this.errors = {}
                this.$dataProvider.storage.updateBuyingPrice({
                    inJobNo: this.jobNo,
                    currency: this.header.currency,
                    prices: this.details.map(d => ({ lineItem: d.lineItem, buyingPrice: d.buyingPrice }))
                })
                    .then(() => this.refresh())
                    .catch(e => {
                        if (e.response && e.response.data.errors) {
                            this.errors = e.response.data.errors
                        }
                    })
            },
            update_price_master() {
                this.errors = {}
                this.$dataProvider.priceMasters.updatePriceMasterInbound({
                    customerCode: this.header.customerCode,
                    jobNo: this.jobNo,
                    supplierID: this.header.supplierID,
                    currency: this.header.currency,
                    inboundDetailForPriceMasterDtos: this.details.map(d => ({ productCode: d.productCode, price: d.buyingPrice }))
                })
                    .then(() => this.refresh())
                    .catch(e => {
                        if (e.response && e.response.data.errors) {
                            this.errors = e.response.data.errors
                        }
                    })
            },
            saveInbound() {
                this.errors = {}
                return this.$dataProvider.inbounds.patchInbound(this.jobNo, this.header).then(() => {
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
            cancel() {
                this.modal = { type: 'cancel', jobNo: this.jobNo }
            },
            undoPutaway() {
                this.modal = { type: 'undoPutaway', pids: this.locationDetailsSelected.map(ld => ld.pid) }
            },
            backToList() {
                this.$router.push({ name: 'inbound' })
            },
            onPackageDetailClick(row) {
                var index = this.packageDetailsSelected.indexOf(row);
                if (index !== -1) {
                    this.packageDetailsSelected.splice(index, 1);
                }
                else {
                    this.packageDetailsSelected.push(row);
                }
            },
            onLocationDetailClick(row) {
                var index = this.locationDetailsSelected.indexOf(row);
                if (index !== -1) {
                    this.locationDetailsSelected.splice(index, 1);
                }
                else {
                    this.locationDetailsSelected.push(row);
                }
            },
            sortLocation(selector, by, mode) {
                if (mode) this.sortedAsc = mode == 'asc';
                else if (this.sortedBy != by) this.sortedAsc = true;
                else this.sortedAsc = !this.sortedAsc;
                this.sortedBy = by;
                this.locationDetails = this.locationDetails.sort((a, b) => {
                    if (typeof (selector(a)) === 'string' || selector(a) instanceof String) {
                        return selector(a).localeCompare(selector(b)) * (this.sortedAsc ? 1 : -1);
                    } else {
                        var x = selector(a); var y = selector(b); var d = this.sortedAsc ? -1 : 1;
                        return ((x < y) ? d : ((x > y) ? (-d) : 0));
                    }
                });
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
            sortIcon(table) {
                if (!table) {
                    return this.sortedAsc ? 'las la-sort-amount-up-alt' : 'las la-sort-amount-down';
                } else {
                    return this.sortedAsc_details ? 'las la-sort-amount-up-alt' : 'las la-sort-amount-down';
                }
            },
            async printExternalPIDLabel() {
                if (this.printing) {
                    return
                }
                try {
                    this.printing = true;
                    const doc = jsPDF({
                        format: [85, 100],
                        orientation: 'l'
                    })
                    const qrCodes = await this.$dataProvider.storage.getExternalQRStorageLabelsForInbound(this.jobNo)
                    for (let i = 0; i < this.locationDetails.length; i += 1) {
                        const row = this.locationDetails[i]
                        if (!row.externalPID) {
                            //return
                        }
                        const qr = await toBase64QR(qrCodes.find(c => c.lineItem == row.lineItem).code);
                        doc
                            .setFillColor('#000000').rect(50, 5, 45, 15, 'F')
                            .setTextColor('#FFFFFF')
                            .setFontSize(9).text("PID", 95, 5, { align: 'right', baseline: 'top' })
                            .setFontSize(13).text(' ' + (row.externalPID || ''), 95, 10, { align: 'right', baseline: 'top' })
                            .setTextColor('#000000')
                            .setFontSize(9)
                            .text("Product Code", 5, 48, { baseline: 'top' })
                            .text("Supplier Code", 5, 70, { baseline: 'top' })
                            .text("Quantity", 95, 20, { align: 'right', baseline: 'top' })
                            .text("Line number", 95, 35, { align: 'right', baseline: 'top' })
                            .text("ETA", 95, 48, { align: 'right', baseline: 'top' })
                            .setFontSize(13)
                            .text(row.productCode, 5, 51, { baseline: 'top' })
                            .text(this.header.supplierID, 5, 73, { baseline: 'top' })
                            .text(row.qty.toString(), 95, 23, { align: 'right', baseline: 'top' })
                            .text(row.lineItem.toString(), 95, 38, { align: 'right', baseline: 'top' })
                            .text(this.header.eta.substring(0, 10), 95, 51, { align: 'right', baseline: 'top' })
                            .addImage(qr, 'png', 5, 5, 40, 40);
                        if (i < this.locationDetails.length - 1) doc.addPage();

                    }
                    doc.save("external_labels_" + this.jobNo + ".pdf");
                } finally {
                    this.printing = false;
                }

            }
        }
    })
</script>
<style lang="scss">

    /*.tabs {
        .tab-content {
            max-height: 400px;
            overflow: auto;
        }
    }*/
</style>