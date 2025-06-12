<template>
    <div class="loading">
        
        <div class="toolbar-new">
            <div class="row pt-2 pb-2">
                <div class="col-md-6 ps-4 main-action-icons">
                    <div class="d-inline-block link-box">
                        <a href="#" @click.stop.prevent="add()" class="text-center">
                            <div><i class="las la-file-medical"></i></div>
                            <div>{{$t('loading.operation.add')}}</div>
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
        
        <div class="wrap">
            <dynamic-table 
                :func="(params) => $dataProvider.loadings.getLoadings(params)" 
                ref="table" 
                :rowColor="rowColor" 
                :columns="cols" 
                :filters="defaultFilter"
                :multiple="false" 
                :search="true" 
                @row-dblclick="modify($event.jobNo)" 
                :actions="false" 
                :hoverActions="true">

                <template v-slot:calculatedStatusString="{value}">
                    {{$t('loading.status.' + value)}}
                </template>
                <template v-slot:createdDate="{value}">
                    <div style="text-align: center">
                        <date :value="value" />
                    </div>
                </template>
                <template v-slot:truckDepartureDate="{value, row}">
                    <div :class="{'overlap-container': value}" style="text-align: center">
                        <date-time :value="value" nbsp />
                        <div :class="{overlap: value}" style="text-align: center">
                            <button class="button secondary" :class="{'vh-center': value}" @click.prevent.stop="setTruckDeparture(row)">{{$t('loading.operation.now')}}</button>
                        </div>
                    </div>
                </template>
                <template v-slot:truckArrivalDate="{value, row}">
                    <div :class="{'overlap-container': value}" style="text-align: center">
                        <date-time :value="value" nbsp />
                        <div :class="{overlap: value}" style="text-align: center">
                            <button class="button secondary" :class="{'vh-center': value}" @click.prevent.stop="setTruckArrival(row)">{{$t('loading.operation.now')}}</button>
                        </div>
                    </div>
                </template>
                <template v-slot:eta="{value}">
                    <div style="text-align: center">
                        <date-time :value="value" nbsp />
                    </div>
                </template>
                <template v-slot:etd="{value}">
                    <div style="text-align: center">
                        <date-time :value="value" nbsp />
                    </div>
                </template>
                <template v-slot:filter-calculatedStatusString="{filter}">
                    <multiselect :options="statuses" label="label" v-model="status" @update:modelValue="filter({statuses : $event})" :can-clear="false" :showLabels="false"></multiselect>
                </template>
                <template v-slot:filter-etd="{filter}">
                    <date-picker :date="etdFilter" @set="(date) => {filter({etd: date})}" />
                </template>
                <template v-slot:filter-eta="{filter}">
                    <date-picker :date="etaFilter" @set="(date) => {filter({eta: date})}" />
                </template>
                <template v-slot:filter-truckArrivalDate="{filter}">
                    <date-picker :date="arrivalFilter" @set="(date) => {filter({truckArrivalDate: date})}" />
                </template>
                <template v-slot:filter-truckDepartureDate="{filter}">
                    <date-picker :date="departureFilter" @set="(date) => {filter({truckDepartureDate: date})}" />
                </template>
                <template v-slot:filter-remark="{filter}">
                    <input type="text" placeholder="..." class="filter" @keyup.enter.stop="remarkFilter($event.target.value, filter)" />
                </template>
                <template v-slot:filter-customerCode="{filter}">
                    <multiselect-customer-code v-model="customers" :can-clear="false" @update:modelValue="filter({customerCodes: $event})"></multiselect-customer-code>
                </template>
                <template v-slot:hoverActions="{row}">
                    <ul>
                        <li><button class="btn btn-sm btn-primary" @click="modify(row.jobNo)"><i class="las la-pen"></i>{{$t('operation.general.modify')}}</button></li>
                        <li><button v-if="row.statusString == 'NewJob' || row.statusString == 'Processing'" class="btn btn-sm btn-danger" @click="cancel(row.jobNo)"><i class="las la-trash-alt"></i>{{$t('operation.general.cancel')}}</button></li>                       
                    </ul>
                </template>
            </dynamic-table>
        </div>

        <confirm v-if="modal != null && modal.type == 'confirmtruck'" @ok="modal.method(modal.jobNo), modal=null" @close="modal=null">{{$t('loading.detail.truckOverwrite')}} {{modal.subtype}}?</confirm>
        <cancel-feature v-if="modal != null && modal.type == 'cancel'" :jobNo="modal.jobNo" @done="modal = null, refresh()" @close="modal=null" />
        <manual-feature v-if="modal != null && modal.type == 'add'" @done="modal = null, modify($event.jobNo), refresh()" @close="modal=null" />

    </div>
</template>

<script>
    import DynamicTable from '@/widgets/Table.vue'
    import Multiselect from '@vueform/multiselect'
    import MultiselectCustomerCode from '@/widgets/MultiselectCustomerCode.vue'
    import Date from '@/widgets/Date'
    import Confirm from '@/widgets/Confirm'
    import DateTime from '@/widgets/DateTime'
    import ManualFeature from './loading/Manual'
    import CancelFeature from './loading/Cancel'
    import { toServer } from '@/service/date_converter.js'
    import rowColor from '@/service/loading_color.js'
    import textFilter from '@/service/text_filter'
    import RouteRefreshMixin from '@/mixins/routeRefreshMixin.js'
    import bus from '@/event_bus.js'
    import DatePicker from '@/widgets/DatePicker';

    import { defineComponent } from 'vue';
export default defineComponent({
        name: 'Loading',
        mixins: [RouteRefreshMixin],
        components: { Confirm, DynamicTable, Date, DateTime, ManualFeature, CancelFeature, MultiselectCustomerCode, Multiselect, DatePicker },
        data() {
            return {
                etdFilter: null,
                etaFilter: null,
                arrivalFilter: null,
                departureFilter: null,
                modal: null,
                customers: [],
                status: ["NewJob", "Processing", "Picked"],
                defaultFilter: {
                    orderBy: 'jobNo',
                    desc: true,
                    pageSize: 20,
                    pageNo: 1,
                    statuses: ["NewJob", "Processing", "Picked"]
                },
                statuses: [
                    {
                        label: this.$t('loading.status.NewJob'),
                        value: ["NewJob"]
                    },
                    {
                        label: this.$t('loading.status.Processing'),
                        value: ["Processing"]
                    },
                    {
                        label: this.$t('loading.status.Picked'),
                        value: ["Picked"]
                    },
                    {
                        label: this.$t('loading.status.Outstanding'),
                        value: ["NewJob", "Processing", "Picked"]
                    },
                    {
                        label: this.$t('loading.status.Confirmed'),
                        value: ["Confirmed"]
                    },
                    {
                        label: this.$t('loading.status.InTransit'),
                        value: ["InTransit"]
                    },
                    {
                        label: this.$t('loading.status.Completed'),
                        value: ["Completed"]
                    },
                    {
                        label: this.$t('loading.status.Cancelled'),
                        value: ["Cancelled"]
                    },
                    {
                        label: this.$t('loading.status.All'),
                        value: []
                    }],
                cols: [
                    {
                        data: 'jobNo',
                        title: this.$t('loading.columns.jobno'),
                        sortable: true,
                        filter: true,
                        width: 170
                    },
                    {
                        data: 'customerCode',
                        abbrev: this.$t('loading.columns.customerAbbrev'),
                        title: this.$t('loading.columns.customer'),
                        sortable: true,
                        filter: true,
                        width: 90
                    },
                    {
                        data: 'refNo',
                        title: this.$t('loading.columns.ref'),
                        sortable: true,
                        filter: true,
                        width: 120
                    },
                    {
                        data: 'etd',
                        title: this.$t('loading.columns.etd'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'eta',
                        title: this.$t('loading.columns.eta'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'truckArrivalDate',
                        title: this.$t('loading.columns.arrival'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'truckDepartureDate',
                        title: this.$t('loading.columns.departure'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'truckLicencePlate',
                        title: this.$t('loading.columns.licencePlate'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'truckSeqNo',
                        title: this.$t('loading.columns.truckNo'),
                        sortable: true,
                        filter: true,
                        width: 50
                    },
                    {
                        data: 'trailerNo',
                        title: this.$t('loading.columns.trailerNo'),
                        sortable: true,
                        filter: true,
                        width: 50
                    },
                    {
                        data: 'dockNo',
                        title: this.$t('loading.columns.dockNo'),
                        sortable: true,
                        filter: true,
                        width: 50
                    },
                    {
                        data: 'noOfPallet',
                        title: this.$t('loading.columns.pids'),
                        sortable: true,
                        filter: true,
                        width: 40
                    },
                    {
                        data: 'calculatedStatusString',
                        title: this.$t('loading.columns.status'),
                        sortable: true,
                        filter: true,
                        width: 110
                    },
                    {
                        data: 'remark',
                        title: this.$t('loading.columns.remark'),
                        sortable: true,
                        filter: true,
                        width: 440
                    }]
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
            rowColor,
            refresh() {
                this.$refs.table.refresh()
            },
            modify(jobNo) {
                this.$router.push({ name: 'loading_detail', params: { jobNo } })
            },
            add() {
                this.modal = { type: 'add' }
            },
            cancel(jobNo) {
                this.modal = {
                    type: 'cancel', jobNo: jobNo
                }
            },
            confirmTruckArrival(jobNo) {
                this.$dataProvider.loadings.truckArrival(jobNo).then(() => {
                    this.refresh()
                })
            },
            confirmTruckDeparture(jobNo) {
                this.$dataProvider.loadings.truckDeparture(jobNo).then(() => {
                    this.refresh()
                })
            },
            setTruckArrival(job) {
                if (job.truckArrivalDate) {
                    this.modal = { type: 'confirmtruck', jobNo: job.jobNo, method: this.confirmTruckArrival, subtype: 'arrival' }
                }
                else {
                    this.confirmTruckArrival(job.jobNo)
                }
            },
            setTruckDeparture(job) {
                if (job.truckDepartureDate) {
                    this.modal = { type: 'confirmtruck', jobNo: job.jobNo, method: this.confirmTruckDeparture, subtype: 'departure' }
                }
                else {
                    this.confirmTruckDeparture(job.jobNo)
                }
            },
            setAllowForDispatch(row, v) {
                this.$dataProvider.loadings.setAllowForDispatch(row.jobNo, v.target.checked).then(() => {
                    this.refresh()
                })
            },
            remarkFilter(v, filter) {
                let t = textFilter(v)
                filter({ remark: t.filter, remarkFilter: t.filterType })
            }
        }

    })
</script>
<style lang="scss" scoped>
    .overlap {
        display: none;
        position: absolute;
        top: 0;
        left: 0;
        backdrop-filter: blur(3px);
        width: 100%;
        height: 100%;
    }
    .overlap-container{
        position: relative;
    }
    .overlap-container:hover > .overlap {
        display: block;
    }
</style>
