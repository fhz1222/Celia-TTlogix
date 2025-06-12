<script lang="ts" setup>
    import bus from '@/event_bus.js'
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref, onActivated, onDeactivated } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useRoute } from 'vue-router';
    import { useI18n } from 'vue-i18n';

    // import types
    import { TableColumn } from '@/store/commonModels/config';
    import { Sorting } from '@/store/commonModels/Sorting';
    import { StockTake } from '@/store/models/stockTake';
    import { StockTakeDetail } from '@/store/models/stockTakeDetail';
    import { Modal } from '@/store/commonModels/modal';
    import { Report } from '@/store/commonModels/report';
    
    // import widgets and modals
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import SendToStandByPopup from '@/components/stockTake/modals/SendToStandBy.vue';
    import PDFPreviewer from '@/widgets/PDFPreviewer.vue';

    // import common logic
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';
    
    // init vue stuff
    const store = useStore();
    const route = useRoute();
    const { t } = useI18n({ useScope: 'global' });

    // local vars
    const jobNo = ref<string>(route.params.jobNo?.toString());
    const loadingList = ref(false);
    const stockTakeJob = ref(new StockTake());
    const list_STLD_uploaded = ref(new Array<StockTakeDetail>());
    const list_STLD_pidAnotherLoc = ref(new Array<StockTakeDetail>());
    const list_STLD_pidInvalid = ref(new Array<StockTakeDetail>());
    const list_STLD_pidMissing = ref(new Array<StockTakeDetail>());
    const whsCode = store.state.whsCode;
    const sorting_STLD_uploaded = new Sorting();
    const sorting_STLD_pidAnotherLoc = new Sorting();
    const sorting_STLD_pidInvalid = new Sorting();
    const sorting_STLD_pidMissing = new Sorting();
    const printing = ref(false);
    const getStockTakeStatus = store.getters.getStockTakeStatus;
    const getDetailStatus = store.getters.getStorageDetailStatus;
    

    // functions
    const reloadItem = async () => {
        loadingList.value = true;
        await store.dispatch(ActionTypes.STL_LOAD_JOB, { jobNo: jobNo.value })
                   .then((job) => stockTakeJob.value = job).catch(() => loadingList.value = false);
        await reloadUploadedList();
        await reloadPidAnotherLoc();
        await reloadInvalidPid();
        await reloadMissingPid();
        
        loadingList.value = false;
    }
    const sendToStandByNegative = () => {
        sendToStandByModal.value.on = true;
        sendToStandByModal.value.customProps = jobNo.value;
    }
    const sendToStandByNegativeExecute = async (locationCode: string) => {
        closeModal(sendToStandByModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.STL_SEND_TO_STANDBY_NEGATIVE, { jobNo: stockTakeJob.value.jobNo, locationCode: locationCode, whsCode: whsCode })
                   .then(async () => {
                       await reloadUploadedList();
                       await reloadPidAnotherLoc();
                       await reloadInvalidPid();
                       await reloadMissingPid();
                       closeModal(waitModal.value);
                   })
                   .catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value);
    }
    const reloadUploadedList = async () => {
        await store.dispatch(ActionTypes.STL_LOAD_UPLOADED_LIST, { jobNo: jobNo.value, sorting: sorting_STLD_uploaded })
                   .then((uploadedList) => list_STLD_uploaded.value = uploadedList).catch(() => loadingList.value = false);
    }
    const reloadPidAnotherLoc = async () => {
        await store.dispatch(ActionTypes.STL_LOAD_PID_ANOTHER_LOC_LIST, { jobNo: jobNo.value, sorting: sorting_STLD_pidAnotherLoc })
                   .then((anotherLocList) => list_STLD_pidAnotherLoc.value = anotherLocList).catch(() => loadingList.value = false);
    }
    const reloadInvalidPid = async() => {
        await store.dispatch(ActionTypes.STL_LOAD_PID_INVALID_LIST, { jobNo: jobNo.value, sorting: sorting_STLD_pidInvalid })
                   .then((invalidPidList) => list_STLD_pidInvalid.value = invalidPidList).catch(() => loadingList.value = false);
    }
    const reloadMissingPid = async () => {
        await store.dispatch(ActionTypes.STL_LOAD_PID_MISSING_LIST, { jobNo: jobNo.value, sorting: sorting_STLD_pidMissing })
                   .then((missingPidList) => list_STLD_pidMissing.value = missingPidList).catch(() => loadingList.value = false);
    }
    const canCancel = () => {
        return stockTakeJob.value.status < 2;
    }
    const canComplete = () => {
        return stockTakeJob.value.status < 2;
    }
    const canSave = () => {
        return stockTakeJob.value.status < 2;
    }
    const cancel = async () => {
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.STL_CANCEL_JOB, { jobNo: stockTakeJob.value.jobNo }).catch(() => closeModal(waitModal.value));
        await reloadItem();
        closeModal(waitModal.value);
    }
    const complete = () => {
        confirmModal.value.fnMod.message = t('generic.completeConfirm', { jobNo: jobNo.value });
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await completeExecute() };
    }
    const completeExecute = async () => {
        closeModal(confirmModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.STL_COMPLETE_JOB, { jobNo: stockTakeJob.value.jobNo }).catch(() => closeModal(waitModal.value));
        await reloadItem();
        closeModal(waitModal.value);
    }
    const save = async () => {
        waitModal.value.on = true;
        stockTakeJob.value.refNo = stockTakeJob.value.refNo ?? '';
        await store.dispatch(ActionTypes.STL_UPDATE_JOB, { stockTake: stockTakeJob.value }).catch(() => { closeModal(waitModal.value) });
        closeModal(waitModal.value);
    };
    const sendToStandBy = (pid: string) => {
        sendToStandByModal.value.on = true;
    };
    const isSaveVisible = computed(() => stockTakeJob.value?.status != 10 && stockTakeJob.value?.status != 2);
    const printStockTakeReport = () => {
        const report: Report = new Report();
        report.alias = 'report/inventory/stocktakereport';
        report.title = 'Stock Take Report';
        report.props = { whsCode: whsCode, jobNo: jobNo.value }
        printReport(report);
    };
    const printReport = async(report: Report) => { 
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
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const confirmModal = ref(new Modal(600, t('generic.pleaseConfirm'),false));
    const waitModal = ref(new Modal(500, t('generic.processing'), false));
    const sendToStandByModal = ref(new Modal(600, t('stockTake.detail.modals.sendToStandBy'), true));
    const pdfPreviewer = ref(new Modal('90%', t('generic.report'), true));
 
    // Sorting
    const sortUploaded = (by: string) => { sortingFn.sort(by, sorting_STLD_uploaded); reloadUploadedList(); };
    const sortAnotherLoc = (by: string) => { sortingFn.sort(by, sorting_STLD_pidAnotherLoc); reloadPidAnotherLoc(); };
    const sortPidInvalid = (by: string) => { sortingFn.sort(by, sorting_STLD_pidInvalid); reloadInvalidPid(); };
    const sortPidMissing = (by: string) => { sortingFn.sort(by, sorting_STLD_pidMissing); reloadMissingPid(); };
    // Date
    const showDate = (date:string, showTime: boolean = false) => { return miscFn.showDate(date, showTime); }

    // table related stuff
    const columns_STLD_uploaded: Array<TableColumn> = [
        { alias: 'pid',         columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'customerCode',columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'productCode', columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'supplierId',  columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'qty',         columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'inJobNo',     columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'inboundDate', columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'whsCode',     columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'locationCode',columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'status',      columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null }
    ];
    const columns_STLD_pidAnotherLoc: Array<TableColumn> = [
        { alias: 'pid',         columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'customerCode',columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'productCode', columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'supplierId',  columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'inJobNo',     columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'status',      columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'whsCode',     columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'locationCode',columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null }
    ];
    const columns_STLD_pidInvalid: Array<TableColumn> = [
        { alias: 'pid',         columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'customerCode',columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'productCode', columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'supplierId',  columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'whsCode',     columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'locationCode',columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'status',      columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'remark',      columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null }
    ];
    const columns_STLD_pidMissing: Array<TableColumn> = [
        { alias: 'pid',         columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'customerCode',columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'productCode', columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'supplierId',  columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'whsCode',     columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'locationCode',columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'status',      columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'remark',      columnType: null, dropdown: null, sortable: true, f1: null, f2: null, resetTo: null }
    ];

    // init the screen
    onActivated(() => {
        reloadItem();
        bus.on("refresh", () => reloadItem())
    });
    onDeactivated(() => bus.off("refresh", () => reloadItem()))
</script>
<template>
    <div class="row mt-3 mx-1">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <h5 class="float-start">{{t('stockTake.detail.header.title')}}</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-1 text-end pt-1 no-right-padding">{{t('stockTake.mainGrid.jobNo')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="stockTakeJob.jobNo" disabled /></div>
                        <div class="col-md-1 text-end pt-1 no-right-padding">{{t('stockTake.detail.header.location')}}:</div>
                        <div class="col-md-1"><input class="form-control form-control-sm" :value="stockTakeJob.locationCode" disabled /></div>
                        <div class="col-md-1 text-end pt-1 no-right-padding">{{t('stockTake.mainGrid.whsCode')}}:</div>
                        <div class="col-md-1"><input class="form-control form-control-sm" :value="stockTakeJob.whsCode" disabled /></div>
                        <div class="col-md-1 text-end pt-1 no-right-padding">{{t('stockTake.mainGrid.status')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="getStockTakeStatus(stockTakeJob.status)" disabled /></div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-1 text-end pt-1 no-right-padding">{{t('stockTake.mainGrid.refNo')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" v-model="stockTakeJob.refNo" :disabled="!canCancel()" /></div>
                        <div class="col-md-1 text-end pt-1 no-right-padding">{{t('stockTake.mainGrid.createdDate')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="showDate(stockTakeJob.createdDate)" disabled /></div>
                        <div class="col-md-2 text-end pt-1 no-right-padding">{{t('stockTake.detail.header.completedDate')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="showDate(stockTakeJob.completedDate)" disabled /></div>
                    </div>
                    <div class="row mt-2">
                        <div class="col-md-1 text-end pt-1 no-right-padding">{{t('stockTake.detail.header.remark')}}</div>
                        <div class="col-md-7"><textarea class="form-control form-control-sm" v-model="stockTakeJob.remark" :disabled="!canCancel()"></textarea></div>
                    </div>
                </div>
                <div class="card-footer">
                    <div class="row">
                        <div class="col-md-12 text-end">
                            <button class="btn btn-primary btn-sm" @click="printStockTakeReport"><i class="las la-print"></i>{{t('stockTake.detail.tabs.printStockTakeReport')}}</button>
                            <button class="btn btn-primary ms-2 btn-sm" @click="complete" v-if="canComplete()"><i class="las la-check-circle"></i> {{t('operation.general.complete')}}</button>
                            <button class="btn btn-danger ms-2 btn-sm" @click="cancel" v-if="canCancel()"><i class="las la-check-circle"></i> {{t('operation.general.cancel')}}</button>
                            <div class="d-inline-block" v-tooltip="canSave() ? null : 'Fill Ref No. first.'">
                                <button class="btn btn-primary ms-2 btn-sm" @click="save" :disabled="!canSave()" v-if="isSaveVisible"><i class="las la-save"></i> {{t('operation.general.save')}}</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row my-4 mx-1">
        <div class="col-md-12">
            <nav>
                <div class="nav nav-tabs" id="nav-tab" role="tablist">
                    <button class="nav-link active" id="uploaded-items-tab" type="button" role="tab"
                            data-bs-toggle="tab" data-bs-target="#nav-uploaded-items"
                            aria-controls="nav-uploaded-items" aria-selected="true">
                        {{ $t('stockTake.detail.tabs.uploadedList') }}
                    </button>
                    <button class="nav-link stl-warning-tab" id="extra-pid-another-loc-tab" data-bs-toggle="tab"
                            data-bs-target="#nav-extra-pid-another-loc" type="button" role="tab"
                            aria-controls="nav-extra-pid-another-loc" aria-selected="false">
                        {{ $t('stockTake.detail.tabs.anotherLocPid') }}
                    </button>
                    <button class="nav-link stl-danger-tab" id="extra-pid-invalid-tab" data-bs-toggle="tab"
                            data-bs-target="#nav-extra-pid-invalid" type="button" role="tab"
                            aria-controls="nav-extra-pid-invalid" aria-selected="false">
                        {{ $t('stockTake.detail.tabs.invalidPid') }}
                    </button>
                    <button class="nav-link stl-danger-tab" id="missing-pid-tab" data-bs-toggle="tab"
                            data-bs-target="#nav-missing-pid" type="button" role="tab"
                            aria-controls="nav-missing-pid" aria-selected="false">
                        {{ $t('stockTake.detail.tabs.missingPid') }}
                    </button>
                </div>
            </nav>
            <div class="tab-content" id="nav-tabContent">
                <!-- Uploaded Items Tab -->
                <div class="tab-pane fade show active" id="nav-uploaded-items" role="tabpanel" aria-labelledby="uploaded-items-tab">
                    <div class="row">
                        <div class="col-md-12">
                            <table class="table table-striped table-hover filtered-sorted-table" v-if="!loadingList && list_STLD_uploaded.length > 0">
                                <table-sort-filter-header :columns=columns_STLD_uploaded
                                                          :sorting=sorting_STLD_uploaded
                                                          t_core="stockTake.detail.grids"
                                                          @sort="sortUploaded"
                                                          :sortable=true
                                                          :filtreable=false />
                                <tbody>
                                    <tr v-for="stlu, i in list_STLD_uploaded" :key="i" class="table-row">
                                        <td>{{stlu.pid}}</td>
                                        <td>{{stlu.customerCode}}</td>
                                        <td>{{stlu.productCode}}</td>
                                        <td>{{stlu.supplierID}}</td>
                                        <td>{{stlu.qty}}</td>
                                        <td>{{stlu.inJobNo}}</td>
                                        <td>{{stlu.inboundDate}}</td>
                                        <td>{{stlu.whsCode}}</td>
                                        <td>{{stlu.locationCode}}</td>
                                        <td>{{getDetailStatus(stlu.status)}}</td>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="text-center py-4 main-action-icons-small" v-if="!loadingList && list_STLD_uploaded.length == 0">
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

                <!-- Extra PID Another Location Tab -->
                <div class="tab-pane fade show stl-warning-tab" id="nav-extra-pid-another-loc" role="tabpanel" aria-labelledby="extra-pid-another-loc-tab">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="pt-1 ps-2 extra-header-info">{{ $t('stockTake.detail.tabs.existInAnotherLoc') }}</div>
                            <table class="table table-striped table-hover filtered-sorted-table" v-if="!loadingList && list_STLD_pidAnotherLoc.length > 0">
                                <table-sort-filter-header :columns=columns_STLD_pidAnotherLoc
                                                          :sorting=sorting_STLD_pidAnotherLoc
                                                          t_core="stockTake.detail.grids"
                                                          @sort="sortAnotherLoc"
                                                          :sortable=true
                                                          :filtreable=false />
                                <tbody>
                                    <tr v-for="stlal, i in list_STLD_pidAnotherLoc" :key="i" class="table-row">
                                        <td>{{stlal.pid}}</td>
                                        <td>{{stlal.customerCode}}</td>
                                        <td>{{stlal.productCode}}</td>
                                        <td>{{stlal.supplierID}}</td>
                                        <td>{{stlal.inJobNo}}</td>
                                        <td>{{getDetailStatus(stlal.status)}}</td>
                                        <td>{{stlal.whsCode}}</td>
                                        <td>{{stlal.locationCode}}</td>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="text-center py-4 main-action-icons-small" v-if="!loadingList && list_STLD_pidAnotherLoc.length == 0">
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

                <!-- Extra PID Invalid PID Tab -->
                <div class="tab-pane fade show stl-danger-tab" id="nav-extra-pid-invalid" role="tabpanel" aria-labelledby="extra-pid-invalid-tab">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="pt-1 ps-2 extra-header-info">{{ $t('stockTake.detail.tabs.invalidPidText') }}</div>
                            <table class="table table-striped table-hover filtered-sorted-table" v-if="!loadingList && list_STLD_pidInvalid.length > 0">
                                <table-sort-filter-header :columns=columns_STLD_pidInvalid
                                                          :sorting=sorting_STLD_pidInvalid
                                                          t_core="stockTake.detail.grids"
                                                          @sort="sortPidInvalid"
                                                          :sortable=true
                                                          :filtreable=false />
                                <tbody>
                                    <tr v-for="stli, i in list_STLD_pidInvalid" :key="i" class="table-row">
                                        <td>{{stli.pid}}</td>
                                        <td>{{stli.customerCode}}</td>
                                        <td>{{stli.productCode}}</td>
                                        <td>{{stli.supplierID}}</td>
                                        <td>{{stli.whsCode}}</td>
                                        <td>{{stli.locationCode}}</td>
                                        <td>{{getDetailStatus(stli.status)}}</td>
                                        <td>{{stli.remark}}</td>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="text-center py-4 main-action-icons-small" v-if="!loadingList && list_STLD_pidInvalid.length == 0">
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

                <!-- Missing PID Tab -->
                <div class="tab-pane fade show stl-danger-tab" id="nav-missing-pid" role="tabpanel" aria-labelledby="missing-pid-tab">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="pt-1 ps-2 extra-header-info">{{ $t('stockTake.detail.tabs.missingPidText') }}</div>
                            <div class="text-start ps-2 button-div" v-if="list_STLD_pidMissing.length > 20">
                                <button class="btn btn-sm btn-primary" @click="sendToStandByNegative()"><i class="las la-exclamation-triangle"></i> {{ $t('stockTake.detail.tabs.sendToStdbyByNeg') }}</button>
                            </div>
                            <table class="table table-striped table-hover filtered-sorted-table" v-if="!loadingList && list_STLD_pidMissing.length > 0">
                                <table-sort-filter-header :columns=columns_STLD_pidMissing
                                                          :sorting=sorting_STLD_pidMissing
                                                          t_core="stockTake.detail.grids"
                                                          @sort="sortPidMissing"
                                                          :sortable=true
                                                          :filtreable=false />
                                <tbody>
                                    <tr v-for="stlm, i in list_STLD_pidMissing" :key="i" class="table-row">
                                        <td>{{stlm.pid}}</td>
                                        <td>{{stlm.customerCode}}</td>
                                        <td>{{stlm.productCode}}</td>
                                        <td>{{stlm.supplierID}}</td>
                                        <td>{{stlm.whsCode}}</td>
                                        <td>{{stlm.locationCode}}</td>
                                        <td>{{getDetailStatus(stlm.status)}}</td>
                                        <td>{{stlm.remark}}</td>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="table-buttons text-end mt-2" v-if="list_STLD_pidMissing.length > 0">
                                <button class="btn btn-sm btn-primary" @click="sendToStandByNegative()"><i class="las la-exclamation-triangle"></i> {{ $t('stockTake.detail.tabs.sendToStdbyByNeg') }}</button>
                            </div>
                            <div class="text-center py-4 main-action-icons-small" v-if="!loadingList && list_STLD_pidMissing.length == 0">
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
            </div>
        </div>
    </div>

    <confirm-popup :modalBase=confirmModal @close="closeModal(confirmModal)" />
    <processing-popup :modalBase=waitModal />
    <send-to-stand-by-popup :modalBase="sendToStandByModal" @close="closeModal(sendToStandByModal)" @proceed="sendToStandByNegativeExecute" />
    <p-d-f-previewer :modalBase=pdfPreviewer @close="closeModal(pdfPreviewer)" />
</template>