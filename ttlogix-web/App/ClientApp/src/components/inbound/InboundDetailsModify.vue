<template>
    <div>
        <modal containerClass="modal-create-inbound-decrease">
            <template name="header" v-slot:header>
                {{ $t('inbound.detail.modifyModalTitle') }}
            </template>
            <template name="body" v-slot:body>

                <!--<div class="scrollable">-->
                <div><strong>{{ $t('inbound.detail.modifyModalProductInfoTitle') }}</strong></div>
                <div class="card">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-md-2 pt-1 text-end no-right-padding">{{ $t('inbound.detail.modifyModalPartNo') }}</div>
                            <div class="col-md-10">
                                <select class="form-select form-select-sm" v-model="model.productCode" :disabled="modifyModel">
                                    <option v-for="part in parts" :value="part.productCode1" :key="part.productCode1">
                                        {{ part.productCode1 }} | {{ part.description }}
                                    </option>
                                </select>
                                <form-error :errors="errors" field="ProductCode" />
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-md-2 pt-1 text-end no-right-padding">{{ $t('inbound.detail.modifyModalQty') }}</div>
                            <div class="col-md-4">
                                <input type="text" v-model="model.qty" @change="onQtyChange()" class="form-control form-control-sm" :disabled="modifyModel">
                                <form-error :errors="errors" field="Qty" />
                            </div>
                            <div class="col-md-2 pt-1 text-end no-right-padding">{{ $t('inbound.detail.modifyModalPreInformQty') }}</div>
                            <div class="col-md-4">
                                <input type="text" v-model="model.importedQty" class="form-control form-control-sm" :disabled="modifyModel">
                                <form-error :errors="errors" field="ImportedQty" />
                            </div>
                        </div>
                        <div class="row mt-2">
                            <div class="col-md-2 text-end no-right-padding">
                                {{ $t('inbound.detail.modifyModalRemark') }}
                                <br /><small>{{ $t('inbound.detail.modifyModalMax100') }}</small>
                            </div>
                            <div class="col-md-10">
                                <textarea v-model="model.remark" class="form-control form-control-sm" :disabled="modifyModel"></textarea>
                                <form-error :errors="errors" field="Remark" />
                            </div>
                        </div>
                    </div>
                </div>

                <div class="row mt-3">

                    <div class="col-md-4">
                        <div><strong>{{ $t('inbound.detail.modifyModalPackingDetailTitle') }}</strong></div>
                        <div class="card">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-4 pt-1 text-end no-right-padding">{{ $t('inbound.detail.modifyModalLength') }}</div>
                                    <div class="col-md-8">
                                        <input type="text" v-model="model.length" class="form-control form-control-sm" :disabled="modifyModel">
                                        <form-error :errors="errors" field="Length" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-4 pt-1 text-end no-right-padding">{{ $t('inbound.detail.modifyModalWidth') }}</div>
                                    <div class="col-md-8">
                                        <input type="text" v-model="model.width" class="form-control form-control-sm" :disabled="modifyModel">
                                        <form-error :errors="errors" field="Width" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-4 pt-1 text-end no-right-padding">{{ $t('inbound.detail.modifyModalHeight') }}</div>
                                    <div class="col-md-8">
                                        <input type="text" v-model="model.height" class="form-control form-control-sm" :disabled="modifyModel">
                                        <form-error :errors="errors" field="Height" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-4 pt-1 text-end no-right-padding">{{ $t('inbound.detail.modifyModalM3Package') }}</div>
                                    <div class="col-md-8">
                                        <input type="text" :value="m3" class="form-control form-control-sm" disabled>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-4">
                        <div><strong>{{ $t('inbound.detail.modifyModalPackageTitle') }}</strong></div>
                        <div class="card">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-5 pt-1 text-end no-right-padding">{{ $t('inbound.detail.modifyModalSPQ') }}</div>
                                    <div class="col-md-7">
                                        <input type="text" :value="model.spq" class="form-control form-control-sm" disabled>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-5 pt-1 text-end no-right-padding">{{ $t('inbound.detail.modifyModalQtyPerBox') }}</div>
                                    <div class="col-md-7">
                                        <input type="text" v-model="model.qtyPerPkg" class="form-control form-control-sm">
                                        <form-error :errors="errors" field="QtyPerPkg" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-5 pt-1 text-end no-right-padding">{{ $t('inbound.detail.modifyModalNoOfPackages') }}</div>
                                    <div class="col-md-7">
                                        <input type="text" v-model="model.noOfPackage" class="form-control form-control-sm" disabled>
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-5 pt-1 text-end no-right-padding">{{ $t('inbound.detail.modifyModalNoOfLabels') }}</div>
                                    <div class="col-md-7">
                                        <input type="text" v-model="model.noOfLabel" class="form-control form-control-sm" disabled>
                                    </div>
                                </div>
                            </div>
                        </div>


                    </div>

                    <div class="col-md-4">
                        <div><strong>{{ $t('inbound.detail.modifyModalCCITitle') }}</strong></div>
                        <div class="card">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-md-4 pt-1 text-end no-right-padding">{{ controlCodes.cC1Name ? controlCodes.cC1Name : '.............'}}</div>
                                    <div class="col-md-8">
                                        <input type="text" v-model="model.controlCode1" class="form-control form-control-sm" :disabled="!controlCodes.cC1Name || modifyModel" />
                                        <form-error :errors="errors" field="ControlCode1" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-4 pt-1 text-end no-right-padding">{{ controlCodes.cC2Name ? controlCodes.cC2Name : '.............'}}</div>
                                    <div class="col-md-8">
                                        <input type="text" v-model="model.controlCode2" class="form-control form-control-sm" :disabled="!controlCodes.cC2Name || modifyModel" />
                                        <form-error :errors="errors" field="ControlCode2" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-4 pt-1 text-end no-right-padding">{{ controlCodes.cC3Name ? controlCodes.cC3Name : '.............'}}</div>
                                    <div class="col-md-8">
                                        <input type="text" v-model="model.controlCode3" class="form-control form-control-sm" :disabled="isControlCode3Disabled" />
                                        <form-error :errors="errors" field="ControlCode3" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-4 pt-1 text-end no-right-padding">{{ controlCodes.cC4Name ? controlCodes.cC4Name : '.............'}}</div>
                                    <div class="col-md-8">
                                        <input type="text" v-model="model.controlCode4" class="form-control form-control-sm" :disabled="!controlCodes.cC4Name || modifyModel" />
                                        <form-error :errors="errors" field="ControlCode4" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-4 pt-1 text-end no-right-padding">{{ controlCodes.cC5Name ? controlCodes.cC5Name : '.............'}}</div>
                                    <div class="col-md-8">
                                        <input type="text" v-model="model.controlCode5" class="form-control form-control-sm" :disabled="!controlCodes.cC5Name || modifyModel" />
                                        <form-error :errors="errors" field="ControlCode5" />
                                    </div>
                                </div>
                                <div class="row mt-2">
                                    <div class="col-md-4 pt-1 text-end no-right-padding">{{ controlCodes.cC6Name ? controlCodes.cC6Name : '.............'}}</div>
                                    <div class="col-md-8">
                                        <input type="text" v-model="model.controlCode6" class="form-control form-control-sm" :disabled="!controlCodes.cC6Name || modifyModel" />
                                        <form-error :errors="errors" field="ControlCode6" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!--</div>-->


            </template>
            <template name="footer" v-slot:footer>
                <button class="btn btn-primary btn-sm me-2" type="button" @click.stop="process(false)" :disabled="processing">
                    <i class="las la-check-circle"></i> {{$t('inbound.operation.ok')}}
                </button>
                <button class="btn btn-secondary btn-sm me-2" type="button" @click.stop="$emit(anythingAdded ? 'done' : 'close')" :disabled="processing">
                    <i class="las la-times"></i> {{$t('inbound.operation.close')}}
                </button>
                <button class="btn btn-primary btn-sm" type="button" @click.stop="process(true)" v-if="!modifyModel" :disabled="processing">
                    <i class="las la-arrow-right"></i> {{$t('inbound.operation.next')}}
                </button>
            </template>
        </modal>
    </div>
</template>
<script>
    import FormError from '@/widgets/FormError'
    import { defineComponent } from 'vue';
export default defineComponent({
        components: { FormError },
        props: ['controlCodes', 'customerCode', 'supplierID', 'jobNo', 'modifyModel'],
        created() {
            this.$dataProvider.partMasters.getPartMasterListBySupplier(this.customerCode, this.supplierID)
                .then(resp => this.parts = resp)
            this.$dataProvider.settings.get().then(resp => this.settings = resp)
            if (this.modifyModel) this.model = this.modifyModel
            else this.reset()
        },
        data() {
            return {
                errors: {},
                processing: false,
                anythingAdded: false,
                model: {},
                parts: [],
                settings: null
            }
        },
        watch: {
            'model.productCode'(v) {
                let part = this.parts.find(x => x.productCode1 == v)
                if (part) {
                    this.model.height = part.height
                    this.model.width = part.width
                    this.model.length = part.length
                    this.model.spq = part.spq
                    this.model.qtyPerPkg = part.spq
                    this.model.netWeight = part.netWeight
                    this.model.grossWeight = part.grossWeight
                    this.model.packageType = part.packageType
                    this.model.partMasterDecimalNum = part.decimalNum
                    this.model.partMasterSPQ = part.spq
                }
            },
            'model.qtyPerPkg'() {
                this.updatenNoOfPackage()
            }
        },
        computed: {
            m3() {
                let val = (this.model.height / 100) * (this.model.width / 100) * (this.model.length / 100)
                return Math.round((Number.parseFloat(val) + Number.EPSILON) * 1000000) / 1000000

            },
            isControlCode3Disabled() {
                return !this.controlCodes.cC3Name || (this.modifyModel && (!this.settings || this.settings.ownerCode != 'TTKL'))
            }
        },
        methods: {
            updatenNoOfPackage() {
                if (this.model.qtyPerPkg == 0) {
                    this.errors = {}
                    this.model.noOfPackage = 0
                }
                else if (this.model.qty % this.model.qtyPerPkg == 0) {
                    this.errors = {}
                    this.model.noOfPackage = this.model.qty / this.model.qtyPerPkg
                }
                else {
                    this.errors = { 'QtyPerPkg': 'ProvideValidEntryQtyDivisableToQty'}
                    this.model.noOfPackage = null
                }
                this.model.noOfLabel = this.model.noOfPackage
            },
            onQtyChange() {
                if (this.model.qty < this.model.qtyPerPkg) this.model.qtyPerPkg = this.model.qty
                this.updatenNoOfPackage()
            },
            process(next) {
                this.processing = true
                const f = this.modifyModel
                    ? (d) => this.$dataProvider.inbounds.modifyInboundDetail(d)
                    : (d) => this.$dataProvider.inbounds.createInboundDetail(d)
                f(this.model)
                    .then(() => {
                        this.anythingAdded = true
                        if (next) {
                            this.reset()
                        }
                        else {
                            this.$emit('done')
                        }
                    })
                    .catch(e => {
                        if (e.response && e.response.data.errors) {
                            this.errors = e.response.data.errors
                        }
                    })
                    .finally(() => this.processing = false)
            },
            reset() {
                this.errors = {}
                this.model = {
                    productCode: null,
                    height: 0,
                    width: 0,
                    length: 0,
                    spq: null,
                    qty: 0,
                    importedQty: 0,
                    remark: "",
                    qtyPerPkg: 0,
                    noOfPackage: 0,
                    noOfLabel: 0,
                    netWeight: 0,
                    grossWeight: 0,
                    packageType: 0,
                    controlCode1: null,
                    controlCode2: null,
                    controlCode3: null,
                    controlCode4: null,
                    controlCode5: null,
                    controlCode6: null,
                    partMasterDecimalNum: null,
                    partMasterSPQ: null,
                    jobNo: this.jobNo
                }
            }
        }
})
</script>

<style lang="scss">
    .modal-create-inbound-decrease {
        .modal-container {
            width: 97vw;
        }
    }
</style>