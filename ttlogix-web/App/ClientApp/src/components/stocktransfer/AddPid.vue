<template>
    <modal containerClass="modal-add-detail-loading">
        <template name="header" v-slot:header>
            <span>{{$t('stockTransfer.addPid.addPIDs')}}</span>
        </template>
        <template name="body" v-slot:body>

            <!-- Filter -->
            <alert style="max-width:300px;" v-if="alert" @close="alert = null" :text="alert" />

            <div class="row">
                <div class="col-md-12">
                    <!--<div><strong>Filter</strong></div>-->
                    <div class="card">
                        <div class="card-body">

                            <div class="row mt-2">
                                <div class="col-md-1 pt-1 text-end no-right-padding">
                                    {{$t('stockTransfer.addPid.customer')}}:
                                </div>
                                <div class="col-md-1 pt-1 multiselect-as-select">
                                    {{customerCode}}
                                </div>

                                <div class="col-md-1 pt-1 text-end no-right-padding">
                                    {{$t('stockTransfer.addPid.supplier')}}
                                </div>
                                <div class="col-md-3 multiselect-as-select">
                                    <select-supplier v-model="supplierID"
                                                     :customerCode="customerCode"
                                                     :stockTransferJobNo="jobNo"
                                                     :disabled="customerCode == null"
                                                     :key="supplierKey">
                                    </select-supplier>
                                </div>

                                <div class="col-md-1 pt-1 text-end no-right-padding">
                                    {{$t('stockTransfer.addPid.inboundJob')}}
                                </div>
                                <div class="col-md-3 multiselect-as-select">
                                    <select-inbound-job v-model="inboundJobPID"
                                                        :customerCode="customerCode"
                                                        :supplierID="supplierID"
                                                        :disabled="customerCode == null || supplierID == null"
                                                        :key="inboundJobKey">
                                    </select-inbound-job>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <!-- Available items list -->
            <div class="row mt-3">
                <div class="col-md-12">
                    <div><strong>{{$t('stockTransfer.addPid.available')}}</strong></div>
                    <div class="card">
                        <div class="card-body">

                            <div class="scrollable-20">
                                <!-- Table -->
                                <table class="table table-sm table-striped table-hover align-middle selectable">
                                    <thead class="sticky-header">
                                        <tr>
                                            <th class="pointer" @click.prevent.stop="sortAvailable(row => row.pid,'pid')">
                                                {{$t('stockTransfer.addPid.columns.pid')}}
                                                <i v-if="sortedByA == 'pid'" :class="sortIcon()"></i>
                                            </th>
                                            <th class="text-end pointer" @click.prevent.stop="sortAvailable(row => row.productCode,'productCode')">
                                                {{$t('stockTransfer.addPid.columns.partNo')}}
                                                <i v-if="sortedByA == 'productCode'" :class="sortIcon()"></i>
                                            </th>
                                            <th class="text-end pointer" @click.prevent.stop="sortAvailable(row => row.qty,'qty')">
                                                {{$t('stockTransfer.addPid.columns.qty')}}
                                                <i v-if="sortedByA == 'qty'" :class="sortIcon()"></i>
                                            </th>
                                            <th class="pointer" @click.prevent.stop="sortAvailable(row => row.inboundDate,'inboundDate')">
                                                {{$t('stockTransfer.addPid.columns.inDate')}}
                                                <i v-if="sortedByA == 'inboundDate'" :class="sortIcon()"></i>
                                            </th>
                                            <th class="pointer" @click.prevent.stop="sortAvailable(row => row.locationCode,'locationCode')">
                                                {{$t('stockTransfer.addPid.columns.locationId')}}
                                                <i v-if="sortedByA == 'locationCode'" :class="sortIcon()"></i>
                                            </th>
                                            <th class="pointer" @click.prevent.stop="sortAvailable(row => row.whsCode,'whsCode')">
                                                {{$t('stockTransfer.addPid.columns.whs')}}
                                                <i v-if="sortedByA == 'whsCode'" :class="sortIcon()"></i>
                                            </th>
                                            <th class="text-end pointer" @click.prevent.stop="sortAvailable(row => row.daysInStock,'daysInStock')">
                                                {{$t('stockTransfer.addPid.columns.dayInStock')}}
                                                <i v-if="sortedByA == 'daysInStock'" :class="sortIcon()"></i>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="row in availableDetailsList"
                                            :key="row.pid"
                                            @click.stop="onAvailableListClick(row, $event)"
                                            :class="getSelectedClass(availableDetailsListSelected,row)">
                                            <td>{{row.pid}}</td>
                                            <td class="text-end">{{row.productCode}}</td>
                                            <td class="text-end">{{row.qty}}</td>
                                            <td>{{row.inboundDate}}</td>
                                            <td>{{row.locationCode}}</td>
                                            <td>{{row.whsCode}}</td>
                                            <td class="text-end">{{row.daysInStock}}</td>
                                        </tr>
                                    </tbody>
                                </table>

                                <!-- Loading -->
                                <div v-if="loading" class="mt-5 mb-5 text-center">
                                    <div class="spinner-border text-primary" role="status">
                                        <span class="visually-hidden">{{$t('generic.loading')}}</span>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Action buttons -->
            <div class="row mt-3">
                <div class="col-md-3">&nbsp;</div>
                <div class="col-md-6 text-center">
                    <button class="btn btn-sm me-4 btn-primary btn-extra-width" @click.stop="allocate()">
                        <i class="las la-arrow-down"></i>
                    </button>
                    <button class="btn btn-sm btn-primary btn-extra-width" @click.stop="unallocate()">
                        <i class="las la-arrow-up"></i>
                    </button>
                </div>
                <!--<div class="col-md-3 text-end">
            <button class="btn btn-sm btn-primary"><i class="las la-save"></i> Finish</button>
        </div>-->
            </div>

            <!-- Stock transfer list -->
            <div class="row mt-2">
                <div class="col-md-12">
                    <div><strong>{{$t('stockTransfer.addPid.selected')}}</strong></div>
                    <div class="card">
                        <div class="card-body">

                            <div class="scrollable-20">
                                <!-- Table -->
                                <table class="table table-sm table-striped table-hover align-middle selectable">
                                    <thead class="sticky-header">
                                        <tr>
                                            <th class="pointer" @click.prevent.stop="sortStock(row => row.pid,'pid')">
                                                {{$t('stockTransfer.addPid.columns.pid')}}
                                                <i v-if="sortedByS == 'pid'" :class="sortIcon('stock')"></i>
                                            </th>
                                            <th class="text-end pointer" @click.prevent.stop="sortStock(row => row.productCode1,'productCode1')">
                                                {{$t('stockTransfer.addPid.columns.partNo')}}
                                                <i v-if="sortedByS == 'productCode1'" :class="sortIcon('stock')"></i>
                                            </th>
                                            <th class="text-end pointer" @click.prevent.stop="sortStock(row => row.qty,'qty')">
                                                {{$t('stockTransfer.addPid.columns.qty')}}
                                                <i v-if="sortedByS == 'qty'" :class="sortIcon('stock')"></i>
                                            </th>
                                            <th class="pointer" @click.prevent.stop="sortStock(row => row.inboundDate,'inboundDate')">
                                                {{$t('stockTransfer.addPid.columns.inDate')}}
                                                <i v-if="sortedByS == 'inboundDate'" :class="sortIcon('stock')"></i>
                                            </th>
                                            <th class="pointer" @click.prevent.stop="sortStock(row => row.locationCode,'locationCode')">
                                                {{$t('stockTransfer.addPid.columns.locationId')}}
                                                <i v-if="sortedByS == 'locationCode'" :class="sortIcon('stock')"></i>
                                            </th>
                                            <th class="pointer" @click.prevent.stop="sortStock(row => row.whsCode,'whsCode')">
                                                {{$t('stockTransfer.addPid.columns.whs')}}
                                                <i v-if="sortedByS == 'whsCode'" :class="sortIcon('stock')"></i>
                                            </th>
                                            <th class="text-end pointer" @click.prevent.stop="sortStock(row => row.daysInStock,'daysInStock')">
                                                {{$t('stockTransfer.addPid.columns.dayInStock')}}
                                                <i v-if="sortedByS == 'daysInStock'" :class="sortIcon('stock')"></i>
                                            </th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="row in stockTransferDetailsList"
                                            :key="row.pid"
                                            @click.stop="onStockTransferListClick(row, $event)"
                                            :class="getSelectedClass(stockTransferDetailsListSelected,row)">
                                            <td>{{row.pid}}</td>
                                            <td class="text-end">{{row.productCode1}}</td>
                                            <td class="text-end">{{row.qty}}</td>
                                            <td>{{row.inboundDate}}</td>
                                            <td>{{row.locationCode}}</td>
                                            <td>{{row.whsCode}}</td>
                                            <td class="text-end">{{row.daysInStock}}</td>
                                        </tr>
                                    </tbody>
                                </table>

                                <!-- Loading -->
                                <div v-if="loading" class="mt-5 mb-5 text-center">
                                    <div class="spinner-border text-primary" role="status">
                                        <span class="visually-hidden">{{$t('generic.loading')}}</span>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <!--<button class="btn btn-sm btn-primary me-2" type="button" @click.stop="process()" :disabled="processing">
            <i :class="['las', newUser ? 'la-plus-circle' : 'la-save']"></i> {{newUser ? $t('operation.general.add') : $t('operation.general.save')}}
        </button>-->
            <!--<button class="btn btn-sm btn-primary me-2" type="button" @click.stop="process()">
            <i class="las la-file-download"></i> Import
        </button>-->
            <button class="btn btn-sm btn-primary" type="button" @click.stop="$emit('close')">
                <i class="las la-times"></i> {{$t('operation.general.close')}}
            </button>
        </template>
    </modal>
</template>
<script>
    /*import DynamicTable from '@/widgets/Table.vue'
    import DateTime from '@/widgets/DateTime.vue'*/
    import { toServer } from '@/service/date_converter.js'
    import SelectSupplier from '@/widgets/SelectSupplier'
    import SelectInboundJob from '@/widgets/SelectInboundJob'
    import { defineComponent } from 'vue';
export default defineComponent({
        props: {

            transType: {
                default: 0
            },
            jobNo: {
                default: null
            },
            customerCode: {
                default: null
            },
            supplier: {
                default: null
            }
        },
        created() {
            this.load();
        },
        components: { SelectSupplier, SelectInboundJob/*DynamicTable, DateTime*/ },
        data() {
            let manualType = 1
            switch (this.transType) {
                case 0:
                    manualType = 1
                    break;
                case 3:
                    manualType = 1
                    break;
                case 4:
                    manualType = 4
                    break;
            }
            return {
                whsList: [],
                availableDetailsList: [],
                stockTransferDetailsList: [],
                availableDetailsListSelected: [],
                stockTransferDetailsListSelected: [],
                etaFilter: null,
                loading: false,
                supplierID: this.supplier,
                inboundJobPID: null,
                supplierKey: 0,
                inboundJobKey: 0,
                alert: null,
                model: {
                    customerCode: null,
                    whsCode: this.$auth.user().whsCode,
                    newWhsCode: null,
                    manualType: manualType,
                    refNo: null,
                    etd: new Date(),
                    transType: this.transType,
                    remark: null,
                    status: 0
                },
                defaultFilter: {
                    UserWHSCode: this.$auth.user().whsCode
                },
                errors: {},
                sortedAscA: false,
                sortedAscS: false,
                sortedByA: "",
                sortedByS: ""
            }
        },
        methods: {
            toServer,

            load() {
                this.refresh();
            },
            loadStockTransferDetails() {
                this.stockTransferDetailsList = []
                this.loading = true;
                return this.$dataProvider.stockTransfers.getStockTransferDetails(this.jobNo)
                    .then((stfList) => { this.stockTransferDetailsList = stfList })
                    .finally(() => { this.loading = false })
            },
            loadAvailableList() {
                this.availableDetailsList = []
                if (this.supplierID) {
                    this.loading = true;
                    return this.$dataProvider.storage.getSTFStorageDetailList(this.jobNo, this.inboundJobPID, this.supplierID)
                        .then((available) => { this.availableDetailsList = available })
                        .finally(() => { this.loading = false })
                } else {
                    return Promise.resolve();
                }
            },
            refresh() {
                this.loadStockTransferDetails().then(() => {
                    this.loadAvailableList();
                })
            },
            process() {
                if (this.$refs.table.selection.length == 0) {
                    this.$notify({
                        title: this.$t('error.ErrorOccured'),
                        text: this.$t('outbound.error.select_ekanban'),
                        type: 'error',
                        group: 'temporary'
                    })
                } else {
                    let jobs = []
                    this.$refs.table.selection.forEach(o => jobs.push(o.orderNo))
                    this.processJobs(jobs)
                }
            },
            processJobs(jobs) {
                this.$dataProvider.stockTransfers.importEKanbanEUCPartMulti(jobs).then(resp => {
                    this.$emit('close', resp)
                })
            },
            onAvailableListClick(row, evt) {
                document.getSelection().removeAllRanges();
                var index = this.availableDetailsListSelected.indexOf(row);
                if (index !== -1) {
                    this.availableDetailsListSelected.splice(index, 1);
                }
                else {
                    if (evt && evt.shiftKey && this.availableDetailsListSelected.length > 0) {
                        let i = this.availableDetailsList.indexOf(row)
                        let j = this.availableDetailsList.indexOf(this.availableDetailsListSelected[this.availableDetailsListSelected.length - 1])
                        this.availableDetailsListSelected = []
                        if (j < i) [j, i] = [i, j]
                        for (let k = i; k <= j; k++)
                            this.availableDetailsListSelected.push(this.availableDetailsList[k])
                    }
                    else {
                        this.availableDetailsListSelected.push(row);
                    }
                }
            },
            onStockTransferListClick(row, evt) {
                document.getSelection().removeAllRanges();
                var index = this.stockTransferDetailsListSelected.indexOf(row);
                if (index !== -1) {
                    this.stockTransferDetailsListSelected.splice(index, 1);
                }
                else {
                    if (evt && evt.shiftKey && this.stockTransferDetailsListSelected.length > 0) {
                        let i = this.stockTransferDetailsList.indexOf(row)
                        let j = this.stockTransferDetailsList.indexOf(this.stockTransferDetailsListSelected[this.stockTransferDetailsListSelected.length - 1])
                        this.stockTransferDetailsListSelected = []
                        if (j < i) [j, i] = [i, j]
                        for (let k = i; k <= j; k++)
                            this.stockTransferDetailsListSelected.push(this.stockTransferDetailsList[k])
                    }
                    else {
                        this.stockTransferDetailsListSelected.push(row);
                    }
                }
            },
            getSelectedClass(array, row) {
                return array.indexOf(row) > -1 ? 'selected' : '';
            },
            allocate() {
                if (this.availableDetailsListSelected.length <= 0) {
                    this.alert = this.$t('stockTransfer.addPid.SelectRow')
                } else {
                    var pids = [];
                    this.availableDetailsListSelected.forEach(o => pids.push(o.pid));
                    var dto = { jobNo: this.jobNo, PIDs: pids };
                    this.$dataProvider.stockTransfers.addStockTransferDetailByPID(dto).then(() => {
                        this.availableDetailsList = [];
                        this.stockTransferDetailsList = [];
                        this.availableDetailsListSelected = [];
                        this.refresh();
                    });
                }
            },
            unallocate() {
                if (this.stockTransferDetailsListSelected.length <= 0) {
                    this.alert = this.$t('stockTransfer.addPid.SelectRow')
                } else {
                    var pids = [];
                    this.stockTransferDetailsListSelected.forEach(o => pids.push(o.pid));
                    var dto = { jobNo: this.jobNo, PIDs: pids };
                    this.$dataProvider.stockTransfers.deleteStockTransferDetailByPID(dto).then(() => {
                        this.availableDetailsList = [];
                        this.stockTransferDetailsList = [];
                        this.stockTransferDetailsListSelected = [];
                        this.refresh();
                    });
                }
            },
            sortAvailable(selector, by, mode) {
                if (mode) this.sortedAscA = mode == 'asc';
                else if (this.sortedByA != by) this.sortedAscA = true;
                else this.sortedAscA = !this.sortedAscA;
                this.sortedByA = by;
                this.availableDetailsList = this.availableDetailsList.sort((a, b) => {
                    if (typeof (selector(a)) === 'string' || selector(a) instanceof String) {
                        return selector(a).localeCompare(selector(b)) * (this.sortedAscA ? 1 : -1);
                    } else {
                        var x = selector(a); var y = selector(b); var d = this.sortedAscA ? -1 : 1;
                        return ((x < y) ? d : ((x > y) ? (-d) : 0));
                    }
                });
            },
            sortStock(selector, by, mode) {
                if (mode) this.sortedAscS = mode == 'asc';
                else if (this.sortedByS != by) this.sortedAscS = true;
                else this.sortedAscS = !this.sortedAscS;
                this.sortedByS = by;
                this.stockTransferDetailsList = this.stockTransferDetailsList.sort((a, b) => {
                    if (typeof (selector(a)) === 'string' || selector(a) instanceof String) {
                        return selector(a).localeCompare(selector(b)) * (this.sortedAscS ? 1 : -1);
                    } else {
                        var x = selector(a); var y = selector(b); var d = this.sortedAscS ? -1 : 1;
                        return ((x < y) ? d : ((x > y) ? (-d) : 0));
                    }
                });
            },
            sortIcon(table) {
                if (!table) {
                    return this.sortedAscA ? 'las la-sort-amount-up-alt' : 'las la-sort-amount-down';
                } else {
                    return this.sortedAscS ? 'las la-sort-amount-up-alt' : 'las la-sort-amount-down';
                }
            }
        },
        watch: {
            'customerCode'() {
                this.availableDetailsList = [];
                this.stockTransferDetailsList = [];
                this.supplierID = null;
                this.inboundJobPID = null;
                this.supplierKey += 1;
            },
            'supplierID'() {
                this.availableDetailsList = [];
                //this.stockTransferDetailsList = [];
                this.inboundJobPID = null;
                this.inboundJobKey += 1;
                this.loadAvailableList();
            },
            'inboundJobPID'() {
                this.loadAvailableList();
            }
        }
    })
</script>
<style lang="scss">
    .modal-add-detail-loading {
        .modal-container

    {
        width: 80vw;
    }
    }
</style>