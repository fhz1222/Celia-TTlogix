<template>
    <modal containerClass="modal-create-loading-outbounds">
        <template name="header" v-slot:header>
            {{$t('loading.detail.createLoading')}}
        </template>
        <template name="body" v-slot:body>
            <div class="form-row">
                <label class="form-label">{{$t('loading.detail.customer')}}</label>
                <div class="form-input full">
                    <input type="text" class="input" :value="jobs[0].customerName" disabled />
                    <form-error :errors="errors" field="Loading.CustomerCode" />
                </div>
            </div>
            <div class="form-row">
                <label class="form-label">{{$t('loading.detail.warehouse')}}</label>
                <div class="form-input">
                    <input type="text" class="input" :value="model.whsCode" disabled />
                    <form-error :errors="errors" field="Loading.WhsCode" />
                </div>
            </div>
            <div class="form-row">
                <label class="form-label">{{$t('loading.detail.refNo')}}</label>
                <div class="form-input">
                    <input type="text" class="input" v-model="model.refNo" :disabled="processing" />
                    <form-error :errors="errors" field="Loading.RefNo" />
                </div>
            </div>
            <div class="form-row">
                <label class="form-label">{{$t('loading.detail.remark')}}</label>
                <div class="form-input full">
                    <textarea class="input" v-model="model.remark" :disabled="processing"></textarea>
                    <form-error :errors="errors" field="Loading.Remark" />
                </div>
            </div>
            <div class="form-row">
                <label class="form-label">{{$t('loading.detail.etd')}}</label>
                <div class="form-input">
                    <date-picker :date="model.etd" @set="(date) => {model.etd = date}" :disabled="processing"/>
                    <form-error :errors="errors" field="Loading.Etd" />
                </div>
            </div>
            <fieldset>
                <legend>{{$t('loading.detail.selectedOutbounds')}}</legend>

                <simple-table-scroll height="300px">

                    <template v-slot:head>
                        <th class="num">{{$t('loading.detail.outboundList_No')}}</th>
                        <th>{{$t('loading.detail.outboundList_JobNo')}}</th>
                        <th>{{$t('loading.detail.outboundList_SupplierName')}}</th>
                        <th>{{$t('loading.detail.outboundList_Status')}}</th>
                        <th>{{$t('loading.detail.outboundList_Remark')}}</th>
                    </template>
                    <template v-slot:body>
                        <tr v-for="(row, index) in jobs" :key="index" :ref="'row' + index">
                            <td class="num">{{index + 1}}</td>
                            <td>{{row.jobNo}}</td>
                            <td>{{row.supplierName}}</td>
                            <td>{{$t('outbound.status.' + row.status)}}</td>
                            <td>{{row.remark}}</td>
                        </tr>
                    </template>
                </simple-table-scroll>
            </fieldset>
        </template>
        <template name="footer" v-slot:footer>
            <button class="button small" type="button" @click.stop="process()" :disabled="processing">
                {{$t('loading.operation.create')}}
            </button>
            <button class="button small" type="button" @click.stop="$emit('close')" :disabled="processing">
                {{$t('loading.operation.close')}}
            </button>
        </template>
    </modal>
</template>
<script>
    import FormError from '@/widgets/FormError'
    import SimpleTableScroll from '@/widgets/SimpleTableScroll'
    import { toServer, fromServer } from '@/service/date_converter.js'
    import { defineComponent } from 'vue';
    import DatePicker from '@/widgets/DatePicker';
export default defineComponent({
        props: {
            title: {
                default: 'loading.detail.createLoading'
            },
            transType: {
                default: 0
            },
            jobs: { }
        },
        components: { FormError, SimpleTableScroll, DatePicker },
        data() {
            return {
                whsList: [],
                model: {
                    customerCode: null,
                    whsCode: this.$auth.user().whsCode,
                    refNo: null,
                    etd: new Date(),
                    remark: null,
                },
                errors: {},
                processing: false
            }
        },
        methods: {
            process() {
                this.processing = true

                this.$dataProvider.customers.getCustomers().then(resp => {
                    this.model.customerCode = resp.find(c => c.name == this.jobs[0].customerName).code
                    this.$dataProvider.loadings.addLoadingFromOutbound(this.model, this.jobs.map(j => j.jobNo)).then(resp => {
                        this.$emit('done', resp)
                    }).catch(e => {
                        if (e.response && e.response.data.errors) {
                            this.errors = e.response.data.errors
                        }
                    }).finally(() => this.processing = false)
                })
            },
            toServer,
            fromServer
        }
    })
</script>
<style lang="scss">
    .modal-create-loading-outbounds {
        .modal-container

    {
        width: 1000px;
    }
    }
</style>