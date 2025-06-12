<template>
    <modal containerClass="modal-create-manual">
        <template name="header" v-slot:header>
            {{$t('outbound.detail.manualOutbound')}}
        </template>
        <template name="body" v-slot:body>
            <div class="form-row">
                <label class="form-label">{{$t('outbound.detail.customer')}}</label>
                <div class="form-input full multiselect-as-select">
                    <select-customer v-model="model.customerCode" />
                    <form-error :errors="errors" field="CustomerCode" />
                </div>
            </div>
            <div class="form-row">
                <label class="form-label">{{$t('outbound.detail.warehouse')}}</label>
                <div class="form-input">
                    <input type="text" class="input" :value="model.whsCode" disabled="disabled" />
                    <form-error :errors="errors" field="WhsCode" />
                </div>
                <template v-if="transType == 4">
                    <label class="form-label">{{$t('outbound.detail.destinationWhs')}}</label>
                    <div class="form-input" style="width: 100px;">
                        <v-select :clearable="false" :options="whsList" label="code" :reduce="l => l.code" v-model="model.newWhsCode" />
                        <form-error :errors="errors" field="NewWhsCode" />
                    </div>
                </template>
            </div>
            <div class="form-row">
                <label class="form-label">{{$t('outbound.detail.releasedDate')}}</label>
                <div class="form-input">
                    <date-picker :date="model.etd" @set="(date) => {model.etd = date}" />
                    <form-error :errors="errors" field="Etd" />
                </div>
            </div>
            <div class="form-row">
                <label class="form-label">{{$t('outbound.detail.remark')}}</label>
                <div class="form-input full">
                    <textarea class="input" v-model="model.remark"></textarea>
                    <form-error :errors="errors" field="Remark" />
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <button class="button small" type="button" @click.stop="process()">
                {{$t('outbound.detail.create')}}
            </button>
            <button class="button small" type="button" @click.stop="$emit('close')">
                {{$t('outbound.detail.close')}}
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
            default: 'outbound.detail.manualOutbound'
        },
        transType: {
            default: 0
        }
    },
    created() {
        if (this.transType == 4) {
            this.$dataProvider.warehouses.getWarehouses().then(resp => this.whsList = resp.filter(f => f.code != this.$auth.user().whsCode))
        }
    },
    components: {FormError, SelectCustomer, DatePicker},
    data() {
        let manualType =1
        switch(this.transType) {
            case 0:
                manualType = 1
                break;
            case 3:
                manualType = 1
                break;
            case 4:
                manualType = 4
                break;
        }
        return {
            whsList: [],
            model : {
                customerCode : null,
                whsCode : this.$auth.user().whsCode,
                newWhsCode : null,
                manualType : manualType,
                refNo : null,
                etd: new Date(),
                transType: this.transType,
                remark : null,
                status: 0
            },
            errors: {}
        }
    },
    methods: {
        process() {
           
            this.$dataProvider.outbounds.createOutboundManual(this.model).then(resp => {
                this.$emit('done', resp)
            }).catch(e => {
                if (e.response && e.response.data.errors) {
                    this.errors = e.response.data.errors
                }
            })
        },
        toServer,
        fromServer
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