<template>
    <div v-if="model != null">
        <div class="card text-dark bg-light">
            <div class="card-header">
                <h5>{{$t('outbound.header.outboundHeader')}}</h5>
            </div>
            <div class="card-body small-font">
                <!-- Customer -->
                <div class="row">
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.customer')}}</div>
                    <div class="col-md-10 col-lg-8 mt-2 multiselect-as-select">
                        <select-customer v-model="model.customerCode" :disabled="true" />
                        <form-error :errors="errors" field="CustomerCode" />
                    </div>
                </div>

                <!-- Job, Warehouse, Created Date, Currency -->
                <div class="row">
                    <!-- Job No -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.jobNo')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.jobNo" disabled /></div>

                    <!-- Warehouse -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.warehouse')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.whsCode" disabled /></div>

                    <!-- Ref No -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.refNo')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.refNo" :disabled="isRefNoDisabled" @input="$emit('update:modelValue',model)" />
                        <form-error :errors="errors" field="RefNo" />
                    </div>

                    <!-- Currency -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.currency')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" :value="currency" disabled /></div>
                </div>

                <!-- Job Status, ASN, Created By, Total ASN Value -->
                <div class="row">
                    <!-- Job Status -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.jobStatus')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" :value="statuses[model.status]" disabled /></div>

                    <!-- Created Date -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.createdDate')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <div class="form-control form-control-sm disabled"><date :value="model.createdDate" @input="$emit('update:modelValue',model)" /></div>
                    </div>

                    <!-- Created By -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.createdBy')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.createdBy" disabled /></div>

                    <!-- Total Value -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.totalValue')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" :value="$n(totalValue)" disabled /></div>
                </div>

                <!-- Job Type, IM7/Ref No., ETA, Total Residual Value -->
                <div class="row">
                    <!-- Job Type -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('inbound.header.jobType')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" :value="types[model.transType]" disabled /></div>

                    <!-- Released date -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.releasedDate')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <date-picker :type="'date-time'" :date="model.etd" @set="(date) => {model.etd = date}" />
                        <form-error :errors="errors" field="Etd" />
                    </div>

                    <!-- Comm inv number -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.commInvNo')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.commInvNo" @input="$emit('update:modelValue',model)" />
                        <form-error :errors="errors" field="CommInvNo" />
                    </div>

                    <!-- Actual number of pallets -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end py-1 no-right-padding">{{$t('outbound.header.noOfPallet')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" :value="model.noOfPallet == 0 ? '' : model.noOfPallet" @input="inputNoOfPallet($event); $emit('update:modelValue',model)" />
                    </div>
                </div>

                <!-- Remark, IM4 No. -->
                <div class="row">
                    <!-- Remark -->
                    <div class="col-md-1 text-end mt-2 pt-1 no-right-padding">{{$t('outbound.header.remarks')}}</div>
                    <div class="col-md-5 mt-2">
                        <textarea ref="remarks" class="form-control form-control-sm" v-model="model.remark" @input="$emit('update:modelValue',model)"></textarea>
                        <form-error :errors="errors" field="Remark" />
                    </div>

                    <!-- XDock -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.xDock')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2 pt-2"><input type="checkbox" v-model="model.im4No" @changed="$emit('update:modelValue',model)" /></div>

                    <!-- Calc. # of Pallets -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('outbound.header.calcNoOfPallet')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" :value="model.calculatedNoOfPallet" disabled />
                        <form-error :errors="errors" field="CalculatedNoOfPallet" />
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-2 col-lg-1 mt-2 text-end no-right-padding pt-1">{{$t('outbound.header.transportNumber')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" v-model="model.transportNo" /></div>
                </div>

            </div>

            <div class="card-footer">
                <div class="row">
                    <div class="col text-end">
                        <button class="btn btn-primary btn-sm me-2" @click.prevent="$emit('releaseForPicking')" v-if="ilogEnabled && ![8,9].includes(model.status)">
                            <i class="las la-dolly"></i> {{$t('outbound.operation.releaseforpicking')}}
                        </button>
                        <awaiting-button v-if="model.status < 9"
                                        @proceed="(callback) => { $emit('save', callback) }"
                                        :btnText="'outbound.operation.save'"
                                        :btnIcon="'las la-save'"
                                        :btnClass="'btn-primary'">
                        </awaiting-button>

                        <awaiting-button v-if="!isBondedStockDisabled"
                                        @proceed="(callback) => { $emit('releaseBondedStock', callback) }"
                                        :btnText="'outbound.operation.releaseBonded'"
                                        :btnIcon="'las la-share-square'"
                                        :btnClass="'btn-primary'">
                        </awaiting-button>

                        <button class="btn btn-primary btn-sm me-2" @click.prevent="$emit('eKanbanDownload')" v-if="model.transType == 2">
                            <i class="las la-arrow-down"></i> {{$t('outbound.operation.eKanbanDownload')}}
                        </button>

                        <awaiting-button v-if="isCargoOutVisible"
                                        @proceed="(callback) => { $emit('cargoOut', callback) }"
                                        :btnText="cargoOutText"
                                        :btnIcon="'las la-share-square'"
                                        :btnClass="'btn-primary'">
                        </awaiting-button>

                        <awaiting-button  v-if="isTruckDepartureVisible"
                                        @proceed="(callback) => { $emit('truckDeparture', callback) }"
                                        :btnText="'outbound.operation.truckDeparture'"
                                        :btnIcon="'las la-share-square'"
                                        :btnClass="'btn-primary'">
                        </awaiting-button>
                        <awaiting-button v-if="model.allowAutoallocation && model.showBatchAllocationButtons" :disabled="disableBatchAllocation"
                                        @proceed="(callback) => { disableBatchAllocation = true; $emit('autoallocateAll', callback); }"
                                        @reenableButtons="disableBatchAllocation=false"
                                        :btnText="'outbound.operation.autoallocateAll'"
                                        :btnIcon="'las la-project-diagram'"
                                        :btnClass="'btn-primary'">
                        </awaiting-button>
                        <awaiting-button v-if="model.allowAutoallocation && model.showBatchAllocationButtons" :disabled="disableBatchAllocation"
                                        @proceed="(callback) => { disableBatchAllocation = true; $emit('undoAutoallocateAll', callback); }"
                                        @reenableButtons="disableBatchAllocation=false"
                                        :btnText="'outbound.operation.undoAutoallocateAll'"
                                        :btnIcon="'las la-reply'"
                                        :btnClass="'btn-primary'">
                        </awaiting-button>
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>
<script>
    import SelectCustomer from '@/widgets/SelectCustomer'
    import FormError from '@/widgets/FormError'
    import outboundStatus from '@/enum/outbound_status'
    import transType from '@/enum/trans_type'
    import DateField from '@/widgets/Date'
    import { toServer, fromServer } from '@/service/date_converter.js'
    import { ref, onMounted, defineComponent } from 'vue';
    import moment from 'moment';
    import AwaitingButton from '@/widgets/AwaitingButton'
    import DatePicker from '@/widgets/DatePicker';

    export default defineComponent({
        props: ['modelValue', 'errors', 'pickingList'],
        components: { FormError, SelectCustomer, date: DateField, AwaitingButton, DatePicker },
        async created() {
            this.model = this.modelValue;
            await this.$dataProvider.settings.get().then((settings) => { this.settings = settings });
            this.ilogEnabled = await this.$dataProvider.ilogIntegration.isILogEnabled();
        },
        data() {
            return {
                model: null,
                statuses: outboundStatus,
                types: transType,
                customers: [],
                settings: null,
                ilogEnabled: false,
                disableBatchAllocation: false
            }
        },
        watch: {
            modelValue() {
                this.model = this.modelValue
            }
        },
        computed: {
            currency() {
                let currency = null, first = true

                this.pickingList.forEach(p => {
                    if (first) {
                        currency = p.currency
                        first = false
                    }
                    if (currency != p.currency) {
                        currency = 'MIXED'
                    }
                })

                return currency
            },
            totalValue() {
                let value = 0

                this.pickingList.forEach(p => {
                    value += p.pidValue

                })
                return value
            },
            isRefNoDisabled() {
                return !(this.model.transType == 3 && ![7, 8, 9, 10].includes(this.model.status))
            },
            isBondedStockDisabled() {
                return !(this.model.hasBondedStock && ![0, 1, 2, 3, 7, 8, 9, 10].includes(this.model.status))
            },
            isTruckDepartureVisible() {
                return this.settings ? this.settings.ownerCode == 'TESAG' && this.model.transType == 4 && [4,5].includes(this.model.status) : false;
            },
            isCargoOutVisible() {
                return this.settings ? (this.settings.ownerCode == 'TESAG' && this.model.status == 12) || (this.settings.ownerCode != 'TESAG' && this.model.status == 4) : false;
            },
            cargoOutText() {
                return this.settings ? this.settings.ownerCode == 'TESAG' ? 'outbound.operation.complete' : 'outbound.operation.cargoOut' : '';
            }
        },
        mounted() { this.$refs.remarks.focus(); },
        methods: {
            toServer,
            fromServer,
            inputNoOfPallet(v) {
                let val = parseInt(v.target.value, 10);
                this.model.noOfPallet = isNaN(val) ? 0 : val
            },
            asInt(e) {
                return parseInt(e)
            },
            showDate(date) {
                return moment(date).format("DD.MM.YYYY HH:mm")
            },
            enableAllocationButtons() {
                this.disableBatchAllocation = false;
            }
        }
    });
    
</script>