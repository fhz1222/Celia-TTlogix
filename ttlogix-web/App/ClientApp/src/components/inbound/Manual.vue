<template>
    <div>
        <modal containerClass="modal-create-manual-inbound">
            <template name="header" v-slot:header>
                {{$t('inbound.operation.manual_title')}}
            </template>
            <template name="body" v-slot:body>
                <div class="form-row">
                    <label class="form-label">{{$t('inbound.detail.customer')}}</label>
                    <div class="form-input full multiselect-as-select">
                        <select-customer v-model="model.customerCode" :disabled="processing" />
                        <form-error :errors="errors" field="CustomerCode" />
                    </div>
                </div>
                <div class="form-row">
                    <label class="form-label">{{$t('inbound.detail.supplier')}}</label>
                    <div class="form-input full">
                        <v-select v-model="model.supplierID" :options="suppliers" :reduce="p => p.supplierID">
                            <template v-slot:option="option">
                                <span :style="{display: 'inline-block', width: idLength*9 + 'px'}">{{option.supplierID}}</span>
                                | {{option.companyName}}
                            </template>
                        </v-select>
                        <form-error :errors="errors" field="SupplierID" />
                    </div>
                </div>

                <div class="form-row">
                    <label class="form-label">{{$t('inbound.detail.warehouse')}}</label>
                    <div class="form-input">
                        <input type="text" class="input" :value="model.whsCode" disabled />
                        <form-error :errors="errors" field="WhsCode" />
                    </div>
                    <label class="form-label">{{$t('inbound.detail.receiptDate')}}</label>
                    <div class="form-input">
                        <date-picker :date="model.receiptDate" @set="(date) => {model.receiptDate = date}" :disabled="true" />
                        <form-error :errors="errors" field="ReceiptDate" />
                    </div>
                </div>

                <div class="form-row">
                    <label class="form-label">{{$t('inbound.detail.refNo')}}*</label>
                    <div class="form-input">
                        <input type="text" class="input" v-model="model.refNo" :disabled="processing" />
                        <form-error :errors="errors" field="RefNo" />
                    </div>
                    <label class="form-label">{{$t('inbound.detail.eta')}}*</label>
                    <div class="form-input">
                        <date-picker :date="model.eta" @set="(date) => {model.eta = date}" />
                        <form-error :errors="errors" field="Eta" />
                    </div>
                </div>
                <div class="form-row">
                    <label class="form-label">{{$t('inbound.detail.remark')}}</label>
                    <div class="form-input full">
                        <textarea class="input" style="resize:vertical; max-height: 150px" v-model="model.remark" :disabled="processing"></textarea>
                        <form-error :errors="errors" field="Remark" />
                    </div>
                </div>
                <div class="form-row">
                    <label class="form-label" style="color: red">{{$t('inbound.detail.jobType')}}*</label>
                    <div class="form-input full">
                        <v-select v-model="model.transType" :options="transTypeOptions" :reduce="p => p.type"></v-select>
                        <form-error :errors="errors" field="transType" />
                    </div>
                </div>
            </template>
            <template name="footer" v-slot:footer>
                <button class="btn btn-primary" type="button" @click.stop="process()" :disabled="processing">
                    {{$t('inbound.operation.create')}}
                </button>
                <button class="btn btn-secondary" type="button" @click.stop="$emit('close')" :disabled="processing">
                    {{$t('inbound.operation.close')}}
                </button>
            </template>
        </modal>
        <confirm v-if="modal != null && modal.type == 'manual_1'" :title="$t('inbound.creation_warnings.title')" @ok="modal = {type: 'manual_2'}" @close="modal = null">
            {{$t('inbound.creation_warnings.manual_1')}}
        </confirm>
        <confirm v-if="modal != null && modal.type == 'manual_2'" :title="$t('inbound.creation_warnings.title')" @ok="modal = null; createInbound()" @close="modal = null">
            {{$t('inbound.creation_warnings.manual_2')}}
        </confirm>
        <confirm v-if="modal != null && modal.type == 'qret'" :title="$t('inbound.creation_warnings.title')" @ok="modal = null; createInbound()" @close="modal = null">
            {{$t('inbound.creation_warnings.qret')}}
            <template v-if="modal.isVMI">
                <br />
                <br />
                {{$t('inbound.creation_warnings.qret_vmi')}}
            </template>
        </confirm>
        <confirm v-if="modal != null && modal.type == 'eret'" :title="$t('inbound.creation_warnings.title')" @ok="modal = null; createInbound()" @close="modal = null">
            {{$t('inbound.creation_warnings.eret')}}
        </confirm>
    </div>
</template>
<script>
import FormError from '@/widgets/FormError'
    import SelectCustomer from '@/widgets/SelectCustomer'
    import { toServer, fromServer } from '@/service/date_converter.js'
    import inboundTypes from '@/enum/inbound_type.js'
    import Confirm from '@/widgets/Confirm.vue'
    import DatePicker from '@/widgets/DatePicker';
export default {
    components: {FormError, SelectCustomer, Confirm, DatePicker},
    data() {
        return {
            modal: null,
            model: {
                customerCode : null,
                supplierID: null,
                whsCode : this.$auth.user().whsCode,
                receiptDate: new Date(),
                eta : null,
                refNo : null,
                remark : null,
                transType : null,
            },
            transTypeOptions: [
                { type: inboundTypes.ManualEntry, label: this.$t('inbound.type.Manual') },
                { type: inboundTypes.Return, label: this.$t('inbound.type.Return') }
            ].concat(this.$auth.user().whsCode.toLowerCase() == "de" ? [] : [{ type: inboundTypes.Excess, label: this.$t('inbound.type.Excess') }]),
            suppliers: [],
            errors: {},
            processing: false
        }
        },
        created() {
        },
        watch: {
            'model.customerCode'(v) {
                this.model.supplierID = null;
                this.$dataProvider.supplierMasters.getList(v).then(resp => {
                    this.idLength = Math.max(...resp.map(s => s.supplierID.length))
                    this.suppliers = resp.map(s => { return { label: `${s.supplierID} | ${s.companyName}`, ...s } })
                })
            }
        },
    methods: {
        process() {
            if (this.model.transType == inboundTypes.ManualEntry) this.modal = { type: 'manual_1' }
            if (this.model.transType == inboundTypes.Return) {
                let supp = this.suppliers.find(x => x.supplierID == this.model.supplierID)
                this.modal = { type: 'qret', isVMI: supp ? supp.isVMI : false }
            }
            if (this.model.transType == inboundTypes.Excess) this.modal = { type: 'eret' }
        },
        createInbound() {
            this.processing = true
            this.$dataProvider.inbounds.createInboundManual(this.model).then(resp => {
                this.$emit('done', resp)
            }).catch(e => {
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                }
            }).finally(() => this.processing = false)
        },
        toServer,
        fromServer
    }
}
</script>
<style lang="scss">
    .modal-create-manual-inbound {
        .modal-container

    {
        width: 760px;
    }
    }
</style>