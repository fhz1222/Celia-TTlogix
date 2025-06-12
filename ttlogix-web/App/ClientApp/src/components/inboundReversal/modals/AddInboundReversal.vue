<script lang="ts" setup>
    // import vue stuff
    import { ref } from 'vue';
    import { store } from '@/store';
    import { useI18n } from 'vue-i18n';
    
    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { Error } from '@/store/commonModels/errors';
    import { ReversibleInbound } from '@/store/models/reversibleInbound'
    import { ReversibleInboundFilters } from '@/store/models/reversibleInboundFilters'
    import AwaitingButton from '@/widgets/AwaitingButton.vue'
    import ErrorPopup from '@/widgets/ErrorPopup.vue'

    // import widgets and modals
    import ModalWidget from '@/widgets/Modal3.vue';

    // props & emits
    const props = defineProps<{modalBase: Modal }>();
    const emit = defineEmits<{(e: 'close'): void, (e: 'addNew', inJobNo:string): void }>();

    // local vars
    //const inJobNo = ref('');
    const reversibleInbound = ref(new ReversibleInbound());
    const RIFilters: ReversibleInboundFilters = new ReversibleInboundFilters();
    RIFilters.whsCode = store.state.whsCode;
    const { t } = useI18n({ useScope: 'global' });

    const loading = ref(false);
    const saving = ref(false);

    // functions
    const loadInJob = async () => {
        loading.value = true;
        RIFilters.inJobNo = reversibleInbound.value.jobNo.trim();
        await store.dispatch(ActionTypes.IR_LOAD_REVERSIBLE_INBOUND, { filters: RIFilters })
            .then((revInb: ReversibleInbound) => {
                if (revInb) {
                    reversibleInbound.value = revInb
                }
                else {
                    errorModal.value.customProps = t('error.ReversibleInboundLoadError');
                    errorModal.value.fnMod.on = true;
                    errorModal.value.fnMod.type = 'error';
                    errorModal.value.on = true
                }
            })
        loading.value = false;
    }
    
    const proceed = async () => {
        emit('addNew', reversibleInbound.value.jobNo);
        resetModal();
    };
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    const resetModal = () => { reversibleInbound.value = new ReversibleInbound() }

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
                <div class="col-md-3 text-end pt-1">Inbound No:</div>
                <div class="col-md-6"><input class="form-control form-control-sm" type="text" v-model="reversibleInbound.jobNo" /></div>
                <div class="col-md-3">
                    <AwaitingButton @proceed="(callback) => { loadInJob().finally(callback) }"
                                    :btnText="'operation.general.load'"
                                    :btnIcon="'las la-download'"
                                    :btnClass="'btn-primary'"
                                    :enabled="!saving">
                    </AwaitingButton>
                </div>
            </div>
            <div class="row mt-3">
                <div class="col-md-3 text-end pt-1">Ref No:</div>
                <div class="col-md-6"><input class="form-control form-control-sm" type="text" v-model="reversibleInbound.refNo" disabled /></div>
            </div>
            <div class="row mt-3">
                <div class="col-md-3 text-end pt-1">Supplier ID:</div>
                <div class="col-md-6"><input class="form-control form-control-sm" type="text" v-model="reversibleInbound.supplierId" disabled /></div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-secondary me-2" @click="cancel">{{ t('operation.general.cancel') }}</button>
                    <AwaitingButton @proceed="(callback) => { proceed().finally(callback) }"
                                    :btnText="'operation.general.ok'"
                                    :btnIcon="'las la-disk'"
                                    :btnClass="'btn-primary'"
                                    :enabled="!loading"> 
                    </AwaitingButton>
                </div>
            </div>
        </template>
    </modal-widget>
    <error-popup :modalBase=errorModal @close="closeModal(errorModal)" />
</template>