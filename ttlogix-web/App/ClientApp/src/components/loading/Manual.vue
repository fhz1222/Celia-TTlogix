<template>
    <modal containerClass="modal-create-manual">
        <template name="header" v-slot:header>
            {{$t('loading.manual.title')}}
        </template>
        <template name="body" v-slot:body>
            <div class="form-row">
                <label class="form-label">{{$t('loading.manual.customer')}}</label>
                <div class="form-input full multiselect-as-select">
                    <select-customer v-model="model.customerCode" :disabled="processing"/>
                    <form-error :errors="errors" field="CustomerCode" />
                </div>
            </div>
            <div class="form-row">
                <label class="form-label">{{$t('loading.manual.warehouse')}}</label>
                <div class="form-input">
                    <input type="text" class="input" :value="model.whsCode" disabled />
                    <form-error :errors="errors" field="WhsCode" />
                </div>
            </div>
            <div class="form-row">
                <label class="form-label">{{$t('loading.manual.refno')}}</label>
                <div class="form-input">
                    <input type="text" class="input" v-model="model.refNo" :disabled="processing"/>
                    <form-error :errors="errors" field="RefNo" />
                </div>
            </div>
            <div class="form-row">
                <label class="form-label">{{$t('loading.manual.remark')}}</label>
                <div class="form-input full">
                    <textarea class="input" v-model="model.remark" :disabled="processing"></textarea>
                    <form-error :errors="errors" field="Remark" />
                </div>
            </div>
            <div class="form-row">
                <label class="form-label">{{$t('loading.manual.etd')}}</label>
                <div class="form-input">
                    <date-picker :type="'date-time'" :date="model.etd" @set="(date) => {model.etd = date}" :disabled="processing" />
                    <form-error :errors="errors" field="Etd" />
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <button class="button small" type="button" @click.stop="process()" :disabled="processing">
                {{$t('operation.general.create')}}
            </button>
            <button class="button small" type="button" @click.stop="$emit('close')" :disabled="processing">
                {{$t('operation.general.close')}}
            </button>
        </template>
    </modal>
</template>
<script>
import FormError from '@/widgets/FormError'
    import SelectCustomer from '@/widgets/SelectCustomer'
    import { toServer, fromServer } from '@/service/date_converter.js'
import { defineComponent } from 'vue';
    import DatePicker from '@/widgets/DatePicker';
export default defineComponent({
    props: {
        title: {
            default: 'Manual Loading'
        }
    },
    components: {FormError, SelectCustomer, DatePicker},
    data() {
        return {
            whsList: [],
            model : {
                customerCode : null,
                whsCode : this.$auth.user().whsCode,
                refNo : null,
                etd: new Date(),
                remark : null,
            },
            errors: {},
            processing: false
        }
    },
    methods: {
        process() {
            this.processing = true
            this.$dataProvider.loadings.postLoading(this.model).then(resp => {
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
})
</script>
<style lang="scss">
    .modal-create-manual-loading {
        .modal-container

    {
        width: 700px;
    }
    }
</style>