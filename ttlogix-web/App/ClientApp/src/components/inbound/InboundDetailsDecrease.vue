<template>
    <div>
        <modal containerClass="modal-create-inbound-decrease">
            <template name="header" v-slot:header>
                {{ $t('inbound.detail.decreaseModalTitle') }}
            </template>
            <template name="body" v-slot:body>

                <div class="row">
                    <div class="col-md-1 text-end no-right-padding pt-1">
                        {{ $t('inbound.detail.removePIDModalPartNo') }}
                    </div>
                    <div class="col-md-4">
                        <input type="text" :value="header.productCode" class="form-control form-control-sm" disabled />
                    </div>
                    <div class="col-md-1 text-end no-right-padding pt-1">
                        #
                    </div>
                    <div class="col-md-2">
                        <input type="text" :value="header.lineItem" class="form-control form-control-sm" disabled />
                    </div>
                    <div class="col-md-1 text-end no-right-padding pt-1">
                        {{$t('generic.total')}}
                    </div>
                    <div class="col-md-2">
                        <input type="text" :value="header.qty" class="form-control form-control-sm" disabled />
                    </div>
                </div>

                <div class="scrollable mt-3">
                    <div v-if="loading" class="mt-5 mb-5 text-center">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">{{$t('generic.loading')}}</span>
                        </div>
                    </div>

                    <table class="table table-sm table-striped table-hover align-middle selectable">
                        <thead>
                            <tr>
                                <th class="num">{{$t('inbound.detail.removePIDModalNo')}}</th>
                                <th class="num">{{$t('inbound.detail.removePIDModalPID')}}</th>
                                <th class="num">{{$t('inbound.detail.removePIDModalQtyPerBox')}}</th>
                                <th class="num">{{$t('inbound.detail.removePIDModalQuantity')}}</th>
                                <th class="num">{{$t('inbound.detail.removePIDModalLocation')}}</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(row, index) in details"
                                :key="index"
                                :class="{selected: isSelected(row)}" @click.prevent.stop="onClick(row)">
                                <td>{{row.seqNo}}</td>
                                <td>{{row.pid}}</td>
                                <td>{{row.qtyPerPkg}}</td>
                                <td>{{row.qty}}</td>
                                <td>{{row.locationCode}}</td>
                                <td class="action-icon"><i class="las la-trash-alt" @click.stop="removeSingle(row)" :class="{'disabled': true}"></i></td>
                            </tr>
                        </tbody>
                    </table>
                </div>

            </template>
            <template name="footer" v-slot:footer>
                <button class="btn btn-primary btn-sm me-2" type="button" @click.stop="removeSelected()" :disabled="processing">
                    <i class="las la-trash-alt"></i> {{$t('inbound.operation.removeselected')}}
                </button>
                <button class="btn btn-primary btn-sm me-2" type="button" @click.stop="removeAll()" :disabled="processing">
                    <i class="las la-trash-alt"></i> {{$t('inbound.operation.removeall')}}
                </button>
                <button class="btn btn-secondary btn-sm" type="button" @click.stop="$emit(anyRemoved ? 'done' : 'close')">
                    <i class="las la-times"></i> {{$t('inbound.operation.close')}}
                </button>
            </template>
        </modal>
        <confirm v-if="modal != null && modal.type == 'remove'" @ok="modal.action(), modal = null" @close="modal = null">
            {{$t(modal.msg)}}
        </confirm>
    </div>
</template>
<script>
    //import DynamicTable from '@/widgets/Table.vue'
    import Confirm from '@/widgets/Confirm.vue'
    import { defineComponent } from 'vue';
export default defineComponent({
        components: { /*DynamicTable, */Confirm },
        props: ['header', 'inJobNo'],
        created() {
            this.refresh();
        },
        data() {
            return {
                confirmModal: null,
                modal: null,
                errors: {},
                details: [],
                processing: false,
                anyRemoved: false,
                loading: true,
                selectedPids: []
            }
        },
        methods: {
            remove(pids, all) {
                if (!this.processing) {
                    this.processing = true
                    this.$dataProvider.inbounds.removePIDs({
                        jobNo: this.inJobNo,
                        lineItem: this.header.lineItem,
                        pids: pids,
                        removeAll: all
                    })
                        .then(() => {
                            this.anyRemoved = true
                            this.refresh()
                        })
                        .finally(() => this.processing = false)
                }
            },
            removeAll() {
                this.modal = {
                    type: 'remove',
                    msg: 'inbound.operation.RemoveEntireLot',
                    action: () => this.remove([], true)
                }
            },
            removeSelected() {
                this.modal = {
                    type: 'remove',
                    msg: 'inbound.operation.RemoveSelectedPIDs',
                    action: () => this.remove(this.selectedPids, false)
                }
            },
            removeSingle(row) {
                this.modal = {
                    type: 'remove',
                    msg: 'inbound.operation.RemoveSelectedPIDs',
                    action: () => this.remove([row.pid], false)
                }
            },
            refresh() {
                this.$dataProvider.storage.getStoragePutawayList(this.inJobNo, this.header.lineItem)
                .then(resp => {
                    this.details = resp;
                    this.loading = false;
                })
            },
            isSelected(row) {
                return this.selectedPids.includes(row.pid)
            },
            onClick(row) {
                var index = this.selectedPids.indexOf(row.pid);
                if (index !== -1) {
                    this.selectedPids.splice(index, 1);
                }
                else {
                    this.selectedPids.push(row.pid);
                }
            },
        }
    })
</script>

<style lang="scss">
    .modal-create-inbound-decrease {
        .modal-container{
            width: 700px;
        }
    }
</style>