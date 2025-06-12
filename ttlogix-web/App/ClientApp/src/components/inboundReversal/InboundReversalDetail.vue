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

    // import widgets and modals
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import { InboundReversal } from '@/store/models/inboundReversal'
    import { InboundReversalItem } from '@/store/models/inboundReversalItem'
    import { InboundReversalItemFilters } from '@/store/models/inboundReversalItemFilters'
    import AddInboundReversalItems from './modals/AddInboundReversalItems.vue'

    // import common logic
    import * as sortingFn from '@/store/commonFunctions/sorting';
    import * as miscFn from '@/store/commonFunctions/miscellaneous';

    // init vue stuff
    const store = useStore();
    const route = useRoute();
    const { t } = useI18n({ useScope: 'global' });

    // local vars
    const jobNo = ref<string>(route.params.jobNo?.toString());
    const inJobNo = ref('');
    const loadingList = ref(false);
    const filters_IR_ITEMS = ref(new InboundReversalItemFilters());
    filters_IR_ITEMS.value.jobNo = jobNo.value;
    const getInboundReversalStatus = store.getters.getInboundReversalStatus;
    const sorting_IR_ITEMS = new Sorting();
    var inboundReversal = ref(new InboundReversal());

    // functions
    const canDelete = () => {
        return inboundReversal.value.status < 2;
    };
    const reload = async () => {
        inboundReversal.value = await store.dispatch(ActionTypes.IR_GET_DETAILS, { jobNo: jobNo.value });
        inJobNo.value = inboundReversal.value.inJobNo;
        reloadItems();
    };
    const reloadItems = async () => {
        loadingList.value = true;
        await store.dispatch(ActionTypes.IR_LOAD_ITEMS, { filters: filters_IR_ITEMS.value })
            .then((items) => {
                inboundReversal.value.items = items;
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
        await store.dispatch(ActionTypes.IR_COMPLETE, { jobNo: jobNo.value }).catch(() => { closeModal(waitModal.value) });
        reload();
        closeModal(waitModal.value);
    };
    const canComplete = () => {
        return inboundReversal.value.status < 2;
    };
    const cancel = async () => {
        confirmModal.value.fnMod.message = t('generic.cancelConfirm', { jobNo: jobNo.value });
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await cancelExecute() };
    };
    const cancelExecute = async () => {
        closeModal(confirmModal.value);
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.IR_CANCEL, { jobNo: jobNo.value })
            .then(() => {
                reload();
                reloadItems();
            })
            .catch(() => { closeModal(waitModal.value) });
        closeModal(waitModal.value);
    };
    const canCancel = () => {
        return inboundReversal.value.status < 2;
    };
    const canDeleteItem = () => {
        return inboundReversal.value.status < 2;
    };
    const deleteItem = (pid: string) => {
        confirmModal.value.fnMod.message = t('generic.deleteConfirm', { jobNo: pid });
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await deleteItemExecute(pid) };
    };
    const deleteItemExecute = async (pid: string) => {
        closeModal(confirmModal.value);
        loadingList.value = true;
        await store.dispatch(ActionTypes.IR_DELETE_ITEM, { jobNo: jobNo.value, pid: pid })
            .then(() => {
                inboundReversal.value.items = inboundReversal.value.items.filter((revi) => { return revi.pid != pid; });
                loadingList.value = false;
                reload();
                reloadItems();
            })
            .catch(() => loadingList.value = false);
    }

    const canSave = () => {
        return inboundReversal.value.refNo && inboundReversal.value.status < 2;
    };

    const save = async () => {
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.IR_UPDATE, { jobNo: jobNo.value, refNo: inboundReversal.value.refNo, reason: inboundReversal.value.reason }).catch(() => closeModal(waitModal.value));
        closeModal(waitModal.value)
    }

    const addItems = () => { addItemsModal.value.on = true };
    const addItemsExecute = async (pids: Array<string>) => {
        closeModal(addItemsModal.value);
        waitModal.value.on = true;
        waitModal.value.title = t('inboundReversal.operation.addingItems');
        await store.dispatch(ActionTypes.IR_ADD_INBOUND_REVERSAL_ITEMS, { jobNo: jobNo.value, pids: pids }).catch(() => { closeModal(waitModal.value) });
        closeModal(waitModal.value);
        reload();
        reloadItems();
    }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const confirmModal = ref(new Modal(600, t('generic.pleaseConfirm'), true));
    const addItemsModal = ref(new Modal(1000, t('inboundReversal.operation.addItems'), true));
    const waitModal = ref(new Modal(500, t('generic.processing'), false));

    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, sorting_IR_ITEMS); reloadItems(); };
    // Date
    const showDate = (date: string, showTime: boolean = false) => { return miscFn.showDate(date, showTime); }

    // table related stuff
    const columns_IR_ITEMS: Array<TableColumn> = [
        { alias: 'no', columnType: null, dropdown: null, sortable: false, f1: null, f2: null, resetTo: null },
        { alias: 'pid', columnType: 'string', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'productCode', columnType: 'string', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null },
        { alias: 'originalQty', columnType: 'range', dropdown: null, sortable: true, f1: null, f2: null, resetTo: null }
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
                    <h5 class="float-start">{{t('inboundReversal.details.header.title')}}</h5>
                </div>
                <div class="card-body">
                    <div class="row g-0">
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('inboundReversal.details.header.jobNo')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="inboundReversal.jobNo" disabled /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('inboundReversal.details.header.customerName')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="inboundReversal.customerName" disabled /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('inboundReversal.details.header.supplier')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="inboundReversal.supplierName" disabled /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('inboundReversal.details.header.whs')}}:</div>
                        <div class="col-md-1"><input class="form-control form-control-sm" :value="inboundReversal.whsCode" disabled /></div>
                    </div>
                    <div class="row my-3 g-0">
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('inboundReversal.details.header.inJobNo')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="inboundReversal.inJobNo" disabled /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('inboundReversal.details.header.refNo')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" v-model="inboundReversal.refNo" /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('inboundReversal.details.header.createdDate')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="showDate(inboundReversal.createdDate, true)" disabled /></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('inboundReversal.details.header.createdBy')}}:</div>
                        <div class="col-md-1"><input class="form-control form-control-sm" :value="inboundReversal.createdBy" disabled /></div>
                    </div>
                    <div class="row g-0">
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('inboundReversal.details.header.reason')}}:<br /><small>{{t('inboundReversal.details.header.maxChars')}}</small></div>
                        <div class="col-md-5"><textarea class="form-control form-control-sm" v-model="inboundReversal.reason"></textarea></div>
                        <div class="col-md-1 text-end pt-1 pe-1">{{t('inboundReversal.details.header.status')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="getInboundReversalStatus(inboundReversal.status)" disabled /></div>
                    </div>
                </div>
                <div class="card-footer">
                    <div class="row">
                        <div class="col-md-12 text-end">
                            <button class="btn btn-primary me-2 btn-sm" @click="complete" v-if="canComplete()"><i class="las la-check-circle"></i> {{ t('operation.general.complete') }}</button>
                            <button class="btn btn-danger me-2 btn-sm" @click="cancel" v-if="canCancel()"><i class="las la-ban"></i> {{ t('operation.general.cancel') }}</button>
                            <div class="d-inline-block" v-tooltip="canSave() ? null : 'Fill Ref No. first.'">
                                <button class="btn btn-primary me-2 btn-sm" @click="save" :disabled="!canSave()"><i class="las la-save"></i> {{ t('operation.general.save') }}</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row my-4 mx-1">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <h5>{{t('inboundReversal.details.list.title')}}</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-12">
                            <table class="table table-striped table-hover filtered-sorted-table" v-if="!loadingList && inboundReversal.items.length > 0">
                                <table-sort-filter-header :columns=columns_IR_ITEMS
                                                          :sorting=sorting_IR_ITEMS
                                                          t_core="inboundReversal.details.list"
                                                          @sort="sort"
                                                          :sortable=true
                                                          :filtreable=false />
                                <tbody>
                                    <tr v-for="revItem, i in inboundReversal.items" :key="i" class="table-row">
                                        <td>{{i+1}}</td>
                                        <td>{{revItem.pid}}</td>
                                        <td>{{revItem.productCode}}</td>
                                        <td>{{revItem.originalQty}}</td>
                                        <div class="table-buttons" v-if="canDeleteItem()">
                                            <button class="btn btn-danger me-2 btn-sm" @click="deleteItem(revItem.pid)"><i class="las la-trash"></i> {{ t('operation.general.delete') }}</button>
                                        </div>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="text-center py-4 main-action-icons-small" v-if="!loadingList && inboundReversal.items.length == 0">
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
                <div class="card-footer" v-if="canCancel()">
                    <button class="btn btn-primary me-2 btn-sm" @click="addItems"><i class="las la-plus-circle"></i> {{ t('inboundReversal.operation.addNew') }}</button>
                </div>
            </div>
        </div>
    </div>
    <add-inbound-reversal-items :modalBase=addItemsModal :inJobNo=inJobNo @close="closeModal(addItemsModal)" @addItems="addItemsExecute" />
    <confirm-popup :modalBase=confirmModal @close="closeModal(confirmModal)" />
    <processing-popup :modalBase=waitModal />
</template>