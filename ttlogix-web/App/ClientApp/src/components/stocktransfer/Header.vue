<template>
    <div v-if="model != null">
        <div class="card text-dark bg-light">
            <div class="card-header">
                <h5>{{$t('stockTransferDetailHeaderTitle')}}</h5>
            </div>
            <div class="card-body small-font">

                <!-- Customer Name -->
                <div class="row">
                    <!-- Customer Name -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('customerHeader')}}</div>
                    <div class="col-md-8 col-lg-5 mt-2">
                        <input type="text" class="form-control form-control-sm" :value="model.customerCode" disabled />
                        <form-error :errors="errors" field="CustomerCode" />
                    </div>
                </div>

                <!-- Job No, Created by, Confirmed by, Currency -->
                <div class="row">

                    <!-- Job No -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('jobNoHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.jobNo" disabled />
                        <form-error :errors="errors" field="JobNo" />
                    </div>

                    <!-- Created by -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('createdByHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" :value="model.createdBy" disabled />
                        <form-error :errors="errors" field="CreatedBy" />
                    </div>

                    <!-- Confirmed by -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('confirmedByHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" :value="model.confirmedBy" disabled />
                        <form-error :errors="errors" field="ConfirmedBy" />
                    </div>

                    <!-- Currency -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('currencyHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" :class="model.isMixedCurrency && detailsLength > 0 ? 'input-alert' : ''" :value="model.currency != null ? model.currency : detailsLength > 0 ? 'MIXED' : ''" disabled />
                        <form-error :errors="errors" field="Currency" />
                    </div>
                </div>

                <!-- Status, Created date, Confirmed Date, Total Outbound -->
                <div class="row">

                    <!-- Status -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('statusHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" :value="$t('stockTransferStatus.' + model.statusString)" disabled />
                        <form-error :errors="errors" field="Status" />
                    </div>  

                    <!-- Created date -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('createdDateHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <div class="form-control form-control-sm disabled"><date-time :value="model.createdDate" /></div>
                    </div>

                    <!-- Confirmed Date -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('confirmedDateHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <div class="form-control form-control-sm disabled"><date-time :value="model.confirmedDate" /></div>
                    </div>

                    <!-- Total Outbound -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('totalOutboundHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" :value="model.outboundTotalValue" disabled />
                    </div>
                </div>

                <!-- WHS Code, Ref No, Comm Inv No, Comm Inv Date -->
                <div class="row">

                    <!-- WHS Code -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('whsCodeHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.whsCode" disabled />
                        <form-error :errors="errors" field="WHSCode" />
                    </div>

                    <!-- Ref No -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('refNoHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.refNo" :disabled="isRefNoDisabled" />
                        <form-error :errors="errors" field="RefNo" />
                    </div>

                    <!-- Comm Inv No -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('commInvNoHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.commInvNo" />
                        <form-error :errors="errors" field="CommInvNo" />
                    </div>

                    <!-- Comm Inv Date -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('commInvDateHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <date-picker :type="'date'" :date="model.commInvDate" @set="(date) => {model.commInvDate = date}" :mandatory="false"></date-picker>
                        <form-error :errors="errors" field="CommInvDate" />
                    </div>
                </div>

                <!-- Remarks, Transfer Type -->
                <div class="row multiselect-as-select">

                    <!-- Transfer Type -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('transferTypeHeader')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <multiselect v-model="model.transferTypeString"
                                     :options="transferTypes"
                                     :close-on-select="true"
                                     :can-clear="false"
                                     :can-deselect="false"
                                     placeholder="Select"
                                     :customLabel="(x)=>$t('stockTransferType.' + x)"
                                     select-label=""
                                     deselect-label=""
                                     selected-label=""
                                     :disabled="isTransferTypeDisabled"
                                     :classes="{container: 'form-control-sm multiselect'}"
                                     >
                        </multiselect>
                        <form-error :errors="errors" field="TransferType" />
                    </div>
                    <!-- Remarks -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('remarkHeader')
                        }}
                    </div>
                    <div class="col-md-8 col-lg-5 mt-2">
                        <textarea class="form-control form-control-sm" v-model="model.remark" style="height: 100%" :disabled="isRemarkDisabled"></textarea>
                        <form-error :errors="errors" field="Remark" />
                    </div>
                </div>


            </div>

            <div class="card-footer">
                <!-- Buttons -->
                <div class="row">
                    <div class="col text-end">
                        <awaiting-button v-if="model.status != 8 && model.status != 10 && model.status != 0"
                                         @proceed="(callback) => { $emit('complete', callback) }"
                                         :btnText="'partsmaster.operation.complete'"
                                         :btnIcon="'las la-check-circle'"
                                         :btnClass="'btn-success'">
                        </awaiting-button>
                        <awaiting-button @proceed="(callback) => { $emit('save', callback) }"
                                         :btnText="'saveButton'"
                                         :btnIcon="'las la-save'"
                                         :btnClass="'btn-primary'">
                        </awaiting-button>
                    </div>
                </div>
            </div>

        </div>


    </div>
</template>
<script>
    import Multiselect from '@vueform/multiselect'
    import FormError from '@/widgets/FormError'
    import DateTimeField from '@/widgets/DateTime'
    import { toServer, fromServer } from '@/service/date_converter.js'
    import { defineComponent } from 'vue';
    import moment from 'moment';
    import AwaitingButton from '@/widgets/AwaitingButton'
    import DatePicker from '@/widgets/DatePicker';
export default defineComponent({
        props: ['modelValue', 'errors', 'detailsLength'],
        components: { FormError, dateTime: DateTimeField, Multiselect, AwaitingButton, DatePicker },
        data() {
            return {
                model: this.modelValue,
                transferTypes: ["Over90Days", "Damaged"]
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
                if (this.model && this.model.transferTypeString == 'EStockTransfer') {
                    this.transferTypes = ['EStockTransfer'] 
                }
            }
        },
        computed: {
            isRemarkDisabled() {  return this.model && ['Completed', 'Cancelled'].includes(this.model.statusString) },
            isRefNoDisabled() { return this.model && (['Completed', 'Cancelled'].includes(this.model.statusString) || this.model.transferTypeString == 'EStockTransfer') },
            isTransferTypeDisabled() { return this.model && (['Completed', 'Cancelled'].includes(this.model.statusString) || this.model.transferTypeString == 'EStockTransfer') },
        },
        methods: {
            toServer,
            fromServer,
            showDate(date) {
                return moment(date).format("DD.MM.YYYY HH:mm")
            }
        }
    })
</script>