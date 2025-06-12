<script lang="ts" setup>
    // import vue stuff
    import { computed, ref, watch } from 'vue';
    import { useI18n } from 'vue-i18n';
    import { useStore } from '@/store';
    
    // import types
    import { Modal } from '@/store/commonModels/modal';
    import { ActionTypes } from '@/store/action-types';
    import { Area } from '@/store/models/settings/area';
    import { Warehouse } from '@/store/models/settings/warehouse';
    import { SettingsComboBox } from '@/store/models/settings/settingsComboBox';

    // modals and widgets
    import ModalWidget from '@/widgets/Modal3.vue';

    // props & emits
    const props = defineProps<{ modalBase: Modal }>();
    const emit = defineEmits<{ (e: 'close'): void, (e: 'add', updatedArea: Area, addNext: boolean): void, (e: 'update', updatedArea: Area): void }>();

    // local vars
    const store = useStore();
    const { t } = useI18n({ useScope: 'global' });
    const loading = ref(false);
    const currentArea = ref(new Area());
    const activeAreaTypes = ref(new Array<SettingsComboBox>());
    const activeWarehouses = ref(new Array<SettingsComboBox>());
    const newlyAdded = ref(true);

    // functions
    const initializeScreen = async () => {
        loading.value = true;
        if (currentArea.value.code != null) {
            await store.dispatch(ActionTypes.SETT_LOAD_AREA, { code: currentArea.value.code ?? '', whsCode: currentArea.value.whsCode ?? '' })
                       .then((a) => currentArea.value = a).catch(() => loading.value = false);
        }
        await store.dispatch(ActionTypes.SETT_CB_LOAD_ACTIVE_AREA_TYPES).then((aat) => activeAreaTypes.value = aat).catch(() => loading.value = false);
        await store.dispatch(ActionTypes.SETT_CB_LOAD_ACTIVE_WAREHOUSES).then((aw) => activeWarehouses.value = aw).catch(() => loading.value = false);
        loading.value = false;
    }
    const canSave = () => {
        return currentArea.value.code && currentArea.value.name && currentArea.value.type && currentArea.value.whsCode;
    }
    const proceed = async (addNext: boolean) => {
        if (newlyAdded.value) emit('add', currentArea.value, addNext)
        else emit('update', currentArea.value)
        resetModal();
    }
    const cancel = async () => {
        emit('close');
        resetModal();
    }
    const resetModal = () => {
        currentArea.value = new Area();
    }

    watch(props.modalBase, async (nV, oV) => {
        if (nV.on == false) { newlyAdded.value = false; resetModal(); }
        if (nV.on == true) {
            if (nV.customProps.code != null) {
                currentArea.value = nV.customProps;
                newlyAdded.value = false;
            } else {
                newlyAdded.value = true;
            }
            await initializeScreen();
        }
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
                        <div class="col-md-3 text-end no-right-padding">{{ t('settings.area.area') }}:</div>
                        <div class="col-md-4" v-if="newlyAdded">
                            <input type="text" class="form-control form-control-sm" v-model="currentArea.code" />
                        </div>
                        <div class="col-md-4" v-if="!newlyAdded"><strong>{{currentArea.code}}</strong></div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-3 text-end no-right-padding">{{ t('settings.area.areaName') }}:</div>
                        <div class="col-md-9">
                            <input type="text" class="form-control form-control-sm" v-model="currentArea.name" />
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-3 text-end no-right-padding">{{ t('settings.area.areaType') }}:</div>
                        <div class="col-md-9 multiselect-as-select">
                            <select class="form-select form-select-sm" v-model="currentArea.type">
                                <option v-for="at,i in activeAreaTypes" :key="i" :value="at.code">{{at.code + ' - ' + at.label}}</option>
                            </select>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-3 text-end no-right-padding">{{ t('settings.area.warehouse') }}:</div>
                        <div class="col-md-9 multiselect-as-select">
                            <select class="form-select form-select-sm" v-if="newlyAdded" v-model="currentArea.whsCode" >
                                <option v-for="whs,i in activeWarehouses" :key="i" :value="whs.code">{{whs.label}}</option>
                            </select>
                            <span v-if="!newlyAdded"><strong>{{currentArea.whsCode}}</strong></span>
                        </div>
                    </div>
                    <div class="row mt-3">
                        <div class="col-md-3 text-end pt-1 no-right-padding">{{ t('settings.area.capacity') }}:</div>
                        <div class="col-md-3">
                            <input type="text" class="form-control form-control-sm" v-model="currentArea.capacity" />
                        </div>
                        <div class="col-md-2 no-left-padding pt-1">m<sup>2</sup></div>
                    </div>
                </div>
            </div>
        </template>
        <template name="footer" v-slot:footer>
            <div class="row">
                <div class="col-md-12 text-end">
                    <button class="btn btn-sm btn-secondary" @click="cancel"><i class="las la-ban"></i> {{ t('operation.general.cancel') }}</button>
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(false)" :disabled="!canSave()"><i class="las la-save"></i> {{ t('operation.general.save') }}</button>
                    <button class="btn btn-sm btn-secondary ms-2" @click="proceed(true)" v-if="newlyAdded" :disabled="!canSave()"><i class="las la-save"></i> {{t('operation.general.saveAndAddNext')}}</button>
                </div>
            </div>
        </template>
    </modal-widget>
</template>