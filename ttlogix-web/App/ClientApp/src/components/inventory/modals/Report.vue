<script lang="ts" setup>
    // import vue stuff
    import { store } from '@/store';
    import { ActionTypes } from '@/store/action-types';
    import { computed, ref, watch } from 'vue';
    import moment from 'moment';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { Customer } from '@/store/models/customer';
    import { SupplierRegular } from '@/store/models/supplier';
    import { ProductFull } from '@/store/models/product';
    import { ReportFilter } from '@/store/commonModels/reportFilter';
    import { Report } from '@/store/commonModels/report';
    import DatePicker from '@/widgets/DatePicker';

    // import widgets and modals
    import ModalWidget from '@/widgets/Modal3.vue';

    const props = defineProps<{modalBase: Modal}>();
    const emit = defineEmits<{(e: 'close'): void, (e: 'print', modalToClose:Modal, reportFilter:ReportFilter, report:Report): void }>();

    const reportFilter = ref(new ReportFilter());
    reportFilter.value.reportByProductCode = true;
    reportFilter.value.productSelect = false;
    reportFilter.value.bondedStatus = null;
    const report = ref<Report | null>(null);
    const showBonded = ref(true);
    const showDates = ref(true);
    const showProducts = ref(true);
    const showRepBy = ref(true);
    const showSupp = ref(true);
    const reqSupp = ref(false);
    const reqDates = ref(false);

    watch(() => props.modalBase.customProps, (nVal, oVal) => {
        if(nVal){
            report.value = nVal.report ?? nVal.report;
            showBonded.value = nVal.showBonded;
            showDates.value = nVal.showDates;
            showProducts.value = nVal.showProducts;
            showRepBy.value = nVal.showRepBy;
            showSupp.value = nVal.showSupp;
            reqSupp.value = nVal.reqSupp;
            reqDates.value = nVal.reqDates;
            reportFilter.value.supplierSelect = reqSupp.value;
        } else {
            reportFilter.value = new ReportFilter();
            reportFilter.value.reportByProductCode = true;
            reportFilter.value.productSelect = false;
            reportFilter.value.bondedStatus = null;
            report.value = null;
            suppliers.value = new Array<SupplierRegular>();
            supplier.value = null;
            products.value = new Array<ProductFull>();
            product.value = null;
            customer.value = null;
        }
    });

    const suppliers = ref(new Array<SupplierRegular>());
    const supplier = ref<SupplierRegular | null>(null);

    var customers = computed(() => store.state.customers);
    const customer = ref<Customer | null>(null);

    const products = ref(new Array<ProductFull>());
    const product = ref<ProductFull | null>(null);

    if(customers.value.length == 0) await store.dispatch(ActionTypes.INV_LOAD_CUSTOMERS);
    const loadingSuppliers = ref(false);
    const loadingProducts = ref(false);
    const printDisabled = ():boolean => {
        var sup:boolean = reportFilter.value.supplierSelect && !reportFilter.value.supplierID;
        var prod:boolean = reportFilter.value.productSelect && !reportFilter.value.productCode;
        var dates:boolean = reqDates.value && (!reportFilter.value.dateStart || !reportFilter.value.dateEnd);
        
        return !customer.value || loadingSuppliers.value || loadingProducts.value || sup || prod || dates;
    }

    // Customer
    const changeCustomer = async (customer:Customer) => {
        reportFilter.value.customerCode = customer ? customer.code : null;
        if((customer && showSupp.value) || (customer && showProducts.value)){
            if(showProducts.value && !showSupp.value){
                loadingProducts.value = true;
                await store.dispatch(ActionTypes.GET_PRODUCTS_BY_CUSTOMER, { customerCode: customer.code })
                           .then((prod) => {
                                products.value = prod;
                                loadingProducts.value = false;
                           });
            } else {
                loadingSuppliers.value = true;
                await store.dispatch(ActionTypes.GET_SUPPLIERS, { factoryId: customer.code })
                           .then((sup) => {
                                suppliers.value = sup;
                                loadingSuppliers.value = false;
                           });
            }
        }
    }
    
    // Suppliers
    const changeSupplier = async (supplier:SupplierRegular) => {
        reportFilter.value.supplierID = supplier ? supplier.supplierID : null;
        if(supplier && customer){
            loadingProducts.value = true;
            await store.dispatch(ActionTypes.GET_PRODUCTS, { customerCode: customer?.value?.code, supplierID: supplier.supplierID })
                       .then((prod) => {
                           products.value = prod;
                           loadingProducts.value = false;
                       });
        }
    }
    
    // Products
    const changeProduct = (product:ProductFull) => {
        reportFilter.value.productCode = product ? product.productCode1 : null;
    }

    const print = () => {
        if(reportFilter.value.dateStart) reportFilter.value.dateStart = moment(reportFilter.value.dateStart).format();
        if(reportFilter.value.dateEnd) reportFilter.value.dateEnd = moment(reportFilter.value.dateEnd).add(1,'day').format();
        emit('print', props.modalBase, reportFilter.value, props.modalBase.customProps.report);
    }
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close')">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="card" v-if="showDates">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-3 pt-1 text-end">From:</div>
                        <div class="col-md-4">
                            <date-picker :date="reportFilter.dateStart" @set="(date) => {reportFilter.dateStart = date}" />
                        </div>
                        <div class="col-md-1 pt-1 text-end no-left-padding">To:</div>
                        <div class="col-md-4">
                            <date-picker :date="reportFilter.dateEnd" @set="(date) => {reportFilter.dateEnd = date}" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="card" :class="showDates ? 'mt-2' : ''">
                <div class="card-body">
                    <div class="row" v-if="showRepBy">
                        <div class="col-md-3 text-end">Report by:</div>
                        <div class="col-md-4">
                            <input class="form-check-input" type="radio" name="reportByProductCode" id="reportBy_pc" :value=true v-model="reportFilter.reportByProductCode">
                            <label class="form-check-label ms-2" for="reportBy_pc">Product Code</label>
                        </div>
                        <div class="col-md-4">
                            <input class="form-check-input" type="radio" name="reportByProductCode" id="reportBy_pid" :value=false v-model="reportFilter.reportByProductCode">
                            <label class="form-check-label ms-2" for="reportBy_pid">PID</label>
                        </div>
                    </div>
                    <div class="row" :class="showRepBy ? 'mt-3' : ''">
                        <div class="col-md-3 pt-1 text-end">Customer:</div>
                        <div class="col-md-9">
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
            <div class="card mt-2" v-if="showSupp">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-5" v-if="!reqSupp">
                            <input class="form-check-input" type="radio" name="suppliers" id="suppliers_all" :value=false v-model="reportFilter.supplierSelect">
                            <label class="form-check-label ms-2" for="suppliers_all">All Suppliers</label>
                        </div>
                        <div class="col-md-2" :class="reqSupp ? 'offset-md-5' : ''" v-if="showBonded">
                            <input class="form-check-input" type="radio" :value=true id="bonded" v-model="reportFilter.bondedStatus">
                            <label class="form-check-label ms-2" for="bonded">Bonded</label>
                        </div>
                        <div class="col-md-3" v-if="showBonded">
                            <input class="form-check-input" type="radio" :value=false id="nonBonded" v-model="reportFilter.bondedStatus">
                            <label class="form-check-label ms-2" for="nonBonded">Non-Bonded</label>
                        </div>
                        <div class="col-md-2" v-if="showBonded">
                            <input class="form-check-input" type="radio" :value=null id="all" v-model="reportFilter.bondedStatus">
                            <label class="form-check-label ms-2" for="all">All</label>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-3">
                            <input class="form-check-input" type="radio" name="suppliers" id="suppliers_sel" :value=true v-model="reportFilter.supplierSelect">
                            <label class="form-check-label ms-2" for="suppliers_sel">Select</label>
                        </div>
                        <div class="col-md-9">
                            <v-select :options="suppliers" 
                                    v-if="!loadingSuppliers"                 
                                    @update:modelValue="changeSupplier"
                                    v-model="supplier"
                                    :disabled = "!reportFilter.customerCode || !reportFilter.supplierSelect"
                                    placeholder="Select a supplier">
                                    <template v-slot:selected-option="option">{{ option.supplierID + ' - ' + option.companyName }}</template>
                                    <template v-slot:option="option">{{option.supplierID}} - {{option.companyName}}</template>
                            </v-select>
                            <span v-if="loadingSuppliers"><i><small>Loading suppliers...</small></i></span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card mt-2" v-if="showProducts">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-4">
                            <input class="form-check-input" type="radio" name="products" id="product_all" :value=false v-model="reportFilter.productSelect">
                            <label class="form-check-label ms-2" for="product_all">All Products</label>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-3">
                            <input class="form-check-input" type="radio" name="products" id="product_sel" :value=true v-model="reportFilter.productSelect">
                            <label class="form-check-label ms-2" for="product_sel">Select</label>
                        </div>
                        <div class="col-md-9">
                            <v-select :options="products"                     
                                    @update:modelValue="changeProduct"
                                    v-model="product"
                                    v-if="!loadingProducts"
                                    :disabled="(showSupp && !reportFilter.supplierID) || !reportFilter.productSelect"
                                    placeholder="Select a product">
                                    <template v-slot:selected-option="option">{{ option.productCode1 + ' - ' + option.description }}</template>
                                    <template v-slot:option="option">{{option.productCode1}} - {{option.description}}</template>
                            </v-select>
                            <span v-if="loadingProducts"><i><small>Loading products...</small></i></span>
                        </div>
                    </div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="text-end">
                <button class="btn btn-sm btn-primary"
                        @click="print"
                        :disabled="printDisabled()">
                        <i class="las la-print"></i> Print
                </button>
            </div>
        </template>
    </modal-widget>
</template>