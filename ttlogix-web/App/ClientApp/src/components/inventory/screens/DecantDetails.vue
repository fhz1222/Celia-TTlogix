<script lang="ts" setup>
    // import vue stuff
    import { useStore } from '@/store';
    import { useRoute } from 'vue-router';
    import { ref } from 'vue';
    import { ActionTypes } from '@/store/action-types';
    import { useI18n } from 'vue-i18n'
    
    // import types
    import { TableColumn } from '@/store/commonModels/config';
    import { Sorting } from '@/store/commonModels/Sorting';
    import { Modal } from '@/store/commonModels/modal';
    import { Decant } from '@/store/models/decant';
    import { Error } from '@/store/commonModels/errors';

    // import widgets and modals
    import TableSortFilterHeader from '@/widgets/TableSortFilterHeader.vue';
    import AddDecantItem from '../modals/AddDecantItem.vue';
    import ConfirmPopup from '@/widgets/ConfirmPopup.vue'
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
    const sorting_DEC_ITEMS = ref(new Sorting());
    const whsCode = store.state.whsCode;
    const decant = ref<Decant>(new Decant());
    const getDecantStatus = store.getters.getDecantStatus;

    // Print - old modal
    const modal: any = ref(null);

    // functions
    const setDecant = (jn:string | null = null) => {
        const decantResult = store.state.decantList?.items.find(el => { return el.jobNo == (jn ? jn : jobNo.value) });
        decant.value = decantResult ? decantResult : new Decant();
    }
    const reloadItem = async () => {
        loadingList.value = true;
        await store.dispatch(ActionTypes.INV_DEC_DETAILS, {jobNo: jobNo.value});
        setDecant();
        loadingList.value = false;
    }

    const complete = async () => { 
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_DEC_COMPLETE, { jobNo: decant.value.jobNo }).catch(() => { closeModal(waitModal.value) });
        await reloadItem();
        closeModal(waitModal.value);
    };
    const canComplete = () => { return decant.value.status == 1; };
    const cancel = async () => { 
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_DEC_CANCEL, { jobNo: decant.value.jobNo }).catch(() => { closeModal(waitModal.value) });
        await reloadItem();
        closeModal(waitModal.value);
    };
    const canCancel = () => { return decant.value.status < 2; };
    const save = async () => {
        waitModal.value.on = true;
        await store.dispatch(ActionTypes.INV_DEC_UPDATE, { jobNo: decant.value.jobNo, referenceNo: decant.value.referenceNo, remark: decant.value.remark })
            .catch(() => { closeModal(waitModal.value) });
        setDecant();
        closeModal(waitModal.value);
    };
    //const canSave = () => { return typeof decant.value.referenceNo != 'undefined' && decant.value.referenceNo; };
    const printDetails = async () => { 
        await store.dispatch(ActionTypes.INV_DEC_REPORT, {decantDetails: decant.value.detailsList})
                                .then((report:Blob) => {
                                    const url = URL.createObjectURL(new Blob([report], {
                                        type: 'application/vnd.ms-excel'
                                    }));
                                    const link = document.createElement('a');
                                    link.href = url;
                                    link.setAttribute('download', (jobNo.value + '.xls'));
                                    document.body.appendChild(link);
                                    link.click();
                                });
        
    };
    const canPrintDetails = () => { return decant.value && decant.value.items && decant.value.items.length > 0; };

    const printPIDLabel = async () => {
        if (canPrintLabel()) {
            var pids = decant.value.detailsList.map(o => o.pid);
            modal.value = { type: 'printLabel', allPids: pids, selectedPids: pids, labelType: 'QRPRINT_STORAGELABEL' };
            modal.value.type = 'printLabel';
        }
    };
    /*const areSelected = () => {
        var result = decant.value.detailsList.filter(obj => {
            return obj.selected === true;
        });
        return result;
    }*/
    const canPrintLabel = () => {
        return decant.value.detailsList.length > 0 && decant.value.status == 8 /*&& areSelected().length > 0*/;
    };
    

    const addNewItem = () => { addDetailitem.value.on = true };
    const addItemsExecute = async (pid:string, newQuantities:Array<number>) => { 
        closeModal(addDetailitem.value);
        if(jobNo.value){
            waitModal.value.on = true;
            await store.dispatch(ActionTypes.INV_DEC_ADD_ITEMS, { jobNo: jobNo.value, pid: pid, newQuantities: newQuantities }).catch(() => { closeModal(waitModal.value) });
            setDecant();
            closeModal(waitModal.value);
        }
    }

    const deleteItem = async (pid:string) => { 
        confirmModal.value.fnMod.message = 'Do you really want to delete items associated with ' + pid + ' ?';
        confirmModal.value.on = true;
        confirmModal.value.fnMod.callback = async () => { await deleteItemExecute(pid) };
    };
    const deleteItemExecute = async (pid:string): Promise<void> => { 
        closeModal(confirmModal.value);
        waitModal.value.on = true;
        if(jobNo.value){
            await store.dispatch(ActionTypes.INV_DEC_DEL_ITEMS, { jobNo: jobNo.value, pid: pid }).catch(() => { closeModal(waitModal.value) });
            setDecant();
            closeModal(waitModal.value);
        }
    };

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const addDetailitem = ref(new Modal(750, 'Add decant item', true));
    const confirmModal = ref(new Modal(600, 'Please confirm', true));
    const waitModal = ref(new Modal(500, 'Processing', false));
    const errorModal = ref(new Modal(500, 'Error', false));

    // Sorting
    const sort = (by: string) => { 
        sorting_DEC_ITEMS.value.by = by;
        sorting_DEC_ITEMS.value.descending = !sorting_DEC_ITEMS.value.descending;
        decant.value.detailsList = sortingFn.sortLocally(sorting_DEC_ITEMS.value, decant.value.detailsList); };
    // Date
    const showDate = (date:string) => { return miscFn.showDate(date, true); }

    // table related stuff
    const columns_DEC_ITEMS:Array<TableColumn> = [
        { alias:'no',         columnType:null, dropdown:null, sortable: false,f1:null, f2:null, resetTo: null },
        { alias:'originalPid',columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        { alias:'productCode',columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        { alias:'supplierID', columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        { alias:'originalQty',columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        { alias:'sequenceNo', columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        { alias:'pid',        columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null },
        { alias:'newQty',     columnType:null, dropdown:null, sortable: true, f1:null, f2:null, resetTo: null }
    ];

    // init the screen
    reloadItem();
</script>
<template>
    <div class="row mt-3 mx-1" v-if="!addingNew">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <h5 class="float-start">{{t('inventory.decant.detail.header.header')}}</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-2 text-end pt-1">{{t('inventory.decant.detail.header.jobNo')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="decant.jobNo" disabled /></div>
                        <div class="col-md-1 text-end pt-1">{{t('inventory.decant.detail.header.referenceNo')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" v-model="decant.referenceNo" :disabled="!canCancel()" /></div>
                        <div class="col-md-1 text-end pt-1">{{t('inventory.decant.detail.header.whsCode')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="whsCode" disabled /></div>
                    </div>
                    <div class="row my-3">
                        <div class="col-md-2 text-end pt-1">{{t('inventory.decant.detail.header.customerName')}}:</div>
                        <div class="col-md-2" v-if="jobNo"><input class="form-control form-control-sm" :value="decant.customerName" disabled /></div>
                        <div class="col-md-1 text-end pt-1">{{t('inventory.decant.detail.header.status')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="getDecantStatus(decant.status)" disabled /></div>
                        <div class="col-md-1 text-end pt-1">{{t('inventory.decant.detail.header.createdDate')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="showDate(!jobNo ? (new Date()).toString() : decant.createdDate)" disabled /></div>
                    </div>
                    <div class="row">
                        <div class="col-md-2 text-end pt-1">{{t('inventory.decant.detail.header.remark')}}:<br /><small>{{t('inventory.decant.detail.header.maxChars')}}</small></div>
                        <div class="col-md-5"><textarea class="form-control form-control-sm" v-model="decant.remark" :disabled="!canCancel()"></textarea></div>
                        <div class="col-md-1 text-end pt-1">{{t('inventory.decant.detail.header.createdDate')}}:</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="showDate(!jobNo ? (new Date()).toString() : decant.createdDate)" disabled /></div>
                    </div>
                </div>
                <div class="card-footer text-end" v-if="canCancel()">
                    <button class="btn btn-primary me-2 btn-sm" @click="complete" v-if="canComplete()"><i class="las la-check-circle"></i> Complete</button>
                    <button class="btn btn-danger me-2 btn-sm" @click="cancel" v-if="canCancel()"><i class="las la-check-circle"></i> Cancel</button>
                    <button class="btn btn-primary btn-sm" @click="save"><i class="las la-save"></i> Save</button>
                </div>
            </div>
        </div>
    </div>

    <div class="row my-4 mx-1" v-if="!addingNew">
        <div class="col-md-12">
            <div class="card">
                <div class="card-header">
                    <h5>{{t('inventory.decant.detail.details.detailsListTitle')}}</h5>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-12">
                            <table class="table table-striped table-hover filtered-sorted-table" v-if="!loadingList && (decant.detailsList && decant.detailsList.length > 0)">
                                <table-sort-filter-header :columns=columns_DEC_ITEMS
                                                          :sorting=sorting_DEC_ITEMS
                                                          t_core="inventory.decant.detail.details"
                                                          @sort="sort"
                                                          :sortable=true
                                                          :filtreable=false />
                                <tbody>
                                    <tr v-for="dec, i in decant.detailsList" :key="i" class="table-row">
                                        <td>{{i+1}}</td>
                                        <td>{{dec.originalPID}}</td>
                                        <td>{{dec.productCode}}</td>
                                        <td>{{dec.supplierID}}</td>
                                        <td>{{dec.originalQty}}</td>
                                        <td>{{dec.sequenceNo}}</td>
                                        <td>{{dec.pid}}</td>
                                        <td>{{dec.newQty}}</td>
                                        <div class="table-buttons" v-if="canCancel()">
                                            <button class="btn btn-danger me-2 btn-sm" @click="deleteItem(dec.originalPID)"><i class="las la-trash"></i> Delete</button>
                                        </div>
                                    </tr>
                                </tbody>
                            </table>
                            <div class="text-center py-4 main-action-icons-small" v-if="!loadingList && decant.items.length == 0">
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
                <div class="card-footer">
                    <div class="row">
                        <div class="col-md-6">
                            <button class="btn btn-primary me-2 btn-sm" @click="addNewItem" v-if="canCancel()"><i class="las la-plus-circle"></i> Add new</button>
                            <button class="btn btn-primary me-2 btn-sm" @click="printDetails" :disabled="!canPrintDetails()">
                                <i class="las la-print"></i>  {{ printing ? 'Printing in progress...' : 'Print Details' }}
                            </button>
                        </div>
                        <div class="col-md-6 text-end">
                            <button class="btn btn-primary me-2 btn-sm" @click="printPIDLabel" :disabled="!canPrintLabel()"><i class="las la-print"></i> Print Labels</button>
                        </div>
                    </div>
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
    <add-decant-item :modalBase=addDetailitem :job-no=jobNo @close="closeModal(addDetailitem)" @save="addItemsExecute" />
    <confirm-popup :modalBase=confirmModal @close="closeModal(confirmModal)" />
    <processing-popup :modalBase=waitModal />
    <error-popup :modalBase=errorModal @close="closeModal(errorModal)" />
    <print-label v-if="modal != null && modal.type=='printLabel'" :all="modal.allPids" :selected="modal.selectedPids" :labelType="modal.labelType" :selectedOnly=true
                 @done="modal=null" @close="modal=null" />
</template>