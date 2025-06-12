<template>
    <div>
        <modal containerClass="modal-import-asn-container">
            <template name="header" v-slot:header>
                {{$t('inbound.operation.importASN_container_title')}}
            </template>
            <template name="body" v-slot:body>
                <div class="form-row">
                    <label class="form-label">{{$t('inbound.detail.asn')}}</label>
                    <div class="form-input full">
                        <input type="text" class="input" :value="header.asnNo" disabled />
                    </div>
                </div>
                <div class="form-row">
                    <label class="form-label">{{$t('inbound.detail.supplier')}}</label>
                    <div class="form-input full">
                        <input type="text" class="input" style="width: 20%; border-right: none" :value="header.supplierID" disabled />
                        <input type="text" class="input" style="width: 80%" :value="header.supplierName" disabled />
                    </div>
                </div>
                <div class="form-row">
                    <label class="form-label">{{$t('inbound.detail.factory')}}</label>
                    <div class="form-input">
                        <input type="text" class="input" :value="header.factoryID" disabled />
                    </div>
                </div>
                <div class="form-row">
                    <label class="form-label">{{$t('inbound.detail.isVMI')}}</label>
                    <div class="form-input">
                        <input type="text" class="input" :value="$t(header.isVMI ? 'YES': 'NO')" disabled />
                    </div>
                </div>
                <div class="form-row">
                    <label class="form-label">{{$t('inbound.detail.containerNo')}}</label>
                    <div class="form-input">
                        <input type="text" class="input" :value="header.containerNo" disabled />
                    </div>
                </div>
                <div v-if="loading" class="mt-2 text-center">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">{{$t('generic.loading')}}</span>
                    </div>
                </div>
                <simple-table-scroll height="250px" v-else>
                    <template v-slot:head>
                        <th class="pointer" style="width: 200px" @click.prevent.stop="sort(row => row.productCode)">
                            {{$t('inbound.columns.partNo')}}
                            <i :class="sortedAsc ? 'las la-sort-amount-up-alt' : 'las la-sort-amount-down'"></i>
                        </th>
                        <th style="width: 100px">{{$t('inbound.columns.qtyOuter')}}</th>
                        <th style="width: 177px">{{$t('inbound.columns.outer#')}}</th>
                    </template>
                    <template v-slot:body>
                        <tr v-for="(row, index) in asnDetails" :key="index">
                            <td style="width: 200px">{{row.productCode}}</td>
                            <td style="width: 100px">{{row.qtyPerOuter}}</td>
                            <td style="width: 177px">{{row.noOfOuter}}</td>
                        </tr>
                    </template>
                </simple-table-scroll>
            </template>
            <template name="footer" v-slot:footer>
                <button class="btn btn-primary btn-sm" type="button" @click.stop="modal = { type: 'confirm', asnNo: header.asnNo}" :disabled="processing || loading">
                    <i class="las la-download"></i> {{$t('inbound.operation.importASN')}}
                </button>
                <button class="btn btn-secondary btn-sm" type="button" @click.stop="$emit('close')" :disabled="processing || loading">
                    <i class="las la-times"></i> {{$t('inbound.operation.close')}}
                </button>
            </template>
        </modal>
        <alert v-if="alert" :text="alert" @close="alert=null"></alert>
        <confirm v-if="modal != null && modal.type == 'confirm'" @ok="importASN(), modal = null" @close="modal = null">{{$t('inbound.operation.import_container_in_asn', modal)}}</confirm>
    </div>
</template>
<script>
    import SimpleTableScroll from '@/widgets/SimpleTableScroll'
    import Alert from '@/widgets/Alert'
    import Confirm from '@/widgets/Confirm'
    import { defineComponent } from 'vue';
export default defineComponent({
        props: ['header'],
        components: { SimpleTableScroll, Alert, Confirm },
        created() {
            this.loading = true
            this.$dataProvider.inbounds.getASNDetails(this.header.asnNo).then(r => {
                this.asnDetails = r
                this.sort(row => row.productCode)
            }).finally(() => this.loading = false)
        },
        data() {
            return {
                alert: null,
                modal: null,
                pos: null,
                asnDetails: [],
                loading: false,
                processing: false,
                expandedRow: null,
                sortedAsc: false
            }
        },
        methods: {
            importASN() {
                this.processing = true
                this.$dataProvider.inbounds.importASN(this.header.asnNo)
                    .then(resp => this.$emit('done', resp))
                    .finally(() => this.processing = false)
            },
            isSelected(row) {
                return this.selectedContainers.includes(row.containerNo)
            },
            onClick(row) {
                var index = this.selectedContainers.indexOf(row.containerNo);
                if (index !== -1) {
                    this.selectedContainers.splice(index, 1);
                }
                else {
                    this.selectedContainers.push(row.containerNo);
                }
            },
            sort(selector) {
                this.sortedAsc = !this.sortedAsc
                this.asnDetails = this.asnDetails.sort((a, b) => selector(a).localeCompare(selector(b)) * (this.sortedAsc ? 1 : -1));
            }
        }
})
</script>
<style lang="scss">
    .modal-import-asn-container {
        .modal-container {
        width: 500px;
    }

    tbody {
        background-color: white;

            tr {
        border: none;
        cursor: pointer;

                &.selected {
        background-color: #4b8ede78;
    }
    }
    }
    }
</style>