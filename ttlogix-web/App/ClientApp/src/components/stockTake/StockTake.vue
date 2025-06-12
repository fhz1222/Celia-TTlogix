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
    import { StockTake } from '@/store/models/stockTake';
    import { StockTakeFilters } from '@/store/models/stockTakeFilters';

    // import widgets and modals
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import Paging from '@/widgets/Paging.vue';
    
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
    const list_STL = computed(() => store.state.stockTakeList);
    const filters_STL = ref(new StockTakeFilters());
    filters_STL.value.whsCode = store.state.whsCode;
    const loadingList = ref(false);
    const getStockTakeStatus = store.getters.getStockTakeStatus;

    // functions
    const reloadList = async (internalReload: boolean = false) => {
        loadingList.value = true;
        if (typeof (filters_STL.value.statuses) != 'object') {
            if (filters_STL.value.statuses == '10') filters_STL.value.statuses = null;
            if (filters_STL.value.statuses == '2') filters_STL.value.statuses = [0, 1];
        }
        filters_STL.value.whsCode = filters_STL.value.whsCode.trim() ? filters_STL.value.whsCode : store.state.whsCode;
        await store.dispatch(ActionTypes.STL_LOAD_LIST, { filters: filters_STL.value });
        loadingList.value = false;
    }
    const modify = (jobNo: string) => {
        goTo('stock-take-detail', { jobNo: jobNo });
    }
    const cancel = async (jobNo: string) => {
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.STL_CANCEL_JOB, { jobNo: jobNo }).catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value);
    }
    const complete = (jobNo: string) => {
        confirmModal.value.fnMod.message = t('generic.completeConfirm', { jobNo: jobNo });
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await completeExecute(jobNo) };
    }
    const completeExecute = async (jobNo: string) => {
        closeModal(confirmModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.STL_COMPLETE_JOB, { jobNo: jobNo }).catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value);
    }
    const canComplete = (stl: StockTake) => {
        return stl.status < 2;
    }
    const canCancel = (stl: StockTake) => {
        return stl.status != 8 && stl.status != 9;
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const confirmModal = ref(new Modal(600, t('generic.pleaseConfirm'), false));

    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_STL.value.sorting); reloadList(true); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_STL.value); if (load) reloadList(true); };
    // Paging 
    const goToPage = (page: number) => { pagingFn.goToPage(page, filters_STL.value.pagination); reloadList(); };
    const setItemsPerPage = (itemsPerPage: number) => { pagingFn.setItemsPerPage(itemsPerPage, filters_STL.value.pagination); reloadList(); };
    // GoTo
    const goTo = (state: string, params?: any) => { return routerFn.goTo(state, params); }

    // section Menu
    const menuItems_STL:Array<MenuItem> = [
        { state:null, stateParams:null, title:'refresh',function:reloadList, icon:'la-redo-alt', float: 'end' }
    ];

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const stockTakeStatus = store.state.config.dictionaries.stockTakeStatus;

    // table related stuff
    const columns_STL: Array<TableColumn> = [
        { alias:'jobNo',       columnType:'string',  dropdown:null, sortable:true, f1:'', f2:null, resetTo:null },
        { alias:'refNo',       columnType:'string',  dropdown:null, sortable:true, f1:'', f2:null, resetTo:null },
        { alias:'whsCode',     columnType:'string',  dropdown:null, sortable:true, f1:'', f2:null, resetTo:null },
        { alias:'locationCode',columnType:'string',  dropdown:null, sortable:true, f1:'', f2:null, resetTo:null },
        { alias:'createdDate', columnType:'date',    dropdown:null, sortable:true, f1:'', f2:null, resetTo:null },
        { alias:'status',      columnType:'dropdown',dropdown:stockTakeStatus, sortable: true, f1: '2', f2: null, resetTo: '10', filterName: 'statuses' },
        { alias:'remark',      columnType:'string',  dropdown:null, sortable: true, f1: '', f2: null, resetTo: null }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", () => reloadList())
    });
    onDeactivated(() => bus.off("refresh", () => reloadList()))
</script>
<template>
    <section-menu :menu-items="menuItems_STL" t_orig="inboundReversal.operation" @action="menuAction" />
    <paging :pagingData="list_STL.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" v-if="!loadingList" />
    <div class="container-fluid">
        <div class="row my-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_STL
                                              :sorting=filters_STL.sorting
                                              t_core="stockTake.mainGrid"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_STL.items.length > 0">
                        <tr v-for="stl, i in list_STL.items" :key="i" class="table-row" @dblclick="modify(stl.jobNo)">
                            <td>{{stl.jobNo}}</td>
                            <td>{{stl.refNo}}</td>
                            <td>{{stl.whsCode}}</td>
                            <td>{{stl.locationCode}}</td>
                            <td>{{stl.createdDate}}</td>
                            <td>{{getStockTakeStatus(stl.status)}}</td>
                            <td>{{stl.remark}}</td>
                            <div class="table-buttons">
                                <button class="btn btn-primary me-2 btn-sm" @click="modify(stl.jobNo)"><i class="las la-pen"></i> {{t('modify')}}</button>
                                <button class="btn btn-primary me-2 btn-sm" @click="complete(stl.jobNo)" v-if="canComplete(stl)"><i class="las la-check-circle"></i> {{t('operation.general.complete')}}</button>
                                <button class="btn btn-danger me-2 btn-sm" @click="cancel(stl.jobNo)" v-if="canCancel(stl)"><i class="las la-ban"></i> {{t('operation.general.cancel')}}</button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_STL.items.length == 0">
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
            <paging :pagingData="list_STL.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" />
        </div>
    </div>
    <confirm-popup :modalBase=confirmModal @close="closeModal(confirmModal)" />
    <processing-popup :modalBase=waitModal />
</template>