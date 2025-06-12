<template>
    <div class="small-font">
        <modal containerClass="modal-create-inbound-increase">
            <template name="header" v-slot:header>
                {{ $t('inbound.detail.increaseModalTitle') }}
            </template>
            <template name="body" v-slot:body>
                <!--<dynamic-table :rowColor="rowColor" :func="(params) => $dataProvider.inbounds.getASNsToImport(params)" ref="table" :columns="cols" :multiple="false" :actions="false" :search="true" @row-dblclick="modal = {type: 'open', row: $event}">
                <template v-slot:isVMI="{value}">
                    {{$t(value ? 'YES' : 'NO')}}
                </template>
            </dynamic-table>-->
                <div class="row">
                    <div class="col-md-2 pt-1 text-end no-right-padding">
                        {{ $t('inbound.detail.increaseModalPartNo') }}
                    </div>
                    <div class="col-md-7">
                        <input type="text" :value="model.productCode" class="form-control form-control-sm" disabled />
                    </div>
                    <div class="col-md-1 pt-1 text-end no-right-padding">
                        #
                    </div>
                    <div class="col-md-2">
                        <input type="text" :value="model.lineItem" class="form-control form-control-sm" disabled />
                    </div>
                </div>

                <div class="row mt-2">
                    <div class="col-md-2 pt-1 text-end no-right-padding">
                        {{ $t('inbound.detail.increaseModalQty') }}
                    </div>
                    <div class="col-md-3">
                        <input type="text" :value="model.qty" class="form-control form-control-sm" disabled />
                    </div>
                    <div class="col-md-1 pt-1 text-center">
                        +
                    </div>
                    <div class="col-md-3">
                        <input type="text" v-model="qty" class="form-control form-control-sm" />
                    </div>
                </div>
                <div class="row mt-2">
                    <div class="col-md-6">&nbsp;</div>
                    <div class="col-md-6">
                        <form-error :errors="errors" field="Qty" />
                    </div>
                </div>

                <div class="row mt-2">
                    <div class="col-md-2 pt-1 text-end no-right-padding">
                        {{ $t('inbound.detail.increaseModalQtyPerPkg') }}
                    </div>
                    <div class="col-md-3">
                        <input type="text" :value="qtyPerPkg" class="form-control form-control-sm" disabled />
                    </div>
                </div>

                <div class="row mt-2">
                    <div class="col-md-2 pt-1 text-end no-right-padding">
                        {{ $t('inbound.detail.increaseModalNoOfPkg') }}
                    </div>
                    <div class="col-md-3">
                        <input type="text" :value="model.noOfPackage" class="form-control form-control-sm" disabled />
                    </div>
                    <div class="col-md-1 pt-1 text-center">
                        +
                    </div>
                    <div class="col-md-3">
                        <input type="text" :value="pkgToAdd" class="form-control form-control-sm" disabled />
                    </div>
                </div>

            </template>
            <template name="footer" v-slot:footer>
                <button class="btn btn-primary btn-sm me-2" type="button" @click.stop="modal = { type: 'confirm' }" :disabled="processing">
                    <i class="las la-save"></i> {{$t('inbound.operation.save')}}
                </button>
                <button class="btn btn-secondary btn-sm" type="button" @click.stop="$emit('close')" :disabled="processing">
                    <i class="las la-times"></i> {{$t('inbound.operation.close')}}
                </button>
            </template>
        </modal>
        <confirm v-if="modal != null && modal.type == 'confirm'" @ok="process()" @close="modal = null">
            {{$t('inbound.operation.increase_pkg_qty')}}
        </confirm>
        <alert v-if="alert" @close="alert = null, $emit('done')" :text="alert" />
    </div>
</template>
<script>

    import FormError from '@/widgets/FormError'
    import Confirm from '@/widgets/Confirm.vue'
    import Alert from '@/widgets/Alert.vue'
    import { defineComponent } from 'vue';
export default defineComponent({
        props: ['model', 'jobNo'],
        components: { FormError, Confirm, Alert },
        data() {
            return {
                alert: null,
                modal: null,
                errors: {},
                qty: null,
                processing: false
            }
        },
        watch: {
            'qty'(v) {
                if (v % this.qtyPerPkg != 0) {
                    this.errors = { 'Qty': 'ProvideValidEntryQtyDivisableByPackage' }
                }
                else {
                    this.errors = {}
                }
            }

        },
        computed: {
            qtyPerPkg() {
                return this.model.qty / this.model.noOfPackage
            },
            pkgToAdd() {
                if (this.qty % this.qtyPerPkg != 0) {
                    return null
                }
                else {
                    return this.qty / this.qtyPerPkg
                }
            }
        },
        methods: {
            process() {
                this.processing = true
                this.modal = null
                this.$dataProvider.inbounds.increasePkgQty({ jobNo: this.jobNo, lineItem: this.model.lineItem, qty: this.qty })
                    .then(() => {
                        this.alert = this.$t('inbound.operation.QtyRevised')
                    })
                    .finally(() => this.processing = false)
            }
        }

    })
</script>

<style lang="scss">
    .modal-create-inbound-increase {
        .modal-container {
            width: 50vw !important;
        }
    }
</style>