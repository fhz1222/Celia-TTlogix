<script lang="ts" setup>
    // import vue stuff
    import { useStore } from '@/store';
    import { ActionTypes } from '@/store/action-types';
    import { computed, ref, onActivated, onDeactivated } from 'vue';
    import bus from '@/event_bus.js';
    
    // import types
    import { MenuItem, TableColumn } from '@/store/commonModels/config';
    import { Modal } from '@/store/commonModels/modal';
    import { DecantFilters } from '@/store/models/decantFilters';
    import { Decant } from '@/store/models/decant';

    // import widgets and modals
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import AddDecant from '../modals/AddAdjustment.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import Paging from '../../../widgets/Paging.vue';
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';

    // import common logic
    import * as pagingFn from '@/store/commonFunctions/paging';
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    import * as routerFn from '@/router';
    
    // init vue stuff
    const store = useStore();

    // local vars
    const list_DEC = computed(() => store.state.decantList);
    const filters_DEC = ref(new DecantFilters());
    filters_DEC.value.whsCode = store.state.whsCode;
    const getDecantStatus = store.getters.getDecantStatus;
    const loadingList = ref(false);
    const customersDict: {[id: string]: string} = {};
    await store.dispatch(ActionTypes.INV_LOAD_CUSTOMERS);
    store.state.customers.filter(c => c.code != null).forEach(c => customersDict[c.code!] = c.name);

    // Print - old modal
    const modal: any = ref(null);

    // functions
    const addNewDecant = () => { addDecant.value.on = true; };
    const addNewDecantExecute = async(customerCode: string | null) => {
        closeModal(addDecant.value);
        if(customerCode){
            waitModal.value.on = true;
            await store.dispatch(ActionTypes.INV_DEC_ADD, {customerCode: customerCode})
                .then((jn) => { closeModal(waitModal.value); goToDecant(jn); })
                .catch(() => { closeModal(waitModal.value) });
        }
    };
    
    const reloadList = async () => {
        loadingList.value = true;
        await store.dispatch(ActionTypes.INV_DEC_LOAD_LIST, { filters: filters_DEC.value });
        loadingList.value = false;
    }
    const goToDecant = (jobNo: string) => { 
        goTo('decant_details', { jobNo: jobNo });
    };
    const complete = async (jobNo:string) => { 
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_DEC_COMPLETE, { jobNo: jobNo }).catch(() => { closeModal(waitModal.value) });
        waitModal.value.on = false;
    };
    const canComplete = (dec:Decant) => {
        return dec.status == 1;
    };
    const cancel = async (jobNo: string) => { 
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_DEC_CANCEL, { jobNo: jobNo }).catch(() => { closeModal(waitModal.value) });
        waitModal.value.on = false;
    };
    const canCancel = (dec:Decant) => {
        return dec.status < 2;
    };
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };

    // modals
    const waitModal = ref(new Modal(500, 'Processing', false));
    const addDecant = ref(new Modal(500,'Add Decant', true));
    const errorModal = ref(new Modal(500, 'Error', false));
    
    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // GoTo
    const goTo = (state: string, params?:any) => { return routerFn.goTo(state, params); }
    // Paging 
    const goToPage = (page: number) => { pagingFn.goToPage(page,filters_DEC.value.pagination); reloadList(); };
    const setItemsPerPage = (itemsPerPage: number) => { pagingFn.setItemsPerPage(itemsPerPage,filters_DEC.value.pagination); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column,value,filters_DEC.value); if(load) reloadList(); };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_DEC.value.sorting); reloadList(); };
    // Date
    const showDate = (date:string) => { return miscFn.showDate(date, true); }

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const decantStatus = store.state.config.dictionaries.decantStatus;

    // section Menu
    const menuItems_DEC:Array<MenuItem> = [
        { state: null, stateParams: null, title: 'addNew', function: addNewDecant, icon: 'la-plus-circle' },
       /* { state: null, stateParams: null, title: 'print labels', function: printPIDLabel, icon: 'la-print' },*/
        { state:null, stateParams:null, title:'refresh', function:reloadList, icon:'la-redo-alt', float: 'end' },
    ];

    // table related stuff
    const columns_DEC:Array<TableColumn> = [
        { alias:'no',          columnType:null,      dropdown:null,         sortable:false,f1:null,f2:null, resetTo: null  },
        { alias:'jobNo',       columnType:'string',  dropdown:null,         sortable:true, f1:'',  f2:null, resetTo: null  },
        { alias:'customerName',columnType:'dropdown',dropdown:customersDict,sortable:true, f1:'',  f2:null, resetTo: null, filterName: 'customerCode' },
        { alias:'referenceNo', columnType:'string',  dropdown:null,         sortable:true, f1:'',  f2:null, resetTo: null  },
        { alias:'createdDate', columnType:'date',    dropdown:null,         sortable:true, f1:null,f2:null, resetTo: null  },
        { alias:'status',      columnType:'dropdown',dropdown:decantStatus, sortable:true, f1:'7', f2:null, resetTo: '10'  },
        { alias:'remark',      columnType:'string',  dropdown:null,         sortable:true, f1:'',  f2:null, resetTo: null  }
    ];

    // init the screen
    onActivated(() => {
        reloadList();
        bus.on("refresh", reloadList);
    });
    onDeactivated(() => bus.off("refresh", reloadList))
</script>

<template>
    <section-menu :menu-items="menuItems_DEC" t_orig="inventory.operation.decantScreen" @action="menuAction" />
    <paging :pagingData="list_DEC.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" v-if="!loadingList" />
    <div>
        <div class="row py-2 px-3">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_DEC 
                                              :sorting=filters_DEC.sorting 
                                              t_core="inventory.decant.grid" 
                                              @sort="sort" 
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_DEC.items.length > 0">
                        <tr v-for="dec, i in list_DEC.items" :key="i" class="table-row" @dblclick="goToDecant(dec.jobNo)">
                            <td>{{(((filters_DEC.pagination.pageNumber - 1) * filters_DEC.pagination.itemsPerPage) + 1) + i}}</td>
                            <td>{{dec.jobNo}}</td>
                            <td>{{dec.customerName}}</td>
                            <td>{{dec.referenceNo}}</td>
                            <td>{{showDate(dec.createdDate)}}</td>
                            <td>{{getDecantStatus(dec.status)}}</td>
                            <td>{{dec.remark}}</td>
                            <div class="table-buttons">
                                <button class="btn btn-primary me-2 btn-sm" @click="complete(dec.jobNo)" v-if="canComplete(dec)"><i class="las la-check-circle"></i> Complete</button>
                                <button class="btn btn-primary me-2 btn-sm" @click="goToDecant(dec.jobNo)"><i class="las la-pen"></i> Modify</button>
                                <button class="btn btn-danger me-2 btn-sm" @click="cancel(dec.jobNo)" v-if="canCancel(dec)">
                                    <i class="las la-ban"></i> Cancel
                                </button>
                            </div>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_DEC.items.length == 0">
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
    <add-decant :modal-base=addDecant @close="closeModal(addDecant)" @addNew="addNewDecantExecute" />
    <paging :pagingData="list_DEC.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" v-if="!loadingList" />
    <processing-popup :modalBase=waitModal />
    <error-popup :modalBase=errorModal @close="closeModal(errorModal)" />
</template>
