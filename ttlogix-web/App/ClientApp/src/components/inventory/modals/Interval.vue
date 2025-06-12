<script lang="ts" setup>
    import { store } from '@/store';
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { Customer } from '@/store/models/customer';
    import ModalWidget from '@/widgets/Modal3.vue';
    import { computed, ref, watch } from 'vue';
    import { ReportFilter } from '@/store/commonModels/reportFilter';
    import { Report } from '@/store/commonModels/report';

    const props = defineProps<{modalBase: Modal}>();
    const emit = defineEmits<{(e: 'close'): void, (e: 'print', modalToClose:Modal, reportFilter:ReportFilter, report:Report): void }>();

    const reportFilter = ref(new ReportFilter());
    const report = ref<Report | null>(null);

    watch(() => props.modalBase.customProps, (nVal, oVal) => {
        if(nVal){
            report.value = nVal.report ?? nVal.report;
        } else {
            report.value = null;
        }
    });

    const customers = computed(() => store.state.customers);
    const customer = ref<Customer | null>(null);

    if(customers.value.length == 0) await store.dispatch(ActionTypes.INV_LOAD_CUSTOMERS);
    const printDisabled = ():boolean => {
        return !customer.value || !reportFilter.value.interval || reportFilter.value.interval < 1;
    }

    // Customer
    const changeCustomer = (customer:Customer) => {
        reportFilter.value.customerCode = customer ? customer.code : null;
    }
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close')">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="card mt-2">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4 pt-1 text-end">Customer:</div>
                        <div class="col-md-8">
                            <v-select :options="customers"                     
                                    @update:modelValue="changeCustomer"
                                    v-model="customer"
                                    placeholder="Select a customer">
                                    <template v-slot:selected-option="option">{{ option.code + ' - ' + option.name }}</template>
                                    <template v-slot:option="option">{{option.code}} - {{option.name}}</template>
                            </v-select>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card mt-2">
                <div class="card-body">
                    <div class="row mt-3">
                        <div class="col-md-4 pt-1 text-end">Time interval in days:</div>
                        <div class="col-md-8">
                            <input class="form-control form-control-sm text-end" v-model="reportFilter.interval" />
                        </div>
                    </div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="text-end">
                <button class="btn btn-sm btn-primary"
                        @click="emit('print', modalBase, reportFilter, modalBase.customProps.report)"
                        :disabled="printDisabled()">
                        <i class="las la-print"></i> Print
                </button>
            </div>
        </template>
    </modal-widget>
</template>