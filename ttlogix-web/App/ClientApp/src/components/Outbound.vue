<template>
    <div class="outbound">

        <div class="toolbar-new">
            <div class="row pt-2 pb-2">
                <div class="col-md-6 ps-4 main-action-icons">
                    <div class="d-inline-block me-5 link-box" v-if="allowedOutboundCreationMethods && allowedOutboundCreationMethods.allowEKanbanImport">
                        <a href="#" @click.stop.prevent="importEkanban()" class="text-center">
                            <div><i class="las la-file-alt"></i></div>
                            <div>{{$t('outbound.operation.ekanban')}}</div>
                        </a>
                    </div>
                    <div class="d-inline-block me-5 link-box" v-if="allowedOutboundCreationMethods && allowedOutboundCreationMethods.allowManual">
                        <a href="#" @click.stop.prevent="createManual()" class="text-center">
                            <div><i class="las la-file-medical"></i></div>
                            <div>{{$t('outbound.operation.manual')}}</div>
                        </a>
                    </div>
                    <div class="d-inline-block me-5 link-box">
                        <a href="#" @click.stop.prevent="createReturn()" class="text-center">
                            <div><i class="las la-reply"></i></div>
                            <div>{{$t('outbound.operation.return')}}</div>
                        </a>
                    </div>
                    <div class="d-inline-block me-5 link-box">
                        <a href="#" @click.stop.prevent="createTransfer()" class="text-center">
                            <div><i class="las la-dolly"></i></div>
                            <div>{{$t('outbound.operation.whs')}}</div>
                        </a>
                    </div>
                    <div class="d-inline-block link-box">
                        <a href="#" @click.stop.prevent="createLoading()" class="text-center">
                            <div><i class="las la-file-medical"></i></div>
                            <div>{{$t('outbound.operation.create_loading')}}</div>
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
            <dynamic-table :func="(params) => $dataProvider.outbounds.getOutbounds(params)" ref="table" :columns="cols" :filters="defaultFilter" :multiple="true" :search="true" @row-dblclick="modify($event.jobNo)" :hoverActions="true" :actions="false">

                <template v-slot:transType="{value}">
                    {{ transType[value] || value }}
                </template>
                <template v-slot:status="{value}">
                    {{outboundStatus[value]}}
                </template>
                <template v-slot:createdDate="{value}">
                    <date :value="value" />
                </template>
                <template v-slot:dispatchedDate="{value}">
                    <date :value="value" />
                </template>
                <template v-slot:filter-createdDate="{filter}">
                    <date-picker :date="createdFilter" @set="(date) => {filter({createdDate: date})}" />
                </template>
                <template v-slot:filter-dispatchedDate="{filter}">
                    <date-picker :date="dispatchedFilter" @set="(date) => {filter({dispatchedFilter: date})}" />
                </template>
                <template v-slot:filter-status="{filter}">
                    <multiselect :can-clear="false" :options="statuses" label="label" v-model="status" @update:modelValue="filter({statuses : $event ?? []})" :showLabels="false"></multiselect>
                </template>
                <template v-slot:filter-remark="{filter}">
                    <input type="text" placeholder="..." class="filter" @keyup.enter.stop="remarkFilter($event.target.value, filter)" />
                </template>
                <template v-slot:filter-transType="{filter}">
                    <multiselect :can-clear="false" placeholder="..." :options="transTypeOptions" label="label" v-model="transTypeFilter" @update:modelValue="filter({transType: $event})" :showLabels="false"></multiselect>
                </template>
                <template v-slot:filter-customerCode="{filter}">
                    <multiselect-customer-code v-model="customers" :can-clear="false" @input="filter({customerCodes: $event})"></multiselect-customer-code>
                </template>
                <!--<template v-slot:hoverActions="{row}">
                <ul>
                    <li>
                        <button class="button" @click="modify(row.jobNo)"><span v-if="row.status < 8">{{$t('outbound.operation.modify')}}</span><span v-else>{{$t('outbound.operation.view')}}</span></button>
                    </li>
                    <li v-if="row.status == 4">
                        <button class="button" @click="cargoOut(row)">{{$t('outbound.operation.cargoOut')}}</button>
                    </li>
                    <li v-if="row.status != 8 && row.status != 9">
                        <button class="button danger" @click="cancel(row)">{{$t('outbound.operation.cancel')}}</button>
                    </li>
                </ul>
            </template>-->

                <template v-slot:hoverActions="{row}">
                    <ul>
                        <li>
                            <button class="btn btn-sm btn-primary" @click="modify(row.jobNo)"><i class="las la-pen"></i> <span v-if="row.status < 8">{{$t('outbound.operation.modify')}}</span><span v-else>{{$t('outbound.operation.view')}}</span></button>
                        </li>
                        <li v-if="row.status != 8 && row.status != 10 && row.status != 0 && (!isTESAG() || row.transType != 4 )">
                            <awaiting-button @proceed="(callback) => { cargoOut(row).finally(callback) }"
                                             :btnText="'outbound.operation.cargoOut'"
                                             :btnIcon="'las la-check-circle'"
                                             :btnClass="'btn-primary'">
                            </awaiting-button>
                        </li>
                        <li v-if="row.status != 8 && row.status != 10">
                            <button class="btn btn-sm btn-danger" @click="cancel(row)"><i class="las la-trash-alt"></i> {{$t('outbound.operation.cancel')}}</button>
                        </li>
                    </ul>
                </template>


            </dynamic-table>
        </div>

        <alert v-if="alert" @close="alert = null" :text="alert" />
        <cancel-feature v-if="modal != null && modal.type == 'cancel'" :job="modal.job" @done="refresh()" @close="modal=null" />
        <e-kanban-feature v-if="modal != null && modal.type == 'ekanban'" @done="refresh()" @close="modal=null, refresh()" />
        <manual-feature v-if="modal != null && modal.type == 'manual'" @done="modal = null, refresh(), modify($event)" @close="modal=null" />
        <manual-feature v-if="modal != null && modal.type == 'transfer'" :transType="4" :title="$t('outbound.opearation.whs_title')" @done="modal = null, refresh(), modify($event)" @close="modal=null" />
        <manual-feature v-if="modal != null && modal.type == 'return'" :transType="3" :title="$t('outbound.opearation.return_title')" @done="modal = null, refresh(), modify($event)" @close="modal=null" />
        <create-loading-feature v-if="modal != null && modal.type == 'createloading'" :jobs="modal.jobs" :title="$t('outbound.operation.create_loading')" @done="modal = null, refresh()" @close="modal=null" />
    </div>
</template>

<script>
    import Multiselect from '@vueform/multiselect'
    import DynamicTable from '@/widgets/Table.vue'
    import outboundStatus from '@/enum/outbound_status'
    import transType from '@/enum/trans_type'
    import Date from '@/widgets/Date'
    import Alert from '@/widgets/Alert'
    import CancelFeature from './outbound/Cancel'
    import ManualFeature from './outbound/Manual'
    import EKanbanFeature from './outbound/EKanban'
    import CreateLoadingFeature from './outbound/CreateLoading'
    import outboundService from '@/service/outbound'
    import textFilter from '@/service/text_filter'
    import { toServer } from '@/service/date_converter.js'
    import RouteRefreshMixin from '@/mixins/routeRefreshMixin.js'
    import bus from '@/event_bus.js'
    import MultiselectCustomerCode from '@/widgets/MultiselectCustomerCode.vue'
    import AwaitingButton from '@/widgets/AwaitingButton'
    import DatePicker from '@/widgets/DatePicker';

    import { defineComponent } from 'vue';
export default defineComponent({
        name: 'Outbound',
        mixins: [RouteRefreshMixin],
    components: { Alert, DynamicTable, Date, CancelFeature, ManualFeature, EKanbanFeature, CreateLoadingFeature, Multiselect, MultiselectCustomerCode, AwaitingButton, DatePicker },
        data() {
            return {
                createdFilter: null,
                allowedOutboundCreationMethods: null,
                dispatchedFilter: null,
                alert: null,
                modal: null,
                customers: [],
                outboundService : outboundService,
                status: [0, 1, 2, 3, 4, 5, 23],
                transType: transType,
                transTypeOptions: Object.entries(transType).map(e => ({ label: e[1], value: e[0] })),
                transTypeFilter: null,
                defaultFilter: {
                    orderBy : 'jobNo',
                    desc : true,
                    pageSize: 20,
                    pageNo : 1,
                    statuses : [0, 1, 2, 3, 4, 5, 23]
                },
                statuses: [{
                    label : this.$t('outbound.status.0'),
                    value: [0]
                }, 
                {
                    label : this.$t('outbound.status.1'),
                    value: [1]
                },
                {
                    label : this.$t('outbound.status.2'),
                    value: [2]
                },
                {
                    label : this.$t('outbound.status.3'),
                    value: [3]
                }, 
                {
                    label : this.$t('outbound.status.4'),
                    value: [4]
                }, 
                {
                    label : this.$t('outbound.status.5'),
                    value: [5]
                }, 
                {
                    label :this.$t('outbound.status.out'),
                    value: [0, 1, 2, 3, 4, 5, 23]
                }, 
                {
                    label : this.$t('outbound.status.7'),
                    value: [7]
                }, 
                {
                    label : this.$t('outbound.status.8'),
                    value: [8]
                }, 
                {
                    label : this.$t('outbound.status.9'),
                    value: [9]
                }, 
                {
                    label : this.$t('outbound.status.10'),
                    value: [10, 23]
                }, 
                {
                    label : this.$t('outbound.status.all'),
                    value: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 23]
                }],
                outboundStatus: outboundStatus,
                cols: [
                    {
                        data: 'jobNo',
                        title: this.$t('outbound.columns.jobno'),
                        sortable: true,
                        filter: true,
                        width: 170
                    },
                   {
                       data : 'customerCode',
                        abbrev: this.$t('outbound.columns.customerAbbrev'),
                       title : this.$t('outbound.columns.customer'),
                       sortable: true,
                       filter: true,
                       width : 90
                   },
                   {
                       data : 'supplierName',
                       title : this.$t('outbound.columns.supplier'),
                       filter: true,
                       sortable: true,
                       width : 145
                   },
                   {
                       data : 'refNo',
                       title : this.$t('outbound.columns.ref'),
                       sortable: true,
                       filter: true,
                       width : 180
                   },
                   {
                       data : 'createdDate',
                       title : this.$t('outbound.columns.created'),
                       sortable: true,
                       filter: true,
                       width : 130
                   },
                   {
                       data : 'dispatchedDate',
                       title: this.$t('outbound.columns.released'),
                       sortable: true,
                       filter: true,
                       width: 130
                   },
                   {
                        data: 'transType',
                        title: this.$t('outbound.columns.type'),
                        sortable: true,
                        filter: true,
                        width: 96
                   },
                   {
                        data: 'status',
                        title: this.$t('outbound.columns.status'),
                        sortable: true,
                        filter: true,
                        width: 145
                   },
                   {
                        data: 'remark',
                        title: this.$t('outbound.columns.remark'),
                        sortable: true,
                        filter: true,
                        width: 400
                   }
               ]
            }
        },
        async created(){
            this.settings = await this.$dataProvider.settings.get();
            this.$dataProvider.outbounds.getAllowedOutboundCreationMethods().then(resp => this.allowedOutboundCreationMethods = resp);
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
                this.$dataProvider.outbounds.getAllowedOutboundCreationMethods().then(resp => this.allowedOutboundCreationMethods = resp);
            },
            cancel(job) {
                this.modal = { type: 'cancel', job: job }
            },
            createManual() {
                this.modal = { type: 'manual' }
            },
            importEkanban() {
                this.modal = { type: 'ekanban' }
            },
            createTransfer() {
                this.modal = { type: 'transfer' }
            },
            createReturn() {
                this.modal = { type: 'return' }
            },
            ekanbancheck() {
                this.modal = { type: 'ekanbancheck' }
            },
            isTESAG() {
                return this.settings.ownerCode == 'TESAG';
            },
            createLoading() {
                let jobs = []
                if (this.$refs.table && this.$refs.table.selection.length) {
                    jobs = this.$refs.table.selection
                }
                if (jobs.length == 0) {
                    this.alert = this.$t('outbound.main.selectOutboundFirst')
                    return 
                }
                for (let i = 0; i < jobs.length; i++) {
                    if (jobs[i].customerName != jobs[0].customerName) {
                        this.alert = this.$t('outbound.main.sameCustomerWarning')
                        return
                    }
                    if (jobs[i].status == 9) {
                        this.alert = this.$t('outbound.main.cannotCancel')
                        return
                    }
                    if (jobs[i].transType == 3) {
                        this.alert = this.$t('outbound.main.cannotReturn')
                        return
                    }
                    if (this.$auth.user().whsCode == 'HU') {
                        //todo: eKanban validation
                    }
                    else {
                        if (jobs[i].status == 8) {
                            this.alert = this.$t('outbound.main.cannotComplete')
                            return
                        }
                        //todo: eKanban validation
                    }
                }
                this.modal = { type: 'createloading', jobs: jobs }
            },
            modify(jobNo) {
                this.$router.push({name : 'outbound_detail', params: {jobNo}})
            },
            cargoOut(job) {
                let type = null
                switch(job.transType) {
                    
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
                return this.$dataProvider.outbounds.complete(type, job.jobNo).then(() => {
                    this.$notify({
                        title: this.$t('success.Done'),
                        text: this.$t('success.Completed'),
                        type: 'success',
                        group: 'temporary'
                    })
                    this.refresh()
                })
            },
            remarkFilter(v, filter) {
                // outbound remarkFilter is ignored on the backend in favor of Wildcard support
                filter({ remark: v, remarkFilter: 'Contains' })
            }
        }

    })
</script>

