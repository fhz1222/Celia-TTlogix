<template>
    <modal containerClass="modal-add-customer-code">
        <template name="header" v-slot:header>
            <span>{{$t('stockTransfer.new.createNewStockTransfer')}}</span>
        </template>
        <template name="body" v-slot:body>

            <div class="row">
                <div class="col-md-12">
                    <div><strong>{{$t('stockTransfer.new.selectACustomer')}}</strong></div>
                    <div class="card">
                        <div class="card-body">
                            <div class="row mt-2">
                                <div class="col-md-3 pt-1 text-end no-right-padding">
                                    {{$t('stockTransfer.new.customer')}}
                                </div>
                                <div class="col-md-9 multiselect-as-select">
                                    <select-customer v-model="customerCode"></select-customer>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </template>
        <template name="footer" v-slot:footer>
            <button class="btn btn-sm btn-primary me-2" type="button" @click.stop="process()" :disabled="customerCode==null">
                <i class="las la-plus-circle"></i> {{$t('stockTransfer.new.create')}}
            </button>
            <button class="btn btn-sm btn-secondary" type="button" @click.stop="$emit('close')">
                <i class="las la-times"></i> {{$t('operation.general.close')}}
            </button>
        </template>
    </modal>
</template>
<script>
    import SelectCustomer from '@/widgets/SelectCustomer'
    import { defineComponent } from 'vue';
export default defineComponent({
        props: {

        },
        created() {
            //this.load();
        },
        components: { SelectCustomer },
        data() {
            return {
                customerCode: null,
            }
        },
        methods: {
            process() {
                this.$emit('customer-selected', this.customerCode);
                this.$emit('close');
            }
        }
    })
</script>
<style lang="scss">
    .modal-add-customer-code {
        .modal-container {
            width: 40vw;
        }
    }
</style>