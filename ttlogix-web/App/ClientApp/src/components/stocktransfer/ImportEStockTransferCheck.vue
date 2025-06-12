<template>
    <modal containerClass="modal-check-ekanban">
        <template name="header" v-slot:header>
            {{$t('stockTransfer.operation.ekanban_check_title')}}
        </template>
        <template name="body" v-slot:body>
            <dynamic-table :func="() => $dataProvider.eStockTransfers.EStockTransferCheck(checks)" ref="table" :columns="cols" :filters="defaultFilter" :actions="false" :search="true">
                <template v-slot:statusColor="{row}">
                    <div class="user-management-status" :style="{backgroundColor: getColor(row.status)}"></div>
                </template>
            </dynamic-table>
        </template>
        <template name="footer" v-slot:footer>
            <button class="button small" type="button" @click.stop="process(checks)">
                {{$t('stockTransfer.operation.import')}}
            </button>
            <button class="button small" type="button" @click.stop="$emit('close')">
                {{$t('stockTransfer.operation.cancel')}}
            </button>
        </template>
    </modal>
</template>
<script>
    import DynamicTable from '@/widgets/Table.vue'
    import { toServer } from '@/service/date_converter.js'
    export default {
        props: {
            checks: {
                type: Array,
                default: () => []
            },
        },
        components: { DynamicTable },
        data() {
            return {
                defaultFilter: {
                    UserWHSCode: this.$auth.user().whsCode
                },
                cols: [
                    {
                        data: 'statusColor',
                        title: '',
                        sortable: false,
                        filter: false,
                        width: 30
                    },
                    {
                        data: 'productCode',
                        title: this.$t('stockTransfer.columns.partno'),
                        sortable: true,
                        filter: true,
                        width: 120
                    },
                    {
                        data: 'supplierID',
                        title: this.$t('stockTransfer.columns.supplierId'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'qtyOrdered',
                        title: this.$t('stockTransfer.columns.qtyRequested'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'pkgOrdered',
                        title: this.$t('stockTransfer.columns.pkgRequested'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'onHandPkg',
                        title: this.$t('stockTransfer.columns.onhandPkg'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'allocatedPkg',
                        title: this.$t('stockTransfer.columns.allocatedPkg'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'quarantinePkg',
                        title: this.$t('stockTransfer.columns.quarantinePkg'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'standbyPkg',
                        title: this.$t('stockTransfer.columns.standByPkg'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'availablePkg',
                        title: this.$t('stockTransfer.columns.availablePkg'),
                        sortable: true,
                        filter: true,
                        width: 100
                    },
                    {
                        data: 'status',
                        title: this.$t('stockTransfer.columns.status'),
                        sortable: true,
                        filter: true,
                        width: 40
                    }
                ]
            }
        },
        methods: {
            toServer,
            async process(jobs) {
                this.$dataProvider.stockTransfers.importEStockTransferMulti(jobs)
                    .then(() => { this.$emit('closeall') })
                    .finally(() => this.processing = false)
            },
            getColor(status) {
                const s = status.trim();
                return s == 'UNABLE TO SUPPLY' ? '#cd0000' : (s.startsWith('SUPPLY PARTIALLY') ? '#d3aa17' : '#149414')
            }
        }
    }
</script>
<style lang="scss">
    .modal-check-ekanban {
        .modal-container {
            width: 1300px;
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