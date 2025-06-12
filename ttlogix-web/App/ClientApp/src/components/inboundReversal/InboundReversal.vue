<script lang="ts" setup>

    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref, onActivated, onDeactivated } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useI18n } from 'vue-i18n';

    // import types
    import { MenuItem, TableColumn } from '@/store/commonModels/config';
    import { Modal } from '@/store/commonModels/modal';
    import { InboundReversal } from '@/store/models/inboundReversal';
    import { InboundReversalFilters } from '@/store/models/inboundReversalFilters';

    // import widgets and modals
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import Paging from '@/widgets/Paging.vue';
    import AddInboundReversal from './modals/AddInboundReversal.vue'
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';
    
    // import common logic
    import * as pagingFn from '@/store/commonFunctions/paging';
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    import * as routerFn from '@/router';

    // init vue stuff
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });

    // local vars
    const filters_IR = ref(new InboundReversalFilters());
    filters_IR.value.whsCode = store.state.whsCode;
    const list_IR = computed(() => store.state.inboundReversalList);
    const customersDict: { [id: string]: string } = {};
    const loadingList = ref(false);
    const getInboundReversalStatus = store.getters.getInboundReversalStatus;
    
    
    
    // functions
    const reloadList = async () => {
        loadingList.value = true;
        if (typeof (filters_IR.value.statuses) != 'object') {
            if (filters_IR.value.statuses == '10') filters_IR.value.statuses = null;
            if (filters_IR.value.statuses == '2') filters_IR.value.statuses = [0, 1];
        }
        await store.dispatch(ActionTypes.IR_LOAD_LIST, { filters: filters_IR.value});
        loadingList.value = false;
    }
    const goToReversal = (jobNo: string) => { 
        goTo('inboundReversal-detail', { jobNo: jobNo } );
    };
    const complete = async (jobNo: string) => {
        confirmModal.value.fnMod.message = t('generic.completeConfirm', { jobNo: jobNo });
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await completeExecute(jobNo) };
    };
    const completeExecute = async (jobNo: string) => {
        closeModal(confirmModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.IR_COMPLETE, { jobNo: jobNo }).catch(() => { closeModal(waitModal.value) });
        closeModal(waitModal.value);
    };
    const canComplete = (rev: InboundReversal) => {
        return rev.status < 2;
    };
    const cancel = async (jobNo: string) => { 
        confirmModal.value.fnMod.message = t('generic.cancelConfirm', { jobNo: jobNo });
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await cancelExecute(jobNo) };
    };
    const cancelExecute = async (jobNo: string) => {
        closeModal(confirmModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.IR_CANCEL, { jobNo: jobNo }).catch(() => { closeModal(waitModal.value) });
        closeModal(waitModal.value);
    };
    const canCancel = (rev:InboundReversal) => {
        return rev.status < 2;
    };
    const addNewInboundReversal = () => { addModal.value.on = true; };
    const addNewInboundReversalExecute = async (inJobNo: string) => {
        closeModal(addModal.value);
        if (inJobNo) {
            waitModal.value.on = true;
            await store.dispatch(ActionTypes.IR_ADD_REVERSAL, { inJobNo: inJobNo }).catch(() => { closeModal(waitModal.value) })
                .then((nr) => { closeModal(waitModal.value); if(nr) goToReversal(nr.jobNo); })
                .catch(() => { closeModal(waitModal.value) });
        }
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const addModal = ref(new Modal(500, t('inboundReversal.operation.addReversal'), false));
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const confirmModal = ref(new Modal(500, t('generic.pleaseConfirm'), true));
    
    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // GoTo
    const goTo = (state: string, params?:any) => { return routerFn.goTo(state, params); }
    // Date
    const showDate = (date:string) => { return miscFn.showDate(date, true); }
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column,value,filters_IR.value); if(load) reloadList(); };
    // Paging 
    const goToPage = (page: number) => { pagingFn.goToPage(page,filters_IR.value.pagination); reloadList(); };
    const setItemsPerPage = (itemsPerPage: number) => { pagingFn.setItemsPerPage(itemsPerPage,filters_IR.value.pagination); reloadList(); };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_IR.value.sorting); reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const inboundReversalStatus = store.state.config.dictionaries.inboundReversalStatus;

    // get customers if there are no any in the store yet
    if (store.state.customers.length == 0) {
        await store.dispatch(ActionTypes.INV_LOAD_CUSTOMERS);
        const customers = computed(() => store.state.customers);
        store.state.customers.filter(c => c.code != null).forEach(c => customersDict[c.code!] = c.name);
    }

    // section Menu
    const menuItems_IR:Array<MenuItem> = [
        { state:null, stateParams:null, title:'addNew',  function:addNewInboundReversal, icon:'la-plus-circle' },
        { state:null, stateParams:null, title:'refresh', function:reloadList, icon:'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_IR:Array<TableColumn> = [
        { alias:'no',          columnType:null,      dropdown:null,             sortable:false,f1:null,f2:null, resetTo: null },
        { alias:'jobNo',       columnType:'string',  dropdown:null,             sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'customerName',columnType:'dropdown',dropdown:customersDict,    sortable:true, f1:'',  f2:null, resetTo: null, filterName:'customerCode' },
        { alias:'refNo',       columnType:'string',  dropdown:null,             sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'status',      columnType:'dropdown',dropdown:inboundReversalStatus, sortable:true, f1:'2', f2:null, resetTo:'10', filterName:'statuses' },
        { alias:'createdDate', columnType:'date',    dropdown:null,             sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'reason',      columnType:'string',  dropdown:null,             sortable:true, f1:'',  f2:null, resetTo: null }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("reloadList", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_IR" t_orig="inboundReversal.operation" @action="menuAction" />
    <paging :pagingData="list_IR.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" v-if="!loadingList" />
    <div>
        <div class="row py-2 px-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_IR
                                              :sorting=filters_IR.sorting
                                              t_core="inboundReversal.grid"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_IR.items.length > 0">
                        <tr v-for="rev, i in list_IR.items" :key="i" class="table-row" @dblclick="goToReversal(rev.jobNo)">
                            <td>{{(((filters_IR.pagination.pageNumber - 1) * filters_IR.pagination.itemsPerPage) + 1) + i}}</td>
                            <td>{{rev.jobNo}}</td>
                            <td>{{customersDict[rev.customerCode]}}</td>
                            <td>{{rev.refNo}}</td>
                            <td>{{getInboundReversalStatus(rev.status)}}</td>
                            <td>{{showDate(rev.createdDate)}}</td>
                            <td>{{rev.reason}}</td>
                            <div class="table-buttons">
                                <button class="btn btn-primary me-2 btn-sm" @click="goToReversal(rev.jobNo)"><i class="las la-pen"></i> {{ t('operation.general.modify') }}</button>
                                <button class="btn btn-primary me-2 btn-sm" @click="complete(rev.jobNo)" v-if="canComplete(rev)">
                                    <i class="las la-check-circle"></i> {{ t('operation.general.complete') }}
                                </button>
                                <button class="btn btn-danger me-2 btn-sm" @click="cancel(rev.jobNo)" v-if="canCancel(rev)">
                                    <i class="las la-ban"></i> {{ t('operation.general.cancel') }}
                                </button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_IR.items.length == 0">
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
    <div class="row pt-3 mb-4" v-if="!loadingList">
        <div class="col-md-10 ps-4">
            <paging :pagingData="list_IR.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" />
        </div>
    </div>
    <processing-popup :modalBase=waitModal />
    <add-inbound-reversal :modalBase=addModal @close="closeModal(addModal)" @addNew="addNewInboundReversalExecute" />
    <confirm-popup :modalBase=confirmModal @close="closeModal(confirmModal)" />
</template>