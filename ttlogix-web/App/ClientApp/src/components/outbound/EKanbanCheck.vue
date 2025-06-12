<template>
    <modal containerClass="modal-check-ekanban">
        <template name="header" v-slot:header>
            {{$t('outbound.operation.ekanban_check_title')}}
        </template>
        <template name="body" v-slot:body>
            <dynamic-table :func="() => $dataProvider.ekanbans.EKanbanCheck(checks)" ref="table" :columns="cols" :filters="defaultFilter" :actions="false" :search="true">
                <template v-slot:statusColor="{row}">
                    <div class="user-management-status" :style="{backgroundColor: getColor(row.status)}"></div>
                </template>
            </dynamic-table>
        </template>
        <template name="footer" v-slot:footer>
                <div v-if="progress.total > 0">
                    {{progress.done}} / {{progress.total}}
                </div>
            <button :disabled="progress.total > 0" class="button small" type="button" @click.stop="process(eKanbans)">
                {{$t('outbound.operation.import')}}
            </button>
            <button :disabled="progress.total > 0" class="button small" type="button" @click.stop="$emit('close')">
                {{$t('outbound.operation.cancel')}}
            </button>
        </template>
    </modal>
</template>
<script>
    //import DynamicTable from '@/widgets/Table.vue'
    //import DateTime from '@/widgets/DateTime.vue'
    import DynamicTable from '@/widgets/Table.vue'
    import { toServer } from '@/service/date_converter.js'
    import { defineComponent } from 'vue';
export default defineComponent({
        props: {
            eKanbans: {
                type: Array,
                default: () => []
            }
        },
        components: { DynamicTable },
        data() {
            return {
                progress: {done: 0, total: 0},
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
                        title: this.$t('outbound.columns.partno'),
                        sortable: true,
                        filter: true,
                        width: 120
                    },
                    {
                        data: 'supplierID',
                        title: this.$t('outbound.columns.supplierId'),
                        sortable: true,
                        filter: true,
                        width: 80
                    },
                    {
                        data: 'qtyOrdered',
                        title: this.$t('outbound.columns.qtyRequested'),
                        sortable: true,
                        filter: true,
                        width: 80
                    },
                    {
                        data: 'pkgOrdered',
                        title: this.$t('outbound.columns.pkgRequested'),
                        sortable: true,
                        filter: true,
                        width: 80
                    },
                    {
                        data: 'onHandPkg',
                        title: this.$t('outbound.columns.onhandPkg'),
                        sortable: true,
                        filter: true,
                        width: 60
                    },
                    {
                        data: 'allocatedPkg',
                        title: this.$t('outbound.columns.allocatedPkg'),
                        sortable: true,
                        filter: true,
                        width: 60
                    },
                    {
                        data: 'quarantinePkg',
                        title: this.$t('outbound.columns.quarantinePkg'),
                        sortable: true,
                        filter: true,
                        width: 80
                    },
                    {
                        data: 'availablePkg',
                        title: this.$t('outbound.columns.availablePkg'),
                        sortable: true,
                        filter: true,
                        width: 60
                    },
                    {
                        data: 'elxOnhandPkg',
                        title: this.$t('outbound.columns.elxOnhandPkg'),
                        sortable: true,
                        filter: true,
                        width: 70
                    },
                    {
                        data: 'elxAllocatedPkg',
                        title: this.$t('outbound.columns.elxAllocatedPkg'),
                        sortable: true,
                        filter: true,
                        width: 60
                    },
                    {
                        data: 'elxQuarantinePkg',
                        title: this.$t('outbound.columns.elxQuarantinePkg'),
                        sortable: true,
                        filter: true,
                        width: 80
                    },
                    {
                        data: 'elxAvailablePkg',
                        title: this.$t('outbound.columns.elxAvailablePkg'),
                        sortable: true,
                        filter: true,
                        width: 60
                    },
                    {
                        data: 'status',
                        title: this.$t('outbound.columns.status'),
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
                this.$emit('closeall'); 
            },
            getColor(status) {
                const s = status.trim();
                return s == 'UNABLE TO SUPPLY' ? '#cd0000' : (s.startsWith('SUPPLY PARTIALLY') ? '#d3aa17' : '#149414')
            }
        },
        computed:{
            checks() {
                return this.eKanbans.map(k => k.orderNo)
            }
        }
    })
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