<script lang="ts" setup>
    // import vue stuff
    import { ref, watch } from 'vue';
    import { store } from '@/store';

    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { AdjustmentItemAddMod } from '@/store/models/adjustmentItemAddMod';
    import { Error } from '@/store/commonModels/errors';
    import { AdjustmentItem } from '@/store/models/adjustmentItem';

    // import widgets and modals
    import ModalWidget from '@/widgets/Modal3.vue';

    // props & emits
    const props = defineProps<{modalBase: Modal, jobNo: string | null, itemToModify: AdjustmentItem | null }>();
    const emit = defineEmits<{(e: 'close'): void, (e: 'closeadded'): void }>();

    // local vars
    const preloadingData = ref(false);
    const savingData = ref(false);
    const pid = ref<string | null>(null);
    const preData = ref(new AdjustmentItemAddMod());
    const diff = ref(0);
    const diffPerPkg = ref(0);

    // functions
    const preloadData = async () => {
        preloadingData.value = true;
        await store.dispatch(ActionTypes.INV_ADJ_ITEM_PREPARE_NEW, {pid: pid.value, jobNo: props.jobNo})
                   .then((adjDet:AdjustmentItemAddMod) => {
                       preData.value = adjDet;
                       preData.value.newQty = adjDet.pallet.qty;
                       preData.value.newQtyPerPkg = adjDet.pallet.qtyPerPkg;
                       preloadingData.value = false;
                   })
                   .catch((err:Error) => { 
                        errorModal.value.customProps = err;
                        errorModal.value.fnMod.on = true;
                        errorModal.value.fnMod.type = 'error';
                        errorModal.value.on = true 
                        preloadingData.value = false;
                   });
    }
    const getItemForModification = async () => {
        preloadingData.value = true;
        await store.dispatch(ActionTypes.INV_ADJ_ITEM_GET, {jobNo: props.jobNo, lineItem: props.itemToModify?.lineItem })
                   .then((adjMod:AdjustmentItemAddMod) => {
                        preData.value = adjMod;
                        preloadingData.value = false;
                   })
                   .catch((err:Error) => { 
                        errorModal.value.customProps = err;
                        errorModal.value.fnMod.on = true;
                        errorModal.value.fnMod.type = 'error';
                        errorModal.value.on = true 
                        preloadingData.value = false;
                   });
    }
    watch(() => props.itemToModify, (nVal, oVal) => {
        pid.value = nVal ? nVal.pid : null;
        preData.value = new AdjustmentItemAddMod();
        if(nVal) { 
            getItemForModification().then(() => { 
                preData.value.lineItem = nVal.lineItem;
                preData.value.remarks = nVal.remarks;
            });
        }
    });
    watch(() => props.modalBase.on, (nVal, oVal) => {
        if(nVal && !props.itemToModify) { 
            preData.value = new AdjustmentItemAddMod();
            pid.value = null;
            diff.value = 0;
            diffPerPkg.value = 0;
        }
    });
    const save = async () => {
        savingData.value = true;
        var savedItem = new AdjustmentItem(preData.value);
        await store.dispatch(ActionTypes.INV_ADJ_ITEM_SAVE, {adjItem: savedItem})
                   .then(() => { 
                       savingData.value = false;
                       emit('closeadded')})
                   .catch((err:Error) => { 
                        savingData.value = false;
                        errorModal.value.customProps = err;
                        errorModal.value.fnMod.on = true;
                        errorModal.value.fnMod.type = 'error';
                        errorModal.value.on = true });
        
    }
    const calculateQty = () => { 
        if(!isNaN(preData.value.newQty)){
            diff.value = preData.value.newQty - preData.value.pallet.qty;
            preData.value.newQtyPerPkg = preData.value.newQty;
            calculateQtyPerPackage();
        } else {
            preData.value.newQty = preData.value.pallet.qty;
            diff.value = 0;
            diffPerPkg.value = 0;
        }
    };
    const calculateQtyPerPackage = () => { 
        diffPerPkg.value = preData.value.newQtyPerPkg - preData.value.pallet.qtyPerPkg; 
    };

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const errorModal = ref(new Modal(500,'Error',false));
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close')">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="row">
                <div class="col-md-12">
                    <div class="row">
                        <div class="col-md-2 text-end pt-1">PID</div>
                        <div class="col-md-8"><input class="form-control form-control-sm" type="text" v-model="pid" /></div>
                        <div class="col-md-2 text-start">
                            <button class="btn btn-sm btn-primary" 
                                    @click="preloadData" 
                                    :disabled="preloadingData || !pid">{{!preloadingData ? 'Load' : 'Loading'}}
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-md-12">
                    <div><strong>Inventory Information</strong></div>
                    <div class="card">
                        
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-3 text-end pt-1">Product Code</div>
                                <div class="col-md-9"><input class="form-control form-control-sm" :value="preData?.pallet?.product?.code" type="text" disabled /></div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end pt-1">Description</div>
                                <div class="col-md-9">
                                    <textarea class="form-control form-control-sm" :value="preData?.pallet?.product?.description" type="text" disabled></textarea>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end pt-1">Supplier Id</div>
                                <div class="col-md-3"><input class="form-control form-control-sm" :value="preData?.pallet?.product?.customerSupplier?.supplierId" type="text" disabled /></div>
                                <div class="col-md-3 text-end pt-1">Inbound Date</div>
                                <div class="col-md-3"><input class="form-control form-control-sm" :value="preData?.pallet?.inboundDate" type="text" disabled /></div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end pt-1">Warehouse</div>
                                <div class="col-md-3"><input class="form-control form-control-sm" :value="preData?.pallet?.whsCode" type="text" disabled /></div>
                                <div class="col-md-3 text-end pt-1">Location</div>
                                <div class="col-md-3"><input class="form-control form-control-sm" :value="preData?.pallet?.location" type="text" disabled /></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-md-12">
                    <div><strong>Adjustment</strong></div>
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="offset-md-10 col-md-2">Diff</div>
                            </div>
                            <div class="row">
                                <div class="col-md-3 text-end pt-1">Qty</div>
                                <div class="col-md-3"><input class="form-control form-control-sm" :value="preData?.pallet?.qty" type="text" disabled /></div>
                                <div class="col-md-1 text-center"><i class="las la-minus"></i></div>
                                <div class="col-md-3"><input class="form-control form-control-sm" type="text" v-model="preData.newQty" @keyup="calculateQty" /></div>
                                <div class="col-md-2"><input class="form-control form-control-sm" type="text" v-model="diff" disabled /></div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-3 text-end pt-1">Qty / Package</div>
                                <div class="col-md-3"><input class="form-control form-control-sm" :value="preData?.pallet?.qtyPerPkg" type="text" disabled /></div>
                                <div class="col-md-1 text-center"><i class="las la-minus"></i></div>
                                <div class="col-md-3"><input class="form-control form-control-sm" type="text" v-model="preData.newQtyPerPkg" @keyup="calculateQtyPerPackage" disabled /></div>
                                <div class="col-md-2"><input class="form-control form-control-sm" type="text" v-model="diffPerPkg" disabled /></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-md-12">
                    <div><strong>Remark</strong> <small>(max 100 characters)</small></div>
                    <div class="mt-2"><textarea class="form-control form-control-sm" v-model="preData.remarks"></textarea></div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-primary" :disabled="!preData || savingData || preloadingData || !pid" @click="save">
                        <i class="las la-disk"></i> {{!savingData    ? 'Save' : 'Saving'}}
                    </button>
                </div>
            </div>
        </template>
    </modal-widget>
    <error-popup :modalBase=errorModal @close="closeModal(errorModal)" />
</template>