<script lang="ts" setup>

    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, onActivated, onDeactivated, ref } from 'vue';

    // import types
    import { MenuItem, TableColumn } from '@/store/commonModels/config';
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { QuarantineFilters } from '@/store/models/quarantineFilters';
    import { Quarantine } from '@/store/models/quarantine';
    import { ReportFilter } from '@/store/commonModels/reportFilter';

    // import widgets and modals
    import SectionMenu from '@/widgets/SectionMenu.vue';
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import Paging from '../../../widgets/Paging.vue';
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import QuarantineReason from '../modals/QuarantineReason.vue';
    import ReportPopup from '../modals/Report.vue';
    import PDFPreviewer from '@/widgets/PDFPreviewer.vue';

    // import common logic
    import * as pagingFn from '@/store/commonFunctions/paging';
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';

    // init vue stuff
    const store = useStore();

    // local vars
    const list_QNE = computed(() => store.state.quarantineList);
    const filters_QNE = ref(new QuarantineFilters());
    const loadingList = ref(false);
    const customersDict: {[id: string]: string} = {};
    await store.dispatch(ActionTypes.INV_LOAD_CUSTOMERS);
    store.state.customers.filter(c => c.code != null).forEach(c => customersDict[c.code!] = c.name);

    // functions
    const reloadList = async () => {
        loadingList.value = true;
        await store.dispatch(ActionTypes.INV_QNE_LOAD_LIST, { filters: filters_QNE.value });
        loadingList.value = false;
    }
    const changeReason = (item:Quarantine) => {
        const singleChange = areSelected().length > 0;
        reasonModal.value.customProps = !singleChange ? Array(item) : areSelected().map(o => o);
        reasonModal.value.on = true;
    }
    const changeReasonExecute = async(pids: Array<string>, reason: string):Promise<void> => {
        closeModal(reasonModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_QNE_REASON, { pids, reason }).catch(() => { closeModal(waitModal.value) });
        list_QNE.value.items.forEach((el)=>{el.selected = false;});
        waitModal.value.on = false;
    }
    const report = () => {
        reportModal.value.on = true;
        reportModal.value.customProps = { report: null, showBonded: false, showDates: false, showProducts: true, showRepBy: true }
    }
    const printReport = async (modalToClose: Modal | null = null, filter:ReportFilter | null = null) => {
        if(modalToClose) closeModal(reportModal.value);
        waitModal.value.on = true;
        if(filter) {
            filter.whsCode = store.state.whsCode;
            await store.dispatch(ActionTypes.REPORT_QUARANTINE, { filter })
                    .then((report) => {
                        waitModal.value.on = false;
                        pdfPreviewer.value.customProps = { file: report, name: 'Quarantine Report' };
                        pdfPreviewer.value.on = true;
                    })
                .catch(() => { closeModal(waitModal.value) });
        }
    }
    const menuAction = (menuFunction: Function) => { menuFunction() };
    const toggleSelected = (qne: Quarantine):void => {
        const objIndex = list_QNE.value.items.findIndex((obj => obj.pid == qne.pid));
        list_QNE.value.items[objIndex].selected = !list_QNE.value.items[objIndex].selected;
    };
    const reasonButton = () => {
        return areSelected().length > 0 ? "Reason for selected" : "Reason";
    }
    const areSelected = () => {
        var result = list_QNE.value.items.filter(obj => {
            return obj.selected === true;
        });
        return result;
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const reasonModal = ref(new Modal(500,'Update Reason', true));
    const waitModal = ref(new Modal(500,'Working', false));
    const reportModal = ref(new Modal(650,'Reports', true));
    const pdfPreviewer = ref(new Modal('90%','Report',true));

    // Paging 
    const goToPage = (page: number) => { pagingFn.goToPage(page,filters_QNE.value.pagination); reloadList(); };
    const setItemsPerPage = (itemsPerPage: number) => { pagingFn.setItemsPerPage(itemsPerPage,filters_QNE.value.pagination); reloadList(); };
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { 
        filteringFn.applyFilters(column,value,filters_QNE.value); if(load) reloadList(); 
    };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_QNE.value.sorting); reloadList(); };
    // Date
    const showDate = (date:string) => { return miscFn.showDate(date, true); }

    // Initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);

    // Section Menu
    const menuItems_QNE:Array<MenuItem> = [
        { state:null, stateParams:null, title:'reports', function:report, icon:'la-file-alt' },
        { state:null, stateParams:null, title:'refresh', function:reloadList, icon:'la-redo-alt', float: 'end' },
    ];

    // Table related stuff
    const columns_QNE:Array<TableColumn> = [
        { alias:'',              columnType:null,      dropdown:null,         sortable:false,f1:null,f2:null, resetTo: null },
        { alias:'no',            columnType:null,      dropdown:null,         sortable:false,f1:null,f2:null, resetTo: null },
        { alias:'pid',           columnType:'string',  dropdown:null,         sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'productCode',   columnType:'string',  dropdown:null,         sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'supplierId',    columnType:'string',  dropdown:null,         sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'customerName',  columnType:'dropdown',dropdown:customersDict,sortable:true, f1:'',  f2:null, resetTo: null, filterName: 'customerCode' },
        { alias:'qty',           columnType:'range',   dropdown:null,         sortable:true, f1:null,f2:null, resetTo: null },
        { alias:'location',      columnType:'string',  dropdown:null,         sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'decimalNum',    columnType:'range',   dropdown:null,         sortable:true, f1:null,f2:null, resetTo: null },
        { alias:'reason',        columnType:'string',  dropdown:null,         sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'createdBy',     columnType:'string',  dropdown:null,         sortable:true, f1:'',  f2:null, resetTo: null },
        { alias:'quarantineDate',columnType:'date',    dropdown:null,         sortable:true, f1:null,f2:null, resetTo: null }
    ];

    const setTableHeight = () => {
        const height = window.innerHeight;
        return (height - 240) + 'px';
    }

    // init the screen
    reloadList();
    
    onActivated(() => bus.on("refresh", reloadList))
    onDeactivated(() => bus.off("refresh", reloadList))
</script>

<template>
    <!--section-menu :menu-items="menuItems_QNE" t_orig="inventory.operation.quarantineScreen" @action="menuAction" />
    <paging :pagingData="list_QNE.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" v-if="!loadingList" /-->
    <!--div-->
        <div class="row py-2 px-3">
            <div class="col-md-12">
                <div class="table-responsive" style="overflow: auto" v-bind:style="{'height': setTableHeight()}">
                    <table class="table table-striped table-hover filtered-sorted-table">
                        <table-sort-filter-header :columns=columns_QNE
                                                  :sorting=filters_QNE.sorting
                                                  t_core="inventory.quarantine.grid"
                                                  @sort="sort"
                                                  @apply-filters="applyFilters"
                                                  :sortable=true
                                                  :filtreable=true />
                        <tbody v-if="!loadingList && list_QNE.items.length > 0">
                            <tr v-for="qne, i in list_QNE.items" :key="i" class="table-row" :class="qne.selected ? 'selected-row' : ''" @click="toggleSelected(qne)">
                                <td><input type="checkbox" v-model="qne.selected" class="form-check-input"></td>
                                <td>{{(((filters_QNE.pagination.pageNumber - 1) * filters_QNE.pagination.itemsPerPage) + 1) + i}}</td>
                                <td>{{qne.pid}}</td>
                                <td>{{qne.productCode}}</td>
                                <td>{{qne.supplierId}}</td>
                                <td>{{qne.customerName}}</td>
                                <td>{{qne.qty}}</td>
                                <td>{{qne.location}}</td>
                                <td>{{qne.decimalNum}}</td>
                                <td style="white-space: normal !important; word-wrap: break-word !important; min-width: 280px; max-width: 280px;">{{qne.reason}}</td>
                                <td>{{qne.createdBy}}</td>
                                <td>{{showDate(qne.quarantineDate)}}</td>
                                <div class="table-buttons">
                                    <button class="btn btn-primary me-2 btn-sm" @click.stop.prevent="changeReason(qne)"><i class="las la-pen"></i> {{reasonButton()}}</button>
                                </div>
                            </tr>
                        </tbody>
                    </table>
                    <h5 class="text-center pt-5 main-action-icons-small" v-if="!loadingList && list_QNE.items.length == 0">
                        <i class="las la-ban"></i> No records found :(
                    </h5>
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

    <div class="row">
        <div class="col-md-9">
            <paging :pagingData="list_QNE.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" v-if="!loadingList" />
        </div>
        <div class="col-md-3 text-end">
            <section-menu :menu-items="menuItems_QNE" t_orig="inventory.operation.quarantineScreen" top="false" background=0 @action="menuAction" />
        </div>
    </div>

    <!--/div-->
    <!--paging :pagingData="list_QNE.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" v-if="!loadingList" /-->
    <quarantine-reason :modalBase=reasonModal @close="closeModal(reasonModal)" @update-reason="changeReasonExecute" />
    <report-popup :modalBase=reportModal @close="closeModal(reportModal)" @print="printReport" :show-bonded=false />
    <p-d-f-previewer :modalBase=pdfPreviewer @close="closeModal(pdfPreviewer)" />
    <processing-popup :modalBase=waitModal />
</template>
