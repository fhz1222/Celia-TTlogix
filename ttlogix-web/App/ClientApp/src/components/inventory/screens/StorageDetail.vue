<script lang="ts" setup>

    import bus from '@/event_bus.js';
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref, onMounted, onActivated, onDeactivated } from 'vue';

    // import types
    import { ActionTypes } from '@/store/action-types';
    import { TableColumn } from '@/store/commonModels/config';
    import { StorageDetailFilters } from '@/store/models/storageDetailFilters';
    import { PaginationData } from '@/store/commonModels/paginationData';
    import { MenuItem } from '@/store/commonModels/config';

    // import widgets and modals
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import Paging from '../../../widgets/Paging.vue';
    import PrintLabel from '@/components/print/PrintLabelPID.vue';
    import SectionMenu from '@/widgets/SectionMenu.vue';

    // import common logic
    import * as pagingFn from '@/store/commonFunctions/paging';
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as filteringFn from '@/store/commonFunctions/filtering';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    import { StorageDetail } from '@/store/models/storageDetail';

    // init vue stuff
    const store = useStore();

    // local vars
    const filters_SD = ref(new StorageDetailFilters());
    filters_SD.value.whsCode = store.state.whsCode;
    const getOwnership = store.getters.getOwnership;
    const getStorageDetailStatus = store.getters.getStorageDetailStatus;
    const getBondedStatus = store.getters.getBondedStatus;
    const list_SD = computed(() => store.state.storageDetailList);
    list_SD.value.items = [];
    const loadingList = ref(false);
    const totalScreenQty = ref(0);
    const selectionCount = ref(-1);
    const iLogColors = {
        inILog: '#afffad',
        requested: '#fff2ad',
    }
    const pidsRequestedFromILog = computed(() => store.state.pidsRequestedFromILog);
    const isiLogEnabled = computed(() => store.state.isILogEnabled)

    // Print - old modal
    const modal: any = ref(null);

    // functions
    const mandatoryFiltersSet = () => {
        return filters_SD.value.pid || filters_SD.value.externalPID || filters_SD.value.productCode || filters_SD.value.location;
    }
    const reloadList = async () => {
        if (isiLogEnabled.value) {
            await store.dispatch(ActionTypes.INV_RELOAD_ILOG_PIDS);
        }
        if (mandatoryFiltersSet()) {
            loadingList.value = true;
            await store.dispatch(ActionTypes.INV_SD_LOAD_LIST, { filters: filters_SD.value });
            //totalScreenQty.value = 0;
            /*list_SD.value.items.forEach((el) => {
                totalScreenQty.value += el.qty;
            });*/
            loadingList.value = false;
        } else {
            list_SD.value.items = [];
            list_SD.value.pagination = new PaginationData();
        }
    }
    const printPIDLabel = async () => {
        if (canPrintLabel()) {
            var pids = areSelected().map(o => o.pid);
            modal.value = { type: 'printLabel', allPids: pids, selectedPids: pids, labelType: 'QRPRINT_STORAGELABEL' };
            modal.value.type = 'printLabel';
        }
    };
    const areSelected = () => {
        var result = list_SD.value.items.filter(obj => {
            return obj.selected === true;
        });
        return result;
    }
    const canPrintLabel = () => {
        return list_SD.value.items.length > 0 && areSelected().length > 0;
    }
    const menuAction = (menuFunction: Function) => { menuFunction() };


    const requestFromILog = async () => {
        await store.dispatch(ActionTypes.REQUEST_FROM_ILOG, { pids: areSelected().map(p => p.pid) });
        await store.dispatch(ActionTypes.INV_RELOAD_ILOG_PIDS);
        await reloadList();
    }
    const getRowColor = (sd: StorageDetail) => {
        if (!isiLogEnabled.value) return 'inherit';
        if (pidsRequestedFromILog.value.includes(sd.pid)) return iLogColors.requested;
        if (sd.iLogLocationCategory == "iLogStorage" && sd.status != 6 && sd.qty > 0) return iLogColors.inILog;
        if (sd.selected) return '#4b8ede78';
        return 'inherit';
    }

    //onMounted(() => {
    //    // scroll table sideways
    //    miscFn.scrollSideways('mainTable');
    //});
    const toggleSelected = (e: MouseEvent, sd: StorageDetail): void => {
        const objIndex = list_SD.value.items.findIndex((obj => obj.pid == sd.pid));

        if (selectionCount.value == -1) {
            toggleState(objIndex); selectionCount.value = objIndex;
        } else {
            if (e.shiftKey) {
                const startingIndex = objIndex > selectionCount.value ? selectionCount.value : objIndex;
                const diff = Math.abs(objIndex - selectionCount.value);
                for (var i = startingIndex; i < (startingIndex + diff); i++) {
                    list_SD.value.items[i].selected = i == startingIndex ? list_SD.value.items[i].selected : !list_SD.value.items[i].selected;
                }
                selectionCount.value = -1;
            } else {
                toggleState(objIndex); selectionCount.value = objIndex;
            }
        }
    };

    const toggleState = (index: number) => {
        list_SD.value.items[index].selected = !list_SD.value.items[index].selected;
    }

    // Date
    const showDate = (date: string) => { return miscFn.showDate(date); }
    // Filtering
    const applyFilters = (column: string, value: any, load: boolean) => { filteringFn.applyFilters(column, value, filters_SD.value); if (load) reloadList(); };
    // Paging
    const goToPage = (page: number) => { pagingFn.goToPage(page, filters_SD.value.pagination); reloadList(); };
    const setItemsPerPage = (itemsPerPage: number) => { pagingFn.setItemsPerPage(itemsPerPage, filters_SD.value.pagination); reloadList(); };
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, filters_SD.value.sorting); reloadList(); };

    // Initially launched functions
    await store.dispatch(ActionTypes.LOAD_DICT);
    const ownershipDict = store.state.config.dictionaries.ownership;
    const storageDetailStatus = store.state.config.dictionaries.storageDetailStatus;
    const bondedStatus = store.state.config.dictionaries.bondedStatus;

    // section Menu
    const menuItems_REL = computed<Array<MenuItem>>(() =>
        (isiLogEnabled.value ?
            [{ state: null, stateParams: null, title: 'request from iLog', function: requestFromILog, icon: 'la-dolly' }] : []).concat(
                [
                    { state: null, stateParams: null, title: 'refresh', function: reloadList, icon: 'la-redo-alt' },
                    { state: null, stateParams: null, title: 'print labels', function: printPIDLabel, icon: 'la-print' },
                ]));

    const setTableHeight = () => {
        const height = window.innerHeight;
        return (height - 220) + 'px';
    }

    // Table related stuff
    const columns_SD: Array<TableColumn> = [
        { alias: '', columnType: null, dropdown: null, sortable: false, f1: null, f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'no', columnType: null, dropdown: null, sortable: false, f1: null, f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'pid', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'externalPID', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'customerCode', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'inboundJobNo', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'lineItem', columnType: 'range', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'sequenceNo', columnType: 'range', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'productCode', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'location', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'status', columnType: 'dropdown', dropdown: storageDetailStatus, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'supplierId', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'ownership', columnType: 'dropdown', dropdown: ownershipDict, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'inboundDate', columnType: 'date', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'qty', columnType: 'range', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'allocatedQty', columnType: 'range', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'outboundJobNo', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'controlCode1', columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'controlCode2', columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'parentId', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'bondedStatus', columnType: 'dropdown', dropdown: bondedStatus, sortable: true, f1: null, f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'refNo', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] },
        { alias: 'commInvNo', columnType: 'string', dropdown: null, sortable: true, f1: '', f2: null, resetTo: null, specialClasses: ['non-arrow-operated'] }];


    onActivated(async () => {
        await store.dispatch(ActionTypes.INV_RELOAD_ILOG_ENABLED);
        bus.on("refresh", reloadList)
    });
    onDeactivated(() => bus.off("refresh", reloadList));
</script>
<template>

    <div class="row p-2">
        <div class="col-md-12">
            <h5 class="text-center mt-2 blue-text weight-normal">
                Use the <strong>%</strong> symbol in filter fields to search for a substring instead of exact value.
            </h5>
            <div class="table-responsive" style="overflow: auto" v-bind:style="{'height': setTableHeight()}">
                <table class="table table-striped table-hover filtered-sorted-table">
                    <table-sort-filter-header :columns=columns_SD
                                              :sorting=filters_SD.sorting
                                              t_core="inventory.storageDetailGrid"
                                              @sort="sort"
                                              @apply-filters="applyFilters"
                                              :sortable=true
                                              :filtreable=true />
                    <tbody v-if="!loadingList && list_SD.items.length > 0">
                        <tr v-for="sd, i in list_SD.items" :key="i" @click="toggleSelected($event, sd)">
                            <td :style="{'background-color':  getRowColor(sd)}"><input type="checkbox" v-model="sd.selected" class="form-check-input"></td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{(((filters_SD.pagination.pageNumber - 1) * filters_SD.pagination.itemsPerPage) + 1) + i}}</td>
                            <td v-text="sd.pid" :style="{'background-color':  getRowColor(sd)}"></td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.externalPID}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.product.customerSupplier.customerCode}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.inboundJobNo}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.lineItem}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}" v-text="sd.sequenceNo"></td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.product.code}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.location}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{getStorageDetailStatus(sd.status)}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.product.customerSupplier.supplierId}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{getOwnership(sd.ownership)}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{showDate(sd.inboundDate)}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.qty}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.allocatedQty}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.outboundJobNo}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.controlCode1}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.controlCode2}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.parentId}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{getBondedStatus(sd.bondedStatus)}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.refNo}}</td>
                            <td :style="{'background-color':  getRowColor(sd)}">{{sd.commInvNo}}</td>
                        </tr>
                    </tbody>
                </table>
                <h5 class="text-center pt-5" v-if="!mandatoryFiltersSet() && !loadingList">
                    <i class="las la-exclamation-triangle"></i> Enter at least the PID, ExternalPID, Part No or the Location
                </h5>
                <h5 class="text-center pt-5" v-if="mandatoryFiltersSet() && !loadingList && list_SD.items.length == 0">
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
            <div class="row ps-2" v-if="list_SD.items.length > 0">
                <div class="col-md-2">Total No of PIDs: <strong>{{ list_SD.pagination.totalCount }}</strong></div>
                <div class="col-md-2">Total Qty: <strong>{{ list_SD.totalQty }}</strong></div>
                <div class="col-md-2" v-if="isiLogEnabled">
                    <div class="row mb-1"><div class="me-2 float-start pallet-ilog-status-indicator" :style="{'background-color': iLogColors.requested}"></div>requested</div>
                    <div class="row"><div class="me-2 float-start pallet-ilog-status-indicator" :style="{'background-color': iLogColors.inILog}"></div>in iLog</div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <paging :pagingData="list_SD.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" v-if="!loadingList" />
                </div>
            </div>
        </div>
        <div class="col-md-3 text-end">
            <section-menu :menu-items="menuItems_REL" t_orig="inventory.operation.storageDetailGrid" top="false" background=0 @action="menuAction" />
        </div>
    </div>

    <!--<div class="row ps-2" v-if="list_SD.items.length > 0">
        <div class="col-md-2">Total No of PIDs: <strong>{{ list_SD.pagination.totalCount }}</strong></div>
        <div class="col-md-2">Total Qty: <strong>{{ list_SD.totalQty }}</strong></div>
    </div>

    <div class="row">
        <div class="col-md-9">
            <paging :pagingData="list_SD.pagination" :padding=true @go-to-page="goToPage" @set-items-per-page="setItemsPerPage" v-if="!loadingList" />
        </div>
        <div class="col-md-3 text-end">

            <button class="btn btn-primary me-2 btn-sm" :disabled="!canPrintLabel()" @click="printPIDLabel"><i class="las la-print"></i>  Print Labels</button>
        </div>
    </div>-->

    <print-label v-if="modal != null && modal.type=='printLabel'" :all="modal.allPids" :selected="modal.selectedPids" :labelType="modal.labelType" :selectedOnly=true
                 @done="modal=null" @close="modal=null" />
</template>