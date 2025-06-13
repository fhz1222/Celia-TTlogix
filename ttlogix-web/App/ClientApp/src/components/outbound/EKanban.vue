<template>
    <div>
        <modal containerClass="modal-create-ekanban">
            <template name="header" v-slot:header>
                {{ $t('outbound.operation.ekanban_title') }}
            </template>
            <template name="body" v-slot:body>
                <dynamic-table :func="(params) => $dataProvider.ekanbans.getEKanbanListForEurope(params)" ref="table" :columns="cols" :multiple="true" :filters="defaultFilter" :actions="false" :search="true" @selection="evaluateSelected()" @row-click="evaluateSelected()" @row-dblclick="processJobs([$event.orderNo])">

                    <template v-slot:eta="{value}">
                        <date-time :value="value" />
                    </template>
                    <template v-slot:ediDate="{value}">
                        <date-time :value="value" />
                    </template>
                    <template v-slot:createdDates="{value}">
                        <date-time :value="value" />
                    </template>
                    <template v-slot:filter-eta="{filter}">
                        <date-picker :date="etaFrom" @set="(date) => {filter({etaFrom: date})}" />
                        <div class="float-start" style="margin-top: -3px">-&nbsp;</div>
                        <date-picker :date="etaTo" @set="(date) => {filter({etaTo: date})}" />
                    </template>
                    <template v-slot:filter-ediDate="{filter}">
                        <date-picker :date="ediDateFrom" @set="(date) => {filter({ediDateFrom: date})}" />
                        <div class="float-start" style="margin-top: -3px">-&nbsp;</div>
                        <date-picker :date="ediDateTo" @set="(date) => {filter({ediDateTo: date})}" />
                    </template>
                    <template v-slot:actions="{row}">
                        <ul>
                            <li>
                                <button class="button" @click="modify(row.jobNo)">
                                    <span v-if="row.status < 8">{{$t('outbound.operation.modify')}}</span>
                                    <span v-else>{{$t('outbound.operation.view')}}</span>
                                </button>
                            </li>
                        </ul>
                    </template>
                </dynamic-table>

            </template>
            <template name="footer" v-slot:footer>
                <div v-if="progress.total > 0">
                    {{progress.done}} / {{progress.total}}
                </div>
                <button :disabled="isCheckDisabled || progress.total > 0" class="button small" type="button" @click.stop="cancelEkanban()">
                    {{$t('outbound.operation.cancelEKanban')}}
                </button>
                <button :disabled="isCheckDisabled || progress.total > 0" class="button small" type="button" @click.stop="check()">
                    {{$t('outbound.operation.check')}}
                </button>
                <button :disabled="progress.total > 0" class="button small" type="button" @click.stop="process()">
                    {{$t('outbound.operation.create')}}
                </button>
                <button :disabled="progress.total > 0" class="button small" type="button" @click.stop="$emit('close')">
                    {{$t('outbound.operation.close')}}
                </button>
            </template>
        </modal>
        <e-kanban-check-feature v-if="modal != null && modal.type == 'ekanbancheck'" :eKanbans="modal.eKanbans" @close="modal=null" @closeall="modal=null; $emit('close')" />
        <cancel-e-kanban-feature v-if="modal != null && modal.type == 'cancelEKanban'" :eKanbans="modal.eKanbans" @done="reload" @close="modal = null" />
    </div>
</template>
<script>
    import DynamicTable from '@/widgets/Table.vue'
    import DateTime from '@/widgets/DateTime.vue'
    import { toServer } from '@/service/date_converter.js'
    import EKanbanCheckFeature from './EKanbanCheck.vue'
    import { defineComponent } from 'vue';
    import DatePicker from '@/widgets/DatePicker';
    import CancelEKanbanFeature from './CancelEKanban.vue';
export default defineComponent({
        props: {
            transType: {
                default: 0
            }
        },
        created() {
            this.load()
        },
        components: { DynamicTable, DateTime, EKanbanCheckFeature, DatePicker, CancelEKanbanFeature },
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
                progress: {done: 0, total: 0},
                whsList: [],
                isCheckDisabled: true,
                etaFilter: null,
                ediDateFilter: null,
                modal: null,
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
                cols: [
                    {
                        data: 'orderNo',
                        title: this.$t('outbound.columns.orderno'),
                        sortable: true,
                        filter: true,
                        width: 120
                    },
                    {
                        data: 'factoryId',
                        title: this.$t('outbound.columns.factory'),
                        sortable: true,
                        filter: true,
                        width: 70
                    },
                    {
                        data: 'blanketOrderNo',
                        title: this.$t('outbound.columns.blanket'),
                        sortable: true,
                        filter: true,
                        width: 150
                    },
                    {
                        data: 'ediDate',
                        title: this.$t('outbound.columns.ediDate'),
                        sortable: true,
                        filter: true,
                        width: 230
                    },
                    {
                        data: 'eta',
                        title: this.$t('outbound.columns.delivery'),
                        sortable: true,
                        filter: true,
                        width: 230
                    },
                    {
                        data: 'supplierId',
                        title: this.$t('outbound.columns.supplierid'),
                        sortable: true,
                        filter: true,
                        width: 85
                    },
                    {
                        data: 'supplierName',
                        title: this.$t('outbound.columns.suppliername'),
                        sortable: true,
                        filter: true,
                        width: 100
                    }
                ],
                errors: {}
            }
        },
        methods: {
            toServer,
            load() {

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
                    this.$refs.table.selection.forEach(o => jobs.push(o))
                    this.processJobs(jobs)
                }
            },
            async processJobs(jobs) {
                this.progress = {total: jobs.length, done: 0};
                for (var i = 0; i < jobs.length; i++){
                    try {
                        await this.$dataProvider.outbounds.importEKanbanEUCPart(jobs[i].orderNo, null, jobs[i])
                    }
                    catch {
                        //todo 
                    }
                    this.progress.done++;
                }
                this.progress = {total: 0, done: 0};
                this.$emit('close'); 
            },
            check() {
                let checks = []
                this.$refs.table.selection.forEach(o => checks.push(o));
                this.modal = { type: "ekanbancheck", eKanbans: checks };
            },
            evaluateSelected() {
                this.isCheckDisabled = this.$refs.table.selection.length == 0 ? true : false;
            },
            reload() {
                if (this.$refs.table && this.$refs.table.load) {
                    this.$refs.table.load();
                    console.log('EKanban reload compelted'); 
                }
            },
            cancelEkanban() {
                let orders = []
                this.$refs.table.selection.forEach(o => orders.push(o))
                this.modal = { type: "cancelEKanban", eKanbans: orders };
            }
        }
    })
</script>
<style lang="scss">
    .modal-create-ekanban {
        .modal-container {
            width: 95%;
        }

        .modal-body {
            max-height: 70vh;
            overflow: auto;

            .table {
                padding-top: 0px;
            }
        }
    }
</style>