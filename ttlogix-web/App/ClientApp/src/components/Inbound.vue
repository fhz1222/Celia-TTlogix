<template>
    <div class="inbound">

        <div class="toolbar-new">
            <div class="row pt-2 pb-2">
                <div class="col-md-6 ps-4 main-action-icons">
                    <div class="d-inline-block me-5 link-box">
                        <a href="#" @click.stop.prevent="importASN()" class="text-center">
                            <div><i class="las la-file-download"></i></div>
                            <div>{{$t('inbound.operation.importASN')}}</div>
                        </a>
                    </div>
                    <div class="d-inline-block me-5 link-box">
                        <a href="#" @click.stop.prevent="importFile()" class="text-center">
                            <div><i class="las la-file-download"></i></div>
                            <div>{{$t('inbound.operation.importFile.button')}}</div>
                        </a>
                    </div>
                    <div class="d-inline-block me-5 link-box">
                        <a href="#" @click.stop.prevent="createManual()" class="text-center">
                            <div><i class="las la-file-medical"></i></div>
                            <div>{{$t('inbound.operation.manual')}}</div>
                        </a>
                    </div>
                    <div class="d-inline-block me-5 link-box">
                        <a href="#" @click.stop.prevent="report()" class="text-center">
                            <div><i class="las la-file-alt"></i></div>
                            <div>{{$t('inbound.operation.report')}}</div>
                        </a>
                    </div>
                    <div class="d-inline-block link-box" v-if="inboundReversalAvailable">
                        <a href="#" @click.stop.prevent="inboundReversal()" class="text-center">
                            <div><i class="las la-reply"></i></div>
                            <div>{{$t('inbound.operation.inboundReversal')}}</div>
                        </a>
                    </div>
                </div>
                <div class="col-md-6 text-end pe-5 main-action-icons">
                    <div class="d-inline-block link-box">
                        <a href="#" @click.stop.prevent="refresh()" class="text-center">
                            <div><i class="las la-redo-alt"></i></div>
                            <div>{{$t('operation.general.refresh')}}</div>
                        </a>
                    </div>
                </div>
            </div>
        </div>
        <h5 class="text-center mt-2 blue-text weight-normal">
            Use the <strong>%</strong> symbol in Remark field to search for a substring instead of exact value.
        </h5>
        <div class="wrap">
            <dynamic-table :func="(params) => $dataProvider.inbounds.getInbounds(params)" ref="table" :columns="cols" :filters="defaultFilter" :multiple="true" :search="true" @row-dblclick="modify($event.jobNo)" :hoverActions="true" :actions="false">

                <template v-slot:statusString="{value}">
                    {{$t('inbound.status.' + value)}}
                </template>
                <template v-slot:transTypeString="{value}">
                    {{$t('inbound.transType.' + value)}}
                </template>
                <template v-slot:receivedDate="{value}">
                    <date :value="value" />
                </template>
                <template v-slot:filter-receivedDate="{filter}">
                    <date-picker :date="receivedFilter" @set="(date) => {filter({receivedDate: date})}" />
                </template>
                <template v-slot:filter-statusString="{filter}">
                    <multiselect :options="statuses" label="label" v-model="status" @update:modelValue="filter({statuses : $event ?? []})" :can-clear="false" :showLabels="false"></multiselect>
                </template>
                <template v-slot:filter-transTypeString="{filter}">
                    <multiselect :options="transTypes" label="label" v-model="transType" @update:modelValue="filter({transType : $event})" :can-clear="false" :showLabels="false"></multiselect>
                </template>
                <template v-slot:filter-customerCode="{filter}">
                    <multiselect-customer-code v-model="customers" :can-clear="false" @input="filter({customerCodes: $event})"></multiselect-customer-code>
                </template>
                <template v-slot:hoverActions="{row}">
                    <ul>
                        <li>
                            <button class="btn btn-sm btn-primary" @click="modify(row.jobNo)"><i class="las la-pen"></i> {{$t('inbound.operation.modify')}}</button>
                        </li>
                        <li>
                            <button class="btn btn-sm btn-danger" v-if="row.statusString == 'NewJob'" @click="cancel(row.jobNo)"><i class="las la-trash-alt"></i> {{$t('inbound.operation.cancel')}}</button>
                        </li>
                    </ul>
                </template>
            </dynamic-table>
        </div>

        <cancel-feature v-if="modal != null && modal.type == 'cancel'" :jobNo="modal.jobNo" @done="modal=null, refresh()" @close="modal=null" />
        <import-asn-feature v-if="modal != null && modal.type == 'asn'" @done="modal=null, refresh(), modify($event)" @close="modal=null" />
        <import-file-feature v-if="modal != null && modal.type == 'file'" @done="modal=null, refresh(), modifyAll($event)" @close="modal=null" />
        <manual-feature v-if="modal != null && modal.type == 'manual'" @done="modal = null, refresh(), modify($event)" @close="modal=null" />
    </div>
</template>

<script>
    import Multiselect from '@vueform/multiselect'
    import DynamicTable from '@/widgets/Table.vue'
    import Date from '@/widgets/Date'
    import CancelFeature from './inbound/Cancel'
    import ManualFeature from './inbound/Manual'
    import ImportAsnFeature from './inbound/ImportASN'
    import ImportFileFeature from './inbound/ImportFile'
    import { toServer } from '@/service/date_converter.js'
    import RouteRefreshMixin from '@/mixins/routeRefreshMixin.js'
    import bus from '@/event_bus.js'
    import MultiselectCustomerCode from '@/widgets/MultiselectCustomerCode.vue'
    import DatePicker from '@/widgets/DatePicker';

    import { defineComponent } from 'vue';
export default defineComponent({
        name: 'Inbound',
        mixins: [RouteRefreshMixin],
        components: { DynamicTable, Date, CancelFeature, ManualFeature, ImportAsnFeature, Multiselect, ImportFileFeature, MultiselectCustomerCode, DatePicker },
        data() {
            return {
                receivedFilter: null,
                modal: null,
                customers: [],
                status: ["NewJob", "PartialDownload", "Downloaded", "PartialPutaway"],
                transType: null,
                defaultFilter: {
                    orderBy : 'jobNo',
                    desc : true,
                    pageSize: 20,
                    pageNo : 1,
                    statuses: ["NewJob", "PartialDownload", "Downloaded", "PartialPutaway"]
                },
                statuses: [{
                    label: this.$t('inbound.status.NewJob'),
                    value: ["NewJob"]
                },
                {
                    label: this.$t('inbound.status.PartialDownload'),
                    value: ["PartialDownload"]
                },
                {
                    label: this.$t('inbound.status.Downloaded'),
                    value: ["Downloaded"]
                },
                {
                    label: this.$t('inbound.status.PartialPutaway'),
                    value: ["PartialPutaway"]
                },
                {
                    label: this.$t('inbound.status.Outstanding'),
                    value: ["NewJob", "PartialDownload", "Downloaded", "PartialPutaway"]
                },
                {
                    label: this.$t('inbound.status.Completed'),
                    value: ["Completed"]
                },
                {
                    label: this.$t('inbound.status.Cancelled'),
                    value: ["Cancelled"]
                },
                {
                    label: this.$t('inbound.status.All'),
                    value: []
                }],
                transTypes: [
                    {
                        label: this.$t('inbound.transType.ManualEntry'),
                        value: "ManualEntry"
                    },
                    {
                        label: this.$t('inbound.transType.ASN'),
                        value: "ASN"
                    },
                    {
                        label: this.$t('inbound.transType.Return'),
                        value: "Return"
                    },
                    {
                        label: this.$t('inbound.transType.Excess'),
                        value: "Excess"
                    },
                    {
                        label: this.$t('inbound.transType.ScannerManual'),
                        value: "ScannerManual"
                    },
                    {
                        label: this.$t('inbound.transType.All'),
                        value: null
                    }
                ],
                cols: [
                    {
                        data: 'jobNo',
                        title: this.$t('inbound.columns.jobno'),
                        sortable: true,
                        filter: true,
                        width: 170
                    },
                   {
                       data : 'customerCode',
                        abbrev: this.$t('inbound.columns.customerAbbrev'),
                       title : this.$t('inbound.columns.customer'),
                       sortable: true,
                       filter: true,
                       width : 90
                   },
                   {
                       data : 'refNo',
                       title : this.$t('inbound.columns.ref'),
                       sortable: true,
                       filter: true,
                       width : 180
                    },
                    {
                        data: 'supplierName',
                        title: this.$t('inbound.columns.supplier'),
                        filter: true,
                        sortable: true,
                        width: 145
                    },
                   {
                       data : 'asnNumber',
                       title : this.$t('inbound.columns.asn'),
                       sortable: true,
                       filter: true,
                       width : 145
                   },
                   {
                       data : 'containerNo',
                       title: this.$t('inbound.columns.container'),
                       sortable: true,
                       filter: true,
                       width: 145
                    },
                    {
                        data: 'transTypeString',
                        title: this.$t('inbound.columns.type'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'receivedDate',
                        title: this.$t('inbound.columns.received'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                   {
                        data: 'statusString',
                        title: this.$t('inbound.columns.status'),
                        sortable: true,
                        filter: true,
                        width: 145
                   },
                   {
                        data: 'remark',
                        title: this.$t('inbound.columns.remark'),
                        sortable: true,
                        filter: true,
                        width: 400
                   }
                ],
                inboundReversalAvailable: false
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
                this.inboundReversalStatus()
            },
            cancel(jobNo) {
                this.modal = { type: 'cancel', jobNo: jobNo }
            },
            createManual() {
                this.modal = { type: 'manual' }
            },
            importASN() {
                this.modal = { type: 'asn' }
            },
            importFile() {
                this.modal = { type: 'file' }
            },
            modify(jobNo) {
                this.$router.push({name : 'inbound_detail', params: {jobNo}})
            },
            inboundReversal() {
                this.$router.push({ name: 'inboundReversal'})
            },
            modifyAll(arr) {
                arr.forEach(jobNo => this.modify(jobNo))
            },
            report() {
                this.$dataProvider.inbounds.getOutstandingInboundsXlsReport()
            },
            async inboundReversalStatus() {
                this.inboundReversalAvailable = await this.$dataProvider.inboundReversal.isActive();
            }
        },
        mounted() {
            this.inboundReversalStatus();
        }
    })
</script>

