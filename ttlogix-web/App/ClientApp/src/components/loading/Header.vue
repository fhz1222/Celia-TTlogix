<template>
    <div v-if="model != null">
        <div class="card text-dark bg-light">
            <div class="card-header">
                <h5>{{$t('loading.detail.loadingDetailHeaderTitle')}}</h5>
            </div>
            <div class="card-body small-font">
                <!-- Job No, RefNo, Status, Actual # of pallets -->
                <div class="row">
                    <!-- Job No -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.jobno')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.jobNo" disabled />
                        <form-error :errors="errors" field="JobNo" />
                    </div>

                    <!-- Ref No. -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.refno')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.refNo" />
                        <form-error :errors="errors" field="RefNo" />
                    </div>

                    <!-- Status -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.status')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" :value="$t('loading.status.' + model.statusString)" disabled />
                        <form-error :errors="errors" field="RefNo" />
                    </div>

                    <!-- Actual # of Pallets -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end no-right-padding long-label">{{$t('loading.header.actualNumOfPallets')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2"><input type="text" class="form-control form-control-sm" :value="model.noOfPallet == 0 ? '' : model.noOfPallet" @input="inputNoOfPallet($event)" /></div>
                </div>

                <!-- Customer Name, Created Date, Calc. # of Pallets -->
                <div class="row">
                    <!-- Customer Name -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.customerName')}}</div>
                    <div class="col-md-8 col-lg-5 mt-2">
                        <input type="text" class="form-control form-control-sm" :value="model.customerName" disabled />
                        <form-error :errors="errors" field="CustomerName" />
                    </div>

                    <!-- Created Date -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.createdDate')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <div class="form-control form-control-sm disabled"><date-time :value="model.createdDate" /></div>
                        <form-error :errors="errors" field="CreatedDate" />
                    </div>

                    <!-- Calc. # of Pallets -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.calcNumOfPallets')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" :value="model.calculatedNoOfPallet" disabled />
                        <form-error :errors="errors" field="NoOfPallet" />
                    </div>
                </div>

                <!-- Remarks, Confirmed Date, Currency, ETD, ETA, Allowed for Dispatch -->
                <div class="row">
                    <!-- Remarks -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.remarks')}}</div>
                    <div class="col-md-8 col-lg-5 mt-2">
                        <textarea class="form-control form-control-sm" v-model="model.remark" style="height: 100%"></textarea>
                        <form-error :errors="errors" field="Remark" />
                    </div>

                    <div class="col-md-6 col-lg-6">

                        <div class="row">
                            <!-- Confirmed Date -->
                            <div class="col-md-2 col-lg-2 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.confirmedDate')}}</div>
                            <div class="col-md-4 col-lg-4 mt-2">
                                <input type="text" class="form-control form-control-sm" :value="model.confirmedDate" disabled />
                                <form-error :errors="errors" field="ConfirmedDate" />
                            </div>

                            <!-- Currency -->
                            <div class="col-md-2 col-lg-2 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.currency')}}</div>
                            <div class="col-md-4 col-lg-4 mt-2">
                                <input type="text" class="form-control form-control-sm" :value="model.currency" disabled />
                                <form-error :errors="errors" field="Currency" />
                            </div>
                        </div>

                        <div class="row">
                            <!-- ETD -->
                            <div class="col-md-2 col-lg-2 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.etd')}}</div>
                            <div class="col-md-4 col-lg-4 mt-2">
                                <date-picker :type="'date-time'" :date="model.etd" @set="(date) => {model.etd = date}" :mandatory="false"></date-picker>
                                <form-error :errors="errors" field="Etd" />
                            </div>

                            <!-- ETA -->
                            <div class="col-md-2 col-lg-2 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.eta')}}</div>
                            <div class="col-md-4 col-lg-4 mt-2">
                                <date-picker :type="'date-time'" :date="model.eta" @set="(date) => {model.eta = date}" :mandatory="false"></date-picker>
                                <form-error :errors="errors" field="Eta" />
                            </div>
                        </div>

                        <div class="row">
                            <!-- Allowed for Dispatch -->
                            <div class="col-md-2 col-lg-2 mt-2 text-end long-label no-right-padding">{{$t('loading.header.allowedForDispatch')}}</div>
                            <div class="col-md-4 col-lg-4 pt-2 mt-2">
                                <input type="checkbox" v-model="model.allowedForDispatch" :disabled="!$auth.check('LOADINGDISPATCH') || model.confirmedDate"/>
                                <form-error :errors="errors" field="Etd" />
                            </div>
                        </div>

                    </div>

                </div>

                <!-- Truck Licence Plate, Trailer No. -->
                <div class="row">
                    <!-- Truck Licence Plate -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end long-label no-right-padding">{{$t('loading.header.truckLicencePlate')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.truckLicencePlate" />
                        <form-error :errors="errors" field="TruckLicencePlate" />
                    </div>

                    <!-- Trailer No. -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.trailerNo')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.trailerNo" />
                        <form-error :errors="errors" field="Trailer No." />
                    </div>
                </div>

                <!-- Truck Sequence No, Dock No. -->
                <div class="row">
                    <!-- Truck Sequence No -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end long-label no-right-padding">{{$t('loading.header.truckSeqNo')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.truckSeqNo" />
                        <form-error :errors="errors" field="TruckSequenceNo" />
                    </div>

                    <!-- Dock No. -->
                    <div class="col-md-2 col-lg-1 mt-2 text-end pt-1 no-right-padding">{{$t('loading.header.dockNo')}}</div>
                    <div class="col-md-4 col-lg-2 mt-2">
                        <input type="text" class="form-control form-control-sm" v-model="model.dockNo" />
                        <form-error :errors="errors" field="DockNo" />
                    </div>
                </div>

            </div>

            <div class="card-footer">
                <!-- Buttons -->
                <div class="row">
                    <div class="col-md-12 no-right-padding text-end">
                        <button class="btn btn-primary btn-sm me-2" @click.prevent="$emit('releaseForPicking')" v-if="ilogEnabled && ![5,7,9].includes(model.status)">
                            <i class="las la-dolly"></i> {{$t('loading.operation.releaseforpicking')}}
                        </button>
                        <awaiting-button @proceed="(callback) => { $emit('save', callback) }"
                                         :btnText="'loading.operation.save'"
                                         :btnIcon="'las la-save'"
                                         :btnClass="'btn-primary'">
                        </awaiting-button>

                        <button class="btn btn-sm btn-primary me-2" @click.prevent="$emit('confirm')" :disabled="!model.allowedForDispatch || model.confirmedDate">
                            <i class="las la-check-circle"></i> {{$t('loading.operation.confirm')}}
                        </button>
                        <button class="btn btn-sm btn-danger" @click.prevent="$emit('cancel')">
                            <i class="las la-trash-alt"></i> {{$t('loading.operation.cancel')}}
                        </button>
                    </div>
                </div>
            </div>

        </div>

        
    </div>
</template>
<script>
    import FormError from '@/widgets/FormError'
    //import DateField from '@/widgets/Date'
    import { toServer, fromServer } from '@/service/date_converter.js'
    import { defineComponent } from 'vue';
    import AwaitingButton from '@/widgets/AwaitingButton'
    import DatePicker from '@/widgets/DatePicker';
    import DateTime from '@/widgets/DateTime'
export default defineComponent({
        props: ['modelValue', 'errors'],
        components: { FormError, AwaitingButton, DatePicker, DateTime },
        async created() {
            this.ilogEnabled = await this.$dataProvider.ilogIntegration.isILogEnabled();
        },
        data() {
            return {
                model : this.modelValue,
                ilogEnabled: false
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