<script lang="ts" setup>

    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { useRoute } from 'vue-router';
    import { computed, ref, onActivated, onDeactivated, watch } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useI18n } from 'vue-i18n';

    // import types
    import { MenuItem, TableColumn } from '@/store/commonModels/config';
    import { Modal } from '@/store/commonModels/modal';
    import { CustomerClientFilters } from '@/store/models/customerClientFilters'
    import { CustomerClient } from '../../store/models/customerClient';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ModifyCustomerClientPopup from './modals/ModifyCustomerClient.vue';
    
    // import common logic
    import * as pagingFn from '@/store/commonFunctions/paging';
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    import * as routerFn from '@/router';

    // init vue stuff
    const store = useStore();
    const route = useRoute();
    const { t } = useI18n({ useScope: 'global' });

    // local vars
    const customerCode = ref<string>(route.params.customerCode?.toString());
    const list_CLI = ref(new Array<CustomerClient>());
    const filters_CLI = ref(new CustomerClientFilters());
    filters_CLI.value.customerCode = customerCode.value;
    const loadingList = ref(false);
    const getCustomerClientStatus = store.getters.getCustomerClientStatus;

    
    // functions
    const reloadList = async (internalReload: boolean = false) => {
        loadingList.value = true;
        filters_CLI.value.status = filters_CLI.value.status == 3 ? null : filters_CLI.value.status;
        await store.dispatch(ActionTypes.CS_LOAD_CLIENTS, { filters: filters_CLI.value })
                   .then((clients) => list_CLI.value = clients)
                   .catch(() => loadingList.value = false);
        loadingList.value = false;
    }
    const addOrModify = (customerClient: CustomerClient | null) => {
        modifyCustomerClientModal.value.on = true;
        modifyCustomerClientModal.value.customProps = customerClient ? customerClient.code : null;
        if (customerClient) modifyCustomerClientModal.value.title = t('customer.popups.modifyCustomer.title') + customerClient.code;
        else modifyCustomerClientModal.value.title = t('customer.popups.modifyCustomer.addNewCustomerTitle');
    }
    const updateCustomerClient = async (customerClient: CustomerClient, addNext: boolean) => {
        if (!addNext) closeModal(modifyCustomerClientModal.value);
        loadingList.value = true;
        await store.dispatch(ActionTypes.CS_UPDATE_CUSTOMER_CLIENT, { customerClient: customerClient })
                   .catch(() => { loadingList.value = false });
        reloadList(true);
        loadingList.value = false;
    }
    const toggleCustomerClient = async (customerClient: CustomerClient) => {
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.CS_TOGGLE_CUSTOMER_CLIENT, { customerClientCode: customerClient.code })
            .then((updatedcustomerClient) => {
                const csIndex = list_CLI.value.findIndex((cli) => cli.code == customerClient.code);
                list_CLI.value[csIndex].status = updatedcustomerClient.status;
            }).catch(() => waitModal.value.on = false);
        closeModal(waitModal.value);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const invCodesModal = ref(new Modal(700, t('customer.popups.invCodes'), true));
    const modifyCustomerClientModal = ref(new Modal(750, t('customer.popups.modifyCustomer'), true));
    
    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_CLI.value.sorting); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_CLI.value); if (load) reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const customerClientStatus = store.state.config.dictionaries.customerClientStatus;

    // section Menu
    const menuItems_CLI:Array<MenuItem> = [
        { state: null, stateParams: null, title: 'addNew', function: addOrModify, icon:'la-plus-circle' },
        { state:null, stateParams:null, title:'refresh', function:reloadList, icon:'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_CLI: Array<TableColumn> = [
        { alias: 'companyCode', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'companyName', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'contactPerson', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'telephoneNo', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'faxNo', columnType: 'string', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'status', columnType: 'dropdown', dropdown: customerClientStatus, sortable: true, f1: '3', f2: null, resetTo: '3' }   
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_CLI" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_CLI
                                              :sorting=filters_CLI.sorting
                                              t_core="customer.clientGrid"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_CLI.length > 0">
                        <tr v-for="cs, i in list_CLI" :key="i" class="table-row">
                            <td>{{cs.code}}</td>
                            <td>{{cs.name}}</td>
                            <td>{{cs.contactPerson}}</td>
                            <td>{{cs.telephoneNo}}</td>
                            <td>{{cs.faxNo}}</td>
                            <td>{{getCustomerClientStatus(cs.status)}}</td>
                            <div class="table-buttons">
                                <button class="btn btn-primary me-2 btn-sm" @click="addOrModify(cs)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-sm btn-primary me-2" @click="toggleCustomerClient(cs)"><i :class="['las',cs.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{cs.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_CLI.length == 0">
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
    <modify-customer-client-popup :modalBase="modifyCustomerClientModal" @close="closeModal(modifyCustomerClientModal)" @proceed="updateCustomerClient" />
    <processing-popup :modalBase=waitModal />
</template>