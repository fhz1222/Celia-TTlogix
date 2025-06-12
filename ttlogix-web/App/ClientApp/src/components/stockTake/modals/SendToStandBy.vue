<script lang="ts" setup>
    // import vue stuff
    import { computed, ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    import { useStore } from '@/store';
    
    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { CustomerInventory } from '@/store/models/customerInventory';
    import { ActionTypes } from '@/store/action-types';
    import { StandbyLocation } from '@/store/models/standbyLocation';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';
    import SelectUom from '@/widgets/SelectUOMWithDecimal.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'proceed', locationCode: string): void }>();

    // local vars
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });
    const standbyLocations = ref();
    const jobNo = ref();
    const currentLocationCode = ref();
    const whsCode = ref(store.state.whsCode);
    const loading = ref(false);
    //const standbyLocations = ref(new Array<StandbyLocation>());

    // functions
    const canSave = () => {
        return currentLocationCode.value != null;
    }

    const proceed = async (addNext: boolean) => {
        emit('proceed', currentLocationCode.value);
        resetModal();
    }
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    const resetModal = () => {
        standbyLocations.value =null;
        jobNo.value = null;
        currentLocationCode.value = null;
    }

    const loadLocations = async () => {
        loading.value = true;
        await store.dispatch(ActionTypes.STL_LOAD_STANDBY_LOCATIONS, { jobNo: jobNo.value })
            .then((loc) => { standbyLocations.value = loc; loading.value = false; })
            .catch(() => loading.value = false);
    }

    watch(props.modalBase, (nV, oV) => {
        if (!nV.on) resetModal();
        else { jobNo.value = nV.customProps; loadLocations(); }
    })
    watch(() => props.modalBase.customProps, (nV, oV) => {
        if (nV != null) jobNo.value = nV
    })
</script>

<template>
    <modal-widget :modalBase=modalBase @close="emit('close', false)">
        <template name="title" v-slot:title>
            <h5>{{modalBase.title}}</h5>
        </template>
        <template name="body" v-slot:body>
            <div class="card">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-2 text-end">Job No</div>
                        <div class="col-md-6 no-left-padding"><strong>{{ jobNo }}</strong></div>
                        <div class="col-md-2 no-right-padding">Warehouse</div>
                        <div class="col-md-2"><strong>IT</strong></div>
                    </div>
                </div>
            </div>
            <div class="card mt-3">
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-7 text-end pt-1">Select Stand-by location for MISSING PID</div>
                        <div class="col-md-5 no-left-padding" v-if="!loading">
                            <select class="form-select form-select-sm" v-model="currentLocationCode">
                                <option v-for="(sl,i) in standbyLocations" :key="i" :value="sl.code">{{sl.name}}</option>
                            </select>
                        </div>
                        <div class="col-md-5 no-left-padding" v-if="loading">Loading locations...</div>
                    </div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-secondary" @click="cancel"><i class="las la-ban"></i> {{ t('operation.general.cancel') }}</button>
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(false)" :disabled="!canSave()"><i class="las la-save"></i> {{ t('operation.general.ok') }}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>