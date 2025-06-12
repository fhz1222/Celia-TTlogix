<script lang="ts" setup>
    // import event bus
    import bus from '@/event_bus.js'

    // import vue stuff
    import { useStore } from '@/store';
    import { computed, onActivated, onDeactivated, ref } from 'vue';
    import { ActionTypes } from '@/store/action-types';

    // import types
    import { Customer } from '@/store/models/customer';
    import { InventoryFilters } from '@/store/models/inventoryFilters';
    import { MenuItem, TableColumn } from '@/store/commonModels/config';

    // import widgets and modals
    import Paging from '../../widgets/Paging.vue'
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';

    // import common logic
    import * as pagingFn from '@/store/commonFunctions/paging';
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';

    // init vue stuff
    const store = useStore();

    // local vars
    const filters_INV = ref(new InventoryFilters());
    filters_INV.value.whsCode = store.state.whsCode;
    const list_INV = computed(() => store.state.inventoryList);
    const customers = computed(() => store.state.customers);
    const loadingList = ref(false);
    const getOwnership = store.getters.getOwnership;

    // Functions 
    const reloadList = async () => { 
        loadingList.value = true;  
        await store.dispatch(ActionTypes.INV_LOAD_LIST, { filters: filters_INV.value });
        loadingList.value = false; 
    };
    const changeCustomer = (customer:Customer) => {
        filters_INV.value.customerCode = customer?.code;
        filters_INV.value.pagination.pageNumber = 1;
        list_INV.value.items = [];
        const refreshNotPresentInMenu = !menuItems_INV.value.find(x => x.title == 'refresh');
        if (customer && refreshNotPresentInMenu) menuItems_INV.value = menuItems_INV.value.concat([{ state: null, stateParams: null, title: 'refresh', function: reloadList, icon: 'la-redo-alt', float: 'end' }]);
        else if(!customer) menuItems_INV.value = menuItems_INV.value.filter(x => x.title != 'refresh')
        reloadList();
    };

    // initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const ownershipDict = store.state.config.dictionaries.ownership;
    await store.dispatch(ActionTypes.INV_LOAD_CUSTOMERS);
    
    // Menu Action
    const menuAction = (menuFunction: Function) => { menuFunction() };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column,value,filters_INV.value); if(load) reloadList(); };
    // Paging 
    const goToPage = (page: number) => { pagingFn.goToPage(page,filters_INV.value.pagination); reloadList(); };
    const setItemsPerPage = (itemsPerPage: number) => { pagingFn.setItemsPerPage(itemsPerPage,filters_INV.value.pagination); reloadList(); };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_INV.value.sorting); reloadList(); };

    // Section Menu
    const menuItems_INV = ref<Array<MenuItem>>([
        { state:'adjustment',    icon:'la-sliders-h',   title:null, function:null, stateParams:null },
        { state:'quarantine',    icon:'la-warehouse',   title:null, function:null, stateParams:null },
        { state:'relocation',    icon:'la-random',      title:null, function:null, stateParams:null }, 
        { state:'decant',        icon:'la-boxes',       title:null, function:null, stateParams:null }, 
        { state:'storage_detail',icon:'la-dolly',       title:null, function:null, stateParams:null }, 
        { state:'reports',       icon:'la-file-alt',    title:null, function:null, stateParams:null }
    ]);

    // Table related stuff
    const columns_INV:Array<TableColumn> = [
        {alias:'no',           columnType:null,      dropdown:null,         sortable: false,f1:null, f2:null, resetTo:null},
        {alias:'productCode',  columnType:'string',  dropdown:null,         sortable: true, f1:'',   f2:null, resetTo:null},
        {alias:'supplierId',   columnType:'string',  dropdown:null,         sortable: true, f1:'',   f2:null, resetTo:null},
        {alias:'ownership',    columnType:'dropdown',dropdown:ownershipDict,sortable: true, f1:'',   f2:null, resetTo:null},
        {alias:'incomingQty',  columnType:'range',   dropdown:null,         sortable: true, f1:null, f2:null, resetTo:null},
        {alias:'onHandQty',    columnType:'range',   dropdown:null,         sortable: true, f1:null, f2:null, resetTo:null},
        {alias:'allocatedQty', columnType:'range',   dropdown:null,         sortable: true, f1:null, f2:null, resetTo:null},
        {alias:'quarantineQty',columnType:'range',   dropdown:null,         sortable: true, f1:null, f2:null, resetTo:null},
        {alias:'pickableQty',  columnType:'range',   dropdown:null,         sortable: true, f1:null, f2:null, resetTo:null},
        {alias:'bondedQty',    columnType:'range',   dropdown:null,         sortable: true, f1:null, f2:null, resetTo:null},
        {alias:'nonBondedQty', columnType:'range',   dropdown:null,         sortable: true, f1:null, f2:null, resetTo:null}
    ];

    // init the screen
    reloadList();

    onActivated(() => bus.on("refresh", reloadList))
    onDeactivated(() => bus.off("refresh", reloadList))
</script>
<template>
    <section-menu :menu-items="menuItems_INV" t_orig="inventory.operation"  @action="menuAction" />

    <div>
        <div class="row pt-3 pb-2 px-2">
            <div class="col-md-2 multiselect-as-select">
                <v-select :options="customers"                     
                          @update:modelValue="changeCustomer"
                          placeholder="Select a customer">
                          <template v-slot:selected-option="option">{{ option.code + ' - ' + option.name }}</template>
                          <template v-slot:option="option">{{option.code}} - {{option.name}}</template>
                </v-select>
            </div>
            <div class="col-md-10">
                <paging :pagingData="list_INV.pagination" :padding=false @go-to-page="goToPage" @set-items-per-page="setItemsPerPage"/>
            </div>
        </div>
        <div class="row p-2">
            <div class="col-md-12">
                <table class="table table-striped table-hover filtered-sorted-table" v-if="filters_INV.customerCode">
                    <table-sort-filter-header :columns=columns_INV 
                                              :sorting=filters_INV.sorting 
                                              t_core="inventory.mainGrid" 
                                              @sort="sort" 
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_INV.items.length > 0">
                        <tr v-for="inv, i in list_INV.items" :key="i">
                            <td>{{(((filters_INV.pagination.pageNumber - 1) * filters_INV.pagination.itemsPerPage) + 1) + i}}</td>
                            <td>{{inv.product.code}}</td>
                            <td>{{inv.product.customerSupplier.supplierId}}</td>
                            <td>{{getOwnership(inv.ownership)}}</td>
                            <td>{{inv.incomingQty}}</td>
                            <td>{{inv.onHandQty}}</td>
                            <td>{{inv.allocatedQty}}</td>
                            <td>{{inv.quarantineQty}}</td>
                            <td>{{inv.pickableQty}}</td>
                            <td>{{inv.bondedQty}}</td>
                            <td>{{inv.nonBondedQty}}</td>
                        </tr>
                    </tbody>
                </table>
                <div class="text-center pt-5 main-action-icons-small" v-if="filters_INV.customerCode && !loadingList && list_INV.items.length == 0">
                    <h5><i class="las la-ban"></i> No records found :(</h5>
                </div>
                <div class="text-center pt-5 main-action-icons-small" v-if="!filters_INV.customerCode && !loadingList">
                    <h5><i class="las la-list-alt"></i> Select a customer to load the records</h5>
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

        <div class="row pt-3 mb-4" v-if="!loadingList">
            <div class="col-md-10 ps-4">
                <paging :pagingData="list_INV.pagination" :padding=false @go-to-page="goToPage" @set-per-page="setItemsPerPage"/>
            </div>
        </div>
    </div>
</template> 