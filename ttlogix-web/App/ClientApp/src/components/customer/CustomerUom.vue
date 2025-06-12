<script lang="ts" setup>

    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref, onActivated, onDeactivated, watch } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useI18n } from 'vue-i18n';
    import { useRoute } from 'vue-router';

    // import types
    import { MenuItem, TableColumn } from '@/store/commonModels/config';
    import { Modal } from '@/store/commonModels/modal';
    import { CustomerUomFilters } from '@/store/models/customerUomFilters'
    import { CustomerUom } from '@/store/models/customerUom'

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ModifyCustomerUomPopup from './modals/ModifyCustomerUom.vue'
    
    // import common logic
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
    const list_UOM = ref(new Array<CustomerUom>());
    const filters_UOM = ref(new CustomerUomFilters());
    filters_UOM.value.customerCode = customerCode.value;
    const loadingList = ref(false);
    const getCustomerUomStatus = store.getters.getCustomerUomStatus;

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        filters_UOM.value.status = filters_UOM.value.status == 3 ? null : filters_UOM.value.status;
        await store.dispatch(ActionTypes.CS_LOAD_UOM_LIST, { filters: filters_UOM.value })
                   .then((uom) => list_UOM.value = uom)
                   .catch(() => loadingList.value = false);
        await store.dispatch(ActionTypes.CS_LOAD_GLOBAL_UOM).catch(() => loadingList.value = false);
        loadingList.value = false;
    }
    const modifyUom = (uom: CustomerUom | null = null) => {
        modifyUomModal.value.on = true;
        modifyUomModal.value.title = !uom ? (t('customer.uomGrid.addUom') + customerCode.value) : t('customer.uomGrid.modifyUom');
        modifyUomModal.value.customProps = { mUom: uom ? { ...uom } : null, customerCode: customerCode.value };
    }
    const updateUom = async (updatingUom: CustomerUom, addNext: boolean) => {
        if (!addNext) closeModal(modifyUomModal.value);
        waitModal.value.on = true;
        updatingUom.customerCode = customerCode.value; 
        await store.dispatch(ActionTypes.CS_UPDATE_UOM, { customerUom: updatingUom })
                   .catch(() => waitModal.value.on = false);
        closeModal(waitModal.value);
        reloadList();
    }
    const toggleUom = async (uom: CustomerUom) => {
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.CS_TOGGLE_UOM, { customerCode: customerCode.value, code: uom.uom ?? '' })
                    .then((updatedUom) => {
                        const lIndex = list_UOM.value.findIndex((lu) => lu.uom == uom.uom);
                        list_UOM.value[lIndex].status = updatedUom.status;
                    }).catch(() => waitModal.value.on = false);
        closeModal(waitModal.value);
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const modifyUomModal = ref(new Modal(400, '', true));
    
    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_UOM.value.sorting); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_UOM.value); if (load) reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const customerUomStatus = store.state.config.dictionaries.customerUomStatus;

    // section Menu
    const menuItems_UOM:Array<MenuItem> = [
        { state:null, stateParams:null, title:'addNew',  function:() => modifyUom(), icon:'la-plus-circle' },
        { state:null, stateParams:null, title:'refresh', function:reloadList, icon:'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_UOM: Array<TableColumn> = [
        { alias: 'name', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'decimalNum', columnType: 'range', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'status', columnType: 'dropdown', dropdown: customerUomStatus, sortable: true, f1: '3', f2: null, resetTo: '3' }   
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_UOM" t_orig="inboundReversal.operation" @action="menuAction" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_UOM
                                              :sorting=filters_UOM.sorting
                                              t_core="customer.uomGrid"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_UOM.length > 0">
                        <tr v-for="u, i in list_UOM" :key="i" class="table-row">
                            <td>{{u.name}}</td>
                            <td>{{u.decimalNum}}</td>
                            <td>{{getCustomerUomStatus(u.status)}}</td>
                            <div class="table-buttons">
                                <button class="btn btn-primary me-2 btn-sm" @click="modifyUom(u)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-sm btn-primary me-2" @click="toggleUom(u)"><i :class="['las',u.status == 1 ? 'la-ban' : 'la-redo-alt']"></i> {{u.status == 1 ? t('operation.general.deactivate') : t('operation.general.reactivate')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_UOM.length == 0">
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
    <modify-customer-uom-popup :modalBase="modifyUomModal" @close="closeModal(modifyUomModal)" @proceed="updateUom" />
    <processing-popup :modalBase=waitModal />
</template>