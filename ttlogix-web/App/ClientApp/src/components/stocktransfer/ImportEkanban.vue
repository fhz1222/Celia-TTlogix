<template>
    <modal containerClass="modal-add-detail-loading">
        <template name="header" v-slot:header>
            <span>{{$t('stockTransfer.import.importEKanban')}}</span>
        </template>
        <template name="body" v-slot:body>

            <div class="scrollable">

                <dynamic-table :func="(params) => $dataProvider.ekanbans.getEKanbanListForEurope(params)" ref="table" :columns="cols" :multiple="true" :filters="defaultFilter" :actions="false" :search="true" @row-dblclick="processJobs([$event.orderNo])" @table-data-loaded="loading = false">

                    <template v-slot:eta="{value}">
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

                <div v-if="loading" class="mt-5 mb-5 text-center">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">{{$t('generic.loading')}}</span>
                    </div>
                </div>

            </div>

        </template>
        <template name="footer" v-slot:footer>
            <!--<button class="btn btn-sm btn-primary me-2" type="button" @click.stop="process()" :disabled="processing">
                <i :class="['las', newUser ? 'la-plus-circle' : 'la-save']"></i> {{newUser ? $t('operation.general.add') : $t('operation.general.save')}}
            </button>-->
            <button class="btn btn-sm btn-primary me-2" type="button" @click.stop="process()" :disabled="processing">
                <i class="las la-file-download"></i> {{$t('stockTransfer.import.importButton')}}
            </button>
            <button class="btn btn-sm btn-secondary" type="button" @click.stop="$emit('close')" :disabled="processing">
                <i class="las la-times"></i> {{$t('operation.general.close')}}
            </button>
        </template>
    </modal>
</template>
<script>
    import DynamicTable from '@/widgets/Table.vue'
    import DateTime from '@/widgets/DateTime.vue'
    import { toServer } from '@/service/date_converter.js'
    import DatePicker from '@/widgets/DatePicker';
    import { defineComponent } from 'vue';
export default defineComponent({
        props: {

            transType: {
                default: 0
            }
        },
        components: { DynamicTable, DateTime, DatePicker },
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
                etaFilter: null,
                loading: true,
                processing: false,
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
                        width: 160
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
                this.processing = true
                this.$dataProvider.stockTransfers.importEKanbanEUCPartMulti(jobs)
                    .then(resp => { this.$emit('close', resp) })
                    .finally(() => this.processing = false)
            }
        }
    })
</script>
<style lang="scss">
    .modal-add-detail-loading {
        .modal-container {
            width: 80vw;
        }
    }
</style>