<template>
    <div class="inbound part-master">

        <div class="toolbar-new">
            <div class="row mt-3 mb-2 ms-1">
                <div class="col-md-3 pt-3 multiselect-as-select">
                    <select-customer v-model="customerCode" />
                </div>
                <div class="col-md-3 text-start main-action-icons">

                    <div class="d-inline-block me-5 link-box" v-if="customerCode">
                        <a href="#" @click.stop.prevent="modal = {type: 'details', subtype: 'new'}" class="text-center">
                            <div><i class="las la-file-medical"></i></div>
                            <div>{{$t('outbound.operation.manual')}}</div>
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
            <dynamic-table :key="customerCode" :func="(params) => $dataProvider.partMasters.getPartMasterList({customerCode: customerCode, ...params})" ref="table" :columns="cols" :filters="defaultFilter" :multiple="false" :search="true" @row-dblclick="showDetails($event)" :hoverActions="true" :actions="false">

                <template v-slot:title-productCode1>
                    {{productCodeMap.pC1Name}}
                </template>
                <template v-slot:title-productCode2>
                    {{productCodeMap.pC2Name}}
                </template>
                <template v-slot:isDefected="{value}">
                    {{value ? $t('partsmaster.status.Defected') : $t('partsmaster.status.NotDefected')}}
                </template>
                <template v-slot:filter-statusString="{filter}">
                    <multiselect :options="statuses" label="label" v-model="status" @update:modelValue="filter({statuses : $event})" :can-clear="false" :showLabels="false"></multiselect>
                </template>
                <template v-slot:filter-isDefected="{filter}">
                    <multiselect :options="defectedStatuses" label="label" v-model="defectedStatus" @update:modelValue="filter({isDefected : $event})" :can-clear="false" :showLabels="false"></multiselect>
                </template>
                <template v-slot:filter-iLogReadinessStatus="{filter}">
                    <multiselect :options="iLogReadinessStatuses" label="label" v-model="iLogReadinessStatus" @update:modelValue="filter({iLogReadinessStatus : $event})" :can-clear="true" :showLabels="false"></multiselect>
                </template>
                <template v-slot:hoverActions="{row}">
                    <ul>
                        <li>
                            <button class="btn btn-sm btn-primary me-2" @click="modal = {type: 'details', subtype: 'view', row: row}"><i class="las la-search"></i> {{$t('partsmaster.operation.show')}}</button>
                            <button class="btn btn-sm btn-primary" @click="showDetails(row)"><i class="las la-pen"></i> {{$t('partsmaster.operation.modify')}}</button>
                        </li>
                    </ul>
                </template>
            </dynamic-table>
        </div>
        <details-feature v-if="modal != null && modal.type == 'details'" :type="modal.subtype" :productCodeMap="productCodeMap" :customerCode="customerCode" :productCode1="modal.subtype == 'new' ? null : modal.row.productCode1" :supplierID="modal.subtype == 'new' ? null : modal.row.supplierID" @close="modal = null, refresh()"/>
    </div>
</template>

<script>
    import SelectCustomer from '@/widgets/SelectCustomer.vue'
    import Multiselect from '@vueform/multiselect'
    import DynamicTable from '@/widgets/Table.vue'
    import RouteRefreshMixin from '@/mixins/routeRefreshMixin.js'
    import DetailsFeature from './partsmaster/Details.vue'
    import bus from '@/event_bus.js'

    import { defineComponent } from 'vue';
export default defineComponent({
        name: 'PartsMaster',
        mixins: [RouteRefreshMixin],
        components: { DynamicTable, Multiselect, SelectCustomer, DetailsFeature },
        data() {
            return {
                productCodeMap: {},
                modal: null,
                customerCode: null,
                defaultFilter: {
                    orderBy : 'supplierID',
                    desc : true,
                    pageSize: 20,
                    pageNo : 1,
                },
                iLogReadinessStatuses: {},
                iLogReadinessStatus: '',
                defectedStatus: null,
                defectedStatuses: [
                    {
                        label: this.$t('partsmaster.status.Defected'),
                        value: true
                    },
                    {
                        label: this.$t('partsmaster.status.NotDefected'),
                        value: false
                    },
                    {
                        label: this.$t('partsmaster.status.All'),
                        value: null
                    }
                ],
                status:[],
                statuses: [
                    {
                        label: this.$t('partsmaster.status.InActive'),
                        value: ["InActive"]
                    },
                    {
                        label: this.$t('partsmaster.status.Active'),
                        value: ["Active"]
                    },
                    {
                        label: this.$t('partsmaster.status.All'),
                        value: []
                    }],
                cols: [
                    {
                        data: 'supplierID',
                        title: this.$t('partsmaster.columns.supplierID'),
                        sortable: true,
                        filter: true,
                        width: 170
                    },
                    {
                        data: 'productCode1',
                        title: this.$t('partsmaster.columns.productCode1'),
                        sortable: true,
                        filter: true,
                        width: 190
                    },
                    {
                        data: 'productCode2',
                        title: this.$t('partsmaster.columns.productCode2'),
                        sortable: true,
                        filter: true,
                        width: 190
                    },
                    {
                        data: 'description',
                        title: this.$t('partsmaster.columns.description'),
                        sortable: true,
                        filter: true,
                        width: 190
                    },
                    {
                        data: 'uomName',
                        title: this.$t('partsmaster.columns.uom'),
                        sortable: true,
                        filter: true,
                        width: 90
                    },
                    {
                        data: 'spq',
                        title: this.$t('partsmaster.columns.spq'),
                        sortable: true,
                        filter: true,
                        width: 120
                    },
                    {
                        data: 'isDefected',
                        title: this.$t('partsmaster.columns.defected'),
                        sortable: true,
                        filter: true,
                        width: 190
                    },
                    {
                        data: 'statusString',
                        title: this.$t('partsmaster.columns.status'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'iLogReadinessStatus',
                        title: this.$t('partsmaster.columns.iLogReadinessStatus'),
                        sortable: true,
                        filter: true,
                        width: 170
                    }
               ]
            }
    },
    created() {
        this.getILogReadinessStatuses();
    },
        activated(){
            bus.on('refresh', this.refresh)
        },
        deactivated(){
            bus.off('refresh', this.refresh)
        },
        methods: {
            refresh() {
                this.$refs.table.refresh();
            },
            showDetails(row) {
                this.modal = { type: 'details', subtype: 'modify', row: row }
            },
            getILogReadinessStatuses() {
                this.$dataProvider.partMasters.iLogReadinessStatuses().then((statuses) => {
                    this.iLogReadinessStatuses = statuses;
                })
            }
        },
        watch: {
            customerCode() {
                this.productCodeMap = this.$dataProvider.inventory.getCustomerInventoryProductCodeMap(this.customerCode)
                    .then(r => {
                        this.productCodeMap = r
                    })
            }
        },

    })
</script>

