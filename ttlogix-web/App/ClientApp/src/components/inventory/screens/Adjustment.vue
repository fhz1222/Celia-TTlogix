<script lang="ts" setup>

    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref, onActivated, onDeactivated } from 'vue';
    import { ActionTypes } from '@/store/action-types';

    // import types
    import { MenuItem, TableColumn } from '@/store/commonModels/config';
    import { Modal } from '@/store/commonModels/modal';
    import { Adjustment } from '@/store/models/adjustment';
    import { AdjustmentFilters } from '@/store/models/adjustmentFilters';

    // import widgets and modals
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import Paging from '../../../widgets/Paging.vue';
    import AddAdjustment from '../modals/AddAdjustment.vue';
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';
    
    // import common logic
    import * as pagingFn from '@/store/commonFunctions/paging';
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    import * as routerFn from '@/router';

    // init vue stuff
    const store = useStore();

    // local vars
    const filters_ADJ = ref(new AdjustmentFilters());
    filters_ADJ.value.whsCode = store.state.whsCode;
    const list_ADJ = computed(() => store.state.adjustmentList);
    const loadingList = ref(false);
    const undoZeroOut = ref(false);
    const getAdjustmentStatus = store.getters.getAdjustmentStatus;
    const getAdjustmentJobType = store.getters.getAdjustmentJobType;
    const customersDict: { [id: string]: string } = {};
    store.state.customers.filter(c => c.code != null).forEach(c => customersDict[c.code!] = c.name);

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        await store.dispatch(ActionTypes.INV_LOAD_ADJ_LIST, {filters: filters_ADJ.value});
        loadingList.value = false;
    }
    const goToAdjustment = (jobNo: string) => { 
        goTo('adjustment-item', { jobNo: jobNo } );
    };
    const complete = async (jobNo: string) => { 
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_COMPLETE_ADJ, { jobNo: jobNo }).catch(() => { closeModal(waitModal.value) });
        closeModal(waitModal.value);
    };
    const canComplete = (adj:Adjustment) => {
        return adj.status == 1;
    };
    const cancel = async (jobNo: string) => { 
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_CANCEL_ADJ, { jobNo: jobNo }).catch(() => { closeModal(waitModal.value) });
        closeModal(waitModal.value);
    };
    const canCancel = (adj:Adjustment) => {
        return adj.status < 2;
    };
    const addNewAdjustment = (uzo:boolean) => { undoZeroOut.value = uzo; addModal.value.on = true; };
    const addNewAdjustmentExecute = async (customerCode:string | null) => {
        closeModal(addModal.value);
        if(customerCode){
            waitModal.value.on = true;
            await store.dispatch(ActionTypes.INV_ADD_ADJ, {customerCode: customerCode, isUndoZeroOut: undoZeroOut.value})
                .then((jn) => { closeModal(waitModal.value); goToAdjustment(jn.jobNo); })
                .catch(() => { closeModal(waitModal.value) });
        }
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const addModal = ref(new Modal(500,'Add adjustment item',true));
    const waitModal = ref(new Modal(500,'Processing', false));
    const errorModal = ref(new Modal(500, 'Error', false));
    
    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // GoTo
    const goTo = (state: string, params?:any) => { return routerFn.goTo(state, params); }
    // Date
    const showDate = (date:string) => { return miscFn.showDate(date, true); }
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column,value,filters_ADJ.value); if(load) reloadList(); };
    // Paging 
    const goToPage = (page: number) => { pagingFn.goToPage(page,filters_ADJ.value.pagination); reloadList(); };
    const setItemsPerPage = (itemsPerPage: number) => { pagingFn.setItemsPerPage(itemsPerPage,filters_ADJ.value.pagination); reloadList(); };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_ADJ.value.sorting); reloadList(); };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const adjustmentStatus = store.state.config.dictionaries.adjustmentStatus;
    const adjustmentJobType = store.state.config.dictionaries.adjustmentJobType;

    // section Menu
    const menuItems_ADJ:Array<MenuItem> = [
        { state:null, stateParams:null, title:'addNew',      function: () => addNewAdjustment(false), icon:'la-plus-circle' },
        { state:null, stateParams:null, title:'undoZeroOut', function: () => addNewAdjustment(true),  icon:'la-business-time' },
        { state:null, stateParams:null, title:'refresh', function:reloadList, icon:'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_ADJ:Array<TableColumn> = [
        { alias:'no',          columnType:null,      dropdown:null,             sortable:false,f1:null,f2:null, resetTo: null },
        { alias:'jobNo',       columnType:'string',  dropdown:null,             sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'customerName',columnType:'dropdown',dropdown:customersDict,    sortable:true, f1:'',  f2:null, resetTo: null, filterName: 'customerCode' },
        { alias:'referenceNo', columnType:'string',  dropdown:null,             sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'jobType',     columnType:'dropdown',dropdown:adjustmentJobType,sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'createdDate', columnType:'date',    dropdown:null,             sortable:true, f1:null,f2:null, resetTo: null },
        { alias:'status',      columnType:'dropdown',dropdown:adjustmentStatus, sortable:true, f1:'3', f2:null, resetTo: '11' },
        { alias:'reason',      columnType:'string',  dropdown:null,             sortable:true, f1:'',  f2:null, resetTo: null }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_ADJ" t_orig="inventory.operation.adjustmentScreen" @action="menuAction" />
    <paging :pagingData="list_ADJ.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" v-if="!loadingList" />
    <div>
        <div class="row py-2 px-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_ADJ 
                                              :sorting=filters_ADJ.sorting 
                                              t_core="inventory.adjustmentGrid" 
                                              @sort="sort" 
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_ADJ.items.length > 0">
                        <tr v-for="adj, i in list_ADJ.items" :key="i" class="table-row" @dblclick="goToAdjustment(adj.jobNo)">
                            <td>{{(((filters_ADJ.pagination.pageNumber - 1) * filters_ADJ.pagination.itemsPerPage) + 1) + i}}</td>
                            <td>{{adj.jobNo}}</td>
                            <td>{{adj.customerName}}</td>
                            <td>{{adj.referenceNo}}</td>
                            <td>{{getAdjustmentJobType(adj.jobType)}}</td>
                            <td>{{showDate(adj.createdDate)}}</td>
                            <td>{{getAdjustmentStatus(adj.status)}}</td>
                            <td>{{adj.reason}}</td>
                            <div class="table-buttons">
                                <button class="btn btn-primary me-2 btn-sm" @click="goToAdjustment(adj.jobNo)"><i class="las la-pen"></i> Modify</button>
                                <button class="btn btn-primary me-2 btn-sm" @click="complete(adj.jobNo)" v-if="canComplete(adj)">
                                    <i class="las la-check-circle"></i> Complete
                                </button>
                                <button class="btn btn-danger me-2 btn-sm" @click="cancel(adj.jobNo)" v-if="canCancel(adj)">
                                    <i class="las la-ban"></i> Cancel
                                </button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_ADJ.items.length == 0">
                    <h5><i class="las la-ban"></i> No records found :(</h5>
                </div>
                <div v-if="loadingList" class="bars1-wrapper">
                    <div id="bars1">
                        <span></span>
                        <span></span>
                        <span></span>
                        <span></span>
                        <span></span>
                    </div>
                    <h5>Loading...</h5>
                </div>
            </div>
        </div>
    </div>
    <div class="row pt-3 mb-4" v-if="!loadingList">
        <div class="col-md-10 ps-4">
            <paging :pagingData="list_ADJ.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage"/>
        </div>
    </div>
    <add-adjustment :modalBase=addModal @close="closeModal(addModal)" @addNew="addNewAdjustmentExecute" />
    <processing-popup :modalBase=waitModal />
    <error-popup :modalBase=errorModal @close="closeModal(errorModal)" />
</template>