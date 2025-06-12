<script lang="ts" setup>
    // import vue stuff
    import { useStore } from '@/store';
    import { computed, ref } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useRoute } from 'vue-router';
    import { useI18n } from 'vue-i18n';

    // import types
    import { TableColumn } from '@/store/commonModels/config';
    import { AdjustmentItem } from '@/store/models/adjustmentItem';
    import { Sorting } from '@/store/commonModels/Sorting';
    import { Adjustment } from '@/store/models/adjustment';
    import { Customer } from '@/store/models/customer';
    import { Modal } from '@/store/commonModels/modal';
    
    // import widgets and modals
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import AddAdjustmentItem from '../modals/AddAdjustmentItem.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue';
    import ProcessingPopup from '@/widgets/ProcessingPopup.vue';
    import PrintLabel from '@/components/print/PrintLabelPID.vue';

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
    const addingNew = ref(false);
    const printing = ref(false);
    const deleting = ref(false);
    const itemToModify = ref<AdjustmentItem | null>(null);
    const sorting_ADJ_ITEMS = new Sorting();
    const whsCode = store.state.whsCode;
    const customers = computed(() => store.state.customers);
    const getAdjustmentStatus = store.getters.getAdjustmentStatus;
    const getAdjustmentJobType = store.getters.getAdjustmentJobType;
    var adjustment = ref(new Adjustment());
    const refNo = ref('');

    // functions
    const setAdjustment = (jn:string | null = null) => {
        const adjResult = store.state.adjustmentList?.items.find(el => { return el.jobNo == (jn ? jn : jobNo.value) });
        adjustment.value = adjResult ? adjResult : new Adjustment();
    }
    const reloadItem = async () => {
        loadingList.value = true;
        adjustment.value = await store.dispatch(ActionTypes.INV_ADJ_GET_DETAILS, {jobNo: jobNo.value});
        refNo.value = adjustment.value.referenceNo;
        await store.dispatch(ActionTypes.INV_LOAD_ADJ_ITEMS, {jobNo: jobNo.value, filters: sorting_ADJ_ITEMS})
                   .then((items) => {
                        adjustment.value.items = items;
                        loadingList.value = false;
                   });
    }
    const changeCustomer = (customer:Customer) => {
        adjustment.value.customerCode = customer?.code;
    };
    const complete = async () => { 
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_COMPLETE_ADJ, { jobNo: adjustment.value.jobNo }).catch(() => { closeModal(waitModal.value) });
        await reloadItem();
        closeModal(waitModal.value);
    };
    const canComplete = () => {
        return adjustment.value.status == 1;
    };
    const cancel = async () => { 
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_CANCEL_ADJ, { jobNo: adjustment.value.jobNo }).catch(() => { closeModal(waitModal.value) });
        await reloadItem();
        closeModal(waitModal.value);
    };
    const canCancel = () => {
        return adjustment.value.status < 2;
    };
    const save = async () => {
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_UPDATE_ADJ, {jobNo: adjustment.value.jobNo, referenceNo: adjustment.value.referenceNo, reason: adjustment.value.reason})
                   .then((updatedAdj) => { 
                       adjustment.value = updatedAdj;
                       refNo.value = adjustment.value.referenceNo;
                       closeModal(waitModal.value);
                   }).catch(() => { closeModal(waitModal.value) });
        
    };
    const isSaveVisible = computed(() => adjustment.value?.status != 10 && adjustment.value?.status != 2 )
    const canSave = () => {
        return typeof adjustment.value.referenceNo != 'undefined' && adjustment.value.referenceNo;
    };
    const canAddNew = () => {
        return refNo.value ? true : false;
    }
    const printPIDLabel = async () => { 
        var pids = adjustment.value.items.map((item) => { return item['pid'] });
        modal.value = { type: 'printLabel', allPids: pids, selectedPids: [], labelType: 'QRPRINT_STORAGELABEL' };
        modal.value.type = 'printLabel';
     };
    const canPrintLabel = () => {  
        return adjustment.value && adjustment.value.items && adjustment.value.items.length > 0;
    }
    const addNewItem = () => { itemToModify.value = null; addItemModal.value.customProps = null; addItemModal.value.on = true; };
    const modifyItem = (item:AdjustmentItem) => { itemToModify.value = item; addItemModal.value.on = true; };
    const deleteItem = async (item:AdjustmentItem) => { 
        confirmModal.value.fnMod.message = 'Do you really want to delete ' + item.pid + '?';
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await deleteItemExecute(item) };
    };
    const deleteItemExecute = async (item: AdjustmentItem): Promise<void> => { 
        confirmModal.value.on = false;
        waitModal.value.on = true;
        waitModal.value.title = 'Deleting Adjustment Item...';
       // deleting.value = true;
        await store.dispatch(ActionTypes.INV_DEL_ITEM, { jobNo: item.jobNo, lineItem: item.lineItem }).catch(() => { closeModal(waitModal.value) });
        closeModal(waitModal.value);
       // deleting.value = false;
    };

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const addItemModal = ref(new Modal(600,'Add adjustment item',true));
    const confirmModal = ref(new Modal(600,'Please confirm',false));
    const waitModal = ref(new Modal(500, 'Processing', false));
    // Print - old modal
    const modal: any = ref(null);
    const closeAfterAddItem = async () => {
        closeModal(addItemModal.value);
        await reloadItem();
    }
 
    // Sorting
    const sort = (by: string) => { sortingFn.sort(by, sorting_ADJ_ITEMS); reloadItem(); };
    // Date
    const showDate = (date:string, showTime: boolean = false) => { return miscFn.showDate(date, showTime); }

    // table related stuff
    const columns_ADJ_ITEMS:Array<TableColumn> = [
        {alias:'no',         columnType:null, dropdown:null, sortable: false,f1:null, f2:null, resetTo: null },
        {alias:'lineItem',   columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        {alias:'productCode',columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        {alias:'supplierId', columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        {alias:'palletId',   columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        {alias:'initialQty', columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        {alias:'newQty',     columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        {alias:'remarks',    columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null }
    ];

    // init the screen
    await store.dispatch(ActionTypes.INV_LOAD_CUSTOMERS);
    reloadItem();
</script>
<template>
    <div class="row mt-3 mx-1" v-if="!addingNew">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <h5 class="float-start">{{t('inventory.adjustmentItemDetails.header.header')}}</h5>
                    <div class="float-start ms-4" v-if="getAdjustmentJobType(adjustment.jobType) == 'Undo Zero Out'">
                        <span class="alert-danger py-2 px-3"><strong>Undo Zero Out</strong></span>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-2 text-end pt-1">{{t('inventory.adjustmentItemDetails.header.jobNo')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="adjustment.jobNo" disabled /></div>
                        <div class="col-md-1 text-end pt-1">{{t('inventory.adjustmentItemDetails.header.whsCode')}}:</div>
                        <div class="col-md-1"><input class="form-control form-control-sm" :value="whsCode" disabled /></div>
                        <div class="col-md-1 text-end pt-1">{{t('inventory.adjustmentItemDetails.header.type')}}:</div>
                        <div class="col-md-1"><input class="form-control form-control-sm" :value="getAdjustmentJobType(adjustment.jobType)" disabled /></div>
                        <div class="col-md-1 text-end pt-1">{{t('inventory.adjustmentItemDetails.header.status')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="getAdjustmentStatus(adjustment.status)" disabled /></div>
                    </div>
                    <div class="row my-3">
                        <div class="col-md-2 text-end pt-1">{{t('inventory.adjustmentItemDetails.header.customerName')}}:</div>
                        <div class="col-md-2" v-if="jobNo"><input class="form-control form-control-sm" :value="adjustment.customerName" disabled /></div>
                        <div class="col-md-2" v-if="!jobNo">
                            <v-select :options="customers"                     
                                      @update:modelValue="changeCustomer"
                                      placeholder="Select a customer">
                                      <template v-slot:selected-option="option">{{ option.code + ' - ' + option.name }}</template>
                                      <template v-slot:option="option">{{option.code}} - {{option.name}}</template>
                            </v-select>
                        </div>
                        <div class="col-md-1 text-end pt-1">{{t('inventory.adjustmentItemDetails.header.referenceNo')}}:</div>
                        <div class="col-md-3"><input class="form-control form-control-sm" v-model="adjustment.referenceNo" :disabled="!canCancel()" /></div>
                        <div class="col-md-1 text-end pt-1">{{t('inventory.adjustmentItemDetails.header.createdDate')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="showDate(!jobNo ? (new Date()).toString() : adjustment.createdDate, true)" disabled /></div>
                    </div>
                    <div class="row">
                        <div class="col-md-2 text-end pt-1">{{t('inventory.adjustmentItemDetails.header.reason')}}:<br /><small>{{t('inventory.adjustmentItemDetails.header.maxChars')}}</small></div>
                        <div class="col-md-6"><textarea class="form-control form-control-sm" v-model="adjustment.reason" :disabled="!canCancel()"></textarea></div>
                        <div class="col-md-1 text-end pt-1">{{t('inventory.adjustmentItemDetails.header.adjustedDate')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="showDate(adjustment.revisedDate)" disabled /></div>
                    </div>
                </div>
                <div class="card-footer">
                    <div class="row">
                        <div class="col-md-12 text-end">
                            <button class="btn btn-primary me-2 btn-sm" @click="printPIDLabel" :disabled="!canPrintLabel()">
                                <i class="las la-print"></i>  {{ printing ? 'Printing in progress...' : 'Print Label' }}
                            </button>
                            <button class="btn btn-primary me-2 btn-sm" @click="complete" v-if="canComplete()"><i class="las la-check-circle"></i> Complete</button>
                            <button class="btn btn-danger me-2 btn-sm" @click="cancel" v-if="canCancel()"><i class="las la-check-circle"></i> Cancel</button>
                            <div class="d-inline-block" v-tooltip="canSave() ? null : 'Fill Ref No. first.'">
                                <button class="btn btn-primary me-2 btn-sm" @click="save" :disabled="!canSave()" v-if="isSaveVisible"><i class="las la-save"></i> Save</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row my-4 mx-1" v-if="!addingNew">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <h5>{{t('inventory.adjustmentItemDetails.list.listTitle')}}</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-12">
                            <table class="table table-striped table-hover filtered-sorted-table" v-if="!loadingList && adjustment.items.length > 0">
                                <table-sort-filter-header :columns=columns_ADJ_ITEMS 
                                                        :sorting=sorting_ADJ_ITEMS 
                                                        t_core="inventory.adjustmentItemDetails.list" 
                                                        @sort="sort" 
                                                        :sortable=true 
                                                        :filtreable=false />
                                <tbody>
                                    <tr v-for="adjItem, i in adjustment.items" :key="i" class="table-row">
                                        <td>{{i+1}}</td>
                                        <td>{{adjItem.lineItem}}</td>
                                        <td>{{adjItem.productCode}}</td>
                                        <td>{{adjItem.supplierId}}</td>
                                        <td>{{adjItem.pid}}</td>
                                        <td>{{adjItem.initialQty}}</td>
                                        <td>{{adjItem.newQty}}</td>
                                        <td>{{adjItem.remarks}}</td>
                                        <div class="table-buttons" v-if="canCancel()">
                                            <button class="btn btn-primary me-2 btn-sm" @click="modifyItem(adjItem)"><i class="las la-pen"></i> Modify</button>
                                            <button class="btn btn-danger me-2 btn-sm" @click="deleteItem(adjItem)"><i class="las la-trash"></i> Delete</button>
                                        </div>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="text-center py-4 main-action-icons-small" v-if="!loadingList && adjustment.items.length == 0">
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
                <div class="card-footer" v-if="canCancel()">
                    <button class="btn btn-primary me-2 btn-sm" @click="addNewItem" :disabled="!canAddNew()"><i class="las la-plus-circle"></i> Add new</button>
                </div>
            </div>
        </div>
    </div>

    <div class="row" v-if="addingNew">
        <div class="col-md-12 text-center">
            <div class="bars1-wrapper">
                <div id="bars1">
                    <span></span>
                    <span></span>
                    <span></span>
                    <span></span>
                    <span></span>
                </div>
                <h5>Creating a new adjustment item...</h5>
            </div>
        </div>
    </div>
    <add-adjustment-item :modalBase=addItemModal :job-no=jobNo :itemToModify=itemToModify @close="closeModal(addItemModal)" @closeadded="closeAfterAddItem()" />
    <confirm-popup :modalBase=confirmModal @close="closeModal(confirmModal)" />
    <processing-popup :modalBase=waitModal />
    <print-label v-if="modal != null && modal.type=='printLabel'" :all="modal.allPids" :selected="modal.selectedPids" :labelType="modal.labelType" 
                 @done="modal=null" @close="modal=null" />
</template>