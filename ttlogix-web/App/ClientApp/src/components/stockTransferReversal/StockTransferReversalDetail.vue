<script lang="ts" setup>
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useRoute } from 'vue-router';
    import { useI18n } from 'vue-i18n';

    // import types
    import { TableColumn } from '@/store/commonModels/config';
    import { Sorting } from '@/store/commonModels/Sorting';
    import { Customer } from '@/store/models/customer';
    import { Modal } from '@/store/commonModels/modal';
    import { Report } from '@/store/commonModels/report';
    import { ReportFilter } from '@/store/commonModels/reportFilter';

    // import widgets and modals
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import { StockTransferReversal } from '@/store/models/stockTransferReversal'
    import { StockTransferReversalItem } from '@/store/models/stockTransferReversalItem'
    import { StockTransferReversalItemFilters } from '@/store/models/stockTransferReversalItemFilters'
    import AddStockTransferReversalItems from './modals/AddStockTransferReversalItems.vue'
    import InfoPopup from '@/widgets/InfoPopup.vue'
    import PDFPreviewer from '@/widgets/PDFPreviewer.vue';

    // import common logic
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    import { InvoiceBatch } from '../../store/models/invoiceBatch';
    
    // init vue stuff
    const store = useStore();
    const route = useRoute();
    const { t } = useI18n({ useScope: 'global' });

    // local vars
    const jobNo = ref<string>(route.params.jobNo?.toString());
    const stfJobNo = ref('');
    const loadingList = ref(false);
    const filters_STR_ITEMS = ref(new StockTransferReversalItemFilters());
    filters_STR_ITEMS.value.jobNo = jobNo.value;
    const getStockTransferReversalStatus = store.getters.getStockTransferReversalStatus;
    const sorting_STR_ITEMS = new Sorting();
    var stockTransferReversal = ref(new StockTransferReversal());

    // functions
    const canDelete = () => {
        return stockTransferReversal.value.status < 2;
    };
    const reload = async () => {
        stockTransferReversal.value = await store.dispatch(ActionTypes.STR_GET_DETAILS, { jobNo: jobNo.value });
        stfJobNo.value = stockTransferReversal.value.stfJobNo;
        reloadItems();
    };
    const reloadItems = async () => {
        loadingList.value = true;
        await store.dispatch(ActionTypes.STR_LOAD_ITEMS, { filters: filters_STR_ITEMS.value })
            .then((items) => {
                stockTransferReversal.value.items = items;
                loadingList.value = false;
            }).catch(() => loadingList.value = false);
    };
    const complete = async () => {
        confirmModal.value.fnMod.message = t('generic.completeConfirm', { jobNo: jobNo.value });
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await completeExecute() };
    };
    const completeExecute = async () => {
        closeModal(confirmModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.STR_COMPLETE, { jobNo: stockTransferReversal.value.jobNo })
                   .then(() => {
                       infoModal.value.on = true;
                       infoModal.value.fnMod.message = t('stockTransferReversal.operation.StfReversalCompletedSuccessInfo');
                   })
                   .catch(() => { closeModal(waitModal.value) });
        reload();
        closeModal(waitModal.value);
    };
    const canComplete = () => {
        return stockTransferReversal.value.status < 2;
    };
    const cancel = async () => {
        confirmModal.value.fnMod.message = t('generic.cancelConfirm', { jobNo: jobNo.value });
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await cancelExecute() };
    };
    const cancelExecute = async () => {
        closeModal(confirmModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.STR_CANCEL, { jobNo: jobNo.value })
            .then(() => {
                reload();
                reloadItems();
            })
            .catch(() => { closeModal(waitModal.value) });
        closeModal(waitModal.value);
    };
    const canCancel = () => {
        return stockTransferReversal.value.status < 2;
    };
    const canDeleteItem = () => {
        return stockTransferReversal.value.status < 2;
    };
    const deleteItem = (pid: string) => {
        confirmModal.value.fnMod.message = t('generic.deleteConfirm', { jobNo: jobNo.value });
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await deleteItemExecute(pid) };
    };
    const deleteItemExecute = async (pid: string) => {
        closeModal(confirmModal.value);
        loadingList.value = true;
        await store.dispatch(ActionTypes.STR_DELETE_ITEM, { jobNo: jobNo.value, pid: pid })
            .then(() => {
                stockTransferReversal.value.items = stockTransferReversal.value.items.filter((revi) => { return revi.pid != pid; });
                loadingList.value = false;
                reload();
                reloadItems();
            })
            .catch(() => loadingList.value = false);
    }

    const canSave = () => {
        return stockTransferReversal.value.refNo && stockTransferReversal.value.status < 2;
    };
    const save = async () => {
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.STR_UPDATE, { jobNo: jobNo.value, refNo: stockTransferReversal.value.refNo, reason: stockTransferReversal.value.reason }).catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value)
    }
    const addItems = () => { addItemsModal.value.on = true };
    const addItemsExecute = async (pids: Array<string>) => {
        closeModal(addItemsModal.value);
        waitModal.value.on = true;
        waitModal.value.title = t('stockTransferReversal.operation.addingItems');
        await store.dispatch(ActionTypes.STR_ADD_STOCK_TRANSFER_REVERSAL_ITEMS, { jobNo: jobNo.value, pids: pids }).catch(() => { closeModal(waitModal.value) });
        closeModal(waitModal.value);
        reload();
        reloadItems();
    }
    const canPrintReport = () => {
        return stockTransferReversal.value.status == 8;
    }
    const report = () => {
        const report: Report = new Report();
        report.alias = 'report/inventory/reversalstocktransfer';
        report.title = 'Reversal Stock Transfer Report';
        report.props = { whsCode: stockTransferReversal.value.whsCode, jobNo: jobNo.value }
        printReport(report);
    };
    const printReport = async (report: Report) => {
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.STR_REPORT, { report })
            .then((result) => {
                waitModal.value.on = false;
                pdfPreviewer.value.customProps = { file: result, name: ('RST_Report_' + jobNo.value) };
                pdfPreviewer.value.on = true;
            })
            .catch((err: Error) => {
                closeModal(waitModal.value);
            });
    };

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const confirmModal = ref(new Modal(600, t('generic.pleaseConfirm'), false));
    const addItemsModal = ref(new Modal(1000, t('stockTransferReversal.operation.addItems'), true));
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const infoModal = ref(new Modal(600, t('stockTransferReversal.operation.stockTransferReportReminder'), true));
    const pdfPreviewer = ref(new Modal('90%', 'Report', true));

    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, sorting_STR_ITEMS); reloadItems(); };
    // Date
    const showDate = (date: string, showTime: boolean = false) => { return miscFn.showDate(date, showTime); }

    // table related stuff
    const columns_STR_ITEMS: Array<TableColumn> = [
        { alias: 'no', columnType: null, dropdown: null, sortable: false, f1: null, f2: null, resetTo: null },
        { alias: 'pid', columnType: 'string', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'productCode', columnType: 'string', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'qty', columnType: 'range', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null }
    ];

    // init the screen
    reload();
    reloadItems();
</script>
<template>
    <div class="row mt-3 mx-1">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <h5 class="float-start">{{t('stockTransferReversal.details.header.title')}}</h5>
                </div>
                <div class="card-body">
                    <div class="row g-0">
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('stockTransferReversal.details.header.jobNo')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="stockTransferReversal.jobNo" disabled /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('stockTransferReversal.details.header.customerName')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="stockTransferReversal.customerName" disabled /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('stockTransferReversal.details.header.supplier')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="stockTransferReversal.supplierName" disabled /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('stockTransferReversal.details.header.whs')}}:</div>
                        <div class="col-md-1"><input class="form-control form-control-sm" :value="stockTransferReversal.whsCode" disabled /></div>
                    </div>
                    <div class="row my-3 g-0">
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('stockTransferReversal.details.header.stfJobNo')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="stockTransferReversal.stfJobNo" disabled /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('stockTransferReversal.details.header.refNo')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" v-model="stockTransferReversal.refNo" /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('stockTransferReversal.details.header.createdDate')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="showDate(stockTransferReversal.createdDate, true)" disabled /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('stockTransferReversal.details.header.createdBy')}}:</div>
                        <div class="col-md-1"><input class="form-control form-control-sm" :value="stockTransferReversal.createdBy" disabled /></div>
                    </div>
                    <div class="row g-0">
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('stockTransferReversal.details.header.reason')}}:<br /><small>{{t('stockTransferReversal.details.header.maxChars')}}</small></div>
                        <div class="col-md-5"><textarea class="form-control form-control-sm" v-model="stockTransferReversal.reason"></textarea></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('stockTransferReversal.details.header.status')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="getStockTransferReversalStatus(stockTransferReversal.status)" disabled /></div>
                    </div>
                </div>
                <div class="card-footer">
                    <div class="row">
                        <div class="col-md-12 text-end">
                            <button class="btn btn-primary me-2 btn-sm" @click="complete" v-if="canComplete()" :disabled="stockTransferReversal.items.length == 0"><i class="las la-check-circle"></i> Complete</button>
                            <button class="btn btn-danger me-2 btn-sm" @click="cancel" v-if="canCancel()"><i class="las la-ban"></i> Cancel</button>
                            <div class="d-inline-block" v-tooltip="canSave() ? null : 'Fill Ref No. first.'">
                                <button class="btn btn-primary me-2 btn-sm" @click="save" :disabled="!canSave()"><i class="las la-save"></i> Save</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row mt-4 mx-1">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header bold"><h5>Documentation</h5></div>
                <div class="card-body">
                    <button class="btn btn-sm btn-primary" :disabled="!canPrintReport()" @click="report()">
                        <i class="las la-print"></i> {{t('stockTransferReversal.operation.reportButton')}}
                    </button>
                </div>
            </div>
        </div>
    </div>

    <div class="row my-4 mx-1">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <h5>{{t('stockTransferReversal.details.list.title')}}</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-12">
                            <table class="table table-striped table-hover filtered-sorted-table" v-if="!loadingList && stockTransferReversal.items.length > 0">
                                <table-sort-filter-header :columns=columns_STR_ITEMS
                                                          :sorting=sorting_STR_ITEMS
                                                          t_core="stockTransferReversal.details.list"
                                                          @sort="sort"
                                                          :sortable=true
                                                          :filtreable=false />
                                <tbody>
                                    <tr v-for="revItem, i in stockTransferReversal.items" :key="i" class="table-row">
                                        <td>{{i+1}}</td>
                                        <td>{{revItem.pid}}</td>
                                        <td>{{revItem.productCode}}</td>
                                        <td>{{revItem.qty}}</td>
                                        <div class="table-buttons" v-if="canDeleteItem()">
                                            <button class="btn btn-danger me-2 btn-sm" @click="deleteItem(revItem.pid)"><i class="las la-trash"></i> Delete</button>
                                        </div>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="text-center py-4 main-action-icons-small" v-if="!loadingList && stockTransferReversal.items.length == 0">
                                <h5><i class="las la-ban"></i> {{ t('generic.noRecords') }}</h5>
                            </div>
                            <div v-if="loadingList" class="bars1-wrapper">
                                <div id="bars1">
                                    <span></span>
                                    <span></span>
                                    <span></span>
                                    <span></span>
                                    <span></span>
                                </div>
                                <h5>{{ t('generic.loading') }}</h5>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="card-footer" v-if="canCancel()">
                    <button class="btn btn-primary me-2 btn-sm" @click="addItems"><i class="las la-plus-circle"></i> Add new</button>
                </div>
            </div>
        </div>
    </div>
    <add-stock-transfer-reversal-items :modalBase=addItemsModal :stfJobNo=stfJobNo @close="closeModal(addItemsModal)" @addItems="addItemsExecute" />
    <confirm-popup :modalBase=confirmModal @close="closeModal(confirmModal)" />
    <processing-popup :modalBase=waitModal />
    <info-popup :modalBase="infoModal" @close="closeModal(infoModal)" />
    <p-d-f-previewer :modalBase=pdfPreviewer @close="closeModal(pdfPreviewer)" />
</template>