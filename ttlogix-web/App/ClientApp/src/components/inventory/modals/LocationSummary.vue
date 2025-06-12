<script lang="ts" setup>
    import { store } from '@/store';
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { Customer } from '@/store/models/customer';
    import ModalWidget from '@/widgets/Modal3.vue';
    import { computed, ref, watch } from 'vue';
    import { ReportFilter } from '@/store/commonModels/reportFilter';
    import { Report } from '@/store/commonModels/report';
    import { Location } from '@/store/commonModels/location';

    const props = defineProps<{modalBase: Modal}>();
    const emit = defineEmits<{(e: 'close'): void, (e: 'print', modalToClose:Modal, reportFilter:ReportFilter, report:Report): void }>();

    const reportFilter = ref(new ReportFilter());
    reportFilter.value.isAllProductsSelected = true;
    reportFilter.value.isQuantitySelected = false;
    const report = ref<Report | null>(null);
    const customers = computed(() => store.state.customers);
    const locations = computed(() => store.state.locations);
    const location = ref<Location | null>(null);
    const customer = ref<Customer | null>(null);
    const loadingCustomers = ref(false);
    const loadingLocations = ref(false);
    const whsCode = store.state.whsCode;

    watch(() => props.modalBase.customProps, (nVal, oVal) => {
        if(nVal){
            report.value = nVal.report ?? nVal.report;
        } else {
            report.value = null;
            reportFilter.value = new ReportFilter();
            reportFilter.value.isAllProductsSelected = true;
            reportFilter.value.isQuantitySelected = false;
        }
    });

    if(customers.value.length == 0) {
        loadingCustomers.value = true; 
        await store.dispatch(ActionTypes.INV_LOAD_CUSTOMERS);
        loadingCustomers.value = false;
    }
    if(locations.value.length == 0) {
        loadingLocations.value = true;
        await store.dispatch(ActionTypes.LOAD_LOCATIONS, { whsCode: whsCode });
        loadingLocations.value = false;
    }

    const printDisabled = ():boolean => {
        return !customer.value || (!reportFilter.value.isAllProductsSelected && !location.value);
    }

    const changeCustomer = (customer:Customer) => {
        reportFilter.value.customerCode = customer ? customer.code : null;
    }
    const changeLocation = (location:Location) => {
        reportFilter.value.locationCode = location ? location.code : null;
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
                                    v-if="!loadingCustomers"                   
                                    @update:modelValue="changeCustomer"
                                    v-model="customer"
                                    placeholder="Select a customer">
                                    <template v-slot:selected-option="option">{{ option.code + ' - ' + option.name }}</template>
                                    <template v-slot:option="option">{{option.code}} - {{option.name}}</template>
                            </v-select>
                            <span v-if="loadingCustomers"><i><small>Loading customers...</small></i></span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card mt-2">
                <div class="card-body">
                    <div class="row mt-3">
                        <div class="col-md-12">
                            <input class="form-check-input" type="radio" name="allLocations" id="locations_all" :value=true v-model="reportFilter.isAllProductsSelected">
                            <label class="form-check-label ms-2" for="suppliers_all">All Locations</label>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-4 pt-1">
                            <input class="form-check-input" type="radio" name="allLocations" id="locations_selected" :value=false v-model="reportFilter.isAllProductsSelected">
                            <label class="form-check-label ms-2" for="suppliers_all">Select Location Code</label>
                        </div>
                        <div class="col-md-8">
                            <v-select :options="locations"    
                                    v-if="!loadingLocations"                 
                                    @update:modelValue="changeLocation"
                                    v-model="location"
                                    placeholder="Select a location">
                                    <template v-slot:selected-option="option">{{ option.code + ' - ' + option.name }}</template>
                                    <template v-slot:option="option">{{option.code}} - {{option.name}}</template>
                            </v-select>
                            <span v-if="loadingLocations"><i><small>Loading locations...</small></i></span>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-12">
                            <input class="form-check-input" type="checkbox" name="quantity" id="quantitySelected" :value=false v-model="reportFilter.isQuantitySelected">
                            <label class="form-check-label ms-2" for="quantitySelected">Location with quantity only</label>
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