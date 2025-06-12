<template>
    <div class="stock-transfer">
        <div class="toolbar-new">
            <div class="row mt-3 mb-2 ms-1">
                <div class="col-md-6 text-start pe-4 main-action-icons">
                    <div class="d-inline-block me-5 link-box" v-if="allowedSTFImportMethods && allowedSTFImportMethods.allowEKanbanImport">
                        <a href="#" @click.stop.prevent="importEkanban()" class="text-center">
                            <div><i class="las la-file-download"></i></div>
                            <div>{{$t('operation.general.importEkanban')}}</div>
                        </a>
                    </div>
                    <div class="d-inline-block me-5 link-box" v-if="allowedSTFImportMethods && allowedSTFImportMethods.allowEStockTransferImport">
                        <a href="#" @click.stop.prevent="importEstockTransfer()" class="text-center">
                            <div><i class="las la-file-download"></i></div>
                            <div>{{$t('operation.general.importEstockTransfer')}}</div>
                        </a>
                    </div>

                    <div class="d-inline-block me-5 link-box">
                        <a href="#" @click.stop.prevent="addNew()" class="text-center">
                            <div><i class="las la-file-medical"></i></div>
                            <div>{{$t('operation.general.new')}}</div>
                        </a>
                    </div>
                    <div class="d-inline-block link-box" v-if="stockTransferReversalAvailable">
                        <a href="#" @click.stop.prevent="stockTransferReversal()" class="text-center">
                            <div><i class="las la-reply"></i></div>
                            <div>{{$t('stockTransfer.operation.stockTransferReversal')}}</div>
                        </a>
                    </div>
                </div>
                <div class="col-md-6 text-end pe-4 main-action-icons">
                    <div class="d-inline-block link-box">
                        <a href="#" @click.stop.prevent="refresh()" class="text-center">
                            <div><i class="las la-redo-alt"></i></div>
                            <div>{{$t('operation.general.refresh')}}</div>
                        </a>
                    </div>
                </div>
            </div>
        </div>

        <div class="wrap">
            <dynamic-table :func="(params) => $dataProvider.stockTransfers.getStockTransferList(params)" 
                           ref="table" 
                           :columns="cols" 
                           :filters="defaultFilter" 
                           :multiple="false" 
                           :search="true"
                           @row-dblclick="modify($event)" 
                           :hoverActions="true" :actions="false">
                <template v-slot:statusString="{value}">
                    {{$t('stockTransferStatus.' + value)}}
                </template>
                <template v-slot:filter-statusString="{filter}">
                    <multiselect :options="statuses" label="label" v-model="status" @update:modelValue="filter({statuses : $event ?? []})" :can-clear="false" :showLabels="false"></multiselect>
                </template>
                <template v-slot:transferTypeString="{value}">
                    {{$t('stockTransferType.' + value)}}
                </template>
                <template v-slot:filter-transferTypeString="{filter}">
                    <multiselect :options="transferTypes" label="label" v-model="transferType" @update:modelValue="filter({transferType : $event})" :can-clear="false" :showLabels="false"></multiselect>
                </template>
                <template v-slot:createdDate="{value}">
                    <date-time :value="value" />
                </template>
                <template v-slot:filter-createdDate="{filter}">
                    <date-picker :date="createdFilter" @set="(date) => {filter({createdDate: date})}" />
                </template>
                <template v-slot:filter-customerCode="{filter}">
                    <multiselect-customer-code v-model="customers" :can-clear="false" @input="filter({customerCodes: $event})"></multiselect-customer-code>
                </template>
                <template v-slot:hoverActions="{row}">
                    <ul>
                        <li>
                            <button class="btn btn-sm btn-primary" @click="downloadEDT(row)"><i class="las la-file-invoice"></i> {{$t('stockTransferEDTButton')}}</button>
                        </li>
                        <li>
                            <button class="btn btn-sm btn-primary" @click="modify(row)"><i class="las la-pen"></i> {{$t('partsmaster.operation.modify')}}</button>
                        </li>
                        <li v-if="row.status != 8 && row.status != 10 && row.status != 0">
                            <!--<button class="btn btn-sm btn-primary" @click="complete(row)"><i class="las la-check-circle"></i> {{$t('partsmaster.operation.complete')}}</button>-->
                            <awaiting-button @proceed="(callback) => { complete(row).finally(callback) }"
                                             :btnText="'partsmaster.operation.complete'"
                                             :btnIcon="'las la-check-circle'"
                                             :btnClass="'btn-primary'">
                            </awaiting-button>
                        </li>
                        <li v-if="row.status != 8 && row.status != 10">
                            <!--button class="btn btn-sm btn-danger" @click="cancel(row)"><i class="las la-trash-alt"></i> {{$t('partsmaster.operation.cancel')}}</button-->
                            <awaiting-button @proceed="(callback) => { cancel(row).finally(callback) }"
                                             :btnText="'partsmaster.operation.cancel'"
                                             :btnIcon="'las la-trash-alt'"
                                             :btnClass="'btn-danger'">
                            </awaiting-button>
                        </li>
                    </ul>
                </template>
            </dynamic-table>
        </div>
        <details-feature v-if="modal != null && modal.type == 'details'" :type="modal.subtype" :productCodeMap="productCodeMap" :customerCode="customerCode" :productCode1="modal.subtype == 'new' ? null : modal.row.productCode1" :supplierID="modal.subtype == 'new' ? null : modal.row.supplierID" @close="modal = null, refresh()" />
        <import-ekanban-feature v-if="modal != null && modal.type == 'importEkanban'" @close="modal=null, refresh()" />
        <import-estocktransfer-feature v-if="modal != null && modal.type == 'importEstockTransfer'" @close="modal=null, refresh()" />
        <add-new-feature v-if="modal != null && modal.type == 'addNew'" @close="modal=null" @customer-selected="createNew" />
        <confirm v-if="modal != null && modal.type == 'complete-with-discrepancies'" @close="modal = null" @ok="goComplete(modal.jobNo)">
            {{ $t('stockTransfer.detail.discrepanciesConfirm') }}
        </confirm>
    </div>
</template>

<script>
    import Multiselect from '@vueform/multiselect'
    import DynamicTable from '@/widgets/Table.vue'
    import DateTime from '@/widgets/DateTime.vue'
    import RouteRefreshMixin from '@/mixins/routeRefreshMixin.js'
    import DetailsFeature from './partsmaster/Details.vue'
    import { toServer } from '@/service/date_converter.js'
    import ImportEkanbanFeature from './stocktransfer/ImportEkanban'
    import ImportEstocktransferFeature from './stocktransfer/ImportEStockTransfer'
    import AddNewFeature from './stocktransfer/AddNew'
    import bus from '@/event_bus.js'
    import MultiselectCustomerCode from '@/widgets/MultiselectCustomerCode.vue'
    import AwaitingButton from '@/widgets/AwaitingButton'
    import DatePicker from '@/widgets/DatePicker';

    import { defineComponent } from 'vue';
export default defineComponent({
        name: 'StockTransfer',
        mixins: [RouteRefreshMixin],
    components: { DynamicTable, DetailsFeature, DateTime, Multiselect, ImportEkanbanFeature, AddNewFeature, ImportEstocktransferFeature, MultiselectCustomerCode, AwaitingButton, DatePicker },
        created() {
            this.$dataProvider.settings.get().then(resp => this.settings = resp);
            this.$dataProvider.stockTransfers.getAllowedSTFImportMethods().then(resp => this.allowedSTFImportMethods = resp);
        },
        data() {
            return {
                modal: null,
                settings: null,
                allowedSTFImportMethods: null,
                createdFilter: null,
                customers: [],
                defaultFilter: {
                    orderBy: 'jobNo',
                    desc : true,
                    pageSize: 20,
                    pageNo: 1,
                    statuses: ["New", "Processing"]
                },
                status:["New", "Processing"],
                statuses: [
                    {
                        label: this.$t('stockTransferStatus.New'),
                        value: ["New"]
                    },
                    {
                        label: this.$t('stockTransferStatus.Processing'),
                        value: ["Processing"]
                    },
                    {
                        label: this.$t('stockTransferStatus.Outstanding'),
                        value: ["New", "Processing"]
                    },
                    {
                        label: this.$t('stockTransferStatus.Completed'),
                        value: ["Completed"]
                    },
                    {
                        label: this.$t('stockTransferStatus.Cancelled'),
                        value: ["Cancelled"]
                    },
                    {
                        label: this.$t('stockTransferStatus.All'),
                        value: []
                    }],
                transferType: null,
                transferTypes: [
                    {
                        label: this.$t('stockTransferType.Over90Days'),
                        value: "Over90Days"
                    },
                    {
                        label: this.$t('stockTransferType.Damaged'),
                        value: "Damaged"
                    },
                    {
                        label: this.$t('stockTransferType.EStockTransfer'),
                        value: "EStockTransfer"
                    },
                    {
                        label: this.$t('stockTransferType.All'),
                        value: null
                    }],
                cols: [
                    {
                        data: 'jobNo',
                        title: this.$t('jobNoHeader'),
                        sortable: true,
                        filter: true,
                        width: 170
                    },
                   {
                        data : 'customerCode',
                        abbrev: this.$t('customerCodeAbbrev'),
                        title : this.$t('customerCodeHeader'),
                        sortable: true,
                        filter: true,
                        width : 90
                   },
                    {
                        data: 'supplierName',
                        title: this.$t('supplierNameHeader'),
                        sortable: true,
                        filter: true,
                        width: 170
                    },
                    {
                        data: 'refNo',
                        title: this.$t('refNoHeader'),
                        sortable: true,
                        filter: true,
                        width: 170
                    },
                    {
                        data: 'transferTypeString',
                        title: this.$t('transferTypeHeader'),
                        sortable: true,
                        filter: true,
                        width: 170
                    },
                    {
                        data: 'createdDate',
                        title: this.$t('createdDateHeader'),
                        sortable: true,
                        filter: true,
                        width: 170
                    },
                    {
                        data: 'statusString',
                        title: this.$t('statusHeader'),
                        sortable: true,
                        filter: true,
                        width: 170
                    },
                    {
                        data: 'remark',
                        title: this.$t('remarkHeader'),
                        sortable: true,
                        filter: true,
                        width: 170
                    }
               ],
                stockTransferReversalAvailable: false
            }
        },
        activated(){
            bus.on('refresh', this.refresh)
        },
        deactivated(){
            bus.off('refresh', this.refresh)
        },
        methods: {
            toServer,
            refresh() {
                this.$refs.table.refresh()
                this.stockTransferReversalStatus()
            },
            modify(row) {
                this.$router.push({ name: 'stocktransfer_detail', params: { jobNo: row.jobNo } })
            },
            stockTransferReversal() {
                this.$router.push({ name: 'stockTransferReversal' })
            },
            importEkanban() {
                this.modal = { type: "importEkanban" };
            },
            importEstockTransfer() {
                this.modal = { type: "importEstockTransfer" };
            },
            addNew() {
                this.modal = { type: 'addNew' };
            },
            createNew(e) {
                this.$dataProvider.stockTransfers.createStockTransfer(e).then((stockTransferJob) => {
                    this.modify(stockTransferJob);
                });
            },
            complete(row) {
                return new Promise((resolve, reject) => {
                    if (row.transferType == 2) {
                        this.$dataProvider.eStockTransfers.hasAnyEStockTransferDiscrepancy(row.jobNo).then((discrepanciesPresent) => {
                            if (discrepanciesPresent) {
                                this.modal = { type: 'complete-with-discrepancies', jobNo: row.jobNo };
                            } else {
                                this.$dataProvider.stockTransfers.complete(row.jobNo).then(() => {
                                    this.refresh();
                                }).finally(() => { resolve('success'); });
                            }
                        });
                    } else {
                        this.$dataProvider.stockTransfers.complete(row.jobNo).then(() => {
                            this.refresh();
                        }).finally(() => { resolve('success'); });
                    }
                })
            },
            goComplete(jobNo) {
                return this.$dataProvider.stockTransfers.complete(jobNo).then(() => {
                    this.modal = null;
                    this.refresh();
                });
            },
            cancel(row) {
                return this.$dataProvider.stockTransfers.cancel(row.jobNo).then(() => {
                    this.refresh();
                });
            },
            downloadEDT(row) {
                this.$dataProvider.stockTransfers.downloadEDTToCSV(row.jobNo)
            },
            async stockTransferReversalStatus() {
                this.stockTransferReversalAvailable = await this.$dataProvider.stockTransferReversal.isActive();
            }
        },
        mounted() {
            this.stockTransferReversalStatus();
        }
    })
</script>

