<script lang="ts" setup>

    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref, onActivated, onDeactivated, watch } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useI18n } from 'vue-i18n';

    // import types
    import { MenuItem, TableColumn } from '@/store/commonModels/config';
    import { Modal } from '@/store/commonModels/modal';
    import { CustomerFilters } from '@/store/models/customerFilters';
    import { CustomerFull } from '@/store/models/customer';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import InventoryControlPopup from './modals/InventoryControl.vue';
    import ModifyCustomerPopup from './modals/ModifyCustomer.vue';
    
    // import common logic
    import * as pagingFn from '@/store/commonFunctions/paging';
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    import * as routerFn from '@/router';
    import { CustomerInventory } from '../../store/models/customerInventory';
    import { Customer } from '../../store/models/customer';

    // init vue stuff
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });

    // local vars
    const list_CS = computed(() => store.state.customersFullList);
    const filters_CS = ref(new CustomerFilters());
    filters_CS.value.whsCode = store.state.whsCode;
    const loadingList = ref(false);
    const getCustomerStatus = store.getters.getCustomerStatus;

    
    // functions
    const reloadList = async (internalReload: boolean = false) => {
        loadingList.value = true;
        filters_CS.value.status = filters_CS.value.status == 3 ? null : filters_CS.value.status;
        await store.dispatch(ActionTypes.CS_LOAD_LIST, { filters: filters_CS.value });
        if (!internalReload) await store.dispatch(ActionTypes.CS_GET_PRODUCT_CODES);
        if (!internalReload) await store.dispatch(ActionTypes.CS_GET_CONTROL_CODES);
        if (!store.state.companyProfileList || store.state.companyProfileList.length == 0) await store.dispatch(ActionTypes.CP_LOAD_LIST);
        loadingList.value = false;
    }
    const addNewCustomer = () => {
        return true;
    }
    const addOrModify = (customer: Customer | null) => {
        modifyCustomerModal.value.on = true;
        modifyCustomerModal.value.customProps = customer ? customer.code : null;
        if (customer) modifyCustomerModal.value.title = t('customer.popups.modifyCustomer.title') + customer.code;
        else modifyCustomerModal.value.title = t('customer.popups.modifyCustomer.addNewCustomerTitle');
    }
    const updateCustomer = async(customer: CustomerFull, addNext: boolean) => {
        if (!addNext) closeModal(modifyCustomerModal.value);
        loadingList.value = true;
        customer.whsCode = store.state.whsCode;
        customer.bizUnit = customer.bizUnit ?? "";
        customer.buname = customer.buname ?? "";
        customer.pic2 = customer.pic2 ?? "";
        await store.dispatch(ActionTypes.CS_UPDATE_CUSTOMER, { customer: customer }).catch(() => { loadingList.value = false });
        reloadList(true);
        loadingList.value = false;
    }
    const editInventoryControl = (customerCode: string) => {
        invControlModal.value.on = true;
        invControlModal.value.customProps = customerCode;
        invControlModal.value.title = t('customer.popups.inventoryControl.title') + customerCode;
    }
    const editInventoryControlExecute = async (customerInventory: CustomerInventory) => {
        closeModal(invControlModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.CS_UPDATE_INVENTORY_CONTROL, { inventoryControl: customerInventory })
                   .catch(() => { closeModal(waitModal.value) });
        closeModal(waitModal.value);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const invControlModal = ref(new Modal(500, t('customer.popups.inventoryControl.title'), true));
    const modifyCustomerModal = ref(new Modal(750, t('customer.popups.modifyCustomer.title'), true));
    
    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_CS.value.sorting); reloadList(true); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_CS.value); if (load) reloadList(true); };
    // GoTo
    const goTo = (state: string, params?: any) => { return routerFn.goTo(state, params); }

    // section Menu
    const menuItems_IR:Array<MenuItem> = [
        { state: null, stateParams: null, title: 'addNew', function: addOrModify, icon:'la-plus-circle' },
        { state:null, stateParams:null, title:'refresh', function:reloadList, icon:'la-redo-alt', float: 'end' },
    ];

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const customerStatus = store.state.config.dictionaries.customerStatus;

    // table related stuff
    const columns_CS: Array<TableColumn> = [
        { alias:'code',         columnType:'string',  dropdown:null, sortable:true, f1:'', f2:null, resetTo:null },
        { alias:'name',         columnType:'string',  dropdown:null, sortable:true, f1:'', f2:null, resetTo:null },
        { alias:'contactPerson',columnType:'string',  dropdown:null, sortable:true, f1:'', f2:null, resetTo:null },
        { alias:'telephoneNo',  columnType:'string',  dropdown:null, sortable:true, f1:'', f2:null, resetTo:null },
        { alias:'faxNo',        columnType:'string',  dropdown:null, sortable:true, f1:'', f2:null, resetTo:null },
        { alias:'status',       columnType:'dropdown',dropdown:customerStatus, sortable:true, f1:'3',f2:null, resetTo:'3' }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", () => reloadList())
    });
    onDeactivated(() => bus.off("refresh", () => reloadList()))
</script>
<template>
    <section-menu :menu-items="menuItems_IR" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_CS
                                              :sorting=filters_CS.sorting
                                              t_core="customer.mainGrid"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_CS.length > 0">
                        <tr v-for="cs, i in list_CS" :key="i" class="table-row">
                            <td>{{cs.code}}</td>
                            <td>{{cs.name}}</td>
                            <td>{{cs.contactPerson}}</td>
                            <td>{{cs.telephoneNo}}</td>
                            <td>{{cs.faxNo}}</td>
                            <td>{{getCustomerStatus(cs.status)}}</td>
                            <div class="table-buttons">
                                <button class="btn btn-primary me-2 btn-sm" @click="addOrModify(cs)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-primary me-2 btn-sm" @click="editInventoryControl(cs.code)"><i class="las la-boxes"></i> {{t('customer.mainGrid.inventory')}}</button>
                                <button class="btn btn-primary me-2 btn-sm" @click="goTo('customer-uom', {customerCode: cs.code})"><i class="las la-cog"></i> {{t('customer.mainGrid.uom')}}</button>
                                <button class="btn btn-primary me-2 btn-sm" @click="goTo('customer-client', {customerCode: cs.code})"><i class="las la-industry"></i> {{t('customer.mainGrid.client')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_CS.length == 0">
                    <h5><i class="las la-ban"></i> {{t('generic.noRecords')}}</h5>
                </div>
                <div v-if="loadingList" class="bars1-wrapper">
                    <div id="bars1">
                        <span></span>
                        <span></span>
                        <span></span>
                        <span></span>
                        <span></span>
                    </div>
                    <h5>{{t('generic.loading')}}</h5>
                </div>
            </div>
        </div>
    </div>

    <processing-popup :modalBase=waitModal />
    <inventory-control-popup :modalBase="invControlModal" @close="closeModal(invControlModal)" @proceed="editInventoryControlExecute" />
    <modify-customer-popup :modalBase="modifyCustomerModal" @close="closeModal(modifyCustomerModal)" @proceed="updateCustomer" />
</template>