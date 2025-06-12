<template>
    <modal containerClass="modal-create-manual">
        <template name="header" v-slot:header>
            {{$t('outbound.detail.outboundDetailItem')}}
        </template>
        <template name="body" v-slot:body class="small-font">
            <div class="row">
                <div class="col-md-2 text-end pt-1 no-right-padding small-font">
                    {{$t('outbound.picking.supplier')}}
                </div>
                <div class="col-md-10">
                    <v-select v-model="model.supplierID" :options="suppliers" :reduce="p => p.supplierID" :disabled="details.length > 0">
                        <template v-slot:option="option">
                            <span :style="{display: 'inline-block', width: idLength*9 + 'px'}">{{option.supplierID}}</span>
                            | {{option.companyName}}
                        </template>
                    </v-select>
                    <form-error :errors="errors" field="SupplierID" />
                </div>
            </div>

            <div class="row mt-2">
                <div class="col-md-2 text-end pt-1 no-right-padding small-font">
                    {{$t('outbound.picking.productCode')}}
                </div>
                <div class="col-md-10">
                    <v-select v-model="model.productCode" :options="products" label="productCode1" :reduce="p => p.productCode1" :disabled="model.supplierID == null" >
                        <slot name="no-options">
                            <span>
                                {{loading ? $t('outbound.picking.loadingProducts') : $t('outbound.picking.noProducts')}}
                            </span>
                        </slot>
                    </v-select>
                    <form-error :errors="errors" field="ProductCode" />
                </div>
            </div>

            <div class="row mt-2">
                <div class="col-md-2 text-end pt-1 no-right-padding small-font">
                    {{$t('outbound.picking.description')}}
                </div>
                <div class="col-md-10">
                    <input class="form-control form-control-sm" :value="selectedProduct ? selectedProduct.description : null" disabled />
                </div>
            </div>

            <div class="row mt-2">
                <div class="col-md-2 text-end pt-1 no-right-padding small-font">
                    {{$t('outbound.picking.spq')}}
                </div>
                <div class="col-md-4">
                    <input class="form-control form-control-sm" :value="selectedProduct ? selectedProduct.spq : null" disabled />
                </div>
            </div>

            <div class="row mt-2">
                <div class="col-md-2 text-end pt-1 no-right-padding small-font">
                    {{$t('outbound.picking.avQty')}}
                </div>
                <div class="col-md-4">
                    <input class="form-control form-control-sm" :value="selectedProduct ? selectedProduct.onHandQty : null" disabled />
                </div>
            </div>

            <div class="row mt-2">
                <div class="col-md-2 text-end pt-1 no-right-padding small-font">
                    {{$t('outbound.picking.supQty')}}
                </div>
                <div class="col-md-4">
                    <input class="form-control form-control-sm" :value="selectedProduct ? selectedProduct.supplierQty : null" disabled />
                </div>
                <div class="col-md-2 text-end pt-1 no-right-padding small-font">
                    {{$t('outbound.picking.ehpQty')}}
                </div>
                <div class="col-md-4">
                    <input class="form-control form-control-sm" :value="selectedProduct ? selectedProduct.ehpQty : null" disabled />
                </div>
            </div>

            <div class="row mt-2">
                <div class="col-md-2 text-end pt-1 no-right-padding small-font">
                    {{$t('outbound.picking.qtyToPick')}}
                </div>
                <div class="col-md-4">
                    <input class="form-control form-control-sm" v-model.number="model.qty" />
                    <form-error :errors="errors" field="Qty" />
                </div>
            </div>
        </template>

        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end no-right-padding">
                    <button class="btn btn-sm btn-primary me-2" type="button" @click.stop="process()">
                        <i class="las la-plus-circle"></i> {{$t('outbound.operation.create')}}
                    </button>
                    <button class="btn btn-sm btn-secondary" type="button" @click.stop="$emit('close')">
                        <i class="las la-times"></i> {{$t('outbound.operation.close')}}
                    </button>
                </div>
            </div>
        </template>
    </modal>
</template>
<script>
import FormError from '@/widgets/FormError'
import { defineComponent } from 'vue';
export default defineComponent({
    props: {
        job: null,
        details: null
    },
    created() {
       
        this.$dataProvider.supplierMasters.getList(this.job.customerCode).then(resp => {
            this.idLength = Math.max(...resp.map(s => s.supplierID.length))
            this.suppliers = resp.map(s => { return { label: `${s.supplierID} | ${s.companyName}`,...s}})
        })
        if (this.model.supplierID) {
            this.loadProducts()
        }
    },
    components: {FormError},
    data() {
        let supplier = null
        if (this.details.length) {
            supplier = this.details[0].supplierID
        }
        return {
            suppliers: [],
            idLength: 0,
            products: [],
            selectedProduct : null,
            model : {
                jobNo : this.job.jobNo,
                productCode : null,
                supplierID : supplier,
                qty : 0
            },
            errors: {},
            loading: false
        }
    },
    methods: {
        process() {
            this.errors = []
           if (this.model.supplierID == null) {
               this.errors = { SupplierID: 'PleaseSelectSupplier' }
               return
           }
           else if(this.selectedProduct.onHandQty < this.model.qty){
               this.errors = { Qty : 'PickingQtyMustBeGreaterThanAvQty'}
               return
            }
           else if(this.model.qty <= 0){
               this.errors = { Qty: 'PleaseProvideAValidPickingQty'}
               return
            }

            this.$dataProvider.outbounds.addNewOutboundDetail(this.model).then(resp => {
                this.$emit('done', resp)
            }).catch(e => {
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                } else if (e.response && e.response.data.length) {
                    this.errors = { ProductCode: e.response.data }
                }
            })
        },
        loadProducts() {
            this.model.productCode = null
            this.products = []
            if (!this.model.supplierID) {
                return
            }
            this.loading = true;
            this.$dataProvider.outbounds.getOutboundPickableList({
                    outboundTransType : this.job.transType,
                    customerCode : this.job.customerCode,
                    supplierID : this.model.supplierID,
                    WHSCode : this.$auth.user().whsCode,
                    partMasterStatus: 1,
                    onlyOnHand: true

                }
            ).then(resp => {
                this.products = resp
                this.loading = false
            })
        }
    },
    watch: {
        'model.supplierID'() {
            this.loadProducts()
        },
        'model.productCode'() {
            this.selectedProduct = null
            if (this.model.productCode) {
                this.selectedProduct = this.products.find(e => e.productCode1 == this.model.productCode)
            }
        },
        'products'() {
            this.selectedProduct = null
            if (this.model.productCode) {
                this.selectedProduct = this.products.find(e => e.productCode1 == this.model.productCode)
            }
        }
    }
})
</script>
<style lang="scss">
.modal-create-manual {
    .modal-container {
        width: 700px;
    }
}
</style>