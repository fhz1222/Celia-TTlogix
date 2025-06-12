<template>
    <div v-if="model != null">

        <div class="card text-dark bg-light">
            <div class="card-header">
                <h5>{{$t('inbound.header.inboundHeader')}}</h5>
            </div>
            <div class="card-body small-font">
                <!-- Customer -->
                <div class="row">
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.customer')}}</div>
                    <div class="col-md-10 col-lg-8 mt-2">
                        <div class="input-group">
                            <input type="text" aria-label="First name" class="form-control form-control-sm w-25" :value="model.customerCode" disabled />
                            <input type="text" aria-label="Last name" class="form-control form-control-sm w-75" :value="model.customerName" disabled />
                        </div>
                    </div>
                </div>

                <!-- Supplier -->
                <div class="row">
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.supplier')}}</div>
                    <div class="col-md-10 col-lg-8 mt-2">
                        <div class="input-group">
                            <input type="text" aria-label="First name" class="form-control form-control-sm w-25" :value="model.supplierID" disabled />
                            <input type="text" aria-label="Last name" class="form-control form-control-sm w-75" :value="model.supplierName" disabled />
                        </div>
                    </div>
                </div>

                <!-- Job, Warehouse, Created Date, Currency -->
                <div class="row">
                    <!-- Job No -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.jobNo')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.jobNo" disabled /></div>

                    <!-- Warehouse -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.warehouse')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.whsCode" disabled /></div>

                    <!-- Created Date -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.createdDate')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <div class="form-control form-control-sm disabled"><date-time :value="model.createdDate" /></div>
                    </div>

                    <!-- Currency -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.currency')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2 multiselect-as-select">
                        <select-currency v-model="model.currency" />
                        <form-error :errors="errors" field="Currency" />
                    </div>
                </div>

                <!-- Job Status, ASN, Created By, Total ASN Value -->
                <div class="row">
                    <!-- Job Status -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.jobStatus')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.statusString" disabled /></div>

                    <!-- ASN -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.asn')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.irNo" disabled /></div>

                    <!-- Created By -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.createdBy')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.createdByName" disabled /></div>

                    <!-- Total ASN Value -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.totalASNValue')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.totalASNValue" disabled /></div>
                </div>

                <!-- Job Type, IM7/Ref No., ETA, Total Residual Value -->
                <div class="row">
                    <!-- Job Type -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.jobType')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.transTypeString" disabled /></div>

                    <!-- IM7/Ref No. -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.im7RefNo')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.refNo" :disabled="isRefNoDisabled"/>
                        <form-error :errors="errors" field="RefNo" />
                    </div>

                    <!-- ETA -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.eta')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <date-picker :date="model.eta" @set="(date) => {model.eta = date}" :mandatory="true"></date-picker>
                        <form-error :errors="errors" field="Eta" />
                    </div>

                    <!-- Total Residual Value -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end long-label no-right-padding">{{$t('inbound.header.totalResidualValue')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.totalResidualValue" disabled /></div>
                </div>

                <!-- Remark, IM4 No. -->
                <div class="row">
                    <!-- Remark -->
                    <div class="col-md-1 text-end mt-2 pt-1 no-right-padding">{{$t('inbound.header.remark')}}</div>
                    <div class="col-md-5 mt-2">
                        <textarea class="form-control form-control-sm" v-model="model.remark" :disabled="isRemarkDisabled"></textarea>
                        <form-error :errors="errors" field="Remark" />
                    </div>

                    <!-- IM4 No. -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.im4No')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.iM4No" />
                        <form-error :errors="errors" field="Im4No" />
                    </div>
                    
                    <!-- ETA -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.im4Date')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <date-picker :date="model.customsDeclarationDate" @set="(date) => {model.customsDeclarationDate = date}" />
                        <form-error :errors="errors" field="CustomsDeclarationDate" />
                    </div>
                </div>

            </div>

            <div class="card-footer">
                <!-- Butons -->
                <div class="row">
                    <div class="col-md-12 text-end">
                        <awaiting-button v-if="!isSaveDisabled"
                                         @proceed="(callback) => { $emit('save', callback) }"
                                         :btnText="'inbound.operation.save'"
                                         :btnIcon="'las la-save'"
                                         :btnClass="'btn-primary'">
                        </awaiting-button>

                        <button class="btn btn-danger btn-sm" @click="$emit('cancel')"><i class="las la-ban"></i> {{$t('inbound.operation.cancelInbound')}}</button>


                    </div>
                </div>
            </div>
        </div>
 
    </div>
</template>
<script>
    import FormError from '@/widgets/FormError'
    import DateTimeField from '@/widgets/DateTime'
    import SelectCurrency from '@/widgets/SelectCurrency'
    import { toServer, fromServer } from '@/service/date_converter.js'
    import { defineComponent, getCurrentInstance } from 'vue';
    import AwaitingButton from '@/widgets/AwaitingButton'
    import DatePicker from '@/widgets/DatePicker';
export default defineComponent({
        props: ['modelValue', 'errors'],
        components: { FormError, dateTime: DateTimeField, SelectCurrency, AwaitingButton, DatePicker },
        created(){
            this.model = this.modelValue;
        },
        data() {
            return {
                model: null,
                parent: null,
                open: false
            }
        },
        watch: {
            model: {
                deep: true,
                handler() {
                    this.$emit('update:modelValue', this.model)
                }
            },
            modelValue() {
                this.model = this.modelValue
            }
        },
        computed: {
            isSaveDisabled() { return this.model && this.model.statusString == 'Cancelled' },
            isRemarkDisabled() { return this.model && ['Completed', 'Cancelled'].includes(this.model.statusString) },
            isRefNoDisabled() { return this.model && ['Completed', 'Cancelled'].includes(this.model.statusString) },
            isETADisabled() { return this.model && ['Completed', 'Cancelled'].includes(this.model.statusString) },

        },
        methods: {
            toServer,
            fromServer,
            inputNoOfPallet(v) {
                let val = parseInt(v.target.value, 10);
                this.model.noOfPallet = isNaN(val) ? 0 : val
            }
        }
    })
</script>