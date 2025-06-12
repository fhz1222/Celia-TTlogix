<script lang="ts" setup>
    // import vue stuff
    import { ref } from 'vue';
    import { store } from '@/store';
    
    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { Error } from '@/store/commonModels/errors';
    import { Pallet } from '@/store/models/pallet';

    // import widgets and modals
    import ModalWidget from '@/widgets/Modal3.vue';

    // props & emits
    const props = defineProps<{modalBase: Modal, jobNo: string | null }>();
    const emit = defineEmits<{(e: 'close', added:boolean): void, (e: 'save', pid:string, decanted:Array<number>): void }>();

    // local vars
    const preloadingData = ref(false);
    const savingData = ref(false);
    const pid = ref<string | null>(null);
    const preData = ref(new Pallet());
    const decanted = ref(Array<number>());
    const decantQty = ref<number>(0);
    const splitQty = ref<number>(0);
    const canDecant = () => { return preData.value && decantQty.value !=0 && decantQty.value < preData.value.qty; };
    const canSplit = () => { return preData.value && splitQty.value !=0 && splitQty.value < preData.value.qty; };

    // functions
    const preloadData = async () => {
        preloadingData.value = true;
        await store.dispatch(ActionTypes.INV_DEC_ITEM_PREPARE_NEW, {pid: pid.value, jobNo: props.jobNo})
                   .then((adjDet:Pallet) => {preData.value = adjDet; splitQty.value = Math.round(preData.value.qty / 2)})
                   .catch((err:Error) => { 
                        errorModal.value.customProps = err;
                        errorModal.value.fnMod.on = true;
                        errorModal.value.fnMod.type = 'error';
                        errorModal.value.on = true });
        preloadingData.value = false;
    }
    const decant = (decant:boolean) => {
        decanted.value = new Array<number>();
        var newQty = decant ? decantQty.value : splitQty.value;
        var origQty = preData.value?.qty;
        if(newQty && origQty && (newQty < origQty)){
            if(decant){
                var q = Math.floor(origQty / newQty);
                var r = origQty % newQty;
                for(var i=0; i<q; i++){
                    decanted.value.push(newQty);
                }
                if(r > 0) { decanted.value.push(r) }
            } else {
                decanted.value.push(newQty);
                decanted.value.push(origQty-newQty);
            }
        }
    };
    const save = async () => {
        if(pid.value){
            emit('save', pid.value, decanted.value);
            resetModal();
        }
    };
    const resetModal = () => { pid.value = null; preData.value = new Pallet(); decanted.value = new Array<number>(); }

    // modals
    const closeModal = (modal: Modal) => { modal.on = false, modal.customProps = null };
    const errorModal = ref(new Modal(500,'Error',false));
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close', false)">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="row">
                <div class="col-md-12">
                    <div class="row">
                        <div class="col-md-1 text-end pt-1">PID</div>
                        <div class="col-md-6"><input class="form-control form-control-sm" type="text" v-model="pid" /></div>
                        <div class="col-md-2 text-start">
                            <button class="btn btn-sm btn-primary" 
                                    @click="preloadData" 
                                    :disabled="preloadingData || !pid">{{!preloadingData ? 'Load' : 'Loading'}}
                            </button>
                        </div>
                        <div class="col-md-1 text-end pt-1">Whs</div>
                        <div class="col-md-2"><input class="form-control form-control-sm" :value="preData?.whsCode" type="text" disabled /></div>
                    </div>
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-12">
                    <div><strong>Inventory Information</strong></div>
                    <div class="card">
                        
                        <div class="card-body ">
                            <div class="row">
                                <div class="col-md-2 text-end pt-1">Product Code</div>
                                <div class="col-md-5"><input class="form-control form-control-sm" :value="preData?.product?.code" type="text" disabled /></div>
                                <div class="col-md-5">
                                    <div class="row">
                                        <div class="col-md-6 text-end pt-1">Supplier Id</div>
                                        <div class="col-md-6"><input class="form-control form-control-sm" :value="preData?.product?.customerSupplier?.supplierId" type="text" disabled /></div>
                                    </div>
                                </div>
                            </div>
                            <div class="row mt-2">
                                <div class="col-md-2 text-end pt-1">Description</div>
                                <div class="col-md-5">
                                    <textarea class="form-control form-control-sm two-liner" :value="preData?.product?.description" type="text" disabled></textarea>
                                </div>
                                <div class="col-md-5">
                                    <div class="row">
                                        <div class="col-md-6 text-end pt-1">Inbound Date</div>
                                        <div class="col-md-6"><input class="form-control form-control-sm" :value="preData?.inboundDate" type="text" disabled /></div>
                                    </div>
                                    <div class="row mt-2">
                                        <div class="col-md-6 text-end pt-1">Location</div>
                                        <div class="col-md-6"><input class="form-control form-control-sm" :value="preData?.location" type="text" disabled /></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row mt-2">
                <div class="col-md-12">
                    <div><strong>Decant / Split</strong></div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <div class="card">
                        <div class="card-body">
                            <div class="row mt-2">
                                <div class="col-md-4 text-end pt-1">Original qty</div>
                                <div class="col-md-5"><input class="form-control form-control-sm text-end" :value="preData?.qty" type="text" disabled /></div>
                            </div>
                            <div class="row mt-4">
                                <div class="col-md-4 text-end pt-1">Qty per pkg</div>
                                <div class="col-md-5"><input class="form-control form-control-sm text-end" v-model="decantQty" type="text" /></div>
                                <div class="col-md-3">
                                    <button class="btn btn-sm btn-primary" :disabled="!canDecant()" @click="decant(true)">Decant</button>
                                </div>
                            </div>
                            <div class="row mt-4">
                                <div class="col-md-4 text-end pt-1">Qty per pkg</div>
                                <div class="col-md-5"><input class="form-control form-control-sm text-end" v-model="splitQty" type="text" /></div>
                                <div class="col-md-3">
                                    <button class="btn btn-sm btn-primary" :disabled="!canSplit()" @click="decant(false)">Split</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-6 scrollable-45">
                    <table class="table table-striped table-hover">
                        <thead>
                            <tr>
                                <th>Pkg No</th>
                                <th>New Qty</th>
                            </tr>
                        </thead>
                        <tbody v-if="decanted.length > 0">
                            <tr v-for="item, i in decanted" :key="i">
                                <td>{{i+1}}</td>
                                <td>{{item}}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-primary" :disabled="!preData || decanted.length == 0 || savingData" @click="save">
                        <i class="las la-disk"></i> {{!savingData    ? 'Save' : 'Saving'}}
                    </button>
                </div>
            </div>
        </template>
    </modal-widget>
    <error-popup :modalBase=errorModal @close="closeModal(errorModal)" />
</template>