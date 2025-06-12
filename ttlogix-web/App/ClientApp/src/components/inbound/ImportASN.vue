<template>
    <div>
        <modal containerClass="modal-create-inbound-asn">
            <template name="header" v-slot:header>
                {{ $t('inbound.operation.importASN_title') }}
            </template>
            <template name="body" v-slot:body>
                <dynamic-table :rowColor="rowColor" :func="(params) => $dataProvider.inbounds.getASNsToImport(params)" ref="table" :columns="cols" :multiple="false" :actions="false" :search="true" @row-dblclick="open($event)">
                    <template v-slot:isVMI="{value}">
                        {{$t(value ? 'YES' : 'NO')}}
                    </template>
                </dynamic-table>
            </template>
            <template name="footer" v-slot:footer>
                <button class="button small" type="button" @click.stop="process()">
                    {{$t('inbound.operation.open')}}
                </button>
                <button class="button small" type="button" @click.stop="$emit('close')">
                    {{$t('inbound.operation.close')}}
                </button>
            </template>
        </modal>
        <import-asn-by-container-feature v-if="modal != null && modal.type == 'bycontainer'" :header="modal.row" @done="modal = null, $emit('done', $event)" @close="modal = null"></import-asn-by-container-feature>
    </div>
</template>
<script>
    import DynamicTable from '@/widgets/Table.vue'
    import ImportAsnByContainerFeature from './ImportASNbyContainer.vue'
import { defineComponent } from 'vue';
export default defineComponent({
        components: { DynamicTable, ImportAsnByContainerFeature},
    data() {
        return {
            confirmModal: null,
            modal: null,
            cols: [
                {
                    data: 'asnNo',
                    title: this.$t('inbound.columns.asn'),
                    sortable: true,
                    filter: true,
                    width: 170
                },
                {
                    data: 'containerNo',
                    title: this.$t('inbound.columns.container'),
                    sortable: true,
                    filter: true,
                    width: 170
                },
                {
                    data : 'factoryID',
                    title : this.$t('inbound.columns.factory'),
                    sortable: true,
                    filter: true,
                    width: 70
                },
                {
                    data : 'supplierID',
                    title : this.$t('inbound.columns.supplierid'),
                    sortable: true,
                    filter: true,
                    width: 85
                },
                {
                    data : 'supplierName',
                    title : this.$t('inbound.columns.suppliername'),
                    sortable: true,
                    filter: true,
                    width: 450
                },
                {
                    data: 'isVMI',
                    title: this.$t('inbound.columns.isVMI'),
                    sortable: true,
                    filter: true,
                    width: 40
                }
            ],
            errors : {}
        }
    },
    methods: {
        rowColor(row) {
            if (this.$refs.table.selection.indexOf(row) != -1) {
                return '#4b8ede78'
            }
            return null
        },
        open(row) {
            this.modal = { type: 'bycontainer', row: row }
        },
        process() {
            if (this.$refs.table.selection.length == 0) {
                this.$notify({
                    title: this.$t('error.ErrorOccured'),
                    text: this.$t('inbound.error.select_asn'),
                    type: 'error',
                    group: 'temporary'
                })
            } else {
               this.open(this.$refs.table.selection[0])
            }
        }
    }
})
</script>
<style lang="scss">
    .modal-create-inbound-asn {
        .modal-container

    {
        width: 1170px;
    }

    .modal-body {
        max-height: 70vh;
        overflow: auto;
        .table

    {
        padding-top: 0px;
    }

    }
    }
</style>