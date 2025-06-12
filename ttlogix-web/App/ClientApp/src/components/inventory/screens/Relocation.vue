<script lang="ts" setup>

    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, onActivated, onDeactivated, ref } from 'vue';
    import auth from '@/auth';
    
    // import types
    import { ActionTypes } from '@/store/action-types';
    import { RelocationFilters } from '@/store/models/relocationFilters';
    import { TableColumn } from '@/store/commonModels/config';
    import { PaginationData } from '@/store/commonModels/paginationData';
    import { MenuItem } from '@/store/commonModels/config';
    import { Range } from '@/store/commonModels/Range';

    // import widgets and modals
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import Paging from '../../../widgets/Paging.vue';
    import moment from 'moment';
    import SectionMenu from '@/widgets/SectionMenu.vue';

    // import common logic
    import * as pagingFn from '@/store/commonFunctions/paging';
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';

    // init vue stuff
    const store = useStore();

    // local vars
    const list_REL = computed(() => store.state.relocationList);
    list_REL.value.items = [];
    const filters_REL = ref(new RelocationFilters());
    filters_REL.value.newWhsCode = auth.user().whsCode;
    filters_REL.value.oldWhsCode = auth.user().whsCode;
    const getScannerType = store.getters.getScannerType;
    const loadingList = ref(false);
    const customersDict: { [id: string]: string } = {};
    store.state.customers.filter(c => c.code != null).forEach(c => customersDict[c.code!] = (c.code + ' - ' + c.name));

    // functions
    const mandatoryFiltersSet = () => {
        return filters_REL.value.pid || filters_REL.value.externalPID || (filters_REL.value.relocationDate && relocationDateValid());
    }
    const relocationDateValid = () => {
        const from = filters_REL.value.relocationDate?.from;
        var to = filters_REL.value.relocationDate?.to;
        if(from && !to) { to = filters_REL.value.relocationDate.to = moment().format("YYYY-MM-DD") }
        if((from && typeof(from) == 'string') && (to && typeof(to) == 'string')){
            const fromDate = new Date(from);
            const toDate = new Date(to);
            if(fromDate < toDate){
                return ((toDate.getTime() - fromDate.getTime()) / (1000*3600*24)) < 92;
            }
        } else{ 
            return false;
        }
    }
    const reloadList = async (init: boolean = false) => {
        if(init){
            filters_REL.value.relocationDate = new Range();
            filters_REL.value.relocationDate.from = moment().add(-88,'days').format("YYYY-MM-DD");
            filters_REL.value.relocationDate.to = moment().format("YYYY-MM-DD");
        }
        if(mandatoryFiltersSet()){
            loadingList.value = true;
            await store.dispatch(ActionTypes.INV_REL_LOAD_LIST, { filters: filters_REL.value });
            loadingList.value = false;
        } else {
            list_REL.value.items = [];
            list_REL.value.pagination = new PaginationData();
        }
    }
    const menuAction = (menuFunction: Function) => { menuFunction() };

    // Paging 
    const goToPage = (page: number) => { pagingFn.goToPage(page,filters_REL.value.pagination); reloadList(); };
    const setItemsPerPage = (itemsPerPage: number) => { pagingFn.setItemsPerPage(itemsPerPage,filters_REL.value.pagination); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { 
        filteringFn.applyFilters(column,value,filters_REL.value); if(load) reloadList(); 
    };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_REL.value.sorting); reloadList(); };
    // Date
    const showDate = (date:string) => { return miscFn.showDate(date, true); }

    // init the screen
    await store.dispatch(ActionTypes.LOAD_DICT);
    const scannerType = store.state.config.dictionaries.scannerType;
    reloadList(true);

    // section Menu
    const menuItems_REL:Array<MenuItem> = [
        { state:null, stateParams:null, title:'refresh', function:reloadList, icon:'la-redo-alt', float: 'end' },
    ];

    // Table related stuff
    var from = new Date(moment().add(-88,'days').toString());
    const columns_REL:Array<TableColumn> = [
        { alias:'no',            columnType:null,      dropdown:null,       sortable:false,f1:null,f2:null, resetTo:null },
        { alias:'pid',           columnType:'string',  dropdown:null,       sortable:true, f1:'',  f2:null, resetTo:null },
        { alias:'externalPID',   columnType:'string',  dropdown:null,       sortable:true, f1:'',  f2:null, resetTo:null },
        { alias:'supplierId',    columnType:'string',  dropdown:null,       sortable: true,f1:'',  f2:null, resetTo:null },
        { alias:'customerCode',  columnType:'dropdown',dropdown:customersDict,sortable: true,f1:'',f2:null, resetTo:null, filterName: 'customerCode' },
        { alias:'productCode',   columnType:'string',  dropdown:null,       sortable:true, f1:'',  f2:null, resetTo:null },
        { alias:'qty',           columnType:'range',   dropdown:null,       sortable:true, f1:null,f2:null, resetTo:null },
        { alias:'oldLocation',   columnType:'string',  dropdown:null,       sortable:true, f1:'',  f2:null, resetTo:null },
        { alias:'newLocation',   columnType:'string',  dropdown:null,       sortable:true, f1:'',  f2:null, resetTo:null },
        { alias: 'relocatedBy', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null },
        { alias: 'relocationDate', columnType: 'date', dropdown: null, sortable: true, f1: from, f2: new Date(), resetTo: from, resetF2To: new Date(), resetToDefault: true },
        { alias:'scannerType',   columnType:'dropdown',dropdown:scannerType,sortable:true, f1:null,f2:null, resetTo:null }
    ];

    const setTableHeight = () => {
        const height = window.innerHeight;
        return (height - 240) + 'px';
    }

    onActivated(() => bus.on("refresh", () => reloadList(false)));
    onDeactivated(() => bus.off("refresh", () => reloadList(false)));
</script>

<template>
    <div>
        <div class="row py-2 px-3">
            <div class="col-md-12">
                <div class="table-responsive" style="overflow: auto" v-bind:style="{'height': setTableHeight()}">
                    <table class="table table-striped table-hover filtered-sorted-table">
                        <table-sort-filter-header :columns=columns_REL
                                                  :sorting=filters_REL.sorting
                                                  t_core="inventory.relocation.grid"
                                                  @sort="sort"
                                                  @apply-filters="applyFilters"
                                                  :sortable=true
                                                  :filtreable=true />
                        <tbody>
                            <tr v-for="rel, i in list_REL.items" :key="i" class="table-row">
                                <td>{{(((filters_REL.pagination.pageNumber - 1) * filters_REL.pagination.itemsPerPage) + 1) + i}}</td>
                                <td>{{rel.pid}}</td>
                                <td>{{rel.externalPID}}</td>
                                <td>{{rel.supplierId}}</td>
                                <td>{{rel.customerCode}}</td>
                                <td>{{rel.productCode}}</td>
                                <td>{{rel.qty}}</td>
                                <!-- <td>{{rel.oldWhsCode}}</td> -->
                                <td>{{rel.oldLocation}}</td>
                                <!-- <td>{{rel.newWhsCode}}</td> -->
                                <td>{{rel.newLocation}}</td>
                                <td>{{rel.relocatedBy}}</td>
                                <td>{{showDate(rel.relocationDate)}}</td>
                                <td>{{getScannerType(rel.scannerType)}}</td>
                            </tr>
                        </tbody>
                    </table>
                    <div class="text-center pt-5 main-action-icons-small" v-if="mandatoryFiltersSet() && !loadingList && list_REL.items.length == 0">
                        <h5><i class="las la-ban"></i> No records found :(</h5>
                    </div>
                    <div class="text-center pt-5 main-action-icons-small" v-if="!mandatoryFiltersSet() && !loadingList">
                        <h5><i class="las la-exclamation-triangle"></i> Enter at least the PID, ExternalPID or Relocation date range</h5>
                        <h5 class="mt-5 text-danger" v-if="filters_REL.relocationDate?.from && !relocationDateValid()">
                            <i class="las la-exclamation-triangle text-danger"></i>
                            Invalid relocation dates given. Maximum distance between dates must be smaller than 90 days
                        </h5>
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
    </div>
    <div class="row">
        <div class="col-md-9">
            <paging :pagingData="list_REL.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" v-if="!loadingList" />
        </div>
        <div class="col-md-3">
            <section-menu :menu-items="menuItems_REL" t_orig="inventory.operation.storageDetailGrid" top="false" background=0 @action="menuAction" />
        </div>
    </div>
    
</template>
